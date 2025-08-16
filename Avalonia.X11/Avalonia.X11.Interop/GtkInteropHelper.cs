using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.X11.NativeDialogs;

namespace Avalonia.X11.Interop;

public class GtkInteropHelper
{
	public static async Task<T> RunOnGlibThread<T>(Func<T> cb)
	{
		if (!(await Gtk.StartGtk().ConfigureAwait(continueOnCapturedContext: false)))
		{
			throw new Win32Exception("Unable to initialize GTK");
		}
		return await Glib.RunOnGlibThread(cb).ConfigureAwait(continueOnCapturedContext: false);
	}
}
