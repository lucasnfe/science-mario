using UnityEngine;
using System.Collections;

public class SMBEndPoleBar : MonoBehaviour {

	enum BounceState {
		Up,
		Down
	}

	private Vector3 _posBeforeBounce;
	private BounceState _bounceState;

	public float _bounceVelocity = 1f;
	public float _bounceYDist = 1f;

	void Start() {

		_posBeforeBounce = transform.position;
		_bounceState = BounceState.Up;
	}

	void Update() {
		
		Bounce ();
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

				_bounceState = BounceState.Up;
				transform.position = _posBeforeBounce;
			}
		}
	}
}

