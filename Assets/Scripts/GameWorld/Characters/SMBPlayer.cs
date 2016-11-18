using UnityEngine;
using System.Collections;

public class SMBPlayer : SMBCharacter {

	enum SoundEffects {
		Jump,
		Kick
	}
		
	private float _jumpTimer;

	private SMBConstants.PlayerState _state;
	public SMBConstants.PlayerState State { get { return _state; } }

	public float longJumpTime = 1f;
	public float longJumpWeight = 0.1f;
	public float runningMultiplyer = 2f;

	public AudioClip[] soundEffects;

	void Start() {

		_state = SMBConstants.PlayerState.Short;
	}

	// Update is called once per frame
	void Update () {

		if (_state == SMBConstants.PlayerState.Dead)
			return;

		Jump ();
		_animator.SetBool ("isJumping", !_isOnGround);

		float speed = xSpeed;
		if (Input.GetKey (KeyCode.A))
			speed *= runningMultiplyer;

		if (Input.GetKey (KeyCode.LeftArrow)) {

			Move (speed * (float)SMBConstants.MoveDirection.Backward);

			if (Mathf.Abs (_body.velocity.x) > 0f) {

				_animator.SetBool ("isMoving", true);
				_animator.SetBool ("isRunning", false);
			}

			if(Mathf.Abs(_body.velocity.x) > 1.3f)
				_animator.SetBool ("isRunning", true);

			if(Mathf.Abs(_body.velocity.x) > 0.5f && Mathf.Sign(_body.velocity.x) == 1f)
				_animator.SetBool ("isCoasting", true);

		} 
		else if (Input.GetKey (KeyCode.RightArrow)) {

			Move (speed * (float)SMBConstants.MoveDirection.Forward);

			if (Mathf.Abs (_body.velocity.x) > 0f) {

				_animator.SetBool ("isMoving", true);
				_animator.SetBool ("isRunning", false);
			}

			if(Mathf.Abs(_body.velocity.x) > 1.3f)
				_animator.SetBool ("isRunning", true);

			if(Mathf.Abs(_body.velocity.x) > 0.5f && Mathf.Sign(_body.velocity.x) == -1f)
				_animator.SetBool ("isCoasting", true);
		} 
		else {

			_body.velocity.x = Mathf.Lerp (_body.velocity.x, 0f, momentum * Time.fixedDeltaTime);

			if (Mathf.Abs (_body.velocity.x) < 1.3f)
				_animator.SetBool ("isRunning", false);

			if (Mathf.Abs (_body.velocity.x) <= 0.1f) {

				_animator.SetBool ("isMoving", false);
				_body.velocity.x = 0f;
			}
		}

		if (Mathf.Abs (_body.velocity.x) <= 0.1f && _animator.GetBool("isCoasting"))
			_animator.SetBool ("isCoasting", false);

		if (transform.position.y < 0f)
			Die (0.4f);
	}

	void Die(float timeToDie) {

		_state = SMBConstants.PlayerState.Dead;

		_collider.applyHorizCollision = false;
		_collider.applyVertCollision = false;

		gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");

		_body.velocity = Vector2.zero;
		_body.acceleration = Vector2.zero;
		_body.applyGravity = false;

		_animator.SetBool ("isRunning", false);
		_animator.SetBool ("isMoving", false);
		_animator.SetBool ("isJumping", true);

		Invoke("PlayDeadAnimation", timeToDie);
	}

	void PlayDeadAnimation() {

		_body.applyGravity = true;
		_body.gravityFactor = 0.5f;
		_body.ApplyForce (Vector2.up * 2.5f);
	}
				
	void Jump() {

		if (_isOnGround && Input.GetKeyDown(KeyCode.S)){

			_jumpTimer = longJumpTime;
			_body.velocity.y = ySpeed * Time.fixedDeltaTime;

			_audio.PlayOneShot (soundEffects[(int)SoundEffects.Jump]);
		}

		if (_jumpTimer > 0f) {

			if (Input.GetKeyUp(KeyCode.S)) {

				_jumpTimer = 0f;

			}
			else if(_body.velocity.y > 0f && Input.GetKey(KeyCode.S)) {

				_jumpTimer -= Time.fixedDeltaTime;
				if (_jumpTimer <= longJumpTime/2f)
					_body.velocity.y += ySpeed * longJumpWeight * Time.fixedDeltaTime;
			}
		}
	}


	override protected void OnHalfVerticalCollisionEnter(Collider2D collider) {

		if (collider.tag == "Enemy") {

			_body.acceleration = Vector2.zero;

			_body.ApplyForce (Vector2.up * 2.5f);
			_audio.PlayOneShot (soundEffects[(int)SoundEffects.Kick]);

			collider.gameObject.SendMessage ("Die", SendMessageOptions.DontRequireReceiver);

			return;
		}

		base.OnHalfVerticalCollisionEnter (collider);
	}


	override protected void OnFullVerticalCollisionEnter(Collider2D collider) {

		if (collider.tag == "Enemy") {

			_body.acceleration = Vector2.zero;

			_body.ApplyForce (Vector2.up * 2.5f);
			_audio.PlayOneShot (soundEffects[(int)SoundEffects.Kick]);

			collider.gameObject.SendMessage ("Die", SendMessageOptions.DontRequireReceiver);

			return;
		}

		base.OnFullVerticalCollisionEnter (collider);
	}

	void OnHorizontalCollisionEnter(Collider2D collider) {

		float dist = Mathf.Abs (collider.bounds.center.y - transform.position.y);

		if (collider.tag == "Enemy" && dist < 0.05f)
			Die (0.2f);
	}
}
