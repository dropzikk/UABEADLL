using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com;

internal interface IFileDialog : IModalWindow, IUnknown, IDisposable
{
	ushort FileTypeIndex { get; }

	FILEOPENDIALOGOPTIONS Options { get; }

	IShellItem Folder { get; }

	IShellItem CurrentSelection { get; }

	unsafe char* FileName { get; }

	IShellItem Result { get; }

	unsafe void SetFileTypes(ushort cFileTypes, void* rgFilterSpec);

	void SetFileTypeIndex(ushort iFileType);

	unsafe int Advise(void* pfde);

	void Unadvise(int dwCookie);

	void SetOptions(FILEOPENDIALOGOPTIONS fos);

	void SetDefaultFolder(IShellItem psi);

	void SetFolder(IShellItem psi);

	unsafe void SetFileName(char* pszName);

	unsafe void SetTitle(char* pszTitle);

	unsafe void SetOkButtonLabel(char* pszText);

	unsafe void SetFileNameLabel(char* pszLabel);

	void AddPlace(IShellItem psi, int fdap);

	unsafe void SetDefaultExtension(char* pszDefaultExtension);

	void Close(int hr);

	unsafe void SetClientGuid(Guid* guid);

	void ClearClientData();

	unsafe void SetFilter(void* pFilter);
}
