using UnityEngine;

[RequireComponent(typeof(InputToEvent))]
public class PointedAtGameObjectInfo : MonoBehaviour
{
	private void OnGUI()
	{
		if (InputToEvent.goPointedAt != null)
		{
			PhotonView photonView = InputToEvent.goPointedAt.GetPhotonView();
			if (photonView != null)
			{
				float left = Input.mousePosition.x + 5f;
				float num = Screen.height;
				GUI.Label(new Rect(left, num - Input.mousePosition.y - 15f, 300f, 30f), string.Format("ViewID {0} InstID {1} Lvl {2} {3}", photonView.viewID, photonView.instantiationId, photonView.prefix, photonView.isSceneView ? "scene" : ((!photonView.isMine) ? ("owner: " + photonView.ownerId) : "mine")));
			}
		}
	}
}
