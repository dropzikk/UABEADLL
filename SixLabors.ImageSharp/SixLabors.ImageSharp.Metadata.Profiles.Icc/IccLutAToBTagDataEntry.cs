using System;
using System.Linq;
using System.Numerics;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccLutAToBTagDataEntry : IccTagDataEntry, IEquatable<IccLutAToBTagDataEntry>
{
	public int InputChannelCount { get; }

	public int OutputChannelCount { get; }

	public Matrix4x4? Matrix3x3 { get; }

	public Vector3? Matrix3x1 { get; }

	public IccClut ClutValues { get; }

	public IccTagDataEntry[] CurveB { get; }

	public IccTagDataEntry[] CurveM { get; }

	public IccTagDataEntry[] CurveA { get; }

	public IccLutAToBTagDataEntry(IccTagDataEntry[] curveB, float[,] matrix3x3, float[] matrix3x1, IccTagDataEntry[] curveM, IccClut clutValues, IccTagDataEntry[] curveA)
		: this(curveB, matrix3x3, matrix3x1, curveM, clutValues, curveA, IccProfileTag.Unknown)
	{
	}

	public IccLutAToBTagDataEntry(IccTagDataEntry[] curveB, float[,] matrix3x3, float[] matrix3x1, IccTagDataEntry[] curveM, IccClut clutValues, IccTagDataEntry[] curveA, IccProfileTag tagSignature)
		: base(IccTypeSignature.LutAToB, tagSignature)
	{
		VerifyMatrix(matrix3x3, matrix3x1);
		VerifyCurve(curveA, "curveA");
		VerifyCurve(curveB, "curveB");
		VerifyCurve(curveM, "curveM");
		Matrix3x3 = CreateMatrix3x3(matrix3x3);
		Matrix3x1 = CreateMatrix3x1(matrix3x1);
		CurveA = curveA;
		CurveB = curveB;
		CurveM = curveM;
		ClutValues = clutValues;
		if (IsAClutMMatrixB())
		{
			Guard.IsTrue(CurveB.Length == 3, "CurveB", "CurveB must have a length of three");
			Guard.IsTrue(CurveM.Length == 3, "CurveM", "CurveM must have a length of three");
			Guard.MustBeBetweenOrEqualTo(CurveA.Length, 1, 15, "CurveA");
			InputChannelCount = curveA.Length;
			OutputChannelCount = 3;
			Guard.IsTrue(InputChannelCount == clutValues.InputChannelCount, "clutValues", "Input channel count does not match the CLUT size");
			Guard.IsTrue(OutputChannelCount == clutValues.OutputChannelCount, "clutValues", "Output channel count does not match the CLUT size");
		}
		else if (IsMMatrixB())
		{
			Guard.IsTrue(CurveB.Length == 3, "CurveB", "CurveB must have a length of three");
			Guard.IsTrue(CurveM.Length == 3, "CurveM", "CurveM must have a length of three");
			InputChannelCount = (OutputChannelCount = 3);
		}
		else if (IsAClutB())
		{
			Guard.MustBeBetweenOrEqualTo(CurveA.Length, 1, 15, "CurveA");
			Guard.MustBeBetweenOrEqualTo(CurveB.Length, 1, 15, "CurveB");
			InputChannelCount = curveA.Length;
			OutputChannelCount = curveB.Length;
			Guard.IsTrue(InputChannelCount == clutValues.InputChannelCount, "clutValues", "Input channel count does not match the CLUT size");
			Guard.IsTrue(OutputChannelCount == clutValues.OutputChannelCount, "clutValues", "Output channel count does not match the CLUT size");
		}
		else
		{
			if (!IsB())
			{
				throw new ArgumentException("Invalid combination of values given");
			}
			InputChannelCount = (OutputChannelCount = CurveB.Length);
		}
	}

	public override bool Equals(IccTagDataEntry other)
	{
		if (other is IccLutAToBTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccLutAToBTagDataEntry other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (base.Equals(other) && InputChannelCount == other.InputChannelCount && OutputChannelCount == other.OutputChannelCount && Matrix3x3.Equals(other.Matrix3x3) && Matrix3x1.Equals(other.Matrix3x1) && ClutValues.Equals(other.ClutValues) && EqualsCurve(CurveB, other.CurveB) && EqualsCurve(CurveM, other.CurveM))
		{
			return EqualsCurve(CurveA, other.CurveA);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is IccLutAToBTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		HashCode hashCode = default(HashCode);
		hashCode.Add(base.Signature);
		hashCode.Add(InputChannelCount);
		hashCode.Add(OutputChannelCount);
		hashCode.Add(Matrix3x3);
		hashCode.Add(Matrix3x1);
		hashCode.Add(ClutValues);
		hashCode.Add(CurveB);
		hashCode.Add(CurveM);
		hashCode.Add(CurveA);
		return hashCode.ToHashCode();
	}

	private static bool EqualsCurve(IccTagDataEntry[] thisCurves, IccTagDataEntry[] entryCurves)
	{
		bool num = thisCurves == null;
		bool flag = entryCurves == null;
		if (num && flag)
		{
			return true;
		}
		if (flag)
		{
			return false;
		}
		return thisCurves.SequenceEqual(entryCurves);
	}

	private bool IsAClutMMatrixB()
	{
		if (CurveB != null && Matrix3x3.HasValue && Matrix3x1.HasValue && CurveM != null && ClutValues != null)
		{
			return CurveA != null;
		}
		return false;
	}

	private bool IsMMatrixB()
	{
		if (CurveB != null && Matrix3x3.HasValue && Matrix3x1.HasValue)
		{
			return CurveM != null;
		}
		return false;
	}

	private bool IsAClutB()
	{
		if (CurveB != null && ClutValues != null)
		{
			return CurveA != null;
		}
		return false;
	}

	private bool IsB()
	{
		return CurveB != null;
	}

	private void VerifyCurve(IccTagDataEntry[] curves, string name)
	{
		if (curves != null)
		{
			Guard.IsFalse(curves.Any((IccTagDataEntry t) => !(t is IccParametricCurveTagDataEntry) && !(t is IccCurveTagDataEntry)), "name", "name must be of type IccParametricCurveTagDataEntry or IccCurveTagDataEntry");
		}
	}

	private static void VerifyMatrix(float[,] matrix3x3, float[] matrix3x1)
	{
		if (matrix3x1 != null)
		{
			Guard.IsTrue(matrix3x1.Length == 3, "matrix3x1", "Matrix must have a size of three");
		}
		if (matrix3x3 != null)
		{
			Guard.IsTrue(matrix3x3.GetLength(0) == 3 && matrix3x3.GetLength(1) == 3, "matrix3x3", "Matrix must have a size of three by three");
		}
	}

	private static Vector3? CreateMatrix3x1(float[] matrix)
	{
		if (matrix == null)
		{
			return null;
		}
		return new Vector3(matrix[0], matrix[1], matrix[2]);
	}

	private static Matrix4x4? CreateMatrix3x3(float[,] matrix)
	{
		if (matrix == null)
		{
			return null;
		}
		return new Matrix4x4(matrix[0, 0], matrix[0, 1], matrix[0, 2], 0f, matrix[1, 0], matrix[1, 1], matrix[1, 2], 0f, matrix[2, 0], matrix[2, 1], matrix[2, 2], 0f, 0f, 0f, 0f, 1f);
	}
}
