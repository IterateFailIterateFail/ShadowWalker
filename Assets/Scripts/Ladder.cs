using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour {
	Transform player;
	Collider2D collider;
	PlayerControl pc;
	// Use this for initialization

	void Start () {
		player = GameObject.Find("Player").transform;
		collider = GetComponent<Collider2D>();
		pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
	}

	// Update is called once per frame
	void Update () {
		
		float collsionPoint = this.transform.position.y + (this.transform.localScale.y)/2;
		Vector2 pt;
		pt.x = transform.position.x;
		pt.y = collsionPoint;
		//Debug.Log (pc.isUpsideDown());
		//UnityEngine.Debug.DrawLine(transform.position, pt,Color.yellow);
		//Debug.Log (pc.isUpsideDown()&&(player.position.y < collsionPoint || pc.inLadder));
		// if the player is 'above' teh ladder, it is solid
		if (!pc.isUpsideDown ()) {
			if (player.position.y > collsionPoint && !pc.inLadder) {
				collider.isTrigger = false;
			} else if (player.position.y < collsionPoint || pc.inLadder) {
				collider.isTrigger = true;
			}
		} else {
			if (player.position.y > collsionPoint && !pc.inLadder) {
				collider.isTrigger = false;
			} else if (player.position.y < collsionPoint || pc.inLadder) {
				collider.isTrigger = true;
			}
		}
	}
}
