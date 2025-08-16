using System.Numerics;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

internal abstract class LinearRgbAndCieXyzConverterBase
{
	public static Matrix4x4 GetRgbToCieXyzMatrix(RgbWorkingSpace workingSpace)
	{
		RgbPrimariesChromaticityCoordinates chromaticityCoordinates = workingSpace.ChromaticityCoordinates;
		float x = chromaticityCoordinates.R.X;
		float x2 = chromaticityCoordinates.G.X;
		float x3 = chromaticityCoordinates.B.X;
		float y = chromaticityCoordinates.R.Y;
		float y2 = chromaticityCoordinates.G.Y;
		float y3 = chromaticityCoordinates.B.Y;
		float num = x / y;
		float num2 = (1f - x - y) / y;
		float num3 = x2 / y2;
		float num4 = (1f - x2 - y2) / y2;
		float num5 = x3 / y3;
		float num6 = (1f - x3 - y3) / y3;
		Matrix4x4 matrix = default(Matrix4x4);
		matrix.M11 = num;
		matrix.M21 = num3;
		matrix.M31 = num5;
		matrix.M12 = 1f;
		matrix.M22 = 1f;
		matrix.M32 = 1f;
		matrix.M13 = num2;
		matrix.M23 = num4;
		matrix.M33 = num6;
		matrix.M44 = 1f;
		Matrix4x4.Invert(matrix, out var result);
		Vector3 vector = Vector3.Transform(workingSpace.WhitePoint.ToVector3(), result);
		matrix = default(Matrix4x4);
		matrix.M11 = vector.X * num;
		matrix.M21 = vector.Y * num3;
		matrix.M31 = vector.Z * num5;
		matrix.M12 = vector.X * 1f;
		matrix.M22 = vector.Y * 1f;
		matrix.M32 = vector.Z * 1f;
		matrix.M13 = vector.X * num2;
		matrix.M23 = vector.Y * num4;
		matrix.M33 = vector.Z * num6;
		matrix.M44 = 1f;
		return matrix;
	}
}
