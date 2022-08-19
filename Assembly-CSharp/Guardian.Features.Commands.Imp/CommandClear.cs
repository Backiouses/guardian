using System.Collections.Generic;
using Guardian.Utilities;

namespace Guardian.Features.Commands.Impl
{
	internal class CommandClear : Command
	{
		public CommandClear()
			: base("clear", new string[0], "[log/global/id]", masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length != 0)
			{
				string text = args[0].ToLower();
				if (!(text == "log"))
				{
					if (text == "global")
					{
						if (PhotonNetwork.isMasterClient)
						{
							InRoomChat.Messages = new List<InRoomChat.Message>();
							for (int i = 0; i < 14; i++)
							{
								GameHelper.Broadcast(" ");
							}
							GameHelper.Broadcast("Global chat has been cleared!".AsColor("AAFF00"));
						}
					}
					else
					{
						if (!PhotonNetwork.isMasterClient || !int.TryParse(args[0], out var result))
						{
							return;
						}
						PhotonPlayer photonPlayer = PhotonPlayer.Find(result);
						if (photonPlayer != null)
						{
							for (int j = 0; j < 14; j++)
							{
								FengGameManagerMKII.Instance.photonView.RPC("Chat", photonPlayer, " ", "[MC]".AsColor("AAFF00").AsBold());
							}
							GameHelper.Broadcast("Your chat has been cleared!".AsColor("AAFF00"));
						}
					}
				}
				else
				{
					GuardianClient.Logger.Entries = new SynchronizedList<Logger.Entry>();
					GuardianClient.Logger.Info("Event log has been cleared!");
				}
			}
			else
			{
				InRoomChat.Messages = new List<InRoomChat.Message>();
				irc.AddLine("Local chat has been cleared.".AsColor("AAFF00"));
			}
		}
	}
}
