using Guardian.Utilities;
using UnityEngine;

namespace Guardian.Features.Commands.Impl.MasterClient
{
	internal class CommandScatterTitans : Command
	{
		public CommandScatterTitans()
			: base("scatter", new string[0], string.Empty, masterClient: true)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			foreach (TITAN titan in FengGameManagerMKII.Instance.Titans)
			{
				if (titan.photonView.isMine)
				{
					object[] randomTitanRespawnPoint = GameHelper.GetRandomTitanRespawnPoint();
					titan.transform.position = (Vector3)randomTitanRespawnPoint[0];
					titan.transform.rotation = (Quaternion)randomTitanRespawnPoint[1];
				}
			}
			GameHelper.Broadcast("All titans have been scattered!");
		}
	}
}
