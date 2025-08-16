namespace Avalonia.Native;

internal static class Extensions
{
	public static int AsComBool(this bool b)
	{
		if (!b)
		{
			return 0;
		}
		return 1;
	}

	public static bool FromComBool(this int b)
	{
		return b != 0;
	}
}
