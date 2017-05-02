using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SMBCollision {

	public bool top = true;
	public bool right = true;
	public bool bottom = true;
	public bool left = true;
}

[System.Serializable]
public class SMBTile {

	public string id;
	public string prefab;
	public bool   isPlayer;
	public bool   isLevelEnd;
	public bool   isGround;
	public int    layer = 0;
	public int 	  width = 1;
	public SMBCollision collisions;
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
}


