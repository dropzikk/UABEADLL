using System;
using Avalonia.Media;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.TextMate;

public class ForegroundTextTransformation : TextTransformation
{
	public interface IColorMap
	{
		IBrush GetBrush(int color);
	}

	public IColorMap ColorMap { get; set; }

	public Action<Exception> ExceptionHandler { get; set; }

	public int ForegroundColor { get; set; }

	public int BackgroundColor { get; set; }

	public int FontStyle { get; set; }

	public override void Transform(GenericLineTransformer transformer, DocumentLine line)
	{
		try
		{
			if (base.Length != 0)
			{
				int num = 0;
				int endOffset = line.EndOffset;
				if (base.StartOffset > line.Offset)
				{
					num = base.StartOffset - line.Offset;
				}
				if (base.EndOffset < line.EndOffset)
				{
					endOffset = base.EndOffset;
				}
				transformer.SetTextStyle(line, num, endOffset - line.Offset - num, ColorMap.GetBrush(ForegroundColor), ColorMap.GetBrush(BackgroundColor), GetFontStyle(), GetFontWeight(), IsUnderline());
			}
		}
		catch (Exception obj)
		{
			ExceptionHandler?.Invoke(obj);
		}
	}

	private FontStyle GetFontStyle()
	{
		if (FontStyle != -1 && (FontStyle & 1) != 0)
		{
			return Avalonia.Media.FontStyle.Italic;
		}
		return Avalonia.Media.FontStyle.Normal;
	}

	private FontWeight GetFontWeight()
	{
		if (FontStyle != -1 && (FontStyle & 2) != 0)
		{
			return FontWeight.Bold;
		}
		return FontWeight.Normal;
	}

	private bool IsUnderline()
	{
		if (FontStyle != -1 && (FontStyle & 4) != 0)
		{
			return true;
		}
		return false;
	}
}
