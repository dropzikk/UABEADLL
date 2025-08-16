using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

public abstract class ExifTag : IEquatable<ExifTag>
{
	private readonly ushort value;

	public static ExifTag<byte> FaxProfile { get; } = new ExifTag<byte>(ExifTagValue.FaxProfile);

	public static ExifTag<byte> ModeNumber { get; } = new ExifTag<byte>(ExifTagValue.ModeNumber);

	public static ExifTag<byte> GPSAltitudeRef { get; } = new ExifTag<byte>(ExifTagValue.GPSAltitudeRef);

	public static ExifTag<byte[]> ClipPath => new ExifTag<byte[]>(ExifTagValue.ClipPath);

	public static ExifTag<byte[]> VersionYear => new ExifTag<byte[]>(ExifTagValue.VersionYear);

	public static ExifTag<byte[]> XMP => new ExifTag<byte[]>(ExifTagValue.XMP);

	public static ExifTag<byte[]> IPTC => new ExifTag<byte[]>(ExifTagValue.IPTC);

	public static ExifTag<byte[]> IccProfile => new ExifTag<byte[]>(ExifTagValue.IccProfile);

	public static ExifTag<byte[]> CFAPattern2 => new ExifTag<byte[]>(ExifTagValue.CFAPattern2);

	public static ExifTag<byte[]> TIFFEPStandardID => new ExifTag<byte[]>(ExifTagValue.TIFFEPStandardID);

	public static ExifTag<byte[]> GPSVersionID => new ExifTag<byte[]>(ExifTagValue.GPSVersionID);

	public static ExifTag<double[]> PixelScale { get; } = new ExifTag<double[]>(ExifTagValue.PixelScale);

	public static ExifTag<double[]> IntergraphMatrix { get; } = new ExifTag<double[]>(ExifTagValue.IntergraphMatrix);

	public static ExifTag<double[]> ModelTiePoint { get; } = new ExifTag<double[]>(ExifTagValue.ModelTiePoint);

	public static ExifTag<double[]> ModelTransform { get; } = new ExifTag<double[]>(ExifTagValue.ModelTransform);

	public static ExifTag<EncodedString> UserComment { get; } = new ExifTag<EncodedString>(ExifTagValue.UserComment);

	public static ExifTag<EncodedString> GPSProcessingMethod { get; } = new ExifTag<EncodedString>(ExifTagValue.GPSProcessingMethod);

	public static ExifTag<EncodedString> GPSAreaInformation { get; } = new ExifTag<EncodedString>(ExifTagValue.GPSAreaInformation);

	public static ExifTag<uint> SubfileType { get; } = new ExifTag<uint>(ExifTagValue.SubfileType);

	public static ExifTag<uint> SubIFDOffset { get; } = new ExifTag<uint>(ExifTagValue.SubIFDOffset);

	public static ExifTag<uint> GPSIFDOffset { get; } = new ExifTag<uint>(ExifTagValue.GPSIFDOffset);

	public static ExifTag<uint> T4Options { get; } = new ExifTag<uint>(ExifTagValue.T4Options);

	public static ExifTag<uint> T6Options { get; } = new ExifTag<uint>(ExifTagValue.T6Options);

	public static ExifTag<uint> XClipPathUnits { get; } = new ExifTag<uint>(ExifTagValue.XClipPathUnits);

	public static ExifTag<uint> YClipPathUnits { get; } = new ExifTag<uint>(ExifTagValue.YClipPathUnits);

	public static ExifTag<uint> ProfileType { get; } = new ExifTag<uint>(ExifTagValue.ProfileType);

	public static ExifTag<uint> CodingMethods { get; } = new ExifTag<uint>(ExifTagValue.CodingMethods);

	public static ExifTag<uint> T82ptions { get; } = new ExifTag<uint>(ExifTagValue.T82ptions);

	public static ExifTag<uint> JPEGInterchangeFormat { get; } = new ExifTag<uint>(ExifTagValue.JPEGInterchangeFormat);

	public static ExifTag<uint> JPEGInterchangeFormatLength { get; } = new ExifTag<uint>(ExifTagValue.JPEGInterchangeFormatLength);

	public static ExifTag<uint> MDFileTag { get; } = new ExifTag<uint>(ExifTagValue.MDFileTag);

	public static ExifTag<uint> StandardOutputSensitivity { get; } = new ExifTag<uint>(ExifTagValue.StandardOutputSensitivity);

	public static ExifTag<uint> RecommendedExposureIndex { get; } = new ExifTag<uint>(ExifTagValue.RecommendedExposureIndex);

	public static ExifTag<uint> ISOSpeed { get; } = new ExifTag<uint>(ExifTagValue.ISOSpeed);

	public static ExifTag<uint> ISOSpeedLatitudeyyy { get; } = new ExifTag<uint>(ExifTagValue.ISOSpeedLatitudeyyy);

	public static ExifTag<uint> ISOSpeedLatitudezzz { get; } = new ExifTag<uint>(ExifTagValue.ISOSpeedLatitudezzz);

	public static ExifTag<uint> FaxRecvParams { get; } = new ExifTag<uint>(ExifTagValue.FaxRecvParams);

	public static ExifTag<uint> FaxRecvTime { get; } = new ExifTag<uint>(ExifTagValue.FaxRecvTime);

	public static ExifTag<uint> ImageNumber { get; } = new ExifTag<uint>(ExifTagValue.ImageNumber);

	public static ExifTag<uint[]> FreeOffsets { get; } = new ExifTag<uint[]>(ExifTagValue.FreeOffsets);

	public static ExifTag<uint[]> FreeByteCounts { get; } = new ExifTag<uint[]>(ExifTagValue.FreeByteCounts);

	public static ExifTag<uint[]> ColorResponseUnit { get; } = new ExifTag<uint[]>(ExifTagValue.ColorResponseUnit);

	public static ExifTag<uint[]> SMinSampleValue { get; } = new ExifTag<uint[]>(ExifTagValue.SMinSampleValue);

	public static ExifTag<uint[]> SMaxSampleValue { get; } = new ExifTag<uint[]>(ExifTagValue.SMaxSampleValue);

	public static ExifTag<uint[]> JPEGQTables { get; } = new ExifTag<uint[]>(ExifTagValue.JPEGQTables);

	public static ExifTag<uint[]> JPEGDCTables { get; } = new ExifTag<uint[]>(ExifTagValue.JPEGDCTables);

	public static ExifTag<uint[]> JPEGACTables { get; } = new ExifTag<uint[]>(ExifTagValue.JPEGACTables);

	public static ExifTag<uint[]> StripRowCounts { get; } = new ExifTag<uint[]>(ExifTagValue.StripRowCounts);

	public static ExifTag<uint[]> IntergraphRegisters { get; } = new ExifTag<uint[]>(ExifTagValue.IntergraphRegisters);

	public static ExifTag<uint[]> SubIFDs { get; } = new ExifTag<uint[]>(ExifTagValue.SubIFDs);

	public static ExifTag<Number> ImageWidth { get; } = new ExifTag<Number>(ExifTagValue.ImageWidth);

	public static ExifTag<Number> ImageLength { get; } = new ExifTag<Number>(ExifTagValue.ImageLength);

	public static ExifTag<Number> RowsPerStrip { get; } = new ExifTag<Number>(ExifTagValue.RowsPerStrip);

	public static ExifTag<Number> TileWidth { get; } = new ExifTag<Number>(ExifTagValue.TileWidth);

	public static ExifTag<Number> TileLength { get; } = new ExifTag<Number>(ExifTagValue.TileLength);

	public static ExifTag<Number> BadFaxLines { get; } = new ExifTag<Number>(ExifTagValue.BadFaxLines);

	public static ExifTag<Number> ConsecutiveBadFaxLines { get; } = new ExifTag<Number>(ExifTagValue.ConsecutiveBadFaxLines);

	public static ExifTag<Number> PixelXDimension { get; } = new ExifTag<Number>(ExifTagValue.PixelXDimension);

	public static ExifTag<Number> PixelYDimension { get; } = new ExifTag<Number>(ExifTagValue.PixelYDimension);

	public static ExifTag<Number[]> StripOffsets { get; } = new ExifTag<Number[]>(ExifTagValue.StripOffsets);

	public static ExifTag<Number[]> StripByteCounts { get; } = new ExifTag<Number[]>(ExifTagValue.StripByteCounts);

	public static ExifTag<Number[]> TileByteCounts { get; } = new ExifTag<Number[]>(ExifTagValue.TileByteCounts);

	public static ExifTag<Number[]> TileOffsets { get; } = new ExifTag<Number[]>(ExifTagValue.TileOffsets);

	public static ExifTag<Number[]> ImageLayer { get; } = new ExifTag<Number[]>(ExifTagValue.ImageLayer);

	public static ExifTag<Rational> XPosition { get; } = new ExifTag<Rational>(ExifTagValue.XPosition);

	public static ExifTag<Rational> YPosition { get; } = new ExifTag<Rational>(ExifTagValue.YPosition);

	public static ExifTag<Rational> XResolution { get; } = new ExifTag<Rational>(ExifTagValue.XResolution);

	public static ExifTag<Rational> YResolution { get; } = new ExifTag<Rational>(ExifTagValue.YResolution);

	public static ExifTag<Rational> BatteryLevel { get; } = new ExifTag<Rational>(ExifTagValue.BatteryLevel);

	public static ExifTag<Rational> ExposureTime { get; } = new ExifTag<Rational>(ExifTagValue.ExposureTime);

	public static ExifTag<Rational> FNumber { get; } = new ExifTag<Rational>(ExifTagValue.FNumber);

	public static ExifTag<Rational> MDScalePixel { get; } = new ExifTag<Rational>(ExifTagValue.MDScalePixel);

	public static ExifTag<Rational> CompressedBitsPerPixel { get; } = new ExifTag<Rational>(ExifTagValue.CompressedBitsPerPixel);

	public static ExifTag<Rational> ApertureValue { get; } = new ExifTag<Rational>(ExifTagValue.ApertureValue);

	public static ExifTag<Rational> MaxApertureValue { get; } = new ExifTag<Rational>(ExifTagValue.MaxApertureValue);

	public static ExifTag<Rational> SubjectDistance { get; } = new ExifTag<Rational>(ExifTagValue.SubjectDistance);

	public static ExifTag<Rational> FocalLength { get; } = new ExifTag<Rational>(ExifTagValue.FocalLength);

	public static ExifTag<Rational> FlashEnergy2 { get; } = new ExifTag<Rational>(ExifTagValue.FlashEnergy2);

	public static ExifTag<Rational> FocalPlaneXResolution2 { get; } = new ExifTag<Rational>(ExifTagValue.FocalPlaneXResolution2);

	public static ExifTag<Rational> FocalPlaneYResolution2 { get; } = new ExifTag<Rational>(ExifTagValue.FocalPlaneYResolution2);

	public static ExifTag<Rational> ExposureIndex2 { get; } = new ExifTag<Rational>(ExifTagValue.ExposureIndex2);

	public static ExifTag<Rational> Humidity { get; } = new ExifTag<Rational>(ExifTagValue.Humidity);

	public static ExifTag<Rational> Pressure { get; } = new ExifTag<Rational>(ExifTagValue.Pressure);

	public static ExifTag<Rational> Acceleration { get; } = new ExifTag<Rational>(ExifTagValue.Acceleration);

	public static ExifTag<Rational> FlashEnergy { get; } = new ExifTag<Rational>(ExifTagValue.FlashEnergy);

	public static ExifTag<Rational> FocalPlaneXResolution { get; } = new ExifTag<Rational>(ExifTagValue.FocalPlaneXResolution);

	public static ExifTag<Rational> FocalPlaneYResolution { get; } = new ExifTag<Rational>(ExifTagValue.FocalPlaneYResolution);

	public static ExifTag<Rational> ExposureIndex { get; } = new ExifTag<Rational>(ExifTagValue.ExposureIndex);

	public static ExifTag<Rational> DigitalZoomRatio { get; } = new ExifTag<Rational>(ExifTagValue.DigitalZoomRatio);

	public static ExifTag<Rational> GPSAltitude { get; } = new ExifTag<Rational>(ExifTagValue.GPSAltitude);

	public static ExifTag<Rational> GPSDOP { get; } = new ExifTag<Rational>(ExifTagValue.GPSDOP);

	public static ExifTag<Rational> GPSSpeed { get; } = new ExifTag<Rational>(ExifTagValue.GPSSpeed);

	public static ExifTag<Rational> GPSTrack { get; } = new ExifTag<Rational>(ExifTagValue.GPSTrack);

	public static ExifTag<Rational> GPSImgDirection { get; } = new ExifTag<Rational>(ExifTagValue.GPSImgDirection);

	public static ExifTag<Rational> GPSDestBearing { get; } = new ExifTag<Rational>(ExifTagValue.GPSDestBearing);

	public static ExifTag<Rational> GPSDestDistance { get; } = new ExifTag<Rational>(ExifTagValue.GPSDestDistance);

	public static ExifTag<Rational> GPSHPositioningError { get; } = new ExifTag<Rational>(ExifTagValue.GPSHPositioningError);

	public static ExifTag<Rational[]> WhitePoint { get; } = new ExifTag<Rational[]>(ExifTagValue.WhitePoint);

	public static ExifTag<Rational[]> PrimaryChromaticities { get; } = new ExifTag<Rational[]>(ExifTagValue.PrimaryChromaticities);

	public static ExifTag<Rational[]> YCbCrCoefficients { get; } = new ExifTag<Rational[]>(ExifTagValue.YCbCrCoefficients);

	public static ExifTag<Rational[]> ReferenceBlackWhite { get; } = new ExifTag<Rational[]>(ExifTagValue.ReferenceBlackWhite);

	public static ExifTag<Rational[]> GPSLatitude { get; } = new ExifTag<Rational[]>(ExifTagValue.GPSLatitude);

	public static ExifTag<Rational[]> GPSLongitude { get; } = new ExifTag<Rational[]>(ExifTagValue.GPSLongitude);

	public static ExifTag<Rational[]> GPSTimestamp { get; } = new ExifTag<Rational[]>(ExifTagValue.GPSTimestamp);

	public static ExifTag<Rational[]> GPSDestLatitude { get; } = new ExifTag<Rational[]>(ExifTagValue.GPSDestLatitude);

	public static ExifTag<Rational[]> GPSDestLongitude { get; } = new ExifTag<Rational[]>(ExifTagValue.GPSDestLongitude);

	public static ExifTag<Rational[]> LensSpecification { get; } = new ExifTag<Rational[]>(ExifTagValue.LensSpecification);

	public static ExifTag<ushort> OldSubfileType { get; } = new ExifTag<ushort>(ExifTagValue.OldSubfileType);

	public static ExifTag<ushort> Compression { get; } = new ExifTag<ushort>(ExifTagValue.Compression);

	public static ExifTag<ushort> PhotometricInterpretation { get; } = new ExifTag<ushort>(ExifTagValue.PhotometricInterpretation);

	public static ExifTag<ushort> Thresholding { get; } = new ExifTag<ushort>(ExifTagValue.Thresholding);

	public static ExifTag<ushort> CellWidth { get; } = new ExifTag<ushort>(ExifTagValue.CellWidth);

	public static ExifTag<ushort> CellLength { get; } = new ExifTag<ushort>(ExifTagValue.CellLength);

	public static ExifTag<ushort> FillOrder { get; } = new ExifTag<ushort>(ExifTagValue.FillOrder);

	public static ExifTag<ushort> Orientation { get; } = new ExifTag<ushort>(ExifTagValue.Orientation);

	public static ExifTag<ushort> SamplesPerPixel { get; } = new ExifTag<ushort>(ExifTagValue.SamplesPerPixel);

	public static ExifTag<ushort> PlanarConfiguration { get; } = new ExifTag<ushort>(ExifTagValue.PlanarConfiguration);

	public static ExifTag<ushort> Predictor { get; } = new ExifTag<ushort>(ExifTagValue.Predictor);

	public static ExifTag<ushort> GrayResponseUnit { get; } = new ExifTag<ushort>(ExifTagValue.GrayResponseUnit);

	public static ExifTag<ushort> ResolutionUnit { get; } = new ExifTag<ushort>(ExifTagValue.ResolutionUnit);

	public static ExifTag<ushort> CleanFaxData { get; } = new ExifTag<ushort>(ExifTagValue.CleanFaxData);

	public static ExifTag<ushort> InkSet { get; } = new ExifTag<ushort>(ExifTagValue.InkSet);

	public static ExifTag<ushort> NumberOfInks { get; } = new ExifTag<ushort>(ExifTagValue.NumberOfInks);

	public static ExifTag<ushort> DotRange { get; } = new ExifTag<ushort>(ExifTagValue.DotRange);

	public static ExifTag<ushort> Indexed { get; } = new ExifTag<ushort>(ExifTagValue.Indexed);

	public static ExifTag<ushort> OPIProxy { get; } = new ExifTag<ushort>(ExifTagValue.OPIProxy);

	public static ExifTag<ushort> JPEGProc { get; } = new ExifTag<ushort>(ExifTagValue.JPEGProc);

	public static ExifTag<ushort> JPEGRestartInterval { get; } = new ExifTag<ushort>(ExifTagValue.JPEGRestartInterval);

	public static ExifTag<ushort> YCbCrPositioning { get; } = new ExifTag<ushort>(ExifTagValue.YCbCrPositioning);

	public static ExifTag<ushort> Rating { get; } = new ExifTag<ushort>(ExifTagValue.Rating);

	public static ExifTag<ushort> RatingPercent { get; } = new ExifTag<ushort>(ExifTagValue.RatingPercent);

	public static ExifTag<ushort> ExposureProgram { get; } = new ExifTag<ushort>(ExifTagValue.ExposureProgram);

	public static ExifTag<ushort> Interlace { get; } = new ExifTag<ushort>(ExifTagValue.Interlace);

	public static ExifTag<ushort> SelfTimerMode { get; } = new ExifTag<ushort>(ExifTagValue.SelfTimerMode);

	public static ExifTag<ushort> SensitivityType { get; } = new ExifTag<ushort>(ExifTagValue.SensitivityType);

	public static ExifTag<ushort> MeteringMode { get; } = new ExifTag<ushort>(ExifTagValue.MeteringMode);

	public static ExifTag<ushort> LightSource { get; } = new ExifTag<ushort>(ExifTagValue.LightSource);

	public static ExifTag<ushort> FocalPlaneResolutionUnit2 { get; } = new ExifTag<ushort>(ExifTagValue.FocalPlaneResolutionUnit2);

	public static ExifTag<ushort> SensingMethod2 { get; } = new ExifTag<ushort>(ExifTagValue.SensingMethod2);

	public static ExifTag<ushort> Flash { get; } = new ExifTag<ushort>(ExifTagValue.Flash);

	public static ExifTag<ushort> ColorSpace { get; } = new ExifTag<ushort>(ExifTagValue.ColorSpace);

	public static ExifTag<ushort> FocalPlaneResolutionUnit { get; } = new ExifTag<ushort>(ExifTagValue.FocalPlaneResolutionUnit);

	public static ExifTag<ushort> SensingMethod { get; } = new ExifTag<ushort>(ExifTagValue.SensingMethod);

	public static ExifTag<ushort> CustomRendered { get; } = new ExifTag<ushort>(ExifTagValue.CustomRendered);

	public static ExifTag<ushort> ExposureMode { get; } = new ExifTag<ushort>(ExifTagValue.ExposureMode);

	public static ExifTag<ushort> WhiteBalance { get; } = new ExifTag<ushort>(ExifTagValue.WhiteBalance);

	public static ExifTag<ushort> FocalLengthIn35mmFilm { get; } = new ExifTag<ushort>(ExifTagValue.FocalLengthIn35mmFilm);

	public static ExifTag<ushort> SceneCaptureType { get; } = new ExifTag<ushort>(ExifTagValue.SceneCaptureType);

	public static ExifTag<ushort> GainControl { get; } = new ExifTag<ushort>(ExifTagValue.GainControl);

	public static ExifTag<ushort> Contrast { get; } = new ExifTag<ushort>(ExifTagValue.Contrast);

	public static ExifTag<ushort> Saturation { get; } = new ExifTag<ushort>(ExifTagValue.Saturation);

	public static ExifTag<ushort> Sharpness { get; } = new ExifTag<ushort>(ExifTagValue.Sharpness);

	public static ExifTag<ushort> SubjectDistanceRange { get; } = new ExifTag<ushort>(ExifTagValue.SubjectDistanceRange);

	public static ExifTag<ushort> GPSDifferential { get; } = new ExifTag<ushort>(ExifTagValue.GPSDifferential);

	public static ExifTag<ushort[]> BitsPerSample { get; } = new ExifTag<ushort[]>(ExifTagValue.BitsPerSample);

	public static ExifTag<ushort[]> MinSampleValue { get; } = new ExifTag<ushort[]>(ExifTagValue.MinSampleValue);

	public static ExifTag<ushort[]> MaxSampleValue { get; } = new ExifTag<ushort[]>(ExifTagValue.MaxSampleValue);

	public static ExifTag<ushort[]> GrayResponseCurve { get; } = new ExifTag<ushort[]>(ExifTagValue.GrayResponseCurve);

	public static ExifTag<ushort[]> ColorMap { get; } = new ExifTag<ushort[]>(ExifTagValue.ColorMap);

	public static ExifTag<ushort[]> ExtraSamples { get; } = new ExifTag<ushort[]>(ExifTagValue.ExtraSamples);

	public static ExifTag<ushort[]> PageNumber { get; } = new ExifTag<ushort[]>(ExifTagValue.PageNumber);

	public static ExifTag<ushort[]> TransferFunction { get; } = new ExifTag<ushort[]>(ExifTagValue.TransferFunction);

	public static ExifTag<ushort[]> HalftoneHints { get; } = new ExifTag<ushort[]>(ExifTagValue.HalftoneHints);

	public static ExifTag<ushort[]> SampleFormat { get; } = new ExifTag<ushort[]>(ExifTagValue.SampleFormat);

	public static ExifTag<ushort[]> TransferRange { get; } = new ExifTag<ushort[]>(ExifTagValue.TransferRange);

	public static ExifTag<ushort[]> DefaultImageColor { get; } = new ExifTag<ushort[]>(ExifTagValue.DefaultImageColor);

	public static ExifTag<ushort[]> JPEGLosslessPredictors { get; } = new ExifTag<ushort[]>(ExifTagValue.JPEGLosslessPredictors);

	public static ExifTag<ushort[]> JPEGPointTransforms { get; } = new ExifTag<ushort[]>(ExifTagValue.JPEGPointTransforms);

	public static ExifTag<ushort[]> YCbCrSubsampling { get; } = new ExifTag<ushort[]>(ExifTagValue.YCbCrSubsampling);

	public static ExifTag<ushort[]> CFARepeatPatternDim { get; } = new ExifTag<ushort[]>(ExifTagValue.CFARepeatPatternDim);

	public static ExifTag<ushort[]> IntergraphPacketData { get; } = new ExifTag<ushort[]>(ExifTagValue.IntergraphPacketData);

	public static ExifTag<ushort[]> ISOSpeedRatings { get; } = new ExifTag<ushort[]>(ExifTagValue.ISOSpeedRatings);

	public static ExifTag<ushort[]> SubjectArea { get; } = new ExifTag<ushort[]>(ExifTagValue.SubjectArea);

	public static ExifTag<ushort[]> SubjectLocation { get; } = new ExifTag<ushort[]>(ExifTagValue.SubjectLocation);

	public static ExifTag<SignedRational> ShutterSpeedValue { get; } = new ExifTag<SignedRational>(ExifTagValue.ShutterSpeedValue);

	public static ExifTag<SignedRational> BrightnessValue { get; } = new ExifTag<SignedRational>(ExifTagValue.BrightnessValue);

	public static ExifTag<SignedRational> ExposureBiasValue { get; } = new ExifTag<SignedRational>(ExifTagValue.ExposureBiasValue);

	public static ExifTag<SignedRational> AmbientTemperature { get; } = new ExifTag<SignedRational>(ExifTagValue.AmbientTemperature);

	public static ExifTag<SignedRational> WaterDepth { get; } = new ExifTag<SignedRational>(ExifTagValue.WaterDepth);

	public static ExifTag<SignedRational> CameraElevationAngle { get; } = new ExifTag<SignedRational>(ExifTagValue.CameraElevationAngle);

	public static ExifTag<SignedRational[]> Decode { get; } = new ExifTag<SignedRational[]>(ExifTagValue.Decode);

	public static ExifTag<short[]> TimeZoneOffset { get; } = new ExifTag<short[]>(ExifTagValue.TimeZoneOffset);

	public static ExifTag<string> ImageDescription { get; } = new ExifTag<string>(ExifTagValue.ImageDescription);

	public static ExifTag<string> Make { get; } = new ExifTag<string>(ExifTagValue.Make);

	public static ExifTag<string> Model { get; } = new ExifTag<string>(ExifTagValue.Model);

	public static ExifTag<string> Software { get; } = new ExifTag<string>(ExifTagValue.Software);

	public static ExifTag<string> DateTime { get; } = new ExifTag<string>(ExifTagValue.DateTime);

	public static ExifTag<string> Artist { get; } = new ExifTag<string>(ExifTagValue.Artist);

	public static ExifTag<string> HostComputer { get; } = new ExifTag<string>(ExifTagValue.HostComputer);

	public static ExifTag<string> Copyright { get; } = new ExifTag<string>(ExifTagValue.Copyright);

	public static ExifTag<string> DocumentName { get; } = new ExifTag<string>(ExifTagValue.DocumentName);

	public static ExifTag<string> PageName { get; } = new ExifTag<string>(ExifTagValue.PageName);

	public static ExifTag<string> InkNames { get; } = new ExifTag<string>(ExifTagValue.InkNames);

	public static ExifTag<string> TargetPrinter { get; } = new ExifTag<string>(ExifTagValue.TargetPrinter);

	public static ExifTag<string> ImageID { get; } = new ExifTag<string>(ExifTagValue.ImageID);

	public static ExifTag<string> MDLabName { get; } = new ExifTag<string>(ExifTagValue.MDLabName);

	public static ExifTag<string> MDSampleInfo { get; } = new ExifTag<string>(ExifTagValue.MDSampleInfo);

	public static ExifTag<string> MDPrepDate { get; } = new ExifTag<string>(ExifTagValue.MDPrepDate);

	public static ExifTag<string> MDPrepTime { get; } = new ExifTag<string>(ExifTagValue.MDPrepTime);

	public static ExifTag<string> MDFileUnits { get; } = new ExifTag<string>(ExifTagValue.MDFileUnits);

	public static ExifTag<string> SEMInfo { get; } = new ExifTag<string>(ExifTagValue.SEMInfo);

	public static ExifTag<string> SpectralSensitivity { get; } = new ExifTag<string>(ExifTagValue.SpectralSensitivity);

	public static ExifTag<string> DateTimeOriginal { get; } = new ExifTag<string>(ExifTagValue.DateTimeOriginal);

	public static ExifTag<string> DateTimeDigitized { get; } = new ExifTag<string>(ExifTagValue.DateTimeDigitized);

	public static ExifTag<string> SubsecTime { get; } = new ExifTag<string>(ExifTagValue.SubsecTime);

	public static ExifTag<string> SubsecTimeOriginal { get; } = new ExifTag<string>(ExifTagValue.SubsecTimeOriginal);

	public static ExifTag<string> SubsecTimeDigitized { get; } = new ExifTag<string>(ExifTagValue.SubsecTimeDigitized);

	public static ExifTag<string> RelatedSoundFile { get; } = new ExifTag<string>(ExifTagValue.RelatedSoundFile);

	public static ExifTag<string> FaxSubaddress { get; } = new ExifTag<string>(ExifTagValue.FaxSubaddress);

	public static ExifTag<string> OffsetTime { get; } = new ExifTag<string>(ExifTagValue.OffsetTime);

	public static ExifTag<string> OffsetTimeOriginal { get; } = new ExifTag<string>(ExifTagValue.OffsetTimeOriginal);

	public static ExifTag<string> OffsetTimeDigitized { get; } = new ExifTag<string>(ExifTagValue.OffsetTimeDigitized);

	public static ExifTag<string> SecurityClassification { get; } = new ExifTag<string>(ExifTagValue.SecurityClassification);

	public static ExifTag<string> ImageHistory { get; } = new ExifTag<string>(ExifTagValue.ImageHistory);

	public static ExifTag<string> ImageUniqueID { get; } = new ExifTag<string>(ExifTagValue.ImageUniqueID);

	public static ExifTag<string> OwnerName { get; } = new ExifTag<string>(ExifTagValue.OwnerName);

	public static ExifTag<string> SerialNumber { get; } = new ExifTag<string>(ExifTagValue.SerialNumber);

	public static ExifTag<string> LensMake { get; } = new ExifTag<string>(ExifTagValue.LensMake);

	public static ExifTag<string> LensModel { get; } = new ExifTag<string>(ExifTagValue.LensModel);

	public static ExifTag<string> LensSerialNumber { get; } = new ExifTag<string>(ExifTagValue.LensSerialNumber);

	public static ExifTag<string> GDALMetadata { get; } = new ExifTag<string>(ExifTagValue.GDALMetadata);

	public static ExifTag<string> GDALNoData { get; } = new ExifTag<string>(ExifTagValue.GDALNoData);

	public static ExifTag<string> GPSLatitudeRef { get; } = new ExifTag<string>(ExifTagValue.InteroperabilityIndex);

	public static ExifTag<string> GPSLongitudeRef { get; } = new ExifTag<string>(ExifTagValue.GPSLongitudeRef);

	public static ExifTag<string> GPSSatellites { get; } = new ExifTag<string>(ExifTagValue.GPSSatellites);

	public static ExifTag<string> GPSStatus { get; } = new ExifTag<string>(ExifTagValue.GPSStatus);

	public static ExifTag<string> GPSMeasureMode { get; } = new ExifTag<string>(ExifTagValue.GPSMeasureMode);

	public static ExifTag<string> GPSSpeedRef { get; } = new ExifTag<string>(ExifTagValue.GPSSpeedRef);

	public static ExifTag<string> GPSTrackRef { get; } = new ExifTag<string>(ExifTagValue.GPSTrackRef);

	public static ExifTag<string> GPSImgDirectionRef { get; } = new ExifTag<string>(ExifTagValue.GPSImgDirectionRef);

	public static ExifTag<string> GPSMapDatum { get; } = new ExifTag<string>(ExifTagValue.GPSMapDatum);

	public static ExifTag<string> GPSDestLatitudeRef { get; } = new ExifTag<string>(ExifTagValue.GPSDestLatitudeRef);

	public static ExifTag<string> GPSDestLongitudeRef { get; } = new ExifTag<string>(ExifTagValue.GPSDestLongitudeRef);

	public static ExifTag<string> GPSDestBearingRef { get; } = new ExifTag<string>(ExifTagValue.GPSDestBearingRef);

	public static ExifTag<string> GPSDestDistanceRef { get; } = new ExifTag<string>(ExifTagValue.GPSDestDistanceRef);

	public static ExifTag<string> GPSDateStamp { get; } = new ExifTag<string>(ExifTagValue.GPSDateStamp);

	public static ExifTag<string> XPTitle => new ExifTag<string>(ExifTagValue.XPTitle);

	public static ExifTag<string> XPComment => new ExifTag<string>(ExifTagValue.XPComment);

	public static ExifTag<string> XPAuthor => new ExifTag<string>(ExifTagValue.XPAuthor);

	public static ExifTag<string> XPKeywords => new ExifTag<string>(ExifTagValue.XPKeywords);

	public static ExifTag<string> XPSubject => new ExifTag<string>(ExifTagValue.XPSubject);

	public static ExifTag<byte[]> JPEGTables { get; } = new ExifTag<byte[]>(ExifTagValue.JPEGTables);

	public static ExifTag<byte[]> OECF { get; } = new ExifTag<byte[]>(ExifTagValue.OECF);

	public static ExifTag<byte[]> ExifVersion { get; } = new ExifTag<byte[]>(ExifTagValue.ExifVersion);

	public static ExifTag<byte[]> ComponentsConfiguration { get; } = new ExifTag<byte[]>(ExifTagValue.ComponentsConfiguration);

	public static ExifTag<byte[]> MakerNote { get; } = new ExifTag<byte[]>(ExifTagValue.MakerNote);

	public static ExifTag<byte[]> FlashpixVersion { get; } = new ExifTag<byte[]>(ExifTagValue.FlashpixVersion);

	public static ExifTag<byte[]> SpatialFrequencyResponse { get; } = new ExifTag<byte[]>(ExifTagValue.SpatialFrequencyResponse);

	public static ExifTag<byte[]> SpatialFrequencyResponse2 { get; } = new ExifTag<byte[]>(ExifTagValue.SpatialFrequencyResponse2);

	public static ExifTag<byte[]> Noise { get; } = new ExifTag<byte[]>(ExifTagValue.Noise);

	public static ExifTag<byte[]> CFAPattern { get; } = new ExifTag<byte[]>(ExifTagValue.CFAPattern);

	public static ExifTag<byte[]> DeviceSettingDescription { get; } = new ExifTag<byte[]>(ExifTagValue.DeviceSettingDescription);

	public static ExifTag<byte[]> ImageSourceData { get; } = new ExifTag<byte[]>(ExifTagValue.ImageSourceData);

	public static ExifTag<byte> FileSource { get; } = new ExifTag<byte>(ExifTagValue.FileSource);

	public static ExifTag<byte> SceneType { get; } = new ExifTag<byte>(ExifTagValue.SceneType);

	internal ExifTag(ushort value)
	{
		this.value = value;
	}

	public static explicit operator ushort(ExifTag tag)
	{
		return tag?.value ?? ushort.MaxValue;
	}

	public static bool operator ==(ExifTag left, ExifTag right)
	{
		return object.Equals(left, right);
	}

	public static bool operator !=(ExifTag left, ExifTag right)
	{
		return !object.Equals(left, right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is ExifTag other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(ExifTag? other)
	{
		if ((object)other == null)
		{
			return false;
		}
		if ((object)this == other)
		{
			return true;
		}
		return value == other.value;
	}

	public override int GetHashCode()
	{
		return value.GetHashCode();
	}

	public override string ToString()
	{
		return ((ExifTagValue)value/*cast due to .constrained prefix*/).ToString();
	}
}
public sealed class ExifTag<TValueType> : ExifTag
{
	internal ExifTag(ExifTagValue value)
		: base((ushort)value)
	{
	}
}
