using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.Highlighting;

public class RichText
{
	public static readonly RichText Empty = new RichText(string.Empty);

	internal int[] StateChangeOffsets { get; }

	internal HighlightingColor[] StateChanges { get; }

	public string Text { get; }

	public int Length => Text.Length;

	public RichText(string text, RichTextModel model = null)
	{
		Text = text ?? throw new ArgumentNullException("text");
		if (model != null)
		{
			HighlightedSection[] array = model.GetHighlightedSections(0, text.Length).ToArray();
			StateChangeOffsets = new int[array.Length];
			StateChanges = new HighlightingColor[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				StateChangeOffsets[i] = array[i].Offset;
				StateChanges[i] = array[i].Color;
			}
		}
		else
		{
			StateChangeOffsets = new int[1];
			StateChanges = new HighlightingColor[1] { HighlightingColor.Empty };
		}
	}

	internal RichText(string text, int[] offsets, HighlightingColor[] states)
	{
		Text = text;
		StateChangeOffsets = offsets;
		StateChanges = states;
	}

	private int GetIndexForOffset(int offset)
	{
		if (offset < 0 || offset > Text.Length)
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		int num = Array.BinarySearch(StateChangeOffsets, offset);
		if (num < 0)
		{
			num = ~num - 1;
		}
		return num;
	}

	private int GetEnd(int index)
	{
		if (index + 1 < StateChangeOffsets.Length)
		{
			return StateChangeOffsets[index + 1];
		}
		return Text.Length;
	}

	public HighlightingColor GetHighlightingAt(int offset)
	{
		return StateChanges[GetIndexForOffset(offset)];
	}

	public IEnumerable<HighlightedSection> GetHighlightedSections(int offset, int length)
	{
		int index = GetIndexForOffset(offset);
		int num = offset;
		int endOffset = offset + length;
		while (num < endOffset)
		{
			int endPos = Math.Min(endOffset, GetEnd(index));
			yield return new HighlightedSection
			{
				Offset = num,
				Length = endPos - num,
				Color = StateChanges[index]
			};
			num = endPos;
			index++;
		}
	}

	public RichTextModel ToRichTextModel()
	{
		return new RichTextModel(StateChangeOffsets, StateChanges.Select((HighlightingColor ch) => ch.Clone()).ToArray());
	}

	public override string ToString()
	{
		return Text;
	}

	public RichText Substring(int offset, int length)
	{
		if (offset == 0 && length == Length)
		{
			return this;
		}
		string text = Text.Substring(offset, length);
		RichTextModel richTextModel = ToRichTextModel();
		OffsetChangeMap change = new OffsetChangeMap(2)
		{
			new OffsetChangeMapEntry(offset + length, Text.Length - offset - length, 0),
			new OffsetChangeMapEntry(0, offset, 0)
		};
		richTextModel.UpdateOffsets(change);
		return new RichText(text, richTextModel);
	}

	public static RichText Concat(params RichText[] texts)
	{
		if (texts == null || texts.Length == 0)
		{
			return Empty;
		}
		if (texts.Length == 1)
		{
			return texts[0];
		}
		string text = string.Concat(texts.Select((RichText txt) => txt.Text));
		RichTextModel richTextModel = texts[0].ToRichTextModel();
		int num = texts[0].Length;
		for (int i = 1; i < texts.Length; i++)
		{
			richTextModel.Append(num, texts[i].StateChangeOffsets, texts[i].StateChanges);
			num += texts[i].Length;
		}
		return new RichText(text, richTextModel);
	}

	public static RichText operator +(RichText a, RichText b)
	{
		return Concat(a, b);
	}

	public static implicit operator RichText(string text)
	{
		if (text != null)
		{
			return new RichText(text);
		}
		return null;
	}
}
