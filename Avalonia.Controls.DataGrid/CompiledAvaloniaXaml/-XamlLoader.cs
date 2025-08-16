using System;
using System.ComponentModel;
using Avalonia.Markup.Xaml.XamlIl.Runtime;

namespace CompiledAvaloniaXaml;

[EditorBrowsable(EditorBrowsableState.Never)]
public class _0021XamlLoader
{
	public static object TryLoad(IServiceProvider P_0, string P_1)
	{
		if (string.Equals(P_1, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml", StringComparison.OrdinalIgnoreCase))
		{
			return _0021AvaloniaResources.Build_003A_002FThemes_002FFluent_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(P_0));
		}
		if (string.Equals(P_1, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml", StringComparison.OrdinalIgnoreCase))
		{
			return _0021AvaloniaResources.Build_003A_002FThemes_002FSimple_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(P_0));
		}
		return null;
	}

	public static object TryLoad(string P_0)
	{
		return TryLoad(null, P_0);
	}
}
