using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Avalonia.Utilities;

namespace Avalonia.Media.TextFormatting.Unicode;

internal sealed class BidiAlgorithm
{
	private readonly struct BracketPair : IComparable<BracketPair>
	{
		public int OpeningIndex { get; }

		public int ClosingIndex { get; }

		public BracketPair(int openingIndex, int closingIndex)
		{
			OpeningIndex = openingIndex;
			ClosingIndex = closingIndex;
		}

		public int CompareTo(BracketPair other)
		{
			return OpeningIndex.CompareTo(other.OpeningIndex);
		}
	}

	private readonly struct Status
	{
		public sbyte EmbeddingLevel { get; }

		public BidiClass OverrideStatus { get; }

		public bool IsolateStatus { get; }

		public Status(sbyte embeddingLevel, BidiClass overrideStatus, bool isolateStatus)
		{
			EmbeddingLevel = embeddingLevel;
			OverrideStatus = overrideStatus;
			IsolateStatus = isolateStatus;
		}
	}

	private readonly struct LevelRun
	{
		public int Start { get; }

		public int Length { get; }

		public int Level { get; }

		public BidiClass Sos { get; }

		public BidiClass Eos { get; }

		public LevelRun(int start, int length, int level, BidiClass sos, BidiClass eos)
		{
			Start = start;
			Length = length;
			Level = level;
			Sos = sos;
			Eos = eos;
		}
	}

	private bool _hasCleanState = true;

	private ArraySlice<BidiClass> _originalClasses;

	private ArraySlice<BidiPairedBracketType> _pairedBracketTypes;

	private ArraySlice<int> _pairedBracketValues;

	private bool _hasBrackets;

	private bool _hasEmbeddings;

	private bool _hasIsolates;

	private readonly BidiDictionary<int, int> _isolatePairs = new BidiDictionary<int, int>();

	private ArraySlice<BidiClass> _workingClasses;

	private ArrayBuilder<BidiClass> _workingClassesBuffer;

	private ArraySlice<sbyte> _resolvedLevels;

	private ArrayBuilder<sbyte> _resolvedLevelsBuffer;

	private sbyte _paragraphEmbeddingLevel;

	private readonly Stack<Status> _statusStack = new Stack<Status>();

	private ArrayBuilder<int> _x9Map;

	private readonly List<LevelRun> _levelRuns = new List<LevelRun>();

	private ArrayBuilder<int> _isolatedRunMapping;

	private readonly Stack<int> _pendingIsolateOpenings = new Stack<int>();

	private int _runLevel;

	private BidiClass _runDirection;

	private int _runLength;

	private MappedArraySlice<BidiClass> _runResolvedClasses;

	private MappedArraySlice<BidiClass> _runOriginalClasses;

	private MappedArraySlice<sbyte> _runLevels;

	private MappedArraySlice<BidiPairedBracketType> _runBiDiPairedBracketTypes;

	private MappedArraySlice<int> _runPairedBracketValues;

	private const int MaxPairedBracketDepth = 63;

	private readonly List<int> _pendingOpeningBrackets = new List<int>();

	private readonly List<BracketPair> _pairedBrackets = new List<BracketPair>();

	public ArraySlice<sbyte> ResolvedLevels => _resolvedLevels;

	public int ResolvedParagraphEmbeddingLevel => _paragraphEmbeddingLevel;

	public void Process(BidiData data)
	{
		Process(data.Classes, data.PairedBracketTypes, data.PairedBracketValues, data.ParagraphEmbeddingLevel, data.HasBrackets, data.HasEmbeddings, data.HasIsolates, null);
	}

	public void Process(ArraySlice<BidiClass> types, ArraySlice<BidiPairedBracketType> pairedBracketTypes, ArraySlice<int> pairedBracketValues, sbyte paragraphEmbeddingLevel, bool? hasBrackets, bool? hasEmbeddings, bool? hasIsolates, ArraySlice<sbyte>? outLevels)
	{
		Reset();
		if (types.IsEmpty)
		{
			return;
		}
		_hasCleanState = false;
		_originalClasses = types;
		_workingClasses = _workingClassesBuffer.Add(in types);
		_pairedBracketTypes = pairedBracketTypes;
		_pairedBracketValues = pairedBracketValues;
		_hasBrackets = hasBrackets ?? (_pairedBracketTypes.Length == _originalClasses.Length);
		_hasEmbeddings = hasEmbeddings ?? true;
		_hasIsolates = hasIsolates ?? true;
		FindIsolatePairs();
		if (paragraphEmbeddingLevel == 2)
		{
			_paragraphEmbeddingLevel = ResolveEmbeddingLevel(_originalClasses);
		}
		else
		{
			_paragraphEmbeddingLevel = paragraphEmbeddingLevel;
		}
		if (outLevels.HasValue)
		{
			if (outLevels.Value.Length != _originalClasses.Length)
			{
				throw new ArgumentException("Out levels must be the same length as the input data");
			}
			_resolvedLevels = outLevels.Value;
		}
		else
		{
			_resolvedLevels = _resolvedLevelsBuffer.Add(_originalClasses.Length);
			_resolvedLevels.Fill(_paragraphEmbeddingLevel);
		}
		ResolveExplicitEmbeddingLevels();
		BuildX9RemovalMap();
		ProcessIsolatedRunSequences();
		ResetWhitespaceLevels();
		AssignLevelsToCodePointsRemovedByX9();
	}

	public sbyte ResolveEmbeddingLevel(ArraySlice<BidiClass> data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			switch (data[i])
			{
			case BidiClass.LeftToRight:
				return 0;
			case BidiClass.ArabicLetter:
			case BidiClass.RightToLeft:
				return 1;
			case BidiClass.FirstStrongIsolate:
			case BidiClass.LeftToRightIsolate:
			case BidiClass.RightToLeftIsolate:
				i = ((!_isolatePairs.TryGetValue(data.Start + i, out i)) ? data.Length : (i - data.Start));
				break;
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsIsolateStart(BidiClass type)
	{
		return ((1 << (int)type) & 0x80A00) != 0;
	}

	private void FindIsolatePairs()
	{
		if (!_hasIsolates)
		{
			return;
		}
		_hasIsolates = false;
		_pendingIsolateOpenings.Clear();
		for (int i = 0; i < _originalClasses.Length; i++)
		{
			switch (_originalClasses[i])
			{
			case BidiClass.FirstStrongIsolate:
			case BidiClass.LeftToRightIsolate:
			case BidiClass.RightToLeftIsolate:
				_pendingIsolateOpenings.Push(i);
				_hasIsolates = true;
				break;
			case BidiClass.PopDirectionalIsolate:
				if (_pendingIsolateOpenings.Count > 0)
				{
					_isolatePairs.Add(_pendingIsolateOpenings.Pop(), i);
				}
				_hasIsolates = true;
				break;
			}
		}
	}

	private void ResolveExplicitEmbeddingLevels()
	{
		if (!_hasIsolates && !_hasEmbeddings)
		{
			return;
		}
		_statusStack.Clear();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		_statusStack.Clear();
		_statusStack.Push(new Status(_paragraphEmbeddingLevel, BidiClass.OtherNeutral, isolateStatus: false));
		for (int i = 0; i < _originalClasses.Length; i++)
		{
			switch (_originalClasses[i])
			{
			case BidiClass.RightToLeftEmbedding:
			{
				sbyte b5 = (sbyte)((_statusStack.Peek().EmbeddingLevel + 1) | 1);
				if (b5 <= 125 && num == 0 && num2 == 0)
				{
					_statusStack.Push(new Status(b5, BidiClass.OtherNeutral, isolateStatus: false));
					_resolvedLevels[i] = b5;
				}
				else if (num == 0)
				{
					num2++;
				}
				break;
			}
			case BidiClass.LeftToRightEmbedding:
			{
				sbyte b = (sbyte)((_statusStack.Peek().EmbeddingLevel + 2) & -2);
				if (b < 125 && num == 0 && num2 == 0)
				{
					_statusStack.Push(new Status(b, BidiClass.OtherNeutral, isolateStatus: false));
					_resolvedLevels[i] = b;
				}
				else if (num == 0)
				{
					num2++;
				}
				break;
			}
			case BidiClass.RightToLeftOverride:
			{
				sbyte b4 = (sbyte)((_statusStack.Peek().EmbeddingLevel + 1) | 1);
				if (b4 <= 125 && num == 0 && num2 == 0)
				{
					_statusStack.Push(new Status(b4, BidiClass.RightToLeft, isolateStatus: false));
					_resolvedLevels[i] = b4;
				}
				else if (num == 0)
				{
					num2++;
				}
				break;
			}
			case BidiClass.LeftToRightOverride:
			{
				sbyte b3 = (sbyte)((_statusStack.Peek().EmbeddingLevel + 2) & -2);
				if (b3 <= 125 && num == 0 && num2 == 0)
				{
					_statusStack.Push(new Status(b3, BidiClass.LeftToRight, isolateStatus: false));
					_resolvedLevels[i] = b3;
				}
				else if (num == 0)
				{
					num2++;
				}
				break;
			}
			case BidiClass.FirstStrongIsolate:
			case BidiClass.LeftToRightIsolate:
			case BidiClass.RightToLeftIsolate:
			{
				BidiClass bidiClass = _originalClasses[i];
				if (bidiClass == BidiClass.FirstStrongIsolate)
				{
					if (!_isolatePairs.TryGetValue(i, out var value))
					{
						value = _originalClasses.Length;
					}
					bidiClass = ((ResolveEmbeddingLevel(_originalClasses.Slice(i + 1, value - (i + 1))) != 1) ? BidiClass.LeftToRightIsolate : BidiClass.RightToLeftIsolate);
				}
				Status status = _statusStack.Peek();
				_resolvedLevels[i] = status.EmbeddingLevel;
				if (status.OverrideStatus != BidiClass.OtherNeutral)
				{
					_workingClasses[i] = status.OverrideStatus;
				}
				sbyte b2 = ((bidiClass != BidiClass.RightToLeftIsolate) ? ((sbyte)((status.EmbeddingLevel + 2) & -2)) : ((sbyte)((status.EmbeddingLevel + 1) | 1)));
				if (b2 <= 125 && num == 0 && num2 == 0)
				{
					num3++;
					_statusStack.Push(new Status(b2, BidiClass.OtherNeutral, isolateStatus: true));
				}
				else
				{
					num++;
				}
				break;
			}
			default:
			{
				Status status3 = _statusStack.Peek();
				_resolvedLevels[i] = status3.EmbeddingLevel;
				if (status3.OverrideStatus != BidiClass.OtherNeutral)
				{
					_workingClasses[i] = status3.OverrideStatus;
				}
				break;
			}
			case BidiClass.PopDirectionalIsolate:
			{
				if (num > 0)
				{
					num--;
				}
				else if (num3 != 0)
				{
					num2 = 0;
					while (!_statusStack.Peek().IsolateStatus)
					{
						_statusStack.Pop();
					}
					_statusStack.Pop();
					num3--;
				}
				Status status2 = _statusStack.Peek();
				_resolvedLevels[i] = status2.EmbeddingLevel;
				if (status2.OverrideStatus != BidiClass.OtherNeutral)
				{
					_workingClasses[i] = status2.OverrideStatus;
				}
				break;
			}
			case BidiClass.PopDirectionalFormat:
				if (num == 0)
				{
					if (num2 > 0)
					{
						num2--;
					}
					else if (!_statusStack.Peek().IsolateStatus && _statusStack.Count >= 2)
					{
						_statusStack.Pop();
					}
				}
				break;
			case BidiClass.ParagraphSeparator:
				_resolvedLevels[i] = _paragraphEmbeddingLevel;
				break;
			case BidiClass.BoundaryNeutral:
				break;
			}
		}
	}

	private void BuildX9RemovalMap()
	{
		_x9Map.Length = _originalClasses.Length;
		if (_hasEmbeddings || _hasIsolates)
		{
			int length = 0;
			for (int i = 0; i < _originalClasses.Length; i++)
			{
				if (!IsRemovedByX9(_originalClasses[i]))
				{
					_x9Map[length++] = i;
				}
			}
			_x9Map.Length = length;
		}
		else
		{
			int j = 0;
			for (int length2 = _originalClasses.Length; j < length2; j++)
			{
				_x9Map[j] = j;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int MapX9(int index)
	{
		return _x9Map[index];
	}

	private void AddLevelRun(int start, int length, int level)
	{
		int num = MapX9(start);
		int num2 = MapX9(start + length - 1);
		int num3 = num - 1;
		while (num3 >= 0 && IsRemovedByX9(_originalClasses[num3]))
		{
			num3--;
		}
		BidiClass sos = DirectionFromLevel(Math.Max((num3 < 0) ? _paragraphEmbeddingLevel : _resolvedLevels[num3], level));
		int val;
		if (IsIsolateStart(_workingClasses[num2]))
		{
			val = _paragraphEmbeddingLevel;
		}
		else
		{
			for (num3 = num2 + 1; num3 < _originalClasses.Length && IsRemovedByX9(_originalClasses[num3]); num3++)
			{
			}
			val = ((num3 >= _originalClasses.Length) ? _paragraphEmbeddingLevel : _resolvedLevels[num3]);
		}
		BidiClass eos = DirectionFromLevel(Math.Max(val, level));
		_levelRuns.Add(new LevelRun(start, length, level, sos, eos));
	}

	private void FindLevelRuns()
	{
		int num = -1;
		int num2 = 0;
		for (int i = 0; i < _x9Map.Length; i++)
		{
			int num3 = _resolvedLevels[MapX9(i)];
			if (num3 != num)
			{
				if (num != -1)
				{
					AddLevelRun(num2, i - num2, num);
				}
				num = num3;
				num2 = i;
			}
		}
		if (num != -1)
		{
			AddLevelRun(num2, _x9Map.Length - num2, num);
		}
	}

	private int FindRunForIndex(int index)
	{
		for (int i = 0; i < _levelRuns.Count; i++)
		{
			if (MapX9(_levelRuns[i].Start) == index)
			{
				return i;
			}
		}
		throw new InvalidOperationException("Internal error");
	}

	private void ProcessIsolatedRunSequences()
	{
		FindLevelRuns();
		while (_levelRuns.Count > 0)
		{
			_isolatedRunMapping.Clear();
			int index = 0;
			BidiClass sos = _levelRuns[0].Sos;
			int level = _levelRuns[0].Level;
			BidiClass eos;
			while (true)
			{
				LevelRun levelRun = _levelRuns[index];
				eos = levelRun.Eos;
				_levelRuns.RemoveAt(index);
				_isolatedRunMapping.Add(_x9Map.AsSlice(levelRun.Start, levelRun.Length));
				int num = _isolatedRunMapping[_isolatedRunMapping.Length - 1];
				if (!IsIsolateStart(_originalClasses[num]) || !_isolatePairs.TryGetValue(num, out var value))
				{
					break;
				}
				index = FindRunForIndex(value);
			}
			ProcessIsolatedRunSequence(sos, eos, level);
		}
	}

	private void ProcessIsolatedRunSequence(BidiClass sos, BidiClass eos, int runLevel)
	{
		ArraySlice<int> map = _isolatedRunMapping.AsSlice();
		_runResolvedClasses = new MappedArraySlice<BidiClass>(in _workingClasses, in map);
		_runOriginalClasses = new MappedArraySlice<BidiClass>(in _originalClasses, in map);
		_runLevels = new MappedArraySlice<sbyte>(in _resolvedLevels, in map);
		if (_hasBrackets)
		{
			_runBiDiPairedBracketTypes = new MappedArraySlice<BidiPairedBracketType>(in _pairedBracketTypes, in map);
			_runPairedBracketValues = new MappedArraySlice<int>(in _pairedBracketValues, in map);
		}
		_runLevel = runLevel;
		_runDirection = DirectionFromLevel(runLevel);
		_runLength = _runResolvedClasses.Length;
		BidiClass bidiClass = sos;
		uint num = 0u;
		for (int i = 0; i < _runLength; i++)
		{
			BidiClass bidiClass2 = _runResolvedClasses[i];
			if (bidiClass2 == BidiClass.NonspacingMark)
			{
				_runResolvedClasses[i] = bidiClass;
				continue;
			}
			uint num2 = (uint)(1 << (int)bidiClass2);
			if ((num2 & 0x90A00) != 0)
			{
				bidiClass = BidiClass.OtherNeutral;
				continue;
			}
			num |= num2 & 0x1E6;
			bidiClass = bidiClass2;
		}
		bool flag = (num & 0x40) != 0;
		bool flag2 = (num & 2) != 0;
		bool flag3 = (num & 0x80) != 0;
		bool flag4 = (num & 0x20) != 0;
		bool flag5 = (num & 4) != 0;
		bool flag6 = (num & 0x100) != 0;
		if (flag)
		{
			for (int i = 0; i < _runLength; i++)
			{
				if (_runResolvedClasses[i] != BidiClass.EuropeanNumber)
				{
					continue;
				}
				for (int num3 = i - 1; num3 >= 0; num3--)
				{
					BidiClass bidiClass3 = _runResolvedClasses[num3];
					if ((uint)bidiClass3 <= 1u || bidiClass3 == BidiClass.RightToLeft)
					{
						if (bidiClass3 == BidiClass.ArabicLetter)
						{
							_runResolvedClasses[i] = BidiClass.ArabicNumber;
							flag5 = true;
						}
						num3 = -1;
					}
				}
			}
		}
		if (flag2)
		{
			for (int i = 0; i < _runLength; i++)
			{
				if (_runResolvedClasses[i] == BidiClass.ArabicLetter)
				{
					_runResolvedClasses[i] = BidiClass.RightToLeft;
				}
			}
		}
		if ((flag3 || flag4) && (flag || flag5))
		{
			for (int i = 1; i < _runLength - 1; i++)
			{
				ref BidiClass reference = ref _runResolvedClasses[i];
				if (reference == BidiClass.EuropeanSeparator)
				{
					BidiClass num4 = _runResolvedClasses[i - 1];
					BidiClass bidiClass4 = _runResolvedClasses[i + 1];
					if (num4 == BidiClass.EuropeanNumber && bidiClass4 == BidiClass.EuropeanNumber)
					{
						reference = BidiClass.EuropeanNumber;
					}
				}
				else if (reference == BidiClass.CommonSeparator)
				{
					BidiClass bidiClass5 = _runResolvedClasses[i - 1];
					BidiClass bidiClass6 = _runResolvedClasses[i + 1];
					if ((bidiClass5 == BidiClass.ArabicNumber && bidiClass6 == BidiClass.ArabicNumber) || (bidiClass5 == BidiClass.EuropeanNumber && bidiClass6 == BidiClass.EuropeanNumber))
					{
						reference = bidiClass5;
					}
				}
			}
		}
		if (flag6 && flag)
		{
			for (int i = 0; i < _runLength; i++)
			{
				if (_runResolvedClasses[i] != BidiClass.EuropeanTerminator)
				{
					continue;
				}
				int num5 = i;
				int j;
				for (j = i; j < _runLength && _runResolvedClasses[j] == BidiClass.EuropeanTerminator; j++)
				{
				}
				if (((num5 == 0) ? sos : _runResolvedClasses[num5 - 1]) == BidiClass.EuropeanNumber || ((j == _runLength) ? eos : _runResolvedClasses[j]) == BidiClass.EuropeanNumber)
				{
					for (; i < j; i++)
					{
						_runResolvedClasses[i] = BidiClass.EuropeanNumber;
					}
				}
				i = j;
			}
		}
		if (flag3 || flag6 || flag4)
		{
			for (int i = 0; i < _runLength; i++)
			{
				ref BidiClass reference2 = ref _runResolvedClasses[i];
				BidiClass bidiClass7 = reference2;
				if (bidiClass7 == BidiClass.CommonSeparator || (uint)(bidiClass7 - 7) <= 1u)
				{
					reference2 = BidiClass.OtherNeutral;
				}
			}
		}
		if (flag)
		{
			BidiClass bidiClass8 = sos;
			for (int i = 0; i < _runLength; i++)
			{
				ref BidiClass reference3 = ref _runResolvedClasses[i];
				switch (reference3)
				{
				case BidiClass.EuropeanNumber:
					if (bidiClass8 == BidiClass.LeftToRight)
					{
						_runResolvedClasses[i] = BidiClass.LeftToRight;
					}
					break;
				case BidiClass.LeftToRight:
				case BidiClass.RightToLeft:
					bidiClass8 = reference3;
					break;
				}
			}
		}
		if (_hasBrackets)
		{
			List<BracketPair> list = LocatePairedBrackets();
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				BracketPair bracketPair = list[i];
				BidiClass bidiClass9 = InspectPairedBracket(in bracketPair);
				if (bidiClass9 == BidiClass.OtherNeutral)
				{
					continue;
				}
				if ((bidiClass9 == BidiClass.LeftToRight || bidiClass9 == BidiClass.RightToLeft) && bidiClass9 == _runDirection)
				{
					SetPairedBracketDirection(in bracketPair, bidiClass9);
					continue;
				}
				bidiClass9 = InspectBeforePairedBracket(in bracketPair, sos);
				if (bidiClass9 == _runDirection || bidiClass9 == BidiClass.OtherNeutral)
				{
					bidiClass9 = _runDirection;
				}
				SetPairedBracketDirection(in bracketPair, bidiClass9);
			}
		}
		for (int i = 0; i < _runLength; i++)
		{
			if (!IsNeutralClass(_runResolvedClasses[i]))
			{
				continue;
			}
			int num6 = i;
			int k;
			for (k = i; k < _runLength && IsNeutralClass(_runResolvedClasses[k]); k++)
			{
			}
			BidiClass bidiClass10;
			if (num6 == 0)
			{
				bidiClass10 = sos;
			}
			else
			{
				bidiClass10 = _runResolvedClasses[num6 - 1];
				if (bidiClass10 == BidiClass.ArabicNumber || bidiClass10 == BidiClass.EuropeanNumber)
				{
					bidiClass10 = BidiClass.RightToLeft;
				}
			}
			BidiClass bidiClass11;
			if (k == _runLength)
			{
				bidiClass11 = eos;
			}
			else
			{
				bidiClass11 = _runResolvedClasses[k];
				if (bidiClass11 == BidiClass.ArabicNumber || bidiClass11 == BidiClass.EuropeanNumber)
				{
					bidiClass11 = BidiClass.RightToLeft;
				}
			}
			BidiClass bidiClass12 = ((bidiClass10 != bidiClass11) ? _runDirection : bidiClass10);
			for (int l = num6; l < k; l++)
			{
				_runResolvedClasses[l] = bidiClass12;
			}
			i = k;
		}
		if ((_runLevel & 1) == 0)
		{
			for (int i = 0; i < _runLength; i++)
			{
				BidiClass bidiClass13 = _runResolvedClasses[i];
				ref sbyte reference4 = ref _runLevels[i];
				switch (bidiClass13)
				{
				case BidiClass.RightToLeft:
					reference4++;
					break;
				case BidiClass.ArabicNumber:
				case BidiClass.EuropeanNumber:
					reference4 += 2;
					break;
				}
			}
			return;
		}
		for (int i = 0; i < _runLength; i++)
		{
			BidiClass num7 = _runResolvedClasses[i];
			ref sbyte reference5 = ref _runLevels[i];
			if (num7 != BidiClass.RightToLeft)
			{
				reference5++;
			}
		}
	}

	private List<BracketPair> LocatePairedBrackets()
	{
		_pendingOpeningBrackets.Clear();
		_pairedBrackets.Clear();
		int i = 0;
		for (int runLength = _runLength; i < runLength; i++)
		{
			if (_runResolvedClasses[i] != BidiClass.OtherNeutral)
			{
				continue;
			}
			switch (_runBiDiPairedBracketTypes[i])
			{
			case BidiPairedBracketType.Open:
				break;
			case BidiPairedBracketType.Close:
			{
				for (int j = 0; j < _pendingOpeningBrackets.Count; j++)
				{
					if (_runPairedBracketValues[i] != _runPairedBracketValues[_pendingOpeningBrackets[j]])
					{
						continue;
					}
					int num = _pendingOpeningBrackets[j];
					if (_pairedBrackets.Count < 8)
					{
						int k;
						for (k = 0; k < _pairedBrackets.Count && _pairedBrackets[k].OpeningIndex < num; k++)
						{
						}
						_pairedBrackets.Insert(k, new BracketPair(num, i));
					}
					else
					{
						_pairedBrackets.Add(new BracketPair(num, i));
					}
					_pendingOpeningBrackets.RemoveRange(0, j + 1);
					break;
				}
				continue;
			}
			default:
				continue;
			}
			if (_pendingOpeningBrackets.Count == 63)
			{
				break;
			}
			_pendingOpeningBrackets.Insert(0, i);
		}
		if (_pairedBrackets.Count > 8)
		{
			_pairedBrackets.Sort();
		}
		return _pairedBrackets;
	}

	private BidiClass InspectPairedBracket(in BracketPair bracketPair)
	{
		BidiClass bidiClass = DirectionFromLevel(_runLevel);
		BidiClass result = BidiClass.OtherNeutral;
		for (int i = bracketPair.OpeningIndex + 1; i < bracketPair.ClosingIndex; i++)
		{
			BidiClass strongClassN = GetStrongClassN0(_runResolvedClasses[i]);
			if (strongClassN != BidiClass.OtherNeutral)
			{
				if (strongClassN == bidiClass)
				{
					return strongClassN;
				}
				result = strongClassN;
			}
		}
		return result;
	}

	private BidiClass InspectBeforePairedBracket(in BracketPair bracketPair, BidiClass sos)
	{
		for (int num = bracketPair.OpeningIndex - 1; num >= 0; num--)
		{
			BidiClass strongClassN = GetStrongClassN0(_runResolvedClasses[num]);
			if (strongClassN != BidiClass.OtherNeutral)
			{
				return strongClassN;
			}
		}
		return sos;
	}

	private void SetPairedBracketDirection(in BracketPair pairedBracket, BidiClass direction)
	{
		_runResolvedClasses[pairedBracket.OpeningIndex] = direction;
		_runResolvedClasses[pairedBracket.ClosingIndex] = direction;
		for (int i = pairedBracket.OpeningIndex + 1; i < pairedBracket.ClosingIndex; i++)
		{
			if (_runOriginalClasses[i] == BidiClass.NonspacingMark)
			{
				_runOriginalClasses[i] = direction;
			}
			else if (_runOriginalClasses[i] != BidiClass.BoundaryNeutral)
			{
				break;
			}
		}
		for (int j = pairedBracket.ClosingIndex + 1; j < _runLength; j++)
		{
			if (_runOriginalClasses[j] == BidiClass.NonspacingMark)
			{
				_runOriginalClasses[j] = direction;
			}
			else if (_runOriginalClasses[j] != BidiClass.BoundaryNeutral)
			{
				break;
			}
		}
	}

	private void ResetWhitespaceLevels()
	{
		for (int i = 0; i < _resolvedLevels.Length; i++)
		{
			BidiClass bidiClass = _originalClasses[i];
			if (bidiClass == BidiClass.ParagraphSeparator || bidiClass == BidiClass.SegmentSeparator)
			{
				_resolvedLevels[i] = _paragraphEmbeddingLevel;
				int num = i - 1;
				while (num >= 0 && IsWhitespace(_originalClasses[num]))
				{
					_resolvedLevels[num] = _paragraphEmbeddingLevel;
					num--;
				}
			}
		}
		int num2 = _resolvedLevels.Length - 1;
		while (num2 >= 0 && IsWhitespace(_originalClasses[num2]))
		{
			_resolvedLevels[num2] = _paragraphEmbeddingLevel;
			num2--;
		}
	}

	private void AssignLevelsToCodePointsRemovedByX9()
	{
		if ((!_hasIsolates && !_hasEmbeddings) || _workingClasses.Length == 0)
		{
			return;
		}
		if (_resolvedLevels[0] < 0)
		{
			_resolvedLevels[0] = _paragraphEmbeddingLevel;
		}
		if (IsRemovedByX9(_originalClasses[0]))
		{
			_workingClasses[0] = _originalClasses[0];
		}
		int i = 1;
		for (int length = _workingClasses.Length; i < length; i++)
		{
			BidiClass bidiClass = _originalClasses[i];
			if (IsRemovedByX9(bidiClass))
			{
				_workingClasses[i] = bidiClass;
				_resolvedLevels[i] = _resolvedLevels[i - 1];
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsWhitespace(BidiClass biDiClass)
	{
		return ((1 << (int)biDiClass) & 0x5D9E10) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static BidiClass DirectionFromLevel(int level)
	{
		if ((level & 1) != 0)
		{
			return BidiClass.RightToLeft;
		}
		return BidiClass.LeftToRight;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsRemovedByX9(BidiClass biDiClass)
	{
		return ((1 << (int)biDiClass) & 0x149410) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsNeutralClass(BidiClass direction)
	{
		return ((1 << (int)direction) & 0x694A08) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static BidiClass GetStrongClassN0(BidiClass direction)
	{
		switch (direction)
		{
		case BidiClass.ArabicLetter:
		case BidiClass.ArabicNumber:
		case BidiClass.EuropeanNumber:
		case BidiClass.RightToLeft:
			return BidiClass.RightToLeft;
		case BidiClass.LeftToRight:
			return BidiClass.LeftToRight;
		default:
			return BidiClass.OtherNeutral;
		}
	}

	public void Reset()
	{
		if (!_hasCleanState)
		{
			_originalClasses = default(ArraySlice<BidiClass>);
			_pairedBracketTypes = default(ArraySlice<BidiPairedBracketType>);
			_pairedBracketValues = default(ArraySlice<int>);
			_hasBrackets = false;
			_hasEmbeddings = false;
			_hasIsolates = false;
			_isolatePairs.ClearThenResetIfTooLarge();
			_workingClasses = default(ArraySlice<BidiClass>);
			FormattingBufferHelper.ClearThenResetIfTooLarge(ref _workingClassesBuffer);
			_resolvedLevels = default(ArraySlice<sbyte>);
			FormattingBufferHelper.ClearThenResetIfTooLarge(ref _resolvedLevelsBuffer);
			_paragraphEmbeddingLevel = 0;
			FormattingBufferHelper.ClearThenResetIfTooLarge(_statusStack);
			FormattingBufferHelper.ClearThenResetIfTooLarge(ref _x9Map);
			FormattingBufferHelper.ClearThenResetIfTooLarge(_levelRuns);
			FormattingBufferHelper.ClearThenResetIfTooLarge(ref _isolatedRunMapping);
			FormattingBufferHelper.ClearThenResetIfTooLarge(_pendingIsolateOpenings);
			_runLevel = 0;
			_runDirection = BidiClass.LeftToRight;
			_runLength = 0;
			_runResolvedClasses = default(MappedArraySlice<BidiClass>);
			_runOriginalClasses = default(MappedArraySlice<BidiClass>);
			_runLevels = default(MappedArraySlice<sbyte>);
			_runBiDiPairedBracketTypes = default(MappedArraySlice<BidiPairedBracketType>);
			_runPairedBracketValues = default(MappedArraySlice<int>);
			FormattingBufferHelper.ClearThenResetIfTooLarge(_pendingOpeningBrackets);
			FormattingBufferHelper.ClearThenResetIfTooLarge(_pairedBrackets);
			_hasCleanState = true;
		}
	}
}
