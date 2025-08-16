using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Media.TextFormatting;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Folding;

public sealed class FoldingElementGenerator : VisualLineElementGenerator, ITextViewConnect
{
	private sealed class FoldingLineElement : FormattedTextElement
	{
		private readonly FoldingSection _fs;

		private readonly IBrush _textBrush;

		public FoldingLineElement(FoldingSection fs, TextLine text, int documentLength, IBrush textBrush)
			: base(text, documentLength)
		{
			_fs = fs;
			_textBrush = textBrush;
		}

		public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
		{
			return new FoldingLineTextRun(this, base.TextRunProperties, _textBrush);
		}

		protected internal override void OnPointerPressed(PointerPressedEventArgs e)
		{
			_fs.IsFolded = false;
			e.Handled = true;
		}
	}

	private sealed class FoldingLineTextRun : FormattedTextRun
	{
		private readonly IBrush _textBrush;

		public FoldingLineTextRun(FormattedTextElement element, TextRunProperties properties, IBrush textBrush)
			: base(element, properties)
		{
			_textBrush = textBrush;
		}

		public override void Draw(DrawingContext drawingContext, Point origin)
		{
			var (width, height) = (Size)(ref Size);
			drawingContext.DrawRectangle(rect: new Rect(origin.X, origin.Y, width, height), pen: new ImmutablePen(_textBrush.ToImmutable()));
			base.Draw(drawingContext, origin);
		}
	}

	private readonly List<TextView> _textViews = new List<TextView>();

	private FoldingManager _foldingManager;

	public FoldingManager FoldingManager
	{
		get
		{
			return _foldingManager;
		}
		set
		{
			if (_foldingManager == value)
			{
				return;
			}
			if (_foldingManager != null)
			{
				foreach (TextView textView in _textViews)
				{
					_foldingManager.RemoveFromTextView(textView);
				}
			}
			_foldingManager = value;
			if (_foldingManager == null)
			{
				return;
			}
			foreach (TextView textView2 in _textViews)
			{
				_foldingManager.AddToTextView(textView2);
			}
		}
	}

	public static IBrush DefaultTextBrush { get; } = Brushes.Gray;

	public static IBrush TextBrush { get; set; } = DefaultTextBrush;

	void ITextViewConnect.AddToTextView(TextView textView)
	{
		_textViews.Add(textView);
		_foldingManager?.AddToTextView(textView);
	}

	void ITextViewConnect.RemoveFromTextView(TextView textView)
	{
		_textViews.Remove(textView);
		_foldingManager?.RemoveFromTextView(textView);
	}

	public override void StartGeneration(ITextRunConstructionContext context)
	{
		base.StartGeneration(context);
		if (_foldingManager != null)
		{
			if (!_foldingManager.TextViews.Contains(context.TextView))
			{
				throw new ArgumentException("Invalid TextView");
			}
			if (context.Document != _foldingManager.Document)
			{
				throw new ArgumentException("Invalid document");
			}
		}
	}

	public override int GetFirstInterestedOffset(int startOffset)
	{
		if (_foldingManager != null)
		{
			foreach (FoldingSection item in _foldingManager.GetFoldingsContaining(startOffset))
			{
				if (item.IsFolded)
				{
					_ = item.EndOffset;
				}
			}
			return _foldingManager.GetNextFoldedFoldingStart(startOffset);
		}
		return -1;
	}

	public override VisualLineElement ConstructElement(int offset)
	{
		if (_foldingManager == null)
		{
			return null;
		}
		int num = -1;
		FoldingSection foldingSection = null;
		foreach (FoldingSection item in _foldingManager.GetFoldingsContaining(offset))
		{
			if (item.IsFolded && item.EndOffset > num)
			{
				num = item.EndOffset;
				foldingSection = item;
			}
		}
		if (num > offset && foldingSection != null)
		{
			bool flag;
			do
			{
				flag = false;
				foreach (FoldingSection item2 in FoldingManager.GetFoldingsContaining(num))
				{
					if (item2.IsFolded && item2.EndOffset > num)
					{
						num = item2.EndOffset;
						flag = true;
					}
				}
			}
			while (flag);
			string text = foldingSection.Title;
			if (string.IsNullOrEmpty(text))
			{
				text = "...";
			}
			VisualLineElementTextRunProperties visualLineElementTextRunProperties = new VisualLineElementTextRunProperties(base.CurrentContext.GlobalTextRunProperties);
			visualLineElementTextRunProperties.SetForegroundBrush(TextBrush);
			TextLine text2 = FormattedTextElement.PrepareText(TextFormatterFactory.Create(base.CurrentContext.TextView), text, visualLineElementTextRunProperties);
			return new FoldingLineElement(foldingSection, text2, num - offset, TextBrush);
		}
		return null;
	}
}
