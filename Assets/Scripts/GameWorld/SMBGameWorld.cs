using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (AudioSource))]
public class SMBGameWorld : SMBSingleton<SMBGameWorld> {

	// Custom components
	private AudioSource   _audio;

	// Pointers to main game objects
	private SMBCamera  _camera;
	private SMBPlayer  _player;

	private bool _isReloadingLevel;

	public AudioClip []_soundEffecs;

	// World boundaries
	public float LockLeftX  { get; set; }
	public float LockRightX { get; set; }
	public float LockUpY    { get; set; }
	public float LockDownY  { get; set; }

	// Level representation
	public char[,]  Level   { get; set; }

	public float TileSize   { get; set; }
	public Dictionary<string, SMBTile> TileMap { get; set; }

	void Awake() {

		_audio = GetComponent<AudioSource> ();
	}

	// Use this for initialization
	void Start () {

		SMBTileMap tileMap = SMBLevelParser.ParseTileMap (SMBConstants.tilesDescrition);
		if (tileMap == null)
			return;

		TileSize = tileMap.size;
			
		TileMap = new Dictionary<string, SMBTile>();
		foreach (SMBTile tile in tileMap.tiles)
			TileMap [tile.id] = tile;

		Level = SMBLevelParser.ParseLevel (SMBConstants.levelFilename);
		if (Level == null)
			return;

		// Instantiate the parsed level
		InstantiateLevel();

		if (_player == null)
			return;

		// Camera follow player
		_camera = FindObjectOfType<SMBCamera>();
		if (_camera == null)
			return;
		
		_camera.player = _player;

		// Set Camera position to the players poitions
		_camera.SetCameraPos(_player.transform.position);

		// Set camera locking positions
		int levelWidth = Level.GetLength(1);
		int levelHeight = Level.GetLength(0);

		LockLeftX = TileSize * 0.5f;
		LockRightX = ((float)levelWidth - 1.5f) * TileSize;

		LockDownY = -TileSize * 0.5f;
		LockUpY = ((float)levelHeight + 0.5f) * TileSize;
	}

	void Update() {

		if (_player.State == SMBConstants.PlayerState.Dead && !_isReloadingLevel) {

			PlaySoundEffect ((int)SMBConstants.GameWorldSoundEffects.Death);
			Invoke ("ReloadLevel", SMBConstants.timeToReloadAfterDeath);

			_isReloadingLevel = true;
		}
	}

	public void ReloadLevel() {

		SMBSceneManager.Instance.LoadScene (SceneManager.GetActiveScene ().name);
	}

	void InstantiateLevel() {

		// Transfroming array of Dictionaries into a Dictionary
		GameObject levelParent = new GameObject ();
		levelParent.name = "LevelTiles";
		levelParent.transform.parent = transform.parent;

		for (int i = 0; i < Level.GetLength(0); i++) {

			for (int j = 0; j < Level.GetLength(1); j++) {

				string tileID = Level[i, j].ToString();

				Vector3 position = new Vector2 (j, Level.GetLength(0) - i) * TileSize;
				if (TileMap [tileID].width > 1)
					position.x += TileMap [tileID].width * 0.25f * TileSize;

				position.z = (float)TileMap [tileID].layer;

				if (TileMap.ContainsKey(tileID) && TileMap [tileID].prefab != "") {

					GameObject prefab = Resources.Load<GameObject> (TileMap [tileID].prefab);
					GameObject newTile = Instantiate (prefab, position, Quaternion.identity) as GameObject;
					newTile.name = tileID;

					newTile.transform.parent = levelParent.transform;

					if (TileMap [tileID].isPlayer)
						_player = newTile.GetComponent<SMBPlayer> ();
				}
			}
		}
	}

	public void PlaySoundEffect(int clip) {

		if (clip < _soundEffecs.Length)
			_audio.PlayOneShot (_soundEffecs[clip]);
	}
}
