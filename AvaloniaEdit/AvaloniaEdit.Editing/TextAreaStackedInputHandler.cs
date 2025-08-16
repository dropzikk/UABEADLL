using System;
using Avalonia.Input;

namespace AvaloniaEdit.Editing;

public abstract class TextAreaStackedInputHandler : ITextAreaInputHandler
{
	public TextArea TextArea { get; }

	protected TextAreaStackedInputHandler(TextArea textArea)
	{
		TextArea = textArea ?? throw new ArgumentNullException("textArea");
	}

	public virtual void Attach()
	{
	}

	public virtual void Detach()
	{
	}

	public virtual void OnPreviewKeyDown(KeyEventArgs e)
	{
	}

	public virtual void OnPreviewKeyUp(KeyEventArgs e)
	{
	}
}
