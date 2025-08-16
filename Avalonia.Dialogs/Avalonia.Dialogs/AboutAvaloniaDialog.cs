using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Styling;
using CompiledAvaloniaXaml;

namespace Avalonia.Dialogs;

public class AboutAvaloniaDialog : Window
{
	private class XamlClosure_1
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<AboutAvaloniaDialog> context = new XamlIlContext.Context<AboutAvaloniaDialog>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FAboutAvaloniaDialog_002Examl.Singleton }, "avares://Avalonia.Dialogs/AboutAvaloniaDialog.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (AboutAvaloniaDialog)service;
				}
			}
			DrawingGroup drawingGroup = new DrawingGroup();
			DrawingCollection children = drawingGroup.Children;
			GeometryDrawing geometryDrawing = new GeometryDrawing();
			geometryDrawing.Brush = new ImmutableSolidColorBrush(4287317164u);
			geometryDrawing.Geometry = Geometry.Parse("M215 246.5h1.5a30 30 0 0 0 29.8-26.5l.2-100a123.2 123.2 0 1 0-125.4 126.5H215Z");
			children.Add(geometryDrawing);
			DrawingCollection children2 = drawingGroup.Children;
			GeometryDrawing geometryDrawing2 = new GeometryDrawing();
			geometryDrawing2.Brush = new ImmutableSolidColorBrush(4294572539u);
			geometryDrawing2.Geometry = Geometry.Parse("M123.7 42a81.3 81.3 0 0 0-79.3 63.5 18.4 18.4 0 0 1 0 35.4 81.3 81.3 0 0 0 118.2 53.6v9.4H205v-80.7A81.2 81.2 0 0 0 123.7 42Zm-39 81.2a39 39 0 1 1 77.9 0 39 39 0 0 1-77.8 0Z");
			children2.Add(geometryDrawing2);
			DrawingCollection children3 = drawingGroup.Children;
			GeometryDrawing geometryDrawing3 = new GeometryDrawing();
			geometryDrawing3.Brush = new ImmutableSolidColorBrush(4294572539u);
			geometryDrawing3.Geometry = Geometry.Parse("M52 123.2a13 13 0 1 1-26 0 13 13 0 0 1 26 0Z");
			children3.Add(geometryDrawing3);
			return drawingGroup;
		}
	}

	private static readonly Version s_version = typeof(AboutAvaloniaDialog).Assembly.GetName().Version;

	private static Action<object> _0021XamlIlPopulateOverride;

	public static string Version { get; } = "v" + s_version.ToString(2);

	public static bool IsDevelopmentBuild { get; } = s_version.Revision == 999;

	public static string Copyright { get; } = $"© {DateTime.Now.Year} The Avalonia Project";

	public AboutAvaloniaDialog()
	{
		_0021XamlIlPopulateTrampoline(this);
		base.DataContext = this;
	}

	private static void ShellExec(string cmd, bool waitForExit = true)
	{
		string text = Regex.Replace(cmd, "(?=[`~!#&*()|;'<>])", "\\").Replace("\"", "\\\\\\\"");
		using Process process = Process.Start(new ProcessStartInfo
		{
			FileName = "/bin/sh",
			Arguments = "-c \"" + text + "\"",
			RedirectStandardOutput = true,
			UseShellExecute = false,
			CreateNoWindow = true,
			WindowStyle = ProcessWindowStyle.Hidden
		});
		if (waitForExit)
		{
			process.WaitForExit();
		}
	}

	private void Button_OnClick(object sender, RoutedEventArgs e)
	{
		string text = "https://www.avaloniaui.net/";
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			ShellExec("xdg-open " + text, waitForExit: false);
			return;
		}
		using (Process.Start(new ProcessStartInfo
		{
			FileName = (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? text : "open"),
			Arguments = (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? (text ?? "") : ""),
			CreateNoWindow = true,
			UseShellExecute = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
		}))
		{
		}
	}

	static void _0021XamlIlPopulate(IServiceProvider P_0, AboutAvaloniaDialog P_1)
	{
		XamlIlContext.Context<AboutAvaloniaDialog> context = new XamlIlContext.Context<AboutAvaloniaDialog>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FAboutAvaloniaDialog_002Examl.Singleton }, "avares://Avalonia.Dialogs/AboutAvaloniaDialog.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		((ISupportInitialize)P_1).BeginInit();
		context.PushParent(P_1);
		P_1.MaxWidth = 400.0;
		P_1.MaxHeight = 475.0;
		P_1.MinWidth = 430.0;
		P_1.MinHeight = 475.0;
		P_1.Title = "About Avalonia";
		P_1.FontFamily = new FontFamily(((IUriContext)context).BaseUri, "/Assets/Roboto-Light.ttf#Roboto");
		Styles styles = P_1.Styles;
		Style style = new Style();
		((ResourceDictionary)style.Resources).AddDeferred((object)"AvaloniaLogo", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_1.Build, context));
		styles.Add(style);
		Grid grid;
		Grid grid2 = (grid = new Grid());
		((ISupportInitialize)grid2).BeginInit();
		P_1.Content = grid2;
		Grid grid3;
		Grid grid4 = (grid3 = grid);
		context.PushParent(grid3);
		Grid grid5 = grid3;
		RowDefinitions rowDefinitions = new RowDefinitions();
		rowDefinitions.Capacity = 2;
		rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		rowDefinitions.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
		grid5.RowDefinitions = rowDefinitions;
		grid5.Margin = new Thickness(35.0, 35.0, 35.0, 15.0);
		Avalonia.Controls.Controls children = grid5.Children;
		Grid grid6;
		Grid grid7 = (grid6 = new Grid());
		((ISupportInitialize)grid7).BeginInit();
		children.Add(grid7);
		Grid grid8 = (grid3 = grid6);
		context.PushParent(grid3);
		Grid grid9 = grid3;
		RowDefinitions rowDefinitions2 = new RowDefinitions();
		rowDefinitions2.Capacity = 3;
		rowDefinitions2.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
		rowDefinitions2.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		rowDefinitions2.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		grid9.RowDefinitions = rowDefinitions2;
		Avalonia.Controls.Controls children2 = grid9.Children;
		Image image;
		Image image2 = (image = new Image());
		((ISupportInitialize)image2).BeginInit();
		children2.Add(image2);
		Image image3;
		Image image4 = (image3 = image);
		context.PushParent(image3);
		image3.Width = 130.0;
		image3.Margin = new Thickness(0.0, 0.0, 0.0, 20.0);
		Grid.SetRow(image3, 0);
		DrawingImage drawingImage;
		DrawingImage source = (drawingImage = new DrawingImage());
		context.PushParent(drawingImage);
		StyledProperty<Drawing?> drawingProperty = DrawingImage.DrawingProperty;
		DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("AvaloniaLogo");
		context.ProvideTargetProperty = DrawingImage.DrawingProperty;
		IBinding binding = dynamicResourceExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		drawingImage.Bind(drawingProperty, binding);
		context.PopParent();
		image3.Source = source;
		context.PopParent();
		((ISupportInitialize)image4).EndInit();
		Avalonia.Controls.Controls children3 = grid9.Children;
		TextBlock textBlock;
		TextBlock textBlock2 = (textBlock = new TextBlock());
		((ISupportInitialize)textBlock2).BeginInit();
		children3.Add(textBlock2);
		textBlock.Text = "Avalonia";
		textBlock.FontWeight = FontWeight.Bold;
		textBlock.HorizontalAlignment = HorizontalAlignment.Center;
		textBlock.FontSize = 32.0;
		Grid.SetRow(textBlock, 1);
		((ISupportInitialize)textBlock).EndInit();
		Avalonia.Controls.Controls children4 = grid9.Children;
		TextBlock textBlock3;
		TextBlock textBlock4 = (textBlock3 = new TextBlock());
		((ISupportInitialize)textBlock4).BeginInit();
		children4.Add(textBlock4);
		TextBlock textBlock5;
		TextBlock textBlock6 = (textBlock5 = textBlock3);
		context.PushParent(textBlock5);
		TextBlock textBlock7 = textBlock5;
		StyledProperty<string?> textProperty = TextBlock.TextProperty;
		CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(XamlIlHelpers.Avalonia_002EDialogs_002EAboutAvaloniaDialog_002CAvalonia_002EDialogs_002EVersion_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = TextBlock.TextProperty;
		CompiledBindingExtension binding2 = compiledBindingExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		textBlock7.Bind(textProperty, (IBinding)binding2);
		textBlock7.FontWeight = FontWeight.Normal;
		textBlock7.HorizontalAlignment = HorizontalAlignment.Center;
		textBlock7.FontSize = 14.0;
		Grid.SetRow(textBlock7, 2);
		context.PopParent();
		((ISupportInitialize)textBlock6).EndInit();
		context.PopParent();
		((ISupportInitialize)grid8).EndInit();
		Avalonia.Controls.Controls children5 = grid5.Children;
		Grid grid10;
		Grid grid11 = (grid10 = new Grid());
		((ISupportInitialize)grid11).BeginInit();
		children5.Add(grid11);
		Grid grid12 = (grid3 = grid10);
		context.PushParent(grid3);
		Grid grid13 = grid3;
		RowDefinitions rowDefinitions3 = new RowDefinitions();
		rowDefinitions3.Capacity = 4;
		rowDefinitions3.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		rowDefinitions3.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
		rowDefinitions3.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
		grid13.RowDefinitions = rowDefinitions3;
		Grid.SetRow(grid13, 1);
		grid13.Margin = new Thickness(0.0, 30.0, 0.0, 0.0);
		Avalonia.Controls.Controls children6 = grid13.Children;
		TextBlock textBlock8;
		TextBlock textBlock9 = (textBlock8 = new TextBlock());
		((ISupportInitialize)textBlock9).BeginInit();
		children6.Add(textBlock9);
		Grid.SetRow(textBlock8, 0);
		textBlock8.Text = "This product is built with Avalonia, a cross-platform UI framework.";
		textBlock8.TextWrapping = TextWrapping.Wrap;
		((ISupportInitialize)textBlock8).EndInit();
		Avalonia.Controls.Controls children7 = grid13.Children;
		TextBlock textBlock10;
		TextBlock textBlock11 = (textBlock10 = new TextBlock());
		((ISupportInitialize)textBlock11).BeginInit();
		children7.Add(textBlock11);
		Grid.SetRow(textBlock10, 1);
		textBlock10.Text = "Avalonia is made possible by the generous support of its contributors and community.";
		textBlock10.TextWrapping = TextWrapping.Wrap;
		textBlock10.Margin = new Thickness(0.0, 15.0, 0.0, 0.0);
		((ISupportInitialize)textBlock10).EndInit();
		Avalonia.Controls.Controls children8 = grid13.Children;
		Button button;
		Button button2 = (button = new Button());
		((ISupportInitialize)button2).BeginInit();
		children8.Add(button2);
		Grid.SetRow(button, 2);
		button.Background = new ImmutableSolidColorBrush(4287317164u);
		button.Foreground = new ImmutableSolidColorBrush(uint.MaxValue);
		button.VerticalAlignment = VerticalAlignment.Center;
		button.Padding = new Thickness(12.0, 12.0, 12.0, 12.0);
		button.Content = "Learn more about Avalonia";
		button.Click += context.RootObject.Button_OnClick;
		button.HorizontalAlignment = HorizontalAlignment.Center;
		((ISupportInitialize)button).EndInit();
		Avalonia.Controls.Controls children9 = grid13.Children;
		TextBlock textBlock12;
		TextBlock textBlock13 = (textBlock12 = new TextBlock());
		((ISupportInitialize)textBlock13).BeginInit();
		children9.Add(textBlock13);
		TextBlock textBlock14 = (textBlock5 = textBlock12);
		context.PushParent(textBlock5);
		TextBlock textBlock15 = textBlock5;
		Grid.SetRow(textBlock15, 4);
		textBlock15.VerticalAlignment = VerticalAlignment.Bottom;
		StyledProperty<string?> textProperty2 = TextBlock.TextProperty;
		CompiledBindingExtension compiledBindingExtension2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Property(XamlIlHelpers.Avalonia_002EDialogs_002EAboutAvaloniaDialog_002CAvalonia_002EDialogs_002ECopyright_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build());
		context.ProvideTargetProperty = TextBlock.TextProperty;
		CompiledBindingExtension binding3 = compiledBindingExtension2.ProvideValue(context);
		context.ProvideTargetProperty = null;
		textBlock15.Bind(textProperty2, (IBinding)binding3);
		textBlock15.HorizontalAlignment = HorizontalAlignment.Center;
		context.PopParent();
		((ISupportInitialize)textBlock14).EndInit();
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

	private static void _0021XamlIlPopulateTrampoline(AboutAvaloniaDialog P_0)
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
