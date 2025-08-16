namespace SixLabors.ImageSharp.Formats.Tiff.Constants;

public enum TiffPhotometricInterpretation : ushort
{
	WhiteIsZero = 0,
	BlackIsZero = 1,
	Rgb = 2,
	PaletteColor = 3,
	TransparencyMask = 4,
	Separated = 5,
	YCbCr = 6,
	CieLab = 8,
	IccLab = 9,
	ItuLab = 10,
	ColorFilterArray = 32803,
	LinearRaw = 34892
}
