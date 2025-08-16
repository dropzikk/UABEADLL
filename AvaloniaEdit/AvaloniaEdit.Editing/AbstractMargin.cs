using System;
using Avalonia;
using Avalonia.Controls;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Editing;

public abstract class AbstractMargin : Control, ITextViewConnect
{
	public static readonly StyledProperty<TextView> TextViewProperty = AvaloniaProperty.Register<AbstractMargin, TextView>("TextView");

	private bool _wasAutoAddedToTextView;

	public TextView TextView
	{
		get
		{
			return GetValue(TextViewProperty);
		}
		set
		{
			SetValue(TextViewProperty, value);
		}
	}

	public TextDocument Document { get; private set; }

	protected TextArea TextArea { get; set; }

	public AbstractMargin()
	{
		this.GetPropertyChangedObservable(TextViewProperty).Subscribe(delegate(AvaloniaPropertyChangedEventArgs o)
		{
			_wasAutoAddedToTextView = false;
			OnTextViewChanged(o.OldValue as TextView, o.NewValue as TextView);
		});
	}

	void ITextViewConnect.AddToTextView(TextView textView)
	{
		if (TextView == null)
		{
			TextView = textView;
			_wasAutoAddedToTextView = true;
		}
		else if (TextView != textView)
		{
			throw new InvalidOperationException("This margin belongs to a different TextView.");
		}
	}

	void ITextViewConnect.RemoveFromTextView(TextView textView)
	{
		if (_wasAutoAddedToTextView && TextView == textView)
		{
			TextView = null;
		}
	}

	protected virtual void OnTextViewChanged(TextView oldTextView, TextView newTextView)
	{
		if (oldTextView != null)
		{
			oldTextView.DocumentChanged -= TextViewDocumentChanged;
		}
		if (newTextView != null)
		{
			newTextView.DocumentChanged += TextViewDocumentChanged;
		}
		TextViewDocumentChanged(null, null);
		if (oldTextView != null)
		{
			oldTextView.VisualLinesChanged -= TextViewVisualLinesChanged;
		}
		if (newTextView != null)
		{
			newTextView.VisualLinesChanged += TextViewVisualLinesChanged;
			TextArea = newTextView.GetService(typeof(TextArea)) as TextArea;
		}
		else
		{
			TextArea = null;
		}
	}

	protected virtual void OnTextViewVisualLinesChanged()
	{
		InvalidateVisual();
	}

	private void TextViewVisualLinesChanged(object sender, EventArgs e)
	{
		OnTextViewVisualLinesChanged();
	}

	private void TextViewDocumentChanged(object sender, EventArgs e)
	{
		OnDocumentChanged(Document, TextView?.Document);
	}

	protected virtual void OnDocumentChanged(TextDocument oldDocument, TextDocument newDocument)
	{
		Document = newDocument;
	}
}
