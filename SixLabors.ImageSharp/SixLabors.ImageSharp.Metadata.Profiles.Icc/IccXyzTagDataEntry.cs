using System;
using System.Numerics;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccXyzTagDataEntry : IccTagDataEntry, IEquatable<IccXyzTagDataEntry>
{
	public Vector3[] Data { get; }

	public IccXyzTagDataEntry(Vector3[] data)
		: this(data, IccProfileTag.Unknown)
	{
	}

	public IccXyzTagDataEntry(Vector3[] data, IccProfileTag tagSignature)
		: base(IccTypeSignature.Xyz, tagSignature)
	{
		Data = data ?? throw new ArgumentNullException("data");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (base.Equals(other) && other is IccXyzTagDataEntry iccXyzTagDataEntry)
		{
			return Data.AsSpan().SequenceEqual(iccXyzTagDataEntry.Data);
		}
		return false;
	}

	public bool Equals(IccXyzTagDataEntry? other)
	{
		return Equals((IccTagDataEntry?)other);
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as IccXyzTagDataEntry);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.GetHashCode(), Data);
	}
}
