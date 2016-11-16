using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SMBParticleManager : SMBSingleton<SMBParticleManager> {

	private SMBObjectPool<SMBParticleSystem>  _slugParticleSystems;

	// Use this for initialization
	void Start () {

		_slugParticleSystems = new SMBObjectPool<SMBParticleSystem>(30);
	}

	public SMBParticleSystem AddParticleSystem(SMBParticleSystem clonePart, Vector3 position) {

		SMBParticleSystem particleSys = _slugParticleSystems.GetFreeObject ();
		particleSys.transform.position = position;

		// Randomly assign particle system attributes
		particleSys._minVel = clonePart._minVel;
		particleSys._maxVel = clonePart._maxVel;
		particleSys._minAngle = clonePart._minAngle;
		particleSys._maxAngle = clonePart._maxAngle;
		particleSys._minMass = clonePart._minMass;
		particleSys._maxMass = clonePart._maxMass;
		particleSys._minLifetime = clonePart._minLifetime;
		particleSys._maxLifetime = clonePart._maxLifetime;

		particleSys._systemLifetime = clonePart._systemLifetime;
		particleSys._shootingRate = clonePart._shootingRate;

		particleSys._shootParticles = clonePart._shootParticles;
		particleSys._applyGravity = clonePart._applyGravity;

		particleSys._particleSprites = clonePart._particleSprites;

		return particleSys;
	}
}
