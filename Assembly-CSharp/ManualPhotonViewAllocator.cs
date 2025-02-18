using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ManualPhotonViewAllocator : MonoBehaviour
{
	public GameObject Prefab;

	public void AllocateManualPhotonView()
	{
		PhotonView photonView = base.gameObject.GetPhotonView();
		if (photonView == null)
		{
			Debug.LogError("Can't do manual instantiation without PhotonView component.");
			return;
		}
		int num = PhotonNetwork.AllocateViewID();
		photonView.RPC("InstantiateRpc", PhotonTargets.AllBuffered, num);
	}

	[RPC]
	public void InstantiateRpc(int viewID)
	{
		GameObject obj = Object.Instantiate(Prefab, InputToEvent.inputHitPos + new Vector3(0f, 5f, 0f), Quaternion.identity) as GameObject;
		obj.GetPhotonView().viewID = viewID;
		obj.GetComponent<OnClickDestroy>().DestroyByRpc = true;
	}
}
