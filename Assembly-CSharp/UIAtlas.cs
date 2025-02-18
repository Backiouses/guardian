using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Atlas")]
public class UIAtlas : MonoBehaviour
{
	[Serializable]
	public class Sprite
	{
		public string name = "Unity Bug";

		public Rect outer = new Rect(0f, 0f, 1f, 1f);

		public Rect inner = new Rect(0f, 0f, 1f, 1f);

		public bool rotated;

		public float paddingLeft;

		public float paddingRight;

		public float paddingTop;

		public float paddingBottom;

		public bool hasPadding
		{
			get
			{
				if (paddingLeft == 0f && paddingRight == 0f && paddingTop == 0f)
				{
					return paddingBottom != 0f;
				}
				return true;
			}
		}
	}

	public enum Coordinates
	{
		Pixels,
		TexCoords
	}

	[SerializeField]
	[HideInInspector]
	private Material material;

	[SerializeField]
	[HideInInspector]
	private List<Sprite> sprites = new List<Sprite>();

	[HideInInspector]
	[SerializeField]
	private Coordinates mCoordinates;

	[HideInInspector]
	[SerializeField]
	private float mPixelSize = 1f;

	[HideInInspector]
	[SerializeField]
	private UIAtlas mReplacement;

	private int mPMA = -1;

	public Material spriteMaterial
	{
		get
		{
			if (!(mReplacement == null))
			{
				return mReplacement.spriteMaterial;
			}
			return material;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.spriteMaterial = value;
				return;
			}
			if (material == null)
			{
				mPMA = 0;
				material = value;
				return;
			}
			MarkAsDirty();
			mPMA = -1;
			material = value;
			MarkAsDirty();
		}
	}

	public bool premultipliedAlpha
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.premultipliedAlpha;
			}
			if (mPMA == -1)
			{
				Material material = spriteMaterial;
				mPMA = ((material != null && material.shader != null && material.shader.name.Contains("Premultiplied")) ? 1 : 0);
			}
			return mPMA == 1;
		}
	}

	public List<Sprite> spriteList
	{
		get
		{
			if (!(mReplacement == null))
			{
				return mReplacement.spriteList;
			}
			return sprites;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.spriteList = value;
			}
			else
			{
				sprites = value;
			}
		}
	}

	public Texture texture
	{
		get
		{
			if (!(mReplacement != null))
			{
				if (!(material == null))
				{
					return material.mainTexture;
				}
				return null;
			}
			return mReplacement.texture;
		}
	}

	public Coordinates coordinates
	{
		get
		{
			if (!(mReplacement == null))
			{
				return mReplacement.coordinates;
			}
			return mCoordinates;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.coordinates = value;
			}
			else
			{
				if (mCoordinates == value)
				{
					return;
				}
				if (material == null || material.mainTexture == null)
				{
					Debug.LogError("Can't switch coordinates until the atlas material has a valid texture");
					return;
				}
				mCoordinates = value;
				Texture mainTexture = material.mainTexture;
				int i = 0;
				for (int count = sprites.Count; i < count; i++)
				{
					Sprite sprite = sprites[i];
					if (mCoordinates == Coordinates.TexCoords)
					{
						sprite.outer = NGUIMath.ConvertToTexCoords(sprite.outer, mainTexture.width, mainTexture.height);
						sprite.inner = NGUIMath.ConvertToTexCoords(sprite.inner, mainTexture.width, mainTexture.height);
					}
					else
					{
						sprite.outer = NGUIMath.ConvertToPixels(sprite.outer, mainTexture.width, mainTexture.height, round: true);
						sprite.inner = NGUIMath.ConvertToPixels(sprite.inner, mainTexture.width, mainTexture.height, round: true);
					}
				}
			}
		}
	}

	public float pixelSize
	{
		get
		{
			if (!(mReplacement == null))
			{
				return mReplacement.pixelSize;
			}
			return mPixelSize;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.pixelSize = value;
				return;
			}
			float num = Mathf.Clamp(value, 0.25f, 4f);
			if (mPixelSize != num)
			{
				mPixelSize = num;
				MarkAsDirty();
			}
		}
	}

	public UIAtlas replacement
	{
		get
		{
			return mReplacement;
		}
		set
		{
			UIAtlas uIAtlas = value;
			if (uIAtlas == this)
			{
				uIAtlas = null;
			}
			if (mReplacement != uIAtlas)
			{
				if (uIAtlas != null && uIAtlas.replacement == this)
				{
					uIAtlas.replacement = null;
				}
				if (mReplacement != null)
				{
					MarkAsDirty();
				}
				mReplacement = uIAtlas;
				MarkAsDirty();
			}
		}
	}

	public Sprite GetSprite(string name)
	{
		if (mReplacement != null)
		{
			return mReplacement.GetSprite(name);
		}
		if (!string.IsNullOrEmpty(name))
		{
			int i = 0;
			for (int count = sprites.Count; i < count; i++)
			{
				Sprite sprite = sprites[i];
				if (!string.IsNullOrEmpty(sprite.name) && name == sprite.name)
				{
					return sprite;
				}
			}
		}
		return null;
	}

	private static int CompareString(string a, string b)
	{
		return a.CompareTo(b);
	}

	public BetterList<string> GetListOfSprites()
	{
		if (mReplacement != null)
		{
			return mReplacement.GetListOfSprites();
		}
		BetterList<string> betterList = new BetterList<string>();
		int i = 0;
		for (int count = sprites.Count; i < count; i++)
		{
			Sprite sprite = sprites[i];
			if (sprite != null && !string.IsNullOrEmpty(sprite.name))
			{
				betterList.Add(sprite.name);
			}
		}
		return betterList;
	}

	public BetterList<string> GetListOfSprites(string match)
	{
		if (mReplacement != null)
		{
			return mReplacement.GetListOfSprites(match);
		}
		if (string.IsNullOrEmpty(match))
		{
			return GetListOfSprites();
		}
		BetterList<string> betterList = new BetterList<string>();
		int i = 0;
		for (int count = sprites.Count; i < count; i++)
		{
			Sprite sprite = sprites[i];
			if (sprite != null && !string.IsNullOrEmpty(sprite.name) && string.Equals(match, sprite.name, StringComparison.OrdinalIgnoreCase))
			{
				betterList.Add(sprite.name);
				return betterList;
			}
		}
		string[] array = match.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = array[j].ToLower();
		}
		int k = 0;
		for (int count2 = sprites.Count; k < count2; k++)
		{
			Sprite sprite2 = sprites[k];
			if (sprite2 == null || string.IsNullOrEmpty(sprite2.name))
			{
				continue;
			}
			string text = sprite2.name.ToLower();
			int num = 0;
			for (int l = 0; l < array.Length; l++)
			{
				if (text.Contains(array[l]))
				{
					num++;
				}
			}
			if (num == array.Length)
			{
				betterList.Add(sprite2.name);
			}
		}
		return betterList;
	}

	private bool References(UIAtlas atlas)
	{
		if (atlas == null)
		{
			return false;
		}
		if (atlas == this)
		{
			return true;
		}
		if (mReplacement != null)
		{
			return mReplacement.References(atlas);
		}
		return false;
	}

	public static bool CheckIfRelated(UIAtlas a, UIAtlas b)
	{
		if (a == null || b == null)
		{
			return false;
		}
		if (!(a == b) && !a.References(b))
		{
			return b.References(a);
		}
		return true;
	}

	public void MarkAsDirty()
	{
		if (mReplacement != null)
		{
			mReplacement.MarkAsDirty();
		}
		UISprite[] array = NGUITools.FindActive<UISprite>();
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			UISprite uISprite = array[i];
			if (CheckIfRelated(this, uISprite.atlas))
			{
				UIAtlas atlas = uISprite.atlas;
				uISprite.atlas = null;
				uISprite.atlas = atlas;
			}
		}
		UIFont[] array2 = Resources.FindObjectsOfTypeAll(typeof(UIFont)) as UIFont[];
		int j = 0;
		for (int num2 = array2.Length; j < num2; j++)
		{
			UIFont uIFont = array2[j];
			if (CheckIfRelated(this, uIFont.atlas))
			{
				UIAtlas atlas2 = uIFont.atlas;
				uIFont.atlas = null;
				uIFont.atlas = atlas2;
			}
		}
		UILabel[] array3 = NGUITools.FindActive<UILabel>();
		int k = 0;
		for (int num3 = array3.Length; k < num3; k++)
		{
			UILabel uILabel = array3[k];
			if (uILabel.font != null && CheckIfRelated(this, uILabel.font.atlas))
			{
				UIFont font = uILabel.font;
				uILabel.font = null;
				uILabel.font = font;
			}
		}
	}
}
