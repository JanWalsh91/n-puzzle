using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

	[Range(3, 7)]
	public int N;
	public GameObject cellPrefab;
	public float movingSpeed = 0.015f;

	private Vector2 cellSize;

	// Create getter
	public List<Cell> cells;
	public List<List<int>> values;
	private Queue<MoveDirection> movements;

	private Cell emptyCell;

	private Cell left;
	private Cell right;
	private Cell up;
	private Cell down;

	private GameObject movingCell = null;
	private Vector3 velocity;
	private Vector3 targetDestination;

	public bool canSendAnotherMove = false;

	private Dictionary<int, float> NtoSize;

	public enum MoveDirection {
		Down,
		Up,
		Left,
		Right
	}

	void Start() {
		N = 3;
		movements = new Queue<MoveDirection>();
		NtoSize = new Dictionary<int, float>();

		NtoSize.Add(3, 0.9f);
		NtoSize.Add(4, 0.675f);
		NtoSize.Add(5, 0.54f);
		NtoSize.Add(6, 0.445f);

	
		BuildBoard(null);
	}

	private void OnDrawGizmos() {
		Gizmos.DrawSphere(new Vector3(-1.5f, 0.05f, 1.5f), 0.1f);
		Gizmos.DrawSphere(new Vector3(1.5f, -0.05f, 1.5f), 0.1f);
	}

	public void BuildBoard(List<List<int>> input) {
		Vector3 size = new Vector3(NtoSize[N], 1f, NtoSize[N]);

		//float scale = -0.74f * Mathf.Log((float)N) + 1.843881197f;

		cellSize = size;

		//size *= scale;

		Debug.Log("Size: " + size);
		size.y = 1f;
		Vector3 spawnPosition = new Vector3(-1.45f, 0.15f, 1.45f);


		float gap = (0.2f / (float)N);
		float padding = cellSize.x / 2f + gap / 2f;
		Debug.Log("Gap: " + gap);


		spawnPosition.x += padding;
		spawnPosition.z -= padding;
		Quaternion rotation = cellPrefab.transform.rotation;

		foreach (Transform item in transform) {
			Destroy(item.gameObject);
		}

		if (input != null) {
			values = null;
			values = input;
		} else {
			values = new List<List<int>>();
		}

		cells = new List<Cell>();
		for (int i = 0; i < N; i++) {
			if (input == null) {
				values.Add(new List<int>());
			}
			//spawnPosition.x = -1.5f - gap / 2f + size.x / 2f;
			spawnPosition.x = -1.45f + padding;
			for (int j = 0; j < N; j++) {
				GameObject instance = Instantiate(cellPrefab, spawnPosition, rotation, transform);
				instance.transform.localScale = new Vector3(NtoSize[N], 1, NtoSize[N]);
				if (input == null) {
					instance.GetComponentInChildren<TextMesh>().text = (i == N - 1 && j == N - 1) ? "0" : (i * N + j + 1).ToString();
					values[i].Add((i == N - 1 && j == N - 1) ? 0 : (i * N + j + 1));
					//Debug.Log("Adding an int: " + ((i == N - 1 && j == N - 1) ? 0 : (i * N + j + 1)));
				} else {
					instance.GetComponentInChildren<TextMesh>().text = input[i][j].ToString();
				}
				Cell cell = instance.GetComponent<Cell>();
				cells.Add(cell);
				cell.value = values[i][j];
				if ((input == null && i == N - 1 && j == N - 1) || input != null && input[i][j] == 0) {
					instance.GetComponent<MeshRenderer>().enabled = false;
					instance.GetComponentInChildren<TextMesh>().gameObject.SetActive(false);
					emptyCell = cells[cells.Count - 1];
				}
				spawnPosition.x += cellSize.x + gap;
			}

			spawnPosition.z -= cellSize.x + gap;
		}

		//Debug.Log(" === BuildBoard Values, After Eveything === BEGIN");
		//foreach (var item in values) {
		//	Debug.Log(System.String.Join(" - ", item));
		//}
		//Debug.Log(" === BuildBoard Values, After Eveything === END");


		GetClosestCells();
	}

	public void BuildReversedBoard(List<List<int>> input) {

		//Debug.Log(" === BuildReversedBoard === BEGIN");
		//foreach (var item in input) {
		//	Debug.Log(System.String.Join(" - ", item));
		//}
		//Debug.Log(" === BuildReversedBoard === END");

		GameManager gameManager = FindObjectOfType<GameManager>(); // TODO
		transform.parent.rotation = gameManager.originalWoodenBoardRotation;
		BuildBoard(input);
		transform.parent.rotation = gameManager.inverseWoodenBoardRotation;

	}

	public void AddMovements(params MoveDirection[] moves) {
		foreach (var item in moves) {
			movements.Enqueue(item);
		}
	}

	public void ClearMovements() {
		movements.Clear();
	}

	void Update() {

		if (movingCell != null) {
			movingCell.transform.position = Vector3.SmoothDamp(movingCell.transform.position, targetDestination, ref velocity, movingSpeed);
			if (movingCell.transform.position == targetDestination) {
				movingCell = null;
				canSendAnotherMove = true;
				GetClosestCells();
			}
		} else if (movements.Count > 0) {
			MoveDirection move = movements.Dequeue();
			canSendAnotherMove = false;
			targetDestination = emptyCell.transform.position;
			switch (move) {
				case MoveDirection.Down:
					if (!up) {
						break;
					}
					emptyCell.transform.position = up.transform.position;
					movingCell = up.gameObject;
					for (int i = 0; i < N; i++) {
						int j = values[i].FindIndex(o => o == up.value);
						if (j > -1) {
							values[i + 1][j] = up.value;
							values[i][j] = 0;
							break;
						}
					}
					break;
				case MoveDirection.Up:
					if (!down) {
						break;
					}
					emptyCell.transform.position = down.transform.position;
					movingCell = down.gameObject;
					for (int i = 0; i < N; i++) {
						int j = values[i].FindIndex(o => o == down.value);
						if (j > -1) {
							values[i - 1][j] = down.value;
							values[i][j] = 0;
							break;
						}
					}
					break;
				case MoveDirection.Left:
					if (!right) {
						break;
					}
					emptyCell.transform.position = right.transform.position;
					movingCell = right.gameObject;
					for (int i = 0; i < N; i++) {
						int j = values[i].FindIndex(o => o == right.value);
						if (j > -1) {
							values[i][j - 1] = right.value;
							values[i][j] = 0;
							break;
						}
					}
					break;
				case MoveDirection.Right:
					if (!left) {
						break;
					}
					emptyCell.transform.position = left.transform.position;
					movingCell = left.gameObject;
					for (int i = 0; i < N; i++) {
						int j = values[i].FindIndex(o => o == left.value);
						if (j > -1) {
							values[i][j + 1] = left.value;
							values[i][j] = 0;
							break;
						}
					}
					break;
			}
		}
	}

	public void GetClosestCells() {
		Debug.Log("Cell Size Closest cell: " + cellSize);
		Collider[] colliders = Physics.OverlapSphere(emptyCell.transform.position, cellSize.x + cellSize.x / 2f, LayerMask.GetMask("Cell"));
		left = right = up = down = null;
		//Debug.Log("Sphere size: " + (cellSize.x + (cellSize.y / 4.0f)));
		//Debug.Log(colliders.Length);
		foreach (var item in colliders) {
			if (item.transform.Equals(emptyCell.transform)) {
				continue;
			}
			if (item.transform.position.x < emptyCell.transform.position.x && Mathf.Abs(item.transform.position.z - emptyCell.transform.position.z) < 0.01f) {
				left = item.GetComponent<Cell>();
			} else if (item.transform.position.x > emptyCell.transform.position.x && Mathf.Abs(item.transform.position.z - emptyCell.transform.position.z) < 0.01f) {
				right = item.GetComponent<Cell>();
			} else if (Mathf.Abs(item.transform.position.x - emptyCell.transform.position.x) < 0.01f && item.transform.position.z > emptyCell.transform.position.z) {
				up = item.GetComponent<Cell>();
			} else if (Mathf.Abs(item.transform.position.x - emptyCell.transform.position.x) < 0.01f && item.transform.position.z < emptyCell.transform.position.z) {
				down = item.GetComponent<Cell>();
			}
		}
	}
}
