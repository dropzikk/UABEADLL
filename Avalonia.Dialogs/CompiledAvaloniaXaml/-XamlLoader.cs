using System;
using System.ComponentModel;
using Avalonia.Dialogs;

namespace CompiledAvaloniaXaml;

[EditorBrowsable(EditorBrowsableState.Never)]
public class _0021XamlLoader
{
	public static object TryLoad(IServiceProvider P_0, string P_1)
	{
		if (string.Equals(P_1, "avares://Avalonia.Dialogs/AboutAvaloniaDialog.xaml", StringComparison.OrdinalIgnoreCase))
		{
			return new AboutAvaloniaDialog();
		}
		return null;
	}

	public static object TryLoad(string P_0)
	{
		return TryLoad(null, P_0);
	}
}
