using UnityEngine;
using System.Collections;

public class SMBItem : MonoBehaviour {

	virtual protected void OnInteraction(Collider2D coll) {

		Destroy (gameObject);
	}
}