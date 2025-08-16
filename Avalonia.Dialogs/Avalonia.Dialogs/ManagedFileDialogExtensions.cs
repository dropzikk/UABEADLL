using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Platform.Storage;

namespace Avalonia.Dialogs;

public static class ManagedFileDialogExtensions
{
	internal class ManagedStorageProviderFactory<T> : IStorageProviderFactory where T : Window, new()
	{
		public IStorageProvider CreateProvider(TopLevel topLevel)
		{
			if (topLevel is Window parent)
			{
				ManagedFileDialogOptions service = AvaloniaLocator.Current.GetService<ManagedFileDialogOptions>();
				return new ManagedStorageProvider<T>(parent, service);
			}
			throw new InvalidOperationException("Current platform doesn't support managed picker dialogs");
		}
	}

	public static AppBuilder UseManagedSystemDialogs(this AppBuilder builder)
	{
		builder.AfterSetup(delegate
		{
			AvaloniaLocator.CurrentMutable.Bind<IStorageProviderFactory>().ToSingleton<ManagedStorageProviderFactory<Window>>();
		});
		return builder;
	}

	public static AppBuilder UseManagedSystemDialogs<TWindow>(this AppBuilder builder) where TWindow : Window, new()
	{
		builder.AfterSetup(delegate
		{
			AvaloniaLocator.CurrentMutable.Bind<IStorageProviderFactory>().ToSingleton<ManagedStorageProviderFactory<TWindow>>();
		});
		return builder;
	}

	[Obsolete("Use Window.StorageProvider API or TopLevel.StorageProvider API")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static Task<string[]> ShowManagedAsync(this OpenFileDialog dialog, Window parent, ManagedFileDialogOptions? options = null)
	{
		return dialog.ShowManagedAsync<Window>(parent, options);
	}

	[Obsolete("Use Window.StorageProvider API or TopLevel.StorageProvider API")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static async Task<string[]> ShowManagedAsync<TWindow>(this OpenFileDialog dialog, Window parent, ManagedFileDialogOptions? options = null) where TWindow : Window, new()
	{
		return (await new ManagedStorageProvider<TWindow>(parent, options).OpenFilePickerAsync(dialog.ToFilePickerOpenOptions())).Select((IStorageFile file) => file.TryGetLocalPath() ?? file.Name).ToArray();
	}
}
