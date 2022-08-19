using System;
using System.Collections;
using Anarchy.Custom.Interfaces;
using Anarchy.Custom.Level;
using Guardian;
using Guardian.AntiAbuse.Validators;
using Guardian.Utilities;
using Photon;
using UnityEngine;

public class Bullet : Photon.MonoBehaviour, IAnarchyScriptHook
{
	private GameObject master;

	private Vector3 velocity = Vector3.zero;

	private Vector3 velocity2 = Vector3.zero;

	public GameObject rope;

	public LineRenderer lineRenderer;

	private ArrayList nodes = new ArrayList();

	private ArrayList spiralNodes;

	private int phase;

	private float killTime;

	private float killTime2;

	private bool left = true;

	public bool leviMode;

	public float leviShootTime;

	private GameObject myRef;

	private int spiralcount;

	private bool isdestroying;

	public TITAN myTitan;

	public float tileScale = 1f;

	public Transform Transform => base.transform;

	public IAnarchyScriptHero Master
	{
		get
		{
			if (!(master == null))
			{
				return master.GetComponent<HERO>();
			}
			return null;
		}
	}

	void IAnarchyScriptHook.KillMaster()
	{
		master.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, -1, "Trap");
	}

	public void Launch(Vector3 v, Vector3 v2, string launcherRef, bool isLeft, GameObject master, bool leviMode = false)
	{
		if (phase != 2)
		{
			this.master = master;
			velocity = v;
			if (Mathf.Abs(Mathf.Acos(Vector3.Dot(v.normalized, v2.normalized)) * 57.29578f) > 90f)
			{
				velocity2 = Vector3.zero;
			}
			else
			{
				velocity2 = Vector3.Project(v2, v);
			}
			GameObject gameObject;
			switch (launcherRef)
			{
			case "hookRefL1":
				gameObject = master.GetComponent<HERO>().hookRefL1;
				break;
			case "hookRefL2":
				gameObject = master.GetComponent<HERO>().hookRefL2;
				break;
			case "hookRefR1":
				gameObject = master.GetComponent<HERO>().hookRefR1;
				break;
			case "hookRefR2":
				gameObject = master.GetComponent<HERO>().hookRefR2;
				break;
			default:
				gameObject = null;
				break;
			}
			myRef = gameObject;
			nodes = new ArrayList { myRef.transform.position };
			phase = 0;
			this.leviMode = leviMode;
			left = isLeft;
			if (IN_GAME_MAIN_CAMERA.Gametype != 0 && base.photonView.isMine)
			{
				base.photonView.RPC("myMasterIs", PhotonTargets.Others, master.GetComponent<HERO>().photonView.viewID, launcherRef);
				base.photonView.RPC("setVelocityAndLeft", PhotonTargets.Others, v, velocity2, left);
			}
			base.transform.position = myRef.transform.position;
			base.transform.rotation = Quaternion.LookRotation(v.normalized);
		}
	}

	[RPC]
	private void myMasterIs(int viewId, string launcherRef, PhotonMessageInfo info)
	{
		if (HookChecker.IsHookMasterSetValid(this, viewId, info))
		{
			master = PhotonView.Find(viewId).gameObject;
			GameObject gameObject;
			switch (launcherRef)
			{
			case "hookRefL1":
				gameObject = master.GetComponent<HERO>().hookRefL1;
				break;
			case "hookRefL2":
				gameObject = master.GetComponent<HERO>().hookRefL2;
				break;
			case "hookRefR1":
				gameObject = master.GetComponent<HERO>().hookRefR1;
				break;
			case "hookRefR2":
				gameObject = master.GetComponent<HERO>().hookRefR2;
				break;
			default:
				gameObject = null;
				break;
			}
			myRef = gameObject;
		}
	}

	[RPC]
	private void netLaunch(Vector3 newPosition, PhotonMessageInfo info)
	{
		if (HookChecker.IsLaunchValid(info))
		{
			nodes = new ArrayList();
			nodes.Add(newPosition);
		}
	}

	[RPC]
	private void netUpdatePhase1(Vector3 newPosition, Vector3 masterPosition, PhotonMessageInfo info)
	{
		if (HookChecker.IsPhaseUpdateValid(info))
		{
			lineRenderer.SetVertexCount(2);
			lineRenderer.SetPosition(0, newPosition);
			lineRenderer.SetPosition(1, masterPosition);
			base.transform.position = newPosition;
		}
	}

	[RPC]
	private void netUpdateLeviSpiral(Vector3 newPosition, Vector3 masterPosition, Vector3 masterrotation, PhotonMessageInfo info)
	{
		if (!HookChecker.IsLeviSpiralValid(info))
		{
			return;
		}
		phase = 2;
		leviMode = true;
		GetSpiral(masterPosition, masterrotation);
		Vector3 vector = masterPosition - (Vector3)spiralNodes[0];
		lineRenderer.SetVertexCount((int)((float)spiralNodes.Count - (float)spiralcount * 0.5f));
		for (int i = 0; (float)i <= (float)(spiralNodes.Count - 1) - (float)spiralcount * 0.5f; i++)
		{
			if (spiralcount < 5)
			{
				Vector3 vector2 = (Vector3)spiralNodes[i] + vector;
				float num = (float)(spiralNodes.Count - 1) - (float)spiralcount * 0.5f;
				vector2 = new Vector3(vector2.x, vector2.y * ((num - (float)i) / num) + newPosition.y * ((float)i / num), vector2.z);
				lineRenderer.SetPosition(i, vector2);
			}
			else
			{
				lineRenderer.SetPosition(i, (Vector3)spiralNodes[i] + vector);
			}
		}
	}

	public bool IsHooked()
	{
		return phase == 1;
	}

	private void GetSpiral(Vector3 masterposition, Vector3 masterrotation)
	{
		float num = 1.2f;
		float num2 = 30f;
		float num3 = 0.5f;
		num = 30f;
		float num4 = 0.05f + (float)spiralcount * 0.03f;
		if (spiralcount < 5)
		{
			Vector2 a = new Vector2(masterposition.x, masterposition.z);
			float x = base.gameObject.transform.position.x;
			num = Vector2.Distance(a, new Vector2(x, base.gameObject.transform.position.z));
		}
		else
		{
			num = 1.2f + (float)(60 - spiralcount) * 0.1f;
		}
		num3 -= (float)spiralcount * 0.06f;
		float num5 = num / num2;
		float num6 = num4 / num2 * 2f * (float)Math.PI;
		num3 *= (float)Math.PI * 2f;
		spiralNodes = new ArrayList();
		for (int i = 1; (float)i <= num2; i++)
		{
			float num7 = (float)i * num5 * (1f + 0.05f * (float)i);
			float f = (float)i * num6 + num3 + (float)Math.PI * 2f / 5f + masterrotation.y * 0.0173f;
			float x2 = Mathf.Cos(f) * num7;
			float z = (0f - Mathf.Sin(f)) * num7;
			spiralNodes.Add(new Vector3(x2, 0f, z));
		}
	}

	private void setLinePhase0()
	{
		if (master == null)
		{
			UnityEngine.Object.Destroy(rope);
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if (nodes.Count > 0)
		{
			Vector3 vector = myRef.transform.position - (Vector3)nodes[0];
			lineRenderer.SetVertexCount(nodes.Count);
			for (int i = 0; i <= nodes.Count - 1; i++)
			{
				lineRenderer.SetPosition(i, (Vector3)nodes[i] + vector * Mathf.Pow(0.75f, i));
			}
			if (nodes.Count > 1)
			{
				lineRenderer.SetPosition(1, myRef.transform.position);
			}
		}
	}

	[RPC]
	private void setPhase(int value)
	{
		phase = value;
	}

	[RPC]
	private void setVelocityAndLeft(Vector3 value, Vector3 v2, bool l)
	{
		velocity = value;
		velocity2 = v2;
		left = l;
		base.transform.rotation = Quaternion.LookRotation(value.normalized);
	}

	[RPC]
	private void tieMeTo(Vector3 p)
	{
		base.transform.position = p;
	}

	private void HandleHookToObj(int viewId)
	{
		PhotonView photonView = PhotonView.Find(viewId);
		if (photonView == null || !GuardianClient.Properties.DeadlyHooks.Value || !PhotonNetwork.isMasterClient)
		{
			return;
		}
		HERO component = photonView.gameObject.GetComponent<HERO>();
		if (!(component == null) && !component.HasDied())
		{
			string text = GExtensions.AsString(base.photonView.owner.customProperties[PhotonPlayerProperty.Name]);
			if (text.StripNGUI().Length < 1)
			{
				text = "Player";
			}
			text += $" [FFCC00]({base.photonView.owner.Id})[FFFFFF]";
			component.MarkDead();
			component.photonView.RPC("netDie2", photonView.owner, -1, text + "'s hook");
		}
	}

	[RPC]
	private void tieMeToOBJ(int id, PhotonMessageInfo info)
	{
		if (HookChecker.IsHookTieValid(this, id, info))
		{
			base.transform.parent = PhotonView.Find(id).gameObject.transform;
			HandleHookToObj(id);
		}
	}

	public void update()
	{
		if (master == null)
		{
			RemoveMe();
		}
		else
		{
			if (isdestroying)
			{
				return;
			}
			if (leviMode)
			{
				leviShootTime += Time.deltaTime;
				if (leviShootTime > 0.4f)
				{
					phase = 2;
					base.gameObject.GetComponent<MeshRenderer>().enabled = false;
				}
			}
			switch (phase)
			{
			case 0:
				setLinePhase0();
				break;
			case 1:
			{
				Vector3 vector2 = base.transform.position - myRef.transform.position;
				_ = base.transform.position + myRef.transform.position;
				Vector3 vector3 = master.rigidbody.velocity;
				float magnitude = vector3.magnitude;
				float magnitude2 = vector2.magnitude;
				int value = (int)((magnitude2 + magnitude) / 5f);
				value = Mathf.Clamp(value, 2, 6);
				lineRenderer.SetVertexCount(value);
				lineRenderer.SetPosition(0, myRef.transform.position);
				int j = 1;
				float num = Mathf.Pow(magnitude2, 0.3f);
				for (; j < value; j++)
				{
					int num2 = value / 2;
					float num3 = Mathf.Abs(j - num2);
					float f = ((float)num2 - num3) / (float)num2;
					f = Mathf.Pow(f, 0.5f);
					float num4 = (num + magnitude) * 0.0015f * f;
					lineRenderer.SetPosition(j, new Vector3(UnityEngine.Random.Range(0f - num4, num4), UnityEngine.Random.Range(0f - num4, num4), UnityEngine.Random.Range(0f - num4, num4)) + myRef.transform.position + vector2 * ((float)j / (float)value) - Vector3.up * num * 0.05f * f - vector3 * 0.001f * f * num);
				}
				lineRenderer.SetPosition(value - 1, base.transform.position);
				break;
			}
			case 2:
				if (leviMode)
				{
					GetSpiral(master.transform.position, master.transform.rotation.eulerAngles);
					Vector3 vector4 = myRef.transform.position - (Vector3)spiralNodes[0];
					lineRenderer.SetVertexCount((int)((float)spiralNodes.Count - (float)spiralcount * 0.5f));
					for (int k = 0; (float)k <= (float)(spiralNodes.Count - 1) - (float)spiralcount * 0.5f; k++)
					{
						if (spiralcount < 5)
						{
							Vector3 vector5 = (Vector3)spiralNodes[k] + vector4;
							float num5 = (float)(spiralNodes.Count - 1) - (float)spiralcount * 0.5f;
							float x2 = vector5.x;
							float num6 = vector5.y * ((num5 - (float)k) / num5);
							vector5 = new Vector3(x2, num6 + base.gameObject.transform.position.y * ((float)k / num5), vector5.z);
							lineRenderer.SetPosition(k, vector5);
						}
						else
						{
							lineRenderer.SetPosition(k, (Vector3)spiralNodes[k] + vector4);
						}
					}
				}
				else
				{
					lineRenderer.SetVertexCount(2);
					lineRenderer.SetPosition(0, base.transform.position);
					lineRenderer.SetPosition(1, myRef.transform.position);
					killTime += Time.deltaTime * 0.2f;
					lineRenderer.SetWidth(0.1f - killTime, 0.1f - killTime);
					if (killTime > 0.1f)
					{
						RemoveMe();
					}
				}
				break;
			case 4:
			{
				base.gameObject.transform.position += velocity + velocity2 * Time.deltaTime;
				ArrayList arrayList = nodes;
				float x = base.gameObject.transform.position.x;
				float y = base.gameObject.transform.position.y;
				arrayList.Add(new Vector3(x, y, base.gameObject.transform.position.z));
				Vector3 vector = myRef.transform.position - (Vector3)nodes[0];
				for (int i = 0; i <= nodes.Count - 1; i++)
				{
					lineRenderer.SetVertexCount(nodes.Count);
					lineRenderer.SetPosition(i, (Vector3)nodes[i] + vector * Mathf.Pow(0.5f, i));
				}
				killTime2 += Time.deltaTime;
				if (killTime2 > 0.8f)
				{
					killTime += Time.deltaTime * 0.2f;
					lineRenderer.SetWidth(0.1f - killTime, 0.1f - killTime);
					if (killTime > 0.1f)
					{
						RemoveMe();
					}
				}
				break;
			}
			}
			if (lineRenderer.material != null)
			{
				float magnitude3 = (base.transform.position - myRef.transform.position).magnitude;
				lineRenderer.material.mainTextureScale = new Vector2(tileScale * magnitude3, 1f);
			}
		}
	}

	public void Disable()
	{
		phase = 2;
		killTime = 0f;
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
		{
			base.photonView.RPC("setPhase", PhotonTargets.Others, 2);
		}
	}

	public void RemoveMe()
	{
		CustomAnarchyLevel.Instance.OnHookUntiedGround(this);
		isdestroying = true;
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
		{
			UnityEngine.Object.Destroy(rope);
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if (base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.photonView);
			PhotonNetwork.RemoveRPCs(base.photonView);
		}
	}

	[RPC]
	private void killObject(PhotonMessageInfo info)
	{
		if (HookChecker.IsKillObjectValid(info))
		{
			UnityEngine.Object.Destroy(rope);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (FengGameManagerMKII.Instance != null)
		{
			FengGameManagerMKII.Instance.RemoveHook(this);
		}
		if (myTitan != null)
		{
			myTitan.isHooked = false;
		}
		UnityEngine.Object.Destroy(rope);
	}

	private void FixedUpdate()
	{
		if ((phase == 2 || phase == 1) && leviMode)
		{
			spiralcount++;
			if (spiralcount >= 60)
			{
				isdestroying = true;
				RemoveMe();
				return;
			}
		}
		if (IN_GAME_MAIN_CAMERA.Gametype != 0 && !base.photonView.isMine)
		{
			if (phase == 0)
			{
				base.gameObject.transform.position += velocity * Time.deltaTime * 50f + velocity2 * Time.deltaTime;
				nodes.Add(new Vector3(base.gameObject.transform.position.x, base.gameObject.transform.position.y, base.gameObject.transform.position.z));
			}
		}
		else
		{
			if (phase != 0)
			{
				return;
			}
			CheckTitan();
			base.gameObject.transform.position += velocity * Time.deltaTime * 50f + velocity2 * Time.deltaTime;
			LayerMask layerMask = 1 << LayerMask.NameToLayer("EnemyBox");
			LayerMask layerMask2 = 1 << LayerMask.NameToLayer("Ground");
			LayerMask layerMask3 = 1 << LayerMask.NameToLayer("NetworkObject");
			LayerMask layerMask4 = (int)layerMask | (int)layerMask2 | (int)layerMask3;
			bool flag = false;
			if ((nodes.Count <= 1) ? Physics.Linecast((Vector3)nodes[nodes.Count - 1], base.gameObject.transform.position, out var hitInfo, layerMask4.value) : Physics.Linecast((Vector3)nodes[nodes.Count - 2], base.gameObject.transform.position, out hitInfo, layerMask4.value))
			{
				bool flag2 = true;
				if (hitInfo.collider.transform.gameObject.layer == LayerMask.NameToLayer("EnemyBox"))
				{
					if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
					{
						base.photonView.RPC("tieMeToOBJ", PhotonTargets.Others, hitInfo.collider.transform.root.gameObject.GetPhotonView().viewID);
					}
					master.GetComponent<HERO>().lastHook = hitInfo.collider.transform.root;
					base.transform.parent = hitInfo.collider.transform;
				}
				else if (hitInfo.collider.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
				{
					master.GetComponent<HERO>().lastHook = null;
					CustomAnarchyLevel.Instance.OnHookAttachedToGround(this, hitInfo.collider.gameObject);
				}
				else if (hitInfo.collider.transform.gameObject.layer == LayerMask.NameToLayer("NetworkObject") && hitInfo.collider.transform.gameObject.tag == "Player" && !leviMode)
				{
					if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
					{
						int viewID = hitInfo.collider.transform.root.gameObject.GetPhotonView().viewID;
						base.photonView.RPC("tieMeToOBJ", PhotonTargets.Others, viewID);
						HandleHookToObj(viewID);
					}
					master.GetComponent<HERO>().HookToHuman(hitInfo.collider.transform.root.gameObject, base.transform.position);
					base.transform.parent = hitInfo.collider.transform;
					master.GetComponent<HERO>().lastHook = null;
				}
				else
				{
					flag2 = false;
				}
				if (phase == 2)
				{
					flag2 = false;
				}
				if (flag2)
				{
					master.GetComponent<HERO>().Launch(hitInfo.point, left, leviMode);
					base.transform.position = hitInfo.point;
					if (phase != 2)
					{
						phase = 1;
						if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
						{
							base.photonView.RPC("setPhase", PhotonTargets.Others, 1);
							base.photonView.RPC("tieMeTo", PhotonTargets.Others, base.transform.position);
						}
						if (leviMode)
						{
							GetSpiral(master.transform.position, master.transform.rotation.eulerAngles);
						}
						flag = true;
					}
				}
			}
			nodes.Add(new Vector3(base.gameObject.transform.position.x, base.gameObject.transform.position.y, base.gameObject.transform.position.z));
			if (flag)
			{
				return;
			}
			killTime2 += Time.deltaTime;
			if (killTime2 > 0.8f)
			{
				phase = 4;
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
				{
					base.photonView.RPC("setPhase", PhotonTargets.Others, 4);
				}
			}
		}
	}

	public void CheckTitan()
	{
		GameObject main_object = Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().main_object;
		if (main_object == null || master == null || !(master == main_object))
		{
			return;
		}
		int value = ((LayerMask)(1 << LayerMask.NameToLayer("PlayerAttackBox"))).value;
		if (!Physics.Raycast(base.transform.position, velocity, out var hitInfo, 10f, value))
		{
			return;
		}
		Collider collider = hitInfo.collider;
		if (!collider.name.Contains("PlayerDetectorRC"))
		{
			return;
		}
		TITAN component = collider.transform.root.gameObject.GetComponent<TITAN>();
		if (component != null)
		{
			if (myTitan == null)
			{
				myTitan = component;
				myTitan.isHooked = true;
			}
			else if (myTitan != component)
			{
				myTitan.isHooked = false;
				myTitan = component;
				myTitan.isHooked = true;
			}
		}
	}

	private void Start()
	{
		if (ResourceLoader.TryGetAsset<Texture2D>("Custom/Textures/hook.png", out var value))
		{
			base.gameObject.renderer.material.mainTexture = value;
		}
		rope = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("rope"));
		lineRenderer = rope.GetComponent<LineRenderer>();
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().AddHook(this);
		if (master == null)
		{
			return;
		}
		HERO component = master.GetComponent<HERO>();
		if (component == null)
		{
			return;
		}
		if (left)
		{
			if (component._leftRopeMat != null)
			{
				lineRenderer.material = component._leftRopeMat;
				tileScale = component._leftRopeXScale;
			}
		}
		else if (component._rightRopeMat != null)
		{
			lineRenderer.material = component._rightRopeMat;
			tileScale = component._rightRopeXScale;
		}
	}
}
