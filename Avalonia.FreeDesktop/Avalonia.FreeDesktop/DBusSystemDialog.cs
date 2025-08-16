using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Platform.Storage.FileIO;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace Avalonia.FreeDesktop;

internal class DBusSystemDialog : BclStorageProvider
{
	private readonly Connection _connection;

	private readonly OrgFreedesktopPortalFileChooser _fileChooser;

	private readonly IPlatformHandle _handle;

	public override bool CanOpen => true;

	public override bool CanSave => true;

	public override bool CanPickFolder => true;

	internal static async Task<IStorageProvider?> TryCreateAsync(IPlatformHandle handle)
	{
		if (DBusHelper.Connection == null)
		{
			return null;
		}
		OrgFreedesktopPortalFileChooser dbusFileChooser = new OrgFreedesktopPortalFileChooser(DBusHelper.Connection, "org.freedesktop.portal.Desktop", "/org/freedesktop/portal/desktop");
		try
		{
			await dbusFileChooser.GetVersionPropertyAsync();
		}
		catch
		{
			return null;
		}
		return new DBusSystemDialog(DBusHelper.Connection, handle, dbusFileChooser);
	}

	private DBusSystemDialog(Connection connection, IPlatformHandle handle, OrgFreedesktopPortalFileChooser fileChooser)
	{
		_connection = connection;
		_fileChooser = fileChooser;
		_handle = handle;
	}

	public override async Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
	{
		string parent_window = $"x11:{_handle.Handle:X}";
		Dictionary<string, DBusVariantItem> dictionary = new Dictionary<string, DBusVariantItem>();
		DBusVariantItem dBusVariantItem = ParseFilters(options.FileTypeFilter);
		if (dBusVariantItem != null)
		{
			dictionary.Add("filters", dBusVariantItem);
		}
		dictionary.Add("multiple", new DBusVariantItem("b", new DBusBoolItem(options.AllowMultiple)));
		ObjectPath objectPath = await _fileChooser.OpenFileAsync(parent_window, options.Title ?? string.Empty, dictionary);
		OrgFreedesktopPortalRequest orgFreedesktopPortalRequest = new OrgFreedesktopPortalRequest(_connection, "org.freedesktop.portal.Desktop", objectPath);
		TaskCompletionSource<string[]?> tsc = new TaskCompletionSource<string[]>();
		using (await orgFreedesktopPortalRequest.WatchResponseAsync(delegate(Exception? e, (uint response, Dictionary<string, DBusVariantItem> results) x)
		{
			if (e != null)
			{
				tsc.TrySetException(e);
			}
			else
			{
				tsc.TrySetResult((x.results["uris"].Value as DBusArrayItem)?.Select((DBusItem y) => (y as DBusStringItem).Value).ToArray());
			}
		}))
		{
			return ((await tsc.Task) ?? Array.Empty<string>()).Select((string path) => new BclStorageFile(new FileInfo(new Uri(path).LocalPath))).ToList();
		}
	}

	public override async Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options)
	{
		string parent_window = $"x11:{_handle.Handle:X}";
		Dictionary<string, DBusVariantItem> dictionary = new Dictionary<string, DBusVariantItem>();
		DBusVariantItem dBusVariantItem = ParseFilters(options.FileTypeChoices);
		if (dBusVariantItem != null)
		{
			dictionary.Add("filters", dBusVariantItem);
		}
		string suggestedFileName = options.SuggestedFileName;
		if (suggestedFileName != null)
		{
			dictionary.Add("current_name", new DBusVariantItem("s", new DBusStringItem(suggestedFileName)));
		}
		string text = options.SuggestedStartLocation?.TryGetLocalPath();
		if (text != null)
		{
			dictionary.Add("current_folder", new DBusVariantItem("ay", new DBusArrayItem(DBusType.Byte, from x in Encoding.UTF8.GetBytes(text)
				select new DBusByteItem(x))));
		}
		ObjectPath objectPath = await _fileChooser.SaveFileAsync(parent_window, options.Title ?? string.Empty, dictionary);
		OrgFreedesktopPortalRequest orgFreedesktopPortalRequest = new OrgFreedesktopPortalRequest(_connection, "org.freedesktop.portal.Desktop", objectPath);
		TaskCompletionSource<string[]?> tsc = new TaskCompletionSource<string[]>();
		using (await orgFreedesktopPortalRequest.WatchResponseAsync(delegate(Exception? e, (uint response, Dictionary<string, DBusVariantItem> results) x)
		{
			if (e != null)
			{
				tsc.TrySetException(e);
			}
			else
			{
				tsc.TrySetResult((x.results["uris"].Value as DBusArrayItem)?.Select((DBusItem y) => (y as DBusStringItem).Value).ToArray());
			}
		}))
		{
			string text2 = (await tsc.Task)?.FirstOrDefault();
			string text3 = ((text2 != null) ? new Uri(text2).LocalPath : null);
			if (text3 == null)
			{
				return null;
			}
			text3 = StorageProviderHelpers.NameWithExtension(text3, options.DefaultExtension, null);
			return new BclStorageFile(new FileInfo(text3));
		}
	}

	public override async Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options)
	{
		string parent_window = $"x11:{_handle.Handle:X}";
		Dictionary<string, DBusVariantItem> options2 = new Dictionary<string, DBusVariantItem>
		{
			{
				"directory",
				new DBusVariantItem("b", new DBusBoolItem(value: true))
			},
			{
				"multiple",
				new DBusVariantItem("b", new DBusBoolItem(options.AllowMultiple))
			}
		};
		ObjectPath objectPath = await _fileChooser.OpenFileAsync(parent_window, options.Title ?? string.Empty, options2);
		OrgFreedesktopPortalRequest orgFreedesktopPortalRequest = new OrgFreedesktopPortalRequest(_connection, "org.freedesktop.portal.Desktop", objectPath);
		TaskCompletionSource<string[]?> tsc = new TaskCompletionSource<string[]>();
		using (await orgFreedesktopPortalRequest.WatchResponseAsync(delegate(Exception? e, (uint response, Dictionary<string, DBusVariantItem> results) x)
		{
			if (e != null)
			{
				tsc.TrySetException(e);
			}
			else
			{
				tsc.TrySetResult((x.results["uris"].Value as DBusArrayItem)?.Select((DBusItem y) => (y as DBusStringItem).Value).ToArray());
			}
		}))
		{
			return (from path in ((await tsc.Task) ?? Array.Empty<string>()).Select((string path) => new Uri(path).LocalPath).Where(Directory.Exists)
				select new BclStorageFolder(new DirectoryInfo(path))).ToList();
		}
	}

	private static DBusVariantItem? ParseFilters(IReadOnlyList<FilePickerFileType>? fileTypes)
	{
		if (fileTypes == null)
		{
			return null;
		}
		DBusArrayItem dBusArrayItem = new DBusArrayItem(DBusType.Struct, new List<DBusItem>());
		foreach (FilePickerFileType fileType in fileTypes)
		{
			List<DBusItem> list = new List<DBusItem>();
			IReadOnlyList<string>? patterns = fileType.Patterns;
			if (patterns != null && patterns.Count > 0)
			{
				list.AddRange(fileType.Patterns.Select((string pattern) => new DBusStructItem(new DBusItem[2]
				{
					new DBusUInt32Item(0u),
					new DBusStringItem(pattern)
				})));
			}
			else
			{
				IReadOnlyList<string>? mimeTypes = fileType.MimeTypes;
				if (mimeTypes == null || mimeTypes.Count <= 0)
				{
					continue;
				}
				list.AddRange(fileType.MimeTypes.Select((string mimeType) => new DBusStructItem(new DBusItem[2]
				{
					new DBusUInt32Item(1u),
					new DBusStringItem(mimeType)
				})));
			}
			dBusArrayItem.Add(new DBusStructItem(new DBusItem[2]
			{
				new DBusStringItem(fileType.Name),
				new DBusArrayItem(DBusType.Struct, list)
			}));
		}
		if (dBusArrayItem.Count <= 0)
		{
			return null;
		}
		return new DBusVariantItem("a(sa(us))", dBusArrayItem);
	}
}
