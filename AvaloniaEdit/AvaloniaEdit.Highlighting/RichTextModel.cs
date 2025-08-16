using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.Highlighting;

public sealed class RichTextModel
{
	private readonly List<int> _stateChangeOffsets = new List<int>();

	private readonly List<HighlightingColor> _stateChanges = new List<HighlightingColor>();

	private int GetIndexForOffset(int offset)
	{
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		int num = _stateChangeOffsets.BinarySearch(offset);
		if (num < 0)
		{
			num = ~num;
			_stateChanges.Insert(num, _stateChanges[num - 1].Clone());
			_stateChangeOffsets.Insert(num, offset);
		}
		return num;
	}

	private int GetIndexForOffsetUseExistingSegment(int offset)
	{
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		int num = _stateChangeOffsets.BinarySearch(offset);
		if (num < 0)
		{
			num = ~num - 1;
		}
		return num;
	}

	private int GetEnd(int index)
	{
		if (index + 1 < _stateChangeOffsets.Count)
		{
			return _stateChangeOffsets[index + 1];
		}
		return int.MaxValue;
	}

	public RichTextModel()
	{
		_stateChangeOffsets.Add(0);
		_stateChanges.Add(new HighlightingColor());
	}

	internal RichTextModel(int[] stateChangeOffsets, HighlightingColor[] stateChanges)
	{
		_stateChangeOffsets.AddRange(stateChangeOffsets);
		_stateChanges.AddRange(stateChanges);
	}

	public void UpdateOffsets(TextChangeEventArgs e)
	{
		if (e == null)
		{
			throw new ArgumentNullException("e");
		}
		UpdateOffsets(e.GetNewOffset);
	}

	public void UpdateOffsets(OffsetChangeMap change)
	{
		if (change == null)
		{
			throw new ArgumentNullException("change");
		}
		UpdateOffsets(change.GetNewOffset);
	}

	public void UpdateOffsets(OffsetChangeMapEntry change)
	{
		UpdateOffsets(((OffsetChangeMapEntry)change).GetNewOffset);
	}

	private void UpdateOffsets(Func<int, AnchorMovementType, int> updateOffset)
	{
		int i = 1;
		int num = 1;
		for (; i < _stateChangeOffsets.Count; i++)
		{
			int num2 = updateOffset(_stateChangeOffsets[i], AnchorMovementType.Default);
			if (num2 == _stateChangeOffsets[num - 1])
			{
				_stateChanges[num - 1] = _stateChanges[i];
				continue;
			}
			_stateChangeOffsets[num] = num2;
			_stateChanges[num] = _stateChanges[i];
			num++;
		}
		_stateChangeOffsets.RemoveRange(num, _stateChangeOffsets.Count - num);
		_stateChanges.RemoveRange(num, _stateChanges.Count - num);
	}

	internal void Append(int offset, int[] newOffsets, HighlightingColor[] newColors)
	{
		while (_stateChangeOffsets.Count > 0 && _stateChangeOffsets.Last() >= offset)
		{
			_stateChangeOffsets.RemoveAt(_stateChangeOffsets.Count - 1);
			_stateChanges.RemoveAt(_stateChanges.Count - 1);
		}
		for (int i = 0; i < newOffsets.Length; i++)
		{
			_stateChangeOffsets.Add(offset + newOffsets[i]);
			_stateChanges.Add(newColors[i]);
		}
	}

	public HighlightingColor GetHighlightingAt(int offset)
	{
		return _stateChanges[GetIndexForOffsetUseExistingSegment(offset)].Clone();
	}

	public void ApplyHighlighting(int offset, int length, HighlightingColor color)
	{
		if (color != null && !color.IsEmptyForMerge)
		{
			int indexForOffset = GetIndexForOffset(offset);
			int indexForOffset2 = GetIndexForOffset(offset + length);
			for (int i = indexForOffset; i < indexForOffset2; i++)
			{
				_stateChanges[i].MergeWith(color);
			}
		}
	}

	public void SetHighlighting(int offset, int length, HighlightingColor color)
	{
		if (length > 0)
		{
			int indexForOffset = GetIndexForOffset(offset);
			int indexForOffset2 = GetIndexForOffset(offset + length);
			_stateChanges[indexForOffset] = ((color != null) ? color.Clone() : new HighlightingColor());
			_stateChanges.RemoveRange(indexForOffset + 1, indexForOffset2 - (indexForOffset + 1));
			_stateChangeOffsets.RemoveRange(indexForOffset + 1, indexForOffset2 - (indexForOffset + 1));
		}
	}

	public void SetForeground(int offset, int length, HighlightingBrush brush)
	{
		int indexForOffset = GetIndexForOffset(offset);
		int indexForOffset2 = GetIndexForOffset(offset + length);
		for (int i = indexForOffset; i < indexForOffset2; i++)
		{
			_stateChanges[i].Foreground = brush;
		}
	}

	public void SetBackground(int offset, int length, HighlightingBrush brush)
	{
		int indexForOffset = GetIndexForOffset(offset);
		int indexForOffset2 = GetIndexForOffset(offset + length);
		for (int i = indexForOffset; i < indexForOffset2; i++)
		{
			_stateChanges[i].Background = brush;
		}
	}

	public void SetFontWeight(int offset, int length, FontWeight weight)
	{
		int indexForOffset = GetIndexForOffset(offset);
		int indexForOffset2 = GetIndexForOffset(offset + length);
		for (int i = indexForOffset; i < indexForOffset2; i++)
		{
			_stateChanges[i].FontWeight = weight;
		}
	}

	public void SetFontStyle(int offset, int length, FontStyle style)
	{
		int indexForOffset = GetIndexForOffset(offset);
		int indexForOffset2 = GetIndexForOffset(offset + length);
		for (int i = indexForOffset; i < indexForOffset2; i++)
		{
			_stateChanges[i].FontStyle = style;
		}
	}

	public IEnumerable<HighlightedSection> GetHighlightedSections(int offset, int length)
	{
		int index = GetIndexForOffsetUseExistingSegment(offset);
		int num = offset;
		int endOffset = offset + length;
		while (num < endOffset)
		{
			int endPos = Math.Min(endOffset, GetEnd(index));
			yield return new HighlightedSection
			{
				Offset = num,
				Length = endPos - num,
				Color = _stateChanges[index].Clone()
			};
			num = endPos;
			index++;
		}
	}
}
