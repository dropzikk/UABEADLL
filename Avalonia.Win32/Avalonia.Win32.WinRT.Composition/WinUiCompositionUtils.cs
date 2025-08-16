using System;
using System.Numerics;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Composition;

internal static class WinUiCompositionUtils
{
	public static ICompositionBrush? CreateMicaBackdropBrush(ICompositor compositor, float color, float opacity)
	{
		if (Win32Platform.WindowsVersion.Build < 22000)
		{
			return null;
		}
		using ICompositionEffectSourceParameterFactory backDropParameterFactory = NativeWinRTMethods.CreateActivationFactory<ICompositionEffectSourceParameterFactory>("Windows.UI.Composition.CompositionEffectSourceParameter");
		float[] color2 = new float[4]
		{
			color / 255f,
			color / 255f,
			color / 255f,
			1f
		};
		using ColorSourceEffect colorSourceEffect = new ColorSourceEffect(color2);
		using OpacityEffect graphicsEffect = new OpacityEffect(1f, colorSourceEffect);
		using ICompositionEffectFactory compositionEffectFactory = compositor.CreateEffectFactory(graphicsEffect);
		using ICompositionEffectBrush unknown = compositionEffectFactory.CreateBrush();
		using ICompositionBrush source4 = unknown.QueryInterface<ICompositionBrush>();
		using ColorSourceEffect colorSourceEffect2 = new ColorSourceEffect(color2);
		using OpacityEffect graphicsEffect2 = new OpacityEffect(opacity, colorSourceEffect2);
		using ICompositionEffectFactory compositionEffectFactory2 = compositor.CreateEffectFactory(graphicsEffect2);
		using ICompositionEffectBrush unknown2 = compositionEffectFactory2.CreateBrush();
		using ICompositionBrush source2 = unknown2.QueryInterface<ICompositionBrush>();
		using ICompositorWithBlurredWallpaperBackdropBrush compositorWithBlurredWallpaperBackdropBrush = compositor.QueryInterface<ICompositorWithBlurredWallpaperBackdropBrush>();
		using ICompositionBackdropBrush compositionBackdropBrush = compositorWithBlurredWallpaperBackdropBrush?.TryCreateBlurredWallpaperBackdropBrush();
		using ICompositionBrush source = compositionBackdropBrush?.QueryInterface<ICompositionBrush>();
		IntPtr handle;
		using IGraphicsEffectSource graphicsEffectSource = GetParameterSource("Background", backDropParameterFactory, out handle);
		IntPtr handle2;
		using IGraphicsEffectSource graphicsEffectSource2 = GetParameterSource("Foreground", backDropParameterFactory, out handle2);
		using BlendEffect graphicsEffect3 = new BlendEffect(23, graphicsEffectSource, graphicsEffectSource2);
		using ICompositionEffectFactory compositionEffectFactory3 = compositor.CreateEffectFactory(graphicsEffect3);
		using ICompositionEffectBrush compositionEffectBrush = compositionEffectFactory3.CreateBrush();
		using ICompositionBrush source3 = compositionEffectBrush.QueryInterface<ICompositionBrush>();
		compositionEffectBrush.SetSourceParameter(handle, source);
		compositionEffectBrush.SetSourceParameter(handle2, source2);
		IntPtr handle3;
		using IGraphicsEffectSource graphicsEffectSource3 = GetParameterSource("Background", backDropParameterFactory, out handle3);
		IntPtr handle4;
		using IGraphicsEffectSource graphicsEffectSource4 = GetParameterSource("Foreground", backDropParameterFactory, out handle4);
		using BlendEffect graphicsEffect4 = new BlendEffect(22, graphicsEffectSource3, graphicsEffectSource4);
		using ICompositionEffectFactory compositionEffectFactory4 = compositor.CreateEffectFactory(graphicsEffect4);
		using ICompositionEffectBrush compositionEffectBrush2 = compositionEffectFactory4.CreateBrush();
		compositionEffectBrush2.SetSourceParameter(handle3, source3);
		compositionEffectBrush2.SetSourceParameter(handle4, source4);
		using ICompositionBrush iface = compositionEffectBrush2.QueryInterface<ICompositionBrush>();
		return iface.CloneReference();
	}

	public static ICompositionBrush CreateAcrylicBlurBackdropBrush(ICompositor compositor)
	{
		using ICompositionEffectSourceParameterFactory compositionEffectSourceParameterFactory = NativeWinRTMethods.CreateActivationFactory<ICompositionEffectSourceParameterFactory>("Windows.UI.Composition.CompositionEffectSourceParameter");
		using HStringInterop hStringInterop = new HStringInterop("backdrop");
		using ICompositionEffectSourceParameter unknown = compositionEffectSourceParameterFactory.Create(hStringInterop.Handle);
		using IGraphicsEffectSource source = unknown.QueryInterface<IGraphicsEffectSource>();
		WinUIGaussianBlurEffect winUIGaussianBlurEffect = new WinUIGaussianBlurEffect(source);
		using ICompositionEffectFactory compositionEffectFactory = compositor.CreateEffectFactory(winUIGaussianBlurEffect);
		using ICompositionEffectBrush compositionEffectBrush = compositionEffectFactory.CreateBrush();
		using ICompositionBrush source2 = CreateBackdropBrush(compositor);
		SaturationEffect graphicsEffect = new SaturationEffect(winUIGaussianBlurEffect);
		using ICompositionEffectFactory compositionEffectFactory2 = compositor.CreateEffectFactory(graphicsEffect);
		using (compositionEffectFactory2.CreateBrush())
		{
			compositionEffectBrush.SetSourceParameter(hStringInterop.Handle, source2);
			return compositionEffectBrush.QueryInterface<ICompositionBrush>();
		}
	}

	public static ICompositionRoundedRectangleGeometry? ClipVisual(ICompositor compositor, float? _backdropCornerRadius, params IVisual?[] containerVisuals)
	{
		if (!_backdropCornerRadius.HasValue)
		{
			return null;
		}
		using ICompositor5 compositor2 = compositor.QueryInterface<ICompositor5>();
		using ICompositionRoundedRectangleGeometry compositionRoundedRectangleGeometry = compositor2.CreateRoundedRectangleGeometry();
		compositionRoundedRectangleGeometry.SetCornerRadius(new Vector2(_backdropCornerRadius.Value, _backdropCornerRadius.Value));
		using ICompositor6 compositor3 = compositor.QueryInterface<ICompositor6>();
		using ICompositionGeometry geometry = compositionRoundedRectangleGeometry.QueryInterface<ICompositionGeometry>();
		using ICompositionGeometricClip unknown = compositor3.CreateGeometricClipWithGeometry(geometry);
		for (int i = 0; i < containerVisuals.Length; i++)
		{
			containerVisuals[i]?.SetClip(unknown.QueryInterface<ICompositionClip>());
		}
		return compositionRoundedRectangleGeometry.CloneReference();
	}

	public static IVisual CreateBlurVisual(ICompositor compositor, ICompositionBrush compositionBrush)
	{
		using ISpriteVisual spriteVisual = compositor.CreateSpriteVisual();
		using IVisual visual = spriteVisual.QueryInterface<IVisual>();
		using IVisual2 visual2 = spriteVisual.QueryInterface<IVisual2>();
		spriteVisual.SetBrush(compositionBrush);
		visual.SetIsVisible(0);
		visual2.SetRelativeSizeAdjustment(new Vector2(1f, 1f));
		return visual.CloneReference();
	}

	public static ICompositionBrush CreateBackdropBrush(ICompositor compositor)
	{
		ICompositionBackdropBrush compositionBackdropBrush = null;
		try
		{
			if (Win32Platform.WindowsVersion >= WinUiCompositionShared.MinHostBackdropVersion)
			{
				using ICompositor3 compositor2 = compositor.QueryInterface<ICompositor3>();
				compositionBackdropBrush = compositor2.CreateHostBackdropBrush();
			}
			else
			{
				using ICompositor2 compositor3 = compositor.QueryInterface<ICompositor2>();
				compositionBackdropBrush = compositor3.CreateBackdropBrush();
			}
			return compositionBackdropBrush.QueryInterface<ICompositionBrush>();
		}
		finally
		{
			compositionBackdropBrush?.Dispose();
		}
	}

	private static IGraphicsEffectSource GetParameterSource(string name, ICompositionEffectSourceParameterFactory backDropParameterFactory, out IntPtr handle)
	{
		HStringInterop hStringInterop = new HStringInterop(name);
		IGraphicsEffectSource result = backDropParameterFactory.Create(hStringInterop.Handle).QueryInterface<IGraphicsEffectSource>();
		handle = hStringInterop.Handle;
		return result;
	}
}
