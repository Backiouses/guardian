using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Text List")]
public class UITextList : MonoBehaviour
{
	public enum Style
	{
		Text,
		Chat
	}

	protected class Paragraph
	{
		public string text;

		public string[] lines;
	}

	public Style style;

	public UILabel textLabel;

	public float maxWidth;

	public float maxHeight;

	public int maxEntries = 50;

	public bool supportScrollWheel = true;

	protected char[] mSeparator = new char[1] { '\n' };

	protected List<Paragraph> mParagraphs = new List<Paragraph>();

	protected float mScroll;

	protected bool mSelected;

	protected int mTotalLines;

	public void Clear()
	{
		mParagraphs.Clear();
		UpdateVisibleText();
	}

	public void Add(string text)
	{
		Add(text, updateVisible: true);
	}

	protected void Add(string text, bool updateVisible)
	{
		Paragraph paragraph = null;
		if (mParagraphs.Count < maxEntries)
		{
			paragraph = new Paragraph();
		}
		else
		{
			paragraph = mParagraphs[0];
			mParagraphs.RemoveAt(0);
		}
		paragraph.text = text;
		mParagraphs.Add(paragraph);
		if (textLabel != null && textLabel.font != null)
		{
			Paragraph paragraph2 = paragraph;
			UIFont font = textLabel.font;
			string text2 = paragraph.text;
			float num = maxWidth;
			paragraph2.lines = font.WrapText(text2, num / textLabel.transform.localScale.y, textLabel.maxLineCount, textLabel.supportEncoding, textLabel.symbolStyle).Split(mSeparator);
			mTotalLines = 0;
			int i = 0;
			for (int count = mParagraphs.Count; i < count; i++)
			{
				mTotalLines += mParagraphs[i].lines.Length;
			}
		}
		if (updateVisible)
		{
			UpdateVisibleText();
		}
	}

	private void Awake()
	{
		if (textLabel == null)
		{
			textLabel = GetComponentInChildren<UILabel>();
		}
		if (textLabel != null)
		{
			textLabel.lineWidth = 0;
		}
		Collider collider = base.collider;
		if (collider != null)
		{
			if (maxHeight <= 0f)
			{
				float y = collider.bounds.size.y;
				maxHeight = y / base.transform.lossyScale.y;
			}
			if (maxWidth <= 0f)
			{
				float x = collider.bounds.size.x;
				maxWidth = x / base.transform.lossyScale.x;
			}
		}
	}

	private void OnSelect(bool selected)
	{
		mSelected = selected;
	}

	protected void UpdateVisibleText()
	{
		if (textLabel == null || textLabel.font == null)
		{
			return;
		}
		int num = 0;
		int num2 = ((!(maxHeight > 0f)) ? 100000 : Mathf.FloorToInt(maxHeight / textLabel.cachedTransform.localScale.y));
		int num3 = num2;
		int num4 = Mathf.RoundToInt(mScroll);
		if (num3 + num4 > mTotalLines)
		{
			num4 = Mathf.Max(0, mTotalLines - num3);
			mScroll = num4;
		}
		if (style == Style.Chat)
		{
			num4 = Mathf.Max(0, mTotalLines - num3 - num4);
		}
		StringBuilder stringBuilder = new StringBuilder();
		int i = 0;
		for (int count = mParagraphs.Count; i < count; i++)
		{
			Paragraph paragraph = mParagraphs[i];
			int j = 0;
			for (int num5 = paragraph.lines.Length; j < num5; j++)
			{
				string value = paragraph.lines[j];
				if (num4 > 0)
				{
					num4--;
					continue;
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("\n");
				}
				stringBuilder.Append(value);
				num++;
				if (num >= num3)
				{
					break;
				}
			}
			if (num >= num3)
			{
				break;
			}
		}
		textLabel.text = stringBuilder.ToString();
	}

	private void OnScroll(float val)
	{
		if (mSelected && supportScrollWheel)
		{
			val *= ((style != Style.Chat) ? (-10f) : 10f);
			mScroll = Mathf.Max(0f, mScroll + val);
			UpdateVisibleText();
		}
	}
}
