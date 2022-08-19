using UnityEngine;

public class KillInfoComponent : MonoBehaviour
{
	public GameObject leftTitan;

	public GameObject rightTitan;

	public GameObject labelScore;

	public GameObject labelNameLeft;

	public GameObject labelNameRight;

	public GameObject spriteSkeleton;

	public GameObject spriteSword;

	public GameObject sleftTitan;

	public GameObject srightTitan;

	public GameObject slabelScore;

	public GameObject slabelNameLeft;

	public GameObject slabelNameRight;

	public GameObject sspriteSkeleton;

	public GameObject sspriteSword;

	public GameObject groupBig;

	public GameObject groupSmall;

	private bool start;

	private float timeElapsed;

	private float lifeTime = 8f;

	private float alpha = 1f;

	private float maxScale = 1.5f;

	private int offset = 24;

	private int col;

	private void Start()
	{
		start = true;
		base.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
		base.transform.localPosition = new Vector3(0f, -100f + (float)Screen.height * 0.5f, 0f);
	}

	public void Show(bool isKillerTitan, string killer, bool isVictimTitan, string victim, int damage = 0)
	{
		groupBig.SetActive(value: true);
		groupSmall.SetActive(value: true);
		if (!isKillerTitan)
		{
			leftTitan.SetActive(value: false);
			spriteSkeleton.SetActive(value: false);
			sleftTitan.SetActive(value: false);
			sspriteSkeleton.SetActive(value: false);
			labelNameLeft.transform.position += new Vector3(18f, 0f, 0f);
			slabelNameLeft.transform.position += new Vector3(16f, 0f, 0f);
		}
		else
		{
			spriteSword.SetActive(value: false);
			sspriteSword.SetActive(value: false);
			labelNameRight.transform.position -= new Vector3(18f, 0f, 0f);
			slabelNameRight.transform.position -= new Vector3(16f, 0f, 0f);
		}
		if (!isVictimTitan)
		{
			rightTitan.SetActive(value: false);
			srightTitan.SetActive(value: false);
		}
		labelNameLeft.GetComponent<UILabel>().text = killer;
		labelNameRight.GetComponent<UILabel>().text = victim;
		slabelNameLeft.GetComponent<UILabel>().text = killer;
		slabelNameRight.GetComponent<UILabel>().text = victim;
		if (damage == 0)
		{
			labelScore.GetComponent<UILabel>().text = string.Empty;
			slabelScore.GetComponent<UILabel>().text = string.Empty;
		}
		else
		{
			labelScore.GetComponent<UILabel>().text = damage.ToString();
			slabelScore.GetComponent<UILabel>().text = damage.ToString();
			if (damage >= 1000)
			{
				labelScore.GetComponent<UILabel>().color = Color.red;
				slabelScore.GetComponent<UILabel>().color = Color.red;
			}
		}
		groupSmall.SetActive(value: false);
	}

	public void MoveOn()
	{
		col++;
		if (col > 4)
		{
			timeElapsed = lifeTime;
		}
		groupBig.SetActive(value: false);
		groupSmall.SetActive(value: true);
	}

	public void EndLifeTime()
	{
		timeElapsed = lifeTime;
	}

	private void Update()
	{
		if (start)
		{
			timeElapsed += Time.deltaTime;
			if (timeElapsed < 0.2f)
			{
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.one * maxScale, Time.deltaTime * 10f);
			}
			else if (timeElapsed < 1f)
			{
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.one, Time.deltaTime * 10f);
			}
			if (timeElapsed > lifeTime)
			{
				base.transform.position = base.transform.position + new Vector3(0f, Time.deltaTime * 0.15f, 0f);
				alpha = 1f - Time.deltaTime * 45f + lifeTime - timeElapsed;
				SetAlpha(alpha);
			}
			else
			{
				float num = (int)(100f - (float)Screen.height * 0.5f + (float)(col * offset));
				base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, new Vector3(0f, 0f - num, 0f), Time.deltaTime * 10f);
			}
			if (timeElapsed > lifeTime + 0.5f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	private void SetAlpha(float alpha)
	{
		if (groupBig.activeInHierarchy)
		{
			UILabel component = labelScore.GetComponent<UILabel>();
			float r = labelScore.GetComponent<UILabel>().color.r;
			float g = labelScore.GetComponent<UILabel>().color.g;
			component.color = new Color(r, g, labelScore.GetComponent<UILabel>().color.b, alpha);
			leftTitan.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, alpha);
			rightTitan.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, alpha);
			labelNameLeft.GetComponent<UILabel>().color = new Color(1f, 1f, 1f, alpha);
			labelNameRight.GetComponent<UILabel>().color = new Color(1f, 1f, 1f, alpha);
			spriteSkeleton.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, alpha);
			spriteSword.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, alpha);
		}
		if (groupSmall.activeInHierarchy)
		{
			UILabel component2 = slabelScore.GetComponent<UILabel>();
			float r2 = labelScore.GetComponent<UILabel>().color.r;
			float g2 = labelScore.GetComponent<UILabel>().color.g;
			component2.color = new Color(r2, g2, labelScore.GetComponent<UILabel>().color.b, alpha);
			sleftTitan.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, alpha);
			srightTitan.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, alpha);
			slabelNameLeft.GetComponent<UILabel>().color = new Color(1f, 1f, 1f, alpha);
			slabelNameRight.GetComponent<UILabel>().color = new Color(1f, 1f, 1f, alpha);
			sspriteSkeleton.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, alpha);
			sspriteSword.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, alpha);
		}
	}
}
