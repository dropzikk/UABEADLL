using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avalonia.Platform.Storage.FileIO;
using Avalonia.Win32.Interop;
using Avalonia.Win32.Win32Com;
using MicroCom.Runtime;

namespace Avalonia.Win32;

internal class Win32StorageProvider : BclStorageProvider
{
	private const uint SIGDN_FILESYSPATH = 2147844096u;

	private const FILEOPENDIALOGOPTIONS DefaultDialogOptions = FILEOPENDIALOGOPTIONS.FOS_FORCEFILESYSTEM | FILEOPENDIALOGOPTIONS.FOS_NOVALIDATE | FILEOPENDIALOGOPTIONS.FOS_NOTESTFILECREATE | FILEOPENDIALOGOPTIONS.FOS_DONTADDTORECENT;

	private readonly WindowImpl _windowImpl;

	public override bool CanOpen => true;

	public override bool CanSave => true;

	public override bool CanPickFolder => true;

	public Win32StorageProvider(WindowImpl windowImpl)
	{
		_windowImpl = windowImpl;
	}

	public override async Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options)
	{
		return (await ShowFilePicker(isOpenFile: true, openFolder: true, options.AllowMultiple, false, options.Title, null, options.SuggestedStartLocation, null, null)).Select((string f) => new BclStorageFolder(new DirectoryInfo(f))).ToArray();
	}

	public override async Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
	{
		return (await ShowFilePicker(isOpenFile: true, openFolder: false, options.AllowMultiple, false, options.Title, null, options.SuggestedStartLocation, null, options.FileTypeFilter)).Select((string f) => new BclStorageFile(new FileInfo(f))).ToArray();
	}

	public override async Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options)
	{
		return (await ShowFilePicker(isOpenFile: false, openFolder: false, allowMultiple: false, options.ShowOverwritePrompt, options.Title, options.SuggestedFileName, options.SuggestedStartLocation, options.DefaultExtension, options.FileTypeChoices)).Select((string f) => new BclStorageFile(new FileInfo(f))).FirstOrDefault();
	}

	private unsafe Task<IEnumerable<string>> ShowFilePicker(bool isOpenFile, bool openFolder, bool allowMultiple, bool? showOverwritePrompt, string? title, string? suggestedFileName, IStorageFolder? folder, string? defaultExtension, IReadOnlyList<FilePickerFileType>? filters)
	{
		return Task.Run(delegate
		{
			IEnumerable<string> result = Array.Empty<string>();
			try
			{
				Guid clsid = (isOpenFile ? UnmanagedMethods.ShellIds.OpenFileDialog : UnmanagedMethods.ShellIds.SaveFileDialog);
				Guid iid = UnmanagedMethods.ShellIds.IFileDialog;
				IFileDialog fileDialog = UnmanagedMethods.CreateInstance<IFileDialog>(ref clsid, ref iid);
				FILEOPENDIALOGOPTIONS options = fileDialog.Options;
				options |= FILEOPENDIALOGOPTIONS.FOS_FORCEFILESYSTEM | FILEOPENDIALOGOPTIONS.FOS_NOVALIDATE | FILEOPENDIALOGOPTIONS.FOS_NOTESTFILECREATE | FILEOPENDIALOGOPTIONS.FOS_DONTADDTORECENT;
				if (openFolder)
				{
					options |= FILEOPENDIALOGOPTIONS.FOS_PICKFOLDERS;
				}
				if (allowMultiple)
				{
					options |= FILEOPENDIALOGOPTIONS.FOS_ALLOWMULTISELECT;
				}
				if (showOverwritePrompt == false)
				{
					options &= ~FILEOPENDIALOGOPTIONS.FOS_OVERWRITEPROMPT;
				}
				fileDialog.SetOptions(options);
				if (defaultExtension == null)
				{
					defaultExtension = string.Empty;
				}
				fixed (char* defaultExtension2 = defaultExtension)
				{
					fileDialog.SetDefaultExtension(defaultExtension2);
				}
				if (suggestedFileName == null)
				{
					suggestedFileName = "";
				}
				fixed (char* fileName = suggestedFileName)
				{
					fileDialog.SetFileName(fileName);
				}
				if (title == null)
				{
					title = "";
				}
				fixed (char* title2 = title)
				{
					fileDialog.SetTitle(title2);
				}
				if (!openFolder)
				{
					int length;
					fixed (byte* ptr = FiltersToPointer(filters, out length))
					{
						void* rgFilterSpec = ptr;
						fileDialog.SetFileTypes((ushort)length, rgFilterSpec);
						if (length > 0)
						{
							fileDialog.SetFileTypeIndex(0);
						}
					}
				}
				string text = folder?.TryGetLocalPath();
				if (text != null)
				{
					Guid riid = UnmanagedMethods.ShellIds.IShellItem;
					if (UnmanagedMethods.SHCreateItemFromParsingName(text, IntPtr.Zero, ref riid, out var ppv) == 0L)
					{
						IShellItem shellItem = MicroComRuntime.CreateProxyFor<IShellItem>(ppv, ownsHandle: true);
						fileDialog.SetFolder(shellItem);
						fileDialog.SetDefaultFolder(shellItem);
					}
				}
				int num = fileDialog.Show(_windowImpl.Handle.Handle);
				switch (num)
				{
				case -2147023673:
					return result;
				default:
					throw new Win32Exception(num);
				case 0:
					if (allowMultiple)
					{
						using IFileOpenDialog fileOpenDialog = fileDialog.QueryInterface<IFileOpenDialog>();
						IShellItemArray results = fileOpenDialog.Results;
						int count = results.Count;
						List<string> list = new List<string>();
						for (int i = 0; i < count; i++)
						{
							string absoluteFilePath = GetAbsoluteFilePath(results.GetItemAt(i));
							if (absoluteFilePath != null)
							{
								list.Add(absoluteFilePath);
							}
						}
						result = list;
					}
					else
					{
						IShellItem result2 = fileDialog.Result;
						if (result2 != null)
						{
							string absoluteFilePath2 = GetAbsoluteFilePath(result2);
							if (absoluteFilePath2 != null)
							{
								result = new string[1] { absoluteFilePath2 };
							}
						}
					}
					return result;
				}
			}
			catch (COMException ex)
			{
				throw new COMException(new Win32Exception(ex.HResult).Message, ex);
			}
		});
	}

	private unsafe static string? GetAbsoluteFilePath(IShellItem shellItem)
	{
		IntPtr intPtr = new IntPtr(shellItem.GetDisplayName(2147844096u));
		if (intPtr != IntPtr.Zero)
		{
			try
			{
				return Marshal.PtrToStringUni(intPtr);
			}
			finally
			{
				Marshal.FreeCoTaskMem(intPtr);
			}
		}
		return null;
	}

	private static byte[] FiltersToPointer(IReadOnlyList<FilePickerFileType>? filters, out int length)
	{
		if (filters == null || filters.Count == 0)
		{
			filters = new List<FilePickerFileType> { FilePickerFileTypes.All };
		}
		int num = Marshal.SizeOf<UnmanagedMethods.COMDLG_FILTERSPEC>();
		byte[] array = new byte[num * filters.Count];
		for (int i = 0; i < filters.Count; i++)
		{
			FilePickerFileType filePickerFileType = filters[i];
			if (filePickerFileType.Patterns != null && filePickerFileType.Patterns.Any())
			{
				IntPtr intPtr = Marshal.AllocHGlobal(num);
				try
				{
					UnmanagedMethods.COMDLG_FILTERSPEC structure = default(UnmanagedMethods.COMDLG_FILTERSPEC);
					structure.pszName = filePickerFileType.Name;
					structure.pszSpec = string.Join(";", filePickerFileType.Patterns);
					Marshal.StructureToPtr(structure, intPtr, fDeleteOld: false);
					Marshal.Copy(intPtr, array, i * num, num);
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
		}
		length = filters.Count;
		return array;
	}
}
