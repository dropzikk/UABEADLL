using System.Collections.Generic;
using System.Text;
using Avalonia.Data;
using Avalonia.Media.TextFormatting;
using Avalonia.Metadata;

namespace Avalonia.Controls.Documents;

public class Run : Inline
{
	public static readonly StyledProperty<string?> TextProperty = AvaloniaProperty.Register<Run, string>("Text", null, inherits: false, BindingMode.TwoWay);

	[Content]
	public string? Text
	{
		get
		{
			return GetValue(TextProperty);
		}
		set
		{
			SetValue(TextProperty, value);
		}
	}

	public Run()
	{
	}

	public Run(string? text)
	{
		Text = text;
	}

	internal override void BuildTextRun(IList<TextRun> textRuns)
	{
		string text = Text ?? "";
		if (!string.IsNullOrEmpty(text))
		{
			TextRunProperties textRunProperties = CreateTextRunProperties();
			TextCharacters item = new TextCharacters(text, textRunProperties);
			textRuns.Add(item);
		}
	}

	internal override void AppendText(StringBuilder stringBuilder)
	{
		string value = Text ?? "";
		stringBuilder.Append(value);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property.Name == "Text")
		{
			base.InlineHost?.Invalidate();
		}
	}
}
