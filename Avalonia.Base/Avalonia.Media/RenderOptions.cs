using Avalonia.Media.Imaging;

namespace Avalonia.Media;

public readonly record struct RenderOptions
{
	public BitmapInterpolationMode BitmapInterpolationMode { get; init; }

	public EdgeMode EdgeMode { get; init; }

	public TextRenderingMode TextRenderingMode { get; init; }

	public BitmapBlendingMode BitmapBlendingMode { get; init; }

	public static BitmapInterpolationMode GetBitmapInterpolationMode(Visual visual)
	{
		return visual.RenderOptions.BitmapInterpolationMode;
	}

	public static void SetBitmapInterpolationMode(Visual visual, BitmapInterpolationMode value)
	{
		visual.RenderOptions = visual.RenderOptions with
		{
			BitmapInterpolationMode = value
		};
	}

	public static BitmapBlendingMode GetBitmapBlendingMode(Visual visual)
	{
		return visual.RenderOptions.BitmapBlendingMode;
	}

	public static void SetBitmapBlendingMode(Visual visual, BitmapBlendingMode value)
	{
		visual.RenderOptions = visual.RenderOptions with
		{
			BitmapBlendingMode = value
		};
	}

	public static EdgeMode GetEdgeMode(Visual visual)
	{
		return visual.RenderOptions.EdgeMode;
	}

	public static void SetEdgeMode(Visual visual, EdgeMode value)
	{
		visual.RenderOptions = visual.RenderOptions with
		{
			EdgeMode = value
		};
	}

	public static TextRenderingMode GetTextRenderingMode(Visual visual)
	{
		return visual.RenderOptions.TextRenderingMode;
	}

	public static void SetTextRenderingMode(Visual visual, TextRenderingMode value)
	{
		visual.RenderOptions = visual.RenderOptions with
		{
			TextRenderingMode = value
		};
	}

	public RenderOptions MergeWith(RenderOptions other)
	{
		BitmapInterpolationMode bitmapInterpolationMode = BitmapInterpolationMode;
		if (bitmapInterpolationMode == BitmapInterpolationMode.Unspecified)
		{
			bitmapInterpolationMode = other.BitmapInterpolationMode;
		}
		EdgeMode edgeMode = EdgeMode;
		if (edgeMode == EdgeMode.Unspecified)
		{
			edgeMode = other.EdgeMode;
		}
		TextRenderingMode textRenderingMode = TextRenderingMode;
		if (textRenderingMode == TextRenderingMode.Unspecified)
		{
			textRenderingMode = other.TextRenderingMode;
		}
		BitmapBlendingMode bitmapBlendingMode = BitmapBlendingMode;
		if (bitmapBlendingMode == BitmapBlendingMode.Unspecified)
		{
			bitmapBlendingMode = other.BitmapBlendingMode;
		}
		RenderOptions result = default(RenderOptions);
		result.BitmapInterpolationMode = bitmapInterpolationMode;
		result.EdgeMode = edgeMode;
		result.TextRenderingMode = textRenderingMode;
		result.BitmapBlendingMode = bitmapBlendingMode;
		return result;
	}
}
