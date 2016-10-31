using UnityEngine;
using System.Collections;

public class SMBLevelParser {

	public static char[,] Parse(string filename) {

		int width = 0;
		int height = 0;

		TextAsset levelfile = Resources.Load<TextAsset> (filename);

		if (!levelfile) {
			Debug.LogError ("Could not read file");
			return null;
		}
			
		string text = levelfile.text.Remove (levelfile.text.Length - 1);
		string[] lines = text.Split ('\n');

		width = lines [0].Length;
		height = lines.Length;

		foreach (string line in lines) {

			if (line.Length != width) {
				Debug.LogError ("Level row has different width, they all have to have the same amount of tiles!");
				return null;
			}
		}
			
		char[,] tileMap = new char[height, width] ;

		for (int i = 0; i < height; i++) {

			char[] row = lines [i].ToCharArray ();
			for (int j = 0; j < width-1; j++)
				tileMap[i,j] = row[j];
		}

		return tileMap;
	}

	public static void CreateColliders(char[,] tileMap, Transform collidersParent) {

		int height = tileMap.GetLength (0);
		int width = tileMap.GetLength (1);

		for (int i = 0; i < height; i++) {

			int groundTilesSoFar = 0;

			for (int j = 0; j < width; j++) {

				if (tileMap [i, j] == '1') {

					groundTilesSoFar++;
				}
				else if (tileMap [i, j] == '0' || j == width - 1) {

					if (groundTilesSoFar > 0) {

						GameObject newBoxObject = new GameObject ();
						newBoxObject.name = "Collider_" + i + "_" + j;
						newBoxObject.transform.parent = collidersParent;
						newBoxObject.tag = "Platform";

						float offsetX = ((float)j - (float)groundTilesSoFar / 2f) - 0.5f;

						BoxCollider2D box = newBoxObject.AddComponent<BoxCollider2D> (); 
						box.size = new Vector2(groundTilesSoFar, 1f) * SMBConstants.tileSize;
						box.offset = new Vector2(offsetX, (height - i)) * SMBConstants.tileSize;

						box.usedByEffector = true;
						PlatformEffector2D effector = newBoxObject.AddComponent<PlatformEffector2D> ();
						effector.surfaceArc = 150f;
					}

					groundTilesSoFar = 0;
				}
			}
		}
	}
}


