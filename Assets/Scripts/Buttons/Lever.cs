// A thing that responds to the player

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour {

	public int collisionCount = 0;
	public bool hasPlayer = false;
	public bool triggered = false;
	// public bool heavy = true;

	// Use this for initialization
	// void Start () {
	//
	// }

	// Update is called once per frame
	// void Update () {
	//
	// }

	void Update() {
		if ((Input.GetKeyDown(KeyCode.E)) && collisionCount > 0) {
			// Debug.Log("Flipped the switch!");
			transform.Rotate(Vector3.forward * 90);
			triggered = !triggered;
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.attachedRigidbody.gameObject.tag == "Player") {
			// Debug.Log("Enter");
			collisionCount += 1;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.attachedRigidbody.gameObject.tag == "Player") {
			// Debug.Log("Leave");
			collisionCount -= 1;
		}
	}

}
