using System.Threading;
using Guardian.Networking;

namespace Guardian.Features.Commands.Impl
{
	internal class CommandRejoin : Command
	{
		public CommandRejoin()
			: base("rejoin", new string[2] { "relog", "reconnect" }, string.Empty, masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			string lastRoomName = PhotonNetwork.room.name;
			string[] array = PhotonNetwork.networkingPeer.MasterServerAddress.Split(':');
			string masterServerAddress = array[0];
			int result = NetworkHelper.Connection.Port;
			if (array.Length > 1 && !int.TryParse(array[1], out result))
			{
				return;
			}
			PhotonNetwork.Disconnect();
			if (!PhotonNetwork.ConnectToMaster(masterServerAddress, result, NetworkHelper.App.Id, UIMainReferences.Version))
			{
				return;
			}
			new Thread((ThreadStart)delegate
			{
				while (PhotonNetwork.networkingPeer.State != PeerState.JoinedLobby && IN_GAME_MAIN_CAMERA.Gametype == GameType.Stop && !GuardianClient.WasQuitRequested)
				{
				}
				PhotonNetwork.JoinRoom(lastRoomName);
			}).Start();
		}
	}
}
