using System;
using System.Linq;
using Avalonia.OpenGL;
using Avalonia.Platform;
using Avalonia.Win32.DirectX;
using Avalonia.Win32.OpenGl;
using Avalonia.Win32.OpenGl.Angle;
using Avalonia.Win32.WinRT.Composition;

namespace Avalonia.Win32;

internal static class Win32GlManager
{
	public static IPlatformGraphics? Initialize()
	{
		IPlatformGraphics platformGraphics = InitializeCore();
		if (platformGraphics != null)
		{
			AvaloniaLocator.CurrentMutable.Bind<IPlatformGraphics>().ToConstant(platformGraphics);
		}
		if (platformGraphics is IPlatformGraphicsOpenGlContextFactory constant)
		{
			AvaloniaLocator.CurrentMutable.Bind<IPlatformGraphicsOpenGlContextFactory>().ToConstant(constant);
		}
		return platformGraphics;
	}

	private static IPlatformGraphics? InitializeCore()
	{
		Win32PlatformOptions win32PlatformOptions = AvaloniaLocator.Current.GetService<Win32PlatformOptions>() ?? new Win32PlatformOptions();
		if (win32PlatformOptions.RenderingMode == null || !win32PlatformOptions.RenderingMode.Any())
		{
			throw new InvalidOperationException("Win32PlatformOptions.RenderingMode must not be empty or null");
		}
		foreach (Win32RenderingMode item in win32PlatformOptions.RenderingMode)
		{
			switch (item)
			{
			case Win32RenderingMode.Software:
				return null;
			case Win32RenderingMode.AngleEgl:
			{
				AngleWin32PlatformGraphics angleWin32PlatformGraphics = AngleWin32PlatformGraphics.TryCreate(AvaloniaLocator.Current.GetService<AngleOptions>() ?? new AngleOptions());
				if (angleWin32PlatformGraphics != null && angleWin32PlatformGraphics.PlatformApi == AngleOptions.PlatformApi.DirectX11)
				{
					TryRegisterComposition(win32PlatformOptions);
					return angleWin32PlatformGraphics;
				}
				break;
			}
			}
			if (item == Win32RenderingMode.Wgl)
			{
				WglPlatformOpenGlInterface wglPlatformOpenGlInterface = WglPlatformOpenGlInterface.TryCreate();
				if (wglPlatformOpenGlInterface != null)
				{
					return wglPlatformOpenGlInterface;
				}
			}
		}
		throw new InvalidOperationException($"{"Win32PlatformOptions"}.{"RenderingMode"} has a value of \"{string.Join(", ", win32PlatformOptions.RenderingMode)}\", but no options were applied.");
	}

	private static void TryRegisterComposition(Win32PlatformOptions opts)
	{
		if (opts.CompositionMode == null || !opts.CompositionMode.Any())
		{
			throw new InvalidOperationException("Win32PlatformOptions.CompositionMode must not be empty or null");
		}
		foreach (Win32CompositionMode item in opts.CompositionMode)
		{
			switch (item)
			{
			case Win32CompositionMode.RedirectionSurface:
				return;
			case Win32CompositionMode.WinUIComposition:
				if (WinUiCompositorConnection.IsSupported() && WinUiCompositorConnection.TryCreateAndRegister())
				{
					return;
				}
				break;
			}
			if (item == Win32CompositionMode.LowLatencyDxgiSwapChain && DxgiConnection.TryCreateAndRegister())
			{
				return;
			}
		}
		throw new InvalidOperationException($"{"Win32PlatformOptions"}.{"CompositionMode"} has a value of \"{string.Join(", ", opts.CompositionMode)}\", but no options were applied.");
	}
}
