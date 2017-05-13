using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SMBCollider))]
[RequireComponent(typeof(SMBRigidBody))]
public class SMBBlock : MonoBehaviour {

	enum BounceState {
		None,
		Up,
		Down
	}
		
	private Vector3 _posBeforeBounce;
	private BounceState _bounceState;

	protected bool  _isDestroyed;

	protected SMBRigidBody   _body;
	protected SMBCollider    _collider;
	protected Animator       _animator;

	public float _bounceVelocity = 1f;
	public float _bounceYDist = 0.15f;
	public float _enemyKickForce = 1f;

	void Awake() {

		_body     = GetComponent<SMBRigidBody> ();
		_collider = GetComponent<SMBCollider> ();
		_animator = GetComponent<Animator> ();
	}

	void Update() {

		if (_bounceState != BounceState.None)
			Bounce ();
	}

	void OnVerticalCollisionEnter(Collider2D collider) {

		if (collider.tag == "Player") {

			if (collider.bounds.center.y < transform.position.y && 
				Mathf.Abs(collider.bounds.center.x - transform.position.x) < 0.5f * SMBGameWorld.Instance.TileSize) {

				SMBRigidBody playerBody = collider.GetComponent<SMBRigidBody> ();
				if (playerBody.velocity.y > 0f)
					playerBody.velocity.y = 0f;
				
				if (_bounceState == BounceState.None && !_isDestroyed) {

					DestroyBlock (SMBGameWorld.Instance.Player);

					_posBeforeBounce = transform.position;
					_bounceState = BounceState.Up;
				}
			}
		} 
	}

	void OnVerticalTriggerEnter(Collider2D collider) {

		if (_bounceState == BounceState.Up) {

			SMBRigidBody body = collider.GetComponent<SMBRigidBody> ();
			if (body != null)
				body.ApplyForce (Vector2.up * _enemyKickForce * Time.fixedDeltaTime);

			if (collider.tag == "Enemy")
				collider.SendMessage ("Die", this.gameObject, SendMessageOptions.DontRequireReceiver);	
		}


	}

	private void Bounce() {

		Vector3 currentPos = transform.position;

		if (_bounceState == BounceState.Up) {

			if (currentPos.y <= _posBeforeBounce.y + _bounceYDist) {

				_body.velocity.y = _bounceVelocity;
			}
			else  {
				
				_bounceState = BounceState.Down;
			}
		}
		else if (_bounceState == BounceState.Down) {

			if (currentPos.y >= _posBeforeBounce.y) {

				_body.velocity.y = -_bounceVelocity;
			}
			else {

				_bounceState = BounceState.None;
				_body.velocity.y = 0f;
				transform.position = _posBeforeBounce;

				GivePrize ();
			}
		}
	}
		
	protected virtual void DestroyBlock (SMBPlayer player) {
		
	}

	protected virtual void GivePrize () {

	}

	void OnPauseGame() {

		_animator.enabled = false;
	}

	void OnResumeGame() {

		_animator.enabled = true;
	}
}
