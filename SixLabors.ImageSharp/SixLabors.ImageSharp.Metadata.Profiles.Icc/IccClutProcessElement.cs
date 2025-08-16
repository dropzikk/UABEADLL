using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccClutProcessElement : IccMultiProcessElement, IEquatable<IccClutProcessElement>
{
	public IccClut ClutValue { get; }

	public IccClutProcessElement(IccClut clutValue)
		: base(IccMultiProcessElementSignature.Clut, clutValue?.InputChannelCount ?? 1, clutValue?.OutputChannelCount ?? 1)
	{
		ClutValue = clutValue ?? throw new ArgumentNullException("clutValue");
	}

	public override bool Equals(IccMultiProcessElement? other)
	{
		if (base.Equals(other) && other is IccClutProcessElement iccClutProcessElement)
		{
			return ClutValue.Equals(iccClutProcessElement.ClutValue);
		}
		return false;
	}

	public bool Equals(IccClutProcessElement? other)
	{
		return Equals((IccMultiProcessElement?)other);
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as IccClutProcessElement);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
