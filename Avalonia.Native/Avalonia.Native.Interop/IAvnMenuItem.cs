using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnMenuItem : IUnknown, IDisposable
{
	void SetSubMenu(IAvnMenu menu);

	void SetTitle(string utf8String);

	void SetGesture(AvnKey key, AvnInputModifiers modifiers);

	void SetAction(IAvnPredicateCallback predicate, IAvnActionCallback callback);

	void SetIsChecked(int isChecked);

	void SetToggleType(AvnMenuItemToggleType toggleType);

	unsafe void SetIcon(void* data, IntPtr length);
}
