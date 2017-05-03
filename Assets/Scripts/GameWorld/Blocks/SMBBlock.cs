using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class SMBBlock : MonoBehaviour {

	enum BounceState {
		None,
		Up,
		Down
	}
		
	private Vector3 _posBeforeBounce;
	private BounceState _bounceState;

	protected bool  _isDestroyed;

	protected Animator       _animator;
	protected BoxCollider2D  _collider;

	public float _bounceVelocity = 1f;
	public float _bounceYDist = 0.15f;

	void Awake() {

		_animator = GetComponent<Animator> ();
		_collider = GetComponent<BoxCollider2D> ();
	}

	void Update() {

		if (_bounceState != BounceState.None)
			Bounce ();
	}

	void OnInteraction(SMBPlayer player) {

		if (_bounceState == BounceState.None && !_isDestroyed) {
				
			DestroyBlock (player);

			_posBeforeBounce = transform.position;
			_bounceState = BounceState.Up;
		}
	}

	private void Bounce() {

		Vector3 currentPos = transform.position;

		if (_bounceState == BounceState.Up) {

			if (currentPos.y <= _posBeforeBounce.y + _bounceYDist) {

				transform.Translate (_bounceVelocity * Vector2.up * Time.fixedDeltaTime);
			}
			else  {
				
				_bounceState = BounceState.Down;
			}
		}
		else if (_bounceState == BounceState.Down) {

			if (currentPos.y >= _posBeforeBounce.y) {

				transform.Translate (_bounceVelocity * Vector2.down * Time.fixedDeltaTime);
			}
			else {

				_bounceState = BounceState.None;
				transform.position = _posBeforeBounce;

				GivePrize ();
			}
		}
	}
		
	protected virtual void DestroyBlock (SMBPlayer player) {
		
	}

	protected virtual void GivePrize () {

	}

	void OnPauseGame() {

		_animator.enabled = false;
	}

	void OnResumeGame() {

		_animator.enabled = true;
	}
}
