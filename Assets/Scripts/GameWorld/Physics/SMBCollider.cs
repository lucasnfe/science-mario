using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (SMBRigidBody))]
[RequireComponent (typeof (BoxCollider2D))]
public class SMBCollider : MonoBehaviour {

	private SMBRigidBody  _body;
	private BoxCollider2D _collider;

	public  BoxCollider2D Collider { get { return _collider; } }

	public bool applyHorizCollision = true;
	public bool applyVertCollision = true;

	public int horizontalMask { get; set; }
	public int verticalMask { get; set; }

	void Awake() {

		_body = GetComponent<SMBRigidBody> ();
		_collider = GetComponent<BoxCollider2D> ();
	}

	void Start() {

		for (int i = 0; i < SMBConstants.maxLayers; i++) {
			horizontalMask |= (1 << i);
			verticalMask |= (1 << i);
		}

		int ignoreLayer = LayerMask.NameToLayer ("Ignore Raycast");
		horizontalMask &= ~(1 << ignoreLayer);
		verticalMask &= ~(1 << ignoreLayer);
	}

	void LateUpdate () {

		if(applyHorizCollision)
			CheckHorizontalCollision ();

		if(applyVertCollision)
			CheckVerticalCollision ();
	}

	bool CheckHorizontalCollision() {
		
		bool didCollide = false;
		float xDirection = _body.velocity.x >= 0f ? 1f : -1f;
			
		Vector2 xRayOrigin = (xDirection == 1f) ? _collider.bounds.max + Vector3.right * SMBConstants.playerSkin :
			_collider.bounds.max - Vector3.right * _collider.bounds.size.x - Vector3.right * SMBConstants.playerSkin;

		xRayOrigin.y -= SMBConstants.playerSkin * 2f;

		for (int i = 0; i < 3; i++) {

			RaycastHit2D[] xRays = Physics2D.RaycastAll (xRayOrigin, Vector2.right * xDirection, SMBConstants.playerSkin, horizontalMask);
//			Debug.DrawRay (xRayOrigin, Vector2.right * xDirection);

			foreach (RaycastHit2D xRay in xRays) {

				if (xRay.collider) {

					string tileID = xRay.collider.name;
					if (xRay.collider.isTrigger) {

						SendMessage ("OnHorizontalTriggerEnter", xRay.collider, SendMessageOptions.DontRequireReceiver);
					} 
					else if (!IsOneWayVerticalCollision (xDirection, tileID)) {

						if (!didCollide) {
							
							float xBound = (xDirection == 1f) ? xRay.collider.bounds.min.x : xRay.collider.bounds.max.x;
							ResolveHorizontalCollision (xRayOrigin, xBound, xDirection);

							didCollide = true;
						}

						SendMessage ("OnHorizontalCollisionEnter", xRay.collider, SendMessageOptions.DontRequireReceiver);
					}
				}
			}

			xRayOrigin.y -= (_collider.bounds.size.y / 3f + SMBConstants.playerSkin * 0.5f);
		}

		if(!didCollide)
			SendMessage ("OnHorizontalCollisionExit", SendMessageOptions.DontRequireReceiver);

		return didCollide;
	}

	void ResolveHorizontalCollision(Vector2 xRayOrigin, float xBound, float xDirection) {

		// Player collided on x axis, so stop it
		_body.velocity.x = 0f;

		// Fix player position after collision	
		if (xRayOrigin.x - xBound < SMBConstants.collisionThresholdX) {

			Vector3 currentPos = transform.position;
			currentPos.x = xBound + (_collider.bounds.extents.x - _collider.offset.x) * -xDirection;
			transform.position = currentPos;
		}
	}

	bool CheckVerticalCollision() {

		bool didCollide = false;

		float yDirection = _body.velocity.y > 0f ? 1f : -1f;
		Vector2 yRayOrigin = (yDirection == 1f) ? _collider.bounds.max + Vector3.up * SMBConstants.playerSkin :
			_collider.bounds.max - Vector3.up * _collider.bounds.size.y - Vector3.up * SMBConstants.playerSkin;

		yRayOrigin.x -= SMBConstants.playerSkin;

		for (int i = 0; i < 2; i++) {

			RaycastHit2D []yRays = Physics2D.RaycastAll(yRayOrigin, Vector2.up * yDirection, SMBConstants.playerSkin, verticalMask);
			Debug.DrawRay (yRayOrigin, Vector2.up * yDirection);

			foreach (RaycastHit2D yRay in yRays) {

				if (yRay.collider) {

					string tileID = yRay.collider.name;
					if (yRay.collider.isTrigger) {

						SendMessage ("OnVerticalTriggerEnter", yRay.collider, SendMessageOptions.DontRequireReceiver);
					} 
					else if (!IsOneWayVerticalCollision (yDirection, tileID)) {

						if (!didCollide) {

							float yBound = (yDirection == 1f) ? yRay.collider.bounds.min.y : yRay.collider.bounds.max.y;
							ResolveVerticalCollision (yRayOrigin, yBound, yDirection);

							didCollide = true;
						}

						SendMessage ("OnVerticalCollisionEnter", yRay.collider, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
				
			yRayOrigin.x -= _collider.bounds.size.x - SMBConstants.playerSkin * 2f;
		}
			
		if(!didCollide)
			SendMessage ("OnVerticalCollisionExit", SendMessageOptions.DontRequireReceiver);			

		return didCollide;
	}

	void ResolveVerticalCollision(Vector2 yRayOrigin, float yBound, float yDirection) {

		// Player collided on y axis, so stop it
		_body.velocity.y = 0f;

		// Fix player position after collision
		if (yRayOrigin.y - yBound < SMBConstants.collisionThresholdY) {

			Vector3 currentPos = transform.position;
			currentPos.y = yBound + (_collider.bounds.extents.y - _collider.offset.y) * -yDirection;
			transform.position = currentPos;
		}
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

	public void SetIsTrigger(bool isTrigger) {

		_collider.isTrigger = isTrigger;
	}

	public void SetSize(Bounds bounds) {

		_collider.size = bounds.size;
		_collider.offset = bounds.center;
	}

	public bool GetTrigger(bool isTrigger) {

		return _collider.isTrigger;
	}

	public Bounds GetSize() {

		return _collider.bounds;
	}
}
