using Avalonia.Interactivity;

namespace Avalonia.Controls.Primitives;

public static class TextSearch
{
	public static readonly AttachedProperty<string?> TextProperty = AvaloniaProperty.RegisterAttached<Interactive, string>("Text", typeof(TextSearch));

	public static void SetText(Control control, string? text)
	{
		control.SetValue(TextProperty, text);
	}

	public static string? GetText(Control control)
	{
		return control.GetValue(TextProperty);
	}
}
