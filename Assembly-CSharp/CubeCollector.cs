using UnityEngine;

public class CubeCollector : MonoBehaviour
{
	public int type;

	private void Update()
	{
		if (!(GameObject.FindGameObjectWithTag("Player") == null) && Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, base.transform.position) < 8f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
