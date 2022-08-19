using System;
using System.Collections;
using System.IO;
using Guardian;
using Guardian.Utilities;
using UnityEngine;

public class BTN_save_snapshot : MonoBehaviour
{
	public GameObject targetTexture;

	public GameObject info;

	public GameObject[] thingsNeedToHide;

	private string SaveDir = GuardianClient.RootDir + "\\Screenshots";

	private void OnPress()
	{
		try
		{
			GameObject[] array = thingsNeedToHide;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].transform.position -= Vector3.up * 10000f;
			}
			base.transform.position -= Vector3.up * 10000f;
		}
		catch
		{
			info.GetComponent<UILabel>().text = "Error preparing Snapshot.";
		}
		info.GetComponent<UILabel>().text = "Attempting to save snapshot..";
		StartCoroutine(CoEncodeScreenshot());
	}

	private IEnumerator CoEncodeScreenshot()
	{
		yield return new WaitForEndOfFrame();
		try
		{
			float num = (float)Screen.height / 600f;
			Vector3 localScale = targetTexture.transform.localScale;
			Texture2D texture2D = new Texture2D((int)(num * localScale.x), (int)(num * localScale.y), TextureFormat.RGB24, mipmap: false);
			texture2D.ReadPixels(new Rect((float)Screen.width / 2f - (float)texture2D.width / 2f, (float)Screen.height / 2f - (float)texture2D.height / 2f, texture2D.width, texture2D.height), 0, 0);
			texture2D.Apply();
			DateTime now = DateTime.Now;
			string text = "SnapShot-" + now.Day + "_" + now.Month + "_" + now.Year + "-" + now.Hour + "_" + now.Minute + "_" + now.Second + ".jpg";
			GameHelper.TryCreateFile(SaveDir, directory: true);
			File.WriteAllBytes(SaveDir + "\\" + text, texture2D.EncodeToJPG(100));
			UnityEngine.Object.DestroyObject(texture2D);
			info.GetComponent<UILabel>().text = "Snapshot saved.";
		}
		catch
		{
			info.GetComponent<UILabel>().text = "Error saving Snapshot.";
		}
		GameObject[] array = thingsNeedToHide;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].transform.position += Vector3.up * 10000f;
		}
		base.transform.position += Vector3.up * 10000f;
	}
}
