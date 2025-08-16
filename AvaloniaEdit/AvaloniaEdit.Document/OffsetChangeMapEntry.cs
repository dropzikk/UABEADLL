using System;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Document;

public struct OffsetChangeMapEntry : IEquatable<OffsetChangeMapEntry>
{
	private readonly uint _insertionLengthWithMovementFlag;

	private readonly uint _removalLengthWithDeletionFlag;

	public int Offset { get; }

	public int InsertionLength => (int)(_insertionLengthWithMovementFlag & 0x7FFFFFFF);

	public int RemovalLength => (int)(_removalLengthWithDeletionFlag & 0x7FFFFFFF);

	public bool RemovalNeverCausesAnchorDeletion => (_removalLengthWithDeletionFlag & 0x80000000u) != 0;

	public bool DefaultAnchorMovementIsBeforeInsertion => (_insertionLengthWithMovementFlag & 0x80000000u) != 0;

	public int GetNewOffset(int oldOffset, AnchorMovementType movementType = AnchorMovementType.Default)
	{
		int insertionLength = InsertionLength;
		int removalLength = RemovalLength;
		if (removalLength != 0 || oldOffset != Offset)
		{
			if (oldOffset <= Offset)
			{
				return oldOffset;
			}
			if (oldOffset >= Offset + removalLength)
			{
				return oldOffset + insertionLength - removalLength;
			}
		}
		switch (movementType)
		{
		case AnchorMovementType.AfterInsertion:
			return Offset + insertionLength;
		case AnchorMovementType.BeforeInsertion:
			return Offset;
		default:
			if (!DefaultAnchorMovementIsBeforeInsertion)
			{
				return Offset + insertionLength;
			}
			return Offset;
		}
	}

	public OffsetChangeMapEntry(int offset, int removalLength, int insertionLength)
	{
		ThrowUtil.CheckNotNegative(offset, "offset");
		ThrowUtil.CheckNotNegative(removalLength, "removalLength");
		ThrowUtil.CheckNotNegative(insertionLength, "insertionLength");
		Offset = offset;
		_removalLengthWithDeletionFlag = (uint)removalLength;
		_insertionLengthWithMovementFlag = (uint)insertionLength;
	}

	public OffsetChangeMapEntry(int offset, int removalLength, int insertionLength, bool removalNeverCausesAnchorDeletion, bool defaultAnchorMovementIsBeforeInsertion)
		: this(offset, removalLength, insertionLength)
	{
		if (removalNeverCausesAnchorDeletion)
		{
			_removalLengthWithDeletionFlag |= 2147483648u;
		}
		if (defaultAnchorMovementIsBeforeInsertion)
		{
			_insertionLengthWithMovementFlag |= 2147483648u;
		}
	}

	public override int GetHashCode()
	{
		return Offset + (int)(3559 * _insertionLengthWithMovementFlag) + (int)(3571 * _removalLengthWithDeletionFlag);
	}

	public override bool Equals(object obj)
	{
		if (obj is OffsetChangeMapEntry)
		{
			return Equals((OffsetChangeMapEntry)obj);
		}
		return false;
	}

	public bool Equals(OffsetChangeMapEntry other)
	{
		if (Offset == other.Offset && _insertionLengthWithMovementFlag == other._insertionLengthWithMovementFlag)
		{
			return _removalLengthWithDeletionFlag == other._removalLengthWithDeletionFlag;
		}
		return false;
	}

	public static bool operator ==(OffsetChangeMapEntry left, OffsetChangeMapEntry right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(OffsetChangeMapEntry left, OffsetChangeMapEntry right)
	{
		return !left.Equals(right);
	}
}
