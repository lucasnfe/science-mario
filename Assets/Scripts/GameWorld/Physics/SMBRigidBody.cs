using UnityEngine;
using System.Collections;

public class SMBRigidBody : MonoBehaviour {

	private float side;
	public float Side { get { return side; }}

	public bool applyGravity = true;

	public float mass = 1f;
	public float gravityFactor = 1f;

	public Vector2 velocity;
	public Vector2 acceleration;

	// Use this for initialization
	protected virtual void Start () {

	}

	// Update is called once per frame
	protected virtual void Update () {

		// Apply acceleration
		if (applyGravity)
			ApplyForce (SMBConstants.gravity * gravityFactor * Time.fixedDeltaTime);

		// Update velocity using currently acceleration
		velocity += acceleration;

		// Clamp velocity
		velocity.x = Mathf.Clamp (velocity.x, 
			-SMBConstants.maxVelocityX, SMBConstants.maxVelocityX);

		velocity.y = Mathf.Clamp (velocity.y, 
			-SMBConstants.maxVelocityY, SMBConstants.maxVelocityY);

		if(velocity.x != 0f)
			side = Mathf.Sign (velocity.x);

		// Update position using currently velocity
		transform.Translate(velocity * Time.fixedDeltaTime);

		// Reset acceleration
		acceleration = Vector2.zero;

	}

	public void ApplyForce(Vector2 force) {

		if(mass != 0f)
			force /= mass;
		
		acceleration += force;
	}
}
