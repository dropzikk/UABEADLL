using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccMatrixProcessElement : IccMultiProcessElement, IEquatable<IccMatrixProcessElement>
{
	public DenseMatrix<float> MatrixIxO { get; }

	public float[] MatrixOx1 { get; }

	public IccMatrixProcessElement(float[,] matrixIxO, float[] matrixOx1)
		: base(IccMultiProcessElementSignature.Matrix, matrixIxO?.GetLength(0) ?? 1, matrixIxO?.GetLength(1) ?? 1)
	{
		Guard.NotNull(matrixIxO, "matrixIxO");
		Guard.NotNull(matrixOx1, "matrixOx1");
		Guard.IsTrue(matrixIxO.GetLength(1) == matrixOx1.Length, "matrixIxO,matrixIxO", "Output channel length must match");
		MatrixIxO = matrixIxO;
		MatrixOx1 = matrixOx1;
	}

	public override bool Equals(IccMultiProcessElement? other)
	{
		if (base.Equals(other) && other is IccMatrixProcessElement iccMatrixProcessElement)
		{
			if (EqualsMatrix(iccMatrixProcessElement))
			{
				return MatrixOx1.AsSpan().SequenceEqual(iccMatrixProcessElement.MatrixOx1);
			}
			return false;
		}
		return false;
	}

	public bool Equals(IccMatrixProcessElement? other)
	{
		return Equals((IccMultiProcessElement?)other);
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as IccMatrixProcessElement);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.GetHashCode(), MatrixIxO, MatrixOx1);
	}

	private bool EqualsMatrix(IccMatrixProcessElement element)
	{
		return MatrixIxO.Equals(element.MatrixIxO);
	}
}
