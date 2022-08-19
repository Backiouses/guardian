using UnityEngine;

namespace Guardian.Features.Commands.Impl.RC
{
	internal class CommandSpectate : Command
	{
		public CommandSpectate()
			: base("spectate", new string[2] { "spec", "specmode" }, "[id]", masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length != 0)
			{
				if (!int.TryParse(args[0], out var result))
				{
					return;
				}
				PhotonPlayer photonPlayer = PhotonPlayer.Find(result);
				if (photonPlayer != null && !photonPlayer.IsDead)
				{
					if (photonPlayer.IsTitan)
					{
						Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetMainObjectTitan(photonPlayer.GetTitan().gameObject);
					}
					else
					{
						Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetMainObject(photonPlayer.GetHero().gameObject);
					}
					Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetSpectorMode(val: false);
					irc.AddLine($"Now spectating #{result}.".AsColor("FFCC00"));
				}
			}
			else if ((int)FengGameManagerMKII.Settings[245] == 0)
			{
				FengGameManagerMKII.Settings[245] = 1;
				FengGameManagerMKII.Instance.EnterSpecMode(enter: true);
				irc.AddLine("You entered spectator mode.".AsColor("FFCC00"));
			}
			else
			{
				FengGameManagerMKII.Settings[245] = 0;
				FengGameManagerMKII.Instance.EnterSpecMode(enter: false);
				irc.AddLine("You left spectator mode.".AsColor("FFCC00"));
			}
		}
	}
}
