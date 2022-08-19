using System.Collections;
using Guardian.AntiAbuse.Validators;
using Photon;
using UnityEngine;

public class TITAN_SETUP : Photon.MonoBehaviour
{
	public GameObject eye;

	private GameObject part_hair;

	private CostumeHair hair;

	private int hairType;

	private GameObject hair_go_ref;

	public int skin;

	public bool haseye;

	private void Awake()
	{
		CostumeHair.Init();
		CharacterMaterials.InitData();
		HeroCostume.Init();
		hair_go_ref = new GameObject();
		eye.transform.parent = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").transform;
		hair_go_ref.transform.position = eye.transform.position + Vector3.up * 3.5f + base.transform.forward * 5.2f;
		hair_go_ref.transform.rotation = eye.transform.rotation;
		hair_go_ref.transform.RotateAround(eye.transform.position, base.transform.right, -20f);
		hair_go_ref.transform.localScale = new Vector3(210f, 210f, 210f);
		hair_go_ref.transform.parent = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").transform;
	}

	public void SetEyeTexture(GameObject eyeGo, int eyeIndex)
	{
		if (eyeIndex >= 0)
		{
			float num = 0.25f;
			float x = 0.125f * (float)(int)((float)eyeIndex / 8f);
			float y = (0f - num) * (float)(eyeIndex % 4);
			eyeGo.renderer.material.mainTextureOffset = new Vector2(x, y);
		}
	}

	public void SetPunkHair2()
	{
		if ((int)FengGameManagerMKII.Settings[1] == 1 && (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer || base.photonView.isMine))
		{
			int num = Random.Range(0, 9);
			if (num == 3)
			{
				num = 9;
			}
			int num2 = skin - 70;
			if ((int)FengGameManagerMKII.Settings[32] == 1)
			{
				num2 = Random.Range(16, 20);
			}
			if ((int)FengGameManagerMKII.Settings[num2] >= 0)
			{
				num = (int)FengGameManagerMKII.Settings[num2];
			}
			string text = (string)FengGameManagerMKII.Settings[num2 + 5];
			int num3 = Random.Range(1, 8);
			if (haseye)
			{
				num3 = 0;
			}
			bool flag = false;
			if (text.EndsWith(".jpg") || text.EndsWith(".png") || text.EndsWith(".jpeg"))
			{
				flag = true;
			}
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && base.photonView.isMine)
			{
				if (flag)
				{
					base.photonView.RPC("setHairRPC2", PhotonTargets.AllBuffered, num, num3, text);
				}
				else
				{
					Color hair_color = HeroCostume.Costumes[Random.Range(0, HeroCostume.Costumes.Length - 5)].hair_color;
					base.photonView.RPC("setHairPRC", PhotonTargets.AllBuffered, num, num3, hair_color.r, hair_color.g, hair_color.b);
				}
			}
			else if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
			{
				if (flag)
				{
					StartCoroutine(CoLoadSkin(num, num3, text));
					return;
				}
				Color hair_color2 = HeroCostume.Costumes[Random.Range(0, HeroCostume.Costumes.Length - 5)].hair_color;
				setHairPRC(num, num3, hair_color2.r, hair_color2.g, hair_color2.b);
			}
		}
		else
		{
			Object.Destroy(part_hair);
			hair = CostumeHair.MaleHairs[3];
			hairType = 3;
			GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Character/" + hair.hair));
			gameObject.transform.parent = hair_go_ref.transform.parent;
			gameObject.transform.position = hair_go_ref.transform.position;
			gameObject.transform.rotation = hair_go_ref.transform.rotation;
			gameObject.transform.localScale = hair_go_ref.transform.localScale;
			gameObject.renderer.material = CharacterMaterials.materials[hair.texture];
			switch (Random.Range(1, 4))
			{
			case 1:
				gameObject.renderer.material.color = FengColor.PunkHair1;
				break;
			case 2:
				gameObject.renderer.material.color = FengColor.PunkHair2;
				break;
			case 3:
				gameObject.renderer.material.color = FengColor.PunkHair3;
				break;
			}
			part_hair = gameObject;
			SetEyeTexture(eye, 0);
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && base.photonView.isMine)
			{
				Color color = part_hair.renderer.material.color;
				base.photonView.RPC("setHairPRC", PhotonTargets.OthersBuffered, hairType, 0, color.r, color.g, color.b);
			}
		}
	}

	[RPC]
	private void setHairPRC(int hairIndex, int eyeIndex, float hairR, float hairG, float hairB)
	{
		Object.Destroy(part_hair);
		hair = CostumeHair.MaleHairs[hairIndex];
		hairType = hairIndex;
		if (hair.hair.Length > 0)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Character/" + hair.hair));
			gameObject.transform.parent = hair_go_ref.transform.parent;
			gameObject.transform.position = hair_go_ref.transform.position;
			gameObject.transform.rotation = hair_go_ref.transform.rotation;
			gameObject.transform.localScale = hair_go_ref.transform.localScale;
			gameObject.renderer.material = CharacterMaterials.materials[hair.texture];
			gameObject.renderer.material.color = new Color(hairR, hairG, hairB);
			part_hair = gameObject;
		}
		SetEyeTexture(eye, eyeIndex);
	}

	public void SetHair2()
	{
		if ((int)FengGameManagerMKII.Settings[1] == 1 && (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer || base.photonView.isMine))
		{
			int num = Random.Range(0, 9);
			if (num == 3)
			{
				num = 9;
			}
			int num2 = skin - 70;
			if ((int)FengGameManagerMKII.Settings[32] == 1)
			{
				num2 = Random.Range(16, 20);
			}
			if ((int)FengGameManagerMKII.Settings[num2] >= 0)
			{
				num = (int)FengGameManagerMKII.Settings[num2];
			}
			string text = (string)FengGameManagerMKII.Settings[num2 + 5];
			int num3 = Random.Range(1, 8);
			if (haseye)
			{
				num3 = 0;
			}
			bool flag = false;
			if (text.EndsWith(".jpg") || text.EndsWith(".png") || text.EndsWith(".jpeg"))
			{
				flag = true;
			}
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && base.photonView.isMine)
			{
				if (flag)
				{
					base.photonView.RPC("setHairRPC2", PhotonTargets.AllBuffered, num, num3, text);
				}
				else
				{
					Color hair_color = HeroCostume.Costumes[Random.Range(0, HeroCostume.Costumes.Length - 5)].hair_color;
					base.photonView.RPC("setHairPRC", PhotonTargets.AllBuffered, num, num3, hair_color.r, hair_color.g, hair_color.b);
				}
			}
			else if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
			{
				if (flag)
				{
					StartCoroutine(CoLoadSkin(num, num3, text));
					return;
				}
				Color hair_color2 = HeroCostume.Costumes[Random.Range(0, HeroCostume.Costumes.Length - 5)].hair_color;
				setHairPRC(num, num3, hair_color2.r, hair_color2.g, hair_color2.b);
			}
		}
		else
		{
			int num4 = Random.Range(0, CostumeHair.MaleHairs.Length);
			if (num4 == 3)
			{
				num4 = 9;
			}
			Object.Destroy(part_hair);
			hairType = num4;
			hair = CostumeHair.MaleHairs[num4];
			if (hair.hair == string.Empty)
			{
				hair = CostumeHair.MaleHairs[9];
				hairType = 9;
			}
			part_hair = (GameObject)Object.Instantiate(Resources.Load("Character/" + hair.hair));
			part_hair.transform.parent = hair_go_ref.transform.parent;
			part_hair.transform.position = hair_go_ref.transform.position;
			part_hair.transform.rotation = hair_go_ref.transform.rotation;
			part_hair.transform.localScale = hair_go_ref.transform.localScale;
			part_hair.renderer.material = CharacterMaterials.materials[hair.texture];
			part_hair.renderer.material.color = HeroCostume.Costumes[Random.Range(0, HeroCostume.Costumes.Length - 5)].hair_color;
			int num5 = Random.Range(1, 8);
			SetEyeTexture(eye, num5);
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && base.photonView.isMine)
			{
				object[] parameters = new object[5]
				{
					hairType,
					num5,
					part_hair.renderer.material.color.r,
					part_hair.renderer.material.color.g,
					part_hair.renderer.material.color.b
				};
				base.photonView.RPC("setHairPRC", PhotonTargets.OthersBuffered, parameters);
			}
		}
	}

	[RPC]
	public void setHairRPC2(int hair, int eye, string hairlink)
	{
		if ((int)FengGameManagerMKII.Settings[1] == 1)
		{
			StartCoroutine(CoLoadSkin(hair, eye, hairlink));
		}
	}

	public IEnumerator CoLoadSkin(int hairIndex, int eyeIndex, string hairLink)
	{
		bool unload = false;
		Object.Destroy(part_hair);
		hair = CostumeHair.MaleHairs[hairIndex];
		hairType = hairIndex;
		if (hair.hair.Length > 0)
		{
			GameObject obj2 = (GameObject)Object.Instantiate(Resources.Load("Character/" + hair.hair));
			obj2.transform.parent = hair_go_ref.transform.parent;
			obj2.transform.position = hair_go_ref.transform.position;
			obj2.transform.rotation = hair_go_ref.transform.rotation;
			obj2.transform.localScale = hair_go_ref.transform.localScale;
			obj2.renderer.material = CharacterMaterials.materials[hair.texture];
			bool flag = true;
			if ((int)FengGameManagerMKII.Settings[63] == 1)
			{
				flag = false;
			}
			if (hairLink.EndsWith(".jpg") || hairLink.EndsWith(".png") || hairLink.EndsWith(".jpeg"))
			{
				if (!FengGameManagerMKII.LinkHash[0].ContainsKey(hairLink))
				{
					WWW link = SkinChecker.CreateWWW(hairLink);
					if (link != null)
					{
						yield return link;
						Texture2D mainTexture = RCextensions.LoadImage(link, flag, 300000);
						link.Dispose();
						if (!FengGameManagerMKII.LinkHash[0].ContainsKey(hairLink))
						{
							unload = true;
							obj2.renderer.material.mainTexture = mainTexture;
							FengGameManagerMKII.LinkHash[0].Add(hairLink, obj2.renderer.material);
						}
						obj2.renderer.material = (Material)FengGameManagerMKII.LinkHash[0][hairLink];
					}
				}
				else
				{
					obj2.renderer.material = (Material)FengGameManagerMKII.LinkHash[0][hairLink];
				}
			}
			else if (hairLink.ToLower() == "transparent")
			{
				obj2.renderer.enabled = false;
			}
			part_hair = obj2;
		}
		if (eyeIndex >= 0)
		{
			SetEyeTexture(eye, eyeIndex);
		}
		if (unload)
		{
			FengGameManagerMKII.Instance.UnloadAssets();
		}
	}

	public void SetVar(int skin, bool hasEyes)
	{
		this.skin = skin;
		haseye = hasEyes;
	}
}
