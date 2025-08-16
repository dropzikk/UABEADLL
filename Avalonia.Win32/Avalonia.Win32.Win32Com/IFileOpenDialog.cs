using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com;

internal interface IFileOpenDialog : IFileDialog, IModalWindow, IUnknown, IDisposable
{
	IShellItemArray Results { get; }

	IShellItemArray SelectedItems { get; }
}
