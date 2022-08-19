using Guardian;
using Photon;
using UnityEngine;

public class BombExplode : Photon.MonoBehaviour
{
	public GameObject myExplosion;

	public void Start()
	{
		if (base.photonView == null)
		{
			return;
		}
		PhotonPlayer owner = base.photonView.owner;
		if (RCSettings.TeamMode > 0)
		{
			switch (GExtensions.AsInt(owner.customProperties[PhotonPlayerProperty.RCTeam]))
			{
			case 1:
				GetComponentInChildren<ParticleSystem>().startColor = Color.cyan;
				break;
			case 2:
				GetComponentInChildren<ParticleSystem>().startColor = Color.magenta;
				break;
			default:
			{
				float r = GExtensions.AsFloat(owner.customProperties[PhotonPlayerProperty.RCBombR]);
				float g = GExtensions.AsFloat(owner.customProperties[PhotonPlayerProperty.RCBombG]);
				float b = GExtensions.AsFloat(owner.customProperties[PhotonPlayerProperty.RCBombB]);
				float b2 = GExtensions.AsFloat(owner.customProperties[PhotonPlayerProperty.RCBombA]);
				b2 = Mathf.Max(0.5f, b2);
				GetComponentInChildren<ParticleSystem>().startColor = new Color(r, g, b, b2);
				break;
			}
			}
		}
		else
		{
			float r2 = GExtensions.AsFloat(owner.customProperties[PhotonPlayerProperty.RCBombR]);
			float g2 = GExtensions.AsFloat(owner.customProperties[PhotonPlayerProperty.RCBombG]);
			float b3 = GExtensions.AsFloat(owner.customProperties[PhotonPlayerProperty.RCBombB]);
			float b4 = GExtensions.AsFloat(owner.customProperties[PhotonPlayerProperty.RCBombA]);
			b4 = Mathf.Max(0.5f, b4);
			GetComponentInChildren<ParticleSystem>().startColor = new Color(r2, g2, b3, b4);
		}
		float value = GExtensions.AsFloat(owner.customProperties[PhotonPlayerProperty.RCBombRadius]) * 2f;
		GetComponentInChildren<ParticleSystem>().startSize = Mathf.Clamp(value, 40f, 120f);
		if (!PhotonNetwork.isMasterClient || !GuardianClient.Properties.BombsKillTitans.Value)
		{
			return;
		}
		foreach (TITAN titan in FengGameManagerMKII.Instance.Titans)
		{
			if ((titan.neck.position - base.transform.position).sqrMagnitude <= 400f)
			{
				if (titan.abnormalType == TitanClass.Crawler)
				{
					titan.DieBlow(base.transform.position, 0.2f);
				}
				else
				{
					titan.DieHeadBlow(base.transform.position, 0.2f);
				}
				string victim = titan.name;
				if (titan.nonAI)
				{
					victim = GExtensions.AsString(titan.photonView.owner.customProperties[PhotonPlayerProperty.Name]);
				}
				FengGameManagerMKII.Instance.SendKillInfo(isKillerTitan: false, GExtensions.AsString(base.photonView.owner.customProperties[PhotonPlayerProperty.Name]), isVictimTitan: true, victim);
				FengGameManagerMKII.Instance.UpdatePlayerKillInfo(0, base.photonView.owner);
				break;
			}
		}
	}
}
