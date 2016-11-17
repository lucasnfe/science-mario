using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SMBRigidBody))]
[RequireComponent (typeof (BoxCollider2D))]
public class SMBCollider : MonoBehaviour {

	private SMBRigidBody _body;
	private BoxCollider2D  _collider;

	void Awake() {

		_body = GetComponent<SMBRigidBody> ();
		_collider = GetComponent<BoxCollider2D> ();
	}

	void LateUpdate () {

		CheckHorizontalCollision ();

		CheckVerticalCollision ();
	}

	bool CheckHorizontalCollision() {

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
					return false;

				string tileID = xRay.collider.name;
				if(IsOneWayHorizontalCollision(xDirection, tileID))
					return false;

				SendMessage ("OnHorizontalCollisionEnter", SendMessageOptions.DontRequireReceiver);

				// Player collided on x axis, so stop it
				_body.velocity.x = 0f;

				// Fix player position after collision
				float colBound = (xDirection == 1f) ? xRay.collider.bounds.min.x : xRay.collider.bounds.max.x;

				if (xRayOrigin.x - colBound < 0.01f) {

					Vector3 currentPos = transform.position;
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
					return false;

				string tileID = yRay.collider.name;
				if(IsOneWayVerticalCollision(yDirection, tileID))
					return false;

				SendMessage ("OnVerticalCollisionEnter", SendMessageOptions.DontRequireReceiver);

				// Player collided on y axis, so stop it
				_body.velocity.y = 0f;

				// Fix player position after collision
				float colBound = (yDirection == 1f) ? yRay.collider.bounds.min.y : yRay.collider.bounds.max.y;

				if (yRayOrigin.y - colBound < 0.01f) {

					Vector3 currentPos = transform.position;
					currentPos.y = colBound + _collider.bounds.extents.y * -yDirection;
					transform.position = currentPos;
				}

				return true;
			}

			yRayOrigin.x -= _collider.bounds.size.x - 0.02f;
		}
			
		SendMessage ("OnVerticalCollisionExit", SendMessageOptions.DontRequireReceiver);
		return false;
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
