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
	public Text playPauseButtonText;
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
	private Coroutine playCoroutine = null;

	public Quaternion originalWoodenBoardRotation, inverseWoodenBoardRotation, desiredRotation;

	private float elaspedTime;
	private float rotationSpeed = 0.5f;

	private bool inSettings = false;
	private bool canMove = true;

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
		desiredRotation = originalWoodenBoardRotation;
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}

		if (Quaternion.Angle(woodenBoard.transform.rotation, desiredRotation) > 0.1f) {
			elaspedTime += Time.deltaTime * rotationSpeed;
			woodenBoard.transform.rotation = Quaternion.Lerp(woodenBoard.transform.rotation, desiredRotation, elaspedTime);
		}

		if (needToUpdateNbStep) {
			needToUpdateNbStep = false;
			sideTrayAnimator.SetTrigger("Close");
			nbStep.text = resolutionInformation[0].Split(':')[1].ToString().Trim();
			complexitySize.text = resolutionInformation[1].Split(':')[1].ToString().Trim();
			complexityTime.text = resolutionInformation[2].Split(':')[1].ToString().Trim();
			elapsedTime.text = resolutionInformation[3].Split(':')[1].ToString().Trim();
		}

		if (client.errorMessage != null) {
			uiManager.DisplayError(client.errorMessage);
			client.errorMessage = null;
			sideTrayAnimator.SetTrigger("Close");
			ResetSolution();
		}

		if (!inSettings && canMove) {
			if (Input.GetKeyDown(KeyCode.RightArrow)) {
				boardManager.AddMovements(BoardManager.MoveDirection.Right);
				ResetSolution();
			} else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
				boardManager.AddMovements(BoardManager.MoveDirection.Left);
				ResetSolution();
			} else if (Input.GetKeyDown(KeyCode.UpArrow)) {
				boardManager.AddMovements(BoardManager.MoveDirection.Up);
				ResetSolution();
			} else if (Input.GetKeyDown(KeyCode.DownArrow)) {
				boardManager.AddMovements(BoardManager.MoveDirection.Down);
				ResetSolution();
			}
		}

		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit)) {

				if (hit.collider.CompareTag("Cell") && playCoroutine == null) {
					if (swapFirstCell == null) {
						swapFirstCell = hit.collider.GetComponent<Cell>();
						if (swapFirstCell.value == 0) {
							swapFirstCell = null;
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
	
	public void ResetSolution() {
		solutionNextMoves.Clear();
		solutionPrevMoves.Clear();
		nbStep.text = "-";
		complexitySize.text = "-";
		complexityTime.text = "-";
		elapsedTime.text = "-";
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
		if (playCoroutine == null && solutionNextMoves.Count > 0) {
			playCoroutine = StartCoroutine(PlayCoroutine());
			playPauseButtonText.text = "||";
		} else {
			Pause();
		}
	}

	public void Pause() {
		if (playCoroutine != null) {
			StopCoroutine(playCoroutine);
			boardManager.canSendAnotherMove = true;
			canMove = true;
			playPauseButtonText.text = ">";
			playCoroutine = null;
			//TODO: if move, reset everything
		}
	}

	IEnumerator PlayCoroutine() {
		canMove = false;
		boardManager.canSendAnotherMove = true;
		while (solutionNextMoves.Count > 0) {
			if (boardManager.canSendAnotherMove) {
				NextStep();
			}
			yield return new WaitForSeconds(boardManager.movingSpeed);
		}
		canMove = true;
		playPauseButtonText.text = ">";
	}

	public void GoToSettings() {
		if (!canMove) {
			return;
		}
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

		ResetSolution();
		boardManager.ClearMovements();
		if (solution != null) {
			solution.Clear();
		}

		sideTrayAnimator.SetTrigger("Open");
		Thread serverCommunicationThread = new Thread(new ThreadStart(() => {
			solution = client.CallServer(input);
			if (solution == null) {
				sideTrayAnimator.SetTrigger("Close");
				return;
			}
			if (solution.Count >= 4) {
				resolutionInformation = solution.Skip(solution.Count - 4).Take(4).ToList();
				solution.RemoveAt(solution.Count - 1);
				solution.RemoveAt(solution.Count - 1);
				solution.RemoveAt(solution.Count - 1);
				solution.RemoveAt(solution.Count - 1);
			}
			for (int i = 0; i < solution.Count; i++) {
				solutionNextMoves.AddFirst(stringToMoveDirection[solution[i]]);
			}
			needToUpdateNbStep = true;
		}));
		serverCommunicationThread.IsBackground = true;
		serverCommunicationThread.Start();
	}

	public void SolveBoard() {
		Solve(boardManager.values);
	}
}
