using UnityEngine;

[AddComponentMenu("NGUI/Examples/UI Storage Slot")]
public class UIStorageSlot : UIItemSlot
{
	public UIItemStorage storage;

	public int slot;

	protected override InvGameItem observedItem
	{
		get
		{
			if (!(storage == null))
			{
				return storage.GetItem(slot);
			}
			return null;
		}
	}

	protected override InvGameItem Replace(InvGameItem item)
	{
		if (!(storage == null))
		{
			return storage.Replace(slot, item);
		}
		return item;
	}
}
