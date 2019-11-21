using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaterBehaviour : MonoBehaviour {

	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Player") {
			Debug.Log (other.gameObject.transform.position);
			//PlayerControl pc = other.gameObject.GetComponent<PlayerControl>();
			//pc.die();
			// other.GetComponent(typeof(PlayerControl)).die();
			//SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
