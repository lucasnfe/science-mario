using UnityEngine;
using System.Collections;

public class SMBPipe : MonoBehaviour {

	public string enemy;
	private GameObject _enemyObject;

	void Start() {

		_enemyObject = SMBGameWorld.Instance.InstantiateTile (transform.position, enemy);

		if (_enemyObject != null) {
			_enemyObject.SendMessage ("OnSpawnStart", this.gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
}
