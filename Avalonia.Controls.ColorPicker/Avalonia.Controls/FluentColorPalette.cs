using Avalonia.Media;
using Avalonia.Utilities;

namespace Avalonia.Controls;

public class FluentColorPalette : IColorPalette
{
	private static Color[,] colorChart = new Color[6, 8]
	{
		{
			Color.FromArgb(byte.MaxValue, byte.MaxValue, 67, 67),
			Color.FromArgb(byte.MaxValue, 209, 52, 56),
			Color.FromArgb(byte.MaxValue, 239, 105, 80),
			Color.FromArgb(byte.MaxValue, 218, 59, 1),
			Color.FromArgb(byte.MaxValue, 202, 80, 16),
			Color.FromArgb(byte.MaxValue, 247, 99, 12),
			Color.FromArgb(byte.MaxValue, byte.MaxValue, 140, 0),
			Color.FromArgb(byte.MaxValue, byte.MaxValue, 185, 0)
		},
		{
			Color.FromArgb(byte.MaxValue, 231, 72, 86),
			Color.FromArgb(byte.MaxValue, 232, 17, 35),
			Color.FromArgb(byte.MaxValue, 234, 0, 94),
			Color.FromArgb(byte.MaxValue, 195, 0, 82),
			Color.FromArgb(byte.MaxValue, 227, 0, 140),
			Color.FromArgb(byte.MaxValue, 191, 0, 119),
			Color.FromArgb(byte.MaxValue, 194, 57, 179),
			Color.FromArgb(byte.MaxValue, 154, 0, 137)
		},
		{
			Color.FromArgb(byte.MaxValue, 0, 120, 215),
			Color.FromArgb(byte.MaxValue, 0, 99, 177),
			Color.FromArgb(byte.MaxValue, 142, 140, 216),
			Color.FromArgb(byte.MaxValue, 107, 105, 214),
			Color.FromArgb(byte.MaxValue, 135, 100, 184),
			Color.FromArgb(byte.MaxValue, 116, 77, 169),
			Color.FromArgb(byte.MaxValue, 177, 70, 194),
			Color.FromArgb(byte.MaxValue, 136, 23, 152)
		},
		{
			Color.FromArgb(byte.MaxValue, 0, 153, 188),
			Color.FromArgb(byte.MaxValue, 45, 125, 154),
			Color.FromArgb(byte.MaxValue, 0, 183, 195),
			Color.FromArgb(byte.MaxValue, 3, 131, 135),
			Color.FromArgb(byte.MaxValue, 0, 178, 148),
			Color.FromArgb(byte.MaxValue, 1, 133, 116),
			Color.FromArgb(byte.MaxValue, 0, 204, 106),
			Color.FromArgb(byte.MaxValue, 16, 137, 62)
		},
		{
			Color.FromArgb(byte.MaxValue, 122, 117, 116),
			Color.FromArgb(byte.MaxValue, 93, 90, 80),
			Color.FromArgb(byte.MaxValue, 104, 118, 138),
			Color.FromArgb(byte.MaxValue, 81, 92, 107),
			Color.FromArgb(byte.MaxValue, 86, 124, 115),
			Color.FromArgb(byte.MaxValue, 72, 104, 96),
			Color.FromArgb(byte.MaxValue, 73, 130, 5),
			Color.FromArgb(byte.MaxValue, 16, 124, 16)
		},
		{
			Color.FromArgb(byte.MaxValue, 118, 118, 118),
			Color.FromArgb(byte.MaxValue, 76, 74, 72),
			Color.FromArgb(byte.MaxValue, 105, 121, 126),
			Color.FromArgb(byte.MaxValue, 74, 84, 89),
			Color.FromArgb(byte.MaxValue, 100, 124, 100),
			Color.FromArgb(byte.MaxValue, 82, 94, 84),
			Color.FromArgb(byte.MaxValue, 132, 117, 69),
			Color.FromArgb(byte.MaxValue, 126, 115, 95)
		}
	};

	public int ColorCount => colorChart.GetLength(0);

	public int ShadeCount => colorChart.GetLength(1);

	public Color GetColor(int colorIndex, int shadeIndex)
	{
		return colorChart[MathUtilities.Clamp(colorIndex, 0, colorChart.GetLength(0) - 1), MathUtilities.Clamp(shadeIndex, 0, colorChart.GetLength(1) - 1)];
	}
}
