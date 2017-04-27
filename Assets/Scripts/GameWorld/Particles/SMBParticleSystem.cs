using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SMBParticleSystem : MonoBehaviour {

	public float  _minVel, _maxVel;
	public float  _minAngle, _maxAngle;
	public float  _minMass, _maxMass;
	public float  _minLifetime, _maxLifetime;
	public float  _systemLifetime = 1f;
	public float  _shootingRate   = 1f;
	public float  _gravityFactor  = 1f;
	public bool   _applyGravity;
	public bool   _shootParticles;

	public Bounds _bounds;

	public GameObject[] _particleSprites;

	private float 	  _shootingTimer;
	private float 	  _selfDestructionTimer;

	private SMBObjectPool<SMBParticle> _particles;

	private void Start () {

		_particles = new SMBObjectPool<SMBParticle>(25, _particleSprites, InitParticle);
		_shootingTimer = _shootingRate;
	}

	private void Update () {

		if (_shootParticles) {

			if (_systemLifetime > 0) {

				_selfDestructionTimer += Time.deltaTime;
				if (_selfDestructionTimer >= _systemLifetime) {

					_selfDestructionTimer = 0f;
					_shootParticles = false;
				}
			}

			_shootingTimer += Time.deltaTime;

			if (_shootingTimer >= _shootingRate) {

				ShootParticle ();
				_shootingTimer = 0f;
			}
		}
	}

	private void InitParticle(SMBParticle particle) {

		particle.Create (this);
	}

	public SMBParticle ShootParticle() {

		SMBParticle inactiveParticle = _particles.GetFreeObject();

		if (inactiveParticle != null) {

			float randVel = Random.Range (_minVel, _maxVel);
			float randAng = Random.Range (_minAngle, _maxAngle) * Mathf.Deg2Rad;
			float lifetime = Random.Range (_minLifetime, _maxLifetime);

			Vector2 velocity = new Vector2 (Mathf.Cos(randAng), Mathf.Sin(randAng)) * randVel;

			inactiveParticle.Shoot (velocity, lifetime, _applyGravity, _gravityFactor, _bounds);
		}

		return inactiveParticle;
	}

	public void SetParciclesParent(Transform parent) {

		_particles.SetObjectsParent (parent);
	}

	public List<SMBParticle> GetUsedParticles() {

		return _particles.GetUsedObjects ();
	}

	public void KillAllParticles() {

		_particles.FreeAllObjects ();
	}

	public void KillParticle(SMBParticle particle) {

		_particles.SetFreeObject (particle.gameObject);
	}
}
