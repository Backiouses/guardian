using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Table")]
public class UITable : MonoBehaviour
{
	public enum Direction
	{
		Down,
		Up
	}

	public delegate void OnReposition();

	public int columns;

	public Direction direction;

	public Vector2 padding = Vector2.zero;

	public bool sorted;

	public bool hideInactive = true;

	public bool repositionNow;

	public bool keepWithinPanel;

	public OnReposition onReposition;

	private UIPanel mPanel;

	private UIDraggablePanel mDrag;

	private bool mStarted;

	private List<Transform> mChildren = new List<Transform>();

	public List<Transform> children
	{
		get
		{
			if (mChildren.Count == 0)
			{
				Transform transform = base.transform;
				mChildren.Clear();
				for (int i = 0; i < transform.childCount; i++)
				{
					Transform child = transform.GetChild(i);
					if ((bool)child && (bool)child.gameObject && (!hideInactive || NGUITools.GetActive(child.gameObject)))
					{
						mChildren.Add(child);
					}
				}
				if (sorted)
				{
					mChildren.Sort(SortByName);
				}
			}
			return mChildren;
		}
	}

	public static int SortByName(Transform a, Transform b)
	{
		return string.Compare(a.name, b.name);
	}

	private void RepositionVariableSize(List<Transform> children)
	{
		float num = 0f;
		float num2 = 0f;
		int num3 = ((columns <= 0) ? 1 : (children.Count / columns + 1));
		int num4 = ((columns <= 0) ? children.Count : columns);
		Bounds[,] array = new Bounds[num3, num4];
		Bounds[] array2 = new Bounds[num4];
		Bounds[] array3 = new Bounds[num3];
		int num5 = 0;
		int num6 = 0;
		int i = 0;
		for (int count = children.Count; i < count; i++)
		{
			Transform obj = children[i];
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(obj);
			Vector3 localScale = obj.localScale;
			bounds.min = Vector3.Scale(bounds.min, localScale);
			bounds.max = Vector3.Scale(bounds.max, localScale);
			array[num6, num5] = bounds;
			array2[num5].Encapsulate(bounds);
			array3[num6].Encapsulate(bounds);
			if (++num5 >= columns && columns > 0)
			{
				num5 = 0;
				num6++;
			}
		}
		num5 = 0;
		num6 = 0;
		int j = 0;
		for (int count2 = children.Count; j < count2; j++)
		{
			Transform obj2 = children[j];
			Bounds bounds2 = array[num6, num5];
			Bounds bounds3 = array2[num5];
			Bounds bounds4 = array3[num6];
			Vector3 localPosition = obj2.localPosition;
			float num7 = num + bounds2.extents.x;
			localPosition.x = num7 - bounds2.center.x;
			float x = localPosition.x;
			float x2 = bounds2.min.x;
			localPosition.x = x + (x2 - bounds3.min.x + padding.x);
			if (direction == Direction.Down)
			{
				float num8 = 0f - num2 - bounds2.extents.y;
				localPosition.y = num8 - bounds2.center.y;
				float y = localPosition.y;
				float num9 = bounds2.max.y - bounds2.min.y - bounds4.max.y;
				localPosition.y = y + ((num9 + bounds4.min.y) * 0.5f - padding.y);
			}
			else
			{
				float num10 = num2 + bounds2.extents.y;
				localPosition.y = num10 - bounds2.center.y;
				float y2 = localPosition.y;
				float num11 = bounds2.max.y - bounds2.min.y - bounds4.max.y;
				localPosition.y = y2 + ((num11 + bounds4.min.y) * 0.5f - padding.y);
			}
			float num12 = num;
			float x3 = bounds3.max.x;
			num = num12 + (x3 - bounds3.min.x + padding.x * 2f);
			obj2.localPosition = localPosition;
			if (++num5 >= columns && columns > 0)
			{
				num5 = 0;
				num6++;
				num = 0f;
				num2 += bounds4.size.y + padding.y * 2f;
			}
		}
	}

	public void Reposition()
	{
		if (mStarted)
		{
			Transform target = base.transform;
			mChildren.Clear();
			List<Transform> list = children;
			if (list.Count > 0)
			{
				RepositionVariableSize(list);
			}
			if (mDrag != null)
			{
				mDrag.UpdateScrollbars(recalculateBounds: true);
				mDrag.RestrictWithinBounds(instant: true);
			}
			else if (mPanel != null)
			{
				mPanel.ConstrainTargetToBounds(target, immediate: true);
			}
			if (onReposition != null)
			{
				onReposition();
			}
		}
		else
		{
			repositionNow = true;
		}
	}

	private void Start()
	{
		mStarted = true;
		if (keepWithinPanel)
		{
			mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
			mDrag = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
		}
		Reposition();
	}

	private void LateUpdate()
	{
		if (repositionNow)
		{
			repositionNow = false;
			Reposition();
		}
	}
}
