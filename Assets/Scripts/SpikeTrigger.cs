using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpikeTrigger : MonoBehaviour {
	

	private void OnTriggerEnter2D(Collider2D other) {
		if(other.tag == "Player") {
			PlayerControl pc = other.GetComponent<PlayerControl>();
			pc.die();
			// other.GetComponent(typeof(PlayerControl)).die();
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Player") {
			PlayerControl pc = other.gameObject.GetComponent<PlayerControl>();
			pc.die();
			// other.GetComponent(typeof(PlayerControl)).die();
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

}
