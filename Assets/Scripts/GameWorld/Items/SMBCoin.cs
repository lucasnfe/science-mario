using UnityEngine;
using System.Collections;

public class SMBCoin : SMBItem {

	override protected void OnInteraction() {

		SMBGameWorld.Instance.PlaySoundEffect ((int)SMBConstants.GameWorldSoundEffects.Coin);
		base.OnInteraction ();
	}
}
