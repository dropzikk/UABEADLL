using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccCurveSetProcessElement : IccMultiProcessElement, IEquatable<IccCurveSetProcessElement>
{
	public IccOneDimensionalCurve[] Curves { get; }

	public IccCurveSetProcessElement(IccOneDimensionalCurve[] curves)
		: base(IccMultiProcessElementSignature.CurveSet, (curves == null) ? 1 : curves.Length, (curves == null) ? 1 : curves.Length)
	{
		Curves = curves ?? throw new ArgumentNullException("curves");
	}

	public override bool Equals(IccMultiProcessElement? other)
	{
		if (base.Equals(other) && other is IccCurveSetProcessElement iccCurveSetProcessElement)
		{
			return Curves.AsSpan().SequenceEqual(iccCurveSetProcessElement.Curves);
		}
		return false;
	}

	public bool Equals(IccCurveSetProcessElement? other)
	{
		return Equals((IccMultiProcessElement?)other);
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as IccCurveSetProcessElement);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
