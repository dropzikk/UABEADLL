using System;
using System.Text;
using Avalonia.Collections;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using Avalonia.Utilities;

namespace Avalonia.Controls.Documents;

[WhitespaceSignificantCollection]
public class InlineCollection : AvaloniaList<Inline>
{
	private IAvaloniaList<ILogical>? _logicalChildren;

	private IInlineHost? _inlineHost;

	internal IAvaloniaList<ILogical>? LogicalChildren
	{
		get
		{
			return _logicalChildren;
		}
		set
		{
			IAvaloniaList<ILogical> logicalChildren = _logicalChildren;
			_logicalChildren = value;
			OnParentChanged(logicalChildren, value);
		}
	}

	internal IInlineHost? InlineHost
	{
		get
		{
			return _inlineHost;
		}
		set
		{
			_inlineHost = value;
			OnInlineHostChanged(value);
		}
	}

	public string? Text
	{
		get
		{
			if (base.Count == 0)
			{
				return null;
			}
			StringBuilder stringBuilder = StringBuilderCache.Acquire();
			using (AvaloniaList<Inline>.Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					enumerator.Current.AppendText(stringBuilder);
				}
			}
			return StringBuilderCache.GetStringAndRelease(stringBuilder);
		}
	}

	public event EventHandler? Invalidated;

	public InlineCollection()
	{
		base.ResetBehavior = ResetBehavior.Remove;
		this.ForEachItem(delegate(Inline x)
		{
			x.InlineHost = InlineHost;
			LogicalChildren?.Add(x);
			Invalidate();
		}, delegate(Inline x)
		{
			LogicalChildren?.Remove(x);
			x.InlineHost = InlineHost;
			Invalidate();
		}, delegate
		{
			throw new NotSupportedException();
		});
	}

	public override void Add(Inline inline)
	{
		if (InlineHost is TextBlock textBlock && !string.IsNullOrEmpty(textBlock.Text))
		{
			base.Add(new Run(textBlock.Text));
			textBlock.ClearTextInternal();
		}
		base.Add(inline);
	}

	public void Add(string text)
	{
		if (InlineHost is TextBlock { HasComplexContent: false } textBlock)
		{
			textBlock.Text += text;
		}
		else
		{
			Add(new Run(text));
		}
	}

	public void Add(Control control)
	{
		Add(new InlineUIContainer(control));
	}

	protected void Invalidate()
	{
		if (InlineHost != null)
		{
			InlineHost.Invalidate();
		}
		this.Invalidated?.Invoke(this, EventArgs.Empty);
	}

	private void OnParentChanged(IAvaloniaList<ILogical>? oldParent, IAvaloniaList<ILogical>? newParent)
	{
		using AvaloniaList<Inline>.Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Inline current = enumerator.Current;
			if (oldParent != newParent)
			{
				oldParent?.Remove(current);
				newParent?.Add(current);
			}
		}
	}

	private void OnInlineHostChanged(IInlineHost? inlineHost)
	{
		using AvaloniaList<Inline>.Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.InlineHost = inlineHost;
		}
	}
}
