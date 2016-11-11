using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SMBConstants {

	public enum MoveDirection {
		Forward  =  1,
		Backward = -1
	}

	public static readonly float maxVelocityX = 5f;
	public static readonly float maxVelocityY = 3f;

	public static readonly Vector2 gravity = new Vector2(0f, -7f);

	public static readonly string levelFilename = "Levels/level1";

	public static readonly string tilesDescrition = "tiles";
}
