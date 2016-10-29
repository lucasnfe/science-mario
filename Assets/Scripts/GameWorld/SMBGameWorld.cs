using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SMBGameWorld : SMBSingleton<SMBGameWorld> {

	[System.Serializable]
	public class TilesetMapEntry {

		public int tileID;
		public GameObject prefab;
	}

	public TilesetMapEntry[] _tilesetMapping;

	// Use this for initialization
	void Start () {

		int[,] tileMap = SMBLevelParser.Parse (SMBConstants.LevelFilename);

		if (tileMap == null)
			return;

		// Instantiate the parsed level
		InstantiateLevel(tileMap);
	}

	void InstantiateLevel(int[,] tileMap) {

		// Transfroming array of Dictionaries into a Dictionary
		Dictionary<int, GameObject> tilesetMapping = new Dictionary<int, GameObject>();
		foreach (TilesetMapEntry entry in _tilesetMapping)
			tilesetMapping [entry.tileID] = entry.prefab;

		GameObject levelParent = new GameObject ();
		levelParent.name = "LevelTiles";
		levelParent.transform.parent = transform.parent;

		for (int i = 0; i < tileMap.GetLength(0); i++) {

			for (int j = 0; j < tileMap.GetLength(1); j++) {

				int tileID = tileMap [i, j];

				Vector2 position = new Vector2 (tileMap.GetLength(1) - j, tileMap.GetLength(0) - i) * SMBConstants.tileSize;

				if (tilesetMapping.ContainsKey(tileID) && tilesetMapping [tileID] != null) {

					GameObject newTile = Instantiate (tilesetMapping [tileID], position, Quaternion.identity) as GameObject;
					newTile.transform.parent = levelParent.transform;
				}
			}
		}

	}
}
