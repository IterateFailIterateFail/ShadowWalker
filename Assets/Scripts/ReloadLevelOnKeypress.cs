using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadLevelOnKeypress : MonoBehaviour {


	PlayerControl pc;
	void start(){
		
	}
	void Update () {
		pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
		//Debug.Log (pc);
		if (Input.GetKeyDown("r")) {
			pc.die();
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
}
