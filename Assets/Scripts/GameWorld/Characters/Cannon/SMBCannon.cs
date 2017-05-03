using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]
public class SMBCannon : MonoBehaviour {

	public bool stopShooting = false;

	public GameObject []bullets;
	public float bulletFrequency = 1f;

	public float bulletSpeed = 1f;
	public SMBConstants.MoveDirection direction;

	private float bulletTimer;

	private SMBObjectPool<SMBBullet> _bullets;
	public  SMBObjectPool<SMBBullet> Bullets { get { return _bullets; } }

	private AudioSource  _audio;

	void Awake() {

		_audio = GetComponent<AudioSource> ();
	}

	// Use this for initialization
	void Start () {
	
		_bullets = new SMBObjectPool<SMBBullet> (25, bullets, InitObject);

		foreach (SMBBullet bullet in _bullets.GetAllObjects()) {
			SMBGameWorld.Instance.AddGameObject (bullet.gameObject);
		}
	}

	// Update is called once per frame
	void Update () {

		if (stopShooting)
			return;
		
		bulletTimer += Time.deltaTime;
		if (bulletTimer >= bulletFrequency) {

			Shoot ();
			bulletTimer = 0f;
		}
	}

	void Shoot() {

		SMBBullet bullet = _bullets.GetFreeObject();
		if (bullet) {

			Vector3 position = bullet.transform.position;
			position.x = transform.position.x;
			position.y = transform.position.y;
			position.z = -8;

			bullet.Init (bulletSpeed, direction, position);
			bullet.cannon = this;

			_audio.Play ();
		}
	}

	public void InitObject(ref SMBBullet bullet) {

		bullet.cannon = this;
	}

	void OnPauseGame() {

		stopShooting = true;
	}

	void OnResumeGame() {

		stopShooting = false;
	}
}
