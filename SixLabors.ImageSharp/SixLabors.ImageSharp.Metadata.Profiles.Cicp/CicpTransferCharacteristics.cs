namespace SixLabors.ImageSharp.Metadata.Profiles.Cicp;

public enum CicpTransferCharacteristics : byte
{
	ItuRBt709_6 = 1,
	Unspecified = 2,
	Gamma2_2 = 4,
	Gamma2_8 = 5,
	ItuRBt601_7 = 6,
	SmpteSt240 = 7,
	Linear = 8,
	Log100 = 9,
	Log100Sqrt = 10,
	Iec61966_2_4 = 11,
	ItuRBt1361_0 = 12,
	Iec61966_2_1 = 13,
	ItuRBt2020_2_10bit = 14,
	ItuRBt2020_2_12bit = 15,
	SmpteSt2084 = 16,
	SmpteSt428_1 = 17,
	AribStdB67 = 18
}
