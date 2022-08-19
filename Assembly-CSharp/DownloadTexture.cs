using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class DownloadTexture : MonoBehaviour
{
	public string url = "http://www.tasharen.com/misc/logo.png";

	private Material mMat;

	private Texture2D mTex;

	private IEnumerator Start()
	{
		WWW www = new WWW(url);
		yield return www;
		mTex = www.texture;
		if (mTex != null)
		{
			UITexture component = GetComponent<UITexture>();
			if (component.material == null)
			{
				mMat = new Material(Shader.Find("Unlit/Transparent Colored"));
			}
			else
			{
				mMat = new Material(component.material);
			}
			component.material = mMat;
			mMat.mainTexture = mTex;
			component.MakePixelPerfect();
		}
		www.Dispose();
	}

	private void OnDestroy()
	{
		if (mMat != null)
		{
			Object.Destroy(mMat);
		}
		if (mTex != null)
		{
			Object.Destroy(mTex);
		}
	}
}
