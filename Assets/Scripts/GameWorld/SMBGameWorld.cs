using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class TilesetMapEntry {

	public int tileID;
	public GameObject prefab;
}

public class SMBGameWorld : SMBSingleton<SMBGameWorld> {

	public SMBCamera _camera;
	public SMBPlayer _player;

	public float LockLeftX  { get; set; }
	public float LockRightX { get; set; }
	public float LockUpY    { get; set; }
	public float LockDownY  { get; set; }

	public TilesetMapEntry[] _tilesetMapping;

	// Use this for initialization
	void Start () {

		int[,] tileMap = SMBLevelParser.Parse (SMBConstants.LevelFilename);

		if (tileMap == null)
			return;

		// Instantiate the parsed level
		InstantiateLevel(tileMap);

		// Create colliders for this level
		GameObject levelColliders = new GameObject ();
		levelColliders.name = "SMBColliders";
		levelColliders.transform.parent = transform;

		SMBLevelPosProcessing.CreateColliders(tileMap, levelColliders.transform);

		// Set camera locking positions
		int levelWidth = tileMap.GetLength(1);

		LockLeftX = SMBConstants.tileSize * 0.5f;
		LockRightX = ((float)levelWidth - 1.5f) * SMBConstants.tileSize;
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

				Vector2 position = new Vector2 (j, tileMap.GetLength(0) - i) * SMBConstants.tileSize;

				if (tilesetMapping.ContainsKey(tileID) && tilesetMapping [tileID] != null) {

					GameObject newTile = Instantiate (tilesetMapping [tileID], position, Quaternion.identity) as GameObject;
					newTile.transform.parent = levelParent.transform;
				}
			}
		}

	}
}
