using System.Numerics;

namespace SixLabors.ImageSharp;

public static class Matrix3x2Extensions
{
	public static Matrix3x2 CreateTranslation(PointF position)
	{
		return Matrix3x2.CreateTranslation(position);
	}

	public static Matrix3x2 CreateScale(float xScale, float yScale, PointF centerPoint)
	{
		return Matrix3x2.CreateScale(xScale, yScale, centerPoint);
	}

	public static Matrix3x2 CreateScale(SizeF scales)
	{
		return Matrix3x2.CreateScale(scales);
	}

	public static Matrix3x2 CreateScale(SizeF scales, PointF centerPoint)
	{
		return Matrix3x2.CreateScale(scales, centerPoint);
	}

	public static Matrix3x2 CreateScale(float scale, PointF centerPoint)
	{
		return Matrix3x2.CreateScale(scale, centerPoint);
	}

	public static Matrix3x2 CreateSkewDegrees(float degreesX, float degreesY)
	{
		return Matrix3x2.CreateSkew(GeometryUtilities.DegreeToRadian(degreesX), GeometryUtilities.DegreeToRadian(degreesY));
	}

	public static Matrix3x2 CreateSkew(float radiansX, float radiansY, PointF centerPoint)
	{
		return Matrix3x2.CreateSkew(radiansX, radiansY, centerPoint);
	}

	public static Matrix3x2 CreateSkewDegrees(float degreesX, float degreesY, PointF centerPoint)
	{
		return Matrix3x2.CreateSkew(GeometryUtilities.DegreeToRadian(degreesX), GeometryUtilities.DegreeToRadian(degreesY), centerPoint);
	}

	public static Matrix3x2 CreateRotationDegrees(float degrees)
	{
		return Matrix3x2.CreateRotation(GeometryUtilities.DegreeToRadian(degrees));
	}

	public static Matrix3x2 CreateRotation(float radians, PointF centerPoint)
	{
		return Matrix3x2.CreateRotation(radians, centerPoint);
	}

	public static Matrix3x2 CreateRotationDegrees(float degrees, PointF centerPoint)
	{
		return Matrix3x2.CreateRotation(GeometryUtilities.DegreeToRadian(degrees), centerPoint);
	}
}
