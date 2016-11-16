using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Animator))]
[RequireComponent (typeof (BoxCollider2D))]
[RequireComponent (typeof (SpriteRenderer))]
[RequireComponent (typeof (AudioSource))]
[RequireComponent (typeof (SMBPhysicalBody))]
public class SMBPlayer : MonoBehaviour {

	enum SoundEffects {
		Jump
	}
		
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

	// Custom components
	private AudioSource    _audio;
	private Animator       _animator;
	private BoxCollider2D  _collider;
	private SpriteRenderer _renderer;
	private SMBPhysicalBody   _body;

	void Awake() {

		_animator = GetComponent<Animator> ();
		_collider = GetComponent<BoxCollider2D> ();
		_renderer = GetComponent<SpriteRenderer> ();
		_audio    = GetComponent<AudioSource> ();
		_body     = GetComponent<SMBPhysicalBody> ();
	}

	void Start() {

		_state = SMBConstants.PlayerState.Short;
	}

	// Update is called once per frame
	void Update () {

		Jump ();
		_animator.SetBool ("isJumping", !_isOnGround);

		if (Input.GetKey (KeyCode.LeftArrow)) {

			Move ((float)SMBConstants.MoveDirection.Backward);

			if (Mathf.Abs (_body.velocity.x) > 0f) {

				_animator.SetBool ("isMoving", true);
				_animator.SetBool ("isRunning", false);
			}

			if(Mathf.Abs(_body.velocity.x) > 1.25f)
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

			if(Mathf.Abs(_body.velocity.x) > 1.25f)
				_animator.SetBool ("isRunning", true);

			if(Mathf.Abs(_body.velocity.x) > 0.5f && Mathf.Sign(_body.velocity.x) == -1f)
				_animator.SetBool ("isCoasting", true);
		} 
		else {

			_body.velocity.x = Mathf.Lerp (_body.velocity.x, 0f, momentumReduction * Time.fixedDeltaTime);

			if (Mathf.Abs (_body.velocity.x) < 1.25f)
				_animator.SetBool ("isRunning", false);

			if (Mathf.Abs (_body.velocity.x) <= 0.1f) {

				_animator.SetBool ("isMoving", false);
				_body.velocity.x = 0f;
			}
		}

		if (Mathf.Abs (_body.velocity.x) <= 0.1f && _animator.GetBool("isCoasting"))
			_animator.SetBool ("isCoasting", false);

	}

	void CheckHorizontalCollision() {

		float xDirection = Mathf.Sign(_body.velocity.x);
		Vector2 xRayOrigin = (xDirection == 1f) ? _collider.bounds.max : 
			_collider.bounds.max - Vector3.right * _collider.bounds.size.x;

		xRayOrigin.y -= 0.1f;

		for (int i = 0; i < 2; i++) {

			RaycastHit2D xRay = Physics2D.Raycast (xRayOrigin, Vector2.right * xDirection, 0.01f);
			//Debug.DrawRay (xRayOrigin, Vector2.right * xDirection);
			if (xRay.collider) {

				// Check if the collision was agains an interactable object
				GameObject obj = xRay.collider.gameObject;
				obj.SendMessage ("OnInteraction", _collider, SendMessageOptions.DontRequireReceiver);

				if (xRay.collider.isTrigger)
					return;

				string tileID = xRay.collider.name;
				if(IsOneWayHorizontalCollision(xDirection, tileID))
					return;

				// Player collided on x axis, so stop it
				_body.velocity.x = 0f;

				// Fix player position after collision
				float colBound = (xDirection == 1f) ? xRay.collider.bounds.min.x : xRay.collider.bounds.max.x;

				if (xRayOrigin.x - colBound < 0.01f) {

					Vector3 currentPos = transform.position;
					currentPos.x = colBound + _collider.bounds.extents.x * -xDirection;
					transform.position = currentPos;
				}

				break;
			}

			xRayOrigin.y -= _collider.bounds.size.y - 0.2f;
		}
	}

	void CheckVerticalCollision() {

		float yDirection = Mathf.Sign (_body.velocity.y);
		Vector2 yRayOrigin = (yDirection == 1f) ? _collider.bounds.max :
			_collider.bounds.max - Vector3.up * _collider.bounds.size.y;

		yRayOrigin.x -= 0.01f;

		for (int i = 0; i < 2; i++) {

			RaycastHit2D yRay = Physics2D.Raycast(yRayOrigin, Vector2.up * yDirection, 0.01f);
			//Debug.DrawRay (yRayOrigin, Vector2.up * yDirection);

			if (yRay.collider) {

				// Check if the collision was agains an interactable object
				GameObject obj = yRay.collider.gameObject;
				obj.SendMessage ("OnInteraction", _collider, SendMessageOptions.DontRequireReceiver);

				if (yRay.collider.isTrigger)
					return;

				string tileID = yRay.collider.name;
				if(IsOneWayVerticalCollision(yDirection, tileID))
					return;

				// Player collided on y axis, so stop it
				_body.velocity.y = 0f;

				// If the velocity was negative, player collided with the ground
				if(yDirection == -1f)
					_isOnGround = true;

				// Fix player position after collision
				float colBound = (yDirection == 1f) ? yRay.collider.bounds.min.y : yRay.collider.bounds.max.y;

				if (yRayOrigin.y - colBound < 0.01f) {

					Vector3 currentPos = transform.position;
					currentPos.y = colBound + _collider.bounds.extents.y * -yDirection;
					transform.position = currentPos;
				}

				return;
			}

			yRayOrigin.x -= _collider.bounds.size.x - 0.02f;
		}

		_isOnGround = false;
	}

	bool IsOneWayHorizontalCollision(float direction, string tileID) {

		if (SMBGameWorld.Instance.TileMap.ContainsKey (tileID)) {

			if (direction >= 1f) {

				if (!SMBGameWorld.Instance.TileMap [tileID].collisions.left)
					return true;
			}
			else if (direction == -1f) {

				if (!SMBGameWorld.Instance.TileMap [tileID].collisions.right)
					return true;
			}
		}

		return false;
	}

	bool IsOneWayVerticalCollision(float direction, string tileID) {

		if (SMBGameWorld.Instance.TileMap.ContainsKey (tileID)) {

			if (direction == 1f) {

				if (!SMBGameWorld.Instance.TileMap [tileID].collisions.bottom)
					return true;
			}

			else if (direction == -1f) {

				if (!SMBGameWorld.Instance.TileMap [tileID].collisions.top)
					return true;
			}
		}

		return false;
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

		_body.velocity.x = Mathf.Lerp (_body.velocity.x, speed * Time.fixedDeltaTime, momentumReduction * Time.fixedDeltaTime);

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
