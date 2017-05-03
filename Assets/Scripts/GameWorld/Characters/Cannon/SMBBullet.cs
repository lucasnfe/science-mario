using UnityEngine;
using System.Collections;

public class SMBBullet : SMBEnemy {

	public SMBCannon cannon { get; set; }

	public void Init(float bulletSpeed, SMBConstants.MoveDirection direction, Vector3 position) {

		gameObject.layer = LayerMask.NameToLayer ("Enemy");

		_body.velocity = Vector2.zero;
		_body.applyGravity = false;
		_collider.applyHorizCollision = true;

		_state = SMBConstants.EnemyState.Move;

		lockXPosition = false;

		xSpeed = bulletSpeed;
		startDirection = direction;

		transform.position = position;
	}

	// Use this for initialization
	override protected void Start () {

		base.Start ();

		int player = LayerMask.NameToLayer ("Player");

		_collider.horizontalMask = 0;
		_collider.horizontalMask |= (1 << player);

		_collider.verticalMask = 0;
		_collider.verticalMask |= (1 << player);
	}

	protected override void DestroyCharacter() {

		cannon.Bullets.SetFreeObject (gameObject);
	}
}
