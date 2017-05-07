using UnityEngine;
using System.Collections;

public class SMBShell : SMBItem {

	enum SoundEffects {
		Kick
	}

	enum ShellState {
		Idle,
		Dragged,
		Spinning,
		Destroied
	}

	private int  _horizontalMask;
	private int  _verticalMask;
	private bool _isOnGround;

	private ShellState _sheelState = ShellState.Idle;

	public float _kickForce = 1f;

	void Start() {

		_horizontalMask = _collider.horizontalMask;
		_verticalMask = _collider.verticalMask;
	}

	void Update() {

		if (_sheelState == ShellState.Dragged) {

			SMBConstants.MoveDirection playerDirection = SMBGameWorld.Instance.Player.direction;
			Vector3 playerPosition = SMBGameWorld.Instance.Player.transform.position;

			if (Input.GetKeyUp (KeyCode.Z)) {

				Kick (playerPosition, (float)playerDirection);
			}
			else if (Input.GetKey (KeyCode.Z)) {

				Drag (playerPosition, (float)playerDirection);
			}
		} 
		else if (_sheelState == ShellState.Spinning) {

			_body.velocity.x = xSpeed * _side * Time.fixedDeltaTime;
		}
	}

	void Kick(Vector3 playerPosition, float playerDirection) {

		SetEnemy ();

		xSpeed = _kickForce;
		_side = playerDirection;

		Drag (playerPosition, playerDirection);

		// Move it a bit to avoid collision right after the kick
		Vector3 newPos = transform.position;
		newPos.x = transform.position.x + 0.1f * _side;
		transform.position = newPos;

		_sheelState = ShellState.Spinning;
		_animator.Play ("Spin");

		SMBGameWorld.Instance.Player.DropItem ();

		_audio.PlayOneShot (soundEffects[(int)SoundEffects.Kick]);
	}

	void Drag(Vector3 playerPosition, float playerDirection) {

		float shellWidth = _collider.Collider.bounds.size.x;

		Vector3 newPos = transform.position;
		newPos.x = playerPosition.x + shellWidth * (float)playerDirection;
		newPos.y = playerPosition.y;
		transform.position = newPos;

		SMBGameWorld.Instance.Player.CarryItem ();
	}

	void SetIdle() {

		tag = "Item";
		gameObject.layer = LayerMask.NameToLayer ("Default");

		_body.velocity = Vector2.zero;
		_body.applyGravity = true;

		_sheelState = ShellState.Idle;
		_animator.Play ("Idle");

		_collider.horizontalMask = _horizontalMask;
		_collider.verticalMask = _verticalMask;
	}

	void SetItem() {

		tag = "Item";
		gameObject.layer = LayerMask.NameToLayer ("Item");

		_body.velocity = Vector2.zero;
		_body.applyGravity = false;

		int enemy = LayerMask.NameToLayer ("Enemy");
		int item = LayerMask.NameToLayer ("Item");

		_collider.horizontalMask = 0;
		_collider.horizontalMask |= (1 << enemy);
		_collider.horizontalMask |= (1 << item);

		_collider.verticalMask = 0;
		_collider.verticalMask |= (1 << enemy);
		_collider.verticalMask |= (1 << item);
	}
		
	void SetEnemy() {

		tag = "Enemy";
		gameObject.layer = LayerMask.NameToLayer ("Enemy");

		_body.applyGravity = true;

		_collider.horizontalMask = _horizontalMask;
		_collider.verticalMask = _verticalMask;
	}

	void Die() {

		_sheelState = ShellState.Destroied;
		_animator.Play ("Idle");

		_body.velocity = Vector2.zero;
		_body.ApplyForce (Vector2.up);
		_body.ApplyForce (Vector2.right * -_side * 5f * Time.fixedDeltaTime);

		_collider.applyHorizCollision = false;
		_collider.applyVertCollision = false;
		_body.applyGravity = true;

		gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");

		_audio.PlayOneShot (soundEffects[(int)SoundEffects.Kick]);
	}

	void SolveCollision(Collider2D collider) {

		SMBConstants.MoveDirection playerDirection = SMBGameWorld.Instance.Player.direction;
		Vector3 playerPosition = SMBGameWorld.Instance.Player.transform.position;
		bool isPlayerOnGround = SMBGameWorld.Instance.Player.IsOnGround;

		if (_sheelState == ShellState.Idle) {

			if (isPlayerOnGround && _isOnGround) {

				if (Input.GetKey (KeyCode.Z)) {

					SetItem ();
					_sheelState = ShellState.Dragged;
				} 
				else {
				
					Kick (playerPosition, (float)playerDirection);
				}
			}
		} 
		else if (_sheelState == ShellState.Spinning) {

			if (playerPosition.y > transform.position.y + 0.1f) {

				collider.SendMessage ("KillEnemy");
				SetIdle ();

				_audio.PlayOneShot (soundEffects[(int)SoundEffects.Kick]);
			}
			else
				collider.SendMessage ("TakeDamage", 
				_collider.Collider, SendMessageOptions.RequireReceiver);
		}
	}

	override protected void OnVerticalCollisionEnter(Collider2D collider) {

		if (collider.tag == "Player") {
			
			SolveCollision (collider);
		} 
		else {

			float yDirection = _body.velocity.y > 0f ? 1f : -1f;
			if(yDirection == -1f)
				_isOnGround = true;
		}
	}

	override protected void OnHorizontalCollisionEnter(Collider2D collider) {

		if (collider.tag == "Player") {

			SolveCollision (collider);
		} 
		else
			_side *= -1f;
	}

	void SolveTrigger(Collider2D collider) {

		if (_collider.Collider == collider)
			return;
		
		if (collider.tag == "Enemy" || collider.name == "n" || collider.name == "o") {

			if (_sheelState == ShellState.Spinning || _sheelState == ShellState.Dragged) {

				collider.gameObject.SendMessage ("Die", this.gameObject, SendMessageOptions.DontRequireReceiver);

				if (_sheelState == ShellState.Dragged) {

					SMBGameWorld.Instance.Player.DropItem ();
					_sheelState = ShellState.Destroied;

					Die ();
				} 
			}
		}
	}

	void OnVerticalTriggerEnter(Collider2D collider) {

		SolveTrigger (collider);
	}

	void OnHorizontalTriggerEnter(Collider2D collider) {

		SolveTrigger (collider);
	}

}
