using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tray : MonoBehaviour {

	private Animator animator;
	public bool locked = false;
	public GraphicRaycaster trayGraphicRaycaster;

	void Start () {
		animator = GetComponent<Animator>();
	}
	
	void OnMouseEnter() {
		Open();
	}

	void OnMouseExit() {
		Close();
	}

	public void Open() {
		if (!locked) {
			animator.SetTrigger("Open");
			trayGraphicRaycaster.enabled = true;
		}
	}

	public void Close() {
		if (!locked) {
			animator.SetTrigger("Close");
			trayGraphicRaycaster.enabled = false;
		}
	}

	public void Lock() {
		locked = true;
	}

	public void Unlock() {
		locked = false;
	}
}

