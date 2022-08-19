using Guardian.Networking;
using UnityEngine;

public class BTN_ServerUS : MonoBehaviour
{
	private void OnClick()
	{
		PhotonNetwork.Disconnect();
		FengGameManagerMKII.OnPrivateServer = false;
		if (NetworkHelper.App == PhotonApplication.AoTTG2)
		{
			PhotonNetwork.ConnectToMaster("142.44.242.29", NetworkHelper.Connection.Port, FengGameManagerMKII.ApplicationId, UIMainReferences.Version);
			NetworkHelper.IsCloud = false;
		}
		else
		{
			PhotonNetwork.ConnectToMaster("app-us.exitgamescloud.com", NetworkHelper.Connection.Port, FengGameManagerMKII.ApplicationId, UIMainReferences.Version);
			NetworkHelper.IsCloud = true;
		}
	}
}
