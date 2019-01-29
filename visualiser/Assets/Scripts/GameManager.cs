using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Linq;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {

	public BoardManager boardManager;
	public GameObject woodenBoard;
	public Client client;
	public Text nbStep;
	public Text complexityTime;
	public Text complexitySize;
	public Text elapsedTime;
	public UIManager uiManager;
	public Camera cam;
	public Tray tray;
	public Animator sideTrayAnimator;

	public int algorithmType = 0;
	public int heuristicFunction = 0;
	public int solutionType = 0;
	public int isGreedy = 0;
	public int timeOut = 5000;
	
	public Material swapCellOriginalMaterial;
	public Material swapCellSelectedMaterial;

	public Parser parser;
	private List<string> solution;

	private LinkedList<BoardManager.MoveDirection> solutionNextMoves;
	private LinkedList<BoardManager.MoveDirection> solutionPrevMoves;

	private Dictionary<string, BoardManager.MoveDirection> stringToMoveDirection;
	private Dictionary<BoardManager.MoveDirection, BoardManager.MoveDirection> oppositeMoveDirection;

	private bool needToUpdateNbStep = false;
	private List<String> resolutionInformation;

	private Cell swapFirstCell;
	private Cell swapSecondCell;
	//private MeshRenderer swapFirstCellMeshRenderer;
	//private MeshRenderer swapSecondCellMeshRenderer;

	public Quaternion originalWoodenBoardRotation, inverseWoodenBoardRotation, desiredRotation;

	private float elaspedTime;
	private float rotationSpeed = 0.5f;
	private bool inSettings = false;

	void Start() {
		elaspedTime = 0f;
		parser = new Parser();
		solution = null;

		swapFirstCell = null;
		swapSecondCell = null;

		solutionNextMoves = new LinkedList<BoardManager.MoveDirection>();
		solutionPrevMoves = new LinkedList<BoardManager.MoveDirection>();

		stringToMoveDirection = new Dictionary<string, BoardManager.MoveDirection>();
		stringToMoveDirection.Add("Right", BoardManager.MoveDirection.Right);
		stringToMoveDirection.Add("Left", BoardManager.MoveDirection.Left);
		stringToMoveDirection.Add("Up", BoardManager.MoveDirection.Up);
		stringToMoveDirection.Add("Down", BoardManager.MoveDirection.Down);

		oppositeMoveDirection = new Dictionary<BoardManager.MoveDirection, BoardManager.MoveDirection>();
		oppositeMoveDirection.Add(BoardManager.MoveDirection.Right, BoardManager.MoveDirection.Left);
		oppositeMoveDirection.Add(BoardManager.MoveDirection.Left, BoardManager.MoveDirection.Right);
		oppositeMoveDirection.Add(BoardManager.MoveDirection.Up, BoardManager.MoveDirection.Down);
		oppositeMoveDirection.Add(BoardManager.MoveDirection.Down, BoardManager.MoveDirection.Up);

		originalWoodenBoardRotation = woodenBoard.transform.rotation;
		inverseWoodenBoardRotation = originalWoodenBoardRotation * Quaternion.Euler(Vector3.up * 180f);
		//Debug.Log(originalWoodenBoardRotation);
		desiredRotation = originalWoodenBoardRotation;

		//woodenBoard.transform.rotation = inverseWoodenBoardRotation;
		//boardManager.N = 4;
		//boardManager.BuildReversedBoard(null);
		//woodenBoard.transform.rotation = originalWoodenBoardRotation;
	}

	void Update() {

		//TODO: To remove
		if (Input.GetKeyDown(KeyCode.Comma)) {
			GoToSettings();
		} else if (Input.GetKeyDown(KeyCode.Period)) {
			BackToBoard();
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}

		if (Input.GetKeyDown(KeyCode.Keypad3)) {
			boardManager.N = 3;
			boardManager.BuildBoard(null);
		}
		if (Input.GetKeyDown(KeyCode.Keypad4)) {
			boardManager.N = 4;
			boardManager.BuildBoard(null);
		}
		if (Input.GetKeyDown(KeyCode.Keypad5)) {
			boardManager.N = 5;
			boardManager.BuildBoard(null);
		}
		if (Input.GetKeyDown(KeyCode.Keypad6)) {
			boardManager.N = 6;
			boardManager.BuildBoard(null);
		}

		//woodenBoard.transform.Rotate(new Vector3(0f, 1f, 0f), Space.Self);
		if (Quaternion.Angle(woodenBoard.transform.rotation, desiredRotation) > 0.1f) {
			elaspedTime += Time.deltaTime * rotationSpeed;
			woodenBoard.transform.rotation = Quaternion.Lerp(woodenBoard.transform.rotation, desiredRotation, elaspedTime);
		}

		if (needToUpdateNbStep) {
			Debug.Log("Need To Update Step");
			needToUpdateNbStep = false;
			sideTrayAnimator.SetTrigger("Close");

			Debug.Log("trim: ");
			Debug.Log(resolutionInformation[0].Split(':')[0]);

			nbStep.text = resolutionInformation[0].Split(':')[1].ToString().Trim();
			complexitySize.text = resolutionInformation[1].Split(':')[1].ToString().Trim();
			complexityTime.text = resolutionInformation[2].Split(':')[1].ToString().Trim();
			elapsedTime.text = resolutionInformation[3].Split(':')[1].ToString().Trim();
		}

		if (client.errorMessage != null) {
			uiManager.DisplayError(client.errorMessage);
			client.errorMessage = null;
			sideTrayAnimator.SetTrigger("Close");
		}

		if (!inSettings) {
			if (Input.GetKeyDown(KeyCode.RightArrow)) {
				boardManager.AddMovements(BoardManager.MoveDirection.Right);
				//solutionNextMoves.AddLast(BoardManager.MoveDirection.Left);
			} else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
				boardManager.AddMovements(BoardManager.MoveDirection.Left);
				//solutionNextMoves.AddLast(BoardManager.MoveDirection.Right);
			} else if (Input.GetKeyDown(KeyCode.UpArrow)) {
				boardManager.AddMovements(BoardManager.MoveDirection.Up);
				//solutionNextMoves.AddLast(BoardManager.MoveDirection.Down);
			} else if (Input.GetKeyDown(KeyCode.DownArrow)) {
				boardManager.AddMovements(BoardManager.MoveDirection.Down);
				//solutionNextMoves.AddLast(BoardManager.MoveDirection.Up);
			}
		}

		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit)) {

				if (hit.collider.CompareTag("Cell")) {
					Debug.Log("Event System stuff: " + EventSystem.current.IsPointerOverGameObject());
					Debug.Log("Will Swap: " + hit.collider.gameObject);
					if (swapFirstCell == null) {
						swapFirstCell = hit.collider.GetComponent<Cell>();
						// If hit the empty cell, do nothing... Or..?
						if (swapFirstCell.value == 0) {
							swapFirstCell = null;
							//swapFirstCell.GetComponent<MeshRenderer>().material = swapCellOriginalMaterial;
						} else {
							swapFirstCell.GetComponent<MeshRenderer>().material = swapCellSelectedMaterial;
						}
					} else {
						swapSecondCell = hit.collider.GetComponent<Cell>();

						if (swapSecondCell.value != 0) {
							swapFirstCell.GetComponent<MeshRenderer>().material = swapCellOriginalMaterial;
							Vector3 tmpPos = swapSecondCell.transform.position;
							swapSecondCell.transform.position = swapFirstCell.transform.position;
							swapFirstCell.transform.position = tmpPos;

							// Find in values list, boardManager

							int iFirstCell = -1, iSecondCell = -1;
							int jFirstCell = -1, jSecondCell = -1;

							for (int i = 0; i < boardManager.N; i++) {
								if (jFirstCell == -1) {
									iFirstCell = i;
									jFirstCell = boardManager.values[i].FindIndex(o => o == swapFirstCell.value);
								}
								if (jSecondCell == -1) {
									iSecondCell = i;
									jSecondCell = boardManager.values[i].FindIndex(o => o == swapSecondCell.value);
								}
							}
							if (jFirstCell != -1 && jSecondCell != -1) {
								int tmp = boardManager.values[iFirstCell][jFirstCell];
								boardManager.values[iFirstCell][jFirstCell] = boardManager.values[iSecondCell][jSecondCell];
								boardManager.values[iSecondCell][jSecondCell] = tmp;

								for (int i = 0; i < boardManager.N; i++) {
									Debug.Log(String.Join(" - ", boardManager.values[i]));
								}
							}

							swapFirstCell = null;
							swapSecondCell = null;
							boardManager.GetClosestCells();


						} else {
							swapSecondCell = null;
						}
					}
				}
			}
		}
	}
	
	public void NextStep() {
		if (solutionNextMoves.Count > 0) {
			BoardManager.MoveDirection nextDirection = solutionNextMoves.Last.Value;
			solutionNextMoves.RemoveLast();
			boardManager.AddMovements(nextDirection);
			solutionPrevMoves.AddLast(oppositeMoveDirection[nextDirection]);
			nbStep.text = solutionNextMoves.Count.ToString();
		}
	}

	public void PrevStep() {
		if (solutionPrevMoves.Count > 0) {
			BoardManager.MoveDirection nextDirection = solutionPrevMoves.Last.Value;
			solutionPrevMoves.RemoveLast();
			boardManager.AddMovements(nextDirection);
			solutionNextMoves.AddLast(oppositeMoveDirection[nextDirection]);
			nbStep.text = solutionNextMoves.Count.ToString();
		}
	}

	public void Play() {
		while (solutionNextMoves.Count > 0) {
			NextStep();
		}
		// Create a event in the board manager to update the step to solution counter
	}

	public void GoToSettings() {
		//woodenBoard.transform.Rotate(new Vector3(90f, 0f, 0f));
		//woodenBoard.transform.rotation = inverseWoodenBoardRotation;
		inSettings = true;
		elaspedTime = 0f;
		desiredRotation = inverseWoodenBoardRotation;
		tray.Close();
		tray.Lock();
	}

	public void BackToBoard() {
		elaspedTime = 0f;
		desiredRotation = originalWoodenBoardRotation;
		tray.Unlock();
		inSettings = false;
	}

	private void Solve(List<List<int>> input) {

		solutionNextMoves.Clear();
		solutionPrevMoves.Clear();
		boardManager.ClearMovements();
		if (solution != null) {
			solution.Clear();
		}
		nbStep.text = "0";

		sideTrayAnimator.SetTrigger("Open");

		//foreach (var item in input) {
		//	Debug.Log(String.Join(" - ", item));
		//}

		Thread serverCommunicationThread = new Thread(new ThreadStart(() => {
			solution = client.CallServer(input);

			if (solution == null) {
				return;
			}
			Debug.Log("Solution Count: " + solution.Count);

			if (solution.Count > 4) {
				resolutionInformation = solution.Skip(solution.Count - 4).Take(4).ToList();
				solution.RemoveAt(solution.Count - 1);
				solution.RemoveAt(solution.Count - 1);
				solution.RemoveAt(solution.Count - 1);
				solution.RemoveAt(solution.Count - 1);
			}

			Debug.Log("ResolutionINformation : " + resolutionInformation.Count);

			for (int i = 0; i < solution.Count; i++) {
				solutionNextMoves.AddFirst(stringToMoveDirection[solution[i]]);
			}
			Debug.Log("Setting the boolean to true");
			needToUpdateNbStep = true;
		}));
		serverCommunicationThread.IsBackground = true;
		serverCommunicationThread.Start();
	}

	public void SolveBoard() {
		Solve(boardManager.values);
	}
}
