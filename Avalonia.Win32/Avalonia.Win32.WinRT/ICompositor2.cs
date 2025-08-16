using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositor2 : IInspectable, IUnknown, IDisposable
{
	unsafe void* CreateAmbientLight();

	unsafe void* CreateAnimationGroup();

	ICompositionBackdropBrush CreateBackdropBrush();

	unsafe void* CreateDistantLight();

	unsafe void* CreateDropShadow();

	unsafe void* CreateImplicitAnimationCollection();

	unsafe void* CreateLayerVisual();

	unsafe void* CreateMaskBrush();

	unsafe void* CreateNineGridBrush();

	unsafe void* CreatePointLight();

	unsafe void* CreateSpotLight();

	unsafe void* CreateStepEasingFunction();

	unsafe void* CreateStepEasingFunctionWithStepCount(int stepCount);
}
