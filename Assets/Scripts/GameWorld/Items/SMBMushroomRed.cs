using UnityEngine;
using System.Collections;

public class SMBMushroomRed : SMBItem {

	public float xSpeed = 1f;

	void OnSpawn() {

		float randomSide = (Random.value < 0.5f ? -1f : 1f);
		_body.velocity.x = xSpeed * randomSide * Time.fixedDeltaTime;
	}
}
