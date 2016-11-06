using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SMBConstants {

	public enum MoveDirection {
		Forward  =  1,
		Backward = -1
	}

	public static readonly string levelFilename = "Levels/level1";

	public static readonly string tilesDescrition = "tiles";

	// One way platform angle of collision
	public static readonly float  platformCollisionAngle = 160f;
}
