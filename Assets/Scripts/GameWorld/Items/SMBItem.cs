using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SMBRigidBody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class SMBItem : MonoBehaviour {

	protected SMBRigidBody _body;
	protected SMBCollider  _collider;
	protected AudioSource  _audio;
	protected Animator     _animator;

	virtual protected void Awake() {

		_body = GetComponent<SMBRigidBody> ();
		_collider = GetComponent<SMBCollider> ();
		_audio = GetComponent<AudioSource> ();
		_animator = GetComponent<Animator> ();
	}
		
	protected virtual void OnVerticalCollisionEnter(Collider2D collider) {

		if(collider.tag == "Player")
			Destroy (gameObject);
	}

	protected virtual void OnHorizontalCollisionEnter(Collider2D collider) {

		if(collider.tag == "Player")
			Destroy (gameObject);
	}

	void OnPauseGame() {

		_body.enabled = false;
		_animator.enabled = false;
	}

	void OnResumeGame() {

		_body.enabled = true;
		_animator.enabled = true;
	}
}