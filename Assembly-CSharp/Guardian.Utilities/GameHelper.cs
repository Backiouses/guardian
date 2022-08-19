using System;
using System.IO;
using UnityEngine;

namespace Guardian.Utilities
{
	internal class GameHelper
	{
		public static readonly Vector2 ScrollBottom = new Vector2(0f, float.MaxValue);

		public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static void Broadcast(string message)
		{
			FengGameManagerMKII.Instance.photonView.RPC("Chat", PhotonTargets.All, message, "[MC]".AsColor("AAFF00").AsBold());
		}

		public static object[] GetRandomTitanRespawnPoint()
		{
			Vector3 vector = new Vector3(MathHelper.RandomInt(-400, 401), 0f, MathHelper.RandomInt(-400, 401));
			Quaternion quaternion = new Quaternion(0f, 0f, 0f, 1f);
			if (FengGameManagerMKII.Instance.titanSpawns.Count > 0)
			{
				vector = FengGameManagerMKII.Instance.titanSpawns[MathHelper.RandomInt(0, FengGameManagerMKII.Instance.titanSpawns.Count)];
			}
			else
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag("titanRespawn");
				if (array.Length != 0)
				{
					int num = MathHelper.RandomInt(0, array.Length);
					GameObject gameObject = array[num];
					while (array[num] == null)
					{
						num = MathHelper.RandomInt(0, array.Length);
						gameObject = array[num];
					}
					array[num] = null;
					vector = gameObject.transform.position;
					quaternion = gameObject.transform.rotation;
				}
			}
			return new object[2] { vector, quaternion };
		}

		public static bool TryCreateFile(string path, bool directory)
		{
			try
			{
				if (!directory)
				{
					if (!File.Exists(path))
					{
						File.Create(path).Close();
					}
				}
				else if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		public static long CurrentTimeMillis()
		{
			return (long)DateTime.UtcNow.Subtract(Epoch).TotalMilliseconds;
		}
	}
}
