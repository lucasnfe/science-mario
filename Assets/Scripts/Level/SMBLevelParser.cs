﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SMBTile {

	public string id;
	public string prefab;
	public bool isPlayer;
	public bool hasCollider;
}

[System.Serializable]
public class SMBTileMap {

	public float size;
	public SMBTile []tiles;
}


public class SMBLevelParser {

	public static SMBTileMap ParseTileMap(string filename) {

		TextAsset levelfile = Resources.Load<TextAsset> (filename);
		SMBTileMap tileMap = JsonUtility.FromJson<SMBTileMap>(levelfile.text);

		// In the json file, size is in pixels, here we converting it to units 
		// (1 pixel = 100 units)
		tileMap.size *= 0.01f;

		return tileMap;
	}

	public static char[,] ParseLevel(string filename) {

		int width = 0;
		int height = 0;

		TextAsset levelfile = Resources.Load<TextAsset> (filename);

		if (!levelfile) {
			Debug.LogError ("Could not read file");
			return null;
		}
			
		string text = levelfile.text.Remove (levelfile.text.Length - 1);
		string[] lines = text.Split ('\n');

		width = lines [0].Length - 1;
		height = lines.Length;

		foreach (string line in lines) {

			if (line.Length - 1 != width) {
				Debug.LogError ("Level row has different width, they all have to have the same amount of tiles!");
				return null;
			}
		}
			
		char[,] tileMap = new char[height, width] ;

		for (int i = 0; i < height; i++) {

			char[] row = lines [i].ToCharArray ();
			for (int j = 0; j < width; j++)
				tileMap[i,j] = row[j];
		}

		return tileMap;
	}

	public static void CreateColliders(char[,] tileMap, float tileSize, Transform collidersParent) {

		int height = tileMap.GetLength (0);
		int width  = tileMap.GetLength (1);

		// Transfroming array of Dictionaries into a Dictionary
		Dictionary<string, SMBTile> tilesetMapping = new Dictionary<string, SMBTile>();
		foreach (SMBTile tile in SMBGameWorld.Instance.TileMap.tiles)
			tilesetMapping [tile.id] = tile;

		for (int i = 0; i < height; i++) {

			int groundTilesSoFar = 0;

			for (int j = 0; j < width; j++) {

				string tileID = tileMap [i, j].ToString();

				if (!tilesetMapping[tileID].hasCollider || j == width - 1) {

					if (groundTilesSoFar > 0) {

						GameObject newBoxObject = new GameObject ();
						newBoxObject.name = "Collider_" + i + "_" + j;
						newBoxObject.transform.parent = collidersParent;
						newBoxObject.tag = "Platform";

						float lastTileOffset = 0f;
						if (j == width - 1)
							lastTileOffset = 0.5f;

						float offsetX = (((float)j + lastTileOffset) - (float)groundTilesSoFar / 2f) - 0.5f;

						BoxCollider2D box = newBoxObject.AddComponent<BoxCollider2D> (); 

						if (j == width - 1)
							groundTilesSoFar++;

						box.size = new Vector2(groundTilesSoFar, 1f) * tileSize;
						box.offset = new Vector2(offsetX, (height - i)) * tileSize;

						box.usedByEffector = true;
						PlatformEffector2D effector = newBoxObject.AddComponent<PlatformEffector2D> ();
						effector.surfaceArc = SMBConstants.platformCollisionAngle;
					}

					groundTilesSoFar = 0;
				}
				else {

					groundTilesSoFar++;
				}
			}
		}
	}
}


