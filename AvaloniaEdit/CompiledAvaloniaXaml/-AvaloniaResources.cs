using System;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Styling;
using AvaloniaEdit;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Search;

namespace CompiledAvaloniaXaml;

[EditorBrowsable(EditorBrowsableState.Never)]
public class _0021AvaloniaResources
{
	public class NamespaceInfo_003A_002FCodeCompletion_002FCompletionList_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[31]
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
				CreateNamespaceInfo("Avalonia.Markup.Xaml.Templates", "Avalonia.Markup.Xaml")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("cc", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("AvaloniaEdit.CodeCompletion", "AvaloniaEdit") });
			return dictionary;
		}

		static NamespaceInfo_003A_002FCodeCompletion_002FCompletionList_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FCodeCompletion_002FCompletionList_002Examl();
		}
	}

	private class XamlClosure_1
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FCodeCompletion_002FCompletionList_002Examl.Singleton }, "avares://AvaloniaEdit/CodeCompletion/CompletionList.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			ControlTheme controlTheme2 = controlTheme;
			controlTheme2.TargetType = typeof(CompletionListBox);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension(typeof(ListBox));
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Property();
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			controlTheme2.BasedOn = (ControlTheme)obj;
			Setter setter = new Setter();
			setter.Property = TemplatedControl.PaddingProperty;
			setter.Value = new Thickness(0.0, 0.0, 0.0, 0.0);
			controlTheme2.Add(setter);
			Setter setter2;
			Setter setter3 = (setter2 = new Setter());
			context.PushParent(setter2);
			setter2.Property = ItemsControl.ItemContainerThemeProperty;
			ControlTheme value = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			ControlTheme controlTheme3 = controlTheme;
			controlTheme3.TargetType = typeof(ListBoxItem);
			StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension(typeof(ListBoxItem));
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Property();
			object? obj2 = staticResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			controlTheme3.BasedOn = (ControlTheme)obj2;
			Setter setter4 = new Setter();
			setter4.Property = TemplatedControl.PaddingProperty;
			setter4.Value = new Thickness(4.0, 2.0, 4.0, 2.0);
			controlTheme3.Add(setter4);
			context.PopParent();
			setter2.Value = value;
			context.PopParent();
			controlTheme2.Add(setter3);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_2
	{
		private class XamlClosure_3
		{
			private class XamlClosure_4
			{
				private class DynamicSetters_5
				{
					public static void DynamicSetter_1(ContentPresenter P_0, BindingPriority P_1, CompiledBindingExtension P_2)
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
					XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FCodeCompletion_002FCompletionList_002Examl.Singleton }, "avares://AvaloniaEdit/CodeCompletion/CompletionList.xaml");
					if (P_0 != null)
					{
						object service = P_0.GetService(typeof(IRootObjectProvider));
						if (service != null)
						{
							service = ((IRootObjectProvider)service).RootObject;
							context.RootObject = (ResourceDictionary)service;
						}
					}
					context.IntermediateRoot = new StackPanel();
					object intermediateRoot = context.IntermediateRoot;
					((ISupportInitialize)intermediateRoot).BeginInit();
					StackPanel stackPanel = (StackPanel)intermediateRoot;
					context.PushParent(stackPanel);
					stackPanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
					stackPanel.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
					Controls children = stackPanel.Children;
					Image image;
					Image image2 = (image = new Image());
					((ISupportInitialize)image2).BeginInit();
					children.Add(image2);
					Image image3;
					Image image4 = (image3 = image);
					context.PushParent(image3);
					StyledProperty<IImage?> sourceProperty = Image.SourceProperty;
					CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(XamlIlHelpers.AvaloniaEdit_002ECodeCompletion_002EICompletionData_002CAvaloniaEdit_002EImage_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
					context.ProvideTargetProperty = Image.SourceProperty;
					CompiledBindingExtension binding = compiledBindingExtension.ProvideValue(context);
					context.ProvideTargetProperty = null;
					image3.Bind(sourceProperty, binding);
					image3.SetValue(Layoutable.WidthProperty, 16.0, BindingPriority.Template);
					image3.SetValue(Layoutable.HeightProperty, 16.0, BindingPriority.Template);
					image3.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 2.0, 0.0), BindingPriority.Template);
					context.PopParent();
					((ISupportInitialize)image4).EndInit();
					Controls children2 = stackPanel.Children;
					ContentPresenter contentPresenter;
					ContentPresenter contentPresenter2 = (contentPresenter = new ContentPresenter());
					((ISupportInitialize)contentPresenter2).BeginInit();
					children2.Add(contentPresenter2);
					ContentPresenter contentPresenter3;
					ContentPresenter contentPresenter4 = (contentPresenter3 = contentPresenter);
					context.PushParent(contentPresenter3);
					CompiledBindingExtension compiledBindingExtension2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(XamlIlHelpers.AvaloniaEdit_002ECodeCompletion_002EICompletionData_002CAvaloniaEdit_002EContent_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
					context.ProvideTargetProperty = ContentPresenter.ContentProperty;
					CompiledBindingExtension compiledBindingExtension3 = compiledBindingExtension2.ProvideValue(context);
					context.ProvideTargetProperty = null;
					DynamicSetters_5.DynamicSetter_1(contentPresenter3, BindingPriority.Template, compiledBindingExtension3);
					context.PopParent();
					((ISupportInitialize)contentPresenter4).EndInit();
					context.PopParent();
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FCodeCompletion_002FCompletionList_002Examl.Singleton }, "avares://AvaloniaEdit/CodeCompletion/CompletionList.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (ResourceDictionary)service;
					}
				}
				context.IntermediateRoot = new CompletionListBox();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				CompletionListBox completionListBox = (CompletionListBox)intermediateRoot;
				context.PushParent(completionListBox);
				completionListBox.Name = "PART_ListBox";
				service = completionListBox;
				context.AvaloniaNameScope.Register("PART_ListBox", service);
				StyledProperty<IDataTemplate?> itemTemplateProperty = ItemsControl.ItemTemplateProperty;
				DataTemplate dataTemplate;
				DataTemplate value = (dataTemplate = new DataTemplate());
				context.PushParent(dataTemplate);
				dataTemplate.DataType = typeof(ICompletionData);
				dataTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_4.Build, context);
				context.PopParent();
				completionListBox.SetValue(itemTemplateProperty, value, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FCodeCompletion_002FCompletionList_002Examl.Singleton }, "avares://AvaloniaEdit/CodeCompletion/CompletionList.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(CompletionList);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			setter.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_3.Build, context);
			context.PopParent();
			setter.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			context.PopParent();
			return result;
		}
	}

	public class NamespaceInfo_003A_002FCodeCompletion_002FCompletionWindow_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> dictionary = new Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>>(2);
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[31]
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
				CreateNamespaceInfo("Avalonia.Markup.Xaml.Templates", "Avalonia.Markup.Xaml")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			return dictionary;
		}

		static NamespaceInfo_003A_002FCodeCompletion_002FCompletionWindow_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FCodeCompletion_002FCompletionWindow_002Examl();
		}
	}

	public class NamespaceInfo_003A_002FCodeCompletion_002FInsightWindow_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[31]
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
				CreateNamespaceInfo("Avalonia.Markup.Xaml.Templates", "Avalonia.Markup.Xaml")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("cc", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("AvaloniaEdit.CodeCompletion", "AvaloniaEdit") });
			return dictionary;
		}

		static NamespaceInfo_003A_002FCodeCompletion_002FInsightWindow_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FCodeCompletion_002FInsightWindow_002Examl();
		}
	}

	private class XamlClosure_6
	{
		private class XamlClosure_7
		{
			private class DynamicSetters_8
			{
				public static void DynamicSetter_1(ContentPresenter P_0, BindingPriority P_1, CompiledBindingExtension P_2)
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
				XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FCodeCompletion_002FInsightWindow_002Examl.Singleton }, "avares://AvaloniaEdit/CodeCompletion/InsightWindow.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (ResourceDictionary)service;
					}
				}
				context.IntermediateRoot = new Border();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				Border border = (Border)intermediateRoot;
				context.PushParent(border);
				border.Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				border.Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				border.Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				border.Bind(Decorator.PaddingProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				Grid grid;
				Grid grid2 = (grid = new Grid());
				((ISupportInitialize)grid2).BeginInit();
				border.Child = grid2;
				Grid grid3;
				Grid grid4 = (grid3 = grid);
				context.PushParent(grid3);
				ColumnDefinitions columnDefinitions = new ColumnDefinitions();
				columnDefinitions.Capacity = 2;
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid3.ColumnDefinitions = columnDefinitions;
				RowDefinitions rowDefinitions = new RowDefinitions();
				rowDefinitions.Capacity = 2;
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				rowDefinitions.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid3.RowDefinitions = rowDefinitions;
				Controls children = grid3.Children;
				StackPanel stackPanel;
				StackPanel stackPanel2 = (stackPanel = new StackPanel());
				((ISupportInitialize)stackPanel2).BeginInit();
				children.Add(stackPanel2);
				StackPanel stackPanel3;
				StackPanel stackPanel4 = (stackPanel3 = stackPanel);
				context.PushParent(stackPanel3);
				stackPanel3.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				stackPanel3.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				stackPanel3.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 4.0, 0.0), BindingPriority.Template);
				stackPanel3.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty = Visual.IsVisibleProperty;
				CompiledBindingExtension obj = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(OverloadViewer.ProviderProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Property(XamlIlHelpers.AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECount_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
					.Build())
				{
					Converter = CollapseIfSingleOverloadConverter.Instance
				};
				context.ProvideTargetProperty = Visual.IsVisibleProperty;
				CompiledBindingExtension binding = obj.ProvideValue(context);
				context.ProvideTargetProperty = null;
				stackPanel3.Bind(isVisibleProperty, binding);
				Controls children2 = stackPanel3.Children;
				Button button;
				Button button2 = (button = new Button());
				((ISupportInitialize)button2).BeginInit();
				children2.Add(button2);
				button.Name = "PART_UP";
				service = button;
				context.AvaloniaNameScope.Register("PART_UP", service);
				Path path;
				Path path2 = (path = new Path());
				((ISupportInitialize)path2).BeginInit();
				button.Content = path2;
				path.SetValue(Shape.StrokeProperty, new ImmutableSolidColorBrush(4278190080u), BindingPriority.Template);
				path.SetValue(Shape.FillProperty, new ImmutableSolidColorBrush(4278190080u), BindingPriority.Template);
				path.SetValue(Path.DataProperty, Geometry.Parse("M 0,0.866 L 1,0.866 L 0.5,0 Z"), BindingPriority.Template);
				path.SetValue(Shape.StretchProperty, Stretch.UniformToFill, BindingPriority.Template);
				((ISupportInitialize)path).EndInit();
				((ISupportInitialize)button).EndInit();
				Controls children3 = stackPanel3.Children;
				TextBlock textBlock;
				TextBlock textBlock2 = (textBlock = new TextBlock());
				((ISupportInitialize)textBlock2).BeginInit();
				children3.Add(textBlock2);
				TextBlock textBlock3;
				TextBlock textBlock4 = (textBlock3 = textBlock);
				context.PushParent(textBlock3);
				textBlock3.SetValue(Layoutable.MarginProperty, new Thickness(2.0, 0.0, 2.0, 0.0), BindingPriority.Template);
				StyledProperty<string?> textProperty = TextBlock.TextProperty;
				CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(OverloadViewer.ProviderProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Property(XamlIlHelpers.AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentIndexText_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
					.Build());
				context.ProvideTargetProperty = TextBlock.TextProperty;
				CompiledBindingExtension binding2 = compiledBindingExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock3.Bind(textProperty, binding2);
				context.PopParent();
				((ISupportInitialize)textBlock4).EndInit();
				Controls children4 = stackPanel3.Children;
				Button button3;
				Button button4 = (button3 = new Button());
				((ISupportInitialize)button4).BeginInit();
				children4.Add(button4);
				button3.Name = "PART_DOWN";
				service = button3;
				context.AvaloniaNameScope.Register("PART_DOWN", service);
				Path path3;
				Path path4 = (path3 = new Path());
				((ISupportInitialize)path4).BeginInit();
				button3.Content = path4;
				path3.SetValue(Shape.StrokeProperty, new ImmutableSolidColorBrush(4278190080u), BindingPriority.Template);
				path3.SetValue(Shape.FillProperty, new ImmutableSolidColorBrush(4278190080u), BindingPriority.Template);
				path3.SetValue(Path.DataProperty, Geometry.Parse("M 0,0 L 1,0 L 0.5,0.866 Z"), BindingPriority.Template);
				path3.SetValue(Shape.StretchProperty, Stretch.UniformToFill, BindingPriority.Template);
				((ISupportInitialize)path3).EndInit();
				((ISupportInitialize)button3).EndInit();
				context.PopParent();
				((ISupportInitialize)stackPanel4).EndInit();
				Controls children5 = grid3.Children;
				ContentPresenter contentPresenter;
				ContentPresenter contentPresenter2 = (contentPresenter = new ContentPresenter());
				((ISupportInitialize)contentPresenter2).BeginInit();
				children5.Add(contentPresenter2);
				ContentPresenter contentPresenter3;
				ContentPresenter contentPresenter4 = (contentPresenter3 = contentPresenter);
				context.PushParent(contentPresenter3);
				ContentPresenter contentPresenter5 = contentPresenter3;
				contentPresenter5.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				contentPresenter5.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				CompiledBindingExtension compiledBindingExtension2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(OverloadViewer.ProviderProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Property(XamlIlHelpers.AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentHeader_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
					.Build());
				context.ProvideTargetProperty = ContentPresenter.ContentProperty;
				CompiledBindingExtension compiledBindingExtension3 = compiledBindingExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_8.DynamicSetter_1(contentPresenter5, BindingPriority.Template, compiledBindingExtension3);
				context.PopParent();
				((ISupportInitialize)contentPresenter4).EndInit();
				Controls children6 = grid3.Children;
				ContentPresenter contentPresenter6;
				ContentPresenter contentPresenter7 = (contentPresenter6 = new ContentPresenter());
				((ISupportInitialize)contentPresenter7).BeginInit();
				children6.Add(contentPresenter7);
				ContentPresenter contentPresenter8 = (contentPresenter3 = contentPresenter6);
				context.PushParent(contentPresenter3);
				ContentPresenter contentPresenter9 = contentPresenter3;
				contentPresenter9.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
				contentPresenter9.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				contentPresenter9.SetValue(Grid.ColumnSpanProperty, 2, BindingPriority.Template);
				CompiledBindingExtension compiledBindingExtension4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(OverloadViewer.ProviderProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Property(XamlIlHelpers.AvaloniaEdit_002ECodeCompletion_002EIOverloadProvider_002CAvaloniaEdit_002ECurrentContent_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
					.Build());
				context.ProvideTargetProperty = ContentPresenter.ContentProperty;
				CompiledBindingExtension compiledBindingExtension5 = compiledBindingExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_8.DynamicSetter_1(contentPresenter9, BindingPriority.Template, compiledBindingExtension5);
				context.PopParent();
				((ISupportInitialize)contentPresenter8).EndInit();
				context.PopParent();
				((ISupportInitialize)grid4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		private class XamlClosure_9
		{
			private class DynamicSetters_10
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
				XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FCodeCompletion_002FInsightWindow_002Examl.Singleton }, "avares://AvaloniaEdit/CodeCompletion/InsightWindow.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (ResourceDictionary)service;
					}
				}
				context.IntermediateRoot = new Border();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				((StyledElement)intermediateRoot).Name = "bd";
				service = intermediateRoot;
				context.AvaloniaNameScope.Register("bd", service);
				((AvaloniaObject)intermediateRoot).Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).SetValue(Border.CornerRadiusProperty, new CornerRadius(2.0, 2.0, 2.0, 2.0), BindingPriority.Template);
				ContentPresenter contentPresenter;
				ContentPresenter contentPresenter2 = (contentPresenter = new ContentPresenter());
				((ISupportInitialize)contentPresenter2).BeginInit();
				((Decorator)intermediateRoot).Child = contentPresenter2;
				contentPresenter.Bind(Layoutable.MarginProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				DynamicSetters_10.DynamicSetter_1(contentPresenter, BindingPriority.Template, new TemplateBinding(ContentControl.ContentProperty).ProvideValue());
				((ISupportInitialize)contentPresenter).EndInit();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FCodeCompletion_002FInsightWindow_002Examl.Singleton }, "avares://AvaloniaEdit/CodeCompletion/InsightWindow.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(OverloadViewer);
			Setter setter = new Setter();
			setter.Property = TemplatedControl.BorderThicknessProperty;
			setter.Value = new Thickness(1.0, 1.0, 1.0, 1.0);
			controlTheme.Add(setter);
			Setter setter2 = new Setter();
			setter2.Property = TemplatedControl.BorderBrushProperty;
			setter2.Value = new ImmutableSolidColorBrush(4278190080u);
			controlTheme.Add(setter2);
			Setter setter3 = new Setter();
			setter3.Property = TemplatedControl.BackgroundProperty;
			setter3.Value = new ImmutableSolidColorBrush(4293848814u);
			controlTheme.Add(setter3);
			Setter setter4 = new Setter();
			setter4.Property = TemplatedControl.PaddingProperty;
			setter4.Value = new Thickness(2.0, 2.0, 2.0, 2.0);
			controlTheme.Add(setter4);
			Setter setter5;
			Setter setter6 = (setter5 = new Setter());
			context.PushParent(setter5);
			setter5.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_7.Build, context);
			context.PopParent();
			setter5.Value = value;
			context.PopParent();
			controlTheme.Add(setter6);
			Style style = new Style();
			style.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(Button));
			Setter setter7 = new Setter();
			setter7.Property = TemplatedControl.BackgroundProperty;
			setter7.Value = new ImmutableSolidColorBrush(4292072403u);
			style.Add(setter7);
			Setter setter8 = new Setter();
			setter8.Property = TemplatedControl.PaddingProperty;
			setter8.Value = new Thickness(2.0, 2.0, 2.0, 2.0);
			style.Add(setter8);
			Setter setter9 = new Setter();
			setter9.Property = Layoutable.WidthProperty;
			setter9.Value = 9.0;
			style.Add(setter9);
			Setter setter10 = new Setter();
			setter10.Property = Layoutable.HeightProperty;
			setter10.Value = 9.0;
			style.Add(setter10);
			Setter setter11 = new Setter();
			setter11.Property = TemplatedControl.TemplateProperty;
			setter11.Value = new ControlTemplate
			{
				Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_9.Build, context)
			};
			style.Add(setter11);
			controlTheme.Add(style);
			context.PopParent();
			return result;
		}
	}

	public class NamespaceInfo_003A_002FEditing_002FTextArea_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[31]
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
				CreateNamespaceInfo("Avalonia.Markup.Xaml.Templates", "Avalonia.Markup.Xaml")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("editing", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("AvaloniaEdit.Editing", "AvaloniaEdit") });
			return dictionary;
		}

		static NamespaceInfo_003A_002FEditing_002FTextArea_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FEditing_002FTextArea_002Examl();
		}
	}

	private class XamlClosure_11
	{
		private class XamlClosure_12
		{
			private class XamlClosure_13
			{
				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FEditing_002FTextArea_002Examl.Singleton }, "avares://AvaloniaEdit/Editing/TextArea.xaml");
					if (P_0 != null)
					{
						object service = P_0.GetService(typeof(IRootObjectProvider));
						if (service != null)
						{
							service = ((IRootObjectProvider)service).RootObject;
							context.RootObject = (ResourceDictionary)service;
						}
					}
					context.IntermediateRoot = new StackPanel();
					object intermediateRoot = context.IntermediateRoot;
					((ISupportInitialize)intermediateRoot).BeginInit();
					((AvaloniaObject)intermediateRoot).SetValue(StackPanel.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FEditing_002FTextArea_002Examl.Singleton }, "avares://AvaloniaEdit/Editing/TextArea.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (ResourceDictionary)service;
					}
				}
				context.IntermediateRoot = new DockPanel();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				((AvaloniaObject)intermediateRoot).SetValue(InputElement.FocusableProperty, value: false, BindingPriority.Template);
				((AvaloniaObject)intermediateRoot).Bind(Panel.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				Controls children = ((Panel)intermediateRoot).Children;
				ItemsControl itemsControl;
				ItemsControl itemsControl2 = (itemsControl = new ItemsControl());
				((ISupportInitialize)itemsControl2).BeginInit();
				children.Add(itemsControl2);
				itemsControl.SetValue(DockPanel.DockProperty, Dock.Left, BindingPriority.Template);
				itemsControl.SetValue(InputElement.FocusableProperty, value: false, BindingPriority.Template);
				itemsControl.Bind(ItemsControl.ItemsSourceProperty, new TemplateBinding(TextArea.LeftMarginsProperty).ProvideValue());
				itemsControl.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate
				{
					Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_13.Build, context)
				}, BindingPriority.Template);
				((ISupportInitialize)itemsControl).EndInit();
				Controls children2 = ((Panel)intermediateRoot).Children;
				ContentPresenter contentPresenter;
				ContentPresenter contentPresenter2 = (contentPresenter = new ContentPresenter());
				((ISupportInitialize)contentPresenter2).BeginInit();
				children2.Add(contentPresenter2);
				contentPresenter.Name = "PART_CP";
				service = contentPresenter;
				context.AvaloniaNameScope.Register("PART_CP", service);
				contentPresenter.SetValue(InputElement.CursorProperty, Cursor.Parse("IBeam"), BindingPriority.Template);
				contentPresenter.SetValue(InputElement.FocusableProperty, value: false, BindingPriority.Template);
				contentPresenter.Bind(ContentPresenter.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				((ISupportInitialize)contentPresenter).EndInit();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FEditing_002FTextArea_002Examl.Singleton }, "avares://AvaloniaEdit/Editing/TextArea.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			ControlTheme controlTheme = new ControlTheme();
			controlTheme.TargetType = typeof(TextArea);
			Setter setter = new Setter();
			setter.Property = TextArea.SelectionBrushProperty;
			setter.Value = new ImmutableSolidColorBrush(1711276287u);
			controlTheme.Add(setter);
			Setter setter2 = new Setter();
			setter2.Property = TemplatedControl.TemplateProperty;
			setter2.Value = new ControlTemplate
			{
				Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_12.Build, context)
			};
			controlTheme.Add(setter2);
			return controlTheme;
		}
	}

	public class NamespaceInfo_003A_002FRendering_002FVisualLineDrawingVisual_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[31]
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
				CreateNamespaceInfo("Avalonia.Markup.Xaml.Templates", "Avalonia.Markup.Xaml")
			});
			dictionary.Add("Rendering", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("AvaloniaEdit.Rendering", "AvaloniaEdit") });
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			return dictionary;
		}

		static NamespaceInfo_003A_002FRendering_002FVisualLineDrawingVisual_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FRendering_002FVisualLineDrawingVisual_002Examl();
		}
	}

	public class NamespaceInfo_003A_002FSearch_002FSearchPanel_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> dictionary = new Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>>(4);
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[31]
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
				CreateNamespaceInfo("Avalonia.Markup.Xaml.Templates", "Avalonia.Markup.Xaml")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("ae", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("AvaloniaEdit", "AvaloniaEdit") });
			dictionary.Add("search", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("AvaloniaEdit.Search", "AvaloniaEdit") });
			return dictionary;
		}

		static NamespaceInfo_003A_002FSearch_002FSearchPanel_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FSearch_002FSearchPanel_002Examl();
		}
	}

	private class XamlClosure_14
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FSearch_002FSearchPanel_002Examl.Singleton }, "avares://AvaloniaEdit/Search/SearchPanel.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return StreamGeometry.Parse("m384 780-15-15 189-189-189-189 15-15 204 204-204 204Z");
		}
	}

	private class XamlClosure_15
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FSearch_002FSearchPanel_002Examl.Singleton }, "avares://AvaloniaEdit/Search/SearchPanel.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return StreamGeometry.Parse("m186 768 140-380h30l143 380h-32l-40-113H257l-40 113h-31Zm80-139h151l-75-209h-1l-75 209Zm398 148q-40 0-62.5-21.5T579 697q0-41 27-65.5t74-24.5q18 0 39.5 5t32.5 11v-24q0-39-20-60t-56-21q-20 0-35.5 6T605 544l-17-19q20-16 42.5-23.5T677 494q50 0 75 27.5t25 81.5v164h-26v-39h-4q-17 27-36 38t-47 11Zm-2-26q40 0 64.5-29.5T751 646q-13-8-29.5-12.5T684 629q-35 0-56.5 18.5T606 696q0 26 15 40.5t41 14.5Z");
		}
	}

	private class XamlClosure_16
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FSearch_002FSearchPanel_002Examl.Singleton }, "avares://AvaloniaEdit/Search/SearchPanel.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return StreamGeometry.Parse("M124 764V650h20v94h672v-94h20v114H124Zm266.903-93v-39H387q-17 27-36 38t-46.737 11q-38.789 0-62.026-21.695Q219 637.611 219 601q0-41 27-65.5t73-24.5q18 0 39.5 5t33.5 11v-24q0-39-19.5-60T318 422q-21 0-37 6t-36 20l-17-19q20-16 42.5-23.5t47.84-7.5q49.66 0 74.16 27.284 24.5 27.283 24.5 81.974V671h-26.097ZM324 533q-34 0-56 18.5T246 599q0 27 15.667 41.5Q277.333 655 303 655q40 0 64-29.706t24-75.576Q378 542 361.5 537.5T324 533Zm197 139V285h25v136.182L543 460h2q15-23 37.083-34t49.189-11Q679 415 707 450.5q28 35.5 28 98.5 0 61-28.073 96.5-28.072 35.5-76.53 35.5Q603 681 580 669t-35-33h-2v36h-22Zm108-229q-36 0-61 30.159t-25 76.523Q543 596 568 626.5t61 30.5q36 0 57.5-29t21.5-78q0-49-21.5-78T629 443Z");
		}
	}

	private class XamlClosure_17
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FSearch_002FSearchPanel_002Examl.Singleton }, "avares://AvaloniaEdit/Search/SearchPanel.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return StreamGeometry.Parse("M251 817q-49-48-76.5-109.5T147 577q0-69 27-131.5T251 335l16 14q-48 44-74 103t-26 124q0 64 26 122.5T266 803l-15 14Zm121-27q-14 0-24-10t-10-24q0-14 10-24t24-10q14 0 24 10t10 24q0 14-10 24t-24 10Zm158-192V496l-88 51-10-16 88-51-88-51 10-16 88 51V362h20v102l88-51 10 16-88 51 88 51-10 16-88-51v102h-20Zm178 219-15-14q47-44 73-103t26-124q0-65-25.5-123.5T693 349l15-14q49 49 76.5 111T812 577q0 69-27.5 131.5T708 817Z");
		}
	}

	private class XamlClosure_18
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FSearch_002FSearchPanel_002Examl.Singleton }, "avares://AvaloniaEdit/Search/SearchPanel.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return StreamGeometry.Parse("M470 824V367L247 590l-15-14 248-248 248 248-15 14-223-223v457h-20Z");
		}
	}

	private class XamlClosure_19
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FSearch_002FSearchPanel_002Examl.Singleton }, "avares://AvaloniaEdit/Search/SearchPanel.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return StreamGeometry.Parse("M480 824 232 576l15-14 223 223V328h20v457l223-223 15 14-248 248Z");
		}
	}

	private class XamlClosure_20
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FSearch_002FSearchPanel_002Examl.Singleton }, "avares://AvaloniaEdit/Search/SearchPanel.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return StreamGeometry.Parse("m291 780-15-15 189-189-189-189 15-15 189 189 189-189 15 15-189 189 189 189-15 15-189-189-189 189Z");
		}
	}

	private class XamlClosure_21
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FSearch_002FSearchPanel_002Examl.Singleton }, "avares://AvaloniaEdit/Search/SearchPanel.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return StreamGeometry.Parse("M3.221 3.739L5.482 6.008L7.7 3.784L7 3.084L5.988 4.091L5.98 2.491C5.97909 2.35567 6.03068 2.22525 6.12392 2.12716C6.21716 2.02908 6.3448 1.97095 6.48 1.965H8V1H6.48C6.28496 1.00026 6.09189 1.03902 5.91186 1.11405C5.73183 1.18908 5.56838 1.29892 5.43088 1.43725C5.29338 1.57558 5.18455 1.73969 5.11061 1.92018C5.03667 2.10066 4.99908 2.29396 5 2.489V4.1L3.927 3.033L3.221 3.739ZM9.89014 5.53277H9.90141C10.0836 5.84426 10.3521 6 10.707 6C11.0995 6 11.4131 5.83236 11.6479 5.49708C11.8826 5.1618 12 4.71728 12 4.16353C12 3.65304 11.8995 3.2507 11.6986 2.95652C11.4977 2.66234 11.2113 2.51525 10.8394 2.51525C10.4338 2.51525 10.1211 2.70885 9.90141 3.09604H9.89014V1H9V5.91888H9.89014V5.53277ZM9.87606 4.47177V4.13108C9.87606 3.88449 9.93427 3.6844 10.0507 3.53082C10.169 3.37724 10.3174 3.30045 10.4958 3.30045C10.6854 3.30045 10.831 3.37833 10.9324 3.53407C11.0357 3.68765 11.0873 3.9018 11.0873 4.17651C11.0873 4.50746 11.031 4.76379 10.9183 4.94549C10.8075 5.12503 10.6507 5.2148 10.4479 5.2148C10.2808 5.2148 10.1437 5.14449 10.0366 5.00389C9.92958 4.86329 9.87606 4.68592 9.87606 4.47177ZM9 12.7691C8.74433 12.923 8.37515 13 7.89247 13C7.32855 13 6.87216 12.8225 6.5233 12.4674C6.17443 12.1124 6 11.6543 6 11.0931C6 10.4451 6.18638 9.93484 6.55914 9.5624C6.93429 9.18747 7.43489 9.00001 8.06093 9.00001C8.49343 9.00001 8.80645 9.0596 9 9.17878V10.1769C8.76344 9.99319 8.4994 9.90132 8.20789 9.90132C7.88292 9.90132 7.62485 10.0006 7.43369 10.1993C7.24492 10.3954 7.15054 10.6673 7.15054 11.0149C7.15054 11.3526 7.24134 11.6183 7.42294 11.8119C7.60454 12.0031 7.85424 12.0987 8.17204 12.0987C8.454 12.0987 8.72999 12.0068 9 11.8231V12.7691ZM4 7L3 8V14L4 15H11L12 14V8L11 7H4ZM4 8H5H10H11V9V13V14H10H5H4V13V9V8Z");
		}
	}

	private class XamlClosure_22
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FSearch_002FSearchPanel_002Examl.Singleton }, "avares://AvaloniaEdit/Search/SearchPanel.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return StreamGeometry.Parse("M11.6009 2.67683C11.7474 2.36708 11.9559 2.2122 12.2263 2.2122C12.4742 2.2122 12.6651 2.32987 12.7991 2.56522C12.933 2.80056 13 3.12243 13 3.53082C13 3.97383 12.9218 4.32944 12.7653 4.59766C12.6088 4.86589 12.3997 5 12.138 5C11.9014 5 11.7224 4.87541 11.6009 4.62622H11.5934V4.93511H11V1H11.5934V2.67683H11.6009ZM11.584 3.77742C11.584 3.94873 11.6197 4.09063 11.6911 4.20311C11.7624 4.3156 11.8538 4.37184 11.9653 4.37184C12.1005 4.37184 12.205 4.30002 12.2789 4.15639C12.354 4.01103 12.3915 3.80597 12.3915 3.54121C12.3915 3.32144 12.3571 3.15012 12.2883 3.02726C12.2207 2.90266 12.1236 2.84036 11.9972 2.84036C11.8782 2.84036 11.7793 2.9018 11.7005 3.02466C11.6228 3.14752 11.584 3.30759 11.584 3.50487V3.77742ZM4.11969 7.695L2 5.56781L2.66188 4.90594L3.66781 5.90625V4.39594C3.66695 4.21309 3.70219 4.03187 3.7715 3.86266C3.84082 3.69346 3.94286 3.53961 4.07176 3.40992C4.20066 3.28023 4.3539 3.17727 4.52268 3.10692C4.69146 3.03658 4.87246 3.00024 5.05531 3H7.39906V3.90469H5.05531C4.92856 3.91026 4.8089 3.96476 4.72149 4.05672C4.63408 4.14868 4.58571 4.27094 4.58656 4.39781L4.59406 5.89781L5.54281 4.95375L6.19906 5.61L4.11969 7.695ZM9.3556 4.93017H10V3.22067C10 2.40689 9.68534 2 9.05603 2C8.92098 2 8.77083 2.02421 8.6056 2.07263C8.44181 2.12104 8.3125 2.17691 8.21767 2.24022V2.90503C8.45474 2.70205 8.70474 2.60056 8.96767 2.60056C9.22917 2.60056 9.35991 2.75698 9.35991 3.06983L8.76078 3.17318C8.25359 3.25885 8 3.57914 8 4.13408C8 4.39665 8.06106 4.60708 8.18319 4.76536C8.30675 4.92179 8.47557 5 8.68966 5C8.97989 5 9.19899 4.83985 9.34698 4.51955H9.3556V4.93017ZM9.35991 3.57542V3.76816C9.35991 3.9432 9.31968 4.08845 9.23922 4.20391C9.15876 4.3175 9.0546 4.3743 8.92672 4.3743C8.83477 4.3743 8.76149 4.34264 8.7069 4.27933C8.65374 4.21415 8.62716 4.13128 8.62716 4.03073C8.62716 3.80912 8.73779 3.6797 8.95905 3.64246L9.35991 3.57542ZM7 12.9302H6.3556V12.5196H6.34698C6.19899 12.8399 5.97989 13 5.68966 13C5.47557 13 5.30675 12.9218 5.18319 12.7654C5.06106 12.6071 5 12.3966 5 12.1341C5 11.5791 5.25359 11.2588 5.76078 11.1732L6.35991 11.0698C6.35991 10.757 6.22917 10.6006 5.96767 10.6006C5.70474 10.6006 5.45474 10.702 5.21767 10.905V10.2402C5.3125 10.1769 5.44181 10.121 5.6056 10.0726C5.77083 10.0242 5.92098 10 6.05603 10C6.68534 10 7 10.4069 7 11.2207V12.9302ZM6.35991 11.7682V11.5754L5.95905 11.6425C5.73779 11.6797 5.62716 11.8091 5.62716 12.0307C5.62716 12.1313 5.65374 12.2142 5.7069 12.2793C5.76149 12.3426 5.83477 12.3743 5.92672 12.3743C6.0546 12.3743 6.15876 12.3175 6.23922 12.2039C6.31968 12.0885 6.35991 11.9432 6.35991 11.7682ZM9.26165 13C9.58343 13 9.82955 12.9423 10 12.8268V12.1173C9.81999 12.2551 9.636 12.324 9.44803 12.324C9.23616 12.324 9.06969 12.2523 8.94863 12.1089C8.82756 11.9637 8.76702 11.7644 8.76702 11.5112C8.76702 11.2505 8.82995 11.0466 8.95579 10.8994C9.08323 10.7505 9.25528 10.676 9.47192 10.676C9.66627 10.676 9.84229 10.7449 10 10.8827V10.1341C9.87097 10.0447 9.66229 10 9.37395 10C8.95659 10 8.62286 10.1406 8.37276 10.4218C8.12425 10.7011 8 11.0838 8 11.5698C8 11.9907 8.11629 12.3343 8.34887 12.6006C8.58144 12.8669 8.8857 13 9.26165 13ZM2 9L3 8H12L13 9V14L12 15H3L2 14V9ZM3 9V14H12V9H3ZM6 7L7 6H14L15 7V12L14 13V12V7H7H6Z");
		}
	}

	private class XamlClosure_23
	{
		private class XamlClosure_24
		{
			private class DynamicSetters_25
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
				XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FSearch_002FSearchPanel_002Examl.Singleton }, "avares://AvaloniaEdit/Search/SearchPanel.xaml");
				if (P_0 != null)
				{
					object service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (ResourceDictionary)service;
					}
				}
				context.IntermediateRoot = new Border();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				Border border = (Border)intermediateRoot;
				context.PushParent(border);
				border.SetValue(Border.BackgroundProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				border.SetValue(Decorator.PaddingProperty, new Thickness(5.0, 5.0, 5.0, 5.0), BindingPriority.Template);
				Path path;
				Path path2 = (path = new Path());
				((ISupportInitialize)path2).BeginInit();
				border.Child = path2;
				Path path3;
				Path path4 = (path3 = path);
				context.PushParent(path3);
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ChevronRightIconPath");
				context.ProvideTargetProperty = Path.DataProperty;
				object? obj = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_25.DynamicSetter_1(path3, BindingPriority.Template, obj);
				path3.SetValue(Shape.StretchProperty, Stretch.Uniform, BindingPriority.Template);
				path3.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				path3.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				path3.SetValue(Layoutable.HeightProperty, 13.0, BindingPriority.Template);
				path3.Bind(Shape.FillProperty, new TemplateBinding(TemplatedControl.ForegroundProperty).ProvideValue());
				path3.Bind(Shape.StrokeProperty, new TemplateBinding(TemplatedControl.ForegroundProperty).ProvideValue());
				path3.SetValue(Shape.StrokeThicknessProperty, 1.0, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)path4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FSearch_002FSearchPanel_002Examl.Singleton }, "avares://AvaloniaEdit/Search/SearchPanel.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(ToggleButton);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension(typeof(ToggleButton));
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
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_24.Build, context);
			context.PopParent();
			setter.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Style style = new Style();
			style.Selector = ((Selector?)null).Nesting().Class(":checked").Template()
				.OfType(typeof(Path));
			Setter setter3 = new Setter();
			setter3.Property = Visual.RenderTransformProperty;
			TransformGroup transformGroup = new TransformGroup();
			Transforms children = transformGroup.Children;
			RotateTransform rotateTransform = new RotateTransform();
			rotateTransform.SetValue(RotateTransform.AngleProperty, 90.0, BindingPriority.Template);
			children.Add(rotateTransform);
			Transforms children2 = transformGroup.Children;
			TranslateTransform translateTransform = new TranslateTransform();
			translateTransform.SetValue(TranslateTransform.XProperty, -2.0, BindingPriority.Template);
			children2.Add(translateTransform);
			setter3.Value = transformGroup;
			style.Add(setter3);
			controlTheme.Add(style);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_26
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FSearch_002FSearchPanel_002Examl.Singleton }, "avares://AvaloniaEdit/Search/SearchPanel.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(ToggleButton);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension(typeof(ToggleButton));
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002EControlTheme_002CAvalonia_002EBase_002EBasedOn_0021Property();
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			controlTheme.BasedOn = (ControlTheme)obj;
			Setter setter = new Setter();
			setter.Property = TemplatedControl.PaddingProperty;
			setter.Value = new Thickness(4.0, 4.0, 4.0, 4.0);
			controlTheme.Add(setter);
			Setter setter2 = new Setter();
			setter2.Property = Layoutable.MinWidthProperty;
			setter2.Value = 22.0;
			controlTheme.Add(setter2);
			Setter setter3 = new Setter();
			setter3.Property = Layoutable.MinHeightProperty;
			setter3.Value = 20.0;
			controlTheme.Add(setter3);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_27
	{
		private class XamlClosure_28
		{
			private class DynamicSetters_29
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

				public static void DynamicSetter_2(Path P_0, BindingPriority P_1, object P_2)
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
				XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FSearch_002FSearchPanel_002Examl.Singleton }, "avares://AvaloniaEdit/Search/SearchPanel.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (ResourceDictionary)service;
					}
				}
				context.IntermediateRoot = new Border();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				Border border = (Border)intermediateRoot;
				context.PushParent(border);
				border.Name = "PART_Border";
				service = border;
				context.AvaloniaNameScope.Register("PART_Border", service);
				border.Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				border.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				border.Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				border.Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				border.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Right, BindingPriority.Template);
				border.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top, BindingPriority.Template);
				border.SetValue(Decorator.PaddingProperty, new Thickness(10.0, 10.0, 10.0, 10.0), BindingPriority.Template);
				Grid grid;
				Grid grid2 = (grid = new Grid());
				((ISupportInitialize)grid2).BeginInit();
				border.Child = grid2;
				Grid grid3;
				Grid grid4 = (grid3 = grid);
				context.PushParent(grid3);
				ColumnDefinitions columnDefinitions = new ColumnDefinitions();
				columnDefinitions.Capacity = 3;
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				grid3.ColumnDefinitions = columnDefinitions;
				RowDefinitions rowDefinitions = new RowDefinitions();
				rowDefinitions.Capacity = 3;
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				grid3.RowDefinitions = rowDefinitions;
				Controls children = grid3.Children;
				ToggleButton toggleButton;
				ToggleButton toggleButton2 = (toggleButton = new ToggleButton());
				((ISupportInitialize)toggleButton2).BeginInit();
				children.Add(toggleButton2);
				ToggleButton toggleButton3;
				ToggleButton toggleButton4 = (toggleButton3 = toggleButton);
				context.PushParent(toggleButton3);
				ToggleButton toggleButton5 = toggleButton3;
				toggleButton5.Name = "Expander";
				service = toggleButton5;
				context.AvaloniaNameScope.Register("Expander", service);
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("SearchPanelExpanderToggle");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_29.DynamicSetter_1(toggleButton5, BindingPriority.Template, obj);
				toggleButton5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty = Visual.IsVisibleProperty;
				CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Not().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Property(XamlIlHelpers.AvaloniaEdit_002ESearch_002ESearchPanel_002CAvaloniaEdit_002ETextEditor_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
					.Property(TextEditor.IsReadOnlyProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build());
				context.ProvideTargetProperty = Visual.IsVisibleProperty;
				CompiledBindingExtension binding = compiledBindingExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				toggleButton5.Bind(isVisibleProperty, binding);
				StyledProperty<bool?> isCheckedProperty = ToggleButton.IsCheckedProperty;
				CompiledBindingExtension obj2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(SearchPanel.IsReplaceModeProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
				CompiledBindingExtension binding2 = obj2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				toggleButton5.Bind(isCheckedProperty, binding2);
				toggleButton5.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				toggleButton5.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				toggleButton5.SetValue(Layoutable.WidthProperty, 16.0, BindingPriority.Template);
				toggleButton5.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 5.0, 0.0), BindingPriority.Template);
				AttachedProperty<object?> tipProperty = ToolTip.TipProperty;
				TextBlock textBlock;
				TextBlock textBlock2 = (textBlock = new TextBlock());
				((ISupportInitialize)textBlock2).BeginInit();
				toggleButton5.SetValue(tipProperty, textBlock2, BindingPriority.Template);
				textBlock.SetValue(TextBlock.TextProperty, SR.SearchToggleReplace, BindingPriority.Template);
				((ISupportInitialize)textBlock).EndInit();
				context.PopParent();
				((ISupportInitialize)toggleButton4).EndInit();
				Controls children2 = grid3.Children;
				StackPanel stackPanel;
				StackPanel stackPanel2 = (stackPanel = new StackPanel());
				((ISupportInitialize)stackPanel2).BeginInit();
				children2.Add(stackPanel2);
				StackPanel stackPanel3;
				StackPanel stackPanel4 = (stackPanel3 = stackPanel);
				context.PushParent(stackPanel3);
				StackPanel stackPanel5 = stackPanel3;
				stackPanel5.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				stackPanel5.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				stackPanel5.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				Controls children3 = stackPanel5.Children;
				TextBox textBox;
				TextBox textBox2 = (textBox = new TextBox());
				((ISupportInitialize)textBox2).BeginInit();
				children3.Add(textBox2);
				TextBox textBox3;
				TextBox textBox4 = (textBox3 = textBox);
				context.PushParent(textBox3);
				TextBox textBox5 = textBox3;
				textBox5.SetValue(TextBox.WatermarkProperty, SR.SearchLabel, BindingPriority.Template);
				textBox5.Name = "PART_searchTextBox";
				service = textBox5;
				context.AvaloniaNameScope.Register("PART_searchTextBox", service);
				textBox5.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				textBox5.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				textBox5.SetValue(Layoutable.WidthProperty, 265.0, BindingPriority.Template);
				textBox5.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				StyledProperty<string?> textProperty = TextBox.TextProperty;
				CompiledBindingExtension compiledBindingExtension2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(SearchPanel.SearchPatternProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build());
				context.ProvideTargetProperty = TextBox.TextProperty;
				CompiledBindingExtension binding3 = compiledBindingExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBox5.Bind(textProperty, binding3);
				StyledProperty<object> innerRightContentProperty = TextBox.InnerRightContentProperty;
				StackPanel stackPanel6;
				StackPanel stackPanel7 = (stackPanel6 = new StackPanel());
				((ISupportInitialize)stackPanel7).BeginInit();
				textBox5.SetValue(innerRightContentProperty, stackPanel7, BindingPriority.Template);
				StackPanel stackPanel8 = (stackPanel3 = stackPanel6);
				context.PushParent(stackPanel3);
				StackPanel stackPanel9 = stackPanel3;
				stackPanel9.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				Controls children4 = stackPanel9.Children;
				ToggleButton toggleButton6;
				ToggleButton toggleButton7 = (toggleButton6 = new ToggleButton());
				((ISupportInitialize)toggleButton7).BeginInit();
				children4.Add(toggleButton7);
				ToggleButton toggleButton8 = (toggleButton3 = toggleButton6);
				context.PushParent(toggleButton3);
				ToggleButton toggleButton9 = toggleButton3;
				StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("SearchPanelTextBoxButton");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj3 = staticResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_29.DynamicSetter_1(toggleButton9, BindingPriority.Template, obj3);
				StyledProperty<bool?> isCheckedProperty2 = ToggleButton.IsCheckedProperty;
				CompiledBindingExtension obj4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(SearchPanel), 0).Property(SearchPanel.MatchCaseProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
				CompiledBindingExtension binding4 = obj4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				toggleButton9.Bind(isCheckedProperty2, binding4);
				toggleButton9.SetValue(ToolTip.TipProperty, SR.SearchMatchCaseText, BindingPriority.Template);
				toggleButton9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Path path;
				Path path2 = (path = new Path());
				((ISupportInitialize)path2).BeginInit();
				toggleButton9.Content = path2;
				Path path3;
				Path path4 = (path3 = path);
				context.PushParent(path3);
				Path path5 = path3;
				StaticResourceExtension staticResourceExtension3 = new StaticResourceExtension("CaseSensitiveIconPath");
				context.ProvideTargetProperty = Path.DataProperty;
				object? obj5 = staticResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_29.DynamicSetter_2(path5, BindingPriority.Template, obj5);
				path5.SetValue(Shape.StretchProperty, Stretch.UniformToFill, BindingPriority.Template);
				path5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				path5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				path5.SetValue(Layoutable.WidthProperty, 12.0, BindingPriority.Template);
				StyledProperty<IBrush?> fillProperty = Shape.FillProperty;
				CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(ToggleButton), 0).Property(TemplatedControl.ForegroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = Shape.FillProperty;
				CompiledBindingExtension binding5 = compiledBindingExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				path5.Bind(fillProperty, binding5);
				StyledProperty<IBrush?> strokeProperty = Shape.StrokeProperty;
				CompiledBindingExtension compiledBindingExtension4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(ToggleButton), 0).Property(TemplatedControl.ForegroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = Shape.StrokeProperty;
				CompiledBindingExtension binding6 = compiledBindingExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				path5.Bind(strokeProperty, binding6);
				path5.SetValue(Shape.StrokeThicknessProperty, 1.0, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)path4).EndInit();
				context.PopParent();
				((ISupportInitialize)toggleButton8).EndInit();
				Controls children5 = stackPanel9.Children;
				ToggleButton toggleButton10;
				ToggleButton toggleButton11 = (toggleButton10 = new ToggleButton());
				((ISupportInitialize)toggleButton11).BeginInit();
				children5.Add(toggleButton11);
				ToggleButton toggleButton12 = (toggleButton3 = toggleButton10);
				context.PushParent(toggleButton3);
				ToggleButton toggleButton13 = toggleButton3;
				StaticResourceExtension staticResourceExtension4 = new StaticResourceExtension("SearchPanelTextBoxButton");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj6 = staticResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_29.DynamicSetter_1(toggleButton13, BindingPriority.Template, obj6);
				StyledProperty<bool?> isCheckedProperty3 = ToggleButton.IsCheckedProperty;
				CompiledBindingExtension obj7 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(SearchPanel), 0).Property(SearchPanel.WholeWordsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
				CompiledBindingExtension binding7 = obj7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				toggleButton13.Bind(isCheckedProperty3, binding7);
				toggleButton13.SetValue(ToolTip.TipProperty, SR.SearchMatchWholeWordsText, BindingPriority.Template);
				toggleButton13.SetValue(Layoutable.MarginProperty, new Thickness(3.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				toggleButton13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Path path6;
				Path path7 = (path6 = new Path());
				((ISupportInitialize)path7).BeginInit();
				toggleButton13.Content = path7;
				Path path8 = (path3 = path6);
				context.PushParent(path3);
				Path path9 = path3;
				StaticResourceExtension staticResourceExtension5 = new StaticResourceExtension("CompleteWordIconPath");
				context.ProvideTargetProperty = Path.DataProperty;
				object? obj8 = staticResourceExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_29.DynamicSetter_2(path9, BindingPriority.Template, obj8);
				path9.SetValue(Shape.StretchProperty, Stretch.Uniform, BindingPriority.Template);
				path9.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				path9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				path9.SetValue(Layoutable.WidthProperty, 14.0, BindingPriority.Template);
				StyledProperty<IBrush?> fillProperty2 = Shape.FillProperty;
				CompiledBindingExtension compiledBindingExtension5 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(ToggleButton), 0).Property(TemplatedControl.ForegroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = Shape.FillProperty;
				CompiledBindingExtension binding8 = compiledBindingExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				path9.Bind(fillProperty2, binding8);
				StyledProperty<IBrush?> strokeProperty2 = Shape.StrokeProperty;
				CompiledBindingExtension compiledBindingExtension6 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(ToggleButton), 0).Property(TemplatedControl.ForegroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = Shape.StrokeProperty;
				CompiledBindingExtension binding9 = compiledBindingExtension6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				path9.Bind(strokeProperty2, binding9);
				path9.SetValue(Shape.StrokeThicknessProperty, 1.0, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)path8).EndInit();
				context.PopParent();
				((ISupportInitialize)toggleButton12).EndInit();
				Controls children6 = stackPanel9.Children;
				ToggleButton toggleButton14;
				ToggleButton toggleButton15 = (toggleButton14 = new ToggleButton());
				((ISupportInitialize)toggleButton15).BeginInit();
				children6.Add(toggleButton15);
				ToggleButton toggleButton16 = (toggleButton3 = toggleButton14);
				context.PushParent(toggleButton3);
				ToggleButton toggleButton17 = toggleButton3;
				StaticResourceExtension staticResourceExtension6 = new StaticResourceExtension("SearchPanelTextBoxButton");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj9 = staticResourceExtension6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_29.DynamicSetter_1(toggleButton17, BindingPriority.Template, obj9);
				StyledProperty<bool?> isCheckedProperty4 = ToggleButton.IsCheckedProperty;
				CompiledBindingExtension obj10 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(SearchPanel), 0).Property(SearchPanel.UseRegexProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
				CompiledBindingExtension binding10 = obj10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				toggleButton17.Bind(isCheckedProperty4, binding10);
				toggleButton17.SetValue(ToolTip.TipProperty, SR.SearchUseRegexText, BindingPriority.Template);
				toggleButton17.SetValue(Layoutable.MarginProperty, new Thickness(3.0, 0.0, 5.0, 0.0), BindingPriority.Template);
				toggleButton17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Path path10;
				Path path11 = (path10 = new Path());
				((ISupportInitialize)path11).BeginInit();
				toggleButton17.Content = path11;
				Path path12 = (path3 = path10);
				context.PushParent(path3);
				Path path13 = path3;
				StaticResourceExtension staticResourceExtension7 = new StaticResourceExtension("RegularExpressionIconPath");
				context.ProvideTargetProperty = Path.DataProperty;
				object? obj11 = staticResourceExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_29.DynamicSetter_2(path13, BindingPriority.Template, obj11);
				path13.SetValue(Shape.StretchProperty, Stretch.Uniform, BindingPriority.Template);
				path13.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				path13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				path13.SetValue(Layoutable.WidthProperty, 12.0, BindingPriority.Template);
				StyledProperty<IBrush?> fillProperty3 = Shape.FillProperty;
				CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(ToggleButton), 0).Property(TemplatedControl.ForegroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = Shape.FillProperty;
				CompiledBindingExtension binding11 = compiledBindingExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				path13.Bind(fillProperty3, binding11);
				StyledProperty<IBrush?> strokeProperty3 = Shape.StrokeProperty;
				CompiledBindingExtension compiledBindingExtension8 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(ToggleButton), 0).Property(TemplatedControl.ForegroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = Shape.StrokeProperty;
				CompiledBindingExtension binding12 = compiledBindingExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				path13.Bind(strokeProperty3, binding12);
				path13.SetValue(Shape.StrokeThicknessProperty, 1.0, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)path12).EndInit();
				context.PopParent();
				((ISupportInitialize)toggleButton16).EndInit();
				context.PopParent();
				((ISupportInitialize)stackPanel8).EndInit();
				context.PopParent();
				((ISupportInitialize)textBox4).EndInit();
				Controls children7 = stackPanel5.Children;
				Button button;
				Button button2 = (button = new Button());
				((ISupportInitialize)button2).BeginInit();
				children7.Add(button2);
				Button button3;
				Button button4 = (button3 = button);
				context.PushParent(button3);
				Button button5 = button3;
				button5.SetValue(Layoutable.MarginProperty, new Thickness(5.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				button5.SetValue(Layoutable.MinWidthProperty, 34.0, BindingPriority.Template);
				button5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				button5.SetValue(Button.CommandProperty, SearchCommands.FindPrevious, BindingPriority.Template);
				AttachedProperty<object?> tipProperty2 = ToolTip.TipProperty;
				TextBlock textBlock3;
				TextBlock textBlock4 = (textBlock3 = new TextBlock());
				((ISupportInitialize)textBlock4).BeginInit();
				button5.SetValue(tipProperty2, textBlock4, BindingPriority.Template);
				textBlock3.SetValue(TextBlock.TextProperty, SR.SearchFindPreviousText, BindingPriority.Template);
				((ISupportInitialize)textBlock3).EndInit();
				Path path14;
				Path path15 = (path14 = new Path());
				((ISupportInitialize)path15).BeginInit();
				button5.Content = path15;
				Path path16 = (path3 = path14);
				context.PushParent(path3);
				Path path17 = path3;
				StaticResourceExtension staticResourceExtension8 = new StaticResourceExtension("FindPreviousIconPath");
				context.ProvideTargetProperty = Path.DataProperty;
				object? obj12 = staticResourceExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_29.DynamicSetter_2(path17, BindingPriority.Template, obj12);
				path17.SetValue(Shape.StretchProperty, Stretch.Uniform, BindingPriority.Template);
				path17.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				path17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				path17.SetValue(Layoutable.WidthProperty, 16.0, BindingPriority.Template);
				StyledProperty<IBrush?> fillProperty4 = Shape.FillProperty;
				CompiledBindingExtension compiledBindingExtension9 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(Button), 0).Property(TemplatedControl.ForegroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = Shape.FillProperty;
				CompiledBindingExtension binding13 = compiledBindingExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				path17.Bind(fillProperty4, binding13);
				StyledProperty<IBrush?> strokeProperty4 = Shape.StrokeProperty;
				CompiledBindingExtension compiledBindingExtension10 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(Button), 0).Property(TemplatedControl.ForegroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = Shape.StrokeProperty;
				CompiledBindingExtension binding14 = compiledBindingExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				path17.Bind(strokeProperty4, binding14);
				path17.SetValue(Shape.StrokeThicknessProperty, 1.0, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)path16).EndInit();
				context.PopParent();
				((ISupportInitialize)button4).EndInit();
				Controls children8 = stackPanel5.Children;
				Button button6;
				Button button7 = (button6 = new Button());
				((ISupportInitialize)button7).BeginInit();
				children8.Add(button7);
				Button button8 = (button3 = button6);
				context.PushParent(button3);
				Button button9 = button3;
				button9.SetValue(Layoutable.MarginProperty, new Thickness(3.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				button9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				button9.SetValue(Layoutable.MinWidthProperty, 34.0, BindingPriority.Template);
				button9.SetValue(Button.CommandProperty, SearchCommands.FindNext, BindingPriority.Template);
				AttachedProperty<object?> tipProperty3 = ToolTip.TipProperty;
				TextBlock textBlock5;
				TextBlock textBlock6 = (textBlock5 = new TextBlock());
				((ISupportInitialize)textBlock6).BeginInit();
				button9.SetValue(tipProperty3, textBlock6, BindingPriority.Template);
				textBlock5.SetValue(TextBlock.TextProperty, SR.SearchFindNextText, BindingPriority.Template);
				((ISupportInitialize)textBlock5).EndInit();
				Path path18;
				Path path19 = (path18 = new Path());
				((ISupportInitialize)path19).BeginInit();
				button9.Content = path19;
				Path path20 = (path3 = path18);
				context.PushParent(path3);
				Path path21 = path3;
				StaticResourceExtension staticResourceExtension9 = new StaticResourceExtension("FindNextIconPath");
				context.ProvideTargetProperty = Path.DataProperty;
				object? obj13 = staticResourceExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_29.DynamicSetter_2(path21, BindingPriority.Template, obj13);
				path21.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				path21.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				path21.SetValue(Shape.StretchProperty, Stretch.Uniform, BindingPriority.Template);
				path21.SetValue(Layoutable.WidthProperty, 16.0, BindingPriority.Template);
				StyledProperty<IBrush?> fillProperty5 = Shape.FillProperty;
				CompiledBindingExtension compiledBindingExtension11 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(Button), 0).Property(TemplatedControl.ForegroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = Shape.FillProperty;
				CompiledBindingExtension binding15 = compiledBindingExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				path21.Bind(fillProperty5, binding15);
				StyledProperty<IBrush?> strokeProperty5 = Shape.StrokeProperty;
				CompiledBindingExtension compiledBindingExtension12 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(Button), 0).Property(TemplatedControl.ForegroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = Shape.StrokeProperty;
				CompiledBindingExtension binding16 = compiledBindingExtension12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				path21.Bind(strokeProperty5, binding16);
				path21.SetValue(Shape.StrokeThicknessProperty, 1.0, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)path20).EndInit();
				context.PopParent();
				((ISupportInitialize)button8).EndInit();
				Controls children9 = stackPanel5.Children;
				Button button10;
				Button button11 = (button10 = new Button());
				((ISupportInitialize)button11).BeginInit();
				children9.Add(button11);
				Button button12 = (button3 = button10);
				context.PushParent(button3);
				Button button13 = button3;
				button13.SetValue(Layoutable.MarginProperty, new Thickness(3.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				button13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				button13.SetValue(Layoutable.MinWidthProperty, 34.0, BindingPriority.Template);
				button13.SetValue(Button.CommandProperty, SearchCommands.CloseSearchPanel, BindingPriority.Template);
				button13.SetValue(InputElement.FocusableProperty, value: true, BindingPriority.Template);
				Path path22;
				Path path23 = (path22 = new Path());
				((ISupportInitialize)path23).BeginInit();
				button13.Content = path23;
				Path path24 = (path3 = path22);
				context.PushParent(path3);
				Path path25 = path3;
				StaticResourceExtension staticResourceExtension10 = new StaticResourceExtension("CloseIconPath");
				context.ProvideTargetProperty = Path.DataProperty;
				object? obj14 = staticResourceExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_29.DynamicSetter_2(path25, BindingPriority.Template, obj14);
				path25.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				path25.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				path25.SetValue(Shape.StretchProperty, Stretch.Uniform, BindingPriority.Template);
				path25.SetValue(Layoutable.WidthProperty, 12.0, BindingPriority.Template);
				StyledProperty<IBrush?> fillProperty6 = Shape.FillProperty;
				CompiledBindingExtension compiledBindingExtension13 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(Button), 0).Property(TemplatedControl.ForegroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = Shape.FillProperty;
				CompiledBindingExtension binding17 = compiledBindingExtension13.ProvideValue(context);
				context.ProvideTargetProperty = null;
				path25.Bind(fillProperty6, binding17);
				StyledProperty<IBrush?> strokeProperty6 = Shape.StrokeProperty;
				CompiledBindingExtension compiledBindingExtension14 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(Button), 0).Property(TemplatedControl.ForegroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = Shape.StrokeProperty;
				CompiledBindingExtension binding18 = compiledBindingExtension14.ProvideValue(context);
				context.ProvideTargetProperty = null;
				path25.Bind(strokeProperty6, binding18);
				path25.SetValue(Shape.StrokeThicknessProperty, 1.0, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)path24).EndInit();
				context.PopParent();
				((ISupportInitialize)button12).EndInit();
				context.PopParent();
				((ISupportInitialize)stackPanel4).EndInit();
				Controls children10 = grid3.Children;
				StackPanel stackPanel10;
				StackPanel stackPanel11 = (stackPanel10 = new StackPanel());
				((ISupportInitialize)stackPanel11).BeginInit();
				children10.Add(stackPanel11);
				StackPanel stackPanel12 = (stackPanel3 = stackPanel10);
				context.PushParent(stackPanel3);
				StackPanel stackPanel13 = stackPanel3;
				stackPanel13.Name = "ReplaceButtons";
				service = stackPanel13;
				context.AvaloniaNameScope.Register("ReplaceButtons", service);
				StyledProperty<bool> isVisibleProperty2 = Visual.IsVisibleProperty;
				CompiledBindingExtension compiledBindingExtension15 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Expander").Property(ToggleButton.IsCheckedProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = Visual.IsVisibleProperty;
				CompiledBindingExtension binding19 = compiledBindingExtension15.ProvideValue(context);
				context.ProvideTargetProperty = null;
				stackPanel13.Bind(isVisibleProperty2, binding19);
				stackPanel13.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				stackPanel13.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 5.0, 0.0, 0.0), BindingPriority.Template);
				stackPanel13.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				stackPanel13.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
				Controls children11 = stackPanel13.Children;
				TextBox textBox6;
				TextBox textBox7 = (textBox6 = new TextBox());
				((ISupportInitialize)textBox7).BeginInit();
				children11.Add(textBox7);
				TextBox textBox8 = (textBox3 = textBox6);
				context.PushParent(textBox3);
				TextBox textBox9 = textBox3;
				textBox9.Name = "ReplaceBox";
				service = textBox9;
				context.AvaloniaNameScope.Register("ReplaceBox", service);
				textBox9.SetValue(TextBox.WatermarkProperty, SR.ReplaceLabel, BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty3 = Visual.IsVisibleProperty;
				CompiledBindingExtension compiledBindingExtension16 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(SearchPanel.IsReplaceModeProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build());
				context.ProvideTargetProperty = Visual.IsVisibleProperty;
				CompiledBindingExtension binding20 = compiledBindingExtension16.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBox9.Bind(isVisibleProperty3, binding20);
				textBox9.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				textBox9.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
				textBox9.SetValue(Layoutable.WidthProperty, 265.0, BindingPriority.Template);
				textBox9.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				StyledProperty<string?> textProperty2 = TextBox.TextProperty;
				CompiledBindingExtension compiledBindingExtension17 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(SearchPanel.ReplacePatternProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build());
				context.ProvideTargetProperty = TextBox.TextProperty;
				CompiledBindingExtension binding21 = compiledBindingExtension17.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBox9.Bind(textProperty2, binding21);
				context.PopParent();
				((ISupportInitialize)textBox8).EndInit();
				Controls children12 = stackPanel13.Children;
				Button button14;
				Button button15 = (button14 = new Button());
				((ISupportInitialize)button15).BeginInit();
				children12.Add(button15);
				Button button16 = (button3 = button14);
				context.PushParent(button3);
				Button button17 = button3;
				button17.SetValue(Layoutable.MarginProperty, new Thickness(5.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				button17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				button17.SetValue(Layoutable.MinWidthProperty, 34.0, BindingPriority.Template);
				button17.SetValue(Button.CommandProperty, SearchCommands.ReplaceNext, BindingPriority.Template);
				AttachedProperty<object?> tipProperty4 = ToolTip.TipProperty;
				TextBlock textBlock7;
				TextBlock textBlock8 = (textBlock7 = new TextBlock());
				((ISupportInitialize)textBlock8).BeginInit();
				button17.SetValue(tipProperty4, textBlock8, BindingPriority.Template);
				textBlock7.SetValue(TextBlock.TextProperty, SR.SearchReplaceNext, BindingPriority.Template);
				((ISupportInitialize)textBlock7).EndInit();
				Path path26;
				Path path27 = (path26 = new Path());
				((ISupportInitialize)path27).BeginInit();
				button17.Content = path27;
				Path path28 = (path3 = path26);
				context.PushParent(path3);
				Path path29 = path3;
				StaticResourceExtension staticResourceExtension11 = new StaticResourceExtension("ReplaceNextIconPath");
				context.ProvideTargetProperty = Path.DataProperty;
				object? obj15 = staticResourceExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_29.DynamicSetter_2(path29, BindingPriority.Template, obj15);
				path29.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				path29.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				path29.SetValue(Shape.StretchProperty, Stretch.Uniform, BindingPriority.Template);
				path29.SetValue(Layoutable.WidthProperty, 12.0, BindingPriority.Template);
				StyledProperty<IBrush?> fillProperty7 = Shape.FillProperty;
				CompiledBindingExtension compiledBindingExtension18 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(Button), 0).Property(TemplatedControl.ForegroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = Shape.FillProperty;
				CompiledBindingExtension binding22 = compiledBindingExtension18.ProvideValue(context);
				context.ProvideTargetProperty = null;
				path29.Bind(fillProperty7, binding22);
				path29.SetValue(Shape.StrokeThicknessProperty, 1.0, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)path28).EndInit();
				context.PopParent();
				((ISupportInitialize)button16).EndInit();
				Controls children13 = stackPanel13.Children;
				Button button18;
				Button button19 = (button18 = new Button());
				((ISupportInitialize)button19).BeginInit();
				children13.Add(button19);
				Button button20 = (button3 = button18);
				context.PushParent(button3);
				Button button21 = button3;
				button21.SetValue(Layoutable.MarginProperty, new Thickness(3.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				button21.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				button21.SetValue(Layoutable.MinWidthProperty, 34.0, BindingPriority.Template);
				button21.SetValue(Button.CommandProperty, SearchCommands.ReplaceAll, BindingPriority.Template);
				AttachedProperty<object?> tipProperty5 = ToolTip.TipProperty;
				TextBlock textBlock9;
				TextBlock textBlock10 = (textBlock9 = new TextBlock());
				((ISupportInitialize)textBlock10).BeginInit();
				button21.SetValue(tipProperty5, textBlock10, BindingPriority.Template);
				textBlock9.SetValue(TextBlock.TextProperty, SR.SearchReplaceAll, BindingPriority.Template);
				((ISupportInitialize)textBlock9).EndInit();
				Path path30;
				Path path31 = (path30 = new Path());
				((ISupportInitialize)path31).BeginInit();
				button21.Content = path31;
				Path path32 = (path3 = path30);
				context.PushParent(path3);
				Path path33 = path3;
				StaticResourceExtension staticResourceExtension12 = new StaticResourceExtension("ReplaceAllIconPath");
				context.ProvideTargetProperty = Path.DataProperty;
				object? obj16 = staticResourceExtension12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_29.DynamicSetter_2(path33, BindingPriority.Template, obj16);
				path33.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				path33.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				path33.SetValue(Shape.StretchProperty, Stretch.Uniform, BindingPriority.Template);
				path33.SetValue(Layoutable.WidthProperty, 18.0, BindingPriority.Template);
				StyledProperty<IBrush?> fillProperty8 = Shape.FillProperty;
				CompiledBindingExtension compiledBindingExtension19 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(Button), 0).Property(TemplatedControl.ForegroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = Shape.FillProperty;
				CompiledBindingExtension binding23 = compiledBindingExtension19.ProvideValue(context);
				context.ProvideTargetProperty = null;
				path33.Bind(fillProperty8, binding23);
				context.PopParent();
				((ISupportInitialize)path32).EndInit();
				context.PopParent();
				((ISupportInitialize)button20).EndInit();
				context.PopParent();
				((ISupportInitialize)stackPanel12).EndInit();
				Controls children14 = grid3.Children;
				StackPanel stackPanel14;
				StackPanel stackPanel15 = (stackPanel14 = new StackPanel());
				((ISupportInitialize)stackPanel15).BeginInit();
				children14.Add(stackPanel15);
				stackPanel14.Name = "PART_MessageView";
				service = stackPanel14;
				context.AvaloniaNameScope.Register("PART_MessageView", service);
				stackPanel14.SetValue(Visual.IsVisibleProperty, value: false, BindingPriority.Template);
				stackPanel14.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				stackPanel14.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 5.0, 0.0, 0.0), BindingPriority.Template);
				stackPanel14.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				stackPanel14.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				Controls children15 = stackPanel14.Children;
				TextBlock textBlock11;
				TextBlock textBlock12 = (textBlock11 = new TextBlock());
				((ISupportInitialize)textBlock12).BeginInit();
				children15.Add(textBlock12);
				textBlock11.Name = "PART_MessageContent";
				service = textBlock11;
				context.AvaloniaNameScope.Register("PART_MessageContent", service);
				textBlock11.SetValue(TextBlock.FontSizeProperty, 11.0, BindingPriority.Template);
				((ISupportInitialize)textBlock11).EndInit();
				((ISupportInitialize)stackPanel14).EndInit();
				context.PopParent();
				((ISupportInitialize)grid4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FSearch_002FSearchPanel_002Examl.Singleton }, "avares://AvaloniaEdit/Search/SearchPanel.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			ControlTheme controlTheme;
			ControlTheme result = (controlTheme = new ControlTheme());
			context.PushParent(controlTheme);
			controlTheme.TargetType = typeof(SearchPanel);
			Setter setter = new Setter();
			setter.Property = TemplatedControl.BorderThicknessProperty;
			setter.Value = new Thickness(1.0, 0.0, 1.0, 1.0);
			controlTheme.Add(setter);
			Setter setter2 = new Setter();
			setter2.Property = TemplatedControl.CornerRadiusProperty;
			setter2.Value = new CornerRadius(0.0, 0.0, 4.0, 4.0);
			controlTheme.Add(setter2);
			Setter setter3;
			Setter setter4 = (setter3 = new Setter());
			context.PushParent(setter3);
			Setter setter5 = setter3;
			setter5.Property = TemplatedControl.BorderBrushProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SearchPanelBorderBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter5.Value = value;
			context.PopParent();
			controlTheme.Add(setter4);
			Setter setter6 = (setter3 = new Setter());
			context.PushParent(setter3);
			Setter setter7 = setter3;
			setter7.Property = TemplatedControl.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("SearchPanelBackgroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value2 = dynamicResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter7.Value = value2;
			context.PopParent();
			controlTheme.Add(setter6);
			Setter setter8 = new Setter();
			setter8.Property = InputElement.FocusableProperty;
			setter8.Value = true;
			controlTheme.Add(setter8);
			Setter setter9 = new Setter();
			setter9.Property = Layoutable.MarginProperty;
			setter9.Value = new Thickness(0.0, 0.0, 18.0, 0.0);
			controlTheme.Add(setter9);
			Setter setter10 = (setter3 = new Setter());
			context.PushParent(setter3);
			Setter setter11 = setter3;
			setter11.Property = TemplatedControl.FontSizeProperty;
			DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("SearchPanelFontSize");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value3 = dynamicResourceExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter11.Value = value3;
			context.PopParent();
			controlTheme.Add(setter10);
			Setter setter12 = (setter3 = new Setter());
			context.PushParent(setter3);
			Setter setter13 = setter3;
			setter13.Property = TemplatedControl.FontFamilyProperty;
			DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("SearchPanelFontFamily");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value4 = dynamicResourceExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter13.Value = value4;
			context.PopParent();
			controlTheme.Add(setter12);
			Setter setter14 = (setter3 = new Setter());
			context.PushParent(setter3);
			Setter setter15 = setter3;
			setter15.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value5 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_28.Build, context);
			context.PopParent();
			setter15.Value = value5;
			context.PopParent();
			controlTheme.Add(setter14);
			Style style = new Style();
			style.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(Button));
			Setter setter16 = new Setter();
			setter16.Property = TemplatedControl.BorderThicknessProperty;
			setter16.Value = new Thickness(0.0, 0.0, 0.0, 0.0);
			style.Add(setter16);
			controlTheme.Add(style);
			context.PopParent();
			return result;
		}
	}

	public class NamespaceInfo_003A_002FTextEditor_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[31]
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
				CreateNamespaceInfo("Avalonia.Markup.Xaml.Templates", "Avalonia.Markup.Xaml")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("avaloniaedit", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("AvaloniaEdit", "AvaloniaEdit") });
			return dictionary;
		}

		static NamespaceInfo_003A_002FTextEditor_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FTextEditor_002Examl();
		}
	}

	private class XamlClosure_30
	{
		private class XamlClosure_31
		{
			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FTextEditor_002Examl.Singleton }, "avares://AvaloniaEdit/TextEditor.xaml");
				object service;
				if (P_0 != null)
				{
					service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (ResourceDictionary)service;
					}
				}
				context.IntermediateRoot = new Border();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				((AvaloniaObject)intermediateRoot).Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				ScrollViewer scrollViewer;
				ScrollViewer scrollViewer2 = (scrollViewer = new ScrollViewer());
				((ISupportInitialize)scrollViewer2).BeginInit();
				((Decorator)intermediateRoot).Child = scrollViewer2;
				scrollViewer.SetValue(InputElement.FocusableProperty, value: false, BindingPriority.Template);
				scrollViewer.Name = "PART_ScrollViewer";
				service = scrollViewer;
				context.AvaloniaNameScope.Register("PART_ScrollViewer", service);
				scrollViewer.Bind(ScrollViewer.VerticalScrollBarVisibilityProperty, new TemplateBinding(TextEditor.VerticalScrollBarVisibilityProperty).ProvideValue());
				scrollViewer.Bind(ScrollViewer.HorizontalScrollBarVisibilityProperty, new TemplateBinding(TextEditor.HorizontalScrollBarVisibilityProperty).ProvideValue());
				scrollViewer.SetValue(ContentControl.VerticalContentAlignmentProperty, VerticalAlignment.Top, BindingPriority.Template);
				scrollViewer.SetValue(ContentControl.HorizontalContentAlignmentProperty, HorizontalAlignment.Left, BindingPriority.Template);
				scrollViewer.Bind(TemplatedControl.PaddingProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				((ISupportInitialize)scrollViewer).EndInit();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FTextEditor_002Examl.Singleton }, "avares://AvaloniaEdit/TextEditor.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			ControlTheme controlTheme = new ControlTheme();
			controlTheme.TargetType = typeof(TextEditor);
			Setter setter = new Setter();
			setter.Property = TemplatedControl.TemplateProperty;
			setter.Value = new ControlTemplate
			{
				Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_31.Build, context)
			};
			controlTheme.Add(setter);
			return controlTheme;
		}
	}

	public class NamespaceInfo_003A_002FThemes_002FFluent_002FAvaloniaEdit_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> dictionary = new Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>>(1);
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[31]
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
				CreateNamespaceInfo("Avalonia.Markup.Xaml.Templates", "Avalonia.Markup.Xaml")
			});
			return dictionary;
		}

		static NamespaceInfo_003A_002FThemes_002FFluent_002FAvaloniaEdit_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FThemes_002FFluent_002FAvaloniaEdit_002Examl();
		}
	}

	public class NamespaceInfo_003A_002FThemes_002FFluent_002FBase_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> dictionary = new Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>>(2);
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[31]
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
				CreateNamespaceInfo("Avalonia.Markup.Xaml.Templates", "Avalonia.Markup.Xaml")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			return dictionary;
		}

		static NamespaceInfo_003A_002FThemes_002FFluent_002FBase_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FThemes_002FFluent_002FBase_002Examl();
		}
	}

	private class XamlClosure_32
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Fluent/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ToolTipBackground"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_33
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Fluent/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ToolTipForeground"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_34
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Fluent/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ToolTipBorderBrush"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_35
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Fluent/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ToolTipBorderThemeThickness"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_36
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Fluent/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
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

	private class XamlClosure_37
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Fluent/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemBaseLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_38
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Fluent/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ToolTipBackground"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_39
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Fluent/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ToolTipForeground"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_40
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Fluent/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ToolTipBorderBrush"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_41
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Fluent/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ToolTipBorderThemeThickness"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_42
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Fluent/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
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

	private class XamlClosure_43
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Fluent/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemBaseLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_44
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Fluent/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ControlContentThemeFontSize"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_45
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Fluent/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ContentControlThemeFontFamily"
			}.ProvideValue(context);
		}
	}

	public class NamespaceInfo_003A_002FThemes_002FSimple_002FAvaloniaEdit_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> dictionary = new Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>>(1);
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[31]
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
				CreateNamespaceInfo("Avalonia.Markup.Xaml.Templates", "Avalonia.Markup.Xaml")
			});
			return dictionary;
		}

		static NamespaceInfo_003A_002FThemes_002FSimple_002FAvaloniaEdit_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FThemes_002FSimple_002FAvaloniaEdit_002Examl();
		}
	}

	public class NamespaceInfo_003A_002FThemes_002FSimple_002FBase_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> dictionary = new Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>>(2);
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[31]
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
				CreateNamespaceInfo("Avalonia.Markup.Xaml.Templates", "Avalonia.Markup.Xaml")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			return dictionary;
		}

		static NamespaceInfo_003A_002FThemes_002FSimple_002FBase_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FThemes_002FSimple_002FBase_002Examl();
		}
	}

	private class XamlClosure_46
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Simple/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ThemeBackgroundBrush"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_47
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Simple/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ThemeForegroundColor"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_48
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Simple/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ThemeBorderMidBrush"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_49
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Simple/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ThemeBorderThickness"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_50
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Simple/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeBackgroundColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_51
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Simple/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeBorderLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_52
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Simple/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ThemeBackgroundBrush"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_53
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Simple/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ThemeForegroundColor"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_54
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Simple/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ThemeBorderMidBrush"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_55
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Simple/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ThemeBorderThickness"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_56
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Simple/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeBackgroundColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_57
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Simple/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			SolidColorBrush solidColorBrush;
			SolidColorBrush result = (solidColorBrush = new SolidColorBrush());
			context.PushParent(solidColorBrush);
			StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeBorderLowColor");
			context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
			IBinding binding = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			solidColorBrush.Bind(colorProperty, binding);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_58
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Simple/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "FontSizeNormal"
			}.ProvideValue(context);
		}
	}

	private class XamlClosure_59
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Simple/Base.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ResourceDictionary)service;
				}
			}
			return new StaticResourceExtension
			{
				ResourceKey = "ContentControlThemeFontFamily"
			}.ProvideValue(context);
		}
	}

	public static void Populate_003A_002FCodeCompletion_002FCompletionList_002Examl(IServiceProvider P_0, ResourceDictionary P_1)
	{
		XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FCodeCompletion_002FCompletionList_002Examl.Singleton }, "avares://AvaloniaEdit/CodeCompletion/CompletionList.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		ResourceDictionary resourceDictionary;
		ResourceDictionary resourceDictionary2 = (resourceDictionary = P_1);
		context.PushParent(resourceDictionary);
		resourceDictionary.AddDeferred(typeof(CompletionListBox), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_1.Build, context));
		resourceDictionary.AddDeferred(typeof(CompletionList), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_2.Build, context));
		context.PopParent();
		if (resourceDictionary2 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	public static ResourceDictionary Build_003A_002FCodeCompletion_002FCompletionList_002Examl(IServiceProvider P_0)
	{
		ResourceDictionary resourceDictionary = new ResourceDictionary();
		Populate_003A_002FCodeCompletion_002FCompletionList_002Examl(P_0, resourceDictionary);
		return resourceDictionary;
	}

	public static void Populate_003A_002FCodeCompletion_002FCompletionWindow_002Examl(IServiceProvider P_0, Styles P_1)
	{
		XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FCodeCompletion_002FCompletionWindow_002Examl.Singleton }, "avares://AvaloniaEdit/CodeCompletion/CompletionWindow.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		Styles styles;
		Styles styles2 = (styles = P_1);
		context.PushParent(styles);
		Style style;
		Style item = (style = new Style());
		context.PushParent(style);
		style.Selector = ((Selector?)null).OfType(typeof(ContentControl)).Class("ToolTip");
		Setter setter;
		Setter setter2 = (setter = new Setter());
		context.PushParent(setter);
		Setter setter3 = setter;
		setter3.Property = TemplatedControl.BorderThicknessProperty;
		DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("CompletionToolTipBorderThickness");
		context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
		IBinding value = dynamicResourceExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		setter3.Value = value;
		context.PopParent();
		style.Add(setter2);
		Setter setter4 = (setter = new Setter());
		context.PushParent(setter);
		Setter setter5 = setter;
		setter5.Property = TemplatedControl.BorderBrushProperty;
		DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("CompletionToolTipBorderBrush");
		context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
		IBinding value2 = dynamicResourceExtension2.ProvideValue(context);
		context.ProvideTargetProperty = null;
		setter5.Value = value2;
		context.PopParent();
		style.Add(setter4);
		Setter setter6 = (setter = new Setter());
		context.PushParent(setter);
		Setter setter7 = setter;
		setter7.Property = TemplatedControl.BackgroundProperty;
		DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("CompletionToolTipBackground");
		context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
		IBinding value3 = dynamicResourceExtension3.ProvideValue(context);
		context.ProvideTargetProperty = null;
		setter7.Value = value3;
		context.PopParent();
		style.Add(setter6);
		Setter setter8 = (setter = new Setter());
		context.PushParent(setter);
		Setter setter9 = setter;
		setter9.Property = TemplatedControl.ForegroundProperty;
		DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("CompletionToolTipForeground");
		context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
		IBinding value4 = dynamicResourceExtension4.ProvideValue(context);
		context.ProvideTargetProperty = null;
		setter9.Value = value4;
		context.PopParent();
		style.Add(setter8);
		Setter setter10 = new Setter();
		setter10.Property = TemplatedControl.PaddingProperty;
		setter10.Value = new Thickness(4.0, 2.0, 4.0, 2.0);
		style.Add(setter10);
		context.PopParent();
		styles.Add(item);
		context.PopParent();
		if (styles2 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	public static Styles Build_003A_002FCodeCompletion_002FCompletionWindow_002Examl(IServiceProvider P_0)
	{
		Styles styles = new Styles();
		Populate_003A_002FCodeCompletion_002FCompletionWindow_002Examl(P_0, styles);
		return styles;
	}

	public static void Populate_003A_002FCodeCompletion_002FInsightWindow_002Examl(IServiceProvider P_0, ResourceDictionary P_1)
	{
		XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FCodeCompletion_002FInsightWindow_002Examl.Singleton }, "avares://AvaloniaEdit/CodeCompletion/InsightWindow.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		ResourceDictionary resourceDictionary;
		ResourceDictionary resourceDictionary2 = (resourceDictionary = P_1);
		context.PushParent(resourceDictionary);
		resourceDictionary.AddDeferred(typeof(OverloadViewer), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_6.Build, context));
		context.PopParent();
		if (resourceDictionary2 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	public static ResourceDictionary Build_003A_002FCodeCompletion_002FInsightWindow_002Examl(IServiceProvider P_0)
	{
		ResourceDictionary resourceDictionary = new ResourceDictionary();
		Populate_003A_002FCodeCompletion_002FInsightWindow_002Examl(P_0, resourceDictionary);
		return resourceDictionary;
	}

	public static void Populate_003A_002FEditing_002FTextArea_002Examl(IServiceProvider P_0, ResourceDictionary P_1)
	{
		XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FEditing_002FTextArea_002Examl.Singleton }, "avares://AvaloniaEdit/Editing/TextArea.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		P_1.AddDeferred(typeof(TextArea), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_11.Build, context));
		if (P_1 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	public static ResourceDictionary Build_003A_002FEditing_002FTextArea_002Examl(IServiceProvider P_0)
	{
		ResourceDictionary resourceDictionary = new ResourceDictionary();
		Populate_003A_002FEditing_002FTextArea_002Examl(P_0, resourceDictionary);
		return resourceDictionary;
	}

	public static void Populate_003A_002FRendering_002FVisualLineDrawingVisual_002Examl(IServiceProvider P_0, Styles P_1)
	{
		XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FRendering_002FVisualLineDrawingVisual_002Examl.Singleton }, "avares://AvaloniaEdit/Rendering/VisualLineDrawingVisual.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		Styles styles;
		Styles styles2 = (styles = P_1);
		context.PushParent(styles);
		Style style;
		Style item = (style = new Style());
		context.PushParent(style);
		style.Selector = ((Selector?)null).OfType(typeof(VisualLineDrawingVisual));
		Setter setter;
		Setter setter2 = (setter = new Setter());
		context.PushParent(setter);
		setter.Property = InputElement.CursorProperty;
		ReflectionBindingExtension reflectionBindingExtension = new ReflectionBindingExtension("$parent.Cursor");
		context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
		Binding value = reflectionBindingExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		setter.Value = value;
		context.PopParent();
		style.Add(setter2);
		context.PopParent();
		styles.Add(item);
		context.PopParent();
		if (styles2 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	public static Styles Build_003A_002FRendering_002FVisualLineDrawingVisual_002Examl(IServiceProvider P_0)
	{
		Styles styles = new Styles();
		Populate_003A_002FRendering_002FVisualLineDrawingVisual_002Examl(P_0, styles);
		return styles;
	}

	public static void Populate_003A_002FSearch_002FSearchPanel_002Examl(IServiceProvider P_0, ResourceDictionary P_1)
	{
		XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FSearch_002FSearchPanel_002Examl.Singleton }, "avares://AvaloniaEdit/Search/SearchPanel.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		ResourceDictionary resourceDictionary;
		ResourceDictionary resourceDictionary2 = (resourceDictionary = P_1);
		context.PushParent(resourceDictionary);
		resourceDictionary.AddDeferred("ChevronRightIconPath", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_14.Build, context));
		resourceDictionary.AddDeferred("CaseSensitiveIconPath", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_15.Build, context));
		resourceDictionary.AddDeferred("CompleteWordIconPath", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_16.Build, context));
		resourceDictionary.AddDeferred("RegularExpressionIconPath", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_17.Build, context));
		resourceDictionary.AddDeferred("FindPreviousIconPath", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_18.Build, context));
		resourceDictionary.AddDeferred("FindNextIconPath", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_19.Build, context));
		resourceDictionary.AddDeferred("CloseIconPath", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_20.Build, context));
		resourceDictionary.AddDeferred("ReplaceNextIconPath", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_21.Build, context));
		resourceDictionary.AddDeferred("ReplaceAllIconPath", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_22.Build, context));
		resourceDictionary.AddDeferred("SearchPanelExpanderToggle", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_23.Build, context));
		resourceDictionary.AddDeferred("SearchPanelTextBoxButton", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_26.Build, context));
		resourceDictionary.AddDeferred(typeof(SearchPanel), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_27.Build, context));
		context.PopParent();
		if (resourceDictionary2 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	public static ResourceDictionary Build_003A_002FSearch_002FSearchPanel_002Examl(IServiceProvider P_0)
	{
		ResourceDictionary resourceDictionary = new ResourceDictionary();
		Populate_003A_002FSearch_002FSearchPanel_002Examl(P_0, resourceDictionary);
		return resourceDictionary;
	}

	public static void Populate_003A_002FTextEditor_002Examl(IServiceProvider P_0, ResourceDictionary P_1)
	{
		XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FTextEditor_002Examl.Singleton }, "avares://AvaloniaEdit/TextEditor.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		P_1.AddDeferred(typeof(TextEditor), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_30.Build, context));
		if (P_1 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	public static ResourceDictionary Build_003A_002FTextEditor_002Examl(IServiceProvider P_0)
	{
		ResourceDictionary resourceDictionary = new ResourceDictionary();
		Populate_003A_002FTextEditor_002Examl(P_0, resourceDictionary);
		return resourceDictionary;
	}

	public static void Populate_003A_002FThemes_002FFluent_002FAvaloniaEdit_002Examl(IServiceProvider P_0, Styles P_1)
	{
		XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FAvaloniaEdit_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Fluent/AvaloniaEdit.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		Styles styles;
		Styles styles2 = (styles = P_1);
		context.PushParent(styles);
		ResourceDictionary resourceDictionary;
		ResourceDictionary resources = (resourceDictionary = new ResourceDictionary());
		context.PushParent(resourceDictionary);
		resourceDictionary.MergedDictionaries.Add(Build_003A_002FThemes_002FFluent_002FBase_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		resourceDictionary.MergedDictionaries.Add(Build_003A_002FTextEditor_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		resourceDictionary.MergedDictionaries.Add(Build_003A_002FEditing_002FTextArea_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		resourceDictionary.MergedDictionaries.Add(Build_003A_002FCodeCompletion_002FCompletionList_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		resourceDictionary.MergedDictionaries.Add(Build_003A_002FCodeCompletion_002FInsightWindow_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		resourceDictionary.MergedDictionaries.Add(Build_003A_002FSearch_002FSearchPanel_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		context.PopParent();
		styles.Resources = resources;
		styles.Add(Build_003A_002FCodeCompletion_002FCompletionWindow_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		styles.Add(Build_003A_002FRendering_002FVisualLineDrawingVisual_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		context.PopParent();
		if (styles2 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	public static Styles Build_003A_002FThemes_002FFluent_002FAvaloniaEdit_002Examl(IServiceProvider P_0)
	{
		Styles styles = new Styles();
		Populate_003A_002FThemes_002FFluent_002FAvaloniaEdit_002Examl(P_0, styles);
		return styles;
	}

	public static void Populate_003A_002FThemes_002FFluent_002FBase_002Examl(IServiceProvider P_0, ResourceDictionary P_1)
	{
		XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Fluent/Base.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		ResourceDictionary resourceDictionary;
		ResourceDictionary resourceDictionary2 = (resourceDictionary = P_1);
		context.PushParent(resourceDictionary);
		ResourceDictionary resourceDictionary3 = resourceDictionary;
		IDictionary<ThemeVariant, IThemeVariantProvider> themeDictionaries = resourceDictionary3.ThemeDictionaries;
		ThemeVariant @default = ThemeVariant.Default;
		ResourceDictionary value = (resourceDictionary = new ResourceDictionary());
		context.PushParent(resourceDictionary);
		ResourceDictionary resourceDictionary4 = resourceDictionary;
		((IThemeVariantProvider)resourceDictionary4).Key = ThemeVariant.Default;
		resourceDictionary4.AddDeferred("CompletionToolTipBackground", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_32.Build, context));
		resourceDictionary4.AddDeferred("CompletionToolTipForeground", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_33.Build, context));
		resourceDictionary4.AddDeferred("CompletionToolTipBorderBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_34.Build, context));
		resourceDictionary4.AddDeferred("CompletionToolTipBorderThickness", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_35.Build, context));
		resourceDictionary4.AddDeferred("SearchPanelBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_36.Build, context));
		resourceDictionary4.AddDeferred("SearchPanelBorderBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_37.Build, context));
		context.PopParent();
		themeDictionaries.Add(@default, value);
		IDictionary<ThemeVariant, IThemeVariantProvider> themeDictionaries2 = resourceDictionary3.ThemeDictionaries;
		ThemeVariant dark = ThemeVariant.Dark;
		ResourceDictionary value2 = (resourceDictionary = new ResourceDictionary());
		context.PushParent(resourceDictionary);
		ResourceDictionary resourceDictionary5 = resourceDictionary;
		((IThemeVariantProvider)resourceDictionary5).Key = ThemeVariant.Dark;
		resourceDictionary5.AddDeferred("CompletionToolTipBackground", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_38.Build, context));
		resourceDictionary5.AddDeferred("CompletionToolTipForeground", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_39.Build, context));
		resourceDictionary5.AddDeferred("CompletionToolTipBorderBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_40.Build, context));
		resourceDictionary5.AddDeferred("CompletionToolTipBorderThickness", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_41.Build, context));
		resourceDictionary5.AddDeferred("SearchPanelBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_42.Build, context));
		resourceDictionary5.AddDeferred("SearchPanelBorderBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_43.Build, context));
		context.PopParent();
		themeDictionaries2.Add(dark, value2);
		resourceDictionary3.AddDeferred("SearchPanelFontSize", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_44.Build, context));
		resourceDictionary3.AddDeferred("SearchPanelFontFamily", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_45.Build, context));
		context.PopParent();
		if (resourceDictionary2 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	public static ResourceDictionary Build_003A_002FThemes_002FFluent_002FBase_002Examl(IServiceProvider P_0)
	{
		ResourceDictionary resourceDictionary = new ResourceDictionary();
		Populate_003A_002FThemes_002FFluent_002FBase_002Examl(P_0, resourceDictionary);
		return resourceDictionary;
	}

	public static void Populate_003A_002FThemes_002FSimple_002FAvaloniaEdit_002Examl(IServiceProvider P_0, Styles P_1)
	{
		XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FAvaloniaEdit_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Simple/AvaloniaEdit.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		Styles styles;
		Styles styles2 = (styles = P_1);
		context.PushParent(styles);
		ResourceDictionary resourceDictionary;
		ResourceDictionary resources = (resourceDictionary = new ResourceDictionary());
		context.PushParent(resourceDictionary);
		resourceDictionary.MergedDictionaries.Add(Build_003A_002FThemes_002FSimple_002FBase_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		resourceDictionary.MergedDictionaries.Add(Build_003A_002FTextEditor_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		resourceDictionary.MergedDictionaries.Add(Build_003A_002FEditing_002FTextArea_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		resourceDictionary.MergedDictionaries.Add(Build_003A_002FCodeCompletion_002FCompletionList_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		resourceDictionary.MergedDictionaries.Add(Build_003A_002FCodeCompletion_002FInsightWindow_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		resourceDictionary.MergedDictionaries.Add(Build_003A_002FSearch_002FSearchPanel_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		context.PopParent();
		styles.Resources = resources;
		styles.Add(Build_003A_002FCodeCompletion_002FCompletionWindow_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		styles.Add(Build_003A_002FRendering_002FVisualLineDrawingVisual_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		context.PopParent();
		if (styles2 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	public static Styles Build_003A_002FThemes_002FSimple_002FAvaloniaEdit_002Examl(IServiceProvider P_0)
	{
		Styles styles = new Styles();
		Populate_003A_002FThemes_002FSimple_002FAvaloniaEdit_002Examl(P_0, styles);
		return styles;
	}

	public static void Populate_003A_002FThemes_002FSimple_002FBase_002Examl(IServiceProvider P_0, ResourceDictionary P_1)
	{
		XamlIlContext.Context<ResourceDictionary> context = new XamlIlContext.Context<ResourceDictionary>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FBase_002Examl.Singleton }, "avares://AvaloniaEdit/Themes/Simple/Base.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		ResourceDictionary resourceDictionary;
		ResourceDictionary resourceDictionary2 = (resourceDictionary = P_1);
		context.PushParent(resourceDictionary);
		ResourceDictionary resourceDictionary3 = resourceDictionary;
		IDictionary<ThemeVariant, IThemeVariantProvider> themeDictionaries = resourceDictionary3.ThemeDictionaries;
		ThemeVariant @default = ThemeVariant.Default;
		ResourceDictionary value = (resourceDictionary = new ResourceDictionary());
		context.PushParent(resourceDictionary);
		ResourceDictionary resourceDictionary4 = resourceDictionary;
		((IThemeVariantProvider)resourceDictionary4).Key = ThemeVariant.Default;
		resourceDictionary4.AddDeferred("CompletionToolTipBackground", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_46.Build, context));
		resourceDictionary4.AddDeferred("CompletionToolTipForeground", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_47.Build, context));
		resourceDictionary4.AddDeferred("CompletionToolTipBorderBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_48.Build, context));
		resourceDictionary4.AddDeferred("CompletionToolTipBorderThickness", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_49.Build, context));
		resourceDictionary4.AddDeferred("SearchPanelBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_50.Build, context));
		resourceDictionary4.AddDeferred("SearchPanelBorderBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_51.Build, context));
		context.PopParent();
		themeDictionaries.Add(@default, value);
		IDictionary<ThemeVariant, IThemeVariantProvider> themeDictionaries2 = resourceDictionary3.ThemeDictionaries;
		ThemeVariant dark = ThemeVariant.Dark;
		ResourceDictionary value2 = (resourceDictionary = new ResourceDictionary());
		context.PushParent(resourceDictionary);
		ResourceDictionary resourceDictionary5 = resourceDictionary;
		((IThemeVariantProvider)resourceDictionary5).Key = ThemeVariant.Dark;
		resourceDictionary5.AddDeferred("CompletionToolTipBackground", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_52.Build, context));
		resourceDictionary5.AddDeferred("CompletionToolTipForeground", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_53.Build, context));
		resourceDictionary5.AddDeferred("CompletionToolTipBorderBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_54.Build, context));
		resourceDictionary5.AddDeferred("CompletionToolTipBorderThickness", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_55.Build, context));
		resourceDictionary5.AddDeferred("SearchPanelBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_56.Build, context));
		resourceDictionary5.AddDeferred("SearchPanelBorderBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_57.Build, context));
		context.PopParent();
		themeDictionaries2.Add(dark, value2);
		resourceDictionary3.AddDeferred("SearchPanelFontSize", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_58.Build, context));
		resourceDictionary3.AddDeferred("SearchPanelFontFamily", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_59.Build, context));
		context.PopParent();
		if (resourceDictionary2 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	public static ResourceDictionary Build_003A_002FThemes_002FSimple_002FBase_002Examl(IServiceProvider P_0)
	{
		ResourceDictionary resourceDictionary = new ResourceDictionary();
		Populate_003A_002FThemes_002FSimple_002FBase_002Examl(P_0, resourceDictionary);
		return resourceDictionary;
	}
}
