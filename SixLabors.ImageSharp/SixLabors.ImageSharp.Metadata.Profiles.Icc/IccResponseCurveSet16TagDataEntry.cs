using System;
using System.Linq;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccResponseCurveSet16TagDataEntry : IccTagDataEntry, IEquatable<IccResponseCurveSet16TagDataEntry>
{
	public ushort ChannelCount { get; }

	public IccResponseCurve[] Curves { get; }

	public IccResponseCurveSet16TagDataEntry(IccResponseCurve[] curves)
		: this(curves, IccProfileTag.Unknown)
	{
	}

	public IccResponseCurveSet16TagDataEntry(IccResponseCurve[] curves, IccProfileTag tagSignature)
		: base(IccTypeSignature.ResponseCurveSet16, tagSignature)
	{
		Guard.NotNull(curves, "curves");
		Guard.IsTrue(curves.Length != 0, "curves", "curves needs at least one element");
		Curves = curves;
		ChannelCount = (ushort)curves[0].ResponseArrays.Length;
		Guard.IsFalse(curves.Any((IccResponseCurve t) => t.ResponseArrays.Length != ChannelCount), "curves", "All curves need to have the same number of channels");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccResponseCurveSet16TagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccResponseCurveSet16TagDataEntry? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (base.Equals(other) && ChannelCount == other.ChannelCount)
		{
			return Curves.AsSpan().SequenceEqual(other.Curves);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccResponseCurveSet16TagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, ChannelCount, Curves);
	}
}
