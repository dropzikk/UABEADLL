using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Cicp;

public sealed class CicpProfile : IDeepCloneable<CicpProfile>
{
	public CicpColorPrimaries ColorPrimaries { get; set; }

	public CicpTransferCharacteristics TransferCharacteristics { get; set; }

	public CicpMatrixCoefficients MatrixCoefficients { get; set; }

	public bool FullRange { get; set; }

	public CicpProfile()
		: this(2, 2, 2, null)
	{
	}

	public CicpProfile(byte colorPrimaries, byte transferCharacteristics, byte matrixCoefficients, bool? fullRange)
	{
		ColorPrimaries = (CicpColorPrimaries)(Enum.IsDefined(typeof(CicpColorPrimaries), colorPrimaries) ? colorPrimaries : 2);
		TransferCharacteristics = (CicpTransferCharacteristics)(Enum.IsDefined(typeof(CicpTransferCharacteristics), transferCharacteristics) ? transferCharacteristics : 2);
		MatrixCoefficients = (CicpMatrixCoefficients)(Enum.IsDefined(typeof(CicpMatrixCoefficients), matrixCoefficients) ? matrixCoefficients : 2);
		FullRange = fullRange ?? (MatrixCoefficients == CicpMatrixCoefficients.Identity);
	}

	private CicpProfile(CicpProfile other)
	{
		Guard.NotNull(other, "other");
		ColorPrimaries = other.ColorPrimaries;
		TransferCharacteristics = other.TransferCharacteristics;
		MatrixCoefficients = other.MatrixCoefficients;
		FullRange = other.FullRange;
	}

	public CicpProfile DeepClone()
	{
		return new CicpProfile(this);
	}
}
