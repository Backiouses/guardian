using Guardian.Networking;
using UnityEngine;

public class BTN_Server_ASIA : MonoBehaviour
{
	private void OnClick()
	{
		PhotonNetwork.Disconnect();
		FengGameManagerMKII.OnPrivateServer = false;
		if (NetworkHelper.App == PhotonApplication.AoTTG2)
		{
			PhotonNetwork.ConnectToMaster("51.79.164.137", NetworkHelper.Connection.Port, FengGameManagerMKII.ApplicationId, UIMainReferences.Version);
			NetworkHelper.IsCloud = false;
		}
		else
		{
			PhotonNetwork.ConnectToMaster("app-asia.exitgamescloud.com", NetworkHelper.Connection.Port, FengGameManagerMKII.ApplicationId, UIMainReferences.Version);
			NetworkHelper.IsCloud = true;
		}
	}
}
