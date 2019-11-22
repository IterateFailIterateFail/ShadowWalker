using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI : MonoBehaviour {
	private Rigidbody2D rigidBody;
	Renderer rend;
	public Material[] material;
	Transform player;
	Vector3 lineOfSightEnd;
	public float radius = 8f;
	public float speed = 0.01f;
	int time  = 0;
	Vector3 startPos;

	// Use this for initialization
	   
	void Start () {
		rigidBody = GetComponent<Rigidbody2D>();
		rend = GetComponent<Renderer>();
		//rend.enabled = true;
		rend.sharedMaterial = material[0];
		player = GameObject.Find("Player").transform;
		startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Renderer rend = GetComponent<Renderer>();
		Vector3 temp = transform.position;
		temp.x = temp.x + 8f;
		lineOfSightEnd = temp;
		if (LineOfSight()&& !PlayerHiddenByObstacles()) {
			time = 0;
			rend.sharedMaterial = material[1];
			transform.position = Vector3.MoveTowards (transform.position, player.position, speed);
		}else if(time < 90){// pause as it looks for player
			time++;
		}else if (time == 90){			
			rend.sharedMaterial = material[0];
			transform.position = Vector3.MoveTowards (transform.position,startPos ,speed);
		}
		//Debug.Log (startPos);

	}
	bool LineOfSight() {
		// check if the player is within the enemy's field of view
		// this is only checked if the player is within the enemy's sight range

		// find the angle between the enemy's 'forward' direction and the player's location and return true if it's within 65 degrees (for 130 degree field of view)

		//UnityEngine.Debug.DrawLine(transform.position, player.position, Color.magenta); // a line drawn in the Scene window equivalent to directionToPlayer
		Vector2 directionToPlayer = player.position - transform.position; // represents the direction from the enemy to the player 
		Vector2 lineOfSight1 = lineOfSightEnd - transform.position; // the centre of the enemy's field of view, the direction of looking directly ahead
		//UnityEngine.Debug.DrawLine(transform.position, lineOfSightEnd,Color.yellow);
		//UnityEngine.Debug.DrawLine(transform.position, lineOfSightEnd,Color.yellow);// a line drawn in the Scene window equivalent to the enemy's field of view centre
		Vector2 lineOfSight2 = lineOfSightEnd + transform.position; // the centre of the enemy's field of view, the direction of looking directly ahead
		// calculate the angle formed between the player's position and the centre of the enemy's line of sight
		float angle = Vector2.Angle(directionToPlayer, lineOfSight1);
		float angle2 = Vector2.Angle(directionToPlayer, lineOfSight2);
		if(directionToPlayer.magnitude > radius) return false;
		// if the player is within 65 degrees (either direction) of the enemy's centre of vision (i.e. within a 130 degree cone whose centre is directly ahead of the enemy) return true
		if (angle < 65 || (angle2 > 115 && angle2 < 180))
			return true;
		else
			return false;
	}

	bool PlayerHiddenByObstacles(){
		float distanceToPlayer = Vector2.Distance(transform.position, player.position);
		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, player.position - transform.position, distanceToPlayer);
		//UnityEngine.Debug.DrawLine(transform.position, player.position - transform.position, Color.blue); // draw line in the Scene window to show where the raycast is looking
		List<float> distances = new List<float>();
		foreach (RaycastHit2D hit in hits){           
			// ignore the enemy's own colliders (and other enemies)
			if (hit.transform.tag == "Enemy")continue;
			//Debug.Log (hit.transform.tag);
			// if anything other than the player is hit then it must be between the player and the enemy's eyes (since the player can only see as far as the player)
			if (hit.transform.tag != "Player")return true;
		}
		// if no objects were closer to the enemy than the player return false (player is not hidden by an object)
		return false; 
	}
}
