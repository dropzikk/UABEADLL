using System.Collections.Generic;
using SixLabors.ImageSharp.Common.Helpers;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Metadata.Profiles.Icc;
using SixLabors.ImageSharp.Metadata.Profiles.Iptc;
using SixLabors.ImageSharp.Metadata.Profiles.Xmp;

namespace SixLabors.ImageSharp.Formats.Tiff;

internal class TiffEncoderEntriesCollector
{
	private abstract class BaseProcessor
	{
		protected TiffEncoderEntriesCollector Collector { get; }

		protected BaseProcessor(TiffEncoderEntriesCollector collector)
		{
			Collector = collector;
		}
	}

	private class MetadataProcessor : BaseProcessor
	{
		public MetadataProcessor(TiffEncoderEntriesCollector collector)
			: base(collector)
		{
		}

		public void Process(Image image, bool skipMetadata)
		{
			ProcessProfiles(image.Metadata, skipMetadata);
			if (!skipMetadata)
			{
				ProcessMetadata(image.Metadata.ExifProfile ?? new ExifProfile());
			}
			if (!base.Collector.Entries.Exists((IExifValue t) => t.Tag == ExifTag.Software))
			{
				base.Collector.Add(new ExifString(ExifTagValue.Software)
				{
					Value = "ImageSharp"
				});
			}
		}

		public void Process(ImageFrame frame, bool skipMetadata)
		{
			ProcessProfiles(frame.Metadata, skipMetadata);
			if (!skipMetadata)
			{
				ProcessMetadata(frame.Metadata.ExifProfile ?? new ExifProfile());
			}
			if (!base.Collector.Entries.Exists((IExifValue t) => t.Tag == ExifTag.Software))
			{
				base.Collector.Add(new ExifString(ExifTagValue.Software)
				{
					Value = "ImageSharp"
				});
			}
		}

		private static bool IsPureMetadata(ExifTag tag)
		{
			switch ((ExifTagValue)(ushort)tag)
			{
			case ExifTagValue.DocumentName:
			case ExifTagValue.ImageDescription:
			case ExifTagValue.Make:
			case ExifTagValue.Model:
			case ExifTagValue.Software:
			case ExifTagValue.DateTime:
			case ExifTagValue.Artist:
			case ExifTagValue.HostComputer:
			case ExifTagValue.TargetPrinter:
			case ExifTagValue.XMP:
			case ExifTagValue.Rating:
			case ExifTagValue.RatingPercent:
			case ExifTagValue.ImageID:
			case ExifTagValue.Copyright:
			case ExifTagValue.MDLabName:
			case ExifTagValue.MDSampleInfo:
			case ExifTagValue.MDPrepDate:
			case ExifTagValue.MDPrepTime:
			case ExifTagValue.MDFileUnits:
			case ExifTagValue.SEMInfo:
			case ExifTagValue.XPTitle:
			case ExifTagValue.XPComment:
			case ExifTagValue.XPAuthor:
			case ExifTagValue.XPKeywords:
			case ExifTagValue.XPSubject:
				return true;
			default:
				return false;
			}
		}

		private void ProcessMetadata(ExifProfile exifProfile)
		{
			foreach (IExifValue entry in exifProfile.Values)
			{
				if (entry.DataType == ExifDataType.Ifd)
				{
					continue;
				}
				switch ((ExifTagValue)(ushort)entry.Tag)
				{
				case ExifTagValue.SubIFDs:
				case ExifTagValue.XMP:
				case ExifTagValue.IPTC:
				case ExifTagValue.SubIFDOffset:
				case ExifTagValue.IccProfile:
				case ExifTagValue.GPSIFDOffset:
					continue;
				}
				switch (ExifTags.GetPart(entry.Tag))
				{
				case ExifParts.IfdTags:
					if (!IsPureMetadata(entry.Tag))
					{
						continue;
					}
					break;
				}
				if (!base.Collector.Entries.Exists((IExifValue t) => t.Tag == entry.Tag))
				{
					base.Collector.AddOrReplace(entry.DeepClone());
				}
			}
		}

		private void ProcessProfiles(ImageMetadata imageMetadata, bool skipMetadata)
		{
			ProcessExifProfile(skipMetadata, imageMetadata.ExifProfile);
			ProcessIptcProfile(skipMetadata, imageMetadata.IptcProfile, imageMetadata.ExifProfile);
			ProcessIccProfile(imageMetadata.IccProfile, imageMetadata.ExifProfile);
			ProcessXmpProfile(skipMetadata, imageMetadata.XmpProfile, imageMetadata.ExifProfile);
		}

		private void ProcessProfiles(ImageFrameMetadata frameMetadata, bool skipMetadata)
		{
			ProcessExifProfile(skipMetadata, frameMetadata.ExifProfile);
			ProcessIptcProfile(skipMetadata, frameMetadata.IptcProfile, frameMetadata.ExifProfile);
			ProcessIccProfile(frameMetadata.IccProfile, frameMetadata.ExifProfile);
			ProcessXmpProfile(skipMetadata, frameMetadata.XmpProfile, frameMetadata.ExifProfile);
		}

		private void ProcessExifProfile(bool skipMetadata, ExifProfile exifProfile)
		{
			if (!skipMetadata && exifProfile != null && exifProfile.Parts != 0)
			{
				foreach (IExifValue entry in exifProfile.Values)
				{
					if (!base.Collector.Entries.Exists((IExifValue t) => t.Tag == entry.Tag) && entry.GetValue() != null)
					{
						ExifParts part = ExifTags.GetPart(entry.Tag);
						if (part != 0 && exifProfile.Parts.HasFlag(part))
						{
							base.Collector.AddOrReplace(entry.DeepClone());
						}
					}
				}
				return;
			}
			exifProfile?.RemoveValue(ExifTag.SubIFDOffset);
		}

		private void ProcessIptcProfile(bool skipMetadata, IptcProfile iptcProfile, ExifProfile exifProfile)
		{
			if (!skipMetadata && iptcProfile != null)
			{
				iptcProfile.UpdateData();
				ExifByteArray entry = new ExifByteArray(ExifTagValue.IPTC, ExifDataType.Byte)
				{
					Value = iptcProfile.Data
				};
				base.Collector.AddOrReplace(entry);
			}
			else
			{
				exifProfile?.RemoveValue(ExifTag.IPTC);
			}
		}

		private void ProcessIccProfile(IccProfile iccProfile, ExifProfile exifProfile)
		{
			if (iccProfile != null)
			{
				ExifByteArray entry = new ExifByteArray(ExifTagValue.IccProfile, ExifDataType.Undefined)
				{
					Value = iccProfile.ToByteArray()
				};
				base.Collector.AddOrReplace(entry);
			}
			else
			{
				exifProfile?.RemoveValue(ExifTag.IccProfile);
			}
		}

		private void ProcessXmpProfile(bool skipMetadata, XmpProfile xmpProfile, ExifProfile exifProfile)
		{
			if (!skipMetadata && xmpProfile != null)
			{
				ExifByteArray entry = new ExifByteArray(ExifTagValue.XMP, ExifDataType.Byte)
				{
					Value = xmpProfile.Data
				};
				base.Collector.AddOrReplace(entry);
			}
			else
			{
				exifProfile?.RemoveValue(ExifTag.XMP);
			}
		}
	}

	private class FrameInfoProcessor : BaseProcessor
	{
		public FrameInfoProcessor(TiffEncoderEntriesCollector collector)
			: base(collector)
		{
		}

		public void Process(ImageFrame frame, ImageMetadata imageMetadata)
		{
			base.Collector.AddOrReplace(new ExifLong(ExifTagValue.ImageWidth)
			{
				Value = (uint)frame.Width
			});
			base.Collector.AddOrReplace(new ExifLong(ExifTagValue.ImageLength)
			{
				Value = (uint)frame.Height
			});
			ProcessResolution(imageMetadata);
		}

		private void ProcessResolution(ImageMetadata imageMetadata)
		{
			ExifResolutionValues exifResolutionValues = UnitConverter.GetExifResolutionValues(imageMetadata.ResolutionUnits, imageMetadata.HorizontalResolution, imageMetadata.VerticalResolution);
			base.Collector.AddOrReplace(new ExifShort(ExifTagValue.ResolutionUnit)
			{
				Value = exifResolutionValues.ResolutionUnit
			});
			if (exifResolutionValues.VerticalResolution.HasValue && exifResolutionValues.HorizontalResolution.HasValue)
			{
				base.Collector.AddOrReplace(new ExifRational(ExifTagValue.XResolution)
				{
					Value = new Rational(exifResolutionValues.HorizontalResolution.Value)
				});
				base.Collector.AddOrReplace(new ExifRational(ExifTagValue.YResolution)
				{
					Value = new Rational(exifResolutionValues.VerticalResolution.Value)
				});
			}
		}
	}

	private class ImageFormatProcessor : BaseProcessor
	{
		public ImageFormatProcessor(TiffEncoderEntriesCollector collector)
			: base(collector)
		{
		}

		public void Process(TiffEncoderCore encoder)
		{
			ExifShort entry = new ExifShort(ExifTagValue.PlanarConfiguration)
			{
				Value = 1
			};
			ExifShort entry2 = new ExifShort(ExifTagValue.SamplesPerPixel)
			{
				Value = GetSamplesPerPixel(encoder)
			};
			ushort[] bitsPerSampleValue = GetBitsPerSampleValue(encoder);
			ExifShortArray entry3 = new ExifShortArray(ExifTagValue.BitsPerSample)
			{
				Value = bitsPerSampleValue
			};
			ushort compressionType = GetCompressionType(encoder);
			ExifShort entry4 = new ExifShort(ExifTagValue.Compression)
			{
				Value = compressionType
			};
			ExifShort entry5 = new ExifShort(ExifTagValue.PhotometricInterpretation)
			{
				Value = (ushort)encoder.PhotometricInterpretation.Value
			};
			base.Collector.AddOrReplace(entry);
			base.Collector.AddOrReplace(entry2);
			base.Collector.AddOrReplace(entry3);
			base.Collector.AddOrReplace(entry4);
			base.Collector.AddOrReplace(entry5);
			bool flag = encoder.HorizontalPredictor == TiffPredictor.Horizontal;
			bool flag2;
			if (flag)
			{
				TiffPhotometricInterpretation? photometricInterpretation = encoder.PhotometricInterpretation;
				if (photometricInterpretation.HasValue)
				{
					TiffPhotometricInterpretation valueOrDefault = photometricInterpretation.GetValueOrDefault();
					if (valueOrDefault - 1 <= TiffPhotometricInterpretation.Rgb)
					{
						flag2 = true;
						goto IL_010a;
					}
				}
				flag2 = false;
				goto IL_010a;
			}
			goto IL_010e;
			IL_010e:
			if (flag)
			{
				ExifShort entry6 = new ExifShort(ExifTagValue.Predictor)
				{
					Value = 2
				};
				base.Collector.AddOrReplace(entry6);
			}
			return;
			IL_010a:
			flag = flag2;
			goto IL_010e;
		}

		private static ushort GetSamplesPerPixel(TiffEncoderCore encoder)
		{
			switch (encoder.PhotometricInterpretation)
			{
			case TiffPhotometricInterpretation.WhiteIsZero:
			case TiffPhotometricInterpretation.BlackIsZero:
			case TiffPhotometricInterpretation.PaletteColor:
				return 1;
			default:
				return 3;
			}
		}

		private static ushort[] GetBitsPerSampleValue(TiffEncoderCore encoder)
		{
			switch (encoder.PhotometricInterpretation)
			{
			case TiffPhotometricInterpretation.PaletteColor:
				if (encoder.BitsPerPixel == TiffBitsPerPixel.Bit4)
				{
					return TiffConstants.BitsPerSample4Bit.ToArray();
				}
				return TiffConstants.BitsPerSample8Bit.ToArray();
			case TiffPhotometricInterpretation.Rgb:
				return TiffConstants.BitsPerSampleRgb8Bit.ToArray();
			case TiffPhotometricInterpretation.WhiteIsZero:
				return encoder.BitsPerPixel switch
				{
					TiffBitsPerPixel.Bit1 => TiffConstants.BitsPerSample1Bit.ToArray(), 
					TiffBitsPerPixel.Bit16 => TiffConstants.BitsPerSample16Bit.ToArray(), 
					_ => TiffConstants.BitsPerSample8Bit.ToArray(), 
				};
			case TiffPhotometricInterpretation.BlackIsZero:
				return encoder.BitsPerPixel switch
				{
					TiffBitsPerPixel.Bit1 => TiffConstants.BitsPerSample1Bit.ToArray(), 
					TiffBitsPerPixel.Bit16 => TiffConstants.BitsPerSample16Bit.ToArray(), 
					_ => TiffConstants.BitsPerSample8Bit.ToArray(), 
				};
			default:
				return TiffConstants.BitsPerSampleRgb8Bit.ToArray();
			}
		}

		private static ushort GetCompressionType(TiffEncoderCore encoder)
		{
			bool flag;
			switch (encoder.CompressionType)
			{
			case TiffCompression.Deflate:
				return 8;
			case TiffCompression.PackBits:
				return 32773;
			case TiffCompression.Lzw:
			{
				TiffPhotometricInterpretation? photometricInterpretation = encoder.PhotometricInterpretation;
				if (photometricInterpretation.HasValue)
				{
					TiffPhotometricInterpretation valueOrDefault = photometricInterpretation.GetValueOrDefault();
					if (valueOrDefault - 1 <= TiffPhotometricInterpretation.Rgb)
					{
						flag = true;
						goto IL_0074;
					}
				}
				flag = false;
				goto IL_0074;
			}
			case TiffCompression.CcittGroup3Fax:
				return 3;
			case TiffCompression.CcittGroup4Fax:
				return 4;
			case TiffCompression.Ccitt1D:
				return 2;
			case TiffCompression.Jpeg:
				{
					return 7;
				}
				IL_0074:
				if (flag)
				{
					return 5;
				}
				break;
			}
			return 1;
		}
	}

	private const string SoftwareValue = "ImageSharp";

	public List<IExifValue> Entries { get; } = new List<IExifValue>();

	public void ProcessMetadata(Image image, bool skipMetadata)
	{
		new MetadataProcessor(this).Process(image, skipMetadata);
	}

	public void ProcessMetadata(ImageFrame frame, bool skipMetadata)
	{
		new MetadataProcessor(this).Process(frame, skipMetadata);
	}

	public void ProcessFrameInfo(ImageFrame frame, ImageMetadata imageMetadata)
	{
		new FrameInfoProcessor(this).Process(frame, imageMetadata);
	}

	public void ProcessImageFormat(TiffEncoderCore encoder)
	{
		new ImageFormatProcessor(this).Process(encoder);
	}

	public void AddOrReplace(IExifValue entry)
	{
		int num = Entries.FindIndex((IExifValue t) => t.Tag == entry.Tag);
		if (num >= 0)
		{
			Entries[num] = entry;
		}
		else
		{
			Entries.Add(entry);
		}
	}

	private void Add(IExifValue entry)
	{
		Entries.Add(entry);
	}
}
