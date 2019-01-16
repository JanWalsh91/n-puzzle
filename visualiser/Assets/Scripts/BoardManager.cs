using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

	public int N;
	public GameObject cellPrefab;

	private Vector2 cellSize;

	void Start() {
		Vector3 size = cellPrefab.GetComponent<Renderer>().bounds.size;
		cellSize.x = size.x;
		cellSize.y = size.z;

		float gap = (3 - cellSize.x * N) / 3.0f; // Harcoded 3, buerk

		Vector3 spawnPosition = new Vector3(-1f, 0.05f, 1f);

		for (int i = 0; i < N; i++) {
			for (int j = 0; j < N; j++) {
				if (i == N - 1 && j == N - 1) {
					break;
				}
				GameObject instance = Instantiate(cellPrefab, spawnPosition, Quaternion.identity, transform);
				instance.GetComponentInChildren<TextMesh>().text = (i * N + j + 1).ToString();
				spawnPosition.x += cellSize.x + gap;
			}
			spawnPosition.x = -1f;
			spawnPosition.z -= cellSize.y + gap;
		}
	}

}
