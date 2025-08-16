using System.Collections.Generic;
using System.Text;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Metadata;

namespace Avalonia.Controls.Documents;

public class InlineUIContainer : Inline
{
	public static readonly StyledProperty<Control> ChildProperty;

	[Content]
	public Control Child
	{
		get
		{
			return GetValue(ChildProperty);
		}
		set
		{
			SetValue(ChildProperty, value);
		}
	}

	static InlineUIContainer()
	{
		ChildProperty = AvaloniaProperty.Register<InlineUIContainer, Control>("Child");
		Inline.BaselineAlignmentProperty.OverrideDefaultValue<InlineUIContainer>(BaselineAlignment.Top);
	}

	public InlineUIContainer()
	{
	}

	public InlineUIContainer(Control child)
	{
		Child = child;
	}

	internal override void BuildTextRun(IList<TextRun> textRuns)
	{
		textRuns.Add(new EmbeddedControlRun(Child, CreateTextRunProperties()));
	}

	internal override void AppendText(StringBuilder stringBuilder)
	{
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == ChildProperty)
		{
			if (change.OldValue is Control item)
			{
				base.LogicalChildren.Remove(item);
			}
			if (change.NewValue is Control item2)
			{
				base.LogicalChildren.Add(item2);
			}
		}
	}
}
