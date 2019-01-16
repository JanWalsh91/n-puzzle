using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	private Camera cam;
	private List<CellManager> cells;

	void Start() {
		cam = FindObjectOfType<Camera>();
		cells = new List<CellManager>(FindObjectsOfType<CellManager>());
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)) {

			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 50f)) {
				//draw invisible ray cast/vector
				Debug.DrawLine(ray.origin, hit.point);
				//log hit area to the console
				Debug.Log(hit.point);

				foreach (var item in cells) {
					item.SetSelected(false);
				}

				if (hit.transform.CompareTag("Cell")) {
					CellManager cm = hit.transform.GetComponent<CellManager>();
					cm.SetSelected(true);
				}
			}

		}
	}
}
