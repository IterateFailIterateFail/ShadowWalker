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

	// The maximum horizontal speed that the player may move at
	public float maxSpeed = 5f;
	// How fast the player accelerates when you try to walk
	public float acceleration = 0.5f;
	// How quickly the player slows down, while on the ground, should be between 0 and 1
	// larger numbers mean you slow slown LESS
	public float friction = 0.9f;
	// How quickly the player slows down, while in the air
	public float friction_air = 0f;
	// The upwards speed when you jump, large means a higher jump DARREN:Has no effect to jump velocity
	public float jumpSpeed = 500f;
	// If the horisontal speed is less than this, the player will come to a complete standstill
	// This is used to stop the player from gradually sliding down gentle slopes
	public float movementStopThreshold = 0.5f;

	public Vector2 checkpointLocation;
	// The last point at which the player was touching
	public float checkpointGravity;
	//The gravity at last checkpoint
	public float checkpointRotaion;
	//rotaion of player at last checkpoint
	float prevGrav;

	bool canPortal = true;
	// part of the cooldown for portal use
	float time = 0;
	// Used to determine what objects should be considered as 'ground'
	[SerializeField] private LayerMask whatIsGround;
	// The player's rigidbody
	private Rigidbody2D rigidBody;
	// Track whether we are travelling in a portal
	private bool inPortalTravel = false;
	//Tack whether we are travelling on a ladder
	public bool inLadder = false;
	// speed at which the stumble animation plays
	[SerializeField] float warningSpeed;
	// speed at which you just die
	[SerializeField] float deadlySpeed;
	private int deathMark = 0;
	// flag for mark for height death, 0 is standard, 1 is warnng and 2 is dead
	SpriteRenderer sprite;
	// Spriet transform
	Animator animator;
	//annimator
	[SerializeField]RuntimeAnimatorController[] anim;
	// Place to store animation controllers for certain events like runnign and jumping... 
	[SerializeField] Sprite idle;

    // Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody2D>();
		moverPrefab = Resources.Load("MovePlayerToStart", typeof(GameObject)) as GameObject;
		prevGrav = rigidBody.gravityScale;
		sprite = transform.Find ("shadow kid_character").gameObject.GetComponent<SpriteRenderer>();
		animator = transform.Find ("shadow kid_character").gameObject.GetComponent<Animator>();

	}

	// FixedUpdate is called once per physics step regardless of framerate
	void FixedUpdate () {
		// Grab this once at the start, modify only this, and set it again at the end.
		Vector2 velocity = rigidBody.velocity;
		Vector2 position = rigidBody.position;

		// Accelerate horizontally as we walk, but not if we're in a portal
		if ((!inPortalTravel || !inLadder )) { //&& isGrounded()
			float moveHorizontal = Input.GetAxis("Horizontal");
			velocity.x += moveHorizontal * acceleration;
		}
		// if in ladder, don't move left or right unless you are on the ground
		if(inLadder){
			velocity.x = 0;
			velocity.y = 0;
			float climbLadder = Input.GetAxis("Vertical");
			velocity.y += climbLadder * acceleration * 2;
			rigidBody.gravityScale = 0;
		}

		if (isGrounded()) {
			// Apply horizontal friction
			velocity.x *= friction;
			// We are on the ground, therefore we are not in a portal
			inPortalTravel = false;

		} else if (inPortalTravel) {
			velocity.x *= 0;
			velocity.y = Mathf.Clamp (velocity.y, -30, 30); 
		} else {
			velocity.x *= friction_air;
		}
		float mag = Mathf.Abs(velocity.y);
		//Debug.Log (mag);
		if (mag > warningSpeed) deathMark = 1; // if fall at speed at warning, trigger warning
		if (mag > deadlySpeed) deathMark = 2; // if fall at speed at death, mark for death

		if(isGrounded() && deathMark == 1){
			//Debug.Log ("Warning");
			deathMark = 0;
		}
		if(isGrounded() && deathMark == 2){
			deathMark = 0;
			die();
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}						

		// If we are not trying to walk on the ground, and we are close to being stopped, stop
		if (Input.GetAxis("Horizontal") == 0 && velocity.magnitude < movementStopThreshold  && isGrounded()){   
			velocity.x = 0;
			velocity.y = 0;
		}

		//Limits speed of Player, so Player doesn't fly across screen.
		velocity.x = Mathf.Clamp(velocity.x,  -maxSpeed, maxSpeed);
		if (velocity.x != 0 && isGrounded() && velocity.y == 0) {
			animator.runtimeAnimatorController = anim [0];
			animator.enabled = true;

		} else if (velocity == Vector2.zero && isGrounded()) {
			//Debug.Log ("idling");
			sprite.sprite = idle; 
			animator.enabled = false;
		}
		Debug.Log (velocity);
		// Assign the modified velocit
		rigidBody.velocity = velocity;
		rigidBody.position = position;


	}

	// Update is called once per frame
	// Handle the keypresses here because the functions will return 'true' for a frame,
	// not for a physics step
	void Update() {
		//Debug.Log(canUseLadder());
		//Debug.Log (inLadder);
		//Vector2 vel = rigidBody.velocity; // velcoity was taken for soem reason
		//Debug.Log (inPortalTravel);
		// When the player tries to flip gravity, only do so if we are over a portal
		if (Input.GetKeyDown("space") && canUsePortal() && canPortal && isGrounded()) {
			rigidBody.rotation += 180;
			rigidBody.gravityScale *= -1;
			prevGrav = rigidBody.gravityScale;
			flipSprite ();
			//vel.x = 0;
			// In 0.1 seconds, mark that we are in a portal
			// can't do it immidiately because something ends up un-settings it
			Invoke("YesWeAreInAPortalThanks", 0.1f);
			canPortal = false;

		}
		// When the player tries to jump, jump if we are on the ground
		if ((Input.GetKeyDown("up") || Input.GetKeyDown("space") )&& isGrounded()){
			animator.enabled = true;
			animator.runtimeAnimatorController = anim [1];
			Vector2 velocity = rigidBody.velocity;
			velocity.y = jumpSpeed * rigidBody.gravityScale;
			Debug.Log (jumpSpeed * rigidBody.gravityScale);
			rigidBody.velocity = velocity;

			//Debug.Log (inPortalTravel);
		}
		// if player is in ladder and uses up or down, stick to ladder
		if ((Input.GetKeyDown ("up") || Input.GetKeyDown ("down")) && canUseLadder ()) {
			inLadder = true;

		}
		if ((Input.GetKeyDown ("right") || Input.GetKeyDown ("left")) && inLadder && isGrounded ()) {
			inLadder = false;
			rigidBody.gravityScale = prevGrav;
		}
		if (!canUseLadder ()) {
			inLadder = false;
			rigidBody.gravityScale = prevGrav;
		}
		// cooldown on portal
		if (!canPortal) {
			time++;
			if (time >= 80) {
				canPortal = true;
				time = 0;
			}
		}

		// rotaion of sprite when changng direction
		if (Input.GetKeyDown ("left") && sprite.flipX && !isUpsideDown())  flipSprite();
		else if (Input.GetKeyDown ("right") && !sprite.flipX && !isUpsideDown())  flipSprite();
		else if (Input.GetKeyDown ("left") && !sprite.flipX && isUpsideDown())  flipSprite();
		else if (Input.GetKeyDown ("right") && sprite.flipX && isUpsideDown())  flipSprite();

		//rigidBody.velocity = vel; 

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
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPoint.position, 1.3f, whatIsGround);
		//UnityEngine.Debug.DrawLine(groundCheckPoint,groundCheckPoint + Vector3.right*0.5f, Color.red);
		// Return true if there is a least one
		return colliders.Length > 0;
	}

	/*void OnDrawGizmos() { groudnpoint debugger
		Transform groundCheckPoint = transform.Find("GroundedPoint");
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(groundCheckPoint.position, 0.5f);
	}
	*/
	public bool isUpsideDown() {
		return prevGrav < 0;
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
	// returns true if we are standing on or in a ladder, false otherwise
	bool canUseLadder() {
		// Get a point in the bottom middle of the hitbox
		Transform groundCheckPoint = transform.Find("GroundedPoint");
		// Find all objects that overlap a circle, centered on us, radius of 1.1
		// NOTE: Radius might need tweaking
		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(groundCheckPoint.position, 1.3f);
		for (int i = 0; i < hitColliders.Length; i++) {
			if (hitColliders[i].tag == "Ladder") return true;
		}
		return false;
	}

    public void die() {
		Debug.Log("PlayerControl.die()");
		Debug.Log("before die");
		Debug.Log(checkpointGravity);
		GameObject mover = Instantiate(moverPrefab);
		MovePlayer component = mover.GetComponent<MovePlayer>();

		component.targetLocation = checkpointLocation;
		component.prevGravity = checkpointGravity;
		component.prevRotation = checkpointRotaion;
	}
	void flipSprite(){
		if (!sprite.flipX) sprite.flipX = true;
		else sprite.flipX = false;
	}

}
