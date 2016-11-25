using UnityEngine;
using System.Collections;

public class SMBBlockQuestion : SMBBlock {

	public string prize;
	private GameObject _prizeObject;

	void Start() {

		_prizeObject = SMBGameWorld.Instance.InstantiateTile (transform.position + Vector3.up * 0.15f, prize);
		if (_prizeObject != null)
			_prizeObject.SetActive (false);
	}

	override protected void DestroyBlock () {

		_isDestroyed = true;
		_animator.SetTrigger ("triggerDestroy");
	}

	override protected void GivePrize() {

		if (_prizeObject != null) {
			_prizeObject.SetActive (true);
			_prizeObject.SendMessage ("OnSpawn", SendMessageOptions.DontRequireReceiver);
		}
	}
}
