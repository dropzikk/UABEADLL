using System;
using System.Text;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccDataTagDataEntry : IccTagDataEntry, IEquatable<IccDataTagDataEntry>
{
	public byte[] Data { get; }

	public bool IsAscii { get; }

	public string? AsciiString
	{
		get
		{
			if (!IsAscii)
			{
				return null;
			}
			return Encoding.ASCII.GetString(Data, 0, Data.Length);
		}
	}

	public IccDataTagDataEntry(byte[] data)
		: this(data, isAscii: false, IccProfileTag.Unknown)
	{
	}

	public IccDataTagDataEntry(byte[] data, bool isAscii)
		: this(data, isAscii, IccProfileTag.Unknown)
	{
	}

	public IccDataTagDataEntry(byte[] data, bool isAscii, IccProfileTag tagSignature)
		: base(IccTypeSignature.Data, tagSignature)
	{
		Data = data ?? throw new ArgumentNullException("data");
		IsAscii = isAscii;
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccDataTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccDataTagDataEntry? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (base.Equals(other) && Data.AsSpan().SequenceEqual(other.Data))
		{
			return IsAscii == other.IsAscii;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccDataTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, Data, IsAscii);
	}
}
