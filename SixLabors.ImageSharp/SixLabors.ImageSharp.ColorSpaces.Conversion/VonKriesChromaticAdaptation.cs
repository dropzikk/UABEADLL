using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

public sealed class VonKriesChromaticAdaptation : IChromaticAdaptation
{
	private readonly CieXyzAndLmsConverter converter;

	public VonKriesChromaticAdaptation()
		: this(new CieXyzAndLmsConverter())
	{
	}

	public VonKriesChromaticAdaptation(Matrix4x4 transformationMatrix)
		: this(new CieXyzAndLmsConverter(transformationMatrix))
	{
	}

	internal VonKriesChromaticAdaptation(CieXyzAndLmsConverter converter)
	{
		this.converter = converter;
	}

	public CieXyz Transform(in CieXyz source, in CieXyz sourceWhitePoint, in CieXyz destinationWhitePoint)
	{
		if (sourceWhitePoint.Equals(destinationWhitePoint))
		{
			return source;
		}
		Lms lms = converter.Convert(in source);
		Lms lms2 = converter.Convert(in sourceWhitePoint);
		Vector3 left = converter.Convert(in destinationWhitePoint).ToVector3() / lms2.ToVector3();
		Lms input = new Lms(Vector3.Multiply(left, lms.ToVector3()));
		return converter.Convert(in input);
	}

	public void Transform(ReadOnlySpan<CieXyz> source, Span<CieXyz> destination, CieXyz sourceWhitePoint, in CieXyz destinationWhitePoint)
	{
		Guard.DestinationShouldNotBeTooShort(source, destination, "destination");
		int length = source.Length;
		if (sourceWhitePoint.Equals(destinationWhitePoint))
		{
			source.CopyTo(destination.Slice(0, length));
			return;
		}
		ref CieXyz reference = ref MemoryMarshal.GetReference(source);
		ref CieXyz reference2 = ref MemoryMarshal.GetReference(destination);
		for (nuint num = 0u; num < (uint)length; num++)
		{
			ref CieXyz input = ref Unsafe.Add(ref reference, num);
			ref CieXyz reference3 = ref Unsafe.Add(ref reference2, num);
			Lms lms = converter.Convert(in input);
			Lms lms2 = converter.Convert(in sourceWhitePoint);
			Vector3 left = converter.Convert(in destinationWhitePoint).ToVector3() / lms2.ToVector3();
			Lms input2 = new Lms(Vector3.Multiply(left, lms.ToVector3()));
			reference3 = converter.Convert(in input2);
		}
	}

	CieXyz IChromaticAdaptation.Transform(in CieXyz source, in CieXyz sourceWhitePoint, in CieXyz destinationWhitePoint)
	{
		return Transform(in source, in sourceWhitePoint, in destinationWhitePoint);
	}

	void IChromaticAdaptation.Transform(ReadOnlySpan<CieXyz> source, Span<CieXyz> destination, CieXyz sourceWhitePoint, in CieXyz destinationWhitePoint)
	{
		Transform(source, destination, sourceWhitePoint, in destinationWhitePoint);
	}
}
