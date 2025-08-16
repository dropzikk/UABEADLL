using Avalonia.Media;
using Avalonia.Utilities;

namespace Avalonia.Controls;

public class MaterialHalfColorPalette : IColorPalette
{
	private static Color[,]? _colorChart = null;

	private static readonly object _colorChartMutex = new object();

	public int ColorCount => 10;

	public int ShadeCount => 5;

	protected void InitColorChart()
	{
		lock (_colorChartMutex)
		{
			if (_colorChart == null)
			{
				_colorChart = new Color[10, 5]
				{
					{
						Color.FromUInt32(4294962158u),
						Color.FromUInt32(4293892762u),
						Color.FromUInt32(4293874512u),
						Color.FromUInt32(4293212469u),
						Color.FromUInt32(4291176488u)
					},
					{
						Color.FromUInt32(4294174197u),
						Color.FromUInt32(4291728344u),
						Color.FromUInt32(4289415100u),
						Color.FromUInt32(4287505578u),
						Color.FromUInt32(4285143962u)
					},
					{
						Color.FromUInt32(4293454582u),
						Color.FromUInt32(4288653530u),
						Color.FromUInt32(4284246976u),
						Color.FromUInt32(4281944491u),
						Color.FromUInt32(4280825235u)
					},
					{
						Color.FromUInt32(4292998654u),
						Color.FromUInt32(4286698746u),
						Color.FromUInt32(4280923894u),
						Color.FromUInt32(4278426597u),
						Color.FromUInt32(4278351805u)
					},
					{
						Color.FromUInt32(4292932337u),
						Color.FromUInt32(4286630852u),
						Color.FromUInt32(4280723098u),
						Color.FromUInt32(4278225275u),
						Color.FromUInt32(4278217052u)
					},
					{
						Color.FromUInt32(4294047977u),
						Color.FromUInt32(4291158437u),
						Color.FromUInt32(4288466021u),
						Color.FromUInt32(4286362434u),
						Color.FromUInt32(4283796271u)
					},
					{
						Color.FromUInt32(4294966759u),
						Color.FromUInt32(4294964637u),
						Color.FromUInt32(4294962776u),
						Color.FromUInt32(4294826037u),
						Color.FromUInt32(4294551589u)
					},
					{
						Color.FromUInt32(4294964192u),
						Color.FromUInt32(4294954112u),
						Color.FromUInt32(4294944550u),
						Color.FromUInt32(4294675456u),
						Color.FromUInt32(4293880832u)
					},
					{
						Color.FromUInt32(4293913577u),
						Color.FromUInt32(4290554532u),
						Color.FromUInt32(4287458915u),
						Color.FromUInt32(4285353025u),
						Color.FromUInt32(4283315246u)
					},
					{
						Color.FromUInt32(4293718001u),
						Color.FromUInt32(4289773253u),
						Color.FromUInt32(4286091420u),
						Color.FromUInt32(4283723386u),
						Color.FromUInt32(4281812815u)
					}
				};
			}
		}
	}

	public Color GetColor(int colorIndex, int shadeIndex)
	{
		if (_colorChart == null)
		{
			InitColorChart();
		}
		return _colorChart[MathUtilities.Clamp(colorIndex, 0, ColorCount - 1), MathUtilities.Clamp(shadeIndex, 0, ShadeCount - 1)];
	}
}
