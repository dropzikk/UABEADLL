using System;
using System.Text;
using Avalonia.Controls.Presenters;
using Avalonia.Input.TextInput;
using Avalonia.Media.TextFormatting;
using Avalonia.Utilities;

namespace Avalonia.Controls;

internal class TextBoxTextInputMethodClient : TextInputMethodClient
{
	private TextBox? _parent;

	private TextPresenter? _presenter;

	public override Visual TextViewVisual => _presenter;

	public override string SurroundingText
	{
		get
		{
			if (_presenter == null || _parent == null)
			{
				return "";
			}
			if (_parent.CaretIndex != _presenter.CaretIndex)
			{
				_presenter.SetCurrentValue(TextPresenter.CaretIndexProperty, _parent.CaretIndex);
			}
			if (_parent.Text != _presenter.Text)
			{
				_presenter.SetCurrentValue(TextPresenter.TextProperty, _parent.Text);
			}
			int lineIndexFromCharacterIndex = _presenter.TextLayout.GetLineIndexFromCharacterIndex(_presenter.CaretIndex, trailingEdge: false);
			return GetTextLineText(_presenter.TextLayout.TextLines[lineIndexFromCharacterIndex]);
		}
	}

	public override Rect CursorRectangle
	{
		get
		{
			if (_parent == null || _presenter == null)
			{
				return default(Rect);
			}
			Matrix? matrix = _presenter.TransformToVisual(_parent);
			if (!matrix.HasValue)
			{
				return default(Rect);
			}
			return _presenter.GetCursorRectangle().TransformToAABB(matrix.Value);
		}
	}

	public override TextSelection Selection
	{
		get
		{
			if (_presenter == null || _parent == null)
			{
				return default(TextSelection);
			}
			int lineIndexFromCharacterIndex = _presenter.TextLayout.GetLineIndexFromCharacterIndex(_parent.CaretIndex, trailingEdge: false);
			int firstTextSourceIndex = _presenter.TextLayout.TextLines[lineIndexFromCharacterIndex].FirstTextSourceIndex;
			int start = Math.Max(0, _parent.SelectionStart - firstTextSourceIndex);
			int end = Math.Max(0, _parent.SelectionEnd - firstTextSourceIndex);
			return new TextSelection(start, end);
		}
		set
		{
			if (_parent != null && _presenter != null)
			{
				int lineIndexFromCharacterIndex = _presenter.TextLayout.GetLineIndexFromCharacterIndex(_parent.CaretIndex, trailingEdge: false);
				int firstTextSourceIndex = _presenter.TextLayout.TextLines[lineIndexFromCharacterIndex].FirstTextSourceIndex;
				int selectionStart = firstTextSourceIndex + value.Start;
				int selectionEnd = firstTextSourceIndex + value.End;
				_parent.SelectionStart = selectionStart;
				_parent.SelectionEnd = selectionEnd;
				RaiseSelectionChanged();
			}
		}
	}

	public override bool SupportsPreedit => true;

	public override bool SupportsSurroundingText => true;

	public void SetPresenter(TextPresenter? presenter, TextBox? parent)
	{
		if (_parent != null)
		{
			_parent.PropertyChanged -= OnParentPropertyChanged;
		}
		_parent = parent;
		if (_parent != null)
		{
			_parent.PropertyChanged += OnParentPropertyChanged;
		}
		TextPresenter presenter2 = _presenter;
		if (presenter2 != null)
		{
			presenter2.ClearValue(TextPresenter.PreeditTextProperty);
			presenter2.CaretBoundsChanged -= delegate
			{
				RaiseCursorRectangleChanged();
			};
		}
		_presenter = presenter;
		if (_presenter != null)
		{
			_presenter.CaretBoundsChanged += delegate
			{
				RaiseCursorRectangleChanged();
			};
		}
		RaiseTextViewVisualChanged();
		RaiseCursorRectangleChanged();
	}

	public override void SetPreeditText(string? preeditText)
	{
		if (_presenter != null && _parent != null)
		{
			_presenter.SetCurrentValue(TextPresenter.PreeditTextProperty, preeditText);
		}
	}

	private static string GetTextLineText(TextLine textLine)
	{
		StringBuilder stringBuilder = StringBuilderCache.Acquire(textLine.Length);
		foreach (TextRun textRun in textLine.TextRuns)
		{
			if (textRun.Length > 0)
			{
				stringBuilder.Append(textRun.Text.Span);
			}
		}
		string result = stringBuilder.ToString();
		StringBuilderCache.Release(stringBuilder);
		return result;
	}

	private void OnParentPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == TextBox.TextProperty)
		{
			RaiseSurroundingTextChanged();
		}
		if (e.Property == TextBox.SelectionStartProperty || e.Property == TextBox.SelectionEndProperty)
		{
			RaiseSelectionChanged();
		}
	}
}
