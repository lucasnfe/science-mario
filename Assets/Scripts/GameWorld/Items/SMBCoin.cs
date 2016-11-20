using UnityEngine;
using System.Collections;

public class SMBCoin : SMBItem {

	override protected void OnInteraction() {

		SMBGameWorld.Instance.PlayParticle (transform.position);
		SMBGameWorld.Instance.PlaySoundEffect ((int)SMBConstants.GameWorldSoundEffects.Coin);

		base.OnInteraction ();
	}
}
