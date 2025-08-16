using System;
using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Diagnostics.ViewModels;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Threading;
using CompiledAvaloniaXaml;

namespace Avalonia.Diagnostics.Views;

internal class MainView : UserControl
{
	private class DynamicSetters_47
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

		public static void DynamicSetter_2(StyledElement P_0, CompiledBindingExtension P_1)
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
	}

	private readonly ConsoleView _console;

	private readonly GridSplitter _consoleSplitter;

	private readonly Grid _rootGrid;

	private readonly int _consoleRow;

	private double _consoleHeight = -1.0;

	private static Action<object> _0021XamlIlPopulateOverride;

	public MainView()
	{
		InitializeComponent();
		AddHandler(InputElement.KeyUpEvent, PreviewKeyUp);
		_console = this.GetControl<ConsoleView>("console");
		_consoleSplitter = this.GetControl<GridSplitter>("consoleSplitter");
		_rootGrid = this.GetControl<Grid>("rootGrid");
		_consoleRow = Grid.GetRow(_console);
	}

	public void ToggleConsole()
	{
		MainViewModel mainViewModel = (MainViewModel)base.DataContext;
		if (mainViewModel == null)
		{
			return;
		}
		if (_consoleHeight == -1.0)
		{
			_consoleHeight = base.Bounds.Height / 3.0;
		}
		mainViewModel.Console.ToggleVisibility();
		_consoleSplitter.IsVisible = mainViewModel.Console.IsVisible;
		if (mainViewModel.Console.IsVisible)
		{
			_rootGrid.RowDefinitions[_consoleRow].Height = new GridLength(_consoleHeight, GridUnitType.Pixel);
			Dispatcher.UIThread.Post(delegate
			{
				_console.FocusInput();
			}, DispatcherPriority.Background);
		}
		else
		{
			_consoleHeight = _rootGrid.RowDefinitions[_consoleRow].Height.Value;
			_rootGrid.RowDefinitions[_consoleRow].Height = new GridLength(0.0, GridUnitType.Pixel);
		}
	}

	private void InitializeComponent()
	{
		_0021XamlIlPopulateTrampoline(this);
	}

	private void PreviewKeyUp(object? sender, KeyEventArgs e)
	{
		if (e.Key == Key.Escape)
		{
			ToggleConsole();
			e.Handled = true;
		}
	}

	static void _0021XamlIlPopulate(IServiceProvider P_0, MainView P_1)
	{
		CompiledAvaloniaXaml.XamlIlContext.Context<MainView> context = new CompiledAvaloniaXaml.XamlIlContext.Context<MainView>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FMainView_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/MainView.xaml");
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
		Grid grid5 = grid3;
		grid5.Name = "rootGrid";
		object element = grid5;
		context.AvaloniaNameScope.Register("rootGrid", element);
		RowDefinitions rowDefinitions = new RowDefinitions();
		rowDefinitions.Capacity = 6;
		rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		rowDefinitions.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
		rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Pixel)));
		rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		grid5.RowDefinitions = rowDefinitions;
		Avalonia.Controls.Controls children = grid5.Children;
		Menu menu;
		Menu menu2 = (menu = new Menu());
		((ISupportInitialize)menu2).BeginInit();
		children.Add(menu2);
		Menu menu3;
		Menu menu4 = (menu3 = menu);
		context.PushParent(menu3);
		ItemCollection items = menu3.Items;
		MenuItem menuItem;
		MenuItem menuItem2 = (menuItem = new MenuItem());
		((ISupportInitialize)menuItem2).BeginInit();
		items.Add(menuItem2);
		MenuItem menuItem3;
		MenuItem menuItem4 = (menuItem3 = menuItem);
		context.PushParent(menuItem3);
		MenuItem menuItem5 = menuItem3;
		menuItem5.Header = "_File";
		ItemCollection items2 = menuItem5.Items;
		MenuItem menuItem6;
		MenuItem menuItem7 = (menuItem6 = new MenuItem());
		((ISupportInitialize)menuItem7).BeginInit();
		items2.Add(menuItem7);
		MenuItem menuItem8 = (menuItem3 = menuItem6);
		context.PushParent(menuItem3);
		MenuItem menuItem9 = menuItem3;
		menuItem9.Header = "E_xit";
		StyledProperty<ICommand?> commandProperty = MenuItem.CommandProperty;
		CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(Window), 0).Command("Close", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EControls_003AAvalonia_002EControls_002EWindow_002BClose_0_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = MenuItem.CommandProperty;
		CompiledBindingExtension binding = compiledBindingExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		menuItem9.Bind(commandProperty, binding);
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
		menuItem13.Header = "Screenshot";
		StyledProperty<ICommand?> commandProperty2 = MenuItem.CommandProperty;
		CompiledBindingExtension compiledBindingExtension2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("Shot", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BShot_1_0021CommandExecuteTrampoline, CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BCanShot_0021CommandCanExecuteTrampoline, new string[2] { "SelectedNode", "Content" }).Build());
		context.ProvideTargetProperty = MenuItem.CommandProperty;
		CompiledBindingExtension binding2 = compiledBindingExtension2.ProvideValue(context);
		context.ProvideTargetProperty = null;
		menuItem13.Bind(commandProperty2, binding2);
		menuItem13.HotKey = KeyGesture.Parse("F8");
		Image image;
		Image image2 = (image = new Image());
		((ISupportInitialize)image2).BeginInit();
		menuItem13.Icon = image2;
		DrawingImage drawingImage = new DrawingImage();
		DrawingGroup drawingGroup = new DrawingGroup();
		DrawingCollection children2 = drawingGroup.Children;
		GeometryDrawing geometryDrawing = new GeometryDrawing();
		geometryDrawing.Geometry = Geometry.Parse("F1 M 13.4533,17.56C 13.4533,19.8827 15.344,21.772 17.6653,21.772L 17.6653,21.772C 19.988,21.772 21.8773,19.8827 21.8773,17.56L 21.8773,17.56C 21.8773,15.2373 19.988,13.348 17.6653,13.348L 17.6653,13.348C 15.344,13.348 13.4533,15.2373 13.4533,17.56 Z ");
		RadialGradientBrush radialGradientBrush = new RadialGradientBrush();
		radialGradientBrush.Center = new RelativePoint(0.245696, 0.288009, RelativeUnit.Absolute);
		radialGradientBrush.GradientOrigin = new RelativePoint(0.245696, 0.288009, RelativeUnit.Absolute);
		radialGradientBrush.Radius = 0.499952;
		GradientStops gradientStops = radialGradientBrush.GradientStops;
		GradientStop gradientStop = new GradientStop();
		gradientStop.Color = Color.FromUInt32(4287072908u);
		gradientStop.Offset = 0.0;
		gradientStops.Add(gradientStop);
		GradientStops gradientStops2 = radialGradientBrush.GradientStops;
		GradientStop gradientStop2 = new GradientStop();
		gradientStop2.Color = Color.FromUInt32(4283714124u);
		gradientStop2.Offset = 0.991379;
		gradientStops2.Add(gradientStop2);
		geometryDrawing.Brush = radialGradientBrush;
		children2.Add(geometryDrawing);
		DrawingCollection children3 = drawingGroup.Children;
		GeometryDrawing geometryDrawing2 = new GeometryDrawing();
		geometryDrawing2.Geometry = Geometry.Parse("F1 M 13.332,6.22803L 10.2227,9.72668L 8.49866,9.72668L 8.49866,7.56136L 5.33333,7.56136L 5.33333,9.72668L 3.33333,9.72668L 3.33333,24.3947L 13.1213,24.3947C 14.424,25.264 15.9853,25.772 17.6653,25.772L 17.6653,25.772C 19.344,25.772 20.9067,25.264 22.2094,24.3947L 28.6667,24.3947L 28.6667,9.72668L 24.944,9.72668L 21.8333,6.22803M 12.12,17.56C 12.12,14.5013 14.608,12.0147 17.6653,12.0147L 17.6653,12.0147C 20.7227,12.0147 23.2107,14.5013 23.2107,17.56L 23.2107,17.56C 23.2107,20.6174 20.7227,23.104 17.6653,23.104L 17.6653,23.104C 14.608,23.104 12.12,20.6174 12.12,17.56 Z ");
		RadialGradientBrush radialGradientBrush2 = new RadialGradientBrush();
		radialGradientBrush2.Center = new RelativePoint(0.196943, 0.216757, RelativeUnit.Absolute);
		radialGradientBrush2.GradientOrigin = new RelativePoint(0.196943, 0.216757, RelativeUnit.Absolute);
		radialGradientBrush2.Radius = 0.44654;
		GradientStops gradientStops3 = radialGradientBrush2.GradientStops;
		GradientStop gradientStop3 = new GradientStop();
		gradientStop3.Color = Color.FromUInt32(4287072652u);
		gradientStop3.Offset = 0.0;
		gradientStops3.Add(gradientStop3);
		GradientStops gradientStops4 = radialGradientBrush2.GradientStops;
		GradientStop gradientStop4 = new GradientStop();
		gradientStop4.Color = Color.FromUInt32(4283714124u);
		gradientStop4.Offset = 1.0;
		gradientStops4.Add(gradientStop4);
		geometryDrawing2.Brush = radialGradientBrush2;
		children3.Add(geometryDrawing2);
		drawingImage.Drawing = drawingGroup;
		image.Source = drawingImage;
		((ISupportInitialize)image).EndInit();
		context.PopParent();
		((ISupportInitialize)menuItem12).EndInit();
		context.PopParent();
		((ISupportInitialize)menuItem4).EndInit();
		ItemCollection items4 = menu3.Items;
		MenuItem menuItem14;
		MenuItem menuItem15 = (menuItem14 = new MenuItem());
		((ISupportInitialize)menuItem15).BeginInit();
		items4.Add(menuItem15);
		MenuItem menuItem16 = (menuItem3 = menuItem14);
		context.PushParent(menuItem3);
		MenuItem menuItem17 = menuItem3;
		menuItem17.Header = "_View";
		ItemCollection items5 = menuItem17.Items;
		MenuItem menuItem18;
		MenuItem menuItem19 = (menuItem18 = new MenuItem());
		((ISupportInitialize)menuItem19).BeginInit();
		items5.Add(menuItem19);
		MenuItem menuItem20 = (menuItem3 = menuItem18);
		context.PushParent(menuItem3);
		MenuItem menuItem21 = menuItem3;
		menuItem21.Header = "_Console";
		StyledProperty<ICommand?> commandProperty3 = MenuItem.CommandProperty;
		CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(MainView), 0).Command("ToggleConsole", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViews_002EMainView_002BToggleConsole_0_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = MenuItem.CommandProperty;
		CompiledBindingExtension binding3 = compiledBindingExtension3.ProvideValue(context);
		context.ProvideTargetProperty = null;
		menuItem21.Bind(commandProperty3, binding3);
		CheckBox checkBox;
		CheckBox checkBox2 = (checkBox = new CheckBox());
		((ISupportInitialize)checkBox2).BeginInit();
		menuItem21.Icon = checkBox2;
		CheckBox checkBox3;
		CheckBox checkBox4 = (checkBox3 = checkBox);
		context.PushParent(checkBox3);
		CheckBox checkBox5 = checkBox3;
		checkBox5.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
		StyledProperty<bool?> isCheckedProperty = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension compiledBindingExtension4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002CAvalonia_002EDiagnostics_002EConsole_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EConsoleViewModel_002CAvalonia_002EDiagnostics_002EIsVisible_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension binding4 = compiledBindingExtension4.ProvideValue(context);
		context.ProvideTargetProperty = null;
		checkBox5.Bind(isCheckedProperty, binding4);
		checkBox5.IsEnabled = false;
		context.PopParent();
		((ISupportInitialize)checkBox4).EndInit();
		context.PopParent();
		((ISupportInitialize)menuItem20).EndInit();
		ItemCollection items6 = menuItem17.Items;
		MenuItem menuItem22;
		MenuItem menuItem23 = (menuItem22 = new MenuItem());
		((ISupportInitialize)menuItem23).BeginInit();
		items6.Add(menuItem23);
		MenuItem menuItem24 = (menuItem3 = menuItem22);
		context.PushParent(menuItem3);
		MenuItem menuItem25 = menuItem3;
		menuItem25.Header = "Control _Details";
		ItemCollection items7 = menuItem25.Items;
		MenuItem menuItem26;
		MenuItem menuItem27 = (menuItem26 = new MenuItem());
		((ISupportInitialize)menuItem27).BeginInit();
		items7.Add(menuItem27);
		MenuItem menuItem28 = (menuItem3 = menuItem26);
		context.PushParent(menuItem3);
		MenuItem menuItem29 = menuItem3;
		menuItem29.Header = "Show Implemented Interfaces";
		StyledProperty<ICommand?> commandProperty4 = MenuItem.CommandProperty;
		CompiledBindingExtension compiledBindingExtension5 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("ToggleShowImplementedInterfaces", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BToggleShowImplementedInterfaces_1_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = MenuItem.CommandProperty;
		CompiledBindingExtension binding5 = compiledBindingExtension5.ProvideValue(context);
		context.ProvideTargetProperty = null;
		menuItem29.Bind(commandProperty4, binding5);
		CheckBox checkBox6;
		CheckBox checkBox7 = (checkBox6 = new CheckBox());
		((ISupportInitialize)checkBox7).BeginInit();
		menuItem29.Icon = checkBox7;
		CheckBox checkBox8 = (checkBox3 = checkBox6);
		context.PushParent(checkBox3);
		CheckBox checkBox9 = checkBox3;
		checkBox9.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
		StyledProperty<bool?> isCheckedProperty2 = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension compiledBindingExtension6 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002CAvalonia_002EDiagnostics_002EShowImplementedInterfaces_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension binding6 = compiledBindingExtension6.ProvideValue(context);
		context.ProvideTargetProperty = null;
		checkBox9.Bind(isCheckedProperty2, binding6);
		checkBox9.IsEnabled = false;
		context.PopParent();
		((ISupportInitialize)checkBox8).EndInit();
		context.PopParent();
		((ISupportInitialize)menuItem28).EndInit();
		ItemCollection items8 = menuItem25.Items;
		MenuItem menuItem30;
		MenuItem menuItem31 = (menuItem30 = new MenuItem());
		((ISupportInitialize)menuItem31).BeginInit();
		items8.Add(menuItem31);
		MenuItem menuItem32 = (menuItem3 = menuItem30);
		context.PushParent(menuItem3);
		MenuItem menuItem33 = menuItem3;
		menuItem33.Header = "Split Property Type";
		StyledProperty<ICommand?> commandProperty5 = MenuItem.CommandProperty;
		CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("ToggleShowDetailsPropertyType", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BToggleShowDetailsPropertyType_1_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = MenuItem.CommandProperty;
		CompiledBindingExtension binding7 = compiledBindingExtension7.ProvideValue(context);
		context.ProvideTargetProperty = null;
		menuItem33.Bind(commandProperty5, binding7);
		CheckBox checkBox10;
		CheckBox checkBox11 = (checkBox10 = new CheckBox());
		((ISupportInitialize)checkBox11).BeginInit();
		menuItem33.Icon = checkBox11;
		CheckBox checkBox12 = (checkBox3 = checkBox10);
		context.PushParent(checkBox3);
		CheckBox checkBox13 = checkBox3;
		checkBox13.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
		StyledProperty<bool?> isCheckedProperty3 = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension compiledBindingExtension8 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002CAvalonia_002EDiagnostics_002EShowDetailsPropertyType_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension binding8 = compiledBindingExtension8.ProvideValue(context);
		context.ProvideTargetProperty = null;
		checkBox13.Bind(isCheckedProperty3, binding8);
		checkBox13.IsEnabled = false;
		context.PopParent();
		((ISupportInitialize)checkBox12).EndInit();
		context.PopParent();
		((ISupportInitialize)menuItem32).EndInit();
		context.PopParent();
		((ISupportInitialize)menuItem24).EndInit();
		context.PopParent();
		((ISupportInitialize)menuItem16).EndInit();
		ItemCollection items9 = menu3.Items;
		MenuItem menuItem34;
		MenuItem menuItem35 = (menuItem34 = new MenuItem());
		((ISupportInitialize)menuItem35).BeginInit();
		items9.Add(menuItem35);
		MenuItem menuItem36 = (menuItem3 = menuItem34);
		context.PushParent(menuItem3);
		MenuItem menuItem37 = menuItem3;
		menuItem37.Header = "_Overlays";
		ItemCollection items10 = menuItem37.Items;
		MenuItem menuItem38;
		MenuItem menuItem39 = (menuItem38 = new MenuItem());
		((ISupportInitialize)menuItem39).BeginInit();
		items10.Add(menuItem39);
		MenuItem menuItem40 = (menuItem3 = menuItem38);
		context.PushParent(menuItem3);
		MenuItem menuItem41 = menuItem3;
		menuItem41.Header = "Margin/padding";
		StyledProperty<ICommand?> commandProperty6 = MenuItem.CommandProperty;
		CompiledBindingExtension compiledBindingExtension9 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("ToggleVisualizeMarginPadding", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BToggleVisualizeMarginPadding_0_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = MenuItem.CommandProperty;
		CompiledBindingExtension binding9 = compiledBindingExtension9.ProvideValue(context);
		context.ProvideTargetProperty = null;
		menuItem41.Bind(commandProperty6, binding9);
		CheckBox checkBox14;
		CheckBox checkBox15 = (checkBox14 = new CheckBox());
		((ISupportInitialize)checkBox15).BeginInit();
		menuItem41.Icon = checkBox15;
		CheckBox checkBox16 = (checkBox3 = checkBox14);
		context.PushParent(checkBox3);
		CheckBox checkBox17 = checkBox3;
		checkBox17.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
		StyledProperty<bool?> isCheckedProperty4 = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension compiledBindingExtension10 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002CAvalonia_002EDiagnostics_002EShouldVisualizeMarginPadding_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension binding10 = compiledBindingExtension10.ProvideValue(context);
		context.ProvideTargetProperty = null;
		checkBox17.Bind(isCheckedProperty4, binding10);
		checkBox17.IsEnabled = false;
		context.PopParent();
		((ISupportInitialize)checkBox16).EndInit();
		context.PopParent();
		((ISupportInitialize)menuItem40).EndInit();
		ItemCollection items11 = menuItem37.Items;
		MenuItem menuItem42;
		MenuItem menuItem43 = (menuItem42 = new MenuItem());
		((ISupportInitialize)menuItem43).BeginInit();
		items11.Add(menuItem43);
		MenuItem menuItem44 = (menuItem3 = menuItem42);
		context.PushParent(menuItem3);
		MenuItem menuItem45 = menuItem3;
		menuItem45.Header = "Dirty rects";
		StyledProperty<ICommand?> commandProperty7 = MenuItem.CommandProperty;
		CompiledBindingExtension compiledBindingExtension11 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("ToggleDirtyRectsOverlay", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BToggleDirtyRectsOverlay_0_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = MenuItem.CommandProperty;
		CompiledBindingExtension binding11 = compiledBindingExtension11.ProvideValue(context);
		context.ProvideTargetProperty = null;
		menuItem45.Bind(commandProperty7, binding11);
		CheckBox checkBox18;
		CheckBox checkBox19 = (checkBox18 = new CheckBox());
		((ISupportInitialize)checkBox19).BeginInit();
		menuItem45.Icon = checkBox19;
		CheckBox checkBox20 = (checkBox3 = checkBox18);
		context.PushParent(checkBox3);
		CheckBox checkBox21 = checkBox3;
		checkBox21.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
		StyledProperty<bool?> isCheckedProperty5 = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension compiledBindingExtension12 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002CAvalonia_002EDiagnostics_002EShowDirtyRectsOverlay_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension binding12 = compiledBindingExtension12.ProvideValue(context);
		context.ProvideTargetProperty = null;
		checkBox21.Bind(isCheckedProperty5, binding12);
		checkBox21.IsEnabled = false;
		context.PopParent();
		((ISupportInitialize)checkBox20).EndInit();
		context.PopParent();
		((ISupportInitialize)menuItem44).EndInit();
		ItemCollection items12 = menuItem37.Items;
		MenuItem menuItem46;
		MenuItem menuItem47 = (menuItem46 = new MenuItem());
		((ISupportInitialize)menuItem47).BeginInit();
		items12.Add(menuItem47);
		MenuItem menuItem48 = (menuItem3 = menuItem46);
		context.PushParent(menuItem3);
		MenuItem menuItem49 = menuItem3;
		menuItem49.Header = "FPS";
		StyledProperty<ICommand?> commandProperty8 = MenuItem.CommandProperty;
		CompiledBindingExtension compiledBindingExtension13 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("ToggleFpsOverlay", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BToggleFpsOverlay_0_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = MenuItem.CommandProperty;
		CompiledBindingExtension binding13 = compiledBindingExtension13.ProvideValue(context);
		context.ProvideTargetProperty = null;
		menuItem49.Bind(commandProperty8, binding13);
		CheckBox checkBox22;
		CheckBox checkBox23 = (checkBox22 = new CheckBox());
		((ISupportInitialize)checkBox23).BeginInit();
		menuItem49.Icon = checkBox23;
		CheckBox checkBox24 = (checkBox3 = checkBox22);
		context.PushParent(checkBox3);
		CheckBox checkBox25 = checkBox3;
		checkBox25.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
		StyledProperty<bool?> isCheckedProperty6 = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension compiledBindingExtension14 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002CAvalonia_002EDiagnostics_002EShowFpsOverlay_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension binding14 = compiledBindingExtension14.ProvideValue(context);
		context.ProvideTargetProperty = null;
		checkBox25.Bind(isCheckedProperty6, binding14);
		checkBox25.IsEnabled = false;
		context.PopParent();
		((ISupportInitialize)checkBox24).EndInit();
		context.PopParent();
		((ISupportInitialize)menuItem48).EndInit();
		ItemCollection items13 = menuItem37.Items;
		MenuItem menuItem50;
		MenuItem menuItem51 = (menuItem50 = new MenuItem());
		((ISupportInitialize)menuItem51).BeginInit();
		items13.Add(menuItem51);
		MenuItem menuItem52 = (menuItem3 = menuItem50);
		context.PushParent(menuItem3);
		MenuItem menuItem53 = menuItem3;
		menuItem53.Header = "Layout time graph";
		StyledProperty<ICommand?> commandProperty9 = MenuItem.CommandProperty;
		CompiledBindingExtension compiledBindingExtension15 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("ToggleLayoutTimeGraphOverlay", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BToggleLayoutTimeGraphOverlay_0_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = MenuItem.CommandProperty;
		CompiledBindingExtension binding15 = compiledBindingExtension15.ProvideValue(context);
		context.ProvideTargetProperty = null;
		menuItem53.Bind(commandProperty9, binding15);
		CheckBox checkBox26;
		CheckBox checkBox27 = (checkBox26 = new CheckBox());
		((ISupportInitialize)checkBox27).BeginInit();
		menuItem53.Icon = checkBox27;
		CheckBox checkBox28 = (checkBox3 = checkBox26);
		context.PushParent(checkBox3);
		CheckBox checkBox29 = checkBox3;
		checkBox29.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
		StyledProperty<bool?> isCheckedProperty7 = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension compiledBindingExtension16 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002CAvalonia_002EDiagnostics_002EShowLayoutTimeGraphOverlay_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension binding16 = compiledBindingExtension16.ProvideValue(context);
		context.ProvideTargetProperty = null;
		checkBox29.Bind(isCheckedProperty7, binding16);
		checkBox29.IsEnabled = false;
		context.PopParent();
		((ISupportInitialize)checkBox28).EndInit();
		context.PopParent();
		((ISupportInitialize)menuItem52).EndInit();
		ItemCollection items14 = menuItem37.Items;
		MenuItem menuItem54;
		MenuItem menuItem55 = (menuItem54 = new MenuItem());
		((ISupportInitialize)menuItem55).BeginInit();
		items14.Add(menuItem55);
		MenuItem menuItem56 = (menuItem3 = menuItem54);
		context.PushParent(menuItem3);
		MenuItem menuItem57 = menuItem3;
		menuItem57.Header = "Render time graph";
		StyledProperty<ICommand?> commandProperty10 = MenuItem.CommandProperty;
		CompiledBindingExtension compiledBindingExtension17 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("ToggleRenderTimeGraphOverlay", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BToggleRenderTimeGraphOverlay_0_0021CommandExecuteTrampoline, null, null).Build());
		context.ProvideTargetProperty = MenuItem.CommandProperty;
		CompiledBindingExtension binding17 = compiledBindingExtension17.ProvideValue(context);
		context.ProvideTargetProperty = null;
		menuItem57.Bind(commandProperty10, binding17);
		CheckBox checkBox30;
		CheckBox checkBox31 = (checkBox30 = new CheckBox());
		((ISupportInitialize)checkBox31).BeginInit();
		menuItem57.Icon = checkBox31;
		CheckBox checkBox32 = (checkBox3 = checkBox30);
		context.PushParent(checkBox3);
		CheckBox checkBox33 = checkBox3;
		checkBox33.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
		StyledProperty<bool?> isCheckedProperty8 = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension compiledBindingExtension18 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002CAvalonia_002EDiagnostics_002EShowRenderTimeGraphOverlay_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
		CompiledBindingExtension binding18 = compiledBindingExtension18.ProvideValue(context);
		context.ProvideTargetProperty = null;
		checkBox33.Bind(isCheckedProperty8, binding18);
		checkBox33.IsEnabled = false;
		context.PopParent();
		((ISupportInitialize)checkBox32).EndInit();
		context.PopParent();
		((ISupportInitialize)menuItem56).EndInit();
		context.PopParent();
		((ISupportInitialize)menuItem36).EndInit();
		context.PopParent();
		((ISupportInitialize)menu4).EndInit();
		Avalonia.Controls.Controls children4 = grid5.Children;
		TabStrip tabStrip;
		TabStrip tabStrip2 = (tabStrip = new TabStrip());
		((ISupportInitialize)tabStrip2).BeginInit();
		children4.Add(tabStrip2);
		TabStrip tabStrip3;
		TabStrip tabStrip4 = (tabStrip3 = tabStrip);
		context.PushParent(tabStrip3);
		Grid.SetRow(tabStrip3, 1);
		DirectProperty<SelectingItemsControl, int> selectedIndexProperty = SelectingItemsControl.SelectedIndexProperty;
		CompiledBindingExtension obj = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002CAvalonia_002EDiagnostics_002ESelectedTab_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build())
		{
			Mode = BindingMode.TwoWay
		};
		context.ProvideTargetProperty = SelectingItemsControl.SelectedIndexProperty;
		CompiledBindingExtension binding19 = obj.ProvideValue(context);
		context.ProvideTargetProperty = null;
		tabStrip3.Bind(selectedIndexProperty, binding19);
		ItemCollection items15 = tabStrip3.Items;
		TabStripItem tabStripItem;
		TabStripItem tabStripItem2 = (tabStripItem = new TabStripItem());
		((ISupportInitialize)tabStripItem2).BeginInit();
		items15.Add(tabStripItem2);
		tabStripItem.Content = "Logical Tree";
		((ISupportInitialize)tabStripItem).EndInit();
		ItemCollection items16 = tabStrip3.Items;
		TabStripItem tabStripItem3;
		TabStripItem tabStripItem4 = (tabStripItem3 = new TabStripItem());
		((ISupportInitialize)tabStripItem4).BeginInit();
		items16.Add(tabStripItem4);
		tabStripItem3.Content = "Visual Tree";
		((ISupportInitialize)tabStripItem3).EndInit();
		ItemCollection items17 = tabStrip3.Items;
		TabStripItem tabStripItem5;
		TabStripItem tabStripItem6 = (tabStripItem5 = new TabStripItem());
		((ISupportInitialize)tabStripItem6).BeginInit();
		items17.Add(tabStripItem6);
		tabStripItem5.Content = "Events";
		((ISupportInitialize)tabStripItem5).EndInit();
		context.PopParent();
		((ISupportInitialize)tabStrip4).EndInit();
		Avalonia.Controls.Controls children5 = grid5.Children;
		ContentControl contentControl;
		ContentControl contentControl2 = (contentControl = new ContentControl());
		((ISupportInitialize)contentControl2).BeginInit();
		children5.Add(contentControl2);
		ContentControl contentControl3;
		ContentControl contentControl4 = (contentControl3 = contentControl);
		context.PushParent(contentControl3);
		Grid.SetRow(contentControl3, 2);
		StyledProperty<IBrush?> borderBrushProperty = TemplatedControl.BorderBrushProperty;
		DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeControlMidBrush");
		context.ProvideTargetProperty = TemplatedControl.BorderBrushProperty;
		IBinding binding20 = dynamicResourceExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		contentControl3.Bind(borderBrushProperty, binding20);
		contentControl3.BorderThickness = new Thickness(0.0, 1.0, 0.0, 0.0);
		CompiledBindingExtension compiledBindingExtension19 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002CAvalonia_002EDiagnostics_002EContent_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = ContentControl.ContentProperty;
		CompiledBindingExtension compiledBindingExtension20 = compiledBindingExtension19.ProvideValue(context);
		context.ProvideTargetProperty = null;
		DynamicSetters_47.DynamicSetter_1(contentControl3, compiledBindingExtension20);
		context.PopParent();
		((ISupportInitialize)contentControl4).EndInit();
		Avalonia.Controls.Controls children6 = grid5.Children;
		GridSplitter gridSplitter;
		GridSplitter gridSplitter2 = (gridSplitter = new GridSplitter());
		((ISupportInitialize)gridSplitter2).BeginInit();
		children6.Add(gridSplitter2);
		GridSplitter gridSplitter3;
		GridSplitter gridSplitter4 = (gridSplitter3 = gridSplitter);
		context.PushParent(gridSplitter3);
		gridSplitter3.Name = "consoleSplitter";
		element = gridSplitter3;
		context.AvaloniaNameScope.Register("consoleSplitter", element);
		Grid.SetRow(gridSplitter3, 3);
		gridSplitter3.Height = 1.0;
		StyledProperty<IBrush?> backgroundProperty = TemplatedControl.BackgroundProperty;
		DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ThemeControlMidBrush");
		context.ProvideTargetProperty = TemplatedControl.BackgroundProperty;
		IBinding binding21 = dynamicResourceExtension2.ProvideValue(context);
		context.ProvideTargetProperty = null;
		gridSplitter3.Bind(backgroundProperty, binding21);
		gridSplitter3.IsVisible = false;
		context.PopParent();
		((ISupportInitialize)gridSplitter4).EndInit();
		Avalonia.Controls.Controls children7 = grid5.Children;
		ConsoleView consoleView;
		ConsoleView consoleView2 = (consoleView = new ConsoleView());
		((ISupportInitialize)consoleView2).BeginInit();
		children7.Add(consoleView2);
		ConsoleView consoleView3;
		ConsoleView consoleView4 = (consoleView3 = consoleView);
		context.PushParent(consoleView3);
		consoleView3.Name = "console";
		element = consoleView3;
		context.AvaloniaNameScope.Register("console", element);
		Grid.SetRow(consoleView3, 4);
		CompiledBindingExtension compiledBindingExtension21 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002CAvalonia_002EDiagnostics_002EConsole_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = StyledElement.DataContextProperty;
		CompiledBindingExtension compiledBindingExtension22 = compiledBindingExtension21.ProvideValue(context);
		context.ProvideTargetProperty = null;
		DynamicSetters_47.DynamicSetter_2(consoleView3, compiledBindingExtension22);
		StyledProperty<bool> isVisibleProperty = Visual.IsVisibleProperty;
		CompiledBindingExtension compiledBindingExtension23 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EConsoleViewModel_002CAvalonia_002EDiagnostics_002EIsVisible_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = Visual.IsVisibleProperty;
		CompiledBindingExtension binding22 = compiledBindingExtension23.ProvideValue(context);
		context.ProvideTargetProperty = null;
		consoleView3.Bind(isVisibleProperty, binding22);
		context.PopParent();
		((ISupportInitialize)consoleView4).EndInit();
		Avalonia.Controls.Controls children8 = grid5.Children;
		Border border;
		Border border2 = (border = new Border());
		((ISupportInitialize)border2).BeginInit();
		children8.Add(border2);
		Border border3;
		Border border4 = (border3 = border);
		context.PushParent(border3);
		Grid.SetRow(border3, 5);
		StyledProperty<IBrush?> borderBrushProperty2 = Border.BorderBrushProperty;
		DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ThemeControlMidBrush");
		context.ProvideTargetProperty = Border.BorderBrushProperty;
		IBinding binding23 = dynamicResourceExtension3.ProvideValue(context);
		context.ProvideTargetProperty = null;
		border3.Bind(borderBrushProperty2, binding23);
		border3.BorderThickness = new Thickness(0.0, 1.0, 0.0, 0.0);
		Grid grid6;
		Grid grid7 = (grid6 = new Grid());
		((ISupportInitialize)grid7).BeginInit();
		border3.Child = grid7;
		Grid grid8 = (grid3 = grid6);
		context.PushParent(grid3);
		Grid grid9 = grid3;
		ColumnDefinitions columnDefinitions = new ColumnDefinitions();
		columnDefinitions.Capacity = 2;
		columnDefinitions.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
		columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
		grid9.ColumnDefinitions = columnDefinitions;
		Avalonia.Controls.Controls children9 = grid9.Children;
		StackPanel stackPanel;
		StackPanel stackPanel2 = (stackPanel = new StackPanel());
		((ISupportInitialize)stackPanel2).BeginInit();
		children9.Add(stackPanel2);
		StackPanel stackPanel3;
		StackPanel stackPanel4 = (stackPanel3 = stackPanel);
		context.PushParent(stackPanel3);
		Grid.SetColumn(stackPanel3, 0);
		stackPanel3.Spacing = 4.0;
		stackPanel3.Orientation = Orientation.Horizontal;
		Avalonia.Controls.Controls children10 = stackPanel3.Children;
		TextBlock textBlock;
		TextBlock textBlock2 = (textBlock = new TextBlock());
		((ISupportInitialize)textBlock2).BeginInit();
		children10.Add(textBlock2);
		textBlock.Inlines.Add("Hold Ctrl+Shift over a control to inspect.");
		((ISupportInitialize)textBlock).EndInit();
		Avalonia.Controls.Controls children11 = stackPanel3.Children;
		Separator separator;
		Separator separator2 = (separator = new Separator());
		((ISupportInitialize)separator2).BeginInit();
		children11.Add(separator2);
		separator.Width = 8.0;
		((ISupportInitialize)separator).EndInit();
		Avalonia.Controls.Controls children12 = stackPanel3.Children;
		TextBlock textBlock3;
		TextBlock textBlock4 = (textBlock3 = new TextBlock());
		((ISupportInitialize)textBlock4).BeginInit();
		children12.Add(textBlock4);
		textBlock3.Inlines.Add("Focused:");
		((ISupportInitialize)textBlock3).EndInit();
		Avalonia.Controls.Controls children13 = stackPanel3.Children;
		TextBlock textBlock5;
		TextBlock textBlock6 = (textBlock5 = new TextBlock());
		((ISupportInitialize)textBlock6).BeginInit();
		children13.Add(textBlock6);
		TextBlock textBlock7;
		TextBlock textBlock8 = (textBlock7 = textBlock5);
		context.PushParent(textBlock7);
		TextBlock target = textBlock7;
		StyledProperty<string?> textProperty = TextBlock.TextProperty;
		CompiledBindingExtension compiledBindingExtension24 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002CAvalonia_002EDiagnostics_002EFocusedControl_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = TextBlock.TextProperty;
		CompiledBindingExtension binding24 = compiledBindingExtension24.ProvideValue(context);
		context.ProvideTargetProperty = null;
		target.Bind(textProperty, binding24);
		context.PopParent();
		((ISupportInitialize)textBlock8).EndInit();
		Avalonia.Controls.Controls children14 = stackPanel3.Children;
		Separator separator3;
		Separator separator4 = (separator3 = new Separator());
		((ISupportInitialize)separator4).BeginInit();
		children14.Add(separator4);
		separator3.Width = 8.0;
		((ISupportInitialize)separator3).EndInit();
		Avalonia.Controls.Controls children15 = stackPanel3.Children;
		TextBlock textBlock9;
		TextBlock textBlock10 = (textBlock9 = new TextBlock());
		((ISupportInitialize)textBlock10).BeginInit();
		children15.Add(textBlock10);
		textBlock9.Inlines.Add("Pointer Over:");
		((ISupportInitialize)textBlock9).EndInit();
		Avalonia.Controls.Controls children16 = stackPanel3.Children;
		TextBlock textBlock11;
		TextBlock textBlock12 = (textBlock11 = new TextBlock());
		((ISupportInitialize)textBlock12).BeginInit();
		children16.Add(textBlock12);
		TextBlock textBlock13 = (textBlock7 = textBlock11);
		context.PushParent(textBlock7);
		TextBlock target2 = textBlock7;
		StyledProperty<string?> textProperty2 = TextBlock.TextProperty;
		CompiledBindingExtension compiledBindingExtension25 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002CAvalonia_002EDiagnostics_002EPointerOverElementName_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = TextBlock.TextProperty;
		CompiledBindingExtension binding25 = compiledBindingExtension25.ProvideValue(context);
		context.ProvideTargetProperty = null;
		target2.Bind(textProperty2, binding25);
		context.PopParent();
		((ISupportInitialize)textBlock13).EndInit();
		context.PopParent();
		((ISupportInitialize)stackPanel4).EndInit();
		Avalonia.Controls.Controls children17 = grid9.Children;
		TextBlock textBlock14;
		TextBlock textBlock15 = (textBlock14 = new TextBlock());
		((ISupportInitialize)textBlock15).BeginInit();
		children17.Add(textBlock15);
		TextBlock textBlock16 = (textBlock7 = textBlock14);
		context.PushParent(textBlock7);
		TextBlock textBlock17 = textBlock7;
		Grid.SetColumn(textBlock17, 1);
		textBlock17.Foreground = new ImmutableSolidColorBrush(4286611584u);
		textBlock17.Margin = new Thickness(2.0, 0.0, 2.0, 0.0);
		textBlock17.Text = "Popups frozen";
		StyledProperty<bool> isVisibleProperty2 = Visual.IsVisibleProperty;
		CompiledBindingExtension compiledBindingExtension26 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002CAvalonia_002EDiagnostics_002EFreezePopups_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = Visual.IsVisibleProperty;
		CompiledBindingExtension binding26 = compiledBindingExtension26.ProvideValue(context);
		context.ProvideTargetProperty = null;
		textBlock17.Bind(isVisibleProperty2, binding26);
		context.PopParent();
		((ISupportInitialize)textBlock16).EndInit();
		context.PopParent();
		((ISupportInitialize)grid8).EndInit();
		context.PopParent();
		((ISupportInitialize)border4).EndInit();
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

	private static void _0021XamlIlPopulateTrampoline(MainView P_0)
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
