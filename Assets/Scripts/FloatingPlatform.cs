using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using D = UnityEngine.Debug;

// TODO: Flip equilibrium when gravityScale flips

/**
 * Ask me (Weicong) for clarification
 * 
 * Notes: 
 * 	Collision events will be sent to disabled MonoBehaviours, 
 * 	to allow enabling Behaviours in response to collisions.
 * 	Thus, can be disabled and enabled using GameObject.SetActive(bool)
 * 
 * 	Tracked object must have a Rigidbody2D component
 * 
 * 	Does not modify Transform.Position.x at all
 * 
 * 	Does not modify Transform.Position.y directly. Uses Physics2D internal engine for calculations.
 * 	However this script reads Rigidbody.y to calculate change in y due to the 
 * 	imprecision in using Rigidbody.velocity to calculate change. Other scripts that modify Transform.Position.y
 * 	may affect the workings of this script.
 * 
 * 	Implements Hooke's law and damping coefficients using SI units. 
 * 
 * 	Customise the feel of the platform by changing Rigidbody2D.mass, FloatingPlatform.springConstant, and/or
 * 	FloatingPlatform.dampingRatio. 
 * 
 * 	dampingRatio takes any value >= 0; (but clamped to 3 because there is no real reason to have > 3 unless
 *  you want it to essential crawl to equilibrium I suppose).
 * 	
 * 	Anything critically damped or overdamped (>=1) will not exhibit simple harmonic motion.
 * 	
 **/

[RequireComponent(typeof(Rigidbody2D),typeof(Collision2D))]
public class FloatingPlatform : MonoBehaviour {

	// Add the tags of objects to be detected via collision
	public List<string> trackedObjectTags;
	// Spring constant
	public float springConstant = 500f; // N/m
	[Range(0f,3f)]
	public float dampingRatio = 0.2f; // < 1 underdamped, > 1 overdamped
	// So that there is no initial bounce
	public bool startAtGravitationalEquilibrium = false;

	private List<Rigidbody2D> collisionObjects = new List<Rigidbody2D>();

	private float equilibrium;
	private Rigidbody2D selfRigidbody; // FloatingPlatform's rigidbody

	//private float previousVelocity;
	//private float previousTime;
	private float previousGravityScale;
	private Vector2 vectorEquilibriumMovedBy = new Vector2();

	private void OnEnable(){

		selfRigidbody = GetComponent<Rigidbody2D> ();
		if (selfRigidbody == null)
			throw new MissingComponentException (name + " missing Rigidbody2D component");

		equilibrium = selfRigidbody.position.y;

		if (startAtGravitationalEquilibrium)
			NormalizeEquilibrium ();

		previousGravityScale = selfRigidbody.gravityScale;

	}

	private void FixedUpdate(){

		/* http://hyperphysics.phy-astr.gsu.edu/hbase/oscda.html
		 * https://en.wikipedia.org/wiki/Damping_ratio
		 */

		float netMomentum = selfRigidbody.mass * selfRigidbody.velocity.y;
		float netAcceleration = 0f;
		float totalMass = selfRigidbody.mass;
		float totalWeight = selfRigidbody.mass * selfRigidbody.gravityScale;
		float criticalDamp;

		// Move equilibrium if necessary
		equilibrium += vectorEquilibriumMovedBy.y;
		vectorEquilibriumMovedBy.x = 0;
		vectorEquilibriumMovedBy.y = 0;

		// Start calculation of spring motion

		foreach(Rigidbody2D rb in collisionObjects){

			// p_final = Sum{i}(m_i * v_i)
			netMomentum += rb.mass * rb.velocity.y;
			totalMass += rb.mass;
			totalWeight += rb.mass * rb.gravityScale;
		}

		// a = (-k*x - W)/totalMass
		netAcceleration = -(springConstant*(selfRigidbody.position.y - equilibrium) + totalWeight)/totalMass;

		// c_critical^2 = 4*springConstant*mass
		criticalDamp = 2 * Mathf.Sqrt (springConstant * totalMass);

		// F_damp = -c * v
		// mass * acceleration_damp = -c * v
		netAcceleration += -criticalDamp * dampingRatio * selfRigidbody.velocity.y/totalMass;

		// v = u + a*t
		Vector2 additionalVelocity = new Vector2(
			selfRigidbody.velocity.x, 
			netAcceleration * Time.fixedDeltaTime
		);
		selfRigidbody.velocity += additionalVelocity;

		// Calculate expected change in y position


		// Check if gravityScale was changed from the previous frame
		CheckGravityOnChange ();

		// Set current frame's properties for next frame processing
		previousGravityScale = selfRigidbody.gravityScale;

	}

	private void OnCollisionEnter2D(Collision2D collision){

		// See if the collider has one of the tags we're looking for
		if(trackedObjectTags.Contains(collision.collider.tag)){

			Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D> ();

			if(rb == null){

				UnityEngine.Debug.LogError(collision.collider.name + " must have a RigidBody2D component.");
				return;
			}

			collisionObjects.Add (rb);

		}

	}

	private void OnCollisionExit2D(Collision2D collision){

		// Does not throw error if it doesn't exist so no need to check
		Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();
		if (rb != null) {
			collisionObjects.Remove (rb);
		}

	}

	/// <summary>
	/// Calculates the equilibrium offset from the current position
	/// </summary>
	/// <returns>The equilibrium offset</returns>
	private float CalculateEquilibriumOffset(){

		/**
		 * 		F	= -k*x
		 * 	 -m*g	= -k*x
	 	 * 		x 	= m*g/k 
		 * offset 	= -x
		 * 			= -m*g/k
		 */

		return -selfRigidbody.mass * Physics2D.gravity.y * selfRigidbody.gravityScale / springConstant;

	}

	/// <summary>
	/// Sets the equilibrium at the GameObject's current position
	/// </summary>
	public void NormalizeEquilibrium(){

		equilibrium += CalculateEquilibriumOffset ();

	}

	/// <summary>
	/// Sets the equilibrium at a specific position. The X position is not disregarded.
	/// </summary>
	/// <param name="position">new equilibrium position</param>
	/// <param name="teleportToNewPosition">automatically teleport the platform to equilibrium without any motion</param>
	public void SetEquilibrium(Vector2 position, bool teleportToNewPosition = false){
		
		equilibrium = position.y;

		if (teleportToNewPosition)
			gameObject.transform.position = position + new Vector2 (0f, CalculateEquilibriumOffset ());
		
	}

	/// <summary>
	/// Moves the equilibrium by a certain vector distance
	/// </summary>
	/// <param name="vector">Vector by which the equilibrium will be moved by</param>
	public void MoveEquilibriumBy(Vector2 vector){
		vectorEquilibriumMovedBy += vector;
	}

	/// <summary>
	/// Sets the gravity scale. Use this function instead of Rigidbody2D.gravityScale if you want to
	/// ensure that the equilibrium is correct for that gravity scale (roughly)
	/// </summary>
	/// <param name="gravityScale">Gravity scale.</param>
	[System.Obsolete]
	public void SetGravityScale(float gravityScale){

		/**
		 * Checks to see if the gravity has been flipped
		 * Then sets the new gravityScale on Rigidbody2D
		 * Then if the gravityScale was flipped, a new equilibrium is set using the new gravityScale
		 * 
		 * Note: I got lazy and couldn't be bothered working out how to incorporate velocity and current
		 * position to correctly work out the actual position of the FloatingPlatform as if it were placed 
		 * in the unity editor or changed through Transform.position
		 */

		bool gravityFlipped = false;

		if( Mathf.RoundToInt(gravityScale / Mathf.Abs(gravityScale)) != 
			Mathf.RoundToInt(selfRigidbody.gravityScale / Mathf.Abs(selfRigidbody.gravityScale)) ){
			// Gravity flipped
			gravityFlipped = true;
		}

		selfRigidbody.gravityScale = gravityScale;

		if (gravityFlipped){
			equilibrium += 2 * CalculateEquilibriumOffset ();
		}

		previousGravityScale = gravityScale;

	}

	/// <summary>
	/// Checks whether the gravity scale was change (At the moment only checks if the gravity is flipped
	/// </summary>
	// TODO: Make gravity scale onChange not just flipped
	private void CheckGravityOnChange(){
		bool gravityFlipped = false;

		/**
		 * Checks to see if the gravity has been flipped
		 * Then sets the new gravityScale on Rigidbody2D
		 * Then if the gravityScale was flipped, a new equilibrium is set using the new gravityScale
		 * 
		 * Note: I got lazy and couldn't be bothered working out how to incorporate velocity and current
		 * position to correctly work out the actual position of the FloatingPlatform as if it were placed 
		 * in the unity editor or changed through Transform.position
		 */

		if( Mathf.RoundToInt(previousGravityScale / Mathf.Abs(previousGravityScale)) != 
			Mathf.RoundToInt(selfRigidbody.gravityScale / Mathf.Abs(selfRigidbody.gravityScale)) ){
			// Gravity flipped
			gravityFlipped = true;
		}
			
		if (gravityFlipped){
			equilibrium += 2 * CalculateEquilibriumOffset ();
		}
		
	}

}
