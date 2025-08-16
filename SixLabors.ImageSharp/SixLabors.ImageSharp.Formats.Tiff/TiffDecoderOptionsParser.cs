using System.Linq;
using SixLabors.ImageSharp.Formats.Tiff.Compression;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace SixLabors.ImageSharp.Formats.Tiff;

internal static class TiffDecoderOptionsParser
{
	private const TiffPlanarConfiguration DefaultPlanarConfiguration = TiffPlanarConfiguration.Chunky;

	public static bool VerifyAndParse(this TiffDecoderCore options, ExifProfile exifProfile, TiffFrameMetadata frameMetadata)
	{
		IExifValue valueInternal = exifProfile.GetValueInternal(ExifTag.ExtraSamples);
		if (valueInternal != null)
		{
			short[] obj = (short[])valueInternal.GetValue();
			if (obj.Length != 1)
			{
				TiffThrowHelper.ThrowNotSupported("ExtraSamples is only supported with one extra sample for alpha data.");
			}
			TiffExtraSampleType tiffExtraSampleType = (TiffExtraSampleType)obj[0];
			options.ExtraSamplesType = tiffExtraSampleType;
			if ((uint)(tiffExtraSampleType - 1) > 1u)
			{
				TiffThrowHelper.ThrowNotSupported("Decoding Tiff images with ExtraSamples is not supported with UnspecifiedData.");
			}
		}
		IExifValue<ushort> exifValue;
		TiffFillOrder tiffFillOrder = (TiffFillOrder)((!exifProfile.TryGetValue(ExifTag.FillOrder, out exifValue)) ? 1 : exifValue.Value);
		if (tiffFillOrder == TiffFillOrder.LeastSignificantBitFirst && frameMetadata.BitsPerPixel != TiffBitsPerPixel.Bit1)
		{
			TiffThrowHelper.ThrowNotSupported("The lower-order bits of the byte FillOrder is only supported in combination with 1bit per pixel bicolor tiff's.");
		}
		if (frameMetadata.Predictor == TiffPredictor.FloatingPoint)
		{
			TiffThrowHelper.ThrowNotSupported("TIFF images with FloatingPoint horizontal predictor are not supported.");
		}
		TiffSampleFormat? tiffSampleFormat = null;
		if (exifProfile.TryGetValue(ExifTag.SampleFormat, out IExifValue<ushort[]> exifValue2))
		{
			TiffSampleFormat[] array = exifValue2.Value.Select((ushort a) => (TiffSampleFormat)a).ToArray();
			tiffSampleFormat = array[0];
			TiffSampleFormat[] array2 = array;
			foreach (TiffSampleFormat tiffSampleFormat2 in array2)
			{
				if (tiffSampleFormat2 != TiffSampleFormat.UnsignedInteger && tiffSampleFormat2 != TiffSampleFormat.Float)
				{
					TiffThrowHelper.ThrowNotSupported("ImageSharp only supports the UnsignedInteger and Float SampleFormat.");
				}
			}
		}
		ushort[] array3 = null;
		if (exifProfile.TryGetValue(ExifTag.YCbCrSubsampling, out IExifValue<ushort[]> exifValue3))
		{
			array3 = exifValue3.Value;
		}
		if (array3 != null && array3.Length != 2)
		{
			TiffThrowHelper.ThrowImageFormatException("Invalid YCbCrSubsampling, expected 2 values.");
		}
		if (array3 != null && array3[1] > array3[0])
		{
			TiffThrowHelper.ThrowImageFormatException("ChromaSubsampleVert shall always be less than or equal to ChromaSubsampleHoriz.");
		}
		if (exifProfile.TryGetValue(ExifTag.StripRowCounts, out IExifValue<uint[]> _))
		{
			TiffThrowHelper.ThrowNotSupported("Variable-sized strips are not supported.");
		}
		if (exifProfile.TryGetValue(ExifTag.PlanarConfiguration, out IExifValue<ushort> exifValue5))
		{
			options.PlanarConfiguration = (TiffPlanarConfiguration)exifValue5.Value;
		}
		else
		{
			options.PlanarConfiguration = TiffPlanarConfiguration.Chunky;
		}
		options.Predictor = frameMetadata.Predictor ?? TiffPredictor.None;
		options.PhotometricInterpretation = frameMetadata.PhotometricInterpretation ?? TiffPhotometricInterpretation.Rgb;
		options.SampleFormat = tiffSampleFormat ?? TiffSampleFormat.UnsignedInteger;
		options.BitsPerPixel = (int)(frameMetadata.BitsPerPixel.HasValue ? frameMetadata.BitsPerPixel.Value : TiffBitsPerPixel.Bit24);
		options.BitsPerSample = frameMetadata.BitsPerSample ?? new TiffBitsPerSample(0, 0, 0, 0);
		if (exifProfile.TryGetValue(ExifTag.ReferenceBlackWhite, out IExifValue<Rational[]> exifValue6))
		{
			options.ReferenceBlackAndWhite = exifValue6.Value;
		}
		if (exifProfile.TryGetValue(ExifTag.YCbCrCoefficients, out IExifValue<Rational[]> exifValue7))
		{
			options.YcbcrCoefficients = exifValue7.Value;
		}
		if (exifProfile.TryGetValue(ExifTag.YCbCrSubsampling, out IExifValue<ushort[]> exifValue8))
		{
			options.YcbcrSubSampling = exifValue8.Value;
		}
		options.FillOrder = tiffFillOrder;
		if (exifProfile.TryGetValue(ExifTag.JPEGTables, out IExifValue<byte[]> exifValue9))
		{
			options.JpegTables = exifValue9.Value;
		}
		if (exifProfile.TryGetValue(ExifTag.JPEGInterchangeFormat, out IExifValue<uint> exifValue10))
		{
			options.OldJpegCompressionStartOfImageMarker = exifValue10.Value;
		}
		options.ParseCompression(frameMetadata.Compression, exifProfile);
		options.ParseColorType(exifProfile);
		return VerifyRequiredFieldsArePresent(exifProfile, frameMetadata, options.PlanarConfiguration);
	}

	private static bool VerifyRequiredFieldsArePresent(ExifProfile exifProfile, TiffFrameMetadata frameMetadata, TiffPlanarConfiguration planarConfiguration)
	{
		bool result = false;
		if (exifProfile.GetValueInternal(ExifTag.TileWidth) != null || exifProfile.GetValueInternal(ExifTag.TileLength) != null)
		{
			if (planarConfiguration == TiffPlanarConfiguration.Planar && exifProfile.GetValueInternal(ExifTag.TileOffsets) == null)
			{
				TiffThrowHelper.ThrowImageFormatException("TileOffsets are missing and are required for decoding the TIFF image!");
			}
			if (planarConfiguration == TiffPlanarConfiguration.Chunky && exifProfile.GetValueInternal(ExifTag.TileOffsets) == null && exifProfile.GetValueInternal(ExifTag.StripOffsets) == null)
			{
				TiffThrowHelper.ThrowImageFormatException("TileOffsets are missing and are required for decoding the TIFF image!");
			}
			if (exifProfile.GetValueInternal(ExifTag.TileWidth) == null)
			{
				TiffThrowHelper.ThrowImageFormatException("TileWidth are missing and are required for decoding the TIFF image!");
			}
			if (exifProfile.GetValueInternal(ExifTag.TileLength) == null)
			{
				TiffThrowHelper.ThrowImageFormatException("TileLength are missing and are required for decoding the TIFF image!");
			}
			result = true;
		}
		else
		{
			if (exifProfile.GetValueInternal(ExifTag.StripOffsets) == null)
			{
				TiffThrowHelper.ThrowImageFormatException("StripOffsets are missing and are required for decoding the TIFF image!");
			}
			if (exifProfile.GetValueInternal(ExifTag.StripByteCounts) == null)
			{
				TiffThrowHelper.ThrowImageFormatException("StripByteCounts are missing and are required for decoding the TIFF image!");
			}
		}
		if (!frameMetadata.BitsPerPixel.HasValue && !IsBiColorCompression(frameMetadata.Compression))
		{
			TiffThrowHelper.ThrowNotSupported("The TIFF BitsPerSample entry is missing which is required to decode the image!");
		}
		return result;
	}

	private static void ParseColorType(this TiffDecoderCore options, ExifProfile exifProfile)
	{
		switch (options.PhotometricInterpretation)
		{
		case TiffPhotometricInterpretation.WhiteIsZero:
		{
			if (options.BitsPerSample.Channels != 1)
			{
				TiffThrowHelper.ThrowNotSupported("The number of samples in the TIFF BitsPerSample entry is not supported.");
			}
			ushort channel = options.BitsPerSample.Channel0;
			if (channel > 32)
			{
				TiffThrowHelper.ThrowNotSupported("Bits per sample is not supported.");
			}
			switch (channel)
			{
			case 32:
				if (options.SampleFormat == TiffSampleFormat.Float)
				{
					options.ColorType = TiffColorType.WhiteIsZero32Float;
				}
				else
				{
					options.ColorType = TiffColorType.WhiteIsZero32;
				}
				break;
			case 24:
				options.ColorType = TiffColorType.WhiteIsZero24;
				break;
			case 16:
				options.ColorType = TiffColorType.WhiteIsZero16;
				break;
			case 8:
				options.ColorType = TiffColorType.WhiteIsZero8;
				break;
			case 4:
				options.ColorType = TiffColorType.WhiteIsZero4;
				break;
			case 1:
				options.ColorType = TiffColorType.WhiteIsZero1;
				break;
			default:
				options.ColorType = TiffColorType.WhiteIsZero;
				break;
			}
			break;
		}
		case TiffPhotometricInterpretation.BlackIsZero:
		{
			if (options.BitsPerSample.Channels != 1)
			{
				TiffThrowHelper.ThrowNotSupported("The number of samples in the TIFF BitsPerSample entry is not supported.");
			}
			ushort channel2 = options.BitsPerSample.Channel0;
			if (channel2 > 32)
			{
				TiffThrowHelper.ThrowNotSupported("Bits per sample is not supported.");
			}
			switch (channel2)
			{
			case 32:
				if (options.SampleFormat == TiffSampleFormat.Float)
				{
					options.ColorType = TiffColorType.BlackIsZero32Float;
				}
				else
				{
					options.ColorType = TiffColorType.BlackIsZero32;
				}
				break;
			case 24:
				options.ColorType = TiffColorType.BlackIsZero24;
				break;
			case 16:
				options.ColorType = TiffColorType.BlackIsZero16;
				break;
			case 8:
				options.ColorType = TiffColorType.BlackIsZero8;
				break;
			case 4:
				options.ColorType = TiffColorType.BlackIsZero4;
				break;
			case 1:
				options.ColorType = TiffColorType.BlackIsZero1;
				break;
			default:
				options.ColorType = TiffColorType.BlackIsZero;
				break;
			}
			break;
		}
		case TiffPhotometricInterpretation.Rgb:
		{
			TiffBitsPerSample bitsPerSample = options.BitsPerSample;
			byte channels = bitsPerSample.Channels;
			if ((uint)(channels - 3) > 1u)
			{
				TiffThrowHelper.ThrowNotSupported("The number of samples in the TIFF BitsPerSample entry is not supported.");
			}
			if ((bitsPerSample.Channels == 3 && (bitsPerSample.Channel0 != bitsPerSample.Channel1 || bitsPerSample.Channel1 != bitsPerSample.Channel2)) || (bitsPerSample.Channels == 4 && (bitsPerSample.Channel0 != bitsPerSample.Channel1 || bitsPerSample.Channel1 != bitsPerSample.Channel2 || bitsPerSample.Channel2 != bitsPerSample.Channel3)))
			{
				TiffThrowHelper.ThrowNotSupported("Only BitsPerSample with equal bits per channel are supported.");
			}
			if (options.PlanarConfiguration == TiffPlanarConfiguration.Chunky)
			{
				switch (options.BitsPerSample.Channel0)
				{
				case 32:
					if (options.SampleFormat == TiffSampleFormat.Float)
					{
						options.ColorType = ((options.BitsPerSample.Channels == 3) ? TiffColorType.RgbFloat323232 : TiffColorType.RgbaFloat32323232);
					}
					else
					{
						options.ColorType = ((options.BitsPerSample.Channels == 3) ? TiffColorType.Rgb323232 : TiffColorType.Rgba32323232);
					}
					break;
				case 24:
					options.ColorType = ((options.BitsPerSample.Channels == 3) ? TiffColorType.Rgb242424 : TiffColorType.Rgba24242424);
					break;
				case 16:
					options.ColorType = ((options.BitsPerSample.Channels == 3) ? TiffColorType.Rgb161616 : TiffColorType.Rgba16161616);
					break;
				case 14:
					options.ColorType = ((options.BitsPerSample.Channels == 3) ? TiffColorType.Rgb141414 : TiffColorType.Rgba14141414);
					break;
				case 12:
					options.ColorType = ((options.BitsPerSample.Channels == 3) ? TiffColorType.Rgb121212 : TiffColorType.Rgba12121212);
					break;
				case 10:
					options.ColorType = ((options.BitsPerSample.Channels == 3) ? TiffColorType.Rgb101010 : TiffColorType.Rgba10101010);
					break;
				case 8:
					options.ColorType = ((options.BitsPerSample.Channels == 3) ? TiffColorType.Rgb888 : TiffColorType.Rgba8888);
					break;
				case 6:
					options.ColorType = ((options.BitsPerSample.Channels == 3) ? TiffColorType.Rgb666 : TiffColorType.Rgba6666);
					break;
				case 5:
					options.ColorType = ((options.BitsPerSample.Channels == 3) ? TiffColorType.Rgb555 : TiffColorType.Rgba5555);
					break;
				case 4:
					options.ColorType = ((options.BitsPerSample.Channels == 3) ? TiffColorType.Rgb444 : TiffColorType.Rgba4444);
					break;
				case 3:
					options.ColorType = ((options.BitsPerSample.Channels == 3) ? TiffColorType.Rgb333 : TiffColorType.Rgba3333);
					break;
				case 2:
					options.ColorType = ((options.BitsPerSample.Channels == 3) ? TiffColorType.Rgb222 : TiffColorType.Rgba2222);
					break;
				default:
					TiffThrowHelper.ThrowNotSupported("Bits per sample is not supported.");
					break;
				}
			}
			else
			{
				switch (options.BitsPerSample.Channel0)
				{
				case 32:
					options.ColorType = ((options.BitsPerSample.Channels == 3) ? TiffColorType.Rgb323232Planar : TiffColorType.Rgba32323232Planar);
					break;
				case 24:
					options.ColorType = ((options.BitsPerSample.Channels == 3) ? TiffColorType.Rgb242424Planar : TiffColorType.Rgba24242424Planar);
					break;
				case 16:
					options.ColorType = ((options.BitsPerSample.Channels == 3) ? TiffColorType.Rgb161616Planar : TiffColorType.Rgba16161616Planar);
					break;
				default:
					options.ColorType = ((options.BitsPerSample.Channels == 3) ? TiffColorType.Rgb888Planar : TiffColorType.Rgba8888Planar);
					break;
				}
			}
			break;
		}
		case TiffPhotometricInterpretation.PaletteColor:
		{
			if (exifProfile.TryGetValue(ExifTag.ColorMap, out IExifValue<ushort[]> exifValue))
			{
				options.ColorMap = exifValue.Value;
				if (options.BitsPerSample.Channels != 1)
				{
					TiffThrowHelper.ThrowNotSupported("The number of samples in the TIFF BitsPerSample entry is not supported.");
				}
				options.ColorType = TiffColorType.PaletteColor;
			}
			else
			{
				TiffThrowHelper.ThrowNotSupported("The TIFF ColorMap entry is missing for a palette color image.");
			}
			break;
		}
		case TiffPhotometricInterpretation.YCbCr:
		{
			if (exifProfile.TryGetValue(ExifTag.ColorMap, out IExifValue<ushort[]> exifValue2))
			{
				options.ColorMap = exifValue2.Value;
			}
			if (options.BitsPerSample.Channels != 3)
			{
				TiffThrowHelper.ThrowNotSupported("The number of samples in the TIFF BitsPerSample entry is not supported for YCbCr images.");
			}
			if (options.BitsPerSample.Channel0 != 8)
			{
				TiffThrowHelper.ThrowNotSupported("Only 8 bits per channel is supported for YCbCr images.");
			}
			options.ColorType = ((options.PlanarConfiguration == TiffPlanarConfiguration.Chunky) ? TiffColorType.YCbCr : TiffColorType.YCbCrPlanar);
			break;
		}
		case TiffPhotometricInterpretation.CieLab:
			if (options.BitsPerSample.Channels != 3)
			{
				TiffThrowHelper.ThrowNotSupported("The number of samples in the TIFF BitsPerSample entry is not supported for CieLab images.");
			}
			if (options.BitsPerSample.Channel0 != 8)
			{
				TiffThrowHelper.ThrowNotSupported("Only 8 bits per channel is supported for CieLab images.");
			}
			options.ColorType = ((options.PlanarConfiguration == TiffPlanarConfiguration.Chunky) ? TiffColorType.CieLab : TiffColorType.CieLabPlanar);
			break;
		case TiffPhotometricInterpretation.Separated:
			if (options.BitsPerSample.Channels != 4)
			{
				TiffThrowHelper.ThrowNotSupported("The number of samples in the TIFF BitsPerSample entry is not supported for CMYK images.");
			}
			if (options.BitsPerSample.Channel0 != 8)
			{
				TiffThrowHelper.ThrowNotSupported("Only 8 bits per channel is supported for CMYK images.");
			}
			if (exifProfile.GetValueInternal(ExifTag.InkNames) != null)
			{
				TiffThrowHelper.ThrowNotSupported("The custom ink name strings are not supported for CMYK images.");
			}
			options.ColorType = TiffColorType.Cmyk;
			break;
		default:
			TiffThrowHelper.ThrowNotSupported($"The specified TIFF photometric interpretation is not supported: {options.PhotometricInterpretation}");
			break;
		}
	}

	private static void ParseCompression(this TiffDecoderCore options, TiffCompression? compression, ExifProfile exifProfile)
	{
		switch (compression ?? TiffCompression.None)
		{
		case TiffCompression.None:
			options.CompressionType = TiffDecoderCompressionType.None;
			break;
		case TiffCompression.PackBits:
			options.CompressionType = TiffDecoderCompressionType.PackBits;
			break;
		case TiffCompression.Deflate:
		case TiffCompression.OldDeflate:
			options.CompressionType = TiffDecoderCompressionType.Deflate;
			break;
		case TiffCompression.Lzw:
			options.CompressionType = TiffDecoderCompressionType.Lzw;
			break;
		case TiffCompression.CcittGroup3Fax:
		{
			options.CompressionType = TiffDecoderCompressionType.T4;
			if (exifProfile.TryGetValue(ExifTag.T4Options, out IExifValue<uint> exifValue2))
			{
				options.FaxCompressionOptions = (FaxCompressionOptions)exifValue2.Value;
			}
			else
			{
				options.FaxCompressionOptions = FaxCompressionOptions.None;
			}
			options.BitsPerSample = new TiffBitsPerSample(1, 0, 0, 0);
			options.BitsPerPixel = 1;
			break;
		}
		case TiffCompression.CcittGroup4Fax:
		{
			options.CompressionType = TiffDecoderCompressionType.T6;
			if (exifProfile.TryGetValue(ExifTag.T4Options, out IExifValue<uint> exifValue))
			{
				options.FaxCompressionOptions = (FaxCompressionOptions)exifValue.Value;
			}
			else
			{
				options.FaxCompressionOptions = FaxCompressionOptions.None;
			}
			options.BitsPerSample = new TiffBitsPerSample(1, 0, 0, 0);
			options.BitsPerPixel = 1;
			break;
		}
		case TiffCompression.Ccitt1D:
			options.CompressionType = TiffDecoderCompressionType.HuffmanRle;
			options.BitsPerSample = new TiffBitsPerSample(1, 0, 0, 0);
			options.BitsPerPixel = 1;
			break;
		case TiffCompression.OldJpeg:
			if (!options.OldJpegCompressionStartOfImageMarker.HasValue)
			{
				TiffThrowHelper.ThrowNotSupported("Missing SOI marker offset for tiff with old jpeg compression");
			}
			if (options.PlanarConfiguration == TiffPlanarConfiguration.Planar)
			{
				TiffThrowHelper.ThrowNotSupported("Old Jpeg compression is not supported with planar configuration");
			}
			options.CompressionType = TiffDecoderCompressionType.OldJpeg;
			if (options.PhotometricInterpretation == TiffPhotometricInterpretation.YCbCr)
			{
				options.PhotometricInterpretation = TiffPhotometricInterpretation.Rgb;
				options.ColorType = TiffColorType.Rgb;
			}
			break;
		case TiffCompression.Jpeg:
			options.CompressionType = TiffDecoderCompressionType.Jpeg;
			if (options.PhotometricInterpretation == TiffPhotometricInterpretation.YCbCr && options.JpegTables == null)
			{
				options.PhotometricInterpretation = TiffPhotometricInterpretation.Rgb;
				options.ColorType = TiffColorType.Rgb;
			}
			break;
		case TiffCompression.Webp:
			options.CompressionType = TiffDecoderCompressionType.Webp;
			break;
		default:
			TiffThrowHelper.ThrowNotSupported($"The specified TIFF compression format '{compression}' is not supported");
			break;
		}
	}

	private static bool IsBiColorCompression(TiffCompression? compression)
	{
		bool flag;
		if (compression.HasValue)
		{
			TiffCompression valueOrDefault = compression.GetValueOrDefault();
			if (valueOrDefault - 2 <= TiffCompression.Ccitt1D)
			{
				flag = true;
				goto IL_001d;
			}
		}
		flag = false;
		goto IL_001d;
		IL_001d:
		if (flag)
		{
			return true;
		}
		return false;
	}
}
