namespace SixLabors.ImageSharp.Metadata.Profiles.Cicp;

public enum CicpMatrixCoefficients : byte
{
	Identity = 0,
	ItuRBt709_6 = 1,
	Unspecified = 2,
	Fcc47 = 4,
	ItuRBt601_7_625 = 5,
	ItuRBt601_7_525 = 6,
	SmpteSt240 = 7,
	YCgCo = 8,
	ItuRBt2020_2_Ncl = 9,
	ItuRBt2020_2_Cl = 10,
	SmpteSt2085 = 11,
	ChromaDerivedNcl = 12,
	ChromaDerivedCl = 13,
	ICtCp = 14
}
