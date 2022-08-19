using Guardian;
using Guardian.AntiAbuse.Validators;
using Photon;
using UnityEngine;

public class Horse : Photon.MonoBehaviour
{
	public GameObject myHero;

	public GameObject dust;

	private string State = "idle";

	private float speed = 45f;

	private TITAN_CONTROLLER controller;

	private Vector3 setPoint;

	private float awayTimer;

	private float timeElapsed;

	private void Start()
	{
		controller = base.gameObject.GetComponent<TITAN_CONTROLLER>();
	}

	public void Mount()
	{
		State = "mounted";
		base.gameObject.GetComponent<TITAN_CONTROLLER>().enabled = true;
		if (myHero != null && myHero.GetPhotonView().isMine)
		{
			myHero.rigidbody.interpolation = RigidbodyInterpolation.None;
		}
	}

	public void Unmount()
	{
		State = "idle";
		base.gameObject.GetComponent<TITAN_CONTROLLER>().enabled = false;
		if (myHero != null && myHero.GetPhotonView().isMine && GuardianClient.Properties.Interpolation.Value)
		{
			myHero.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		}
	}

	private void Follow()
	{
		if (!(myHero == null))
		{
			State = "follow";
			setPoint = myHero.transform.position + Vector3.right * Random.Range(-6, 6) + Vector3.forward * Random.Range(-6, 6);
			setPoint.y = GetHeight(setPoint + Vector3.up * 5f);
			awayTimer = 0f;
		}
	}

	private float GetHeight(Vector3 pt)
	{
		if (Physics.Raycast(layerMask: ((LayerMask)(1 << LayerMask.NameToLayer("Ground"))).value, origin: pt, direction: -Vector3.up, hitInfo: out var hitInfo, distance: 1000f))
		{
			return hitInfo.point.y;
		}
		return 0f;
	}

	private void LateUpdate()
	{
		if (myHero == null && base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
		switch (State)
		{
		case "mounted":
			if (myHero == null)
			{
				Unmount();
				return;
			}
			myHero.transform.position = base.transform.position + Vector3.up * 1.68f;
			myHero.transform.rotation = base.transform.rotation;
			myHero.rigidbody.velocity = base.rigidbody.velocity;
			if (controller.targetDirection != -874f)
			{
				base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, controller.targetDirection, 0f), 100f * Time.deltaTime / (base.rigidbody.velocity.magnitude + 20f));
				if (controller.isWALKDown)
				{
					base.rigidbody.AddForce(base.transform.forward * speed * 0.6f, ForceMode.Acceleration);
					if (!(base.rigidbody.velocity.magnitude < speed * 0.6f))
					{
						base.rigidbody.AddForce((0f - speed) * 0.6f * base.rigidbody.velocity.normalized, ForceMode.Acceleration);
					}
				}
				else
				{
					base.rigidbody.AddForce(base.transform.forward * speed, ForceMode.Acceleration);
					if (!(base.rigidbody.velocity.magnitude < speed))
					{
						base.rigidbody.AddForce((0f - speed) * base.rigidbody.velocity.normalized, ForceMode.Acceleration);
					}
				}
				if (base.rigidbody.velocity.magnitude > 8f)
				{
					if (!base.animation.IsPlaying("horse_Run"))
					{
						CrossFade("horse_Run", 0.1f);
					}
					if (!myHero.animation.IsPlaying("horse_run"))
					{
						myHero.GetComponent<HERO>().CrossFade("horse_run", 0.1f);
					}
					if (!dust.GetComponent<ParticleSystem>().enableEmission)
					{
						dust.GetComponent<ParticleSystem>().enableEmission = true;
						base.photonView.RPC("setDust", PhotonTargets.Others, true);
					}
				}
				else
				{
					if (!base.animation.IsPlaying("horse_WALK"))
					{
						CrossFade("horse_WALK", 0.1f);
					}
					if (!myHero.animation.IsPlaying("horse_idle"))
					{
						myHero.GetComponent<HERO>().CrossFade("horse_idle", 0.1f);
					}
					if (dust.GetComponent<ParticleSystem>().enableEmission)
					{
						dust.GetComponent<ParticleSystem>().enableEmission = false;
						base.photonView.RPC("setDust", PhotonTargets.Others, false);
					}
				}
			}
			else
			{
				Idle();
				if (base.rigidbody.velocity.magnitude > 15f)
				{
					if (!myHero.animation.IsPlaying("horse_run"))
					{
						myHero.GetComponent<HERO>().CrossFade("horse_run", 0.1f);
					}
				}
				else if (!myHero.animation.IsPlaying("horse_idle"))
				{
					myHero.GetComponent<HERO>().CrossFade("horse_idle", 0.1f);
				}
			}
			if ((controller.isAttackDown || controller.isAttackIIDown) && IsGrounded())
			{
				base.rigidbody.AddForce(Vector3.up * 25f, ForceMode.VelocityChange);
			}
			break;
		case "follow":
		{
			if (myHero == null)
			{
				Unmount();
				return;
			}
			if (base.rigidbody.velocity.magnitude > 8f)
			{
				if (!base.animation.IsPlaying("horse_Run"))
				{
					CrossFade("horse_Run", 0.1f);
				}
				if (!dust.GetComponent<ParticleSystem>().enableEmission)
				{
					dust.GetComponent<ParticleSystem>().enableEmission = true;
					base.photonView.RPC("setDust", PhotonTargets.Others, true);
				}
			}
			else
			{
				if (!base.animation.IsPlaying("horse_WALK"))
				{
					CrossFade("horse_WALK", 0.1f);
				}
				if (dust.GetComponent<ParticleSystem>().enableEmission)
				{
					dust.GetComponent<ParticleSystem>().enableEmission = false;
					base.photonView.RPC("setDust", PhotonTargets.Others, false);
				}
			}
			float horizontalAngle = FengMath.GetHorizontalAngle(base.transform.position, setPoint);
			float num = 0f - Mathf.DeltaAngle(horizontalAngle, base.gameObject.transform.rotation.eulerAngles.y - 90f);
			Transform obj = base.gameObject.transform;
			Quaternion rotation = base.gameObject.transform.rotation;
			obj.rotation = Quaternion.Lerp(rotation, Quaternion.Euler(0f, base.gameObject.transform.rotation.eulerAngles.y + num, 0f), 200f * Time.deltaTime / (base.rigidbody.velocity.magnitude + 20f));
			if (Vector3.Distance(setPoint, base.transform.position) < 20f)
			{
				base.rigidbody.AddForce(base.transform.forward * speed * 0.7f, ForceMode.Acceleration);
				if (!(base.rigidbody.velocity.magnitude < speed))
				{
					base.rigidbody.AddForce((0f - speed) * 0.7f * base.rigidbody.velocity.normalized, ForceMode.Acceleration);
				}
			}
			else
			{
				base.rigidbody.AddForce(base.transform.forward * speed, ForceMode.Acceleration);
				if (!(base.rigidbody.velocity.magnitude < speed))
				{
					base.rigidbody.AddForce((0f - speed) * base.rigidbody.velocity.normalized, ForceMode.Acceleration);
				}
			}
			timeElapsed += Time.deltaTime;
			if (timeElapsed > 0.6f)
			{
				timeElapsed = 0f;
				if (Vector3.Distance(myHero.transform.position, setPoint) > 20f)
				{
					Follow();
				}
			}
			if (Vector3.Distance(myHero.transform.position, base.transform.position) < 5f)
			{
				Unmount();
			}
			if (Vector3.Distance(setPoint, base.transform.position) < 5f)
			{
				Unmount();
			}
			awayTimer += Time.deltaTime;
			if (awayTimer > 6f)
			{
				awayTimer = 0f;
				if (Physics.Linecast(layerMask: ((LayerMask)(1 << LayerMask.NameToLayer("Ground"))).value, start: base.transform.position + Vector3.up, end: myHero.transform.position + Vector3.up))
				{
					Transform obj2 = base.transform;
					float x = myHero.transform.position.x;
					float height = GetHeight(myHero.transform.position + Vector3.up * 5f);
					obj2.position = new Vector3(x, height, myHero.transform.position.z);
				}
			}
			break;
		}
		case "idle":
			Idle();
			if (myHero != null && Vector3.Distance(myHero.transform.position, base.transform.position) > 20f)
			{
				Follow();
			}
			break;
		}
		base.rigidbody.AddForce(new Vector3(0f, -50f * base.rigidbody.mass, 0f));
	}

	public bool IsGrounded()
	{
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
		int value = ((LayerMask)((int)(LayerMask)(1 << LayerMask.NameToLayer("EnemyBox")) | (int)layerMask)).value;
		return Physics.Raycast(base.gameObject.transform.position + Vector3.up * 0.1f, -Vector3.up, 0.3f, value);
	}

	private void Idle()
	{
		if (base.rigidbody.velocity.magnitude > 0.1f)
		{
			if (base.rigidbody.velocity.magnitude > 15f)
			{
				if (!base.animation.IsPlaying("horse_Run"))
				{
					CrossFade("horse_Run", 0.1f);
				}
				if (!dust.GetComponent<ParticleSystem>().enableEmission)
				{
					dust.GetComponent<ParticleSystem>().enableEmission = true;
					base.photonView.RPC("setDust", PhotonTargets.Others, true);
				}
			}
			else
			{
				if (!base.animation.IsPlaying("horse_WALK"))
				{
					CrossFade("horse_WALK", 0.1f);
				}
				if (dust.GetComponent<ParticleSystem>().enableEmission)
				{
					dust.GetComponent<ParticleSystem>().enableEmission = false;
					base.photonView.RPC("setDust", PhotonTargets.Others, false);
				}
			}
			return;
		}
		if (base.animation.IsPlaying("horse_idle1") && base.animation["horse_idle1"].normalizedTime >= 1f && !base.animation.IsPlaying("horse_idle0"))
		{
			CrossFade("horse_idle0", 0.1f);
		}
		if (base.animation.IsPlaying("horse_idle2") && base.animation["horse_idle2"].normalizedTime >= 1f && !base.animation.IsPlaying("horse_idle0"))
		{
			CrossFade("horse_idle0", 0.1f);
		}
		if (base.animation.IsPlaying("horse_idle3") && base.animation["horse_idle3"].normalizedTime >= 1f && !base.animation.IsPlaying("horse_idle0"))
		{
			CrossFade("horse_idle0", 0.1f);
		}
		if (!base.animation.IsPlaying("horse_idle0") && !base.animation.IsPlaying("horse_idle1") && !base.animation.IsPlaying("horse_idle2") && !base.animation.IsPlaying("horse_idle3"))
		{
			CrossFade("horse_idle0", 0.1f);
		}
		if (base.animation.IsPlaying("horse_idle0"))
		{
			int num = Random.Range(0, 10000);
			if (num < 10)
			{
				CrossFade("horse_idle1", 0.1f);
			}
			else if (num < 20)
			{
				CrossFade("horse_idle2", 0.1f);
			}
			else if (num < 30)
			{
				CrossFade("horse_idle3", 0.1f);
			}
		}
		if (dust.GetComponent<ParticleSystem>().enableEmission)
		{
			dust.GetComponent<ParticleSystem>().enableEmission = false;
			base.photonView.RPC("setDust", PhotonTargets.Others, false);
		}
	}

	[RPC]
	private void setDust(bool state)
	{
		dust.GetComponent<ParticleSystem>().enableEmission = state;
	}

	public void PlayAnimation(string aniName)
	{
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && base.photonView.isMine)
		{
			base.photonView.RPC("netPlayAnimation", PhotonTargets.Others, aniName);
		}
		LocalPlayAnimation(aniName);
	}

	private void PlayAnimationAt(string aniName, float normalizedTime)
	{
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && base.photonView.isMine)
		{
			base.photonView.RPC("netPlayAnimationAt", PhotonTargets.Others, aniName, normalizedTime);
		}
		LocalPlayAnimationAt(aniName, normalizedTime);
	}

	private void CrossFade(string aniName, float time)
	{
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && base.photonView.isMine)
		{
			base.photonView.RPC("netCrossFade", PhotonTargets.Others, aniName, time);
		}
		LocalCrossFade(aniName, time);
	}

	private void LocalPlayAnimation(string aniName)
	{
		base.animation.Play(aniName);
	}

	private void LocalPlayAnimationAt(string aniName, float normalizedTime)
	{
		base.animation.Play(aniName);
		base.animation[aniName].normalizedTime = normalizedTime;
	}

	private void LocalCrossFade(string aniName, float time)
	{
		base.animation.CrossFade(aniName, time);
	}

	[RPC]
	private void netPlayAnimation(string aniName, PhotonMessageInfo info)
	{
		if (HorseChecker.IsAnimationPlayValid(this, info))
		{
			LocalPlayAnimation(aniName);
		}
	}

	[RPC]
	private void netPlayAnimationAt(string aniName, float normalizedTime, PhotonMessageInfo info)
	{
		if (HorseChecker.IsAnimationSeekedPlayValid(this, info))
		{
			LocalPlayAnimationAt(aniName, normalizedTime);
		}
	}

	[RPC]
	private void netCrossFade(string aniName, float time, PhotonMessageInfo info)
	{
		if (HorseChecker.IsCrossFadeValid(this, info))
		{
			LocalCrossFade(aniName, time);
		}
	}
}
