using System;
using Avalonia.Collections;
using Avalonia.Data.Core;
using Avalonia.Styling;

namespace CompiledAvaloniaXaml;

internal class XamlIlHelpers
{
	private static IPropertyInfo Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Field;

	private static IPropertyInfo Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Field;

	private static IPropertyInfo Avalonia_002ECollections_002EDataGridCollectionViewGroup_002CAvalonia_002EControls_002EDataGrid_002EKey_0021Field;

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
		Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Field = new ClrPropertyInfo("BasedOn", (Func<object, object?>?)XamlIlHelpers.Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Getter, (Action<object, object?>?)XamlIlHelpers.Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Setter, typeof(ControlTheme));
		return Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Field;
	}

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
		Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Field = new ClrPropertyInfo("Value", (Func<object, object?>?)XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Getter, (Action<object, object?>?)XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Setter, typeof(object));
		return Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Field;
	}

	static object Avalonia_002ECollections_002EDataGridCollectionViewGroup_002CAvalonia_002EControls_002EDataGrid_002EKey_0021Getter(object P_0)
	{
		return ((DataGridCollectionViewGroup)P_0).Key;
	}

	public static IPropertyInfo Avalonia_002ECollections_002EDataGridCollectionViewGroup_002CAvalonia_002EControls_002EDataGrid_002EKey_0021Property()
	{
		if (Avalonia_002ECollections_002EDataGridCollectionViewGroup_002CAvalonia_002EControls_002EDataGrid_002EKey_0021Field != null)
		{
			return Avalonia_002ECollections_002EDataGridCollectionViewGroup_002CAvalonia_002EControls_002EDataGrid_002EKey_0021Field;
		}
		Avalonia_002ECollections_002EDataGridCollectionViewGroup_002CAvalonia_002EControls_002EDataGrid_002EKey_0021Field = new ClrPropertyInfo("Key", (Func<object, object?>?)XamlIlHelpers.Avalonia_002ECollections_002EDataGridCollectionViewGroup_002CAvalonia_002EControls_002EDataGrid_002EKey_0021Getter, (Action<object, object?>?)null, typeof(object));
		return Avalonia_002ECollections_002EDataGridCollectionViewGroup_002CAvalonia_002EControls_002EDataGrid_002EKey_0021Field;
	}
}
