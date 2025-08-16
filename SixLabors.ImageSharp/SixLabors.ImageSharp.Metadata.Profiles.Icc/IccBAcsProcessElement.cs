using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccBAcsProcessElement : IccMultiProcessElement, IEquatable<IccBAcsProcessElement>
{
	public IccBAcsProcessElement(int inChannelCount, int outChannelCount)
		: base(IccMultiProcessElementSignature.BAcs, inChannelCount, outChannelCount)
	{
	}

	public bool Equals(IccBAcsProcessElement? other)
	{
		return base.Equals(other);
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as IccBAcsProcessElement);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
