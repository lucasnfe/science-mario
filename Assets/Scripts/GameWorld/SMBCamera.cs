using UnityEngine;
using System.Collections;

public class SMBCamera : MonoBehaviour {

	public GameObject player;
	public float cameraSpeed = 5.0f;
	public float deltaFromPlayer = 0f;

	// Update is called once per frame
	void FixedUpdate () {

		if (!player)
			return;

		// X position follow
		Vector3 camPos = transform.position;
		camPos.x = player.transform.position.x;
		transform.position = Vector3.Lerp (transform.position, camPos, cameraSpeed * Time.fixedDeltaTime);

		// Y position follow
		camPos.y = player.transform.position.y ;
		transform.position = Vector3.Lerp (transform.position, camPos, 7.0f * Time.fixedDeltaTime);
	}
}
