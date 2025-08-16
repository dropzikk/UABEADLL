namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal class Vp8FilterHeader
{
	private const int NumRefLfDeltas = 4;

	private const int NumModeLfDeltas = 4;

	private int filterLevel;

	private int sharpness;

	public LoopFilter LoopFilter { get; set; }

	public int FilterLevel
	{
		get
		{
			return filterLevel;
		}
		set
		{
			Guard.MustBeBetweenOrEqualTo(value, 0, 63, "FilterLevel");
			filterLevel = value;
		}
	}

	public int Sharpness
	{
		get
		{
			return sharpness;
		}
		set
		{
			Guard.MustBeBetweenOrEqualTo(value, 0, 7, "Sharpness");
			sharpness = value;
		}
	}

	public bool Simple { get; set; }

	public int I4x4LfDelta { get; set; }

	public bool UseLfDelta { get; set; }

	public int[] RefLfDelta { get; }

	public int[] ModeLfDelta { get; }

	public Vp8FilterHeader()
	{
		RefLfDelta = new int[4];
		ModeLfDelta = new int[4];
	}
}
