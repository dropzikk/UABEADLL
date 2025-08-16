using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IVectorOfCompositionShape : IInspectable, IUnknown, IDisposable
{
	void GetAt();

	void GetSize();

	void GetView();

	void IndexOf();

	void SetAt();

	void InsertAt();

	void RemoveAt();

	void Append(ICompositionShape value);

	void RemoveAtEnd();

	void Clear();
}
