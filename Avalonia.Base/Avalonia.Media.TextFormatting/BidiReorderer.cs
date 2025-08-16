using System;
using Avalonia.Utilities;

namespace Avalonia.Media.TextFormatting;

internal sealed class BidiReorderer
{
	private struct BidiRange
	{
		public sbyte Level { get; set; }

		public int LeftRunIndex { get; set; }

		public int RightRunIndex { get; set; }

		public int PreviousRangeIndex { get; }

		public BidiRange(sbyte level, int leftRunIndex, int rightRunIndex, int previousRangeIndex)
		{
			Level = level;
			LeftRunIndex = leftRunIndex;
			RightRunIndex = rightRunIndex;
			PreviousRangeIndex = previousRangeIndex;
		}
	}

	[ThreadStatic]
	private static BidiReorderer? t_instance;

	private ArrayBuilder<OrderedBidiRun> _runs;

	private ArrayBuilder<BidiRange> _ranges;

	public static BidiReorderer Instance => t_instance ?? (t_instance = new BidiReorderer());

	public IndexedTextRun[] BidiReorder(Span<TextRun> textRuns, FlowDirection flowDirection, int firstTextSourceIndex)
	{
		if (textRuns.IsEmpty)
		{
			return Array.Empty<IndexedTextRun>();
		}
		try
		{
			_runs.Add(textRuns.Length);
			for (int i = 0; i < textRuns.Length; i++)
			{
				TextRun run = textRuns[i];
				_runs[i] = new OrderedBidiRun(i, run, GetRunBidiLevel(run, flowDirection));
				if (i > 0)
				{
					_runs[i - 1].NextRunIndex = i;
				}
			}
			int num = LinearReorder();
			IndexedTextRun[] array = new IndexedTextRun[textRuns.Length];
			for (int j = 0; j < textRuns.Length; j++)
			{
				TextRun textRun = textRuns[j];
				array[j] = new IndexedTextRun
				{
					TextRun = textRun,
					TextSourceCharacterIndex = firstTextSourceIndex,
					RunIndex = j,
					NextRunIndex = j + 1
				};
				firstTextSourceIndex += textRun.Length;
			}
			sbyte b = 0;
			sbyte b2 = sbyte.MaxValue;
			for (int k = 0; k < textRuns.Length; k++)
			{
				sbyte runBidiLevel = GetRunBidiLevel(textRuns[k], flowDirection);
				if (runBidiLevel > b)
				{
					b = runBidiLevel;
				}
				if ((runBidiLevel & 1) != 0 && runBidiLevel < b2)
				{
					b2 = runBidiLevel;
				}
			}
			if (b2 > b)
			{
				b2 = b;
			}
			if (b == 0 || (b2 == b && (b & 1) == 0))
			{
				return array;
			}
			int num2;
			for (sbyte b3 = b; b3 >= b2; b3--)
			{
				num2 = num;
				while (num2 >= 0)
				{
					ref OrderedBidiRun reference = ref _runs[num2];
					if (reference.Level >= b3 && reference.Level % 2 != 0 && reference.Run is ShapedTextRun { IsReversed: false } shapedTextRun)
					{
						shapedTextRun.Reverse();
					}
					num2 = reference.NextRunIndex;
				}
			}
			int num3 = 0;
			num2 = num;
			while (num2 >= 0)
			{
				ref OrderedBidiRun reference2 = ref _runs[num2];
				textRuns[num3] = reference2.Run;
				IndexedTextRun obj = array[num3];
				obj.RunIndex = reference2.RunIndex;
				obj.NextRunIndex = reference2.NextRunIndex;
				num3++;
				num2 = reference2.NextRunIndex;
			}
			return array;
		}
		finally
		{
			FormattingBufferHelper.ClearThenResetIfTooLarge(ref _runs);
			FormattingBufferHelper.ClearThenResetIfTooLarge(ref _ranges);
		}
	}

	private static sbyte GetRunBidiLevel(TextRun run, FlowDirection flowDirection)
	{
		if (run is ShapedTextRun shapedTextRun)
		{
			return shapedTextRun.BidiLevel;
		}
		return (sbyte)((flowDirection != 0) ? 1 : 0);
	}

	private int LinearReorder()
	{
		int num = 0;
		int num2 = -1;
		while (num >= 0)
		{
			ref OrderedBidiRun reference = ref _runs[num];
			int nextRunIndex = reference.NextRunIndex;
			while (num2 >= 0 && _ranges[num2].Level > reference.Level && _ranges[num2].PreviousRangeIndex >= 0 && _ranges[_ranges[num2].PreviousRangeIndex].Level >= reference.Level)
			{
				num2 = MergeRangeWithPrevious(num2);
			}
			if (num2 >= 0 && _ranges[num2].Level >= reference.Level)
			{
				if ((reference.Level & 1) != 0)
				{
					reference.NextRunIndex = _ranges[num2].LeftRunIndex;
					_ranges[num2].LeftRunIndex = num;
				}
				else
				{
					_runs[_ranges[num2].RightRunIndex].NextRunIndex = num;
					_ranges[num2].RightRunIndex = num;
				}
				_ranges[num2].Level = reference.Level;
			}
			else
			{
				BidiRange value = new BidiRange(reference.Level, num, num, num2);
				_ranges.AddItem(value);
				num2 = _ranges.Length - 1;
			}
			num = nextRunIndex;
		}
		while (num2 >= 0 && _ranges[num2].PreviousRangeIndex >= 0)
		{
			num2 = MergeRangeWithPrevious(num2);
		}
		_runs[_ranges[num2].RightRunIndex].NextRunIndex = -1;
		return _runs[_ranges[num2].LeftRunIndex].RunIndex;
	}

	private int MergeRangeWithPrevious(int index)
	{
		int previousRangeIndex = _ranges[index].PreviousRangeIndex;
		ref BidiRange reference = ref _ranges[previousRangeIndex];
		int index2;
		int index3;
		if ((reference.Level & 1) != 0)
		{
			index2 = index;
			index3 = previousRangeIndex;
		}
		else
		{
			index2 = previousRangeIndex;
			index3 = index;
		}
		ref BidiRange reference2 = ref _ranges[index2];
		ref BidiRange reference3 = ref _ranges[index3];
		_runs[reference2.RightRunIndex].NextRunIndex = _runs[reference3.LeftRunIndex].RunIndex;
		reference.LeftRunIndex = reference2.LeftRunIndex;
		reference.RightRunIndex = reference3.RightRunIndex;
		return previousRangeIndex;
	}
}
