using System;
using System.Numerics;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccLut16TagDataEntry : IccTagDataEntry, IEquatable<IccLut16TagDataEntry>
{
	private static readonly float[,] IdentityMatrix = new float[3, 3]
	{
		{ 1f, 0f, 0f },
		{ 0f, 1f, 0f },
		{ 0f, 0f, 1f }
	};

	public int InputChannelCount => InputValues.Length;

	public int OutputChannelCount => OutputValues.Length;

	public Matrix4x4 Matrix { get; }

	public IccLut[] InputValues { get; }

	public IccClut ClutValues { get; }

	public IccLut[] OutputValues { get; }

	public IccLut16TagDataEntry(IccLut[] inputValues, IccClut clutValues, IccLut[] outputValues)
		: this(IdentityMatrix, inputValues, clutValues, outputValues, IccProfileTag.Unknown)
	{
	}

	public IccLut16TagDataEntry(IccLut[] inputValues, IccClut clutValues, IccLut[] outputValues, IccProfileTag tagSignature)
		: this(IdentityMatrix, inputValues, clutValues, outputValues, tagSignature)
	{
	}

	public IccLut16TagDataEntry(float[,] matrix, IccLut[] inputValues, IccClut clutValues, IccLut[] outputValues)
		: this(matrix, inputValues, clutValues, outputValues, IccProfileTag.Unknown)
	{
	}

	public IccLut16TagDataEntry(float[,] matrix, IccLut[] inputValues, IccClut clutValues, IccLut[] outputValues, IccProfileTag tagSignature)
		: base(IccTypeSignature.Lut16, tagSignature)
	{
		Guard.NotNull(matrix, "matrix");
		Guard.IsTrue(matrix.GetLength(0) == 3 && matrix.GetLength(1) == 3, "matrix", "Matrix must have a size of three by three");
		Matrix = CreateMatrix(matrix);
		InputValues = inputValues ?? throw new ArgumentNullException("inputValues");
		ClutValues = clutValues ?? throw new ArgumentNullException("clutValues");
		OutputValues = outputValues ?? throw new ArgumentNullException("outputValues");
		Guard.IsTrue(InputChannelCount == clutValues.InputChannelCount, "clutValues", "Input channel count does not match the CLUT size");
		Guard.IsTrue(OutputChannelCount == clutValues.OutputChannelCount, "clutValues", "Output channel count does not match the CLUT size");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccLut16TagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccLut16TagDataEntry? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (base.Equals(other) && Matrix.Equals(other.Matrix) && InputValues.AsSpan().SequenceEqual(other.InputValues) && ClutValues.Equals(other.ClutValues))
		{
			return OutputValues.AsSpan().SequenceEqual(other.OutputValues);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccLut16TagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, Matrix, InputValues, ClutValues, OutputValues);
	}

	private static Matrix4x4 CreateMatrix(float[,] matrix)
	{
		return new Matrix4x4(matrix[0, 0], matrix[0, 1], matrix[0, 2], 0f, matrix[1, 0], matrix[1, 1], matrix[1, 2], 0f, matrix[2, 0], matrix[2, 1], matrix[2, 2], 0f, 0f, 0f, 0f, 1f);
	}
}
