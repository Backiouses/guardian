using Guardian;
using Guardian.Utilities;
using UnityEngine;

public class FlareMovement : MonoBehaviour
{
	public GameObject hint;

	private GameObject hero;

	public string color;

	private Vector3 offY;

	private bool nohint;

	private float timer;

	private void Start()
	{
		if (ResourceLoader.TryGetAsset<Texture2D>("Custom/Textures/flare.png", out var value))
		{
			GetComponent<ParticleSystem>().renderer.material.mainTexture = value;
		}
		Color color;
		switch (this.color)
		{
		case "Green":
			color = GuardianClient.Properties.Flare1Color.Value.ToColor();
			break;
		case "Red":
			color = GuardianClient.Properties.Flare2Color.Value.ToColor();
			break;
		case "Black":
			color = GuardianClient.Properties.Flare3Color.Value.ToColor();
			break;
		default:
			color = Color.white;
			break;
		}
		Color startColor = color;
		Minimap.Instance.TrackGameObjectOnMinimap(base.gameObject, GetComponent<ParticleSystem>().startColor, trackOrientation: true, depthAboveAll: true);
		GetComponent<ParticleSystem>().startColor = startColor;
		if (GuardianClient.Properties.EmissiveFlares.Value)
		{
			Light obj = base.gameObject.AddComponent<Light>();
			obj.type = LightType.Point;
			obj.intensity = 1f;
			obj.range = 125f;
			obj.color = GetComponent<ParticleSystem>().startColor;
			obj.renderMode = LightRenderMode.ForcePixel;
		}
		hero = GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().main_object;
		if (!nohint && hero != null)
		{
			hint = (GameObject)Object.Instantiate(Resources.Load("UI/" + this.color + "FlareHint"));
			hint.GetComponent<MeshRenderer>().material.color = startColor;
			if (this.color == "Black")
			{
				offY = Vector3.up * 0.4f;
			}
			else
			{
				offY = Vector3.up * 0.5f;
			}
			hint.transform.parent = base.transform.root;
			hint.transform.position = hero.transform.position + offY;
			Vector3 vector = base.transform.position - hint.transform.position;
			float num = Mathf.Atan2(0f - vector.z, vector.x) * 57.29578f;
			hint.transform.rotation = Quaternion.Euler(-90f, num + 180f, 0f);
			hint.transform.localScale = Vector3.zero;
			iTween.ScaleTo(hint, iTween.Hash("x", 1f, "y", 1f, "z", 1f, "easetype", iTween.EaseType.easeOutElastic, "time", 1f));
			iTween.ScaleTo(hint, iTween.Hash("x", 0, "y", 0, "z", 0, "easetype", iTween.EaseType.easeInBounce, "time", 0.5f, "delay", 2.5f));
		}
	}

	public void DontShowHint()
	{
		Object.Destroy(hint);
		nohint = true;
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (hint != null)
		{
			if (timer < 3f)
			{
				hint.transform.position = hero.transform.position + offY;
				Vector3 vector = base.transform.position - hint.transform.position;
				float num = Mathf.Atan2(0f - vector.z, vector.x) * 57.29578f;
				hint.transform.rotation = Quaternion.Euler(-90f, num + 180f, 0f);
			}
			else
			{
				Object.Destroy(hint);
			}
		}
		if (timer < 4f)
		{
			base.rigidbody.AddForce((base.transform.forward + base.transform.up * 5f) * Time.deltaTime * 5f, ForceMode.VelocityChange);
		}
		else
		{
			base.rigidbody.AddForce(-base.transform.up * Time.deltaTime * 7f, ForceMode.Acceleration);
		}
	}
}
