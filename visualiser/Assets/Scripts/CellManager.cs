using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellManager : MonoBehaviour {

	private bool selected;
	private Animator animator;

	void Start () {
		selected = false;
		animator = GetComponent<Animator>();
	}

	void Update() {
		if (selected) {
			// Use animation to move the cell
			if (Input.GetKeyDown(KeyCode.RightArrow)) {
				animator.SetInteger("Direction", 1);
			}
		}
	}
	

	public bool IsSelected() {
		return selected;
	}

	public void SetSelected(bool selected) {
		this.selected = selected;
	}

}
