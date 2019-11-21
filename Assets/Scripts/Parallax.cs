using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {

	private static class ParallaxFactor {
		public static float BG0 = 0.5f;
		public static float BG1 = 0.8f;
		public static float FG0 = 1.2f;
	}

	private List<GameObject> background0 = new List<GameObject>();

	private Vector3 previousLocation;

	// Use this for initialization
	void Start () {
		// Get all of those background0s
		background0 = new List<GameObject>(GameObject.FindGameObjectsWithTag ("Background0"));
		previousLocation = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 dPosition = gameObject.transform.position - previousLocation;
		Debug.Log (dPosition);
		// just for background0 
		foreach(var obj in background0){
			Vector3 pos = new Vector3 (obj.transform.position.x + dPosition.x * ParallaxFactor.BG0,
				obj.transform.position.y + dPosition.y * ParallaxFactor.BG0,
				obj.transform.position.z);
			obj.transform.position = pos;
		}

		previousLocation = gameObject.transform.position;

	}
}
