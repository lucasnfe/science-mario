using UnityEngine;
using System.Collections;

public class SMBItem : MonoBehaviour {

	void OnInteraction(Collider2D coll) {

		Destroy (gameObject);
	}
}