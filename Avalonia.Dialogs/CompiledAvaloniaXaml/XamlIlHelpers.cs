using System;
using Avalonia.Data.Core;
using Avalonia.Dialogs;

namespace CompiledAvaloniaXaml;

internal class XamlIlHelpers
{
	private static IPropertyInfo Avalonia_002EDialogs_002EAboutAvaloniaDialog_002CAvalonia_002EDialogs_002EVersion_0021Field;

	private static IPropertyInfo Avalonia_002EDialogs_002EAboutAvaloniaDialog_002CAvalonia_002EDialogs_002ECopyright_0021Field;

	static object Avalonia_002EDialogs_002EAboutAvaloniaDialog_002CAvalonia_002EDialogs_002EVersion_0021Getter(object P_0)
	{
		return AboutAvaloniaDialog.Version;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EAboutAvaloniaDialog_002CAvalonia_002EDialogs_002EVersion_0021Property()
	{
		if (Avalonia_002EDialogs_002EAboutAvaloniaDialog_002CAvalonia_002EDialogs_002EVersion_0021Field != null)
		{
			return Avalonia_002EDialogs_002EAboutAvaloniaDialog_002CAvalonia_002EDialogs_002EVersion_0021Field;
		}
		Avalonia_002EDialogs_002EAboutAvaloniaDialog_002CAvalonia_002EDialogs_002EVersion_0021Field = new ClrPropertyInfo("Version", (Func<object, object?>?)XamlIlHelpers.Avalonia_002EDialogs_002EAboutAvaloniaDialog_002CAvalonia_002EDialogs_002EVersion_0021Getter, (Action<object, object?>?)null, typeof(string));
		return Avalonia_002EDialogs_002EAboutAvaloniaDialog_002CAvalonia_002EDialogs_002EVersion_0021Field;
	}

	static object Avalonia_002EDialogs_002EAboutAvaloniaDialog_002CAvalonia_002EDialogs_002ECopyright_0021Getter(object P_0)
	{
		return AboutAvaloniaDialog.Copyright;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EAboutAvaloniaDialog_002CAvalonia_002EDialogs_002ECopyright_0021Property()
	{
		if (Avalonia_002EDialogs_002EAboutAvaloniaDialog_002CAvalonia_002EDialogs_002ECopyright_0021Field != null)
		{
			return Avalonia_002EDialogs_002EAboutAvaloniaDialog_002CAvalonia_002EDialogs_002ECopyright_0021Field;
		}
		Avalonia_002EDialogs_002EAboutAvaloniaDialog_002CAvalonia_002EDialogs_002ECopyright_0021Field = new ClrPropertyInfo("Copyright", (Func<object, object?>?)XamlIlHelpers.Avalonia_002EDialogs_002EAboutAvaloniaDialog_002CAvalonia_002EDialogs_002ECopyright_0021Getter, (Action<object, object?>?)null, typeof(string));
		return Avalonia_002EDialogs_002EAboutAvaloniaDialog_002CAvalonia_002EDialogs_002ECopyright_0021Field;
	}
}
