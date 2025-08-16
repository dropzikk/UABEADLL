using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp.Common.Helpers;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Metadata.Profiles.Icc;
using SixLabors.ImageSharp.Metadata.Profiles.Iptc;
using SixLabors.ImageSharp.Metadata.Profiles.Xmp;

namespace SixLabors.ImageSharp.Formats.Tiff;

internal static class TiffDecoderMetadataCreator
{
	public static ImageMetadata Create(List<ImageFrameMetadata> frames, bool ignoreMetadata, ByteOrder byteOrder, bool isBigTiff)
	{
		if (frames.Count < 1)
		{
			TiffThrowHelper.ThrowImageFormatException("Expected at least one frame.");
		}
		ImageMetadata result = Create(byteOrder, isBigTiff, frames[0].ExifProfile);
		if (!ignoreMetadata)
		{
			for (int i = 0; i < frames.Count; i++)
			{
				ImageFrameMetadata imageFrameMetadata = frames[i];
				if (TryGetIptc(imageFrameMetadata.ExifProfile.Values, out var iptcBytes))
				{
					imageFrameMetadata.IptcProfile = new IptcProfile(iptcBytes);
				}
				if (imageFrameMetadata.ExifProfile.TryGetValue(ExifTag.XMP, out IExifValue<byte[]> exifValue))
				{
					imageFrameMetadata.XmpProfile = new XmpProfile(exifValue.Value);
				}
				if (imageFrameMetadata.ExifProfile.TryGetValue(ExifTag.IccProfile, out IExifValue<byte[]> exifValue2))
				{
					imageFrameMetadata.IccProfile = new IccProfile(exifValue2.Value);
				}
			}
		}
		return result;
	}

	private static ImageMetadata Create(ByteOrder byteOrder, bool isBigTiff, ExifProfile exifProfile)
	{
		ImageMetadata imageMetadata = new ImageMetadata();
		SetResolution(imageMetadata, exifProfile);
		TiffMetadata tiffMetadata = imageMetadata.GetTiffMetadata();
		tiffMetadata.ByteOrder = byteOrder;
		tiffMetadata.FormatType = (isBigTiff ? TiffFormatType.BigTIFF : TiffFormatType.Default);
		return imageMetadata;
	}

	private static void SetResolution(ImageMetadata imageMetaData, ExifProfile exifProfile)
	{
		imageMetaData.ResolutionUnits = ((exifProfile == null) ? PixelResolutionUnit.PixelsPerInch : UnitConverter.ExifProfileToResolutionUnit(exifProfile));
		if (exifProfile != null)
		{
			if (exifProfile.TryGetValue(ExifTag.XResolution, out IExifValue<Rational> exifValue))
			{
				imageMetaData.HorizontalResolution = exifValue.Value.ToDouble();
			}
			if (exifProfile.TryGetValue(ExifTag.YResolution, out IExifValue<Rational> exifValue2))
			{
				imageMetaData.VerticalResolution = exifValue2.Value.ToDouble();
			}
		}
	}

	private static bool TryGetIptc(IReadOnlyList<IExifValue> exifValues, out byte[] iptcBytes)
	{
		iptcBytes = null;
		IExifValue exifValue = exifValues.FirstOrDefault((IExifValue f) => f.Tag == ExifTag.IPTC);
		if (exifValue != null)
		{
			ExifDataType dataType = exifValue.DataType;
			if ((dataType == ExifDataType.Byte || dataType == ExifDataType.Undefined) ? true : false)
			{
				iptcBytes = (byte[])exifValue.GetValue();
				return true;
			}
			if (exifValue.DataType == ExifDataType.Long)
			{
				uint[] array = (uint[])exifValue.GetValue();
				iptcBytes = new byte[array.Length * 4];
				Buffer.BlockCopy(array, 0, iptcBytes, 0, array.Length * 4);
				if (iptcBytes[0] == 28)
				{
					return true;
				}
				if (iptcBytes[3] != 28)
				{
					return false;
				}
				Span<byte> span = iptcBytes.AsSpan();
				Span<byte> destination = stackalloc byte[4];
				for (int i = 0; i < iptcBytes.Length; i += 4)
				{
					span.Slice(i, 4).CopyTo(destination);
					iptcBytes[i] = destination[3];
					iptcBytes[i + 1] = destination[2];
					iptcBytes[i + 2] = destination[1];
					iptcBytes[i + 3] = destination[0];
				}
				return true;
			}
		}
		return false;
	}
}
