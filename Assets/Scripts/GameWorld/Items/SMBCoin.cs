using UnityEngine;
using System.Collections;

public class SMBCoin : SMBItem {

	override protected void OnInteraction(Collider2D coll) {

		SMBGameWorld.Instance.PlaySoundEffect ((int)SMBConstants.GameWorldSoundEffects.Coin);
		base.OnInteraction (coll);
	}
}
