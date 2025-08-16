using System.Numerics;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

public static class LmsAdaptationMatrix
{
	public static readonly Matrix4x4 VonKriesHPEAdjusted = Matrix4x4.Transpose(new Matrix4x4
	{
		M11 = 0.40024f,
		M12 = 0.7076f,
		M13 = -0.08081f,
		M21 = -0.2263f,
		M22 = 1.16532f,
		M23 = 0.0457f,
		M31 = 0f,
		M32 = 0f,
		M33 = 0.91822f,
		M44 = 1f
	});

	public static readonly Matrix4x4 VonKriesHPE = Matrix4x4.Transpose(new Matrix4x4
	{
		M11 = 0.3897f,
		M12 = 0.689f,
		M13 = -0.0787f,
		M21 = -0.2298f,
		M22 = 1.1834f,
		M23 = 0.0464f,
		M31 = 0f,
		M32 = 0f,
		M33 = 1f,
		M44 = 1f
	});

	public static readonly Matrix4x4 XyzScaling = Matrix4x4.Transpose(Matrix4x4.Identity);

	public static readonly Matrix4x4 Bradford = Matrix4x4.Transpose(new Matrix4x4
	{
		M11 = 0.8951f,
		M12 = 0.2664f,
		M13 = -0.1614f,
		M21 = -0.7502f,
		M22 = 1.7135f,
		M23 = 0.0367f,
		M31 = 0.0389f,
		M32 = -0.0685f,
		M33 = 1.0296f,
		M44 = 1f
	});

	public static readonly Matrix4x4 BradfordSharp = Matrix4x4.Transpose(new Matrix4x4
	{
		M11 = 1.2694f,
		M12 = -0.0988f,
		M13 = -0.1706f,
		M21 = -0.8364f,
		M22 = 1.8006f,
		M23 = 0.0357f,
		M31 = 0.0297f,
		M32 = -0.0315f,
		M33 = 1.0018f,
		M44 = 1f
	});

	public static readonly Matrix4x4 CMCCAT2000 = Matrix4x4.Transpose(new Matrix4x4
	{
		M11 = 0.7982f,
		M12 = 0.3389f,
		M13 = -0.1371f,
		M21 = -0.5918f,
		M22 = 1.5512f,
		M23 = 0.0406f,
		M31 = 0.0008f,
		M32 = 0.239f,
		M33 = 0.9753f,
		M44 = 1f
	});

	public static readonly Matrix4x4 CAT02 = Matrix4x4.Transpose(new Matrix4x4
	{
		M11 = 0.7328f,
		M12 = 0.4296f,
		M13 = -0.1624f,
		M21 = -0.7036f,
		M22 = 1.6975f,
		M23 = 0.0061f,
		M31 = 0.003f,
		M32 = 0.0136f,
		M33 = 0.9834f,
		M44 = 1f
	});
}
