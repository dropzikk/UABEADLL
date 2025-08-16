using System;

namespace Avalonia.Controls.Primitives;

public abstract class FlyoutBase : AvaloniaObject
{
	public static readonly DirectProperty<FlyoutBase, bool> IsOpenProperty = AvaloniaProperty.RegisterDirect("IsOpen", (FlyoutBase x) => x.IsOpen, null, unsetValue: false);

	public static readonly DirectProperty<FlyoutBase, Control?> TargetProperty = AvaloniaProperty.RegisterDirect("Target", (FlyoutBase x) => x.Target);

	public static readonly AttachedProperty<FlyoutBase?> AttachedFlyoutProperty = AvaloniaProperty.RegisterAttached<FlyoutBase, Control, FlyoutBase>("AttachedFlyout");

	private bool _isOpen;

	private Control? _target;

	public bool IsOpen
	{
		get
		{
			return _isOpen;
		}
		protected set
		{
			SetAndRaise(IsOpenProperty, ref _isOpen, value);
		}
	}

	public Control? Target
	{
		get
		{
			return _target;
		}
		protected set
		{
			SetAndRaise(TargetProperty, ref _target, value);
		}
	}

	public event EventHandler? Opened;

	public event EventHandler? Closed;

	public static FlyoutBase? GetAttachedFlyout(Control element)
	{
		return element.GetValue(AttachedFlyoutProperty);
	}

	public static void SetAttachedFlyout(Control element, FlyoutBase? value)
	{
		element.SetValue(AttachedFlyoutProperty, value);
	}

	public static void ShowAttachedFlyout(Control flyoutOwner)
	{
		GetAttachedFlyout(flyoutOwner)?.ShowAt(flyoutOwner);
	}

	public abstract void ShowAt(Control placementTarget);

	public abstract void Hide();

	protected virtual void OnOpened()
	{
		this.Opened?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void OnClosed()
	{
		this.Closed?.Invoke(this, EventArgs.Empty);
	}
}
