using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Avalonia.Media;
using Avalonia.Styling;
using CompiledAvaloniaXaml;

namespace Avalonia.Themes.Simple;

public class SimpleTheme : Styles
{
	private class XamlClosure_271
	{
		private class DynamicSetters_272
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeBackgroundColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_272.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_273
	{
		private class DynamicSetters_274
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeBorderLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_274.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_275
	{
		private class DynamicSetters_276
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeBorderMidColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_276.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_277
	{
		private class DynamicSetters_278
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeBorderHighColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_278.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_279
	{
		private class DynamicSetters_280
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeControlLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_280.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_281
	{
		private class DynamicSetters_282
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeControlMidColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_282.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_283
	{
		private class DynamicSetters_284
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeControlMidHighColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_284.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_285
	{
		private class DynamicSetters_286
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeControlHighColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_286.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_287
	{
		private class DynamicSetters_288
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeControlVeryHighColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_288.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_289
	{
		private class DynamicSetters_290
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeControlHighlightLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_290.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_291
	{
		private class DynamicSetters_292
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeControlHighlightMidColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_292.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_293
	{
		private class DynamicSetters_294
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeControlHighlightHighColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_294.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_295
	{
		private class DynamicSetters_296
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeForegroundColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_296.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_297
	{
		private class DynamicSetters_298
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("HighlightColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_298.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_299
	{
		private class DynamicSetters_300
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("HighlightColor2");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_300.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_301
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			return new SolidColorBrush
			{
				Color = Color.FromUInt32(4278190080u)
			};
		}
	}

	private class XamlClosure_302
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			return new SolidColorBrush
			{
				Color = Color.FromUInt32(16777215u)
			};
		}
	}

	private class XamlClosure_303
	{
		private class DynamicSetters_304
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeBackgroundColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_304.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_305
	{
		private class DynamicSetters_306
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeBorderLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_306.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_307
	{
		private class DynamicSetters_308
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeBorderMidColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_308.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_309
	{
		private class DynamicSetters_310
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeBorderHighColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_310.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_311
	{
		private class DynamicSetters_312
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeControlLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_312.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_313
	{
		private class DynamicSetters_314
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeControlMidColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_314.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_315
	{
		private class DynamicSetters_316
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeControlMidHighColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_316.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_317
	{
		private class DynamicSetters_318
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeControlHighColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_318.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_319
	{
		private class DynamicSetters_320
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeControlVeryHighColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_320.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_321
	{
		private class DynamicSetters_322
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeControlHighlightLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_322.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_323
	{
		private class DynamicSetters_324
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeControlHighlightMidColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_324.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_325
	{
		private class DynamicSetters_326
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeControlHighlightHighColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_326.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_327
	{
		private class DynamicSetters_328
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeForegroundColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_328.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_329
	{
		private class DynamicSetters_330
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("HighlightColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_330.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_331
	{
		private class DynamicSetters_332
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("HighlightColor2");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_332.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_333
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			return new SolidColorBrush
			{
				Color = Color.FromUInt32(uint.MaxValue)
			};
		}
	}

	private class XamlClosure_334
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			return new SolidColorBrush
			{
				Color = Color.FromUInt32(16777215u)
			};
		}
	}

	private class XamlClosure_335
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			return new FontFamily(((IUriContext)context).BaseUri, "fonts:Inter#Inter, $Default");
		}
	}

	private class XamlClosure_336
	{
		private class DynamicSetters_337
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("HighlightForegroundColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_337.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_338
	{
		private class DynamicSetters_339
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeForegroundLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_339.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_340
	{
		private class DynamicSetters_341
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeAccentColor2");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_341.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_342
	{
		private class DynamicSetters_343
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeAccentColor3");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_343.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_344
	{
		private class DynamicSetters_345
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeAccentColor4");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_345.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_346
	{
		private class DynamicSetters_347
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeAccentColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_347.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_348
	{
		private class DynamicSetters_349
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ErrorColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_349.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_350
	{
		private class DynamicSetters_351
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ErrorLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_351.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_352
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush = new SolidColorBrush();
			solidColorBrush.Opacity = 0.75;
			solidColorBrush.Color = Color.FromUInt32(4282664004u);
			return solidColorBrush;
		}
	}

	private class XamlClosure_353
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush = new SolidColorBrush();
			solidColorBrush.Opacity = 0.75;
			solidColorBrush.Color = Color.FromUInt32(4278221516u);
			return solidColorBrush;
		}
	}

	private class XamlClosure_354
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush = new SolidColorBrush();
			solidColorBrush.Opacity = 0.75;
			solidColorBrush.Color = Color.FromUInt32(4280262213u);
			return solidColorBrush;
		}
	}

	private class XamlClosure_355
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush = new SolidColorBrush();
			solidColorBrush.Opacity = 0.75;
			solidColorBrush.Color = Color.FromUInt32(4294816552u);
			return solidColorBrush;
		}
	}

	private class XamlClosure_356
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush = new SolidColorBrush();
			solidColorBrush.Opacity = 0.75;
			solidColorBrush.Color = Color.FromUInt32(4290584620u);
			return solidColorBrush;
		}
	}

	private class XamlClosure_357
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			return new SolidColorBrush
			{
				Color = Color.FromUInt32(16777215u)
			};
		}
	}

	private class XamlClosure_358
	{
		private class DynamicSetters_359
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			solidColorBrush.Opacity = 0.4;
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeAccentColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_359.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_360
	{
		private class DynamicSetters_361
		{
			public static void DynamicSetter_1(SolidColorBrush P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(SolidColorBrush.ColorProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(SolidColorBrush.ColorProperty, binding);
					return;
				}
				if (P_1 is Color)
				{
					P_0.Color = (Color)P_1;
					return;
				}
				if (P_1 == null)
				{
					throw new NullReferenceException();
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (SimpleTheme)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			solidColorBrush.Opacity = 0.4;
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThemeAccentColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_361.DynamicSetter_1(solidColorBrush, obj);
			context.PopParent();
			return result;
		}
	}

	private static Action<object> _0021XamlIlPopulateOverride;

	public SimpleTheme(IServiceProvider? sp = null)
	{
		_0021XamlIlPopulateTrampoline(sp, this);
	}

	static void _0021XamlIlPopulate(IServiceProvider P_0, SimpleTheme P_1)
	{
		CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme> context = new CompiledAvaloniaXaml.XamlIlContext.Context<SimpleTheme>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FSimpleTheme_002Examl.Singleton }, "avares://Avalonia.Themes.Simple/SimpleTheme.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		SimpleTheme simpleTheme;
		SimpleTheme simpleTheme2 = (simpleTheme = P_1);
		context.PushParent(simpleTheme);
		ResourceDictionary resourceDictionary;
		ResourceDictionary resources = (resourceDictionary = new ResourceDictionary());
		context.PushParent(resourceDictionary);
		ResourceDictionary resourceDictionary2 = resourceDictionary;
		IDictionary<ThemeVariant, IThemeVariantProvider> themeDictionaries = resourceDictionary2.ThemeDictionaries;
		ThemeVariant @default = ThemeVariant.Default;
		ResourceDictionary value = (resourceDictionary = new ResourceDictionary());
		context.PushParent(resourceDictionary);
		ResourceDictionary resourceDictionary3 = resourceDictionary;
		((IThemeVariantProvider)resourceDictionary3).Key = ThemeVariant.Default;
		resourceDictionary3.Add("ThemeBackgroundColor", Color.FromUInt32(uint.MaxValue));
		resourceDictionary3.Add("ThemeBorderLowColor", Color.FromUInt32(4289374890u));
		resourceDictionary3.Add("ThemeBorderMidColor", Color.FromUInt32(4287137928u));
		resourceDictionary3.Add("ThemeBorderHighColor", Color.FromUInt32(4281545523u));
		resourceDictionary3.Add("ThemeControlLowColor", Color.FromUInt32(4287007129u));
		resourceDictionary3.Add("ThemeControlMidColor", Color.FromUInt32(4294309365u));
		resourceDictionary3.Add("ThemeControlMidHighColor", Color.FromUInt32(4290954185u));
		resourceDictionary3.Add("ThemeControlHighColor", Color.FromUInt32(4285032552u));
		resourceDictionary3.Add("ThemeControlVeryHighColor", Color.FromUInt32(4284177243u));
		resourceDictionary3.Add("ThemeControlHighlightLowColor", Color.FromUInt32(4293980400u));
		resourceDictionary3.Add("ThemeControlHighlightMidColor", Color.FromUInt32(4291875024u));
		resourceDictionary3.Add("ThemeControlHighlightHighColor", Color.FromUInt32(4286611584u));
		resourceDictionary3.Add("ThemeForegroundColor", Color.FromUInt32(4278190080u));
		resourceDictionary3.Add("HighlightColor", Color.FromUInt32(4278742942u));
		resourceDictionary3.Add("HighlightColor2", Color.FromUInt32(4278804613u));
		resourceDictionary3.AddDeferred("ThemeBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_271.Build, context));
		resourceDictionary3.AddDeferred("ThemeBorderLowBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_273.Build, context));
		resourceDictionary3.AddDeferred("ThemeBorderMidBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_275.Build, context));
		resourceDictionary3.AddDeferred("ThemeBorderHighBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_277.Build, context));
		resourceDictionary3.AddDeferred("ThemeControlLowBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_279.Build, context));
		resourceDictionary3.AddDeferred("ThemeControlMidBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_281.Build, context));
		resourceDictionary3.AddDeferred("ThemeControlMidHighBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_283.Build, context));
		resourceDictionary3.AddDeferred("ThemeControlHighBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_285.Build, context));
		resourceDictionary3.AddDeferred("ThemeControlVeryHighBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_287.Build, context));
		resourceDictionary3.AddDeferred("ThemeControlHighlightLowBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_289.Build, context));
		resourceDictionary3.AddDeferred("ThemeControlHighlightMidBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_291.Build, context));
		resourceDictionary3.AddDeferred("ThemeControlHighlightHighBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_293.Build, context));
		resourceDictionary3.AddDeferred("ThemeForegroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_295.Build, context));
		resourceDictionary3.AddDeferred("HighlightBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_297.Build, context));
		resourceDictionary3.AddDeferred("HighlightBrush2", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_299.Build, context));
		resourceDictionary3.AddDeferred("RefreshVisualizerForeground", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_301.Build, context));
		resourceDictionary3.AddDeferred("RefreshVisualizerBackground", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_302.Build, context));
		context.PopParent();
		themeDictionaries.Add(@default, value);
		IDictionary<ThemeVariant, IThemeVariantProvider> themeDictionaries2 = resourceDictionary2.ThemeDictionaries;
		ThemeVariant dark = ThemeVariant.Dark;
		ResourceDictionary value2 = (resourceDictionary = new ResourceDictionary());
		context.PushParent(resourceDictionary);
		ResourceDictionary resourceDictionary4 = resourceDictionary;
		((IThemeVariantProvider)resourceDictionary4).Key = ThemeVariant.Dark;
		resourceDictionary4.Add("ThemeBackgroundColor", Color.FromUInt32(4280821800u));
		resourceDictionary4.Add("ThemeBorderLowColor", Color.FromUInt32(4283453520u));
		resourceDictionary4.Add("ThemeBorderMidColor", Color.FromUInt32(4286611584u));
		resourceDictionary4.Add("ThemeBorderHighColor", Color.FromUInt32(4288716960u));
		resourceDictionary4.Add("ThemeControlLowColor", Color.FromUInt32(4280821800u));
		resourceDictionary4.Add("ThemeControlMidColor", Color.FromUInt32(4283453520u));
		resourceDictionary4.Add("ThemeControlMidHighColor", Color.FromUInt32(4285032552u));
		resourceDictionary4.Add("ThemeControlHighColor", Color.FromUInt32(4286611584u));
		resourceDictionary4.Add("ThemeControlVeryHighColor", Color.FromUInt32(4293913583u));
		resourceDictionary4.Add("ThemeControlHighlightLowColor", Color.FromUInt32(4289243304u));
		resourceDictionary4.Add("ThemeControlHighlightMidColor", Color.FromUInt32(4286743170u));
		resourceDictionary4.Add("ThemeControlHighlightHighColor", Color.FromUInt32(4283453520u));
		resourceDictionary4.Add("ThemeForegroundColor", Color.FromUInt32(4292796126u));
		resourceDictionary4.Add("HighlightColor", Color.FromUInt32(4279344858u));
		resourceDictionary4.Add("HighlightColor2", Color.FromUInt32(4278804613u));
		resourceDictionary4.AddDeferred("ThemeBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_303.Build, context));
		resourceDictionary4.AddDeferred("ThemeBorderLowBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_305.Build, context));
		resourceDictionary4.AddDeferred("ThemeBorderMidBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_307.Build, context));
		resourceDictionary4.AddDeferred("ThemeBorderHighBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_309.Build, context));
		resourceDictionary4.AddDeferred("ThemeControlLowBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_311.Build, context));
		resourceDictionary4.AddDeferred("ThemeControlMidBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_313.Build, context));
		resourceDictionary4.AddDeferred("ThemeControlMidHighBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_315.Build, context));
		resourceDictionary4.AddDeferred("ThemeControlHighBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_317.Build, context));
		resourceDictionary4.AddDeferred("ThemeControlVeryHighBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_319.Build, context));
		resourceDictionary4.AddDeferred("ThemeControlHighlightLowBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_321.Build, context));
		resourceDictionary4.AddDeferred("ThemeControlHighlightMidBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_323.Build, context));
		resourceDictionary4.AddDeferred("ThemeControlHighlightHighBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_325.Build, context));
		resourceDictionary4.AddDeferred("ThemeForegroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_327.Build, context));
		resourceDictionary4.AddDeferred("HighlightBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_329.Build, context));
		resourceDictionary4.AddDeferred("HighlightBrush2", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_331.Build, context));
		resourceDictionary4.AddDeferred("RefreshVisualizerForeground", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_333.Build, context));
		resourceDictionary4.AddDeferred("RefreshVisualizerBackground", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_334.Build, context));
		context.PopParent();
		themeDictionaries2.Add(dark, value2);
		resourceDictionary2.AddDeferred("ContentControlThemeFontFamily", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_335.Build, context));
		resourceDictionary2.Add("ThemeAccentColor", Color.FromUInt32(3423706842u));
		resourceDictionary2.Add("ThemeAccentColor2", Color.FromUInt32(2568068826u));
		resourceDictionary2.Add("ThemeAccentColor3", Color.FromUInt32(1712430810u));
		resourceDictionary2.Add("ThemeAccentColor4", Color.FromUInt32(856792794u));
		resourceDictionary2.Add("ThemeForegroundLowColor", Color.FromUInt32(4286611584u));
		resourceDictionary2.Add("HighlightForegroundColor", Color.FromUInt32(uint.MaxValue));
		resourceDictionary2.Add("ErrorColor", Color.FromUInt32(4294901760u));
		resourceDictionary2.Add("ErrorLowColor", Color.FromUInt32(285147136u));
		resourceDictionary2.AddDeferred("HighlightForegroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_336.Build, context));
		resourceDictionary2.AddDeferred("ThemeForegroundLowBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_338.Build, context));
		resourceDictionary2.AddDeferred("ThemeAccentBrush2", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_340.Build, context));
		resourceDictionary2.AddDeferred("ThemeAccentBrush3", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_342.Build, context));
		resourceDictionary2.AddDeferred("ThemeAccentBrush4", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_344.Build, context));
		resourceDictionary2.AddDeferred("ThemeAccentBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_346.Build, context));
		resourceDictionary2.AddDeferred("ErrorBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_348.Build, context));
		resourceDictionary2.AddDeferred("ErrorLowBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_350.Build, context));
		resourceDictionary2.AddDeferred("NotificationCardBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_352.Build, context));
		resourceDictionary2.AddDeferred("NotificationCardInformationBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_353.Build, context));
		resourceDictionary2.AddDeferred("NotificationCardSuccessBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_354.Build, context));
		resourceDictionary2.AddDeferred("NotificationCardWarningBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_355.Build, context));
		resourceDictionary2.AddDeferred("NotificationCardErrorBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_356.Build, context));
		resourceDictionary2.AddDeferred("ThemeControlTransparentBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_357.Build, context));
		resourceDictionary2.AddDeferred("DatePickerFlyoutPresenterHighlightFill", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_358.Build, context));
		resourceDictionary2.AddDeferred("TimePickerFlyoutPresenterHighlightFill", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_360.Build, context));
		resourceDictionary2.Add("ThemeBorderThickness", new Thickness(1.0, 1.0, 1.0, 1.0));
		resourceDictionary2.Add("ThemeDisabledOpacity", 0.5);
		resourceDictionary2.Add("FontSizeSmall", 10.0);
		resourceDictionary2.Add("FontSizeNormal", 12.0);
		resourceDictionary2.Add("FontSizeLarge", 16.0);
		resourceDictionary2.Add("ScrollBarThickness", 18.0);
		resourceDictionary2.Add("ScrollBarThumbThickness", 8.0);
		resourceDictionary2.Add("IconElementThemeHeight", 20.0);
		resourceDictionary2.Add("IconElementThemeWidth", 20.0);
		context.PopParent();
		simpleTheme.Resources = resources;
		simpleTheme.Add(_0021AvaloniaResources.Build_003A_002FControls_002FSimpleControls_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		context.PopParent();
		if (simpleTheme2 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	private static void _0021XamlIlPopulateTrampoline(IServiceProvider P_0, SimpleTheme P_1)
	{
		if (_0021XamlIlPopulateOverride != null)
		{
			_0021XamlIlPopulateOverride(P_1);
		}
		else
		{
			_0021XamlIlPopulate(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(P_0), P_1);
		}
	}
}
