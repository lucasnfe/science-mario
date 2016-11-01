using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TilesetMapEntry {

	public char tileID;
	public GameObject prefab;
}

public class SMBGameWorld : SMBSingleton<SMBGameWorld> {

	private SMBCamera _camera;
	private SMBPlayer _player;
	private char[,] tileMap;

	public float LockLeftX  { get; set; }
	public float LockRightX { get; set; }
	public float LockUpY    { get; set; }
	public float LockDownY  { get; set; }

	public TilesetMapEntry[] _tilesetMapping;

	// Use this for initialization
	void Start () {

		tileMap = SMBLevelParser.Parse (SMBConstants.LevelFilename);
		if (tileMap == null)
			return;

		// Instantiate the parsed level
		InstantiateLevel(tileMap);

		// Camera follow player
		_camera = FindObjectOfType<SMBCamera>();
		if (_camera == null)
			return;
		
		_camera.player = _player.gameObject;

		// Create colliders for this level
		GameObject levelColliders = new GameObject ();
		levelColliders.name = "SMBColliders";
		levelColliders.transform.parent = transform;

		SMBLevelParser.CreateColliders(tileMap, levelColliders.transform);

		// Set Camera position to the players poitions
		_camera.SetCameraPos(_player.transform.position);

		// Set camera locking positions
		int levelWidth = tileMap.GetLength(1);
		int levelHeight = tileMap.GetLength(0);

		LockLeftX = SMBConstants.tileSize * 0.5f;
		LockRightX = ((float)levelWidth - 1.5f) * SMBConstants.tileSize;

		LockDownY = -SMBConstants.tileSize * 0.5f;
		LockUpY = (float)levelHeight * SMBConstants.tileSize;
	}

	void Update() {

		// Kill the player if it is below the camera y limits
		if (_player.transform.position.y < 0.0f)
			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);

	}

	void InstantiateLevel(char[,] tileMap) {

		// Transfroming array of Dictionaries into a Dictionary
		Dictionary<char, GameObject> tilesetMapping = new Dictionary<char, GameObject>();
		foreach (TilesetMapEntry entry in _tilesetMapping)
			tilesetMapping [entry.tileID] = entry.prefab;

		GameObject levelParent = new GameObject ();
		levelParent.name = "LevelTiles";
		levelParent.transform.parent = transform.parent;

		for (int i = 0; i < tileMap.GetLength(0); i++) {

			for (int j = 0; j < tileMap.GetLength(1); j++) {

				char tileID = tileMap [i, j];

				Vector2 position = new Vector2 (j, tileMap.GetLength(0) - i) * SMBConstants.tileSize;

				if (tilesetMapping.ContainsKey(tileID) && tilesetMapping [tileID] != null) {

					GameObject newTile = Instantiate (tilesetMapping [tileID], position, Quaternion.identity) as GameObject;
					newTile.transform.parent = levelParent.transform;

					if (tileID == 'm')
						_player = newTile.GetComponent<SMBPlayer> ();
				}
			}
		}

	}
}
