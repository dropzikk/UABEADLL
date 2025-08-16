using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace Avalonia.Controls;

internal interface IContentControl
{
	object? Content { get; set; }

	IDataTemplate? ContentTemplate { get; set; }

	HorizontalAlignment HorizontalContentAlignment { get; set; }

	VerticalAlignment VerticalContentAlignment { get; set; }
}
