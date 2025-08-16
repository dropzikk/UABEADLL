using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
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

internal class TreePageView : UserControl
{
	private class DynamicSetters_48
	{
		public static void DynamicSetter_1(TreeView P_0, CompiledBindingExtension P_1)
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

		public static void DynamicSetter_2(ContentControl P_0, CompiledBindingExtension P_1)
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

	private class XamlClosure_49
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<TreePageView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<TreePageView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FTreePageView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/TreePageView.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (TreePageView)service;
				}
			}
			context.IntermediateRoot = new StackPanel();
			object intermediateRoot = context.IntermediateRoot;
			((ISupportInitialize)intermediateRoot).BeginInit();
			StackPanel stackPanel = (StackPanel)intermediateRoot;
			context.PushParent(stackPanel);
			stackPanel.Orientation = Orientation.Horizontal;
			stackPanel.Spacing = 8.0;
			stackPanel.PointerEntered += context.RootObject.AddAdorner;
			stackPanel.PointerExited += context.RootObject.RemoveAdorner;
			Avalonia.Controls.Controls children = stackPanel.Children;
			TextBlock textBlock;
			TextBlock textBlock2 = (textBlock = new TextBlock());
			((ISupportInitialize)textBlock2).BeginInit();
			children.Add(textBlock2);
			TextBlock textBlock3;
			TextBlock textBlock4 = (textBlock3 = textBlock);
			context.PushParent(textBlock3);
			TextBlock target = textBlock3;
			StyledProperty<string?> textProperty = TextBlock.TextProperty;
			CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ETreeNode_002CAvalonia_002EDiagnostics_002EType_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = TextBlock.TextProperty;
			CompiledBindingExtension binding = compiledBindingExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			target.Bind(textProperty, binding);
			StyledProperty<FontWeight> fontWeightProperty = TextBlock.FontWeightProperty;
			CompiledBindingExtension compiledBindingExtension2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ETreeNode_002CAvalonia_002EDiagnostics_002EFontWeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = TextBlock.FontWeightProperty;
			CompiledBindingExtension binding2 = compiledBindingExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			target.Bind(fontWeightProperty, binding2);
			context.PopParent();
			((ISupportInitialize)textBlock4).EndInit();
			Avalonia.Controls.Controls children2 = stackPanel.Children;
			TextBlock textBlock5;
			TextBlock textBlock6 = (textBlock5 = new TextBlock());
			((ISupportInitialize)textBlock6).BeginInit();
			children2.Add(textBlock6);
			TextBlock textBlock7 = (textBlock3 = textBlock5);
			context.PushParent(textBlock3);
			TextBlock target2 = textBlock3;
			StyledProperty<string?> textProperty2 = TextBlock.TextProperty;
			CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ETreeNode_002CAvalonia_002EDiagnostics_002EClasses_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = TextBlock.TextProperty;
			CompiledBindingExtension binding3 = compiledBindingExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			target2.Bind(textProperty2, binding3);
			context.PopParent();
			((ISupportInitialize)textBlock7).EndInit();
			Avalonia.Controls.Controls children3 = stackPanel.Children;
			TextBlock textBlock8;
			TextBlock textBlock9 = (textBlock8 = new TextBlock());
			((ISupportInitialize)textBlock9).BeginInit();
			children3.Add(textBlock9);
			TextBlock textBlock10 = (textBlock3 = textBlock8);
			context.PushParent(textBlock3);
			TextBlock textBlock11 = textBlock3;
			textBlock11.Foreground = new ImmutableSolidColorBrush(4286611584u);
			StyledProperty<string?> textProperty3 = TextBlock.TextProperty;
			CompiledBindingExtension compiledBindingExtension4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ETreeNode_002CAvalonia_002EDiagnostics_002EElementName_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
			context.ProvideTargetProperty = TextBlock.TextProperty;
			CompiledBindingExtension binding4 = compiledBindingExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			textBlock11.Bind(textProperty3, binding4);
			context.PopParent();
			((ISupportInitialize)textBlock10).EndInit();
			context.PopParent();
			((ISupportInitialize)intermediateRoot).EndInit();
			return intermediateRoot;
		}
	}

	private readonly Panel _adorner;

	private AdornerLayer? _currentLayer;

	private TreeView _tree;

	private static Action<object> _0021XamlIlPopulateOverride;

	public TreePageView()
	{
		InitializeComponent();
		_tree = this.GetControl<TreeView>("tree");
		_adorner = new Panel
		{
			ClipToBounds = false,
			Children = 
			{
				(Control)new Border
				{
					BorderBrush = new SolidColorBrush(Colors.Green, 0.5)
				},
				(Control)new Border
				{
					Background = new SolidColorBrush(Color.FromRgb(160, 197, 232), 0.5)
				},
				(Control)new Border
				{
					BorderBrush = new SolidColorBrush(Colors.Yellow, 0.5)
				}
			}
		};
		AdornerLayer.SetIsClipEnabled(_adorner, isClipEnabled: false);
	}

	protected void AddAdorner(object? sender, PointerEventArgs e)
	{
		TreeNode treeNode = (TreeNode)((Control)sender).DataContext;
		TreePageViewModel treePageViewModel = (TreePageViewModel)base.DataContext;
		if (treeNode == null || treePageViewModel == null || !(treeNode.Visual is Visual visual))
		{
			return;
		}
		_currentLayer = AdornerLayer.GetAdornerLayer(visual);
		if (_currentLayer != null && !_currentLayer.Children.Contains(_adorner))
		{
			_currentLayer.Children.Add(_adorner);
			AdornerLayer.SetAdornedElement(_adorner, visual);
			if (treePageViewModel.MainView.ShouldVisualizeMarginPadding)
			{
				((Border)_adorner.Children[0]).BorderThickness = visual.GetValue(TemplatedControl.PaddingProperty);
				((Border)_adorner.Children[1]).Margin = visual.GetValue(TemplatedControl.PaddingProperty);
				Border obj = (Border)_adorner.Children[2];
				obj.BorderThickness = visual.GetValue(Layoutable.MarginProperty);
				obj.Margin = InvertThickness(visual.GetValue(Layoutable.MarginProperty));
			}
		}
	}

	private static Thickness InvertThickness(Thickness input)
	{
		return new Thickness(0.0 - input.Left, 0.0 - input.Top, 0.0 - input.Right, 0.0 - input.Bottom);
	}

	protected void RemoveAdorner(object? sender, PointerEventArgs e)
	{
		foreach (Border item in _adorner.Children.OfType<Border>())
		{
			item.Margin = default(Thickness);
			item.Padding = default(Thickness);
			item.BorderThickness = default(Thickness);
		}
		_currentLayer?.Children.Remove(_adorner);
		_currentLayer = null;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == StyledElement.DataContextProperty)
		{
			if (change.GetOldValue<object>() is TreePageViewModel treePageViewModel)
			{
				treePageViewModel.ClipboardCopyRequested -= OnClipboardCopyRequested;
			}
			if (change.GetNewValue<object>() is TreePageViewModel treePageViewModel2)
			{
				treePageViewModel2.ClipboardCopyRequested += OnClipboardCopyRequested;
			}
		}
	}

	private void InitializeComponent()
	{
		_0021XamlIlPopulateTrampoline(this);
	}

	private void OnClipboardCopyRequested(object? sender, string e)
	{
		TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(e);
	}

	static void _0021XamlIlPopulate(IServiceProvider P_0, TreePageView P_1)
	{
		CompiledAvaloniaXaml.XamlIlContext.Context<TreePageView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<TreePageView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FTreePageView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/TreePageView.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		((ISupportInitialize)P_1).BeginInit();
		context.PushParent(P_1);
		Grid grid;
		Grid grid2 = (grid = new Grid());
		((ISupportInitialize)grid2).BeginInit();
		P_1.Content = grid2;
		Grid grid3;
		Grid grid4 = (grid3 = grid);
		context.PushParent(grid3);
		ColumnDefinitions columnDefinitions = new ColumnDefinitions();
		columnDefinitions.Capacity = 3;
		columnDefinitions.Add(new ColumnDefinition(new GridLength(0.35, GridUnitType.Star)));
		columnDefinitions.Add(new ColumnDefinition(new GridLength(4.0, GridUnitType.Pixel)));
		columnDefinitions.Add(new ColumnDefinition(new GridLength(0.65, GridUnitType.Star)));
		grid3.ColumnDefinitions = columnDefinitions;
		Avalonia.Controls.Controls children = grid3.Children;
		TreeView treeView;
		TreeView treeView2 = (treeView = new TreeView());
		((ISupportInitialize)treeView2).BeginInit();
		children.Add(treeView2);
		TreeView treeView3;
		TreeView treeView4 = (treeView3 = treeView);
		context.PushParent(treeView3);
		treeView3.Name = "tree";
		object element = treeView3;
		context.AvaloniaNameScope.Register("tree", element);
		treeView3.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
		StyledProperty<IEnumerable?> itemsSourceProperty = ItemsControl.ItemsSourceProperty;
		CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002CAvalonia_002EDiagnostics_002ENodes_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ItemsControl.ItemsSourceProperty;
		CompiledBindingExtension binding = compiledBindingExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		treeView3.Bind(itemsSourceProperty, binding);
		CompiledBindingExtension obj = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002CAvalonia_002EDiagnostics_002ESelectedNode_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build())
		{
			Mode = BindingMode.TwoWay
		};
		context.ProvideTargetProperty = TreeView.SelectedItemProperty;
		CompiledBindingExtension compiledBindingExtension2 = obj.ProvideValue(context);
		context.ProvideTargetProperty = null;
		DynamicSetters_48.DynamicSetter_1(treeView3, compiledBindingExtension2);
		DataTemplates dataTemplates = treeView3.DataTemplates;
		TreeDataTemplate treeDataTemplate;
		TreeDataTemplate item = (treeDataTemplate = new TreeDataTemplate());
		context.PushParent(treeDataTemplate);
		treeDataTemplate.DataType = typeof(TreeNode);
		CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ETreeNode_002CAvalonia_002EDiagnostics_002EChildren_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EMarkup_002EXaml_002ETemplates_002ETreeDataTemplate_002CAvalonia_002EMarkup_002EXaml_002EItemsSource_0021Property();
		CompiledBindingExtension itemsSource = compiledBindingExtension3.ProvideValue(context);
		context.ProvideTargetProperty = null;
		treeDataTemplate.ItemsSource = itemsSource;
		treeDataTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_49.Build, context);
		context.PopParent();
		dataTemplates.Add(item);
		Styles styles = treeView3.Styles;
		Style style;
		Style item2 = (style = new Style());
		context.PushParent(style);
		style.Selector = ((Selector?)null).OfType(typeof(TreeViewItem));
		Setter setter;
		Setter setter2 = (setter = new Setter());
		context.PushParent(setter);
		setter.Property = TreeViewItem.IsExpandedProperty;
		CompiledBindingExtension obj2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ETreeNode_002CAvalonia_002EDiagnostics_002EIsExpanded_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build())
		{
			Mode = BindingMode.TwoWay
		};
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
		CompiledBindingExtension value = obj2.ProvideValue(context);
		context.ProvideTargetProperty = null;
		setter.Value = value;
		context.PopParent();
		style.Add(setter2);
		Setter setter3 = new Setter();
		setter3.Property = TemplatedControl.BackgroundProperty;
		setter3.Value = new ImmutableSolidColorBrush(16777215u);
		style.Add(setter3);
		context.PopParent();
		styles.Add(item2);
		MenuFlyout menuFlyout;
		MenuFlyout contextFlyout = (menuFlyout = new MenuFlyout());
		context.PushParent(menuFlyout);
		ItemCollection items = menuFlyout.Items;
		MenuItem menuItem;
		MenuItem menuItem2 = (menuItem = new MenuItem());
		((ISupportInitialize)menuItem2).BeginInit();
		items.Add(menuItem2);
		MenuItem menuItem3;
		MenuItem menuItem4 = (menuItem3 = menuItem);
		context.PushParent(menuItem3);
		MenuItem menuItem5 = menuItem3;
		menuItem5.Header = "Copy";
		ItemCollection items2 = menuItem5.Items;
		MenuItem menuItem6;
		MenuItem menuItem7 = (menuItem6 = new MenuItem());
		((ISupportInitialize)menuItem7).BeginInit();
		items2.Add(menuItem7);
		MenuItem menuItem8 = (menuItem3 = menuItem6);
		context.PushParent(menuItem3);
		MenuItem menuItem9 = menuItem3;
		menuItem9.Header = "Copy individual node selector";
		StyledProperty<ICommand?> commandProperty = MenuItem.CommandProperty;
		CompiledBindingExtension compiledBindingExtension4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("CopySelector", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002BCopySelector_0_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = MenuItem.CommandProperty;
		CompiledBindingExtension binding2 = compiledBindingExtension4.ProvideValue(context);
		context.ProvideTargetProperty = null;
		menuItem9.Bind(commandProperty, binding2);
		context.PopParent();
		((ISupportInitialize)menuItem8).EndInit();
		ItemCollection items3 = menuItem5.Items;
		MenuItem menuItem10;
		MenuItem menuItem11 = (menuItem10 = new MenuItem());
		((ISupportInitialize)menuItem11).BeginInit();
		items3.Add(menuItem11);
		MenuItem menuItem12 = (menuItem3 = menuItem10);
		context.PushParent(menuItem3);
		MenuItem menuItem13 = menuItem3;
		menuItem13.Header = "Copy selector from template parent";
		StyledProperty<ICommand?> commandProperty2 = MenuItem.CommandProperty;
		CompiledBindingExtension compiledBindingExtension5 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("CopySelectorFromTemplateParent", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002BCopySelectorFromTemplateParent_0_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = MenuItem.CommandProperty;
		CompiledBindingExtension binding3 = compiledBindingExtension5.ProvideValue(context);
		context.ProvideTargetProperty = null;
		menuItem13.Bind(commandProperty2, binding3);
		context.PopParent();
		((ISupportInitialize)menuItem12).EndInit();
		context.PopParent();
		((ISupportInitialize)menuItem4).EndInit();
		ItemCollection items4 = menuFlyout.Items;
		MenuItem menuItem14;
		MenuItem menuItem15 = (menuItem14 = new MenuItem());
		((ISupportInitialize)menuItem15).BeginInit();
		items4.Add(menuItem15);
		menuItem14.Header = "-";
		((ISupportInitialize)menuItem14).EndInit();
		ItemCollection items5 = menuFlyout.Items;
		MenuItem menuItem16;
		MenuItem menuItem17 = (menuItem16 = new MenuItem());
		((ISupportInitialize)menuItem17).BeginInit();
		items5.Add(menuItem17);
		MenuItem menuItem18 = (menuItem3 = menuItem16);
		context.PushParent(menuItem3);
		MenuItem menuItem19 = menuItem3;
		menuItem19.Header = "Expand recursively";
		StyledProperty<ICommand?> commandProperty3 = MenuItem.CommandProperty;
		CompiledBindingExtension compiledBindingExtension6 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("ExpandRecursively", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002BExpandRecursively_0_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = MenuItem.CommandProperty;
		CompiledBindingExtension binding4 = compiledBindingExtension6.ProvideValue(context);
		context.ProvideTargetProperty = null;
		menuItem19.Bind(commandProperty3, binding4);
		context.PopParent();
		((ISupportInitialize)menuItem18).EndInit();
		ItemCollection items6 = menuFlyout.Items;
		MenuItem menuItem20;
		MenuItem menuItem21 = (menuItem20 = new MenuItem());
		((ISupportInitialize)menuItem21).BeginInit();
		items6.Add(menuItem21);
		MenuItem menuItem22 = (menuItem3 = menuItem20);
		context.PushParent(menuItem3);
		MenuItem menuItem23 = menuItem3;
		menuItem23.Header = "Collapse children";
		StyledProperty<ICommand?> commandProperty4 = MenuItem.CommandProperty;
		CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("CollapseChildren", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002BCollapseChildren_0_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = MenuItem.CommandProperty;
		CompiledBindingExtension binding5 = compiledBindingExtension7.ProvideValue(context);
		context.ProvideTargetProperty = null;
		menuItem23.Bind(commandProperty4, binding5);
		context.PopParent();
		((ISupportInitialize)menuItem22).EndInit();
		ItemCollection items7 = menuFlyout.Items;
		MenuItem menuItem24;
		MenuItem menuItem25 = (menuItem24 = new MenuItem());
		((ISupportInitialize)menuItem25).BeginInit();
		items7.Add(menuItem25);
		MenuItem menuItem26 = (menuItem3 = menuItem24);
		context.PushParent(menuItem3);
		MenuItem menuItem27 = menuItem3;
		menuItem27.Header = "Capture node screenshot";
		StyledProperty<ICommand?> commandProperty5 = MenuItem.CommandProperty;
		CompiledBindingExtension compiledBindingExtension8 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("CaptureNodeScreenshot", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002BCaptureNodeScreenshot_0_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = MenuItem.CommandProperty;
		CompiledBindingExtension binding6 = compiledBindingExtension8.ProvideValue(context);
		context.ProvideTargetProperty = null;
		menuItem27.Bind(commandProperty5, binding6);
		context.PopParent();
		((ISupportInitialize)menuItem26).EndInit();
		ItemCollection items8 = menuFlyout.Items;
		MenuItem menuItem28;
		MenuItem menuItem29 = (menuItem28 = new MenuItem());
		((ISupportInitialize)menuItem29).BeginInit();
		items8.Add(menuItem29);
		MenuItem menuItem30 = (menuItem3 = menuItem28);
		context.PushParent(menuItem3);
		MenuItem menuItem31 = menuItem3;
		menuItem31.Header = "Bring into view";
		StyledProperty<ICommand?> commandProperty6 = MenuItem.CommandProperty;
		CompiledBindingExtension compiledBindingExtension9 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("BringIntoView", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002BBringIntoView_0_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = MenuItem.CommandProperty;
		CompiledBindingExtension binding7 = compiledBindingExtension9.ProvideValue(context);
		context.ProvideTargetProperty = null;
		menuItem31.Bind(commandProperty6, binding7);
		context.PopParent();
		((ISupportInitialize)menuItem30).EndInit();
		ItemCollection items9 = menuFlyout.Items;
		MenuItem menuItem32;
		MenuItem menuItem33 = (menuItem32 = new MenuItem());
		((ISupportInitialize)menuItem33).BeginInit();
		items9.Add(menuItem33);
		MenuItem menuItem34 = (menuItem3 = menuItem32);
		context.PushParent(menuItem3);
		MenuItem menuItem35 = menuItem3;
		menuItem35.Header = "Focus";
		StyledProperty<ICommand?> commandProperty7 = MenuItem.CommandProperty;
		CompiledBindingExtension compiledBindingExtension10 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("Focus", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002BFocus_0_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = MenuItem.CommandProperty;
		CompiledBindingExtension binding8 = compiledBindingExtension10.ProvideValue(context);
		context.ProvideTargetProperty = null;
		menuItem35.Bind(commandProperty7, binding8);
		context.PopParent();
		((ISupportInitialize)menuItem34).EndInit();
		context.PopParent();
		treeView3.ContextFlyout = contextFlyout;
		context.PopParent();
		((ISupportInitialize)treeView4).EndInit();
		Avalonia.Controls.Controls children2 = grid3.Children;
		GridSplitter gridSplitter;
		GridSplitter gridSplitter2 = (gridSplitter = new GridSplitter());
		((ISupportInitialize)gridSplitter2).BeginInit();
		children2.Add(gridSplitter2);
		GridSplitter gridSplitter3;
		GridSplitter gridSplitter4 = (gridSplitter3 = gridSplitter);
		context.PushParent(gridSplitter3);
		StyledProperty<IBrush?> backgroundProperty = TemplatedControl.BackgroundProperty;
		DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeControlMidBrush");
		context.ProvideTargetProperty = TemplatedControl.BackgroundProperty;
		IBinding binding9 = dynamicResourceExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		gridSplitter3.Bind(backgroundProperty, binding9);
		gridSplitter3.Width = 1.0;
		Grid.SetColumn(gridSplitter3, 1);
		context.PopParent();
		((ISupportInitialize)gridSplitter4).EndInit();
		Avalonia.Controls.Controls children3 = grid3.Children;
		ContentControl contentControl;
		ContentControl contentControl2 = (contentControl = new ContentControl());
		((ISupportInitialize)contentControl2).BeginInit();
		children3.Add(contentControl2);
		ContentControl contentControl3;
		ContentControl contentControl4 = (contentControl3 = contentControl);
		context.PushParent(contentControl3);
		CompiledBindingExtension compiledBindingExtension11 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002CAvalonia_002EDiagnostics_002EDetails_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ContentControl.ContentProperty;
		CompiledBindingExtension compiledBindingExtension12 = compiledBindingExtension11.ProvideValue(context);
		context.ProvideTargetProperty = null;
		DynamicSetters_48.DynamicSetter_2(contentControl3, compiledBindingExtension12);
		Grid.SetColumn(contentControl3, 2);
		context.PopParent();
		((ISupportInitialize)contentControl4).EndInit();
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

	private static void _0021XamlIlPopulateTrampoline(TreePageView P_0)
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
