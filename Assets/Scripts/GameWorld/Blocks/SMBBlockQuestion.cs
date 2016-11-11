using UnityEngine;
using System.Collections;

public class SMBBlockQuestion : SMBBlock {

	override protected void DestroyBlock () {

		_animator.SetTrigger ("triggerDestroy");
	}
}
