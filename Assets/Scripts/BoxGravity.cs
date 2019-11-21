using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxGravity : MonoBehaviour {
    Rigidbody2D playerBody;
    Rigidbody2D boxBody;

    GameObject player;
    bool touchingPlayer;
    bool pulling;


    // Use this for initialization
    void Start () {
        boxBody = GetComponent<Rigidbody2D>();
        playerBody = null;
    }

    
    // Update is called once per frame
    void Update(){
		if (touchingPlayer && canUsePortal() && Input.GetKeyDown("space")){	//
			boxBody.gravityScale *= -1.0f;
            pulling = true;
        }

    }

    void OnCollisionEnter2D(Collision2D player){
        //	Debug.Log ("collided");
        if (player.gameObject.tag == "Player"){
            touchingPlayer = true;
            this.player = player.gameObject;
            playerBody = player.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    void OnCollisionExit2D(Collision2D other){
        touchingPlayer = false;
    }
	// Returns true if we are standing over a portal, false otherwise
	bool canUsePortal() {
		// Find all objects that overlap a circle, centered on us, radius of 1.1
		// NOTE: Radius might need tweaking
		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1.1f);
		for (int i = 0; i < hitColliders.Length; i++) {
			if (hitColliders[i].tag == "Portal") return true;
		}
		return false;
	}
}
