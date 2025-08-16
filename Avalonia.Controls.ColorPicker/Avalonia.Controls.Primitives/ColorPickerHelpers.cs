using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Utilities;

namespace Avalonia.Controls.Primitives;

internal static class ColorPickerHelpers
{
	public static async Task<ArrayList<byte>> CreateComponentBitmapAsync(int width, int height, Orientation orientation, ColorModel colorModel, ColorComponent component, HsvColor baseHsvColor, bool isAlphaVisible, bool isPerceptive)
	{
		if (width == 0 || height == 0)
		{
			return new ArrayList<byte>(0);
		}
		return await Task.Run(delegate
		{
			int num = 0;
			Color baseRgbColor = Colors.White;
			ArrayList<byte> arrayList = new ArrayList<byte>(width * height * 4);
			_ = height;
			int num2 = width * 4;
			if (!isAlphaVisible && component != 0)
			{
				baseHsvColor = new HsvColor(1.0, baseHsvColor.H, baseHsvColor.S, baseHsvColor.V);
			}
			if (colorModel == ColorModel.Rgba)
			{
				baseRgbColor = baseHsvColor.ToRgb();
			}
			if (isPerceptive && component != 0)
			{
				if (colorModel == ColorModel.Hsva)
				{
					switch (component)
					{
					case ColorComponent.Component1:
						baseHsvColor = new HsvColor(baseHsvColor.A, baseHsvColor.H, 1.0, 1.0);
						break;
					case ColorComponent.Component2:
						baseHsvColor = new HsvColor(baseHsvColor.A, baseHsvColor.H, baseHsvColor.S, 1.0);
						break;
					case ColorComponent.Component3:
						baseHsvColor = new HsvColor(baseHsvColor.A, baseHsvColor.H, 1.0, baseHsvColor.V);
						break;
					}
				}
				else
				{
					switch (component)
					{
					case ColorComponent.Component1:
						baseRgbColor = new Color(baseRgbColor.A, baseRgbColor.R, 0, 0);
						break;
					case ColorComponent.Component2:
						baseRgbColor = new Color(baseRgbColor.A, 0, baseRgbColor.G, 0);
						break;
					case ColorComponent.Component3:
						baseRgbColor = new Color(baseRgbColor.A, 0, 0, baseRgbColor.B);
						break;
					}
				}
			}
			if (orientation == Orientation.Horizontal)
			{
				double num3 = ((colorModel != 0) ? (255.0 / (double)width) : ((component != ColorComponent.Component1) ? (1.0 / (double)width) : (360.0 / (double)width)));
				for (int i = 0; i < height; i++)
				{
					for (int j = 0; j < width; j++)
					{
						if (i == 0)
						{
							Color color = GetColor((double)j * num3);
							arrayList[num] = Convert.ToByte(color.B * color.A / 255);
							arrayList[num + 1] = Convert.ToByte(color.G * color.A / 255);
							arrayList[num + 2] = Convert.ToByte(color.R * color.A / 255);
							arrayList[num + 3] = color.A;
						}
						else
						{
							arrayList[num] = arrayList[num - num2];
							arrayList[num + 1] = arrayList[num + 1 - num2];
							arrayList[num + 2] = arrayList[num + 2 - num2];
							arrayList[num + 3] = arrayList[num + 3 - num2];
						}
						num += 4;
					}
				}
			}
			else
			{
				double num3 = ((colorModel != 0) ? (255.0 / (double)height) : ((component != ColorComponent.Component1) ? (1.0 / (double)height) : (360.0 / (double)height)));
				for (int k = 0; k < height; k++)
				{
					for (int l = 0; l < width; l++)
					{
						if (l == 0)
						{
							Color color = GetColor((double)(height - 1 - k) * num3);
							arrayList[num] = Convert.ToByte(color.B * color.A / 255);
							arrayList[num + 1] = Convert.ToByte(color.G * color.A / 255);
							arrayList[num + 2] = Convert.ToByte(color.R * color.A / 255);
							arrayList[num + 3] = color.A;
						}
						else
						{
							arrayList[num] = arrayList[num - 4];
							arrayList[num + 1] = arrayList[num - 3];
							arrayList[num + 2] = arrayList[num - 2];
							arrayList[num + 3] = arrayList[num - 1];
						}
						num += 4;
					}
				}
			}
			return arrayList;
		});
		Color GetColor(double componentValue)
		{
			Color result = Colors.White;
			switch (component)
			{
			case ColorComponent.Component1:
				result = ((colorModel != 0) ? new Color(P_1.baseRgbColor.A, Convert.ToByte(MathUtilities.Clamp(componentValue, 0.0, 255.0)), P_1.baseRgbColor.G, P_1.baseRgbColor.B) : HsvColor.ToRgb(MathUtilities.Clamp(componentValue, 0.0, 360.0), baseHsvColor.S, baseHsvColor.V, baseHsvColor.A));
				break;
			case ColorComponent.Component2:
				result = ((colorModel != 0) ? new Color(P_1.baseRgbColor.A, P_1.baseRgbColor.R, Convert.ToByte(MathUtilities.Clamp(componentValue, 0.0, 255.0)), P_1.baseRgbColor.B) : HsvColor.ToRgb(baseHsvColor.H, MathUtilities.Clamp(componentValue, 0.0, 1.0), baseHsvColor.V, baseHsvColor.A));
				break;
			case ColorComponent.Component3:
				result = ((colorModel != 0) ? new Color(P_1.baseRgbColor.A, P_1.baseRgbColor.R, P_1.baseRgbColor.G, Convert.ToByte(MathUtilities.Clamp(componentValue, 0.0, 255.0))) : HsvColor.ToRgb(baseHsvColor.H, baseHsvColor.S, MathUtilities.Clamp(componentValue, 0.0, 1.0), baseHsvColor.A));
				break;
			case ColorComponent.Alpha:
				result = ((colorModel != 0) ? new Color(Convert.ToByte(MathUtilities.Clamp(componentValue, 0.0, 255.0)), P_1.baseRgbColor.R, P_1.baseRgbColor.G, P_1.baseRgbColor.B) : HsvColor.ToRgb(baseHsvColor.H, baseHsvColor.S, baseHsvColor.V, MathUtilities.Clamp(componentValue, 0.0, 1.0)));
				break;
			}
			return result;
		}
	}

	public static Hsv IncrementColorComponent(Hsv originalHsv, HsvComponent component, IncrementDirection direction, IncrementAmount amount, bool shouldWrap, double minBound, double maxBound)
	{
		Hsv result = originalHsv;
		if (amount == IncrementAmount.Small || !ColorHelper.ToDisplayNameExists)
		{
			result.S *= 100.0;
			result.V *= 100.0;
			ref double h = ref result.H;
			double num = 0.0;
			switch (component)
			{
			case HsvComponent.Hue:
				h = ref result.H;
				num = ((amount == IncrementAmount.Small) ? 1 : 30);
				break;
			case HsvComponent.Saturation:
				h = ref result.S;
				num = ((amount == IncrementAmount.Small) ? 1 : 10);
				break;
			case HsvComponent.Value:
				h = ref result.V;
				num = ((amount == IncrementAmount.Small) ? 1 : 10);
				break;
			default:
				throw new InvalidOperationException("Invalid HsvComponent.");
			}
			double num2 = h;
			h += ((direction == IncrementDirection.Lower) ? (0.0 - num) : num);
			if (h < minBound)
			{
				h = ((shouldWrap && num2 == minBound) ? maxBound : minBound);
			}
			if (h > maxBound)
			{
				h = ((shouldWrap && num2 == maxBound) ? minBound : maxBound);
			}
			result.S /= 100.0;
			result.V /= 100.0;
		}
		else
		{
			if (component == HsvComponent.Saturation || component == HsvComponent.Value)
			{
				minBound /= 100.0;
				maxBound /= 100.0;
			}
			result = FindNextNamedColor(originalHsv, component, direction, shouldWrap, minBound, maxBound);
		}
		return result;
	}

	public static Hsv FindNextNamedColor(Hsv originalHsv, HsvComponent component, IncrementDirection direction, bool shouldWrap, double minBound, double maxBound)
	{
		Hsv hsv = originalHsv;
		string text = ColorHelper.ToDisplayName(originalHsv.ToRgb().ToColor());
		string text2 = text;
		double num = 0.0;
		ref double h = ref hsv.H;
		double num2 = 0.0;
		switch (component)
		{
		case HsvComponent.Hue:
			num = originalHsv.H;
			h = ref hsv.H;
			num2 = 1.0;
			break;
		case HsvComponent.Saturation:
			num = originalHsv.S;
			h = ref hsv.S;
			num2 = 0.01;
			break;
		case HsvComponent.Value:
			num = originalHsv.V;
			h = ref hsv.V;
			num2 = 0.01;
			break;
		default:
			throw new InvalidOperationException("Invalid HsvComponent.");
		}
		bool flag = true;
		while (text2 == text)
		{
			double num3 = h;
			h += (double)((direction != 0) ? 1 : (-1)) * num2;
			bool flag2 = false;
			if (h > maxBound)
			{
				if (!shouldWrap)
				{
					h = maxBound;
					flag = false;
					text2 = ColorHelper.ToDisplayName(hsv.ToRgb().ToColor());
					break;
				}
				h = minBound;
				flag2 = true;
			}
			else if (h < minBound)
			{
				if (!shouldWrap)
				{
					h = minBound;
					flag = false;
					text2 = ColorHelper.ToDisplayName(hsv.ToRgb().ToColor());
					break;
				}
				h = maxBound;
				flag2 = true;
			}
			if (!flag2 && num3 != num && Math.Sign(h - num) != Math.Sign(num3 - num))
			{
				flag = false;
				break;
			}
			text2 = ColorHelper.ToDisplayName(hsv.ToRgb().ToColor());
		}
		if (flag)
		{
			Hsv hsv2 = hsv;
			Hsv hsv3 = hsv2;
			double num4 = 0.0;
			string text3 = text2;
			ref double h2 = ref hsv2.H;
			ref double h3 = ref hsv3.H;
			double num5 = 0.0;
			switch (component)
			{
			case HsvComponent.Hue:
				h2 = ref hsv2.H;
				h3 = ref hsv3.H;
				num5 = 360.0;
				break;
			case HsvComponent.Saturation:
				h2 = ref hsv2.S;
				h3 = ref hsv3.S;
				num5 = 1.0;
				break;
			case HsvComponent.Value:
				h2 = ref hsv2.V;
				h3 = ref hsv3.V;
				num5 = 1.0;
				break;
			default:
				throw new InvalidOperationException("Invalid HsvComponent.");
			}
			while (text2 == text3)
			{
				h3 += (double)((direction != 0) ? 1 : (-1)) * num2;
				if (h3 > maxBound)
				{
					if (!shouldWrap)
					{
						h3 = maxBound;
						break;
					}
					h3 = minBound;
					num4 = maxBound - minBound;
				}
				else if (h3 < minBound)
				{
					if (!shouldWrap)
					{
						h3 = minBound;
						break;
					}
					h3 = maxBound;
					num4 = minBound - maxBound;
				}
				text3 = ColorHelper.ToDisplayName(hsv3.ToRgb().ToColor());
			}
			h = (h2 + h3 + num4) / 2.0;
			double num6;
			for (num6 = Math.Abs(h); num6 > num2; num6 -= num2)
			{
			}
			h -= num6;
			while (h < minBound)
			{
				h += num5;
			}
			while (h > maxBound)
			{
				h -= num5;
			}
		}
		return hsv;
	}

	public static WriteableBitmap CreateBitmapFromPixelData(ArrayList<byte> bgraPixelData, int pixelWidth, int pixelHeight)
	{
		WriteableBitmap writeableBitmap = new WriteableBitmap(dpi: new Vector(96.0, 96.0), size: new PixelSize(pixelWidth, pixelHeight), format: PixelFormat.Bgra8888, alphaFormat: AlphaFormat.Premul);
		using ILockedFramebuffer lockedFramebuffer = writeableBitmap.Lock();
		Marshal.Copy(bgraPixelData.Array, 0, lockedFramebuffer.Address, bgraPixelData.Array.Length);
		return writeableBitmap;
	}

	public static void UpdateBitmapFromPixelData(WriteableBitmap bitmap, ArrayList<byte> bgraPixelData)
	{
		using ILockedFramebuffer lockedFramebuffer = bitmap.Lock();
		Marshal.Copy(bgraPixelData.Array, 0, lockedFramebuffer.Address, bgraPixelData.Array.Length);
	}
}
