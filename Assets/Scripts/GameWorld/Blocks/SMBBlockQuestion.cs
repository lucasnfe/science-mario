using UnityEngine;
using System.Collections;

public class SMBBlockQuestion : SMBBlock {

	override protected void DestroyBlock () {

		_isDestroyed = true;
		_animator.SetTrigger ("triggerDestroy");
	}
}
