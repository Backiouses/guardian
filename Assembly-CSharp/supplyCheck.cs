using UnityEngine;

public class supplyCheck : MonoBehaviour
{
	private float stepTime = 1f;

	private float elapsedTime;

	private void Update()
	{
		elapsedTime += Time.deltaTime;
		if (elapsedTime < stepTime)
		{
			return;
		}
		elapsedTime = 0f;
		foreach (HERO hero in FengGameManagerMKII.Instance.Heroes)
		{
			if ((hero.transform.position - base.transform.position).sqrMagnitude < 2.25f && (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer || hero.photonView.isMine))
			{
				hero.GetSupply();
			}
		}
	}

	private void Start()
	{
		if (Minimap.Instance != null)
		{
			Minimap.Instance.TrackGameObjectOnMinimap(base.gameObject, Color.white, trackOrientation: false, depthAboveAll: true, Minimap.IconStyle.SUPPLY);
		}
		stepTime = 0.015f;
	}
}
