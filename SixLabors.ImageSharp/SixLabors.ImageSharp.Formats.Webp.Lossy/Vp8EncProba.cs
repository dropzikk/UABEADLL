using System;

namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal class Vp8EncProba
{
	private const int MaxVariableLevel = 67;

	private const int SkipProbaThreshold = 250;

	public byte[] Segments { get; }

	public byte SkipProba { get; set; }

	public bool UseSkipProba { get; set; }

	public Vp8BandProbas[][] Coeffs { get; }

	public Vp8Stats[][] Stats { get; }

	public Vp8Costs[][] LevelCost { get; }

	public Vp8Costs[][] RemappedCosts { get; }

	public int NbSkip { get; set; }

	public bool Dirty { get; set; }

	public Vp8EncProba()
	{
		Dirty = true;
		UseSkipProba = false;
		Segments = new byte[3];
		Coeffs = new Vp8BandProbas[4][];
		for (int i = 0; i < Coeffs.Length; i++)
		{
			Coeffs[i] = new Vp8BandProbas[8];
			for (int j = 0; j < Coeffs[i].Length; j++)
			{
				Coeffs[i][j] = new Vp8BandProbas();
			}
		}
		Stats = new Vp8Stats[4][];
		for (int k = 0; k < Coeffs.Length; k++)
		{
			Stats[k] = new Vp8Stats[8];
			for (int l = 0; l < Stats[k].Length; l++)
			{
				Stats[k][l] = new Vp8Stats();
			}
		}
		LevelCost = new Vp8Costs[4][];
		for (int m = 0; m < LevelCost.Length; m++)
		{
			LevelCost[m] = new Vp8Costs[8];
			for (int n = 0; n < LevelCost[m].Length; n++)
			{
				LevelCost[m][n] = new Vp8Costs();
			}
		}
		RemappedCosts = new Vp8Costs[4][];
		for (int num = 0; num < RemappedCosts.Length; num++)
		{
			RemappedCosts[num] = new Vp8Costs[16];
			for (int num2 = 0; num2 < RemappedCosts[num].Length; num2++)
			{
				RemappedCosts[num][num2] = new Vp8Costs();
			}
		}
		Segments.AsSpan().Fill(byte.MaxValue);
		for (int num3 = 0; num3 < 4; num3++)
		{
			for (int num4 = 0; num4 < 8; num4++)
			{
				for (int num5 = 0; num5 < 3; num5++)
				{
					Vp8ProbaArray vp8ProbaArray = Coeffs[num3][num4].Probabilities[num5];
					for (int num6 = 0; num6 < 11; num6++)
					{
						vp8ProbaArray.Probabilities[num6] = WebpLookupTables.DefaultCoeffsProba[num3, num4, num5, num6];
					}
				}
			}
		}
	}

	public void CalculateLevelCosts()
	{
		if (!Dirty)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				for (int k = 0; k < 3; k++)
				{
					Vp8ProbaArray vp8ProbaArray = Coeffs[i][j].Probabilities[k];
					Vp8CostArray vp8CostArray = LevelCost[i][j].Costs[k];
					int num = ((k > 0) ? LossyUtils.Vp8BitCost(1, vp8ProbaArray.Probabilities[0]) : 0);
					int num2 = LossyUtils.Vp8BitCost(1, vp8ProbaArray.Probabilities[1]) + num;
					vp8CostArray.Costs[0] = (ushort)(LossyUtils.Vp8BitCost(0, vp8ProbaArray.Probabilities[1]) + num);
					for (int l = 1; l <= 67; l++)
					{
						vp8CostArray.Costs[l] = (ushort)(num2 + VariableLevelCost(l, vp8ProbaArray.Probabilities));
					}
				}
			}
			for (int m = 0; m < 16; m++)
			{
				for (int n = 0; n < 3; n++)
				{
					Vp8CostArray vp8CostArray2 = RemappedCosts[i][m].Costs[n];
					LevelCost[i][WebpConstants.Vp8EncBands[m]].Costs[n].Costs.CopyTo(vp8CostArray2.Costs.AsSpan());
				}
			}
		}
		Dirty = false;
	}

	public int FinalizeTokenProbas()
	{
		bool flag = false;
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				for (int k = 0; k < 3; k++)
				{
					for (int l = 0; l < 11; l++)
					{
						uint num2 = Stats[i][j].Stats[k].Stats[l];
						int nb = (int)(num2 & 0xFFFF);
						int total = (int)((num2 >> 16) & 0xFFFF);
						int num3 = WebpLookupTables.CoeffsUpdateProba[i, j, k, l];
						int num4 = WebpLookupTables.DefaultCoeffsProba[i, j, k, l];
						int num5 = CalcTokenProba(nb, total);
						int num6 = BranchCost(nb, total, num4) + LossyUtils.Vp8BitCost(0, (byte)num3);
						int num7 = BranchCost(nb, total, num5) + LossyUtils.Vp8BitCost(1, (byte)num3) + 2048;
						bool flag2 = num6 > num7;
						num += LossyUtils.Vp8BitCost(flag2 ? 1 : 0, (byte)num3);
						if (flag2)
						{
							Coeffs[i][j].Probabilities[k].Probabilities[l] = (byte)num5;
							flag = flag || num5 != num4;
							num += 2048;
						}
						else
						{
							Coeffs[i][j].Probabilities[k].Probabilities[l] = (byte)num4;
						}
					}
				}
			}
		}
		Dirty = flag;
		return num;
	}

	public int FinalizeSkipProba(int mbw, int mbh)
	{
		int num = mbw * mbh;
		int nbSkip = NbSkip;
		SkipProba = (byte)CalcSkipProba(nbSkip, num);
		UseSkipProba = SkipProba < 250;
		int num2 = 256;
		if (UseSkipProba)
		{
			num2 += nbSkip * LossyUtils.Vp8BitCost(1, SkipProba) + (num - nbSkip) * LossyUtils.Vp8BitCost(0, SkipProba);
			num2 += 2048;
		}
		return num2;
	}

	public void ResetTokenStats()
	{
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				for (int k = 0; k < 3; k++)
				{
					for (int l = 0; l < 11; l++)
					{
						Stats[i][j].Stats[k].Stats[l] = 0u;
					}
				}
			}
		}
	}

	private static int CalcSkipProba(long nb, long total)
	{
		return (int)((total != 0L) ? ((total - nb) * 255 / total) : 255);
	}

	private static int VariableLevelCost(int level, Span<byte> probas)
	{
		int num = WebpLookupTables.Vp8LevelCodes[level - 1][0];
		int num2 = WebpLookupTables.Vp8LevelCodes[level - 1][1];
		int num3 = 0;
		int num4 = 2;
		while (num != 0)
		{
			if ((num & 1) != 0)
			{
				num3 += LossyUtils.Vp8BitCost(num2 & 1, probas[num4]);
			}
			num2 >>= 1;
			num >>= 1;
			num4++;
		}
		return num3;
	}

	private static int CalcTokenProba(int nb, int total)
	{
		if (nb == 0)
		{
			return 255;
		}
		return 255 - nb * 255 / total;
	}

	private static int BranchCost(int nb, int total, int proba)
	{
		return nb * LossyUtils.Vp8BitCost(1, (byte)proba) + (total - nb) * LossyUtils.Vp8BitCost(0, (byte)proba);
	}
}
