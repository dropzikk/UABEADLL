using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Avalonia.Media.Transformation;

public record struct TransformOperation
{
	public enum OperationType
	{
		Translate,
		Rotate,
		Scale,
		Skew,
		Matrix,
		Identity
	}

	[StructLayout(LayoutKind.Explicit)]
	public record struct DataLayout
	{
		public record struct SkewLayout
		{
			public double X;

			public double Y;
		}

		public record struct ScaleLayout
		{
			public double X;

			public double Y;
		}

		public record struct TranslateLayout
		{
			public double X;

			public double Y;
		}

		public record struct RotateLayout
		{
			public double Angle;
		}

		[FieldOffset(0)]
		public SkewLayout Skew;

		[FieldOffset(0)]
		public ScaleLayout Scale;

		[FieldOffset(0)]
		public TranslateLayout Translate;

		[FieldOffset(0)]
		public RotateLayout Rotate;
	}

	public bool IsIdentity => Matrix.IsIdentity;

	public static TransformOperation Identity
	{
		get
		{
			TransformOperation result = default(TransformOperation);
			result.Matrix = Matrix.Identity;
			result.Type = OperationType.Identity;
			return result;
		}
	}

	public OperationType Type;

	public Matrix Matrix;

	public DataLayout Data;

	public void Bake()
	{
		Matrix = Matrix.Identity;
		switch (Type)
		{
		case OperationType.Translate:
			Matrix = Matrix.CreateTranslation(Data.Translate.X, Data.Translate.Y);
			break;
		case OperationType.Rotate:
			Matrix = Matrix.CreateRotation(Data.Rotate.Angle);
			break;
		case OperationType.Scale:
			Matrix = Matrix.CreateScale(Data.Scale.X, Data.Scale.Y);
			break;
		case OperationType.Skew:
			Matrix = Matrix.CreateSkew(Data.Skew.X, Data.Skew.Y);
			break;
		}
	}

	public static bool TryInterpolate(TransformOperation? from, TransformOperation? to, double progress, ref TransformOperation result)
	{
		bool flag = IsOperationIdentity(ref from);
		bool flag2 = IsOperationIdentity(ref to);
		if (flag && flag2)
		{
			result.Matrix = Matrix.Identity;
			return true;
		}
		TransformOperation transformOperation = (flag ? Identity : from.Value);
		TransformOperation transformOperation2 = (flag2 ? Identity : to.Value);
		switch (result.Type = (flag2 ? transformOperation.Type : transformOperation2.Type))
		{
		case OperationType.Translate:
		{
			double from4 = (flag ? 0.0 : transformOperation.Data.Translate.X);
			double from5 = (flag ? 0.0 : transformOperation.Data.Translate.Y);
			double to5 = (flag2 ? 0.0 : transformOperation2.Data.Translate.X);
			double to6 = (flag2 ? 0.0 : transformOperation2.Data.Translate.Y);
			result.Data.Translate.X = InterpolationUtilities.InterpolateScalars(from4, to5, progress);
			result.Data.Translate.Y = InterpolationUtilities.InterpolateScalars(from5, to6, progress);
			result.Bake();
			break;
		}
		case OperationType.Rotate:
		{
			double from8 = (flag ? 0.0 : transformOperation.Data.Rotate.Angle);
			double to9 = (flag2 ? 0.0 : transformOperation2.Data.Rotate.Angle);
			result.Data.Rotate.Angle = InterpolationUtilities.InterpolateScalars(from8, to9, progress);
			result.Bake();
			break;
		}
		case OperationType.Scale:
		{
			double from2 = (flag ? 1.0 : transformOperation.Data.Scale.X);
			double from3 = (flag ? 1.0 : transformOperation.Data.Scale.Y);
			double to3 = (flag2 ? 1.0 : transformOperation2.Data.Scale.X);
			double to4 = (flag2 ? 1.0 : transformOperation2.Data.Scale.Y);
			result.Data.Scale.X = InterpolationUtilities.InterpolateScalars(from2, to3, progress);
			result.Data.Scale.Y = InterpolationUtilities.InterpolateScalars(from3, to4, progress);
			result.Bake();
			break;
		}
		case OperationType.Skew:
		{
			double from6 = (flag ? 0.0 : transformOperation.Data.Skew.X);
			double from7 = (flag ? 0.0 : transformOperation.Data.Skew.Y);
			double to7 = (flag2 ? 0.0 : transformOperation2.Data.Skew.X);
			double to8 = (flag2 ? 0.0 : transformOperation2.Data.Skew.Y);
			result.Data.Skew.X = InterpolationUtilities.InterpolateScalars(from6, to7, progress);
			result.Data.Skew.Y = InterpolationUtilities.InterpolateScalars(from7, to8, progress);
			result.Bake();
			break;
		}
		case OperationType.Matrix:
		{
			Matrix matrix = (flag ? Matrix.Identity : transformOperation.Matrix);
			Matrix matrix2 = (flag2 ? Matrix.Identity : transformOperation2.Matrix);
			if (!Matrix.TryDecomposeTransform(matrix, out var decomposed) || !Matrix.TryDecomposeTransform(matrix2, out var to2))
			{
				return false;
			}
			Matrix.Decomposed decomposed2 = InterpolationUtilities.InterpolateDecomposedTransforms(ref decomposed, ref to2, progress);
			result.Matrix = InterpolationUtilities.ComposeTransform(decomposed2);
			break;
		}
		case OperationType.Identity:
			result.Matrix = Matrix.Identity;
			break;
		}
		return true;
	}

	private static bool IsOperationIdentity(ref TransformOperation? operation)
	{
		if (operation.HasValue)
		{
			return operation.Value.IsIdentity;
		}
		return true;
	}

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append("Type = ");
		builder.Append(Type.ToString());
		builder.Append(", Matrix = ");
		builder.Append(Matrix.ToString());
		builder.Append(", Data = ");
		builder.Append(Data.ToString());
		builder.Append(", IsIdentity = ");
		builder.Append(IsIdentity.ToString());
		return true;
	}
}
