using UnityEngine;
using System.Collections;

public class SMBBlockQuestion : SMBBlock {

	public string prize;
	private GameObject _prizeObject;

	void Start() {

		_prizeObject = SMBGameWorld.Instance.InstantiateTile (transform.position, prize);

		if (_prizeObject != null) {
			_prizeObject.SendMessage ("OnSpawnStart", SendMessageOptions.DontRequireReceiver);
			_prizeObject.SetActive (false);
		}
	}

	override protected void DestroyBlock (SMBPlayer player) {

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
