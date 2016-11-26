using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SMBRigidBody))]
[RequireComponent(typeof(AudioSource))]
public class SMBItem : MonoBehaviour {

	protected SMBRigidBody _body;
	protected AudioSource _audio;

	virtual protected void Awake() {

		_body = GetComponent<SMBRigidBody> ();
		_audio = GetComponent<AudioSource> ();
	}
		
	virtual protected void OnInteraction() {

		Destroy (gameObject);
	}
}