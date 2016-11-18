using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SMBRigidBody))]
[RequireComponent (typeof (BoxCollider2D))]
public class SMBCollider : MonoBehaviour {

	private SMBRigidBody    _body;
	private BoxCollider2D   _collider;

	public bool applyHorizCollision = true;
	public bool applyVertCollision = true;

	void Awake() {

		_body = GetComponent<SMBRigidBody> ();
		_collider = GetComponent<BoxCollider2D> ();
	}

	void LateUpdate () {

		if(applyHorizCollision)
			CheckHorizontalCollision ();

		if(applyVertCollision)
			CheckVerticalCollision ();
	}

	bool CheckHorizontalCollision() {

		float xDirection = Mathf.Sign (_body.velocity.x);
			
		Vector2 xRayOrigin = (xDirection == 1f) ? _collider.bounds.max + Vector3.right * 0.001f:
			_collider.bounds.max - Vector3.right * _collider.bounds.size.x - Vector3.right * 0.001f;

		xRayOrigin.y -= 0.1f;

		for (int i = 0; i < 2; i++) {

			RaycastHit2D xRay = Physics2D.Raycast (xRayOrigin, Vector2.right * xDirection, SMBConstants.player_skin);
			// Debug.DrawRay (xRayOrigin, Vector2.right * xDirection);
			if (xRay.collider) {
				 
				if (xRay.collider.isTrigger) {
					SendMessage ("OnHorizontalTriggerEnter", xRay.collider, SendMessageOptions.DontRequireReceiver);
					return false;
				}

				string tileID = xRay.collider.name;
				if(IsOneWayHorizontalCollision(xDirection, tileID))
					return false;

				SendMessage ("OnHorizontalCollisionEnter", xRay.collider, SendMessageOptions.DontRequireReceiver);

				// Player collided on x axis, so stop it
				_body.velocity.x = 0f;

				// Fix player position after collision
				float distance = Mathf.Abs (xRayOrigin.x - xRay.point.x);

				if (distance <= SMBConstants.player_skin) {

					Vector3 currentPos = transform.position;
					float colBound = (xDirection == 1f) ? xRay.collider.bounds.min.x : xRay.collider.bounds.max.x;
					currentPos.x = colBound + _collider.bounds.extents.x * -xDirection;
					transform.position = currentPos;
				}

				return true;
			}

			xRayOrigin.y -= _collider.bounds.size.y - 0.2f;
		}

		SendMessage ("OnHorizontalCollisionExit", SendMessageOptions.DontRequireReceiver);
		return false;
	}

	bool CheckVerticalCollision() {

		float yDirection = Mathf.Sign (_body.velocity.y);
		Vector2 yRayOrigin = (yDirection == 1f) ? _collider.bounds.max + Vector3.up * 0.001f :
			_collider.bounds.max - Vector3.up * _collider.bounds.size.y - Vector3.up * 0.001f;

		yRayOrigin.x -= 0.01f;

		int collisions = 0;
		float colBound = 0f;
		Collider2D collider = null;

		for (int i = 0; i < 2; i++) {

			RaycastHit2D yRay = Physics2D.Raycast(yRayOrigin, Vector2.up * yDirection, SMBConstants.player_skin);
			// Debug.DrawRay (yRayOrigin, Vector2.up * yDirection);

			if (yRay.collider) {

				collider = yRay.collider;

				if (yRay.collider.isTrigger) {

					SendMessage ("OnVerticalTriggerEnter", yRay.collider, SendMessageOptions.DontRequireReceiver);

					collisions = 0;
					continue;
				}

				string tileID = yRay.collider.name;
				if (IsOneWayVerticalCollision (yDirection, tileID)) {
					collisions = 0;
					continue;
				}

				colBound = (yDirection == 1f) ? yRay.collider.bounds.min.y : yRay.collider.bounds.max.y;
				collisions++;
			}

			yRayOrigin.x -= _collider.bounds.size.x - 0.02f;
		}

		if (collisions == 0) {

			SendMessage ("OnVerticalCollisionExit", SendMessageOptions.DontRequireReceiver);
			return false;
		}

		if (collisions == 1)

			SendMessage ("OnHalfVerticalCollisionEnter", collider, SendMessageOptions.DontRequireReceiver);
		else
			SendMessage ("OnFullVerticalCollisionEnter", collider, SendMessageOptions.DontRequireReceiver);

		// Player collided on y axis, so stop it
		_body.velocity.y = 0f;

		// Fix player position after collision
		if (yRayOrigin.y - colBound < SMBConstants.player_skin) {

			Vector3 currentPos = transform.position;
			currentPos.y = colBound + _collider.bounds.extents.y * -yDirection;
			transform.position = currentPos;
		}

		return true;
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
}
