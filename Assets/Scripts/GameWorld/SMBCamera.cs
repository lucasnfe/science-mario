using UnityEngine;
using System.Collections;

public class SMBCamera : MonoBehaviour {

	private Camera _camera;

	public GameObject player;
	public float cameraSpeed = 5.0f;
	public float deltaFromPlayer = 0f;

	void Awake() {

		_camera = GetComponent<Camera> ();
	}

	// Update is called once per frame
	void LateUpdate () {

		if (!player)
			return;

		Vector3 cameraZPos = Vector3.forward * transform.position.z;
		transform.position = Vector2.Lerp (transform.position, player.transform.position, cameraSpeed * Time.fixedDeltaTime);
		transform.position += cameraZPos;

		// Lock camera position
		Vector3 cameraPos = transform.position;

		float camHeight = 2f * _camera.orthographicSize;
		float camWidth = camHeight * _camera.aspect;

		cameraPos.x = Mathf.Clamp (cameraPos.x, camWidth * 0.5f - SMBGameWorld.Instance.LockLeftX, 
			SMBGameWorld.Instance.LockRightX - camWidth * 0.5f);

		cameraPos.y = Mathf.Clamp (cameraPos.y, camHeight * 0.5f - SMBGameWorld.Instance.LockDownY, 
			SMBGameWorld.Instance.LockUpY - camHeight * 0.5f);

		transform.position = cameraPos;
	}

	public void SetCameraPos(Vector2 newCameraPos) {

		Vector3 cameraZPos = Vector3.forward * transform.position.z;
		transform.position = newCameraPos;
		transform.position += cameraZPos;
	}
		
}
