using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

internal static class ResizeHelper
{
	public unsafe static int CalculateResizeWorkerHeightInWindowBands(int windowBandHeight, int width, int sizeLimitHintInBytes)
	{
		int num = sizeLimitHintInBytes / sizeof(Vector4);
		int num2 = windowBandHeight * width;
		return Math.Max(2, num / num2);
	}

	public static (Size Size, Rectangle Rectangle) CalculateTargetLocationAndBounds(Size sourceSize, ResizeOptions options)
	{
		int num = options.Size.Width;
		int num2 = options.Size.Height;
		if (num <= 0 && num2 <= 0)
		{
			ThrowInvalid($"Target width {num} and height {num2} must be greater than zero.");
		}
		if (num == 0 && num2 > 0)
		{
			num = (int)MathF.Max(1f, MathF.Round((float)(sourceSize.Width * num2) / (float)sourceSize.Height));
		}
		if (num2 == 0 && num > 0)
		{
			num2 = (int)MathF.Max(1f, MathF.Round((float)(sourceSize.Height * num) / (float)sourceSize.Width));
		}
		return options.Mode switch
		{
			ResizeMode.Crop => CalculateCropRectangle(sourceSize, options, num, num2), 
			ResizeMode.Pad => CalculatePadRectangle(sourceSize, options, num, num2), 
			ResizeMode.BoxPad => CalculateBoxPadRectangle(sourceSize, options, num, num2), 
			ResizeMode.Max => CalculateMaxRectangle(sourceSize, num, num2), 
			ResizeMode.Min => CalculateMinRectangle(sourceSize, num, num2), 
			ResizeMode.Manual => CalculateManualRectangle(options, num, num2), 
			_ => (Size: new Size(Sanitize(num), Sanitize(num2)), Rectangle: new Rectangle(0, 0, Sanitize(num), Sanitize(num2))), 
		};
	}

	private static (Size Size, Rectangle Rectangle) CalculateBoxPadRectangle(Size source, ResizeOptions options, int width, int height)
	{
		int width2 = source.Width;
		int height2 = source.Height;
		float num = MathF.Abs((float)height / (float)height2);
		float num2 = MathF.Abs((float)width / (float)width2);
		int num3 = ((height > 0) ? height : ((int)MathF.Round((float)height2 * num2)));
		int num4 = ((width > 0) ? width : ((int)MathF.Round((float)width2 * num)));
		if (width2 < num4 && height2 < num3)
		{
			int input = width2;
			int input2 = height2;
			width = num4;
			height = num3;
			int y;
			int x;
			switch (options.Position)
			{
			case AnchorPositionMode.Left:
				y = (int)((uint)(height - height2) / 2u);
				x = 0;
				break;
			case AnchorPositionMode.Right:
				y = (int)((uint)(height - height2) / 2u);
				x = width - width2;
				break;
			case AnchorPositionMode.TopRight:
				y = 0;
				x = width - width2;
				break;
			case AnchorPositionMode.Top:
				y = 0;
				x = (int)((uint)(width - width2) / 2u);
				break;
			case AnchorPositionMode.TopLeft:
				y = 0;
				x = 0;
				break;
			case AnchorPositionMode.BottomRight:
				y = height - height2;
				x = width - width2;
				break;
			case AnchorPositionMode.Bottom:
				y = height - height2;
				x = (int)((uint)(width - width2) / 2u);
				break;
			case AnchorPositionMode.BottomLeft:
				y = height - height2;
				x = 0;
				break;
			default:
				y = (int)((uint)(height - height2) / 2u);
				x = (int)((uint)(width - width2) / 2u);
				break;
			}
			return (Size: new Size(Sanitize(width), Sanitize(height)), Rectangle: new Rectangle(x, y, Sanitize(input), Sanitize(input2)));
		}
		return CalculatePadRectangle(source, options, width, height);
	}

	private static (Size Size, Rectangle Rectangle) CalculateCropRectangle(Size source, ResizeOptions options, int width, int height)
	{
		int width2 = source.Width;
		int height2 = source.Height;
		int num = 0;
		int num2 = 0;
		int input = width;
		int input2 = height;
		float num3 = MathF.Abs((float)height / (float)height2);
		float num4 = MathF.Abs((float)width / (float)width2);
		if (num3 < num4)
		{
			float num5 = num4;
			if (options.CenterCoordinates.HasValue)
			{
				num2 = (int)MathF.Round((0f - num5 * (float)height2) * options.CenterCoordinates.Value.Y + (float)height / 2f);
				if (num2 > 0)
				{
					num2 = 0;
				}
				if (num2 < (int)MathF.Round((float)height - (float)height2 * num5))
				{
					num2 = (int)MathF.Round((float)height - (float)height2 * num5);
				}
			}
			else
			{
				switch (options.Position)
				{
				case AnchorPositionMode.Top:
				case AnchorPositionMode.TopLeft:
				case AnchorPositionMode.TopRight:
					num2 = 0;
					break;
				case AnchorPositionMode.Bottom:
				case AnchorPositionMode.BottomRight:
				case AnchorPositionMode.BottomLeft:
					num2 = (int)MathF.Round((float)height - (float)height2 * num5);
					break;
				default:
					num2 = (int)MathF.Round(((float)height - (float)height2 * num5) / 2f);
					break;
				}
			}
			input2 = (int)MathF.Ceiling((float)height2 * num4);
		}
		else
		{
			float num5 = num3;
			if (options.CenterCoordinates.HasValue)
			{
				num = (int)MathF.Round((0f - num5 * (float)width2) * options.CenterCoordinates.Value.X + (float)width / 2f);
				if (num > 0)
				{
					num = 0;
				}
				if (num < (int)MathF.Round((float)width - (float)width2 * num5))
				{
					num = (int)MathF.Round((float)width - (float)width2 * num5);
				}
			}
			else
			{
				switch (options.Position)
				{
				case AnchorPositionMode.Left:
				case AnchorPositionMode.TopLeft:
				case AnchorPositionMode.BottomLeft:
					num = 0;
					break;
				case AnchorPositionMode.Right:
				case AnchorPositionMode.TopRight:
				case AnchorPositionMode.BottomRight:
					num = (int)MathF.Round((float)width - (float)width2 * num5);
					break;
				default:
					num = (int)MathF.Round(((float)width - (float)width2 * num5) / 2f);
					break;
				}
			}
			input = (int)MathF.Ceiling((float)width2 * num3);
		}
		return (Size: new Size(Sanitize(width), Sanitize(height)), Rectangle: new Rectangle(num, num2, Sanitize(input), Sanitize(input2)));
	}

	private static (Size Size, Rectangle Rectangle) CalculateMaxRectangle(Size source, int width, int height)
	{
		int input = width;
		int input2 = height;
		float num = MathF.Abs((float)height / (float)source.Height);
		float num2 = MathF.Abs((float)width / (float)source.Width);
		float num3 = (float)height / (float)width;
		if ((float)source.Height / (float)source.Width < num3)
		{
			input2 = (int)MathF.Round((float)source.Height * num2);
		}
		else
		{
			input = (int)MathF.Round((float)source.Width * num);
		}
		return (Size: new Size(Sanitize(input), Sanitize(input2)), Rectangle: new Rectangle(0, 0, Sanitize(input), Sanitize(input2)));
	}

	private static (Size Size, Rectangle Rectangle) CalculateMinRectangle(Size source, int width, int height)
	{
		int width2 = source.Width;
		int height2 = source.Height;
		int input = width;
		int input2 = height;
		if (width > width2 || height > height2)
		{
			return (Size: new Size(width2, height2), Rectangle: new Rectangle(0, 0, width2, height2));
		}
		int num = width2 - width;
		int num2 = height2 - height;
		if (num < num2)
		{
			float num3 = (float)height2 / (float)width2;
			input2 = (int)MathF.Round((float)width * num3);
		}
		else if (num > num2)
		{
			float num4 = (float)width2 / (float)height2;
			input = (int)MathF.Round((float)height * num4);
		}
		else if (height > width)
		{
			float num5 = MathF.Abs((float)width / (float)width2);
			input2 = (int)MathF.Round((float)height2 * num5);
		}
		else
		{
			float num6 = MathF.Abs((float)height / (float)height2);
			input = (int)MathF.Round((float)width2 * num6);
		}
		return (Size: new Size(Sanitize(input), Sanitize(input2)), Rectangle: new Rectangle(0, 0, Sanitize(input), Sanitize(input2)));
	}

	private static (Size Size, Rectangle Rectangle) CalculatePadRectangle(Size sourceSize, ResizeOptions options, int width, int height)
	{
		int width2 = sourceSize.Width;
		int height2 = sourceSize.Height;
		int x = 0;
		int y = 0;
		int input = width;
		int input2 = height;
		float num = MathF.Abs((float)height / (float)height2);
		float num2 = MathF.Abs((float)width / (float)width2);
		if (num < num2)
		{
			float num3 = num;
			input = (int)MathF.Round((float)width2 * num);
			switch (options.Position)
			{
			case AnchorPositionMode.Left:
			case AnchorPositionMode.TopLeft:
			case AnchorPositionMode.BottomLeft:
				x = 0;
				break;
			case AnchorPositionMode.Right:
			case AnchorPositionMode.TopRight:
			case AnchorPositionMode.BottomRight:
				x = (int)MathF.Round((float)width - (float)width2 * num3);
				break;
			default:
				x = (int)MathF.Round(((float)width - (float)width2 * num3) / 2f);
				break;
			}
		}
		else
		{
			float num3 = num2;
			input2 = (int)MathF.Round((float)height2 * num2);
			switch (options.Position)
			{
			case AnchorPositionMode.Top:
			case AnchorPositionMode.TopLeft:
			case AnchorPositionMode.TopRight:
				y = 0;
				break;
			case AnchorPositionMode.Bottom:
			case AnchorPositionMode.BottomRight:
			case AnchorPositionMode.BottomLeft:
				y = (int)MathF.Round((float)height - (float)height2 * num3);
				break;
			default:
				y = (int)MathF.Round(((float)height - (float)height2 * num3) / 2f);
				break;
			}
		}
		return (Size: new Size(Sanitize(width), Sanitize(height)), Rectangle: new Rectangle(x, y, Sanitize(input), Sanitize(input2)));
	}

	private static (Size Size, Rectangle Rectangle) CalculateManualRectangle(ResizeOptions options, int width, int height)
	{
		if (!options.TargetRectangle.HasValue)
		{
			ThrowInvalid("Manual resizing requires a target location and size.");
		}
		Rectangle value = options.TargetRectangle.Value;
		int x = value.X;
		int y = value.Y;
		int input = ((value.Width > 0) ? value.Width : width);
		int input2 = ((value.Height > 0) ? value.Height : height);
		return (Size: new Size(Sanitize(width), Sanitize(height)), Rectangle: new Rectangle(x, y, Sanitize(input), Sanitize(input2)));
	}

	[DoesNotReturn]
	private static void ThrowInvalid(string message)
	{
		throw new InvalidOperationException(message);
	}

	private static int Sanitize(int input)
	{
		return Math.Max(1, input);
	}
}
