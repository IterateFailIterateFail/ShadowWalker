using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Range(1f, 100f)]
    public float Max_distance = 10f;
    [Range(0.01f, 1f)]
    public float speed = 0.1f;
    [Range(0.00f, 360.00f)]
    public float degree = 0; // 0 is lefgt, 180 is right, 90 is up, 270 is down
    private float speed1;
    float distance = 1;
    bool direction = true; // true is forward, flase is backwards
    private GameObject player;
    private Vector2 pos;
    private Vector2 player_pos;
    private bool touchingPlayer = false;
    float rad;
    bool floating = false;
	bool buttons = false;
	public bool stich = false;
    private FloatingPlatform fp;
	private ButtonReciever bp;

    private void Awake(){
		fp = GetComponent<FloatingPlatform>(); // check if other scripts are present
		bp = GetComponent<ButtonReciever> ();
        if (fp != null){
            floating = true;
			//Debug.Log (floating);
        }
		if (bp == null) {
			stich = true;
		} else {
			buttons = true;
		}
    }

    void FixedUpdate(){
		if (buttons) { // if ButtonReciever is present check for tigger input
			if (bp.isTriggered ())
				stich = true;
			else
				stich = false;
		}
		if (stich) { 
			rad = degree * Mathf.Deg2Rad;
			speed1 = speed;
			if (direction == true) {
				speed1 = speed;
			} else {
				speed1 = speed * -1;
			}
			// Debug.Log(speed1);
			pos = transform.position; // wierd hacky think becasue aparently unity doesn't like people directly changing positons
			pos.x += speed1 * Mathf.Cos (rad);
			pos.y += speed1 * Mathf.Sin (rad);
			distance += speed1;
			if (distance >= Max_distance) { // honestly i'm not entirely sure why i checking distance like this put it works so don't touch it
				//     Debug.Log(direction);
				direction = false;
			} else if (distance <= 0) {
				direction = true;
				//     Debug.Log(direction);
			}
			transform.position = pos;
			//Debug.Log(transform.position);
			if (floating) { 
				Vector2 change_pos;
				change_pos.x = speed1 * Mathf.Cos (rad);
				change_pos.y = speed1 * Mathf.Sin (rad);
				fp.MoveEquilibriumBy (change_pos);
			}
			if (touchingPlayer) { // if player present, change positon of player
				player_pos = player.transform.position;
				player_pos.x += speed1 * Mathf.Cos (rad);
				player_pos.y += speed1 * Mathf.Sin (rad);
				player.transform.position = player_pos;
			}
		}
    }
	void OnCollisionEnter2D(Collision2D player) {
			Debug.Log ("collided");
		if (player.gameObject.tag == "Player") {
			touchingPlayer = true;
			this.player = player.gameObject;
		}
	}

	void OnCollisionExit2D(Collision2D other) {
		touchingPlayer = false;
		Debug.Log ("left");
	}
}


