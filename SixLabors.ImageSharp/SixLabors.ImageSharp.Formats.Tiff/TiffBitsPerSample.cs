using System;

namespace SixLabors.ImageSharp.Formats.Tiff;

public readonly struct TiffBitsPerSample : IEquatable<TiffBitsPerSample>
{
	public ushort Channel0 { get; }

	public ushort Channel1 { get; }

	public ushort Channel2 { get; }

	public ushort Channel3 { get; }

	public byte Channels { get; }

	public TiffBitsPerSample(ushort channel0, ushort channel1, ushort channel2, ushort channel3 = 0)
	{
		Channel0 = (ushort)Numerics.Clamp(channel0, 0, 32);
		Channel1 = (ushort)Numerics.Clamp(channel1, 0, 32);
		Channel2 = (ushort)Numerics.Clamp(channel2, 0, 32);
		Channel3 = (ushort)Numerics.Clamp(channel3, 0, 32);
		Channels = 0;
		Channels = (byte)((uint)Channels + ((Channel0 != 0) ? 1u : 0u));
		Channels = (byte)((uint)Channels + ((Channel1 != 0) ? 1u : 0u));
		Channels = (byte)((uint)Channels + ((Channel2 != 0) ? 1u : 0u));
		Channels = (byte)((uint)Channels + ((Channel3 != 0) ? 1u : 0u));
	}

	public static bool operator ==(TiffBitsPerSample left, TiffBitsPerSample right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(TiffBitsPerSample left, TiffBitsPerSample right)
	{
		return !(left == right);
	}

	public static bool TryParse(ushort[]? value, out TiffBitsPerSample sample)
	{
		if (value == null || value.Length == 0)
		{
			sample = default(TiffBitsPerSample);
			return false;
		}
		ushort channel = 0;
		ushort channel2;
		ushort channel3;
		ushort channel4;
		switch (value.Length)
		{
		case 4:
			channel = value[3];
			channel2 = value[2];
			channel3 = value[1];
			channel4 = value[0];
			break;
		case 3:
			channel2 = value[2];
			channel3 = value[1];
			channel4 = value[0];
			break;
		case 2:
			channel2 = 0;
			channel3 = value[1];
			channel4 = value[0];
			break;
		default:
			channel2 = 0;
			channel3 = 0;
			channel4 = value[0];
			break;
		}
		sample = new TiffBitsPerSample(channel4, channel3, channel2, channel);
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is TiffBitsPerSample other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(TiffBitsPerSample other)
	{
		if (Channel0 == other.Channel0 && Channel1 == other.Channel1 && Channel2 == other.Channel2)
		{
			return Channel3 == other.Channel3;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Channel0, Channel1, Channel2, Channel3);
	}

	public ushort[] ToArray()
	{
		if (Channel1 != 0)
		{
			if (Channel2 != 0)
			{
				if (Channel3 != 0)
				{
					return new ushort[4] { Channel0, Channel1, Channel2, Channel3 };
				}
				return new ushort[3] { Channel0, Channel1, Channel2 };
			}
			return new ushort[2] { Channel0, Channel1 };
		}
		return new ushort[1] { Channel0 };
	}

	public TiffBitsPerPixel BitsPerPixel()
	{
		return (TiffBitsPerPixel)(Channel0 + Channel1 + Channel2 + Channel3);
	}

	public override string ToString()
	{
		if (Channel3 == 0)
		{
			return $"TiffBitsPerSample({Channel0}, {Channel1}, {Channel2})";
		}
		return $"TiffBitsPerSample({Channel0}, {Channel1}, {Channel2}, {Channel3})";
	}
}
