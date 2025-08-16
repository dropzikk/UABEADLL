using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

internal static class TransformUtils
{
	public static bool IsDegenerate(Matrix3x2 matrix)
	{
		if (!IsNaN(matrix))
		{
			return IsZero(matrix.GetDeterminant());
		}
		return true;
	}

	public static bool IsDegenerate(Matrix4x4 matrix)
	{
		if (!IsNaN(matrix))
		{
			return IsZero(matrix.GetDeterminant());
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsZero(float a)
	{
		if (a > 0f - Constants.EpsilonSquared)
		{
			return a < Constants.EpsilonSquared;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNaN(Matrix3x2 matrix)
	{
		if (!float.IsNaN(matrix.M11) && !float.IsNaN(matrix.M12) && !float.IsNaN(matrix.M21) && !float.IsNaN(matrix.M22) && !float.IsNaN(matrix.M31))
		{
			return float.IsNaN(matrix.M32);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNaN(Matrix4x4 matrix)
	{
		if (!float.IsNaN(matrix.M11) && !float.IsNaN(matrix.M12) && !float.IsNaN(matrix.M13) && !float.IsNaN(matrix.M14) && !float.IsNaN(matrix.M21) && !float.IsNaN(matrix.M22) && !float.IsNaN(matrix.M23) && !float.IsNaN(matrix.M24) && !float.IsNaN(matrix.M31) && !float.IsNaN(matrix.M32) && !float.IsNaN(matrix.M33) && !float.IsNaN(matrix.M34) && !float.IsNaN(matrix.M41) && !float.IsNaN(matrix.M42) && !float.IsNaN(matrix.M43))
		{
			return float.IsNaN(matrix.M44);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 ProjectiveTransform2D(float x, float y, Matrix4x4 matrix)
	{
		Vector4 vector = Vector4.Transform(new Vector4(x, y, 0f, 1f), matrix);
		return new Vector2(vector.X, vector.Y) / MathF.Max(vector.W, 1E-07f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 CreateRotationMatrixDegrees(float degrees, Size size)
	{
		return CreateCenteredTransformMatrix(new Rectangle(Point.Empty, size), Matrix3x2Extensions.CreateRotationDegrees(degrees, PointF.Empty));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 CreateRotationMatrixRadians(float radians, Size size)
	{
		return CreateCenteredTransformMatrix(new Rectangle(Point.Empty, size), Matrix3x2Extensions.CreateRotation(radians, PointF.Empty));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 CreateSkewMatrixDegrees(float degreesX, float degreesY, Size size)
	{
		return CreateCenteredTransformMatrix(new Rectangle(Point.Empty, size), Matrix3x2Extensions.CreateSkewDegrees(degreesX, degreesY, PointF.Empty));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 CreateSkewMatrixRadians(float radiansX, float radiansY, Size size)
	{
		return CreateCenteredTransformMatrix(new Rectangle(Point.Empty, size), Matrix3x2Extensions.CreateSkew(radiansX, radiansY, PointF.Empty));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 CreateCenteredTransformMatrix(Rectangle sourceRectangle, Matrix3x2 matrix)
	{
		Rectangle transformedBoundingRectangle = GetTransformedBoundingRectangle(sourceRectangle, matrix);
		Matrix3x2.Invert(matrix, out var result);
		Matrix3x2 matrix3x = Matrix3x2.CreateTranslation(new Vector2(-transformedBoundingRectangle.Width, -transformedBoundingRectangle.Height) * 0.5f);
		Matrix3x2 matrix3x2 = Matrix3x2.CreateTranslation(new Vector2(sourceRectangle.Width, sourceRectangle.Height) * 0.5f);
		Matrix3x2.Invert(matrix3x * result * matrix3x2, out var result2);
		return result2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 CreateTaperMatrix(Size size, TaperSide side, TaperCorner corner, float fraction)
	{
		Matrix4x4 identity = Matrix4x4.Identity;
		switch (side)
		{
		case TaperSide.Left:
			identity.M11 = fraction;
			identity.M22 = fraction;
			identity.M14 = (fraction - 1f) / (float)size.Width;
			switch (corner)
			{
			case TaperCorner.LeftOrTop:
				identity.M12 = (float)size.Height * identity.M14;
				identity.M42 = (float)size.Height * (1f - fraction);
				break;
			case TaperCorner.Both:
				identity.M12 = (float)size.Height * 0.5f * identity.M14;
				identity.M42 = (float)size.Height * (1f - fraction) / 2f;
				break;
			}
			break;
		case TaperSide.Top:
			identity.M11 = fraction;
			identity.M22 = fraction;
			identity.M24 = (fraction - 1f) / (float)size.Height;
			switch (corner)
			{
			case TaperCorner.LeftOrTop:
				identity.M21 = (float)size.Width * identity.M24;
				identity.M41 = (float)size.Width * (1f - fraction);
				break;
			case TaperCorner.Both:
				identity.M21 = (float)size.Width * 0.5f * identity.M24;
				identity.M41 = (float)size.Width * (1f - fraction) * 0.5f;
				break;
			}
			break;
		case TaperSide.Right:
			identity.M11 = 1f / fraction;
			identity.M14 = (1f - fraction) / ((float)size.Width * fraction);
			switch (corner)
			{
			case TaperCorner.LeftOrTop:
				identity.M12 = (float)size.Height * identity.M14;
				break;
			case TaperCorner.Both:
				identity.M12 = (float)size.Height * 0.5f * identity.M14;
				break;
			}
			break;
		case TaperSide.Bottom:
			identity.M22 = 1f / fraction;
			identity.M24 = (1f - fraction) / ((float)size.Height * fraction);
			switch (corner)
			{
			case TaperCorner.LeftOrTop:
				identity.M21 = (float)size.Width * identity.M24;
				break;
			case TaperCorner.Both:
				identity.M21 = (float)size.Width * 0.5f * identity.M24;
				break;
			}
			break;
		}
		return identity;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rectangle GetTransformedBoundingRectangle(Rectangle rectangle, Matrix3x2 matrix)
	{
		Rectangle transformedRectangle = GetTransformedRectangle(rectangle, matrix);
		return new Rectangle(0, 0, transformedRectangle.Width, transformedRectangle.Height);
	}

	public static Rectangle GetTransformedRectangle(Rectangle rectangle, Matrix3x2 matrix)
	{
		if (rectangle.Equals(default(Rectangle)) || Matrix3x2.Identity.Equals(matrix))
		{
			return rectangle;
		}
		Vector2 tl = Vector2.Transform(new Vector2(rectangle.Left, rectangle.Top), matrix);
		Vector2 tr = Vector2.Transform(new Vector2(rectangle.Right, rectangle.Top), matrix);
		Vector2 bl = Vector2.Transform(new Vector2(rectangle.Left, rectangle.Bottom), matrix);
		Vector2 br = Vector2.Transform(new Vector2(rectangle.Right, rectangle.Bottom), matrix);
		return GetBoundingRectangle(tl, tr, bl, br);
	}

	public static Size GetTransformedSize(Size size, Matrix3x2 matrix)
	{
		Guard.IsTrue(size.Width > 0 && size.Height > 0, "size", "Source size dimensions cannot be 0!");
		if (matrix.Equals(default(Matrix3x2)) || matrix.Equals(Matrix3x2.Identity))
		{
			return size;
		}
		return ConstrainSize(GetTransformedRectangle(new Rectangle(Point.Empty, size), matrix));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rectangle GetTransformedRectangle(Rectangle rectangle, Matrix4x4 matrix)
	{
		if (rectangle.Equals(default(Rectangle)) || Matrix4x4.Identity.Equals(matrix))
		{
			return rectangle;
		}
		Vector2 tl = ProjectiveTransform2D(rectangle.Left, rectangle.Top, matrix);
		Vector2 tr = ProjectiveTransform2D(rectangle.Right, rectangle.Top, matrix);
		Vector2 bl = ProjectiveTransform2D(rectangle.Left, rectangle.Bottom, matrix);
		Vector2 br = ProjectiveTransform2D(rectangle.Right, rectangle.Bottom, matrix);
		return GetBoundingRectangle(tl, tr, bl, br);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Size GetTransformedSize(Size size, Matrix4x4 matrix)
	{
		Guard.IsTrue(size.Width > 0 && size.Height > 0, "size", "Source size dimensions cannot be 0!");
		if (matrix.Equals(default(Matrix4x4)) || matrix.Equals(Matrix4x4.Identity))
		{
			return size;
		}
		return ConstrainSize(GetTransformedRectangle(new Rectangle(Point.Empty, size), matrix));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Size ConstrainSize(Rectangle rectangle)
	{
		int num = ((rectangle.Top < 0) ? rectangle.Bottom : Math.Max(rectangle.Height, rectangle.Bottom));
		int num2 = ((rectangle.Left < 0) ? rectangle.Right : Math.Max(rectangle.Width, rectangle.Right));
		if (num <= 0)
		{
			num = rectangle.Height;
		}
		if (num2 <= 0)
		{
			num2 = rectangle.Width;
		}
		return new Size(num2, num);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Rectangle GetBoundingRectangle(Vector2 tl, Vector2 tr, Vector2 bl, Vector2 br)
	{
		float left = MathF.Min(tl.X, MathF.Min(tr.X, MathF.Min(bl.X, br.X)));
		float top = MathF.Min(tl.Y, MathF.Min(tr.Y, MathF.Min(bl.Y, br.Y)));
		float right = MathF.Max(tl.X, MathF.Max(tr.X, MathF.Max(bl.X, br.X)));
		float bottom = MathF.Max(tl.Y, MathF.Max(tr.Y, MathF.Max(bl.Y, br.Y)));
		return Rectangle.Round(RectangleF.FromLTRB(left, top, right, bottom));
	}
}
