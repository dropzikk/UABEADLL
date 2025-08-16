using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccEAcsProcessElement : IccMultiProcessElement, IEquatable<IccEAcsProcessElement>
{
	public IccEAcsProcessElement(int inChannelCount, int outChannelCount)
		: base(IccMultiProcessElementSignature.EAcs, inChannelCount, outChannelCount)
	{
	}

	public bool Equals(IccEAcsProcessElement? other)
	{
		return base.Equals(other);
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as IccEAcsProcessElement);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
