using System;
using System.Collections.Generic;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.VisualTree;

namespace Avalonia.Controls.Primitives;

public class OverlayPopupHost : ContentControl, IPopupHost, IDisposable, IFocusScope, IManagedPopupPositionerPopup
{
	public static readonly StyledProperty<Transform?> TransformProperty = PopupRoot.TransformProperty.AddOwner<OverlayPopupHost>();

	private readonly OverlayLayer _overlayLayer;

	private readonly ManagedPopupPositioner _positioner;

	private PopupPositionerParameters _positionerParameters;

	private Point _lastRequestedPosition;

	private bool _shown;

	public Visual? HostedVisualTreeRoot => null;

	public Transform? Transform
	{
		get
		{
			return GetValue(TransformProperty);
		}
		set
		{
			SetValue(TransformProperty, value);
		}
	}

	bool IPopupHost.Topmost
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	internal override Interactive? InteractiveParent => base.Parent as Interactive;

	IReadOnlyList<ManagedPopupPositionerScreenInfo> IManagedPopupPositionerPopup.Screens
	{
		get
		{
			Rect rect = new Rect(default(Point), _overlayLayer.AvailableSize);
			return new ManagedPopupPositionerScreenInfo[1]
			{
				new ManagedPopupPositionerScreenInfo(rect, rect)
			};
		}
	}

	Rect IManagedPopupPositionerPopup.ParentClientAreaScreenGeometry => new Rect(default(Point), _overlayLayer.Bounds.Size);

	double IManagedPopupPositionerPopup.Scaling => 1.0;

	public OverlayPopupHost(OverlayLayer overlayLayer)
	{
		_overlayLayer = overlayLayer;
		_positioner = new ManagedPopupPositioner(this);
	}

	public void SetChild(Control? control)
	{
		base.Content = control;
	}

	public void Dispose()
	{
		Hide();
	}

	public void Show()
	{
		_overlayLayer.Children.Add(this);
		_shown = true;
	}

	public void Hide()
	{
		_overlayLayer.Children.Remove(this);
		_shown = false;
	}

	public void ConfigurePosition(Visual target, PlacementMode placement, Point offset, PopupAnchor anchor = PopupAnchor.None, PopupGravity gravity = PopupGravity.None, PopupPositionerConstraintAdjustment constraintAdjustment = PopupPositionerConstraintAdjustment.All, Rect? rect = null)
	{
		_positionerParameters.ConfigurePosition((TopLevel)_overlayLayer.GetVisualRoot(), target, placement, offset, anchor, gravity, constraintAdjustment, rect, base.FlowDirection);
		UpdatePosition();
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		if (_positionerParameters.Size != finalSize)
		{
			_positionerParameters.Size = finalSize;
			UpdatePosition();
		}
		return base.ArrangeOverride(finalSize);
	}

	private void UpdatePosition()
	{
		if (_positionerParameters.Size.Width != 0.0 && _positionerParameters.Size.Height != 0.0 && _shown)
		{
			_positioner.Update(_positionerParameters);
		}
	}

	void IManagedPopupPositionerPopup.MoveAndResize(Point devicePoint, Size virtualSize)
	{
		_lastRequestedPosition = devicePoint;
		MediaContext.Instance.BeginInvokeOnRender(delegate
		{
			Canvas.SetLeft(this, _lastRequestedPosition.X);
			Canvas.SetTop(this, _lastRequestedPosition.Y);
		});
	}

	public static IPopupHost CreatePopupHost(Visual target, IAvaloniaDependencyResolver? dependencyResolver)
	{
		TopLevel topLevel = TopLevel.GetTopLevel(target);
		if (topLevel != null)
		{
			IPopupImpl popupImpl = topLevel.PlatformImpl?.CreatePopup();
			if (popupImpl != null)
			{
				return new PopupRoot(topLevel, popupImpl, dependencyResolver);
			}
		}
		OverlayLayer overlayLayer = OverlayLayer.GetOverlayLayer(target);
		if (overlayLayer != null)
		{
			return new OverlayPopupHost(overlayLayer);
		}
		throw new InvalidOperationException("Unable to create IPopupImpl and no overlay layer is found for the target control");
	}

	double IPopupHost.get_Width()
	{
		return base.Width;
	}

	void IPopupHost.set_Width(double value)
	{
		base.Width = value;
	}

	double IPopupHost.get_MinWidth()
	{
		return base.MinWidth;
	}

	void IPopupHost.set_MinWidth(double value)
	{
		base.MinWidth = value;
	}

	double IPopupHost.get_MaxWidth()
	{
		return base.MaxWidth;
	}

	void IPopupHost.set_MaxWidth(double value)
	{
		base.MaxWidth = value;
	}

	double IPopupHost.get_Height()
	{
		return base.Height;
	}

	void IPopupHost.set_Height(double value)
	{
		base.Height = value;
	}

	double IPopupHost.get_MinHeight()
	{
		return base.MinHeight;
	}

	void IPopupHost.set_MinHeight(double value)
	{
		base.MinHeight = value;
	}

	double IPopupHost.get_MaxHeight()
	{
		return base.MaxHeight;
	}

	void IPopupHost.set_MaxHeight(double value)
	{
		base.MaxHeight = value;
	}
}
