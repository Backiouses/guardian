using Guardian.Networking;
using UnityEngine;

public class BTN_Server_SA : MonoBehaviour
{
	private void OnClick()
	{
		PhotonNetwork.Disconnect();
		FengGameManagerMKII.OnPrivateServer = false;
		if (NetworkHelper.App == PhotonApplication.AoTTG2)
		{
			PhotonNetwork.ConnectToMaster("172.107.193.233", NetworkHelper.Connection.Port, FengGameManagerMKII.ApplicationId, UIMainReferences.Version);
			NetworkHelper.IsCloud = false;
		}
		else
		{
			PhotonNetwork.ConnectToMaster("app-sa.exitgames.com", NetworkHelper.Connection.Port, FengGameManagerMKII.ApplicationId, UIMainReferences.Version);
			NetworkHelper.IsCloud = true;
		}
	}
}
