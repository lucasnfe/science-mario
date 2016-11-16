using UnityEngine;
using System.Collections;

public class SMBPhysicalBody : MonoBehaviour {

	public bool applyGravity = true;

	public float mass = 1f;
	public Vector2 velocity;
	public Vector2 acceleration;

	// Use this for initialization
	protected virtual void Start () {

	}

	// Update is called once per frame
	protected virtual void Update () {

		// Apply acceleration
		if (applyGravity)
			ApplyForce (SMBConstants.gravity * Time.fixedDeltaTime);

		// Update velocity using currently acceleration
		velocity += acceleration;

		// Clamp velocity
		velocity.x = Mathf.Clamp (velocity.x, 
			-SMBConstants.maxVelocityX, SMBConstants.maxVelocityX);

		velocity.y = Mathf.Clamp (velocity.y, 
			-SMBConstants.maxVelocityY, SMBConstants.maxVelocityY);

		SendMessage ("CheckHorizontalCollision", SendMessageOptions.DontRequireReceiver);

		SendMessage ("CheckVerticalCollision", SendMessageOptions.DontRequireReceiver);

		// Update position using currently velocity
		transform.Translate(velocity * Time.fixedDeltaTime);

		// Reset acceleration
		acceleration = Vector2.zero;

	}

	public void ApplyForce(Vector2 force) {

		force /= mass;
		acceleration += force;
	}
}
