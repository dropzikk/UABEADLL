using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Diagnostics.Views;
using Avalonia.Platform.Storage;

namespace Avalonia.Diagnostics.Screenshots;

public sealed class FilePickerHandler : BaseRenderToStreamHandler
{
	private readonly string _title;

	private readonly string _screenshotRoot;

	public FilePickerHandler()
		: this(null)
	{
	}

	public FilePickerHandler(string? title, string? screenshotRoot = null)
	{
		_title = title ?? "Save Screenshot to ...";
		_screenshotRoot = screenshotRoot ?? Conventions.DefaultScreenshotsRoot;
	}

	private static TopLevel GetTopLevel(Control control)
	{
		TopLevel topLevel = null;
		if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime classicDesktopStyleApplicationLifetime)
		{
			topLevel = classicDesktopStyleApplicationLifetime.Windows.FirstOrDefault((Window w) => w is MainWindow);
		}
		return topLevel ?? TopLevel.GetTopLevel(control) ?? throw new InvalidOperationException("No TopLevel is available.");
	}

	protected override async Task<Stream?> GetStream(Control control)
	{
		IStorageProvider storageProvider = GetTopLevel(control).StorageProvider;
		IStorageFolder storageFolder = await storageProvider.TryGetFolderFromPathAsync(_screenshotRoot);
		if (storageFolder == null)
		{
			storageFolder = await storageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Pictures);
		}
		IStorageFolder suggestedStartLocation = storageFolder;
		IStorageFile storageFile = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
		{
			SuggestedStartLocation = suggestedStartLocation,
			Title = _title,
			FileTypeChoices = new FilePickerFileType[1] { FilePickerFileTypes.ImagePng },
			DefaultExtension = ".png"
		});
		if (storageFile == null)
		{
			return null;
		}
		return await storageFile.OpenWriteAsync();
	}
}
