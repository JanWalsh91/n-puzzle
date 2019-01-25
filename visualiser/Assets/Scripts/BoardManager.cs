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

	public enum MoveDirection {
		Down,
		Up,
		Left,
		Right
	}

	void Start() {

		BuildBoard(null);
		movements = new Queue<MoveDirection>();
	}

	public void BuildBoard(List<List<int>> input) {
		Vector3 size = cellPrefab.GetComponent<Renderer>().bounds.size;
		for (int i = 3; i < N; i++) {
			Debug.Log("[i: " + i + "] = " + size.x);
			size.x /= 1.35f;
			size.z /= 1.35f;

		}
	
		cellSize.x = size.x;
		cellSize.y = size.z;

		Debug.Log("size: " + size);

		Vector3 spawnPosition = new Vector3(-1.5f, 0.05f, 1.5f);
		float gap = (3 - cellSize.x * N) / (float)N; // 3 == board size
		spawnPosition.x += gap / 2f + size.x / 2.0f;
		spawnPosition.z -= gap / 2f + size.z / 2.0f;


		foreach (Transform item in transform) {
			Destroy(item.gameObject);
		}

		if (input != null) {
			values = input;
		} else {
			values = new List<List<int>>();
		}

		cells = new List<Cell>();
		for (int i = 0; i < N; i++) {
			values.Add(new List<int>());
			for (int j = 0; j < N; j++) {
				GameObject instance = Instantiate(cellPrefab, spawnPosition, Quaternion.identity, transform);
				instance.transform.localScale = size;
				if (input == null) {
					instance.GetComponentInChildren<TextMesh>().text = (i == N - 1 && j == N - 1) ? "0" : (i * N + j + 1).ToString();
					values[i].Add((i == N - 1 && j == N - 1) ? 0 : (i * N + j + 1));
				} else {
					instance.GetComponentInChildren<TextMesh>().text = input[i][j].ToString();
				}
				spawnPosition.x += cellSize.x + gap;
				Cell cell = instance.GetComponent<Cell>();
				cells.Add(cell);
				cell.value = values[i][j];
				if ((input == null && i == N - 1 && j == N - 1) || input != null && input[i][j] == 0) {
					instance.GetComponent<MeshRenderer>().enabled = false;
					instance.GetComponentInChildren<TextMesh>().gameObject.SetActive(false);
					emptyCell = cells[cells.Count - 1];
				}
			}
			spawnPosition.x = -1.5f + gap / 2f + size.x / 2.0f;
			spawnPosition.z -= cellSize.y + gap;
		}
		GetClosestCells();
	}

	public void AddMovements(params MoveDirection[] moves) {
		foreach (var item in moves) {
			movements.Enqueue(item);
		}
	}

	void Update() {

		if (movingCell != null) {
			movingCell.transform.position = Vector3.SmoothDamp(movingCell.transform.position, targetDestination, ref velocity, movingSpeed);
			if (movingCell.transform.position == targetDestination) {
				movingCell = null;
				GetClosestCells();
			}
		} else if (movements.Count > 0) {
			MoveDirection move = movements.Dequeue();

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

	void GetClosestCells() {
		Collider[] colliders = Physics.OverlapSphere(emptyCell.transform.position, cellSize.x + (cellSize.y / 4.0f), LayerMask.GetMask("Cell"));
		left = right = up = down = null;
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
