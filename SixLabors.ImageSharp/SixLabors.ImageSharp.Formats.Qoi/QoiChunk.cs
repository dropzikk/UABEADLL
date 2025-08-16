namespace SixLabors.ImageSharp.Formats.Qoi;

internal enum QoiChunk
{
	QoiOpRgb = 254,
	QoiOpRgba = 255,
	QoiOpIndex = 0,
	QoiOpDiff = 64,
	QoiOpLuma = 128,
	QoiOpRun = 192
}
