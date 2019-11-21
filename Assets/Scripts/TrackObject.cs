using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This makes one object follow another object around. It only modifies the
// x and y coordinates, not the z coordinates
public class TrackObject : MonoBehaviour {

	// The object that we want to track
	public GameObject target;
	// The factor we multiply by to delay movement.
	public float delayFactor;

	public float offsetDistance;

	public float smoothTime = 0.3F;

	private Vector3 velocity = Vector3.zero;

	void Start(){
		Vector3 pos = transform.position;

		pos.x = target.transform.position.x;
		pos.y = target.transform.position.y;
		transform.position = pos;
	}

	// Called once at the end of each frame step

	void LateUpdate() {
		Vector3 posTarget = target.transform.position;
		Vector3 posCurrent = transform.position;

		Vector3 posTargetOffsetLeft = target.transform.position;
		Vector3 posTargetOffsetRight = target.transform.position;

		posTargetOffsetLeft.x -= offsetDistance;
		posTargetOffsetRight.x += offsetDistance;

//		LEGACY CODE
//		pos.x = target.transform.position.x;
//		pos.y = target.transform.position.y;
		Vector3 damped;
		if (Input.GetKey (KeyCode.LeftArrow)) {
			damped = Vector3.SmoothDamp(posCurrent,posTargetOffsetLeft, ref velocity, smoothTime);
		} else if (Input.GetKey (KeyCode.RightArrow)) {
			damped = Vector3.SmoothDamp(posCurrent,posTargetOffsetRight, ref velocity, smoothTime);
		} else {
			damped = Vector3.SmoothDamp(posCurrent,posTarget, ref velocity, smoothTime);
		}



		damped.z = -20;
		transform.position = damped;
		//TODO: Find why the x axis keeps approaching infinity

	}

}
