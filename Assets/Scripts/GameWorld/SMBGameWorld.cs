using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (AudioSource))]
public class SMBGameWorld : SMBSingleton<SMBGameWorld> {

	private GameObject _levelParent;

	// Custom components
	private AudioSource    	  _audio;
	private Dictionary<string, SMBParticleSystem> _particleSystems;

	// Pointers to main game objects
	private List<GameObject> _gameObjecs;

	private SMBCamera _camera;
	public  SMBCamera Camera { get { return _camera; }}

	private SMBPlayer _player;
	public  SMBPlayer Player { get { return _player; }}

	private bool _isPaused;
	public  bool IsPaused { get { return _isPaused; }}

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

		_particleSystems = new Dictionary<string, SMBParticleSystem>();
		foreach (SMBParticleSystem particleSys in GetComponentsInChildren<SMBParticleSystem> ()) {
			_particleSystems [particleSys.gameObject.name] = particleSys;
			_particleSystems [particleSys.gameObject.name]._shootParticles = false;
		}
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

		// Level parents object
		_levelParent = new GameObject ();
		_levelParent.name = "LevelTiles";
		_levelParent.transform.parent = transform.parent;

		if (!isLevelValid()) {

#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
			return;
		}
		
		InstantiateLevel();

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

		if (_player == null)
			return;

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

				InstantiateTile (position, tileID, j, i);
			}
		}

		PlaceBackground ();
	}

	public GameObject InstantiateTile(Vector3 position, string tileID, int x = 0, int y = 0) {

		GameObject newTile = null;

		if (TileMap.ContainsKey(tileID) && TileMap [tileID].prefab != "") {

			GameObject prefab = Resources.Load<GameObject> (TileMap [tileID].prefab);
			if (prefab != null) {

				position.z = (float)TileMap [tileID].layer;
				position.z += prefab.transform.position.z;

				newTile = Instantiate (prefab, position, Quaternion.identity) as GameObject;
				newTile.name = tileID;
				newTile.transform.parent = _levelParent.transform;

				if (TileMap [tileID].isPlayer)
					_player = newTile.GetComponent<SMBPlayer> ();

				if (TileMap [tileID].isLevelEnd)
					InstantiateEndPoleBar (position, x, y);

				if (newTile.tag != "Untagged")
					_gameObjecs.Add (newTile);
			}
		}

		return newTile;
	}

	private void InstantiateEndPoleBar(Vector3 position, int x, int y) {

		int poleTileHeight = 1;

		for (int i = y; i < Level.GetLength (0); i++) {
			string tileID = Level [i, x].ToString ();

			if (tileID == "f")
				poleTileHeight++;
		}

		GameObject prefab = Resources.Load<GameObject> (SMBConstants.endPoleBar);
		position = position - Vector3.up * poleTileHeight * TileSize;
		position.z -= 1;
		position.y += TileSize * 0.5f;

		GameObject newTile = Instantiate (prefab, position, Quaternion.identity) as GameObject;
		newTile.name = "!";
		newTile.transform.parent = _levelParent.transform;

		SMBEndPoleBar endPolebar = newTile.GetComponent<SMBEndPoleBar> ();
		endPolebar._bounceYDist = (poleTileHeight - 1) * TileSize;

		GameObject endLevelTrigger = new GameObject ();
		endLevelTrigger.name = "EndLevelTrigger";
		endLevelTrigger.tag = "End";

		Vector3 endLevelPosition = new Vector3(position.x, (Level.GetLength (0) * 0.5f) * TileSize, 0f);
		endLevelTrigger.transform.position = endLevelPosition;
		endLevelTrigger.transform.parent = transform;

		BoxCollider2D collider = endLevelTrigger.AddComponent<BoxCollider2D> ();
		collider.isTrigger = true;
		collider.size = new Vector2 (TileSize, Level.GetLength(0) * TileSize);
	}

	private bool isLevelValid() {

		int playerAmount = 0;
		int endPoleAmount = 0;

		for (int i = 0; i < Level.GetLength(0); i++) {

			for (int j = 0; j < Level.GetLength(1); j++) {

				string tileID = Level[i, j].ToString();

				if (tileID == "m")
					playerAmount++;

				if (tileID == "v")
					endPoleAmount++;
			}
		}

		if (playerAmount != 1) {
			Debug.LogError ("This level does not have a player. Levels must have one and only one player.");
			return false;
		}

		if (endPoleAmount != 1) {
			Debug.LogError ("This level does not have an end. Levels must have one and only one end.");
			return false;
		}

		return true;
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

	void PlaceBackground() {

		float levelWidth = Level.GetLength(1) * TileSize;

		float backgroundWidth = _background.GetComponent<SpriteRenderer> ().bounds.size.x;
		float backgroundHeight = _background.GetComponent<SpriteRenderer> ().bounds.size.y;

		float rate = levelWidth / backgroundWidth;
		int amount = (int)rate + 1;

		Vector3 pos = _background.transform.position;
		pos.x = backgroundWidth/3f;
		pos.y = backgroundHeight / 2f + TileSize / 2f;

		for (int i = 0; i < amount; i++) {

			Instantiate (_background, pos, Quaternion.identity);
			pos += Vector3.right * backgroundWidth; 
		}
	}

	public void PlayParticle(Vector3 position, string particleSystemName) {
	
		_particleSystems[particleSystemName].transform.position = position;
		_particleSystems[particleSystemName]._shootParticles = true;
	}

	public void PlaySoundEffect(int clip) {

		if (clip < _soundEffecs.Length)
			_audio.PlayOneShot (_soundEffecs[clip]);
	}
}
