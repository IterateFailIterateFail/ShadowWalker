using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaterfallTrigger : MonoBehaviour {

	private void OnTriggerEnter2D(Collider2D other) {
		if(other.tag == "Player") {
			PlayerControl pc = other.GetComponent<PlayerControl>();
			Debug.Log("WaterfallTrigger");
			if (pc.isUpsideDown()) {
				pc.die ();
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			} else {

			}
			// other.GetComponent(typeof(PlayerControl)).die();
			//SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

}
