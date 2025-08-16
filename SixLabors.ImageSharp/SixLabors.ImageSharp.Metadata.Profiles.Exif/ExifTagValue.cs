namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal enum ExifTagValue
{
	Unknown = 65535,
	SubIFDOffset = 34665,
	GPSIFDOffset = 34853,
	[ExifTagDescription("R98", "Indicates a file conforming to R98 file specification of Recommended Exif Interoperability Rules (ExifR98) or to DCF basic file stipulated by Design Rule for Camera File System.")]
	[ExifTagDescription("THM", "Indicates a file conforming to DCF thumbnail file stipulated by Design rule for Camera File System.")]
	InteroperabilityIndex = 1,
	[ExifTagDescription(0u, "Full-resolution Image")]
	[ExifTagDescription(1u, "Reduced-resolution image")]
	[ExifTagDescription(2u, "Single page of multi-page image")]
	[ExifTagDescription(3u, "Single page of multi-page reduced-resolution image")]
	[ExifTagDescription(4u, "Transparency mask")]
	[ExifTagDescription(5u, "Transparency mask of reduced-resolution image")]
	[ExifTagDescription(6u, "Transparency mask of multi-page image")]
	[ExifTagDescription(7u, "Transparency mask of reduced-resolution multi-page image")]
	[ExifTagDescription(65537u, "Alternate reduced-resolution image ")]
	SubfileType = 254,
	[ExifTagDescription((ushort)1, "Full-resolution Image")]
	[ExifTagDescription((ushort)2, "Reduced-resolution image")]
	[ExifTagDescription((ushort)3, "Single page of multi-page image")]
	OldSubfileType = 255,
	ImageWidth = 256,
	ImageLength = 257,
	BitsPerSample = 258,
	[ExifTagDescription((ushort)1, "Uncompressed")]
	[ExifTagDescription((ushort)2, "CCITT 1D")]
	[ExifTagDescription((ushort)3, "T4/Group 3 Fax")]
	[ExifTagDescription((ushort)4, "T6/Group 4 Fax")]
	[ExifTagDescription((ushort)5, "LZW")]
	[ExifTagDescription((ushort)6, "JPEG (old-style)")]
	[ExifTagDescription((ushort)7, "JPEG")]
	[ExifTagDescription((ushort)8, "Adobe Deflate")]
	[ExifTagDescription((ushort)9, "JBIG B&W")]
	[ExifTagDescription((ushort)10, "JBIG Color")]
	[ExifTagDescription((ushort)99, "JPEG")]
	[ExifTagDescription((ushort)262, "Kodak 262")]
	[ExifTagDescription((ushort)32766, "Next")]
	[ExifTagDescription((ushort)32767, "Sony ARW Compressed")]
	[ExifTagDescription((ushort)32769, "Packed RAW")]
	[ExifTagDescription((ushort)32770, "Samsung SRW Compressed")]
	[ExifTagDescription((ushort)32771, "CCIRLEW")]
	[ExifTagDescription((ushort)32772, "Samsung SRW Compressed 2")]
	[ExifTagDescription((ushort)32773, "PackBits")]
	[ExifTagDescription((ushort)32809, "Thunderscan")]
	[ExifTagDescription((ushort)32867, "Kodak KDC Compressed")]
	[ExifTagDescription((ushort)32895, "IT8CTPAD")]
	[ExifTagDescription((ushort)32896, "IT8LW")]
	[ExifTagDescription((ushort)32897, "IT8MP")]
	[ExifTagDescription((ushort)32898, "IT8BL")]
	[ExifTagDescription((ushort)32908, "PixarFilm")]
	[ExifTagDescription((ushort)32909, "PixarLog")]
	[ExifTagDescription((ushort)32946, "Deflate")]
	[ExifTagDescription((ushort)32947, "DCS")]
	[ExifTagDescription((ushort)34661, "JBIG")]
	[ExifTagDescription((ushort)34676, "SGILog")]
	[ExifTagDescription((ushort)34677, "SGILog24")]
	[ExifTagDescription((ushort)34712, "JPEG 2000")]
	[ExifTagDescription((ushort)34713, "Nikon NEF Compressed")]
	[ExifTagDescription((ushort)34715, "JBIG2 TIFF FX")]
	[ExifTagDescription((ushort)34718, "Microsoft Document Imaging (MDI) Binary Level Codec")]
	[ExifTagDescription((ushort)34719, "Microsoft Document Imaging (MDI) Progressive Transform Codec")]
	[ExifTagDescription((ushort)34720, "Microsoft Document Imaging (MDI) Vector")]
	[ExifTagDescription((ushort)34892, "Lossy JPEG")]
	[ExifTagDescription((ushort)65000, "Kodak DCR Compressed")]
	[ExifTagDescription(ushort.MaxValue, "Pentax PEF Compressed")]
	Compression = 259,
	[ExifTagDescription((ushort)0, "WhiteIsZero")]
	[ExifTagDescription((ushort)1, "BlackIsZero")]
	[ExifTagDescription((ushort)2, "RGB")]
	[ExifTagDescription((ushort)3, "RGB Palette")]
	[ExifTagDescription((ushort)4, "Transparency Mask")]
	[ExifTagDescription((ushort)5, "CMYK")]
	[ExifTagDescription((ushort)6, "YCbCr")]
	[ExifTagDescription((ushort)8, "CIELab")]
	[ExifTagDescription((ushort)9, "ICCLab")]
	[ExifTagDescription((ushort)10, "TULab")]
	[ExifTagDescription((ushort)32803, "Color Filter Array")]
	[ExifTagDescription((ushort)32844, "Pixar LogL")]
	[ExifTagDescription((ushort)32845, "Pixar LogLuv")]
	[ExifTagDescription((ushort)34892, "Linear Raw")]
	PhotometricInterpretation = 262,
	[ExifTagDescription((ushort)1, "No dithering or halftoning")]
	[ExifTagDescription((ushort)2, "Ordered dither or halftone")]
	[ExifTagDescription((ushort)3, "Randomized dither")]
	Thresholding = 263,
	CellWidth = 264,
	CellLength = 265,
	[ExifTagDescription((ushort)1, "Normal")]
	[ExifTagDescription((ushort)2, "Reversed")]
	FillOrder = 266,
	DocumentName = 269,
	ImageDescription = 270,
	Make = 271,
	Model = 272,
	StripOffsets = 273,
	[ExifTagDescription((ushort)1, "Horizontal (normal)")]
	[ExifTagDescription((ushort)2, "Mirror horizontal")]
	[ExifTagDescription((ushort)3, "Rotate 180")]
	[ExifTagDescription((ushort)4, "Mirror vertical")]
	[ExifTagDescription((ushort)5, "Mirror horizontal and rotate 270 CW")]
	[ExifTagDescription((ushort)6, "Rotate 90 CW")]
	[ExifTagDescription((ushort)7, "Mirror horizontal and rotate 90 CW")]
	[ExifTagDescription((ushort)8, "Rotate 270 CW")]
	Orientation = 274,
	SamplesPerPixel = 277,
	RowsPerStrip = 278,
	StripByteCounts = 279,
	MinSampleValue = 280,
	MaxSampleValue = 281,
	XResolution = 282,
	YResolution = 283,
	[ExifTagDescription((ushort)1, "Chunky")]
	[ExifTagDescription((ushort)2, "Planar")]
	PlanarConfiguration = 284,
	PageName = 285,
	XPosition = 286,
	YPosition = 287,
	FreeOffsets = 288,
	FreeByteCounts = 289,
	[ExifTagDescription((ushort)1, "0.1")]
	[ExifTagDescription((ushort)2, "0.001")]
	[ExifTagDescription((ushort)3, "0.0001")]
	[ExifTagDescription((ushort)4, "1e-05")]
	[ExifTagDescription((ushort)5, "1e-06")]
	GrayResponseUnit = 290,
	GrayResponseCurve = 291,
	[ExifTagDescription(0u, "2-Dimensional encoding")]
	[ExifTagDescription(1u, "Uncompressed")]
	[ExifTagDescription(2u, "Fill bits added")]
	T4Options = 292,
	[ExifTagDescription(1u, "Uncompressed")]
	T6Options = 293,
	[ExifTagDescription((ushort)1, "None")]
	[ExifTagDescription((ushort)2, "Inches")]
	[ExifTagDescription((ushort)3, "Centimeter")]
	ResolutionUnit = 296,
	PageNumber = 297,
	ColorResponseUnit = 300,
	TransferFunction = 301,
	Software = 305,
	DateTime = 306,
	Artist = 315,
	HostComputer = 316,
	Predictor = 317,
	WhitePoint = 318,
	PrimaryChromaticities = 319,
	ColorMap = 320,
	HalftoneHints = 321,
	TileWidth = 322,
	TileLength = 323,
	TileOffsets = 324,
	TileByteCounts = 325,
	BadFaxLines = 326,
	[ExifTagDescription(0u, "Clean")]
	[ExifTagDescription(1u, "Regenerated")]
	[ExifTagDescription(2u, "Unclean")]
	CleanFaxData = 327,
	ConsecutiveBadFaxLines = 328,
	SubIFDs = 330,
	[ExifTagDescription((ushort)1, "CMYK")]
	[ExifTagDescription((ushort)2, "Not CMYK")]
	InkSet = 332,
	InkNames = 333,
	NumberOfInks = 334,
	DotRange = 336,
	TargetPrinter = 337,
	[ExifTagDescription((ushort)0, "Unspecified")]
	[ExifTagDescription((ushort)1, "Associated Alpha")]
	[ExifTagDescription((ushort)2, "Unassociated Alpha")]
	ExtraSamples = 338,
	[ExifTagDescription((ushort)1, "Unsigned")]
	[ExifTagDescription((ushort)2, "Signed")]
	[ExifTagDescription((ushort)3, "Float")]
	[ExifTagDescription((ushort)4, "Undefined")]
	[ExifTagDescription((ushort)5, "Complex int")]
	[ExifTagDescription((ushort)6, "Complex float")]
	SampleFormat = 339,
	SMinSampleValue = 340,
	SMaxSampleValue = 341,
	TransferRange = 342,
	ClipPath = 343,
	XClipPathUnits = 344,
	YClipPathUnits = 345,
	[ExifTagDescription((ushort)0, "Not indexed")]
	[ExifTagDescription((ushort)1, "Indexed")]
	Indexed = 346,
	JPEGTables = 347,
	[ExifTagDescription((ushort)0, "Higher resolution image does not exist")]
	[ExifTagDescription((ushort)1, "Higher resolution image exists")]
	OPIProxy = 351,
	GlobalParametersIFD = 400,
	[ExifTagDescription(0u, "Unspecified")]
	[ExifTagDescription(1u, "Group 3 FAX")]
	ProfileType = 401,
	[ExifTagDescription((byte)0, "Unknown")]
	[ExifTagDescription((byte)1, "Minimal B&W lossless, S")]
	[ExifTagDescription((byte)2, "Extended B&W lossless, F")]
	[ExifTagDescription((byte)3, "Lossless JBIG B&W, J")]
	[ExifTagDescription((byte)4, "Lossy color and grayscale, C")]
	[ExifTagDescription((byte)5, "Lossless color and grayscale, L")]
	[ExifTagDescription((byte)6, "Mixed raster content, M")]
	[ExifTagDescription((byte)7, "Profile T")]
	[ExifTagDescription(byte.MaxValue, "Multi Profiles")]
	FaxProfile = 402,
	[ExifTagDescription(0uL, "Unspecified compression")]
	[ExifTagDescription(1uL, "Modified Huffman")]
	[ExifTagDescription(2uL, "Modified Read")]
	[ExifTagDescription(4uL, "Modified MR")]
	[ExifTagDescription(8uL, "JBIG")]
	[ExifTagDescription(16uL, "Baseline JPEG")]
	[ExifTagDescription(32uL, "JBIG color")]
	CodingMethods = 403,
	VersionYear = 404,
	ModeNumber = 405,
	Decode = 433,
	DefaultImageColor = 434,
	T82ptions = 435,
	[ExifTagDescription((ushort)1, "Baseline")]
	[ExifTagDescription((ushort)14, "Lossless")]
	JPEGProc = 512,
	JPEGInterchangeFormat = 513,
	JPEGInterchangeFormatLength = 514,
	JPEGRestartInterval = 515,
	JPEGLosslessPredictors = 517,
	JPEGPointTransforms = 518,
	JPEGQTables = 519,
	JPEGDCTables = 520,
	JPEGACTables = 521,
	YCbCrCoefficients = 529,
	YCbCrSubsampling = 530,
	[ExifTagDescription((ushort)1, "Centered")]
	[ExifTagDescription((ushort)2, "Co-sited")]
	YCbCrPositioning = 531,
	ReferenceBlackWhite = 532,
	StripRowCounts = 559,
	XMP = 700,
	Rating = 18246,
	RatingPercent = 18249,
	ImageID = 32781,
	WangAnnotation = 32932,
	CFARepeatPatternDim = 33421,
	CFAPattern2 = 33422,
	BatteryLevel = 33423,
	Copyright = 33432,
	ExposureTime = 33434,
	FNumber = 33437,
	[ExifTagDescription((ushort)2, "Squary root data format")]
	[ExifTagDescription((ushort)128, "Linear data format")]
	MDFileTag = 33445,
	MDScalePixel = 33446,
	[ExifTagDescription((ushort)0, "lowest possible")]
	[ExifTagDescription((ushort)1, "low range")]
	[ExifTagDescription("n-2", "high range")]
	[ExifTagDescription("n-1", "highest possible")]
	MDColorTable = 33447,
	MDLabName = 33448,
	MDSampleInfo = 33449,
	MDPrepDate = 33450,
	MDPrepTime = 33451,
	[ExifTagDescription("O.D.", "Densitometer")]
	[ExifTagDescription("Counts", "PhosphorImager")]
	[ExifTagDescription("RFU", "FluorImager")]
	MDFileUnits = 33452,
	PixelScale = 33550,
	IPTC = 33723,
	IntergraphPacketData = 33918,
	IntergraphRegisters = 33919,
	IntergraphMatrix = 33920,
	ModelTiePoint = 33922,
	SEMInfo = 34118,
	ModelTransform = 34264,
	Photoshop = 34377,
	IccProfile = 34675,
	GeoKeyDirectoryTag = 34735,
	GeoDoubleParamsTag = 34736,
	GeoAsciiParamsTag = 34737,
	ImageLayer = 34732,
	[ExifTagDescription((ushort)0, "Not Defined")]
	[ExifTagDescription((ushort)1, "Manual")]
	[ExifTagDescription((ushort)2, "Program AE")]
	[ExifTagDescription((ushort)3, "Aperture-priority AE")]
	[ExifTagDescription((ushort)4, "Shutter speed priority AE")]
	[ExifTagDescription((ushort)5, "Creative (Slow speed)")]
	[ExifTagDescription((ushort)6, "Action (High speed)")]
	[ExifTagDescription((ushort)7, "Portrait")]
	[ExifTagDescription((ushort)8, "Landscape")]
	[ExifTagDescription((ushort)9, "Bulb")]
	ExposureProgram = 34850,
	SpectralSensitivity = 34852,
	ISOSpeedRatings = 34855,
	OECF = 34856,
	Interlace = 34857,
	TimeZoneOffset = 34858,
	SelfTimerMode = 34859,
	[ExifTagDescription((ushort)0, "Unknown")]
	[ExifTagDescription((ushort)1, "Standard Output Sensitivity")]
	[ExifTagDescription((ushort)2, "Recommended Exposure Index")]
	[ExifTagDescription((ushort)3, "ISO Speed")]
	[ExifTagDescription((ushort)4, "Standard Output Sensitivity and Recommended Exposure Index")]
	[ExifTagDescription((ushort)5, "Standard Output Sensitivity and ISO Speed")]
	[ExifTagDescription((ushort)6, "Recommended Exposure Index and ISO Speed")]
	[ExifTagDescription((ushort)7, "Standard Output Sensitivity, Recommended Exposure Index and ISO Speed")]
	SensitivityType = 34864,
	StandardOutputSensitivity = 34865,
	RecommendedExposureIndex = 34866,
	ISOSpeed = 34867,
	ISOSpeedLatitudeyyy = 34868,
	ISOSpeedLatitudezzz = 34869,
	FaxRecvParams = 34908,
	FaxSubaddress = 34909,
	FaxRecvTime = 34910,
	ExifVersion = 36864,
	DateTimeOriginal = 36867,
	DateTimeDigitized = 36868,
	OffsetTime = 36880,
	OffsetTimeOriginal = 36881,
	OffsetTimeDigitized = 36882,
	ComponentsConfiguration = 37121,
	CompressedBitsPerPixel = 37122,
	ShutterSpeedValue = 37377,
	ApertureValue = 37378,
	BrightnessValue = 37379,
	ExposureBiasValue = 37380,
	MaxApertureValue = 37381,
	SubjectDistance = 37382,
	[ExifTagDescription((ushort)0, "Unknown")]
	[ExifTagDescription((ushort)1, "Average")]
	[ExifTagDescription((ushort)2, "Center-weighted average")]
	[ExifTagDescription((ushort)3, "Spot")]
	[ExifTagDescription((ushort)4, "Multi-spot")]
	[ExifTagDescription((ushort)5, "Multi-segment")]
	[ExifTagDescription((ushort)6, "Partial")]
	[ExifTagDescription((ushort)255, "Other")]
	MeteringMode = 37383,
	[ExifTagDescription((ushort)0, "Unknown")]
	[ExifTagDescription((ushort)1, "Daylight")]
	[ExifTagDescription((ushort)2, "Fluorescent")]
	[ExifTagDescription((ushort)3, "Tungsten (Incandescent)")]
	[ExifTagDescription((ushort)4, "Flash")]
	[ExifTagDescription((ushort)9, "Fine Weather")]
	[ExifTagDescription((ushort)10, "Cloudy")]
	[ExifTagDescription((ushort)11, "Shade")]
	[ExifTagDescription((ushort)12, "Daylight Fluorescent")]
	[ExifTagDescription((ushort)13, "Day White Fluorescent")]
	[ExifTagDescription((ushort)14, "Cool White Fluorescent")]
	[ExifTagDescription((ushort)15, "White Fluorescent")]
	[ExifTagDescription((ushort)16, "Warm White Fluorescent")]
	[ExifTagDescription((ushort)17, "Standard Light A")]
	[ExifTagDescription((ushort)18, "Standard Light B")]
	[ExifTagDescription((ushort)19, "Standard Light C")]
	[ExifTagDescription((ushort)20, "D55")]
	[ExifTagDescription((ushort)21, "D65")]
	[ExifTagDescription((ushort)22, "D75")]
	[ExifTagDescription((ushort)23, "D50")]
	[ExifTagDescription((ushort)24, "ISO Studio Tungsten")]
	[ExifTagDescription((ushort)255, "Other")]
	LightSource = 37384,
	[ExifTagDescription((ushort)0, "No Flash")]
	[ExifTagDescription((ushort)1, "Fired")]
	[ExifTagDescription((ushort)5, "Fired, Return not detected")]
	[ExifTagDescription((ushort)7, "Fired, Return detected")]
	[ExifTagDescription((ushort)8, "On, Did not fire")]
	[ExifTagDescription((ushort)9, "On, Fired")]
	[ExifTagDescription((ushort)13, "On, Return not detected")]
	[ExifTagDescription((ushort)15, "On, Return detected")]
	[ExifTagDescription((ushort)16, "Off, Did not fire")]
	[ExifTagDescription((ushort)20, "Off, Did not fire, Return not detected")]
	[ExifTagDescription((ushort)24, "Auto, Did not fire")]
	[ExifTagDescription((ushort)25, "Auto, Fired")]
	[ExifTagDescription((ushort)29, "Auto, Fired, Return not detected")]
	[ExifTagDescription((ushort)31, "Auto, Fired, Return detected")]
	[ExifTagDescription((ushort)32, "No flash function")]
	[ExifTagDescription((ushort)48, "Off, No flash function")]
	[ExifTagDescription((ushort)65, "Fired, Red-eye reduction")]
	[ExifTagDescription((ushort)69, "Fired, Red-eye reduction, Return not detected")]
	[ExifTagDescription((ushort)71, "Fired, Red-eye reduction, Return detected")]
	[ExifTagDescription((ushort)73, "On, Red-eye reduction")]
	[ExifTagDescription((ushort)77, "On, Red-eye reduction, Return not detected")]
	[ExifTagDescription((ushort)79, "On, Red-eye reduction, Return detected")]
	[ExifTagDescription((ushort)80, "Off, Red-eye reduction")]
	[ExifTagDescription((ushort)88, "Auto, Did not fire, Red-eye reduction")]
	[ExifTagDescription((ushort)89, "Auto, Fired, Red-eye reduction")]
	[ExifTagDescription((ushort)93, "Auto, Fired, Red-eye reduction, Return not detected")]
	[ExifTagDescription((ushort)95, "Auto, Fired, Red-eye reduction, Return detected")]
	Flash = 37385,
	FocalLength = 37386,
	FlashEnergy2 = 37387,
	SpatialFrequencyResponse2 = 37388,
	Noise = 37389,
	FocalPlaneXResolution2 = 37390,
	FocalPlaneYResolution2 = 37391,
	[ExifTagDescription((ushort)1, "None")]
	[ExifTagDescription((ushort)2, "Inches")]
	[ExifTagDescription((ushort)3, "Centimeter")]
	[ExifTagDescription((ushort)4, "Millimeter")]
	[ExifTagDescription((ushort)5, "Micrometer")]
	FocalPlaneResolutionUnit2 = 37392,
	ImageNumber = 37393,
	[ExifTagDescription("C", "Confidential")]
	[ExifTagDescription("R", "Restricted")]
	[ExifTagDescription("S", "Secret")]
	[ExifTagDescription("T", "Top Secret")]
	[ExifTagDescription("U", "Unclassified")]
	SecurityClassification = 37394,
	ImageHistory = 37395,
	SubjectArea = 37396,
	ExposureIndex2 = 37397,
	TIFFEPStandardID = 37398,
	[ExifTagDescription((ushort)1, "Not defined")]
	[ExifTagDescription((ushort)2, "One-chip color area")]
	[ExifTagDescription((ushort)3, "Two-chip color area")]
	[ExifTagDescription((ushort)4, "Three-chip color area")]
	[ExifTagDescription((ushort)5, "Color sequential area")]
	[ExifTagDescription((ushort)7, "Trilinear")]
	[ExifTagDescription((ushort)8, "Color sequential linear")]
	SensingMethod2 = 37399,
	MakerNote = 37500,
	UserComment = 37510,
	SubsecTime = 37520,
	SubsecTimeOriginal = 37521,
	SubsecTimeDigitized = 37522,
	ImageSourceData = 37724,
	AmbientTemperature = 37888,
	Humidity = 37889,
	Pressure = 37890,
	WaterDepth = 37891,
	Acceleration = 37892,
	CameraElevationAngle = 37893,
	XPTitle = 40091,
	XPComment = 40092,
	XPAuthor = 40093,
	XPKeywords = 40094,
	XPSubject = 40095,
	FlashpixVersion = 40960,
	[ExifTagDescription((ushort)1, "sRGB")]
	[ExifTagDescription((ushort)2, "Adobe RGB")]
	[ExifTagDescription((ushort)4093, "Wide Gamut RGB")]
	[ExifTagDescription((ushort)65534, "ICC Profile")]
	[ExifTagDescription(ushort.MaxValue, "Uncalibrated")]
	ColorSpace = 40961,
	PixelXDimension = 40962,
	PixelYDimension = 40963,
	RelatedSoundFile = 40964,
	InteroperabilityIFD = 40965,
	FlashEnergy = 41483,
	SpatialFrequencyResponse = 41484,
	FocalPlaneXResolution = 41486,
	FocalPlaneYResolution = 41487,
	[ExifTagDescription((ushort)1, "None")]
	[ExifTagDescription((ushort)2, "Inches")]
	[ExifTagDescription((ushort)3, "Centimeter")]
	[ExifTagDescription((ushort)4, "Millimeter")]
	[ExifTagDescription((ushort)5, "Micrometer")]
	FocalPlaneResolutionUnit = 41488,
	SubjectLocation = 41492,
	ExposureIndex = 41493,
	[ExifTagDescription((ushort)1, "Not defined")]
	[ExifTagDescription((ushort)2, "One-chip color area")]
	[ExifTagDescription((ushort)3, "Two-chip color area")]
	[ExifTagDescription((ushort)4, "Three-chip color area")]
	[ExifTagDescription((ushort)5, "Color sequential area")]
	[ExifTagDescription((ushort)7, "Trilinear")]
	[ExifTagDescription((ushort)8, "Color sequential linear")]
	SensingMethod = 41495,
	FileSource = 41728,
	SceneType = 41729,
	CFAPattern = 41730,
	[ExifTagDescription((ushort)1, "Normal")]
	[ExifTagDescription((ushort)2, "Custom")]
	CustomRendered = 41985,
	[ExifTagDescription((ushort)0, "Auto")]
	[ExifTagDescription((ushort)1, "Manual")]
	[ExifTagDescription((ushort)2, "Auto bracket")]
	ExposureMode = 41986,
	[ExifTagDescription((ushort)0, "Auto")]
	[ExifTagDescription((ushort)1, "Manual")]
	WhiteBalance = 41987,
	DigitalZoomRatio = 41988,
	FocalLengthIn35mmFilm = 41989,
	[ExifTagDescription((ushort)0, "Standard")]
	[ExifTagDescription((ushort)1, "Landscape")]
	[ExifTagDescription((ushort)2, "Portrait")]
	[ExifTagDescription((ushort)3, "Night")]
	SceneCaptureType = 41990,
	[ExifTagDescription((ushort)0, "None")]
	[ExifTagDescription((ushort)1, "Low gain up")]
	[ExifTagDescription((ushort)2, "High gain up")]
	[ExifTagDescription((ushort)3, "Low gain down")]
	[ExifTagDescription((ushort)4, "High gain down")]
	GainControl = 41991,
	[ExifTagDescription((ushort)0, "Normal")]
	[ExifTagDescription((ushort)1, "Low")]
	[ExifTagDescription((ushort)2, "High")]
	Contrast = 41992,
	[ExifTagDescription((ushort)0, "Normal")]
	[ExifTagDescription((ushort)1, "Low")]
	[ExifTagDescription((ushort)2, "High")]
	Saturation = 41993,
	[ExifTagDescription((ushort)0, "Normal")]
	[ExifTagDescription((ushort)1, "Soft")]
	[ExifTagDescription((ushort)2, "Hard")]
	Sharpness = 41994,
	DeviceSettingDescription = 41995,
	[ExifTagDescription((ushort)0, "Unknown")]
	[ExifTagDescription((ushort)1, "Macro")]
	[ExifTagDescription((ushort)2, "Close")]
	[ExifTagDescription((ushort)3, "Distant")]
	SubjectDistanceRange = 41996,
	ImageUniqueID = 42016,
	OwnerName = 42032,
	SerialNumber = 42033,
	LensSpecification = 42034,
	LensMake = 42035,
	LensModel = 42036,
	LensSerialNumber = 42037,
	GDALMetadata = 42112,
	GDALNoData = 42113,
	GPSVersionID = 0,
	GPSLatitudeRef = 1,
	GPSLatitude = 2,
	GPSLongitudeRef = 3,
	GPSLongitude = 4,
	GPSAltitudeRef = 5,
	GPSAltitude = 6,
	GPSTimestamp = 7,
	GPSSatellites = 8,
	GPSStatus = 9,
	GPSMeasureMode = 10,
	GPSDOP = 11,
	GPSSpeedRef = 12,
	GPSSpeed = 13,
	GPSTrackRef = 14,
	GPSTrack = 15,
	GPSImgDirectionRef = 16,
	GPSImgDirection = 17,
	GPSMapDatum = 18,
	GPSDestLatitudeRef = 19,
	GPSDestLatitude = 20,
	GPSDestLongitudeRef = 21,
	GPSDestLongitude = 22,
	GPSDestBearingRef = 23,
	GPSDestBearing = 24,
	GPSDestDistanceRef = 25,
	GPSDestDistance = 26,
	GPSProcessingMethod = 27,
	GPSAreaInformation = 28,
	GPSDateStamp = 29,
	GPSDifferential = 30,
	GPSHPositioningError = 31,
	OceScanjobDescription = 50215,
	OceApplicationSelector = 50216,
	OceIdentificationNumber = 50217,
	OceImageLogicCharacteristics = 50218,
	AliasLayerMetadata = 50784
}
