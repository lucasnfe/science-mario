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
	}

	void OnInteraction(Collider2D coll) {

		if (coll.tag == "Player") {

			float xDist = Mathf.Abs (coll.bounds.center.x - _collider.bounds.center.x);
			float yDist = coll.bounds.center.y - _collider.bounds.center.y;

			if(xDist < coll.bounds.size.x && yDist < 0f)
				DestroyBlock ();
		}
	}
		
	protected abstract void DestroyBlock ();
}
