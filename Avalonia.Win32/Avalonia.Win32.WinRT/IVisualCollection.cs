using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IVisualCollection : IInspectable, IUnknown, IDisposable
{
	int Count { get; }

	void InsertAbove(IVisual newChild, IVisual sibling);

	void InsertAtBottom(IVisual newChild);

	void InsertAtTop(IVisual newChild);

	void InsertBelow(IVisual newChild, IVisual sibling);

	void Remove(IVisual child);

	void RemoveAll();
}
