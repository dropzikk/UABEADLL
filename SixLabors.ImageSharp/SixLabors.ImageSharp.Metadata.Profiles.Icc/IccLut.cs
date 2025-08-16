using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal readonly struct IccLut : IEquatable<IccLut>
{
	public float[] Values { get; }

	public IccLut(float[] values)
	{
		Values = values ?? throw new ArgumentNullException("values");
	}

	public IccLut(ushort[] values)
	{
		Guard.NotNull(values, "values");
		Values = new float[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			Values[i] = (float)(int)values[i] / 65535f;
		}
	}

	public IccLut(byte[] values)
	{
		Guard.NotNull(values, "values");
		Values = new float[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			Values[i] = (float)(int)values[i] / 255f;
		}
	}

	public bool Equals(IccLut other)
	{
		if (Values == other.Values)
		{
			return true;
		}
		return Values.AsSpan().SequenceEqual(other.Values);
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccLut other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Values.GetHashCode();
	}
}
