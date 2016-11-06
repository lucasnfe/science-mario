using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class SMBBlock : MonoBehaviour {

	protected Animator       _animator;
	protected BoxCollider2D  _collider;

	void Awake() {

		_animator = GetComponent<Animator> ();
		_collider = GetComponent<BoxCollider2D> ();
		_collider.isTrigger = true;
	}

	void OnTriggerEnter2D(Collider2D coll) {

		if (coll.tag == "Player") {
			
			Vector2 rayOrigin = coll.bounds.center;
			rayOrigin.y += coll.bounds.extents.y;

			RaycastHit2D ray = Physics2D.Raycast (rayOrigin, Vector2.up, 0.01f);
			if (ray.collider && ray.collider.tag == "Platform") {

				if(Mathf.Abs(rayOrigin.x - _collider.bounds.center.x) < _collider.bounds.extents.x)
					DestroyBlock ();
			}
		}
	}
		
	protected abstract void DestroyBlock ();
}
