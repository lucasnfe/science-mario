using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PerfectOverride
{
	public int referenceOrthographicSize;
	public float referencePixelsPerUnit;
}

public class SMBCamera : MonoBehaviour {

	private float _width;
	private float _height;

	private Camera  _camera;
	private Vector2 _velocity;

	public SMBPlayer player;

	public int referenceOrthographicSize;
	public float referencePixelsPerUnit;
	public float cameraSpeed = 5.0f;

	void Awake() {

		_camera = GetComponent<Camera> ();
	}

	void Start() {

		_height = 2f * _camera.orthographicSize;
		_width = _height * _camera.aspect;
	}

	void UpdateOrthoSize()
	{
		int lastSize = Screen.height;

		// first find the reference orthoSize
		float refOrthoSize = (referenceOrthographicSize / referencePixelsPerUnit) * 0.5f;

		// then find the current orthoSize
		float ppu = referencePixelsPerUnit;
		float orthoSize = (lastSize / ppu) * 0.5f;

		// the multiplier is to make sure the orthoSize is as close to the reference as possible
		float multiplier = Mathf.Max(1, Mathf.Round(orthoSize / refOrthoSize));

		// then we rescale the orthoSize by the multipler
		orthoSize /= multiplier;

		// set it
		_camera.orthographicSize = orthoSize;
	}

	// Update is called once per frame
	void LateUpdate () {

		if (!player || player.State == SMBConstants.PlayerState.Dead)
			return;

		Vector3 cameraZPos = Vector3.forward * transform.position.z;
		transform.position = Vector2.Lerp (transform.position, player.transform.position, cameraSpeed * Time.fixedDeltaTime);
		transform.position += cameraZPos;
	
		// Lock camera position
		Vector3 cameraPos = transform.position;

		cameraPos.x = Mathf.Clamp (cameraPos.x, _width * 0.5f - SMBGameWorld.Instance.LockLeftX, 
			SMBGameWorld.Instance.LockRightX - _width * 0.5f);

		cameraPos.y = Mathf.Clamp (cameraPos.y, _height * 0.5f - SMBGameWorld.Instance.LockDownY, 
			SMBGameWorld.Instance.LockUpY - _height * 0.5f);

		transform.position = cameraPos;
	}

	public void SetCameraPos(Vector2 newCameraPos) {

		Vector3 cameraZPos = Vector3.forward * transform.position.z;
		transform.position = newCameraPos;
		transform.position += cameraZPos;
	}
		
}
