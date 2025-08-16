using System.ComponentModel;

namespace SixLabors.ImageSharp.Formats.Png;

[EditorBrowsable(EditorBrowsableState.Never)]
public enum PngCompressionLevel
{
	Level0 = 0,
	NoCompression = 0,
	Level1 = 1,
	BestSpeed = 1,
	Level2 = 2,
	Level3 = 3,
	Level4 = 4,
	Level5 = 5,
	Level6 = 6,
	DefaultCompression = 6,
	Level7 = 7,
	Level8 = 8,
	Level9 = 9,
	BestCompression = 9
}
