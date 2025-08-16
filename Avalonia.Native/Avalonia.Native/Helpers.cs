using Avalonia.Native.Interop;

namespace Avalonia.Native;

internal static class Helpers
{
	public static Point ToAvaloniaPoint(this AvnPoint pt)
	{
		return new Point(pt.X, pt.Y);
	}

	public static PixelPoint ToAvaloniaPixelPoint(this AvnPoint pt)
	{
		return new PixelPoint((int)pt.X, (int)pt.Y);
	}

	public static AvnPoint ToAvnPoint(this Point pt)
	{
		AvnPoint result = default(AvnPoint);
		result.X = pt.X;
		result.Y = pt.Y;
		return result;
	}

	public static AvnPoint ToAvnPoint(this PixelPoint pt)
	{
		AvnPoint result = default(AvnPoint);
		result.X = pt.X;
		result.Y = pt.Y;
		return result;
	}

	public static AvnRect ToAvnRect(this Rect rect)
	{
		AvnRect result = default(AvnRect);
		result.X = rect.X;
		result.Y = rect.Y;
		result.Height = rect.Height;
		result.Width = rect.Width;
		return result;
	}

	public static AvnSize ToAvnSize(this Size size)
	{
		AvnSize result = default(AvnSize);
		result.Height = size.Height;
		result.Width = size.Width;
		return result;
	}

	public static IAvnString ToAvnString(this string s)
	{
		if (s == null)
		{
			return null;
		}
		return new AvnString(s);
	}

	public static Size ToAvaloniaSize(this AvnSize size)
	{
		return new Size(size.Width, size.Height);
	}

	public static Rect ToAvaloniaRect(this AvnRect rect)
	{
		return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
	}

	public static PixelRect ToAvaloniaPixelRect(this AvnRect rect)
	{
		return new PixelRect((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
	}
}
