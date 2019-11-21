using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spaqner : MonoBehaviour {
	[SerializeField]
	GameObject[] spawnee; // thing we are spawning
	Transform player;
	Camera cammera;
	bool surprise = false;
	[SerializeField]
	float width;
	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player").transform;
		cammera = GameObject.Find ("Camera").GetComponent<Camera> ();
		Vector3 pos = transform.position;
		pos.x = player.position.x;
		pos.y = player.position.y;
		transform.position = pos;
	}

	// Update is called once per frame
	void Update () {

		Vector3 pos = Position(transform.position);
		transform.position = pos;

		// Only one monster at a time plz
		if (GameObject.Find ("Monster") != null) surprise = false;

		//TEMPOARY
		if (Input.GetKeyDown(KeyCode.Equals)){
			surprise = true;
		}
		if (surprise) { //spawn only on surprise
			surprise = false;
			GameObject clone;
			clone = Instantiate (spawnee[0], transform.position, transform.rotation) as GameObject;
		}
	}
	// make a position that is eitehr left or right of the player by an amount
	Vector3 Position(Vector3 pos){
		float left = player.position.x - cammera.orthographicSize - width;
		// I want teh spawner to be a bit more random but for now monster coming from  left
		pos.x = left;

		return pos;
	}
}