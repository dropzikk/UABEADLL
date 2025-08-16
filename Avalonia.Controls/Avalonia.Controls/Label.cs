using Avalonia.Automation.Peers;
using Avalonia.Controls.Automation.Peers;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Controls;

public class Label : ContentControl
{
	public static readonly StyledProperty<IInputElement?> TargetProperty;

	[ResolveByName]
	public IInputElement? Target
	{
		get
		{
			return GetValue(TargetProperty);
		}
		set
		{
			SetValue(TargetProperty, value);
		}
	}

	static Label()
	{
		TargetProperty = AvaloniaProperty.Register<Label, IInputElement>("Target");
		AccessKeyHandler.AccessKeyPressedEvent.AddClassHandler(delegate(Label lbl, RoutedEventArgs args)
		{
			lbl.LabelActivated(args);
		});
		InputElement.FocusableProperty.OverrideDefaultValue<Label>(defaultValue: false);
	}

	private void LabelActivated(RoutedEventArgs args)
	{
		Target?.Focus();
		args.Handled = Target != null;
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
		{
			LabelActivated(e);
		}
		base.OnPointerPressed(e);
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new LabelAutomationPeer(this);
	}
}
