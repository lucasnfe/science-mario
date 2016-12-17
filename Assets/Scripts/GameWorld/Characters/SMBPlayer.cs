using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SMBParticleSystem))]
public class SMBPlayer : SMBCharacter {

	enum SoundEffects {
		Jump,
		Kick,
		GrowUp
	}

	// Custom components
	private SMBParticleSystem _particleSystem;
		
	private bool 	_isInvincible;
	private bool    _lockController;
	private float   _jumpTimer;
	private float   _runningTimer;
	private float   _blinkTimer;
	private int     _blinkAmount;
	private Bounds  _originalCollider;
	private Vector2 _velocityBeforeGrowUp;

	private SMBConstants.PlayerState _state;
	public SMBConstants.PlayerState State { get { return _state; } }

	public float blinkTime = 0.1f;
	public float runTime = 1f;
	public float longJumpTime = 1f;
	public float longJumpWeight = 0.1f;
	public float runningMultiplyer = 2f;
	public float minVelocityToCoast = 0.25f;

	public Bounds grownUpColliderSize;

	public AudioClip[] soundEffects;

	override protected void Awake() {

		_particleSystem = GetComponent<SMBParticleSystem> ();
		base.Awake ();
	}

	void Start() {

		_state = SMBConstants.PlayerState.Short;
		_particleSystem._shootParticles = false;

		_originalCollider = _collider.GetSize();
		_originalCollider.center = Vector3.zero;
	}

	// Update is called once per frame
	override protected void Update () {

		if (_lockController)
			return;

		Jump ();
		_animator.SetBool ("isJumping", !_isOnGround);

		float speed = DefineMoveSpeed ();

		if (Input.GetKey (KeyCode.LeftArrow)) {

			Move (speed * (float)SMBConstants.MoveDirection.Backward);

			_animator.SetInteger ("move", 1);

			if(speed > xSpeed)
				_animator.SetInteger ("move", 2);

			if (_runningTimer >= runTime)
				_animator.SetInteger ("move", 3);

			if (_isOnGround && Mathf.Abs (_body.velocity.x) > minVelocityToCoast && Mathf.Sign (_body.velocity.x) == 1f) {

				_animator.SetBool ("isCoasting", true);
				_particleSystem._shootParticles = true;
				_runningTimer = 0f;
			}

		} 
		else if (Input.GetKey (KeyCode.RightArrow)) {

			Move (speed * (float)SMBConstants.MoveDirection.Forward);

			_animator.SetInteger ("move", 1);

			if(speed > xSpeed)
				_animator.SetInteger ("move", 2);

			if (_runningTimer >= runTime)
				_animator.SetInteger ("move", 3);			

			if (_isOnGround && Mathf.Abs (_body.velocity.x) > minVelocityToCoast && Mathf.Sign (_body.velocity.x) == -1f) {

				_animator.SetBool ("isCoasting", true);
				_particleSystem._shootParticles = true;
				_runningTimer = 0f;
			}
		} 
		else {

			_body.velocity.x = Mathf.Lerp (_body.velocity.x, 0f, momentum * Time.fixedDeltaTime);
			_animator.SetInteger ("move", 1);

			if (Mathf.Abs (_body.velocity.x) <= 0.1f) {

				_animator.SetInteger ("move", 0);
				_body.velocity.x = 0f;
			}

			_runningTimer = 0f;
		}

		if (Mathf.Abs (_body.velocity.x) <= 0.1f && _animator.GetBool ("isCoasting")) {

			_animator.SetBool ("isCoasting", false);
			_particleSystem._shootParticles = false;
		}

		SetInvincible (_isInvincible);
			
		if (transform.position.y < -0.2f)
			Die (0.4f, false);

		base.Update ();
	}

	float DefineMoveSpeed() {

		float speed = xSpeed;
		if (Input.GetKey (KeyCode.A)) {

			speed *= runningMultiplyer;

			if (_isOnGround)
				_runningTimer += Time.fixedDeltaTime;

			_runningTimer = Mathf.Clamp (_runningTimer, 0f, runTime);

			if (_runningTimer >= runTime)
				speed *= runningMultiplyer * 0.625f;
		} 
		else if (Input.GetKeyUp (KeyCode.A)) {

			_runningTimer = 0f;
		}

		return speed;
	}

	void Blink() {

		_blinkTimer += Time.fixedDeltaTime;

		if (_blinkTimer >= blinkTime) {
			_renderer.enabled = !_renderer.enabled;
			_blinkTimer = 0f;
			_blinkAmount++;
		}

		if (_blinkAmount == 10)
			blinkTime *= 0.8f;

		else if (_blinkAmount == 20)
			blinkTime *= 0.8f;

		else if (_blinkAmount >= 40) {

			_blinkAmount = 0;
			_blinkTimer = 0f;

			_isInvincible = false;
			_renderer.enabled = true;
		}
	}

	void SetInvincible(bool invincible) {

		int enemies = LayerMask.NameToLayer ("Enemy");

		if (invincible) {

			Blink ();

			_collider.SetIsTrigger (true);
			_collider.horizontalMask &= ~(1 << enemies);
		} 
		else {
			
			_collider.SetIsTrigger (false);
			_collider.horizontalMask |= (1 << enemies);
		}
			
	}

	void Die(float timeToDie, bool animate = true) {

		_state = SMBConstants.PlayerState.Dead;

		_lockController = true;

		_particleSystem._shootParticles = false;

		_collider.applyHorizCollision = false;
		_collider.applyVertCollision = false;

		gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");

		_body.velocity = Vector2.zero;
		_body.acceleration = Vector2.zero;
		_body.applyGravity = false;

		_animator.SetTrigger ("triggerDie");

		if(animate)
			Invoke("PlayDeadAnimation", timeToDie);
	}

	void PlayDeadAnimation() {

		_body.applyGravity = true;
		_body.gravityFactor = 0.5f;
		_body.ApplyForce (Vector2.up * 2.5f);
	}
				
	void Jump() {

		if (_isOnGround && Input.GetKeyDown(KeyCode.S)){

			_jumpTimer = longJumpTime;
			_body.velocity.y = ySpeed * Time.fixedDeltaTime;

			_audio.PlayOneShot (soundEffects[(int)SoundEffects.Jump]);
		}

		if (_jumpTimer > 0f) {

			if (Input.GetKeyUp(KeyCode.S)) {

				_jumpTimer = 0f;

			}
			else if(_body.velocity.y > 0f && Input.GetKey(KeyCode.S)) {

				float runningBoost = 1f;
				if (_runningTimer >= runTime)
					runningBoost = 1.5f;

				_jumpTimer -= Time.fixedDeltaTime;
				if (_jumpTimer <= longJumpTime/2f)
					_body.velocity.y += ySpeed * longJumpWeight * runningBoost * Time.fixedDeltaTime;
			}
		}
	}

	void GrowUp() {

		if (_state == SMBConstants.PlayerState.GrownUp)
			return;	

		SMBGameWorld.Instance.PauseGame (false);

		_animator.SetTrigger("triggerGrownUp");
		_animator.SetLayerWeight (0, 0);
		_animator.SetLayerWeight (1, 1);

		_collider.SetSize (grownUpColliderSize);

		_velocityBeforeGrowUp = _body.velocity;

		_lockController = true;
		_body.applyGravity = false;
		_body.velocity = Vector2.zero;

		_audio.PlayOneShot (soundEffects[(int)SoundEffects.GrowUp]);

		_state = SMBConstants.PlayerState.GrownUp;
	}

	void TakeDamage() {

		SMBGameWorld.Instance.PauseGame (false);

		_animator.SetTrigger("triggerDamage");
		_animator.SetLayerWeight (0, 1);
		_animator.SetLayerWeight (1, 0);

		_collider.SetSize (_originalCollider);

		_lockController = true;
		_isInvincible = true;

		_body.applyGravity = false;
		_body.velocity = Vector2.zero;
		_velocityBeforeGrowUp = Vector2.zero;

		_audio.PlayOneShot (soundEffects[(int)SoundEffects.GrowUp]);

		_state = SMBConstants.PlayerState.Short;
	}

	void UnlockController() {

		_lockController = false;
		_body.applyGravity = true;
		_body.velocity = _velocityBeforeGrowUp;

		SMBGameWorld.Instance.ResumeGame();
	}

	void KillEnemy(GameObject enemy) {

		_body.acceleration = Vector2.zero;
		_body.velocity.y = 0f;

		_body.ApplyForce (Vector2.up * 2.5f);
		_audio.PlayOneShot (soundEffects[(int)SoundEffects.Kick]);

		enemy.SendMessage ("Die", SendMessageOptions.DontRequireReceiver);
	}

	void SolveVerticalCollision(Collider2D collider) {

		if (collider.tag == "Block") {

			if (collider.bounds.center.y > transform.position.y)
				collider.SendMessage ("OnInteraction", SendMessageOptions.DontRequireReceiver);
		}
	}
		
	override protected void OnHalfVerticalCollisionEnter(Collider2D collider) {

		SolveVerticalCollision (collider);
		base.OnHalfVerticalCollisionEnter (collider);
	}

	override protected void OnFullVerticalCollisionEnter(Collider2D collider) {

		SolveVerticalCollision (collider);
		base.OnFullVerticalCollisionEnter (collider);
	}

	void OnVerticalTriggerEnter(Collider2D collider) {

		if (collider.tag == "Item") {

			collider.SendMessage ("OnInteraction", SendMessageOptions.DontRequireReceiver);

			if (collider.name == "r")
				GrowUp ();
		}
		else if (collider.tag == "Enemy") {

			KillEnemy (collider.gameObject);
		}
	}

	void OnHorizontalCollisionEnter(Collider2D collider) {

		_runningTimer = 0f;
	}

	void OnHorizontalTriggerEnter(Collider2D collider) {

		if (collider.tag == "Item") {

			collider.SendMessage ("OnInteraction", SendMessageOptions.DontRequireReceiver);

			if (collider.name == "r")
				GrowUp ();
		}
		else if (collider.tag == "Enemy") {

			if (transform.position.y > collider.transform.position.y + 0.1f) {

				KillEnemy (collider.gameObject);
				return;
			}
				
			if (_state == SMBConstants.PlayerState.GrownUp)

				TakeDamage ();
			else
				Die (0.2f);
		}
	}
}
