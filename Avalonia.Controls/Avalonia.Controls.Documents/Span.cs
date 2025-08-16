using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Media.TextFormatting;
using Avalonia.Metadata;

namespace Avalonia.Controls.Documents;

public class Span : Inline
{
	public static readonly StyledProperty<InlineCollection> InlinesProperty = AvaloniaProperty.Register<Span, InlineCollection>("Inlines");

	[Content]
	public InlineCollection Inlines
	{
		get
		{
			return GetValue(InlinesProperty);
		}
		set
		{
			SetValue(InlinesProperty, value);
		}
	}

	public Span()
	{
		Inlines = new InlineCollection
		{
			LogicalChildren = base.LogicalChildren
		};
	}

	internal override void BuildTextRun(IList<TextRun> textRuns)
	{
		foreach (Inline inline in Inlines)
		{
			inline.BuildTextRun(textRuns);
		}
	}

	internal override void AppendText(StringBuilder stringBuilder)
	{
		foreach (Inline inline in Inlines)
		{
			inline.AppendText(stringBuilder);
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property.Name == "InlinesProperty")
		{
			OnInlinesChanged(change.OldValue as InlineCollection, change.NewValue as InlineCollection);
			base.InlineHost?.Invalidate();
		}
	}

	internal override void OnInlineHostChanged(IInlineHost? oldValue, IInlineHost? newValue)
	{
		base.OnInlineHostChanged(oldValue, newValue);
		Inlines.InlineHost = newValue;
	}

	private void OnInlinesChanged(InlineCollection? oldValue, InlineCollection? newValue)
	{
		if (oldValue != null)
		{
			oldValue.LogicalChildren = null;
			oldValue.InlineHost = null;
			oldValue.Invalidated -= OnInlinesInvalidated;
		}
		if (newValue != null)
		{
			newValue.LogicalChildren = base.LogicalChildren;
			newValue.InlineHost = base.InlineHost;
			newValue.Invalidated += OnInlinesInvalidated;
		}
		void OnInlinesInvalidated(object? sender, EventArgs e)
		{
			base.InlineHost?.Invalidate();
		}
	}
}
