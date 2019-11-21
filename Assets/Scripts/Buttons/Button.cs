// A button that only responds to boxes

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

	private int collisionCount = 0;
	public bool triggered = false;
	public bool heavy = true;

	// Use this for initialization
	// void Start () {
	//
	// }

	// Update is called once per frame
	// void Update () {
	//
	// }

	void OnTriggerEnter2D(Collider2D other) {
		if (other.attachedRigidbody.gameObject.tag == "Box") {
			// Debug.Log("Enter");
			collisionCount += 1;
		}
		if (other.attachedRigidbody.gameObject.tag == "Player" && !heavy) {
			// Debug.Log("Enter");
			collisionCount += 1;
		}
		triggered = (collisionCount > 0);
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.attachedRigidbody.gameObject.tag == "Box") {
			// Debug.Log("Leave");
			collisionCount -= 1;
		}
		if (other.attachedRigidbody.gameObject.tag == "Player" && !heavy) {
			// Debug.Log("Leave");
			collisionCount -= 1;
		}
		triggered = (collisionCount > 0);
	}

}
