using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Animator))]
[RequireComponent (typeof (SpriteRenderer))]
[RequireComponent (typeof (AudioSource))]
[RequireComponent (typeof (SMBRigidBody))]
[RequireComponent (typeof (SMBCollider))]
public class SMBPlayer : MonoBehaviour {

	enum SoundEffects {
		Jump
	}

	// Custom components
	private AudioSource     _audio;
	private Animator        _animator;
	private SpriteRenderer  _renderer;
	private SMBRigidBody    _body;
	private SMBCollider     _collider;
		
	private float _jumpTimer;
	private bool  _isOnGround;

	private SMBConstants.PlayerState _state;
	public SMBConstants.PlayerState State { get { return _state; } }

	public float xSpeed = 1f;
	public float ySpeed = 5f;
	public float runningMultiplyer = 2f;
	public float longJumpTime = 1f;
	public float longJumpWeight = 0.1f;
	public float momentumReduction = 3f;

	public AudioClip[] soundEffects;

	void Awake() {

		_audio    = GetComponent<AudioSource> ();
		_body     = GetComponent<SMBRigidBody> ();
		_collider = GetComponent<SMBCollider> ();
		_animator = GetComponent<Animator> ();
		_renderer = GetComponent<SpriteRenderer> ();
	}

	void Start() {

		_state = SMBConstants.PlayerState.Short;
	}

	// Update is called once per frame
	void Update () {

		if (_state == SMBConstants.PlayerState.Dead)
			return;

		Jump ();
		_animator.SetBool ("isJumping", !_isOnGround);

		if (Input.GetKey (KeyCode.LeftArrow)) {

			Move ((float)SMBConstants.MoveDirection.Backward);

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

			Move ((float)SMBConstants.MoveDirection.Forward);

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

			_body.velocity.x = Mathf.Lerp (_body.velocity.x, 0f, momentumReduction * Time.fixedDeltaTime);

			if (Mathf.Abs (_body.velocity.x) < 1.3f)
				_animator.SetBool ("isRunning", false);

			if (Mathf.Abs (_body.velocity.x) <= 0.1f) {

				_animator.SetBool ("isMoving", false);
				_body.velocity.x = 0f;
			}
		}

		if (Mathf.Abs (_body.velocity.x) <= 0.1f && _animator.GetBool("isCoasting"))
			_animator.SetBool ("isCoasting", false);


		if (transform.position.y < 0f) {

			_state = SMBConstants.PlayerState.Dead;

			_collider.applyHorizCollision = false;
			_collider.applyVertCollision = false;

			_body.velocity = Vector2.zero;
			_body.applyGravity = false;

			Invoke("Die", 0.4f);
		}
	}

	void Die() {

		_body.applyGravity = true;
		_body.gravityFactor = 0.5f;
		_body.ApplyForce (Vector2.up * 2.5f);
	}

	void OnVerticalCollisionEnter() {

		if(Mathf.Sign(_body.velocity.y) == -1f)
			_isOnGround = true;
	}

	void OnVerticalCollisionExit() {

		_isOnGround = false;
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

	void Move(float side) {

		float speed = xSpeed * side;
		if (Input.GetKey (KeyCode.A))
			speed *= runningMultiplyer;

		_body.velocity.x = Mathf.Lerp (_body.velocity.x, speed * Time.fixedDeltaTime, 
			momentumReduction * Time.fixedDeltaTime);

		if (side == (float)SMBConstants.MoveDirection.Forward)
			_renderer.flipX = false;

		if (side == (float)SMBConstants.MoveDirection.Backward)
			_renderer.flipX = true;

		// Lock player x position
		Vector3 playerPos = transform.position;
		playerPos.x = Mathf.Clamp (playerPos.x, SMBGameWorld.Instance.LockLeftX - SMBGameWorld.Instance.TileSize, 
			SMBGameWorld.Instance.LockRightX);
		transform.position = playerPos;
	}

	public bool isFlipped() {

		return _renderer.flipX;
	}
}
