using UnityEngine;

public class FengPlayer
{
	public string Name = "GUEST";

	public string Guild = string.Empty;

	public void InitAsGuest()
	{
		Name = "GUEST" + Random.Range(0, 65534);
		Guild = "No Guild";
	}
}
