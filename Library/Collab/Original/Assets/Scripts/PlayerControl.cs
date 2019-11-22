using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//TODO: Define key variables for easy tweaking in the Unity Editor

// The controller for the main player, handles user input and interaction
// with various objects. This will probably become a very large script.
public class PlayerControl : MonoBehaviour {

	// The prefab of the object used to move the player to the checkpoint
	// location when the level gets reloaded after the player dies
	static public GameObject moverPrefab;

	// The maximum hirsontal speed that the player may move at
	public float maxSpeed = 5f;
	// How fast the player accelerates when you try to walk
	public float acceleration = 0.5f;
	// How quickly the player slows down, while on the ground, should be between 0 and 1
	// larger numbers mean you slow slown LESS
	public float friction = 0.9f;
	// How quickly the player slows down, while in the air
	public float friction_air = 0.95f;
	// The upwards speed when you jump, large means a higher jump
	public float jumpSpeed = 5f;
	// If the horisontal speed is less than this, the player will come to a complete standstill
	// This is used to stop the player from gradually sliding down gentle slopes
	public float movementStopThreshold = 0.5f;

	public Vector2 checkpointLocation;
	// The last point at which the player was touching

	// Used to determine what objects should be considered as 'ground'
	[SerializeField] private LayerMask whatIsGround;
	// The player's rigidbody
	private Rigidbody2D rigidBody;
	// Track whether we are travelling in a portal
	private bool inPortalTravel = false;


    // Use this for initialization
    void Start () {
		rigidBody = GetComponent<Rigidbody2D>();
		moverPrefab = Resources.Load("MovePlayerToStart", typeof(GameObject)) as GameObject;
	}

	// FixedUpdate is called once per physics step regardless of framerate
	void FixedUpdate () {
		// Grab this once at the start, modify only this, and set it again at the end.
		Vector2 velocity = rigidBody.velocity;

		// Accelerate horizontally as we walk, but not if we're in a portal
		if (!inPortalTravel) {
			float moveHorizontal = Input.GetAxis("Horizontal");
			velocity.x += moveHorizontal * acceleration;
		}

		if (isGrounded()) {
			// Apply horizontal friction
			velocity.x *= friction;
			// We are on the ground, therefore we are not in a portal
			inPortalTravel = false;
			float mag = Mathf.Abs(velocity.y);
			//Debug.Log(mag);
			if (mag > 50) {
				Debug.Log("Hit ground at speed");
				Debug.Log(mag);
				if (mag > 75) {
					// Hit the ground reeeeealy hard, die
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				}
			}
		} else if (inPortalTravel) {
			velocity.x *= friction;
		} else {
			velocity.x *= friction_air;
		}

		// If we are not trying to walk, and we are close to being stopped, stop
		if (Input.GetAxis("Horizontal") == 0 && Mathf.Abs(velocity.x) < movementStopThreshold) {
			velocity.x = 0;
		}

		//Limits speed of Player, so Player doesn't fly across screen.
		velocity.x = Mathf.Clamp(velocity.x,  -maxSpeed, maxSpeed);

		// Assign the modified velocity
		rigidBody.velocity = velocity;

	}

	// Update is called once per frame
	// Handle the keypresses here because the functions will return 'true' for a frame,
	// not for a physics step
	void Update() {

		// When the player tries to flip gravity, only do so if we are over a portal
		if (Input.GetKeyDown("space") && canUsePortal() ) {
			rigidBody.rotation += 180;
			rigidBody.gravityScale *= -1;
			// In 0.1 seconds, mark that we are in a portal
			// can't do it immidiately because something ends up un-settings it
			Invoke("YesWeAreInAPortalThanks", 0.1f);
		}

		// When the player tries to jump, jump if we are on the ground
		if (Input.GetKeyDown("up") && isGrounded()) {
			Vector2 velocity = rigidBody.velocity;
			velocity.y = jumpSpeed * rigidBody.gravityScale;
			rigidBody.velocity = velocity;
		}

	}

	// Yep. We are in a portal
	void YesWeAreInAPortalThanks() {
		inPortalTravel = true;
	}

	// Checks if we are standing on ground
	public bool isGrounded() {
		// Get a point in the bottom middle of the hitbox
		Transform groundCheckPoint = transform.Find("GroundedPoint");
		// Find all ground objects that overlap a circle centered at the groundpoint, radius 0.5
		// NOTE: Radius might not be correct
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPoint.position, 0.5f, whatIsGround);
		// Return true if there is a least one
		return colliders.Length > 0;
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

    public void die() {
		Debug.Log("PlayerControl.die()");
		GameObject mover = Instantiate(moverPrefab);
		MovePlayer component = mover.GetComponent<MovePlayer>();
		component.targetLocation = checkpointLocation;
	}

}
