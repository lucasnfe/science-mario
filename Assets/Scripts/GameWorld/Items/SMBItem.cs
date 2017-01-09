using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SMBRigidBody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class SMBItem : MonoBehaviour {

	protected SMBRigidBody _body;
	protected AudioSource _audio;
	protected Animator _animator;

	virtual protected void Awake() {

		_body = GetComponent<SMBRigidBody> ();
		_audio = GetComponent<AudioSource> ();
		_animator = GetComponent<Animator> ();
	}
		
	virtual protected void OnInteraction() {

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