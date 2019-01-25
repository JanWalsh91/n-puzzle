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
	public List<CellManager> cells;
	private Queue<MoveDirection> movements;

	private CellManager emptyCell;

	private CellManager left;
	private CellManager right;
	private CellManager up;
	private CellManager down;

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
		//cellPrefab.transform.localScale = size;
	
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

		cells = new List<CellManager>();
		for (int i = 0; i < N; i++) {
			for (int j = 0; j < N; j++) {
				GameObject instance = Instantiate(cellPrefab, spawnPosition, Quaternion.identity, transform);
				instance.transform.localScale = size;
				if (input == null) {
					instance.GetComponentInChildren<TextMesh>().text = (i == N - 1 && j == N - 1) ? "0" : (i * N + j + 1).ToString();
				} else {
					instance.GetComponentInChildren<TextMesh>().text = input[i][j].ToString();
				}
				spawnPosition.x += cellSize.x + gap;
				cells.Add(instance.GetComponent<CellManager>());
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
		//Debug.Log("Enqueue, size is now: " + movements.Count);
	}

	void Update() {

		if (movingCell != null) {
			//Debug.Log("movingCell.transform.position: " + movingCell.transform.position);
			movingCell.transform.position = Vector3.SmoothDamp(movingCell.transform.position, targetDestination, ref velocity, movingSpeed);
			if (movingCell.transform.position == targetDestination) {
				//Debug.Log("Here, done");
				movingCell = null;
				GetClosestCells();
			}
		} else if (movements.Count > 0) {
			MoveDirection move = movements.Dequeue();
			//Debug.Log(move.ToString());

			targetDestination = emptyCell.transform.position;
			switch (move) {
				case MoveDirection.Down:
					if (!up) {
						break;
					}
					emptyCell.transform.position = up.transform.position;
					movingCell = up.gameObject;
					break;
				case MoveDirection.Up:
					if (!down) {
						break;
					}
					emptyCell.transform.position = down.transform.position;
					movingCell = down.gameObject;
					break;
				case MoveDirection.Left:
					if (!right) {
						break;
					}
					emptyCell.transform.position = right.transform.position;
					movingCell = right.gameObject;
					break;
				case MoveDirection.Right:
					if (!left) {
						break;
					}
					emptyCell.transform.position = left.transform.position;
					movingCell = left.gameObject;
					break;
			}
		}
	}

	void GetClosestCells() {
		Collider[] colliders = Physics.OverlapSphere(emptyCell.transform.position, cellSize.x + (cellSize.y / 4.0f), LayerMask.GetMask("Cell"));
		//Debug.Log("Found: " + colliders.Length);
		left = right = up = down = null;
		foreach (var item in colliders) {
			if (item.transform.Equals(emptyCell.transform)) {
				continue;
			}
			if (item.transform.position.x < emptyCell.transform.position.x && Mathf.Abs(item.transform.position.z - emptyCell.transform.position.z) < 0.01f) {
				left = item.GetComponent<CellManager>();
				//Debug.Log("Here 1");
				//Debug.Log(left.GetComponentInChildren<TextMesh>().text);
			} else if (item.transform.position.x > emptyCell.transform.position.x && Mathf.Abs(item.transform.position.z - emptyCell.transform.position.z) < 0.01f) {
				right = item.GetComponent<CellManager>();
				//Debug.Log("Here 2");
				//Debug.Log(right.GetComponentInChildren<TextMesh>().text);

			} else if (Mathf.Abs(item.transform.position.x - emptyCell.transform.position.x) < 0.01f && item.transform.position.z > emptyCell.transform.position.z) {
				up = item.GetComponent<CellManager>();
				//Debug.Log("Here 3");
				//Debug.Log(up.GetComponentInChildren<TextMesh>().text);

			} else if (Mathf.Abs(item.transform.position.x - emptyCell.transform.position.x) < 0.01f && item.transform.position.z < emptyCell.transform.position.z) {
				down = item.GetComponent<CellManager>();
				//Debug.Log("Here 4");
				//Debug.Log(down.GetComponentInChildren<TextMesh>().text);

			}
			//Debug.Log(item.GetComponentInChildren<TextMesh>().text);
		}
	}

	//IEnumerator MovePiece() {

	//	while (true) {
	//		if (movements.Count > 0) {
				
	//		}
	//		yield return new WaitForSeconds(movingSpeed);
	//	}
	//}
}
