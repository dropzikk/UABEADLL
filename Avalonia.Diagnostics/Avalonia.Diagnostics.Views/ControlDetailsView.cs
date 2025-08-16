using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Diagnostics.Behaviors;
using Avalonia.Diagnostics.Controls;
using Avalonia.Diagnostics.Converters;
using Avalonia.Diagnostics.ViewModels;
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
using CompiledAvaloniaXaml;

namespace Avalonia.Diagnostics.Views;

internal class ControlDetailsView : UserControl
{
	private class XamlClosure_11
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FControlDetailsView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/ControlDetailsView.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ControlDetailsView)service;
				}
			}
			return new BoolToOpacityConverter
			{
				Opacity = 0.6
			};
		}
	}

	private class XamlClosure_12
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FControlDetailsView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/ControlDetailsView.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ControlDetailsView)service;
				}
			}
			return new GetTypeNameConverter();
		}
	}

	private class DynamicSetters_13
	{
		public static void DynamicSetter_1(StyledElement P_0, CompiledBindingExtension P_1)
		{
			if (P_1 != null)
			{
				IBinding binding = P_1;
				P_0.Bind(StyledElement.DataContextProperty, binding);
			}
			else
			{
				P_0.DataContext = P_1;
			}
		}

		public static void DynamicSetter_2(DataGrid P_0, CompiledBindingExtension P_1)
		{
			if (P_1 != null)
			{
				IBinding binding = P_1;
				P_0.Bind(DataGrid.SelectedItemProperty, binding);
			}
			else
			{
				P_0.SelectedItem = P_1;
			}
		}
	}

	private class XamlClosure_14
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FControlDetailsView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/ControlDetailsView.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ControlDetailsView)service;
				}
			}
			context.IntermediateRoot = new PropertyValueEditorView();
			object intermediateRoot = context.IntermediateRoot;
			((ISupportInitialize)intermediateRoot).BeginInit();
			((ISupportInitialize)intermediateRoot).EndInit();
			return intermediateRoot;
		}
	}

	private class XamlClosure_15
	{
		private class XamlClosure_16
		{
			public static object Build(IServiceProvider P_0)
			{
				CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FControlDetailsView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/ControlDetailsView.xaml");
				if (P_0 != null)
				{
					object service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (ControlDetailsView)service;
					}
				}
				context.IntermediateRoot = new StackPanel();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				StackPanel stackPanel = (StackPanel)intermediateRoot;
				context.PushParent(stackPanel);
				stackPanel.Orientation = Orientation.Horizontal;
				stackPanel.Spacing = 2.0;
				Avalonia.Controls.Controls children = stackPanel.Children;
				Border border;
				Border border2 = (border = new Border());
				((ISupportInitialize)border2).BeginInit();
				children.Add(border2);
				Border border3;
				Border border4 = (border3 = border);
				context.PushParent(border3);
				border3.BorderThickness = new Thickness(1.0, 1.0, 1.0, 1.0);
				border3.BorderBrush = new ImmutableSolidColorBrush(4278190080u);
				StyledProperty<IBrush?> backgroundProperty = Border.BackgroundProperty;
				CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension();
				context.ProvideTargetProperty = Border.BackgroundProperty;
				CompiledBindingExtension binding = compiledBindingExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border3.Bind(backgroundProperty, binding);
				border3.Width = 8.0;
				border3.Height = 8.0;
				context.PopParent();
				((ISupportInitialize)border4).EndInit();
				Avalonia.Controls.Controls children2 = stackPanel.Children;
				TextBlock textBlock;
				TextBlock textBlock2 = (textBlock = new TextBlock());
				((ISupportInitialize)textBlock2).BeginInit();
				children2.Add(textBlock2);
				TextBlock textBlock3;
				TextBlock textBlock4 = (textBlock3 = textBlock);
				context.PushParent(textBlock3);
				StyledProperty<string?> textProperty = TextBlock.TextProperty;
				CompiledBindingExtension compiledBindingExtension2 = new CompiledBindingExtension();
				context.ProvideTargetProperty = TextBlock.TextProperty;
				CompiledBindingExtension binding2 = compiledBindingExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock3.Bind(textProperty, binding2);
				context.PopParent();
				((ISupportInitialize)textBlock4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		private class XamlClosure_17
		{
			public static object Build(IServiceProvider P_0)
			{
				CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FControlDetailsView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/ControlDetailsView.xaml");
				if (P_0 != null)
				{
					object service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (ControlDetailsView)service;
					}
				}
				context.IntermediateRoot = new StackPanel();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				StackPanel stackPanel = (StackPanel)intermediateRoot;
				context.PushParent(stackPanel);
				stackPanel.Orientation = Orientation.Horizontal;
				stackPanel.Spacing = 2.0;
				Avalonia.Controls.Controls children = stackPanel.Children;
				Border border;
				Border border2 = (border = new Border());
				((ISupportInitialize)border2).BeginInit();
				children.Add(border2);
				Border border3;
				Border border4 = (border3 = border);
				context.PushParent(border3);
				border3.BorderThickness = new Thickness(1.0, 1.0, 1.0, 1.0);
				border3.BorderBrush = new ImmutableSolidColorBrush(4278190080u);
				border3.Width = 8.0;
				border3.Height = 8.0;
				SolidColorBrush solidColorBrush;
				SolidColorBrush background = (solidColorBrush = new SolidColorBrush());
				context.PushParent(solidColorBrush);
				StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
				CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension();
				context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
				CompiledBindingExtension binding = compiledBindingExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				solidColorBrush.Bind(colorProperty, binding);
				context.PopParent();
				border3.Background = background;
				context.PopParent();
				((ISupportInitialize)border4).EndInit();
				Avalonia.Controls.Controls children2 = stackPanel.Children;
				TextBlock textBlock;
				TextBlock textBlock2 = (textBlock = new TextBlock());
				((ISupportInitialize)textBlock2).BeginInit();
				children2.Add(textBlock2);
				TextBlock textBlock3;
				TextBlock textBlock4 = (textBlock3 = textBlock);
				context.PushParent(textBlock3);
				StyledProperty<string?> textProperty = TextBlock.TextProperty;
				CompiledBindingExtension compiledBindingExtension2 = new CompiledBindingExtension();
				context.ProvideTargetProperty = TextBlock.TextProperty;
				CompiledBindingExtension binding2 = compiledBindingExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock3.Bind(textProperty, binding2);
				context.PopParent();
				((ISupportInitialize)textBlock4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		private class XamlClosure_18
		{
			private class DynamicSetters_19
			{
				public static void DynamicSetter_1(ToolTip P_0, CompiledBindingExtension P_1)
				{
					if (P_1 != null)
					{
						IBinding binding = P_1;
						P_0.Bind(ToolTip.TipProperty, binding);
					}
					else
					{
						ToolTip.SetTip(P_0, P_1);
					}
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FControlDetailsView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/ControlDetailsView.xaml");
				if (P_0 != null)
				{
					object service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (ControlDetailsView)service;
					}
				}
				context.IntermediateRoot = new Panel();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				Panel panel = (Panel)intermediateRoot;
				context.PushParent(panel);
				StyledProperty<double> opacityProperty = Visual.OpacityProperty;
				CompiledBindingExtension compiledBindingExtension;
				CompiledBindingExtension compiledBindingExtension2 = (compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002CAvalonia_002EDiagnostics_002EIsActive_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build()));
				context.PushParent(compiledBindingExtension);
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("BoolToOpacity");
				context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
				object? obj = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				compiledBindingExtension.Converter = (IValueConverter)obj;
				context.PopParent();
				context.ProvideTargetProperty = Visual.OpacityProperty;
				CompiledBindingExtension binding = compiledBindingExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				panel.Bind(opacityProperty, binding);
				StyledProperty<bool> isVisibleProperty = Visual.IsVisibleProperty;
				CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002CAvalonia_002EDiagnostics_002EIsVisible_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = Visual.IsVisibleProperty;
				CompiledBindingExtension binding2 = compiledBindingExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				panel.Bind(isVisibleProperty, binding2);
				panel.HorizontalAlignment = HorizontalAlignment.Left;
				ContextMenu contextMenu;
				ContextMenu contextMenu2 = (contextMenu = new ContextMenu());
				((ISupportInitialize)contextMenu2).BeginInit();
				panel.ContextMenu = contextMenu2;
				ContextMenu contextMenu3;
				ContextMenu contextMenu4 = (contextMenu3 = contextMenu);
				context.PushParent(contextMenu3);
				ItemCollection items = contextMenu3.Items;
				MenuItem menuItem;
				MenuItem menuItem2 = (menuItem = new MenuItem());
				((ISupportInitialize)menuItem2).BeginInit();
				items.Add(menuItem2);
				MenuItem menuItem3;
				MenuItem menuItem4 = (menuItem3 = menuItem);
				context.PushParent(menuItem3);
				MenuItem menuItem5 = menuItem3;
				menuItem5.Header = "Copy property name";
				StyledProperty<ICommand?> commandProperty = MenuItem.CommandProperty;
				CompiledBindingExtension compiledBindingExtension4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("CopyPropertyName", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002BCopyPropertyName_0_0021CommandExecuteTrampoline, null, null).Build());
				context.ProvideTargetProperty = MenuItem.CommandProperty;
				CompiledBindingExtension binding3 = compiledBindingExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				menuItem5.Bind(commandProperty, binding3);
				context.PopParent();
				((ISupportInitialize)menuItem4).EndInit();
				ItemCollection items2 = contextMenu3.Items;
				MenuItem menuItem6;
				MenuItem menuItem7 = (menuItem6 = new MenuItem());
				((ISupportInitialize)menuItem7).BeginInit();
				items2.Add(menuItem7);
				MenuItem menuItem8 = (menuItem3 = menuItem6);
				context.PushParent(menuItem3);
				MenuItem menuItem9 = menuItem3;
				menuItem9.Header = "Copy value";
				StyledProperty<ICommand?> commandProperty2 = MenuItem.CommandProperty;
				CompiledBindingExtension compiledBindingExtension5 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("CopyValue", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EBindingSetterViewModel_002BCopyValue_0_0021CommandExecuteTrampoline, null, null).Build());
				context.ProvideTargetProperty = MenuItem.CommandProperty;
				CompiledBindingExtension binding4 = compiledBindingExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				menuItem9.Bind(commandProperty2, binding4);
				context.PopParent();
				((ISupportInitialize)menuItem8).EndInit();
				context.PopParent();
				((ISupportInitialize)contextMenu4).EndInit();
				Avalonia.Controls.Controls children = panel.Children;
				StackPanel stackPanel;
				StackPanel stackPanel2 = (stackPanel = new StackPanel());
				((ISupportInitialize)stackPanel2).BeginInit();
				children.Add(stackPanel2);
				StackPanel stackPanel3;
				StackPanel stackPanel4 = (stackPanel3 = stackPanel);
				context.PushParent(stackPanel3);
				stackPanel3.Orientation = Orientation.Horizontal;
				stackPanel3.Spacing = 2.0;
				Avalonia.Controls.Controls children2 = stackPanel3.Children;
				TextBlock textBlock;
				TextBlock textBlock2 = (textBlock = new TextBlock());
				((ISupportInitialize)textBlock2).BeginInit();
				children2.Add(textBlock2);
				TextBlock textBlock3;
				TextBlock textBlock4 = (textBlock3 = textBlock);
				context.PushParent(textBlock3);
				TextBlock textBlock5 = textBlock3;
				textBlock5.Classes.Add("property-name");
				textBlock5.PointerPressed += context.RootObject.PropertyNamePressed;
				StyledProperty<string?> textProperty = TextBlock.TextProperty;
				CompiledBindingExtension compiledBindingExtension6 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002CAvalonia_002EDiagnostics_002EName_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = TextBlock.TextProperty;
				CompiledBindingExtension binding5 = compiledBindingExtension6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock5.Bind(textProperty, binding5);
				context.PopParent();
				((ISupportInitialize)textBlock4).EndInit();
				Avalonia.Controls.Controls children3 = stackPanel3.Children;
				TextBlock textBlock6;
				TextBlock textBlock7 = (textBlock6 = new TextBlock());
				((ISupportInitialize)textBlock7).BeginInit();
				children3.Add(textBlock7);
				textBlock6.Text = ":";
				((ISupportInitialize)textBlock6).EndInit();
				Avalonia.Controls.Controls children4 = stackPanel3.Children;
				TextBlock textBlock8;
				TextBlock textBlock9 = (textBlock8 = new TextBlock());
				((ISupportInitialize)textBlock9).BeginInit();
				children4.Add(textBlock9);
				textBlock8.Inlines.Add("{");
				((ISupportInitialize)textBlock8).EndInit();
				Avalonia.Controls.Controls children5 = stackPanel3.Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children5.Add(rectangle2);
				Rectangle rectangle3;
				Rectangle rectangle4 = (rectangle3 = rectangle);
				context.PushParent(rectangle3);
				Rectangle rectangle5 = rectangle3;
				rectangle5.Height = 8.0;
				rectangle5.Width = 8.0;
				rectangle5.VerticalAlignment = VerticalAlignment.Center;
				StyledProperty<IBrush?> fillProperty = Shape.FillProperty;
				CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EBindingSetterViewModel_002CAvalonia_002EDiagnostics_002ETint_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = Shape.FillProperty;
				CompiledBindingExtension binding6 = compiledBindingExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle5.Bind(fillProperty, binding6);
				CompiledBindingExtension compiledBindingExtension8 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EBindingSetterViewModel_002CAvalonia_002EDiagnostics_002EValueTypeTooltip_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = ToolTip.TipProperty;
				CompiledBindingExtension compiledBindingExtension9 = compiledBindingExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_19.DynamicSetter_1((ToolTip)(object)rectangle5, compiledBindingExtension9);
				context.PopParent();
				((ISupportInitialize)rectangle4).EndInit();
				Avalonia.Controls.Controls children6 = stackPanel3.Children;
				TextBlock textBlock10;
				TextBlock textBlock11 = (textBlock10 = new TextBlock());
				((ISupportInitialize)textBlock11).BeginInit();
				children6.Add(textBlock11);
				TextBlock textBlock12 = (textBlock3 = textBlock10);
				context.PushParent(textBlock3);
				TextBlock target = textBlock3;
				StyledProperty<string?> textProperty2 = TextBlock.TextProperty;
				CompiledBindingExtension compiledBindingExtension10 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EBindingSetterViewModel_002CAvalonia_002EDiagnostics_002EPath_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = TextBlock.TextProperty;
				CompiledBindingExtension binding7 = compiledBindingExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				target.Bind(textProperty2, binding7);
				context.PopParent();
				((ISupportInitialize)textBlock12).EndInit();
				Avalonia.Controls.Controls children7 = stackPanel3.Children;
				TextBlock textBlock13;
				TextBlock textBlock14 = (textBlock13 = new TextBlock());
				((ISupportInitialize)textBlock14).BeginInit();
				children7.Add(textBlock14);
				textBlock13.Inlines.Add("}");
				((ISupportInitialize)textBlock13).EndInit();
				context.PopParent();
				((ISupportInitialize)stackPanel4).EndInit();
				Avalonia.Controls.Controls children8 = panel.Children;
				Rectangle rectangle6;
				Rectangle rectangle7 = (rectangle6 = new Rectangle());
				((ISupportInitialize)rectangle7).BeginInit();
				children8.Add(rectangle7);
				Rectangle rectangle8 = (rectangle3 = rectangle6);
				context.PushParent(rectangle3);
				Rectangle rectangle9 = rectangle3;
				rectangle9.Classes.Add("property-inactive");
				StyledProperty<bool> isVisibleProperty2 = Visual.IsVisibleProperty;
				CompiledBindingExtension compiledBindingExtension11 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Not().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002CAvalonia_002EDiagnostics_002EIsActive_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = Visual.IsVisibleProperty;
				CompiledBindingExtension binding8 = compiledBindingExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle9.Bind(isVisibleProperty2, binding8);
				context.PopParent();
				((ISupportInitialize)rectangle8).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		private class XamlClosure_20
		{
			private class DynamicSetters_21
			{
				public static void DynamicSetter_1(ContentControl P_0, CompiledBindingExtension P_1)
				{
					if (P_1 != null)
					{
						IBinding binding = P_1;
						P_0.Bind(ContentControl.ContentProperty, binding);
					}
					else
					{
						P_0.Content = P_1;
					}
				}

				public static void DynamicSetter_2(ToolTip P_0, CompiledBindingExtension P_1)
				{
					if (P_1 != null)
					{
						IBinding binding = P_1;
						P_0.Bind(ToolTip.TipProperty, binding);
					}
					else
					{
						ToolTip.SetTip(P_0, P_1);
					}
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FControlDetailsView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/ControlDetailsView.xaml");
				if (P_0 != null)
				{
					object service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (ControlDetailsView)service;
					}
				}
				context.IntermediateRoot = new Panel();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				Panel panel = (Panel)intermediateRoot;
				context.PushParent(panel);
				StyledProperty<double> opacityProperty = Visual.OpacityProperty;
				CompiledBindingExtension compiledBindingExtension;
				CompiledBindingExtension compiledBindingExtension2 = (compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002CAvalonia_002EDiagnostics_002EIsActive_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build()));
				context.PushParent(compiledBindingExtension);
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("BoolToOpacity");
				context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
				object? obj = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				compiledBindingExtension.Converter = (IValueConverter)obj;
				context.PopParent();
				context.ProvideTargetProperty = Visual.OpacityProperty;
				CompiledBindingExtension binding = compiledBindingExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				panel.Bind(opacityProperty, binding);
				StyledProperty<bool> isVisibleProperty = Visual.IsVisibleProperty;
				CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002CAvalonia_002EDiagnostics_002EIsVisible_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = Visual.IsVisibleProperty;
				CompiledBindingExtension binding2 = compiledBindingExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				panel.Bind(isVisibleProperty, binding2);
				panel.HorizontalAlignment = HorizontalAlignment.Left;
				ContextMenu contextMenu;
				ContextMenu contextMenu2 = (contextMenu = new ContextMenu());
				((ISupportInitialize)contextMenu2).BeginInit();
				panel.ContextMenu = contextMenu2;
				ContextMenu contextMenu3;
				ContextMenu contextMenu4 = (contextMenu3 = contextMenu);
				context.PushParent(contextMenu3);
				ItemCollection items = contextMenu3.Items;
				MenuItem menuItem;
				MenuItem menuItem2 = (menuItem = new MenuItem());
				((ISupportInitialize)menuItem2).BeginInit();
				items.Add(menuItem2);
				MenuItem menuItem3;
				MenuItem menuItem4 = (menuItem3 = menuItem);
				context.PushParent(menuItem3);
				MenuItem menuItem5 = menuItem3;
				menuItem5.Header = "Copy property name";
				StyledProperty<ICommand?> commandProperty = MenuItem.CommandProperty;
				CompiledBindingExtension compiledBindingExtension4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("CopyPropertyName", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002BCopyPropertyName_0_0021CommandExecuteTrampoline, null, null).Build());
				context.ProvideTargetProperty = MenuItem.CommandProperty;
				CompiledBindingExtension binding3 = compiledBindingExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				menuItem5.Bind(commandProperty, binding3);
				context.PopParent();
				((ISupportInitialize)menuItem4).EndInit();
				ItemCollection items2 = contextMenu3.Items;
				MenuItem menuItem6;
				MenuItem menuItem7 = (menuItem6 = new MenuItem());
				((ISupportInitialize)menuItem7).BeginInit();
				items2.Add(menuItem7);
				MenuItem menuItem8 = (menuItem3 = menuItem6);
				context.PushParent(menuItem3);
				MenuItem menuItem9 = menuItem3;
				menuItem9.Header = "Copy value";
				StyledProperty<ICommand?> commandProperty2 = MenuItem.CommandProperty;
				CompiledBindingExtension compiledBindingExtension5 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("CopyValue", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002BCopyValue_0_0021CommandExecuteTrampoline, null, null).Build());
				context.ProvideTargetProperty = MenuItem.CommandProperty;
				CompiledBindingExtension binding4 = compiledBindingExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				menuItem9.Bind(commandProperty2, binding4);
				context.PopParent();
				((ISupportInitialize)menuItem8).EndInit();
				ItemCollection items3 = contextMenu3.Items;
				MenuItem menuItem10;
				MenuItem menuItem11 = (menuItem10 = new MenuItem());
				((ISupportInitialize)menuItem11).BeginInit();
				items3.Add(menuItem11);
				MenuItem menuItem12 = (menuItem3 = menuItem10);
				context.PushParent(menuItem3);
				MenuItem menuItem13 = menuItem3;
				menuItem13.Header = "Copy resource key";
				StyledProperty<ICommand?> commandProperty3 = MenuItem.CommandProperty;
				CompiledBindingExtension compiledBindingExtension6 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("CopyResourceKey", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EResourceSetterViewModel_002BCopyResourceKey_0_0021CommandExecuteTrampoline, null, null).Build());
				context.ProvideTargetProperty = MenuItem.CommandProperty;
				CompiledBindingExtension binding5 = compiledBindingExtension6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				menuItem13.Bind(commandProperty3, binding5);
				context.PopParent();
				((ISupportInitialize)menuItem12).EndInit();
				context.PopParent();
				((ISupportInitialize)contextMenu4).EndInit();
				Avalonia.Controls.Controls children = panel.Children;
				StackPanel stackPanel;
				StackPanel stackPanel2 = (stackPanel = new StackPanel());
				((ISupportInitialize)stackPanel2).BeginInit();
				children.Add(stackPanel2);
				StackPanel stackPanel3;
				StackPanel stackPanel4 = (stackPanel3 = stackPanel);
				context.PushParent(stackPanel3);
				stackPanel3.Orientation = Orientation.Horizontal;
				stackPanel3.Spacing = 2.0;
				stackPanel3.HorizontalAlignment = HorizontalAlignment.Left;
				Avalonia.Controls.Controls children2 = stackPanel3.Children;
				TextBlock textBlock;
				TextBlock textBlock2 = (textBlock = new TextBlock());
				((ISupportInitialize)textBlock2).BeginInit();
				children2.Add(textBlock2);
				TextBlock textBlock3;
				TextBlock textBlock4 = (textBlock3 = textBlock);
				context.PushParent(textBlock3);
				TextBlock textBlock5 = textBlock3;
				textBlock5.Classes.Add("property-name");
				textBlock5.PointerPressed += context.RootObject.PropertyNamePressed;
				StyledProperty<string?> textProperty = TextBlock.TextProperty;
				CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002CAvalonia_002EDiagnostics_002EName_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = TextBlock.TextProperty;
				CompiledBindingExtension binding6 = compiledBindingExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock5.Bind(textProperty, binding6);
				context.PopParent();
				((ISupportInitialize)textBlock4).EndInit();
				Avalonia.Controls.Controls children3 = stackPanel3.Children;
				TextBlock textBlock6;
				TextBlock textBlock7 = (textBlock6 = new TextBlock());
				((ISupportInitialize)textBlock7).BeginInit();
				children3.Add(textBlock7);
				textBlock6.Text = ":";
				((ISupportInitialize)textBlock6).EndInit();
				Avalonia.Controls.Controls children4 = stackPanel3.Children;
				ContentControl contentControl;
				ContentControl contentControl2 = (contentControl = new ContentControl());
				((ISupportInitialize)contentControl2).BeginInit();
				children4.Add(contentControl2);
				ContentControl contentControl3;
				ContentControl contentControl4 = (contentControl3 = contentControl);
				context.PushParent(contentControl3);
				CompiledBindingExtension compiledBindingExtension8 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002CAvalonia_002EDiagnostics_002EValue_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = ContentControl.ContentProperty;
				CompiledBindingExtension compiledBindingExtension9 = compiledBindingExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_21.DynamicSetter_1(contentControl3, compiledBindingExtension9);
				context.PopParent();
				((ISupportInitialize)contentControl4).EndInit();
				Avalonia.Controls.Controls children5 = stackPanel3.Children;
				TextBlock textBlock8;
				TextBlock textBlock9 = (textBlock8 = new TextBlock());
				((ISupportInitialize)textBlock9).BeginInit();
				children5.Add(textBlock9);
				textBlock8.Inlines.Add("(");
				((ISupportInitialize)textBlock8).EndInit();
				Avalonia.Controls.Controls children6 = stackPanel3.Children;
				Ellipse ellipse;
				Ellipse ellipse2 = (ellipse = new Ellipse());
				((ISupportInitialize)ellipse2).BeginInit();
				children6.Add(ellipse2);
				Ellipse ellipse3;
				Ellipse ellipse4 = (ellipse3 = ellipse);
				context.PushParent(ellipse3);
				ellipse3.Height = 8.0;
				ellipse3.Width = 8.0;
				ellipse3.VerticalAlignment = VerticalAlignment.Center;
				StyledProperty<IBrush?> fillProperty = Shape.FillProperty;
				CompiledBindingExtension compiledBindingExtension10 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EResourceSetterViewModel_002CAvalonia_002EDiagnostics_002ETint_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = Shape.FillProperty;
				CompiledBindingExtension binding7 = compiledBindingExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				ellipse3.Bind(fillProperty, binding7);
				CompiledBindingExtension compiledBindingExtension11 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EResourceSetterViewModel_002CAvalonia_002EDiagnostics_002EValueTypeTooltip_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = ToolTip.TipProperty;
				CompiledBindingExtension compiledBindingExtension12 = compiledBindingExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_21.DynamicSetter_2((ToolTip)(object)ellipse3, compiledBindingExtension12);
				context.PopParent();
				((ISupportInitialize)ellipse4).EndInit();
				Avalonia.Controls.Controls children7 = stackPanel3.Children;
				TextBlock textBlock10;
				TextBlock textBlock11 = (textBlock10 = new TextBlock());
				((ISupportInitialize)textBlock11).BeginInit();
				children7.Add(textBlock11);
				TextBlock textBlock12 = (textBlock3 = textBlock10);
				context.PushParent(textBlock3);
				TextBlock textBlock13 = textBlock3;
				textBlock13.FontStyle = FontStyle.Italic;
				StyledProperty<string?> textProperty2 = TextBlock.TextProperty;
				CompiledBindingExtension compiledBindingExtension13 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EResourceSetterViewModel_002CAvalonia_002EDiagnostics_002EKey_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = TextBlock.TextProperty;
				CompiledBindingExtension binding8 = compiledBindingExtension13.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock13.Bind(textProperty2, binding8);
				context.PopParent();
				((ISupportInitialize)textBlock12).EndInit();
				Avalonia.Controls.Controls children8 = stackPanel3.Children;
				TextBlock textBlock14;
				TextBlock textBlock15 = (textBlock14 = new TextBlock());
				((ISupportInitialize)textBlock15).BeginInit();
				children8.Add(textBlock15);
				textBlock14.Inlines.Add(")");
				((ISupportInitialize)textBlock14).EndInit();
				context.PopParent();
				((ISupportInitialize)stackPanel4).EndInit();
				Avalonia.Controls.Controls children9 = panel.Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children9.Add(rectangle2);
				Rectangle rectangle3;
				Rectangle rectangle4 = (rectangle3 = rectangle);
				context.PushParent(rectangle3);
				rectangle3.Classes.Add("property-inactive");
				StyledProperty<bool> isVisibleProperty2 = Visual.IsVisibleProperty;
				CompiledBindingExtension compiledBindingExtension14 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Not().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002CAvalonia_002EDiagnostics_002EIsActive_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = Visual.IsVisibleProperty;
				CompiledBindingExtension binding9 = compiledBindingExtension14.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle3.Bind(isVisibleProperty2, binding9);
				context.PopParent();
				((ISupportInitialize)rectangle4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		private class XamlClosure_22
		{
			private class DynamicSetters_23
			{
				public static void DynamicSetter_1(ContentControl P_0, CompiledBindingExtension P_1)
				{
					if (P_1 != null)
					{
						IBinding binding = P_1;
						P_0.Bind(ContentControl.ContentProperty, binding);
					}
					else
					{
						P_0.Content = P_1;
					}
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FControlDetailsView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/ControlDetailsView.xaml");
				if (P_0 != null)
				{
					object service = P_0.GetService(typeof(IRootObjectProvider));
					if (service != null)
					{
						service = ((IRootObjectProvider)service).RootObject;
						context.RootObject = (ControlDetailsView)service;
					}
				}
				context.IntermediateRoot = new Panel();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				Panel panel = (Panel)intermediateRoot;
				context.PushParent(panel);
				StyledProperty<double> opacityProperty = Visual.OpacityProperty;
				CompiledBindingExtension compiledBindingExtension;
				CompiledBindingExtension compiledBindingExtension2 = (compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002CAvalonia_002EDiagnostics_002EIsActive_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build()));
				context.PushParent(compiledBindingExtension);
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("BoolToOpacity");
				context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
				object? obj = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				compiledBindingExtension.Converter = (IValueConverter)obj;
				context.PopParent();
				context.ProvideTargetProperty = Visual.OpacityProperty;
				CompiledBindingExtension binding = compiledBindingExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				panel.Bind(opacityProperty, binding);
				StyledProperty<bool> isVisibleProperty = Visual.IsVisibleProperty;
				CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002CAvalonia_002EDiagnostics_002EIsVisible_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = Visual.IsVisibleProperty;
				CompiledBindingExtension binding2 = compiledBindingExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				panel.Bind(isVisibleProperty, binding2);
				panel.HorizontalAlignment = HorizontalAlignment.Left;
				ContextMenu contextMenu;
				ContextMenu contextMenu2 = (contextMenu = new ContextMenu());
				((ISupportInitialize)contextMenu2).BeginInit();
				panel.ContextMenu = contextMenu2;
				ContextMenu contextMenu3;
				ContextMenu contextMenu4 = (contextMenu3 = contextMenu);
				context.PushParent(contextMenu3);
				ItemCollection items = contextMenu3.Items;
				MenuItem menuItem;
				MenuItem menuItem2 = (menuItem = new MenuItem());
				((ISupportInitialize)menuItem2).BeginInit();
				items.Add(menuItem2);
				MenuItem menuItem3;
				MenuItem menuItem4 = (menuItem3 = menuItem);
				context.PushParent(menuItem3);
				MenuItem menuItem5 = menuItem3;
				menuItem5.Header = "Copy property name";
				StyledProperty<ICommand?> commandProperty = MenuItem.CommandProperty;
				CompiledBindingExtension compiledBindingExtension4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("CopyPropertyName", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002BCopyPropertyName_0_0021CommandExecuteTrampoline, null, null).Build());
				context.ProvideTargetProperty = MenuItem.CommandProperty;
				CompiledBindingExtension binding3 = compiledBindingExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				menuItem5.Bind(commandProperty, binding3);
				context.PopParent();
				((ISupportInitialize)menuItem4).EndInit();
				ItemCollection items2 = contextMenu3.Items;
				MenuItem menuItem6;
				MenuItem menuItem7 = (menuItem6 = new MenuItem());
				((ISupportInitialize)menuItem7).BeginInit();
				items2.Add(menuItem7);
				MenuItem menuItem8 = (menuItem3 = menuItem6);
				context.PushParent(menuItem3);
				MenuItem menuItem9 = menuItem3;
				menuItem9.Header = "Copy value";
				StyledProperty<ICommand?> commandProperty2 = MenuItem.CommandProperty;
				CompiledBindingExtension compiledBindingExtension5 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("CopyValue", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002BCopyValue_0_0021CommandExecuteTrampoline, null, null).Build());
				context.ProvideTargetProperty = MenuItem.CommandProperty;
				CompiledBindingExtension binding4 = compiledBindingExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				menuItem9.Bind(commandProperty2, binding4);
				context.PopParent();
				((ISupportInitialize)menuItem8).EndInit();
				context.PopParent();
				((ISupportInitialize)contextMenu4).EndInit();
				Avalonia.Controls.Controls children = panel.Children;
				StackPanel stackPanel;
				StackPanel stackPanel2 = (stackPanel = new StackPanel());
				((ISupportInitialize)stackPanel2).BeginInit();
				children.Add(stackPanel2);
				StackPanel stackPanel3;
				StackPanel stackPanel4 = (stackPanel3 = stackPanel);
				context.PushParent(stackPanel3);
				stackPanel3.Orientation = Orientation.Horizontal;
				stackPanel3.Spacing = 2.0;
				Avalonia.Controls.Controls children2 = stackPanel3.Children;
				TextBlock textBlock;
				TextBlock textBlock2 = (textBlock = new TextBlock());
				((ISupportInitialize)textBlock2).BeginInit();
				children2.Add(textBlock2);
				TextBlock textBlock3;
				TextBlock textBlock4 = (textBlock3 = textBlock);
				context.PushParent(textBlock3);
				textBlock3.Classes.Add("property-name");
				textBlock3.PointerPressed += context.RootObject.PropertyNamePressed;
				StyledProperty<string?> textProperty = TextBlock.TextProperty;
				CompiledBindingExtension compiledBindingExtension6 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002CAvalonia_002EDiagnostics_002EName_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = TextBlock.TextProperty;
				CompiledBindingExtension binding5 = compiledBindingExtension6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock3.Bind(textProperty, binding5);
				context.PopParent();
				((ISupportInitialize)textBlock4).EndInit();
				Avalonia.Controls.Controls children3 = stackPanel3.Children;
				TextBlock textBlock5;
				TextBlock textBlock6 = (textBlock5 = new TextBlock());
				((ISupportInitialize)textBlock6).BeginInit();
				children3.Add(textBlock6);
				textBlock5.Text = ":";
				((ISupportInitialize)textBlock5).EndInit();
				Avalonia.Controls.Controls children4 = stackPanel3.Children;
				ContentControl contentControl;
				ContentControl contentControl2 = (contentControl = new ContentControl());
				((ISupportInitialize)contentControl2).BeginInit();
				children4.Add(contentControl2);
				ContentControl contentControl3;
				ContentControl contentControl4 = (contentControl3 = contentControl);
				context.PushParent(contentControl3);
				CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002CAvalonia_002EDiagnostics_002EValue_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = ContentControl.ContentProperty;
				CompiledBindingExtension compiledBindingExtension8 = compiledBindingExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_23.DynamicSetter_1(contentControl3, compiledBindingExtension8);
				context.PopParent();
				((ISupportInitialize)contentControl4).EndInit();
				context.PopParent();
				((ISupportInitialize)stackPanel4).EndInit();
				Avalonia.Controls.Controls children5 = panel.Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children5.Add(rectangle2);
				Rectangle rectangle3;
				Rectangle rectangle4 = (rectangle3 = rectangle);
				context.PushParent(rectangle3);
				rectangle3.Classes.Add("property-inactive");
				StyledProperty<bool> isVisibleProperty2 = Visual.IsVisibleProperty;
				CompiledBindingExtension compiledBindingExtension9 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Not().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002CAvalonia_002EDiagnostics_002EIsActive_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
				context.ProvideTargetProperty = Visual.IsVisibleProperty;
				CompiledBindingExtension binding6 = compiledBindingExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				rectangle3.Bind(isVisibleProperty2, binding6);
				context.PopParent();
				((ISupportInitialize)rectangle4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FControlDetailsView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/ControlDetailsView.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ControlDetailsView)service;
				}
			}
			context.IntermediateRoot = new Border();
			object intermediateRoot = context.IntermediateRoot;
			((ISupportInitialize)intermediateRoot).BeginInit();
			Border border = (Border)intermediateRoot;
			context.PushParent(border);
			border.BorderThickness = new Thickness(0.0, 0.0, 0.0, 1.0);
			border.BorderBrush = new ImmutableSolidColorBrush(4285295724u);
			StyledProperty<double> opacityProperty = Visual.OpacityProperty;
			CompiledBindingExtension compiledBindingExtension;
			CompiledBindingExtension compiledBindingExtension2 = (compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EStyleViewModel_002CAvalonia_002EDiagnostics_002EIsActive_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build()));
			context.PushParent(compiledBindingExtension);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("BoolToOpacity");
			context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			compiledBindingExtension.Converter = (IValueConverter)obj;
			context.PopParent();
			context.ProvideTargetProperty = Visual.OpacityProperty;
			CompiledBindingExtension binding = compiledBindingExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			border.Bind(opacityProperty, binding);
			StyledProperty<bool> isVisibleProperty = Visual.IsVisibleProperty;
			MultiBinding multiBinding;
			MultiBinding binding2 = (multiBinding = new MultiBinding());
			context.PushParent(multiBinding);
			MultiBinding multiBinding2 = multiBinding;
			multiBinding2.Converter = BoolConverters.And;
			IList<IBinding> bindings = multiBinding2.Bindings;
			MultiBinding item = (multiBinding = new MultiBinding());
			context.PushParent(multiBinding);
			MultiBinding multiBinding3 = multiBinding;
			multiBinding3.Converter = BoolConverters.Or;
			IList<IBinding> bindings2 = multiBinding3.Bindings;
			CompiledBindingExtension obj2 = new CompiledBindingExtension
			{
				Path = new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EStyleViewModel_002CAvalonia_002EDiagnostics_002EIsActive_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build()
			};
			context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
			CompiledBindingExtension item2 = obj2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			bindings2.Add(item2);
			IList<IBinding> bindings3 = multiBinding3.Bindings;
			CompiledBindingExtension obj3 = new CompiledBindingExtension
			{
				Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Main").Property(StyledElement.DataContextProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).TypeCast<ControlDetailsViewModel>()
					.Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EControlDetailsViewModel_002CAvalonia_002EDiagnostics_002EShowInactiveStyles_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
					.Build()
			};
			context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
			CompiledBindingExtension item3 = obj3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			bindings3.Add(item3);
			context.PopParent();
			bindings.Add(item);
			IList<IBinding> bindings4 = multiBinding2.Bindings;
			CompiledBindingExtension obj4 = new CompiledBindingExtension
			{
				Path = new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EStyleViewModel_002CAvalonia_002EDiagnostics_002EIsVisible_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build()
			};
			context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
			CompiledBindingExtension item4 = obj4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			bindings4.Add(item4);
			context.PopParent();
			border.Bind(isVisibleProperty, binding2);
			Expander expander;
			Expander expander2 = (expander = new Expander());
			((ISupportInitialize)expander2).BeginInit();
			border.Child = expander2;
			Expander expander3;
			Expander expander4 = (expander3 = expander);
			context.PushParent(expander3);
			expander3.IsExpanded = true;
			expander3.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
			expander3.Padding = new Thickness(8.0, 0.0, 8.0, 0.0);
			expander3.ContentTransition = null;
			TextBlock textBlock;
			TextBlock textBlock2 = (textBlock = new TextBlock());
			((ISupportInitialize)textBlock2).BeginInit();
			expander3.Header = textBlock2;
			TextBlock textBlock3;
			TextBlock textBlock4 = (textBlock3 = textBlock);
			context.PushParent(textBlock3);
			Grid.SetRow(textBlock3, 0);
			StyledProperty<string?> textProperty = TextBlock.TextProperty;
			CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EStyleViewModel_002CAvalonia_002EDiagnostics_002EName_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = TextBlock.TextProperty;
			CompiledBindingExtension binding3 = compiledBindingExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			textBlock3.Bind(textProperty, binding3);
			context.PopParent();
			((ISupportInitialize)textBlock4).EndInit();
			ItemsControl itemsControl;
			ItemsControl itemsControl2 = (itemsControl = new ItemsControl());
			((ISupportInitialize)itemsControl2).BeginInit();
			expander3.Content = itemsControl2;
			ItemsControl itemsControl3;
			ItemsControl itemsControl4 = (itemsControl3 = itemsControl);
			context.PushParent(itemsControl3);
			itemsControl3.Margin = new Thickness(20.0, 0.0, 0.0, 0.0);
			Grid.SetRow(itemsControl3, 1);
			StyledProperty<IEnumerable?> itemsSourceProperty = ItemsControl.ItemsSourceProperty;
			CompiledBindingExtension compiledBindingExtension4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EStyleViewModel_002CAvalonia_002EDiagnostics_002ESetters_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = ItemsControl.ItemsSourceProperty;
			CompiledBindingExtension binding4 = compiledBindingExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			itemsControl3.Bind(itemsSourceProperty, binding4);
			Styles styles = itemsControl3.Styles;
			Style style = new Style();
			style.Selector = ((Selector?)null).OfType(typeof(TextBlock)).Class("property-name");
			Setter setter = new Setter();
			setter.Property = TextBlock.FontWeightProperty;
			setter.Value = FontWeight.DemiBold;
			style.Add(setter);
			Setter setter2 = new Setter();
			setter2.Property = TextBlock.BackgroundProperty;
			setter2.Value = new ImmutableSolidColorBrush(16777215u);
			style.Add(setter2);
			styles.Add(style);
			Styles styles2 = itemsControl3.Styles;
			Style style2 = new Style();
			style2.Selector = ((Selector?)null).OfType(typeof(TextBlock)).Class("property-name").Class(":pointerover");
			Setter setter3 = new Setter();
			setter3.Property = TextBlock.TextDecorationsProperty;
			setter3.Value = TextDecorations.Underline;
			style2.Add(setter3);
			styles2.Add(style2);
			Styles styles3 = itemsControl3.Styles;
			Style style3 = new Style();
			style3.Selector = ((Selector?)null).OfType(typeof(Rectangle)).Class("property-inactive");
			Setter setter4 = new Setter();
			setter4.Property = InputElement.IsHitTestVisibleProperty;
			setter4.Value = false;
			style3.Add(setter4);
			Setter setter5 = new Setter();
			setter5.Property = Layoutable.HeightProperty;
			setter5.Value = 1.0;
			style3.Add(setter5);
			Setter setter6 = new Setter();
			setter6.Property = Shape.FillProperty;
			setter6.Value = new ImmutableSolidColorBrush(4285295724u);
			style3.Add(setter6);
			Setter setter7 = new Setter();
			setter7.Property = Layoutable.VerticalAlignmentProperty;
			setter7.Value = VerticalAlignment.Center;
			style3.Add(setter7);
			styles3.Add(style3);
			DataTemplates dataTemplates = itemsControl3.DataTemplates;
			DataTemplate dataTemplate;
			DataTemplate item5 = (dataTemplate = new DataTemplate());
			context.PushParent(dataTemplate);
			DataTemplate dataTemplate2 = dataTemplate;
			dataTemplate2.DataType = typeof(IBrush);
			dataTemplate2.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_16.Build, context);
			context.PopParent();
			dataTemplates.Add(item5);
			DataTemplates dataTemplates2 = itemsControl3.DataTemplates;
			DataTemplate item6 = (dataTemplate = new DataTemplate());
			context.PushParent(dataTemplate);
			DataTemplate dataTemplate3 = dataTemplate;
			dataTemplate3.DataType = typeof(Color);
			dataTemplate3.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_17.Build, context);
			context.PopParent();
			dataTemplates2.Add(item6);
			DataTemplates dataTemplates3 = itemsControl3.DataTemplates;
			DataTemplate item7 = (dataTemplate = new DataTemplate());
			context.PushParent(dataTemplate);
			DataTemplate dataTemplate4 = dataTemplate;
			dataTemplate4.DataType = typeof(BindingSetterViewModel);
			dataTemplate4.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_18.Build, context);
			context.PopParent();
			dataTemplates3.Add(item7);
			DataTemplates dataTemplates4 = itemsControl3.DataTemplates;
			DataTemplate item8 = (dataTemplate = new DataTemplate());
			context.PushParent(dataTemplate);
			DataTemplate dataTemplate5 = dataTemplate;
			dataTemplate5.DataType = typeof(ResourceSetterViewModel);
			dataTemplate5.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_20.Build, context);
			context.PopParent();
			dataTemplates4.Add(item8);
			DataTemplates dataTemplates5 = itemsControl3.DataTemplates;
			DataTemplate item9 = (dataTemplate = new DataTemplate());
			context.PushParent(dataTemplate);
			DataTemplate dataTemplate6 = dataTemplate;
			dataTemplate6.DataType = typeof(SetterViewModel);
			dataTemplate6.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_22.Build, context);
			context.PopParent();
			dataTemplates5.Add(item9);
			context.PopParent();
			((ISupportInitialize)itemsControl4).EndInit();
			context.PopParent();
			((ISupportInitialize)expander4).EndInit();
			context.PopParent();
			((ISupportInitialize)intermediateRoot).EndInit();
			return intermediateRoot;
		}
	}

	private class XamlClosure_24
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FControlDetailsView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/ControlDetailsView.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ControlDetailsView)service;
				}
			}
			context.IntermediateRoot = new WrapPanel();
			object intermediateRoot = context.IntermediateRoot;
			((ISupportInitialize)intermediateRoot).BeginInit();
			((ISupportInitialize)intermediateRoot).EndInit();
			return intermediateRoot;
		}
	}

	private class XamlClosure_25
	{
		private class DynamicSetters_26
		{
			public static void DynamicSetter_1(ContentControl P_0, CompiledBindingExtension P_1)
			{
				if (P_1 != null)
				{
					IBinding binding = P_1;
					P_0.Bind(ContentControl.ContentProperty, binding);
				}
				else
				{
					P_0.Content = P_1;
				}
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FControlDetailsView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/ControlDetailsView.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ControlDetailsView)service;
				}
			}
			context.IntermediateRoot = new CheckBox();
			object intermediateRoot = context.IntermediateRoot;
			((ISupportInitialize)intermediateRoot).BeginInit();
			CheckBox checkBox = (CheckBox)intermediateRoot;
			context.PushParent(checkBox);
			checkBox.Margin = new Thickness(2.0, 2.0, 2.0, 2.0);
			CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EPseudoClassViewModel_002CAvalonia_002EDiagnostics_002EName_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = ContentControl.ContentProperty;
			CompiledBindingExtension compiledBindingExtension2 = compiledBindingExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_26.DynamicSetter_1(checkBox, compiledBindingExtension2);
			StyledProperty<bool?> isCheckedProperty = ToggleButton.IsCheckedProperty;
			CompiledBindingExtension obj = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EPseudoClassViewModel_002CAvalonia_002EDiagnostics_002EIsActive_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build())
			{
				Mode = BindingMode.TwoWay
			};
			context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
			CompiledBindingExtension binding = obj.ProvideValue(context);
			context.ProvideTargetProperty = null;
			checkBox.Bind(isCheckedProperty, binding);
			context.PopParent();
			((ISupportInitialize)intermediateRoot).EndInit();
			return intermediateRoot;
		}
	}

	private DataGrid _dataGrid;

	private static Action<object> _0021XamlIlPopulateOverride;

	public ControlDetailsView()
	{
		InitializeComponent();
		_dataGrid = this.GetControl<DataGrid>("DataGrid");
	}

	private void InitializeComponent()
	{
		_0021XamlIlPopulateTrampoline(this);
	}

	private void PropertiesGrid_OnDoubleTapped(object sender, TappedEventArgs e)
	{
		if (sender is DataGrid { DataContext: ControlDetailsViewModel dataContext })
		{
			dataContext.NavigateToSelectedProperty();
		}
	}

	public void PropertyNamePressed(object sender, PointerPressedEventArgs e)
	{
		ControlDetailsViewModel controlDetailsViewModel = (ControlDetailsViewModel)base.DataContext;
		if (controlDetailsViewModel != null && sender is Control { DataContext: SetterViewModel dataContext })
		{
			controlDetailsViewModel.SelectProperty(dataContext.Property);
			if (controlDetailsViewModel.SelectedProperty != null)
			{
				_dataGrid.ScrollIntoView(controlDetailsViewModel.SelectedProperty, null);
			}
		}
	}

	static void _0021XamlIlPopulate(IServiceProvider P_0, ControlDetailsView P_1)
	{
		CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<ControlDetailsView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FControlDetailsView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/ControlDetailsView.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		((ISupportInitialize)P_1).BeginInit();
		context.PushParent(P_1);
		P_1.Name = "Main";
		object element = P_1;
		context.AvaloniaNameScope.Register("Main", element);
		((ResourceDictionary)P_1.Resources).AddDeferred((object)"BoolToOpacity", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_11.Build, context));
		((ResourceDictionary)P_1.Resources).AddDeferred((object)"GetTypeName", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_12.Build, context));
		Grid grid;
		Grid grid2 = (grid = new Grid());
		((ISupportInitialize)grid2).BeginInit();
		P_1.Content = grid2;
		Grid grid3;
		Grid grid4 = (grid3 = grid);
		context.PushParent(grid3);
		Grid grid5 = grid3;
		grid5.ColumnDefinitions.Add(new Avalonia.Controls.ColumnDefinition
		{
			Width = new GridLength(1.0, GridUnitType.Star)
		});
		grid5.ColumnDefinitions.Add(new Avalonia.Controls.ColumnDefinition
		{
			Width = new GridLength(0.0, GridUnitType.Auto)
		});
		ColumnDefinitions columnDefinitions = grid5.ColumnDefinitions;
		Avalonia.Controls.ColumnDefinition columnDefinition;
		Avalonia.Controls.ColumnDefinition item = (columnDefinition = new Avalonia.Controls.ColumnDefinition());
		context.PushParent(columnDefinition);
		columnDefinition.Width = new GridLength(320.0, GridUnitType.Pixel);
		AttachedProperty<bool> isVisibleProperty = Avalonia.Diagnostics.Behaviors.ColumnDefinition.IsVisibleProperty;
		CompiledBindingExtension obj = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EControlDetailsViewModel_002CAvalonia_002EDiagnostics_002ELayout_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build())
		{
			Converter = ObjectConverters.IsNotNull
		};
		context.ProvideTargetProperty = Avalonia.Diagnostics.Behaviors.ColumnDefinition.IsVisibleProperty;
		CompiledBindingExtension binding = obj.ProvideValue(context);
		context.ProvideTargetProperty = null;
		columnDefinition.Bind(isVisibleProperty, binding);
		context.PopParent();
		columnDefinitions.Add(item);
		Avalonia.Controls.Controls children = grid5.Children;
		Grid grid6;
		Grid grid7 = (grid6 = new Grid());
		((ISupportInitialize)grid7).BeginInit();
		children.Add(grid7);
		Grid grid8 = (grid3 = grid6);
		context.PushParent(grid3);
		Grid grid9 = grid3;
		Grid.SetColumn(grid9, 0);
		RowDefinitions rowDefinitions = new RowDefinitions();
		rowDefinitions.Capacity = 3;
		rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		rowDefinitions.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
		grid9.RowDefinitions = rowDefinitions;
		Avalonia.Controls.Controls children2 = grid9.Children;
		Grid grid10;
		Grid grid11 = (grid10 = new Grid());
		((ISupportInitialize)grid11).BeginInit();
		children2.Add(grid11);
		Grid grid12 = (grid3 = grid10);
		context.PushParent(grid3);
		Grid grid13 = grid3;
		ColumnDefinitions columnDefinitions2 = new ColumnDefinitions();
		columnDefinitions2.Capacity = 2;
		columnDefinitions2.Add(new Avalonia.Controls.ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
		columnDefinitions2.Add(new Avalonia.Controls.ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
		grid13.ColumnDefinitions = columnDefinitions2;
		RowDefinitions rowDefinitions2 = new RowDefinitions();
		rowDefinitions2.Capacity = 2;
		rowDefinitions2.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		rowDefinitions2.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		grid13.RowDefinitions = rowDefinitions2;
		Avalonia.Controls.Controls children3 = grid13.Children;
		Button button;
		Button button2 = (button = new Button());
		((ISupportInitialize)button2).BeginInit();
		children3.Add(button2);
		Button button3;
		Button button4 = (button3 = button);
		context.PushParent(button3);
		Grid.SetColumn(button3, 0);
		Grid.SetRowSpan(button3, 2);
		button3.Content = "^";
		StyledProperty<bool> isEnabledProperty = InputElement.IsEnabledProperty;
		CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EControlDetailsViewModel_002CAvalonia_002EDiagnostics_002ECanNavigateToParentProperty_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = InputElement.IsEnabledProperty;
		CompiledBindingExtension binding2 = compiledBindingExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		button3.Bind(isEnabledProperty, binding2);
		button3.Margin = new Thickness(0.0, 0.0, 4.0, 0.0);
		ToolTip.SetTip(button3, "Navigate to parent property");
		StyledProperty<ICommand?> commandProperty = Button.CommandProperty;
		CompiledBindingExtension compiledBindingExtension2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("NavigateToParentProperty", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EControlDetailsViewModel_002BNavigateToParentProperty_0_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = Button.CommandProperty;
		CompiledBindingExtension binding3 = compiledBindingExtension2.ProvideValue(context);
		context.ProvideTargetProperty = null;
		button3.Bind(commandProperty, binding3);
		context.PopParent();
		((ISupportInitialize)button4).EndInit();
		Avalonia.Controls.Controls children4 = grid13.Children;
		TextBlock textBlock;
		TextBlock textBlock2 = (textBlock = new TextBlock());
		((ISupportInitialize)textBlock2).BeginInit();
		children4.Add(textBlock2);
		TextBlock textBlock3;
		TextBlock textBlock4 = (textBlock3 = textBlock);
		context.PushParent(textBlock3);
		TextBlock textBlock5 = textBlock3;
		Grid.SetColumn(textBlock5, 1);
		Grid.SetRow(textBlock5, 0);
		StyledProperty<string?> textProperty = TextBlock.TextProperty;
		CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EControlDetailsViewModel_002CAvalonia_002EDiagnostics_002ESelectedEntityName_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = TextBlock.TextProperty;
		CompiledBindingExtension binding4 = compiledBindingExtension3.ProvideValue(context);
		context.ProvideTargetProperty = null;
		textBlock5.Bind(textProperty, binding4);
		textBlock5.FontWeight = FontWeight.Bold;
		context.PopParent();
		((ISupportInitialize)textBlock4).EndInit();
		Avalonia.Controls.Controls children5 = grid13.Children;
		TextBlock textBlock6;
		TextBlock textBlock7 = (textBlock6 = new TextBlock());
		((ISupportInitialize)textBlock7).BeginInit();
		children5.Add(textBlock7);
		TextBlock textBlock8 = (textBlock3 = textBlock6);
		context.PushParent(textBlock3);
		TextBlock textBlock9 = textBlock3;
		Grid.SetColumn(textBlock9, 1);
		Grid.SetRow(textBlock9, 1);
		StyledProperty<string?> textProperty2 = TextBlock.TextProperty;
		CompiledBindingExtension compiledBindingExtension4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EControlDetailsViewModel_002CAvalonia_002EDiagnostics_002ESelectedEntityType_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = TextBlock.TextProperty;
		CompiledBindingExtension binding5 = compiledBindingExtension4.ProvideValue(context);
		context.ProvideTargetProperty = null;
		textBlock9.Bind(textProperty2, binding5);
		textBlock9.FontStyle = FontStyle.Italic;
		context.PopParent();
		((ISupportInitialize)textBlock8).EndInit();
		context.PopParent();
		((ISupportInitialize)grid12).EndInit();
		Avalonia.Controls.Controls children6 = grid9.Children;
		FilterTextBox filterTextBox;
		FilterTextBox filterTextBox2 = (filterTextBox = new FilterTextBox());
		((ISupportInitialize)filterTextBox2).BeginInit();
		children6.Add(filterTextBox2);
		FilterTextBox filterTextBox3;
		FilterTextBox filterTextBox4 = (filterTextBox3 = filterTextBox);
		context.PushParent(filterTextBox3);
		FilterTextBox filterTextBox5 = filterTextBox3;
		Grid.SetRow(filterTextBox5, 1);
		filterTextBox5.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
		CompiledBindingExtension compiledBindingExtension5 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EControlDetailsViewModel_002CAvalonia_002EDiagnostics_002ETreePage_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002CAvalonia_002EDiagnostics_002EPropertiesFilter_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = StyledElement.DataContextProperty;
		CompiledBindingExtension compiledBindingExtension6 = compiledBindingExtension5.ProvideValue(context);
		context.ProvideTargetProperty = null;
		DynamicSetters_13.DynamicSetter_1(filterTextBox5, compiledBindingExtension6);
		StyledProperty<string?> textProperty3 = TextBox.TextProperty;
		CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFilterViewModel_002CAvalonia_002EDiagnostics_002EFilterString_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = TextBox.TextProperty;
		CompiledBindingExtension binding6 = compiledBindingExtension7.ProvideValue(context);
		context.ProvideTargetProperty = null;
		filterTextBox5.Bind(textProperty3, binding6);
		filterTextBox5.Watermark = "Filter properties";
		StyledProperty<bool> useCaseSensitiveFilterProperty = FilterTextBox.UseCaseSensitiveFilterProperty;
		CompiledBindingExtension compiledBindingExtension8 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFilterViewModel_002CAvalonia_002EDiagnostics_002EUseCaseSensitiveFilter_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = FilterTextBox.UseCaseSensitiveFilterProperty;
		CompiledBindingExtension binding7 = compiledBindingExtension8.ProvideValue(context);
		context.ProvideTargetProperty = null;
		filterTextBox5.Bind(useCaseSensitiveFilterProperty, binding7);
		StyledProperty<bool> useWholeWordFilterProperty = FilterTextBox.UseWholeWordFilterProperty;
		CompiledBindingExtension compiledBindingExtension9 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFilterViewModel_002CAvalonia_002EDiagnostics_002EUseWholeWordFilter_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = FilterTextBox.UseWholeWordFilterProperty;
		CompiledBindingExtension binding8 = compiledBindingExtension9.ProvideValue(context);
		context.ProvideTargetProperty = null;
		filterTextBox5.Bind(useWholeWordFilterProperty, binding8);
		StyledProperty<bool> useRegexFilterProperty = FilterTextBox.UseRegexFilterProperty;
		CompiledBindingExtension compiledBindingExtension10 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFilterViewModel_002CAvalonia_002EDiagnostics_002EUseRegexFilter_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = FilterTextBox.UseRegexFilterProperty;
		CompiledBindingExtension binding9 = compiledBindingExtension10.ProvideValue(context);
		context.ProvideTargetProperty = null;
		filterTextBox5.Bind(useRegexFilterProperty, binding9);
		context.PopParent();
		((ISupportInitialize)filterTextBox4).EndInit();
		Avalonia.Controls.Controls children7 = grid9.Children;
		DataGrid dataGrid;
		DataGrid dataGrid2 = (dataGrid = new DataGrid());
		((ISupportInitialize)dataGrid2).BeginInit();
		children7.Add(dataGrid2);
		DataGrid dataGrid3;
		DataGrid dataGrid4 = (dataGrid3 = dataGrid);
		context.PushParent(dataGrid3);
		dataGrid3.Name = "DataGrid";
		element = dataGrid3;
		context.AvaloniaNameScope.Register("DataGrid", element);
		StyledProperty<IEnumerable> itemsSourceProperty = DataGrid.ItemsSourceProperty;
		CompiledBindingExtension compiledBindingExtension11 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EControlDetailsViewModel_002CAvalonia_002EDiagnostics_002EPropertiesView_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = DataGrid.ItemsSourceProperty;
		CompiledBindingExtension binding10 = compiledBindingExtension11.ProvideValue(context);
		context.ProvideTargetProperty = null;
		dataGrid3.Bind(itemsSourceProperty, binding10);
		Grid.SetRow(dataGrid3, 2);
		dataGrid3.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
		dataGrid3.RowBackground = new ImmutableSolidColorBrush(16777215u);
		CompiledBindingExtension obj2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EControlDetailsViewModel_002CAvalonia_002EDiagnostics_002ESelectedProperty_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build())
		{
			Mode = BindingMode.TwoWay
		};
		context.ProvideTargetProperty = DataGrid.SelectedItemProperty;
		CompiledBindingExtension compiledBindingExtension12 = obj2.ProvideValue(context);
		context.ProvideTargetProperty = null;
		DynamicSetters_13.DynamicSetter_2(dataGrid3, compiledBindingExtension12);
		dataGrid3.CanUserResizeColumns = true;
		dataGrid3.DoubleTapped += context.RootObject.PropertiesGrid_OnDoubleTapped;
		ObservableCollection<DataGridColumn> columns = dataGrid3.Columns;
		DataGridTextColumn dataGridTextColumn;
		DataGridTextColumn item2 = (dataGridTextColumn = new DataGridTextColumn());
		context.PushParent(dataGridTextColumn);
		DataGridTextColumn dataGridTextColumn2 = dataGridTextColumn;
		dataGridTextColumn2.Header = "Property";
		CompiledBindingExtension compiledBindingExtension13 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EPropertyViewModel_002CAvalonia_002EDiagnostics_002EName_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EControls_002EDataGridBoundColumn_002CAvalonia_002EControls_002EDataGrid_002EBinding_0021Property();
		CompiledBindingExtension binding11 = compiledBindingExtension13.ProvideValue(context);
		context.ProvideTargetProperty = null;
		dataGridTextColumn2.Binding = binding11;
		dataGridTextColumn2.IsReadOnly = true;
		context.PopParent();
		columns.Add(item2);
		ObservableCollection<DataGridColumn> columns2 = dataGrid3.Columns;
		DataGridTemplateColumn dataGridTemplateColumn;
		DataGridTemplateColumn item3 = (dataGridTemplateColumn = new DataGridTemplateColumn());
		context.PushParent(dataGridTemplateColumn);
		dataGridTemplateColumn.Header = "Value";
		dataGridTemplateColumn.Width = (DataGridLength)new DataGridLengthConverter().ConvertFrom(context, CultureInfo.InvariantCulture, "100");
		dataGridTemplateColumn.CellTemplate = new DataTemplate
		{
			Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_14.Build, context)
		};
		context.PopParent();
		columns2.Add(item3);
		ObservableCollection<DataGridColumn> columns3 = dataGrid3.Columns;
		DataGridTextColumn item4 = (dataGridTextColumn = new DataGridTextColumn());
		context.PushParent(dataGridTextColumn);
		DataGridTextColumn dataGridTextColumn3 = dataGridTextColumn;
		dataGridTextColumn3.Header = "Type";
		CompiledBindingExtension compiledBindingExtension14 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EPropertyViewModel_002CAvalonia_002EDiagnostics_002EType_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EControls_002EDataGridBoundColumn_002CAvalonia_002EControls_002EDataGrid_002EBinding_0021Property();
		CompiledBindingExtension binding12 = compiledBindingExtension14.ProvideValue(context);
		context.ProvideTargetProperty = null;
		dataGridTextColumn3.Binding = binding12;
		dataGridTextColumn3.IsReadOnly = true;
		StyledProperty<bool> isVisibleProperty2 = DataGridColumn.IsVisibleProperty;
		CompiledBindingExtension compiledBindingExtension15 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Not().Ancestor(typeof(UserControl), 2).Property(StyledElement.DataContextProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
			.TypeCast<MainViewModel>()
			.Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002CAvalonia_002EDiagnostics_002EShowDetailsPropertyType_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
			.Build());
		context.ProvideTargetProperty = DataGridColumn.IsVisibleProperty;
		CompiledBindingExtension binding13 = compiledBindingExtension15.ProvideValue(context);
		context.ProvideTargetProperty = null;
		dataGridTextColumn3.Bind(isVisibleProperty2, binding13);
		context.PopParent();
		columns3.Add(item4);
		ObservableCollection<DataGridColumn> columns4 = dataGrid3.Columns;
		DataGridTextColumn item5 = (dataGridTextColumn = new DataGridTextColumn());
		context.PushParent(dataGridTextColumn);
		DataGridTextColumn dataGridTextColumn4 = dataGridTextColumn;
		dataGridTextColumn4.Header = "Assigned Type";
		CompiledBindingExtension compiledBindingExtension16;
		CompiledBindingExtension compiledBindingExtension17 = (compiledBindingExtension16 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EPropertyViewModel_002CAvalonia_002EDiagnostics_002EAssignedType_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build()));
		context.PushParent(compiledBindingExtension16);
		CompiledBindingExtension compiledBindingExtension18 = compiledBindingExtension16;
		StaticResourceExtension staticResourceExtension = new StaticResourceExtension("GetTypeName");
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
		object? obj3 = staticResourceExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		compiledBindingExtension18.Converter = (IValueConverter)obj3;
		context.PopParent();
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EControls_002EDataGridBoundColumn_002CAvalonia_002EControls_002EDataGrid_002EBinding_0021Property();
		CompiledBindingExtension binding14 = compiledBindingExtension17.ProvideValue(context);
		context.ProvideTargetProperty = null;
		dataGridTextColumn4.Binding = binding14;
		dataGridTextColumn4.IsReadOnly = true;
		StyledProperty<bool> isVisibleProperty3 = DataGridColumn.IsVisibleProperty;
		CompiledBindingExtension compiledBindingExtension19 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(UserControl), 2).Property(StyledElement.DataContextProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).TypeCast<MainViewModel>()
			.Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002CAvalonia_002EDiagnostics_002EShowDetailsPropertyType_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
			.Build());
		context.ProvideTargetProperty = DataGridColumn.IsVisibleProperty;
		CompiledBindingExtension binding15 = compiledBindingExtension19.ProvideValue(context);
		context.ProvideTargetProperty = null;
		dataGridTextColumn4.Bind(isVisibleProperty3, binding15);
		context.PopParent();
		columns4.Add(item5);
		ObservableCollection<DataGridColumn> columns5 = dataGrid3.Columns;
		DataGridTextColumn item6 = (dataGridTextColumn = new DataGridTextColumn());
		context.PushParent(dataGridTextColumn);
		DataGridTextColumn dataGridTextColumn5 = dataGridTextColumn;
		dataGridTextColumn5.Header = "Property Type";
		CompiledBindingExtension compiledBindingExtension20 = (compiledBindingExtension16 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EPropertyViewModel_002CAvalonia_002EDiagnostics_002EPropertyType_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build()));
		context.PushParent(compiledBindingExtension16);
		CompiledBindingExtension compiledBindingExtension21 = compiledBindingExtension16;
		StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("GetTypeName");
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
		object? obj4 = staticResourceExtension2.ProvideValue(context);
		context.ProvideTargetProperty = null;
		compiledBindingExtension21.Converter = (IValueConverter)obj4;
		context.PopParent();
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EControls_002EDataGridBoundColumn_002CAvalonia_002EControls_002EDataGrid_002EBinding_0021Property();
		CompiledBindingExtension binding16 = compiledBindingExtension20.ProvideValue(context);
		context.ProvideTargetProperty = null;
		dataGridTextColumn5.Binding = binding16;
		dataGridTextColumn5.IsReadOnly = true;
		StyledProperty<bool> isVisibleProperty4 = DataGridColumn.IsVisibleProperty;
		CompiledBindingExtension compiledBindingExtension22 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(UserControl), 2).Property(StyledElement.DataContextProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).TypeCast<MainViewModel>()
			.Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002CAvalonia_002EDiagnostics_002EShowDetailsPropertyType_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
			.Build());
		context.ProvideTargetProperty = DataGridColumn.IsVisibleProperty;
		CompiledBindingExtension binding17 = compiledBindingExtension22.ProvideValue(context);
		context.ProvideTargetProperty = null;
		dataGridTextColumn5.Bind(isVisibleProperty4, binding17);
		context.PopParent();
		columns5.Add(item6);
		ObservableCollection<DataGridColumn> columns6 = dataGrid3.Columns;
		DataGridTextColumn item7 = (dataGridTextColumn = new DataGridTextColumn());
		context.PushParent(dataGridTextColumn);
		DataGridTextColumn dataGridTextColumn6 = dataGridTextColumn;
		dataGridTextColumn6.Header = "Priority";
		CompiledBindingExtension compiledBindingExtension23 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EPropertyViewModel_002CAvalonia_002EDiagnostics_002EPriority_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EControls_002EDataGridBoundColumn_002CAvalonia_002EControls_002EDataGrid_002EBinding_0021Property();
		CompiledBindingExtension binding18 = compiledBindingExtension23.ProvideValue(context);
		context.ProvideTargetProperty = null;
		dataGridTextColumn6.Binding = binding18;
		dataGridTextColumn6.IsReadOnly = true;
		context.PopParent();
		columns6.Add(item7);
		Styles styles = dataGrid3.Styles;
		Style style = new Style();
		style.Selector = ((Selector?)null).OfType(typeof(DataGridRow)).Descendant().OfType(typeof(TextBox));
		Setter setter = new Setter();
		setter.Property = TextBox.SelectionBrushProperty;
		setter.Value = new ImmutableSolidColorBrush(4289583334u);
		style.Add(setter);
		styles.Add(style);
		context.PopParent();
		((ISupportInitialize)dataGrid4).EndInit();
		context.PopParent();
		((ISupportInitialize)grid8).EndInit();
		Avalonia.Controls.Controls children8 = grid5.Children;
		GridSplitter gridSplitter;
		GridSplitter gridSplitter2 = (gridSplitter = new GridSplitter());
		((ISupportInitialize)gridSplitter2).BeginInit();
		children8.Add(gridSplitter2);
		Grid.SetColumn(gridSplitter, 1);
		((ISupportInitialize)gridSplitter).EndInit();
		Avalonia.Controls.Controls children9 = grid5.Children;
		Grid grid14;
		Grid grid15 = (grid14 = new Grid());
		((ISupportInitialize)grid15).BeginInit();
		children9.Add(grid15);
		Grid grid16 = (grid3 = grid14);
		context.PushParent(grid3);
		Grid grid17 = grid3;
		Grid.SetColumn(grid17, 2);
		RowDefinitions rowDefinitions3 = new RowDefinitions();
		rowDefinitions3.Capacity = 3;
		rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
		rowDefinitions3.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
		grid17.RowDefinitions = rowDefinitions3;
		StyledProperty<bool> isVisibleProperty5 = Visual.IsVisibleProperty;
		CompiledBindingExtension obj5 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EControlDetailsViewModel_002CAvalonia_002EDiagnostics_002ELayout_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build())
		{
			Converter = ObjectConverters.IsNotNull
		};
		context.ProvideTargetProperty = Visual.IsVisibleProperty;
		CompiledBindingExtension binding19 = obj5.ProvideValue(context);
		context.ProvideTargetProperty = null;
		grid17.Bind(isVisibleProperty5, binding19);
		Avalonia.Controls.Controls children10 = grid17.Children;
		Grid grid18;
		Grid grid19 = (grid18 = new Grid());
		((ISupportInitialize)grid19).BeginInit();
		children10.Add(grid19);
		Grid grid20 = (grid3 = grid18);
		context.PushParent(grid3);
		Grid grid21 = grid3;
		RowDefinitions rowDefinitions4 = new RowDefinitions();
		rowDefinitions4.Capacity = 2;
		rowDefinitions4.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		rowDefinitions4.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
		grid21.RowDefinitions = rowDefinitions4;
		Grid.SetRow(grid21, 0);
		Avalonia.Controls.Controls children11 = grid21.Children;
		TextBlock textBlock10;
		TextBlock textBlock11 = (textBlock10 = new TextBlock());
		((ISupportInitialize)textBlock11).BeginInit();
		children11.Add(textBlock11);
		textBlock10.FontWeight = FontWeight.Bold;
		Grid.SetRow(textBlock10, 0);
		textBlock10.Text = "Layout Visualizer";
		textBlock10.Margin = new Thickness(4.0, 4.0, 4.0, 4.0);
		((ISupportInitialize)textBlock10).EndInit();
		Avalonia.Controls.Controls children12 = grid21.Children;
		LayoutExplorerView layoutExplorerView;
		LayoutExplorerView layoutExplorerView2 = (layoutExplorerView = new LayoutExplorerView());
		((ISupportInitialize)layoutExplorerView2).BeginInit();
		children12.Add(layoutExplorerView2);
		LayoutExplorerView layoutExplorerView3;
		LayoutExplorerView layoutExplorerView4 = (layoutExplorerView3 = layoutExplorerView);
		context.PushParent(layoutExplorerView3);
		Grid.SetRow(layoutExplorerView3, 1);
		CompiledBindingExtension compiledBindingExtension24 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EControlDetailsViewModel_002CAvalonia_002EDiagnostics_002ELayout_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = StyledElement.DataContextProperty;
		CompiledBindingExtension compiledBindingExtension25 = compiledBindingExtension24.ProvideValue(context);
		context.ProvideTargetProperty = null;
		DynamicSetters_13.DynamicSetter_1(layoutExplorerView3, compiledBindingExtension25);
		context.PopParent();
		((ISupportInitialize)layoutExplorerView4).EndInit();
		context.PopParent();
		((ISupportInitialize)grid20).EndInit();
		Avalonia.Controls.Controls children13 = grid17.Children;
		GridSplitter gridSplitter3;
		GridSplitter gridSplitter4 = (gridSplitter3 = new GridSplitter());
		((ISupportInitialize)gridSplitter4).BeginInit();
		children13.Add(gridSplitter4);
		Grid.SetRow(gridSplitter3, 1);
		((ISupportInitialize)gridSplitter3).EndInit();
		Avalonia.Controls.Controls children14 = grid17.Children;
		Grid grid22;
		Grid grid23 = (grid22 = new Grid());
		((ISupportInitialize)grid23).BeginInit();
		children14.Add(grid23);
		Grid grid24 = (grid3 = grid22);
		context.PushParent(grid3);
		Grid grid25 = grid3;
		RowDefinitions rowDefinitions5 = new RowDefinitions();
		rowDefinitions5.Capacity = 3;
		rowDefinitions5.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		rowDefinitions5.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
		rowDefinitions5.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		grid25.RowDefinitions = rowDefinitions5;
		Grid.SetRow(grid25, 2);
		Avalonia.Controls.Controls children15 = grid25.Children;
		Grid grid26;
		Grid grid27 = (grid26 = new Grid());
		((ISupportInitialize)grid27).BeginInit();
		children15.Add(grid27);
		Grid grid28 = (grid3 = grid26);
		context.PushParent(grid3);
		Grid grid29 = grid3;
		Grid.SetRow(grid29, 0);
		grid29.Margin = new Thickness(4.0, 4.0, 4.0, 4.0);
		RowDefinitions rowDefinitions6 = new RowDefinitions();
		rowDefinitions6.Capacity = 2;
		rowDefinitions6.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		rowDefinitions6.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		grid29.RowDefinitions = rowDefinitions6;
		Avalonia.Controls.Controls children16 = grid29.Children;
		Grid grid30;
		Grid grid31 = (grid30 = new Grid());
		((ISupportInitialize)grid31).BeginInit();
		children16.Add(grid31);
		Grid grid32 = (grid3 = grid30);
		context.PushParent(grid3);
		Grid grid33 = grid3;
		Grid.SetRow(grid33, 0);
		grid33.Margin = new Thickness(2.0, 2.0, 2.0, 2.0);
		ColumnDefinitions columnDefinitions3 = new ColumnDefinitions();
		columnDefinitions3.Capacity = 4;
		columnDefinitions3.Add(new Avalonia.Controls.ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
		columnDefinitions3.Add(new Avalonia.Controls.ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
		columnDefinitions3.Add(new Avalonia.Controls.ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
		columnDefinitions3.Add(new Avalonia.Controls.ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
		grid33.ColumnDefinitions = columnDefinitions3;
		Avalonia.Controls.Controls children17 = grid33.Children;
		TextBlock textBlock12;
		TextBlock textBlock13 = (textBlock12 = new TextBlock());
		((ISupportInitialize)textBlock13).BeginInit();
		children17.Add(textBlock13);
		TextBlock textBlock14 = (textBlock3 = textBlock12);
		context.PushParent(textBlock3);
		TextBlock textBlock15 = textBlock3;
		textBlock15.FontWeight = FontWeight.Bold;
		Grid.SetColumn(textBlock15, 0);
		StyledProperty<string?> textProperty4 = TextBlock.TextProperty;
		CompiledBindingExtension compiledBindingExtension26 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EControlDetailsViewModel_002CAvalonia_002EDiagnostics_002EStyleStatus_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = TextBlock.TextProperty;
		CompiledBindingExtension binding20 = compiledBindingExtension26.ProvideValue(context);
		context.ProvideTargetProperty = null;
		textBlock15.Bind(textProperty4, binding20);
		textBlock15.VerticalAlignment = VerticalAlignment.Center;
		context.PopParent();
		((ISupportInitialize)textBlock14).EndInit();
		Avalonia.Controls.Controls children18 = grid33.Children;
		CheckBox checkBox;
		CheckBox checkBox2 = (checkBox = new CheckBox());
		((ISupportInitialize)checkBox2).BeginInit();
		children18.Add(checkBox2);
		CheckBox checkBox3;
		CheckBox checkBox4 = (checkBox3 = checkBox);
		context.PushParent(checkBox3);
		checkBox3.Margin = new Thickness(2.0, 0.0, 0.0, 0.0);
		Grid.SetColumn(checkBox3, 2);
		checkBox3.Content = "Show inactive";
		StyledProperty<bool?> isCheckedProperty = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension compiledBindingExtension27 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EControlDetailsViewModel_002CAvalonia_002EDiagnostics_002EShowInactiveStyles_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension binding21 = compiledBindingExtension27.ProvideValue(context);
		context.ProvideTargetProperty = null;
		checkBox3.Bind(isCheckedProperty, binding21);
		ToolTip.SetTip(checkBox3, "Show styles that are currently inactive");
		context.PopParent();
		((ISupportInitialize)checkBox4).EndInit();
		Avalonia.Controls.Controls children19 = grid33.Children;
		ToggleButton toggleButton;
		ToggleButton toggleButton2 = (toggleButton = new ToggleButton());
		((ISupportInitialize)toggleButton2).BeginInit();
		children19.Add(toggleButton2);
		ToggleButton toggleButton3;
		ToggleButton toggleButton4 = (toggleButton3 = toggleButton);
		context.PushParent(toggleButton3);
		toggleButton3.Margin = new Thickness(2.0, 0.0, 0.0, 0.0);
		Grid.SetColumn(toggleButton3, 3);
		ToolTip.SetTip(toggleButton3, "Snapshot current styles (Alt+S/Alt+D to enable/disable within debugged window)");
		toggleButton3.Content = "Snapshot";
		StyledProperty<bool?> isCheckedProperty2 = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension compiledBindingExtension28 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EControlDetailsViewModel_002CAvalonia_002EDiagnostics_002ESnapshotStyles_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension binding22 = compiledBindingExtension28.ProvideValue(context);
		context.ProvideTargetProperty = null;
		toggleButton3.Bind(isCheckedProperty2, binding22);
		context.PopParent();
		((ISupportInitialize)toggleButton4).EndInit();
		context.PopParent();
		((ISupportInitialize)grid32).EndInit();
		Avalonia.Controls.Controls children20 = grid29.Children;
		FilterTextBox filterTextBox6;
		FilterTextBox filterTextBox7 = (filterTextBox6 = new FilterTextBox());
		((ISupportInitialize)filterTextBox7).BeginInit();
		children20.Add(filterTextBox7);
		FilterTextBox filterTextBox8 = (filterTextBox3 = filterTextBox6);
		context.PushParent(filterTextBox3);
		FilterTextBox filterTextBox9 = filterTextBox3;
		Grid.SetRow(filterTextBox9, 1);
		filterTextBox9.Margin = new Thickness(2.0, 2.0, 2.0, 2.0);
		Grid.SetColumn(filterTextBox9, 0);
		CompiledBindingExtension compiledBindingExtension29 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EControlDetailsViewModel_002CAvalonia_002EDiagnostics_002ETreePage_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002CAvalonia_002EDiagnostics_002ESettersFilter_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = StyledElement.DataContextProperty;
		CompiledBindingExtension compiledBindingExtension30 = compiledBindingExtension29.ProvideValue(context);
		context.ProvideTargetProperty = null;
		DynamicSetters_13.DynamicSetter_1(filterTextBox9, compiledBindingExtension30);
		StyledProperty<string?> textProperty5 = TextBox.TextProperty;
		CompiledBindingExtension compiledBindingExtension31 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFilterViewModel_002CAvalonia_002EDiagnostics_002EFilterString_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = TextBox.TextProperty;
		CompiledBindingExtension binding23 = compiledBindingExtension31.ProvideValue(context);
		context.ProvideTargetProperty = null;
		filterTextBox9.Bind(textProperty5, binding23);
		filterTextBox9.Watermark = "Filter setters";
		StyledProperty<bool> useCaseSensitiveFilterProperty2 = FilterTextBox.UseCaseSensitiveFilterProperty;
		CompiledBindingExtension compiledBindingExtension32 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFilterViewModel_002CAvalonia_002EDiagnostics_002EUseCaseSensitiveFilter_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = FilterTextBox.UseCaseSensitiveFilterProperty;
		CompiledBindingExtension binding24 = compiledBindingExtension32.ProvideValue(context);
		context.ProvideTargetProperty = null;
		filterTextBox9.Bind(useCaseSensitiveFilterProperty2, binding24);
		StyledProperty<bool> useWholeWordFilterProperty2 = FilterTextBox.UseWholeWordFilterProperty;
		CompiledBindingExtension compiledBindingExtension33 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFilterViewModel_002CAvalonia_002EDiagnostics_002EUseWholeWordFilter_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = FilterTextBox.UseWholeWordFilterProperty;
		CompiledBindingExtension binding25 = compiledBindingExtension33.ProvideValue(context);
		context.ProvideTargetProperty = null;
		filterTextBox9.Bind(useWholeWordFilterProperty2, binding25);
		StyledProperty<bool> useRegexFilterProperty2 = FilterTextBox.UseRegexFilterProperty;
		CompiledBindingExtension compiledBindingExtension34 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFilterViewModel_002CAvalonia_002EDiagnostics_002EUseRegexFilter_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = FilterTextBox.UseRegexFilterProperty;
		CompiledBindingExtension binding26 = compiledBindingExtension34.ProvideValue(context);
		context.ProvideTargetProperty = null;
		filterTextBox9.Bind(useRegexFilterProperty2, binding26);
		context.PopParent();
		((ISupportInitialize)filterTextBox8).EndInit();
		context.PopParent();
		((ISupportInitialize)grid28).EndInit();
		Avalonia.Controls.Controls children21 = grid25.Children;
		ScrollViewer scrollViewer;
		ScrollViewer scrollViewer2 = (scrollViewer = new ScrollViewer());
		((ISupportInitialize)scrollViewer2).BeginInit();
		children21.Add(scrollViewer2);
		ScrollViewer scrollViewer3;
		ScrollViewer scrollViewer4 = (scrollViewer3 = scrollViewer);
		context.PushParent(scrollViewer3);
		Grid.SetRow(scrollViewer3, 1);
		scrollViewer3.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
		ItemsControl itemsControl;
		ItemsControl itemsControl2 = (itemsControl = new ItemsControl());
		((ISupportInitialize)itemsControl2).BeginInit();
		scrollViewer3.Content = itemsControl2;
		ItemsControl itemsControl3;
		ItemsControl itemsControl4 = (itemsControl3 = itemsControl);
		context.PushParent(itemsControl3);
		ItemsControl itemsControl5 = itemsControl3;
		StyledProperty<IEnumerable?> itemsSourceProperty2 = ItemsControl.ItemsSourceProperty;
		CompiledBindingExtension compiledBindingExtension35 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EControlDetailsViewModel_002CAvalonia_002EDiagnostics_002EAppliedStyles_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ItemsControl.ItemsSourceProperty;
		CompiledBindingExtension binding27 = compiledBindingExtension35.ProvideValue(context);
		context.ProvideTargetProperty = null;
		itemsControl5.Bind(itemsSourceProperty2, binding27);
		DataTemplate dataTemplate;
		DataTemplate itemTemplate = (dataTemplate = new DataTemplate());
		context.PushParent(dataTemplate);
		dataTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_15.Build, context);
		context.PopParent();
		itemsControl5.ItemTemplate = itemTemplate;
		context.PopParent();
		((ISupportInitialize)itemsControl4).EndInit();
		context.PopParent();
		((ISupportInitialize)scrollViewer4).EndInit();
		Avalonia.Controls.Controls children22 = grid25.Children;
		Expander expander;
		Expander expander2 = (expander = new Expander());
		((ISupportInitialize)expander2).BeginInit();
		children22.Add(expander2);
		Expander expander3;
		Expander expander4 = (expander3 = expander);
		context.PushParent(expander3);
		expander3.Header = "Pseudo Classes";
		Grid.SetRow(expander3, 2);
		ItemsControl itemsControl6;
		ItemsControl itemsControl7 = (itemsControl6 = new ItemsControl());
		((ISupportInitialize)itemsControl7).BeginInit();
		expander3.Content = itemsControl7;
		ItemsControl itemsControl8 = (itemsControl3 = itemsControl6);
		context.PushParent(itemsControl3);
		ItemsControl itemsControl9 = itemsControl3;
		StyledProperty<IEnumerable?> itemsSourceProperty3 = ItemsControl.ItemsSourceProperty;
		CompiledBindingExtension compiledBindingExtension36 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EControlDetailsViewModel_002CAvalonia_002EDiagnostics_002EPseudoClasses_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ItemsControl.ItemsSourceProperty;
		CompiledBindingExtension binding28 = compiledBindingExtension36.ProvideValue(context);
		context.ProvideTargetProperty = null;
		itemsControl9.Bind(itemsSourceProperty3, binding28);
		itemsControl9.ItemsPanel = new ItemsPanelTemplate
		{
			Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_24.Build, context)
		};
		DataTemplate itemTemplate2 = (dataTemplate = new DataTemplate());
		context.PushParent(dataTemplate);
		dataTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_25.Build, context);
		context.PopParent();
		itemsControl9.ItemTemplate = itemTemplate2;
		context.PopParent();
		((ISupportInitialize)itemsControl8).EndInit();
		context.PopParent();
		((ISupportInitialize)expander4).EndInit();
		context.PopParent();
		((ISupportInitialize)grid24).EndInit();
		context.PopParent();
		((ISupportInitialize)grid16).EndInit();
		context.PopParent();
		((ISupportInitialize)grid4).EndInit();
		context.PopParent();
		((ISupportInitialize)P_1).EndInit();
		if (P_1 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	private static void _0021XamlIlPopulateTrampoline(ControlDetailsView P_0)
	{
		if (_0021XamlIlPopulateOverride != null)
		{
			_0021XamlIlPopulateOverride(P_0);
		}
		else
		{
			_0021XamlIlPopulate(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(null), P_0);
		}
	}
}
