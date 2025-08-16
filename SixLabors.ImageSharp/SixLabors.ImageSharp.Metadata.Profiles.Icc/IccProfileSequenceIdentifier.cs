using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal readonly struct IccProfileSequenceIdentifier : IEquatable<IccProfileSequenceIdentifier>
{
	public IccProfileId Id { get; }

	public IccLocalizedString[] Description { get; }

	public IccProfileSequenceIdentifier(IccProfileId id, IccLocalizedString[] description)
	{
		Id = id;
		Description = description ?? throw new ArgumentNullException("description");
	}

	public bool Equals(IccProfileSequenceIdentifier other)
	{
		if (Id.Equals(other.Id))
		{
			return Description.AsSpan().SequenceEqual(other.Description);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccProfileSequenceIdentifier other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Id, Description);
	}
}
