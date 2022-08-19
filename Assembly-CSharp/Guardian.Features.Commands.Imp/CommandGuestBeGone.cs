using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Guardian.Utilities;

namespace Guardian.Features.Commands.Impl.MasterClient
{
	internal class CommandGuestBeGone : Command
	{
		private Regex GuestNamePattern = new Regex("GUEST-?[0-9]+", RegexOptions.IgnoreCase);

		public CommandGuestBeGone()
			: base("guestbegone", new string[1] { "gbg" }, string.Empty, masterClient: true)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			List<string> list = new List<string>();
			PhotonPlayer[] otherPlayers = PhotonNetwork.otherPlayers;
			foreach (PhotonPlayer photonPlayer in otherPlayers)
			{
				string text = GExtensions.AsString(photonPlayer.customProperties[PhotonPlayerProperty.Name]);
				if (GuestNamePattern.IsMatch(text))
				{
					FengGameManagerMKII.Instance.KickPlayer(photonPlayer, ban: false, string.Empty);
					list.Add(text);
				}
			}
			GameHelper.Broadcast($"Guest-Be-Gone kicked {list.Count} guest(s)!");
			if (list.Count > 0)
			{
				GameHelper.Broadcast("Guests kicked: " + string.Join(", ", list.Select((string name) => name.NGUIToUnity()).ToArray()));
			}
		}
	}
}
