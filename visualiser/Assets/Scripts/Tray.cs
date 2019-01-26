using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tray : MonoBehaviour {

	private Animator animator;
	public bool locked = false;

	void Start () {
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseEnter() {
		Debug.Log("MouseEnter");
		Open();
	}

	void OnMouseExit() {
		Debug.Log("MouseExit");
		Close();
	}

	public void Open() {
		if (!locked) {
			animator.SetTrigger("Open");
		}
	}

	public void Close() {
		animator.SetTrigger("Close");
	}

	public void Lock() {
		locked = true;
	}

	public void Unlock() {
		locked = false;
	}
}

