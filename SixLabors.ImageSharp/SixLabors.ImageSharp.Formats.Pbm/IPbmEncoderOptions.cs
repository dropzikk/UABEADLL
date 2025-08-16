namespace SixLabors.ImageSharp.Formats.Pbm;

internal interface IPbmEncoderOptions
{
	PbmEncoding? Encoding { get; }

	PbmColorType? ColorType { get; }

	PbmComponentType? ComponentType { get; }
}
