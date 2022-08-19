using System;
using Guardian.Utilities;
using UnityEngine;

public static class RCextensions
{
	public static Texture2D LoadImage(WWW link, bool mipmapEnabled, int maxFileSize)
	{
		if (link.size < maxFileSize)
		{
			Texture2D texture = link.texture;
			int width = texture.width;
			int height = texture.height;
			int num = 0;
			if (width < 4 || !MathHelper.IsPowerOf2(width))
			{
				num = 4;
				width = Math.Min(width, 2047);
				if (num < width)
				{
					num = MathHelper.NextPowerOf2(width);
				}
			}
			else if (height < 4 || !MathHelper.IsPowerOf2(height))
			{
				num = 4;
				height = Math.Min(height, 2047);
				if (num < height)
				{
					num = MathHelper.NextPowerOf2(height);
				}
			}
			if (num == 0)
			{
				Texture2D texture2D = new Texture2D(4, 4, texture.format, mipmapEnabled);
				try
				{
					link.LoadImageIntoTexture(texture2D);
				}
				catch
				{
					texture2D = new Texture2D(4, 4, texture.format, mipmap: false);
					link.LoadImageIntoTexture(texture2D);
				}
				texture2D.Compress(highQuality: true);
				return texture2D;
			}
			TextureFormat format = ((texture.format == TextureFormat.RGB24) ? TextureFormat.DXT1 : TextureFormat.DXT5);
			Texture2D texture2D2 = new Texture2D(4, 4, format, mipmap: false);
			link.LoadImageIntoTexture(texture2D2);
			try
			{
				texture2D2.Resize(num, num, format, mipmapEnabled);
			}
			catch
			{
				texture2D2.Resize(num, num, format, hasMipMap: false);
			}
			texture2D2.Apply();
			return texture2D2;
		}
		return new Texture2D(2, 2, TextureFormat.DXT1, mipmap: false);
	}

	public static void Add<T>(ref T[] source, T value)
	{
		T[] array = new T[source.Length + 1];
		Array.Copy(source, array, source.Length);
		array[array.Length - 1] = value;
		source = array;
	}

	public static void RemoveAt<T>(ref T[] source, int position)
	{
		if (source.Length == 1)
		{
			source = new T[0];
		}
		else
		{
			if (source.Length <= 1)
			{
				return;
			}
			T[] array = new T[source.Length - 1];
			int num = 0;
			for (int i = 0; i < source.Length; i++)
			{
				if (i != position)
				{
					array[num] = source[i];
					num++;
				}
			}
			source = array;
		}
	}
}
