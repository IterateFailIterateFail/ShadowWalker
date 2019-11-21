using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pullable : MonoBehaviour {

	private Collider2D _collider;
	private bool touchingPlayer = false;
	private bool pulling = false;
	private Vector3 equilibirum = new Vector3();
	private GameObject player;
	private Rigidbody2D rb;
	// Change eventually
	[Range(1f,100f)]
	public float springConstant = 100f;

	private Vector3 distance {
		get{ return transform.position - player.transform.position; }
	}

	void Awake(){
		_collider =GetComponent<Collider2D> ();
		rb = GetComponent<Rigidbody2D>();
		if (_collider != null) {
			_collider.isTrigger = false;
		}

	}
	void FixedUpdate(){
		if (pulling) {
			PlayerControl p = player.GetComponent<PlayerControl> ();
			// IF YOU ARE SEEING AN ERROR HERE CHANGE PLAYERCONTROL NAME THING bajing
			Debug.Log("pulling");
			// NO JUMPING WHILE PULLING!!
			// p.isGrounded IS SUPER EXPENSIVE DON'T CALL IT!!!
			if (!p.isGrounded () ) { // Tachancak forgive me for this long line || player.transform.GetComponent<Rigidbody2D>().position.y <= rb.position.y || player != null 
				Debug.Log("apretly not groudned");
				pulling = false;
				return;
			}
			Vector2 x = (Vector2)(distance - equilibirum);
			Debug.Log (x);
			//Debug.Log (equilibirum);
			//Debug.Log (distance);
			if(distance.magnitude > equilibirum.magnitude){
				// we want to pull
				rb.velocity = -x*springConstant;
			//	Debug.Log ("Pulling wtih a force of " + (-x) * springConstant);
			} else if(distance.magnitude < equilibirum.magnitude) {
				// we fwant to push
			//Debug.Log ("Pushimg");
				rb.velocity = x*springConstant;
			}
		}	
	}
	// Update is called once per frame
	void Update () {
		if (touchingPlayer && Input.GetKeyDown(KeyCode.F) && player.GetComponent<Rigidbody2D>().position.y < rb.position.y) { //
			pulling = true;
			equilibirum = distance;

		} else if (!touchingPlayer && Input.GetKeyUp(KeyCode.F)){
			Debug.Log ("Not meeting condriions");
			pulling = false;
		}
		//Debug.Log ("Pulling condition");
		//Debug.Log (touchingPlayer&& Input.GetKey(KeyCode.F));
		//Debug.Log ("touchingplayer: ");
		//Debug.Log(touchingPlayer);
		//Debug.Log ("pulling: ");
	//	Debug.Log(pulling);	
	}

	void OnCollisionEnter2D(Collision2D thing){
	//	Debug.Log ("collided");
		if (thing.gameObject.tag == "Player") {
			touchingPlayer = true;
			player = thing.gameObject;
			Debug.Log("touchy");
		}
	}

	void OnCollisionExit2D(Collision2D other){
		touchingPlayer = false;
	}


}