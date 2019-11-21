using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonReciever : MonoBehaviour {

	public int numRequired = 1;
	public GameObject[] triggers;
	public Vector2 move;
	public int moveTime = 60;

	private Vector2 startPoint;
	private int position = 0;
	private MovingPlatform mp;

	// Use this for initialization
	void Start () {
		startPoint = transform.position;
		mp = GetComponent<MovingPlatform>();
	}

	public bool isTriggered() {
		int numTriggered = 0;
		foreach (GameObject	go in triggers) {
			Button button = go.GetComponent<Button>();
			if (button && button.triggered) numTriggered += 1;
			Lever lever = go.GetComponent<Lever>();
			if (lever && lever.triggered) numTriggered += 1;
		}
		return (numTriggered >= numRequired);
	}

	void FixedUpdate() {
		if (mp == null) {// chaneg this if there becomes oterh scripts that might use button reiciever
			if (isTriggered ()) {
				position = Mathf.Min (position + 1, moveTime);
				//Debug.Log ("lose!");
			} else {
				position = Mathf.Max (position - 1, 0);
			}
			transform.position = startPoint + move * ((float)position / (float)moveTime);
		}
	}
}
