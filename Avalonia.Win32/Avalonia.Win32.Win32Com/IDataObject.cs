using System;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com;

internal interface IDataObject : IUnknown, IDisposable
{
	unsafe uint GetData(FORMATETC* pformatetcIn, STGMEDIUM* pmedium);

	unsafe uint GetDataHere(FORMATETC* pformatetc, STGMEDIUM* pmedium);

	unsafe uint QueryGetData(FORMATETC* pformatetc);

	unsafe FORMATETC GetCanonicalFormatEtc(FORMATETC* pformatectIn);

	unsafe uint SetData(FORMATETC* pformatetc, STGMEDIUM* pmedium, int fRelease);

	IEnumFORMATETC EnumFormatEtc(int dwDirection);

	unsafe int DAdvise(FORMATETC* pformatetc, int advf, void* pAdvSink);

	void DUnadvise(int dwConnection);

	unsafe void* EnumDAdvise();
}
