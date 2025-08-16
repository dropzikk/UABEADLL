using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnAutomationNode : IUnknown, IDisposable
{
	new void Dispose();

	void ChildrenChanged();

	void PropertyChanged(AvnAutomationProperty property);

	void FocusChanged();
}
