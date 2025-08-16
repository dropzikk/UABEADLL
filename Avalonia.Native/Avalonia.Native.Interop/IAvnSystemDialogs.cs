using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnSystemDialogs : IUnknown, IDisposable
{
	void SelectFolderDialog(IAvnWindow parentWindowHandle, IAvnSystemDialogEvents events, int allowMultiple, string title, string initialPath);

	void OpenFileDialog(IAvnWindow parentWindowHandle, IAvnSystemDialogEvents events, int allowMultiple, string title, string initialDirectory, string initialFile, string filters);

	void SaveFileDialog(IAvnWindow parentWindowHandle, IAvnSystemDialogEvents events, string title, string initialDirectory, string initialFile, string filters);
}
