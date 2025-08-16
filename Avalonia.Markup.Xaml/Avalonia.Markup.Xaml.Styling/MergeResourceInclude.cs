using System;
using System.Diagnostics.CodeAnalysis;

namespace Avalonia.Markup.Xaml.Styling;

[RequiresUnreferencedCode("StyleInclude and ResourceInclude use AvaloniaXamlLoader.Load which dynamically loads referenced assembly with Avalonia resources. Note, StyleInclude and ResourceInclude defined in XAML are resolved compile time and are safe with trimming and AOT.")]
public class MergeResourceInclude : ResourceInclude
{
	public MergeResourceInclude(Uri? baseUri)
		: base(baseUri)
	{
	}

	public MergeResourceInclude(IServiceProvider serviceProvider)
		: base(serviceProvider)
	{
	}
}
