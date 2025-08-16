using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Diagnostics.Controls;
using Avalonia.Diagnostics.Models;
using Avalonia.Diagnostics.ViewModels;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Styling;
using Avalonia.Threading;
using CompiledAvaloniaXaml;

namespace Avalonia.Diagnostics.Views;

internal class EventsPageView : UserControl
{
	private class DynamicSetters_27
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

		public static void DynamicSetter_2(TreeView P_0, CompiledBindingExtension P_1)
		{
			if (P_1 != null)
			{
				IBinding binding = P_1;
				P_0.Bind(TreeView.SelectedItemProperty, binding);
			}
			else
			{
				P_0.SelectedItem = P_1;
			}
		}

		public static void DynamicSetter_3(SelectingItemsControl P_0, CompiledBindingExtension P_1)
		{
			if (P_1 != null)
			{
				IBinding binding = P_1;
				P_0.Bind(SelectingItemsControl.SelectedItemProperty, binding);
			}
			else
			{
				P_0.SelectedItem = P_1;
			}
		}
	}

	private class XamlClosure_28
	{
		private class DynamicSetters_29
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
			CompiledAvaloniaXaml.XamlIlContext.Context<EventsPageView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<EventsPageView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FEventsPageView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/EventsPageView.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (EventsPageView)service;
				}
			}
			context.IntermediateRoot = new CheckBox();
			object intermediateRoot = context.IntermediateRoot;
			((ISupportInitialize)intermediateRoot).BeginInit();
			CheckBox checkBox = (CheckBox)intermediateRoot;
			context.PushParent(checkBox);
			CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EEventTreeNodeBase_002CAvalonia_002EDiagnostics_002EText_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = ContentControl.ContentProperty;
			CompiledBindingExtension compiledBindingExtension2 = compiledBindingExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_29.DynamicSetter_1(checkBox, compiledBindingExtension2);
			StyledProperty<bool?> isCheckedProperty = ToggleButton.IsCheckedProperty;
			CompiledBindingExtension obj = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EEventTreeNodeBase_002CAvalonia_002EDiagnostics_002EIsEnabled_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build())
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

	private class XamlClosure_30
	{
		private class DynamicSetters_31
		{
			public static void DynamicSetter_1(Control P_0, CompiledBindingExtension P_1)
			{
				if (P_1 != null)
				{
					IBinding binding = P_1;
					P_0.Bind(Control.TagProperty, binding);
				}
				else
				{
					P_0.Tag = P_1;
				}
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<EventsPageView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<EventsPageView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FEventsPageView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/EventsPageView.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (EventsPageView)service;
				}
			}
			context.IntermediateRoot = new ListBoxItem();
			object intermediateRoot = context.IntermediateRoot;
			((ISupportInitialize)intermediateRoot).BeginInit();
			ListBoxItem listBoxItem = (ListBoxItem)intermediateRoot;
			context.PushParent(listBoxItem);
			CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFiredEvent_002CAvalonia_002EDiagnostics_002EIsHandled_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = "class:handled";
			CompiledBindingExtension compiledBindingExtension2 = compiledBindingExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			IBinding source = compiledBindingExtension2;
			listBoxItem.BindClass("handled", source, null);
			Grid grid;
			Grid grid2 = (grid = new Grid());
			((ISupportInitialize)grid2).BeginInit();
			listBoxItem.Content = grid2;
			Grid grid3;
			Grid grid4 = (grid3 = grid);
			context.PushParent(grid3);
			ColumnDefinitions columnDefinitions = new ColumnDefinitions();
			columnDefinitions.Capacity = 5;
			columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
			columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
			columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
			columnDefinitions.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
			columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
			grid3.ColumnDefinitions = columnDefinitions;
			Avalonia.Controls.Controls children = grid3.Children;
			TextBlock textBlock;
			TextBlock textBlock2 = (textBlock = new TextBlock());
			((ISupportInitialize)textBlock2).BeginInit();
			children.Add(textBlock2);
			TextBlock textBlock3;
			TextBlock textBlock4 = (textBlock3 = textBlock);
			context.PushParent(textBlock3);
			TextBlock textBlock5 = textBlock3;
			Grid.SetColumn(textBlock5, 0);
			StyledProperty<string?> textProperty = TextBlock.TextProperty;
			CompiledBindingExtension obj = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFiredEvent_002CAvalonia_002EDiagnostics_002ETriggerTime_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build())
			{
				StringFormat = "{0:HH:mm:ss.fff}"
			};
			context.ProvideTargetProperty = TextBlock.TextProperty;
			CompiledBindingExtension binding = obj.ProvideValue(context);
			context.ProvideTargetProperty = null;
			textBlock5.Bind(textProperty, binding);
			context.PopParent();
			((ISupportInitialize)textBlock4).EndInit();
			Avalonia.Controls.Controls children2 = grid3.Children;
			StackPanel stackPanel;
			StackPanel stackPanel2 = (stackPanel = new StackPanel());
			((ISupportInitialize)stackPanel2).BeginInit();
			children2.Add(stackPanel2);
			StackPanel stackPanel3;
			StackPanel stackPanel4 = (stackPanel3 = stackPanel);
			context.PushParent(stackPanel3);
			StackPanel stackPanel5 = stackPanel3;
			stackPanel5.Margin = new Thickness(10.0, 0.0, 0.0, 0.0);
			Grid.SetColumn(stackPanel5, 1);
			stackPanel5.Spacing = 2.0;
			stackPanel5.Orientation = Orientation.Horizontal;
			Avalonia.Controls.Controls children3 = stackPanel5.Children;
			TextBlock textBlock6;
			TextBlock textBlock7 = (textBlock6 = new TextBlock());
			((ISupportInitialize)textBlock7).BeginInit();
			children3.Add(textBlock7);
			TextBlock textBlock8 = (textBlock3 = textBlock6);
			context.PushParent(textBlock3);
			TextBlock textBlock9 = textBlock3;
			CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFiredEvent_002CAvalonia_002EDiagnostics_002EEvent_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = Control.TagProperty;
			CompiledBindingExtension compiledBindingExtension4 = compiledBindingExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_31.DynamicSetter_1(textBlock9, compiledBindingExtension4);
			textBlock9.DoubleTapped += context.RootObject.NavigateTo;
			StyledProperty<string?> textProperty2 = TextBlock.TextProperty;
			CompiledBindingExtension compiledBindingExtension5 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFiredEvent_002CAvalonia_002EDiagnostics_002EEvent_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EInteractivity_002ERoutedEvent_002CAvalonia_002EBase_002EName_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = TextBlock.TextProperty;
			CompiledBindingExtension binding2 = compiledBindingExtension5.ProvideValue(context);
			context.ProvideTargetProperty = null;
			textBlock9.Bind(textProperty2, binding2);
			textBlock9.FontWeight = FontWeight.Bold;
			textBlock9.Classes.Add("nav");
			context.PopParent();
			((ISupportInitialize)textBlock8).EndInit();
			Avalonia.Controls.Controls children4 = stackPanel5.Children;
			TextBlock textBlock10;
			TextBlock textBlock11 = (textBlock10 = new TextBlock());
			((ISupportInitialize)textBlock11).BeginInit();
			children4.Add(textBlock11);
			textBlock10.Text = "on";
			((ISupportInitialize)textBlock10).EndInit();
			Avalonia.Controls.Controls children5 = stackPanel5.Children;
			TextBlock textBlock12;
			TextBlock textBlock13 = (textBlock12 = new TextBlock());
			((ISupportInitialize)textBlock13).BeginInit();
			children5.Add(textBlock13);
			TextBlock textBlock14 = (textBlock3 = textBlock12);
			context.PushParent(textBlock3);
			TextBlock textBlock15 = textBlock3;
			CompiledBindingExtension compiledBindingExtension6 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFiredEvent_002CAvalonia_002EDiagnostics_002EOriginator_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = Control.TagProperty;
			CompiledBindingExtension compiledBindingExtension7 = compiledBindingExtension6.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_31.DynamicSetter_1(textBlock15, compiledBindingExtension7);
			textBlock15.DoubleTapped += context.RootObject.NavigateTo;
			StyledProperty<string?> textProperty3 = TextBlock.TextProperty;
			CompiledBindingExtension compiledBindingExtension8 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFiredEvent_002CAvalonia_002EDiagnostics_002EOriginator_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EModels_002EEventChainLink_002CAvalonia_002EDiagnostics_002EHandlerName_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = TextBlock.TextProperty;
			CompiledBindingExtension binding3 = compiledBindingExtension8.ProvideValue(context);
			context.ProvideTargetProperty = null;
			textBlock15.Bind(textProperty3, binding3);
			textBlock15.Classes.Add("nav");
			context.PopParent();
			((ISupportInitialize)textBlock14).EndInit();
			context.PopParent();
			((ISupportInitialize)stackPanel4).EndInit();
			Avalonia.Controls.Controls children6 = grid3.Children;
			StackPanel stackPanel6;
			StackPanel stackPanel7 = (stackPanel6 = new StackPanel());
			((ISupportInitialize)stackPanel7).BeginInit();
			children6.Add(stackPanel7);
			StackPanel stackPanel8 = (stackPanel3 = stackPanel6);
			context.PushParent(stackPanel3);
			StackPanel stackPanel9 = stackPanel3;
			stackPanel9.Margin = new Thickness(2.0, 0.0, 0.0, 0.0);
			Grid.SetColumn(stackPanel9, 2);
			stackPanel9.Spacing = 2.0;
			stackPanel9.Orientation = Orientation.Horizontal;
			StyledProperty<bool> isVisibleProperty = Visual.IsVisibleProperty;
			CompiledBindingExtension compiledBindingExtension9 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFiredEvent_002CAvalonia_002EDiagnostics_002EIsHandled_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = Visual.IsVisibleProperty;
			CompiledBindingExtension binding4 = compiledBindingExtension9.ProvideValue(context);
			context.ProvideTargetProperty = null;
			stackPanel9.Bind(isVisibleProperty, binding4);
			Avalonia.Controls.Controls children7 = stackPanel9.Children;
			TextBlock textBlock16;
			TextBlock textBlock17 = (textBlock16 = new TextBlock());
			((ISupportInitialize)textBlock17).BeginInit();
			children7.Add(textBlock17);
			textBlock16.Text = "::";
			((ISupportInitialize)textBlock16).EndInit();
			Avalonia.Controls.Controls children8 = stackPanel9.Children;
			TextBlock textBlock18;
			TextBlock textBlock19 = (textBlock18 = new TextBlock());
			((ISupportInitialize)textBlock19).BeginInit();
			children8.Add(textBlock19);
			textBlock18.Text = "Handled by";
			((ISupportInitialize)textBlock18).EndInit();
			Avalonia.Controls.Controls children9 = stackPanel9.Children;
			TextBlock textBlock20;
			TextBlock textBlock21 = (textBlock20 = new TextBlock());
			((ISupportInitialize)textBlock21).BeginInit();
			children9.Add(textBlock21);
			TextBlock textBlock22 = (textBlock3 = textBlock20);
			context.PushParent(textBlock3);
			TextBlock textBlock23 = textBlock3;
			CompiledBindingExtension compiledBindingExtension10 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFiredEvent_002CAvalonia_002EDiagnostics_002EHandledBy_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = Control.TagProperty;
			CompiledBindingExtension compiledBindingExtension11 = compiledBindingExtension10.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_31.DynamicSetter_1(textBlock23, compiledBindingExtension11);
			textBlock23.DoubleTapped += context.RootObject.NavigateTo;
			StyledProperty<string?> textProperty4 = TextBlock.TextProperty;
			CompiledBindingExtension compiledBindingExtension12 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFiredEvent_002CAvalonia_002EDiagnostics_002EHandledBy_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EModels_002EEventChainLink_002CAvalonia_002EDiagnostics_002EHandlerName_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = TextBlock.TextProperty;
			CompiledBindingExtension binding5 = compiledBindingExtension12.ProvideValue(context);
			context.ProvideTargetProperty = null;
			textBlock23.Bind(textProperty4, binding5);
			textBlock23.Classes.Add("nav");
			context.PopParent();
			((ISupportInitialize)textBlock22).EndInit();
			context.PopParent();
			((ISupportInitialize)stackPanel8).EndInit();
			Avalonia.Controls.Controls children10 = grid3.Children;
			StackPanel stackPanel10;
			StackPanel stackPanel11 = (stackPanel10 = new StackPanel());
			((ISupportInitialize)stackPanel11).BeginInit();
			children10.Add(stackPanel11);
			StackPanel stackPanel12 = (stackPanel3 = stackPanel10);
			context.PushParent(stackPanel3);
			StackPanel stackPanel13 = stackPanel3;
			Grid.SetColumn(stackPanel13, 4);
			stackPanel13.Orientation = Orientation.Horizontal;
			stackPanel13.HorizontalAlignment = HorizontalAlignment.Right;
			Avalonia.Controls.Controls children11 = stackPanel13.Children;
			TextBlock textBlock24;
			TextBlock textBlock25 = (textBlock24 = new TextBlock());
			((ISupportInitialize)textBlock25).BeginInit();
			children11.Add(textBlock25);
			textBlock24.Text = "Routing (";
			((ISupportInitialize)textBlock24).EndInit();
			Avalonia.Controls.Controls children12 = stackPanel13.Children;
			TextBlock textBlock26;
			TextBlock textBlock27 = (textBlock26 = new TextBlock());
			((ISupportInitialize)textBlock27).BeginInit();
			children12.Add(textBlock27);
			TextBlock textBlock28 = (textBlock3 = textBlock26);
			context.PushParent(textBlock3);
			TextBlock target = textBlock3;
			StyledProperty<string?> textProperty5 = TextBlock.TextProperty;
			CompiledBindingExtension compiledBindingExtension13 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFiredEvent_002CAvalonia_002EDiagnostics_002EEvent_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EInteractivity_002ERoutedEvent_002CAvalonia_002EBase_002ERoutingStrategies_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = TextBlock.TextProperty;
			CompiledBindingExtension binding6 = compiledBindingExtension13.ProvideValue(context);
			context.ProvideTargetProperty = null;
			target.Bind(textProperty5, binding6);
			context.PopParent();
			((ISupportInitialize)textBlock28).EndInit();
			Avalonia.Controls.Controls children13 = stackPanel13.Children;
			TextBlock textBlock29;
			TextBlock textBlock30 = (textBlock29 = new TextBlock());
			((ISupportInitialize)textBlock30).BeginInit();
			children13.Add(textBlock30);
			textBlock29.Text = ")";
			((ISupportInitialize)textBlock29).EndInit();
			context.PopParent();
			((ISupportInitialize)stackPanel12).EndInit();
			context.PopParent();
			((ISupportInitialize)grid4).EndInit();
			context.PopParent();
			((ISupportInitialize)intermediateRoot).EndInit();
			return intermediateRoot;
		}
	}

	private class XamlClosure_32
	{
		private class DynamicSetters_33
		{
			public static void DynamicSetter_1(Control P_0, CompiledBindingExtension P_1)
			{
				if (P_1 != null)
				{
					IBinding binding = P_1;
					P_0.Bind(Control.TagProperty, binding);
				}
				else
				{
					P_0.Tag = P_1;
				}
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<EventsPageView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<EventsPageView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FEventsPageView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/EventsPageView.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (EventsPageView)service;
				}
			}
			context.IntermediateRoot = new ListBoxItem();
			object intermediateRoot = context.IntermediateRoot;
			((ISupportInitialize)intermediateRoot).BeginInit();
			ListBoxItem listBoxItem = (ListBoxItem)intermediateRoot;
			context.PushParent(listBoxItem);
			CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EModels_002EEventChainLink_002CAvalonia_002EDiagnostics_002EHandled_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = "class:handled";
			CompiledBindingExtension compiledBindingExtension2 = compiledBindingExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			IBinding source = compiledBindingExtension2;
			listBoxItem.BindClass("handled", source, null);
			StackPanel stackPanel;
			StackPanel stackPanel2 = (stackPanel = new StackPanel());
			((ISupportInitialize)stackPanel2).BeginInit();
			listBoxItem.Content = stackPanel2;
			StackPanel stackPanel3;
			StackPanel stackPanel4 = (stackPanel3 = stackPanel);
			context.PushParent(stackPanel3);
			StackPanel stackPanel5 = stackPanel3;
			stackPanel5.Orientation = Orientation.Vertical;
			Avalonia.Controls.Controls children = stackPanel5.Children;
			Rectangle rectangle;
			Rectangle rectangle2 = (rectangle = new Rectangle());
			((ISupportInitialize)rectangle2).BeginInit();
			children.Add(rectangle2);
			Rectangle rectangle3;
			Rectangle rectangle4 = (rectangle3 = rectangle);
			context.PushParent(rectangle3);
			StyledProperty<bool> isVisibleProperty = Visual.IsVisibleProperty;
			CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EModels_002EEventChainLink_002CAvalonia_002EDiagnostics_002EBeginsNewRoute_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = Visual.IsVisibleProperty;
			CompiledBindingExtension binding = compiledBindingExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			rectangle3.Bind(isVisibleProperty, binding);
			AvaloniaList<double> avaloniaList = new AvaloniaList<double>();
			avaloniaList.Capacity = 2;
			avaloniaList.Add(2.0);
			avaloniaList.Add(2.0);
			rectangle3.StrokeDashArray = avaloniaList;
			rectangle3.StrokeThickness = 1.0;
			rectangle3.Stroke = new ImmutableSolidColorBrush(4286611584u);
			context.PopParent();
			((ISupportInitialize)rectangle4).EndInit();
			Avalonia.Controls.Controls children2 = stackPanel5.Children;
			StackPanel stackPanel6;
			StackPanel stackPanel7 = (stackPanel6 = new StackPanel());
			((ISupportInitialize)stackPanel7).BeginInit();
			children2.Add(stackPanel7);
			StackPanel stackPanel8 = (stackPanel3 = stackPanel6);
			context.PushParent(stackPanel3);
			StackPanel stackPanel9 = stackPanel3;
			stackPanel9.Orientation = Orientation.Horizontal;
			stackPanel9.Spacing = 2.0;
			Avalonia.Controls.Controls children3 = stackPanel9.Children;
			TextBlock textBlock;
			TextBlock textBlock2 = (textBlock = new TextBlock());
			((ISupportInitialize)textBlock2).BeginInit();
			children3.Add(textBlock2);
			TextBlock textBlock3;
			TextBlock textBlock4 = (textBlock3 = textBlock);
			context.PushParent(textBlock3);
			TextBlock textBlock5 = textBlock3;
			StyledProperty<string?> textProperty = TextBlock.TextProperty;
			CompiledBindingExtension compiledBindingExtension4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EModels_002EEventChainLink_002CAvalonia_002EDiagnostics_002ERoute_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = TextBlock.TextProperty;
			CompiledBindingExtension binding2 = compiledBindingExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			textBlock5.Bind(textProperty, binding2);
			textBlock5.FontWeight = FontWeight.Bold;
			context.PopParent();
			((ISupportInitialize)textBlock4).EndInit();
			Avalonia.Controls.Controls children4 = stackPanel9.Children;
			TextBlock textBlock6;
			TextBlock textBlock7 = (textBlock6 = new TextBlock());
			((ISupportInitialize)textBlock7).BeginInit();
			children4.Add(textBlock7);
			TextBlock textBlock8 = (textBlock3 = textBlock6);
			context.PushParent(textBlock3);
			TextBlock textBlock9 = textBlock3;
			CompiledBindingExtension compiledBindingExtension5 = new CompiledBindingExtension();
			context.ProvideTargetProperty = Control.TagProperty;
			CompiledBindingExtension compiledBindingExtension6 = compiledBindingExtension5.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_33.DynamicSetter_1(textBlock9, compiledBindingExtension6);
			textBlock9.DoubleTapped += context.RootObject.NavigateTo;
			StyledProperty<string?> textProperty2 = TextBlock.TextProperty;
			CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EModels_002EEventChainLink_002CAvalonia_002EDiagnostics_002EHandlerName_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = TextBlock.TextProperty;
			CompiledBindingExtension binding3 = compiledBindingExtension7.ProvideValue(context);
			context.ProvideTargetProperty = null;
			textBlock9.Bind(textProperty2, binding3);
			textBlock9.Classes.Add("nav");
			context.PopParent();
			((ISupportInitialize)textBlock8).EndInit();
			context.PopParent();
			((ISupportInitialize)stackPanel8).EndInit();
			context.PopParent();
			((ISupportInitialize)stackPanel4).EndInit();
			context.PopParent();
			((ISupportInitialize)intermediateRoot).EndInit();
			return intermediateRoot;
		}
	}

	private readonly ListBox _events;

	private static Action<object> _0021XamlIlPopulateOverride;

	public EventsPageView()
	{
		InitializeComponent();
		_events = this.GetControl<ListBox>("EventsList");
	}

	public void NavigateTo(object sender, TappedEventArgs e)
	{
		if (!(base.DataContext is EventsPageViewModel eventsPageViewModel) || !(sender is Control control))
		{
			return;
		}
		object tag = control.Tag;
		if (!(tag is EventChainLink navTarget))
		{
			if (tag is RoutedEvent evt)
			{
				eventsPageViewModel.SelectEventByType(evt);
			}
		}
		else
		{
			eventsPageViewModel.RequestTreeNavigateTo(navTarget);
		}
	}

	protected override void OnDataContextChanged(EventArgs e)
	{
		base.OnDataContextChanged(e);
		if (base.DataContext is EventsPageViewModel eventsPageViewModel)
		{
			eventsPageViewModel.RecordedEvents.CollectionChanged += OnRecordedEventsChanged;
		}
	}

	private void OnRecordedEventsChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (!(sender is ObservableCollection<FiredEvent> source))
		{
			return;
		}
		FiredEvent evt = source.LastOrDefault();
		if (evt != null)
		{
			Dispatcher.UIThread.Post(delegate
			{
				_events.ScrollIntoView(evt);
			});
		}
	}

	private void InitializeComponent()
	{
		_0021XamlIlPopulateTrampoline(this);
	}

	static void _0021XamlIlPopulate(IServiceProvider P_0, EventsPageView P_1)
	{
		CompiledAvaloniaXaml.XamlIlContext.Context<EventsPageView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<EventsPageView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FEventsPageView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/EventsPageView.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		((ISupportInitialize)P_1).BeginInit();
		context.PushParent(P_1);
		P_1.Margin = new Thickness(2.0, 2.0, 2.0, 2.0);
		Styles styles = P_1.Styles;
		Style style = new Style();
		style.Selector = ((Selector?)null).OfType(typeof(TextBlock)).Class("nav");
		Setter setter = new Setter();
		setter.Property = TextBlock.TextDecorationsProperty;
		TextDecorationCollection textDecorationCollection = new TextDecorationCollection();
		TextDecoration textDecoration = new TextDecoration();
		textDecoration.Location = TextDecorationLocation.Underline;
		textDecoration.Stroke = new ImmutableSolidColorBrush(4278190080u);
		textDecoration.StrokeThickness = 1.0;
		AvaloniaList<double> avaloniaList = new AvaloniaList<double>();
		avaloniaList.Capacity = 2;
		avaloniaList.Add(2.0);
		avaloniaList.Add(2.0);
		textDecoration.StrokeDashArray = avaloniaList;
		textDecorationCollection.Add(textDecoration);
		setter.Value = textDecorationCollection;
		style.Add(setter);
		styles.Add(style);
		Styles styles2 = P_1.Styles;
		Style style2;
		Style item = (style2 = new Style());
		context.PushParent(style2);
		Style style3 = style2;
		style3.Selector = ((Selector?)null).OfType(typeof(TextBlock)).Class("nav").Class(":pointerover");
		Setter setter2;
		Setter setter3 = (setter2 = new Setter());
		context.PushParent(setter2);
		Setter setter4 = setter2;
		setter4.Property = TextBlock.ForegroundProperty;
		DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeAccentBrush");
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
		IBinding value = dynamicResourceExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		setter4.Value = value;
		context.PopParent();
		style3.Add(setter3);
		Setter setter5 = new Setter();
		setter5.Property = InputElement.CursorProperty;
		setter5.Value = new Cursor(StandardCursorType.Help);
		style3.Add(setter5);
		context.PopParent();
		styles2.Add(item);
		Styles styles3 = P_1.Styles;
		Style style4 = new Style();
		style4.Selector = ((Selector?)null).OfType(typeof(ListBoxItem));
		Setter setter6 = new Setter();
		setter6.Property = TemplatedControl.BorderThicknessProperty;
		setter6.Value = new Thickness(1.0, 1.0, 1.0, 1.0);
		style4.Add(setter6);
		styles3.Add(style4);
		Styles styles4 = P_1.Styles;
		Style style5 = new Style();
		style5.Selector = ((Selector?)null).OfType(typeof(ListBoxItem)).Class(":selected").Template()
			.OfType(typeof(ContentPresenter));
		Setter setter7 = new Setter();
		setter7.Property = ContentPresenter.BorderBrushProperty;
		setter7.Value = new ImmutableSolidColorBrush(4278190080u);
		style5.Add(setter7);
		styles4.Add(style5);
		Styles styles5 = P_1.Styles;
		Style style6 = new Style();
		style6.Selector = ((Selector?)null).OfType(typeof(ListBoxItem)).Class("handled");
		Setter setter8 = new Setter();
		setter8.Property = TemplatedControl.BackgroundProperty;
		setter8.Value = new ImmutableSolidColorBrush(4281649472u);
		style6.Add(setter8);
		Setter setter9 = new Setter();
		setter9.Property = TemplatedControl.ForegroundProperty;
		setter9.Value = new ImmutableSolidColorBrush(4278190080u);
		style6.Add(setter9);
		styles5.Add(style6);
		Grid grid;
		Grid grid2 = (grid = new Grid());
		((ISupportInitialize)grid2).BeginInit();
		P_1.Content = grid2;
		Grid grid3;
		Grid grid4 = (grid3 = grid);
		context.PushParent(grid3);
		Grid grid5 = grid3;
		ColumnDefinitions columnDefinitions = new ColumnDefinitions();
		columnDefinitions.Capacity = 3;
		columnDefinitions.Add(new ColumnDefinition(new GridLength(1.1, GridUnitType.Star)));
		columnDefinitions.Add(new ColumnDefinition(new GridLength(4.0, GridUnitType.Pixel)));
		columnDefinitions.Add(new ColumnDefinition(new GridLength(3.0, GridUnitType.Star)));
		grid5.ColumnDefinitions = columnDefinitions;
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
		rowDefinitions.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
		rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		grid9.RowDefinitions = rowDefinitions;
		Avalonia.Controls.Controls children2 = grid9.Children;
		FilterTextBox filterTextBox;
		FilterTextBox filterTextBox2 = (filterTextBox = new FilterTextBox());
		((ISupportInitialize)filterTextBox2).BeginInit();
		children2.Add(filterTextBox2);
		FilterTextBox filterTextBox3;
		FilterTextBox filterTextBox4 = (filterTextBox3 = filterTextBox);
		context.PushParent(filterTextBox3);
		Grid.SetRow(filterTextBox3, 0);
		filterTextBox3.Margin = new Thickness(0.0, 0.0, 0.0, 2.0);
		CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EEventsPageViewModel_002CAvalonia_002EDiagnostics_002EEventsFilter_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = StyledElement.DataContextProperty;
		CompiledBindingExtension compiledBindingExtension2 = compiledBindingExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		DynamicSetters_27.DynamicSetter_1(filterTextBox3, compiledBindingExtension2);
		StyledProperty<string?> textProperty = TextBox.TextProperty;
		CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFilterViewModel_002CAvalonia_002EDiagnostics_002EFilterString_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = TextBox.TextProperty;
		CompiledBindingExtension binding = compiledBindingExtension3.ProvideValue(context);
		context.ProvideTargetProperty = null;
		filterTextBox3.Bind(textProperty, binding);
		filterTextBox3.Watermark = "Filter events";
		StyledProperty<bool> useCaseSensitiveFilterProperty = FilterTextBox.UseCaseSensitiveFilterProperty;
		CompiledBindingExtension compiledBindingExtension4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFilterViewModel_002CAvalonia_002EDiagnostics_002EUseCaseSensitiveFilter_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = FilterTextBox.UseCaseSensitiveFilterProperty;
		CompiledBindingExtension binding2 = compiledBindingExtension4.ProvideValue(context);
		context.ProvideTargetProperty = null;
		filterTextBox3.Bind(useCaseSensitiveFilterProperty, binding2);
		StyledProperty<bool> useWholeWordFilterProperty = FilterTextBox.UseWholeWordFilterProperty;
		CompiledBindingExtension compiledBindingExtension5 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFilterViewModel_002CAvalonia_002EDiagnostics_002EUseWholeWordFilter_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = FilterTextBox.UseWholeWordFilterProperty;
		CompiledBindingExtension binding3 = compiledBindingExtension5.ProvideValue(context);
		context.ProvideTargetProperty = null;
		filterTextBox3.Bind(useWholeWordFilterProperty, binding3);
		StyledProperty<bool> useRegexFilterProperty = FilterTextBox.UseRegexFilterProperty;
		CompiledBindingExtension compiledBindingExtension6 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFilterViewModel_002CAvalonia_002EDiagnostics_002EUseRegexFilter_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = FilterTextBox.UseRegexFilterProperty;
		CompiledBindingExtension binding4 = compiledBindingExtension6.ProvideValue(context);
		context.ProvideTargetProperty = null;
		filterTextBox3.Bind(useRegexFilterProperty, binding4);
		context.PopParent();
		((ISupportInitialize)filterTextBox4).EndInit();
		Avalonia.Controls.Controls children3 = grid9.Children;
		TreeView treeView;
		TreeView treeView2 = (treeView = new TreeView());
		((ISupportInitialize)treeView2).BeginInit();
		children3.Add(treeView2);
		TreeView treeView3;
		TreeView treeView4 = (treeView3 = treeView);
		context.PushParent(treeView3);
		Grid.SetRow(treeView3, 1);
		StyledProperty<IEnumerable?> itemsSourceProperty = ItemsControl.ItemsSourceProperty;
		CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EEventsPageViewModel_002CAvalonia_002EDiagnostics_002ENodes_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ItemsControl.ItemsSourceProperty;
		CompiledBindingExtension binding5 = compiledBindingExtension7.ProvideValue(context);
		context.ProvideTargetProperty = null;
		treeView3.Bind(itemsSourceProperty, binding5);
		CompiledBindingExtension obj = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EEventsPageViewModel_002CAvalonia_002EDiagnostics_002ESelectedNode_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build())
		{
			Mode = BindingMode.TwoWay
		};
		context.ProvideTargetProperty = TreeView.SelectedItemProperty;
		CompiledBindingExtension compiledBindingExtension8 = obj.ProvideValue(context);
		context.ProvideTargetProperty = null;
		DynamicSetters_27.DynamicSetter_2(treeView3, compiledBindingExtension8);
		DataTemplates dataTemplates = treeView3.DataTemplates;
		TreeDataTemplate treeDataTemplate;
		TreeDataTemplate item2 = (treeDataTemplate = new TreeDataTemplate());
		context.PushParent(treeDataTemplate);
		treeDataTemplate.DataType = typeof(EventTreeNodeBase);
		CompiledBindingExtension compiledBindingExtension9 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EEventTreeNodeBase_002CAvalonia_002EDiagnostics_002EChildren_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EMarkup_002EXaml_002ETemplates_002ETreeDataTemplate_002CAvalonia_002EMarkup_002EXaml_002EItemsSource_0021Property();
		CompiledBindingExtension itemsSource = compiledBindingExtension9.ProvideValue(context);
		context.ProvideTargetProperty = null;
		treeDataTemplate.ItemsSource = itemsSource;
		treeDataTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_28.Build, context);
		context.PopParent();
		dataTemplates.Add(item2);
		Styles styles6 = treeView3.Styles;
		Style item3 = (style2 = new Style());
		context.PushParent(style2);
		Style style7 = style2;
		style7.Selector = ((Selector?)null).OfType(typeof(TreeViewItem));
		Setter setter10 = (setter2 = new Setter());
		context.PushParent(setter2);
		Setter setter11 = setter2;
		setter11.Property = TreeViewItem.IsExpandedProperty;
		CompiledBindingExtension obj2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EEventTreeNodeBase_002CAvalonia_002EDiagnostics_002EIsExpanded_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build())
		{
			Mode = BindingMode.TwoWay
		};
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
		CompiledBindingExtension value2 = obj2.ProvideValue(context);
		context.ProvideTargetProperty = null;
		setter11.Value = value2;
		context.PopParent();
		style7.Add(setter10);
		Setter setter12 = (setter2 = new Setter());
		context.PushParent(setter2);
		Setter setter13 = setter2;
		setter13.Property = Visual.IsVisibleProperty;
		CompiledBindingExtension compiledBindingExtension10 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EEventTreeNodeBase_002CAvalonia_002EDiagnostics_002EIsVisible_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
		CompiledBindingExtension value3 = compiledBindingExtension10.ProvideValue(context);
		context.ProvideTargetProperty = null;
		setter13.Value = value3;
		context.PopParent();
		style7.Add(setter12);
		context.PopParent();
		styles6.Add(item3);
		context.PopParent();
		((ISupportInitialize)treeView4).EndInit();
		Avalonia.Controls.Controls children4 = grid9.Children;
		StackPanel stackPanel;
		StackPanel stackPanel2 = (stackPanel = new StackPanel());
		((ISupportInitialize)stackPanel2).BeginInit();
		children4.Add(stackPanel2);
		StackPanel stackPanel3;
		StackPanel stackPanel4 = (stackPanel3 = stackPanel);
		context.PushParent(stackPanel3);
		StackPanel stackPanel5 = stackPanel3;
		Grid.SetRow(stackPanel5, 2);
		stackPanel5.Margin = new Thickness(0.0, 2.0, 0.0, 2.0);
		stackPanel5.Orientation = Orientation.Horizontal;
		stackPanel5.Spacing = 2.0;
		Avalonia.Controls.Controls children5 = stackPanel5.Children;
		Button button;
		Button button2 = (button = new Button());
		((ISupportInitialize)button2).BeginInit();
		children5.Add(button2);
		Button button3;
		Button button4 = (button3 = button);
		context.PushParent(button3);
		Button button5 = button3;
		button5.Content = "Disable all";
		StyledProperty<ICommand?> commandProperty = Button.CommandProperty;
		CompiledBindingExtension compiledBindingExtension11 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("DisableAll", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EEventsPageViewModel_002BDisableAll_0_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = Button.CommandProperty;
		CompiledBindingExtension binding6 = compiledBindingExtension11.ProvideValue(context);
		context.ProvideTargetProperty = null;
		button5.Bind(commandProperty, binding6);
		context.PopParent();
		((ISupportInitialize)button4).EndInit();
		Avalonia.Controls.Controls children6 = stackPanel5.Children;
		Button button6;
		Button button7 = (button6 = new Button());
		((ISupportInitialize)button7).BeginInit();
		children6.Add(button7);
		Button button8 = (button3 = button6);
		context.PushParent(button3);
		Button button9 = button3;
		button9.Content = "Enable default";
		StyledProperty<ICommand?> commandProperty2 = Button.CommandProperty;
		CompiledBindingExtension compiledBindingExtension12 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("EnableDefault", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EEventsPageViewModel_002BEnableDefault_0_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = Button.CommandProperty;
		CompiledBindingExtension binding7 = compiledBindingExtension12.ProvideValue(context);
		context.ProvideTargetProperty = null;
		button9.Bind(commandProperty2, binding7);
		context.PopParent();
		((ISupportInitialize)button8).EndInit();
		context.PopParent();
		((ISupportInitialize)stackPanel4).EndInit();
		context.PopParent();
		((ISupportInitialize)grid8).EndInit();
		Avalonia.Controls.Controls children7 = grid5.Children;
		GridSplitter gridSplitter;
		GridSplitter gridSplitter2 = (gridSplitter = new GridSplitter());
		((ISupportInitialize)gridSplitter2).BeginInit();
		children7.Add(gridSplitter2);
		gridSplitter.Width = 4.0;
		Grid.SetColumn(gridSplitter, 1);
		((ISupportInitialize)gridSplitter).EndInit();
		Avalonia.Controls.Controls children8 = grid5.Children;
		Grid grid10;
		Grid grid11 = (grid10 = new Grid());
		((ISupportInitialize)grid11).BeginInit();
		children8.Add(grid11);
		Grid grid12 = (grid3 = grid10);
		context.PushParent(grid3);
		Grid grid13 = grid3;
		RowDefinitions rowDefinitions2 = new RowDefinitions();
		rowDefinitions2.Capacity = 4;
		rowDefinitions2.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
		rowDefinitions2.Add(new RowDefinition(new GridLength(4.0, GridUnitType.Pixel)));
		rowDefinitions2.Add(new RowDefinition(new GridLength(2.0, GridUnitType.Star)));
		rowDefinitions2.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		grid13.RowDefinitions = rowDefinitions2;
		Grid.SetColumn(grid13, 2);
		Avalonia.Controls.Controls children9 = grid13.Children;
		ListBox listBox;
		ListBox listBox2 = (listBox = new ListBox());
		((ISupportInitialize)listBox2).BeginInit();
		children9.Add(listBox2);
		ListBox listBox3;
		ListBox listBox4 = (listBox3 = listBox);
		context.PushParent(listBox3);
		ListBox listBox5 = listBox3;
		listBox5.Name = "EventsList";
		object element = listBox5;
		context.AvaloniaNameScope.Register("EventsList", element);
		StyledProperty<IEnumerable?> itemsSourceProperty2 = ItemsControl.ItemsSourceProperty;
		CompiledBindingExtension compiledBindingExtension13 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EEventsPageViewModel_002CAvalonia_002EDiagnostics_002ERecordedEvents_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ItemsControl.ItemsSourceProperty;
		CompiledBindingExtension binding8 = compiledBindingExtension13.ProvideValue(context);
		context.ProvideTargetProperty = null;
		listBox5.Bind(itemsSourceProperty2, binding8);
		CompiledBindingExtension obj3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EEventsPageViewModel_002CAvalonia_002EDiagnostics_002ESelectedEvent_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build())
		{
			Mode = BindingMode.TwoWay
		};
		context.ProvideTargetProperty = SelectingItemsControl.SelectedItemProperty;
		CompiledBindingExtension compiledBindingExtension14 = obj3.ProvideValue(context);
		context.ProvideTargetProperty = null;
		DynamicSetters_27.DynamicSetter_3(listBox5, compiledBindingExtension14);
		DataTemplate dataTemplate;
		DataTemplate itemTemplate = (dataTemplate = new DataTemplate());
		context.PushParent(dataTemplate);
		dataTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_30.Build, context);
		context.PopParent();
		listBox5.ItemTemplate = itemTemplate;
		context.PopParent();
		((ISupportInitialize)listBox4).EndInit();
		Avalonia.Controls.Controls children10 = grid13.Children;
		GridSplitter gridSplitter3;
		GridSplitter gridSplitter4 = (gridSplitter3 = new GridSplitter());
		((ISupportInitialize)gridSplitter4).BeginInit();
		children10.Add(gridSplitter4);
		gridSplitter3.Height = 4.0;
		Grid.SetRow(gridSplitter3, 1);
		((ISupportInitialize)gridSplitter3).EndInit();
		Avalonia.Controls.Controls children11 = grid13.Children;
		DockPanel dockPanel;
		DockPanel dockPanel2 = (dockPanel = new DockPanel());
		((ISupportInitialize)dockPanel2).BeginInit();
		children11.Add(dockPanel2);
		DockPanel dockPanel3;
		DockPanel dockPanel4 = (dockPanel3 = dockPanel);
		context.PushParent(dockPanel3);
		Grid.SetRow(dockPanel3, 2);
		dockPanel3.LastChildFill = true;
		Avalonia.Controls.Controls children12 = dockPanel3.Children;
		TextBlock textBlock;
		TextBlock textBlock2 = (textBlock = new TextBlock());
		((ISupportInitialize)textBlock2).BeginInit();
		children12.Add(textBlock2);
		DockPanel.SetDock(textBlock, Dock.Top);
		textBlock.FontSize = 16.0;
		textBlock.Text = "Event chain:";
		((ISupportInitialize)textBlock).EndInit();
		Avalonia.Controls.Controls children13 = dockPanel3.Children;
		ListBox listBox6;
		ListBox listBox7 = (listBox6 = new ListBox());
		((ISupportInitialize)listBox7).BeginInit();
		children13.Add(listBox7);
		ListBox listBox8 = (listBox3 = listBox6);
		context.PushParent(listBox3);
		ListBox listBox9 = listBox3;
		StyledProperty<IEnumerable?> itemsSourceProperty3 = ItemsControl.ItemsSourceProperty;
		CompiledBindingExtension compiledBindingExtension15 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EEventsPageViewModel_002CAvalonia_002EDiagnostics_002ESelectedEvent_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EFiredEvent_002CAvalonia_002EDiagnostics_002EEventChain_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ItemsControl.ItemsSourceProperty;
		CompiledBindingExtension binding9 = compiledBindingExtension15.ProvideValue(context);
		context.ProvideTargetProperty = null;
		listBox9.Bind(itemsSourceProperty3, binding9);
		DataTemplate itemTemplate2 = (dataTemplate = new DataTemplate());
		context.PushParent(dataTemplate);
		dataTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_32.Build, context);
		context.PopParent();
		listBox9.ItemTemplate = itemTemplate2;
		context.PopParent();
		((ISupportInitialize)listBox8).EndInit();
		context.PopParent();
		((ISupportInitialize)dockPanel4).EndInit();
		Avalonia.Controls.Controls children14 = grid13.Children;
		StackPanel stackPanel6;
		StackPanel stackPanel7 = (stackPanel6 = new StackPanel());
		((ISupportInitialize)stackPanel7).BeginInit();
		children14.Add(stackPanel7);
		StackPanel stackPanel8 = (stackPanel3 = stackPanel6);
		context.PushParent(stackPanel3);
		StackPanel stackPanel9 = stackPanel3;
		stackPanel9.Orientation = Orientation.Horizontal;
		Grid.SetRow(stackPanel9, 3);
		stackPanel9.Spacing = 2.0;
		stackPanel9.Margin = new Thickness(2.0, 2.0, 2.0, 2.0);
		Avalonia.Controls.Controls children15 = stackPanel9.Children;
		Button button10;
		Button button11 = (button10 = new Button());
		((ISupportInitialize)button11).BeginInit();
		children15.Add(button11);
		Button button12 = (button3 = button10);
		context.PushParent(button3);
		Button button13 = button3;
		button13.Content = "Clear";
		StyledProperty<ICommand?> commandProperty3 = Button.CommandProperty;
		CompiledBindingExtension compiledBindingExtension16 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("Clear", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EEventsPageViewModel_002BClear_0_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = Button.CommandProperty;
		CompiledBindingExtension binding10 = compiledBindingExtension16.ProvideValue(context);
		context.ProvideTargetProperty = null;
		button13.Bind(commandProperty3, binding10);
		context.PopParent();
		((ISupportInitialize)button12).EndInit();
		context.PopParent();
		((ISupportInitialize)stackPanel8).EndInit();
		context.PopParent();
		((ISupportInitialize)grid12).EndInit();
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

	private static void _0021XamlIlPopulateTrampoline(EventsPageView P_0)
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
