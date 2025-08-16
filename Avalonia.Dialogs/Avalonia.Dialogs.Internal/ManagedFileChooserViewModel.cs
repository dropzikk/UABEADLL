using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Reactive;
using Avalonia.Threading;

namespace Avalonia.Dialogs.Internal;

public class ManagedFileChooserViewModel : AvaloniaDialogsInternalViewModelBase
{
	private readonly ManagedFileDialogOptions _options;

	private string _location;

	private string _fileName;

	private bool _showHiddenFiles;

	private ManagedFileChooserFilterViewModel _selectedFilter;

	private bool _selectingDirectory;

	private bool _savingFile;

	private bool _scheduledSelectionValidation;

	private bool _alreadyCancelled;

	private string _defaultExtension;

	private bool _overwritePrompt;

	private CompositeDisposable _disposables;

	public AvaloniaList<ManagedFileChooserItemViewModel> QuickLinks { get; } = new AvaloniaList<ManagedFileChooserItemViewModel>();

	public AvaloniaList<ManagedFileChooserItemViewModel> Items { get; } = new AvaloniaList<ManagedFileChooserItemViewModel>();

	public AvaloniaList<ManagedFileChooserFilterViewModel> Filters { get; } = new AvaloniaList<ManagedFileChooserFilterViewModel>();

	public AvaloniaList<ManagedFileChooserItemViewModel> SelectedItems { get; } = new AvaloniaList<ManagedFileChooserItemViewModel>();

	public string Location
	{
		get
		{
			return _location;
		}
		private set
		{
			RaiseAndSetIfChanged(ref _location, value, "Location");
		}
	}

	public string FileName
	{
		get
		{
			return _fileName;
		}
		private set
		{
			RaiseAndSetIfChanged(ref _fileName, value, "FileName");
		}
	}

	public bool SelectingFolder => _selectingDirectory;

	public bool ShowFilters { get; }

	public SelectionMode SelectionMode { get; }

	public string Title { get; }

	public int QuickLinksSelectedIndex
	{
		get
		{
			for (int i = 0; i < QuickLinks.Count; i++)
			{
				if (QuickLinks[i].Path == Location)
				{
					return i;
				}
			}
			return -1;
		}
		set
		{
			RaisePropertyChanged("QuickLinksSelectedIndex");
		}
	}

	public ManagedFileChooserFilterViewModel SelectedFilter
	{
		get
		{
			return _selectedFilter;
		}
		set
		{
			RaiseAndSetIfChanged(ref _selectedFilter, value, "SelectedFilter");
			Refresh();
		}
	}

	public bool ShowHiddenFiles
	{
		get
		{
			return _showHiddenFiles;
		}
		set
		{
			RaiseAndSetIfChanged(ref _showHiddenFiles, value, "ShowHiddenFiles");
			Refresh();
		}
	}

	public event Action CancelRequested;

	public event Action<string[]> CompleteRequested;

	public event Action<string> OverwritePrompt;

	private void RefreshQuickLinks(ManagedFileChooserSources quickSources)
	{
		QuickLinks.Clear();
		QuickLinks.AddRange(from i in quickSources.GetAllItems()
			select new ManagedFileChooserItemViewModel(i));
	}

	public ManagedFileChooserViewModel(ManagedFileDialogOptions options)
	{
		_options = options;
		_disposables = new CompositeDisposable();
		ManagedFileChooserSources quickSources = AvaloniaLocator.Current.GetService<ManagedFileChooserSources>() ?? new ManagedFileChooserSources();
		IDisposable item = AvaloniaLocator.Current.GetRequiredService<IMountedVolumeInfoProvider>().Listen(ManagedFileChooserSources.MountedVolumes);
		IDisposable item2 = ManagedFileChooserSources.MountedVolumes.GetWeakCollectionChangedObservable().Subscribe(delegate
		{
			Dispatcher.UIThread.Post(delegate
			{
				RefreshQuickLinks(quickSources);
			});
		});
		_disposables.Add(item);
		_disposables.Add(item2);
		CompleteRequested += delegate
		{
			_disposables?.Dispose();
		};
		CancelRequested += delegate
		{
			_disposables?.Dispose();
		};
		RefreshQuickLinks(quickSources);
		SelectedItems.CollectionChanged += OnSelectionChangedAsync;
	}

	public ManagedFileChooserViewModel(FilePickerOpenOptions filePickerOpen, ManagedFileDialogOptions options)
		: this(options)
	{
		Title = filePickerOpen.Title ?? "Open file";
		IReadOnlyList<FilePickerFileType>? fileTypeFilter = filePickerOpen.FileTypeFilter;
		if (fileTypeFilter != null && fileTypeFilter.Count > 0)
		{
			Filters.AddRange(filePickerOpen.FileTypeFilter.Select((FilePickerFileType f) => new ManagedFileChooserFilterViewModel(f)));
			_selectedFilter = Filters[0];
			ShowFilters = true;
		}
		if (filePickerOpen.AllowMultiple)
		{
			SelectionMode = SelectionMode.Multiple;
		}
		Navigate(filePickerOpen.SuggestedStartLocation);
	}

	public ManagedFileChooserViewModel(FilePickerSaveOptions filePickerSave, ManagedFileDialogOptions options)
		: this(options)
	{
		Title = filePickerSave.Title ?? "Save file";
		IReadOnlyList<FilePickerFileType>? fileTypeChoices = filePickerSave.FileTypeChoices;
		if (fileTypeChoices != null && fileTypeChoices.Count > 0)
		{
			Filters.AddRange(filePickerSave.FileTypeChoices.Select((FilePickerFileType f) => new ManagedFileChooserFilterViewModel(f)));
			_selectedFilter = Filters[0];
			ShowFilters = true;
		}
		_savingFile = true;
		_defaultExtension = filePickerSave.DefaultExtension;
		_overwritePrompt = filePickerSave.ShowOverwritePrompt ?? true;
		FileName = filePickerSave.SuggestedFileName;
		Navigate(filePickerSave.SuggestedStartLocation, FileName);
	}

	public ManagedFileChooserViewModel(FolderPickerOpenOptions folderPickerOpen, ManagedFileDialogOptions options)
		: this(options)
	{
		Title = folderPickerOpen.Title ?? "Select directory";
		_selectingDirectory = true;
		if (folderPickerOpen.AllowMultiple)
		{
			SelectionMode = SelectionMode.Multiple;
		}
		Navigate(folderPickerOpen.SuggestedStartLocation);
	}

	public void EnterPressed()
	{
		if (Directory.Exists(Location))
		{
			Navigate(Location);
		}
		else if (File.Exists(Location))
		{
			this.CompleteRequested?.Invoke(new string[1] { Location });
		}
	}

	private async void OnSelectionChangedAsync(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (_scheduledSelectionValidation)
		{
			return;
		}
		_scheduledSelectionValidation = true;
		await Dispatcher.UIThread.InvokeAsync(delegate
		{
			try
			{
				if (_selectingDirectory)
				{
					SelectedItems.Clear();
				}
				else
				{
					if (!_options.AllowDirectorySelection)
					{
						foreach (ManagedFileChooserItemViewModel item in SelectedItems.Where((ManagedFileChooserItemViewModel i) => i.ItemType == ManagedFileChooserItemType.Folder).ToList())
						{
							SelectedItems.Remove(item);
						}
					}
					if (!_selectingDirectory)
					{
						ManagedFileChooserItemViewModel managedFileChooserItemViewModel = SelectedItems.FirstOrDefault();
						if (managedFileChooserItemViewModel != null)
						{
							FileName = managedFileChooserItemViewModel.DisplayName;
						}
					}
				}
			}
			finally
			{
				_scheduledSelectionValidation = false;
			}
		});
	}

	private void NavigateRoot(string initialSelectionName)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			Navigate(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)), initialSelectionName);
		}
		else
		{
			Navigate("/", initialSelectionName);
		}
	}

	public void Refresh()
	{
		Navigate(Location);
	}

	public void Navigate(IStorageFolder path, string initialSelectionName = null)
	{
		string path2 = path?.TryGetLocalPath() ?? Directory.GetCurrentDirectory();
		Navigate(path2, initialSelectionName);
	}

	public void Navigate(string path, string initialSelectionName = null)
	{
		if (!Directory.Exists(path))
		{
			NavigateRoot(initialSelectionName);
			return;
		}
		Location = path;
		Items.Clear();
		SelectedItems.Clear();
		try
		{
			IEnumerable<FileSystemInfo> source = new DirectoryInfo(path).EnumerateFileSystemInfos();
			if (!ShowHiddenFiles)
			{
				source = ((!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) ? source.Where((FileSystemInfo i) => !i.Name.StartsWith(".")) : source.Where((FileSystemInfo i) => (i.Attributes & (FileAttributes.Hidden | FileAttributes.System)) == 0));
			}
			if (SelectedFilter != null)
			{
				source = source.Where((FileSystemInfo i) => i is DirectoryInfo || SelectedFilter.Match(i.Name));
			}
			Items.AddRange((from x in source
				where (!_selectingDirectory || x is DirectoryInfo) ? true : false
				where x.Exists
				select x into info
				select new ManagedFileChooserItemViewModel
				{
					DisplayName = info.Name,
					Path = info.FullName,
					Type = ((info is FileInfo) ? info.Extension : "File Folder"),
					ItemType = ((!(info is FileInfo)) ? ManagedFileChooserItemType.Folder : ManagedFileChooserItemType.File),
					Size = ((info is FileInfo fileInfo) ? fileInfo.Length : 0),
					Modified = info.LastWriteTime
				} into x
				orderby x.ItemType == ManagedFileChooserItemType.Folder descending
				select x).ThenBy<ManagedFileChooserItemViewModel, string>((ManagedFileChooserItemViewModel x) => x.DisplayName, StringComparer.InvariantCultureIgnoreCase));
			if (initialSelectionName != null)
			{
				ManagedFileChooserItemViewModel managedFileChooserItemViewModel = Items.FirstOrDefault((ManagedFileChooserItemViewModel i) => i.ItemType == ManagedFileChooserItemType.File && i.DisplayName == initialSelectionName);
				if (managedFileChooserItemViewModel != null)
				{
					SelectedItems.Add(managedFileChooserItemViewModel);
				}
			}
			RaisePropertyChanged("QuickLinksSelectedIndex");
		}
		catch (UnauthorizedAccessException)
		{
		}
	}

	public void GoUp()
	{
		string directoryName = Path.GetDirectoryName(Location);
		if (!string.IsNullOrWhiteSpace(directoryName))
		{
			Navigate(directoryName);
		}
	}

	public void Cancel()
	{
		if (!_alreadyCancelled)
		{
			_alreadyCancelled = true;
			this.CancelRequested?.Invoke();
		}
	}

	public void Ok()
	{
		if (_selectingDirectory)
		{
			this.CompleteRequested?.Invoke(new string[1] { Location });
		}
		else if (_savingFile)
		{
			if (!string.IsNullOrWhiteSpace(FileName))
			{
				if (!Path.HasExtension(FileName) && !string.IsNullOrWhiteSpace(_defaultExtension))
				{
					FileName = Path.ChangeExtension(FileName, _defaultExtension);
				}
				string text = Path.Combine(Location, FileName);
				if (_overwritePrompt && File.Exists(text))
				{
					this.OverwritePrompt?.Invoke(text);
					return;
				}
				this.CompleteRequested?.Invoke(new string[1] { text });
			}
		}
		else
		{
			this.CompleteRequested?.Invoke(SelectedItems.Select((ManagedFileChooserItemViewModel i) => i.Path).ToArray());
		}
	}

	public void SelectSingleFile(ManagedFileChooserItemViewModel item)
	{
		this.CompleteRequested?.Invoke(new string[1] { item.Path });
	}
}
