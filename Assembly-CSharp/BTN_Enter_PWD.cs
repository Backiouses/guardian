using UnityEngine;

public class BTN_Enter_PWD : MonoBehaviour
{
	private void OnClick()
	{
		string text = GameObject.Find("InputEnterPWD").GetComponent<UIInput>().label.text;
		SimpleAES simpleAES = new SimpleAES();
		if (text == simpleAES.Decrypt(PanelMultiJoinPWD.Password))
		{
			PhotonNetwork.JoinRoom(PanelMultiJoinPWD.RoomName);
			return;
		}
		UIMainReferences component = GameObject.Find("UIRefer").GetComponent<UIMainReferences>();
		NGUITools.SetActive(component.PanelMultiPWD, state: false);
		NGUITools.SetActive(component.panelMultiROOM, state: true);
		GameObject.Find("PanelMultiROOM").GetComponent<PanelMultiJoin>().Refresh();
	}
}
