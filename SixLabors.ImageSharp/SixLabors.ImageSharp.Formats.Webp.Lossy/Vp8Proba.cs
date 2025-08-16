namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal class Vp8Proba
{
	private const int MbFeatureTreeProbs = 3;

	public uint[] Segments { get; }

	public Vp8BandProbas[,] Bands { get; }

	public Vp8BandProbas[][] BandsPtr { get; }

	public Vp8Proba()
	{
		Segments = new uint[3];
		Bands = new Vp8BandProbas[4, 8];
		BandsPtr = new Vp8BandProbas[4][];
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				Bands[i, j] = new Vp8BandProbas();
			}
		}
		for (int k = 0; k < 4; k++)
		{
			BandsPtr[k] = new Vp8BandProbas[17];
		}
	}
}
