using System;
using System.Linq;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccMultiProcessElementsTagDataEntry : IccTagDataEntry, IEquatable<IccMultiProcessElementsTagDataEntry>
{
	public int InputChannelCount { get; }

	public int OutputChannelCount { get; }

	public IccMultiProcessElement[] Data { get; }

	public IccMultiProcessElementsTagDataEntry(IccMultiProcessElement[] data)
		: this(data, IccProfileTag.Unknown)
	{
	}

	public IccMultiProcessElementsTagDataEntry(IccMultiProcessElement[] data, IccProfileTag tagSignature)
		: base(IccTypeSignature.MultiProcessElements, tagSignature)
	{
		Guard.NotNull(data, "data");
		Guard.IsTrue(data.Length != 0, "data", "data must have at least one element");
		InputChannelCount = data[0].InputChannelCount;
		OutputChannelCount = data[0].OutputChannelCount;
		Data = data;
		Guard.IsFalse(data.Any((IccMultiProcessElement t) => t.InputChannelCount != InputChannelCount || t.OutputChannelCount != OutputChannelCount), "data", "The number of input and output channels are not the same for all elements");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccMultiProcessElementsTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccMultiProcessElementsTagDataEntry? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (base.Equals(other) && InputChannelCount == other.InputChannelCount && OutputChannelCount == other.OutputChannelCount)
		{
			return Data.AsSpan().SequenceEqual(other.Data);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccMultiProcessElementsTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, InputChannelCount, OutputChannelCount, Data);
	}
}
