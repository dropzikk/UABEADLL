using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnNativeControlHostTopLevelAttachment : IUnknown, IDisposable
{
	IntPtr ParentHandle { get; }

	void InitializeWithChildHandle(IntPtr child);

	void AttachTo(IAvnNativeControlHost host);

	void ShowInBounds(float x, float y, float width, float height);

	void HideWithSize(float width, float height);

	void ReleaseChild();
}
