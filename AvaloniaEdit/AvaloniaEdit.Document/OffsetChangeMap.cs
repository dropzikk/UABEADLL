using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AvaloniaEdit.Document;

public sealed class OffsetChangeMap : Collection<OffsetChangeMapEntry>
{
	public static readonly OffsetChangeMap Empty = new OffsetChangeMap(new OffsetChangeMapEntry[0], isFrozen: true);

	public bool IsFrozen { get; private set; }

	public static OffsetChangeMap FromSingleElement(OffsetChangeMapEntry entry)
	{
		return new OffsetChangeMap(new OffsetChangeMapEntry[1] { entry }, isFrozen: true);
	}

	public OffsetChangeMap()
	{
	}

	internal OffsetChangeMap(int capacity)
		: base((IList<OffsetChangeMapEntry>)new List<OffsetChangeMapEntry>(capacity))
	{
	}

	private OffsetChangeMap(IList<OffsetChangeMapEntry> entries, bool isFrozen)
		: base(entries)
	{
		IsFrozen = isFrozen;
	}

	public int GetNewOffset(int offset, AnchorMovementType movementType = AnchorMovementType.Default)
	{
		IList<OffsetChangeMapEntry> list = base.Items;
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			offset = list[i].GetNewOffset(offset, movementType);
		}
		return offset;
	}

	public bool IsValidForDocumentChange(int offset, int removalLength, int insertionLength)
	{
		int num = offset + removalLength;
		using (IEnumerator<OffsetChangeMapEntry> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				OffsetChangeMapEntry current = enumerator.Current;
				if (current.Offset < offset || current.Offset + current.RemovalLength > num)
				{
					return false;
				}
				num += current.InsertionLength - current.RemovalLength;
			}
		}
		return num == offset + insertionLength;
	}

	public OffsetChangeMap Invert()
	{
		if (this == Empty)
		{
			return this;
		}
		OffsetChangeMap offsetChangeMap = new OffsetChangeMap(base.Count);
		for (int num = base.Count - 1; num >= 0; num--)
		{
			OffsetChangeMapEntry offsetChangeMapEntry = base[num];
			offsetChangeMap.Add(new OffsetChangeMapEntry(offsetChangeMapEntry.Offset, offsetChangeMapEntry.InsertionLength, offsetChangeMapEntry.RemovalLength));
		}
		return offsetChangeMap;
	}

	protected override void ClearItems()
	{
		CheckFrozen();
		base.ClearItems();
	}

	protected override void InsertItem(int index, OffsetChangeMapEntry item)
	{
		CheckFrozen();
		base.InsertItem(index, item);
	}

	protected override void RemoveItem(int index)
	{
		CheckFrozen();
		base.RemoveItem(index);
	}

	protected override void SetItem(int index, OffsetChangeMapEntry item)
	{
		CheckFrozen();
		base.SetItem(index, item);
	}

	private void CheckFrozen()
	{
		if (IsFrozen)
		{
			throw new InvalidOperationException("This instance is frozen and cannot be modified.");
		}
	}

	public void Freeze()
	{
		IsFrozen = true;
	}
}
