using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

	public int N;
	public GameObject cellPrefab;
	public float movingSpeed = 0.2f;

	private Vector2 cellSize;

	private List<CellManager> cells;
	private Queue<MoveDirection> movements;

	private CellManager emptyCell;

	private CellManager left;
	private CellManager right;
	private CellManager up;
	private CellManager down;

	private GameObject movingCell = null;
	private Vector3 velocity;
	private Vector3 targetDestination;

	enum MoveDirection {
		Down,
		Up,
		Left,
		Right
	}

	void Start() {
		Vector3 size = cellPrefab.GetComponent<Renderer>().bounds.size;
		cellSize.x = size.x;
		cellSize.y = size.z;

		float gap = (3 - cellSize.x * N) / 3.0f; // Harcoded 3, buerk

		Vector3 spawnPosition = new Vector3(-1f, 0.05f, 1f);

		cells = new List<CellManager>();
		for (int i = 0; i < N; i++) {
			for (int j = 0; j < N; j++) {
				GameObject instance = Instantiate(cellPrefab, spawnPosition, Quaternion.identity, transform);
				instance.GetComponentInChildren<TextMesh>().text = (i == N - 1 && j == N - 1) ? "0" : (i * N + j + 1).ToString();
				spawnPosition.x += cellSize.x + gap;
				cells.Add(instance.GetComponent<CellManager>());
				if (i == N - 1 && j == N - 1) {
					instance.GetComponent<MeshRenderer>().enabled = false;
					instance.GetComponentInChildren<TextMesh>().gameObject.SetActive(false);
					emptyCell = cells[cells.Count - 1];
				}
			}
			spawnPosition.x = -1f;
			spawnPosition.z -= cellSize.y + gap;
		}

		////////
		
		movements = new Queue<MoveDirection>();
		StartCoroutine(MovePiece());
		GetClosestCells();

		
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			movements.Enqueue(MoveDirection.Right);
		} else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			movements.Enqueue(MoveDirection.Left);
		} else if (Input.GetKeyDown(KeyCode.UpArrow)) {
			movements.Enqueue(MoveDirection.Up);
		} else if (Input.GetKeyDown(KeyCode.DownArrow)) {
			movements.Enqueue(MoveDirection.Down);
		}

		if (movingCell != null) {
			//Debug.Log("movingCell.transform.position: " + movingCell.transform.position);
			movingCell.transform.position = Vector3.SmoothDamp(movingCell.transform.position, targetDestination, ref velocity, movingSpeed);
		}
	}

	void GetClosestCells() {
		Collider[] colliders = Physics.OverlapSphere(emptyCell.transform.position, cellSize.x + (cellSize.y / 4.0f), LayerMask.GetMask("Cell"));
		Debug.Log("Found: " + colliders.Length);
		left = right = up = down = null;
		foreach (var item in colliders) {
			if (item.transform.Equals(emptyCell.transform)) {
				continue;
			}
			if (item.transform.position.x < emptyCell.transform.position.x && item.transform.position.z == emptyCell.transform.position.z) {
				left = item.GetComponent<CellManager>();
				//Debug.Log("Here 1");
			} else if (item.transform.position.x > emptyCell.transform.position.x && item.transform.position.z == emptyCell.transform.position.z) {
				right = item.GetComponent<CellManager>();
				//Debug.Log("Here 2");
			} else if (item.transform.position.x == emptyCell.transform.position.x && item.transform.position.z > emptyCell.transform.position.z) {
				up = item.GetComponent<CellManager>();
				//Debug.Log("Here 3");
			} else if (item.transform.position.x == emptyCell.transform.position.x && item.transform.position.z < emptyCell.transform.position.z) {
				down = item.GetComponent<CellManager>();
				//Debug.Log("Here 4");
			}
			//Debug.Log(item.GetComponentInChildren<TextMesh>().text);
		}
	}

	IEnumerator MovePiece() {

		while (true) {
			if (movements.Count > 0) {
				MoveDirection move = movements.Dequeue();
				Debug.Log(move.ToString());
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
				GetClosestCells();
			}
			yield return new WaitForSeconds(movingSpeed);
		}
	}
}
