using UnityEngine;
using System.Collections;

public class SMBItem : MonoBehaviour {

	virtual protected void OnInteraction() {

		Destroy (gameObject);
	}
}