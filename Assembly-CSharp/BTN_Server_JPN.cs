using Guardian.Networking;
using UnityEngine;

public class BTN_Server_JPN : MonoBehaviour
{
	private void OnClick()
	{
		PhotonNetwork.Disconnect();
		FengGameManagerMKII.OnPrivateServer = false;
		NetworkHelper.IsCloud = true;
		string appID = "b92ae2ae-b815-4f37-806a-58b4f58573ff";
		if (NetworkHelper.App == PhotonApplication.Custom)
		{
			appID = FengGameManagerMKII.ApplicationId;
		}
		PhotonNetwork.ConnectToMaster("app-jp.exitgamescloud.com", NetworkHelper.Connection.Port, appID, UIMainReferences.Version);
	}
}
