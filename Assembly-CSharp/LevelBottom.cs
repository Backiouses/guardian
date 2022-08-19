using Guardian;
using UnityEngine;

public class LevelBottom : MonoBehaviour
{
	public GameObject link;

	public BottomType type;

	private void OnTriggerStay(Collider other)
	{
		if (!(other.gameObject.tag == "Player"))
		{
			return;
		}
		switch (type)
		{
		case BottomType.Die:
		{
			HERO component = other.gameObject.GetComponent<HERO>();
			if (component != null && !component.HasDied())
			{
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
				{
					component.Die(other.gameObject.rigidbody.velocity * 50f, isBite: false);
				}
				else if (component.photonView.isMine)
				{
					component.NetDieLocal2(other.gameObject.rigidbody.velocity * 50f, isBite: false, -1, GuardianClient.Properties.LavaDeathMessage.Value);
				}
			}
			break;
		}
		case BottomType.Teleport:
			other.gameObject.transform.position = ((link != null) ? link.transform.position : Vector3.zero);
			break;
		}
	}
}
