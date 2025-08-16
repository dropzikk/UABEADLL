using System;
using System.Collections.Generic;
using Avalonia.FreeDesktop.DBusIme.Fcitx;
using Avalonia.FreeDesktop.DBusIme.IBus;
using Tmds.DBus.Protocol;

namespace Avalonia.FreeDesktop.DBusIme;

internal class X11DBusImeHelper
{
	private static readonly Dictionary<string, Func<Connection, IX11InputMethodFactory>> KnownMethods = new Dictionary<string, Func<Connection, IX11InputMethodFactory>>
	{
		["fcitx"] = (Connection conn) => new DBusInputMethodFactory<FcitxX11TextInputMethod>((IntPtr _) => new FcitxX11TextInputMethod(conn)),
		["ibus"] = (Connection conn) => new DBusInputMethodFactory<IBusX11TextInputMethod>((IntPtr _) => new IBusX11TextInputMethod(conn))
	};

	private static Func<Connection, IX11InputMethodFactory>? DetectInputMethod()
	{
		string[] array = new string[3] { "AVALONIA_IM_MODULE", "GTK_IM_MODULE", "QT_IM_MODULE" };
		for (int i = 0; i < array.Length; i++)
		{
			string environmentVariable = Environment.GetEnvironmentVariable(array[i]);
			if (environmentVariable == "none")
			{
				return null;
			}
			if (environmentVariable != null && KnownMethods.TryGetValue(environmentVariable, out Func<Connection, IX11InputMethodFactory> value))
			{
				return value;
			}
		}
		return null;
	}

	public static bool DetectAndRegister()
	{
		Func<Connection, IX11InputMethodFactory> func = DetectInputMethod();
		if (func != null)
		{
			Connection connection = DBusHelper.TryInitialize();
			if (connection != null)
			{
				AvaloniaLocator.CurrentMutable.Bind<IX11InputMethodFactory>().ToConstant(func(connection));
				return true;
			}
		}
		return false;
	}
}
