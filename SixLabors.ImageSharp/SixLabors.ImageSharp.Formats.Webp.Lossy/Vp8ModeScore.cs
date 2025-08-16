using System;

namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal class Vp8ModeScore
{
	public const long MaxCost = 36028797018963967L;

	private const int RdDistoMult = 256;

	public long D { get; set; }

	public long SD { get; set; }

	public long H { get; set; }

	public long R { get; set; }

	public long Score { get; set; }

	public short[] YDcLevels { get; }

	public short[] YAcLevels { get; }

	public short[] UvLevels { get; }

	public int ModeI16 { get; set; }

	public byte[] ModesI4 { get; }

	public int ModeUv { get; set; }

	public uint Nz { get; set; }

	public int[,] Derr { get; }

	public Vp8ModeScore()
	{
		YDcLevels = new short[16];
		YAcLevels = new short[256];
		UvLevels = new short[128];
		ModesI4 = new byte[16];
		Derr = new int[2, 3];
	}

	public void Clear()
	{
		Array.Clear(YDcLevels);
		Array.Clear(YAcLevels);
		Array.Clear(UvLevels);
		Array.Clear(ModesI4);
		Array.Clear(Derr);
	}

	public void InitScore()
	{
		D = 0L;
		SD = 0L;
		R = 0L;
		H = 0L;
		Nz = 0u;
		Score = 36028797018963967L;
	}

	public void CopyScore(Vp8ModeScore other)
	{
		D = other.D;
		SD = other.SD;
		R = other.R;
		H = other.H;
		Nz = other.Nz;
		Score = other.Score;
	}

	public void AddScore(Vp8ModeScore other)
	{
		D += other.D;
		SD += other.SD;
		R += other.R;
		H += other.H;
		Nz |= other.Nz;
		Score += other.Score;
	}

	public void SetRdScore(int lambda)
	{
		Score = (R + H) * lambda + 256 * (D + SD);
	}
}
