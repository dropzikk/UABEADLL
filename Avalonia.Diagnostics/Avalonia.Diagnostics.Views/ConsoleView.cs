using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Diagnostics.ViewModels;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using CompiledAvaloniaXaml;

namespace Avalonia.Diagnostics.Views;

internal class ConsoleView : UserControl
{
	private class XamlClosure_9
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<ConsoleView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<ConsoleView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FConsoleView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/ConsoleView.xaml");
			object service;
			if (P_0 != null)
			{
				service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ConsoleView)service;
				}
			}
			context.IntermediateRoot = new Border();
			object intermediateRoot = context.IntermediateRoot;
			((ISupportInitialize)intermediateRoot).BeginInit();
			((StyledElement)intermediateRoot).Name = "border";
			service = intermediateRoot;
			context.AvaloniaNameScope.Register("border", service);
			((AvaloniaObject)intermediateRoot).Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
			((AvaloniaObject)intermediateRoot).Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
			((AvaloniaObject)intermediateRoot).Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
			DockPanel dockPanel;
			DockPanel dockPanel2 = (dockPanel = new DockPanel());
			((ISupportInitialize)dockPanel2).BeginInit();
			((Decorator)intermediateRoot).Child = dockPanel2;
			dockPanel.Bind(Layoutable.MarginProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
			Avalonia.Controls.Controls children = dockPanel.Children;
			TextBlock textBlock;
			TextBlock textBlock2 = (textBlock = new TextBlock());
			((ISupportInitialize)textBlock2).BeginInit();
			children.Add(textBlock2);
			textBlock.SetValue(DockPanel.DockProperty, Dock.Left, BindingPriority.Template);
			textBlock.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 4.0, 0.0), BindingPriority.Template);
			((ISupportInitialize)textBlock).EndInit();
			Avalonia.Controls.Controls children2 = dockPanel.Children;
			TextPresenter textPresenter;
			TextPresenter textPresenter2 = (textPresenter = new TextPresenter());
			((ISupportInitialize)textPresenter2).BeginInit();
			children2.Add(textPresenter2);
			textPresenter.Name = "PART_TextPresenter";
			service = textPresenter;
			context.AvaloniaNameScope.Register("PART_TextPresenter", service);
			textPresenter.Bind(TextPresenter.TextProperty, new TemplateBinding(TextBox.TextProperty)
			{
				Mode = BindingMode.TwoWay
			}.ProvideValue());
			textPresenter.Bind(TextPresenter.CaretIndexProperty, new TemplateBinding(TextBox.CaretIndexProperty).ProvideValue());
			textPresenter.Bind(TextPresenter.SelectionStartProperty, new TemplateBinding(TextBox.SelectionStartProperty).ProvideValue());
			textPresenter.Bind(TextPresenter.SelectionEndProperty, new TemplateBinding(TextBox.SelectionEndProperty).ProvideValue());
			textPresenter.Bind(TextPresenter.TextAlignmentProperty, new TemplateBinding(TextBox.TextAlignmentProperty).ProvideValue());
			textPresenter.Bind(TextPresenter.TextWrappingProperty, new TemplateBinding(TextBox.TextWrappingProperty).ProvideValue());
			textPresenter.Bind(TextPresenter.PasswordCharProperty, new TemplateBinding(TextBox.PasswordCharProperty).ProvideValue());
			((ISupportInitialize)textPresenter).EndInit();
			((ISupportInitialize)dockPanel).EndInit();
			((ISupportInitialize)intermediateRoot).EndInit();
			return intermediateRoot;
		}
	}

	private class XamlClosure_10
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<ConsoleView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<ConsoleView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FConsoleView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/ConsoleView.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (ConsoleView)service;
				}
			}
			context.IntermediateRoot = new StackPanel();
			object intermediateRoot = context.IntermediateRoot;
			((ISupportInitialize)intermediateRoot).BeginInit();
			StackPanel stackPanel = (StackPanel)intermediateRoot;
			context.PushParent(stackPanel);
			stackPanel.Orientation = Orientation.Vertical;
			Avalonia.Controls.Controls children = stackPanel.Children;
			DockPanel dockPanel;
			DockPanel dockPanel2 = (dockPanel = new DockPanel());
			((ISupportInitialize)dockPanel2).BeginInit();
			children.Add(dockPanel2);
			DockPanel dockPanel3;
			DockPanel dockPanel4 = (dockPanel3 = dockPanel);
			context.PushParent(dockPanel3);
			Avalonia.Controls.Controls children2 = dockPanel3.Children;
			TextBlock textBlock;
			TextBlock textBlock2 = (textBlock = new TextBlock());
			((ISupportInitialize)textBlock2).BeginInit();
			children2.Add(textBlock2);
			DockPanel.SetDock(textBlock, Dock.Left);
			textBlock.Margin = new Thickness(0.0, 0.0, 4.0, 0.0);
			((ISupportInitialize)textBlock).EndInit();
			Avalonia.Controls.Controls children3 = dockPanel3.Children;
			TextBlock textBlock3;
			TextBlock textBlock4 = (textBlock3 = new TextBlock());
			((ISupportInitialize)textBlock4).BeginInit();
			children3.Add(textBlock4);
			TextBlock textBlock5;
			TextBlock textBlock6 = (textBlock5 = textBlock3);
			context.PushParent(textBlock5);
			TextBlock target = textBlock5;
			StyledProperty<string?> textProperty = TextBlock.TextProperty;
			CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EModels_002EConsoleHistoryItem_002CAvalonia_002EDiagnostics_002EInput_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = TextBlock.TextProperty;
			CompiledBindingExtension binding = compiledBindingExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			target.Bind(textProperty, binding);
			context.PopParent();
			((ISupportInitialize)textBlock6).EndInit();
			context.PopParent();
			((ISupportInitialize)dockPanel4).EndInit();
			Avalonia.Controls.Controls children4 = stackPanel.Children;
			TextBlock textBlock7;
			TextBlock textBlock8 = (textBlock7 = new TextBlock());
			((ISupportInitialize)textBlock8).BeginInit();
			children4.Add(textBlock8);
			TextBlock textBlock9 = (textBlock5 = textBlock7);
			context.PushParent(textBlock5);
			TextBlock target2 = textBlock5;
			StyledProperty<IBrush?> foregroundProperty = TextBlock.ForegroundProperty;
			CompiledBindingExtension compiledBindingExtension2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EModels_002EConsoleHistoryItem_002CAvalonia_002EDiagnostics_002EForeground_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = TextBlock.ForegroundProperty;
			CompiledBindingExtension binding2 = compiledBindingExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			target2.Bind(foregroundProperty, binding2);
			StyledProperty<string?> textProperty2 = TextBlock.TextProperty;
			CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EModels_002EConsoleHistoryItem_002CAvalonia_002EDiagnostics_002EOutput_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = TextBlock.TextProperty;
			CompiledBindingExtension binding3 = compiledBindingExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			target2.Bind(textProperty2, binding3);
			context.PopParent();
			((ISupportInitialize)textBlock9).EndInit();
			context.PopParent();
			((ISupportInitialize)intermediateRoot).EndInit();
			return intermediateRoot;
		}
	}

	private readonly ListBox _historyList;

	private readonly TextBox _input;

	private static Action<object> _0021XamlIlPopulateOverride;

	public ConsoleView()
	{
		InitializeComponent();
		_historyList = this.GetControl<ListBox>("historyList");
		((ILogical)_historyList).LogicalChildren.CollectionChanged += HistoryChanged;
		_input = this.GetControl<TextBox>("input");
		_input.KeyDown += InputKeyDown;
	}

	public void FocusInput()
	{
		_input.Focus();
	}

	private void InitializeComponent()
	{
		_0021XamlIlPopulateTrampoline(this);
	}

	private void HistoryChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems?[0] is Control @object)
		{
			DispatcherTimer.RunOnce(@object.BringIntoView, TimeSpan.Zero);
		}
	}

	private void InputKeyDown(object? sender, KeyEventArgs e)
	{
		ConsoleViewModel consoleViewModel = (ConsoleViewModel)base.DataContext;
		if (consoleViewModel != null)
		{
			switch (e.Key)
			{
			case Key.Return:
				consoleViewModel.Execute();
				e.Handled = true;
				break;
			case Key.Up:
				consoleViewModel.HistoryUp();
				_input.CaretIndex = _input.Text?.Length ?? 0;
				e.Handled = true;
				break;
			case Key.Down:
				consoleViewModel.HistoryDown();
				_input.CaretIndex = _input.Text?.Length ?? 0;
				e.Handled = true;
				break;
			}
		}
	}

	static void _0021XamlIlPopulate(IServiceProvider P_0, ConsoleView P_1)
	{
		CompiledAvaloniaXaml.XamlIlContext.Context<ConsoleView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<ConsoleView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FConsoleView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/ConsoleView.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		((ISupportInitialize)P_1).BeginInit();
		context.PushParent(P_1);
		Styles styles = P_1.Styles;
		Style style = new Style();
		style.Selector = ((Selector?)null).OfType(typeof(TextBox)).Class("console");
		Setter setter = new Setter();
		setter.Property = TemplatedControl.FontFamilyProperty;
		setter.Value = new FontFamily(((IUriContext)context).BaseUri, "/Assets/Fonts/SourceSansPro-Regular.ttf");
		style.Add(setter);
		Setter setter2 = new Setter();
		setter2.Property = TemplatedControl.TemplateProperty;
		setter2.Value = new ControlTemplate
		{
			Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_9.Build, context)
		};
		style.Add(setter2);
		styles.Add(style);
		DockPanel dockPanel;
		DockPanel dockPanel2 = (dockPanel = new DockPanel());
		((ISupportInitialize)dockPanel2).BeginInit();
		P_1.Content = dockPanel2;
		DockPanel dockPanel3;
		DockPanel dockPanel4 = (dockPanel3 = dockPanel);
		context.PushParent(dockPanel3);
		Avalonia.Controls.Controls children = dockPanel3.Children;
		TextBox textBox;
		TextBox textBox2 = (textBox = new TextBox());
		((ISupportInitialize)textBox2).BeginInit();
		children.Add(textBox2);
		TextBox textBox3;
		TextBox textBox4 = (textBox3 = textBox);
		context.PushParent(textBox3);
		textBox3.Name = "input";
		object element = textBox3;
		context.AvaloniaNameScope.Register("input", element);
		textBox3.Classes.Add("console");
		DockPanel.SetDock(textBox3, Dock.Bottom);
		textBox3.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
		StyledProperty<string?> textProperty = TextBox.TextProperty;
		CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EConsoleViewModel_002CAvalonia_002EDiagnostics_002EInput_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = TextBox.TextProperty;
		CompiledBindingExtension binding = compiledBindingExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		textBox3.Bind(textProperty, binding);
		context.PopParent();
		((ISupportInitialize)textBox4).EndInit();
		Avalonia.Controls.Controls children2 = dockPanel3.Children;
		ListBox listBox;
		ListBox listBox2 = (listBox = new ListBox());
		((ISupportInitialize)listBox2).BeginInit();
		children2.Add(listBox2);
		ListBox listBox3;
		ListBox listBox4 = (listBox3 = listBox);
		context.PushParent(listBox3);
		listBox3.Name = "historyList";
		element = listBox3;
		context.AvaloniaNameScope.Register("historyList", element);
		StyledProperty<IBrush?> borderBrushProperty = TemplatedControl.BorderBrushProperty;
		DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeControlMidBrush");
		context.ProvideTargetProperty = TemplatedControl.BorderBrushProperty;
		IBinding binding2 = dynamicResourceExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		listBox3.Bind(borderBrushProperty, binding2);
		listBox3.BorderThickness = new Thickness(0.0, 0.0, 0.0, 1.0);
		listBox3.FontFamily = new FontFamily(((IUriContext)context).BaseUri, "/Assets/Fonts/SourceSansPro-Regular.ttf");
		StyledProperty<IEnumerable?> itemsSourceProperty = ItemsControl.ItemsSourceProperty;
		CompiledBindingExtension compiledBindingExtension2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EConsoleViewModel_002CAvalonia_002EDiagnostics_002EHistory_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ItemsControl.ItemsSourceProperty;
		CompiledBindingExtension binding3 = compiledBindingExtension2.ProvideValue(context);
		context.ProvideTargetProperty = null;
		listBox3.Bind(itemsSourceProperty, binding3);
		DataTemplate dataTemplate;
		DataTemplate itemTemplate = (dataTemplate = new DataTemplate());
		context.PushParent(dataTemplate);
		dataTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_10.Build, context);
		context.PopParent();
		listBox3.ItemTemplate = itemTemplate;
		context.PopParent();
		((ISupportInitialize)listBox4).EndInit();
		context.PopParent();
		((ISupportInitialize)dockPanel4).EndInit();
		context.PopParent();
		((ISupportInitialize)P_1).EndInit();
		if (P_1 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	private static void _0021XamlIlPopulateTrampoline(ConsoleView P_0)
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
