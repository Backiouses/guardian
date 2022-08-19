using ExitGames.Client.Photon;
using Guardian.Utilities;
using UnityEngine;

namespace Guardian.Features.Commands.Impl
{
	internal class CommandSetLighting : Command
	{
		public CommandSetLighting()
			: base("setlighting", new string[3] { "lighting", "settime", "time" }, "<day/dawn/night>", masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length >= 1 && GExtensions.TryParseEnum<DayLight>(args[0], out var value))
			{
				if (PhotonNetwork.isMasterClient)
				{
					PhotonNetwork.room.SetCustomProperties(new Hashtable { 
					{
						"Lighting",
						args[0].ToUpper()
					} });
					GameHelper.Broadcast("The current map lighting is now " + args[0].ToUpper() + "!");
				}
				else
				{
					Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetLighting(value);
					irc.AddLine("Map lighting is now " + args[0].ToUpper() + ".");
				}
			}
		}
	}
}
