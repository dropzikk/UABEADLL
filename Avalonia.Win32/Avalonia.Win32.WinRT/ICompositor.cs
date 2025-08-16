using System;
using System.Numerics;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositor : IInspectable, IUnknown, IDisposable
{
	unsafe void* CreateColorKeyFrameAnimation();

	unsafe void* CreateColorBrush();

	unsafe ICompositionColorBrush CreateColorBrushWithColor(WinRTColor* color);

	IContainerVisual CreateContainerVisual();

	unsafe void* CreateCubicBezierEasingFunction(Vector2 controlPoint1, Vector2 controlPoint2);

	ICompositionEffectFactory CreateEffectFactory(IGraphicsEffect graphicsEffect);

	unsafe void* CreateEffectFactoryWithProperties(void* graphicsEffect, void* animatableProperties);

	unsafe void* CreateExpressionAnimation();

	unsafe void* CreateExpressionAnimationWithExpression(IntPtr expression);

	unsafe void* CreateInsetClip();

	unsafe void* CreateInsetClipWithInsets(float leftInset, float topInset, float rightInset, float bottomInset);

	unsafe void* CreateLinearEasingFunction();

	unsafe void* CreatePropertySet();

	unsafe void* CreateQuaternionKeyFrameAnimation();

	unsafe void* CreateScalarKeyFrameAnimation();

	ICompositionScopedBatch CreateScopedBatch(CompositionBatchTypes batchType);

	ISpriteVisual CreateSpriteVisual();

	ICompositionSurfaceBrush CreateSurfaceBrush();

	ICompositionSurfaceBrush CreateSurfaceBrushWithSurface(ICompositionSurface surface);

	unsafe void* CreateTargetForCurrentView();

	unsafe void* CreateVector2KeyFrameAnimation();

	unsafe void* CreateVector3KeyFrameAnimation();

	unsafe void* CreateVector4KeyFrameAnimation();

	unsafe void* GetCommitBatch(CompositionBatchTypes batchType);
}
