using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnWindowBase : IUnknown, IDisposable
{
	AvnSize ClientSize { get; }

	double Scaling { get; }

	AvnPoint Position { get; }

	IAvnTextInputMethod InputMethod { get; }

	void Show(int activate, int isDialog);

	void Hide();

	void Close();

	void Activate();

	unsafe void GetFrameSize(AvnSize* result);

	void SetMinMaxSize(AvnSize minSize, AvnSize maxSize);

	void Resize(double width, double height, AvnPlatformResizeReason reason);

	void Invalidate(AvnRect rect);

	void BeginMoveDrag();

	void BeginResizeDrag(AvnWindowEdge edge);

	void SetPosition(AvnPoint point);

	AvnPoint PointToClient(AvnPoint point);

	AvnPoint PointToScreen(AvnPoint point);

	void SetTopMost(int value);

	void SetCursor(IAvnCursor cursor);

	IAvnGlSurfaceRenderTarget CreateGlRenderTarget(IAvnGlContext context);

	IAvnSoftwareRenderTarget CreateSoftwareRenderTarget();

	IAvnMetalRenderTarget CreateMetalRenderTarget(IAvnMetalDevice device);

	void SetMainMenu(IAvnMenu menu);

	IntPtr ObtainNSWindowHandle();

	IntPtr ObtainNSWindowHandleRetained();

	IntPtr ObtainNSViewHandle();

	IntPtr ObtainNSViewHandleRetained();

	IAvnNativeControlHost CreateNativeControlHost();

	void BeginDragAndDropOperation(AvnDragDropEffects effects, AvnPoint point, IAvnClipboard clipboard, IAvnDndResultCallback cb, IntPtr sourceHandle);

	void SetTransparencyMode(AvnWindowTransparencyMode mode);

	void SetFrameThemeVariant(AvnPlatformThemeVariant mode);
}
