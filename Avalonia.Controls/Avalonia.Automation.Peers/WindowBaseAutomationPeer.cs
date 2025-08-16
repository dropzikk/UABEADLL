using System;
using System.ComponentModel;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform;
using Avalonia.VisualTree;

namespace Avalonia.Automation.Peers;

public class WindowBaseAutomationPeer : ControlAutomationPeer, IRootProvider
{
	private Control? _focus;

	public new WindowBase Owner => (WindowBase)base.Owner;

	public ITopLevelImpl? PlatformImpl => Owner.PlatformImpl;

	public event EventHandler? FocusChanged;

	public WindowBaseAutomationPeer(WindowBase owner)
		: base(owner)
	{
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.Window;
	}

	public AutomationPeer? GetFocus()
	{
		if (_focus == null)
		{
			return null;
		}
		return GetOrCreate(_focus);
	}

	public AutomationPeer? GetPeerFromPoint(Point p)
	{
		Control control = Owner.GetVisualAt(p)?.FindAncestorOfType<Control>(includeSelf: true);
		if (control == null)
		{
			return null;
		}
		return GetOrCreate(control);
	}

	protected void StartTrackingFocus()
	{
		if (KeyboardDevice.Instance != null)
		{
			KeyboardDevice.Instance.PropertyChanged += KeyboardDevicePropertyChanged;
			OnFocusChanged(KeyboardDevice.Instance.FocusedElement);
		}
	}

	protected void StopTrackingFocus()
	{
		if (KeyboardDevice.Instance != null)
		{
			KeyboardDevice.Instance.PropertyChanged -= KeyboardDevicePropertyChanged;
		}
	}

	private void OnFocusChanged(IInputElement? focus)
	{
		Control focus2 = _focus;
		Control control = focus as Control;
		_focus = ((control?.VisualRoot == Owner) ? control : null);
		if (_focus != focus2)
		{
			if (_focus != null && _focus != Owner)
			{
				GetOrCreate(_focus);
			}
			this.FocusChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private void KeyboardDevicePropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == "FocusedElement")
		{
			OnFocusChanged(KeyboardDevice.Instance.FocusedElement);
		}
	}
}
