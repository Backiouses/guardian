using UnityEngine;

public class HERO_ON_MENU : MonoBehaviour
{
	public int costumeId;

	private Transform head;

	private Transform cameraPref;

	public float headRotationX;

	public float headRotationY;

	private Vector3 cameraOffset;

	private void Start()
	{
		HERO_SETUP component = base.gameObject.GetComponent<HERO_SETUP>();
		HeroCostume.Init();
		component.Init();
		component.myCostume = HeroCostume.Costumes[costumeId];
		component.CreateCharacterComponent();
		head = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head");
		cameraPref = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
		if (costumeId == 9)
		{
			cameraOffset = GameObject.Find("MainCamera_Mono").transform.position - cameraPref.position;
		}
		if (component.myCostume.sex == Sex.Female)
		{
			base.animation.Play("stand");
			base.animation["stand"].normalizedTime = Random.Range(0f, 1f);
		}
		else
		{
			base.animation.Play("stand_levi");
			base.animation["stand_levi"].normalizedTime = Random.Range(0f, 1f);
		}
		AnimationState animationState = base.animation["stand_levi"];
		float speed = 0.5f;
		base.animation["stand"].speed = speed;
		animationState.speed = speed;
	}

	private void LateUpdate()
	{
		Transform obj = head;
		float x = head.rotation.eulerAngles.x + headRotationX;
		float y = head.rotation.eulerAngles.y + headRotationY;
		obj.rotation = Quaternion.Euler(x, y, head.rotation.eulerAngles.z);
		if (costumeId == 9)
		{
			GameObject.Find("MainCamera_Mono").transform.position = cameraPref.position + cameraOffset;
		}
	}
}
