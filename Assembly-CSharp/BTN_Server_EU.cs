using Guardian.Networking;
using UnityEngine;

public class BTN_Server_EU : MonoBehaviour
{
	private void OnClick()
	{
		PhotonNetwork.Disconnect();
		FengGameManagerMKII.OnPrivateServer = false;
		if (NetworkHelper.App == PhotonApplication.AoTTG2)
		{
			PhotonNetwork.ConnectToMaster("135.125.239.180", NetworkHelper.Connection.Port, FengGameManagerMKII.ApplicationId, UIMainReferences.Version);
			NetworkHelper.IsCloud = false;
		}
		else
		{
			PhotonNetwork.ConnectToMaster("app-eu.exitgamescloud.com", NetworkHelper.Connection.Port, FengGameManagerMKII.ApplicationId, UIMainReferences.Version);
			NetworkHelper.IsCloud = true;
		}
	}
}
