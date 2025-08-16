using System;
using Avalonia.Data.Core;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaEdit;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Search;

namespace CompiledAvaloniaXaml;

internal class XamlIlHelpers
{
	private static IPropertyInfo Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Field;

	private static IPropertyInfo AvaloniaEdit_002ECodeCompletion_002EICompletionData_002CAvaloniaEdit_002EImage_0021Field;

	private static IPropertyInfo AvaloniaEdit_002ECodeCompletion_002EICompletionData_002CAvaloniaEdit_002EContent_0021Field;

	private static IPropertyInfo Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Field;

	private static IPropertyInfo AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECount_0021Field;

	private static IPropertyInfo AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentIndexText_0021Field;

	private static IPropertyInfo AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentHeader_0021Field;

	private static IPropertyInfo AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentContent_0021Field;

	private static IPropertyInfo AvaloniaEdit_002ESearch_002ESearchPanel_002CAvaloniaEdit_002ETextEditor_0021Field;

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

	static object AvaloniaEdit_002ECodeCompletion_002EICompletionData_002CAvaloniaEdit_002EImage_0021Getter(object P_0)
	{
		return ((ICompletionData)P_0).Image;
	}

	public static IPropertyInfo AvaloniaEdit_002ECodeCompletion_002EICompletionData_002CAvaloniaEdit_002EImage_0021Property()
	{
		if (AvaloniaEdit_002ECodeCompletion_002EICompletionData_002CAvaloniaEdit_002EImage_0021Field != null)
		{
			return AvaloniaEdit_002ECodeCompletion_002EICompletionData_002CAvaloniaEdit_002EImage_0021Field;
		}
		AvaloniaEdit_002ECodeCompletion_002EICompletionData_002CAvaloniaEdit_002EImage_0021Field = new ClrPropertyInfo("Image", (Func<object, object?>?)XamlIlHelpers.AvaloniaEdit_002ECodeCompletion_002EICompletionData_002CAvaloniaEdit_002EImage_0021Getter, (Action<object, object?>?)null, typeof(IImage));
		return AvaloniaEdit_002ECodeCompletion_002EICompletionData_002CAvaloniaEdit_002EImage_0021Field;
	}

	static object AvaloniaEdit_002ECodeCompletion_002EICompletionData_002CAvaloniaEdit_002EContent_0021Getter(object P_0)
	{
		return ((ICompletionData)P_0).Content;
	}

	public static IPropertyInfo AvaloniaEdit_002ECodeCompletion_002EICompletionData_002CAvaloniaEdit_002EContent_0021Property()
	{
		if (AvaloniaEdit_002ECodeCompletion_002EICompletionData_002CAvaloniaEdit_002EContent_0021Field != null)
		{
			return AvaloniaEdit_002ECodeCompletion_002EICompletionData_002CAvaloniaEdit_002EContent_0021Field;
		}
		AvaloniaEdit_002ECodeCompletion_002EICompletionData_002CAvaloniaEdit_002EContent_0021Field = new ClrPropertyInfo("Content", (Func<object, object?>?)XamlIlHelpers.AvaloniaEdit_002ECodeCompletion_002EICompletionData_002CAvaloniaEdit_002EContent_0021Getter, (Action<object, object?>?)null, typeof(object));
		return AvaloniaEdit_002ECodeCompletion_002EICompletionData_002CAvaloniaEdit_002EContent_0021Field;
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

	static object AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECount_0021Getter(object P_0)
	{
		return ((IOverloadProvider)P_0).Count;
	}

	public static IPropertyInfo AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECount_0021Property()
	{
		if (AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECount_0021Field != null)
		{
			return AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECount_0021Field;
		}
		AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECount_0021Field = new ClrPropertyInfo("Count", (Func<object, object?>?)XamlIlHelpers.AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECount_0021Getter, (Action<object, object?>?)null, typeof(int));
		return AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECount_0021Field;
	}

	static object AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentIndexText_0021Getter(object P_0)
	{
		return ((IOverloadProvider)P_0).CurrentIndexText;
	}

	public static IPropertyInfo AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentIndexText_0021Property()
	{
		if (AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentIndexText_0021Field != null)
		{
			return AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentIndexText_0021Field;
		}
		AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentIndexText_0021Field = new ClrPropertyInfo("CurrentIndexText", (Func<object, object?>?)XamlIlHelpers.AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentIndexText_0021Getter, (Action<object, object?>?)null, typeof(string));
		return AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentIndexText_0021Field;
	}

	static object AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentHeader_0021Getter(object P_0)
	{
		return ((IOverloadProvider)P_0).CurrentHeader;
	}

	public static IPropertyInfo AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentHeader_0021Property()
	{
		if (AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentHeader_0021Field != null)
		{
			return AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentHeader_0021Field;
		}
		AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentHeader_0021Field = new ClrPropertyInfo("CurrentHeader", (Func<object, object?>?)XamlIlHelpers.AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentHeader_0021Getter, (Action<object, object?>?)null, typeof(object));
		return AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentHeader_0021Field;
	}

	static object AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentContent_0021Getter(object P_0)
	{
		return ((IOverloadProvider)P_0).CurrentContent;
	}

	public static IPropertyInfo AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentContent_0021Property()
	{
		if (AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentContent_0021Field != null)
		{
			return AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentContent_0021Field;
		}
		AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentContent_0021Field = new ClrPropertyInfo("CurrentContent", (Func<object, object?>?)XamlIlHelpers.AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentContent_0021Getter, (Action<object, object?>?)null, typeof(object));
		return AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentContent_0021Field;
	}

	static object AvaloniaEdit_002ESearch_002ESearchPanel_002CAvaloniaEdit_002ETextEditor_0021Getter(object P_0)
	{
		return ((SearchPanel)P_0).TextEditor;
	}

	public static IPropertyInfo AvaloniaEdit_002ESearch_002ESearchPanel_002CAvaloniaEdit_002ETextEditor_0021Property()
	{
		if (AvaloniaEdit_002ESearch_002ESearchPanel_002CAvaloniaEdit_002ETextEditor_0021Field != null)
		{
			return AvaloniaEdit_002ESearch_002ESearchPanel_002CAvaloniaEdit_002ETextEditor_0021Field;
		}
		AvaloniaEdit_002ESearch_002ESearchPanel_002CAvaloniaEdit_002ETextEditor_0021Field = new ClrPropertyInfo("TextEditor", (Func<object, object?>?)XamlIlHelpers.AvaloniaEdit_002ESearch_002ESearchPanel_002CAvaloniaEdit_002ETextEditor_0021Getter, (Action<object, object?>?)null, typeof(TextEditor));
		return AvaloniaEdit_002ESearch_002ESearchPanel_002CAvaloniaEdit_002ETextEditor_0021Field;
	}
}
