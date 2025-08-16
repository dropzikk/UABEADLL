using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Diagnostics.Controls;
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

namespace CompiledAvaloniaXaml;

[EditorBrowsable(EditorBrowsableState.Never)]
public class _0021AvaloniaResources
{
	public class NamespaceInfo_003A_002FDiagnostics_002FControls_002FBrushEditor_002Eaxaml : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[38]
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
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.DataGrid"),
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
				CreateNamespaceInfo("Avalonia.Themes.Simple", "Avalonia.Themes.Simple")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("controls", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.Controls", (string)null) });
			return dictionary;
		}

		static NamespaceInfo_003A_002FDiagnostics_002FControls_002FBrushEditor_002Eaxaml()
		{
			Singleton = new NamespaceInfo_003A_002FDiagnostics_002FControls_002FBrushEditor_002Eaxaml();
		}
	}

	private class XamlClosure_1
	{
		private class DynamicSetters_2
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
			CompiledAvaloniaXaml.XamlIlContext.Context<Styles> context = new CompiledAvaloniaXaml.XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FDiagnostics_002FControls_002FBrushEditor_002Eaxaml.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Controls/BrushEditor.axaml");
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
			Controls children = grid.Children;
			Button button;
			Button button2 = (button = new Button());
			((ISupportInitialize)button2).BeginInit();
			children.Add(button2);
			Button button3;
			Button button4 = (button3 = button);
			context.PushParent(button3);
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("SimpleTextBoxClearButtonTheme");
			context.ProvideTargetProperty = StyledElement.ThemeProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_2.DynamicSetter_1(button3, BindingPriority.Template, obj);
			button3.Name = "PART_ClearButton";
			service = button3;
			context.AvaloniaNameScope.Register("PART_ClearButton", service);
			button3.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Right, BindingPriority.Template);
			button3.SetValue(ContentControl.VerticalContentAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
			context.PopParent();
			((ISupportInitialize)button4).EndInit();
			context.PopParent();
			((ISupportInitialize)intermediateRoot).EndInit();
			return intermediateRoot;
		}
	}

	public class NamespaceInfo_003A_002FDiagnostics_002FControls_002FFilterTextBox_002Eaxaml : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[38]
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
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.DataGrid"),
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
				CreateNamespaceInfo("Avalonia.Themes.Simple", "Avalonia.Themes.Simple")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("controls", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.Controls", (string)null) });
			return dictionary;
		}

		static NamespaceInfo_003A_002FDiagnostics_002FControls_002FFilterTextBox_002Eaxaml()
		{
			Singleton = new NamespaceInfo_003A_002FDiagnostics_002FControls_002FFilterTextBox_002Eaxaml();
		}
	}

	private class XamlClosure_3
	{
		private class DynamicSetters_4
		{
			public static void DynamicSetter_1(StyledElement P_0, object P_1)
			{
				if (P_1 is UnsetValueType)
				{
					P_0.SetValue(StyledElement.ThemeProperty, AvaloniaProperty.UnsetValue);
					return;
				}
				if (P_1 is IBinding)
				{
					IBinding binding = (IBinding)P_1;
					P_0.Bind(StyledElement.ThemeProperty, binding);
					return;
				}
				if (P_1 is ControlTheme)
				{
					P_0.Theme = (ControlTheme?)P_1;
					return;
				}
				if (P_1 == null)
				{
					P_0.Theme = (ControlTheme?)P_1;
					return;
				}
				throw new InvalidCastException();
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<Styles> context = new CompiledAvaloniaXaml.XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FDiagnostics_002FControls_002FFilterTextBox_002Eaxaml.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Controls/FilterTextBox.axaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			context.IntermediateRoot = new StackPanel();
			object intermediateRoot = context.IntermediateRoot;
			((ISupportInitialize)intermediateRoot).BeginInit();
			StackPanel stackPanel = (StackPanel)intermediateRoot;
			context.PushParent(stackPanel);
			stackPanel.Orientation = Orientation.Horizontal;
			stackPanel.Spacing = 1.0;
			Controls children = stackPanel.Children;
			Button button;
			Button button2 = (button = new Button());
			((ISupportInitialize)button2).BeginInit();
			children.Add(button2);
			Button button3;
			Button button4 = (button3 = button);
			context.PushParent(button3);
			button3.Margin = new Thickness(0.0, 0.0, 2.0, 0.0);
			button3.Classes.Add("textBoxClearButton");
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("SimpleTextBoxClearButtonTheme");
			context.ProvideTargetProperty = StyledElement.ThemeProperty;
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			DynamicSetters_4.DynamicSetter_1(button3, obj);
			button3.Focusable = false;
			ToolTip.SetTip(button3, "Clear");
			button3.Cursor = new Cursor(StandardCursorType.Hand);
			StyledProperty<ICommand?> commandProperty = Button.CommandProperty;
			CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(TextBox), 0).Command("Clear", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EControls_003AAvalonia_002EControls_002ETextBox_002BClear_0_0021CommandExecuteTrampoline, null, null).Build());
			context.ProvideTargetProperty = Button.CommandProperty;
			CompiledBindingExtension binding = compiledBindingExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			button3.Bind(commandProperty, binding);
			button3.Opacity = 0.5;
			context.PopParent();
			((ISupportInitialize)button4).EndInit();
			Controls children2 = stackPanel.Children;
			ToggleButton toggleButton;
			ToggleButton toggleButton2 = (toggleButton = new ToggleButton());
			((ISupportInitialize)toggleButton2).BeginInit();
			children2.Add(toggleButton2);
			ToggleButton toggleButton3;
			ToggleButton toggleButton4 = (toggleButton3 = toggleButton);
			context.PushParent(toggleButton3);
			ToggleButton toggleButton5 = toggleButton3;
			toggleButton5.Classes.Add("filter-text-box-toggle");
			ToolTip.SetTip(toggleButton5, "Match Case");
			StyledProperty<bool?> isCheckedProperty = ToggleButton.IsCheckedProperty;
			CompiledBindingExtension compiledBindingExtension2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(FilterTextBox), 0).Property(FilterTextBox.UseCaseSensitiveFilterProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
			context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
			CompiledBindingExtension binding2 = compiledBindingExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			toggleButton5.Bind(isCheckedProperty, binding2);
			Image image;
			Image image2 = (image = new Image());
			((ISupportInitialize)image2).BeginInit();
			toggleButton5.Content = image2;
			Image image3;
			Image image4 = (image3 = image);
			context.PushParent(image3);
			Image image5 = image3;
			DrawingImage drawingImage;
			DrawingImage source = (drawingImage = new DrawingImage());
			context.PushParent(drawingImage);
			DrawingImage drawingImage2 = drawingImage;
			GeometryDrawing geometryDrawing;
			GeometryDrawing drawing = (geometryDrawing = new GeometryDrawing());
			context.PushParent(geometryDrawing);
			GeometryDrawing geometryDrawing2 = geometryDrawing;
			StyledProperty<IBrush?> brushProperty = GeometryDrawing.BrushProperty;
			CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(ToggleButton), 0).Property(TemplatedControl.ForegroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
			context.ProvideTargetProperty = GeometryDrawing.BrushProperty;
			CompiledBindingExtension binding3 = compiledBindingExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			geometryDrawing2.Bind(brushProperty, binding3);
			geometryDrawing2.Geometry = Geometry.Parse("M7.495 9.052L8.386 11.402H9.477L6.237 3H5.217L2 11.402H3.095L3.933 9.052H7.495ZM5.811 4.453L5.855 4.588L7.173 8.162H4.255L5.562 4.588L5.606 4.453L5.644 4.297L5.676 4.145L5.697 4.019H5.72L5.744 4.145L5.773 4.297L5.811 4.453ZM13.795 10.464V11.4H14.755V7.498C14.755 6.779 14.575 6.226 14.216 5.837C13.857 5.448 13.327 5.254 12.628 5.254C12.429 5.254 12.227 5.273 12.022 5.31C11.817 5.347 11.622 5.394 11.439 5.451C11.256 5.508 11.091 5.569 10.944 5.636C10.797 5.703 10.683 5.765 10.601 5.824V6.808C10.867 6.578 11.167 6.397 11.505 6.268C11.843 6.139 12.194 6.075 12.557 6.075C12.745 6.075 12.915 6.103 13.07 6.16C13.225 6.217 13.357 6.306 13.466 6.427C13.575 6.548 13.659 6.706 13.718 6.899C13.777 7.092 13.806 7.326 13.806 7.599L11.995 7.851C11.651 7.898 11.355 7.977 11.107 8.088C10.859 8.199 10.654 8.339 10.492 8.507C10.33 8.675 10.21 8.868 10.132 9.087C10.054 9.306 10.015 9.546 10.015 9.808C10.015 10.054 10.057 10.283 10.139 10.496C10.221 10.709 10.342 10.893 10.502 11.047C10.662 11.201 10.862 11.323 11.1 11.413C11.338 11.503 11.613 11.548 11.926 11.548C12.328 11.548 12.686 11.456 13.001 11.27C13.316 11.084 13.573 10.816 13.772 10.464H13.795ZM11.667 8.721C11.843 8.657 12.068 8.607 12.341 8.572L13.806 8.367V8.976C13.806 9.222 13.765 9.451 13.683 9.664C13.601 9.877 13.486 10.063 13.34 10.221C13.194 10.379 13.019 10.503 12.816 10.593C12.613 10.683 12.39 10.728 12.148 10.728C11.961 10.728 11.795 10.703 11.653 10.652C11.511 10.601 11.392 10.53 11.296 10.441C11.2 10.352 11.127 10.247 11.076 10.125C11.025 10.003 11 9.873 11 9.732C11 9.568 11.018 9.421 11.055 9.292C11.092 9.163 11.16 9.051 11.257 8.958C11.354 8.865 11.491 8.785 11.667 8.721Z");
			context.PopParent();
			drawingImage2.Drawing = drawing;
			context.PopParent();
			image5.Source = source;
			context.PopParent();
			((ISupportInitialize)image4).EndInit();
			context.PopParent();
			((ISupportInitialize)toggleButton4).EndInit();
			Controls children3 = stackPanel.Children;
			ToggleButton toggleButton6;
			ToggleButton toggleButton7 = (toggleButton6 = new ToggleButton());
			((ISupportInitialize)toggleButton7).BeginInit();
			children3.Add(toggleButton7);
			ToggleButton toggleButton8 = (toggleButton3 = toggleButton6);
			context.PushParent(toggleButton3);
			ToggleButton toggleButton9 = toggleButton3;
			toggleButton9.Classes.Add("filter-text-box-toggle");
			ToolTip.SetTip(toggleButton9, "Match Whole Word");
			StyledProperty<bool?> isCheckedProperty2 = ToggleButton.IsCheckedProperty;
			CompiledBindingExtension compiledBindingExtension4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(FilterTextBox), 0).Property(FilterTextBox.UseWholeWordFilterProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
			context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
			CompiledBindingExtension binding4 = compiledBindingExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			toggleButton9.Bind(isCheckedProperty2, binding4);
			Image image6;
			Image image7 = (image6 = new Image());
			((ISupportInitialize)image7).BeginInit();
			toggleButton9.Content = image7;
			Image image8 = (image3 = image6);
			context.PushParent(image3);
			Image image9 = image3;
			DrawingImage source2 = (drawingImage = new DrawingImage());
			context.PushParent(drawingImage);
			DrawingImage drawingImage3 = drawingImage;
			GeometryDrawing drawing2 = (geometryDrawing = new GeometryDrawing());
			context.PushParent(geometryDrawing);
			GeometryDrawing geometryDrawing3 = geometryDrawing;
			StyledProperty<IBrush?> brushProperty2 = GeometryDrawing.BrushProperty;
			CompiledBindingExtension compiledBindingExtension5 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(ToggleButton), 0).Property(TemplatedControl.ForegroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
			context.ProvideTargetProperty = GeometryDrawing.BrushProperty;
			CompiledBindingExtension binding5 = compiledBindingExtension5.ProvideValue(context);
			context.ProvideTargetProperty = null;
			geometryDrawing3.Bind(brushProperty2, binding5);
			geometryDrawing3.Geometry = Geometry.Parse("M1 2H15V3H1V2ZM14 4H13V12H14V4ZM11.272 8.387C11.194 8.088 11.073 7.825 10.912 7.601C10.751 7.377 10.547 7.2 10.303 7.071C10.059 6.942 9.769 6.878 9.437 6.878C9.239 6.878 9.057 6.902 8.89 6.951C8.725 7 8.574 7.068 8.437 7.156C8.301 7.244 8.18 7.35 8.072 7.474L7.893 7.732V4.578H7V12H7.893V11.425L8.019 11.6C8.106 11.702 8.208 11.79 8.323 11.869C8.44 11.947 8.572 12.009 8.721 12.055C8.87 12.101 9.035 12.123 9.219 12.123C9.572 12.123 9.885 12.052 10.156 11.911C10.428 11.768 10.655 11.573 10.838 11.325C11.021 11.075 11.159 10.782 11.252 10.446C11.345 10.108 11.392 9.743 11.392 9.349C11.391 9.007 11.352 8.686 11.272 8.387ZM9.793 7.78C9.944 7.851 10.075 7.956 10.183 8.094C10.292 8.234 10.377 8.407 10.438 8.611C10.489 8.785 10.52 8.982 10.527 9.198L10.52 9.323C10.52 9.65 10.487 9.943 10.42 10.192C10.353 10.438 10.259 10.645 10.142 10.806C10.025 10.968 9.882 11.091 9.721 11.172C9.399 11.334 8.961 11.338 8.652 11.187C8.499 11.112 8.366 11.012 8.259 10.891C8.174 10.795 8.103 10.675 8.041 10.524C8.041 10.524 7.862 10.077 7.862 9.577C7.862 9.077 8.041 8.575 8.041 8.575C8.103 8.398 8.177 8.257 8.265 8.145C8.379 8.002 8.521 7.886 8.689 7.8C8.857 7.714 9.054 7.671 9.276 7.671C9.466 7.671 9.64 7.708 9.793 7.78ZM15 13H1V14H15V13ZM2.813 10L2.085 12.031H1L1.025 11.959L3.466 4.87305H4.407L6.892 12.031H5.81L5.032 10H2.813ZM3.934 6.42205H3.912L3.007 9.17505H4.848L3.934 6.42205Z");
			context.PopParent();
			drawingImage3.Drawing = drawing2;
			context.PopParent();
			image9.Source = source2;
			context.PopParent();
			((ISupportInitialize)image8).EndInit();
			context.PopParent();
			((ISupportInitialize)toggleButton8).EndInit();
			Controls children4 = stackPanel.Children;
			ToggleButton toggleButton10;
			ToggleButton toggleButton11 = (toggleButton10 = new ToggleButton());
			((ISupportInitialize)toggleButton11).BeginInit();
			children4.Add(toggleButton11);
			ToggleButton toggleButton12 = (toggleButton3 = toggleButton10);
			context.PushParent(toggleButton3);
			ToggleButton toggleButton13 = toggleButton3;
			toggleButton13.Classes.Add("filter-text-box-toggle");
			ToolTip.SetTip(toggleButton13, "Use Regular Expression");
			StyledProperty<bool?> isCheckedProperty3 = ToggleButton.IsCheckedProperty;
			CompiledBindingExtension compiledBindingExtension6 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(FilterTextBox), 0).Property(FilterTextBox.UseRegexFilterProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
			context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
			CompiledBindingExtension binding6 = compiledBindingExtension6.ProvideValue(context);
			context.ProvideTargetProperty = null;
			toggleButton13.Bind(isCheckedProperty3, binding6);
			Image image10;
			Image image11 = (image10 = new Image());
			((ISupportInitialize)image11).BeginInit();
			toggleButton13.Content = image11;
			Image image12 = (image3 = image10);
			context.PushParent(image3);
			Image image13 = image3;
			DrawingImage source3 = (drawingImage = new DrawingImage());
			context.PushParent(drawingImage);
			DrawingImage drawingImage4 = drawingImage;
			GeometryDrawing drawing3 = (geometryDrawing = new GeometryDrawing());
			context.PushParent(geometryDrawing);
			GeometryDrawing geometryDrawing4 = geometryDrawing;
			StyledProperty<IBrush?> brushProperty3 = GeometryDrawing.BrushProperty;
			CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(ToggleButton), 0).Property(TemplatedControl.ForegroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
			context.ProvideTargetProperty = GeometryDrawing.BrushProperty;
			CompiledBindingExtension binding7 = compiledBindingExtension7.ProvideValue(context);
			context.ProvideTargetProperty = null;
			geometryDrawing4.Bind(brushProperty3, binding7);
			geometryDrawing4.Geometry = Geometry.Parse("M10.0122 2H10.9879V5.11346L13.5489 3.55609L14.034 4.44095L11.4702 6L14.034 7.55905L13.5489 8.44391L10.9879 6.88654V10H10.0122V6.88654L7.45114 8.44391L6.96606 7.55905L9.5299 6L6.96606 4.44095L7.45114 3.55609L10.0122 5.11346V2ZM2 10H6V14H2V10Z");
			context.PopParent();
			drawingImage4.Drawing = drawing3;
			context.PopParent();
			image13.Source = source3;
			context.PopParent();
			((ISupportInitialize)image12).EndInit();
			context.PopParent();
			((ISupportInitialize)toggleButton12).EndInit();
			context.PopParent();
			((ISupportInitialize)intermediateRoot).EndInit();
			return intermediateRoot;
		}
	}

	public class NamespaceInfo_003A_002FDiagnostics_002FControls_002FThicknessEditor_002Eaxaml : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[38]
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
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.DataGrid"),
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
				CreateNamespaceInfo("Avalonia.Themes.Simple", "Avalonia.Themes.Simple")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("controls", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.Controls", (string)null) });
			return dictionary;
		}

		static NamespaceInfo_003A_002FDiagnostics_002FControls_002FThicknessEditor_002Eaxaml()
		{
			Singleton = new NamespaceInfo_003A_002FDiagnostics_002FControls_002FThicknessEditor_002Eaxaml();
		}
	}

	private class XamlClosure_5
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<Styles> context = new CompiledAvaloniaXaml.XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FDiagnostics_002FControls_002FThicknessEditor_002Eaxaml.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Controls/ThicknessEditor.axaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new SolidColorBrush
			{
				Color = Color.FromUInt32(4284782061u)
			};
		}
	}

	private class XamlClosure_6
	{
		public static object Build(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<Styles> context = new CompiledAvaloniaXaml.XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FDiagnostics_002FControls_002FThicknessEditor_002Eaxaml.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Controls/ThicknessEditor.axaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new SolidColorBrush
			{
				Color = Color.FromUInt32(4284900966u)
			};
		}
	}

	private class XamlClosure_7
	{
		private class DynamicSetters_8
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
			CompiledAvaloniaXaml.XamlIlContext.Context<Styles> context = new CompiledAvaloniaXaml.XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FDiagnostics_002FControls_002FThicknessEditor_002Eaxaml.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Controls/ThicknessEditor.axaml");
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
			context.IntermediateRoot = new Panel();
			object intermediateRoot = context.IntermediateRoot;
			((ISupportInitialize)intermediateRoot).BeginInit();
			Panel panel = (Panel)intermediateRoot;
			context.PushParent(panel);
			Controls children = panel.Children;
			Rectangle rectangle;
			Rectangle rectangle2 = (rectangle = new Rectangle());
			((ISupportInitialize)rectangle2).BeginInit();
			children.Add(rectangle2);
			Rectangle rectangle3;
			Rectangle rectangle4 = (rectangle3 = rectangle);
			context.PushParent(rectangle3);
			rectangle3.Name = "PART_Background";
			service = rectangle3;
			context.AvaloniaNameScope.Register("PART_Background", service);
			CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Not().ElementName(context.AvaloniaNameScope, "PART_ContentPresenter").Property(InputElement.IsPointerOverProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
				.Build());
			context.ProvideTargetProperty = "class:no-content-pointerover";
			CompiledBindingExtension compiledBindingExtension2 = compiledBindingExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			IBinding source = compiledBindingExtension2;
			rectangle3.BindClass("no-content-pointerover", source, null);
			context.PopParent();
			((ISupportInitialize)rectangle4).EndInit();
			Controls children2 = panel.Children;
			Border border;
			Border border2 = (border = new Border());
			((ISupportInitialize)border2).BeginInit();
			children2.Add(border2);
			Border border3;
			Border border4 = (border3 = border);
			context.PushParent(border3);
			border3.Name = "PART_Border";
			service = border3;
			context.AvaloniaNameScope.Register("PART_Border", service);
			CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Not().ElementName(context.AvaloniaNameScope, "PART_ContentPresenter").Property(InputElement.IsPointerOverProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
				.Build());
			context.ProvideTargetProperty = "class:no-content-pointerover";
			CompiledBindingExtension compiledBindingExtension4 = compiledBindingExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			source = compiledBindingExtension4;
			border3.BindClass("no-content-pointerover", source, null);
			border3.Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
			border3.Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
			Grid grid;
			Grid grid2 = (grid = new Grid());
			((ISupportInitialize)grid2).BeginInit();
			border3.Child = grid2;
			Grid grid3;
			Grid grid4 = (grid3 = grid);
			context.PushParent(grid3);
			RowDefinitions rowDefinitions = new RowDefinitions();
			rowDefinitions.Capacity = 3;
			rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
			rowDefinitions.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
			rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
			grid3.RowDefinitions = rowDefinitions;
			ColumnDefinitions columnDefinitions = new ColumnDefinitions();
			columnDefinitions.Capacity = 3;
			columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
			columnDefinitions.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
			columnDefinitions.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
			grid3.ColumnDefinitions = columnDefinitions;
			Styles styles = grid3.Styles;
			Style style;
			Style item = (style = new Style());
			context.PushParent(style);
			style.Selector = ((Selector?)null).OfType(typeof(TextBox)).Class("thickness-edit");
			Setter setter = new Setter();
			setter.Property = TemplatedControl.BackgroundProperty;
			setter.Value = new ImmutableSolidColorBrush(16777215u);
			style.Add(setter);
			Setter setter2 = new Setter();
			setter2.Property = TemplatedControl.BorderThicknessProperty;
			setter2.Value = new Thickness(0.0, 0.0, 0.0, 0.0);
			style.Add(setter2);
			Setter setter3 = new Setter();
			setter3.Property = Layoutable.MarginProperty;
			setter3.Value = new Thickness(2.0, 2.0, 2.0, 2.0);
			style.Add(setter3);
			Setter setter4 = new Setter();
			setter4.Property = Layoutable.HorizontalAlignmentProperty;
			setter4.Value = HorizontalAlignment.Stretch;
			style.Add(setter4);
			Setter setter5 = new Setter();
			setter5.Property = Layoutable.VerticalAlignmentProperty;
			setter5.Value = VerticalAlignment.Stretch;
			style.Add(setter5);
			Setter setter6 = new Setter();
			setter6.Property = TextBox.HorizontalContentAlignmentProperty;
			setter6.Value = HorizontalAlignment.Center;
			style.Add(setter6);
			Setter setter7 = new Setter();
			setter7.Property = TextBox.VerticalContentAlignmentProperty;
			setter7.Value = VerticalAlignment.Center;
			style.Add(setter7);
			Setter setter8 = new Setter();
			setter8.Property = ScrollViewer.HorizontalScrollBarVisibilityProperty;
			setter8.Value = ScrollBarVisibility.Disabled;
			style.Add(setter8);
			Setter setter9 = new Setter();
			setter9.Property = ScrollViewer.VerticalScrollBarVisibilityProperty;
			setter9.Value = ScrollBarVisibility.Disabled;
			style.Add(setter9);
			Setter setter10;
			Setter setter11 = (setter10 = new Setter());
			context.PushParent(setter10);
			setter10.Property = Visual.IsVisibleProperty;
			CompiledBindingExtension compiledBindingExtension5 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(ThicknessEditor), 0).Property(ThicknessEditor.IsPresentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
			context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			CompiledBindingExtension value = compiledBindingExtension5.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter10.Value = value;
			context.PopParent();
			style.Add(setter11);
			context.PopParent();
			styles.Add(item);
			Controls children3 = grid3.Children;
			TextBlock textBlock;
			TextBlock textBlock2 = (textBlock = new TextBlock());
			((ISupportInitialize)textBlock2).BeginInit();
			children3.Add(textBlock2);
			textBlock.Bind(Visual.IsVisibleProperty, new TemplateBinding(ThicknessEditor.IsPresentProperty).ProvideValue());
			textBlock.SetValue(Layoutable.MarginProperty, new Thickness(4.0, 0.0, 0.0, 0.0), BindingPriority.Template);
			textBlock.Bind(TextBlock.TextProperty, new TemplateBinding(ThicknessEditor.HeaderProperty).ProvideValue());
			textBlock.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
			textBlock.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
			textBlock.SetValue(Grid.ColumnSpanProperty, 2, BindingPriority.Template);
			((ISupportInitialize)textBlock).EndInit();
			Controls children4 = grid3.Children;
			TextBox textBox;
			TextBox textBox2 = (textBox = new TextBox());
			((ISupportInitialize)textBox2).BeginInit();
			children4.Add(textBox2);
			TextBox textBox3;
			TextBox textBox4 = (textBox3 = textBox);
			context.PushParent(textBox3);
			TextBox textBox5 = textBox3;
			textBox5.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
			textBox5.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
			StyledProperty<string?> textProperty = TextBox.TextProperty;
			CompiledBindingExtension compiledBindingExtension6 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ThicknessEditor.LeftProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
				.Build());
			context.ProvideTargetProperty = TextBox.TextProperty;
			CompiledBindingExtension binding = compiledBindingExtension6.ProvideValue(context);
			context.ProvideTargetProperty = null;
			textBox5.Bind(textProperty, binding);
			textBox5.Classes.Add("thickness-edit");
			context.PopParent();
			((ISupportInitialize)textBox4).EndInit();
			Controls children5 = grid3.Children;
			TextBox textBox6;
			TextBox textBox7 = (textBox6 = new TextBox());
			((ISupportInitialize)textBox7).BeginInit();
			children5.Add(textBox7);
			TextBox textBox8 = (textBox3 = textBox6);
			context.PushParent(textBox3);
			TextBox textBox9 = textBox3;
			textBox9.Name = "Right";
			service = textBox9;
			context.AvaloniaNameScope.Register("Right", service);
			textBox9.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
			textBox9.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
			StyledProperty<string?> textProperty2 = TextBox.TextProperty;
			CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ThicknessEditor.TopProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
				.Build());
			context.ProvideTargetProperty = TextBox.TextProperty;
			CompiledBindingExtension binding2 = compiledBindingExtension7.ProvideValue(context);
			context.ProvideTargetProperty = null;
			textBox9.Bind(textProperty2, binding2);
			textBox9.Classes.Add("thickness-edit");
			context.PopParent();
			((ISupportInitialize)textBox8).EndInit();
			Controls children6 = grid3.Children;
			TextBox textBox10;
			TextBox textBox11 = (textBox10 = new TextBox());
			((ISupportInitialize)textBox11).BeginInit();
			children6.Add(textBox11);
			TextBox textBox12 = (textBox3 = textBox10);
			context.PushParent(textBox3);
			TextBox textBox13 = textBox3;
			textBox13.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
			textBox13.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
			StyledProperty<string?> textProperty3 = TextBox.TextProperty;
			CompiledBindingExtension compiledBindingExtension8 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ThicknessEditor.RightProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
				.Build());
			context.ProvideTargetProperty = TextBox.TextProperty;
			CompiledBindingExtension binding3 = compiledBindingExtension8.ProvideValue(context);
			context.ProvideTargetProperty = null;
			textBox13.Bind(textProperty3, binding3);
			textBox13.Classes.Add("thickness-edit");
			context.PopParent();
			((ISupportInitialize)textBox12).EndInit();
			Controls children7 = grid3.Children;
			TextBox textBox14;
			TextBox textBox15 = (textBox14 = new TextBox());
			((ISupportInitialize)textBox15).BeginInit();
			children7.Add(textBox15);
			TextBox textBox16 = (textBox3 = textBox14);
			context.PushParent(textBox3);
			TextBox textBox17 = textBox3;
			textBox17.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
			textBox17.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
			StyledProperty<string?> textProperty4 = TextBox.TextProperty;
			CompiledBindingExtension compiledBindingExtension9 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ThicknessEditor.BottomProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
				.Build());
			context.ProvideTargetProperty = TextBox.TextProperty;
			CompiledBindingExtension binding4 = compiledBindingExtension9.ProvideValue(context);
			context.ProvideTargetProperty = null;
			textBox17.Bind(textProperty4, binding4);
			textBox17.Classes.Add("thickness-edit");
			context.PopParent();
			((ISupportInitialize)textBox16).EndInit();
			Controls children8 = grid3.Children;
			ContentPresenter contentPresenter;
			ContentPresenter contentPresenter2 = (contentPresenter = new ContentPresenter());
			((ISupportInitialize)contentPresenter2).BeginInit();
			children8.Add(contentPresenter2);
			contentPresenter.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
			contentPresenter.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
			contentPresenter.Name = "PART_ContentPresenter";
			service = contentPresenter;
			context.AvaloniaNameScope.Register("PART_ContentPresenter", service);
			contentPresenter.Bind(ContentPresenter.ContentTemplateProperty, new TemplateBinding(ContentControl.ContentTemplateProperty).ProvideValue());
			DynamicSetters_8.DynamicSetter_1(contentPresenter, BindingPriority.Template, new TemplateBinding(ContentControl.ContentProperty).ProvideValue());
			contentPresenter.Bind(ContentPresenter.PaddingProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
			contentPresenter.Bind(ContentPresenter.VerticalContentAlignmentProperty, new TemplateBinding(ContentControl.VerticalContentAlignmentProperty).ProvideValue());
			contentPresenter.Bind(ContentPresenter.HorizontalContentAlignmentProperty, new TemplateBinding(ContentControl.HorizontalContentAlignmentProperty).ProvideValue());
			((ISupportInitialize)contentPresenter).EndInit();
			context.PopParent();
			((ISupportInitialize)grid4).EndInit();
			context.PopParent();
			((ISupportInitialize)border4).EndInit();
			context.PopParent();
			((ISupportInitialize)intermediateRoot).EndInit();
			return intermediateRoot;
		}
	}

	public class NamespaceInfo_003A_002FDiagnostics_002FViews_002FConsoleView_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[38]
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
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.DataGrid"),
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
				CreateNamespaceInfo("Avalonia.Themes.Simple", "Avalonia.Themes.Simple")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("viewModels", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.ViewModels", (string)null) });
			return dictionary;
		}

		static NamespaceInfo_003A_002FDiagnostics_002FViews_002FConsoleView_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FDiagnostics_002FViews_002FConsoleView_002Examl();
		}
	}

	public class NamespaceInfo_003A_002FDiagnostics_002FViews_002FControlDetailsView_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> dictionary = new Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>>(7);
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[38]
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
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.DataGrid"),
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
				CreateNamespaceInfo("Avalonia.Themes.Simple", "Avalonia.Themes.Simple")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("conv", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.Converters", (string)null) });
			dictionary.Add("local", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.Views", (string)null) });
			dictionary.Add("controls", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.Controls", (string)null) });
			dictionary.Add("vm", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.ViewModels", (string)null) });
			dictionary.Add("lb", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.Behaviors", (string)null) });
			return dictionary;
		}

		static NamespaceInfo_003A_002FDiagnostics_002FViews_002FControlDetailsView_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FDiagnostics_002FViews_002FControlDetailsView_002Examl();
		}
	}

	public class NamespaceInfo_003A_002FDiagnostics_002FViews_002FEventsPageView_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[38]
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
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.DataGrid"),
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
				CreateNamespaceInfo("Avalonia.Themes.Simple", "Avalonia.Themes.Simple")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("vm", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.ViewModels", (string)null) });
			dictionary.Add("controls", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.Controls", (string)null) });
			return dictionary;
		}

		static NamespaceInfo_003A_002FDiagnostics_002FViews_002FEventsPageView_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FDiagnostics_002FViews_002FEventsPageView_002Examl();
		}
	}

	public class NamespaceInfo_003A_002FDiagnostics_002FViews_002FLayoutExplorerView_002Eaxaml : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> dictionary = new Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>>(5);
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[38]
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
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.DataGrid"),
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
				CreateNamespaceInfo("Avalonia.Themes.Simple", "Avalonia.Themes.Simple")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("controls", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.Controls", (string)null) });
			dictionary.Add("converters", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.Converters", (string)null) });
			dictionary.Add("viewModels", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.ViewModels", (string)null) });
			return dictionary;
		}

		static NamespaceInfo_003A_002FDiagnostics_002FViews_002FLayoutExplorerView_002Eaxaml()
		{
			Singleton = new NamespaceInfo_003A_002FDiagnostics_002FViews_002FLayoutExplorerView_002Eaxaml();
		}
	}

	public class NamespaceInfo_003A_002FDiagnostics_002FViews_002FMainView_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[38]
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
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.DataGrid"),
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
				CreateNamespaceInfo("Avalonia.Themes.Simple", "Avalonia.Themes.Simple")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("views", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.Views", (string)null) });
			dictionary.Add("viewModels", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.ViewModels", (string)null) });
			return dictionary;
		}

		static NamespaceInfo_003A_002FDiagnostics_002FViews_002FMainView_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FDiagnostics_002FViews_002FMainView_002Examl();
		}
	}

	public class NamespaceInfo_003A_002FDiagnostics_002FViews_002FMainWindow_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> dictionary = new Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>>(5);
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[38]
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
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.DataGrid"),
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
				CreateNamespaceInfo("Avalonia.Themes.Simple", "Avalonia.Themes.Simple")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("views", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.Views", (string)null) });
			dictionary.Add("diag", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics", (string)null) });
			dictionary.Add("viewModels", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.ViewModels", (string)null) });
			return dictionary;
		}

		static NamespaceInfo_003A_002FDiagnostics_002FViews_002FMainWindow_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FDiagnostics_002FViews_002FMainWindow_002Examl();
		}
	}

	public class NamespaceInfo_003A_002FDiagnostics_002FViews_002FTreePageView_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[38]
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
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.DataGrid"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.DataGrid"),
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
				CreateNamespaceInfo("Avalonia.Themes.Simple", "Avalonia.Themes.Simple")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("vm", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Diagnostics.ViewModels", (string)null) });
			return dictionary;
		}

		static NamespaceInfo_003A_002FDiagnostics_002FViews_002FTreePageView_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FDiagnostics_002FViews_002FTreePageView_002Examl();
		}
	}

	static void Populate_003A_002FDiagnostics_002FControls_002FThicknessEditor_002Eaxaml(IServiceProvider P_0, Styles P_1)
	{
		CompiledAvaloniaXaml.XamlIlContext.Context<Styles> context = new CompiledAvaloniaXaml.XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FDiagnostics_002FControls_002FThicknessEditor_002Eaxaml.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Controls/ThicknessEditor.axaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		Styles styles;
		Styles styles2 = (styles = P_1);
		context.PushParent(styles);
		((ResourceDictionary)styles.Resources).AddDeferred((object)"HighlightBorderBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_5.Build, context));
		((ResourceDictionary)styles.Resources).AddDeferred((object)"ThicknessBorderBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_6.Build, context));
		Style style;
		Style item = (style = new Style());
		context.PushParent(style);
		Style style2 = style;
		style2.Selector = ((Selector?)null).OfType(typeof(ThicknessEditor));
		Setter setter = new Setter();
		setter.Property = ContentControl.HorizontalContentAlignmentProperty;
		setter.Value = HorizontalAlignment.Center;
		style2.Add(setter);
		Setter setter2 = new Setter();
		setter2.Property = ContentControl.VerticalContentAlignmentProperty;
		setter2.Value = VerticalAlignment.Center;
		style2.Add(setter2);
		Setter setter3 = new Setter();
		setter3.Property = TemplatedControl.BorderThicknessProperty;
		setter3.Value = new Thickness(1.0, 1.0, 1.0, 1.0);
		style2.Add(setter3);
		Setter setter4;
		Setter setter5 = (setter4 = new Setter());
		context.PushParent(setter4);
		Setter setter6 = setter4;
		setter6.Property = TemplatedControl.BorderBrushProperty;
		StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ThicknessBorderBrush");
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
		object? value = staticResourceExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		setter6.Value = value;
		context.PopParent();
		style2.Add(setter5);
		Setter setter7 = (setter4 = new Setter());
		context.PushParent(setter4);
		Setter setter8 = setter4;
		setter8.Property = TemplatedControl.TemplateProperty;
		ControlTemplate controlTemplate;
		ControlTemplate value2 = (controlTemplate = new ControlTemplate());
		context.PushParent(controlTemplate);
		controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_7.Build, context);
		context.PopParent();
		setter8.Value = value2;
		context.PopParent();
		style2.Add(setter7);
		context.PopParent();
		styles.Add(item);
		Style style3 = new Style();
		style3.Selector = ((Selector?)null).OfType(typeof(ThicknessEditor)).PropertyEquals((AvaloniaProperty)ThicknessEditor.IsPresentProperty, (object?)false);
		Setter setter9 = new Setter();
		setter9.Property = TemplatedControl.BorderThicknessProperty;
		setter9.Value = new Thickness(0.0, 0.0, 0.0, 0.0);
		style3.Add(setter9);
		styles.Add(style3);
		Style item2 = (style = new Style());
		context.PushParent(style);
		Style style4 = style;
		style4.Selector = ((Selector?)null).OfType(typeof(ThicknessEditor)).Template().OfType(typeof(Rectangle))
			.Name("PART_Background");
		Setter setter10 = (setter4 = new Setter());
		context.PushParent(setter4);
		Setter setter11 = setter4;
		setter11.Property = Shape.FillProperty;
		CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(TemplatedControl.BackgroundProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
			.Build());
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
		CompiledBindingExtension value3 = compiledBindingExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		setter11.Value = value3;
		context.PopParent();
		style4.Add(setter10);
		context.PopParent();
		styles.Add(item2);
		Style item3 = (style = new Style());
		context.PushParent(style);
		Style style5 = style;
		style5.Selector = ((Selector?)null).OfType(typeof(ThicknessEditor)).Class(":pointerover").Template()
			.OfType(typeof(Rectangle))
			.Name("PART_Background")
			.Class("no-content-pointerover");
		Setter setter12 = (setter4 = new Setter());
		context.PushParent(setter4);
		Setter setter13 = setter4;
		setter13.Property = Shape.FillProperty;
		CompiledBindingExtension compiledBindingExtension2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ThicknessEditor.HighlightProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
			.Build());
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
		CompiledBindingExtension value4 = compiledBindingExtension2.ProvideValue(context);
		context.ProvideTargetProperty = null;
		setter13.Value = value4;
		context.PopParent();
		style5.Add(setter12);
		context.PopParent();
		styles.Add(item3);
		Style item4 = (style = new Style());
		context.PushParent(style);
		Style style6 = style;
		style6.Selector = ((Selector?)null).OfType(typeof(ThicknessEditor)).Class(":pointerover").Template()
			.OfType(typeof(Border))
			.Name("PART_Border")
			.Class("no-content-pointerover");
		Setter setter14 = (setter4 = new Setter());
		context.PushParent(setter4);
		Setter setter15 = setter4;
		setter15.Property = Border.BorderBrushProperty;
		StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("HighlightBorderBrush");
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
		object? value5 = staticResourceExtension2.ProvideValue(context);
		context.ProvideTargetProperty = null;
		setter15.Value = value5;
		context.PopParent();
		style6.Add(setter14);
		context.PopParent();
		styles.Add(item4);
		context.PopParent();
		if (styles2 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	static Styles Build_003A_002FDiagnostics_002FControls_002FThicknessEditor_002Eaxaml(IServiceProvider P_0)
	{
		Styles styles = new Styles();
		Populate_003A_002FDiagnostics_002FControls_002FThicknessEditor_002Eaxaml(P_0, styles);
		return styles;
	}

	static void Populate_003A_002FDiagnostics_002FControls_002FFilterTextBox_002Eaxaml(IServiceProvider P_0, Styles P_1)
	{
		CompiledAvaloniaXaml.XamlIlContext.Context<Styles> context = new CompiledAvaloniaXaml.XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FDiagnostics_002FControls_002FFilterTextBox_002Eaxaml.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Controls/FilterTextBox.axaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		Styles styles;
		Styles styles2 = (styles = P_1);
		context.PushParent(styles);
		Style style;
		Style item = (style = new Style());
		context.PushParent(style);
		Style style2 = style;
		style2.Selector = ((Selector?)null).OfType(typeof(TextBox)).Class("filter-text-box");
		Setter setter = new Setter();
		setter.Property = TextBox.VerticalContentAlignmentProperty;
		setter.Value = VerticalAlignment.Center;
		style2.Add(setter);
		Setter setter2;
		Setter setter3 = (setter2 = new Setter());
		context.PushParent(setter2);
		Setter setter4 = setter2;
		setter4.Property = TextBox.InnerRightContentProperty;
		Template template;
		Template value = (template = new Template());
		context.PushParent(template);
		template.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_3.Build, context);
		context.PopParent();
		setter4.Value = value;
		context.PopParent();
		style2.Add(setter3);
		context.PopParent();
		styles.Add(item);
		Style style3 = new Style();
		style3.Selector = ((Selector?)null).OfType(typeof(TextBox)).Class("filter-text-box").Descendant()
			.OfType(typeof(Button))
			.Class("textBoxClearButton");
		Setter setter5 = new Setter();
		setter5.Property = Visual.IsVisibleProperty;
		setter5.Value = false;
		style3.Add(setter5);
		styles.Add(style3);
		Style style4 = new Style();
		style4.Selector = ((Selector?)null).OfType(typeof(TextBox)).Class("filter-text-box").PropertyEquals((AvaloniaProperty)TextBox.AcceptsReturnProperty, (object?)false)
			.PropertyEquals((AvaloniaProperty)TextBox.IsReadOnlyProperty, (object?)false)
			.Class(":focus")
			.Not(((Selector?)null).OfType(typeof(TextBox)).Class(":empty"))
			.Descendant()
			.OfType(typeof(Button))
			.Class("textBoxClearButton");
		Setter setter6 = new Setter();
		setter6.Property = Visual.IsVisibleProperty;
		setter6.Value = true;
		style4.Add(setter6);
		styles.Add(style4);
		Style style5 = new Style();
		style5.Selector = ((Selector?)null).OfType(typeof(ToggleButton)).Class("filter-text-box-toggle").Child()
			.OfType(typeof(Image));
		Setter setter7 = new Setter();
		setter7.Property = Layoutable.WidthProperty;
		setter7.Value = 16.0;
		style5.Add(setter7);
		Setter setter8 = new Setter();
		setter8.Property = Layoutable.HeightProperty;
		setter8.Value = 16.0;
		style5.Add(setter8);
		Setter setter9 = new Setter();
		setter9.Property = Visual.OpacityProperty;
		setter9.Value = 0.5;
		style5.Add(setter9);
		styles.Add(style5);
		Style style6 = new Style();
		style6.Selector = ((Selector?)null).OfType(typeof(ToggleButton)).Class("filter-text-box-toggle").Class(":pointerover")
			.Child()
			.OfType(typeof(Image));
		Setter setter10 = new Setter();
		setter10.Property = Visual.OpacityProperty;
		setter10.Value = 0.7;
		style6.Add(setter10);
		styles.Add(style6);
		Style style7 = new Style();
		style7.Selector = Selectors.Or(new List<Selector>
		{
			((Selector?)null).OfType(typeof(ToggleButton)).Class("filter-text-box-toggle").Class(":pressed")
				.Child()
				.OfType(typeof(Image)),
			((Selector?)null).OfType(typeof(ToggleButton)).Class("filter-text-box-toggle").Class(":checked")
				.Child()
				.OfType(typeof(Image))
		});
		Setter setter11 = new Setter();
		setter11.Property = Visual.OpacityProperty;
		setter11.Value = 0.7;
		style7.Add(setter11);
		styles.Add(style7);
		Style style8 = new Style();
		style8.Selector = ((Selector?)null).OfType(typeof(ToggleButton)).Class("filter-text-box-toggle");
		Setter setter12 = new Setter();
		setter12.Property = InputElement.CursorProperty;
		setter12.Value = new Cursor(StandardCursorType.Hand);
		style8.Add(setter12);
		Setter setter13 = new Setter();
		setter13.Property = TemplatedControl.PaddingProperty;
		setter13.Value = new Thickness(0.0, 1.0, 0.0, 1.0);
		style8.Add(setter13);
		Setter setter14 = new Setter();
		setter14.Property = Layoutable.WidthProperty;
		setter14.Value = 24.0;
		style8.Add(setter14);
		Setter setter15 = new Setter();
		setter15.Property = TemplatedControl.BorderThicknessProperty;
		setter15.Value = new Thickness(1.0, 1.0, 1.0, 1.0);
		style8.Add(setter15);
		Setter setter16 = new Setter();
		setter16.Property = Layoutable.VerticalAlignmentProperty;
		setter16.Value = VerticalAlignment.Top;
		style8.Add(setter16);
		styles.Add(style8);
		Style style9 = new Style();
		style9.Selector = ((Selector?)null).OfType(typeof(ToggleButton)).Class("filter-text-box-toggle").Template()
			.OfType(typeof(ContentPresenter));
		Setter setter17 = new Setter();
		setter17.Property = ContentPresenter.BackgroundProperty;
		setter17.Value = new ImmutableSolidColorBrush(16777215u);
		style9.Add(setter17);
		Setter setter18 = new Setter();
		setter18.Property = ContentPresenter.BorderBrushProperty;
		setter18.Value = new ImmutableSolidColorBrush(16777215u);
		style9.Add(setter18);
		Setter setter19 = new Setter();
		setter19.Property = ContentPresenter.CornerRadiusProperty;
		setter19.Value = new CornerRadius(0.0, 0.0, 0.0, 0.0);
		style9.Add(setter19);
		styles.Add(style9);
		Style item2 = (style = new Style());
		context.PushParent(style);
		Style style10 = style;
		style10.Selector = Selectors.Or(new List<Selector>
		{
			((Selector?)null).OfType(typeof(ToggleButton)).Class("filter-text-box-toggle").Class(":pressed")
				.Template()
				.OfType(typeof(ContentPresenter)),
			((Selector?)null).OfType(typeof(ToggleButton)).Class("filter-text-box-toggle").Class(":checked")
				.Template()
				.OfType(typeof(ContentPresenter))
		});
		Setter setter20 = (setter2 = new Setter());
		context.PushParent(setter2);
		Setter setter21 = setter2;
		setter21.Property = ContentPresenter.BorderBrushProperty;
		DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeAccentColor");
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
		IBinding value2 = dynamicResourceExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		setter21.Value = value2;
		context.PopParent();
		style10.Add(setter20);
		Setter setter22 = (setter2 = new Setter());
		context.PushParent(setter2);
		Setter setter23 = setter2;
		setter23.Property = ContentPresenter.BackgroundProperty;
		DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ThemeAccentColor4");
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
		IBinding value3 = dynamicResourceExtension2.ProvideValue(context);
		context.ProvideTargetProperty = null;
		setter23.Value = value3;
		context.PopParent();
		style10.Add(setter22);
		context.PopParent();
		styles.Add(item2);
		context.PopParent();
		if (styles2 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	static Styles Build_003A_002FDiagnostics_002FControls_002FFilterTextBox_002Eaxaml(IServiceProvider P_0)
	{
		Styles styles = new Styles();
		Populate_003A_002FDiagnostics_002FControls_002FFilterTextBox_002Eaxaml(P_0, styles);
		return styles;
	}

	static void Populate_003A_002FDiagnostics_002FControls_002FBrushEditor_002Eaxaml(IServiceProvider P_0, Styles P_1)
	{
		CompiledAvaloniaXaml.XamlIlContext.Context<Styles> context = new CompiledAvaloniaXaml.XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FDiagnostics_002FControls_002FBrushEditor_002Eaxaml.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Controls/BrushEditor.axaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		Styles styles;
		Styles styles2 = (styles = P_1);
		context.PushParent(styles);
		Style style;
		Style item = (style = new Style());
		context.PushParent(style);
		style.Selector = ((Selector?)null).Is(typeof(BrushEditor));
		Setter setter;
		Setter setter2 = (setter = new Setter());
		context.PushParent(setter);
		setter.Property = TemplatedControl.TemplateProperty;
		ControlTemplate controlTemplate;
		ControlTemplate value = (controlTemplate = new ControlTemplate());
		context.PushParent(controlTemplate);
		controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_1.Build, context);
		context.PopParent();
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

	static Styles Build_003A_002FDiagnostics_002FControls_002FBrushEditor_002Eaxaml(IServiceProvider P_0)
	{
		Styles styles = new Styles();
		Populate_003A_002FDiagnostics_002FControls_002FBrushEditor_002Eaxaml(P_0, styles);
		return styles;
	}
}
