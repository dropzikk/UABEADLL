using System;
using System.Linq;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccClut : IEquatable<IccClut>
{
	public float[][] Values { get; }

	public IccClutDataType DataType { get; }

	public int InputChannelCount { get; }

	public int OutputChannelCount { get; }

	public byte[] GridPointCount { get; }

	public IccClut(float[][] values, byte[] gridPointCount, IccClutDataType type)
	{
		Guard.NotNull(values, "values");
		Guard.NotNull(gridPointCount, "gridPointCount");
		Values = values;
		DataType = type;
		InputChannelCount = gridPointCount.Length;
		OutputChannelCount = values[0].Length;
		GridPointCount = gridPointCount;
		CheckValues();
	}

	public IccClut(ushort[][] values, byte[] gridPointCount)
	{
		Guard.NotNull(values, "values");
		Guard.NotNull(gridPointCount, "gridPointCount");
		Values = new float[values.Length][];
		for (int i = 0; i < values.Length; i++)
		{
			Values[i] = new float[values[i].Length];
			for (int j = 0; j < values[i].Length; j++)
			{
				Values[i][j] = (float)(int)values[i][j] / 65535f;
			}
		}
		DataType = IccClutDataType.UInt16;
		InputChannelCount = gridPointCount.Length;
		OutputChannelCount = values[0].Length;
		GridPointCount = gridPointCount;
		CheckValues();
	}

	public IccClut(byte[][] values, byte[] gridPointCount)
	{
		Guard.NotNull(values, "values");
		Guard.NotNull(gridPointCount, "gridPointCount");
		Values = new float[values.Length][];
		for (int i = 0; i < values.Length; i++)
		{
			Values[i] = new float[values[i].Length];
			for (int j = 0; j < values[i].Length; j++)
			{
				Values[i][j] = (float)(int)values[i][j] / 255f;
			}
		}
		DataType = IccClutDataType.UInt8;
		InputChannelCount = gridPointCount.Length;
		OutputChannelCount = values[0].Length;
		GridPointCount = gridPointCount;
		CheckValues();
	}

	public bool Equals(IccClut? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (EqualsValuesArray(other) && DataType == other.DataType && InputChannelCount == other.InputChannelCount && OutputChannelCount == other.OutputChannelCount)
		{
			return GridPointCount.AsSpan().SequenceEqual(other.GridPointCount);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccClut other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Values, DataType, InputChannelCount, OutputChannelCount, GridPointCount);
	}

	private bool EqualsValuesArray(IccClut other)
	{
		if (Values.Length != other.Values.Length)
		{
			return false;
		}
		for (int i = 0; i < Values.Length; i++)
		{
			if (!Values[i].AsSpan().SequenceEqual(other.Values[i]))
			{
				return false;
			}
		}
		return true;
	}

	private void CheckValues()
	{
		Guard.MustBeBetweenOrEqualTo(InputChannelCount, 1, 15, "InputChannelCount");
		Guard.MustBeBetweenOrEqualTo(OutputChannelCount, 1, 15, "OutputChannelCount");
		Guard.IsFalse(Values.Any((float[] t) => t.Length != OutputChannelCount), "Values", "The number of output values varies");
		int num = 0;
		for (int i = 0; i < InputChannelCount; i++)
		{
			num += (int)Math.Pow((int)GridPointCount[i], InputChannelCount);
		}
		num /= InputChannelCount;
		Guard.IsTrue(Values.Length == num, "Values", "Length of values array does not match the grid points");
	}
}
