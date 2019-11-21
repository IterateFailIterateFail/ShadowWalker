using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI : MonoBehaviour {
	private Rigidbody2D rigidBody;
	Renderer rend;
	public Material[] material;
	Transform player;
	Vector3 lineOfSightEnd;
	[SerializeField]
	float radius; 
	[SerializeField]
	float speed;
	int time  = 90;
	Vector3 startPos;
	Vector3 endPos;
	[SerializeField]
	bool patrol = false;
	[SerializeField]
	float endPointX = 0f;
	[SerializeField]
	float endPointY = 0f;
	bool atStart = true;
	int layermask;
	bool chase = false;
	// Use this for initialization
	   
	void Start () {
		layermask = 1 << 2;
		layermask = ~layermask;
		rigidBody = GetComponent<Rigidbody2D>();
		rend = GetComponent<Renderer>();
		//rend.enabled = true;
		rend.sharedMaterial = material[0];
		player = GameObject.Find("Player").transform;
		startPos = transform.position;
		if (patrol) {
			Vector3 temp = transform.position;
			temp.x += endPointX;
			temp.y += endPointY;
			endPos = temp;
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (layermask);
		Renderer rend = GetComponent<Renderer>();
		Vector3 temp = transform.position;
		temp.x = temp.x + radius;
		lineOfSightEnd = temp;
		// clear line of sight CHARGE
		if (LineOfSight () && !PlayerHiddenByObstacles ()) {
			chase = true;
			time = 0;
			rend.sharedMaterial = material [1];

			transform.position = Vector3.MoveTowards (transform.position, player.position, speed);
//			Debug.Log (player.position);
			if (rigidBody.velocity.x == 0) {// collison..ish
				//Debug.Log("Jump!");
				Vector2 velocity = rigidBody.velocity;
				velocity.y = speed * rigidBody.gravityScale;
				rigidBody.velocity = velocity;
			}
		}else if (time == 90) {
			
			// if no players, jus go back to what it was doing before, either patroling and sulking
			if (chase) {
				startPos = transform.position;
				if (patrol) {
					temp = transform.position;
					temp.x += endPointX;
					temp.y += endPointY;
					endPos = temp;
				}
				chase = false;
			}
			rend.sharedMaterial = material [0];
			if (Vector3.Distance (transform.position, endPos) <= 0.05 && atStart) {
				atStart = false;
				//Debug.Log ("at end");
			} else if (Vector3.Distance (transform.position, startPos) <= 0.05 && !atStart) {
				atStart = true;
				//Debug.Log ("at start");
			}
			if (atStart && patrol)
				transform.position = Vector3.MoveTowards (transform.position, endPos, speed);
			else
				transform.position = Vector3.MoveTowards (transform.position, startPos, speed);

		// might be temporay?
		} else if (time == 200) { // disappear 
			Object.Destroy(this.gameObject);
		}
		if (!chase) {
			time++;
		}
//		Debug.Log (time);

	}
	bool LineOfSight() {
		// check if the player is within the enemy's field of view
		// this is only checked if the player is within the enemy's sight range

		Vector2 directionToPlayer = player.position - transform.position; // represents the direction from the enemy to the player 

		float angle = Vector2.SignedAngle(transform.position,directionToPlayer );
		//Debug.Log (angle);
		if(directionToPlayer.magnitude > radius) return false;
		// if the player is within 65 degrees (either direction) of the enemy's centre of vision (i.e. within a 130 degree cone whose centre is directly ahead of the enemy) return true
		// Note: Not sure why but compasss is moved 135 degees
		if (((angle < 180 && angle > 90 )||(angle > -90 && angle < -0))&& directionToPlayer.magnitude < radius) {
		//	Debug.Log ("In range");
			return true;
		} else {
		//	Debug.Log ("No in range");
			return false;
		}
	}

	bool PlayerHiddenByObstacles(){
		float distanceToPlayer = Vector2.Distance(transform.position, player.position);
		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, player.position - transform.position, distanceToPlayer,layermask);
		//UnityEngine.Debug.DrawLine(transform.position, player.position - transform.position, Color.blue); // draw line in the Scene window to show where the raycast is looking
		//List<float> distances = new List<float>();
		foreach (RaycastHit2D hit in hits){  
			//Debug.Log (hit.transform.gameObject.layer);
			//Debug.Log (hit.transform.tag);
			// ignore the enemy's own colliders (and other enemies)
			if (hit.transform.tag == "Enemy")continue;
			//Debug.Log (hit.transform.tag);
			// if anything other than the player is hit then it must be between the player and the enemy's eyes (since the player can only see as far as the player)
			if (hit.transform.tag == "Player") {
				//Debug.Log("Not Hidden");
				return false;
			}
		}
		//Debug.Log ("hidden");
		// if no objects were closer to the enemy than the player return false (player is not hidden by an object)
		return true; 
	}

}
