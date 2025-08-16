using System.Runtime.InteropServices;
using Avalonia.Media;

namespace Avalonia.Win32.WinRT;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal record struct WinRTColor
{
	public byte A;

	public byte R;

	public byte G;

	public byte B;

	public static WinRTColor FromArgb(byte a, byte r, byte g, byte b)
	{
		WinRTColor result = default(WinRTColor);
		result.A = a;
		result.R = r;
		result.G = g;
		result.B = b;
		return result;
	}

	public Color ToAvalonia()
	{
		return new Color(A, R, G, B);
	}
}
