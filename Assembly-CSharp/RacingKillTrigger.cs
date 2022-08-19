using Guardian;
using UnityEngine;

public class RacingKillTrigger : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8 && IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
		{
			HERO component = other.gameObject.transform.root.gameObject.GetComponent<HERO>();
			if (!(component == null) && component.photonView.isMine && !component.HasDied())
			{
				component.MarkDead();
				component.photonView.RPC("netDie2", PhotonTargets.All, -1, GuardianClient.Properties.LavaDeathMessage.Value);
			}
		}
	}
}
