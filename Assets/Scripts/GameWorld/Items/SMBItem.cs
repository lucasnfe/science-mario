using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SMBRigidBody))]
public class SMBItem : MonoBehaviour {

	protected SMBRigidBody _body;

	void Awake() {

		_body = GetComponent<SMBRigidBody> ();
	}
		
	virtual protected void OnInteraction() {

		Destroy (gameObject);
	}
}