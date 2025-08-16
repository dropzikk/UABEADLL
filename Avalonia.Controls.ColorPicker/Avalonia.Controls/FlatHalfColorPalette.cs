using Avalonia.Media;
using Avalonia.Utilities;

namespace Avalonia.Controls;

public class FlatHalfColorPalette : IColorPalette
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
						Color.FromUInt32(4294568938u),
						Color.FromUInt32(4293308586u),
						Color.FromUInt32(4291649877u),
						Color.FromUInt32(4289278502u),
						Color.FromUInt32(4286260252u)
					},
					{
						Color.FromUInt32(4294307576u),
						Color.FromUInt32(4292328930u),
						Color.FromUInt32(4289690309u),
						Color.FromUInt32(4287123104u),
						Color.FromUInt32(4284692852u)
					},
					{
						Color.FromUInt32(4293587704u),
						Color.FromUInt32(4289318115u),
						Color.FromUInt32(4283734471u),
						Color.FromUInt32(4280578467u),
						Color.FromUInt32(4279915126u)
					},
					{
						Color.FromUInt32(4293458165u),
						Color.FromUInt32(4288931031u),
						Color.FromUInt32(4282960304u),
						Color.FromUInt32(4279739785u),
						Color.FromUInt32(4279335012u)
					},
					{
						Color.FromUInt32(4293523439u),
						Color.FromUInt32(4289322943u),
						Color.FromUInt32(4283612800u),
						Color.FromUInt32(4280457556u),
						Color.FromUInt32(4279856957u)
					},
					{
						Color.FromUInt32(4294900199u),
						Color.FromUInt32(4294567839u),
						Color.FromUInt32(4294234175u),
						Color.FromUInt32(4292127757u),
						Color.FromUInt32(4288314634u)
					},
					{
						Color.FromUInt32(4294832873u),
						Color.FromUInt32(4294298535u),
						Color.FromUInt32(4293630030u),
						Color.FromUInt32(4291456798u),
						Color.FromUInt32(4287844630u)
					},
					{
						Color.FromUInt32(4294835966u),
						Color.FromUInt32(4294441465u),
						Color.FromUInt32(4293981172u),
						Color.FromUInt32(4291875796u),
						Color.FromUInt32(4288125594u)
					},
					{
						Color.FromUInt32(4294244086u),
						Color.FromUInt32(4292205531u),
						Color.FromUInt32(4289378232u),
						Color.FromUInt32(4286812562u),
						Color.FromUInt32(4284443242u)
					},
					{
						Color.FromUInt32(4293651951u),
						Color.FromUInt32(4289640127u),
						Color.FromUInt32(4284312958u),
						Color.FromUInt32(4281221203u),
						Color.FromUInt32(4280364860u)
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
