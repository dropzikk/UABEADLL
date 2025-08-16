using System;
using Avalonia.Utilities;

namespace Avalonia.Media.TextFormatting.Unicode;

internal sealed class BidiData
{
	private bool _hasCleanState = true;

	private ArrayBuilder<BidiClass> _classes;

	private ArrayBuilder<BidiPairedBracketType> _pairedBracketTypes;

	private ArrayBuilder<int> _pairedBracketValues;

	private ArrayBuilder<BidiClass> _savedClasses;

	private ArrayBuilder<BidiPairedBracketType> _savedPairedBracketTypes;

	private ArrayBuilder<sbyte> _tempLevelBuffer;

	public sbyte ParagraphEmbeddingLevel { get; set; }

	public bool HasBrackets { get; private set; }

	public bool HasEmbeddings { get; private set; }

	public bool HasIsolates { get; private set; }

	public int Length { get; private set; }

	public ArraySlice<BidiClass> Classes { get; private set; }

	public ArraySlice<BidiPairedBracketType> PairedBracketTypes { get; private set; }

	public ArraySlice<int> PairedBracketValues { get; private set; }

	public void Append(ReadOnlySpan<char> text)
	{
		_hasCleanState = false;
		_classes.Add(text.Length);
		_pairedBracketTypes.Add(text.Length);
		_pairedBracketValues.Add(text.Length);
		int num = Length;
		CodepointEnumerator codepointEnumerator = new CodepointEnumerator(text);
		Codepoint codepoint;
		while (codepointEnumerator.MoveNext(out codepoint))
		{
			BidiClass biDiClass = codepoint.BiDiClass;
			_classes[num] = biDiClass;
			uint num2 = (uint)(1 << (int)biDiClass);
			HasEmbeddings = (num2 & 0x149400) != 0;
			HasIsolates = (num2 & 0x90A00) != 0;
			BidiPairedBracketType pairedBracketType = codepoint.PairedBracketType;
			_pairedBracketTypes[num] = pairedBracketType;
			switch (pairedBracketType)
			{
			case BidiPairedBracketType.Open:
			{
				codepoint.TryGetPairedBracket(out var codepoint2);
				_pairedBracketValues[num] = (int)Codepoint.GetCanonicalType(codepoint2).Value;
				HasBrackets = true;
				break;
			}
			case BidiPairedBracketType.Close:
				_pairedBracketValues[num] = (int)Codepoint.GetCanonicalType(codepoint).Value;
				HasBrackets = true;
				break;
			}
			num++;
		}
		Length = num;
		Classes = _classes.AsSlice(0, Length);
		PairedBracketTypes = _pairedBracketTypes.AsSlice(0, Length);
		PairedBracketValues = _pairedBracketValues.AsSlice(0, Length);
	}

	public void SaveTypes()
	{
		_hasCleanState = false;
		_savedClasses.Clear();
		_savedClasses.Add(_classes.AsSlice());
		_savedPairedBracketTypes.Clear();
		_savedPairedBracketTypes.Add(_pairedBracketTypes.AsSlice());
	}

	public void RestoreTypes()
	{
		_hasCleanState = false;
		_classes.Clear();
		_classes.Add(_savedClasses.AsSlice());
		_pairedBracketTypes.Clear();
		_pairedBracketTypes.Add(_savedPairedBracketTypes.AsSlice());
	}

	public ArraySlice<sbyte> GetTempLevelBuffer(int length)
	{
		_tempLevelBuffer.Clear();
		return _tempLevelBuffer.Add(length, clear: false);
	}

	public void Reset()
	{
		if (!_hasCleanState)
		{
			FormattingBufferHelper.ClearThenResetIfTooLarge(ref _classes);
			FormattingBufferHelper.ClearThenResetIfTooLarge(ref _pairedBracketTypes);
			FormattingBufferHelper.ClearThenResetIfTooLarge(ref _pairedBracketValues);
			FormattingBufferHelper.ClearThenResetIfTooLarge(ref _savedClasses);
			FormattingBufferHelper.ClearThenResetIfTooLarge(ref _savedPairedBracketTypes);
			FormattingBufferHelper.ClearThenResetIfTooLarge(ref _tempLevelBuffer);
			ParagraphEmbeddingLevel = 0;
			HasBrackets = false;
			HasEmbeddings = false;
			HasIsolates = false;
			Length = 0;
			Classes = default(ArraySlice<BidiClass>);
			PairedBracketTypes = default(ArraySlice<BidiPairedBracketType>);
			PairedBracketValues = default(ArraySlice<int>);
			_hasCleanState = true;
		}
	}
}
