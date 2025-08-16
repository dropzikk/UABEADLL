using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccFormulaCurveElement : IccCurveSegment, IEquatable<IccFormulaCurveElement>
{
	public IccFormulaCurveType Type { get; }

	public float Gamma { get; }

	public float A { get; }

	public float B { get; }

	public float C { get; }

	public float D { get; }

	public float E { get; }

	public IccFormulaCurveElement(IccFormulaCurveType type, float gamma, float a, float b, float c, float d, float e)
		: base(IccCurveSegmentSignature.FormulaCurve)
	{
		Type = type;
		Gamma = gamma;
		A = a;
		B = b;
		C = c;
		D = d;
		E = e;
	}

	public override bool Equals(IccCurveSegment? other)
	{
		if (base.Equals(other) && other is IccFormulaCurveElement iccFormulaCurveElement)
		{
			if (Type == iccFormulaCurveElement.Type && Gamma == iccFormulaCurveElement.Gamma && A == iccFormulaCurveElement.A && B == iccFormulaCurveElement.B && C == iccFormulaCurveElement.C && D == iccFormulaCurveElement.D)
			{
				return E == iccFormulaCurveElement.E;
			}
			return false;
		}
		return false;
	}

	public bool Equals(IccFormulaCurveElement? other)
	{
		return Equals((IccCurveSegment?)other);
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as IccFormulaCurveElement);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Type, Gamma, A, B, C, D, E);
	}
}
