using System;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace Avalonia.Controls.Primitives;

public sealed class PopupRoot : WindowBase, IHostedVisualTreeRoot, IDisposable, IStyleHost, IPopupHost, IFocusScope
{
	public static readonly StyledProperty<Transform?> TransformProperty;

	private PopupPositionerParameters _positionerParameters;

	public new IPopupImpl? PlatformImpl => (IPopupImpl)base.PlatformImpl;

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

	internal override Interactive? InteractiveParent => (Interactive)base.Parent;

	Visual? IHostedVisualTreeRoot.Host => base.VisualParent;

	IStyleHost? IStyleHost.StylingParent => base.Parent;

	public TopLevel ParentTopLevel { get; }

	Visual IPopupHost.HostedVisualTreeRoot => this;

	static PopupRoot()
	{
		TransformProperty = AvaloniaProperty.Register<PopupRoot, Transform>("Transform");
		TemplatedControl.BackgroundProperty.OverrideDefaultValue(typeof(PopupRoot), Brushes.White);
	}

	public PopupRoot(TopLevel parent, IPopupImpl impl)
		: this(parent, impl, null)
	{
	}

	public PopupRoot(TopLevel parent, IPopupImpl impl, IAvaloniaDependencyResolver? dependencyResolver)
		: base(impl, dependencyResolver)
	{
		ParentTopLevel = parent;
	}

	public void Dispose()
	{
		PlatformImpl?.Dispose();
	}

	private void UpdatePosition()
	{
		PlatformImpl?.PopupPositioner?.Update(_positionerParameters);
	}

	public void ConfigurePosition(Visual target, PlacementMode placement, Point offset, PopupAnchor anchor = PopupAnchor.None, PopupGravity gravity = PopupGravity.None, PopupPositionerConstraintAdjustment constraintAdjustment = PopupPositionerConstraintAdjustment.All, Rect? rect = null)
	{
		_positionerParameters.ConfigurePosition(ParentTopLevel, target, placement, offset, anchor, gravity, constraintAdjustment, rect, base.FlowDirection);
		if (_positionerParameters.Size != default(Size))
		{
			UpdatePosition();
		}
	}

	public void SetChild(Control? control)
	{
		base.Content = control;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		Size size = PlatformImpl?.MaxAutoSizeHint ?? Size.Infinity;
		Size availableSize2 = availableSize;
		if (double.IsInfinity(availableSize2.Width))
		{
			availableSize2 = availableSize2.WithWidth(size.Width);
		}
		if (double.IsInfinity(availableSize2.Height))
		{
			availableSize2 = availableSize2.WithHeight(size.Height);
		}
		Size size2 = base.MeasureOverride(availableSize2);
		double val = size2.Width;
		double val2 = size2.Height;
		double width = base.Width;
		double height = base.Height;
		if (!double.IsNaN(width))
		{
			val = width;
		}
		val = Math.Min(val, base.MaxWidth);
		val = Math.Max(val, base.MinWidth);
		if (!double.IsNaN(height))
		{
			val2 = height;
		}
		val2 = Math.Min(val2, base.MaxHeight);
		val2 = Math.Max(val2, base.MinHeight);
		return new Size(val, val2);
	}

	protected sealed override Size ArrangeSetBounds(Size size)
	{
		_positionerParameters.Size = size;
		UpdatePosition();
		return base.ClientSize;
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new PopupRootAutomationPeer(this);
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
