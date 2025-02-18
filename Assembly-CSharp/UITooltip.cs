using UnityEngine;

[AddComponentMenu("NGUI/UI/Tooltip")]
public class UITooltip : MonoBehaviour
{
	private static UITooltip mInstance;

	public Camera uiCamera;

	public UILabel text;

	public UISprite background;

	public float appearSpeed = 10f;

	public bool scalingTransitions = true;

	private Transform mTrans;

	private float mTarget;

	private float mCurrent;

	private Vector3 mPos;

	private Vector3 mSize;

	private UIWidget[] mWidgets;

	private void Awake()
	{
		mInstance = this;
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void Start()
	{
		mTrans = base.transform;
		mWidgets = GetComponentsInChildren<UIWidget>();
		mPos = mTrans.localPosition;
		mSize = mTrans.localScale;
		if (uiCamera == null)
		{
			uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
		}
		SetAlpha(0f);
	}

	private void Update()
	{
		if (mCurrent != mTarget)
		{
			mCurrent = Mathf.Lerp(mCurrent, mTarget, Time.deltaTime * appearSpeed);
			if (Mathf.Abs(mCurrent - mTarget) < 0.001f)
			{
				mCurrent = mTarget;
			}
			SetAlpha(mCurrent * mCurrent);
			if (scalingTransitions)
			{
				Vector3 vector = mSize * 0.25f;
				vector.y = 0f - vector.y;
				Vector3 localScale = Vector3.one * (1.5f - mCurrent * 0.5f);
				Vector3 localPosition = Vector3.Lerp(mPos - vector, mPos, mCurrent);
				mTrans.localPosition = localPosition;
				mTrans.localScale = localScale;
			}
		}
	}

	private void SetAlpha(float val)
	{
		int i = 0;
		for (int num = mWidgets.Length; i < num; i++)
		{
			UIWidget obj = mWidgets[i];
			Color color = obj.color;
			color.a = val;
			obj.color = color;
		}
	}

	private void SetText(string tooltipText)
	{
		if (text != null && !string.IsNullOrEmpty(tooltipText))
		{
			mTarget = 1f;
			if (text != null)
			{
				text.text = tooltipText;
			}
			mPos = Input.mousePosition;
			if (background != null)
			{
				Transform obj = background.transform;
				Transform obj2 = text.transform;
				Vector3 localPosition = obj2.localPosition;
				Vector3 localScale = obj2.localScale;
				mSize = text.relativeSize;
				mSize.x *= localScale.x;
				mSize.y *= localScale.y;
				ref Vector3 reference = ref mSize;
				float x = reference.x;
				float num = background.border.x + background.border.z;
				float x2 = localPosition.x;
				reference.x = x + (num + (x2 - background.border.x) * 2f);
				ref Vector3 reference2 = ref mSize;
				float y = reference2.y;
				float num2 = background.border.y + background.border.w;
				float num3 = 0f - localPosition.y;
				reference2.y = y + (num2 + (num3 - background.border.y) * 2f);
				mSize.z = 1f;
				obj.localScale = mSize;
			}
			if (uiCamera != null)
			{
				mPos.x = Mathf.Clamp01(mPos.x / (float)Screen.width);
				mPos.y = Mathf.Clamp01(mPos.y / (float)Screen.height);
				float num4 = uiCamera.orthographicSize / mTrans.parent.lossyScale.y;
				float num5 = (float)Screen.height * 0.5f / num4;
				Vector2 vector = new Vector2(num5 * mSize.x / (float)Screen.width, num5 * mSize.y / (float)Screen.height);
				mPos.x = Mathf.Min(mPos.x, 1f - vector.x);
				mPos.y = Mathf.Max(mPos.y, vector.y);
				mTrans.position = uiCamera.ViewportToWorldPoint(mPos);
				mPos = mTrans.localPosition;
				mPos.x = Mathf.Round(mPos.x);
				mPos.y = Mathf.Round(mPos.y);
				mTrans.localPosition = mPos;
			}
			else
			{
				if (mPos.x + mSize.x > (float)Screen.width)
				{
					mPos.x = (float)Screen.width - mSize.x;
				}
				if (mPos.y - mSize.y < 0f)
				{
					mPos.y = mSize.y;
				}
				mPos.x -= (float)Screen.width * 0.5f;
				mPos.y -= (float)Screen.height * 0.5f;
			}
		}
		else
		{
			mTarget = 0f;
		}
	}

	public static void ShowText(string tooltipText)
	{
		if (mInstance != null)
		{
			mInstance.SetText(tooltipText);
		}
	}
}
