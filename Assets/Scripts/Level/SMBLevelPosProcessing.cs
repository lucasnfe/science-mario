using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SMBLevelPosProcessing {

	public static void CreateColliders(int[,] tileMap, Transform collidersParent) {

		int height = tileMap.GetLength (0);
		int width = tileMap.GetLength (1);

		for (int i = 0; i < height; i++) {

			int groundTilesSoFar = 0;
			int tileStart = 0;

			for (int j = 0; j < width; j++) {

				if (tileMap [i, j] == 1) {

					groundTilesSoFar++;

					if (j > 0 && tileMap [i, j - 1] == 0)
						tileStart = i;
				}
				else if (tileMap [i, j] == 0) {
			
					if (groundTilesSoFar > 0) {

						GameObject newBoxObject = new GameObject ();
						newBoxObject.name = "Collider_" + i + "_" + j;
						newBoxObject.transform.parent = collidersParent;
						newBoxObject.tag = "Platform";

						float offsetX = ((float)j - (float)groundTilesSoFar / 2f) - 0.5f;

						BoxCollider2D box = newBoxObject.AddComponent<BoxCollider2D> (); 
						box.size = new Vector2(groundTilesSoFar, 1f) * SMBConstants.tileSize;
						box.offset = new Vector2(offsetX, (height - i)) * SMBConstants.tileSize;

						// If platform is above the ground, make it one-way
						if (i != height - 1) {

							box.usedByEffector = true;

							PlatformEffector2D effector = newBoxObject.AddComponent<PlatformEffector2D> ();
							effector.surfaceArc = 150f;
						}
					}

					groundTilesSoFar = 0;
				}
			}
		}
	}
}
