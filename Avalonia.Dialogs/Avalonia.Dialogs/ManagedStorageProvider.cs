using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Dialogs.Internal;
using Avalonia.Layout;
using Avalonia.Platform.Storage;
using Avalonia.Platform.Storage.FileIO;

namespace Avalonia.Dialogs;

internal class ManagedStorageProvider<T> : BclStorageProvider where T : Window, new()
{
	private readonly Window _parent;

	private readonly ManagedFileDialogOptions _managedOptions;

	public override bool CanSave => true;

	public override bool CanOpen => true;

	public override bool CanPickFolder => true;

	public ManagedStorageProvider(Window parent, ManagedFileDialogOptions? managedOptions)
	{
		_parent = parent;
		_managedOptions = managedOptions ?? new ManagedFileDialogOptions();
	}

	public override async Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
	{
		return (await Show(new ManagedFileChooserViewModel(options, _managedOptions), _parent)).Select((string f) => new BclStorageFile(new FileInfo(f))).ToArray();
	}

	public override async Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options)
	{
		string text = (await Show(new ManagedFileChooserViewModel(options, _managedOptions), _parent)).FirstOrDefault();
		return (text != null) ? new BclStorageFile(new FileInfo(text)) : null;
	}

	public override async Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options)
	{
		return (await Show(new ManagedFileChooserViewModel(options, _managedOptions), _parent)).Select((string f) => new BclStorageFolder(new DirectoryInfo(f))).ToArray();
	}

	private static async Task<string[]> Show(ManagedFileChooserViewModel model, Window parent)
	{
		T dialog = new T
		{
			Content = new ManagedFileChooser(),
			Title = model.Title,
			DataContext = model
		};
		dialog.Closed += delegate
		{
			model.Cancel();
		};
		string[] result = null;
		model.CompleteRequested += delegate(string[] items)
		{
			result = items;
			dialog.Close();
		};
		model.OverwritePrompt += async delegate(string filename)
		{
			Window overwritePromptDialog = new Window
			{
				Title = "Confirm Save As",
				SizeToContent = SizeToContent.WidthAndHeight,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				Padding = new Thickness(10.0),
				MinWidth = 270.0
			};
			string fileName = Path.GetFileName(filename);
			DockPanel dockPanel = new DockPanel
			{
				HorizontalAlignment = HorizontalAlignment.Stretch
			};
			Label label = new Label
			{
				Content = fileName + " already exists.\nDo you want to replace it?"
			};
			dockPanel.Children.Add(label);
			DockPanel.SetDock(label, Dock.Top);
			StackPanel stackPanel = new StackPanel
			{
				HorizontalAlignment = HorizontalAlignment.Right,
				Orientation = Orientation.Horizontal,
				Spacing = 10.0
			};
			Button button = new Button
			{
				Content = "Yes",
				HorizontalAlignment = HorizontalAlignment.Right
			};
			button.Click += delegate
			{
				result = new string[1] { filename };
				overwritePromptDialog.Close();
				dialog.Close();
			};
			stackPanel.Children.Add(button);
			button = new Button
			{
				Content = "No",
				HorizontalAlignment = HorizontalAlignment.Right
			};
			button.Click += delegate
			{
				overwritePromptDialog.Close();
			};
			stackPanel.Children.Add(button);
			dockPanel.Children.Add(stackPanel);
			DockPanel.SetDock(stackPanel, Dock.Bottom);
			overwritePromptDialog.Content = dockPanel;
			await overwritePromptDialog.ShowDialog(dialog);
		};
		model.CancelRequested += dialog.Close;
		await dialog.ShowDialog<object>(parent);
		return result ?? Array.Empty<string>();
	}
}
