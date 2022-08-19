using System.Text.RegularExpressions;
using Guardian;
using Guardian.Networking;
using UnityEngine;

public class BTN_Join_LAN : MonoBehaviour
{
	private static readonly Regex PhotonCloud = new Regex("app-\\w+\\.exitgames(cloud)?\\.com", RegexOptions.IgnoreCase);

	private void OnClick()
	{
		string text = base.transform.parent.Find("InputIP").GetComponent<UIInput>().text;
		string text2 = base.transform.parent.Find("InputPort").GetComponent<UIInput>().text;
		string text3 = base.transform.parent.Find("InputAuthPass").GetComponent<UIInput>().text;
		PhotonNetwork.Disconnect();
		if (int.TryParse(text2, out var result) && PhotonNetwork.ConnectToMaster(text, result, FengGameManagerMKII.ApplicationId, UIMainReferences.Version))
		{
			PlayerPrefs.SetString("lastIP", text);
			PlayerPrefs.SetString("lastPort", text2);
			PlayerPrefs.SetString("lastAuthPass", text3);
			if (PhotonCloud.IsMatch(text))
			{
				GuardianClient.Logger.Info("Joining a Photon Cloud server.");
			}
			else
			{
				GuardianClient.Logger.Info("Joining a non-Photon Cloud server.");
			}
			FengGameManagerMKII.OnPrivateServer = !PhotonCloud.IsMatch(text);
			NetworkHelper.IsCloud = !FengGameManagerMKII.OnPrivateServer;
			FengGameManagerMKII.PrivateServerAuthPass = text3;
		}
	}
}
