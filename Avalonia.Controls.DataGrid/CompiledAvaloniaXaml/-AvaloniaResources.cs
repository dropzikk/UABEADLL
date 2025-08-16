using System;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Styling;

namespace CompiledAvaloniaXaml;

[EditorBrowsable(EditorBrowsableState.Never)]
public class _0021AvaloniaResources
{
	public class NamespaceInfo_003A_002FThemes_002FFluent_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
	{
		private IReadOnlyDictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> _xmlNamespaces;

		public static IAvaloniaXamlIlXmlNamespaceInfoProvider Singleton;

		public virtual IReadOnlyDictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> XmlNamespaces
		{
			get
			{
				if (_xmlNamespaces == null)
				{
					_xmlNamespaces = CreateNamespaces();
				}
				return _xmlNamespaces;
			}
		}

		static AvaloniaXamlIlXmlNamespaceInfo CreateNamespaceInfo(string P_0, string P_1)
		{
			return new AvaloniaXamlIlXmlNamespaceInfo
			{
				ClrNamespace = P_0,
				ClrAssemblyName = P_1
			};
		}

		static IReadOnlyDictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> CreateNamespaces()
		{
			Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> dictionary = new Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>>(3);
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[34]
			{
				CreateNamespaceInfo("Avalonia", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Animation", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Animation.Easings", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Data", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Data.Converters", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Input", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Input.GestureRecognizers", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Input.TextInput", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Layout", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.LogicalTree", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Media", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Media.Imaging", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Media.Transformation", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Styling", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Automation", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Embedding", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Presenters", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Shapes", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Templates", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Notifications", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Chrome", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Documents", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Data", "Avalonia.Markup"),
				CreateNamespaceInfo("Avalonia.Markup.Data", "Avalonia.Markup"),
				CreateNamespaceInfo("Avalonia.Markup.Xaml.MarkupExtensions", "Avalonia.Markup.Xaml"),
				CreateNamespaceInfo("Avalonia.Markup.Xaml.Styling", "Avalonia.Markup.Xaml"),
				CreateNamespaceInfo("Avalonia.Markup.Xaml.Templates", "Avalonia.Markup.Xaml"),
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.DataGrid")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("collections", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Collections", (string)null) });
			return dictionary;
		}

		static NamespaceInfo_003A_002FThemes_002FFluent_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FThemes_002FFluent_002Examl();
		}
	}

	private class XamlClosure_1
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemBaseMediumColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_2
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemAltHighColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_3
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemListLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_4
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemListMediumColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_5
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemChromeMediumLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_6
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemChromeMediumColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_7
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemListMediumColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_8
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemBaseHighColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_9
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemListLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_10
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemListLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_11
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemErrorTextColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_12
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemBaseHighColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_13
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemAltMediumColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_14
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemErrorTextColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_15
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			solidColorBrush.Opacity = 0.4;
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemBaseMediumLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_16
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemChromeMediumLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_17
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemBaseMediumColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_18
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemAltHighColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_19
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemListLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_20
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemListMediumColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_21
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemChromeMediumLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_22
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemChromeMediumColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_23
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemListMediumColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_24
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemBaseHighColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_25
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemListLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_26
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemListLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_27
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemErrorTextColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_28
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemBaseHighColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_29
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemAltMediumColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_30
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemErrorTextColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_31
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			solidColorBrush.Opacity = 0.4;
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemBaseMediumLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_32
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemChromeMediumLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_33
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return StreamGeometry.Parse("M1875 1011l-787 787v-1798h-128v1798l-787 -787l-90 90l941 941l941 -941z");
		}
	}

	private class XamlClosure_34
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return StreamGeometry.Parse("M1965 947l-941 -941l-941 941l90 90l787 -787v1798h128v-1798l787 787z");
		}
	}

	private class XamlClosure_35
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return StreamGeometry.Parse("M515 93l930 931l-930 931l90 90l1022 -1021l-1022 -1021z");
		}
	}

	private class XamlClosure_36
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return StreamGeometry.Parse("M109 486 19 576 1024 1581 2029 576 1939 486 1024 1401z");
		}
	}

	private class XamlClosure_37
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "SystemControlTransparentBrush"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_38
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemAccentColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_39
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ListAccentLowOpacity"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_40
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemAccentColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_41
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ListAccentMediumOpacity"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_42
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemAccentColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_43
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ListAccentLowOpacity"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_44
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemAccentColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_45
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ListAccentMediumOpacity"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_46
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "SystemControlTransparentBrush"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_47
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "SystemControlTransparentBrush"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_48
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "SystemControlTransparentBrush"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_49
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme = new ControlTheme();
			controlTheme.TargetType = typeof(TextBlock);
			Setter setter = new Setter();
			setter.Property = Layoutable.MarginProperty;
			setter.Value = new Thickness(12.0, 0.0, 12.0, 0.0);
			controlTheme.Add(setter);
			Setter setter2 = new Setter();
			setter2.Property = Layoutable.VerticalAlignmentProperty;
			setter2.Value = VerticalAlignment.Center;
			controlTheme.Add(setter2);
			return controlTheme;
		}
	}

	private class XamlClosure_50
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(TextBox);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension(typeof(TextBox));
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Property();
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			controlTheme.BasedOn = (ControlTheme)obj;
			Setter setter = new Setter();
			setter.Property = Layoutable.VerticalAlignmentProperty;
			setter.Value = VerticalAlignment.Stretch;
			controlTheme.Add(setter);
			Setter setter2 = new Setter();
			setter2.Property = TemplatedControl.BackgroundProperty;
			setter2.Value = new ImmutableSolidColorBrush(16777215u);
			controlTheme.Add(setter2);
			Style style;
			Style style2 = (style = new Style());
			context.PushParent(style);
			style.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(DataValidationErrors));
			Setter setter3;
			Setter setter4 = (setter3 = new Setter());
			context.PushParent(setter3);
			setter3.Property = StyledElement.ThemeProperty;
			StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("TooltipDataValidationErrors");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			object? value = staticResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter3.Value = value;
			context.PopParent();
			style.Add(setter4);
			context.PopParent();
			controlTheme.Add(style2);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_51
	{
		private class XamlClosure_52
		{
			private class DynamicSetters_53
			{
				public static void DynamicSetter_1(ContentPresenter P_0, BindingPriority P_1, IBinding P_2)
				{
					if (P_2 != null)
					{
						IBinding binding = P_2;
						P_0.Bind(ContentPresenter.ContentProperty, binding);
					}
					else
					{
						object value = P_2;
						int priority = (int)P_1;
						P_0.SetValue(ContentPresenter.ContentProperty, value, (BindingPriority)priority);
					}
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (Styles)service;
					}
				}
				context.IntermediateRoot = new Border();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				Border border = (Border)intermediateRoot;
				context.PushParent(border);
				border.Name = "CellBorder";
				service = border;
				context.AvaloniaNameScope.Register("CellBorder", service);
				border.Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				border.Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				border.Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				border.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				Grid grid;
				Grid grid2 = (grid = new Grid());
				((ISupportInitialize)grid2).BeginInit();
				border.Child = grid2;
				Grid grid3;
				Grid grid4 = (grid3 = grid);
				context.PushParent(grid3);
				Grid grid5 = grid3;
				grid5.Name = "PART_CellRoot";
				service = grid5;
				context.AvaloniaNameScope.Register("PART_CellRoot", service);
				ColumnDefinitions columnDefinitions = new ColumnDefinitions();
				columnDefinitions.Capacity = 2;
				columnDefinitions.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				grid5.ColumnDefinitions = columnDefinitions;
				Controls children = grid5.Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children.Add(rectangle2);
				Rectangle rectangle3;
				Rectangle rectangle4 = (rectangle3 = rectangle);
				context.PushParent(rectangle3);
				Rectangle rectangle5 = rectangle3;
				rectangle5.Name = "CurrencyVisual";
				service = rectangle5;
				context.AvaloniaNameScope.Register("CurrencyVisual", service);
				rectangle5.SetValue(Visual.IsVisibleProperty, value: false, BindingPriority.Template);
				rectangle5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				rectangle5.SetValue(Shape.FillProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				rectangle5.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				StyledProperty<IBrush?> strokeProperty = Shape.StrokeProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("DataGridCurrencyVisualPrimaryBrush");
				context.ProvideTargetProperty = Shape.StrokeProperty;
				IBinding binding = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle5.Bind(strokeProperty, binding);
				rectangle5.SetValue(Shape.StrokeThicknessProperty, 1.0, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)rectangle4).EndInit();
				Controls children2 = grid5.Children;
				Grid grid6;
				Grid grid7 = (grid6 = new Grid());
				((ISupportInitialize)grid7).BeginInit();
				children2.Add(grid7);
				Grid grid8 = (grid3 = grid6);
				context.PushParent(grid3);
				Grid grid9 = grid3;
				grid9.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				grid9.Name = "FocusVisual";
				service = grid9;
				context.AvaloniaNameScope.Register("FocusVisual", service);
				grid9.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				grid9.SetValue(Visual.IsVisibleProperty, value: false, BindingPriority.Template);
				Controls children3 = grid9.Children;
				Rectangle rectangle6;
				Rectangle rectangle7 = (rectangle6 = new Rectangle());
				((ISupportInitialize)rectangle7).BeginInit();
				children3.Add(rectangle7);
				Rectangle rectangle8 = (rectangle3 = rectangle6);
				context.PushParent(rectangle3);
				Rectangle rectangle9 = rectangle3;
				rectangle9.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				rectangle9.SetValue(Shape.FillProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				rectangle9.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				StyledProperty<IBrush?> strokeProperty2 = Shape.StrokeProperty;
				DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("DataGridCellFocusVisualPrimaryBrush");
				context.ProvideTargetProperty = Shape.StrokeProperty;
				IBinding binding2 = dynamicResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle9.Bind(strokeProperty2, binding2);
				rectangle9.SetValue(Shape.StrokeThicknessProperty, 2.0, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)rectangle8).EndInit();
				Controls children4 = grid9.Children;
				Rectangle rectangle10;
				Rectangle rectangle11 = (rectangle10 = new Rectangle());
				((ISupportInitialize)rectangle11).BeginInit();
				children4.Add(rectangle11);
				Rectangle rectangle12 = (rectangle3 = rectangle10);
				context.PushParent(rectangle3);
				Rectangle rectangle13 = rectangle3;
				rectangle13.SetValue(Layoutable.MarginProperty, new Thickness(2.0, 2.0, 2.0, 2.0), BindingPriority.Template);
				rectangle13.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				rectangle13.SetValue(Shape.FillProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				rectangle13.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				StyledProperty<IBrush?> strokeProperty3 = Shape.StrokeProperty;
				DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("DataGridCellFocusVisualSecondaryBrush");
				context.ProvideTargetProperty = Shape.StrokeProperty;
				IBinding binding3 = dynamicResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle13.Bind(strokeProperty3, binding3);
				rectangle13.SetValue(Shape.StrokeThicknessProperty, 1.0, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)rectangle12).EndInit();
				context.PopParent();
				((ISupportInitialize)grid8).EndInit();
				Controls children5 = grid5.Children;
				ContentPresenter contentPresenter;
				ContentPresenter contentPresenter2 = (contentPresenter = new ContentPresenter());
				((ISupportInitialize)contentPresenter2).BeginInit();
				children5.Add(contentPresenter2);
				contentPresenter.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				contentPresenter.Bind(Layoutable.MarginProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				contentPresenter.Bind(Layoutable.HorizontalAlignmentProperty, new TemplateBinding(ContentControl.HorizontalContentAlignmentProperty).ProvideValue());
				contentPresenter.Bind(Layoutable.VerticalAlignmentProperty, new TemplateBinding(ContentControl.VerticalContentAlignmentProperty).ProvideValue());
				DynamicSetters_53.DynamicSetter_1(contentPresenter, BindingPriority.Template, new TemplateBinding(ContentControl.ContentProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.ContentTemplateProperty, new TemplateBinding(ContentControl.ContentTemplateProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.ForegroundProperty, new TemplateBinding(TemplatedControl.ForegroundProperty).ProvideValue());
				((ISupportInitialize)contentPresenter).EndInit();
				Controls children6 = grid5.Children;
				Rectangle rectangle14;
				Rectangle rectangle15 = (rectangle14 = new Rectangle());
				((ISupportInitialize)rectangle15).BeginInit();
				children6.Add(rectangle15);
				Rectangle rectangle16 = (rectangle3 = rectangle14);
				context.PushParent(rectangle3);
				Rectangle rectangle17 = rectangle3;
				rectangle17.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				rectangle17.Name = "InvalidVisualElement";
				service = rectangle17;
				context.AvaloniaNameScope.Register("InvalidVisualElement", service);
				rectangle17.SetValue(Visual.IsVisibleProperty, value: false, BindingPriority.Template);
				rectangle17.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				rectangle17.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				StyledProperty<IBrush?> strokeProperty4 = Shape.StrokeProperty;
				DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("DataGridCellInvalidBrush");
				context.ProvideTargetProperty = Shape.StrokeProperty;
				IBinding binding4 = dynamicResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle17.Bind(strokeProperty4, binding4);
				rectangle17.SetValue(Shape.StrokeThicknessProperty, 1.0, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)rectangle16).EndInit();
				Controls children7 = grid5.Children;
				Rectangle rectangle18;
				Rectangle rectangle19 = (rectangle18 = new Rectangle());
				((ISupportInitialize)rectangle19).BeginInit();
				children7.Add(rectangle19);
				Rectangle rectangle20 = (rectangle3 = rectangle18);
				context.PushParent(rectangle3);
				Rectangle rectangle21 = rectangle3;
				rectangle21.Name = "PART_RightGridLine";
				service = rectangle21;
				context.AvaloniaNameScope.Register("PART_RightGridLine", service);
				rectangle21.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				rectangle21.SetValue(Layoutable.WidthProperty, 1.0, BindingPriority.Template);
				rectangle21.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<IBrush?> fillProperty = Shape.FillProperty;
				DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("DataGridFillerColumnGridLinesBrush");
				context.ProvideTargetProperty = Shape.FillProperty;
				IBinding binding5 = dynamicResourceExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle21.Bind(fillProperty, binding5);
				context.PopParent();
				((ISupportInitialize)rectangle20).EndInit();
				context.PopParent();
				((ISupportInitialize)grid4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(DataGridCell);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = TemplatedControl.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("DataGridCellBackgroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Setter setter4 = new Setter();
			setter4.Property = ContentControl.HorizontalContentAlignmentProperty;
			setter4.Value = HorizontalAlignment.Stretch;
			controlTheme.Add(setter4);
			Setter setter5 = new Setter();
			setter5.Property = ContentControl.VerticalContentAlignmentProperty;
			setter5.Value = VerticalAlignment.Stretch;
			controlTheme.Add(setter5);
			Setter setter6 = new Setter();
			setter6.Property = TemplatedControl.FontSizeProperty;
			setter6.Value = 15.0;
			controlTheme.Add(setter6);
			Setter setter7 = new Setter();
			setter7.Property = Layoutable.MinHeightProperty;
			setter7.Value = 32.0;
			controlTheme.Add(setter7);
			Setter setter8 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter9 = setter;
			setter9.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value2 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_52.Build, context);
			context.PopParent();
			setter9.Value = value2;
			context.PopParent();
			controlTheme.Add(setter8);
			Style style = new Style();
			style.Selector = ((Selector?)null).Nesting().Class(":current").Template()
				.OfType(typeof(Rectangle))
				.Name("CurrencyVisual");
			Setter setter10 = new Setter();
			setter10.Property = Visual.IsVisibleProperty;
			setter10.Value = true;
			style.Add(setter10);
			controlTheme.Add(style);
			Style style2 = new Style();
			style2.Selector = ((Selector?)null).Nesting().Class(":focus").Template()
				.OfType(typeof(Grid))
				.Name("FocusVisual");
			Setter setter11 = new Setter();
			setter11.Property = Visual.IsVisibleProperty;
			setter11.Value = true;
			style2.Add(setter11);
			controlTheme.Add(style2);
			Style style3 = new Style();
			style3.Selector = ((Selector?)null).Nesting().Class(":invalid").Template()
				.OfType(typeof(Rectangle))
				.Name("InvalidVisualElement");
			Setter setter12 = new Setter();
			setter12.Property = Visual.IsVisibleProperty;
			setter12.Value = true;
			style3.Add(setter12);
			controlTheme.Add(style3);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_54
	{
		private class XamlClosure_55
		{
			private class DynamicSetters_56
			{
				public static void DynamicSetter_1(ContentPresenter P_0, BindingPriority P_1, IBinding P_2)
				{
					if (P_2 != null)
					{
						IBinding binding = P_2;
						P_0.Bind(ContentPresenter.ContentProperty, binding);
					}
					else
					{
						object value = P_2;
						int priority = (int)P_1;
						P_0.SetValue(ContentPresenter.ContentProperty, value, (BindingPriority)priority);
					}
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (Styles)service;
					}
				}
				context.IntermediateRoot = new Border();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				Border border = (Border)intermediateRoot;
				context.PushParent(border);
				border.Name = "HeaderBorder";
				service = border;
				context.AvaloniaNameScope.Register("HeaderBorder", service);
				border.Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				border.Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				border.Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				border.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				Grid grid;
				Grid grid2 = (grid = new Grid());
				((ISupportInitialize)grid2).BeginInit();
				border.Child = grid2;
				Grid grid3;
				Grid grid4 = (grid3 = grid);
				context.PushParent(grid3);
				Grid grid5 = grid3;
				grid5.Name = "PART_ColumnHeaderRoot";
				service = grid5;
				context.AvaloniaNameScope.Register("PART_ColumnHeaderRoot", service);
				ColumnDefinitions columnDefinitions = new ColumnDefinitions();
				columnDefinitions.Capacity = 2;
				columnDefinitions.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				grid5.ColumnDefinitions = columnDefinitions;
				Controls children = grid5.Children;
				Grid grid6;
				Grid grid7 = (grid6 = new Grid());
				((ISupportInitialize)grid7).BeginInit();
				children.Add(grid7);
				Grid grid8 = (grid3 = grid6);
				context.PushParent(grid3);
				Grid grid9 = grid3;
				grid9.Bind(Layoutable.MarginProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				grid9.Bind(Layoutable.HorizontalAlignmentProperty, new TemplateBinding(ContentControl.HorizontalContentAlignmentProperty).ProvideValue());
				grid9.Bind(Layoutable.VerticalAlignmentProperty, new TemplateBinding(ContentControl.VerticalContentAlignmentProperty).ProvideValue());
				ColumnDefinitions columnDefinitions2 = grid9.ColumnDefinitions;
				ColumnDefinition columnDefinition = new ColumnDefinition();
				columnDefinition.SetValue(ColumnDefinition.WidthProperty, new GridLength(1.0, GridUnitType.Star), BindingPriority.Template);
				columnDefinitions2.Add(columnDefinition);
				ColumnDefinitions columnDefinitions3 = grid9.ColumnDefinitions;
				ColumnDefinition columnDefinition2;
				ColumnDefinition item = (columnDefinition2 = new ColumnDefinition());
				context.PushParent(columnDefinition2);
				columnDefinition2.SetValue(ColumnDefinition.WidthProperty, new GridLength(0.0, GridUnitType.Auto), BindingPriority.Template);
				StyledProperty<double> minWidthProperty = ColumnDefinition.MinWidthProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("DataGridSortIconMinWidth");
				context.ProvideTargetProperty = ColumnDefinition.MinWidthProperty;
				IBinding binding = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				columnDefinition2.Bind(minWidthProperty, binding);
				context.PopParent();
				columnDefinitions3.Add(item);
				Controls children2 = grid9.Children;
				ContentPresenter contentPresenter;
				ContentPresenter contentPresenter2 = (contentPresenter = new ContentPresenter());
				((ISupportInitialize)contentPresenter2).BeginInit();
				children2.Add(contentPresenter2);
				DynamicSetters_56.DynamicSetter_1(contentPresenter, BindingPriority.Template, new TemplateBinding(ContentControl.ContentProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.ContentTemplateProperty, new TemplateBinding(ContentControl.ContentTemplateProperty).ProvideValue());
				((ISupportInitialize)contentPresenter).EndInit();
				Controls children3 = grid9.Children;
				Path path;
				Path path2 = (path = new Path());
				((ISupportInitialize)path2).BeginInit();
				children3.Add(path2);
				path.Name = "SortIcon";
				service = path;
				context.AvaloniaNameScope.Register("SortIcon", service);
				path.SetValue(Visual.IsVisibleProperty, value: false, BindingPriority.Template);
				path.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				path.SetValue(Layoutable.HeightProperty, 12.0, BindingPriority.Template);
				path.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				path.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				path.Bind(Shape.FillProperty, new TemplateBinding(TemplatedControl.ForegroundProperty).ProvideValue());
				path.SetValue(Shape.StretchProperty, Stretch.Uniform, BindingPriority.Template);
				((ISupportInitialize)path).EndInit();
				context.PopParent();
				((ISupportInitialize)grid8).EndInit();
				Controls children4 = grid5.Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children4.Add(rectangle2);
				rectangle.Name = "VerticalSeparator";
				service = rectangle;
				context.AvaloniaNameScope.Register("VerticalSeparator", service);
				rectangle.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				rectangle.SetValue(Layoutable.WidthProperty, 1.0, BindingPriority.Template);
				rectangle.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				rectangle.Bind(Shape.FillProperty, new TemplateBinding(DataGridColumnHeader.SeparatorBrushProperty).ProvideValue());
				rectangle.Bind(Visual.IsVisibleProperty, new TemplateBinding(DataGridColumnHeader.AreSeparatorsVisibleProperty).ProvideValue());
				((ISupportInitialize)rectangle).EndInit();
				Controls children5 = grid5.Children;
				Grid grid10;
				Grid grid11 = (grid10 = new Grid());
				((ISupportInitialize)grid11).BeginInit();
				children5.Add(grid11);
				Grid grid12 = (grid3 = grid10);
				context.PushParent(grid3);
				Grid grid13 = grid3;
				grid13.Name = "FocusVisual";
				service = grid13;
				context.AvaloniaNameScope.Register("FocusVisual", service);
				grid13.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				grid13.SetValue(Visual.IsVisibleProperty, value: false, BindingPriority.Template);
				Controls children6 = grid13.Children;
				Rectangle rectangle3;
				Rectangle rectangle4 = (rectangle3 = new Rectangle());
				((ISupportInitialize)rectangle4).BeginInit();
				children6.Add(rectangle4);
				Rectangle rectangle5;
				Rectangle rectangle6 = (rectangle5 = rectangle3);
				context.PushParent(rectangle5);
				Rectangle rectangle7 = rectangle5;
				rectangle7.Name = "FocusVisualPrimary";
				service = rectangle7;
				context.AvaloniaNameScope.Register("FocusVisualPrimary", service);
				rectangle7.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle7.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				rectangle7.SetValue(Shape.FillProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				rectangle7.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				StyledProperty<IBrush?> strokeProperty = Shape.StrokeProperty;
				DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("DataGridCellFocusVisualPrimaryBrush");
				context.ProvideTargetProperty = Shape.StrokeProperty;
				IBinding binding2 = dynamicResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle7.Bind(strokeProperty, binding2);
				rectangle7.SetValue(Shape.StrokeThicknessProperty, 2.0, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)rectangle6).EndInit();
				Controls children7 = grid13.Children;
				Rectangle rectangle8;
				Rectangle rectangle9 = (rectangle8 = new Rectangle());
				((ISupportInitialize)rectangle9).BeginInit();
				children7.Add(rectangle9);
				Rectangle rectangle10 = (rectangle5 = rectangle8);
				context.PushParent(rectangle5);
				Rectangle rectangle11 = rectangle5;
				rectangle11.Name = "FocusVisualSecondary";
				service = rectangle11;
				context.AvaloniaNameScope.Register("FocusVisualSecondary", service);
				rectangle11.SetValue(Layoutable.MarginProperty, new Thickness(2.0, 2.0, 2.0, 2.0), BindingPriority.Template);
				rectangle11.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle11.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				rectangle11.SetValue(Shape.FillProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				rectangle11.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				StyledProperty<IBrush?> strokeProperty2 = Shape.StrokeProperty;
				DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("DataGridCellFocusVisualSecondaryBrush");
				context.ProvideTargetProperty = Shape.StrokeProperty;
				IBinding binding3 = dynamicResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle11.Bind(strokeProperty2, binding3);
				rectangle11.SetValue(Shape.StrokeThicknessProperty, 1.0, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)rectangle10).EndInit();
				context.PopParent();
				((ISupportInitialize)grid12).EndInit();
				context.PopParent();
				((ISupportInitialize)grid4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(DataGridColumnHeader);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = TemplatedControl.ForegroundProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("DataGridColumnHeaderForegroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Setter setter4 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter5 = setter;
			setter5.Property = TemplatedControl.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("DataGridColumnHeaderBackgroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value2 = dynamicResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter5.Value = value2;
			context.PopParent();
			controlTheme.Add(setter4);
			Setter setter6 = new Setter();
			setter6.Property = ContentControl.HorizontalContentAlignmentProperty;
			setter6.Value = HorizontalAlignment.Stretch;
			controlTheme.Add(setter6);
			Setter setter7 = new Setter();
			setter7.Property = ContentControl.VerticalContentAlignmentProperty;
			setter7.Value = VerticalAlignment.Center;
			controlTheme.Add(setter7);
			Setter setter8 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter9 = setter;
			setter9.Property = DataGridColumnHeader.SeparatorBrushProperty;
			DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("DataGridGridLinesBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value3 = dynamicResourceExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter9.Value = value3;
			context.PopParent();
			controlTheme.Add(setter8);
			Setter setter10 = new Setter();
			setter10.Property = TemplatedControl.PaddingProperty;
			setter10.Value = new Thickness(12.0, 0.0, 0.0, 0.0);
			controlTheme.Add(setter10);
			Setter setter11 = new Setter();
			setter11.Property = TemplatedControl.FontSizeProperty;
			setter11.Value = 12.0;
			controlTheme.Add(setter11);
			Setter setter12 = new Setter();
			setter12.Property = Layoutable.MinHeightProperty;
			setter12.Value = 32.0;
			controlTheme.Add(setter12);
			Setter setter13 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter14 = setter;
			setter14.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value4 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_55.Build, context);
			context.PopParent();
			setter14.Value = value4;
			context.PopParent();
			controlTheme.Add(setter13);
			Style style = new Style();
			style.Selector = ((Selector?)null).Nesting().Class(":focus-visible").Template()
				.OfType(typeof(Grid))
				.Name("FocusVisual");
			Setter setter15 = new Setter();
			setter15.Property = Visual.IsVisibleProperty;
			setter15.Value = true;
			style.Add(setter15);
			controlTheme.Add(style);
			Style style2;
			Style style3 = (style2 = new Style());
			context.PushParent(style2);
			Style style4 = style2;
			style4.Selector = ((Selector?)null).Nesting().Class(":pointerover").Template()
				.OfType(typeof(Grid))
				.Name("PART_ColumnHeaderRoot");
			Setter setter16 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter17 = setter;
			setter17.Property = Panel.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("DataGridColumnHeaderHoveredBackgroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value5 = dynamicResourceExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter17.Value = value5;
			context.PopParent();
			style4.Add(setter16);
			context.PopParent();
			controlTheme.Add(style3);
			Style style5 = (style2 = new Style());
			context.PushParent(style2);
			Style style6 = style2;
			style6.Selector = ((Selector?)null).Nesting().Class(":pressed").Template()
				.OfType(typeof(Grid))
				.Name("PART_ColumnHeaderRoot");
			Setter setter18 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter19 = setter;
			setter19.Property = Panel.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("DataGridColumnHeaderPressedBackgroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value6 = dynamicResourceExtension5.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter19.Value = value6;
			context.PopParent();
			style6.Add(setter18);
			context.PopParent();
			controlTheme.Add(style5);
			Style style7 = new Style();
			style7.Selector = ((Selector?)null).Nesting().Class(":dragIndicator");
			Setter setter20 = new Setter();
			setter20.Property = Visual.OpacityProperty;
			setter20.Value = 0.5;
			style7.Add(setter20);
			controlTheme.Add(style7);
			Style style8 = (style2 = new Style());
			context.PushParent(style2);
			Style style9 = style2;
			style9.Selector = ((Selector?)null).Nesting().Class(":sortascending").Template()
				.OfType(typeof(Path))
				.Name("SortIcon");
			Setter setter21 = new Setter();
			setter21.Property = Visual.IsVisibleProperty;
			setter21.Value = true;
			style9.Add(setter21);
			Setter setter22 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter23 = setter;
			setter23.Property = Path.DataProperty;
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("DataGridSortIconAscendingPath");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			object? value7 = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter23.Value = value7;
			context.PopParent();
			style9.Add(setter22);
			context.PopParent();
			controlTheme.Add(style8);
			Style style10 = (style2 = new Style());
			context.PushParent(style2);
			Style style11 = style2;
			style11.Selector = ((Selector?)null).Nesting().Class(":sortdescending").Template()
				.OfType(typeof(Path))
				.Name("SortIcon");
			Setter setter24 = new Setter();
			setter24.Property = Visual.IsVisibleProperty;
			setter24.Value = true;
			style11.Add(setter24);
			Setter setter25 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter26 = setter;
			setter26.Property = Path.DataProperty;
			StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("DataGridSortIconDescendingPath");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			object? value8 = staticResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter26.Value = value8;
			context.PopParent();
			style11.Add(setter25);
			context.PopParent();
			controlTheme.Add(style10);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_57
	{
		private class XamlClosure_58
		{
			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (Styles)service;
					}
				}
				context.IntermediateRoot = new Grid();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				Grid grid = (Grid)intermediateRoot;
				context.PushParent(grid);
				grid.Name = "TopLeftHeaderRoot";
				service = grid;
				context.AvaloniaNameScope.Register("TopLeftHeaderRoot", service);
				RowDefinitions rowDefinitions = new RowDefinitions();
				rowDefinitions.Capacity = 3;
				rowDefinitions.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				grid.RowDefinitions = rowDefinitions;
				Controls children = grid.Children;
				Border border;
				Border border2 = (border = new Border());
				((ISupportInitialize)border2).BeginInit();
				children.Add(border2);
				Border border3;
				Border border4 = (border3 = border);
				context.PushParent(border3);
				border3.SetValue(Grid.RowSpanProperty, 2, BindingPriority.Template);
				border3.SetValue(Border.BorderThicknessProperty, new Thickness(0.0, 0.0, 1.0, 0.0), BindingPriority.Template);
				StyledProperty<IBrush?> borderBrushProperty = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("DataGridGridLinesBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border3.Bind(borderBrushProperty, binding);
				context.PopParent();
				((ISupportInitialize)border4).EndInit();
				Controls children2 = grid.Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children2.Add(rectangle2);
				Rectangle rectangle3;
				Rectangle rectangle4 = (rectangle3 = rectangle);
				context.PushParent(rectangle3);
				rectangle3.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				rectangle3.SetValue(Grid.RowSpanProperty, 2, BindingPriority.Template);
				rectangle3.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Bottom, BindingPriority.Template);
				rectangle3.SetValue(Shape.StrokeThicknessProperty, 1.0, BindingPriority.Template);
				rectangle3.SetValue(Layoutable.HeightProperty, 1.0, BindingPriority.Template);
				StyledProperty<IBrush?> fillProperty = Shape.FillProperty;
				DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("DataGridGridLinesBrush");
				context.ProvideTargetProperty = Shape.FillProperty;
				IBinding binding2 = dynamicResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle3.Bind(fillProperty, binding2);
				context.PopParent();
				((ISupportInitialize)rectangle4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(DataGridColumnHeader);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension(typeof(DataGridColumnHeader));
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Property();
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			controlTheme.BasedOn = (ControlTheme)obj;
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			setter.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_58.Build, context);
			context.PopParent();
			setter.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_59
	{
		private class XamlClosure_60
		{
			private class DynamicSetters_61
			{
				public static void DynamicSetter_1(ContentPresenter P_0, BindingPriority P_1, IBinding P_2)
				{
					if (P_2 != null)
					{
						IBinding binding = P_2;
						P_0.Bind(ContentPresenter.ContentProperty, binding);
					}
					else
					{
						object value = P_2;
						int priority = (int)P_1;
						P_0.SetValue(ContentPresenter.ContentProperty, value, (BindingPriority)priority);
					}
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (Styles)service;
					}
				}
				context.IntermediateRoot = new Grid();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				Grid grid = (Grid)intermediateRoot;
				context.PushParent(grid);
				Grid grid2 = grid;
				grid2.Name = "PART_Root";
				service = grid2;
				context.AvaloniaNameScope.Register("PART_Root", service);
				RowDefinitions rowDefinitions = new RowDefinitions();
				rowDefinitions.Capacity = 3;
				rowDefinitions.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				grid2.RowDefinitions = rowDefinitions;
				ColumnDefinitions columnDefinitions = new ColumnDefinitions();
				columnDefinitions.Capacity = 2;
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid2.ColumnDefinitions = columnDefinitions;
				Controls children = grid2.Children;
				Border border;
				Border border2 = (border = new Border());
				((ISupportInitialize)border2).BeginInit();
				children.Add(border2);
				Border border3;
				Border border4 = (border3 = border);
				context.PushParent(border3);
				border3.SetValue(Grid.RowSpanProperty, 3, BindingPriority.Template);
				border3.SetValue(Grid.ColumnSpanProperty, 2, BindingPriority.Template);
				border3.Bind(Border.BorderBrushProperty, new TemplateBinding(DataGridRowHeader.SeparatorBrushProperty).ProvideValue());
				border3.SetValue(Border.BorderThicknessProperty, new Thickness(0.0, 0.0, 1.0, 0.0), BindingPriority.Template);
				Grid grid3;
				Grid grid4 = (grid3 = new Grid());
				((ISupportInitialize)grid4).BeginInit();
				border3.Child = grid4;
				Grid grid5 = (grid = grid3);
				context.PushParent(grid);
				Grid grid6 = grid;
				grid6.Bind(Panel.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				Controls children2 = grid6.Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children2.Add(rectangle2);
				Rectangle rectangle3;
				Rectangle rectangle4 = (rectangle3 = rectangle);
				context.PushParent(rectangle3);
				Rectangle rectangle5 = rectangle3;
				rectangle5.Name = "RowInvalidVisualElement";
				service = rectangle5;
				context.AvaloniaNameScope.Register("RowInvalidVisualElement", service);
				rectangle5.SetValue(Visual.OpacityProperty, 0.0, BindingPriority.Template);
				StyledProperty<IBrush?> fillProperty = Shape.FillProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("DataGridRowInvalidBrush");
				context.ProvideTargetProperty = Shape.FillProperty;
				IBinding binding = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle5.Bind(fillProperty, binding);
				rectangle5.SetValue(Shape.StretchProperty, Stretch.Fill, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)rectangle4).EndInit();
				Controls children3 = grid6.Children;
				Rectangle rectangle6;
				Rectangle rectangle7 = (rectangle6 = new Rectangle());
				((ISupportInitialize)rectangle7).BeginInit();
				children3.Add(rectangle7);
				Rectangle rectangle8 = (rectangle3 = rectangle6);
				context.PushParent(rectangle3);
				Rectangle rectangle9 = rectangle3;
				rectangle9.Name = "BackgroundRectangle";
				service = rectangle9;
				context.AvaloniaNameScope.Register("BackgroundRectangle", service);
				StyledProperty<IBrush?> fillProperty2 = Shape.FillProperty;
				DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("DataGridRowBackgroundBrush");
				context.ProvideTargetProperty = Shape.FillProperty;
				IBinding binding2 = dynamicResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle9.Bind(fillProperty2, binding2);
				rectangle9.SetValue(Shape.StretchProperty, Stretch.Fill, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)rectangle8).EndInit();
				context.PopParent();
				((ISupportInitialize)grid5).EndInit();
				context.PopParent();
				((ISupportInitialize)border4).EndInit();
				Controls children4 = grid2.Children;
				Rectangle rectangle10;
				Rectangle rectangle11 = (rectangle10 = new Rectangle());
				((ISupportInitialize)rectangle11).BeginInit();
				children4.Add(rectangle11);
				rectangle10.Name = "HorizontalSeparator";
				service = rectangle10;
				context.AvaloniaNameScope.Register("HorizontalSeparator", service);
				rectangle10.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				rectangle10.SetValue(Grid.ColumnSpanProperty, 2, BindingPriority.Template);
				rectangle10.SetValue(Layoutable.HeightProperty, 1.0, BindingPriority.Template);
				rectangle10.SetValue(Layoutable.MarginProperty, new Thickness(1.0, 0.0, 1.0, 0.0), BindingPriority.Template);
				rectangle10.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle10.Bind(Shape.FillProperty, new TemplateBinding(DataGridRowHeader.SeparatorBrushProperty).ProvideValue());
				rectangle10.Bind(Visual.IsVisibleProperty, new TemplateBinding(DataGridRowHeader.AreSeparatorsVisibleProperty).ProvideValue());
				((ISupportInitialize)rectangle10).EndInit();
				Controls children5 = grid2.Children;
				ContentPresenter contentPresenter;
				ContentPresenter contentPresenter2 = (contentPresenter = new ContentPresenter());
				((ISupportInitialize)contentPresenter2).BeginInit();
				children5.Add(contentPresenter2);
				contentPresenter.SetValue(Grid.RowSpanProperty, 2, BindingPriority.Template);
				contentPresenter.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				contentPresenter.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				contentPresenter.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				DynamicSetters_61.DynamicSetter_1(contentPresenter, BindingPriority.Template, new TemplateBinding(ContentControl.ContentProperty).ProvideValue());
				((ISupportInitialize)contentPresenter).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(DataGridRowHeader);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = DataGridRowHeader.SeparatorBrushProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("DataGridGridLinesBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Setter setter4 = new Setter();
			setter4.Property = DataGridRowHeader.AreSeparatorsVisibleProperty;
			setter4.Value = false;
			controlTheme.Add(setter4);
			Setter setter5 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter6 = setter;
			setter6.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value2 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_60.Build, context);
			context.PopParent();
			setter6.Value = value2;
			context.PopParent();
			controlTheme.Add(setter5);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_62
	{
		private class XamlClosure_63
		{
			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (Styles)service;
					}
				}
				context.IntermediateRoot = new Border();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				Border border = (Border)intermediateRoot;
				context.PushParent(border);
				border.Name = "RowBorder";
				service = border;
				context.AvaloniaNameScope.Register("RowBorder", service);
				border.Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				border.Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				border.Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				border.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				DataGridFrozenGrid dataGridFrozenGrid;
				DataGridFrozenGrid dataGridFrozenGrid2 = (dataGridFrozenGrid = new DataGridFrozenGrid());
				((ISupportInitialize)dataGridFrozenGrid2).BeginInit();
				border.Child = dataGridFrozenGrid2;
				DataGridFrozenGrid dataGridFrozenGrid3;
				DataGridFrozenGrid dataGridFrozenGrid4 = (dataGridFrozenGrid3 = dataGridFrozenGrid);
				context.PushParent(dataGridFrozenGrid3);
				dataGridFrozenGrid3.Name = "PART_Root";
				service = dataGridFrozenGrid3;
				context.AvaloniaNameScope.Register("PART_Root", service);
				ColumnDefinitions columnDefinitions = new ColumnDefinitions();
				columnDefinitions.Capacity = 2;
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				dataGridFrozenGrid3.ColumnDefinitions = columnDefinitions;
				RowDefinitions rowDefinitions = new RowDefinitions();
				rowDefinitions.Capacity = 3;
				rowDefinitions.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				dataGridFrozenGrid3.RowDefinitions = rowDefinitions;
				Controls children = dataGridFrozenGrid3.Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children.Add(rectangle2);
				Rectangle rectangle3;
				Rectangle rectangle4 = (rectangle3 = rectangle);
				context.PushParent(rectangle3);
				Rectangle rectangle5 = rectangle3;
				rectangle5.Name = "BackgroundRectangle";
				service = rectangle5;
				context.AvaloniaNameScope.Register("BackgroundRectangle", service);
				StyledProperty<IBrush?> fillProperty = Shape.FillProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("DataGridRowBackgroundBrush");
				context.ProvideTargetProperty = Shape.FillProperty;
				IBinding binding = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle5.Bind(fillProperty, binding);
				rectangle5.SetValue(Grid.RowSpanProperty, 2, BindingPriority.Template);
				rectangle5.SetValue(Grid.ColumnSpanProperty, 2, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)rectangle4).EndInit();
				Controls children2 = dataGridFrozenGrid3.Children;
				Rectangle rectangle6;
				Rectangle rectangle7 = (rectangle6 = new Rectangle());
				((ISupportInitialize)rectangle7).BeginInit();
				children2.Add(rectangle7);
				Rectangle rectangle8 = (rectangle3 = rectangle6);
				context.PushParent(rectangle3);
				Rectangle rectangle9 = rectangle3;
				rectangle9.Name = "InvalidVisualElement";
				service = rectangle9;
				context.AvaloniaNameScope.Register("InvalidVisualElement", service);
				rectangle9.SetValue(Visual.OpacityProperty, 0.0, BindingPriority.Template);
				rectangle9.SetValue(Grid.ColumnSpanProperty, 2, BindingPriority.Template);
				StyledProperty<IBrush?> fillProperty2 = Shape.FillProperty;
				DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("DataGridRowInvalidBrush");
				context.ProvideTargetProperty = Shape.FillProperty;
				IBinding binding2 = dynamicResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle9.Bind(fillProperty2, binding2);
				context.PopParent();
				((ISupportInitialize)rectangle8).EndInit();
				Controls children3 = dataGridFrozenGrid3.Children;
				DataGridRowHeader dataGridRowHeader;
				DataGridRowHeader dataGridRowHeader2 = (dataGridRowHeader = new DataGridRowHeader());
				((ISupportInitialize)dataGridRowHeader2).BeginInit();
				children3.Add(dataGridRowHeader2);
				dataGridRowHeader.Name = "PART_RowHeader";
				service = dataGridRowHeader;
				context.AvaloniaNameScope.Register("PART_RowHeader", service);
				dataGridRowHeader.SetValue(Grid.RowSpanProperty, 3, BindingPriority.Template);
				dataGridRowHeader.SetValue(DataGridFrozenGrid.IsFrozenProperty, value: true, BindingPriority.Template);
				((ISupportInitialize)dataGridRowHeader).EndInit();
				Controls children4 = dataGridFrozenGrid3.Children;
				DataGridCellsPresenter dataGridCellsPresenter;
				DataGridCellsPresenter dataGridCellsPresenter2 = (dataGridCellsPresenter = new DataGridCellsPresenter());
				((ISupportInitialize)dataGridCellsPresenter2).BeginInit();
				children4.Add(dataGridCellsPresenter2);
				dataGridCellsPresenter.Name = "PART_CellsPresenter";
				service = dataGridCellsPresenter;
				context.AvaloniaNameScope.Register("PART_CellsPresenter", service);
				dataGridCellsPresenter.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				dataGridCellsPresenter.SetValue(DataGridFrozenGrid.IsFrozenProperty, value: true, BindingPriority.Template);
				((ISupportInitialize)dataGridCellsPresenter).EndInit();
				Controls children5 = dataGridFrozenGrid3.Children;
				DataGridDetailsPresenter dataGridDetailsPresenter;
				DataGridDetailsPresenter dataGridDetailsPresenter2 = (dataGridDetailsPresenter = new DataGridDetailsPresenter());
				((ISupportInitialize)dataGridDetailsPresenter2).BeginInit();
				children5.Add(dataGridDetailsPresenter2);
				DataGridDetailsPresenter dataGridDetailsPresenter3;
				DataGridDetailsPresenter dataGridDetailsPresenter4 = (dataGridDetailsPresenter3 = dataGridDetailsPresenter);
				context.PushParent(dataGridDetailsPresenter3);
				dataGridDetailsPresenter3.Name = "PART_DetailsPresenter";
				service = dataGridDetailsPresenter3;
				context.AvaloniaNameScope.Register("PART_DetailsPresenter", service);
				dataGridDetailsPresenter3.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
				dataGridDetailsPresenter3.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty = Panel.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("DataGridDetailsPresenterBackgroundBrush");
				context.ProvideTargetProperty = Panel.BackgroundProperty;
				IBinding binding3 = dynamicResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				dataGridDetailsPresenter3.Bind(backgroundProperty, binding3);
				context.PopParent();
				((ISupportInitialize)dataGridDetailsPresenter4).EndInit();
				Controls children6 = dataGridFrozenGrid3.Children;
				Rectangle rectangle10;
				Rectangle rectangle11 = (rectangle10 = new Rectangle());
				((ISupportInitialize)rectangle11).BeginInit();
				children6.Add(rectangle11);
				rectangle10.Name = "PART_BottomGridLine";
				service = rectangle10;
				context.AvaloniaNameScope.Register("PART_BottomGridLine", service);
				rectangle10.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				rectangle10.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				rectangle10.SetValue(Layoutable.HeightProperty, 1.0, BindingPriority.Template);
				rectangle10.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				((ISupportInitialize)rectangle10).EndInit();
				context.PopParent();
				((ISupportInitialize)dataGridFrozenGrid4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(DataGridRow);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = TemplatedControl.BackgroundProperty;
			CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(DataGrid), 0).Property(DataGrid.RowBackgroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			CompiledBindingExtension value = compiledBindingExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Setter setter4 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter5 = setter;
			setter5.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value2 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_63.Build, context);
			context.PopParent();
			setter5.Value = value2;
			context.PopParent();
			controlTheme.Add(setter4);
			Style style = new Style();
			style.Selector = ((Selector?)null).Nesting().Class(":invalid");
			Style style2 = new Style();
			style2.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(Rectangle))
				.Name("InvalidVisualElement");
			Setter setter6 = new Setter();
			setter6.Property = Visual.OpacityProperty;
			setter6.Value = 0.4;
			style2.Add(setter6);
			style.Add(style2);
			Style style3 = new Style();
			style3.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(Rectangle))
				.Name("BackgroundRectangle");
			Setter setter7 = new Setter();
			setter7.Property = Visual.OpacityProperty;
			setter7.Value = 0.0;
			style3.Add(setter7);
			style.Add(style3);
			controlTheme.Add(style);
			Style style4;
			Style style5 = (style4 = new Style());
			context.PushParent(style4);
			Style style6 = style4;
			style6.Selector = ((Selector?)null).Nesting().Class(":pointerover").Template()
				.OfType(typeof(Rectangle))
				.Name("BackgroundRectangle");
			Setter setter8 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter9 = setter;
			setter9.Property = Shape.FillProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("DataGridRowHoveredBackgroundColor");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value3 = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter9.Value = value3;
			context.PopParent();
			style6.Add(setter8);
			context.PopParent();
			controlTheme.Add(style5);
			Style style7 = (style4 = new Style());
			context.PushParent(style4);
			Style style8 = style4;
			style8.Selector = ((Selector?)null).Nesting().Class(":selected");
			Style style9 = (style4 = new Style());
			context.PushParent(style4);
			Style style10 = style4;
			style10.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(Rectangle))
				.Name("BackgroundRectangle");
			Setter setter10 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter11 = setter;
			setter11.Property = Shape.FillProperty;
			DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("DataGridRowSelectedUnfocusedBackgroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value4 = dynamicResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter11.Value = value4;
			context.PopParent();
			style10.Add(setter10);
			Setter setter12 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter13 = setter;
			setter13.Property = Visual.OpacityProperty;
			DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("DataGridRowSelectedUnfocusedBackgroundOpacity");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value5 = dynamicResourceExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter13.Value = value5;
			context.PopParent();
			style10.Add(setter12);
			context.PopParent();
			style8.Add(style9);
			Style style11 = (style4 = new Style());
			context.PushParent(style4);
			Style style12 = style4;
			style12.Selector = ((Selector?)null).Nesting().Class(":pointerover").Template()
				.OfType(typeof(Rectangle))
				.Name("BackgroundRectangle");
			Setter setter14 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter15 = setter;
			setter15.Property = Shape.FillProperty;
			DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("DataGridRowSelectedHoveredUnfocusedBackgroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value6 = dynamicResourceExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter15.Value = value6;
			context.PopParent();
			style12.Add(setter14);
			Setter setter16 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter17 = setter;
			setter17.Property = Visual.OpacityProperty;
			DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("DataGridRowSelectedHoveredUnfocusedBackgroundOpacity");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value7 = dynamicResourceExtension5.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter17.Value = value7;
			context.PopParent();
			style12.Add(setter16);
			context.PopParent();
			style8.Add(style11);
			Style style13 = (style4 = new Style());
			context.PushParent(style4);
			Style style14 = style4;
			style14.Selector = ((Selector?)null).Nesting().Class(":focus").Template()
				.OfType(typeof(Rectangle))
				.Name("BackgroundRectangle");
			Setter setter18 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter19 = setter;
			setter19.Property = Shape.FillProperty;
			DynamicResourceExtension dynamicResourceExtension6 = new DynamicResourceExtension("DataGridRowSelectedBackgroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value8 = dynamicResourceExtension6.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter19.Value = value8;
			context.PopParent();
			style14.Add(setter18);
			Setter setter20 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter21 = setter;
			setter21.Property = Visual.OpacityProperty;
			DynamicResourceExtension dynamicResourceExtension7 = new DynamicResourceExtension("DataGridRowSelectedBackgroundOpacity");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value9 = dynamicResourceExtension7.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter21.Value = value9;
			context.PopParent();
			style14.Add(setter20);
			context.PopParent();
			style8.Add(style13);
			Style style15 = (style4 = new Style());
			context.PushParent(style4);
			Style style16 = style4;
			style16.Selector = ((Selector?)null).Nesting().Class(":pointerover").Class(":focus")
				.Template()
				.OfType(typeof(Rectangle))
				.Name("BackgroundRectangle");
			Setter setter22 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter23 = setter;
			setter23.Property = Shape.FillProperty;
			DynamicResourceExtension dynamicResourceExtension8 = new DynamicResourceExtension("DataGridRowSelectedHoveredBackgroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value10 = dynamicResourceExtension8.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter23.Value = value10;
			context.PopParent();
			style16.Add(setter22);
			Setter setter24 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter25 = setter;
			setter25.Property = Visual.OpacityProperty;
			DynamicResourceExtension dynamicResourceExtension9 = new DynamicResourceExtension("DataGridRowSelectedHoveredBackgroundOpacity");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value11 = dynamicResourceExtension9.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter25.Value = value11;
			context.PopParent();
			style16.Add(setter24);
			context.PopParent();
			style8.Add(style15);
			context.PopParent();
			controlTheme.Add(style7);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_64
	{
		private class XamlClosure_65
		{
			private class DynamicSetters_66
			{
				public static void DynamicSetter_1(Path P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(Path.DataProperty, binding);
						return;
					}
					if (P_2 is Geometry)
					{
						Geometry value = (Geometry)P_2;
						int priority = (int)P_1;
						P_0.SetValue(Path.DataProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						Geometry value = (Geometry)P_2;
						int priority = (int)P_1;
						P_0.SetValue(Path.DataProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
				if (P_0 != null)
				{
					object service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (Styles)service;
					}
				}
				context.IntermediateRoot = new Border();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				Border border = (Border)intermediateRoot;
				context.PushParent(border);
				border.SetValue(Layoutable.WidthProperty, 12.0, BindingPriority.Template);
				border.SetValue(Layoutable.HeightProperty, 12.0, BindingPriority.Template);
				border.SetValue(Border.BackgroundProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				border.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				border.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Path path;
				Path path2 = (path = new Path());
				((ISupportInitialize)path2).BeginInit();
				border.Child = path2;
				Path path3;
				Path path4 = (path3 = path);
				context.PushParent(path3);
				path3.Bind(Shape.FillProperty, new TemplateBinding(TemplatedControl.ForegroundProperty).ProvideValue());
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("DataGridRowGroupHeaderIconClosedPath");
				context.ProvideTargetProperty = Path.DataProperty;
				object? obj = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_66.DynamicSetter_1(path3, BindingPriority.Template, obj);
				path3.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Right, BindingPriority.Template);
				path3.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				path3.SetValue(Shape.StretchProperty, Stretch.Uniform, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)path4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(ToggleButton);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_65.Build, context);
			context.PopParent();
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Style style;
			Style style2 = (style = new Style());
			context.PushParent(style);
			style.Selector = ((Selector?)null).Nesting().Class(":checked").Template()
				.OfType(typeof(Path));
			Setter setter4 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter5 = setter;
			setter5.Property = Path.DataProperty;
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("DataGridRowGroupHeaderIconOpenedPath");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			object? value2 = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter5.Value = value2;
			context.PopParent();
			style.Add(setter4);
			context.PopParent();
			controlTheme.Add(style2);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_67
	{
		private class XamlClosure_68
		{
			private class DynamicSetters_69
			{
				public static void DynamicSetter_1(StyledElement P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(StyledElement.ThemeProperty, binding);
						return;
					}
					if (P_2 is ControlTheme)
					{
						ControlTheme value = (ControlTheme)P_2;
						int priority = (int)P_1;
						P_0.SetValue(StyledElement.ThemeProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						ControlTheme value = (ControlTheme)P_2;
						int priority = (int)P_1;
						P_0.SetValue(StyledElement.ThemeProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (Styles)service;
					}
				}
				context.IntermediateRoot = new DataGridFrozenGrid();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				DataGridFrozenGrid dataGridFrozenGrid = (DataGridFrozenGrid)intermediateRoot;
				context.PushParent(dataGridFrozenGrid);
				dataGridFrozenGrid.Name = "PART_Root";
				service = dataGridFrozenGrid;
				context.AvaloniaNameScope.Register("PART_Root", service);
				dataGridFrozenGrid.Bind(Panel.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				dataGridFrozenGrid.Bind(Layoutable.MinHeightProperty, new TemplateBinding(Layoutable.MinHeightProperty).ProvideValue());
				ColumnDefinitions columnDefinitions = new ColumnDefinitions();
				columnDefinitions.Capacity = 5;
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				dataGridFrozenGrid.ColumnDefinitions = columnDefinitions;
				RowDefinitions rowDefinitions = new RowDefinitions();
				rowDefinitions.Capacity = 2;
				rowDefinitions.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				dataGridFrozenGrid.RowDefinitions = rowDefinitions;
				Controls children = dataGridFrozenGrid.Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children.Add(rectangle2);
				rectangle.Name = "PART_IndentSpacer";
				service = rectangle;
				context.AvaloniaNameScope.Register("PART_IndentSpacer", service);
				rectangle.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				((ISupportInitialize)rectangle).EndInit();
				Controls children2 = dataGridFrozenGrid.Children;
				ToggleButton toggleButton;
				ToggleButton toggleButton2 = (toggleButton = new ToggleButton());
				((ISupportInitialize)toggleButton2).BeginInit();
				children2.Add(toggleButton2);
				ToggleButton toggleButton3;
				ToggleButton toggleButton4 = (toggleButton3 = toggleButton);
				context.PushParent(toggleButton3);
				toggleButton3.Name = "PART_ExpanderButton";
				service = toggleButton3;
				context.AvaloniaNameScope.Register("PART_ExpanderButton", service);
				toggleButton3.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				toggleButton3.SetValue(Layoutable.WidthProperty, 12.0, BindingPriority.Template);
				toggleButton3.SetValue(Layoutable.HeightProperty, 12.0, BindingPriority.Template);
				toggleButton3.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("FluentDataGridRowGroupExpanderButtonTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_69.DynamicSetter_1(toggleButton3, BindingPriority.Template, obj);
				toggleButton3.Bind(TemplatedControl.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				toggleButton3.Bind(TemplatedControl.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				toggleButton3.Bind(TemplatedControl.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				toggleButton3.Bind(TemplatedControl.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				toggleButton3.SetValue(InputElement.IsTabStopProperty, value: false, BindingPriority.Template);
				toggleButton3.Bind(TemplatedControl.ForegroundProperty, new TemplateBinding(TemplatedControl.ForegroundProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)toggleButton4).EndInit();
				Controls children3 = dataGridFrozenGrid.Children;
				StackPanel stackPanel;
				StackPanel stackPanel2 = (stackPanel = new StackPanel());
				((ISupportInitialize)stackPanel2).BeginInit();
				children3.Add(stackPanel2);
				StackPanel stackPanel3;
				StackPanel stackPanel4 = (stackPanel3 = stackPanel);
				context.PushParent(stackPanel3);
				stackPanel3.SetValue(Grid.ColumnProperty, 3, BindingPriority.Template);
				stackPanel3.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				stackPanel3.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				stackPanel3.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				Controls children4 = stackPanel3.Children;
				TextBlock textBlock;
				TextBlock textBlock2 = (textBlock = new TextBlock());
				((ISupportInitialize)textBlock2).BeginInit();
				children4.Add(textBlock2);
				textBlock.Name = "PART_PropertyNameElement";
				service = textBlock;
				context.AvaloniaNameScope.Register("PART_PropertyNameElement", service);
				textBlock.SetValue(Layoutable.MarginProperty, new Thickness(4.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				textBlock.Bind(Visual.IsVisibleProperty, new TemplateBinding(DataGridRowGroupHeader.IsPropertyNameVisibleProperty).ProvideValue());
				textBlock.Bind(TextBlock.ForegroundProperty, new TemplateBinding(TemplatedControl.ForegroundProperty).ProvideValue());
				((ISupportInitialize)textBlock).EndInit();
				Controls children5 = stackPanel3.Children;
				TextBlock textBlock3;
				TextBlock textBlock4 = (textBlock3 = new TextBlock());
				((ISupportInitialize)textBlock4).BeginInit();
				children5.Add(textBlock4);
				TextBlock textBlock5;
				TextBlock textBlock6 = (textBlock5 = textBlock3);
				context.PushParent(textBlock5);
				textBlock5.SetValue(Layoutable.MarginProperty, new Thickness(4.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				StyledProperty<string?> textProperty = TextBlock.TextProperty;
				CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(XamlIlHelpers.Avalonia_002ECollections_002EDataGridCollectionViewGroup_002CAvalonia_002EControls_002EDataGrid_002EKey_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = TextBlock.TextProperty;
				CompiledBindingExtension binding = compiledBindingExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock5.Bind(textProperty, binding);
				textBlock5.Bind(TextBlock.ForegroundProperty, new TemplateBinding(TemplatedControl.ForegroundProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock6).EndInit();
				Controls children6 = stackPanel3.Children;
				TextBlock textBlock7;
				TextBlock textBlock8 = (textBlock7 = new TextBlock());
				((ISupportInitialize)textBlock8).BeginInit();
				children6.Add(textBlock8);
				textBlock7.Name = "PART_ItemCountElement";
				service = textBlock7;
				context.AvaloniaNameScope.Register("PART_ItemCountElement", service);
				textBlock7.SetValue(Layoutable.MarginProperty, new Thickness(4.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				textBlock7.Bind(Visual.IsVisibleProperty, new TemplateBinding(DataGridRowGroupHeader.IsItemCountVisibleProperty).ProvideValue());
				textBlock7.Bind(TextBlock.ForegroundProperty, new TemplateBinding(TemplatedControl.ForegroundProperty).ProvideValue());
				((ISupportInitialize)textBlock7).EndInit();
				context.PopParent();
				((ISupportInitialize)stackPanel4).EndInit();
				Controls children7 = dataGridFrozenGrid.Children;
				Rectangle rectangle3;
				Rectangle rectangle4 = (rectangle3 = new Rectangle());
				((ISupportInitialize)rectangle4).BeginInit();
				children7.Add(rectangle4);
				Rectangle rectangle5;
				Rectangle rectangle6 = (rectangle5 = rectangle3);
				context.PushParent(rectangle5);
				Rectangle rectangle7 = rectangle5;
				rectangle7.Name = "CurrencyVisual";
				service = rectangle7;
				context.AvaloniaNameScope.Register("CurrencyVisual", service);
				rectangle7.SetValue(Grid.ColumnSpanProperty, 5, BindingPriority.Template);
				rectangle7.SetValue(Visual.IsVisibleProperty, value: false, BindingPriority.Template);
				rectangle7.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle7.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				rectangle7.SetValue(Shape.FillProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				rectangle7.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				StyledProperty<IBrush?> strokeProperty = Shape.StrokeProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("DataGridCurrencyVisualPrimaryBrush");
				context.ProvideTargetProperty = Shape.StrokeProperty;
				IBinding binding2 = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle7.Bind(strokeProperty, binding2);
				rectangle7.SetValue(Shape.StrokeThicknessProperty, 1.0, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)rectangle6).EndInit();
				Controls children8 = dataGridFrozenGrid.Children;
				Grid grid;
				Grid grid2 = (grid = new Grid());
				((ISupportInitialize)grid2).BeginInit();
				children8.Add(grid2);
				Grid grid3;
				Grid grid4 = (grid3 = grid);
				context.PushParent(grid3);
				grid3.Name = "FocusVisual";
				service = grid3;
				context.AvaloniaNameScope.Register("FocusVisual", service);
				grid3.SetValue(Grid.ColumnSpanProperty, 5, BindingPriority.Template);
				grid3.SetValue(Visual.IsVisibleProperty, value: false, BindingPriority.Template);
				grid3.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				Controls children9 = grid3.Children;
				Rectangle rectangle8;
				Rectangle rectangle9 = (rectangle8 = new Rectangle());
				((ISupportInitialize)rectangle9).BeginInit();
				children9.Add(rectangle9);
				Rectangle rectangle10 = (rectangle5 = rectangle8);
				context.PushParent(rectangle5);
				Rectangle rectangle11 = rectangle5;
				rectangle11.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle11.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				rectangle11.SetValue(Shape.FillProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				rectangle11.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				StyledProperty<IBrush?> strokeProperty2 = Shape.StrokeProperty;
				DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("DataGridCellFocusVisualPrimaryBrush");
				context.ProvideTargetProperty = Shape.StrokeProperty;
				IBinding binding3 = dynamicResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle11.Bind(strokeProperty2, binding3);
				rectangle11.SetValue(Shape.StrokeThicknessProperty, 2.0, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)rectangle10).EndInit();
				Controls children10 = grid3.Children;
				Rectangle rectangle12;
				Rectangle rectangle13 = (rectangle12 = new Rectangle());
				((ISupportInitialize)rectangle13).BeginInit();
				children10.Add(rectangle13);
				Rectangle rectangle14 = (rectangle5 = rectangle12);
				context.PushParent(rectangle5);
				Rectangle rectangle15 = rectangle5;
				rectangle15.SetValue(Layoutable.MarginProperty, new Thickness(2.0, 2.0, 2.0, 2.0), BindingPriority.Template);
				rectangle15.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle15.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				rectangle15.SetValue(Shape.FillProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				rectangle15.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				StyledProperty<IBrush?> strokeProperty3 = Shape.StrokeProperty;
				DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("DataGridCellFocusVisualSecondaryBrush");
				context.ProvideTargetProperty = Shape.StrokeProperty;
				IBinding binding4 = dynamicResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle15.Bind(strokeProperty3, binding4);
				rectangle15.SetValue(Shape.StrokeThicknessProperty, 1.0, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)rectangle14).EndInit();
				context.PopParent();
				((ISupportInitialize)grid4).EndInit();
				Controls children11 = dataGridFrozenGrid.Children;
				DataGridRowHeader dataGridRowHeader;
				DataGridRowHeader dataGridRowHeader2 = (dataGridRowHeader = new DataGridRowHeader());
				((ISupportInitialize)dataGridRowHeader2).BeginInit();
				children11.Add(dataGridRowHeader2);
				dataGridRowHeader.Name = "PART_RowHeader";
				service = dataGridRowHeader;
				context.AvaloniaNameScope.Register("PART_RowHeader", service);
				dataGridRowHeader.SetValue(Grid.RowSpanProperty, 2, BindingPriority.Template);
				dataGridRowHeader.SetValue(DataGridFrozenGrid.IsFrozenProperty, value: true, BindingPriority.Template);
				((ISupportInitialize)dataGridRowHeader).EndInit();
				Controls children12 = dataGridFrozenGrid.Children;
				Rectangle rectangle16;
				Rectangle rectangle17 = (rectangle16 = new Rectangle());
				((ISupportInitialize)rectangle17).BeginInit();
				children12.Add(rectangle17);
				rectangle16.Name = "PART_BottomGridLine";
				service = rectangle16;
				context.AvaloniaNameScope.Register("PART_BottomGridLine", service);
				rectangle16.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
				rectangle16.SetValue(Grid.ColumnSpanProperty, 5, BindingPriority.Template);
				rectangle16.SetValue(Layoutable.HeightProperty, 1.0, BindingPriority.Template);
				((ISupportInitialize)rectangle16).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(DataGridRowGroupHeader);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = TemplatedControl.ForegroundProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("DataGridRowGroupHeaderForegroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Setter setter4 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter5 = setter;
			setter5.Property = TemplatedControl.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("DataGridRowGroupHeaderBackgroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value2 = dynamicResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter5.Value = value2;
			context.PopParent();
			controlTheme.Add(setter4);
			Setter setter6 = new Setter();
			setter6.Property = TemplatedControl.FontSizeProperty;
			setter6.Value = 15.0;
			controlTheme.Add(setter6);
			Setter setter7 = new Setter();
			setter7.Property = Layoutable.MinHeightProperty;
			setter7.Value = 32.0;
			controlTheme.Add(setter7);
			Setter setter8 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter9 = setter;
			setter9.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value3 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_68.Build, context);
			context.PopParent();
			setter9.Value = value3;
			context.PopParent();
			controlTheme.Add(setter8);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_70
	{
		private class XamlClosure_71
		{
			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
				if (P_0 != null)
				{
					object service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (Styles)service;
					}
				}
				context.IntermediateRoot = new Rectangle();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				Rectangle rectangle = (Rectangle)intermediateRoot;
				context.PushParent(rectangle);
				StyledProperty<IBrush?> fillProperty = Shape.FillProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("DataGridDropLocationIndicatorBackground");
				context.ProvideTargetProperty = Shape.FillProperty;
				IBinding binding = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle.Bind(fillProperty, binding);
				rectangle.Width = 2.0;
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		private class XamlClosure_72
		{
			private class DynamicSetters_73
			{
				public static void DynamicSetter_1(StyledElement P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(StyledElement.ThemeProperty, binding);
						return;
					}
					if (P_2 is ControlTheme)
					{
						ControlTheme value = (ControlTheme)P_2;
						int priority = (int)P_1;
						P_0.SetValue(StyledElement.ThemeProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						ControlTheme value = (ControlTheme)P_2;
						int priority = (int)P_1;
						P_0.SetValue(StyledElement.ThemeProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (Styles)service;
					}
				}
				context.IntermediateRoot = new Border();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				Border border = (Border)intermediateRoot;
				context.PushParent(border);
				Border border2 = border;
				border2.Name = "DataGridBorder";
				service = border2;
				context.AvaloniaNameScope.Register("DataGridBorder", service);
				border2.Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				border2.Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				border2.Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				border2.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				Grid grid;
				Grid grid2 = (grid = new Grid());
				((ISupportInitialize)grid2).BeginInit();
				border2.Child = grid2;
				Grid grid3;
				Grid grid4 = (grid3 = grid);
				context.PushParent(grid3);
				Grid grid5 = grid3;
				ColumnDefinitions columnDefinitions = new ColumnDefinitions();
				columnDefinitions.Capacity = 3;
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				grid5.ColumnDefinitions = columnDefinitions;
				RowDefinitions rowDefinitions = new RowDefinitions();
				rowDefinitions.Capacity = 4;
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				rowDefinitions.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				grid5.RowDefinitions = rowDefinitions;
				grid5.SetValue(Visual.ClipToBoundsProperty, value: true, BindingPriority.Template);
				Controls children = grid5.Children;
				DataGridColumnHeader dataGridColumnHeader;
				DataGridColumnHeader dataGridColumnHeader2 = (dataGridColumnHeader = new DataGridColumnHeader());
				((ISupportInitialize)dataGridColumnHeader2).BeginInit();
				children.Add(dataGridColumnHeader2);
				DataGridColumnHeader dataGridColumnHeader3;
				DataGridColumnHeader dataGridColumnHeader4 = (dataGridColumnHeader3 = dataGridColumnHeader);
				context.PushParent(dataGridColumnHeader3);
				dataGridColumnHeader3.Name = "PART_TopLeftCornerHeader";
				service = dataGridColumnHeader3;
				context.AvaloniaNameScope.Register("PART_TopLeftCornerHeader", service);
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("DataGridTopLeftColumnHeader");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_73.DynamicSetter_1(dataGridColumnHeader3, BindingPriority.Template, obj);
				context.PopParent();
				((ISupportInitialize)dataGridColumnHeader4).EndInit();
				Controls children2 = grid5.Children;
				DataGridColumnHeadersPresenter dataGridColumnHeadersPresenter;
				DataGridColumnHeadersPresenter dataGridColumnHeadersPresenter2 = (dataGridColumnHeadersPresenter = new DataGridColumnHeadersPresenter());
				((ISupportInitialize)dataGridColumnHeadersPresenter2).BeginInit();
				children2.Add(dataGridColumnHeadersPresenter2);
				dataGridColumnHeadersPresenter.Name = "PART_ColumnHeadersPresenter";
				service = dataGridColumnHeadersPresenter;
				context.AvaloniaNameScope.Register("PART_ColumnHeadersPresenter", service);
				dataGridColumnHeadersPresenter.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				dataGridColumnHeadersPresenter.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				dataGridColumnHeadersPresenter.SetValue(Grid.ColumnSpanProperty, 2, BindingPriority.Template);
				((ISupportInitialize)dataGridColumnHeadersPresenter).EndInit();
				Controls children3 = grid5.Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children3.Add(rectangle2);
				Rectangle rectangle3;
				Rectangle rectangle4 = (rectangle3 = rectangle);
				context.PushParent(rectangle3);
				Rectangle rectangle5 = rectangle3;
				rectangle5.Name = "PART_ColumnHeadersAndRowsSeparator";
				service = rectangle5;
				context.AvaloniaNameScope.Register("PART_ColumnHeadersAndRowsSeparator", service);
				rectangle5.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				rectangle5.SetValue(Grid.ColumnSpanProperty, 3, BindingPriority.Template);
				rectangle5.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				rectangle5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Bottom, BindingPriority.Template);
				rectangle5.SetValue(Layoutable.HeightProperty, 1.0, BindingPriority.Template);
				StyledProperty<IBrush?> fillProperty = Shape.FillProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("DataGridGridLinesBrush");
				context.ProvideTargetProperty = Shape.FillProperty;
				IBinding binding = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle5.Bind(fillProperty, binding);
				context.PopParent();
				((ISupportInitialize)rectangle4).EndInit();
				Controls children4 = grid5.Children;
				DataGridRowsPresenter dataGridRowsPresenter;
				DataGridRowsPresenter dataGridRowsPresenter2 = (dataGridRowsPresenter = new DataGridRowsPresenter());
				((ISupportInitialize)dataGridRowsPresenter2).BeginInit();
				children4.Add(dataGridRowsPresenter2);
				dataGridRowsPresenter.Name = "PART_RowsPresenter";
				service = dataGridRowsPresenter;
				context.AvaloniaNameScope.Register("PART_RowsPresenter", service);
				dataGridRowsPresenter.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
				dataGridRowsPresenter.SetValue(Grid.RowSpanProperty, 2, BindingPriority.Template);
				dataGridRowsPresenter.SetValue(Grid.ColumnSpanProperty, 3, BindingPriority.Template);
				dataGridRowsPresenter.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				GestureRecognizerCollection gestureRecognizers = dataGridRowsPresenter.GestureRecognizers;
				ScrollGestureRecognizer scrollGestureRecognizer = new ScrollGestureRecognizer();
				((ISupportInitialize)scrollGestureRecognizer).BeginInit();
				scrollGestureRecognizer.CanHorizontallyScroll = true;
				scrollGestureRecognizer.CanVerticallyScroll = true;
				((ISupportInitialize)scrollGestureRecognizer).EndInit();
				gestureRecognizers.Add(scrollGestureRecognizer);
				((ISupportInitialize)dataGridRowsPresenter).EndInit();
				Controls children5 = grid5.Children;
				Rectangle rectangle6;
				Rectangle rectangle7 = (rectangle6 = new Rectangle());
				((ISupportInitialize)rectangle7).BeginInit();
				children5.Add(rectangle7);
				Rectangle rectangle8 = (rectangle3 = rectangle6);
				context.PushParent(rectangle3);
				Rectangle rectangle9 = rectangle3;
				rectangle9.Name = "PART_BottomRightCorner";
				service = rectangle9;
				context.AvaloniaNameScope.Register("PART_BottomRightCorner", service);
				StyledProperty<IBrush?> fillProperty2 = Shape.FillProperty;
				DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("DataGridScrollBarsSeparatorBackground");
				context.ProvideTargetProperty = Shape.FillProperty;
				IBinding binding2 = dynamicResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle9.Bind(fillProperty2, binding2);
				rectangle9.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				rectangle9.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)rectangle8).EndInit();
				Controls children6 = grid5.Children;
				ScrollBar scrollBar;
				ScrollBar scrollBar2 = (scrollBar = new ScrollBar());
				((ISupportInitialize)scrollBar2).BeginInit();
				children6.Add(scrollBar2);
				ScrollBar scrollBar3;
				ScrollBar scrollBar4 = (scrollBar3 = scrollBar);
				context.PushParent(scrollBar3);
				ScrollBar scrollBar5 = scrollBar3;
				scrollBar5.Name = "PART_VerticalScrollbar";
				service = scrollBar5;
				context.AvaloniaNameScope.Register("PART_VerticalScrollbar", service);
				scrollBar5.SetValue(ScrollBar.OrientationProperty, Orientation.Vertical, BindingPriority.Template);
				scrollBar5.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				scrollBar5.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
				StyledProperty<double> widthProperty = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ScrollBarSize");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding3 = dynamicResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				scrollBar5.Bind(widthProperty, binding3);
				context.PopParent();
				((ISupportInitialize)scrollBar4).EndInit();
				Controls children7 = grid5.Children;
				Grid grid6;
				Grid grid7 = (grid6 = new Grid());
				((ISupportInitialize)grid7).BeginInit();
				children7.Add(grid7);
				Grid grid8 = (grid3 = grid6);
				context.PushParent(grid3);
				Grid grid9 = grid3;
				grid9.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				grid9.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				ColumnDefinitions columnDefinitions2 = new ColumnDefinitions();
				columnDefinitions2.Capacity = 2;
				columnDefinitions2.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions2.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid9.ColumnDefinitions = columnDefinitions2;
				Controls children8 = grid9.Children;
				Rectangle rectangle10;
				Rectangle rectangle11 = (rectangle10 = new Rectangle());
				((ISupportInitialize)rectangle11).BeginInit();
				children8.Add(rectangle11);
				rectangle10.Name = "PART_FrozenColumnScrollBarSpacer";
				service = rectangle10;
				context.AvaloniaNameScope.Register("PART_FrozenColumnScrollBarSpacer", service);
				((ISupportInitialize)rectangle10).EndInit();
				Controls children9 = grid9.Children;
				ScrollBar scrollBar6;
				ScrollBar scrollBar7 = (scrollBar6 = new ScrollBar());
				((ISupportInitialize)scrollBar7).BeginInit();
				children9.Add(scrollBar7);
				ScrollBar scrollBar8 = (scrollBar3 = scrollBar6);
				context.PushParent(scrollBar3);
				ScrollBar scrollBar9 = scrollBar3;
				scrollBar9.Name = "PART_HorizontalScrollbar";
				service = scrollBar9;
				context.AvaloniaNameScope.Register("PART_HorizontalScrollbar", service);
				scrollBar9.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				scrollBar9.SetValue(ScrollBar.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				StyledProperty<double> heightProperty = Layoutable.HeightProperty;
				DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("ScrollBarSize");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				IBinding binding4 = dynamicResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				scrollBar9.Bind(heightProperty, binding4);
				context.PopParent();
				((ISupportInitialize)scrollBar8).EndInit();
				context.PopParent();
				((ISupportInitialize)grid8).EndInit();
				Controls children10 = grid5.Children;
				Border border3;
				Border border4 = (border3 = new Border());
				((ISupportInitialize)border4).BeginInit();
				children10.Add(border4);
				Border border5 = (border = border3);
				context.PushParent(border);
				Border border6 = border;
				border6.Name = "PART_DisabledVisualElement";
				service = border6;
				context.AvaloniaNameScope.Register("PART_DisabledVisualElement", service);
				border6.SetValue(Grid.ColumnSpanProperty, 3, BindingPriority.Template);
				border6.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border6.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				border6.SetValue(Grid.RowSpanProperty, 4, BindingPriority.Template);
				border6.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				border6.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border6.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				border6.SetValue(Border.CornerRadiusProperty, new CornerRadius(2.0, 2.0, 2.0, 2.0), BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("DataGridDisabledVisualElementBackground");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding5 = dynamicResourceExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border6.Bind(backgroundProperty, binding5);
				StyledProperty<bool> isVisibleProperty = Visual.IsVisibleProperty;
				CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Not().Ancestor(typeof(DataGrid), 0).Property(InputElement.IsEnabledProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build());
				context.ProvideTargetProperty = Visual.IsVisibleProperty;
				CompiledBindingExtension binding6 = compiledBindingExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border6.Bind(isVisibleProperty, binding6);
				context.PopParent();
				((ISupportInitialize)border5).EndInit();
				context.PopParent();
				((ISupportInitialize)grid4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(DataGrid);
			Setter setter = new Setter();
			setter.Property = DataGrid.RowBackgroundProperty;
			setter.Value = new ImmutableSolidColorBrush(16777215u);
			controlTheme.Add(setter);
			Setter setter2 = new Setter();
			setter2.Property = DataGrid.HeadersVisibilityProperty;
			setter2.Value = DataGridHeadersVisibility.Column;
			controlTheme.Add(setter2);
			Setter setter3 = new Setter();
			setter3.Property = DataGrid.HorizontalScrollBarVisibilityProperty;
			setter3.Value = ScrollBarVisibility.Auto;
			controlTheme.Add(setter3);
			Setter setter4 = new Setter();
			setter4.Property = DataGrid.VerticalScrollBarVisibilityProperty;
			setter4.Value = ScrollBarVisibility.Auto;
			controlTheme.Add(setter4);
			Setter setter5 = new Setter();
			setter5.Property = DataGrid.SelectionModeProperty;
			setter5.Value = DataGridSelectionMode.Extended;
			controlTheme.Add(setter5);
			Setter setter6 = new Setter();
			setter6.Property = DataGrid.GridLinesVisibilityProperty;
			setter6.Value = DataGridGridLinesVisibility.None;
			controlTheme.Add(setter6);
			Setter setter7;
			Setter setter8 = (setter7 = new Setter());
			context.PushParent(setter7);
			Setter setter9 = setter7;
			setter9.Property = DataGrid.HorizontalGridLinesBrushProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("DataGridGridLinesBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter9.Value = value;
			context.PopParent();
			controlTheme.Add(setter8);
			Setter setter10 = (setter7 = new Setter());
			context.PushParent(setter7);
			Setter setter11 = setter7;
			setter11.Property = DataGrid.VerticalGridLinesBrushProperty;
			DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("DataGridGridLinesBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value2 = dynamicResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter11.Value = value2;
			context.PopParent();
			controlTheme.Add(setter10);
			Setter setter12 = new Setter();
			setter12.Property = Control.FocusAdornerProperty;
			setter12.Value = null;
			controlTheme.Add(setter12);
			Setter setter13 = (setter7 = new Setter());
			context.PushParent(setter7);
			Setter setter14 = setter7;
			setter14.Property = DataGrid.DropLocationIndicatorTemplateProperty;
			Template template;
			Template value3 = (template = new Template());
			context.PushParent(template);
			template.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_71.Build, context);
			context.PopParent();
			setter14.Value = value3;
			context.PopParent();
			controlTheme.Add(setter13);
			Setter setter15 = (setter7 = new Setter());
			context.PushParent(setter7);
			Setter setter16 = setter7;
			setter16.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value4 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_72.Build, context);
			context.PopParent();
			setter16.Value = value4;
			context.PopParent();
			controlTheme.Add(setter15);
			Style style = new Style();
			style.Selector = ((Selector?)null).Nesting().Class(":empty-columns");
			Style style2 = new Style();
			style2.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(DataGridColumnHeader))
				.Name("PART_TopLeftCornerHeader");
			Setter setter17 = new Setter();
			setter17.Property = Visual.IsVisibleProperty;
			setter17.Value = false;
			style2.Add(setter17);
			style.Add(style2);
			Style style3 = new Style();
			style3.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(DataGridColumnHeadersPresenter))
				.Name("PART_ColumnHeadersPresenter");
			Setter setter18 = new Setter();
			setter18.Property = Visual.IsVisibleProperty;
			setter18.Value = false;
			style3.Add(setter18);
			style.Add(style3);
			Style style4 = new Style();
			style4.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(Rectangle))
				.Name("PART_ColumnHeadersAndRowsSeparator");
			Setter setter19 = new Setter();
			setter19.Property = Visual.IsVisibleProperty;
			setter19.Value = false;
			style4.Add(setter19);
			style.Add(style4);
			controlTheme.Add(style);
			context.PopParent();
			return result;
		}
	}

	public class NamespaceInfo_003A_002FThemes_002FSimple_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
	{
		private IReadOnlyDictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> _xmlNamespaces;

		public static IAvaloniaXamlIlXmlNamespaceInfoProvider Singleton;

		public virtual IReadOnlyDictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> XmlNamespaces
		{
			get
			{
				if (_xmlNamespaces == null)
				{
					_xmlNamespaces = CreateNamespaces();
				}
				return _xmlNamespaces;
			}
		}

		static AvaloniaXamlIlXmlNamespaceInfo CreateNamespaceInfo(string P_0, string P_1)
		{
			return new AvaloniaXamlIlXmlNamespaceInfo
			{
				ClrNamespace = P_0,
				ClrAssemblyName = P_1
			};
		}

		static IReadOnlyDictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> CreateNamespaces()
		{
			Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> dictionary = new Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>>(3);
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[34]
			{
				CreateNamespaceInfo("Avalonia", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Animation", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Animation.Easings", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Data", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Data.Converters", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Input", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Input.GestureRecognizers", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Input.TextInput", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Layout", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.LogicalTree", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Media", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Media.Imaging", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Media.Transformation", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Styling", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Automation", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Embedding", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Presenters", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Shapes", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Templates", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Notifications", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Chrome", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Documents", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Data", "Avalonia.Markup"),
				CreateNamespaceInfo("Avalonia.Markup.Data", "Avalonia.Markup"),
				CreateNamespaceInfo("Avalonia.Markup.Xaml.MarkupExtensions", "Avalonia.Markup.Xaml"),
				CreateNamespaceInfo("Avalonia.Markup.Xaml.Styling", "Avalonia.Markup.Xaml"),
				CreateNamespaceInfo("Avalonia.Markup.Xaml.Templates", "Avalonia.Markup.Xaml"),
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.DataGrid")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("collections", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Collections", (string)null) });
			return dictionary;
		}

		static NamespaceInfo_003A_002FThemes_002FSimple_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FThemes_002FSimple_002Examl();
		}
	}

	private class XamlClosure_74
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(TextBlock);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			setter.Property = Layoutable.MarginProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("DataGridTextColumnCellTextBlockMargin");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Setter setter3 = new Setter();
			setter3.Property = Layoutable.VerticalAlignmentProperty;
			setter3.Value = VerticalAlignment.Center;
			controlTheme.Add(setter3);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_75
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension(typeof(TextBox));
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Property();
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			controlTheme.BasedOn = (ControlTheme)obj;
			controlTheme.TargetType = typeof(TextBox);
			Setter setter = new Setter();
			setter.Property = Layoutable.VerticalAlignmentProperty;
			setter.Value = VerticalAlignment.Stretch;
			controlTheme.Add(setter);
			Setter setter2 = new Setter();
			setter2.Property = TemplatedControl.BackgroundProperty;
			setter2.Value = new ImmutableSolidColorBrush(16777215u);
			controlTheme.Add(setter2);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_76
	{
		private class XamlClosure_77
		{
			private class DynamicSetters_78
			{
				public static void DynamicSetter_1(ContentPresenter P_0, BindingPriority P_1, IBinding P_2)
				{
					if (P_2 != null)
					{
						IBinding binding = P_2;
						P_0.Bind(ContentPresenter.ContentProperty, binding);
					}
					else
					{
						object value = P_2;
						int priority = (int)P_1;
						P_0.SetValue(ContentPresenter.ContentProperty, value, (BindingPriority)priority);
					}
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (Styles)service;
					}
				}
				context.IntermediateRoot = new Border();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				((StyledElement)intermediateRoot).Name = "CellBorder";
				service = intermediateRoot;
				context.AvaloniaNameScope.Register("CellBorder", service);
				((AvaloniaObject)intermediateRoot).Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				Grid grid;
				Grid grid2 = (grid = new Grid());
				((ISupportInitialize)grid2).BeginInit();
				((Decorator)intermediateRoot).Child = grid2;
				ColumnDefinitions columnDefinitions = new ColumnDefinitions();
				columnDefinitions.Capacity = 2;
				columnDefinitions.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				grid.ColumnDefinitions = columnDefinitions;
				Controls children = grid.Children;
				ContentPresenter contentPresenter;
				ContentPresenter contentPresenter2 = (contentPresenter = new ContentPresenter());
				((ISupportInitialize)contentPresenter2).BeginInit();
				children.Add(contentPresenter2);
				contentPresenter.Bind(Layoutable.MarginProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				contentPresenter.Bind(Layoutable.HorizontalAlignmentProperty, new TemplateBinding(ContentControl.HorizontalContentAlignmentProperty).ProvideValue());
				contentPresenter.Bind(Layoutable.VerticalAlignmentProperty, new TemplateBinding(ContentControl.VerticalContentAlignmentProperty).ProvideValue());
				DynamicSetters_78.DynamicSetter_1(contentPresenter, BindingPriority.Template, new TemplateBinding(ContentControl.ContentProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.ContentTemplateProperty, new TemplateBinding(ContentControl.ContentTemplateProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.ForegroundProperty, new TemplateBinding(TemplatedControl.ForegroundProperty).ProvideValue());
				((ISupportInitialize)contentPresenter).EndInit();
				Controls children2 = grid.Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children2.Add(rectangle2);
				rectangle.Name = "PART_RightGridLine";
				service = rectangle;
				context.AvaloniaNameScope.Register("PART_RightGridLine", service);
				rectangle.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				rectangle.SetValue(Layoutable.WidthProperty, 1.0, BindingPriority.Template);
				rectangle.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				((ISupportInitialize)rectangle).EndInit();
				((ISupportInitialize)grid).EndInit();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme = new ControlTheme();
			controlTheme.TargetType = typeof(DataGridCell);
			Setter setter = new Setter();
			setter.Property = TemplatedControl.BackgroundProperty;
			setter.Value = new ImmutableSolidColorBrush(16777215u);
			controlTheme.Add(setter);
			Setter setter2 = new Setter();
			setter2.Property = ContentControl.HorizontalContentAlignmentProperty;
			setter2.Value = HorizontalAlignment.Stretch;
			controlTheme.Add(setter2);
			Setter setter3 = new Setter();
			setter3.Property = ContentControl.VerticalContentAlignmentProperty;
			setter3.Value = VerticalAlignment.Stretch;
			controlTheme.Add(setter3);
			Setter setter4 = new Setter();
			setter4.Property = TemplatedControl.TemplateProperty;
			setter4.Value = new ControlTemplate
			{
				Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_77.Build, context)
			};
			controlTheme.Add(setter4);
			return controlTheme;
		}
	}

	private class XamlClosure_79
	{
		private class XamlClosure_80
		{
			private class DynamicSetters_81
			{
				public static void DynamicSetter_1(ContentPresenter P_0, BindingPriority P_1, IBinding P_2)
				{
					if (P_2 != null)
					{
						IBinding binding = P_2;
						P_0.Bind(ContentPresenter.ContentProperty, binding);
					}
					else
					{
						object value = P_2;
						int priority = (int)P_1;
						P_0.SetValue(ContentPresenter.ContentProperty, value, (BindingPriority)priority);
					}
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (Styles)service;
					}
				}
				context.IntermediateRoot = new Border();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				((StyledElement)intermediateRoot).Name = "HeaderBorder";
				service = intermediateRoot;
				context.AvaloniaNameScope.Register("HeaderBorder", service);
				((AvaloniaObject)intermediateRoot).Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				Grid grid;
				Grid grid2 = (grid = new Grid());
				((ISupportInitialize)grid2).BeginInit();
				((Decorator)intermediateRoot).Child = grid2;
				ColumnDefinitions columnDefinitions = new ColumnDefinitions();
				columnDefinitions.Capacity = 2;
				columnDefinitions.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				grid.ColumnDefinitions = columnDefinitions;
				Controls children = grid.Children;
				Grid grid3;
				Grid grid4 = (grid3 = new Grid());
				((ISupportInitialize)grid4).BeginInit();
				children.Add(grid4);
				grid3.Bind(Layoutable.MarginProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				grid3.Bind(Layoutable.HorizontalAlignmentProperty, new TemplateBinding(ContentControl.HorizontalContentAlignmentProperty).ProvideValue());
				grid3.Bind(Layoutable.VerticalAlignmentProperty, new TemplateBinding(ContentControl.VerticalContentAlignmentProperty).ProvideValue());
				ColumnDefinitions columnDefinitions2 = new ColumnDefinitions();
				columnDefinitions2.Capacity = 2;
				columnDefinitions2.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions2.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				grid3.ColumnDefinitions = columnDefinitions2;
				Controls children2 = grid3.Children;
				ContentPresenter contentPresenter;
				ContentPresenter contentPresenter2 = (contentPresenter = new ContentPresenter());
				((ISupportInitialize)contentPresenter2).BeginInit();
				children2.Add(contentPresenter2);
				DynamicSetters_81.DynamicSetter_1(contentPresenter, BindingPriority.Template, new TemplateBinding(ContentControl.ContentProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.ContentTemplateProperty, new TemplateBinding(ContentControl.ContentTemplateProperty).ProvideValue());
				((ISupportInitialize)contentPresenter).EndInit();
				Controls children3 = grid3.Children;
				Path path;
				Path path2 = (path = new Path());
				((ISupportInitialize)path2).BeginInit();
				children3.Add(path2);
				path.Name = "SortIcon";
				service = path;
				context.AvaloniaNameScope.Register("SortIcon", service);
				path.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				path.SetValue(Layoutable.WidthProperty, 8.0, BindingPriority.Template);
				path.SetValue(Layoutable.MarginProperty, new Thickness(4.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				path.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left, BindingPriority.Template);
				path.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				path.SetValue(Path.DataProperty, Geometry.Parse("F1 M -5.215,6.099L 5.215,6.099L 0,0L -5.215,6.099 Z "), BindingPriority.Template);
				path.Bind(Shape.FillProperty, new TemplateBinding(TemplatedControl.ForegroundProperty).ProvideValue());
				path.SetValue(Visual.IsVisibleProperty, value: false, BindingPriority.Template);
				path.SetValue(Shape.StretchProperty, Stretch.Uniform, BindingPriority.Template);
				((ISupportInitialize)path).EndInit();
				((ISupportInitialize)grid3).EndInit();
				Controls children4 = grid.Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children4.Add(rectangle2);
				rectangle.Name = "VerticalSeparator";
				service = rectangle;
				context.AvaloniaNameScope.Register("VerticalSeparator", service);
				rectangle.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				rectangle.SetValue(Layoutable.WidthProperty, 1.0, BindingPriority.Template);
				rectangle.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				rectangle.Bind(Shape.FillProperty, new TemplateBinding(DataGridColumnHeader.SeparatorBrushProperty).ProvideValue());
				rectangle.Bind(Visual.IsVisibleProperty, new TemplateBinding(DataGridColumnHeader.AreSeparatorsVisibleProperty).ProvideValue());
				((ISupportInitialize)rectangle).EndInit();
				((ISupportInitialize)grid).EndInit();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(DataGridColumnHeader);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = TemplatedControl.ForegroundProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeForegroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Setter setter4 = new Setter();
			setter4.Property = ContentControl.HorizontalContentAlignmentProperty;
			setter4.Value = HorizontalAlignment.Left;
			controlTheme.Add(setter4);
			Setter setter5 = new Setter();
			setter5.Property = ContentControl.VerticalContentAlignmentProperty;
			setter5.Value = VerticalAlignment.Center;
			controlTheme.Add(setter5);
			Setter setter6 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter7 = setter;
			setter7.Property = DataGridColumnHeader.SeparatorBrushProperty;
			DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ThemeControlLowColor");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value2 = dynamicResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter7.Value = value2;
			context.PopParent();
			controlTheme.Add(setter6);
			Setter setter8 = new Setter();
			setter8.Property = TemplatedControl.PaddingProperty;
			setter8.Value = new Thickness(4.0, 4.0, 4.0, 4.0);
			controlTheme.Add(setter8);
			Setter setter9 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter10 = setter;
			setter10.Property = TemplatedControl.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ThemeControlMidBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value3 = dynamicResourceExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter10.Value = value3;
			context.PopParent();
			controlTheme.Add(setter9);
			Setter setter11 = new Setter();
			setter11.Property = TemplatedControl.TemplateProperty;
			setter11.Value = new ControlTemplate
			{
				Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_80.Build, context)
			};
			controlTheme.Add(setter11);
			Style style = new Style();
			style.Selector = ((Selector?)null).Nesting().Class(":focus-visible").Template()
				.OfType(typeof(Grid))
				.Name("FocusVisual");
			Setter setter12 = new Setter();
			setter12.Property = Visual.IsVisibleProperty;
			setter12.Value = true;
			style.Add(setter12);
			controlTheme.Add(style);
			Style style2;
			Style style3 = (style2 = new Style());
			context.PushParent(style2);
			Style style4 = style2;
			style4.Selector = ((Selector?)null).Nesting().Class(":pointerover").Template()
				.OfType(typeof(Grid))
				.Name("PART_ColumnHeaderRoot");
			Setter setter13 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter14 = setter;
			setter14.Property = Panel.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("DataGridColumnHeaderHoveredBackgroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value4 = dynamicResourceExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter14.Value = value4;
			context.PopParent();
			style4.Add(setter13);
			context.PopParent();
			controlTheme.Add(style3);
			Style style5 = (style2 = new Style());
			context.PushParent(style2);
			Style style6 = style2;
			style6.Selector = ((Selector?)null).Nesting().Class(":pressed").Template()
				.OfType(typeof(Grid))
				.Name("PART_ColumnHeaderRoot");
			Setter setter15 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter16 = setter;
			setter16.Property = Panel.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("DataGridColumnHeaderPressedBackgroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value5 = dynamicResourceExtension5.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter16.Value = value5;
			context.PopParent();
			style6.Add(setter15);
			context.PopParent();
			controlTheme.Add(style5);
			Style style7 = new Style();
			style7.Selector = ((Selector?)null).Nesting().Class(":dragIndicator");
			Setter setter17 = new Setter();
			setter17.Property = Visual.OpacityProperty;
			setter17.Value = 0.5;
			style7.Add(setter17);
			controlTheme.Add(style7);
			Style style8 = new Style();
			style8.Selector = ((Selector?)null).Nesting().Class(":sortascending").Template()
				.OfType(typeof(Path))
				.Name("SortIcon");
			Setter setter18 = new Setter();
			setter18.Property = Visual.IsVisibleProperty;
			setter18.Value = true;
			style8.Add(setter18);
			controlTheme.Add(style8);
			Style style9 = new Style();
			style9.Selector = ((Selector?)null).Nesting().Class(":sortdescending").Template()
				.OfType(typeof(Path))
				.Name("SortIcon");
			Setter setter19 = new Setter();
			setter19.Property = Visual.IsVisibleProperty;
			setter19.Value = true;
			style9.Add(setter19);
			Setter setter20 = new Setter();
			setter20.Property = Visual.RenderTransformProperty;
			ScaleTransform scaleTransform = new ScaleTransform();
			scaleTransform.SetValue(ScaleTransform.ScaleXProperty, 1.0, BindingPriority.Template);
			scaleTransform.SetValue(ScaleTransform.ScaleYProperty, -1.0, BindingPriority.Template);
			setter20.Value = scaleTransform;
			style9.Add(setter20);
			controlTheme.Add(style9);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_82
	{
		private class XamlClosure_83
		{
			private class DynamicSetters_84
			{
				public static void DynamicSetter_1(ContentPresenter P_0, BindingPriority P_1, IBinding P_2)
				{
					if (P_2 != null)
					{
						IBinding binding = P_2;
						P_0.Bind(ContentPresenter.ContentProperty, binding);
					}
					else
					{
						object value = P_2;
						int priority = (int)P_1;
						P_0.SetValue(ContentPresenter.ContentProperty, value, (BindingPriority)priority);
					}
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (Styles)service;
					}
				}
				context.IntermediateRoot = new Grid();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				((StyledElement)intermediateRoot).Name = "PART_Root";
				service = intermediateRoot;
				context.AvaloniaNameScope.Register("PART_Root", service);
				ColumnDefinitions columnDefinitions = new ColumnDefinitions();
				columnDefinitions.Capacity = 2;
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				((Grid)intermediateRoot).ColumnDefinitions = columnDefinitions;
				RowDefinitions rowDefinitions = new RowDefinitions();
				rowDefinitions.Capacity = 3;
				rowDefinitions.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				((Grid)intermediateRoot).RowDefinitions = rowDefinitions;
				Controls children = ((Panel)intermediateRoot).Children;
				Border border;
				Border border2 = (border = new Border());
				((ISupportInitialize)border2).BeginInit();
				children.Add(border2);
				border.SetValue(Grid.RowSpanProperty, 3, BindingPriority.Template);
				border.SetValue(Grid.ColumnSpanProperty, 2, BindingPriority.Template);
				border.Bind(Border.BorderBrushProperty, new TemplateBinding(DataGridRowHeader.SeparatorBrushProperty).ProvideValue());
				border.SetValue(Border.BorderThicknessProperty, new Thickness(0.0, 0.0, 1.0, 0.0), BindingPriority.Template);
				Grid grid;
				Grid grid2 = (grid = new Grid());
				((ISupportInitialize)grid2).BeginInit();
				border.Child = grid2;
				grid.Bind(Panel.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				Controls children2 = grid.Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children2.Add(rectangle2);
				rectangle.Name = "RowInvalidVisualElement";
				service = rectangle;
				context.AvaloniaNameScope.Register("RowInvalidVisualElement", service);
				rectangle.SetValue(Shape.StretchProperty, Stretch.Fill, BindingPriority.Template);
				((ISupportInitialize)rectangle).EndInit();
				Controls children3 = grid.Children;
				Rectangle rectangle3;
				Rectangle rectangle4 = (rectangle3 = new Rectangle());
				((ISupportInitialize)rectangle4).BeginInit();
				children3.Add(rectangle4);
				rectangle3.Name = "BackgroundRectangle";
				service = rectangle3;
				context.AvaloniaNameScope.Register("BackgroundRectangle", service);
				rectangle3.SetValue(Shape.StretchProperty, Stretch.Fill, BindingPriority.Template);
				((ISupportInitialize)rectangle3).EndInit();
				((ISupportInitialize)grid).EndInit();
				((ISupportInitialize)border).EndInit();
				Controls children4 = ((Panel)intermediateRoot).Children;
				Rectangle rectangle5;
				Rectangle rectangle6 = (rectangle5 = new Rectangle());
				((ISupportInitialize)rectangle6).BeginInit();
				children4.Add(rectangle6);
				rectangle5.Name = "HorizontalSeparator";
				service = rectangle5;
				context.AvaloniaNameScope.Register("HorizontalSeparator", service);
				rectangle5.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				rectangle5.SetValue(Grid.ColumnSpanProperty, 2, BindingPriority.Template);
				rectangle5.SetValue(Layoutable.HeightProperty, 1.0, BindingPriority.Template);
				rectangle5.SetValue(Layoutable.MarginProperty, new Thickness(1.0, 0.0, 1.0, 0.0), BindingPriority.Template);
				rectangle5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle5.Bind(Shape.FillProperty, new TemplateBinding(DataGridRowHeader.SeparatorBrushProperty).ProvideValue());
				rectangle5.Bind(Visual.IsVisibleProperty, new TemplateBinding(DataGridRowHeader.AreSeparatorsVisibleProperty).ProvideValue());
				((ISupportInitialize)rectangle5).EndInit();
				Controls children5 = ((Panel)intermediateRoot).Children;
				ContentPresenter contentPresenter;
				ContentPresenter contentPresenter2 = (contentPresenter = new ContentPresenter());
				((ISupportInitialize)contentPresenter2).BeginInit();
				children5.Add(contentPresenter2);
				contentPresenter.SetValue(Grid.RowSpanProperty, 2, BindingPriority.Template);
				contentPresenter.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				contentPresenter.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				contentPresenter.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				DynamicSetters_84.DynamicSetter_1(contentPresenter, BindingPriority.Template, new TemplateBinding(ContentControl.ContentProperty).ProvideValue());
				((ISupportInitialize)contentPresenter).EndInit();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme = new ControlTheme();
			controlTheme.TargetType = typeof(DataGridRowHeader);
			Setter setter = new Setter();
			setter.Property = TemplatedControl.TemplateProperty;
			setter.Value = new ControlTemplate
			{
				Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_83.Build, context)
			};
			controlTheme.Add(setter);
			return controlTheme;
		}
	}

	private class XamlClosure_85
	{
		private class XamlClosure_86
		{
			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (Styles)service;
					}
				}
				context.IntermediateRoot = new Border();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				((StyledElement)intermediateRoot).Name = "RowBorder";
				service = intermediateRoot;
				context.AvaloniaNameScope.Register("RowBorder", service);
				((AvaloniaObject)intermediateRoot).Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				DataGridFrozenGrid dataGridFrozenGrid;
				DataGridFrozenGrid dataGridFrozenGrid2 = (dataGridFrozenGrid = new DataGridFrozenGrid());
				((ISupportInitialize)dataGridFrozenGrid2).BeginInit();
				((Decorator)intermediateRoot).Child = dataGridFrozenGrid2;
				dataGridFrozenGrid.Name = "PART_Root";
				service = dataGridFrozenGrid;
				context.AvaloniaNameScope.Register("PART_Root", service);
				ColumnDefinitions columnDefinitions = new ColumnDefinitions();
				columnDefinitions.Capacity = 2;
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				dataGridFrozenGrid.ColumnDefinitions = columnDefinitions;
				RowDefinitions rowDefinitions = new RowDefinitions();
				rowDefinitions.Capacity = 3;
				rowDefinitions.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				dataGridFrozenGrid.RowDefinitions = rowDefinitions;
				Controls children = dataGridFrozenGrid.Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children.Add(rectangle2);
				rectangle.Name = "BackgroundRectangle";
				service = rectangle;
				context.AvaloniaNameScope.Register("BackgroundRectangle", service);
				rectangle.SetValue(Grid.RowSpanProperty, 2, BindingPriority.Template);
				rectangle.SetValue(Grid.ColumnSpanProperty, 2, BindingPriority.Template);
				((ISupportInitialize)rectangle).EndInit();
				Controls children2 = dataGridFrozenGrid.Children;
				DataGridRowHeader dataGridRowHeader;
				DataGridRowHeader dataGridRowHeader2 = (dataGridRowHeader = new DataGridRowHeader());
				((ISupportInitialize)dataGridRowHeader2).BeginInit();
				children2.Add(dataGridRowHeader2);
				dataGridRowHeader.Name = "PART_RowHeader";
				service = dataGridRowHeader;
				context.AvaloniaNameScope.Register("PART_RowHeader", service);
				dataGridRowHeader.SetValue(Grid.RowSpanProperty, 3, BindingPriority.Template);
				dataGridRowHeader.SetValue(DataGridFrozenGrid.IsFrozenProperty, value: true, BindingPriority.Template);
				((ISupportInitialize)dataGridRowHeader).EndInit();
				Controls children3 = dataGridFrozenGrid.Children;
				DataGridCellsPresenter dataGridCellsPresenter;
				DataGridCellsPresenter dataGridCellsPresenter2 = (dataGridCellsPresenter = new DataGridCellsPresenter());
				((ISupportInitialize)dataGridCellsPresenter2).BeginInit();
				children3.Add(dataGridCellsPresenter2);
				dataGridCellsPresenter.Name = "PART_CellsPresenter";
				service = dataGridCellsPresenter;
				context.AvaloniaNameScope.Register("PART_CellsPresenter", service);
				dataGridCellsPresenter.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				dataGridCellsPresenter.SetValue(DataGridFrozenGrid.IsFrozenProperty, value: true, BindingPriority.Template);
				((ISupportInitialize)dataGridCellsPresenter).EndInit();
				Controls children4 = dataGridFrozenGrid.Children;
				DataGridDetailsPresenter dataGridDetailsPresenter;
				DataGridDetailsPresenter dataGridDetailsPresenter2 = (dataGridDetailsPresenter = new DataGridDetailsPresenter());
				((ISupportInitialize)dataGridDetailsPresenter2).BeginInit();
				children4.Add(dataGridDetailsPresenter2);
				dataGridDetailsPresenter.Name = "PART_DetailsPresenter";
				service = dataGridDetailsPresenter;
				context.AvaloniaNameScope.Register("PART_DetailsPresenter", service);
				dataGridDetailsPresenter.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
				dataGridDetailsPresenter.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				((ISupportInitialize)dataGridDetailsPresenter).EndInit();
				Controls children5 = dataGridFrozenGrid.Children;
				Rectangle rectangle3;
				Rectangle rectangle4 = (rectangle3 = new Rectangle());
				((ISupportInitialize)rectangle4).BeginInit();
				children5.Add(rectangle4);
				rectangle3.Name = "PART_BottomGridLine";
				service = rectangle3;
				context.AvaloniaNameScope.Register("PART_BottomGridLine", service);
				rectangle3.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				rectangle3.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				rectangle3.SetValue(Layoutable.HeightProperty, 1.0, BindingPriority.Template);
				rectangle3.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				((ISupportInitialize)rectangle3).EndInit();
				((ISupportInitialize)dataGridFrozenGrid).EndInit();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(DataGridRow);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = TemplatedControl.BackgroundProperty;
			CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(DataGrid), 0).Property(DataGrid.RowBackgroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			CompiledBindingExtension value = compiledBindingExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Setter setter4 = new Setter();
			setter4.Property = TemplatedControl.TemplateProperty;
			setter4.Value = new ControlTemplate
			{
				Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_86.Build, context)
			};
			controlTheme.Add(setter4);
			Style style;
			Style style2 = (style = new Style());
			context.PushParent(style);
			Style style3 = style;
			style3.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(Rectangle))
				.Name("BackgroundRectangle");
			Setter setter5 = new Setter();
			setter5.Property = Visual.IsVisibleProperty;
			setter5.Value = false;
			style3.Add(setter5);
			Setter setter6 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter7 = setter;
			setter7.Property = Shape.FillProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("HighlightBrush2");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value2 = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter7.Value = value2;
			context.PopParent();
			style3.Add(setter6);
			context.PopParent();
			controlTheme.Add(style2);
			Style style4 = new Style();
			style4.Selector = ((Selector?)null).Nesting().Class(":pointerover").Template()
				.OfType(typeof(Rectangle))
				.Name("BackgroundRectangle");
			Setter setter8 = new Setter();
			setter8.Property = Visual.IsVisibleProperty;
			setter8.Value = true;
			style4.Add(setter8);
			Setter setter9 = new Setter();
			setter9.Property = Visual.OpacityProperty;
			setter9.Value = 0.5;
			style4.Add(setter9);
			controlTheme.Add(style4);
			Style style5 = new Style();
			style5.Selector = ((Selector?)null).Nesting().Class(":selected").Template()
				.OfType(typeof(Rectangle))
				.Name("BackgroundRectangle");
			Setter setter10 = new Setter();
			setter10.Property = Visual.IsVisibleProperty;
			setter10.Value = true;
			style5.Add(setter10);
			Setter setter11 = new Setter();
			setter11.Property = Visual.OpacityProperty;
			setter11.Value = 1.0;
			style5.Add(setter11);
			controlTheme.Add(style5);
			Style style6 = (style = new Style());
			context.PushParent(style);
			Style style7 = style;
			style7.Selector = ((Selector?)null).Nesting().Class(":selected");
			Setter setter12 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter13 = setter;
			setter13.Property = TemplatedControl.ForegroundProperty;
			DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("HighlightForegroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value3 = dynamicResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter13.Value = value3;
			context.PopParent();
			style7.Add(setter12);
			context.PopParent();
			controlTheme.Add(style6);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_87
	{
		private class XamlClosure_88
		{
			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml");
				if (P_0 != null)
				{
					object service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (Styles)service;
					}
				}
				context.IntermediateRoot = new Border();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				((AvaloniaObject)intermediateRoot).SetValue((StyledProperty<int>)Grid.ColumnProperty, 0, BindingPriority.Template);
				((AvaloniaObject)intermediateRoot).SetValue(Layoutable.WidthProperty, 20.0, BindingPriority.Template);
				((AvaloniaObject)intermediateRoot).SetValue(Layoutable.HeightProperty, 20.0, BindingPriority.Template);
				((AvaloniaObject)intermediateRoot).SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				((AvaloniaObject)intermediateRoot).SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				((AvaloniaObject)intermediateRoot).SetValue(Border.BackgroundProperty, (IBrush)new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				Path path;
				Path path2 = (path = new Path());
				((ISupportInitialize)path2).BeginInit();
				((Decorator)intermediateRoot).Child = path2;
				path.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				path.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				path.SetValue(Path.DataProperty, Geometry.Parse("M 0 2 L 4 6 L 0 10 Z"), BindingPriority.Template);
				path.Bind(Shape.FillProperty, new TemplateBinding(TemplatedControl.ForegroundProperty).ProvideValue());
				((ISupportInitialize)path).EndInit();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme = new ControlTheme();
			controlTheme.TargetType = typeof(ToggleButton);
			Setter setter = new Setter();
			setter.Property = TemplatedControl.TemplateProperty;
			setter.Value = new ControlTemplate
			{
				Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_88.Build, context)
			};
			controlTheme.Add(setter);
			Style style = new Style();
			style.Selector = ((Selector?)null).Nesting().Class(":checked").Template()
				.OfType(typeof(Path));
			Setter setter2 = new Setter();
			setter2.Property = Visual.RenderTransformProperty;
			RotateTransform rotateTransform = new RotateTransform();
			rotateTransform.SetValue(RotateTransform.AngleProperty, 90.0, BindingPriority.Template);
			setter2.Value = rotateTransform;
			style.Add(setter2);
			controlTheme.Add(style);
			return controlTheme;
		}
	}

	private class XamlClosure_89
	{
		private class XamlClosure_90
		{
			private class DynamicSetters_91
			{
				public static void DynamicSetter_1(StyledElement P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(StyledElement.ThemeProperty, binding);
						return;
					}
					if (P_2 is ControlTheme)
					{
						ControlTheme value = (ControlTheme)P_2;
						int priority = (int)P_1;
						P_0.SetValue(StyledElement.ThemeProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						ControlTheme value = (ControlTheme)P_2;
						int priority = (int)P_1;
						P_0.SetValue(StyledElement.ThemeProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (Styles)service;
					}
				}
				context.IntermediateRoot = new DataGridFrozenGrid();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				DataGridFrozenGrid dataGridFrozenGrid = (DataGridFrozenGrid)intermediateRoot;
				context.PushParent(dataGridFrozenGrid);
				dataGridFrozenGrid.Name = "Root";
				service = dataGridFrozenGrid;
				context.AvaloniaNameScope.Register("Root", service);
				ColumnDefinitions columnDefinitions = new ColumnDefinitions();
				columnDefinitions.Capacity = 4;
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				dataGridFrozenGrid.ColumnDefinitions = columnDefinitions;
				RowDefinitions rowDefinitions = new RowDefinitions();
				rowDefinitions.Capacity = 3;
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				rowDefinitions.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				dataGridFrozenGrid.RowDefinitions = rowDefinitions;
				Controls children = dataGridFrozenGrid.Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children.Add(rectangle2);
				rectangle.Name = "PART_IndentSpacer";
				service = rectangle;
				context.AvaloniaNameScope.Register("PART_IndentSpacer", service);
				rectangle.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
				rectangle.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				((ISupportInitialize)rectangle).EndInit();
				Controls children2 = dataGridFrozenGrid.Children;
				ToggleButton toggleButton;
				ToggleButton toggleButton2 = (toggleButton = new ToggleButton());
				((ISupportInitialize)toggleButton2).BeginInit();
				children2.Add(toggleButton2);
				ToggleButton toggleButton3;
				ToggleButton toggleButton4 = (toggleButton3 = toggleButton);
				context.PushParent(toggleButton3);
				toggleButton3.Name = "PART_ExpanderButton";
				service = toggleButton3;
				context.AvaloniaNameScope.Register("PART_ExpanderButton", service);
				toggleButton3.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
				toggleButton3.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				toggleButton3.SetValue(Layoutable.MarginProperty, new Thickness(2.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				toggleButton3.Bind(TemplatedControl.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				toggleButton3.Bind(TemplatedControl.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				toggleButton3.Bind(TemplatedControl.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				toggleButton3.Bind(TemplatedControl.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				toggleButton3.Bind(TemplatedControl.ForegroundProperty, new TemplateBinding(TemplatedControl.ForegroundProperty).ProvideValue());
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("SimpleDataGridRowGroupExpanderButtonTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_91.DynamicSetter_1(toggleButton3, BindingPriority.Template, obj);
				context.PopParent();
				((ISupportInitialize)toggleButton4).EndInit();
				Controls children3 = dataGridFrozenGrid.Children;
				StackPanel stackPanel;
				StackPanel stackPanel2 = (stackPanel = new StackPanel());
				((ISupportInitialize)stackPanel2).BeginInit();
				children3.Add(stackPanel2);
				StackPanel stackPanel3;
				StackPanel stackPanel4 = (stackPanel3 = stackPanel);
				context.PushParent(stackPanel3);
				stackPanel3.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
				stackPanel3.SetValue(Grid.ColumnProperty, 3, BindingPriority.Template);
				stackPanel3.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				stackPanel3.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				stackPanel3.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				Controls children4 = stackPanel3.Children;
				TextBlock textBlock;
				TextBlock textBlock2 = (textBlock = new TextBlock());
				((ISupportInitialize)textBlock2).BeginInit();
				children4.Add(textBlock2);
				textBlock.Name = "PART_PropertyNameElement";
				service = textBlock;
				context.AvaloniaNameScope.Register("PART_PropertyNameElement", service);
				textBlock.SetValue(Layoutable.MarginProperty, new Thickness(4.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				textBlock.Bind(Visual.IsVisibleProperty, new TemplateBinding(DataGridRowGroupHeader.IsPropertyNameVisibleProperty).ProvideValue());
				((ISupportInitialize)textBlock).EndInit();
				Controls children5 = stackPanel3.Children;
				TextBlock textBlock3;
				TextBlock textBlock4 = (textBlock3 = new TextBlock());
				((ISupportInitialize)textBlock4).BeginInit();
				children5.Add(textBlock4);
				TextBlock textBlock5;
				TextBlock textBlock6 = (textBlock5 = textBlock3);
				context.PushParent(textBlock5);
				textBlock5.SetValue(Layoutable.MarginProperty, new Thickness(4.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				StyledProperty<string?> textProperty = TextBlock.TextProperty;
				CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(XamlIlHelpers.Avalonia_002ECollections_002EDataGridCollectionViewGroup_002CAvalonia_002EControls_002EDataGrid_002EKey_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = TextBlock.TextProperty;
				CompiledBindingExtension binding = compiledBindingExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock5.Bind(textProperty, binding);
				context.PopParent();
				((ISupportInitialize)textBlock6).EndInit();
				Controls children6 = stackPanel3.Children;
				TextBlock textBlock7;
				TextBlock textBlock8 = (textBlock7 = new TextBlock());
				((ISupportInitialize)textBlock8).BeginInit();
				children6.Add(textBlock8);
				textBlock7.Name = "PART_ItemCountElement";
				service = textBlock7;
				context.AvaloniaNameScope.Register("PART_ItemCountElement", service);
				textBlock7.SetValue(Layoutable.MarginProperty, new Thickness(4.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				textBlock7.Bind(Visual.IsVisibleProperty, new TemplateBinding(DataGridRowGroupHeader.IsItemCountVisibleProperty).ProvideValue());
				((ISupportInitialize)textBlock7).EndInit();
				context.PopParent();
				((ISupportInitialize)stackPanel4).EndInit();
				Controls children7 = dataGridFrozenGrid.Children;
				DataGridRowHeader dataGridRowHeader;
				DataGridRowHeader dataGridRowHeader2 = (dataGridRowHeader = new DataGridRowHeader());
				((ISupportInitialize)dataGridRowHeader2).BeginInit();
				children7.Add(dataGridRowHeader2);
				dataGridRowHeader.Name = "RowHeader";
				service = dataGridRowHeader;
				context.AvaloniaNameScope.Register("RowHeader", service);
				dataGridRowHeader.SetValue(Grid.RowSpanProperty, 3, BindingPriority.Template);
				dataGridRowHeader.SetValue(DataGridFrozenGrid.IsFrozenProperty, value: true, BindingPriority.Template);
				((ISupportInitialize)dataGridRowHeader).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(DataGridRowGroupHeader);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = TemplatedControl.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeControlMidHighBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Setter setter4 = new Setter();
			setter4.Property = Layoutable.HeightProperty;
			setter4.Value = 20.0;
			controlTheme.Add(setter4);
			Setter setter5 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter6 = setter;
			setter6.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value2 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_90.Build, context);
			context.PopParent();
			setter6.Value = value2;
			context.PopParent();
			controlTheme.Add(setter5);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_92
	{
		private class XamlClosure_93
		{
			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml");
				if (P_0 != null)
				{
					object service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (Styles)service;
					}
				}
				context.IntermediateRoot = new Rectangle();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				Rectangle rectangle = (Rectangle)intermediateRoot;
				context.PushParent(rectangle);
				rectangle.Width = 2.0;
				StyledProperty<IBrush?> fillProperty = Shape.FillProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeBorderHighColor");
				context.ProvideTargetProperty = Shape.FillProperty;
				IBinding binding = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle.Bind(fillProperty, binding);
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		private class XamlClosure_94
		{
			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (Styles)service;
					}
				}
				context.IntermediateRoot = new Border();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				Border border = (Border)intermediateRoot;
				context.PushParent(border);
				border.Name = "DataGridBorder";
				service = border;
				context.AvaloniaNameScope.Register("DataGridBorder", service);
				border.Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				border.Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				border.Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				border.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				Grid grid;
				Grid grid2 = (grid = new Grid());
				((ISupportInitialize)grid2).BeginInit();
				border.Child = grid2;
				Grid grid3;
				Grid grid4 = (grid3 = grid);
				context.PushParent(grid3);
				Grid grid5 = grid3;
				ColumnDefinitions columnDefinitions = new ColumnDefinitions();
				columnDefinitions.Capacity = 3;
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				grid5.ColumnDefinitions = columnDefinitions;
				RowDefinitions rowDefinitions = new RowDefinitions();
				rowDefinitions.Capacity = 4;
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				rowDefinitions.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				grid5.RowDefinitions = rowDefinitions;
				grid5.SetValue(Visual.ClipToBoundsProperty, value: true, BindingPriority.Template);
				Controls children = grid5.Children;
				DataGridColumnHeader dataGridColumnHeader;
				DataGridColumnHeader dataGridColumnHeader2 = (dataGridColumnHeader = new DataGridColumnHeader());
				((ISupportInitialize)dataGridColumnHeader2).BeginInit();
				children.Add(dataGridColumnHeader2);
				dataGridColumnHeader.Name = "PART_TopLeftCornerHeader";
				service = dataGridColumnHeader;
				context.AvaloniaNameScope.Register("PART_TopLeftCornerHeader", service);
				dataGridColumnHeader.SetValue(Layoutable.WidthProperty, 22.0, BindingPriority.Template);
				((ISupportInitialize)dataGridColumnHeader).EndInit();
				Controls children2 = grid5.Children;
				DataGridColumnHeadersPresenter dataGridColumnHeadersPresenter;
				DataGridColumnHeadersPresenter dataGridColumnHeadersPresenter2 = (dataGridColumnHeadersPresenter = new DataGridColumnHeadersPresenter());
				((ISupportInitialize)dataGridColumnHeadersPresenter2).BeginInit();
				children2.Add(dataGridColumnHeadersPresenter2);
				dataGridColumnHeadersPresenter.Name = "PART_ColumnHeadersPresenter";
				service = dataGridColumnHeadersPresenter;
				context.AvaloniaNameScope.Register("PART_ColumnHeadersPresenter", service);
				dataGridColumnHeadersPresenter.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				((ISupportInitialize)dataGridColumnHeadersPresenter).EndInit();
				Controls children3 = grid5.Children;
				DataGridColumnHeader dataGridColumnHeader3;
				DataGridColumnHeader dataGridColumnHeader4 = (dataGridColumnHeader3 = new DataGridColumnHeader());
				((ISupportInitialize)dataGridColumnHeader4).BeginInit();
				children3.Add(dataGridColumnHeader4);
				dataGridColumnHeader3.Name = "PART_TopRightCornerHeader";
				service = dataGridColumnHeader3;
				context.AvaloniaNameScope.Register("PART_TopRightCornerHeader", service);
				dataGridColumnHeader3.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				((ISupportInitialize)dataGridColumnHeader3).EndInit();
				Controls children4 = grid5.Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children4.Add(rectangle2);
				Rectangle rectangle3;
				Rectangle rectangle4 = (rectangle3 = rectangle);
				context.PushParent(rectangle3);
				Rectangle rectangle5 = rectangle3;
				rectangle5.Name = "PART_ColumnHeadersAndRowsSeparator";
				service = rectangle5;
				context.AvaloniaNameScope.Register("PART_ColumnHeadersAndRowsSeparator", service);
				rectangle5.SetValue(Grid.ColumnSpanProperty, 3, BindingPriority.Template);
				rectangle5.SetValue(Layoutable.HeightProperty, 1.0, BindingPriority.Template);
				rectangle5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Bottom, BindingPriority.Template);
				StyledProperty<IBrush?> fillProperty = Shape.FillProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeControlMidHighBrush");
				context.ProvideTargetProperty = Shape.FillProperty;
				IBinding binding = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle5.Bind(fillProperty, binding);
				rectangle5.SetValue(Shape.StrokeThicknessProperty, 1.0, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)rectangle4).EndInit();
				Controls children5 = grid5.Children;
				DataGridRowsPresenter dataGridRowsPresenter;
				DataGridRowsPresenter dataGridRowsPresenter2 = (dataGridRowsPresenter = new DataGridRowsPresenter());
				((ISupportInitialize)dataGridRowsPresenter2).BeginInit();
				children5.Add(dataGridRowsPresenter2);
				dataGridRowsPresenter.Name = "PART_RowsPresenter";
				service = dataGridRowsPresenter;
				context.AvaloniaNameScope.Register("PART_RowsPresenter", service);
				dataGridRowsPresenter.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
				dataGridRowsPresenter.SetValue(Grid.ColumnSpanProperty, 2, BindingPriority.Template);
				GestureRecognizerCollection gestureRecognizers = dataGridRowsPresenter.GestureRecognizers;
				ScrollGestureRecognizer scrollGestureRecognizer = new ScrollGestureRecognizer();
				((ISupportInitialize)scrollGestureRecognizer).BeginInit();
				scrollGestureRecognizer.CanHorizontallyScroll = true;
				scrollGestureRecognizer.CanVerticallyScroll = true;
				((ISupportInitialize)scrollGestureRecognizer).EndInit();
				gestureRecognizers.Add(scrollGestureRecognizer);
				((ISupportInitialize)dataGridRowsPresenter).EndInit();
				Controls children6 = grid5.Children;
				Rectangle rectangle6;
				Rectangle rectangle7 = (rectangle6 = new Rectangle());
				((ISupportInitialize)rectangle7).BeginInit();
				children6.Add(rectangle7);
				Rectangle rectangle8 = (rectangle3 = rectangle6);
				context.PushParent(rectangle3);
				Rectangle rectangle9 = rectangle3;
				rectangle9.Name = "PART_BottomRightCorner";
				service = rectangle9;
				context.AvaloniaNameScope.Register("PART_BottomRightCorner", service);
				rectangle9.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				rectangle9.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				StyledProperty<IBrush?> fillProperty2 = Shape.FillProperty;
				DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ThemeControlMidHighBrush");
				context.ProvideTargetProperty = Shape.FillProperty;
				IBinding binding2 = dynamicResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle9.Bind(fillProperty2, binding2);
				context.PopParent();
				((ISupportInitialize)rectangle8).EndInit();
				Controls children7 = grid5.Children;
				Rectangle rectangle10;
				Rectangle rectangle11 = (rectangle10 = new Rectangle());
				((ISupportInitialize)rectangle11).BeginInit();
				children7.Add(rectangle11);
				Rectangle rectangle12 = (rectangle3 = rectangle10);
				context.PushParent(rectangle3);
				Rectangle rectangle13 = rectangle3;
				rectangle13.Name = "BottomLeftCorner";
				service = rectangle13;
				context.AvaloniaNameScope.Register("BottomLeftCorner", service);
				rectangle13.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				rectangle13.SetValue(Grid.ColumnSpanProperty, 2, BindingPriority.Template);
				StyledProperty<IBrush?> fillProperty3 = Shape.FillProperty;
				DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ThemeControlMidHighBrush");
				context.ProvideTargetProperty = Shape.FillProperty;
				IBinding binding3 = dynamicResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle13.Bind(fillProperty3, binding3);
				context.PopParent();
				((ISupportInitialize)rectangle12).EndInit();
				Controls children8 = grid5.Children;
				ScrollBar scrollBar;
				ScrollBar scrollBar2 = (scrollBar = new ScrollBar());
				((ISupportInitialize)scrollBar2).BeginInit();
				children8.Add(scrollBar2);
				ScrollBar scrollBar3;
				ScrollBar scrollBar4 = (scrollBar3 = scrollBar);
				context.PushParent(scrollBar3);
				ScrollBar scrollBar5 = scrollBar3;
				scrollBar5.Name = "PART_VerticalScrollbar";
				service = scrollBar5;
				context.AvaloniaNameScope.Register("PART_VerticalScrollbar", service);
				scrollBar5.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
				scrollBar5.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				StyledProperty<double> widthProperty = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("ScrollBarThickness");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding4 = dynamicResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				scrollBar5.Bind(widthProperty, binding4);
				scrollBar5.SetValue(ScrollBar.OrientationProperty, Orientation.Vertical, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)scrollBar4).EndInit();
				Controls children9 = grid5.Children;
				Grid grid6;
				Grid grid7 = (grid6 = new Grid());
				((ISupportInitialize)grid7).BeginInit();
				children9.Add(grid7);
				Grid grid8 = (grid3 = grid6);
				context.PushParent(grid3);
				Grid grid9 = grid3;
				grid9.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				grid9.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				ColumnDefinitions columnDefinitions2 = new ColumnDefinitions();
				columnDefinitions2.Capacity = 2;
				columnDefinitions2.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions2.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid9.ColumnDefinitions = columnDefinitions2;
				Controls children10 = grid9.Children;
				Rectangle rectangle14;
				Rectangle rectangle15 = (rectangle14 = new Rectangle());
				((ISupportInitialize)rectangle15).BeginInit();
				children10.Add(rectangle15);
				rectangle14.Name = "PART_FrozenColumnScrollBarSpacer";
				service = rectangle14;
				context.AvaloniaNameScope.Register("PART_FrozenColumnScrollBarSpacer", service);
				((ISupportInitialize)rectangle14).EndInit();
				Controls children11 = grid9.Children;
				ScrollBar scrollBar6;
				ScrollBar scrollBar7 = (scrollBar6 = new ScrollBar());
				((ISupportInitialize)scrollBar7).BeginInit();
				children11.Add(scrollBar7);
				ScrollBar scrollBar8 = (scrollBar3 = scrollBar6);
				context.PushParent(scrollBar3);
				ScrollBar scrollBar9 = scrollBar3;
				scrollBar9.Name = "PART_HorizontalScrollbar";
				service = scrollBar9;
				context.AvaloniaNameScope.Register("PART_HorizontalScrollbar", service);
				scrollBar9.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				StyledProperty<double> heightProperty = Layoutable.HeightProperty;
				DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("ScrollBarThickness");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				IBinding binding5 = dynamicResourceExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				scrollBar9.Bind(heightProperty, binding5);
				scrollBar9.SetValue(ScrollBar.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)scrollBar8).EndInit();
				context.PopParent();
				((ISupportInitialize)grid8).EndInit();
				context.PopParent();
				((ISupportInitialize)grid4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(DataGrid);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = DataGrid.RowBackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeAccentBrush4");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Setter setter4 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter5 = setter;
			setter5.Property = TemplatedControl.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ThemeBackgroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value2 = dynamicResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter5.Value = value2;
			context.PopParent();
			controlTheme.Add(setter4);
			Setter setter6 = new Setter();
			setter6.Property = DataGrid.HeadersVisibilityProperty;
			setter6.Value = DataGridHeadersVisibility.Column;
			controlTheme.Add(setter6);
			Setter setter7 = new Setter();
			setter7.Property = DataGrid.HorizontalScrollBarVisibilityProperty;
			setter7.Value = ScrollBarVisibility.Auto;
			controlTheme.Add(setter7);
			Setter setter8 = new Setter();
			setter8.Property = DataGrid.VerticalScrollBarVisibilityProperty;
			setter8.Value = ScrollBarVisibility.Auto;
			controlTheme.Add(setter8);
			Setter setter9 = new Setter();
			setter9.Property = DataGrid.SelectionModeProperty;
			setter9.Value = DataGridSelectionMode.Extended;
			controlTheme.Add(setter9);
			Setter setter10 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter11 = setter;
			setter11.Property = DataGrid.HorizontalGridLinesBrushProperty;
			DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ThemeBorderHighColor");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value3 = dynamicResourceExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter11.Value = value3;
			context.PopParent();
			controlTheme.Add(setter10);
			Setter setter12 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter13 = setter;
			setter13.Property = DataGrid.VerticalGridLinesBrushProperty;
			DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("ThemeBorderHighColor");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value4 = dynamicResourceExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter13.Value = value4;
			context.PopParent();
			controlTheme.Add(setter12);
			Setter setter14 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter15 = setter;
			setter15.Property = TemplatedControl.BorderBrushProperty;
			DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("ThemeBorderLowColor");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value5 = dynamicResourceExtension5.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter15.Value = value5;
			context.PopParent();
			controlTheme.Add(setter14);
			Setter setter16 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter17 = setter;
			setter17.Property = TemplatedControl.BorderThicknessProperty;
			DynamicResourceExtension dynamicResourceExtension6 = new DynamicResourceExtension("ThemeBorderThickness");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value6 = dynamicResourceExtension6.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter17.Value = value6;
			context.PopParent();
			controlTheme.Add(setter16);
			Setter setter18 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter19 = setter;
			setter19.Property = DataGrid.DropLocationIndicatorTemplateProperty;
			Template template;
			Template value7 = (template = new Template());
			context.PushParent(template);
			template.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_93.Build, context);
			context.PopParent();
			setter19.Value = value7;
			context.PopParent();
			controlTheme.Add(setter18);
			Setter setter20 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter21 = setter;
			setter21.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value8 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_94.Build, context);
			context.PopParent();
			setter21.Value = value8;
			context.PopParent();
			controlTheme.Add(setter20);
			context.PopParent();
			return result;
		}
	}

	public static void Populate_003A_002FThemes_002FFluent_002Examl(IServiceProvider P_0, Styles P_1)
	{
		XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		Styles styles;
		Styles styles2 = (styles = P_1);
		context.PushParent(styles);
		ResourceDictionary resourceDictionary;
		ResourceDictionary resources = (resourceDictionary = new ResourceDictionary());
		context.PushParent(resourceDictionary);
		ResourceDictionary resourceDictionary2 = resourceDictionary;
		IDictionary<ThemeVariant, IThemeVariantProvider> themeDictionaries = resourceDictionary2.ThemeDictionaries;
		ThemeVariant dark = ThemeVariant.Dark;
		ResourceDictionary value = (resourceDictionary = new ResourceDictionary());
		context.PushParent(resourceDictionary);
		ResourceDictionary resourceDictionary3 = resourceDictionary;
		((IThemeVariantProvider)resourceDictionary3).Key = ThemeVariant.Dark;
		resourceDictionary3.AddDeferred("DataGridColumnHeaderForegroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_1.Build, context));
		resourceDictionary3.AddDeferred("DataGridColumnHeaderBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_2.Build, context));
		resourceDictionary3.AddDeferred("DataGridColumnHeaderHoveredBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_3.Build, context));
		resourceDictionary3.AddDeferred("DataGridColumnHeaderPressedBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_4.Build, context));
		resourceDictionary3.AddDeferred("DataGridColumnHeaderDraggedBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_5.Build, context));
		resourceDictionary3.AddDeferred("DataGridRowGroupHeaderBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_6.Build, context));
		resourceDictionary3.AddDeferred("DataGridRowGroupHeaderPressedBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_7.Build, context));
		resourceDictionary3.AddDeferred("DataGridRowGroupHeaderForegroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_8.Build, context));
		resourceDictionary3.AddDeferred("DataGridRowGroupHeaderHoveredBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_9.Build, context));
		resourceDictionary3.AddDeferred("DataGridRowHoveredBackgroundColor", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_10.Build, context));
		resourceDictionary3.AddDeferred("DataGridRowInvalidBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_11.Build, context));
		resourceDictionary3.AddDeferred("DataGridCellFocusVisualPrimaryBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_12.Build, context));
		resourceDictionary3.AddDeferred("DataGridCellFocusVisualSecondaryBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_13.Build, context));
		resourceDictionary3.AddDeferred("DataGridCellInvalidBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_14.Build, context));
		resourceDictionary3.AddDeferred("DataGridGridLinesBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_15.Build, context));
		resourceDictionary3.AddDeferred("DataGridDetailsPresenterBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_16.Build, context));
		context.PopParent();
		themeDictionaries.Add(dark, value);
		IDictionary<ThemeVariant, IThemeVariantProvider> themeDictionaries2 = resourceDictionary2.ThemeDictionaries;
		ThemeVariant @default = ThemeVariant.Default;
		ResourceDictionary value2 = (resourceDictionary = new ResourceDictionary());
		context.PushParent(resourceDictionary);
		ResourceDictionary resourceDictionary4 = resourceDictionary;
		((IThemeVariantProvider)resourceDictionary4).Key = ThemeVariant.Default;
		resourceDictionary4.AddDeferred("DataGridColumnHeaderForegroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_17.Build, context));
		resourceDictionary4.AddDeferred("DataGridColumnHeaderBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_18.Build, context));
		resourceDictionary4.AddDeferred("DataGridColumnHeaderHoveredBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_19.Build, context));
		resourceDictionary4.AddDeferred("DataGridColumnHeaderPressedBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_20.Build, context));
		resourceDictionary4.AddDeferred("DataGridColumnHeaderDraggedBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_21.Build, context));
		resourceDictionary4.AddDeferred("DataGridRowGroupHeaderBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_22.Build, context));
		resourceDictionary4.AddDeferred("DataGridRowGroupHeaderPressedBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_23.Build, context));
		resourceDictionary4.AddDeferred("DataGridRowGroupHeaderForegroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_24.Build, context));
		resourceDictionary4.AddDeferred("DataGridRowGroupHeaderHoveredBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_25.Build, context));
		resourceDictionary4.AddDeferred("DataGridRowHoveredBackgroundColor", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_26.Build, context));
		resourceDictionary4.AddDeferred("DataGridRowInvalidBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_27.Build, context));
		resourceDictionary4.AddDeferred("DataGridCellFocusVisualPrimaryBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_28.Build, context));
		resourceDictionary4.AddDeferred("DataGridCellFocusVisualSecondaryBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_29.Build, context));
		resourceDictionary4.AddDeferred("DataGridCellInvalidBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_30.Build, context));
		resourceDictionary4.AddDeferred("DataGridGridLinesBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_31.Build, context));
		resourceDictionary4.AddDeferred("DataGridDetailsPresenterBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_32.Build, context));
		context.PopParent();
		themeDictionaries2.Add(@default, value2);
		resourceDictionary2.Add("ListAccentLowOpacity", 0.6);
		resourceDictionary2.Add("ListAccentMediumOpacity", 0.8);
		resourceDictionary2.Add("DataGridSortIconMinWidth", 32.0);
		resourceDictionary2.AddDeferred("DataGridSortIconDescendingPath", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_33.Build, context));
		resourceDictionary2.AddDeferred("DataGridSortIconAscendingPath", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_34.Build, context));
		resourceDictionary2.AddDeferred("DataGridRowGroupHeaderIconClosedPath", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_35.Build, context));
		resourceDictionary2.AddDeferred("DataGridRowGroupHeaderIconOpenedPath", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_36.Build, context));
		resourceDictionary2.AddDeferred("DataGridRowBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_37.Build, context));
		resourceDictionary2.AddDeferred("DataGridRowSelectedBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_38.Build, context));
		resourceDictionary2.AddDeferred("DataGridRowSelectedBackgroundOpacity", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_39.Build, context));
		resourceDictionary2.AddDeferred("DataGridRowSelectedHoveredBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_40.Build, context));
		resourceDictionary2.AddDeferred("DataGridRowSelectedHoveredBackgroundOpacity", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_41.Build, context));
		resourceDictionary2.AddDeferred("DataGridRowSelectedUnfocusedBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_42.Build, context));
		resourceDictionary2.AddDeferred("DataGridRowSelectedUnfocusedBackgroundOpacity", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_43.Build, context));
		resourceDictionary2.AddDeferred("DataGridRowSelectedHoveredUnfocusedBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_44.Build, context));
		resourceDictionary2.AddDeferred("DataGridRowSelectedHoveredUnfocusedBackgroundOpacity", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_45.Build, context));
		resourceDictionary2.AddDeferred("DataGridCellBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_46.Build, context));
		resourceDictionary2.AddDeferred("DataGridCurrencyVisualPrimaryBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_47.Build, context));
		resourceDictionary2.AddDeferred("DataGridFillerColumnGridLinesBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_48.Build, context));
		resourceDictionary2.AddDeferred("DataGridCellTextBlockTheme", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_49.Build, context));
		resourceDictionary2.AddDeferred("DataGridCellTextBoxTheme", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_50.Build, context));
		resourceDictionary2.AddDeferred(typeof(DataGridCell), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_51.Build, context));
		resourceDictionary2.AddDeferred(typeof(DataGridColumnHeader), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_54.Build, context));
		resourceDictionary2.AddDeferred("DataGridTopLeftColumnHeader", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_57.Build, context));
		resourceDictionary2.AddDeferred(typeof(DataGridRowHeader), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_59.Build, context));
		resourceDictionary2.AddDeferred(typeof(DataGridRow), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_62.Build, context));
		resourceDictionary2.AddDeferred("FluentDataGridRowGroupExpanderButtonTheme", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_64.Build, context));
		resourceDictionary2.AddDeferred(typeof(DataGridRowGroupHeader), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_67.Build, context));
		resourceDictionary2.AddDeferred(typeof(DataGrid), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_70.Build, context));
		context.PopParent();
		styles.Resources = resources;
		context.PopParent();
		if (styles2 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	public static Styles Build_003A_002FThemes_002FFluent_002Examl(IServiceProvider P_0)
	{
		Styles styles = new Styles();
		Populate_003A_002FThemes_002FFluent_002Examl(P_0, styles);
		return styles;
	}

	public static void Populate_003A_002FThemes_002FSimple_002Examl(IServiceProvider P_0, Styles P_1)
	{
		XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		Styles styles;
		Styles styles2 = (styles = P_1);
		context.PushParent(styles);
		styles.Resources.Add("DataGridTextColumnCellTextBlockMargin", new Thickness(4.0, 4.0, 4.0, 4.0));
		((ResourceDictionary)styles.Resources).AddDeferred((object)"DataGridCellTextBlockTheme", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_74.Build, context));
		((ResourceDictionary)styles.Resources).AddDeferred((object)"DataGridCellTextBoxTheme", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_75.Build, context));
		((ResourceDictionary)styles.Resources).AddDeferred((object)typeof(DataGridCell), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_76.Build, context));
		((ResourceDictionary)styles.Resources).AddDeferred((object)typeof(DataGridColumnHeader), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_79.Build, context));
		((ResourceDictionary)styles.Resources).AddDeferred((object)typeof(DataGridRowHeader), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_82.Build, context));
		((ResourceDictionary)styles.Resources).AddDeferred((object)typeof(DataGridRow), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_85.Build, context));
		((ResourceDictionary)styles.Resources).AddDeferred((object)"SimpleDataGridRowGroupExpanderButtonTheme", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_87.Build, context));
		((ResourceDictionary)styles.Resources).AddDeferred((object)typeof(DataGridRowGroupHeader), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_89.Build, context));
		((ResourceDictionary)styles.Resources).AddDeferred((object)typeof(DataGrid), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_92.Build, context));
		context.PopParent();
		if (styles2 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	public static Styles Build_003A_002FThemes_002FSimple_002Examl(IServiceProvider P_0)
	{
		Styles styles = new Styles();
		Populate_003A_002FThemes_002FSimple_002Examl(P_0, styles);
		return styles;
	}
}
