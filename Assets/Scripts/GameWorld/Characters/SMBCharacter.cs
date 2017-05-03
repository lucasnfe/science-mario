using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]
[RequireComponent (typeof (SMBRigidBody))]
[RequireComponent (typeof (SMBCollider))]
[RequireComponent (typeof (Animator))]
[RequireComponent (typeof (SpriteRenderer))]
public class SMBCharacter : MonoBehaviour {

	protected bool  _isOnGround;

	// Custom components
	protected AudioSource     _audio;
	protected Animator        _animator;
	protected SpriteRenderer  _renderer;
	protected SMBRigidBody    _body;
	protected SMBCollider     _collider;

	public float xSpeed = 1f;
	public float ySpeed = 5f;
	public float momentum = 3f;
	public bool  lockXPosition = true;

	public SMBConstants.MoveDirection startDirection;

	virtual protected void Awake() {

		_audio    = GetComponent<AudioSource> ();
		_body     = GetComponent<SMBRigidBody> ();
		_collider = GetComponent<SMBCollider> ();
		_animator = GetComponent<Animator> ();
		_renderer = GetComponent<SpriteRenderer> ();
	}

	virtual protected void Start() {

		if (startDirection == SMBConstants.MoveDirection.Backward)
			_renderer.flipX = true;
	}

	protected void Move(float speed) {

		_body.velocity.x = Mathf.Lerp (_body.velocity.x, speed * Time.fixedDeltaTime, 
			momentum * Time.fixedDeltaTime);

		float side = Mathf.Sign (speed);

		if (side == (float)SMBConstants.MoveDirection.Forward)
			_renderer.flipX = false;

		if (side == (float)SMBConstants.MoveDirection.Backward)
			_renderer.flipX = true;
	}

	virtual protected void Update() {

		if (lockXPosition) {
			
			Vector3 playerPos = transform.position;
			playerPos.x = Mathf.Clamp (playerPos.x, SMBGameWorld.Instance.LockLeftX - SMBGameWorld.Instance.TileSize, 
				SMBGameWorld.Instance.LockRightX);
			transform.position = playerPos;
		}

		// Check if mario is at the bottom of the screen
		float levelWorldWidth = SMBGameWorld.Instance.Level.GetLength(1) * SMBGameWorld.Instance.TileSize + 0.5f;
		if (transform.position.y < -0.5f || transform.position.x < -0.5f || transform.position.x > levelWorldWidth)
			DestroyCharacter ();
	}

	virtual protected void DestroyCharacter() {

		Destroy (gameObject);
	}

	virtual protected void OnHorizontalCollisionEnter(Collider2D collider) {

	}

	virtual protected void OnVerticalCollisionEnter(Collider2D collider) {

		float yDirection = _body.velocity.y > 0f ? 1f : -1f;
		if(yDirection == -1f)
			_isOnGround = true;
	}

	virtual protected void OnVerticalCollisionExit() {

		_isOnGround = false;
	}

	virtual protected void OnPauseGame() {

		_body.enabled = false;
		_collider.enabled = false;
		_animator.enabled = false;
	}

	virtual protected void OnResumeGame() {

		_body.enabled = true;
		_collider.enabled = true;
		_animator.enabled = true;
	}

	public bool isFlipped() {

		return _renderer.flipX;
	}
}
