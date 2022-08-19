namespace Guardian.Networking
{
	internal class NetworkHelper
	{
		public static PhotonApplication App = PhotonApplication.AoTTG2;

		public static PhotonConnection Connection = PhotonConnection.UDP;

		public static bool IsCloud = false;

		public static string GetRegionCode()
		{
			string text = PhotonNetwork.networkingPeer.MasterServerAddress.ToLower();
			if (text.StartsWith("app-"))
			{
				return text.Substr(text.IndexOf('-') + 1, text.IndexOf('.') - 1);
			}
			if (text.Contains(".aottg.tk"))
			{
				return text.Substr(0, text.IndexOf('.') - 1);
			}
			return "??";
		}
	}
}
