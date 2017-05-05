﻿using UnityEngine;
using System.Collections;

public class SMBKoopa : SMBEnemy {

	public string sheelID;
	private GameObject _shell;

	// Use this for initialization
	override protected void Start() {
	
		base.Start ();

		_shell = SMBGameWorld.Instance.InstantiateTile (transform.position, sheelID);

		if (_shell != null) {
			_shell.SendMessage ("OnSpawnStart", SendMessageOptions.DontRequireReceiver);
			_shell.SetActive (false);
		}
	}
	
	override protected void Die() {

		Vector3 position = _shell.transform.position;
		position.x = transform.position.x;
		position.y = transform.position.y;

		_shell.transform.position = position;
		_shell.SetActive (true);

		DestroyCharacter ();
	}
}
