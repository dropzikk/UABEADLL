using System;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Reactive;

namespace Avalonia.Controls;

[PseudoClasses(new string[] { ":open" })]
public class ToolTip : ContentControl, IPopupHostProvider
{
	public static readonly AttachedProperty<object?> TipProperty;

	public static readonly AttachedProperty<bool> IsOpenProperty;

	public static readonly AttachedProperty<PlacementMode> PlacementProperty;

	public static readonly AttachedProperty<double> HorizontalOffsetProperty;

	public static readonly AttachedProperty<double> VerticalOffsetProperty;

	public static readonly AttachedProperty<int> ShowDelayProperty;

	internal static readonly AttachedProperty<ToolTip?> ToolTipProperty;

	private IPopupHost? _popupHost;

	private Action<IPopupHost?>? _popupHostChangedHandler;

	IPopupHost? IPopupHostProvider.PopupHost => _popupHost;

	event Action<IPopupHost?>? IPopupHostProvider.PopupHostChanged
	{
		add
		{
			_popupHostChangedHandler = (Action<IPopupHost>)Delegate.Combine(_popupHostChangedHandler, value);
		}
		remove
		{
			_popupHostChangedHandler = (Action<IPopupHost>)Delegate.Remove(_popupHostChangedHandler, value);
		}
	}

	static ToolTip()
	{
		TipProperty = AvaloniaProperty.RegisterAttached<ToolTip, Control, object>("Tip");
		IsOpenProperty = AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("IsOpen", defaultValue: false);
		PlacementProperty = AvaloniaProperty.RegisterAttached<ToolTip, Control, PlacementMode>("Placement", PlacementMode.Pointer);
		HorizontalOffsetProperty = AvaloniaProperty.RegisterAttached<ToolTip, Control, double>("HorizontalOffset", 0.0);
		VerticalOffsetProperty = AvaloniaProperty.RegisterAttached<ToolTip, Control, double>("VerticalOffset", 20.0);
		ShowDelayProperty = AvaloniaProperty.RegisterAttached<ToolTip, Control, int>("ShowDelay", 400);
		ToolTipProperty = AvaloniaProperty.RegisterAttached<ToolTip, Control, ToolTip>("ToolTip");
		TipProperty.Changed.Subscribe(ToolTipService.Instance.TipChanged);
		IsOpenProperty.Changed.Subscribe(ToolTipService.Instance.TipOpenChanged);
		IsOpenProperty.Changed.Subscribe(IsOpenChanged);
		HorizontalOffsetProperty.Changed.Subscribe(RecalculatePositionOnPropertyChanged);
		VerticalOffsetProperty.Changed.Subscribe(RecalculatePositionOnPropertyChanged);
		PlacementProperty.Changed.Subscribe(RecalculatePositionOnPropertyChanged);
	}

	public static object? GetTip(Control element)
	{
		return element.GetValue(TipProperty);
	}

	public static void SetTip(Control element, object? value)
	{
		element.SetValue(TipProperty, value);
	}

	public static bool GetIsOpen(Control element)
	{
		return element.GetValue(IsOpenProperty);
	}

	public static void SetIsOpen(Control element, bool value)
	{
		element.SetValue(IsOpenProperty, value);
	}

	public static PlacementMode GetPlacement(Control element)
	{
		return element.GetValue(PlacementProperty);
	}

	public static void SetPlacement(Control element, PlacementMode value)
	{
		element.SetValue(PlacementProperty, value);
	}

	public static double GetHorizontalOffset(Control element)
	{
		return element.GetValue(HorizontalOffsetProperty);
	}

	public static void SetHorizontalOffset(Control element, double value)
	{
		element.SetValue(HorizontalOffsetProperty, value);
	}

	public static double GetVerticalOffset(Control element)
	{
		return element.GetValue(VerticalOffsetProperty);
	}

	public static void SetVerticalOffset(Control element, double value)
	{
		element.SetValue(VerticalOffsetProperty, value);
	}

	public static int GetShowDelay(Control element)
	{
		return element.GetValue(ShowDelayProperty);
	}

	public static void SetShowDelay(Control element, int value)
	{
		element.SetValue(ShowDelayProperty, value);
	}

	private static void IsOpenChanged(AvaloniaPropertyChangedEventArgs e)
	{
		Control control = (Control)e.Sender;
		bool flag = (bool)e.NewValue;
		ToolTip toolTip;
		if (flag)
		{
			object tip = GetTip(control);
			if (tip == null)
			{
				return;
			}
			toolTip = control.GetValue(ToolTipProperty);
			if (toolTip == null || (tip != toolTip && tip != toolTip.Content))
			{
				toolTip?.Close();
				toolTip = (tip as ToolTip) ?? new ToolTip
				{
					Content = tip
				};
				control.SetValue(ToolTipProperty, toolTip);
			}
			toolTip.Open(control);
		}
		else
		{
			toolTip = control.GetValue(ToolTipProperty);
			toolTip?.Close();
		}
		toolTip?.UpdatePseudoClasses(flag);
	}

	private static void RecalculatePositionOnPropertyChanged(AvaloniaPropertyChangedEventArgs args)
	{
		Control control = (Control)args.Sender;
		control.GetValue(ToolTipProperty)?.RecalculatePosition(control);
	}

	internal void RecalculatePosition(Control control)
	{
		_popupHost?.ConfigurePosition(control, GetPlacement(control), new Point(GetHorizontalOffset(control), GetVerticalOffset(control)), PopupAnchor.None, PopupGravity.None, PopupPositionerConstraintAdjustment.All, null);
	}

	private void Open(Control control)
	{
		Close();
		_popupHost = OverlayPopupHost.CreatePopupHost(control, null);
		_popupHost.SetChild(this);
		((ISetLogicalParent)_popupHost).SetParent(control);
		TemplatedControl.ApplyTemplatedParent(this, control.TemplatedParent);
		_popupHost.ConfigurePosition(control, GetPlacement(control), new Point(GetHorizontalOffset(control), GetVerticalOffset(control)), PopupAnchor.None, PopupGravity.None, PopupPositionerConstraintAdjustment.All, null);
		WindowManagerAddShadowHintChanged(_popupHost, hint: false);
		_popupHost.Show();
		_popupHostChangedHandler?.Invoke(_popupHost);
	}

	private void Close()
	{
		if (_popupHost != null)
		{
			_popupHost.SetChild(null);
			_popupHost.Dispose();
			_popupHost = null;
			_popupHostChangedHandler?.Invoke(null);
		}
	}

	private void WindowManagerAddShadowHintChanged(IPopupHost host, bool hint)
	{
		if (host is PopupRoot popupRoot)
		{
			popupRoot.PlatformImpl?.SetWindowManagerAddShadowHint(hint);
		}
	}

	private void UpdatePseudoClasses(bool newValue)
	{
		base.PseudoClasses.Set(":open", newValue);
	}
}
