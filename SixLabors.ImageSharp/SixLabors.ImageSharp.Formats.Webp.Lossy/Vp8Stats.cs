namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal class Vp8Stats
{
	public Vp8StatsArray[] Stats { get; }

	public Vp8Stats()
	{
		Stats = new Vp8StatsArray[3];
		for (int i = 0; i < 3; i++)
		{
			Stats[i] = new Vp8StatsArray();
		}
	}
}
