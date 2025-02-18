using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class CannonBall : Photon.MonoBehaviour
{
	private Vector3 correctPos;

	private Vector3 correctVelocity;

	public float SmoothingDelay = 10f;

	public Transform firingPoint;

	public List<TitanTrigger> myTitanTriggers;

	public bool disabled;

	public bool isCollider;

	public HERO myHero;

	private void Awake()
	{
		if (base.photonView != null)
		{
			base.photonView.observed = this;
			correctPos = base.transform.position;
			correctVelocity = Vector3.zero;
			GetComponent<SphereCollider>().enabled = false;
			if (base.photonView.isMine)
			{
				StartCoroutine(CoWaitAndDestroy(10f));
				myTitanTriggers = new List<TitanTrigger>();
			}
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.rigidbody.velocity);
		}
		else
		{
			correctPos = (Vector3)stream.ReceiveNext();
			correctVelocity = (Vector3)stream.ReceiveNext();
		}
	}

	public void Update()
	{
		if (!base.photonView.isMine)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, correctPos, Time.deltaTime * SmoothingDelay);
			base.rigidbody.velocity = correctVelocity;
		}
	}

	public void FixedUpdate()
	{
		if (!base.photonView.isMine || disabled)
		{
			return;
		}
		LayerMask layerMask = 1 << LayerMask.NameToLayer("PlayerAttackBox");
		LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyBox");
		LayerMask layerMask3 = (int)layerMask | (int)layerMask2;
		if (!isCollider)
		{
			LayerMask layerMask4 = 1 << LayerMask.NameToLayer("Ground");
			layerMask3 = (int)layerMask3 | (int)layerMask4;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, 0.6f, layerMask3.value);
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i].gameObject;
			if (gameObject.layer == 16)
			{
				TitanTrigger component = gameObject.GetComponent<TitanTrigger>();
				if (component != null && !myTitanTriggers.Contains(component))
				{
					component.isCollide = true;
					myTitanTriggers.Add(component);
				}
			}
			else if (gameObject.layer == 10)
			{
				TITAN component2 = gameObject.transform.root.gameObject.GetComponent<TITAN>();
				if (component2 == null)
				{
					continue;
				}
				if (component2.abnormalType == TitanClass.Crawler)
				{
					if (gameObject.name == "head")
					{
						component2.photonView.RPC("DieByCannon", component2.photonView.owner, myHero.photonView.viewID);
						component2.DieBlow(base.transform.position, 0.2f);
						i = array.Length;
					}
				}
				else if (gameObject.name == "head")
				{
					component2.photonView.RPC("DieByCannon", component2.photonView.owner, myHero.photonView.viewID);
					component2.DieHeadBlow(base.transform.position, 0.2f);
					i = array.Length;
				}
				else if (Random.Range(0f, 1f) < 0.5f)
				{
					component2.HitLeft(base.transform.position, 0.05f);
				}
				else
				{
					component2.HitRight(base.transform.position, 0.05f);
				}
				destroyMe();
			}
			else if (gameObject.layer == 9 && (gameObject.transform.root.name.Contains("CannonWall") || gameObject.transform.root.name.Contains("CannonGround")))
			{
				flag = true;
			}
		}
		if (!isCollider && !flag)
		{
			isCollider = true;
			GetComponent<SphereCollider>().enabled = true;
		}
	}

	public void OnCollisionEnter(Collision myCollision)
	{
		if (base.photonView.isMine)
		{
			destroyMe();
		}
	}

	public IEnumerator CoWaitAndDestroy(float time)
	{
		yield return new WaitForSeconds(time);
		destroyMe();
	}

	public void destroyMe()
	{
		if (disabled)
		{
			return;
		}
		disabled = true;
		EnemyCheckCollider[] componentsInChildren = PhotonNetwork.Instantiate("FX/boom4", base.transform.position, base.transform.rotation, 0).GetComponentsInChildren<EnemyCheckCollider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].dmg = 0;
		}
		if (RCSettings.DeadlyCannons == 1)
		{
			foreach (HERO hero in FengGameManagerMKII.Instance.Heroes)
			{
				if (!(hero != null) || !(Vector3.Distance(hero.transform.position, base.transform.position) <= 20f) || hero.photonView.isMine)
				{
					continue;
				}
				PhotonPlayer owner = hero.photonView.owner;
				if (RCSettings.TeamMode > 0 && PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCTeam] != null && owner.customProperties[PhotonPlayerProperty.RCTeam] != null)
				{
					int num = GExtensions.AsInt(PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCTeam]);
					int num2 = GExtensions.AsInt(owner.customProperties[PhotonPlayerProperty.RCTeam]);
					if (num == 0 || num != num2)
					{
						hero.MarkDead();
						hero.photonView.RPC("netDie2", PhotonTargets.All, -1, GExtensions.AsString(PhotonNetwork.player.customProperties[PhotonPlayerProperty.Name]) + " ");
						FengGameManagerMKII.Instance.UpdatePlayerKillInfo(0, PhotonNetwork.player);
					}
				}
				else
				{
					hero.MarkDead();
					hero.photonView.RPC("netDie2", PhotonTargets.All, -1, GExtensions.AsString(PhotonNetwork.player.customProperties[PhotonPlayerProperty.Name]) + " ");
					FengGameManagerMKII.Instance.UpdatePlayerKillInfo(0, PhotonNetwork.player);
				}
			}
		}
		if (myTitanTriggers != null)
		{
			for (int j = 0; j < myTitanTriggers.Count; j++)
			{
				if (myTitanTriggers[j] != null)
				{
					myTitanTriggers[j].isCollide = false;
				}
			}
		}
		PhotonNetwork.Destroy(base.gameObject);
	}
}
