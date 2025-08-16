using System;
using Avalonia.Automation.Peers;
using Avalonia.Controls;

namespace Avalonia.Automation;

public static class AutomationProperties
{
	internal const int AutomationPositionInSetDefault = -1;

	internal const int AutomationSizeOfSetDefault = -1;

	public static readonly AttachedProperty<string?> AcceleratorKeyProperty = AvaloniaProperty.RegisterAttached<StyledElement, string>("AcceleratorKey", typeof(AutomationProperties));

	public static readonly AttachedProperty<AccessibilityView> AccessibilityViewProperty = AvaloniaProperty.RegisterAttached<StyledElement, AccessibilityView>("AccessibilityView", typeof(AutomationProperties), AccessibilityView.Default);

	public static readonly AttachedProperty<string?> AccessKeyProperty = AvaloniaProperty.RegisterAttached<StyledElement, string>("AccessKey", typeof(AutomationProperties));

	public static readonly AttachedProperty<string?> AutomationIdProperty = AvaloniaProperty.RegisterAttached<StyledElement, string>("AutomationId", typeof(AutomationProperties));

	public static readonly AttachedProperty<AutomationControlType?> ControlTypeOverrideProperty = AvaloniaProperty.RegisterAttached<StyledElement, AutomationControlType?>("ControlTypeOverride", typeof(AutomationProperties), null);

	public static readonly AttachedProperty<string?> HelpTextProperty = AvaloniaProperty.RegisterAttached<StyledElement, string>("HelpText", typeof(AutomationProperties));

	public static readonly AttachedProperty<bool> IsColumnHeaderProperty = AvaloniaProperty.RegisterAttached<StyledElement, bool>("IsColumnHeader", typeof(AutomationProperties), defaultValue: false);

	public static readonly AttachedProperty<bool> IsRequiredForFormProperty = AvaloniaProperty.RegisterAttached<StyledElement, bool>("IsRequiredForForm", typeof(AutomationProperties), defaultValue: false);

	public static readonly AttachedProperty<bool> IsRowHeaderProperty = AvaloniaProperty.RegisterAttached<StyledElement, bool>("IsRowHeader", typeof(AutomationProperties), defaultValue: false);

	public static readonly AttachedProperty<IsOffscreenBehavior> IsOffscreenBehaviorProperty = AvaloniaProperty.RegisterAttached<StyledElement, IsOffscreenBehavior>("IsOffscreenBehavior", typeof(AutomationProperties), IsOffscreenBehavior.Default);

	public static readonly AttachedProperty<string?> ItemStatusProperty = AvaloniaProperty.RegisterAttached<StyledElement, string>("ItemStatus", typeof(AutomationProperties));

	public static readonly AttachedProperty<string?> ItemTypeProperty = AvaloniaProperty.RegisterAttached<StyledElement, string>("ItemType", typeof(AutomationProperties));

	public static readonly AttachedProperty<Control> LabeledByProperty = AvaloniaProperty.RegisterAttached<StyledElement, Control>("LabeledBy", typeof(AutomationProperties));

	public static readonly AttachedProperty<AutomationLiveSetting> LiveSettingProperty = AvaloniaProperty.RegisterAttached<StyledElement, AutomationLiveSetting>("LiveSetting", typeof(AutomationProperties), AutomationLiveSetting.Off);

	public static readonly AttachedProperty<string?> NameProperty = AvaloniaProperty.RegisterAttached<StyledElement, string>("Name", typeof(AutomationProperties));

	public static readonly AttachedProperty<int> PositionInSetProperty = AvaloniaProperty.RegisterAttached<StyledElement, int>("PositionInSet", typeof(AutomationProperties), -1);

	public static readonly AttachedProperty<int> SizeOfSetProperty = AvaloniaProperty.RegisterAttached<StyledElement, int>("SizeOfSet", typeof(AutomationProperties), -1);

	public static void SetAcceleratorKey(StyledElement element, string value)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		element.SetValue(AcceleratorKeyProperty, value);
	}

	public static string? GetAcceleratorKey(StyledElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		return element.GetValue(AcceleratorKeyProperty);
	}

	public static void SetAccessibilityView(StyledElement element, AccessibilityView value)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		element.SetValue(AccessibilityViewProperty, value);
	}

	public static AccessibilityView GetAccessibilityView(StyledElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		return element.GetValue(AccessibilityViewProperty);
	}

	public static void SetAccessKey(StyledElement element, string value)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		element.SetValue(AccessKeyProperty, value);
	}

	public static string? GetAccessKey(StyledElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		return element.GetValue(AccessKeyProperty);
	}

	public static void SetAutomationId(StyledElement element, string? value)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		element.SetValue(AutomationIdProperty, value);
	}

	public static string? GetAutomationId(StyledElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		return element.GetValue(AutomationIdProperty);
	}

	public static void SetControlTypeOverride(StyledElement element, AutomationControlType? value)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		element.SetValue(ControlTypeOverrideProperty, value);
	}

	public static AutomationControlType? GetControlTypeOverride(StyledElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		return element.GetValue(ControlTypeOverrideProperty);
	}

	public static void SetHelpText(StyledElement element, string? value)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		element.SetValue(HelpTextProperty, value);
	}

	public static string? GetHelpText(StyledElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		return element.GetValue(HelpTextProperty);
	}

	public static void SetIsColumnHeader(StyledElement element, bool value)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		element.SetValue(IsColumnHeaderProperty, value);
	}

	public static bool GetIsColumnHeader(StyledElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		return element.GetValue(IsColumnHeaderProperty);
	}

	public static void SetIsRequiredForForm(StyledElement element, bool value)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		element.SetValue(IsRequiredForFormProperty, value);
	}

	public static bool GetIsRequiredForForm(StyledElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		return element.GetValue(IsRequiredForFormProperty);
	}

	public static bool GetIsRowHeader(StyledElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		return element.GetValue(IsRowHeaderProperty);
	}

	public static void SetIsRowHeader(StyledElement element, bool value)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		element.SetValue(IsRowHeaderProperty, value);
	}

	public static void SetIsOffscreenBehavior(StyledElement element, IsOffscreenBehavior value)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		element.SetValue(IsOffscreenBehaviorProperty, value);
	}

	public static IsOffscreenBehavior GetIsOffscreenBehavior(StyledElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		return element.GetValue(IsOffscreenBehaviorProperty);
	}

	public static void SetItemStatus(StyledElement element, string? value)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		element.SetValue(ItemStatusProperty, value);
	}

	public static string? GetItemStatus(StyledElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		return element.GetValue(ItemStatusProperty);
	}

	public static void SetItemType(StyledElement element, string? value)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		element.SetValue(ItemTypeProperty, value);
	}

	public static string? GetItemType(StyledElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		return element.GetValue(ItemTypeProperty);
	}

	public static void SetLabeledBy(StyledElement element, Control value)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		element.SetValue(LabeledByProperty, value);
	}

	public static Control GetLabeledBy(StyledElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		return element.GetValue(LabeledByProperty);
	}

	public static void SetLiveSetting(StyledElement element, AutomationLiveSetting value)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		element.SetValue(LiveSettingProperty, value);
	}

	public static AutomationLiveSetting GetLiveSetting(StyledElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		return element.GetValue(LiveSettingProperty);
	}

	public static void SetName(StyledElement element, string? value)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		element.SetValue(NameProperty, value);
	}

	public static string? GetName(StyledElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		return element.GetValue(NameProperty);
	}

	public static void SetPositionInSet(StyledElement element, int value)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		element.SetValue(PositionInSetProperty, value);
	}

	public static int GetPositionInSet(StyledElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		return element.GetValue(PositionInSetProperty);
	}

	public static void SetSizeOfSet(StyledElement element, int value)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		element.SetValue(SizeOfSetProperty, value);
	}

	public static int GetSizeOfSet(StyledElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		return element.GetValue(SizeOfSetProperty);
	}
}
