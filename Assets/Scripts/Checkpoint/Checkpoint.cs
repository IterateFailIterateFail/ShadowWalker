using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

	private void OnTriggerEnter2D(Collider2D other) {
		if(other.tag == "Player") {
			PlayerControl pc = other.GetComponent<PlayerControl>(); // save player's location,gravty and rotation
			pc.checkpointLocation = transform.position;
			Rigidbody2D rigidBody = other.GetComponent<Rigidbody2D>();
			pc.checkpointGravity = rigidBody.gravityScale;
			pc.checkpointRotaion = rigidBody.rotation;
			//Debug.Log (pc.checkpointGravity);
		}
	}
}
