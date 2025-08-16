using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal abstract class IccMultiProcessElement : IEquatable<IccMultiProcessElement>
{
	public IccMultiProcessElementSignature Signature { get; }

	public int InputChannelCount { get; }

	public int OutputChannelCount { get; }

	protected IccMultiProcessElement(IccMultiProcessElementSignature signature, int inChannelCount, int outChannelCount)
	{
		Guard.MustBeBetweenOrEqualTo(inChannelCount, 1, 15, "inChannelCount");
		Guard.MustBeBetweenOrEqualTo(outChannelCount, 1, 15, "outChannelCount");
		Signature = signature;
		InputChannelCount = inChannelCount;
		OutputChannelCount = outChannelCount;
	}

	public virtual bool Equals(IccMultiProcessElement? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (Signature == other.Signature && InputChannelCount == other.InputChannelCount)
		{
			return OutputChannelCount == other.OutputChannelCount;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as IccMultiProcessElement);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Signature, InputChannelCount, OutputChannelCount);
	}
}
