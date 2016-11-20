using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SpriteRenderer))]
public class SMBParticle : SMBRigidBody {

	private float _lifetime;
	private bool  _isDying;

	private SpriteRenderer 	 _renderer;
	private SMBParticleSystem _emitter;

	public void Create (SMBParticleSystem emitter) {

		_renderer = GetComponent<SpriteRenderer> ();
		_emitter = emitter;
	}

	// Update is called once per frame
	protected override void Update () {

		base.Update ();

		if (_emitter != null)
			applyGravity = _emitter._applyGravity;

		if(_isDying) {

			_lifetime += 10f * Time.deltaTime;

			Color rendColor = _renderer.color;
			rendColor.a = Mathf.Lerp(rendColor.a, 0f, 10f * Time.deltaTime);
			_renderer.color = rendColor;

			if (_lifetime >= 1f)
				_emitter.KillParticle (this);
		}
	}

	public void Shoot(Vector2 velocity, float lifetime, bool applyGravity, float gravityFactor, Bounds bounds) {

		_isDying = false;

		this.velocity = velocity;
		this.gravityFactor = gravityFactor;
		this.applyGravity = applyGravity;

		Vector3 noise = Vector3.zero;
		noise.x = Random.Range(bounds.min.x, bounds.max.x);
		noise.y = Random.Range(bounds.min.y, bounds.max.y);

		transform.position = _emitter.transform.position + bounds.center + noise;

		gameObject.SetActive (true);

		Color rendColor = _renderer.color;
		rendColor.a = 1f;
		_renderer.color = rendColor;

		if(lifetime > 0f)
			Invoke ("Kill", lifetime);
	}

	public void Kill() {

		_isDying = true;
	}
}
