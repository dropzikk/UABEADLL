using System;
using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Metadata;
using Avalonia.Platform;
using Avalonia.Rendering.Composition;

namespace Avalonia.Controls.Embedding.Offscreen;

[Unstable]
public abstract class OffscreenTopLevelImplBase : ITopLevelImpl, IOptionalFeatureProvider, IDisposable
{
	private double _scaling = 1.0;

	private Size _clientSize;

	public IInputRoot? InputRoot { get; private set; }

	public bool IsDisposed { get; private set; }

	public Compositor Compositor { get; }

	public abstract IEnumerable<object> Surfaces { get; }

	public Size ClientSize
	{
		get
		{
			return _clientSize;
		}
		set
		{
			_clientSize = value;
			Resized?.Invoke(value, WindowResizeReason.Unspecified);
		}
	}

	public Size? FrameSize => null;

	public double RenderScaling
	{
		get
		{
			return _scaling;
		}
		set
		{
			_scaling = value;
			ScalingChanged?.Invoke(value);
		}
	}

	public Action<RawInputEventArgs>? Input { get; set; }

	public Action<Rect>? Paint { get; set; }

	public Action<Size, WindowResizeReason>? Resized { get; set; }

	public Action<double>? ScalingChanged { get; set; }

	public Action<WindowTransparencyLevel>? TransparencyLevelChanged { get; set; }

	public AcrylicPlatformCompensationLevels AcrylicCompensationLevels { get; } = new AcrylicPlatformCompensationLevels(1.0, 1.0, 1.0);

	public Action? Closed { get; set; }

	public Action? LostFocus { get; set; }

	public abstract IMouseDevice MouseDevice { get; }

	public WindowTransparencyLevel TransparencyLevel => WindowTransparencyLevel.None;

	public virtual void Dispose()
	{
		IsDisposed = true;
	}

	public OffscreenTopLevelImplBase()
	{
		Compositor = new Compositor(null);
	}

	public void SetFrameThemeVariant(PlatformThemeVariant themeVariant)
	{
	}

	public void SetInputRoot(IInputRoot inputRoot)
	{
		InputRoot = inputRoot;
	}

	public virtual Point PointToClient(PixelPoint point)
	{
		return point.ToPoint(1.0);
	}

	public virtual PixelPoint PointToScreen(Point point)
	{
		return PixelPoint.FromPoint(point, 1.0);
	}

	public virtual void SetCursor(ICursorImpl? cursor)
	{
	}

	public void SetTransparencyLevelHint(IReadOnlyList<WindowTransparencyLevel> transparencyLevel)
	{
	}

	public IPopupImpl? CreatePopup()
	{
		return null;
	}

	public virtual object? TryGetFeature(Type featureType)
	{
		return null;
	}
}
