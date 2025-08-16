namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal class Vp8StatsArray
{
	public uint[] Stats { get; }

	public Vp8StatsArray()
	{
		Stats = new uint[11];
	}
}
