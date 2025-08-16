namespace Avalonia.Animation.Utils;

internal static class BounceEaseUtils
{
	internal static double Bounce(double progress)
	{
		if (progress < 0.36363636363636365)
		{
			return 121.0 * progress * progress / 16.0;
		}
		if (progress < 0.7272727272727273)
		{
			return 9.075 * progress * progress - 9.9 * progress + 3.4;
		}
		if (progress < 0.9)
		{
			return 12.066481994459833 * progress * progress - 19.63545706371191 * progress + 8.898060941828255;
		}
		return 10.8 * progress * progress - 20.52 * progress + 10.72;
	}
}
