using System;
using System.Collections.Generic;
using System.Numerics;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace SixLabors.ImageSharp.Processing;

public class AffineTransformBuilder
{
	private readonly List<Func<Size, Matrix3x2>> matrixFactories = new List<Func<Size, Matrix3x2>>();

	public AffineTransformBuilder PrependRotationDegrees(float degrees)
	{
		return PrependRotationRadians(GeometryUtilities.DegreeToRadian(degrees));
	}

	public AffineTransformBuilder PrependRotationRadians(float radians)
	{
		return Prepend((Size size) => TransformUtils.CreateRotationMatrixRadians(radians, size));
	}

	public AffineTransformBuilder PrependRotationDegrees(float degrees, Vector2 origin)
	{
		return PrependRotationRadians(GeometryUtilities.DegreeToRadian(degrees), origin);
	}

	public AffineTransformBuilder PrependRotationRadians(float radians, Vector2 origin)
	{
		return PrependMatrix(Matrix3x2.CreateRotation(radians, origin));
	}

	public AffineTransformBuilder AppendRotationDegrees(float degrees)
	{
		return AppendRotationRadians(GeometryUtilities.DegreeToRadian(degrees));
	}

	public AffineTransformBuilder AppendRotationRadians(float radians)
	{
		return Append((Size size) => TransformUtils.CreateRotationMatrixRadians(radians, size));
	}

	public AffineTransformBuilder AppendRotationDegrees(float degrees, Vector2 origin)
	{
		return AppendRotationRadians(GeometryUtilities.DegreeToRadian(degrees), origin);
	}

	public AffineTransformBuilder AppendRotationRadians(float radians, Vector2 origin)
	{
		return AppendMatrix(Matrix3x2.CreateRotation(radians, origin));
	}

	public AffineTransformBuilder PrependScale(float scale)
	{
		return PrependMatrix(Matrix3x2.CreateScale(scale));
	}

	public AffineTransformBuilder PrependScale(SizeF scale)
	{
		return PrependScale((Vector2)scale);
	}

	public AffineTransformBuilder PrependScale(Vector2 scales)
	{
		return PrependMatrix(Matrix3x2.CreateScale(scales));
	}

	public AffineTransformBuilder AppendScale(float scale)
	{
		return AppendMatrix(Matrix3x2.CreateScale(scale));
	}

	public AffineTransformBuilder AppendScale(SizeF scales)
	{
		return AppendScale((Vector2)scales);
	}

	public AffineTransformBuilder AppendScale(Vector2 scales)
	{
		return AppendMatrix(Matrix3x2.CreateScale(scales));
	}

	public AffineTransformBuilder PrependSkewDegrees(float degreesX, float degreesY)
	{
		return Prepend((Size size) => TransformUtils.CreateSkewMatrixDegrees(degreesX, degreesY, size));
	}

	public AffineTransformBuilder PrependSkewRadians(float radiansX, float radiansY)
	{
		return Prepend((Size size) => TransformUtils.CreateSkewMatrixRadians(radiansX, radiansY, size));
	}

	public AffineTransformBuilder PrependSkewDegrees(float degreesX, float degreesY, Vector2 origin)
	{
		return PrependSkewRadians(GeometryUtilities.DegreeToRadian(degreesX), GeometryUtilities.DegreeToRadian(degreesY), origin);
	}

	public AffineTransformBuilder PrependSkewRadians(float radiansX, float radiansY, Vector2 origin)
	{
		return PrependMatrix(Matrix3x2.CreateSkew(radiansX, radiansY, origin));
	}

	public AffineTransformBuilder AppendSkewDegrees(float degreesX, float degreesY)
	{
		return Append((Size size) => TransformUtils.CreateSkewMatrixDegrees(degreesX, degreesY, size));
	}

	public AffineTransformBuilder AppendSkewRadians(float radiansX, float radiansY)
	{
		return Append((Size size) => TransformUtils.CreateSkewMatrixRadians(radiansX, radiansY, size));
	}

	public AffineTransformBuilder AppendSkewDegrees(float degreesX, float degreesY, Vector2 origin)
	{
		return AppendSkewRadians(GeometryUtilities.DegreeToRadian(degreesX), GeometryUtilities.DegreeToRadian(degreesY), origin);
	}

	public AffineTransformBuilder AppendSkewRadians(float radiansX, float radiansY, Vector2 origin)
	{
		return AppendMatrix(Matrix3x2.CreateSkew(radiansX, radiansY, origin));
	}

	public AffineTransformBuilder PrependTranslation(PointF position)
	{
		return PrependTranslation((Vector2)position);
	}

	public AffineTransformBuilder PrependTranslation(Vector2 position)
	{
		return PrependMatrix(Matrix3x2.CreateTranslation(position));
	}

	public AffineTransformBuilder AppendTranslation(PointF position)
	{
		return AppendTranslation((Vector2)position);
	}

	public AffineTransformBuilder AppendTranslation(Vector2 position)
	{
		return AppendMatrix(Matrix3x2.CreateTranslation(position));
	}

	public AffineTransformBuilder PrependMatrix(Matrix3x2 matrix)
	{
		CheckDegenerate(matrix);
		return Prepend((Size _) => matrix);
	}

	public AffineTransformBuilder AppendMatrix(Matrix3x2 matrix)
	{
		CheckDegenerate(matrix);
		return Append((Size _) => matrix);
	}

	public Matrix3x2 BuildMatrix(Size sourceSize)
	{
		return BuildMatrix(new Rectangle(Point.Empty, sourceSize));
	}

	public Matrix3x2 BuildMatrix(Rectangle sourceRectangle)
	{
		Guard.MustBeGreaterThan(sourceRectangle.Width, 0, "sourceRectangle");
		Guard.MustBeGreaterThan(sourceRectangle.Height, 0, "sourceRectangle");
		Matrix3x2 matrix3x = Matrix3x2.CreateTranslation(-sourceRectangle.Location);
		Size size = sourceRectangle.Size;
		foreach (Func<Size, Matrix3x2> matrixFactory in matrixFactories)
		{
			matrix3x *= matrixFactory(size);
		}
		CheckDegenerate(matrix3x);
		return matrix3x;
	}

	private static void CheckDegenerate(Matrix3x2 matrix)
	{
		if (TransformUtils.IsDegenerate(matrix))
		{
			throw new DegenerateTransformException("Matrix is degenerate. Check input values.");
		}
	}

	private AffineTransformBuilder Prepend(Func<Size, Matrix3x2> factory)
	{
		matrixFactories.Insert(0, factory);
		return this;
	}

	private AffineTransformBuilder Append(Func<Size, Matrix3x2> factory)
	{
		matrixFactories.Add(factory);
		return this;
	}
}
