using System;
using System.Collections.Generic;
using System.Numerics;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace SixLabors.ImageSharp.Processing;

public class ProjectiveTransformBuilder
{
	private readonly List<Func<Size, Matrix4x4>> matrixFactories = new List<Func<Size, Matrix4x4>>();

	public ProjectiveTransformBuilder PrependTaper(TaperSide side, TaperCorner corner, float fraction)
	{
		return Prepend((Size size) => TransformUtils.CreateTaperMatrix(size, side, corner, fraction));
	}

	public ProjectiveTransformBuilder AppendTaper(TaperSide side, TaperCorner corner, float fraction)
	{
		return Append((Size size) => TransformUtils.CreateTaperMatrix(size, side, corner, fraction));
	}

	public ProjectiveTransformBuilder PrependRotationDegrees(float degrees)
	{
		return PrependRotationRadians(GeometryUtilities.DegreeToRadian(degrees));
	}

	public ProjectiveTransformBuilder PrependRotationRadians(float radians)
	{
		return Prepend((Size size) => new Matrix4x4(TransformUtils.CreateRotationMatrixRadians(radians, size)));
	}

	internal ProjectiveTransformBuilder PrependRotationDegrees(float degrees, Vector2 origin)
	{
		return PrependRotationRadians(GeometryUtilities.DegreeToRadian(degrees), origin);
	}

	internal ProjectiveTransformBuilder PrependRotationRadians(float radians, Vector2 origin)
	{
		return PrependMatrix(Matrix4x4.CreateRotationZ(radians, new Vector3(origin, 0f)));
	}

	public ProjectiveTransformBuilder AppendRotationDegrees(float degrees)
	{
		return AppendRotationRadians(GeometryUtilities.DegreeToRadian(degrees));
	}

	public ProjectiveTransformBuilder AppendRotationRadians(float radians)
	{
		return Append((Size size) => new Matrix4x4(TransformUtils.CreateRotationMatrixRadians(radians, size)));
	}

	internal ProjectiveTransformBuilder AppendRotationDegrees(float degrees, Vector2 origin)
	{
		return AppendRotationRadians(GeometryUtilities.DegreeToRadian(degrees), origin);
	}

	internal ProjectiveTransformBuilder AppendRotationRadians(float radians, Vector2 origin)
	{
		return AppendMatrix(Matrix4x4.CreateRotationZ(radians, new Vector3(origin, 0f)));
	}

	public ProjectiveTransformBuilder PrependScale(float scale)
	{
		return PrependMatrix(Matrix4x4.CreateScale(scale));
	}

	public ProjectiveTransformBuilder PrependScale(SizeF scale)
	{
		return PrependScale((Vector2)scale);
	}

	public ProjectiveTransformBuilder PrependScale(Vector2 scales)
	{
		return PrependMatrix(Matrix4x4.CreateScale(new Vector3(scales, 1f)));
	}

	public ProjectiveTransformBuilder AppendScale(float scale)
	{
		return AppendMatrix(Matrix4x4.CreateScale(scale));
	}

	public ProjectiveTransformBuilder AppendScale(SizeF scales)
	{
		return AppendScale((Vector2)scales);
	}

	public ProjectiveTransformBuilder AppendScale(Vector2 scales)
	{
		return AppendMatrix(Matrix4x4.CreateScale(new Vector3(scales, 1f)));
	}

	internal ProjectiveTransformBuilder PrependSkewDegrees(float degreesX, float degreesY)
	{
		return PrependSkewRadians(GeometryUtilities.DegreeToRadian(degreesX), GeometryUtilities.DegreeToRadian(degreesY));
	}

	public ProjectiveTransformBuilder PrependSkewRadians(float radiansX, float radiansY)
	{
		return Prepend((Size size) => new Matrix4x4(TransformUtils.CreateSkewMatrixRadians(radiansX, radiansY, size)));
	}

	public ProjectiveTransformBuilder PrependSkewDegrees(float degreesX, float degreesY, Vector2 origin)
	{
		return PrependSkewRadians(GeometryUtilities.DegreeToRadian(degreesX), GeometryUtilities.DegreeToRadian(degreesY), origin);
	}

	public ProjectiveTransformBuilder PrependSkewRadians(float radiansX, float radiansY, Vector2 origin)
	{
		return PrependMatrix(new Matrix4x4(Matrix3x2.CreateSkew(radiansX, radiansY, origin)));
	}

	internal ProjectiveTransformBuilder AppendSkewDegrees(float degreesX, float degreesY)
	{
		return AppendSkewRadians(GeometryUtilities.DegreeToRadian(degreesX), GeometryUtilities.DegreeToRadian(degreesY));
	}

	public ProjectiveTransformBuilder AppendSkewRadians(float radiansX, float radiansY)
	{
		return Append((Size size) => new Matrix4x4(TransformUtils.CreateSkewMatrixRadians(radiansX, radiansY, size)));
	}

	public ProjectiveTransformBuilder AppendSkewDegrees(float degreesX, float degreesY, Vector2 origin)
	{
		return AppendSkewRadians(GeometryUtilities.DegreeToRadian(degreesX), GeometryUtilities.DegreeToRadian(degreesY), origin);
	}

	public ProjectiveTransformBuilder AppendSkewRadians(float radiansX, float radiansY, Vector2 origin)
	{
		return AppendMatrix(new Matrix4x4(Matrix3x2.CreateSkew(radiansX, radiansY, origin)));
	}

	public ProjectiveTransformBuilder PrependTranslation(PointF position)
	{
		return PrependTranslation((Vector2)position);
	}

	public ProjectiveTransformBuilder PrependTranslation(Vector2 position)
	{
		return PrependMatrix(Matrix4x4.CreateTranslation(new Vector3(position, 0f)));
	}

	public ProjectiveTransformBuilder AppendTranslation(PointF position)
	{
		return AppendTranslation((Vector2)position);
	}

	public ProjectiveTransformBuilder AppendTranslation(Vector2 position)
	{
		return AppendMatrix(Matrix4x4.CreateTranslation(new Vector3(position, 0f)));
	}

	public ProjectiveTransformBuilder PrependMatrix(Matrix4x4 matrix)
	{
		CheckDegenerate(matrix);
		return Prepend((Size _) => matrix);
	}

	public ProjectiveTransformBuilder AppendMatrix(Matrix4x4 matrix)
	{
		CheckDegenerate(matrix);
		return Append((Size _) => matrix);
	}

	public Matrix4x4 BuildMatrix(Size sourceSize)
	{
		return BuildMatrix(new Rectangle(Point.Empty, sourceSize));
	}

	public Matrix4x4 BuildMatrix(Rectangle sourceRectangle)
	{
		Guard.MustBeGreaterThan(sourceRectangle.Width, 0, "sourceRectangle");
		Guard.MustBeGreaterThan(sourceRectangle.Height, 0, "sourceRectangle");
		Matrix4x4 matrix4x = Matrix4x4.CreateTranslation(new Vector3(-sourceRectangle.Location, 0f));
		Size size = sourceRectangle.Size;
		foreach (Func<Size, Matrix4x4> matrixFactory in matrixFactories)
		{
			matrix4x *= matrixFactory(size);
		}
		CheckDegenerate(matrix4x);
		return matrix4x;
	}

	private static void CheckDegenerate(Matrix4x4 matrix)
	{
		if (TransformUtils.IsDegenerate(matrix))
		{
			throw new DegenerateTransformException("Matrix is degenerate. Check input values.");
		}
	}

	private ProjectiveTransformBuilder Prepend(Func<Size, Matrix4x4> factory)
	{
		matrixFactories.Insert(0, factory);
		return this;
	}

	private ProjectiveTransformBuilder Append(Func<Size, Matrix4x4> factory)
	{
		matrixFactories.Add(factory);
		return this;
	}
}
