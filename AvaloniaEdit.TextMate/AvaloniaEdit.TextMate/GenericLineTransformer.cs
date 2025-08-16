using System;
using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;

namespace AvaloniaEdit.TextMate;

public abstract class GenericLineTransformer : DocumentColorizingTransformer
{
	private Action<Exception> _exceptionHandler;

	public GenericLineTransformer(Action<Exception> exceptionHandler)
	{
		_exceptionHandler = exceptionHandler;
	}

	protected override void ColorizeLine(DocumentLine line)
	{
		try
		{
			TransformLine(line, base.CurrentContext);
		}
		catch (Exception obj)
		{
			_exceptionHandler?.Invoke(obj);
		}
	}

	protected abstract void TransformLine(DocumentLine line, ITextRunConstructionContext context);

	public void SetTextStyle(DocumentLine line, int startIndex, int length, IBrush foreground, IBrush background, FontStyle fontStyle, FontWeight fontWeigth, bool isUnderline)
	{
		int num = 0;
		int num2 = 0;
		if (startIndex >= 0 && length > 0)
		{
			if (line.Offset + startIndex + length > line.EndOffset)
			{
				length = line.EndOffset - startIndex - line.Offset - startIndex;
			}
			num = line.Offset + startIndex;
			num2 = line.Offset + startIndex + length;
		}
		else
		{
			num = line.Offset;
			num2 = line.EndOffset;
		}
		if (num <= base.CurrentContext.Document.TextLength && num2 <= base.CurrentContext.Document.TextLength)
		{
			ChangeLinePart(num, num2, delegate(VisualLineElement visualLine)
			{
				ChangeVisualLine(visualLine, foreground, background, fontStyle, fontWeigth, isUnderline);
			});
		}
	}

	private void ChangeVisualLine(VisualLineElement visualLine, IBrush foreground, IBrush background, FontStyle fontStyle, FontWeight fontWeigth, bool isUnderline)
	{
		if (foreground != null)
		{
			visualLine.TextRunProperties.SetForegroundBrush(foreground);
		}
		if (background != null)
		{
			visualLine.TextRunProperties.SetBackgroundBrush(background);
		}
		if (isUnderline)
		{
			visualLine.TextRunProperties.SetTextDecorations(TextDecorations.Underline);
		}
		if (visualLine.TextRunProperties.Typeface.Style != fontStyle || visualLine.TextRunProperties.Typeface.Weight != fontWeigth)
		{
			visualLine.TextRunProperties.SetTypeface(new Typeface(visualLine.TextRunProperties.Typeface.FontFamily, fontStyle, fontWeigth));
		}
	}
}
