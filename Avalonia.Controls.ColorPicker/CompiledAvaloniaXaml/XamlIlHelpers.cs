using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Data.Core;
using Avalonia.Styling;

namespace CompiledAvaloniaXaml;

internal class XamlIlHelpers
{
	private static IPropertyInfo Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Field;

	private static IPropertyInfo Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Field;

	private static IPropertyInfo Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Field;

	private static IPropertyInfo Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Field;

	private static IPropertyInfo Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Field;

	private static IPropertyInfo Avalonia_002ERect_002CAvalonia_002EBase_002EWidth_0021Field;

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
		Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Field = new ClrPropertyInfo("Converter", (Func<object, object?>?)XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Getter, (Action<object, object?>?)XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Setter, typeof(IValueConverter));
		return Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Field;
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
		Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Field = new ClrPropertyInfo("Bindings", (Func<object, object?>?)XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Getter, (Action<object, object?>?)XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Setter, typeof(IList<IBinding>));
		return Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Field;
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
		Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Field = new ClrPropertyInfo("Converter", (Func<object, object?>?)XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Getter, (Action<object, object?>?)XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Setter, typeof(IValueConverter));
		return Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Field;
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
		Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Field = new ClrPropertyInfo("Height", (Func<object, object?>?)XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Getter, (Action<object, object?>?)null, typeof(double));
		return Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Field;
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
		Avalonia_002ERect_002CAvalonia_002EBase_002EWidth_0021Field = new ClrPropertyInfo("Width", (Func<object, object?>?)XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EWidth_0021Getter, (Action<object, object?>?)null, typeof(double));
		return Avalonia_002ERect_002CAvalonia_002EBase_002EWidth_0021Field;
	}
}
