using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform.Interop;

namespace Avalonia.X11.NativeDialogs;

internal static class Gtk
{
	public delegate bool signal_generic(IntPtr gtkWidget, IntPtr userData);

	public delegate bool signal_dialog_response(IntPtr gtkWidget, GtkResponseType response, IntPtr userData);

	private static IntPtr s_display;

	private const string GdkName = "libgdk-3.so.0";

	private const string GtkName = "libgtk-3.so.0";

	[DllImport("libgtk-3.so.0")]
	private static extern void gtk_main_iteration();

	[DllImport("libgtk-3.so.0")]
	public static extern void gtk_window_set_modal(IntPtr window, bool modal);

	[DllImport("libgtk-3.so.0")]
	public static extern void gtk_window_present(IntPtr gtkWindow);

	[DllImport("libgtk-3.so.0")]
	public static extern IntPtr gtk_file_chooser_dialog_new(Utf8Buffer title, IntPtr parent, GtkFileChooserAction action, IntPtr ignore);

	[DllImport("libgtk-3.so.0")]
	public static extern void gtk_file_chooser_set_select_multiple(IntPtr chooser, bool allow);

	[DllImport("libgtk-3.so.0")]
	public static extern void gtk_file_chooser_set_do_overwrite_confirmation(IntPtr chooser, bool do_overwrite_confirmation);

	[DllImport("libgtk-3.so.0")]
	public static extern void gtk_dialog_add_button(IntPtr raw, Utf8Buffer button_text, GtkResponseType response_id);

	[DllImport("libgtk-3.so.0")]
	public unsafe static extern GSList* gtk_file_chooser_get_filenames(IntPtr chooser);

	[DllImport("libgtk-3.so.0")]
	public static extern void gtk_file_chooser_set_filename(IntPtr chooser, Utf8Buffer file);

	[DllImport("libgtk-3.so.0")]
	public static extern void gtk_file_chooser_set_current_name(IntPtr chooser, Utf8Buffer file);

	[DllImport("libgtk-3.so.0")]
	public static extern void gtk_file_chooser_set_current_folder(IntPtr chooser, Utf8Buffer file);

	[DllImport("libgtk-3.so.0")]
	public static extern IntPtr gtk_file_filter_new();

	[DllImport("libgtk-3.so.0")]
	public static extern IntPtr gtk_file_filter_set_name(IntPtr filter, Utf8Buffer name);

	[DllImport("libgtk-3.so.0")]
	public static extern IntPtr gtk_file_filter_add_pattern(IntPtr filter, Utf8Buffer pattern);

	[DllImport("libgtk-3.so.0")]
	public static extern IntPtr gtk_file_filter_add_mime_type(IntPtr filter, Utf8Buffer mimeType);

	[DllImport("libgtk-3.so.0")]
	public static extern IntPtr gtk_file_chooser_add_filter(IntPtr chooser, IntPtr filter);

	[DllImport("libgtk-3.so.0")]
	public static extern IntPtr gtk_file_chooser_get_filter(IntPtr chooser);

	[DllImport("libgtk-3.so.0")]
	public static extern void gtk_widget_realize(IntPtr gtkWidget);

	[DllImport("libgtk-3.so.0")]
	public static extern void gtk_widget_destroy(IntPtr gtkWidget);

	[DllImport("libgtk-3.so.0")]
	public static extern IntPtr gtk_widget_get_window(IntPtr gtkWidget);

	[DllImport("libgtk-3.so.0")]
	public static extern void gtk_widget_hide(IntPtr gtkWidget);

	[DllImport("libgtk-3.so.0")]
	private static extern bool gtk_init_check(int argc, IntPtr argv);

	[DllImport("libgdk-3.so.0")]
	private static extern IntPtr gdk_x11_window_foreign_new_for_display(IntPtr display, IntPtr xid);

	[DllImport("libgdk-3.so.0")]
	public static extern IntPtr gdk_x11_window_get_xid(IntPtr window);

	[DllImport("libgtk-3.so.0")]
	public static extern IntPtr gtk_container_add(IntPtr container, IntPtr widget);

	[DllImport("libgdk-3.so.0")]
	private static extern IntPtr gdk_set_allowed_backends(Utf8Buffer backends);

	[DllImport("libgdk-3.so.0")]
	private static extern IntPtr gdk_display_get_default();

	[DllImport("libgtk-3.so.0")]
	private static extern IntPtr gtk_application_new(Utf8Buffer appId, int flags);

	[DllImport("libgdk-3.so.0")]
	public static extern void gdk_window_set_transient_for(IntPtr window, IntPtr parent);

	public static IntPtr GetForeignWindow(IntPtr xid)
	{
		return gdk_x11_window_foreign_new_for_display(s_display, xid);
	}

	public static Task<bool> StartGtk()
	{
		return StartGtkCore();
	}

	private static void GtkThread(TaskCompletionSource<bool> tcs)
	{
		try
		{
			try
			{
				using Utf8Buffer backends = new Utf8Buffer("x11");
				gdk_set_allowed_backends(backends);
			}
			catch
			{
			}
			Environment.SetEnvironmentVariable("WAYLAND_DISPLAY", "/proc/fake-display-to-prevent-wayland-initialization-by-gtk3");
			if (!gtk_init_check(0, IntPtr.Zero))
			{
				tcs.SetResult(result: false);
				return;
			}
			IntPtr intPtr;
			using (Utf8Buffer appId = new Utf8Buffer($"avalonia.app.a{Guid.NewGuid():N}"))
			{
				intPtr = gtk_application_new(appId, 0);
			}
			if (intPtr == IntPtr.Zero)
			{
				tcs.SetResult(result: false);
				return;
			}
			s_display = gdk_display_get_default();
			tcs.SetResult(result: true);
			while (true)
			{
				gtk_main_iteration();
			}
		}
		catch
		{
			tcs.SetResult(result: false);
		}
	}

	private static Task<bool> StartGtkCore()
	{
		TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
		Thread thread = new Thread((ThreadStart)delegate
		{
			GtkThread(tcs);
		});
		thread.Name = "GTK3THREAD";
		thread.IsBackground = true;
		thread.Start();
		return tcs.Task;
	}
}
