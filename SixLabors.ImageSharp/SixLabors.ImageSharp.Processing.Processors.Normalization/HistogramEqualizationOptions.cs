namespace SixLabors.ImageSharp.Processing.Processors.Normalization;

public class HistogramEqualizationOptions
{
	public HistogramEqualizationMethod Method { get; set; }

	public int LuminanceLevels { get; set; } = 256;

	public bool ClipHistogram { get; set; }

	public int ClipLimit { get; set; } = 350;

	public int NumberOfTiles { get; set; } = 8;

	public bool SyncChannels { get; set; } = true;
}
