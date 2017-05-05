using UnityEngine;
using System.Collections;

public class SMBEnemy : SMBCharacter {

	protected enum SoundEffects {
		Kick
	}

	protected SMBConstants.EnemyState _state;

	override protected void Start() {

		_state = SMBConstants.EnemyState.Move;
		base.Start ();
	}

	// Update is called once per frame
	override protected void Update () {

		base.Update ();

		if (_state == SMBConstants.EnemyState.Dead)
			return;

		SMBConstants.MoveDirection side = SMBConstants.MoveDirection.Forward;
		if(isFlipped())
			side = SMBConstants.MoveDirection.Backward;
		
		Move (xSpeed * (float)side);
	}

	virtual protected void Die(GameObject killer) {

		if (_state == SMBConstants.EnemyState.Dead)
			return;

		_body.velocity = Vector2.zero;
		_body.ApplyForce (Vector2.up);
		_body.ApplyForce (Vector2.right * Random.Range(-2f, 2f) * Time.fixedDeltaTime);

		_collider.applyHorizCollision = false;
		_collider.applyVertCollision = false;
		_body.applyGravity = true;

		gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");

		_state = SMBConstants.EnemyState.Dead;
		_animator.Play ("Die");
		_audio.PlayOneShot (soundEffects[(int)SoundEffects.Kick]);
	}

	bool isOnPlatformEdge(float side) {

		Vector2 yRayOrigin = _collider.Collider.bounds.max -
			Vector3.up * _collider.Collider.bounds.size.y - Vector3.up * SMBConstants.playerSkin;

		if (side == (float)SMBConstants.MoveDirection.Backward)
			yRayOrigin.x -= _collider.Collider.bounds.size.x;

		RaycastHit2D yRay = Physics2D.Raycast (yRayOrigin, Vector2.down, SMBConstants.playerSkin);
		if (!yRay.collider)
			return true;

		return false;
	}

	virtual protected void OnCollisionWithPlayer(Collider2D playerCollider) {

		bool isPlayerOnGround = SMBGameWorld.Instance.Player.IsOnGround;
		Vector3 playerPosition = playerCollider.transform.position;

		if (playerPosition.y > transform.position.y + 0.1f) {

			if (!isPlayerOnGround) {
				Die (SMBGameWorld.Instance.Player.gameObject);
				playerCollider.SendMessage ("KillEnemy");
			}
		}
		else
			playerCollider.SendMessage ("TakeDamage");
	}

	override protected void OnVerticalCollisionEnter(Collider2D collider) {
				
		if (collider.tag == "Player") {

			OnCollisionWithPlayer (collider);
		} 
		else {

			float side = (float)SMBConstants.MoveDirection.Forward;
			if (isFlipped ())
				side = (float)SMBConstants.MoveDirection.Backward;

			if(isOnPlatformEdge(side)) {
				
				Vector3 newVelocity = _body.velocity;
				newVelocity.x = 0f;
				_body.velocity = newVelocity;

				_renderer.flipX = !_renderer.flipX;

				transform.position = transform.position - Vector3.right * side * SMBConstants.playerSkin;
			}
		}

		base.OnVerticalCollisionEnter (collider);
	}

	override protected void OnHorizontalCollisionEnter(Collider2D collider) {

		if (collider.tag == "Player") {

			OnCollisionWithPlayer (collider);
		}
		else
			_renderer.flipX = !_renderer.flipX;

		base.OnHorizontalCollisionEnter (collider);
	}
}
