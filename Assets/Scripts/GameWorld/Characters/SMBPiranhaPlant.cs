using UnityEngine;
using System.Collections;

public class SMBPiranhaPlant : SMBEnemy {

	enum MoveState {
		Idle,
		Up,
		Down
	}
		
	private float _idleTimer;
	private Vector3 _posBeforeBounce;
	private MoveState _moveState;

	public float _moveYDist = 0.15f;
	public float _timeIdle  = 1f;

	public bool _isMoving;

	// Use this for initialization
	override protected void Start () {

		base.Start ();
	
		int player = LayerMask.NameToLayer ("Player");

		_collider.horizontalMask = 0;
		_collider.horizontalMask |= (1 << player);

		_collider.verticalMask = 0;
		_collider.verticalMask |= (1 << player);
	}

	override protected void Update() {

		base.Update ();

		if (_state == SMBConstants.CharacterState.Dead)
			return;

		if (_isMoving)
			UpdateSpeed ();
	}

	void OnSpawnStart(GameObject spawner) {

		_isMoving = true;
		_moveState = MoveState.Up;

		Vector3 pos = transform.position;
		pos.x = spawner.transform.position.x;
		pos.y = spawner.transform.position.y - 0.5f * SMBGameWorld.Instance.TileSize;
		transform.position = pos;

		_posBeforeBounce = transform.position;
	}

	private void UpdateSpeed() {

		Vector3 currentPos = transform.position;

		if (_moveState == MoveState.Idle) {

			_idleTimer += Time.fixedDeltaTime;
			if (_idleTimer >= _timeIdle) {

				_collider.Collider.enabled = true;
				_animator.Play ("Move");
				_moveState = MoveState.Up;
				_idleTimer = 0f;
			}
			
		}
		else if (_moveState == MoveState.Up) {

			if (currentPos.y <= _posBeforeBounce.y + _moveYDist) {

				_body.velocity.y = ySpeed * Time.fixedDeltaTime;
			}
			else  {

				_moveState = MoveState.Down;
			}
		}
		else if (_moveState == MoveState.Down) {

			if (currentPos.y >= _posBeforeBounce.y) {

				_body.velocity.y = -ySpeed * Time.fixedDeltaTime;
			}
			else {

				_body.velocity.y = 0f;
				_moveState = MoveState.Idle;
				transform.position = _posBeforeBounce;

				_animator.Play ("Idle");
				_collider.Collider.enabled = false;
			}
		}
	}

	override protected void OnCollisionWithPlayer(Collider2D playerCollider) {

		playerCollider.SendMessage ("TakeDamage", this.gameObject);
	}
}
