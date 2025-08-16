using System;
using System.ComponentModel;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Avalonia.Themes.Simple;

namespace CompiledAvaloniaXaml;

[EditorBrowsable(EditorBrowsableState.Never)]
public class _0021XamlLoader
{
	public static object TryLoad(IServiceProvider P_0, string P_1)
	{
		if (string.Equals(P_1, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml", StringComparison.OrdinalIgnoreCase))
		{
			return new SimpleTheme(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(P_0));
		}
		return null;
	}

	public static object TryLoad(string P_0)
	{
		return TryLoad(null, P_0);
	}
}
