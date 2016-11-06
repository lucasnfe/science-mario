using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class SMBItem : MonoBehaviour {

	private BoxCollider2D  _collider;

	void Awake() {

		_collider = GetComponent<BoxCollider2D> ();
	}

	void OnTriggerEnter2D(Collider2D other) {

		Destroy (gameObject);
	}
}