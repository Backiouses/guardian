using ExitGames.Client.Photon;

namespace Guardian.Networking
{
	internal class PhotonConnection
	{
		public static PhotonConnection TCP = new PhotonConnection("TCP", 4530, ConnectionProtocol.Tcp);

		public static PhotonConnection UDP = new PhotonConnection("UDP", 5055, ConnectionProtocol.Udp);

		public string Name;

		public int Port;

		public ConnectionProtocol Protocol;

		public PhotonConnection(string name, int port, ConnectionProtocol protocol)
		{
			Name = name;
			Port = port;
			Protocol = protocol;
		}
	}
}
