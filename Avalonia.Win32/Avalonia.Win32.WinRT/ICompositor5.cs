using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositor5 : IInspectable, IUnknown, IDisposable
{
	IntPtr Comment { get; }

	float GlobalPlaybackRate { get; }

	void SetComment(IntPtr value);

	void SetGlobalPlaybackRate(float value);

	unsafe void* CreateBounceScalarAnimation();

	unsafe void* CreateBounceVector2Animation();

	unsafe void* CreateBounceVector3Animation();

	unsafe void* CreateContainerShape();

	unsafe void* CreateEllipseGeometry();

	unsafe void* CreateLineGeometry();

	unsafe void* CreatePathGeometry();

	unsafe void* CreatePathGeometryWithPath(void* path);

	unsafe void* CreatePathKeyFrameAnimation();

	unsafe void* CreateRectangleGeometry();

	ICompositionRoundedRectangleGeometry CreateRoundedRectangleGeometry();

	IShapeVisual CreateShapeVisual();

	unsafe void* CreateSpriteShape();

	unsafe void* CreateSpriteShapeWithGeometry(void* geometry);

	unsafe void* CreateViewBox();

	IAsyncAction RequestCommitAsync();
}
