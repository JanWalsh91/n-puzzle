using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour {


	//private Quaternion randomRotation;
	private float speed = 5f;

	void Start() {
		//InvokeRepeating("UpdateRotation", 0f, 1f);
	}

	void Update() {
		transform.Rotate(new Vector3(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f)) * speed);
	}

	//void UpdateRotation() {
	//	randomRotation = Random.rotation;
	//}

}
