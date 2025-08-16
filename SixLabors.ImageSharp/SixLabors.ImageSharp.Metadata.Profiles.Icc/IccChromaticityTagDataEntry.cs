using System;
using System.Linq;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccChromaticityTagDataEntry : IccTagDataEntry, IEquatable<IccChromaticityTagDataEntry>
{
	public int ChannelCount => ChannelValues.Length;

	public IccColorantEncoding ColorantType { get; }

	public double[][] ChannelValues { get; }

	public IccChromaticityTagDataEntry(IccColorantEncoding colorantType)
		: this(colorantType, GetColorantArray(colorantType), IccProfileTag.Unknown)
	{
	}

	public IccChromaticityTagDataEntry(double[][] channelValues)
		: this(IccColorantEncoding.Unknown, channelValues, IccProfileTag.Unknown)
	{
	}

	public IccChromaticityTagDataEntry(IccColorantEncoding colorantType, IccProfileTag tagSignature)
		: this(colorantType, GetColorantArray(colorantType), tagSignature)
	{
	}

	public IccChromaticityTagDataEntry(double[][] channelValues, IccProfileTag tagSignature)
		: this(IccColorantEncoding.Unknown, channelValues, tagSignature)
	{
	}

	private IccChromaticityTagDataEntry(IccColorantEncoding colorantType, double[][] channelValues, IccProfileTag tagSignature)
		: base(IccTypeSignature.Chromaticity, tagSignature)
	{
		Guard.NotNull(channelValues, "channelValues");
		Guard.MustBeBetweenOrEqualTo(channelValues.Length, 1, 15, "channelValues");
		ColorantType = colorantType;
		ChannelValues = channelValues;
		int channelLength = channelValues[0].Length;
		Guard.IsFalse(channelValues.Any((double[] t) => t == null || t.Length != channelLength), "channelValues", "The number of values per channel is not the same for all channels");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccChromaticityTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccChromaticityTagDataEntry? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (base.Equals(other) && ColorantType == other.ColorantType)
		{
			return EqualsChannelValues(other);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccChromaticityTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, ColorantType, ChannelValues);
	}

	private static double[][] GetColorantArray(IccColorantEncoding colorantType)
	{
		return colorantType switch
		{
			IccColorantEncoding.EbuTech3213E => new double[3][]
			{
				new double[2] { 0.64, 0.33 },
				new double[2] { 0.29, 0.6 },
				new double[2] { 0.15, 0.06 }
			}, 
			IccColorantEncoding.ItuRBt709_2 => new double[3][]
			{
				new double[2] { 0.64, 0.33 },
				new double[2] { 0.3, 0.6 },
				new double[2] { 0.15, 0.06 }
			}, 
			IccColorantEncoding.P22 => new double[3][]
			{
				new double[2] { 0.625, 0.34 },
				new double[2] { 0.28, 0.605 },
				new double[2] { 0.155, 0.07 }
			}, 
			IccColorantEncoding.SmpteRp145 => new double[3][]
			{
				new double[2] { 0.63, 0.34 },
				new double[2] { 0.31, 0.595 },
				new double[2] { 0.155, 0.07 }
			}, 
			_ => throw new ArgumentException("Unrecognized colorant encoding"), 
		};
	}

	private bool EqualsChannelValues(IccChromaticityTagDataEntry entry)
	{
		if (ChannelValues.Length != entry.ChannelValues.Length)
		{
			return false;
		}
		for (int i = 0; i < ChannelValues.Length; i++)
		{
			if (!ChannelValues[i].AsSpan().SequenceEqual(entry.ChannelValues[i]))
			{
				return false;
			}
		}
		return true;
	}
}
