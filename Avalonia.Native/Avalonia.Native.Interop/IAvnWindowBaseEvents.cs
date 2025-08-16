using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnWindowBaseEvents : IUnknown, IDisposable
{
	IAvnAutomationPeer AutomationPeer { get; }

	void Paint();

	void Closed();

	void Activated();

	void Deactivated();

	unsafe void Resized(AvnSize* size, AvnPlatformResizeReason reason);

	void PositionChanged(AvnPoint position);

	void RawMouseEvent(AvnRawMouseEventType type, ulong timeStamp, AvnInputModifiers modifiers, AvnPoint point, AvnVector delta);

	int RawKeyEvent(AvnRawKeyEventType type, ulong timeStamp, AvnInputModifiers modifiers, uint key);

	int RawTextInputEvent(ulong timeStamp, string text);

	void ScalingChanged(double scaling);

	void RunRenderPriorityJobs();

	void LostFocus();

	AvnDragDropEffects DragEvent(AvnDragEventType type, AvnPoint position, AvnInputModifiers modifiers, AvnDragDropEffects effects, IAvnClipboard clipboard, IntPtr dataObjectHandle);
}
