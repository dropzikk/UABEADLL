using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal readonly struct IccResponseNumber : IEquatable<IccResponseNumber>
{
	public ushort DeviceCode { get; }

	public float MeasurementValue { get; }

	public IccResponseNumber(ushort deviceCode, float measurementValue)
	{
		DeviceCode = deviceCode;
		MeasurementValue = measurementValue;
	}

	public static bool operator ==(IccResponseNumber left, IccResponseNumber right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(IccResponseNumber left, IccResponseNumber right)
	{
		return !left.Equals(right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccResponseNumber other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(IccResponseNumber other)
	{
		if (DeviceCode == other.DeviceCode)
		{
			return MeasurementValue == other.MeasurementValue;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(DeviceCode, MeasurementValue);
	}

	public override string ToString()
	{
		return $"Code: {DeviceCode}; Value: {MeasurementValue}";
	}
}
