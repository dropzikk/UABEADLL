using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnNativeControlHost : IUnknown, IDisposable
{
	IntPtr CreateDefaultChild(IntPtr parent);

	IAvnNativeControlHostTopLevelAttachment CreateAttachment();

	void DestroyDefaultChild(IntPtr child);
}
