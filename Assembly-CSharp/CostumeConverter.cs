using ExitGames.Client.Photon;
using UnityEngine;

public class CostumeConverter
{
	public static void ToLocalData(HeroCostume costume, string slot)
	{
		slot = slot.ToUpper();
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.Sex, (int)costume.sex);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.CostumeId, costume.costumeId);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.HeroCostumeId, costume.id);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.Cape, costume.cape ? 1 : 0);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.HairInfo, costume.hairInfo.id);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.EyeTextureId, costume.eye_texture_id);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.BeardTextureId, costume.beard_texture_id);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.GlassTextureId, costume.glass_texture_id);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.SkinColor, costume.skin_color);
		PlayerPrefs.SetFloat(slot + PhotonPlayerProperty.HairColor1, costume.hair_color.r);
		PlayerPrefs.SetFloat(slot + PhotonPlayerProperty.HairColor2, costume.hair_color.g);
		PlayerPrefs.SetFloat(slot + PhotonPlayerProperty.HairColor3, costume.hair_color.b);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.Division, (int)costume.division);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.StatSpeed, costume.stat.Speed);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.StatGas, costume.stat.Gas);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.StatBlade, costume.stat.Blade);
		PlayerPrefs.SetInt(slot + PhotonPlayerProperty.StatAccel, costume.stat.Accel);
		PlayerPrefs.SetString(slot + PhotonPlayerProperty.StatSkill, costume.stat.SkillId);
	}

	public static HeroCostume FromLocalData(string slot)
	{
		slot = slot.ToUpper();
		if (!PlayerPrefs.HasKey(slot + PhotonPlayerProperty.Sex))
		{
			return HeroCostume.Costumes[0];
		}
		HeroCostume obj = new HeroCostume
		{
			sex = (Sex)PlayerPrefs.GetInt(slot + PhotonPlayerProperty.Sex),
			id = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.HeroCostumeId),
			costumeId = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.CostumeId),
			cape = (PlayerPrefs.GetInt(slot + PhotonPlayerProperty.Cape) == 1)
		};
		obj.hairInfo = ((obj.sex != Sex.Male) ? CostumeHair.FemaleHairs[PlayerPrefs.GetInt(slot + PhotonPlayerProperty.HairInfo)] : CostumeHair.MaleHairs[PlayerPrefs.GetInt(slot + PhotonPlayerProperty.HairInfo)]);
		obj.eye_texture_id = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.EyeTextureId);
		obj.beard_texture_id = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.BeardTextureId);
		obj.glass_texture_id = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.GlassTextureId);
		obj.skin_color = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.SkinColor);
		obj.hair_color = new Color(PlayerPrefs.GetFloat(slot + PhotonPlayerProperty.HairColor1), PlayerPrefs.GetFloat(slot + PhotonPlayerProperty.HairColor2), PlayerPrefs.GetFloat(slot + PhotonPlayerProperty.HairColor3));
		obj.division = (Division)PlayerPrefs.GetInt(slot + PhotonPlayerProperty.Division);
		obj.stat = new HeroStat();
		obj.stat.Speed = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.StatSpeed);
		obj.stat.Gas = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.StatGas);
		obj.stat.Blade = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.StatBlade);
		obj.stat.Accel = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.StatAccel);
		obj.stat.SkillId = PlayerPrefs.GetString(slot + PhotonPlayerProperty.StatSkill);
		obj.setBodyByCostumeId();
		obj.SetMesh();
		obj.setTexture();
		return obj;
	}

	public static void ToPhotonData(HeroCostume costume, PhotonPlayer player)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add(PhotonPlayerProperty.Sex, (int)costume.sex);
		hashtable.Add(PhotonPlayerProperty.CostumeId, costume.costumeId);
		hashtable.Add(PhotonPlayerProperty.HeroCostumeId, costume.id);
		hashtable.Add(PhotonPlayerProperty.Cape, costume.cape);
		hashtable.Add(PhotonPlayerProperty.HairInfo, costume.hairInfo.id);
		hashtable.Add(PhotonPlayerProperty.EyeTextureId, costume.eye_texture_id);
		hashtable.Add(PhotonPlayerProperty.BeardTextureId, costume.beard_texture_id);
		hashtable.Add(PhotonPlayerProperty.GlassTextureId, costume.glass_texture_id);
		hashtable.Add(PhotonPlayerProperty.SkinColor, costume.skin_color);
		hashtable.Add(PhotonPlayerProperty.HairColor1, costume.hair_color.r);
		hashtable.Add(PhotonPlayerProperty.HairColor2, costume.hair_color.g);
		hashtable.Add(PhotonPlayerProperty.HairColor3, costume.hair_color.b);
		hashtable.Add(PhotonPlayerProperty.Division, (int)costume.division);
		hashtable.Add(PhotonPlayerProperty.StatSpeed, costume.stat.Speed);
		hashtable.Add(PhotonPlayerProperty.StatGas, costume.stat.Gas);
		hashtable.Add(PhotonPlayerProperty.StatBlade, costume.stat.Blade);
		hashtable.Add(PhotonPlayerProperty.StatAccel, costume.stat.Accel);
		hashtable.Add(PhotonPlayerProperty.StatSkill, costume.stat.SkillId);
		player.SetCustomProperties(hashtable);
	}

	public static HeroCostume FromPhotonData(PhotonPlayer player)
	{
		Sex sex = (Sex)(int)player.customProperties[PhotonPlayerProperty.Sex];
		HeroCostume heroCostume = new HeroCostume();
		heroCostume.sex = sex;
		heroCostume.costumeId = (int)player.customProperties[PhotonPlayerProperty.CostumeId];
		heroCostume.id = (int)player.customProperties[PhotonPlayerProperty.HeroCostumeId];
		heroCostume.cape = (bool)player.customProperties[PhotonPlayerProperty.Cape];
		heroCostume.hairInfo = ((sex != Sex.Male) ? CostumeHair.FemaleHairs[(int)player.customProperties[PhotonPlayerProperty.HairInfo]] : CostumeHair.MaleHairs[(int)player.customProperties[PhotonPlayerProperty.HairInfo]]);
		heroCostume.eye_texture_id = (int)player.customProperties[PhotonPlayerProperty.EyeTextureId];
		heroCostume.beard_texture_id = (int)player.customProperties[PhotonPlayerProperty.BeardTextureId];
		heroCostume.glass_texture_id = (int)player.customProperties[PhotonPlayerProperty.GlassTextureId];
		heroCostume.skin_color = (int)player.customProperties[PhotonPlayerProperty.SkinColor];
		heroCostume.hair_color = new Color((float)player.customProperties[PhotonPlayerProperty.HairColor1], (float)player.customProperties[PhotonPlayerProperty.HairColor2], (float)player.customProperties[PhotonPlayerProperty.HairColor3]);
		heroCostume.division = (Division)(int)player.customProperties[PhotonPlayerProperty.Division];
		heroCostume.stat = new HeroStat();
		heroCostume.stat.Speed = (int)player.customProperties[PhotonPlayerProperty.StatSpeed];
		heroCostume.stat.Gas = (int)player.customProperties[PhotonPlayerProperty.StatGas];
		heroCostume.stat.Blade = (int)player.customProperties[PhotonPlayerProperty.StatBlade];
		heroCostume.stat.Accel = (int)player.customProperties[PhotonPlayerProperty.StatAccel];
		heroCostume.stat.SkillId = (string)player.customProperties[PhotonPlayerProperty.StatSkill];
		heroCostume.setBodyByCostumeId();
		heroCostume.SetMesh();
		heroCostume.setTexture();
		return heroCostume;
	}
}
