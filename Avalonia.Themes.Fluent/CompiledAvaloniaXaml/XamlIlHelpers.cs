using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Notifications;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Data.Core;
using Avalonia.Dialogs.Internal;
using Avalonia.Styling;

namespace CompiledAvaloniaXaml;

internal class XamlIlHelpers
{
	private static IPropertyInfo Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Field;

	private static IPropertyInfo Avalonia_002ERect_002CAvalonia_002EBase_002EWidth_0021Field;

	private static IPropertyInfo Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Field;

	private static IPropertyInfo Avalonia_002EVector_002CAvalonia_002EBase_002EY_0021Field;

	private static IPropertyInfo Avalonia_002ESize_002CAvalonia_002EBase_002EHeight_0021Field;

	private static IPropertyInfo Avalonia_002EControls_002ENotifications_002EINotification_002CAvalonia_002EControls_002ETitle_0021Field;

	private static IPropertyInfo Avalonia_002EControls_002ENotifications_002EINotification_002CAvalonia_002EControls_002EMessage_0021Field;

	private static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EQuickLinks_0021Field;

	private static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EQuickLinksSelectedIndex_0021Field;

	private static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EIconKey_0021Field;

	private static IPropertyInfo Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Field;

	private static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EDisplayName_0021Field;

	private static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ELocation_0021Field;

	private static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EShowFilters_0021Field;

	private static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EFilters_0021Field;

	private static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectedFilter_0021Field;

	private static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EFileName_0021Field;

	private static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectingFolder_0021Field;

	private static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EShowHiddenFiles_0021Field;

	private static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EItems_0021Field;

	private static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectionMode_0021Field;

	private static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectedItems_0021Field;

	private static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EModified_0021Field;

	private static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EType_0021Field;

	private static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002ESize_0021Field;

	private static IPropertyInfo Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Field;

	private static IPropertyInfo Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Field;

	private static IPropertyInfo Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EConverter_0021Field;

	private static IPropertyInfo Avalonia_002EControls_002EProgressBar_002CAvalonia_002EControls_002ETemplateSettings_0021Field;

	private static IPropertyInfo System_002EDateTime_002CSystem_002ERuntime_002EDay_0021Field;

	private static IPropertyInfo Avalonia_002EControls_002ENativeMenu_002CAvalonia_002EControls_002EItems_0021Field;

	private static IPropertyInfo Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EDuration_0021Field;

	private static IPropertyInfo Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EEasing_0021Field;

	private static IPropertyInfo Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Field;

	private static IPropertyInfo Avalonia_002EControls_002EConverters_002EMarginMultiplierConverter_002CAvalonia_002EControls_002EIndent_0021Field;

	static object Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Getter(object P_0)
	{
		return ((Setter)P_0).Value;
	}

	static void Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Setter(object P_0, object P_1)
	{
		((Setter)P_0).Value = P_1;
	}

	public static IPropertyInfo Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property()
	{
		if (Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Field != null)
		{
			return Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Field;
		}
		Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Field = new ClrPropertyInfo("Value", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Getter, (Action<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Setter, typeof(object));
		return Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Field;
	}

	static object Avalonia_002ERect_002CAvalonia_002EBase_002EWidth_0021Getter(object P_0)
	{
		return ((Rect)P_0).Width;
	}

	public static IPropertyInfo Avalonia_002ERect_002CAvalonia_002EBase_002EWidth_0021Property()
	{
		if (Avalonia_002ERect_002CAvalonia_002EBase_002EWidth_0021Field != null)
		{
			return Avalonia_002ERect_002CAvalonia_002EBase_002EWidth_0021Field;
		}
		Avalonia_002ERect_002CAvalonia_002EBase_002EWidth_0021Field = new ClrPropertyInfo("Width", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EWidth_0021Getter, (Action<object, object?>?)null, typeof(double));
		return Avalonia_002ERect_002CAvalonia_002EBase_002EWidth_0021Field;
	}

	static object Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Getter(object P_0)
	{
		return ((MultiBinding)P_0).Bindings;
	}

	static void Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Setter(object P_0, object P_1)
	{
		((MultiBinding)P_0).Bindings = (IList<IBinding>)P_1;
	}

	public static IPropertyInfo Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property()
	{
		if (Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Field != null)
		{
			return Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Field;
		}
		Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Field = new ClrPropertyInfo("Bindings", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Getter, (Action<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Setter, typeof(IList<IBinding>));
		return Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Field;
	}

	static object Avalonia_002EVector_002CAvalonia_002EBase_002EY_0021Getter(object P_0)
	{
		return ((Vector)P_0).Y;
	}

	public static IPropertyInfo Avalonia_002EVector_002CAvalonia_002EBase_002EY_0021Property()
	{
		if (Avalonia_002EVector_002CAvalonia_002EBase_002EY_0021Field != null)
		{
			return Avalonia_002EVector_002CAvalonia_002EBase_002EY_0021Field;
		}
		Avalonia_002EVector_002CAvalonia_002EBase_002EY_0021Field = new ClrPropertyInfo("Y", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EVector_002CAvalonia_002EBase_002EY_0021Getter, (Action<object, object?>?)null, typeof(double));
		return Avalonia_002EVector_002CAvalonia_002EBase_002EY_0021Field;
	}

	static object Avalonia_002ESize_002CAvalonia_002EBase_002EHeight_0021Getter(object P_0)
	{
		return ((Size)P_0).Height;
	}

	public static IPropertyInfo Avalonia_002ESize_002CAvalonia_002EBase_002EHeight_0021Property()
	{
		if (Avalonia_002ESize_002CAvalonia_002EBase_002EHeight_0021Field != null)
		{
			return Avalonia_002ESize_002CAvalonia_002EBase_002EHeight_0021Field;
		}
		Avalonia_002ESize_002CAvalonia_002EBase_002EHeight_0021Field = new ClrPropertyInfo("Height", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002ESize_002CAvalonia_002EBase_002EHeight_0021Getter, (Action<object, object?>?)null, typeof(double));
		return Avalonia_002ESize_002CAvalonia_002EBase_002EHeight_0021Field;
	}

	static object Avalonia_002EControls_002ENotifications_002EINotification_002CAvalonia_002EControls_002ETitle_0021Getter(object P_0)
	{
		return ((INotification)P_0).Title;
	}

	public static IPropertyInfo Avalonia_002EControls_002ENotifications_002EINotification_002CAvalonia_002EControls_002ETitle_0021Property()
	{
		if (Avalonia_002EControls_002ENotifications_002EINotification_002CAvalonia_002EControls_002ETitle_0021Field != null)
		{
			return Avalonia_002EControls_002ENotifications_002EINotification_002CAvalonia_002EControls_002ETitle_0021Field;
		}
		Avalonia_002EControls_002ENotifications_002EINotification_002CAvalonia_002EControls_002ETitle_0021Field = new ClrPropertyInfo("Title", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EControls_002ENotifications_002EINotification_002CAvalonia_002EControls_002ETitle_0021Getter, (Action<object, object?>?)null, typeof(string));
		return Avalonia_002EControls_002ENotifications_002EINotification_002CAvalonia_002EControls_002ETitle_0021Field;
	}

	static object Avalonia_002EControls_002ENotifications_002EINotification_002CAvalonia_002EControls_002EMessage_0021Getter(object P_0)
	{
		return ((INotification)P_0).Message;
	}

	public static IPropertyInfo Avalonia_002EControls_002ENotifications_002EINotification_002CAvalonia_002EControls_002EMessage_0021Property()
	{
		if (Avalonia_002EControls_002ENotifications_002EINotification_002CAvalonia_002EControls_002EMessage_0021Field != null)
		{
			return Avalonia_002EControls_002ENotifications_002EINotification_002CAvalonia_002EControls_002EMessage_0021Field;
		}
		Avalonia_002EControls_002ENotifications_002EINotification_002CAvalonia_002EControls_002EMessage_0021Field = new ClrPropertyInfo("Message", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EControls_002ENotifications_002EINotification_002CAvalonia_002EControls_002EMessage_0021Getter, (Action<object, object?>?)null, typeof(string));
		return Avalonia_002EControls_002ENotifications_002EINotification_002CAvalonia_002EControls_002EMessage_0021Field;
	}

	static object Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EQuickLinks_0021Getter(object P_0)
	{
		return ((ManagedFileChooserViewModel)P_0).QuickLinks;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EQuickLinks_0021Property()
	{
		if (Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EQuickLinks_0021Field != null)
		{
			return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EQuickLinks_0021Field;
		}
		Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EQuickLinks_0021Field = new ClrPropertyInfo("QuickLinks", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EQuickLinks_0021Getter, (Action<object, object?>?)null, typeof(AvaloniaList<ManagedFileChooserItemViewModel>));
		return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EQuickLinks_0021Field;
	}

	static object Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EQuickLinksSelectedIndex_0021Getter(object P_0)
	{
		return ((ManagedFileChooserViewModel)P_0).QuickLinksSelectedIndex;
	}

	static void Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EQuickLinksSelectedIndex_0021Setter(object P_0, object P_1)
	{
		((ManagedFileChooserViewModel)P_0).QuickLinksSelectedIndex = (int)P_1;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EQuickLinksSelectedIndex_0021Property()
	{
		if (Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EQuickLinksSelectedIndex_0021Field != null)
		{
			return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EQuickLinksSelectedIndex_0021Field;
		}
		Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EQuickLinksSelectedIndex_0021Field = new ClrPropertyInfo("QuickLinksSelectedIndex", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EQuickLinksSelectedIndex_0021Getter, (Action<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EQuickLinksSelectedIndex_0021Setter, typeof(int));
		return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EQuickLinksSelectedIndex_0021Field;
	}

	static object Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EIconKey_0021Getter(object P_0)
	{
		return ((ManagedFileChooserItemViewModel)P_0).IconKey;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EIconKey_0021Property()
	{
		if (Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EIconKey_0021Field != null)
		{
			return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EIconKey_0021Field;
		}
		Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EIconKey_0021Field = new ClrPropertyInfo("IconKey", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EIconKey_0021Getter, (Action<object, object?>?)null, typeof(string));
		return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EIconKey_0021Field;
	}

	static object Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Getter(object P_0)
	{
		return ((BindingBase)P_0).Converter;
	}

	static void Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Setter(object P_0, object P_1)
	{
		((BindingBase)P_0).Converter = (IValueConverter)P_1;
	}

	public static IPropertyInfo Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property()
	{
		if (Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Field != null)
		{
			return Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Field;
		}
		Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Field = new ClrPropertyInfo("Converter", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Getter, (Action<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Setter, typeof(IValueConverter));
		return Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Field;
	}

	static object Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EDisplayName_0021Getter(object P_0)
	{
		return ((ManagedFileChooserItemViewModel)P_0).DisplayName;
	}

	static void Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EDisplayName_0021Setter(object P_0, object P_1)
	{
		((ManagedFileChooserItemViewModel)P_0).DisplayName = (string)P_1;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EDisplayName_0021Property()
	{
		if (Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EDisplayName_0021Field != null)
		{
			return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EDisplayName_0021Field;
		}
		Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EDisplayName_0021Field = new ClrPropertyInfo("DisplayName", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EDisplayName_0021Getter, (Action<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EDisplayName_0021Setter, typeof(string));
		return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EDisplayName_0021Field;
	}

	static object Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ELocation_0021Getter(object P_0)
	{
		return ((ManagedFileChooserViewModel)P_0).Location;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ELocation_0021Property()
	{
		if (Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ELocation_0021Field != null)
		{
			return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ELocation_0021Field;
		}
		Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ELocation_0021Field = new ClrPropertyInfo("Location", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ELocation_0021Getter, (Action<object, object?>?)null, typeof(string));
		return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ELocation_0021Field;
	}

	static object Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EShowFilters_0021Getter(object P_0)
	{
		return ((ManagedFileChooserViewModel)P_0).ShowFilters;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EShowFilters_0021Property()
	{
		if (Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EShowFilters_0021Field != null)
		{
			return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EShowFilters_0021Field;
		}
		Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EShowFilters_0021Field = new ClrPropertyInfo("ShowFilters", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EShowFilters_0021Getter, (Action<object, object?>?)null, typeof(bool));
		return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EShowFilters_0021Field;
	}

	static object Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EFilters_0021Getter(object P_0)
	{
		return ((ManagedFileChooserViewModel)P_0).Filters;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EFilters_0021Property()
	{
		if (Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EFilters_0021Field != null)
		{
			return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EFilters_0021Field;
		}
		Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EFilters_0021Field = new ClrPropertyInfo("Filters", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EFilters_0021Getter, (Action<object, object?>?)null, typeof(AvaloniaList<ManagedFileChooserFilterViewModel>));
		return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EFilters_0021Field;
	}

	static object Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectedFilter_0021Getter(object P_0)
	{
		return ((ManagedFileChooserViewModel)P_0).SelectedFilter;
	}

	static void Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectedFilter_0021Setter(object P_0, object P_1)
	{
		((ManagedFileChooserViewModel)P_0).SelectedFilter = (ManagedFileChooserFilterViewModel)P_1;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectedFilter_0021Property()
	{
		if (Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectedFilter_0021Field != null)
		{
			return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectedFilter_0021Field;
		}
		Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectedFilter_0021Field = new ClrPropertyInfo("SelectedFilter", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectedFilter_0021Getter, (Action<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectedFilter_0021Setter, typeof(ManagedFileChooserFilterViewModel));
		return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectedFilter_0021Field;
	}

	static object Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EFileName_0021Getter(object P_0)
	{
		return ((ManagedFileChooserViewModel)P_0).FileName;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EFileName_0021Property()
	{
		if (Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EFileName_0021Field != null)
		{
			return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EFileName_0021Field;
		}
		Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EFileName_0021Field = new ClrPropertyInfo("FileName", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EFileName_0021Getter, (Action<object, object?>?)null, typeof(string));
		return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EFileName_0021Field;
	}

	static object Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectingFolder_0021Getter(object P_0)
	{
		return ((ManagedFileChooserViewModel)P_0).SelectingFolder;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectingFolder_0021Property()
	{
		if (Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectingFolder_0021Field != null)
		{
			return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectingFolder_0021Field;
		}
		Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectingFolder_0021Field = new ClrPropertyInfo("SelectingFolder", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectingFolder_0021Getter, (Action<object, object?>?)null, typeof(bool));
		return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectingFolder_0021Field;
	}

	static object Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EShowHiddenFiles_0021Getter(object P_0)
	{
		return ((ManagedFileChooserViewModel)P_0).ShowHiddenFiles;
	}

	static void Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EShowHiddenFiles_0021Setter(object P_0, object P_1)
	{
		((ManagedFileChooserViewModel)P_0).ShowHiddenFiles = (bool)P_1;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EShowHiddenFiles_0021Property()
	{
		if (Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EShowHiddenFiles_0021Field != null)
		{
			return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EShowHiddenFiles_0021Field;
		}
		Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EShowHiddenFiles_0021Field = new ClrPropertyInfo("ShowHiddenFiles", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EShowHiddenFiles_0021Getter, (Action<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EShowHiddenFiles_0021Setter, typeof(bool));
		return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EShowHiddenFiles_0021Field;
	}

	static object Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EItems_0021Getter(object P_0)
	{
		return ((ManagedFileChooserViewModel)P_0).Items;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EItems_0021Property()
	{
		if (Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EItems_0021Field != null)
		{
			return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EItems_0021Field;
		}
		Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EItems_0021Field = new ClrPropertyInfo("Items", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EItems_0021Getter, (Action<object, object?>?)null, typeof(AvaloniaList<ManagedFileChooserItemViewModel>));
		return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002EItems_0021Field;
	}

	static object Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectionMode_0021Getter(object P_0)
	{
		return ((ManagedFileChooserViewModel)P_0).SelectionMode;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectionMode_0021Property()
	{
		if (Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectionMode_0021Field != null)
		{
			return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectionMode_0021Field;
		}
		Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectionMode_0021Field = new ClrPropertyInfo("SelectionMode", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectionMode_0021Getter, (Action<object, object?>?)null, typeof(SelectionMode));
		return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectionMode_0021Field;
	}

	static object Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectedItems_0021Getter(object P_0)
	{
		return ((ManagedFileChooserViewModel)P_0).SelectedItems;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectedItems_0021Property()
	{
		if (Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectedItems_0021Field != null)
		{
			return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectedItems_0021Field;
		}
		Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectedItems_0021Field = new ClrPropertyInfo("SelectedItems", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectedItems_0021Getter, (Action<object, object?>?)null, typeof(AvaloniaList<ManagedFileChooserItemViewModel>));
		return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002CAvalonia_002EDialogs_002ESelectedItems_0021Field;
	}

	static object Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EModified_0021Getter(object P_0)
	{
		return ((ManagedFileChooserItemViewModel)P_0).Modified;
	}

	static void Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EModified_0021Setter(object P_0, object P_1)
	{
		((ManagedFileChooserItemViewModel)P_0).Modified = (DateTime)P_1;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EModified_0021Property()
	{
		if (Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EModified_0021Field != null)
		{
			return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EModified_0021Field;
		}
		Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EModified_0021Field = new ClrPropertyInfo("Modified", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EModified_0021Getter, (Action<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EModified_0021Setter, typeof(DateTime));
		return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EModified_0021Field;
	}

	static object Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EType_0021Getter(object P_0)
	{
		return ((ManagedFileChooserItemViewModel)P_0).Type;
	}

	static void Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EType_0021Setter(object P_0, object P_1)
	{
		((ManagedFileChooserItemViewModel)P_0).Type = (string)P_1;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EType_0021Property()
	{
		if (Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EType_0021Field != null)
		{
			return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EType_0021Field;
		}
		Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EType_0021Field = new ClrPropertyInfo("Type", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EType_0021Getter, (Action<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EType_0021Setter, typeof(string));
		return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002EType_0021Field;
	}

	static object Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002ESize_0021Getter(object P_0)
	{
		return ((ManagedFileChooserItemViewModel)P_0).Size;
	}

	static void Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002ESize_0021Setter(object P_0, object P_1)
	{
		((ManagedFileChooserItemViewModel)P_0).Size = (long)P_1;
	}

	public static IPropertyInfo Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002ESize_0021Property()
	{
		if (Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002ESize_0021Field != null)
		{
			return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002ESize_0021Field;
		}
		Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002ESize_0021Field = new ClrPropertyInfo("Size", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002ESize_0021Getter, (Action<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002ESize_0021Setter, typeof(long));
		return Avalonia_002EDialogs_002EInternal_002EManagedFileChooserItemViewModel_002CAvalonia_002EDialogs_002ESize_0021Field;
	}

	static object Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Getter(object P_0)
	{
		return ((ControlTheme)P_0).BasedOn;
	}

	static void Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Setter(object P_0, object P_1)
	{
		((ControlTheme)P_0).BasedOn = (ControlTheme)P_1;
	}

	public static IPropertyInfo Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Property()
	{
		if (Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Field != null)
		{
			return Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Field;
		}
		Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Field = new ClrPropertyInfo("BasedOn", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Getter, (Action<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Setter, typeof(ControlTheme));
		return Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Field;
	}

	static object Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Getter(object P_0)
	{
		return ((TemplateBinding)P_0).Converter;
	}

	static void Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Setter(object P_0, object P_1)
	{
		((TemplateBinding)P_0).Converter = (IValueConverter)P_1;
	}

	public static IPropertyInfo Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property()
	{
		if (Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Field != null)
		{
			return Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Field;
		}
		Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Field = new ClrPropertyInfo("Converter", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Getter, (Action<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Setter, typeof(IValueConverter));
		return Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Field;
	}

	static object Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EConverter_0021Getter(object P_0)
	{
		return ((MultiBinding)P_0).Converter;
	}

	static void Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EConverter_0021Setter(object P_0, object P_1)
	{
		((MultiBinding)P_0).Converter = (IMultiValueConverter)P_1;
	}

	public static IPropertyInfo Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EConverter_0021Property()
	{
		if (Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EConverter_0021Field != null)
		{
			return Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EConverter_0021Field;
		}
		Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EConverter_0021Field = new ClrPropertyInfo("Converter", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EConverter_0021Getter, (Action<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EConverter_0021Setter, typeof(IMultiValueConverter));
		return Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EConverter_0021Field;
	}

	static object Avalonia_002EControls_002EProgressBar_002CAvalonia_002EControls_002ETemplateSettings_0021Getter(object P_0)
	{
		return ((ProgressBar)P_0).TemplateSettings;
	}

	public static IPropertyInfo Avalonia_002EControls_002EProgressBar_002CAvalonia_002EControls_002ETemplateSettings_0021Property()
	{
		if (Avalonia_002EControls_002EProgressBar_002CAvalonia_002EControls_002ETemplateSettings_0021Field != null)
		{
			return Avalonia_002EControls_002EProgressBar_002CAvalonia_002EControls_002ETemplateSettings_0021Field;
		}
		Avalonia_002EControls_002EProgressBar_002CAvalonia_002EControls_002ETemplateSettings_0021Field = new ClrPropertyInfo("TemplateSettings", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EControls_002EProgressBar_002CAvalonia_002EControls_002ETemplateSettings_0021Getter, (Action<object, object?>?)null, typeof(ProgressBar.ProgressBarTemplateSettings));
		return Avalonia_002EControls_002EProgressBar_002CAvalonia_002EControls_002ETemplateSettings_0021Field;
	}

	static object System_002EDateTime_002CSystem_002ERuntime_002EDay_0021Getter(object P_0)
	{
		return ((DateTime)P_0).Day;
	}

	public static IPropertyInfo System_002EDateTime_002CSystem_002ERuntime_002EDay_0021Property()
	{
		if (System_002EDateTime_002CSystem_002ERuntime_002EDay_0021Field != null)
		{
			return System_002EDateTime_002CSystem_002ERuntime_002EDay_0021Field;
		}
		System_002EDateTime_002CSystem_002ERuntime_002EDay_0021Field = new ClrPropertyInfo("Day", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.System_002EDateTime_002CSystem_002ERuntime_002EDay_0021Getter, (Action<object, object?>?)null, typeof(int));
		return System_002EDateTime_002CSystem_002ERuntime_002EDay_0021Field;
	}

	static object Avalonia_002EControls_002ENativeMenu_002CAvalonia_002EControls_002EItems_0021Getter(object P_0)
	{
		return ((NativeMenu)P_0).Items;
	}

	public static IPropertyInfo Avalonia_002EControls_002ENativeMenu_002CAvalonia_002EControls_002EItems_0021Property()
	{
		if (Avalonia_002EControls_002ENativeMenu_002CAvalonia_002EControls_002EItems_0021Field != null)
		{
			return Avalonia_002EControls_002ENativeMenu_002CAvalonia_002EControls_002EItems_0021Field;
		}
		Avalonia_002EControls_002ENativeMenu_002CAvalonia_002EControls_002EItems_0021Field = new ClrPropertyInfo("Items", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EControls_002ENativeMenu_002CAvalonia_002EControls_002EItems_0021Getter, (Action<object, object?>?)null, typeof(IList<NativeMenuItemBase>));
		return Avalonia_002EControls_002ENativeMenu_002CAvalonia_002EControls_002EItems_0021Field;
	}

	static object Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EDuration_0021Getter(object P_0)
	{
		return ((Transition<double>)P_0).Duration;
	}

	static void Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EDuration_0021Setter(object P_0, object P_1)
	{
		((Transition<double>)P_0).Duration = (TimeSpan)P_1;
	}

	public static IPropertyInfo Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EDuration_0021Property()
	{
		if (Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EDuration_0021Field != null)
		{
			return Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EDuration_0021Field;
		}
		Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EDuration_0021Field = new ClrPropertyInfo("Duration", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EDuration_0021Getter, (Action<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EDuration_0021Setter, typeof(TimeSpan));
		return Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EDuration_0021Field;
	}

	static object Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EEasing_0021Getter(object P_0)
	{
		return ((Transition<double>)P_0).Easing;
	}

	static void Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EEasing_0021Setter(object P_0, object P_1)
	{
		((Transition<double>)P_0).Easing = (Easing)P_1;
	}

	public static IPropertyInfo Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EEasing_0021Property()
	{
		if (Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EEasing_0021Field != null)
		{
			return Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EEasing_0021Field;
		}
		Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EEasing_0021Field = new ClrPropertyInfo("Easing", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EEasing_0021Getter, (Action<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EEasing_0021Setter, typeof(Easing));
		return Avalonia_002EAnimation_002ETransition_00601_002CAvalonia_002EBase_002EEasing_0021Field;
	}

	static object Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Getter(object P_0)
	{
		return ((Rect)P_0).Height;
	}

	public static IPropertyInfo Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property()
	{
		if (Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Field != null)
		{
			return Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Field;
		}
		Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Field = new ClrPropertyInfo("Height", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Getter, (Action<object, object?>?)null, typeof(double));
		return Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Field;
	}

	static object Avalonia_002EControls_002EConverters_002EMarginMultiplierConverter_002CAvalonia_002EControls_002EIndent_0021Getter(object P_0)
	{
		return ((MarginMultiplierConverter)P_0).Indent;
	}

	static void Avalonia_002EControls_002EConverters_002EMarginMultiplierConverter_002CAvalonia_002EControls_002EIndent_0021Setter(object P_0, object P_1)
	{
		((MarginMultiplierConverter)P_0).Indent = (double)P_1;
	}

	public static IPropertyInfo Avalonia_002EControls_002EConverters_002EMarginMultiplierConverter_002CAvalonia_002EControls_002EIndent_0021Property()
	{
		if (Avalonia_002EControls_002EConverters_002EMarginMultiplierConverter_002CAvalonia_002EControls_002EIndent_0021Field != null)
		{
			return Avalonia_002EControls_002EConverters_002EMarginMultiplierConverter_002CAvalonia_002EControls_002EIndent_0021Field;
		}
		Avalonia_002EControls_002EConverters_002EMarginMultiplierConverter_002CAvalonia_002EControls_002EIndent_0021Field = new ClrPropertyInfo("Indent", (Func<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EControls_002EConverters_002EMarginMultiplierConverter_002CAvalonia_002EControls_002EIndent_0021Getter, (Action<object, object?>?)CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EControls_002EConverters_002EMarginMultiplierConverter_002CAvalonia_002EControls_002EIndent_0021Setter, typeof(double));
		return Avalonia_002EControls_002EConverters_002EMarginMultiplierConverter_002CAvalonia_002EControls_002EIndent_0021Field;
	}
}
