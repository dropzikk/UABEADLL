using System;

namespace Avalonia.Input.TextInput;

public abstract class TextInputMethodClient
{
	public abstract Visual TextViewVisual { get; }

	public abstract bool SupportsPreedit { get; }

	public abstract bool SupportsSurroundingText { get; }

	public abstract string SurroundingText { get; }

	public abstract Rect CursorRectangle { get; }

	public abstract TextSelection Selection { get; set; }

	public event EventHandler? TextViewVisualChanged;

	public event EventHandler? CursorRectangleChanged;

	public event EventHandler? SurroundingTextChanged;

	public event EventHandler? SelectionChanged;

	public virtual void SetPreeditText(string? preeditText)
	{
	}

	protected virtual void RaiseTextViewVisualChanged()
	{
		this.TextViewVisualChanged?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void RaiseCursorRectangleChanged()
	{
		this.CursorRectangleChanged?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void RaiseSurroundingTextChanged()
	{
		this.SurroundingTextChanged?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void RaiseSelectionChanged()
	{
		this.SelectionChanged?.Invoke(this, EventArgs.Empty);
	}
}
