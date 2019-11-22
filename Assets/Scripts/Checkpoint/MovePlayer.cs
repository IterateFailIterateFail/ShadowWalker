using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovePlayer : MonoBehaviour {

	public Vector2 targetLocation;
	Rigidbody2D rigidBody;
	public float prevGravity;
	public float prevRotation;
	// Use this for initialization
	void Start() {
		DontDestroyOnLoad(transform.gameObject);
	}

	void OnEnable() {

		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	// Update is called once per frame
	void OnSceneLoaded(Scene scene, LoadSceneMode mod) {
		// Don't move the player if our values haven't actually been set.
		if (targetLocation.x != 0 || targetLocation.y != 0) {
			Debug.Log("Loaded level. Will move player to:");
			Debug.Log(targetLocation);
			GameObject[] gos = GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject go in gos) {
				go.transform.position = targetLocation;
				PlayerControl pc = go.GetComponent(typeof(PlayerControl)) as PlayerControl;
				rigidBody = go.GetComponent<Rigidbody2D>(); // when reloading player, get their old data fro  their checkpoint
				Debug.Log("after die");
				Debug.Log(pc.checkpointGravity);
				rigidBody.gravityScale = prevGravity;
				rigidBody.rotation = prevRotation;
				pc.checkpointLocation = pc.transform.position;

			}
		} else {
			Debug.Log("<color=red>MovePlayer.targetLocation was not set.</color>");
		}
		Destroy(transform.gameObject);
	}
}
