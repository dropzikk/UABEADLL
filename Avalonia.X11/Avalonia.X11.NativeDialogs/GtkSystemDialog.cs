using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform;
using Avalonia.Platform.Interop;
using Avalonia.Platform.Storage;
using Avalonia.Platform.Storage.FileIO;

namespace Avalonia.X11.NativeDialogs;

internal class GtkSystemDialog : BclStorageProvider
{
	private static Task<bool>? _initialized;

	private readonly X11Window _window;

	public override bool CanOpen => true;

	public override bool CanSave => true;

	public override bool CanPickFolder => true;

	private GtkSystemDialog(X11Window window)
	{
		_window = window;
	}

	internal static async Task<IStorageProvider?> TryCreate(X11Window window)
	{
		if (_initialized == null)
		{
			_initialized = Gtk.StartGtk();
		}
		return (await _initialized) ? new GtkSystemDialog(window) : null;
	}

	public override async Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
	{
		return await (await Glib.RunOnGlibThread(async delegate
		{
			IStorageFile[] array = (await ShowDialog(options.Title, _window, GtkFileChooserAction.Open, options.AllowMultiple, options.SuggestedStartLocation, null, options.FileTypeFilter, null, overwritePrompt: false).ConfigureAwait(continueOnCapturedContext: false))?.Select((string f) => new BclStorageFile(new FileInfo(f))).ToArray();
			return array ?? Array.Empty<IStorageFile>();
		}));
	}

	public override async Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options)
	{
		return await (await Glib.RunOnGlibThread(async delegate
		{
			IStorageFolder[] array = (await ShowDialog(options.Title, _window, GtkFileChooserAction.SelectFolder, options.AllowMultiple, options.SuggestedStartLocation, null, null, null, overwritePrompt: false).ConfigureAwait(continueOnCapturedContext: false))?.Select((string f) => new BclStorageFolder(new DirectoryInfo(f))).ToArray();
			return array ?? Array.Empty<IStorageFolder>();
		}));
	}

	public override async Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options)
	{
		return await (await Glib.RunOnGlibThread(async delegate
		{
			string text = (await ShowDialog(options.Title, _window, GtkFileChooserAction.Save, multiSelect: false, options.SuggestedStartLocation, options.SuggestedFileName, options.FileTypeChoices, options.DefaultExtension, options.ShowOverwritePrompt == true).ConfigureAwait(continueOnCapturedContext: false))?.FirstOrDefault();
			return (text != null) ? new BclStorageFile(new FileInfo(text)) : null;
		}));
	}

	private unsafe Task<string[]?> ShowDialog(string? title, IWindowImpl parent, GtkFileChooserAction action, bool multiSelect, IStorageFolder? initialFolder, string? initialFileName, IEnumerable<FilePickerFileType>? filters, string? defaultExtension, bool overwritePrompt)
	{
		IntPtr dlg;
		using (Utf8Buffer title2 = new Utf8Buffer(title))
		{
			dlg = Gtk.gtk_file_chooser_dialog_new(title2, IntPtr.Zero, action, IntPtr.Zero);
		}
		UpdateParent(dlg, parent);
		if (multiSelect)
		{
			Gtk.gtk_file_chooser_set_select_multiple(dlg, allow: true);
		}
		Gtk.gtk_window_set_modal(dlg, modal: true);
		TaskCompletionSource<string[]?> tcs = new TaskCompletionSource<string[]>();
		List<IDisposable> disposables = null;
		Dictionary<IntPtr, FilePickerFileType> filtersDic = new Dictionary<IntPtr, FilePickerFileType>();
		if (filters != null)
		{
			foreach (FilePickerFileType filter in filters)
			{
				IReadOnlyList<string>? patterns = filter.Patterns;
				if (patterns == null || !patterns.Any())
				{
					IReadOnlyList<string>? mimeTypes = filter.MimeTypes;
					if (mimeTypes == null || !mimeTypes.Any())
					{
						continue;
					}
				}
				IntPtr intPtr = Gtk.gtk_file_filter_new();
				filtersDic[intPtr] = filter;
				using (Utf8Buffer name = new Utf8Buffer(filter.Name))
				{
					Gtk.gtk_file_filter_set_name(intPtr, name);
				}
				if (filter.Patterns != null)
				{
					foreach (string pattern2 in filter.Patterns)
					{
						using Utf8Buffer pattern = new Utf8Buffer(pattern2);
						Gtk.gtk_file_filter_add_pattern(intPtr, pattern);
					}
				}
				if (filter.MimeTypes != null)
				{
					foreach (string mimeType2 in filter.MimeTypes)
					{
						using Utf8Buffer mimeType = new Utf8Buffer(mimeType2);
						Gtk.gtk_file_filter_add_mime_type(intPtr, mimeType);
					}
				}
				Gtk.gtk_file_chooser_add_filter(dlg, intPtr);
			}
		}
		disposables = new List<IDisposable>
		{
			Glib.ConnectSignal<Gtk.signal_generic>(dlg, "close", delegate
			{
				tcs.TrySetResult(null);
				Dispose();
				return false;
			}),
			Glib.ConnectSignal<Gtk.signal_dialog_response>(dlg, "response", delegate(IntPtr _, GtkResponseType resp, IntPtr __)
			{
				string[] array = null;
				if (resp == GtkResponseType.Accept)
				{
					List<string> list = new List<string>();
					GSList* ptr = Gtk.gtk_file_chooser_get_filenames(dlg);
					for (GSList* ptr2 = ptr; ptr2 != null; ptr2 = ptr2->Next)
					{
						if (ptr2->Data != IntPtr.Zero)
						{
							string text = Utf8Buffer.StringFromPtr(ptr2->Data);
							if (text != null)
							{
								list.Add(text);
							}
						}
					}
					Glib.g_slist_free(ptr);
					array = list.ToArray();
					if (action == GtkFileChooserAction.Save)
					{
						IntPtr key = Gtk.gtk_file_chooser_get_filter(dlg);
						filtersDic.TryGetValue(key, out FilePickerFileType value);
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = StorageProviderHelpers.NameWithExtension(array[i], defaultExtension, value);
						}
					}
				}
				Gtk.gtk_widget_hide(dlg);
				Dispose();
				tcs.TrySetResult(array);
				return false;
			})
		};
		using (Utf8Buffer button_text = new Utf8Buffer((action == GtkFileChooserAction.Save) ? "Save" : ((action == GtkFileChooserAction.SelectFolder) ? "Select" : "Open")))
		{
			Gtk.gtk_dialog_add_button(dlg, button_text, GtkResponseType.Accept);
		}
		using (Utf8Buffer button_text2 = new Utf8Buffer("Cancel"))
		{
			Gtk.gtk_dialog_add_button(dlg, button_text2, GtkResponseType.Cancel);
		}
		string text2 = initialFolder?.TryGetLocalPath();
		if (text2 != null)
		{
			using Utf8Buffer file = new Utf8Buffer(text2);
			Gtk.gtk_file_chooser_set_current_folder(dlg, file);
		}
		if (initialFileName != null)
		{
			using Utf8Buffer file2 = ((action == GtkFileChooserAction.Open) ? new Utf8Buffer(Path.Combine(text2 ?? "", initialFileName)) : new Utf8Buffer(initialFileName));
			if (action == GtkFileChooserAction.Save)
			{
				Gtk.gtk_file_chooser_set_current_name(dlg, file2);
			}
			else
			{
				Gtk.gtk_file_chooser_set_filename(dlg, file2);
			}
		}
		Gtk.gtk_file_chooser_set_do_overwrite_confirmation(dlg, overwritePrompt);
		Gtk.gtk_window_present(dlg);
		return tcs.Task;
		void Dispose()
		{
			foreach (IDisposable item in disposables)
			{
				item.Dispose();
			}
			disposables.Clear();
		}
	}

	private static void UpdateParent(IntPtr chooser, IWindowImpl parentWindow)
	{
		IntPtr handle = parentWindow.Handle.Handle;
		Gtk.gtk_widget_realize(chooser);
		IntPtr intPtr = Gtk.gtk_widget_get_window(chooser);
		IntPtr foreignWindow = Gtk.GetForeignWindow(handle);
		if (intPtr != IntPtr.Zero && foreignWindow != IntPtr.Zero)
		{
			Gtk.gdk_window_set_transient_for(intPtr, foreignWindow);
		}
	}
}
