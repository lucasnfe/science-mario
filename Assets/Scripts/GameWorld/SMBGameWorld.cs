using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (AudioSource))]
[RequireComponent(typeof(SMBParticleSystem))]
public class SMBGameWorld : SMBSingleton<SMBGameWorld> {

	private GameObject _levelParent;

	// Custom components
	private AudioSource    	  _audio;
	private SMBParticleSystem _particleSystem;

	// Pointers to main game objects
	private List<GameObject> _gameObjecs;

	private SMBCamera  _camera;
	public SMBCamera Camera { get { return _camera; }}

	private SMBPlayer  _player;
	public SMBPlayer Player { get { return _player; }}

	private bool _isPaused;
	public bool IsPaused { get { return _isPaused; }}

	private bool _isReloadingLevel;

	// Custom parameters
	public AudioSource  _theme;
	public AudioClip  []_soundEffecs;

	public GameObject _background;

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
		_particleSystem = GetComponent<SMBParticleSystem> ();
	}

	// Use this for initialization
	void Start () {

		_particleSystem._shootParticles = false;

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

		// Level parents object
		_levelParent = new GameObject ();
		_levelParent.name = "LevelTiles";
		_levelParent.transform.parent = transform.parent;

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

			_theme.Stop ();

			PauseGame (false);

			PlaySoundEffect ((int)SMBConstants.GameWorldSoundEffects.Death);
			Invoke ("ReloadLevel", SMBConstants.timeToReloadAfterDeath);

			_isReloadingLevel = true;
		}
	}

	public void ReloadLevel() {

		SMBSceneManager.Instance.LoadScene (SceneManager.GetActiveScene ().name);
	}

	void InstantiateLevel() {

		_gameObjecs = new List<GameObject> ();

		for (int i = 0; i < Level.GetLength(0); i++) {

			for (int j = 0; j < Level.GetLength(1); j++) {

				string tileID = Level[i, j].ToString();

				Vector3 position = new Vector2 (j, Level.GetLength(0) - i) * TileSize;
				if (TileMap [tileID].width > 1)
					position.x += TileMap [tileID].width * 0.25f * TileSize;

				GameObject obj = InstantiateTile (position, tileID);
				if (obj != null && obj.tag != "Untagged")
					_gameObjecs.Add (obj);
			}
		}

		PlaceBackground ();
	}

	public void PauseGame(bool pausePlayer = true) {

		foreach (GameObject go in _gameObjecs) {

			if (go == null)
				continue;

			if (!pausePlayer && go.tag == "Player")
				continue;
				
			go.SendMessage ("OnPauseGame", SendMessageOptions.DontRequireReceiver);
		}

		_isPaused = true;
	}

	public void ResumeGame() {

		foreach (GameObject go in _gameObjecs) {

			if (go == null)
				continue;
			
			go.SendMessage ("OnResumeGame", SendMessageOptions.DontRequireReceiver);
		}

		_isPaused = false;
	}

	public GameObject InstantiateTile(Vector3 position, string tileID) {

		GameObject newTile = null;

		if (TileMap.ContainsKey(tileID) && TileMap [tileID].prefab != "") {

			position.z = (float)TileMap [tileID].layer;

			GameObject prefab = Resources.Load<GameObject> (TileMap [tileID].prefab);
			newTile = Instantiate (prefab, position, Quaternion.identity) as GameObject;
			newTile.name = tileID;

			newTile.transform.parent = _levelParent.transform;

			if (TileMap [tileID].isPlayer)
				_player = newTile.GetComponent<SMBPlayer> ();
		}

		return newTile;
	}

	void PlaceBackground() {

		float levelWidth = Level.GetLength(1) * TileSize;

		float backgroundWidth = _background.GetComponent<SpriteRenderer> ().bounds.size.x;
		float backgroundHeight = _background.GetComponent<SpriteRenderer> ().bounds.size.y;

		float rate = levelWidth / backgroundWidth;
		int amount = (int)rate + 1;

		Vector3 pos = _background.transform.position;
		pos.x = 0f;
		pos.y = backgroundHeight / 2f + TileSize / 2f;

		for (int i = 0; i < amount; i++) {

			Instantiate (_background, pos, Quaternion.identity);
			pos += Vector3.right * backgroundWidth; 
		}
	}

	public void PlayParticle(Vector3 position) {
	
		_particleSystem.transform.position = position;
		_particleSystem._shootParticles = true;
	}

	public void PlaySoundEffect(int clip) {

		if (clip < _soundEffecs.Length)
			_audio.PlayOneShot (_soundEffecs[clip]);
	}
}
