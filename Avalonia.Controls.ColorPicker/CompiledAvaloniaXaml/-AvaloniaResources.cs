using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.Converters;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
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
	public class NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.ColorPicker")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("converters", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Controls.Converters", (string)null) });
			return dictionary;
		}

		static NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl();
		}
	}

	private class XamlClosure_1
	{
		private class XamlClosure_2
		{
			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
				Panel panel2 = panel;
				panel2.Name = "PART_LayoutRoot";
				service = panel2;
				context.AvaloniaNameScope.Register("PART_LayoutRoot", service);
				panel2.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				panel2.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				Controls children = panel2.Children;
				Panel panel3;
				Panel panel4 = (panel3 = new Panel());
				((ISupportInitialize)panel4).BeginInit();
				children.Add(panel4);
				Panel panel5 = (panel = panel3);
				context.PushParent(panel);
				Panel panel6 = panel;
				panel6.Name = "PART_SizingPanel";
				service = panel6;
				context.AvaloniaNameScope.Register("PART_SizingPanel", service);
				panel6.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				panel6.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				panel6.SetValue(Visual.ClipToBoundsProperty, value: true, BindingPriority.Template);
				Controls children2 = panel6.Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children2.Add(rectangle2);
				Rectangle rectangle3;
				Rectangle rectangle4 = (rectangle3 = rectangle);
				context.PushParent(rectangle3);
				Rectangle rectangle5 = rectangle3;
				rectangle5.Name = "PART_SpectrumRectangle";
				service = rectangle5;
				context.AvaloniaNameScope.Register("PART_SpectrumRectangle", service);
				rectangle5.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				rectangle5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty = Visual.IsVisibleProperty;
				TemplateBinding templateBinding;
				TemplateBinding templateBinding2 = (templateBinding = new TemplateBinding(ColorSpectrum.ShapeProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding3 = templateBinding;
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding3.Converter = (IValueConverter)obj;
				templateBinding3.ConverterParameter = ColorSpectrumShape.Box;
				context.PopParent();
				rectangle5.Bind(isVisibleProperty, templateBinding2.ProvideValue());
				StyledProperty<double> radiusXProperty = Rectangle.RadiusXProperty;
				TemplateBinding templateBinding4 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding5 = templateBinding;
				StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("TopLeftCornerRadiusConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj2 = staticResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding5.Converter = (IValueConverter)obj2;
				context.PopParent();
				rectangle5.Bind(radiusXProperty, templateBinding4.ProvideValue());
				StyledProperty<double> radiusYProperty = Rectangle.RadiusYProperty;
				TemplateBinding templateBinding6 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding7 = templateBinding;
				StaticResourceExtension staticResourceExtension3 = new StaticResourceExtension("BottomRightCornerRadiusConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj3 = staticResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding7.Converter = (IValueConverter)obj3;
				context.PopParent();
				rectangle5.Bind(radiusYProperty, templateBinding6.ProvideValue());
				context.PopParent();
				((ISupportInitialize)rectangle4).EndInit();
				Controls children3 = panel6.Children;
				Rectangle rectangle6;
				Rectangle rectangle7 = (rectangle6 = new Rectangle());
				((ISupportInitialize)rectangle7).BeginInit();
				children3.Add(rectangle7);
				Rectangle rectangle8 = (rectangle3 = rectangle6);
				context.PushParent(rectangle3);
				Rectangle rectangle9 = rectangle3;
				rectangle9.Name = "PART_SpectrumOverlayRectangle";
				service = rectangle9;
				context.AvaloniaNameScope.Register("PART_SpectrumOverlayRectangle", service);
				rectangle9.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				rectangle9.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty2 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding8 = (templateBinding = new TemplateBinding(ColorSpectrum.ShapeProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding9 = templateBinding;
				StaticResourceExtension staticResourceExtension4 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj4 = staticResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding9.Converter = (IValueConverter)obj4;
				templateBinding9.ConverterParameter = ColorSpectrumShape.Box;
				context.PopParent();
				rectangle9.Bind(isVisibleProperty2, templateBinding8.ProvideValue());
				StyledProperty<double> radiusXProperty2 = Rectangle.RadiusXProperty;
				TemplateBinding templateBinding10 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding11 = templateBinding;
				StaticResourceExtension staticResourceExtension5 = new StaticResourceExtension("TopLeftCornerRadiusConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj5 = staticResourceExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding11.Converter = (IValueConverter)obj5;
				context.PopParent();
				rectangle9.Bind(radiusXProperty2, templateBinding10.ProvideValue());
				StyledProperty<double> radiusYProperty2 = Rectangle.RadiusYProperty;
				TemplateBinding templateBinding12 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding13 = templateBinding;
				StaticResourceExtension staticResourceExtension6 = new StaticResourceExtension("BottomRightCornerRadiusConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj6 = staticResourceExtension6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding13.Converter = (IValueConverter)obj6;
				context.PopParent();
				rectangle9.Bind(radiusYProperty2, templateBinding12.ProvideValue());
				context.PopParent();
				((ISupportInitialize)rectangle8).EndInit();
				Controls children4 = panel6.Children;
				Ellipse ellipse;
				Ellipse ellipse2 = (ellipse = new Ellipse());
				((ISupportInitialize)ellipse2).BeginInit();
				children4.Add(ellipse2);
				Ellipse ellipse3;
				Ellipse ellipse4 = (ellipse3 = ellipse);
				context.PushParent(ellipse3);
				Ellipse ellipse5 = ellipse3;
				ellipse5.Name = "PART_SpectrumEllipse";
				service = ellipse5;
				context.AvaloniaNameScope.Register("PART_SpectrumEllipse", service);
				ellipse5.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				ellipse5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				ellipse5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty3 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding14 = (templateBinding = new TemplateBinding(ColorSpectrum.ShapeProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding15 = templateBinding;
				StaticResourceExtension staticResourceExtension7 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj7 = staticResourceExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding15.Converter = (IValueConverter)obj7;
				templateBinding15.ConverterParameter = ColorSpectrumShape.Ring;
				context.PopParent();
				ellipse5.Bind(isVisibleProperty3, templateBinding14.ProvideValue());
				context.PopParent();
				((ISupportInitialize)ellipse4).EndInit();
				Controls children5 = panel6.Children;
				Ellipse ellipse6;
				Ellipse ellipse7 = (ellipse6 = new Ellipse());
				((ISupportInitialize)ellipse7).BeginInit();
				children5.Add(ellipse7);
				Ellipse ellipse8 = (ellipse3 = ellipse6);
				context.PushParent(ellipse3);
				Ellipse ellipse9 = ellipse3;
				ellipse9.Name = "PART_SpectrumOverlayEllipse";
				service = ellipse9;
				context.AvaloniaNameScope.Register("PART_SpectrumOverlayEllipse", service);
				ellipse9.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				ellipse9.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				ellipse9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty4 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding16 = (templateBinding = new TemplateBinding(ColorSpectrum.ShapeProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding17 = templateBinding;
				StaticResourceExtension staticResourceExtension8 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj8 = staticResourceExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding17.Converter = (IValueConverter)obj8;
				templateBinding17.ConverterParameter = ColorSpectrumShape.Ring;
				context.PopParent();
				ellipse9.Bind(isVisibleProperty4, templateBinding16.ProvideValue());
				context.PopParent();
				((ISupportInitialize)ellipse8).EndInit();
				Controls children6 = panel6.Children;
				Canvas canvas;
				Canvas canvas2 = (canvas = new Canvas());
				((ISupportInitialize)canvas2).BeginInit();
				children6.Add(canvas2);
				canvas.Name = "PART_InputTarget";
				service = canvas;
				context.AvaloniaNameScope.Register("PART_InputTarget", service);
				canvas.SetValue(Panel.BackgroundProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				canvas.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				canvas.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				Controls children7 = canvas.Children;
				Panel panel7;
				Panel panel8 = (panel7 = new Panel());
				((ISupportInitialize)panel8).BeginInit();
				children7.Add(panel8);
				panel7.Name = "PART_SelectionEllipsePanel";
				service = panel7;
				context.AvaloniaNameScope.Register("PART_SelectionEllipsePanel", service);
				panel7.SetValue(ToolTip.VerticalOffsetProperty, -10.0, BindingPriority.Template);
				panel7.SetValue(ToolTip.PlacementProperty, PlacementMode.Top, BindingPriority.Template);
				Controls children8 = panel7.Children;
				Ellipse ellipse10;
				Ellipse ellipse11 = (ellipse10 = new Ellipse());
				((ISupportInitialize)ellipse11).BeginInit();
				children8.Add(ellipse11);
				ellipse10.Name = "FocusEllipse";
				service = ellipse10;
				context.AvaloniaNameScope.Register("FocusEllipse", service);
				ellipse10.SetValue(Layoutable.MarginProperty, new Thickness(-2.0, -2.0, -2.0, -2.0), BindingPriority.Template);
				ellipse10.SetValue(Shape.StrokeThicknessProperty, 2.0, BindingPriority.Template);
				ellipse10.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				ellipse10.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				ellipse10.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				((ISupportInitialize)ellipse10).EndInit();
				Controls children9 = panel7.Children;
				Ellipse ellipse12;
				Ellipse ellipse13 = (ellipse12 = new Ellipse());
				((ISupportInitialize)ellipse13).BeginInit();
				children9.Add(ellipse13);
				ellipse12.Name = "SelectionEllipse";
				service = ellipse12;
				context.AvaloniaNameScope.Register("SelectionEllipse", service);
				ellipse12.SetValue(Shape.StrokeThicknessProperty, 2.0, BindingPriority.Template);
				ellipse12.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				ellipse12.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				ellipse12.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				((ISupportInitialize)ellipse12).EndInit();
				((ISupportInitialize)panel7).EndInit();
				((ISupportInitialize)canvas).EndInit();
				Controls children10 = panel6.Children;
				Rectangle rectangle10;
				Rectangle rectangle11 = (rectangle10 = new Rectangle());
				((ISupportInitialize)rectangle11).BeginInit();
				children10.Add(rectangle11);
				Rectangle rectangle12 = (rectangle3 = rectangle10);
				context.PushParent(rectangle3);
				Rectangle rectangle13 = rectangle3;
				rectangle13.Name = "BorderRectangle";
				service = rectangle13;
				context.AvaloniaNameScope.Register("BorderRectangle", service);
				rectangle13.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				rectangle13.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty5 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding18 = (templateBinding = new TemplateBinding(ColorSpectrum.ShapeProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding19 = templateBinding;
				StaticResourceExtension staticResourceExtension9 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj9 = staticResourceExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding19.Converter = (IValueConverter)obj9;
				templateBinding19.ConverterParameter = ColorSpectrumShape.Box;
				context.PopParent();
				rectangle13.Bind(isVisibleProperty5, templateBinding18.ProvideValue());
				StyledProperty<double> radiusXProperty3 = Rectangle.RadiusXProperty;
				TemplateBinding templateBinding20 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding21 = templateBinding;
				StaticResourceExtension staticResourceExtension10 = new StaticResourceExtension("TopLeftCornerRadiusConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj10 = staticResourceExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding21.Converter = (IValueConverter)obj10;
				context.PopParent();
				rectangle13.Bind(radiusXProperty3, templateBinding20.ProvideValue());
				StyledProperty<double> radiusYProperty3 = Rectangle.RadiusYProperty;
				TemplateBinding templateBinding22 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding23 = templateBinding;
				StaticResourceExtension staticResourceExtension11 = new StaticResourceExtension("BottomRightCornerRadiusConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj11 = staticResourceExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding23.Converter = (IValueConverter)obj11;
				context.PopParent();
				rectangle13.Bind(radiusYProperty3, templateBinding22.ProvideValue());
				context.PopParent();
				((ISupportInitialize)rectangle12).EndInit();
				Controls children11 = panel6.Children;
				Ellipse ellipse14;
				Ellipse ellipse15 = (ellipse14 = new Ellipse());
				((ISupportInitialize)ellipse15).BeginInit();
				children11.Add(ellipse15);
				Ellipse ellipse16 = (ellipse3 = ellipse14);
				context.PushParent(ellipse3);
				Ellipse ellipse17 = ellipse3;
				ellipse17.Name = "BorderEllipse";
				service = ellipse17;
				context.AvaloniaNameScope.Register("BorderEllipse", service);
				ellipse17.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				ellipse17.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				ellipse17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty6 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding24 = (templateBinding = new TemplateBinding(ColorSpectrum.ShapeProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding25 = templateBinding;
				StaticResourceExtension staticResourceExtension12 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj12 = staticResourceExtension12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding25.Converter = (IValueConverter)obj12;
				templateBinding25.ConverterParameter = ColorSpectrumShape.Ring;
				context.PopParent();
				ellipse17.Bind(isVisibleProperty6, templateBinding24.ProvideValue());
				context.PopParent();
				((ISupportInitialize)ellipse16).EndInit();
				context.PopParent();
				((ISupportInitialize)panel5).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
			controlTheme.TargetType = typeof(ColorSpectrum);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.TargetType = typeof(ColorSpectrum);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_2.Build, context);
			context.PopParent();
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Style style;
			Style style2 = (style = new Style());
			context.PushParent(style);
			Style style3 = style;
			style3.Selector = Selectors.Or(new List<Selector>
			{
				((Selector?)null).Nesting().Template().OfType(typeof(Ellipse))
					.Name("BorderEllipse"),
				((Selector?)null).Nesting().Template().OfType(typeof(Rectangle))
					.Name("BorderRectangle")
			});
			Setter setter4 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter5 = setter;
			setter5.Property = Shape.StrokeProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemControlForegroundListLowBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value2 = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter5.Value = value2;
			context.PopParent();
			style3.Add(setter4);
			Setter setter6 = new Setter();
			setter6.Property = Shape.StrokeThicknessProperty;
			setter6.Value = 1.0;
			style3.Add(setter6);
			context.PopParent();
			controlTheme.Add(style2);
			Style style4 = new Style();
			style4.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(Ellipse))
				.Name("FocusEllipse");
			Setter setter7 = new Setter();
			setter7.Property = Visual.IsVisibleProperty;
			setter7.Value = false;
			style4.Add(setter7);
			controlTheme.Add(style4);
			Style style5 = new Style();
			style5.Selector = ((Selector?)null).Nesting().Class(":focus-visible").Template()
				.OfType(typeof(Ellipse))
				.Name("FocusEllipse");
			Setter setter8 = new Setter();
			setter8.Property = Visual.IsVisibleProperty;
			setter8.Value = true;
			style5.Add(setter8);
			controlTheme.Add(style5);
			Style style6 = (style = new Style());
			context.PushParent(style);
			Style style7 = style;
			style7.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(Ellipse))
				.Name("FocusEllipse");
			Setter setter9 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter10 = setter;
			setter10.Property = Shape.StrokeProperty;
			DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ColorControlLightSelectorBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value3 = dynamicResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter10.Value = value3;
			context.PopParent();
			style7.Add(setter9);
			context.PopParent();
			controlTheme.Add(style6);
			Style style8 = (style = new Style());
			context.PushParent(style);
			Style style9 = style;
			style9.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(Ellipse))
				.Name("SelectionEllipse");
			Setter setter11 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter12 = setter;
			setter12.Property = Shape.StrokeProperty;
			DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ColorControlDarkSelectorBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value4 = dynamicResourceExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter12.Value = value4;
			context.PopParent();
			style9.Add(setter11);
			context.PopParent();
			controlTheme.Add(style8);
			Style style10 = (style = new Style());
			context.PushParent(style);
			Style style11 = style;
			style11.Selector = ((Selector?)null).Nesting().Class(":light-selector").Template()
				.OfType(typeof(Ellipse))
				.Name("FocusEllipse");
			Setter setter13 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter14 = setter;
			setter14.Property = Shape.StrokeProperty;
			DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("ColorControlDarkSelectorBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value5 = dynamicResourceExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter14.Value = value5;
			context.PopParent();
			style11.Add(setter13);
			context.PopParent();
			controlTheme.Add(style10);
			Style style12 = (style = new Style());
			context.PushParent(style);
			Style style13 = style;
			style13.Selector = ((Selector?)null).Nesting().Class(":light-selector").Template()
				.OfType(typeof(Ellipse))
				.Name("SelectionEllipse");
			Setter setter15 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter16 = setter;
			setter16.Property = Shape.StrokeProperty;
			DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("ColorControlLightSelectorBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value6 = dynamicResourceExtension5.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter16.Value = value6;
			context.PopParent();
			style13.Add(setter15);
			context.PopParent();
			controlTheme.Add(style12);
			Style style14 = new Style();
			style14.Selector = ((Selector?)null).Nesting().Class(":pointerover").Template()
				.OfType(typeof(Ellipse))
				.Name("SelectionEllipse");
			Setter setter17 = new Setter();
			setter17.Property = Visual.OpacityProperty;
			setter17.Value = 0.7;
			style14.Add(setter17);
			controlTheme.Add(style14);
			Style style15 = new Style();
			style15.Selector = ((Selector?)null).Nesting().Class(":pointerover").Class(":light-selector")
				.Template()
				.OfType(typeof(Ellipse))
				.Name("SelectionEllipse");
			Setter setter18 = new Setter();
			setter18.Property = Visual.OpacityProperty;
			setter18.Value = 0.8;
			style15.Add(setter18);
			controlTheme.Add(style15);
			Style style16 = new Style();
			style16.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(Panel))
				.Name("PART_SelectionEllipsePanel");
			Setter setter19 = new Setter();
			setter19.Property = Layoutable.WidthProperty;
			setter19.Value = 16.0;
			style16.Add(setter19);
			Setter setter20 = new Setter();
			setter20.Property = Layoutable.HeightProperty;
			setter20.Value = 16.0;
			style16.Add(setter20);
			controlTheme.Add(style16);
			Style style17 = new Style();
			style17.Selector = ((Selector?)null).Nesting().Class(":large-selector").Template()
				.OfType(typeof(Panel))
				.Name("PART_SelectionEllipsePanel");
			Setter setter21 = new Setter();
			setter21.Property = Layoutable.WidthProperty;
			setter21.Value = 48.0;
			style17.Add(setter21);
			Setter setter22 = new Setter();
			setter22.Property = Layoutable.HeightProperty;
			setter22.Value = 48.0;
			style17.Add(setter22);
			controlTheme.Add(style17);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_3
	{
		private class XamlClosure_4
		{
			private class DynamicSetters_5
			{
				public static void DynamicSetter_1(Border P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(Border.BackgroundProperty, binding);
						return;
					}
					if (P_2 is IBrush)
					{
						IBrush value = (IBrush)P_2;
						int priority = (int)P_1;
						P_0.SetValue(Border.BackgroundProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						IBrush value = (IBrush)P_2;
						int priority = (int)P_1;
						P_0.SetValue(Border.BackgroundProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
				}

				public static void DynamicSetter_2(StyledElement P_0, BindingPriority P_1, object P_2)
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

				public static void DynamicSetter_3(ItemsControl P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(ItemsControl.ItemContainerThemeProperty, binding);
						return;
					}
					if (P_2 is ControlTheme)
					{
						ControlTheme value = (ControlTheme)P_2;
						int priority = (int)P_1;
						P_0.SetValue(ItemsControl.ItemContainerThemeProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						ControlTheme value = (ControlTheme)P_2;
						int priority = (int)P_1;
						P_0.SetValue(ItemsControl.ItemContainerThemeProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
				}

				public static void DynamicSetter_4(SelectingItemsControl P_0, CompiledBindingExtension P_1)
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

				public static void DynamicSetter_5(NumericUpDown P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(NumericUpDown.NumberFormatProperty, binding);
						return;
					}
					if (P_2 is NumberFormatInfo)
					{
						NumberFormatInfo value = (NumberFormatInfo)P_2;
						int priority = (int)P_1;
						P_0.SetValue(NumericUpDown.NumberFormatProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						NumberFormatInfo value = (NumberFormatInfo)P_2;
						int priority = (int)P_1;
						P_0.SetValue(NumericUpDown.NumberFormatProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
				}
			}

			private class XamlClosure_6
			{
				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
					if (P_0 != null)
					{
						object service = P_0.GetService(typeof(IRootObjectProvider));
						if (service != null)
						{
							service = ((IRootObjectProvider)service).RootObject;
							context.RootObject = (Styles)service;
						}
					}
					context.IntermediateRoot = new UniformGrid();
					object intermediateRoot = context.IntermediateRoot;
					((ISupportInitialize)intermediateRoot).BeginInit();
					((AvaloniaObject)intermediateRoot).SetValue(UniformGrid.ColumnsProperty, 0, BindingPriority.Template);
					((AvaloniaObject)intermediateRoot).SetValue(UniformGrid.RowsProperty, 1, BindingPriority.Template);
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			private class XamlClosure_7
			{
				private class DynamicSetters_8
				{
					public static void DynamicSetter_1(ToolTip P_0, BindingPriority P_1, CompiledBindingExtension P_2)
					{
						if (P_2 != null)
						{
							IBinding binding = P_2;
							P_0.Bind(ToolTip.TipProperty, binding);
						}
						else
						{
							object value = P_2;
							int priority = (int)P_1;
							P_0.SetValue(ToolTip.TipProperty, value, (BindingPriority)priority);
						}
					}
				}

				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
					AttachedProperty<string?> nameProperty = AutomationProperties.NameProperty;
					CompiledBindingExtension compiledBindingExtension;
					CompiledBindingExtension compiledBindingExtension2 = (compiledBindingExtension = new CompiledBindingExtension());
					context.PushParent(compiledBindingExtension);
					CompiledBindingExtension compiledBindingExtension3 = compiledBindingExtension;
					StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ColorToDisplayNameConverter");
					context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
					object? obj = staticResourceExtension.ProvideValue(context);
					context.ProvideTargetProperty = null;
					compiledBindingExtension3.Converter = (IValueConverter)obj;
					context.PopParent();
					context.ProvideTargetProperty = AutomationProperties.NameProperty;
					CompiledBindingExtension binding = compiledBindingExtension2.ProvideValue(context);
					context.ProvideTargetProperty = null;
					border.Bind(nameProperty, binding);
					CompiledBindingExtension compiledBindingExtension4 = (compiledBindingExtension = new CompiledBindingExtension());
					context.PushParent(compiledBindingExtension);
					CompiledBindingExtension compiledBindingExtension5 = compiledBindingExtension;
					StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("ColorToDisplayNameConverter");
					context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
					object? obj2 = staticResourceExtension2.ProvideValue(context);
					context.ProvideTargetProperty = null;
					compiledBindingExtension5.Converter = (IValueConverter)obj2;
					context.PopParent();
					context.ProvideTargetProperty = ToolTip.TipProperty;
					CompiledBindingExtension compiledBindingExtension6 = compiledBindingExtension4.ProvideValue(context);
					context.ProvideTargetProperty = null;
					DynamicSetters_8.DynamicSetter_1((ToolTip)(object)border, BindingPriority.Template, compiledBindingExtension6);
					border.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
					border.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
					StyledProperty<IBrush?> backgroundProperty = Border.BackgroundProperty;
					SolidColorBrush solidColorBrush;
					SolidColorBrush value = (solidColorBrush = new SolidColorBrush());
					context.PushParent(solidColorBrush);
					StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
					CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension();
					context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
					CompiledBindingExtension binding2 = compiledBindingExtension7.ProvideValue(context);
					context.ProvideTargetProperty = null;
					solidColorBrush.Bind(colorProperty, binding2);
					context.PopParent();
					border.SetValue(backgroundProperty, value, BindingPriority.Template);
					context.PopParent();
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			private class XamlClosure_9
			{
				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
					if (P_0 != null)
					{
						object service = P_0.GetService(typeof(IRootObjectProvider));
						if (service != null)
						{
							service = ((IRootObjectProvider)service).RootObject;
							context.RootObject = (Styles)service;
						}
					}
					context.IntermediateRoot = new UniformGrid();
					object intermediateRoot = context.IntermediateRoot;
					((ISupportInitialize)intermediateRoot).BeginInit();
					UniformGrid uniformGrid = (UniformGrid)intermediateRoot;
					context.PushParent(uniformGrid);
					StyledProperty<int> columnsProperty = UniformGrid.ColumnsProperty;
					CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(ColorView), 0).Property(ColorView.PaletteColumnCountProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
					context.ProvideTargetProperty = UniformGrid.ColumnsProperty;
					CompiledBindingExtension binding = compiledBindingExtension.ProvideValue(context);
					context.ProvideTargetProperty = null;
					uniformGrid.Bind(columnsProperty, binding);
					context.PopParent();
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
				context.IntermediateRoot = new DropDownButton();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				DropDownButton dropDownButton = (DropDownButton)intermediateRoot;
				context.PushParent(dropDownButton);
				dropDownButton.Bind(TemplatedControl.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				dropDownButton.Bind(Layoutable.HeightProperty, new TemplateBinding(Layoutable.HeightProperty).ProvideValue());
				dropDownButton.Bind(Layoutable.WidthProperty, new TemplateBinding(Layoutable.WidthProperty).ProvideValue());
				dropDownButton.SetValue(ContentControl.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				dropDownButton.SetValue(ContentControl.VerticalContentAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				dropDownButton.SetValue(TemplatedControl.PaddingProperty, new Thickness(0.0, 0.0, 10.0, 0.0), BindingPriority.Template);
				dropDownButton.SetValue(Layoutable.UseLayoutRoundingProperty, value: false, BindingPriority.Template);
				Styles styles = dropDownButton.Styles;
				Style style = new Style();
				style.Selector = ((Selector?)null).OfType(typeof(FlyoutPresenter)).Class("nopadding");
				Setter setter = new Setter();
				setter.Property = TemplatedControl.PaddingProperty;
				setter.Value = new Thickness(0.0, 0.0, 0.0, 0.0);
				style.Add(setter);
				styles.Add(style);
				StyledProperty<object?> contentProperty = ContentControl.ContentProperty;
				Panel panel;
				Panel panel2 = (panel = new Panel());
				((ISupportInitialize)panel2).BeginInit();
				dropDownButton.SetValue(contentProperty, panel2, BindingPriority.Template);
				Panel panel3;
				Panel panel4 = (panel3 = panel);
				context.PushParent(panel3);
				Panel panel5 = panel3;
				Controls children = panel5.Children;
				Border border;
				Border border2 = (border = new Border());
				((ISupportInitialize)border2).BeginInit();
				children.Add(border2);
				Border border3;
				Border border4 = (border3 = border);
				context.PushParent(border3);
				Border border5 = border3;
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ColorControlCheckeredBackgroundBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				object? obj = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_5.DynamicSetter_1(border5, BindingPriority.Template, obj);
				StyledProperty<CornerRadius> cornerRadiusProperty = Border.CornerRadiusProperty;
				TemplateBinding templateBinding;
				TemplateBinding templateBinding2 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding3 = templateBinding;
				StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("LeftCornerRadiusFilterConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj2 = staticResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding3.Converter = (IValueConverter)obj2;
				context.PopParent();
				border5.Bind(cornerRadiusProperty, templateBinding2.ProvideValue());
				border5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				border5.SetValue(Layoutable.MarginProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)border4).EndInit();
				Controls children2 = panel5.Children;
				Border border6;
				Border border7 = (border6 = new Border());
				((ISupportInitialize)border7).BeginInit();
				children2.Add(border7);
				Border border8 = (border3 = border6);
				context.PushParent(border3);
				Border border9 = border3;
				StyledProperty<IBrush?> backgroundProperty = Border.BackgroundProperty;
				TemplateBinding templateBinding4 = (templateBinding = new TemplateBinding(ColorView.HsvColorProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding5 = templateBinding;
				StaticResourceExtension staticResourceExtension3 = new StaticResourceExtension("ToBrushConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj3 = staticResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding5.Converter = (IValueConverter)obj3;
				context.PopParent();
				border9.Bind(backgroundProperty, templateBinding4.ProvideValue());
				StyledProperty<CornerRadius> cornerRadiusProperty2 = Border.CornerRadiusProperty;
				TemplateBinding templateBinding6 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding7 = templateBinding;
				StaticResourceExtension staticResourceExtension4 = new StaticResourceExtension("LeftCornerRadiusFilterConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj4 = staticResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding7.Converter = (IValueConverter)obj4;
				context.PopParent();
				border9.Bind(cornerRadiusProperty2, templateBinding6.ProvideValue());
				border9.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				border9.SetValue(Layoutable.MarginProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)border8).EndInit();
				context.PopParent();
				((ISupportInitialize)panel4).EndInit();
				StyledProperty<FlyoutBase?> flyoutProperty = Button.FlyoutProperty;
				Flyout flyout;
				Flyout value = (flyout = new Flyout());
				context.PushParent(flyout);
				flyout.FlyoutPresenterClasses.Add("nopadding");
				flyout.SetValue(PopupFlyoutBase.PlacementProperty, PlacementMode.Top, BindingPriority.Template);
				Grid grid;
				Grid grid2 = (grid = new Grid());
				((ISupportInitialize)grid2).BeginInit();
				flyout.Content = grid2;
				Grid grid3;
				Grid grid4 = (grid3 = grid);
				context.PushParent(grid3);
				Grid grid5 = grid3;
				RowDefinitions rowDefinitions = new RowDefinitions();
				rowDefinitions.Capacity = 2;
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				grid5.RowDefinitions = rowDefinitions;
				grid5.Resources.Add("ColorViewTabBackgroundCornerRadius", new CornerRadius(5.0, 5.0, 0.0, 0.0));
				Controls children3 = grid5.Children;
				Border border10;
				Border border11 = (border10 = new Border());
				((ISupportInitialize)border11).BeginInit();
				children3.Add(border11);
				Border border12 = (border3 = border10);
				context.PushParent(border3);
				Border border13 = border3;
				border13.Name = "TabBackgroundBorder";
				service = border13;
				context.AvaloniaNameScope.Register("TabBackgroundBorder", service);
				border13.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				border13.SetValue(Grid.RowSpanProperty, 2, BindingPriority.Template);
				border13.SetValue(Layoutable.HeightProperty, 48.0, BindingPriority.Template);
				border13.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top, BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty2 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemControlBackgroundBaseLowBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border13.Bind(backgroundProperty2, binding);
				StyledProperty<IBrush?> borderBrushProperty = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ColorViewTabBorderBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding2 = dynamicResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border13.Bind(borderBrushProperty, binding2);
				StyledProperty<CornerRadius> cornerRadiusProperty3 = Border.CornerRadiusProperty;
				DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ColorViewTabBackgroundCornerRadius");
				context.ProvideTargetProperty = Border.CornerRadiusProperty;
				IBinding binding3 = dynamicResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border13.Bind(cornerRadiusProperty3, binding3);
				context.PopParent();
				((ISupportInitialize)border12).EndInit();
				Controls children4 = grid5.Children;
				Border border14;
				Border border15 = (border14 = new Border());
				((ISupportInitialize)border15).BeginInit();
				children4.Add(border15);
				Border border16 = (border3 = border14);
				context.PushParent(border3);
				Border border17 = border3;
				border17.Name = "ContentBackgroundBorder";
				service = border17;
				context.AvaloniaNameScope.Register("ContentBackgroundBorder", service);
				border17.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				border17.SetValue(Grid.RowSpanProperty, 2, BindingPriority.Template);
				border17.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 48.0, 0.0, 0.0), BindingPriority.Template);
				border17.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<CornerRadius> cornerRadiusProperty4 = Border.CornerRadiusProperty;
				TemplateBinding templateBinding8 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding9 = templateBinding;
				StaticResourceExtension staticResourceExtension5 = new StaticResourceExtension("BottomCornerRadiusFilterConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj5 = staticResourceExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding9.Converter = (IValueConverter)obj5;
				context.PopParent();
				border17.Bind(cornerRadiusProperty4, templateBinding8.ProvideValue());
				StyledProperty<IBrush?> backgroundProperty3 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("ColorViewContentBackgroundBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding4 = dynamicResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border17.Bind(backgroundProperty3, binding4);
				StyledProperty<IBrush?> borderBrushProperty2 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("ColorViewContentBorderBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding5 = dynamicResourceExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border17.Bind(borderBrushProperty2, binding5);
				border17.SetValue(Border.BorderThicknessProperty, new Thickness(0.0, 1.0, 0.0, 0.0), BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)border16).EndInit();
				Controls children5 = grid5.Children;
				TabControl tabControl;
				TabControl tabControl2 = (tabControl = new TabControl());
				((ISupportInitialize)tabControl2).BeginInit();
				children5.Add(tabControl2);
				TabControl tabControl3;
				TabControl tabControl4 = (tabControl3 = tabControl);
				context.PushParent(tabControl3);
				tabControl3.Name = "PART_TabControl";
				service = tabControl3;
				context.AvaloniaNameScope.Register("PART_TabControl", service);
				tabControl3.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				tabControl3.SetValue(Layoutable.HeightProperty, 338.0, BindingPriority.Template);
				tabControl3.SetValue(Layoutable.WidthProperty, 350.0, BindingPriority.Template);
				tabControl3.SetValue(TemplatedControl.PaddingProperty, new Thickness(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				DirectProperty<SelectingItemsControl, int> selectedIndexProperty = SelectingItemsControl.SelectedIndexProperty;
				CompiledBindingExtension obj6 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.SelectedIndexProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = SelectingItemsControl.SelectedIndexProperty;
				CompiledBindingExtension binding6 = obj6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				tabControl3.Bind(selectedIndexProperty, binding6);
				tabControl3.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate
				{
					Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_6.Build, context)
				}, BindingPriority.Template);
				ItemCollection items = tabControl3.Items;
				TabItem tabItem;
				TabItem tabItem2 = (tabItem = new TabItem());
				((ISupportInitialize)tabItem2).BeginInit();
				items.Add(tabItem2);
				TabItem tabItem3;
				TabItem tabItem4 = (tabItem3 = tabItem);
				context.PushParent(tabItem3);
				TabItem tabItem5 = tabItem3;
				StaticResourceExtension staticResourceExtension6 = new StaticResourceExtension("ColorViewTabItemTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj7 = staticResourceExtension6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_5.DynamicSetter_2(tabItem5, BindingPriority.Template, obj7);
				tabItem5.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorSpectrumVisibleProperty).ProvideValue());
				StyledProperty<object?> headerProperty = HeaderedContentControl.HeaderProperty;
				Border border18;
				Border border19 = (border18 = new Border());
				((ISupportInitialize)border19).BeginInit();
				tabItem5.SetValue(headerProperty, border19, BindingPriority.Template);
				Border border20 = (border3 = border18);
				context.PushParent(border3);
				Border border21 = border3;
				StyledProperty<double> heightProperty = Layoutable.HeightProperty;
				DynamicResourceExtension dynamicResourceExtension6 = new DynamicResourceExtension("ColorViewTabStripHeight");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				IBinding binding7 = dynamicResourceExtension6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border21.Bind(heightProperty, binding7);
				PathIcon pathIcon;
				PathIcon pathIcon2 = (pathIcon = new PathIcon());
				((ISupportInitialize)pathIcon2).BeginInit();
				border21.Child = pathIcon2;
				PathIcon pathIcon3;
				PathIcon pathIcon4 = (pathIcon3 = pathIcon);
				context.PushParent(pathIcon3);
				PathIcon pathIcon5 = pathIcon3;
				pathIcon5.SetValue(Layoutable.WidthProperty, 20.0, BindingPriority.Template);
				pathIcon5.SetValue(Layoutable.HeightProperty, 20.0, BindingPriority.Template);
				StyledProperty<Geometry> dataProperty = PathIcon.DataProperty;
				DynamicResourceExtension dynamicResourceExtension7 = new DynamicResourceExtension("ColorViewSpectrumIconGeometry");
				context.ProvideTargetProperty = PathIcon.DataProperty;
				IBinding binding8 = dynamicResourceExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				pathIcon5.Bind(dataProperty, binding8);
				context.PopParent();
				((ISupportInitialize)pathIcon4).EndInit();
				context.PopParent();
				((ISupportInitialize)border20).EndInit();
				Grid grid6;
				Grid grid7 = (grid6 = new Grid());
				((ISupportInitialize)grid7).BeginInit();
				tabItem5.Content = grid7;
				Grid grid8 = (grid3 = grid6);
				context.PushParent(grid3);
				Grid grid9 = grid3;
				RowDefinitions rowDefinitions2 = new RowDefinitions();
				rowDefinitions2.Capacity = 1;
				rowDefinitions2.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid9.RowDefinitions = rowDefinitions2;
				grid9.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 12.0, 12.0, 12.0), BindingPriority.Template);
				ColumnDefinitions columnDefinitions = grid9.ColumnDefinitions;
				ColumnDefinition columnDefinition = new ColumnDefinition();
				columnDefinition.SetValue(ColumnDefinition.WidthProperty, new GridLength(0.0, GridUnitType.Auto), BindingPriority.Template);
				columnDefinition.SetValue(ColumnDefinition.MinWidthProperty, 32.0, BindingPriority.Template);
				columnDefinitions.Add(columnDefinition);
				ColumnDefinitions columnDefinitions2 = grid9.ColumnDefinitions;
				ColumnDefinition columnDefinition2 = new ColumnDefinition();
				columnDefinition2.SetValue(ColumnDefinition.WidthProperty, new GridLength(1.0, GridUnitType.Star), BindingPriority.Template);
				columnDefinitions2.Add(columnDefinition2);
				ColumnDefinitions columnDefinitions3 = grid9.ColumnDefinitions;
				ColumnDefinition columnDefinition3 = new ColumnDefinition();
				columnDefinition3.SetValue(ColumnDefinition.WidthProperty, new GridLength(0.0, GridUnitType.Auto), BindingPriority.Template);
				columnDefinition3.SetValue(ColumnDefinition.MinWidthProperty, 32.0, BindingPriority.Template);
				columnDefinitions3.Add(columnDefinition3);
				Controls children6 = grid9.Children;
				ColorSlider colorSlider;
				ColorSlider colorSlider2 = (colorSlider = new ColorSlider());
				((ISupportInitialize)colorSlider2).BeginInit();
				children6.Add(colorSlider2);
				ColorSlider colorSlider3;
				ColorSlider colorSlider4 = (colorSlider3 = colorSlider);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider5 = colorSlider3;
				colorSlider5.Name = "ColorSpectrumThirdComponentSlider";
				service = colorSlider5;
				context.AvaloniaNameScope.Register("ColorSpectrumThirdComponentSlider", service);
				colorSlider5.SetValue(AutomationProperties.NameProperty, "Third Component", BindingPriority.Template);
				colorSlider5.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				colorSlider5.SetValue(ColorSlider.IsAlphaVisibleProperty, value: false, BindingPriority.Template);
				colorSlider5.SetValue(ColorSlider.IsPerceptiveProperty, value: true, BindingPriority.Template);
				colorSlider5.SetValue(Slider.OrientationProperty, Orientation.Vertical, BindingPriority.Template);
				colorSlider5.SetValue(ColorSlider.ColorModelProperty, ColorModel.Hsva, BindingPriority.Template);
				StyledProperty<ColorComponent> colorComponentProperty = ColorSlider.ColorComponentProperty;
				CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "ColorSpectrum").Property(ColorSpectrum.ThirdComponentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = ColorSlider.ColorComponentProperty;
				CompiledBindingExtension binding9 = compiledBindingExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider5.Bind(colorComponentProperty, binding9);
				StyledProperty<HsvColor> hsvColorProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension compiledBindingExtension2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "ColorSpectrum").Property(ColorSpectrum.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding10 = compiledBindingExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider5.Bind(hsvColorProperty, binding10);
				colorSlider5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				colorSlider5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				colorSlider5.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				colorSlider5.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorSpectrumSliderVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorSlider4).EndInit();
				Controls children7 = grid9.Children;
				ColorSpectrum colorSpectrum;
				ColorSpectrum colorSpectrum2 = (colorSpectrum = new ColorSpectrum());
				((ISupportInitialize)colorSpectrum2).BeginInit();
				children7.Add(colorSpectrum2);
				ColorSpectrum colorSpectrum3;
				ColorSpectrum colorSpectrum4 = (colorSpectrum3 = colorSpectrum);
				context.PushParent(colorSpectrum3);
				colorSpectrum3.Name = "ColorSpectrum";
				service = colorSpectrum3;
				context.AvaloniaNameScope.Register("ColorSpectrum", service);
				colorSpectrum3.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				colorSpectrum3.Bind(ColorSpectrum.ComponentsProperty, new TemplateBinding(ColorView.ColorSpectrumComponentsProperty).ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty2 = ColorSpectrum.HsvColorProperty;
				CompiledBindingExtension obj8 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSpectrum.HsvColorProperty;
				CompiledBindingExtension binding11 = obj8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSpectrum3.Bind(hsvColorProperty2, binding11);
				colorSpectrum3.Bind(ColorSpectrum.MinHueProperty, new TemplateBinding(ColorView.MinHueProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MaxHueProperty, new TemplateBinding(ColorView.MaxHueProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MinSaturationProperty, new TemplateBinding(ColorView.MinSaturationProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MaxSaturationProperty, new TemplateBinding(ColorView.MaxSaturationProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MinValueProperty, new TemplateBinding(ColorView.MinValueProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MaxValueProperty, new TemplateBinding(ColorView.MaxValueProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.ShapeProperty, new TemplateBinding(ColorView.ColorSpectrumShapeProperty).ProvideValue());
				colorSpectrum3.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSpectrum3.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)colorSpectrum4).EndInit();
				Controls children8 = grid9.Children;
				ColorSlider colorSlider6;
				ColorSlider colorSlider7 = (colorSlider6 = new ColorSlider());
				((ISupportInitialize)colorSlider7).BeginInit();
				children8.Add(colorSlider7);
				ColorSlider colorSlider8 = (colorSlider3 = colorSlider6);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider9 = colorSlider3;
				colorSlider9.Name = "ColorSpectrumAlphaSlider";
				service = colorSlider9;
				context.AvaloniaNameScope.Register("ColorSpectrumAlphaSlider", service);
				colorSlider9.SetValue(AutomationProperties.NameProperty, "Alpha Component", BindingPriority.Template);
				colorSlider9.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider9.SetValue(Slider.OrientationProperty, Orientation.Vertical, BindingPriority.Template);
				colorSlider9.SetValue(ColorSlider.ColorModelProperty, ColorModel.Hsva, BindingPriority.Template);
				colorSlider9.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Alpha, BindingPriority.Template);
				StyledProperty<HsvColor> hsvColorProperty3 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "ColorSpectrum").Property(ColorSpectrum.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding12 = compiledBindingExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider9.Bind(hsvColorProperty3, binding12);
				colorSlider9.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				colorSlider9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				colorSlider9.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				colorSlider9.Bind(InputElement.IsEnabledProperty, new TemplateBinding(ColorView.IsAlphaEnabledProperty).ProvideValue());
				StyledProperty<bool> isVisibleProperty = Visual.IsVisibleProperty;
				MultiBinding multiBinding;
				MultiBinding binding13 = (multiBinding = new MultiBinding());
				context.PushParent(multiBinding);
				MultiBinding multiBinding2 = multiBinding;
				multiBinding2.Converter = BoolConverters.And;
				IList<IBinding> bindings = multiBinding2.Bindings;
				CompiledBindingExtension obj9 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsAlphaVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item = obj9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings.Add(item);
				context.PopParent();
				colorSlider9.Bind(isVisibleProperty, binding13);
				context.PopParent();
				((ISupportInitialize)colorSlider8).EndInit();
				context.PopParent();
				((ISupportInitialize)grid8).EndInit();
				context.PopParent();
				((ISupportInitialize)tabItem4).EndInit();
				ItemCollection items2 = tabControl3.Items;
				TabItem tabItem6;
				TabItem tabItem7 = (tabItem6 = new TabItem());
				((ISupportInitialize)tabItem7).BeginInit();
				items2.Add(tabItem7);
				TabItem tabItem8 = (tabItem3 = tabItem6);
				context.PushParent(tabItem3);
				TabItem tabItem9 = tabItem3;
				StaticResourceExtension staticResourceExtension7 = new StaticResourceExtension("ColorViewTabItemTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj10 = staticResourceExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_5.DynamicSetter_2(tabItem9, BindingPriority.Template, obj10);
				tabItem9.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorPaletteVisibleProperty).ProvideValue());
				StyledProperty<object?> headerProperty2 = HeaderedContentControl.HeaderProperty;
				Border border22;
				Border border23 = (border22 = new Border());
				((ISupportInitialize)border23).BeginInit();
				tabItem9.SetValue(headerProperty2, border23, BindingPriority.Template);
				Border border24 = (border3 = border22);
				context.PushParent(border3);
				Border border25 = border3;
				StyledProperty<double> heightProperty2 = Layoutable.HeightProperty;
				DynamicResourceExtension dynamicResourceExtension8 = new DynamicResourceExtension("ColorViewTabStripHeight");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				IBinding binding14 = dynamicResourceExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border25.Bind(heightProperty2, binding14);
				PathIcon pathIcon6;
				PathIcon pathIcon7 = (pathIcon6 = new PathIcon());
				((ISupportInitialize)pathIcon7).BeginInit();
				border25.Child = pathIcon7;
				PathIcon pathIcon8 = (pathIcon3 = pathIcon6);
				context.PushParent(pathIcon3);
				PathIcon pathIcon9 = pathIcon3;
				pathIcon9.SetValue(Layoutable.WidthProperty, 20.0, BindingPriority.Template);
				pathIcon9.SetValue(Layoutable.HeightProperty, 20.0, BindingPriority.Template);
				StyledProperty<Geometry> dataProperty2 = PathIcon.DataProperty;
				DynamicResourceExtension dynamicResourceExtension9 = new DynamicResourceExtension("ColorViewPaletteIconGeometry");
				context.ProvideTargetProperty = PathIcon.DataProperty;
				IBinding binding15 = dynamicResourceExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				pathIcon9.Bind(dataProperty2, binding15);
				context.PopParent();
				((ISupportInitialize)pathIcon8).EndInit();
				context.PopParent();
				((ISupportInitialize)border24).EndInit();
				ListBox listBox;
				ListBox listBox2 = (listBox = new ListBox());
				((ISupportInitialize)listBox2).BeginInit();
				tabItem9.Content = listBox2;
				ListBox listBox3;
				ListBox listBox4 = (listBox3 = listBox);
				context.PushParent(listBox3);
				StaticResourceExtension staticResourceExtension8 = new StaticResourceExtension("ColorViewPaletteListBoxTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj11 = staticResourceExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_5.DynamicSetter_2(listBox3, BindingPriority.Template, obj11);
				StaticResourceExtension staticResourceExtension9 = new StaticResourceExtension("ColorViewPaletteListBoxItemTheme");
				context.ProvideTargetProperty = ItemsControl.ItemContainerThemeProperty;
				object? obj12 = staticResourceExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_5.DynamicSetter_3(listBox3, BindingPriority.Template, obj12);
				listBox3.Bind(ItemsControl.ItemsSourceProperty, new TemplateBinding(ColorView.PaletteColorsProperty).ProvideValue());
				CompiledBindingExtension compiledBindingExtension4;
				CompiledBindingExtension compiledBindingExtension5 = (compiledBindingExtension4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.ColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build()));
				context.PushParent(compiledBindingExtension4);
				StaticResourceExtension staticResourceExtension10 = new StaticResourceExtension("DoNothingForNullConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
				object? obj13 = staticResourceExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				compiledBindingExtension4.Converter = (IValueConverter)obj13;
				compiledBindingExtension4.Mode = BindingMode.TwoWay;
				context.PopParent();
				context.ProvideTargetProperty = SelectingItemsControl.SelectedItemProperty;
				CompiledBindingExtension compiledBindingExtension6 = compiledBindingExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_5.DynamicSetter_4(listBox3, compiledBindingExtension6);
				listBox3.SetValue(Layoutable.UseLayoutRoundingProperty, value: false, BindingPriority.Template);
				listBox3.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 12.0, 12.0, 12.0), BindingPriority.Template);
				StyledProperty<IDataTemplate?> itemTemplateProperty = ItemsControl.ItemTemplateProperty;
				DataTemplate dataTemplate;
				DataTemplate value2 = (dataTemplate = new DataTemplate());
				context.PushParent(dataTemplate);
				dataTemplate.DataType = typeof(Color);
				dataTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_7.Build, context);
				context.PopParent();
				listBox3.SetValue(itemTemplateProperty, value2, BindingPriority.Template);
				StyledProperty<ITemplate<Panel?>> itemsPanelProperty = ItemsControl.ItemsPanelProperty;
				ItemsPanelTemplate itemsPanelTemplate;
				ItemsPanelTemplate value3 = (itemsPanelTemplate = new ItemsPanelTemplate());
				context.PushParent(itemsPanelTemplate);
				itemsPanelTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_9.Build, context);
				context.PopParent();
				listBox3.SetValue(itemsPanelProperty, value3, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)listBox4).EndInit();
				context.PopParent();
				((ISupportInitialize)tabItem8).EndInit();
				ItemCollection items3 = tabControl3.Items;
				TabItem tabItem10;
				TabItem tabItem11 = (tabItem10 = new TabItem());
				((ISupportInitialize)tabItem11).BeginInit();
				items3.Add(tabItem11);
				TabItem tabItem12 = (tabItem3 = tabItem10);
				context.PushParent(tabItem3);
				TabItem tabItem13 = tabItem3;
				StaticResourceExtension staticResourceExtension11 = new StaticResourceExtension("ColorViewTabItemTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj14 = staticResourceExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_5.DynamicSetter_2(tabItem13, BindingPriority.Template, obj14);
				tabItem13.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorComponentsVisibleProperty).ProvideValue());
				StyledProperty<object?> headerProperty3 = HeaderedContentControl.HeaderProperty;
				Border border26;
				Border border27 = (border26 = new Border());
				((ISupportInitialize)border27).BeginInit();
				tabItem13.SetValue(headerProperty3, border27, BindingPriority.Template);
				Border border28 = (border3 = border26);
				context.PushParent(border3);
				Border border29 = border3;
				StyledProperty<double> heightProperty3 = Layoutable.HeightProperty;
				DynamicResourceExtension dynamicResourceExtension10 = new DynamicResourceExtension("ColorViewTabStripHeight");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				IBinding binding16 = dynamicResourceExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border29.Bind(heightProperty3, binding16);
				PathIcon pathIcon10;
				PathIcon pathIcon11 = (pathIcon10 = new PathIcon());
				((ISupportInitialize)pathIcon11).BeginInit();
				border29.Child = pathIcon11;
				PathIcon pathIcon12 = (pathIcon3 = pathIcon10);
				context.PushParent(pathIcon3);
				PathIcon pathIcon13 = pathIcon3;
				pathIcon13.SetValue(Layoutable.WidthProperty, 20.0, BindingPriority.Template);
				pathIcon13.SetValue(Layoutable.HeightProperty, 20.0, BindingPriority.Template);
				StyledProperty<Geometry> dataProperty3 = PathIcon.DataProperty;
				DynamicResourceExtension dynamicResourceExtension11 = new DynamicResourceExtension("ColorViewComponentsIconGeometry");
				context.ProvideTargetProperty = PathIcon.DataProperty;
				IBinding binding17 = dynamicResourceExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				pathIcon13.Bind(dataProperty3, binding17);
				context.PopParent();
				((ISupportInitialize)pathIcon12).EndInit();
				context.PopParent();
				((ISupportInitialize)border28).EndInit();
				Grid grid10;
				Grid grid11 = (grid10 = new Grid());
				((ISupportInitialize)grid11).BeginInit();
				tabItem13.Content = grid11;
				Grid grid12 = (grid3 = grid10);
				context.PushParent(grid3);
				Grid grid13 = grid3;
				ColumnDefinitions columnDefinitions4 = new ColumnDefinitions();
				columnDefinitions4.Capacity = 3;
				columnDefinitions4.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions4.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions4.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid13.ColumnDefinitions = columnDefinitions4;
				RowDefinitions rowDefinitions3 = new RowDefinitions();
				rowDefinitions3.Capacity = 7;
				rowDefinitions3.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(24.0, GridUnitType.Pixel)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(12.0, GridUnitType.Pixel)));
				grid13.RowDefinitions = rowDefinitions3;
				grid13.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 12.0, 12.0, 12.0), BindingPriority.Template);
				Controls children9 = grid13.Children;
				Grid grid14;
				Grid grid15 = (grid14 = new Grid());
				((ISupportInitialize)grid15).BeginInit();
				children9.Add(grid15);
				Grid grid16 = (grid3 = grid14);
				context.PushParent(grid3);
				Grid grid17 = grid3;
				grid17.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				grid17.SetValue(Grid.ColumnSpanProperty, 3, BindingPriority.Template);
				grid17.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				ColumnDefinitions columnDefinitions5 = new ColumnDefinitions();
				columnDefinitions5.Capacity = 3;
				columnDefinitions5.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions5.Add(new ColumnDefinition(new GridLength(12.0, GridUnitType.Pixel)));
				columnDefinitions5.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid17.ColumnDefinitions = columnDefinitions5;
				Controls children10 = grid17.Children;
				Grid grid18;
				Grid grid19 = (grid18 = new Grid());
				((ISupportInitialize)grid19).BeginInit();
				children10.Add(grid19);
				Grid grid20 = (grid3 = grid18);
				context.PushParent(grid3);
				Grid grid21 = grid3;
				ColumnDefinitions columnDefinitions6 = new ColumnDefinitions();
				columnDefinitions6.Capacity = 2;
				columnDefinitions6.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions6.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid21.ColumnDefinitions = columnDefinitions6;
				grid21.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorModelVisibleProperty).ProvideValue());
				Controls children11 = grid21.Children;
				RadioButton radioButton;
				RadioButton radioButton2 = (radioButton = new RadioButton());
				((ISupportInitialize)radioButton2).BeginInit();
				children11.Add(radioButton2);
				RadioButton radioButton3;
				RadioButton radioButton4 = (radioButton3 = radioButton);
				context.PushParent(radioButton3);
				RadioButton radioButton5 = radioButton3;
				radioButton5.Name = "RgbRadioButton";
				service = radioButton5;
				context.AvaloniaNameScope.Register("RgbRadioButton", service);
				StaticResourceExtension staticResourceExtension12 = new StaticResourceExtension("ColorViewColorModelRadioButtonTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj15 = staticResourceExtension12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_5.DynamicSetter_2(radioButton5, BindingPriority.Template, obj15);
				radioButton5.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				radioButton5.SetValue(ContentControl.ContentProperty, "RGB", BindingPriority.Template);
				radioButton5.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(4.0, 0.0, 0.0, 4.0), BindingPriority.Template);
				radioButton5.SetValue(TemplatedControl.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				StyledProperty<double> heightProperty4 = Layoutable.HeightProperty;
				CompiledBindingExtension obj16 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "PART_HexTextBox").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding18 = obj16.ProvideValue(context);
				context.ProvideTargetProperty = null;
				radioButton5.Bind(heightProperty4, binding18);
				StyledProperty<bool?> isCheckedProperty = ToggleButton.IsCheckedProperty;
				TemplateBinding templateBinding10 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding11 = templateBinding;
				StaticResourceExtension staticResourceExtension13 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj17 = staticResourceExtension13.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding11.Converter = (IValueConverter)obj17;
				templateBinding11.ConverterParameter = ColorModel.Rgba;
				templateBinding11.Mode = BindingMode.TwoWay;
				context.PopParent();
				radioButton5.Bind(isCheckedProperty, templateBinding10.ProvideValue());
				context.PopParent();
				((ISupportInitialize)radioButton4).EndInit();
				Controls children12 = grid21.Children;
				RadioButton radioButton6;
				RadioButton radioButton7 = (radioButton6 = new RadioButton());
				((ISupportInitialize)radioButton7).BeginInit();
				children12.Add(radioButton7);
				RadioButton radioButton8 = (radioButton3 = radioButton6);
				context.PushParent(radioButton3);
				RadioButton radioButton9 = radioButton3;
				radioButton9.Name = "HsvRadioButton";
				service = radioButton9;
				context.AvaloniaNameScope.Register("HsvRadioButton", service);
				StaticResourceExtension staticResourceExtension14 = new StaticResourceExtension("ColorViewColorModelRadioButtonTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj18 = staticResourceExtension14.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_5.DynamicSetter_2(radioButton9, BindingPriority.Template, obj18);
				radioButton9.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				radioButton9.SetValue(ContentControl.ContentProperty, "HSV", BindingPriority.Template);
				radioButton9.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 4.0, 4.0, 0.0), BindingPriority.Template);
				radioButton9.SetValue(TemplatedControl.BorderThicknessProperty, new Thickness(0.0, 1.0, 1.0, 1.0), BindingPriority.Template);
				StyledProperty<double> heightProperty5 = Layoutable.HeightProperty;
				CompiledBindingExtension obj19 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "PART_HexTextBox").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding19 = obj19.ProvideValue(context);
				context.ProvideTargetProperty = null;
				radioButton9.Bind(heightProperty5, binding19);
				StyledProperty<bool?> isCheckedProperty2 = ToggleButton.IsCheckedProperty;
				TemplateBinding templateBinding12 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding13 = templateBinding;
				StaticResourceExtension staticResourceExtension15 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj20 = staticResourceExtension15.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding13.Converter = (IValueConverter)obj20;
				templateBinding13.ConverterParameter = ColorModel.Hsva;
				templateBinding13.Mode = BindingMode.TwoWay;
				context.PopParent();
				radioButton9.Bind(isCheckedProperty2, templateBinding12.ProvideValue());
				context.PopParent();
				((ISupportInitialize)radioButton8).EndInit();
				context.PopParent();
				((ISupportInitialize)grid20).EndInit();
				Controls children13 = grid17.Children;
				Grid grid22;
				Grid grid23 = (grid22 = new Grid());
				((ISupportInitialize)grid23).BeginInit();
				children13.Add(grid23);
				Grid grid24 = (grid3 = grid22);
				context.PushParent(grid3);
				Grid grid25 = grid3;
				grid25.Name = "HexInputGrid";
				service = grid25;
				context.AvaloniaNameScope.Register("HexInputGrid", service);
				grid25.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				grid25.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsHexInputVisibleProperty).ProvideValue());
				ColumnDefinitions columnDefinitions7 = grid25.ColumnDefinitions;
				ColumnDefinition columnDefinition4 = new ColumnDefinition();
				columnDefinition4.SetValue(ColumnDefinition.WidthProperty, new GridLength(30.0, GridUnitType.Pixel), BindingPriority.Template);
				columnDefinitions7.Add(columnDefinition4);
				ColumnDefinitions columnDefinitions8 = grid25.ColumnDefinitions;
				ColumnDefinition columnDefinition5 = new ColumnDefinition();
				columnDefinition5.SetValue(ColumnDefinition.WidthProperty, new GridLength(1.0, GridUnitType.Star), BindingPriority.Template);
				columnDefinitions8.Add(columnDefinition5);
				Controls children14 = grid25.Children;
				Border border30;
				Border border31 = (border30 = new Border());
				((ISupportInitialize)border31).BeginInit();
				children14.Add(border31);
				Border border32 = (border3 = border30);
				context.PushParent(border3);
				Border border33 = border3;
				border33.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border33.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty4 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension12 = new DynamicResourceExtension("TextControlBackgroundDisabled");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding20 = dynamicResourceExtension12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border33.Bind(backgroundProperty4, binding20);
				StyledProperty<IBrush?> borderBrushProperty3 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension13 = new DynamicResourceExtension("TextControlBorderBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding21 = dynamicResourceExtension13.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border33.Bind(borderBrushProperty3, binding21);
				border33.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border33.SetValue(Border.CornerRadiusProperty, new CornerRadius(4.0, 0.0, 0.0, 4.0), BindingPriority.Template);
				TextBlock textBlock;
				TextBlock textBlock2 = (textBlock = new TextBlock());
				((ISupportInitialize)textBlock2).BeginInit();
				border33.Child = textBlock2;
				TextBlock textBlock3;
				TextBlock textBlock4 = (textBlock3 = textBlock);
				context.PushParent(textBlock3);
				TextBlock textBlock5 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension14 = new DynamicResourceExtension("TextControlForegroundDisabled");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding22 = dynamicResourceExtension14.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock5.Bind(foregroundProperty, binding22);
				textBlock5.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock5.SetValue(TextBlock.TextProperty, "#", BindingPriority.Template);
				textBlock5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				textBlock5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)textBlock4).EndInit();
				context.PopParent();
				((ISupportInitialize)border32).EndInit();
				Controls children15 = grid25.Children;
				TextBox textBox;
				TextBox textBox2 = (textBox = new TextBox());
				((ISupportInitialize)textBox2).BeginInit();
				children15.Add(textBox2);
				textBox.Name = "PART_HexTextBox";
				service = textBox;
				context.AvaloniaNameScope.Register("PART_HexTextBox", service);
				textBox.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				textBox.SetValue(AutomationProperties.NameProperty, "Hexadecimal Color", BindingPriority.Template);
				textBox.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				textBox.SetValue(TextBox.MaxLengthProperty, 9, BindingPriority.Template);
				textBox.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				textBox.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 4.0, 4.0, 0.0), BindingPriority.Template);
				((ISupportInitialize)textBox).EndInit();
				context.PopParent();
				((ISupportInitialize)grid24).EndInit();
				context.PopParent();
				((ISupportInitialize)grid16).EndInit();
				Controls children16 = grid13.Children;
				Border border34;
				Border border35 = (border34 = new Border());
				((ISupportInitialize)border35).BeginInit();
				children16.Add(border35);
				Border border36 = (border3 = border34);
				context.PushParent(border3);
				Border border37 = border3;
				border37.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border37.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				StyledProperty<double> heightProperty6 = Layoutable.HeightProperty;
				CompiledBindingExtension obj21 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component1NumericUpDown").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding23 = obj21.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border37.Bind(heightProperty6, binding23);
				StyledProperty<double> widthProperty = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension15 = new DynamicResourceExtension("ColorViewComponentLabelWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding24 = dynamicResourceExtension15.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border37.Bind(widthProperty, binding24);
				StyledProperty<IBrush?> backgroundProperty5 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension16 = new DynamicResourceExtension("TextControlBackgroundDisabled");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding25 = dynamicResourceExtension16.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border37.Bind(backgroundProperty5, binding25);
				StyledProperty<IBrush?> borderBrushProperty4 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension17 = new DynamicResourceExtension("TextControlBorderBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding26 = dynamicResourceExtension17.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border37.Bind(borderBrushProperty4, binding26);
				border37.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border37.SetValue(Border.CornerRadiusProperty, new CornerRadius(4.0, 0.0, 0.0, 4.0), BindingPriority.Template);
				border37.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				border37.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				Panel panel6;
				Panel panel7 = (panel6 = new Panel());
				((ISupportInitialize)panel7).BeginInit();
				border37.Child = panel7;
				Panel panel8 = (panel3 = panel6);
				context.PushParent(panel3);
				Panel panel9 = panel3;
				panel9.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				panel9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Controls children17 = panel9.Children;
				TextBlock textBlock6;
				TextBlock textBlock7 = (textBlock6 = new TextBlock());
				((ISupportInitialize)textBlock7).BeginInit();
				children17.Add(textBlock7);
				TextBlock textBlock8 = (textBlock3 = textBlock6);
				context.PushParent(textBlock3);
				TextBlock textBlock9 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty2 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension18 = new DynamicResourceExtension("TextControlForegroundDisabled");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding27 = dynamicResourceExtension18.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock9.Bind(foregroundProperty2, binding27);
				textBlock9.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock9.SetValue(TextBlock.TextProperty, "R", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty2 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding14 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding15 = templateBinding;
				StaticResourceExtension staticResourceExtension16 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj22 = staticResourceExtension16.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding15.Converter = (IValueConverter)obj22;
				templateBinding15.ConverterParameter = ColorModel.Rgba;
				templateBinding15.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock9.Bind(isVisibleProperty2, templateBinding14.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock8).EndInit();
				Controls children18 = panel9.Children;
				TextBlock textBlock10;
				TextBlock textBlock11 = (textBlock10 = new TextBlock());
				((ISupportInitialize)textBlock11).BeginInit();
				children18.Add(textBlock11);
				TextBlock textBlock12 = (textBlock3 = textBlock10);
				context.PushParent(textBlock3);
				TextBlock textBlock13 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty3 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension19 = new DynamicResourceExtension("TextControlForegroundDisabled");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding28 = dynamicResourceExtension19.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock13.Bind(foregroundProperty3, binding28);
				textBlock13.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock13.SetValue(TextBlock.TextProperty, "H", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty3 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding16 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding17 = templateBinding;
				StaticResourceExtension staticResourceExtension17 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj23 = staticResourceExtension17.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding17.Converter = (IValueConverter)obj23;
				templateBinding17.ConverterParameter = ColorModel.Hsva;
				templateBinding17.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock13.Bind(isVisibleProperty3, templateBinding16.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock12).EndInit();
				context.PopParent();
				((ISupportInitialize)panel8).EndInit();
				context.PopParent();
				((ISupportInitialize)border36).EndInit();
				Controls children19 = grid13.Children;
				NumericUpDown numericUpDown;
				NumericUpDown numericUpDown2 = (numericUpDown = new NumericUpDown());
				((ISupportInitialize)numericUpDown2).BeginInit();
				children19.Add(numericUpDown2);
				NumericUpDown numericUpDown3;
				NumericUpDown numericUpDown4 = (numericUpDown3 = numericUpDown);
				context.PushParent(numericUpDown3);
				NumericUpDown numericUpDown5 = numericUpDown3;
				numericUpDown5.Name = "Component1NumericUpDown";
				service = numericUpDown5;
				context.AvaloniaNameScope.Register("Component1NumericUpDown", service);
				numericUpDown5.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				numericUpDown5.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				numericUpDown5.SetValue(NumericUpDown.AllowSpinProperty, value: true, BindingPriority.Template);
				numericUpDown5.SetValue(NumericUpDown.ShowButtonSpinnerProperty, value: false, BindingPriority.Template);
				numericUpDown5.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<double> widthProperty2 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension20 = new DynamicResourceExtension("ColorViewComponentTextInputWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding29 = dynamicResourceExtension20.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown5.Bind(widthProperty2, binding29);
				numericUpDown5.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 4.0, 4.0, 0.0), BindingPriority.Template);
				numericUpDown5.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				numericUpDown5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension18 = new StaticResourceExtension("ColorViewComponentNumberFormat");
				context.ProvideTargetProperty = NumericUpDown.NumberFormatProperty;
				object? obj24 = staticResourceExtension18.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_5.DynamicSetter_5(numericUpDown5, BindingPriority.Template, obj24);
				StyledProperty<decimal> minimumProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component1Slider").Property(RangeBase.MinimumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension binding30 = compiledBindingExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown5.Bind(minimumProperty, binding30);
				StyledProperty<decimal> maximumProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension compiledBindingExtension8 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component1Slider").Property(RangeBase.MaximumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension binding31 = compiledBindingExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown5.Bind(maximumProperty, binding31);
				StyledProperty<decimal?> valueProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension compiledBindingExtension9 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component1Slider").Property(RangeBase.ValueProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension binding32 = compiledBindingExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown5.Bind(valueProperty, binding32);
				numericUpDown5.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)numericUpDown4).EndInit();
				Controls children20 = grid13.Children;
				ColorSlider colorSlider10;
				ColorSlider colorSlider11 = (colorSlider10 = new ColorSlider());
				((ISupportInitialize)colorSlider11).BeginInit();
				children20.Add(colorSlider11);
				ColorSlider colorSlider12 = (colorSlider3 = colorSlider10);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider13 = colorSlider3;
				colorSlider13.Name = "Component1Slider";
				service = colorSlider13;
				context.AvaloniaNameScope.Register("Component1Slider", service);
				colorSlider13.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider13.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				colorSlider13.SetValue(Slider.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				colorSlider13.SetValue(ColorSlider.IsRoundingEnabledProperty, value: true, BindingPriority.Template);
				colorSlider13.SetValue(Slider.IsSnapToTickEnabledProperty, value: true, BindingPriority.Template);
				colorSlider13.SetValue(Slider.TickFrequencyProperty, 1.0, BindingPriority.Template);
				colorSlider13.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Component1, BindingPriority.Template);
				colorSlider13.Bind(ColorSlider.ColorModelProperty, new TemplateBinding(ColorView.ColorModelProperty)
				{
					Mode = BindingMode.OneWay
				}.ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty4 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension obj25 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding33 = obj25.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider13.Bind(hsvColorProperty4, binding33);
				colorSlider13.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSlider13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				colorSlider13.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentSliderVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorSlider12).EndInit();
				Controls children21 = grid13.Children;
				Border border38;
				Border border39 = (border38 = new Border());
				((ISupportInitialize)border39).BeginInit();
				children21.Add(border39);
				Border border40 = (border3 = border38);
				context.PushParent(border3);
				Border border41 = border3;
				border41.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border41.SetValue(Grid.RowProperty, 3, BindingPriority.Template);
				StyledProperty<double> widthProperty3 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension21 = new DynamicResourceExtension("ColorViewComponentLabelWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding34 = dynamicResourceExtension21.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border41.Bind(widthProperty3, binding34);
				StyledProperty<double> heightProperty7 = Layoutable.HeightProperty;
				CompiledBindingExtension obj26 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component2NumericUpDown").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding35 = obj26.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border41.Bind(heightProperty7, binding35);
				StyledProperty<IBrush?> backgroundProperty6 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension22 = new DynamicResourceExtension("TextControlBackgroundDisabled");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding36 = dynamicResourceExtension22.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border41.Bind(backgroundProperty6, binding36);
				StyledProperty<IBrush?> borderBrushProperty5 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension23 = new DynamicResourceExtension("TextControlBorderBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding37 = dynamicResourceExtension23.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border41.Bind(borderBrushProperty5, binding37);
				border41.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border41.SetValue(Border.CornerRadiusProperty, new CornerRadius(4.0, 0.0, 0.0, 4.0), BindingPriority.Template);
				border41.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				border41.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				Panel panel10;
				Panel panel11 = (panel10 = new Panel());
				((ISupportInitialize)panel11).BeginInit();
				border41.Child = panel11;
				Panel panel12 = (panel3 = panel10);
				context.PushParent(panel3);
				Panel panel13 = panel3;
				panel13.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				panel13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Controls children22 = panel13.Children;
				TextBlock textBlock14;
				TextBlock textBlock15 = (textBlock14 = new TextBlock());
				((ISupportInitialize)textBlock15).BeginInit();
				children22.Add(textBlock15);
				TextBlock textBlock16 = (textBlock3 = textBlock14);
				context.PushParent(textBlock3);
				TextBlock textBlock17 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty4 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension24 = new DynamicResourceExtension("TextControlForegroundDisabled");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding38 = dynamicResourceExtension24.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock17.Bind(foregroundProperty4, binding38);
				textBlock17.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock17.SetValue(TextBlock.TextProperty, "G", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty4 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding18 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding19 = templateBinding;
				StaticResourceExtension staticResourceExtension19 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj27 = staticResourceExtension19.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding19.Converter = (IValueConverter)obj27;
				templateBinding19.ConverterParameter = ColorModel.Rgba;
				templateBinding19.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock17.Bind(isVisibleProperty4, templateBinding18.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock16).EndInit();
				Controls children23 = panel13.Children;
				TextBlock textBlock18;
				TextBlock textBlock19 = (textBlock18 = new TextBlock());
				((ISupportInitialize)textBlock19).BeginInit();
				children23.Add(textBlock19);
				TextBlock textBlock20 = (textBlock3 = textBlock18);
				context.PushParent(textBlock3);
				TextBlock textBlock21 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty5 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension25 = new DynamicResourceExtension("TextControlForegroundDisabled");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding39 = dynamicResourceExtension25.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock21.Bind(foregroundProperty5, binding39);
				textBlock21.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock21.SetValue(TextBlock.TextProperty, "S", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty5 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding20 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding21 = templateBinding;
				StaticResourceExtension staticResourceExtension20 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj28 = staticResourceExtension20.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding21.Converter = (IValueConverter)obj28;
				templateBinding21.ConverterParameter = ColorModel.Hsva;
				templateBinding21.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock21.Bind(isVisibleProperty5, templateBinding20.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock20).EndInit();
				context.PopParent();
				((ISupportInitialize)panel12).EndInit();
				context.PopParent();
				((ISupportInitialize)border40).EndInit();
				Controls children24 = grid13.Children;
				NumericUpDown numericUpDown6;
				NumericUpDown numericUpDown7 = (numericUpDown6 = new NumericUpDown());
				((ISupportInitialize)numericUpDown7).BeginInit();
				children24.Add(numericUpDown7);
				NumericUpDown numericUpDown8 = (numericUpDown3 = numericUpDown6);
				context.PushParent(numericUpDown3);
				NumericUpDown numericUpDown9 = numericUpDown3;
				numericUpDown9.Name = "Component2NumericUpDown";
				service = numericUpDown9;
				context.AvaloniaNameScope.Register("Component2NumericUpDown", service);
				numericUpDown9.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				numericUpDown9.SetValue(Grid.RowProperty, 3, BindingPriority.Template);
				numericUpDown9.SetValue(NumericUpDown.AllowSpinProperty, value: true, BindingPriority.Template);
				numericUpDown9.SetValue(NumericUpDown.ShowButtonSpinnerProperty, value: false, BindingPriority.Template);
				numericUpDown9.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<double> widthProperty4 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension26 = new DynamicResourceExtension("ColorViewComponentTextInputWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding40 = dynamicResourceExtension26.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown9.Bind(widthProperty4, binding40);
				numericUpDown9.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 4.0, 4.0, 0.0), BindingPriority.Template);
				numericUpDown9.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				numericUpDown9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension21 = new StaticResourceExtension("ColorViewComponentNumberFormat");
				context.ProvideTargetProperty = NumericUpDown.NumberFormatProperty;
				object? obj29 = staticResourceExtension21.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_5.DynamicSetter_5(numericUpDown9, BindingPriority.Template, obj29);
				StyledProperty<decimal> minimumProperty2 = NumericUpDown.MinimumProperty;
				CompiledBindingExtension compiledBindingExtension10 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component2Slider").Property(RangeBase.MinimumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension binding41 = compiledBindingExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown9.Bind(minimumProperty2, binding41);
				StyledProperty<decimal> maximumProperty2 = NumericUpDown.MaximumProperty;
				CompiledBindingExtension compiledBindingExtension11 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component2Slider").Property(RangeBase.MaximumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension binding42 = compiledBindingExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown9.Bind(maximumProperty2, binding42);
				StyledProperty<decimal?> valueProperty2 = NumericUpDown.ValueProperty;
				CompiledBindingExtension compiledBindingExtension12 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component2Slider").Property(RangeBase.ValueProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension binding43 = compiledBindingExtension12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown9.Bind(valueProperty2, binding43);
				numericUpDown9.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)numericUpDown8).EndInit();
				Controls children25 = grid13.Children;
				ColorSlider colorSlider14;
				ColorSlider colorSlider15 = (colorSlider14 = new ColorSlider());
				((ISupportInitialize)colorSlider15).BeginInit();
				children25.Add(colorSlider15);
				ColorSlider colorSlider16 = (colorSlider3 = colorSlider14);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider17 = colorSlider3;
				colorSlider17.Name = "Component2Slider";
				service = colorSlider17;
				context.AvaloniaNameScope.Register("Component2Slider", service);
				colorSlider17.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider17.SetValue(Grid.RowProperty, 3, BindingPriority.Template);
				colorSlider17.SetValue(Slider.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				colorSlider17.SetValue(ColorSlider.IsRoundingEnabledProperty, value: true, BindingPriority.Template);
				colorSlider17.SetValue(Slider.IsSnapToTickEnabledProperty, value: true, BindingPriority.Template);
				colorSlider17.SetValue(Slider.TickFrequencyProperty, 1.0, BindingPriority.Template);
				colorSlider17.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Component2, BindingPriority.Template);
				colorSlider17.Bind(ColorSlider.ColorModelProperty, new TemplateBinding(ColorView.ColorModelProperty)
				{
					Mode = BindingMode.OneWay
				}.ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty5 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension obj30 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding44 = obj30.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider17.Bind(hsvColorProperty5, binding44);
				colorSlider17.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSlider17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				colorSlider17.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentSliderVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorSlider16).EndInit();
				Controls children26 = grid13.Children;
				Border border42;
				Border border43 = (border42 = new Border());
				((ISupportInitialize)border43).BeginInit();
				children26.Add(border43);
				Border border44 = (border3 = border42);
				context.PushParent(border3);
				Border border45 = border3;
				border45.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border45.SetValue(Grid.RowProperty, 4, BindingPriority.Template);
				StyledProperty<double> widthProperty5 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension27 = new DynamicResourceExtension("ColorViewComponentLabelWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding45 = dynamicResourceExtension27.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border45.Bind(widthProperty5, binding45);
				StyledProperty<double> heightProperty8 = Layoutable.HeightProperty;
				CompiledBindingExtension obj31 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component3NumericUpDown").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding46 = obj31.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border45.Bind(heightProperty8, binding46);
				StyledProperty<IBrush?> backgroundProperty7 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension28 = new DynamicResourceExtension("TextControlBackgroundDisabled");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding47 = dynamicResourceExtension28.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border45.Bind(backgroundProperty7, binding47);
				StyledProperty<IBrush?> borderBrushProperty6 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension29 = new DynamicResourceExtension("TextControlBorderBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding48 = dynamicResourceExtension29.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border45.Bind(borderBrushProperty6, binding48);
				border45.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border45.SetValue(Border.CornerRadiusProperty, new CornerRadius(4.0, 0.0, 0.0, 4.0), BindingPriority.Template);
				border45.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				border45.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				Panel panel14;
				Panel panel15 = (panel14 = new Panel());
				((ISupportInitialize)panel15).BeginInit();
				border45.Child = panel15;
				Panel panel16 = (panel3 = panel14);
				context.PushParent(panel3);
				Panel panel17 = panel3;
				panel17.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				panel17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Controls children27 = panel17.Children;
				TextBlock textBlock22;
				TextBlock textBlock23 = (textBlock22 = new TextBlock());
				((ISupportInitialize)textBlock23).BeginInit();
				children27.Add(textBlock23);
				TextBlock textBlock24 = (textBlock3 = textBlock22);
				context.PushParent(textBlock3);
				TextBlock textBlock25 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty6 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension30 = new DynamicResourceExtension("TextControlForegroundDisabled");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding49 = dynamicResourceExtension30.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock25.Bind(foregroundProperty6, binding49);
				textBlock25.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock25.SetValue(TextBlock.TextProperty, "B", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty6 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding22 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding23 = templateBinding;
				StaticResourceExtension staticResourceExtension22 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj32 = staticResourceExtension22.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding23.Converter = (IValueConverter)obj32;
				templateBinding23.ConverterParameter = ColorModel.Rgba;
				templateBinding23.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock25.Bind(isVisibleProperty6, templateBinding22.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock24).EndInit();
				Controls children28 = panel17.Children;
				TextBlock textBlock26;
				TextBlock textBlock27 = (textBlock26 = new TextBlock());
				((ISupportInitialize)textBlock27).BeginInit();
				children28.Add(textBlock27);
				TextBlock textBlock28 = (textBlock3 = textBlock26);
				context.PushParent(textBlock3);
				TextBlock textBlock29 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty7 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension31 = new DynamicResourceExtension("TextControlForegroundDisabled");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding50 = dynamicResourceExtension31.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock29.Bind(foregroundProperty7, binding50);
				textBlock29.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock29.SetValue(TextBlock.TextProperty, "V", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty7 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding24 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding25 = templateBinding;
				StaticResourceExtension staticResourceExtension23 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj33 = staticResourceExtension23.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding25.Converter = (IValueConverter)obj33;
				templateBinding25.ConverterParameter = ColorModel.Hsva;
				templateBinding25.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock29.Bind(isVisibleProperty7, templateBinding24.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock28).EndInit();
				context.PopParent();
				((ISupportInitialize)panel16).EndInit();
				context.PopParent();
				((ISupportInitialize)border44).EndInit();
				Controls children29 = grid13.Children;
				NumericUpDown numericUpDown10;
				NumericUpDown numericUpDown11 = (numericUpDown10 = new NumericUpDown());
				((ISupportInitialize)numericUpDown11).BeginInit();
				children29.Add(numericUpDown11);
				NumericUpDown numericUpDown12 = (numericUpDown3 = numericUpDown10);
				context.PushParent(numericUpDown3);
				NumericUpDown numericUpDown13 = numericUpDown3;
				numericUpDown13.Name = "Component3NumericUpDown";
				service = numericUpDown13;
				context.AvaloniaNameScope.Register("Component3NumericUpDown", service);
				numericUpDown13.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				numericUpDown13.SetValue(Grid.RowProperty, 4, BindingPriority.Template);
				numericUpDown13.SetValue(NumericUpDown.AllowSpinProperty, value: true, BindingPriority.Template);
				numericUpDown13.SetValue(NumericUpDown.ShowButtonSpinnerProperty, value: false, BindingPriority.Template);
				numericUpDown13.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<double> widthProperty6 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension32 = new DynamicResourceExtension("ColorViewComponentTextInputWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding51 = dynamicResourceExtension32.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown13.Bind(widthProperty6, binding51);
				numericUpDown13.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 4.0, 4.0, 0.0), BindingPriority.Template);
				numericUpDown13.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				numericUpDown13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension24 = new StaticResourceExtension("ColorViewComponentNumberFormat");
				context.ProvideTargetProperty = NumericUpDown.NumberFormatProperty;
				object? obj34 = staticResourceExtension24.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_5.DynamicSetter_5(numericUpDown13, BindingPriority.Template, obj34);
				StyledProperty<decimal> minimumProperty3 = NumericUpDown.MinimumProperty;
				CompiledBindingExtension compiledBindingExtension13 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component3Slider").Property(RangeBase.MinimumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension binding52 = compiledBindingExtension13.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown13.Bind(minimumProperty3, binding52);
				StyledProperty<decimal> maximumProperty3 = NumericUpDown.MaximumProperty;
				CompiledBindingExtension compiledBindingExtension14 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component3Slider").Property(RangeBase.MaximumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension binding53 = compiledBindingExtension14.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown13.Bind(maximumProperty3, binding53);
				StyledProperty<decimal?> valueProperty3 = NumericUpDown.ValueProperty;
				CompiledBindingExtension compiledBindingExtension15 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component3Slider").Property(RangeBase.ValueProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension binding54 = compiledBindingExtension15.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown13.Bind(valueProperty3, binding54);
				numericUpDown13.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)numericUpDown12).EndInit();
				Controls children30 = grid13.Children;
				ColorSlider colorSlider18;
				ColorSlider colorSlider19 = (colorSlider18 = new ColorSlider());
				((ISupportInitialize)colorSlider19).BeginInit();
				children30.Add(colorSlider19);
				ColorSlider colorSlider20 = (colorSlider3 = colorSlider18);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider21 = colorSlider3;
				colorSlider21.Name = "Component3Slider";
				service = colorSlider21;
				context.AvaloniaNameScope.Register("Component3Slider", service);
				colorSlider21.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider21.SetValue(Grid.RowProperty, 4, BindingPriority.Template);
				colorSlider21.SetValue(Slider.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				colorSlider21.SetValue(ColorSlider.IsRoundingEnabledProperty, value: true, BindingPriority.Template);
				colorSlider21.SetValue(Slider.IsSnapToTickEnabledProperty, value: true, BindingPriority.Template);
				colorSlider21.SetValue(Slider.TickFrequencyProperty, 1.0, BindingPriority.Template);
				colorSlider21.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Component3, BindingPriority.Template);
				colorSlider21.Bind(ColorSlider.ColorModelProperty, new TemplateBinding(ColorView.ColorModelProperty)
				{
					Mode = BindingMode.OneWay
				}.ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty6 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension obj35 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding55 = obj35.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider21.Bind(hsvColorProperty6, binding55);
				colorSlider21.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSlider21.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				colorSlider21.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentSliderVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorSlider20).EndInit();
				Controls children31 = grid13.Children;
				Border border46;
				Border border47 = (border46 = new Border());
				((ISupportInitialize)border47).BeginInit();
				children31.Add(border47);
				Border border48 = (border3 = border46);
				context.PushParent(border3);
				Border border49 = border3;
				border49.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border49.SetValue(Grid.RowProperty, 5, BindingPriority.Template);
				StyledProperty<double> widthProperty7 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension33 = new DynamicResourceExtension("ColorViewComponentLabelWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding56 = dynamicResourceExtension33.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border49.Bind(widthProperty7, binding56);
				StyledProperty<double> heightProperty9 = Layoutable.HeightProperty;
				CompiledBindingExtension obj36 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "AlphaComponentNumericUpDown").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding57 = obj36.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border49.Bind(heightProperty9, binding57);
				StyledProperty<IBrush?> backgroundProperty8 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension34 = new DynamicResourceExtension("TextControlBackgroundDisabled");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding58 = dynamicResourceExtension34.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border49.Bind(backgroundProperty8, binding58);
				StyledProperty<IBrush?> borderBrushProperty7 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension35 = new DynamicResourceExtension("TextControlBorderBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding59 = dynamicResourceExtension35.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border49.Bind(borderBrushProperty7, binding59);
				border49.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border49.SetValue(Border.CornerRadiusProperty, new CornerRadius(4.0, 0.0, 0.0, 4.0), BindingPriority.Template);
				border49.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				border49.Bind(InputElement.IsEnabledProperty, new TemplateBinding(ColorView.IsAlphaEnabledProperty).ProvideValue());
				StyledProperty<bool> isVisibleProperty8 = Visual.IsVisibleProperty;
				MultiBinding binding60 = (multiBinding = new MultiBinding());
				context.PushParent(multiBinding);
				MultiBinding multiBinding3 = multiBinding;
				multiBinding3.Converter = BoolConverters.And;
				IList<IBinding> bindings2 = multiBinding3.Bindings;
				CompiledBindingExtension obj37 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsAlphaVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item2 = obj37.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings2.Add(item2);
				IList<IBinding> bindings3 = multiBinding3.Bindings;
				CompiledBindingExtension obj38 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsComponentTextInputVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item3 = obj38.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings3.Add(item3);
				context.PopParent();
				border49.Bind(isVisibleProperty8, binding60);
				TextBlock textBlock30;
				TextBlock textBlock31 = (textBlock30 = new TextBlock());
				((ISupportInitialize)textBlock31).BeginInit();
				border49.Child = textBlock31;
				TextBlock textBlock32 = (textBlock3 = textBlock30);
				context.PushParent(textBlock3);
				TextBlock textBlock33 = textBlock3;
				textBlock33.Name = "AlphaComponentTextBlock";
				service = textBlock33;
				context.AvaloniaNameScope.Register("AlphaComponentTextBlock", service);
				StyledProperty<IBrush?> foregroundProperty8 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension36 = new DynamicResourceExtension("TextControlForegroundDisabled");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding61 = dynamicResourceExtension36.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock33.Bind(foregroundProperty8, binding61);
				textBlock33.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock33.SetValue(TextBlock.TextProperty, "A", BindingPriority.Template);
				textBlock33.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				textBlock33.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)textBlock32).EndInit();
				context.PopParent();
				((ISupportInitialize)border48).EndInit();
				Controls children32 = grid13.Children;
				NumericUpDown numericUpDown14;
				NumericUpDown numericUpDown15 = (numericUpDown14 = new NumericUpDown());
				((ISupportInitialize)numericUpDown15).BeginInit();
				children32.Add(numericUpDown15);
				NumericUpDown numericUpDown16 = (numericUpDown3 = numericUpDown14);
				context.PushParent(numericUpDown3);
				NumericUpDown numericUpDown17 = numericUpDown3;
				numericUpDown17.Name = "AlphaComponentNumericUpDown";
				service = numericUpDown17;
				context.AvaloniaNameScope.Register("AlphaComponentNumericUpDown", service);
				numericUpDown17.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				numericUpDown17.SetValue(Grid.RowProperty, 5, BindingPriority.Template);
				numericUpDown17.SetValue(NumericUpDown.AllowSpinProperty, value: true, BindingPriority.Template);
				numericUpDown17.SetValue(NumericUpDown.ShowButtonSpinnerProperty, value: false, BindingPriority.Template);
				numericUpDown17.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<double> widthProperty8 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension37 = new DynamicResourceExtension("ColorViewComponentTextInputWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding62 = dynamicResourceExtension37.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown17.Bind(widthProperty8, binding62);
				numericUpDown17.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 4.0, 4.0, 0.0), BindingPriority.Template);
				numericUpDown17.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				numericUpDown17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension25 = new StaticResourceExtension("ColorViewComponentNumberFormat");
				context.ProvideTargetProperty = NumericUpDown.NumberFormatProperty;
				object? obj39 = staticResourceExtension25.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_5.DynamicSetter_5(numericUpDown17, BindingPriority.Template, obj39);
				StyledProperty<decimal> minimumProperty4 = NumericUpDown.MinimumProperty;
				CompiledBindingExtension compiledBindingExtension16 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "AlphaComponentSlider").Property(RangeBase.MinimumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension binding63 = compiledBindingExtension16.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown17.Bind(minimumProperty4, binding63);
				StyledProperty<decimal> maximumProperty4 = NumericUpDown.MaximumProperty;
				CompiledBindingExtension compiledBindingExtension17 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "AlphaComponentSlider").Property(RangeBase.MaximumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension binding64 = compiledBindingExtension17.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown17.Bind(maximumProperty4, binding64);
				StyledProperty<decimal?> valueProperty4 = NumericUpDown.ValueProperty;
				CompiledBindingExtension compiledBindingExtension18 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "AlphaComponentSlider").Property(RangeBase.ValueProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension binding65 = compiledBindingExtension18.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown17.Bind(valueProperty4, binding65);
				numericUpDown17.Bind(InputElement.IsEnabledProperty, new TemplateBinding(ColorView.IsAlphaEnabledProperty).ProvideValue());
				StyledProperty<bool> isVisibleProperty9 = Visual.IsVisibleProperty;
				MultiBinding binding66 = (multiBinding = new MultiBinding());
				context.PushParent(multiBinding);
				MultiBinding multiBinding4 = multiBinding;
				multiBinding4.Converter = BoolConverters.And;
				IList<IBinding> bindings4 = multiBinding4.Bindings;
				CompiledBindingExtension obj40 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsAlphaVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item4 = obj40.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings4.Add(item4);
				IList<IBinding> bindings5 = multiBinding4.Bindings;
				CompiledBindingExtension obj41 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsComponentTextInputVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item5 = obj41.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings5.Add(item5);
				context.PopParent();
				numericUpDown17.Bind(isVisibleProperty9, binding66);
				context.PopParent();
				((ISupportInitialize)numericUpDown16).EndInit();
				Controls children33 = grid13.Children;
				ColorSlider colorSlider22;
				ColorSlider colorSlider23 = (colorSlider22 = new ColorSlider());
				((ISupportInitialize)colorSlider23).BeginInit();
				children33.Add(colorSlider23);
				ColorSlider colorSlider24 = (colorSlider3 = colorSlider22);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider25 = colorSlider3;
				colorSlider25.Name = "AlphaComponentSlider";
				service = colorSlider25;
				context.AvaloniaNameScope.Register("AlphaComponentSlider", service);
				colorSlider25.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider25.SetValue(Grid.RowProperty, 5, BindingPriority.Template);
				colorSlider25.SetValue(Slider.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				colorSlider25.SetValue(ColorSlider.IsRoundingEnabledProperty, value: true, BindingPriority.Template);
				colorSlider25.SetValue(Slider.IsSnapToTickEnabledProperty, value: true, BindingPriority.Template);
				colorSlider25.SetValue(Slider.TickFrequencyProperty, 1.0, BindingPriority.Template);
				colorSlider25.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Alpha, BindingPriority.Template);
				colorSlider25.Bind(ColorSlider.ColorModelProperty, new TemplateBinding(ColorView.ColorModelProperty)
				{
					Mode = BindingMode.OneWay
				}.ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty7 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension obj42 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding67 = obj42.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider25.Bind(hsvColorProperty7, binding67);
				colorSlider25.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSlider25.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				colorSlider25.Bind(InputElement.IsEnabledProperty, new TemplateBinding(ColorView.IsAlphaEnabledProperty).ProvideValue());
				StyledProperty<bool> isVisibleProperty10 = Visual.IsVisibleProperty;
				MultiBinding binding68 = (multiBinding = new MultiBinding());
				context.PushParent(multiBinding);
				MultiBinding multiBinding5 = multiBinding;
				multiBinding5.Converter = BoolConverters.And;
				IList<IBinding> bindings6 = multiBinding5.Bindings;
				CompiledBindingExtension obj43 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsAlphaVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item6 = obj43.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings6.Add(item6);
				IList<IBinding> bindings7 = multiBinding5.Bindings;
				CompiledBindingExtension obj44 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsComponentSliderVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item7 = obj44.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings7.Add(item7);
				context.PopParent();
				colorSlider25.Bind(isVisibleProperty10, binding68);
				context.PopParent();
				((ISupportInitialize)colorSlider24).EndInit();
				context.PopParent();
				((ISupportInitialize)grid12).EndInit();
				context.PopParent();
				((ISupportInitialize)tabItem12).EndInit();
				context.PopParent();
				((ISupportInitialize)tabControl4).EndInit();
				Controls children34 = grid5.Children;
				ColorPreviewer colorPreviewer;
				ColorPreviewer colorPreviewer2 = (colorPreviewer = new ColorPreviewer());
				((ISupportInitialize)colorPreviewer2).BeginInit();
				children34.Add(colorPreviewer2);
				ColorPreviewer colorPreviewer3;
				ColorPreviewer colorPreviewer4 = (colorPreviewer3 = colorPreviewer);
				context.PushParent(colorPreviewer3);
				colorPreviewer3.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
				StyledProperty<HsvColor> hsvColorProperty8 = ColorPreviewer.HsvColorProperty;
				CompiledBindingExtension obj45 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorPreviewer.HsvColorProperty;
				CompiledBindingExtension binding69 = obj45.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorPreviewer3.Bind(hsvColorProperty8, binding69);
				colorPreviewer3.Bind(ColorPreviewer.IsAccentColorsVisibleProperty, new TemplateBinding(ColorView.IsAccentColorsVisibleProperty).ProvideValue());
				colorPreviewer3.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 0.0, 12.0, 12.0), BindingPriority.Template);
				colorPreviewer3.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorPreviewVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorPreviewer4).EndInit();
				context.PopParent();
				((ISupportInitialize)grid4).EndInit();
				context.PopParent();
				dropDownButton.SetValue(flyoutProperty, value, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
			controlTheme.TargetType = typeof(ColorPicker);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = TemplatedControl.CornerRadiusProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ControlCornerRadius");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Setter setter4 = new Setter();
			setter4.Property = ColorView.HexInputAlphaPositionProperty;
			setter4.Value = AlphaComponentPosition.Trailing;
			controlTheme.Add(setter4);
			Setter setter5 = new Setter();
			setter5.Property = Layoutable.HeightProperty;
			setter5.Value = 32.0;
			controlTheme.Add(setter5);
			Setter setter6 = new Setter();
			setter6.Property = Layoutable.WidthProperty;
			setter6.Value = 64.0;
			controlTheme.Add(setter6);
			Setter setter7 = new Setter();
			setter7.Property = Layoutable.MinWidthProperty;
			setter7.Value = 64.0;
			controlTheme.Add(setter7);
			Setter setter8 = new Setter();
			setter8.Property = ColorView.PaletteProperty;
			setter8.Value = new FluentColorPalette();
			controlTheme.Add(setter8);
			Setter setter9 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter10 = setter;
			setter10.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value2 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.TargetType = typeof(ColorPicker);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_4.Build, context);
			context.PopParent();
			setter10.Value = value2;
			context.PopParent();
			controlTheme.Add(setter9);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_10
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new AccentColorConverter();
		}
	}

	private class XamlClosure_11
	{
		private class XamlClosure_12
		{
			private class DynamicSetters_13
			{
				public static void DynamicSetter_1(Layoutable P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(Layoutable.HeightProperty, binding);
						return;
					}
					if (P_2 is double value)
					{
						int priority = (int)P_1;
						P_0.SetValue(Layoutable.HeightProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						throw new NullReferenceException();
					}
					throw new InvalidCastException();
				}

				public static void DynamicSetter_2(Layoutable P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(Layoutable.WidthProperty, binding);
						return;
					}
					if (P_2 is double value)
					{
						int priority = (int)P_1;
						P_0.SetValue(Layoutable.WidthProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						throw new NullReferenceException();
					}
					throw new InvalidCastException();
				}

				public static void DynamicSetter_3(Border P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(Border.BackgroundProperty, binding);
						return;
					}
					if (P_2 is IBrush)
					{
						IBrush value = (IBrush)P_2;
						int priority = (int)P_1;
						P_0.SetValue(Border.BackgroundProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						IBrush value = (IBrush)P_2;
						int priority = (int)P_1;
						P_0.SetValue(Border.BackgroundProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
				Panel panel2 = panel;
				Controls children = panel2.Children;
				Grid grid;
				Grid grid2 = (grid = new Grid());
				((ISupportInitialize)grid2).BeginInit();
				children.Add(grid2);
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
				grid5.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorPreviewer.IsAccentColorsVisibleProperty).ProvideValue());
				Controls children2 = grid5.Children;
				Grid grid6;
				Grid grid7 = (grid6 = new Grid());
				((ISupportInitialize)grid7).BeginInit();
				children2.Add(grid7);
				Grid grid8 = (grid3 = grid6);
				context.PushParent(grid3);
				Grid grid9 = grid3;
				grid9.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ColorPreviewerAccentSectionHeight");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				object? obj = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_13.DynamicSetter_1(grid9, BindingPriority.Template, obj);
				StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("ColorPreviewerAccentSectionWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				object? obj2 = staticResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_13.DynamicSetter_2(grid9, BindingPriority.Template, obj2);
				ColumnDefinitions columnDefinitions2 = new ColumnDefinitions();
				columnDefinitions2.Capacity = 2;
				columnDefinitions2.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions2.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid9.ColumnDefinitions = columnDefinitions2;
				grid9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Controls children3 = grid9.Children;
				Border border;
				Border border2 = (border = new Border());
				((ISupportInitialize)border2).BeginInit();
				children3.Add(border2);
				Border border3;
				Border border4 = (border3 = border);
				context.PushParent(border3);
				Border border5 = border3;
				border5.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border5.SetValue(Grid.ColumnSpanProperty, 2, BindingPriority.Template);
				border5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension3 = new StaticResourceExtension("ColorControlCheckeredBackgroundBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				object? obj3 = staticResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_13.DynamicSetter_3(border5, BindingPriority.Template, obj3);
				context.PopParent();
				((ISupportInitialize)border4).EndInit();
				Controls children4 = grid9.Children;
				Border border6;
				Border border7 = (border6 = new Border());
				((ISupportInitialize)border7).BeginInit();
				children4.Add(border7);
				Border border8 = (border3 = border6);
				context.PushParent(border3);
				Border border9 = border3;
				border9.Name = "PART_AccentDecrement2Border";
				service = border9;
				context.AvaloniaNameScope.Register("PART_AccentDecrement2Border", service);
				border9.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				StyledProperty<CornerRadius> cornerRadiusProperty = Border.CornerRadiusProperty;
				TemplateBinding templateBinding;
				TemplateBinding templateBinding2 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding3 = templateBinding;
				StaticResourceExtension staticResourceExtension4 = new StaticResourceExtension("LeftCornerRadiusFilterConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj4 = staticResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding3.Converter = (IValueConverter)obj4;
				context.PopParent();
				border9.Bind(cornerRadiusProperty, templateBinding2.ProvideValue());
				border9.SetValue(Control.TagProperty, "-2", BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty = Border.BackgroundProperty;
				TemplateBinding templateBinding4 = (templateBinding = new TemplateBinding(ColorPreviewer.HsvColorProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding5 = templateBinding;
				StaticResourceExtension staticResourceExtension5 = new StaticResourceExtension("AccentColorConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj5 = staticResourceExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding5.Converter = (IValueConverter)obj5;
				templateBinding5.ConverterParameter = "-2";
				context.PopParent();
				border9.Bind(backgroundProperty, templateBinding4.ProvideValue());
				context.PopParent();
				((ISupportInitialize)border8).EndInit();
				Controls children5 = grid9.Children;
				Border border10;
				Border border11 = (border10 = new Border());
				((ISupportInitialize)border11).BeginInit();
				children5.Add(border11);
				Border border12 = (border3 = border10);
				context.PushParent(border3);
				Border border13 = border3;
				border13.Name = "PART_AccentDecrement1Border";
				service = border13;
				context.AvaloniaNameScope.Register("PART_AccentDecrement1Border", service);
				border13.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				border13.SetValue(Control.TagProperty, "-1", BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty2 = Border.BackgroundProperty;
				TemplateBinding templateBinding6 = (templateBinding = new TemplateBinding(ColorPreviewer.HsvColorProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding7 = templateBinding;
				StaticResourceExtension staticResourceExtension6 = new StaticResourceExtension("AccentColorConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj6 = staticResourceExtension6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding7.Converter = (IValueConverter)obj6;
				templateBinding7.ConverterParameter = "-1";
				context.PopParent();
				border13.Bind(backgroundProperty2, templateBinding6.ProvideValue());
				context.PopParent();
				((ISupportInitialize)border12).EndInit();
				context.PopParent();
				((ISupportInitialize)grid8).EndInit();
				Controls children6 = grid5.Children;
				Grid grid10;
				Grid grid11 = (grid10 = new Grid());
				((ISupportInitialize)grid11).BeginInit();
				children6.Add(grid11);
				Grid grid12 = (grid3 = grid10);
				context.PushParent(grid3);
				Grid grid13 = grid3;
				grid13.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension7 = new StaticResourceExtension("ColorPreviewerAccentSectionHeight");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				object? obj7 = staticResourceExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_13.DynamicSetter_1(grid13, BindingPriority.Template, obj7);
				StaticResourceExtension staticResourceExtension8 = new StaticResourceExtension("ColorPreviewerAccentSectionWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				object? obj8 = staticResourceExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_13.DynamicSetter_2(grid13, BindingPriority.Template, obj8);
				ColumnDefinitions columnDefinitions3 = new ColumnDefinitions();
				columnDefinitions3.Capacity = 2;
				columnDefinitions3.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions3.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid13.ColumnDefinitions = columnDefinitions3;
				grid13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Controls children7 = grid13.Children;
				Border border14;
				Border border15 = (border14 = new Border());
				((ISupportInitialize)border15).BeginInit();
				children7.Add(border15);
				Border border16 = (border3 = border14);
				context.PushParent(border3);
				Border border17 = border3;
				border17.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border17.SetValue(Grid.ColumnSpanProperty, 2, BindingPriority.Template);
				border17.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension9 = new StaticResourceExtension("ColorControlCheckeredBackgroundBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				object? obj9 = staticResourceExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_13.DynamicSetter_3(border17, BindingPriority.Template, obj9);
				context.PopParent();
				((ISupportInitialize)border16).EndInit();
				Controls children8 = grid13.Children;
				Border border18;
				Border border19 = (border18 = new Border());
				((ISupportInitialize)border19).BeginInit();
				children8.Add(border19);
				Border border20 = (border3 = border18);
				context.PushParent(border3);
				Border border21 = border3;
				border21.Name = "PART_AccentIncrement1Border";
				service = border21;
				context.AvaloniaNameScope.Register("PART_AccentIncrement1Border", service);
				border21.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border21.SetValue(Control.TagProperty, "1", BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty3 = Border.BackgroundProperty;
				TemplateBinding templateBinding8 = (templateBinding = new TemplateBinding(ColorPreviewer.HsvColorProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding9 = templateBinding;
				StaticResourceExtension staticResourceExtension10 = new StaticResourceExtension("AccentColorConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj10 = staticResourceExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding9.Converter = (IValueConverter)obj10;
				templateBinding9.ConverterParameter = "1";
				context.PopParent();
				border21.Bind(backgroundProperty3, templateBinding8.ProvideValue());
				context.PopParent();
				((ISupportInitialize)border20).EndInit();
				Controls children9 = grid13.Children;
				Border border22;
				Border border23 = (border22 = new Border());
				((ISupportInitialize)border23).BeginInit();
				children9.Add(border23);
				Border border24 = (border3 = border22);
				context.PushParent(border3);
				Border border25 = border3;
				border25.Name = "PART_AccentIncrement2Border";
				service = border25;
				context.AvaloniaNameScope.Register("PART_AccentIncrement2Border", service);
				border25.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				StyledProperty<CornerRadius> cornerRadiusProperty2 = Border.CornerRadiusProperty;
				TemplateBinding templateBinding10 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding11 = templateBinding;
				StaticResourceExtension staticResourceExtension11 = new StaticResourceExtension("RightCornerRadiusFilterConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj11 = staticResourceExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding11.Converter = (IValueConverter)obj11;
				context.PopParent();
				border25.Bind(cornerRadiusProperty2, templateBinding10.ProvideValue());
				border25.SetValue(Control.TagProperty, "2", BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty4 = Border.BackgroundProperty;
				TemplateBinding templateBinding12 = (templateBinding = new TemplateBinding(ColorPreviewer.HsvColorProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding13 = templateBinding;
				StaticResourceExtension staticResourceExtension12 = new StaticResourceExtension("AccentColorConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj12 = staticResourceExtension12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding13.Converter = (IValueConverter)obj12;
				templateBinding13.ConverterParameter = "2";
				context.PopParent();
				border25.Bind(backgroundProperty4, templateBinding12.ProvideValue());
				context.PopParent();
				((ISupportInitialize)border24).EndInit();
				context.PopParent();
				((ISupportInitialize)grid12).EndInit();
				Controls children10 = grid5.Children;
				Border border26;
				Border border27 = (border26 = new Border());
				((ISupportInitialize)border27).BeginInit();
				children10.Add(border27);
				Border border28 = (border3 = border26);
				context.PushParent(border3);
				Border border29 = border3;
				border29.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				border29.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border29.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				border29.SetValue(Border.BoxShadowProperty, BoxShadows.Parse("0 0 10 2 #BF000000"), BindingPriority.Template);
				border29.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				Panel panel3;
				Panel panel4 = (panel3 = new Panel());
				((ISupportInitialize)panel4).BeginInit();
				border29.Child = panel4;
				Panel panel5 = (panel = panel3);
				context.PushParent(panel);
				Panel panel6 = panel;
				Controls children11 = panel6.Children;
				Border border30;
				Border border31 = (border30 = new Border());
				((ISupportInitialize)border31).BeginInit();
				children11.Add(border31);
				Border border32 = (border3 = border30);
				context.PushParent(border3);
				Border border33 = border3;
				StaticResourceExtension staticResourceExtension13 = new StaticResourceExtension("ColorControlCheckeredBackgroundBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				object? obj13 = staticResourceExtension13.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_13.DynamicSetter_3(border33, BindingPriority.Template, obj13);
				border33.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)border32).EndInit();
				Controls children12 = panel6.Children;
				Border border34;
				Border border35 = (border34 = new Border());
				((ISupportInitialize)border35).BeginInit();
				children12.Add(border35);
				Border border36 = (border3 = border34);
				context.PushParent(border3);
				Border border37 = border3;
				border37.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				StyledProperty<IBrush?> backgroundProperty5 = Border.BackgroundProperty;
				TemplateBinding templateBinding14 = (templateBinding = new TemplateBinding(ColorPreviewer.HsvColorProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding15 = templateBinding;
				StaticResourceExtension staticResourceExtension14 = new StaticResourceExtension("ToBrushConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj14 = staticResourceExtension14.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding15.Converter = (IValueConverter)obj14;
				context.PopParent();
				border37.Bind(backgroundProperty5, templateBinding14.ProvideValue());
				border37.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border37.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)border36).EndInit();
				context.PopParent();
				((ISupportInitialize)panel5).EndInit();
				context.PopParent();
				((ISupportInitialize)border28).EndInit();
				context.PopParent();
				((ISupportInitialize)grid4).EndInit();
				Controls children13 = panel2.Children;
				Border border38;
				Border border39 = (border38 = new Border());
				((ISupportInitialize)border39).BeginInit();
				children13.Add(border39);
				Border border40 = (border3 = border38);
				context.PushParent(border3);
				Border border41 = border3;
				border41.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				border41.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorPreviewer.IsAccentColorsVisibleProperty)
				{
					Converter = BoolConverters.Not
				}.ProvideValue());
				border41.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border41.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				Panel panel7;
				Panel panel8 = (panel7 = new Panel());
				((ISupportInitialize)panel8).BeginInit();
				border41.Child = panel8;
				Panel panel9 = (panel = panel7);
				context.PushParent(panel);
				Panel panel10 = panel;
				Controls children14 = panel10.Children;
				Border border42;
				Border border43 = (border42 = new Border());
				((ISupportInitialize)border43).BeginInit();
				children14.Add(border43);
				Border border44 = (border3 = border42);
				context.PushParent(border3);
				Border border45 = border3;
				StaticResourceExtension staticResourceExtension15 = new StaticResourceExtension("ColorControlCheckeredBackgroundBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				object? obj15 = staticResourceExtension15.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_13.DynamicSetter_3(border45, BindingPriority.Template, obj15);
				border45.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)border44).EndInit();
				Controls children15 = panel10.Children;
				Border border46;
				Border border47 = (border46 = new Border());
				((ISupportInitialize)border47).BeginInit();
				children15.Add(border47);
				Border border48 = (border3 = border46);
				context.PushParent(border3);
				Border border49 = border3;
				border49.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				StyledProperty<IBrush?> backgroundProperty6 = Border.BackgroundProperty;
				TemplateBinding templateBinding16 = (templateBinding = new TemplateBinding(ColorPreviewer.HsvColorProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding17 = templateBinding;
				StaticResourceExtension staticResourceExtension16 = new StaticResourceExtension("ToBrushConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj16 = staticResourceExtension16.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding17.Converter = (IValueConverter)obj16;
				context.PopParent();
				border49.Bind(backgroundProperty6, templateBinding16.ProvideValue());
				border49.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border49.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)border48).EndInit();
				context.PopParent();
				((ISupportInitialize)panel9).EndInit();
				context.PopParent();
				((ISupportInitialize)border40).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
			controlTheme.TargetType = typeof(ColorPreviewer);
			Setter setter = new Setter();
			setter.Property = Layoutable.HeightProperty;
			setter.Value = 50.0;
			controlTheme.Add(setter);
			Setter setter2 = new Setter();
			setter2.Property = Visual.ClipToBoundsProperty;
			setter2.Value = false;
			controlTheme.Add(setter2);
			Setter setter3;
			Setter setter4 = (setter3 = new Setter());
			context.PushParent(setter3);
			Setter setter5 = setter3;
			setter5.Property = TemplatedControl.CornerRadiusProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ControlCornerRadius");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter5.Value = value;
			context.PopParent();
			controlTheme.Add(setter4);
			Setter setter6 = (setter3 = new Setter());
			context.PushParent(setter3);
			Setter setter7 = setter3;
			setter7.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value2 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.TargetType = typeof(ColorPreviewer);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_12.Build, context);
			context.PopParent();
			setter7.Value = value2;
			context.PopParent();
			controlTheme.Add(setter6);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_14
	{
		private class XamlClosure_15
		{
			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
				((AvaloniaObject)intermediateRoot).Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
			controlTheme.TargetType = typeof(Thumb);
			Setter setter = new Setter();
			setter.Property = TemplatedControl.BackgroundProperty;
			setter.Value = new ImmutableSolidColorBrush(16777215u);
			controlTheme.Add(setter);
			Setter setter2;
			Setter setter3 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter4 = setter2;
			setter4.Property = TemplatedControl.BorderBrushProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemControlForegroundBaseHighBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter4.Value = value;
			context.PopParent();
			controlTheme.Add(setter3);
			Setter setter5 = new Setter();
			setter5.Property = TemplatedControl.BorderThicknessProperty;
			setter5.Value = new Thickness(3.0, 3.0, 3.0, 3.0);
			controlTheme.Add(setter5);
			Setter setter6 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter7 = setter2;
			setter7.Property = TemplatedControl.CornerRadiusProperty;
			DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ColorSliderCornerRadius");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value2 = dynamicResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter7.Value = value2;
			context.PopParent();
			controlTheme.Add(setter6);
			Setter setter8 = new Setter();
			setter8.Property = TemplatedControl.TemplateProperty;
			setter8.Value = new ControlTemplate
			{
				Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_15.Build, context)
			};
			controlTheme.Add(setter8);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_16
	{
		private class XamlClosure_17
		{
			private class DynamicSetters_18
			{
				public static void DynamicSetter_1(Border P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(Border.BackgroundProperty, binding);
						return;
					}
					if (P_2 is IBrush)
					{
						IBrush value = (IBrush)P_2;
						int priority = (int)P_1;
						P_0.SetValue(Border.BackgroundProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						IBrush value = (IBrush)P_2;
						int priority = (int)P_1;
						P_0.SetValue(Border.BackgroundProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
				}

				public static void DynamicSetter_2(StyledElement P_0, BindingPriority P_1, IBinding P_2)
				{
					if (P_2 != null)
					{
						IBinding binding = P_2;
						P_0.Bind(StyledElement.DataContextProperty, binding);
					}
					else
					{
						object value = P_2;
						int priority = (int)P_1;
						P_0.SetValue(StyledElement.DataContextProperty, value, (BindingPriority)priority);
					}
				}
			}

			private class XamlClosure_19
			{
				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
					((StyledElement)intermediateRoot).Name = "FocusTarget";
					service = intermediateRoot;
					context.AvaloniaNameScope.Register("FocusTarget", service);
					((AvaloniaObject)intermediateRoot).SetValue(Border.BackgroundProperty, (IBrush)new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
					((AvaloniaObject)intermediateRoot).SetValue(Layoutable.MarginProperty, new Thickness(0.0, -10.0, 0.0, -10.0), BindingPriority.Template);
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			private class XamlClosure_20
			{
				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
					((StyledElement)intermediateRoot).Name = "FocusTarget";
					service = intermediateRoot;
					context.AvaloniaNameScope.Register("FocusTarget", service);
					((AvaloniaObject)intermediateRoot).SetValue(Border.BackgroundProperty, (IBrush)new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
					((AvaloniaObject)intermediateRoot).SetValue(Layoutable.MarginProperty, new Thickness(0.0, -10.0, 0.0, -10.0), BindingPriority.Template);
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
				border2.Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				border2.Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				border2.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				Grid grid;
				Grid grid2 = (grid = new Grid());
				((ISupportInitialize)grid2).BeginInit();
				border2.Child = grid2;
				Grid grid3;
				Grid grid4 = (grid3 = grid);
				context.PushParent(grid3);
				grid3.Bind(Layoutable.MarginProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				Controls children = grid3.Children;
				Border border3;
				Border border4 = (border3 = new Border());
				((ISupportInitialize)border4).BeginInit();
				children.Add(border4);
				Border border5 = (border = border3);
				context.PushParent(border);
				Border border6 = border;
				border6.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border6.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StyledProperty<double> heightProperty = Layoutable.HeightProperty;
				CompiledBindingExtension obj = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "PART_Track").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding = obj.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border6.Bind(heightProperty, binding);
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ColorControlCheckeredBackgroundBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				object? obj2 = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_18.DynamicSetter_1(border6, BindingPriority.Template, obj2);
				StyledProperty<CornerRadius> cornerRadiusProperty = Border.CornerRadiusProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ColorSliderTrackCornerRadius");
				context.ProvideTargetProperty = Border.CornerRadiusProperty;
				IBinding binding2 = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border6.Bind(cornerRadiusProperty, binding2);
				context.PopParent();
				((ISupportInitialize)border5).EndInit();
				Controls children2 = grid3.Children;
				Border border7;
				Border border8 = (border7 = new Border());
				((ISupportInitialize)border8).BeginInit();
				children2.Add(border8);
				Border border9 = (border = border7);
				context.PushParent(border);
				Border border10 = border;
				border10.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border10.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StyledProperty<double> heightProperty2 = Layoutable.HeightProperty;
				CompiledBindingExtension obj3 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "PART_Track").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding3 = obj3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border10.Bind(heightProperty2, binding3);
				border10.Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				StyledProperty<CornerRadius> cornerRadiusProperty2 = Border.CornerRadiusProperty;
				DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ColorSliderTrackCornerRadius");
				context.ProvideTargetProperty = Border.CornerRadiusProperty;
				IBinding binding4 = dynamicResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border10.Bind(cornerRadiusProperty2, binding4);
				context.PopParent();
				((ISupportInitialize)border9).EndInit();
				Controls children3 = grid3.Children;
				Track track;
				Track track2 = (track = new Track());
				((ISupportInitialize)track2).BeginInit();
				children3.Add(track2);
				Track track3;
				Track track4 = (track3 = track);
				context.PushParent(track3);
				track3.Name = "PART_Track";
				service = track3;
				context.AvaloniaNameScope.Register("PART_Track", service);
				StyledProperty<double> heightProperty3 = Layoutable.HeightProperty;
				DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ColorSliderTrackSize");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				IBinding binding5 = dynamicResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				track3.Bind(heightProperty3, binding5);
				track3.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				track3.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				track3.Bind(Track.MinimumProperty, new TemplateBinding(RangeBase.MinimumProperty).ProvideValue());
				track3.Bind(Track.MaximumProperty, new TemplateBinding(RangeBase.MaximumProperty).ProvideValue());
				track3.Bind(Track.ValueProperty, new TemplateBinding(RangeBase.ValueProperty)
				{
					Mode = BindingMode.TwoWay
				}.ProvideValue());
				track3.Bind(Track.IsDirectionReversedProperty, new TemplateBinding(Slider.IsDirectionReversedProperty).ProvideValue());
				track3.SetValue(Track.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				StyledProperty<Button?> decreaseButtonProperty = Track.DecreaseButtonProperty;
				RepeatButton repeatButton;
				RepeatButton repeatButton2 = (repeatButton = new RepeatButton());
				((ISupportInitialize)repeatButton2).BeginInit();
				track3.SetValue(decreaseButtonProperty, repeatButton2, BindingPriority.Template);
				repeatButton.Name = "PART_DecreaseButton";
				service = repeatButton;
				context.AvaloniaNameScope.Register("PART_DecreaseButton", service);
				repeatButton.SetValue(TemplatedControl.BackgroundProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				repeatButton.SetValue(InputElement.FocusableProperty, value: false, BindingPriority.Template);
				repeatButton.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				repeatButton.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				repeatButton.SetValue(TemplatedControl.TemplateProperty, new ControlTemplate
				{
					Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_19.Build, context)
				}, BindingPriority.Template);
				((ISupportInitialize)repeatButton).EndInit();
				StyledProperty<Button?> increaseButtonProperty = Track.IncreaseButtonProperty;
				RepeatButton repeatButton3;
				RepeatButton repeatButton4 = (repeatButton3 = new RepeatButton());
				((ISupportInitialize)repeatButton4).BeginInit();
				track3.SetValue(increaseButtonProperty, repeatButton4, BindingPriority.Template);
				repeatButton3.Name = "PART_IncreaseButton";
				service = repeatButton3;
				context.AvaloniaNameScope.Register("PART_IncreaseButton", service);
				repeatButton3.SetValue(TemplatedControl.BackgroundProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				repeatButton3.SetValue(InputElement.FocusableProperty, value: false, BindingPriority.Template);
				repeatButton3.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				repeatButton3.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				repeatButton3.SetValue(TemplatedControl.TemplateProperty, new ControlTemplate
				{
					Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_20.Build, context)
				}, BindingPriority.Template);
				((ISupportInitialize)repeatButton3).EndInit();
				Thumb thumb;
				Thumb thumb2 = (thumb = new Thumb());
				((ISupportInitialize)thumb2).BeginInit();
				track3.Thumb = thumb2;
				Thumb thumb3;
				Thumb thumb4 = (thumb3 = thumb);
				context.PushParent(thumb3);
				thumb3.Name = "ColorSliderThumb";
				service = thumb3;
				context.AvaloniaNameScope.Register("ColorSliderThumb", service);
				StyledProperty<ControlTheme?> themeProperty = StyledElement.ThemeProperty;
				DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("ColorSliderThumbTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				IBinding binding6 = dynamicResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				thumb3.Bind(themeProperty, binding6);
				thumb3.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				thumb3.SetValue(TemplatedControl.PaddingProperty, new Thickness(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				DynamicSetters_18.DynamicSetter_2(thumb3, BindingPriority.Template, new TemplateBinding(RangeBase.ValueProperty).ProvideValue());
				thumb3.Bind(Layoutable.HeightProperty, new TemplateBinding(Layoutable.HeightProperty).ProvideValue());
				thumb3.Bind(Layoutable.WidthProperty, new TemplateBinding(Layoutable.HeightProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)thumb4).EndInit();
				context.PopParent();
				((ISupportInitialize)track4).EndInit();
				context.PopParent();
				((ISupportInitialize)grid4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		private class XamlClosure_21
		{
			private class DynamicSetters_22
			{
				public static void DynamicSetter_1(Border P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(Border.BackgroundProperty, binding);
						return;
					}
					if (P_2 is IBrush)
					{
						IBrush value = (IBrush)P_2;
						int priority = (int)P_1;
						P_0.SetValue(Border.BackgroundProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						IBrush value = (IBrush)P_2;
						int priority = (int)P_1;
						P_0.SetValue(Border.BackgroundProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
				}

				public static void DynamicSetter_2(StyledElement P_0, BindingPriority P_1, IBinding P_2)
				{
					if (P_2 != null)
					{
						IBinding binding = P_2;
						P_0.Bind(StyledElement.DataContextProperty, binding);
					}
					else
					{
						object value = P_2;
						int priority = (int)P_1;
						P_0.SetValue(StyledElement.DataContextProperty, value, (BindingPriority)priority);
					}
				}
			}

			private class XamlClosure_23
			{
				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
					((StyledElement)intermediateRoot).Name = "FocusTarget";
					service = intermediateRoot;
					context.AvaloniaNameScope.Register("FocusTarget", service);
					((AvaloniaObject)intermediateRoot).SetValue(Border.BackgroundProperty, (IBrush)new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
					((AvaloniaObject)intermediateRoot).SetValue(Layoutable.MarginProperty, new Thickness(0.0, -10.0, 0.0, -10.0), BindingPriority.Template);
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			private class XamlClosure_24
			{
				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
					((StyledElement)intermediateRoot).Name = "FocusTarget";
					service = intermediateRoot;
					context.AvaloniaNameScope.Register("FocusTarget", service);
					((AvaloniaObject)intermediateRoot).SetValue(Border.BackgroundProperty, (IBrush)new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
					((AvaloniaObject)intermediateRoot).SetValue(Layoutable.MarginProperty, new Thickness(0.0, -10.0, 0.0, -10.0), BindingPriority.Template);
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
				border2.Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				border2.Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				border2.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				Grid grid;
				Grid grid2 = (grid = new Grid());
				((ISupportInitialize)grid2).BeginInit();
				border2.Child = grid2;
				Grid grid3;
				Grid grid4 = (grid3 = grid);
				context.PushParent(grid3);
				grid3.Bind(Layoutable.MarginProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				Controls children = grid3.Children;
				Border border3;
				Border border4 = (border3 = new Border());
				((ISupportInitialize)border4).BeginInit();
				children.Add(border4);
				Border border5 = (border = border3);
				context.PushParent(border);
				Border border6 = border;
				border6.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				border6.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<double> widthProperty = Layoutable.WidthProperty;
				CompiledBindingExtension obj = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "PART_Track").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EWidth_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				CompiledBindingExtension binding = obj.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border6.Bind(widthProperty, binding);
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ColorControlCheckeredBackgroundBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				object? obj2 = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_22.DynamicSetter_1(border6, BindingPriority.Template, obj2);
				StyledProperty<CornerRadius> cornerRadiusProperty = Border.CornerRadiusProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ColorSliderTrackCornerRadius");
				context.ProvideTargetProperty = Border.CornerRadiusProperty;
				IBinding binding2 = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border6.Bind(cornerRadiusProperty, binding2);
				context.PopParent();
				((ISupportInitialize)border5).EndInit();
				Controls children2 = grid3.Children;
				Border border7;
				Border border8 = (border7 = new Border());
				((ISupportInitialize)border8).BeginInit();
				children2.Add(border8);
				Border border9 = (border = border7);
				context.PushParent(border);
				Border border10 = border;
				border10.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				border10.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<double> widthProperty2 = Layoutable.WidthProperty;
				CompiledBindingExtension obj3 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "PART_Track").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EWidth_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				CompiledBindingExtension binding3 = obj3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border10.Bind(widthProperty2, binding3);
				border10.Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				StyledProperty<CornerRadius> cornerRadiusProperty2 = Border.CornerRadiusProperty;
				DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ColorSliderTrackCornerRadius");
				context.ProvideTargetProperty = Border.CornerRadiusProperty;
				IBinding binding4 = dynamicResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border10.Bind(cornerRadiusProperty2, binding4);
				context.PopParent();
				((ISupportInitialize)border9).EndInit();
				Controls children3 = grid3.Children;
				Track track;
				Track track2 = (track = new Track());
				((ISupportInitialize)track2).BeginInit();
				children3.Add(track2);
				Track track3;
				Track track4 = (track3 = track);
				context.PushParent(track3);
				track3.Name = "PART_Track";
				service = track3;
				context.AvaloniaNameScope.Register("PART_Track", service);
				StyledProperty<double> widthProperty3 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ColorSliderTrackSize");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding5 = dynamicResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				track3.Bind(widthProperty3, binding5);
				track3.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				track3.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				track3.Bind(Track.MinimumProperty, new TemplateBinding(RangeBase.MinimumProperty).ProvideValue());
				track3.Bind(Track.MaximumProperty, new TemplateBinding(RangeBase.MaximumProperty).ProvideValue());
				track3.Bind(Track.ValueProperty, new TemplateBinding(RangeBase.ValueProperty)
				{
					Mode = BindingMode.TwoWay
				}.ProvideValue());
				track3.Bind(Track.IsDirectionReversedProperty, new TemplateBinding(Slider.IsDirectionReversedProperty).ProvideValue());
				track3.SetValue(Track.OrientationProperty, Orientation.Vertical, BindingPriority.Template);
				StyledProperty<Button?> decreaseButtonProperty = Track.DecreaseButtonProperty;
				RepeatButton repeatButton;
				RepeatButton repeatButton2 = (repeatButton = new RepeatButton());
				((ISupportInitialize)repeatButton2).BeginInit();
				track3.SetValue(decreaseButtonProperty, repeatButton2, BindingPriority.Template);
				repeatButton.Name = "PART_DecreaseButton";
				service = repeatButton;
				context.AvaloniaNameScope.Register("PART_DecreaseButton", service);
				repeatButton.SetValue(TemplatedControl.BackgroundProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				repeatButton.SetValue(InputElement.FocusableProperty, value: false, BindingPriority.Template);
				repeatButton.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				repeatButton.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				repeatButton.SetValue(TemplatedControl.TemplateProperty, new ControlTemplate
				{
					Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_23.Build, context)
				}, BindingPriority.Template);
				((ISupportInitialize)repeatButton).EndInit();
				StyledProperty<Button?> increaseButtonProperty = Track.IncreaseButtonProperty;
				RepeatButton repeatButton3;
				RepeatButton repeatButton4 = (repeatButton3 = new RepeatButton());
				((ISupportInitialize)repeatButton4).BeginInit();
				track3.SetValue(increaseButtonProperty, repeatButton4, BindingPriority.Template);
				repeatButton3.Name = "PART_IncreaseButton";
				service = repeatButton3;
				context.AvaloniaNameScope.Register("PART_IncreaseButton", service);
				repeatButton3.SetValue(TemplatedControl.BackgroundProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				repeatButton3.SetValue(InputElement.FocusableProperty, value: false, BindingPriority.Template);
				repeatButton3.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				repeatButton3.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				repeatButton3.SetValue(TemplatedControl.TemplateProperty, new ControlTemplate
				{
					Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_24.Build, context)
				}, BindingPriority.Template);
				((ISupportInitialize)repeatButton3).EndInit();
				Thumb thumb;
				Thumb thumb2 = (thumb = new Thumb());
				((ISupportInitialize)thumb2).BeginInit();
				track3.Thumb = thumb2;
				Thumb thumb3;
				Thumb thumb4 = (thumb3 = thumb);
				context.PushParent(thumb3);
				thumb3.Name = "ColorSliderThumb";
				service = thumb3;
				context.AvaloniaNameScope.Register("ColorSliderThumb", service);
				StyledProperty<ControlTheme?> themeProperty = StyledElement.ThemeProperty;
				DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("ColorSliderThumbTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				IBinding binding6 = dynamicResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				thumb3.Bind(themeProperty, binding6);
				thumb3.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				thumb3.SetValue(TemplatedControl.PaddingProperty, new Thickness(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				DynamicSetters_22.DynamicSetter_2(thumb3, BindingPriority.Template, new TemplateBinding(RangeBase.ValueProperty).ProvideValue());
				thumb3.Bind(Layoutable.HeightProperty, new TemplateBinding(Layoutable.WidthProperty).ProvideValue());
				thumb3.Bind(Layoutable.WidthProperty, new TemplateBinding(Layoutable.WidthProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)thumb4).EndInit();
				context.PopParent();
				((ISupportInitialize)track4).EndInit();
				context.PopParent();
				((ISupportInitialize)grid4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
			controlTheme.TargetType = typeof(ColorSlider);
			Style style;
			Style style2 = (style = new Style());
			context.PushParent(style);
			Style style3 = style;
			style3.Selector = ((Selector?)null).Nesting().Class(":horizontal");
			Setter setter = new Setter();
			setter.Property = TemplatedControl.BorderThicknessProperty;
			setter.Value = new Thickness(0.0, 0.0, 0.0, 0.0);
			style3.Add(setter);
			Setter setter2;
			Setter setter3 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter4 = setter2;
			setter4.Property = TemplatedControl.CornerRadiusProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ColorSliderCornerRadius");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter4.Value = value;
			context.PopParent();
			style3.Add(setter3);
			Setter setter5 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter6 = setter2;
			setter6.Property = Layoutable.HeightProperty;
			DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ColorSliderSize");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value2 = dynamicResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter6.Value = value2;
			context.PopParent();
			style3.Add(setter5);
			Setter setter7 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter8 = setter2;
			setter8.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value3 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			ControlTemplate controlTemplate2 = controlTemplate;
			controlTemplate2.TargetType = typeof(ColorSlider);
			controlTemplate2.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_17.Build, context);
			context.PopParent();
			setter8.Value = value3;
			context.PopParent();
			style3.Add(setter7);
			context.PopParent();
			controlTheme.Add(style2);
			Style style4 = (style = new Style());
			context.PushParent(style);
			Style style5 = style;
			style5.Selector = ((Selector?)null).Nesting().Class(":vertical");
			Setter setter9 = new Setter();
			setter9.Property = TemplatedControl.BorderThicknessProperty;
			setter9.Value = new Thickness(0.0, 0.0, 0.0, 0.0);
			style5.Add(setter9);
			Setter setter10 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter11 = setter2;
			setter11.Property = TemplatedControl.CornerRadiusProperty;
			DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ColorSliderCornerRadius");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value4 = dynamicResourceExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter11.Value = value4;
			context.PopParent();
			style5.Add(setter10);
			Setter setter12 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter13 = setter2;
			setter13.Property = Layoutable.WidthProperty;
			DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("ColorSliderSize");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value5 = dynamicResourceExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter13.Value = value5;
			context.PopParent();
			style5.Add(setter12);
			Setter setter14 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter15 = setter2;
			setter15.Property = TemplatedControl.TemplateProperty;
			ControlTemplate value6 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			ControlTemplate controlTemplate3 = controlTemplate;
			controlTemplate3.TargetType = typeof(ColorSlider);
			controlTemplate3.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_21.Build, context);
			context.PopParent();
			setter15.Value = value6;
			context.PopParent();
			style5.Add(setter14);
			context.PopParent();
			controlTheme.Add(style4);
			Style style6 = new Style();
			style6.Selector = ((Selector?)null).Nesting().Class(":pointerover").Template()
				.OfType(typeof(Thumb))
				.Name("ColorSliderThumb");
			Setter setter16 = new Setter();
			setter16.Property = Visual.OpacityProperty;
			setter16.Value = 0.75;
			style6.Add(setter16);
			controlTheme.Add(style6);
			Style style7 = new Style();
			style7.Selector = ((Selector?)null).Nesting().Class(":pointerover").Class(":dark-selector")
				.Template()
				.OfType(typeof(Thumb))
				.Name("ColorSliderThumb");
			Setter setter17 = new Setter();
			setter17.Property = Visual.OpacityProperty;
			setter17.Value = 0.7;
			style7.Add(setter17);
			controlTheme.Add(style7);
			Style style8 = new Style();
			style8.Selector = ((Selector?)null).Nesting().Class(":pointerover").Class(":light-selector")
				.Template()
				.OfType(typeof(Thumb))
				.Name("ColorSliderThumb");
			Setter setter18 = new Setter();
			setter18.Property = Visual.OpacityProperty;
			setter18.Value = 0.8;
			style8.Add(setter18);
			controlTheme.Add(style8);
			Style style9 = (style = new Style());
			context.PushParent(style);
			Style style10 = style;
			style10.Selector = ((Selector?)null).Nesting().Class(":dark-selector").Template()
				.OfType(typeof(Thumb))
				.Name("ColorSliderThumb");
			Setter setter19 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter20 = setter2;
			setter20.Property = TemplatedControl.BorderBrushProperty;
			DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("ColorControlDarkSelectorBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value7 = dynamicResourceExtension5.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter20.Value = value7;
			context.PopParent();
			style10.Add(setter19);
			context.PopParent();
			controlTheme.Add(style9);
			Style style11 = (style = new Style());
			context.PushParent(style);
			Style style12 = style;
			style12.Selector = ((Selector?)null).Nesting().Class(":light-selector").Template()
				.OfType(typeof(Thumb))
				.Name("ColorSliderThumb");
			Setter setter21 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter22 = setter2;
			setter22.Property = TemplatedControl.BorderBrushProperty;
			DynamicResourceExtension dynamicResourceExtension6 = new DynamicResourceExtension("ColorControlLightSelectorBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value8 = dynamicResourceExtension6.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter22.Value = value8;
			context.PopParent();
			style12.Add(setter21);
			context.PopParent();
			controlTheme.Add(style11);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_25
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new ContrastBrushConverter();
		}
	}

	private class XamlClosure_26
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new ColorToDisplayNameConverter();
		}
	}

	private class XamlClosure_27
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new DoNothingForNullConverter();
		}
	}

	private class XamlClosure_28
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new NumberFormatInfo
			{
				NumberDecimalDigits = 0
			};
		}
	}

	private class XamlClosure_29
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return PathGeometry.Parse("\n    M3 2C3.27614 2 3.5 2.22386 3.5 2.5V5.5C3.5 5.77614 3.72386 6 4 6H16C16.2761 6 16.5 5.77614\n    16.5 5.5V2.5C16.5 2.22386 16.7239 2 17 2C17.2761 2 17.5 2.22386 17.5 2.5V5.5C17.5 6.32843\n    16.8284 7 16 7H15.809L12.2236 14.1708C12.0615 14.4951 11.7914 14.7431 11.4695\n    14.8802C11.4905 15.0808 11.5 15.2891 11.5 15.5C11.5 16.0818 11.4278 16.6623 11.2268\n    17.1165C11.019 17.5862 10.6266 18 10 18C9.37343 18 8.98105 17.5862 8.77323 17.1165C8.57222\n    16.6623 8.5 16.0818 8.5 15.5C8.5 15.2891 8.50952 15.0808 8.53051 14.8802C8.20863 14.7431\n    7.93851 14.4951 7.77639 14.1708L4.19098 7H4C3.17157 7 2.5 6.32843 2.5 5.5V2.5C2.5 2.22386\n    2.72386 2 3 2ZM9.11803 14H10.882C11.0714 14 11.2445 13.893 11.3292 13.7236L14.691\n    7H5.30902L8.67082 13.7236C8.75552 13.893 8.92865 14 9.11803 14ZM9.52346 15C9.50787 15.1549\n    9.5 15.3225 9.5 15.5C9.5 16.0228 9.56841 16.4423 9.6877 16.7119C9.8002 16.9661 9.90782 17\n    10 17C10.0922 17 10.1998 16.9661 10.3123 16.7119C10.4316 16.4423 10.5 16.0228 10.5\n    15.5C10.5 15.3225 10.4921 15.1549 10.4765 15H9.52346Z\n  ");
		}
	}

	private class XamlClosure_30
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return PathGeometry.Parse("\n    M9.75003 6.5C10.1642 6.5 10.5 6.16421 10.5 5.75C10.5 5.33579 10.1642 5 9.75003 5C9.33582\n    5 9.00003 5.33579 9.00003 5.75C9.00003 6.16421 9.33582 6.5 9.75003 6.5ZM12.75 7.5C13.1642\n    7.5 13.5 7.16421 13.5 6.75C13.5 6.33579 13.1642 6 12.75 6C12.3358 6 12 6.33579 12 6.75C12\n    7.16421 12.3358 7.5 12.75 7.5ZM15.25 9C15.25 9.41421 14.9142 9.75 14.5 9.75C14.0858 9.75\n    13.75 9.41421 13.75 9C13.75 8.58579 14.0858 8.25 14.5 8.25C14.9142 8.25 15.25 8.58579\n    15.25 9ZM14.5 12.75C14.9142 12.75 15.25 12.4142 15.25 12C15.25 11.5858 14.9142 11.25 14.5\n    11.25C14.0858 11.25 13.75 11.5858 13.75 12C13.75 12.4142 14.0858 12.75 14.5 12.75ZM13.25\n    14C13.25 14.4142 12.9142 14.75 12.5 14.75C12.0858 14.75 11.75 14.4142 11.75 14C11.75\n    13.5858 12.0858 13.25 12.5 13.25C12.9142 13.25 13.25 13.5858 13.25 14ZM13.6972\n    2.99169C10.9426 1.57663 8.1432 1.7124 5.77007 3.16636C4.55909 3.9083 3.25331 5.46925\n    2.51605 7.05899C2.14542 7.85816 1.89915 8.70492 1.90238 9.49318C1.90566 10.2941 2.16983\n    11.0587 2.84039 11.6053C3.45058 12.1026 3.98165 12.353 4.49574 12.3784C5.01375 12.404\n    5.41804 12.1942 5.73429 12.0076C5.80382 11.9666 5.86891 11.927 5.93113 11.8892C6.17332\n    11.7421 6.37205 11.6214 6.62049 11.5426C6.90191 11.4534 7.2582 11.4205 7.77579\n    11.5787C7.96661 11.637 8.08161 11.7235 8.16212 11.8229C8.24792 11.9289 8.31662 12.0774\n    8.36788 12.2886C8.41955 12.5016 8.44767 12.7527 8.46868 13.0491C8.47651 13.1594 8.48379\n    13.2855 8.49142 13.4176C8.50252 13.6098 8.51437 13.8149 8.52974 14.0037C8.58435 14.6744\n    8.69971 15.4401 9.10362 16.1357C9.51764 16.8488 10.2047 17.439 11.307 17.8158C12.9093\n    18.3636 14.3731 17.9191 15.5126 17.0169C16.6391 16.125 17.4691 14.7761 17.8842\n    13.4272C19.1991 9.15377 17.6728 5.03394 13.6972 2.99169ZM6.29249 4.01905C8.35686 2.75426\n    10.7844 2.61959 13.2403 3.88119C16.7473 5.68275 18.1135 9.28161 16.9284 13.1332C16.5624\n    14.3227 15.8338 15.4871 14.8919 16.2329C13.963 16.9684 12.8486 17.286 11.6305\n    16.8696C10.7269 16.5607 10.2467 16.1129 9.96842 15.6336C9.68001 15.1369 9.57799 14.5556\n    9.52644 13.9225C9.51101 13.733 9.50132 13.5621 9.49147 13.3884C9.48399 13.2564 9.47642\n    13.1229 9.46618 12.9783C9.44424 12.669 9.41175 12.3499 9.33968 12.0529C9.26719 11.7541\n    9.14902 11.4527 8.93935 11.1937C8.72439 10.9282 8.43532 10.7346 8.06801 10.6223C7.36648\n    10.408 6.80266 10.4359 6.31839 10.5893C5.94331 10.7082 5.62016 10.9061 5.37179\n    11.0582C5.31992 11.0899 5.2713 11.1197 5.22616 11.1463C4.94094 11.3146 4.75357 11.39\n    4.54514 11.3796C4.33279 11.3691 4.00262 11.2625 3.47218 10.8301C3.0866 10.5158 2.90473\n    10.0668 2.90237 9.48908C2.89995 8.89865 3.08843 8.20165 3.42324 7.47971C4.09686 6.0272\n    5.28471 4.63649 6.29249 4.01905Z\n  ");
		}
	}

	private class XamlClosure_31
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return PathGeometry.Parse("\n    M14.95 5C14.7184 3.85888 13.7095 3 12.5 3C11.2905 3 10.2816 3.85888 10.05 5H2.5C2.22386\n    5 2 5.22386 2 5.5C2 5.77614 2.22386 6 2.5 6H10.05C10.2816 7.14112 11.2905 8 12.5 8C13.7297\n    8 14.752 7.11217 14.961 5.94254C14.9575 5.96177 14.9539 5.98093 14.95 6H17.5C17.7761 6 18\n    5.77614 18 5.5C18 5.22386 17.7761 5 17.5 5H14.95ZM12.5 7C11.6716 7 11 6.32843 11 5.5C11\n    4.67157 11.6716 4 12.5 4C13.3284 4 14 4.67157 14 5.5C14 6.32843 13.3284 7 12.5 7ZM9.94999\n    14C9.71836 12.8589 8.70948 12 7.5 12C6.29052 12 5.28164 12.8589 5.05001 14H2.5C2.22386\n    14 2 14.2239 2 14.5C2 14.7761 2.22386 15 2.5 15H5.05001C5.28164 16.1411 6.29052 17 7.5\n    17C8.70948 17 9.71836 16.1411 9.94999 15H17.5C17.7761 15 18 14.7761 18 14.5C18 14.2239\n    17.7761 14 17.5 14H9.94999ZM7.5 16C6.67157 16 6 15.3284 6 14.5C6 13.6716 6.67157 13 7.5\n    13C8.32843 13 9 13.6716 9 14.5C9 15.3284 8.32843 16 7.5 16Z\n  ");
		}
	}

	private class XamlClosure_32
	{
		private class XamlClosure_33
		{
			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
				((StyledElement)intermediateRoot).Name = "border";
				service = intermediateRoot;
				context.AvaloniaNameScope.Register("border", service);
				((AvaloniaObject)intermediateRoot).Bind(Visual.ClipToBoundsProperty, new TemplateBinding(Visual.ClipToBoundsProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				ScrollViewer scrollViewer;
				ScrollViewer scrollViewer2 = (scrollViewer = new ScrollViewer());
				((ISupportInitialize)scrollViewer2).BeginInit();
				((Decorator)intermediateRoot).Child = scrollViewer2;
				scrollViewer.Name = "PART_ScrollViewer";
				service = scrollViewer;
				context.AvaloniaNameScope.Register("PART_ScrollViewer", service);
				scrollViewer.Bind(ScrollViewer.HorizontalScrollBarVisibilityProperty, new TemplateBinding(ScrollViewer.HorizontalScrollBarVisibilityProperty).ProvideValue());
				scrollViewer.Bind(ScrollViewer.VerticalScrollBarVisibilityProperty, new TemplateBinding(ScrollViewer.VerticalScrollBarVisibilityProperty).ProvideValue());
				scrollViewer.Bind(ScrollViewer.IsScrollChainingEnabledProperty, new TemplateBinding(ScrollViewer.IsScrollChainingEnabledProperty).ProvideValue());
				scrollViewer.Bind(ScrollViewer.AllowAutoHideProperty, new TemplateBinding(ScrollViewer.AllowAutoHideProperty).ProvideValue());
				ItemsPresenter itemsPresenter;
				ItemsPresenter itemsPresenter2 = (itemsPresenter = new ItemsPresenter());
				((ISupportInitialize)itemsPresenter2).BeginInit();
				scrollViewer.Content = itemsPresenter2;
				itemsPresenter.Name = "PART_ItemsPresenter";
				service = itemsPresenter;
				context.AvaloniaNameScope.Register("PART_ItemsPresenter", service);
				itemsPresenter.Bind(ItemsPresenter.ItemsPanelProperty, new TemplateBinding(ItemsControl.ItemsPanelProperty).ProvideValue());
				itemsPresenter.Bind(Layoutable.MarginProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				((ISupportInitialize)itemsPresenter).EndInit();
				((ISupportInitialize)scrollViewer).EndInit();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
			controlTheme.TargetType = typeof(ListBox);
			Setter setter = new Setter();
			setter.Property = ScrollViewer.HorizontalScrollBarVisibilityProperty;
			setter.Value = ScrollBarVisibility.Disabled;
			controlTheme.Add(setter);
			Setter setter2 = new Setter();
			setter2.Property = ScrollViewer.VerticalScrollBarVisibilityProperty;
			setter2.Value = ScrollBarVisibility.Auto;
			controlTheme.Add(setter2);
			Setter setter3 = new Setter();
			setter3.Property = ScrollViewer.IsScrollChainingEnabledProperty;
			setter3.Value = true;
			controlTheme.Add(setter3);
			Setter setter4 = new Setter();
			setter4.Property = TemplatedControl.TemplateProperty;
			setter4.Value = new ControlTemplate
			{
				Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_33.Build, context)
			};
			controlTheme.Add(setter4);
			return controlTheme;
		}
	}

	private class XamlClosure_34
	{
		private class XamlClosure_35
		{
			private class DynamicSetters_36
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
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
				((AvaloniaObject)intermediateRoot).SetValue(Layoutable.UseLayoutRoundingProperty, value: false, BindingPriority.Template);
				Controls children = ((Panel)intermediateRoot).Children;
				ContentPresenter contentPresenter;
				ContentPresenter contentPresenter2 = (contentPresenter = new ContentPresenter());
				((ISupportInitialize)contentPresenter2).BeginInit();
				children.Add(contentPresenter2);
				contentPresenter.Name = "PART_ContentPresenter";
				service = contentPresenter;
				context.AvaloniaNameScope.Register("PART_ContentPresenter", service);
				contentPresenter.Bind(ContentPresenter.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.ContentTemplateProperty, new TemplateBinding(ContentControl.ContentTemplateProperty).ProvideValue());
				DynamicSetters_36.DynamicSetter_1(contentPresenter, BindingPriority.Template, new TemplateBinding(ContentControl.ContentProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.PaddingProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.VerticalContentAlignmentProperty, new TemplateBinding(ContentControl.VerticalContentAlignmentProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.HorizontalContentAlignmentProperty, new TemplateBinding(ContentControl.HorizontalContentAlignmentProperty).ProvideValue());
				((ISupportInitialize)contentPresenter).EndInit();
				Controls children2 = ((Panel)intermediateRoot).Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children2.Add(rectangle2);
				rectangle.Name = "BorderRectangle";
				service = rectangle;
				context.AvaloniaNameScope.Register("BorderRectangle", service);
				rectangle.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				rectangle.SetValue(Shape.StrokeThicknessProperty, 3.0, BindingPriority.Template);
				rectangle.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				((ISupportInitialize)rectangle).EndInit();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
			controlTheme.TargetType = typeof(ListBoxItem);
			Setter setter = new Setter();
			setter.Property = TemplatedControl.BackgroundProperty;
			setter.Value = new ImmutableSolidColorBrush(16777215u);
			controlTheme.Add(setter);
			Setter setter2 = new Setter();
			setter2.Property = TemplatedControl.PaddingProperty;
			setter2.Value = new Thickness(0.0, 0.0, 0.0, 0.0);
			controlTheme.Add(setter2);
			Setter setter3 = new Setter();
			setter3.Property = ContentControl.HorizontalContentAlignmentProperty;
			setter3.Value = HorizontalAlignment.Stretch;
			controlTheme.Add(setter3);
			Setter setter4 = new Setter();
			setter4.Property = ContentControl.VerticalContentAlignmentProperty;
			setter4.Value = VerticalAlignment.Stretch;
			controlTheme.Add(setter4);
			Setter setter5 = new Setter();
			setter5.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate = new ControlTemplate();
			controlTemplate.TargetType = typeof(ListBoxItem);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_35.Build, context);
			setter5.Value = controlTemplate;
			controlTheme.Add(setter5);
			Style style;
			Style style2 = (style = new Style());
			context.PushParent(style);
			Style style3 = style;
			style3.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(Rectangle))
				.Name("BorderRectangle");
			Setter setter6;
			Setter setter7 = (setter6 = new Setter());
			context.PushParent(setter6);
			Setter setter8 = setter6;
			setter8.Property = Shape.StrokeProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemControlHighlightListAccentLowBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter8.Value = value;
			context.PopParent();
			style3.Add(setter7);
			Setter setter9 = new Setter();
			setter9.Property = Visual.OpacityProperty;
			setter9.Value = 0.0;
			style3.Add(setter9);
			context.PopParent();
			controlTheme.Add(style2);
			Style style4 = (style = new Style());
			context.PushParent(style);
			Style style5 = style;
			style5.Selector = ((Selector?)null).Nesting().Class(":pointerover").Template()
				.OfType(typeof(Rectangle))
				.Name("BorderRectangle");
			Setter setter10 = (setter6 = new Setter());
			context.PushParent(setter6);
			Setter setter11 = setter6;
			setter11.Property = Shape.StrokeProperty;
			CompiledBindingExtension compiledBindingExtension;
			CompiledBindingExtension compiledBindingExtension2 = (compiledBindingExtension = new CompiledBindingExtension());
			context.PushParent(compiledBindingExtension);
			CompiledBindingExtension compiledBindingExtension3 = compiledBindingExtension;
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ContrastBrushConverter");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			compiledBindingExtension3.Converter = (IValueConverter)obj;
			context.PopParent();
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			CompiledBindingExtension value2 = compiledBindingExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter11.Value = value2;
			context.PopParent();
			style5.Add(setter10);
			Setter setter12 = new Setter();
			setter12.Property = Visual.OpacityProperty;
			setter12.Value = 0.5;
			style5.Add(setter12);
			context.PopParent();
			controlTheme.Add(style4);
			Style style6 = (style = new Style());
			context.PushParent(style);
			Style style7 = style;
			style7.Selector = ((Selector?)null).Nesting().Class(":selected").Template()
				.OfType(typeof(Rectangle))
				.Name("BorderRectangle");
			Setter setter13 = (setter6 = new Setter());
			context.PushParent(setter6);
			Setter setter14 = setter6;
			setter14.Property = Shape.StrokeProperty;
			CompiledBindingExtension compiledBindingExtension4 = (compiledBindingExtension = new CompiledBindingExtension());
			context.PushParent(compiledBindingExtension);
			CompiledBindingExtension compiledBindingExtension5 = compiledBindingExtension;
			StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("ContrastBrushConverter");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
			object? obj2 = staticResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			compiledBindingExtension5.Converter = (IValueConverter)obj2;
			context.PopParent();
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			CompiledBindingExtension value3 = compiledBindingExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter14.Value = value3;
			context.PopParent();
			style7.Add(setter13);
			Setter setter15 = new Setter();
			setter15.Property = Visual.OpacityProperty;
			setter15.Value = 1.0;
			style7.Add(setter15);
			context.PopParent();
			controlTheme.Add(style6);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_37
	{
		private class XamlClosure_38
		{
			private class DynamicSetters_39
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
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
				context.IntermediateRoot = new ContentPresenter();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				((StyledElement)intermediateRoot).Name = "PART_ContentPresenter";
				service = intermediateRoot;
				context.AvaloniaNameScope.Register("PART_ContentPresenter", service);
				((AvaloniaObject)intermediateRoot).Bind(ContentPresenter.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(ContentPresenter.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(ContentPresenter.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(ContentPresenter.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				DynamicSetters_39.DynamicSetter_1((ContentPresenter)intermediateRoot, BindingPriority.Template, new TemplateBinding(ContentControl.ContentProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(ContentPresenter.ContentTemplateProperty, new TemplateBinding(ContentControl.ContentTemplateProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(ContentPresenter.PaddingProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).SetValue(ContentPresenter.RecognizesAccessKeyProperty, value: true, BindingPriority.Template);
				((AvaloniaObject)intermediateRoot).Bind(ContentPresenter.HorizontalContentAlignmentProperty, new TemplateBinding(ContentControl.HorizontalContentAlignmentProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(ContentPresenter.VerticalContentAlignmentProperty, new TemplateBinding(ContentControl.VerticalContentAlignmentProperty).ProvideValue());
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
			controlTheme.TargetType = typeof(RadioButton);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = TemplatedControl.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ToggleButtonBackground");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Setter setter4 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter5 = setter;
			setter5.Property = TemplatedControl.ForegroundProperty;
			DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ToggleButtonForeground");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value2 = dynamicResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter5.Value = value2;
			context.PopParent();
			controlTheme.Add(setter4);
			Setter setter6 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter7 = setter;
			setter7.Property = TemplatedControl.BorderBrushProperty;
			DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ToggleButtonBorderBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value3 = dynamicResourceExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter7.Value = value3;
			context.PopParent();
			controlTheme.Add(setter6);
			Setter setter8 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter9 = setter;
			setter9.Property = TemplatedControl.BorderThicknessProperty;
			DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("ToggleButtonBorderThemeThickness");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value4 = dynamicResourceExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter9.Value = value4;
			context.PopParent();
			controlTheme.Add(setter8);
			Setter setter10 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter11 = setter;
			setter11.Property = TemplatedControl.CornerRadiusProperty;
			DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("ControlCornerRadius");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value5 = dynamicResourceExtension5.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter11.Value = value5;
			context.PopParent();
			controlTheme.Add(setter10);
			Setter setter12 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter13 = setter;
			setter13.Property = TemplatedControl.PaddingProperty;
			DynamicResourceExtension dynamicResourceExtension6 = new DynamicResourceExtension("ButtonPadding");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value6 = dynamicResourceExtension6.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter13.Value = value6;
			context.PopParent();
			controlTheme.Add(setter12);
			Setter setter14 = new Setter();
			setter14.Property = Layoutable.HorizontalAlignmentProperty;
			setter14.Value = HorizontalAlignment.Stretch;
			controlTheme.Add(setter14);
			Setter setter15 = new Setter();
			setter15.Property = Layoutable.VerticalAlignmentProperty;
			setter15.Value = VerticalAlignment.Center;
			controlTheme.Add(setter15);
			Setter setter16 = new Setter();
			setter16.Property = TemplatedControl.FontWeightProperty;
			setter16.Value = FontWeight.Normal;
			controlTheme.Add(setter16);
			Setter setter17 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter18 = setter;
			setter18.Property = TemplatedControl.FontSizeProperty;
			DynamicResourceExtension dynamicResourceExtension7 = new DynamicResourceExtension("ControlContentThemeFontSize");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value7 = dynamicResourceExtension7.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter18.Value = value7;
			context.PopParent();
			controlTheme.Add(setter17);
			Setter setter19 = new Setter();
			setter19.Property = ContentControl.HorizontalContentAlignmentProperty;
			setter19.Value = HorizontalAlignment.Center;
			controlTheme.Add(setter19);
			Setter setter20 = new Setter();
			setter20.Property = ContentControl.VerticalContentAlignmentProperty;
			setter20.Value = VerticalAlignment.Center;
			controlTheme.Add(setter20);
			Setter setter21 = new Setter();
			setter21.Property = TemplatedControl.TemplateProperty;
			setter21.Value = new ControlTemplate
			{
				Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_38.Build, context)
			};
			controlTheme.Add(setter21);
			Style style;
			Style style2 = (style = new Style());
			context.PushParent(style);
			Style style3 = style;
			style3.Selector = ((Selector?)null).Nesting().Class(":pointerover").Template()
				.OfType(typeof(ContentPresenter))
				.Name("PART_ContentPresenter");
			Setter setter22 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter23 = setter;
			setter23.Property = ContentPresenter.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension8 = new DynamicResourceExtension("ToggleButtonBackgroundPointerOver");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value8 = dynamicResourceExtension8.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter23.Value = value8;
			context.PopParent();
			style3.Add(setter22);
			Setter setter24 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter25 = setter;
			setter25.Property = ContentPresenter.BorderBrushProperty;
			DynamicResourceExtension dynamicResourceExtension9 = new DynamicResourceExtension("ToggleButtonBorderBrushPointerOver");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value9 = dynamicResourceExtension9.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter25.Value = value9;
			context.PopParent();
			style3.Add(setter24);
			Setter setter26 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter27 = setter;
			setter27.Property = ContentPresenter.ForegroundProperty;
			DynamicResourceExtension dynamicResourceExtension10 = new DynamicResourceExtension("ToggleButtonForegroundPointerOver");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value10 = dynamicResourceExtension10.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter27.Value = value10;
			context.PopParent();
			style3.Add(setter26);
			context.PopParent();
			controlTheme.Add(style2);
			Style style4 = (style = new Style());
			context.PushParent(style);
			Style style5 = style;
			style5.Selector = ((Selector?)null).Nesting().Class(":pressed").Template()
				.OfType(typeof(ContentPresenter))
				.Name("PART_ContentPresenter");
			Setter setter28 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter29 = setter;
			setter29.Property = ContentPresenter.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension11 = new DynamicResourceExtension("ToggleButtonBackgroundPressed");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value11 = dynamicResourceExtension11.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter29.Value = value11;
			context.PopParent();
			style5.Add(setter28);
			Setter setter30 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter31 = setter;
			setter31.Property = ContentPresenter.BorderBrushProperty;
			DynamicResourceExtension dynamicResourceExtension12 = new DynamicResourceExtension("ToggleButtonBorderBrushPressed");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value12 = dynamicResourceExtension12.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter31.Value = value12;
			context.PopParent();
			style5.Add(setter30);
			Setter setter32 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter33 = setter;
			setter33.Property = ContentPresenter.ForegroundProperty;
			DynamicResourceExtension dynamicResourceExtension13 = new DynamicResourceExtension("ToggleButtonForegroundPressed");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value13 = dynamicResourceExtension13.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter33.Value = value13;
			context.PopParent();
			style5.Add(setter32);
			context.PopParent();
			controlTheme.Add(style4);
			Style style6 = (style = new Style());
			context.PushParent(style);
			Style style7 = style;
			style7.Selector = ((Selector?)null).Nesting().Class(":checked");
			Style style8 = (style = new Style());
			context.PushParent(style);
			Style style9 = style;
			style9.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(ContentPresenter))
				.Name("PART_ContentPresenter");
			Setter setter34 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter35 = setter;
			setter35.Property = ContentPresenter.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension14 = new DynamicResourceExtension("ToggleButtonBackgroundChecked");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value14 = dynamicResourceExtension14.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter35.Value = value14;
			context.PopParent();
			style9.Add(setter34);
			Setter setter36 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter37 = setter;
			setter37.Property = ContentPresenter.BorderBrushProperty;
			DynamicResourceExtension dynamicResourceExtension15 = new DynamicResourceExtension("ToggleButtonBorderBrushChecked");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value15 = dynamicResourceExtension15.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter37.Value = value15;
			context.PopParent();
			style9.Add(setter36);
			Setter setter38 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter39 = setter;
			setter39.Property = ContentPresenter.ForegroundProperty;
			DynamicResourceExtension dynamicResourceExtension16 = new DynamicResourceExtension("ToggleButtonForegroundChecked");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value16 = dynamicResourceExtension16.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter39.Value = value16;
			context.PopParent();
			style9.Add(setter38);
			context.PopParent();
			style7.Add(style8);
			Style style10 = (style = new Style());
			context.PushParent(style);
			Style style11 = style;
			style11.Selector = ((Selector?)null).Nesting().Class(":pointerover").Template()
				.OfType(typeof(ContentPresenter))
				.Name("PART_ContentPresenter");
			Setter setter40 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter41 = setter;
			setter41.Property = ContentPresenter.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension17 = new DynamicResourceExtension("ToggleButtonBackgroundCheckedPointerOver");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value17 = dynamicResourceExtension17.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter41.Value = value17;
			context.PopParent();
			style11.Add(setter40);
			Setter setter42 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter43 = setter;
			setter43.Property = ContentPresenter.BorderBrushProperty;
			DynamicResourceExtension dynamicResourceExtension18 = new DynamicResourceExtension("ToggleButtonBorderBrushCheckedPointerOver");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value18 = dynamicResourceExtension18.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter43.Value = value18;
			context.PopParent();
			style11.Add(setter42);
			Setter setter44 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter45 = setter;
			setter45.Property = ContentPresenter.ForegroundProperty;
			DynamicResourceExtension dynamicResourceExtension19 = new DynamicResourceExtension("ToggleButtonForegroundCheckedPointerOver");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value19 = dynamicResourceExtension19.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter45.Value = value19;
			context.PopParent();
			style11.Add(setter44);
			context.PopParent();
			style7.Add(style10);
			Style style12 = (style = new Style());
			context.PushParent(style);
			Style style13 = style;
			style13.Selector = ((Selector?)null).Nesting().Class(":pressed").Template()
				.OfType(typeof(ContentPresenter))
				.Name("PART_ContentPresenter");
			Setter setter46 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter47 = setter;
			setter47.Property = ContentPresenter.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension20 = new DynamicResourceExtension("ToggleButtonBackgroundCheckedPressed");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value20 = dynamicResourceExtension20.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter47.Value = value20;
			context.PopParent();
			style13.Add(setter46);
			Setter setter48 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter49 = setter;
			setter49.Property = ContentPresenter.BorderBrushProperty;
			DynamicResourceExtension dynamicResourceExtension21 = new DynamicResourceExtension("ToggleButtonBorderBrushCheckedPressed");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value21 = dynamicResourceExtension21.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter49.Value = value21;
			context.PopParent();
			style13.Add(setter48);
			Setter setter50 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter51 = setter;
			setter51.Property = ContentPresenter.ForegroundProperty;
			DynamicResourceExtension dynamicResourceExtension22 = new DynamicResourceExtension("ToggleButtonForegroundCheckedPressed");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value22 = dynamicResourceExtension22.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter51.Value = value22;
			context.PopParent();
			style13.Add(setter50);
			context.PopParent();
			style7.Add(style12);
			context.PopParent();
			controlTheme.Add(style6);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_40
	{
		private class XamlClosure_41
		{
			private class DynamicSetters_42
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
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
				border2.Name = "PART_LayoutRoot";
				service = border2;
				context.AvaloniaNameScope.Register("PART_LayoutRoot", service);
				border2.Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				border2.Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				border2.Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				border2.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				border2.Bind(Decorator.PaddingProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				Panel panel;
				Panel panel2 = (panel = new Panel());
				((ISupportInitialize)panel2).BeginInit();
				border2.Child = panel2;
				Panel panel3;
				Panel panel4 = (panel3 = panel);
				context.PushParent(panel3);
				Controls children = panel3.Children;
				ContentPresenter contentPresenter;
				ContentPresenter contentPresenter2 = (contentPresenter = new ContentPresenter());
				((ISupportInitialize)contentPresenter2).BeginInit();
				children.Add(contentPresenter2);
				contentPresenter.Name = "PART_ContentPresenter";
				service = contentPresenter;
				context.AvaloniaNameScope.Register("PART_ContentPresenter", service);
				contentPresenter.Bind(Layoutable.HorizontalAlignmentProperty, new TemplateBinding(ContentControl.HorizontalContentAlignmentProperty).ProvideValue());
				contentPresenter.Bind(Layoutable.VerticalAlignmentProperty, new TemplateBinding(ContentControl.VerticalContentAlignmentProperty).ProvideValue());
				DynamicSetters_42.DynamicSetter_1(contentPresenter, BindingPriority.Template, new TemplateBinding(HeaderedContentControl.HeaderProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.ContentTemplateProperty, new TemplateBinding(HeaderedContentControl.HeaderTemplateProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.FontFamilyProperty, new TemplateBinding(TemplatedControl.FontFamilyProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.FontSizeProperty, new TemplateBinding(TemplatedControl.FontSizeProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.FontWeightProperty, new TemplateBinding(TemplatedControl.FontWeightProperty).ProvideValue());
				((ISupportInitialize)contentPresenter).EndInit();
				Controls children2 = panel3.Children;
				Border border3;
				Border border4 = (border3 = new Border());
				((ISupportInitialize)border4).BeginInit();
				children2.Add(border4);
				Border border5 = (border = border3);
				context.PushParent(border);
				Border border6 = border;
				border6.Name = "PART_SelectedPipe";
				service = border6;
				context.AvaloniaNameScope.Register("PART_SelectedPipe", service);
				StyledProperty<double> heightProperty = Layoutable.HeightProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("TabItemPipeThickness");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				IBinding binding = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border6.Bind(heightProperty, binding);
				border6.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 0.0, 2.0), BindingPriority.Template);
				border6.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border6.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Bottom, BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("TabItemHeaderSelectedPipeFill");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding2 = dynamicResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border6.Bind(backgroundProperty, binding2);
				StyledProperty<CornerRadius> cornerRadiusProperty = Border.CornerRadiusProperty;
				DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ControlCornerRadius");
				context.ProvideTargetProperty = Border.CornerRadiusProperty;
				IBinding binding3 = dynamicResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border6.Bind(cornerRadiusProperty, binding3);
				border6.SetValue(Visual.IsVisibleProperty, value: false, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)border5).EndInit();
				context.PopParent();
				((ISupportInitialize)panel4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
			controlTheme.TargetType = typeof(TabItem);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = TemplatedControl.FontSizeProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("TabItemHeaderFontSize");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Setter setter4 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter5 = setter;
			setter5.Property = TemplatedControl.FontWeightProperty;
			DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("TabItemHeaderThemeFontWeight");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value2 = dynamicResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter5.Value = value2;
			context.PopParent();
			controlTheme.Add(setter4);
			Setter setter6 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter7 = setter;
			setter7.Property = TemplatedControl.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("TabItemHeaderBackgroundUnselected");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value3 = dynamicResourceExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter7.Value = value3;
			context.PopParent();
			controlTheme.Add(setter6);
			Setter setter8 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter9 = setter;
			setter9.Property = TemplatedControl.ForegroundProperty;
			DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("TabItemHeaderForegroundUnselected");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value4 = dynamicResourceExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter9.Value = value4;
			context.PopParent();
			controlTheme.Add(setter8);
			Setter setter10 = new Setter();
			setter10.Property = TemplatedControl.PaddingProperty;
			setter10.Value = new Thickness(6.0, 0.0, 6.0, 0.0);
			controlTheme.Add(setter10);
			Setter setter11 = new Setter();
			setter11.Property = Layoutable.MarginProperty;
			setter11.Value = new Thickness(0.0, 0.0, 0.0, 0.0);
			controlTheme.Add(setter11);
			Setter setter12 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter13 = setter;
			setter13.Property = Layoutable.MinHeightProperty;
			DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("ColorViewTabStripHeight");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value5 = dynamicResourceExtension5.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter13.Value = value5;
			context.PopParent();
			controlTheme.Add(setter12);
			Setter setter14 = new Setter();
			setter14.Property = ContentControl.VerticalContentAlignmentProperty;
			setter14.Value = VerticalAlignment.Center;
			controlTheme.Add(setter14);
			Setter setter15 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter16 = setter;
			setter16.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value6 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_41.Build, context);
			context.PopParent();
			setter16.Value = value6;
			context.PopParent();
			controlTheme.Add(setter15);
			Style style;
			Style style2 = (style = new Style());
			context.PushParent(style);
			Style style3 = style;
			style3.Selector = ((Selector?)null).Nesting().Class(":selected");
			Setter setter17 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter18 = setter;
			setter18.Property = TemplatedControl.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension6 = new DynamicResourceExtension("TabItemHeaderBackgroundSelected");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value7 = dynamicResourceExtension6.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter18.Value = value7;
			context.PopParent();
			style3.Add(setter17);
			Setter setter19 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter20 = setter;
			setter20.Property = TemplatedControl.ForegroundProperty;
			DynamicResourceExtension dynamicResourceExtension7 = new DynamicResourceExtension("TabItemHeaderForegroundSelected");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value8 = dynamicResourceExtension7.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter20.Value = value8;
			context.PopParent();
			style3.Add(setter19);
			context.PopParent();
			controlTheme.Add(style2);
			Style style4 = new Style();
			style4.Selector = ((Selector?)null).Nesting().Class(":selected").Template()
				.OfType(typeof(Border))
				.Name("PART_SelectedPipe");
			Setter setter21 = new Setter();
			setter21.Property = Visual.IsVisibleProperty;
			setter21.Value = true;
			style4.Add(setter21);
			controlTheme.Add(style4);
			Style style5 = (style = new Style());
			context.PushParent(style);
			Style style6 = style;
			style6.Selector = ((Selector?)null).Nesting().Class(":pointerover").Template()
				.OfType(typeof(Border))
				.Name("PART_LayoutRoot");
			Setter setter22 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter23 = setter;
			setter23.Property = Border.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension8 = new DynamicResourceExtension("TabItemHeaderBackgroundUnselectedPointerOver");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value9 = dynamicResourceExtension8.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter23.Value = value9;
			context.PopParent();
			style6.Add(setter22);
			Setter setter24 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter25 = setter;
			setter25.Property = TextElement.ForegroundProperty;
			DynamicResourceExtension dynamicResourceExtension9 = new DynamicResourceExtension("TabItemHeaderForegroundUnselectedPointerOver");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value10 = dynamicResourceExtension9.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter25.Value = value10;
			context.PopParent();
			style6.Add(setter24);
			context.PopParent();
			controlTheme.Add(style5);
			Style style7 = (style = new Style());
			context.PushParent(style);
			Style style8 = style;
			style8.Selector = ((Selector?)null).Nesting().Class(":selected").Class(":pointerover")
				.Template()
				.OfType(typeof(Border))
				.Name("PART_LayoutRoot");
			Setter setter26 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter27 = setter;
			setter27.Property = Border.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension10 = new DynamicResourceExtension("TabItemHeaderBackgroundSelectedPointerOver");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value11 = dynamicResourceExtension10.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter27.Value = value11;
			context.PopParent();
			style8.Add(setter26);
			Setter setter28 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter29 = setter;
			setter29.Property = TextElement.ForegroundProperty;
			DynamicResourceExtension dynamicResourceExtension11 = new DynamicResourceExtension("TabItemHeaderForegroundSelectedPointerOver");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value12 = dynamicResourceExtension11.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter29.Value = value12;
			context.PopParent();
			style8.Add(setter28);
			context.PopParent();
			controlTheme.Add(style7);
			Style style9 = (style = new Style());
			context.PushParent(style);
			Style style10 = style;
			style10.Selector = ((Selector?)null).Nesting().Class(":pressed").Template()
				.OfType(typeof(Border))
				.Name("PART_LayoutRoot");
			Setter setter30 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter31 = setter;
			setter31.Property = Border.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension12 = new DynamicResourceExtension("TabItemHeaderBackgroundUnselectedPressed");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value13 = dynamicResourceExtension12.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter31.Value = value13;
			context.PopParent();
			style10.Add(setter30);
			Setter setter32 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter33 = setter;
			setter33.Property = TextElement.ForegroundProperty;
			DynamicResourceExtension dynamicResourceExtension13 = new DynamicResourceExtension("TabItemHeaderForegroundUnselectedPressed");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value14 = dynamicResourceExtension13.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter33.Value = value14;
			context.PopParent();
			style10.Add(setter32);
			context.PopParent();
			controlTheme.Add(style9);
			Style style11 = (style = new Style());
			context.PushParent(style);
			Style style12 = style;
			style12.Selector = ((Selector?)null).Nesting().Class(":selected").Class(":pressed")
				.Template()
				.OfType(typeof(Border))
				.Name("PART_LayoutRoot");
			Setter setter34 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter35 = setter;
			setter35.Property = Border.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension14 = new DynamicResourceExtension("TabItemHeaderBackgroundSelectedPressed");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value15 = dynamicResourceExtension14.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter35.Value = value15;
			context.PopParent();
			style12.Add(setter34);
			Setter setter36 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter37 = setter;
			setter37.Property = TextElement.ForegroundProperty;
			DynamicResourceExtension dynamicResourceExtension15 = new DynamicResourceExtension("TabItemHeaderForegroundSelectedPressed");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value16 = dynamicResourceExtension15.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter37.Value = value16;
			context.PopParent();
			style12.Add(setter36);
			context.PopParent();
			controlTheme.Add(style11);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_43
	{
		private class XamlClosure_44
		{
			private class XamlClosure_45
			{
				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
					if (P_0 != null)
					{
						object service = P_0.GetService(typeof(IRootObjectProvider));
						if (service != null)
						{
							service = ((IRootObjectProvider)service).RootObject;
							context.RootObject = (Styles)service;
						}
					}
					context.IntermediateRoot = new UniformGrid();
					object intermediateRoot = context.IntermediateRoot;
					((ISupportInitialize)intermediateRoot).BeginInit();
					((AvaloniaObject)intermediateRoot).SetValue(UniformGrid.ColumnsProperty, 0, BindingPriority.Template);
					((AvaloniaObject)intermediateRoot).SetValue(UniformGrid.RowsProperty, 1, BindingPriority.Template);
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			private class DynamicSetters_46
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

				public static void DynamicSetter_2(ItemsControl P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(ItemsControl.ItemContainerThemeProperty, binding);
						return;
					}
					if (P_2 is ControlTheme)
					{
						ControlTheme value = (ControlTheme)P_2;
						int priority = (int)P_1;
						P_0.SetValue(ItemsControl.ItemContainerThemeProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						ControlTheme value = (ControlTheme)P_2;
						int priority = (int)P_1;
						P_0.SetValue(ItemsControl.ItemContainerThemeProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
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

				public static void DynamicSetter_4(NumericUpDown P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(NumericUpDown.NumberFormatProperty, binding);
						return;
					}
					if (P_2 is NumberFormatInfo)
					{
						NumberFormatInfo value = (NumberFormatInfo)P_2;
						int priority = (int)P_1;
						P_0.SetValue(NumericUpDown.NumberFormatProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						NumberFormatInfo value = (NumberFormatInfo)P_2;
						int priority = (int)P_1;
						P_0.SetValue(NumericUpDown.NumberFormatProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
				}
			}

			private class XamlClosure_47
			{
				private class DynamicSetters_48
				{
					public static void DynamicSetter_1(ToolTip P_0, BindingPriority P_1, CompiledBindingExtension P_2)
					{
						if (P_2 != null)
						{
							IBinding binding = P_2;
							P_0.Bind(ToolTip.TipProperty, binding);
						}
						else
						{
							object value = P_2;
							int priority = (int)P_1;
							P_0.SetValue(ToolTip.TipProperty, value, (BindingPriority)priority);
						}
					}
				}

				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
					AttachedProperty<string?> nameProperty = AutomationProperties.NameProperty;
					CompiledBindingExtension compiledBindingExtension;
					CompiledBindingExtension compiledBindingExtension2 = (compiledBindingExtension = new CompiledBindingExtension());
					context.PushParent(compiledBindingExtension);
					CompiledBindingExtension compiledBindingExtension3 = compiledBindingExtension;
					StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ColorToDisplayNameConverter");
					context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
					object? obj = staticResourceExtension.ProvideValue(context);
					context.ProvideTargetProperty = null;
					compiledBindingExtension3.Converter = (IValueConverter)obj;
					context.PopParent();
					context.ProvideTargetProperty = AutomationProperties.NameProperty;
					CompiledBindingExtension binding = compiledBindingExtension2.ProvideValue(context);
					context.ProvideTargetProperty = null;
					border.Bind(nameProperty, binding);
					CompiledBindingExtension compiledBindingExtension4 = (compiledBindingExtension = new CompiledBindingExtension());
					context.PushParent(compiledBindingExtension);
					CompiledBindingExtension compiledBindingExtension5 = compiledBindingExtension;
					StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("ColorToDisplayNameConverter");
					context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
					object? obj2 = staticResourceExtension2.ProvideValue(context);
					context.ProvideTargetProperty = null;
					compiledBindingExtension5.Converter = (IValueConverter)obj2;
					context.PopParent();
					context.ProvideTargetProperty = ToolTip.TipProperty;
					CompiledBindingExtension compiledBindingExtension6 = compiledBindingExtension4.ProvideValue(context);
					context.ProvideTargetProperty = null;
					DynamicSetters_48.DynamicSetter_1((ToolTip)(object)border, BindingPriority.Template, compiledBindingExtension6);
					border.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
					border.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
					StyledProperty<IBrush?> backgroundProperty = Border.BackgroundProperty;
					SolidColorBrush solidColorBrush;
					SolidColorBrush value = (solidColorBrush = new SolidColorBrush());
					context.PushParent(solidColorBrush);
					StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
					CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension();
					context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
					CompiledBindingExtension binding2 = compiledBindingExtension7.ProvideValue(context);
					context.ProvideTargetProperty = null;
					solidColorBrush.Bind(colorProperty, binding2);
					context.PopParent();
					border.SetValue(backgroundProperty, value, BindingPriority.Template);
					context.PopParent();
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			private class XamlClosure_49
			{
				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
					if (P_0 != null)
					{
						object service = P_0.GetService(typeof(IRootObjectProvider));
						if (service != null)
						{
							service = ((IRootObjectProvider)service).RootObject;
							context.RootObject = (Styles)service;
						}
					}
					context.IntermediateRoot = new UniformGrid();
					object intermediateRoot = context.IntermediateRoot;
					((ISupportInitialize)intermediateRoot).BeginInit();
					UniformGrid uniformGrid = (UniformGrid)intermediateRoot;
					context.PushParent(uniformGrid);
					StyledProperty<int> columnsProperty = UniformGrid.ColumnsProperty;
					CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(ColorView), 0).Property(ColorView.PaletteColumnCountProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
					context.ProvideTargetProperty = UniformGrid.ColumnsProperty;
					CompiledBindingExtension binding = compiledBindingExtension.ProvideValue(context);
					context.ProvideTargetProperty = null;
					uniformGrid.Bind(columnsProperty, binding);
					context.PopParent();
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
				RowDefinitions rowDefinitions = new RowDefinitions();
				rowDefinitions.Capacity = 2;
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				grid2.RowDefinitions = rowDefinitions;
				Controls children = grid2.Children;
				Border border;
				Border border2 = (border = new Border());
				((ISupportInitialize)border2).BeginInit();
				children.Add(border2);
				Border border3;
				Border border4 = (border3 = border);
				context.PushParent(border3);
				Border border5 = border3;
				border5.Name = "TabBackgroundBorder";
				service = border5;
				context.AvaloniaNameScope.Register("TabBackgroundBorder", service);
				border5.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				border5.SetValue(Grid.RowSpanProperty, 2, BindingPriority.Template);
				border5.SetValue(Layoutable.HeightProperty, 48.0, BindingPriority.Template);
				border5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top, BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemControlBackgroundBaseLowBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border5.Bind(backgroundProperty, binding);
				StyledProperty<IBrush?> borderBrushProperty = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ColorViewTabBorderBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding2 = dynamicResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border5.Bind(borderBrushProperty, binding2);
				StyledProperty<CornerRadius> cornerRadiusProperty = Border.CornerRadiusProperty;
				DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ColorViewTabBackgroundCornerRadius");
				context.ProvideTargetProperty = Border.CornerRadiusProperty;
				IBinding binding3 = dynamicResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border5.Bind(cornerRadiusProperty, binding3);
				context.PopParent();
				((ISupportInitialize)border4).EndInit();
				Controls children2 = grid2.Children;
				Border border6;
				Border border7 = (border6 = new Border());
				((ISupportInitialize)border7).BeginInit();
				children2.Add(border7);
				Border border8 = (border3 = border6);
				context.PushParent(border3);
				Border border9 = border3;
				border9.Name = "ContentBackgroundBorder";
				service = border9;
				context.AvaloniaNameScope.Register("ContentBackgroundBorder", service);
				border9.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				border9.SetValue(Grid.RowSpanProperty, 2, BindingPriority.Template);
				border9.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 48.0, 0.0, 0.0), BindingPriority.Template);
				border9.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<CornerRadius> cornerRadiusProperty2 = Border.CornerRadiusProperty;
				TemplateBinding templateBinding;
				TemplateBinding templateBinding2 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding3 = templateBinding;
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("BottomCornerRadiusFilterConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding3.Converter = (IValueConverter)obj;
				context.PopParent();
				border9.Bind(cornerRadiusProperty2, templateBinding2.ProvideValue());
				StyledProperty<IBrush?> backgroundProperty2 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("ColorViewContentBackgroundBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding4 = dynamicResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border9.Bind(backgroundProperty2, binding4);
				StyledProperty<IBrush?> borderBrushProperty2 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("ColorViewContentBorderBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding5 = dynamicResourceExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border9.Bind(borderBrushProperty2, binding5);
				border9.SetValue(Border.BorderThicknessProperty, new Thickness(0.0, 1.0, 0.0, 0.0), BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)border8).EndInit();
				Controls children3 = grid2.Children;
				TabControl tabControl;
				TabControl tabControl2 = (tabControl = new TabControl());
				((ISupportInitialize)tabControl2).BeginInit();
				children3.Add(tabControl2);
				TabControl tabControl3;
				TabControl tabControl4 = (tabControl3 = tabControl);
				context.PushParent(tabControl3);
				tabControl3.Name = "PART_TabControl";
				service = tabControl3;
				context.AvaloniaNameScope.Register("PART_TabControl", service);
				tabControl3.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				tabControl3.SetValue(Layoutable.HeightProperty, 338.0, BindingPriority.Template);
				tabControl3.SetValue(Layoutable.WidthProperty, 350.0, BindingPriority.Template);
				tabControl3.SetValue(TemplatedControl.PaddingProperty, new Thickness(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				DirectProperty<SelectingItemsControl, int> selectedIndexProperty = SelectingItemsControl.SelectedIndexProperty;
				CompiledBindingExtension obj2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.SelectedIndexProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = SelectingItemsControl.SelectedIndexProperty;
				CompiledBindingExtension binding6 = obj2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				tabControl3.Bind(selectedIndexProperty, binding6);
				tabControl3.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate
				{
					Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_45.Build, context)
				}, BindingPriority.Template);
				ItemCollection items = tabControl3.Items;
				TabItem tabItem;
				TabItem tabItem2 = (tabItem = new TabItem());
				((ISupportInitialize)tabItem2).BeginInit();
				items.Add(tabItem2);
				TabItem tabItem3;
				TabItem tabItem4 = (tabItem3 = tabItem);
				context.PushParent(tabItem3);
				TabItem tabItem5 = tabItem3;
				StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("ColorViewTabItemTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj3 = staticResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_46.DynamicSetter_1(tabItem5, BindingPriority.Template, obj3);
				tabItem5.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorSpectrumVisibleProperty).ProvideValue());
				StyledProperty<object?> headerProperty = HeaderedContentControl.HeaderProperty;
				Border border10;
				Border border11 = (border10 = new Border());
				((ISupportInitialize)border11).BeginInit();
				tabItem5.SetValue(headerProperty, border11, BindingPriority.Template);
				Border border12 = (border3 = border10);
				context.PushParent(border3);
				Border border13 = border3;
				StyledProperty<double> heightProperty = Layoutable.HeightProperty;
				DynamicResourceExtension dynamicResourceExtension6 = new DynamicResourceExtension("ColorViewTabStripHeight");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				IBinding binding7 = dynamicResourceExtension6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border13.Bind(heightProperty, binding7);
				PathIcon pathIcon;
				PathIcon pathIcon2 = (pathIcon = new PathIcon());
				((ISupportInitialize)pathIcon2).BeginInit();
				border13.Child = pathIcon2;
				PathIcon pathIcon3;
				PathIcon pathIcon4 = (pathIcon3 = pathIcon);
				context.PushParent(pathIcon3);
				PathIcon pathIcon5 = pathIcon3;
				pathIcon5.SetValue(Layoutable.WidthProperty, 20.0, BindingPriority.Template);
				pathIcon5.SetValue(Layoutable.HeightProperty, 20.0, BindingPriority.Template);
				StyledProperty<Geometry> dataProperty = PathIcon.DataProperty;
				DynamicResourceExtension dynamicResourceExtension7 = new DynamicResourceExtension("ColorViewSpectrumIconGeometry");
				context.ProvideTargetProperty = PathIcon.DataProperty;
				IBinding binding8 = dynamicResourceExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				pathIcon5.Bind(dataProperty, binding8);
				context.PopParent();
				((ISupportInitialize)pathIcon4).EndInit();
				context.PopParent();
				((ISupportInitialize)border12).EndInit();
				Grid grid3;
				Grid grid4 = (grid3 = new Grid());
				((ISupportInitialize)grid4).BeginInit();
				tabItem5.Content = grid4;
				Grid grid5 = (grid = grid3);
				context.PushParent(grid);
				Grid grid6 = grid;
				RowDefinitions rowDefinitions2 = new RowDefinitions();
				rowDefinitions2.Capacity = 1;
				rowDefinitions2.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid6.RowDefinitions = rowDefinitions2;
				grid6.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 12.0, 12.0, 12.0), BindingPriority.Template);
				ColumnDefinitions columnDefinitions = grid6.ColumnDefinitions;
				ColumnDefinition columnDefinition = new ColumnDefinition();
				columnDefinition.SetValue(ColumnDefinition.WidthProperty, new GridLength(0.0, GridUnitType.Auto), BindingPriority.Template);
				columnDefinition.SetValue(ColumnDefinition.MinWidthProperty, 32.0, BindingPriority.Template);
				columnDefinitions.Add(columnDefinition);
				ColumnDefinitions columnDefinitions2 = grid6.ColumnDefinitions;
				ColumnDefinition columnDefinition2 = new ColumnDefinition();
				columnDefinition2.SetValue(ColumnDefinition.WidthProperty, new GridLength(1.0, GridUnitType.Star), BindingPriority.Template);
				columnDefinitions2.Add(columnDefinition2);
				ColumnDefinitions columnDefinitions3 = grid6.ColumnDefinitions;
				ColumnDefinition columnDefinition3 = new ColumnDefinition();
				columnDefinition3.SetValue(ColumnDefinition.WidthProperty, new GridLength(0.0, GridUnitType.Auto), BindingPriority.Template);
				columnDefinition3.SetValue(ColumnDefinition.MinWidthProperty, 32.0, BindingPriority.Template);
				columnDefinitions3.Add(columnDefinition3);
				Controls children4 = grid6.Children;
				ColorSlider colorSlider;
				ColorSlider colorSlider2 = (colorSlider = new ColorSlider());
				((ISupportInitialize)colorSlider2).BeginInit();
				children4.Add(colorSlider2);
				ColorSlider colorSlider3;
				ColorSlider colorSlider4 = (colorSlider3 = colorSlider);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider5 = colorSlider3;
				colorSlider5.Name = "ColorSpectrumThirdComponentSlider";
				service = colorSlider5;
				context.AvaloniaNameScope.Register("ColorSpectrumThirdComponentSlider", service);
				colorSlider5.SetValue(AutomationProperties.NameProperty, "Third Component", BindingPriority.Template);
				colorSlider5.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				colorSlider5.SetValue(ColorSlider.IsAlphaVisibleProperty, value: false, BindingPriority.Template);
				colorSlider5.SetValue(ColorSlider.IsPerceptiveProperty, value: true, BindingPriority.Template);
				colorSlider5.SetValue(Slider.OrientationProperty, Orientation.Vertical, BindingPriority.Template);
				colorSlider5.SetValue(ColorSlider.ColorModelProperty, ColorModel.Hsva, BindingPriority.Template);
				StyledProperty<ColorComponent> colorComponentProperty = ColorSlider.ColorComponentProperty;
				CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "ColorSpectrum").Property(ColorSpectrum.ThirdComponentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = ColorSlider.ColorComponentProperty;
				CompiledBindingExtension binding9 = compiledBindingExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider5.Bind(colorComponentProperty, binding9);
				StyledProperty<HsvColor> hsvColorProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension compiledBindingExtension2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "ColorSpectrum").Property(ColorSpectrum.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding10 = compiledBindingExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider5.Bind(hsvColorProperty, binding10);
				colorSlider5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				colorSlider5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				colorSlider5.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				colorSlider5.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorSpectrumSliderVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorSlider4).EndInit();
				Controls children5 = grid6.Children;
				ColorSpectrum colorSpectrum;
				ColorSpectrum colorSpectrum2 = (colorSpectrum = new ColorSpectrum());
				((ISupportInitialize)colorSpectrum2).BeginInit();
				children5.Add(colorSpectrum2);
				ColorSpectrum colorSpectrum3;
				ColorSpectrum colorSpectrum4 = (colorSpectrum3 = colorSpectrum);
				context.PushParent(colorSpectrum3);
				colorSpectrum3.Name = "ColorSpectrum";
				service = colorSpectrum3;
				context.AvaloniaNameScope.Register("ColorSpectrum", service);
				colorSpectrum3.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				colorSpectrum3.Bind(ColorSpectrum.ComponentsProperty, new TemplateBinding(ColorView.ColorSpectrumComponentsProperty).ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty2 = ColorSpectrum.HsvColorProperty;
				CompiledBindingExtension obj4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSpectrum.HsvColorProperty;
				CompiledBindingExtension binding11 = obj4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSpectrum3.Bind(hsvColorProperty2, binding11);
				colorSpectrum3.Bind(ColorSpectrum.MinHueProperty, new TemplateBinding(ColorView.MinHueProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MaxHueProperty, new TemplateBinding(ColorView.MaxHueProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MinSaturationProperty, new TemplateBinding(ColorView.MinSaturationProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MaxSaturationProperty, new TemplateBinding(ColorView.MaxSaturationProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MinValueProperty, new TemplateBinding(ColorView.MinValueProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MaxValueProperty, new TemplateBinding(ColorView.MaxValueProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.ShapeProperty, new TemplateBinding(ColorView.ColorSpectrumShapeProperty).ProvideValue());
				colorSpectrum3.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSpectrum3.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)colorSpectrum4).EndInit();
				Controls children6 = grid6.Children;
				ColorSlider colorSlider6;
				ColorSlider colorSlider7 = (colorSlider6 = new ColorSlider());
				((ISupportInitialize)colorSlider7).BeginInit();
				children6.Add(colorSlider7);
				ColorSlider colorSlider8 = (colorSlider3 = colorSlider6);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider9 = colorSlider3;
				colorSlider9.Name = "ColorSpectrumAlphaSlider";
				service = colorSlider9;
				context.AvaloniaNameScope.Register("ColorSpectrumAlphaSlider", service);
				colorSlider9.SetValue(AutomationProperties.NameProperty, "Alpha Component", BindingPriority.Template);
				colorSlider9.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider9.SetValue(Slider.OrientationProperty, Orientation.Vertical, BindingPriority.Template);
				colorSlider9.SetValue(ColorSlider.ColorModelProperty, ColorModel.Hsva, BindingPriority.Template);
				colorSlider9.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Alpha, BindingPriority.Template);
				StyledProperty<HsvColor> hsvColorProperty3 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "ColorSpectrum").Property(ColorSpectrum.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding12 = compiledBindingExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider9.Bind(hsvColorProperty3, binding12);
				colorSlider9.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				colorSlider9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				colorSlider9.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				colorSlider9.Bind(InputElement.IsEnabledProperty, new TemplateBinding(ColorView.IsAlphaEnabledProperty).ProvideValue());
				StyledProperty<bool> isVisibleProperty = Visual.IsVisibleProperty;
				MultiBinding multiBinding;
				MultiBinding binding13 = (multiBinding = new MultiBinding());
				context.PushParent(multiBinding);
				MultiBinding multiBinding2 = multiBinding;
				multiBinding2.Converter = BoolConverters.And;
				IList<IBinding> bindings = multiBinding2.Bindings;
				CompiledBindingExtension obj5 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsAlphaVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item = obj5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings.Add(item);
				context.PopParent();
				colorSlider9.Bind(isVisibleProperty, binding13);
				context.PopParent();
				((ISupportInitialize)colorSlider8).EndInit();
				context.PopParent();
				((ISupportInitialize)grid5).EndInit();
				context.PopParent();
				((ISupportInitialize)tabItem4).EndInit();
				ItemCollection items2 = tabControl3.Items;
				TabItem tabItem6;
				TabItem tabItem7 = (tabItem6 = new TabItem());
				((ISupportInitialize)tabItem7).BeginInit();
				items2.Add(tabItem7);
				TabItem tabItem8 = (tabItem3 = tabItem6);
				context.PushParent(tabItem3);
				TabItem tabItem9 = tabItem3;
				StaticResourceExtension staticResourceExtension3 = new StaticResourceExtension("ColorViewTabItemTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj6 = staticResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_46.DynamicSetter_1(tabItem9, BindingPriority.Template, obj6);
				tabItem9.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorPaletteVisibleProperty).ProvideValue());
				StyledProperty<object?> headerProperty2 = HeaderedContentControl.HeaderProperty;
				Border border14;
				Border border15 = (border14 = new Border());
				((ISupportInitialize)border15).BeginInit();
				tabItem9.SetValue(headerProperty2, border15, BindingPriority.Template);
				Border border16 = (border3 = border14);
				context.PushParent(border3);
				Border border17 = border3;
				StyledProperty<double> heightProperty2 = Layoutable.HeightProperty;
				DynamicResourceExtension dynamicResourceExtension8 = new DynamicResourceExtension("ColorViewTabStripHeight");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				IBinding binding14 = dynamicResourceExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border17.Bind(heightProperty2, binding14);
				PathIcon pathIcon6;
				PathIcon pathIcon7 = (pathIcon6 = new PathIcon());
				((ISupportInitialize)pathIcon7).BeginInit();
				border17.Child = pathIcon7;
				PathIcon pathIcon8 = (pathIcon3 = pathIcon6);
				context.PushParent(pathIcon3);
				PathIcon pathIcon9 = pathIcon3;
				pathIcon9.SetValue(Layoutable.WidthProperty, 20.0, BindingPriority.Template);
				pathIcon9.SetValue(Layoutable.HeightProperty, 20.0, BindingPriority.Template);
				StyledProperty<Geometry> dataProperty2 = PathIcon.DataProperty;
				DynamicResourceExtension dynamicResourceExtension9 = new DynamicResourceExtension("ColorViewPaletteIconGeometry");
				context.ProvideTargetProperty = PathIcon.DataProperty;
				IBinding binding15 = dynamicResourceExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				pathIcon9.Bind(dataProperty2, binding15);
				context.PopParent();
				((ISupportInitialize)pathIcon8).EndInit();
				context.PopParent();
				((ISupportInitialize)border16).EndInit();
				ListBox listBox;
				ListBox listBox2 = (listBox = new ListBox());
				((ISupportInitialize)listBox2).BeginInit();
				tabItem9.Content = listBox2;
				ListBox listBox3;
				ListBox listBox4 = (listBox3 = listBox);
				context.PushParent(listBox3);
				StaticResourceExtension staticResourceExtension4 = new StaticResourceExtension("ColorViewPaletteListBoxTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj7 = staticResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_46.DynamicSetter_1(listBox3, BindingPriority.Template, obj7);
				StaticResourceExtension staticResourceExtension5 = new StaticResourceExtension("ColorViewPaletteListBoxItemTheme");
				context.ProvideTargetProperty = ItemsControl.ItemContainerThemeProperty;
				object? obj8 = staticResourceExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_46.DynamicSetter_2(listBox3, BindingPriority.Template, obj8);
				listBox3.Bind(ItemsControl.ItemsSourceProperty, new TemplateBinding(ColorView.PaletteColorsProperty).ProvideValue());
				CompiledBindingExtension compiledBindingExtension4;
				CompiledBindingExtension compiledBindingExtension5 = (compiledBindingExtension4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.ColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build()));
				context.PushParent(compiledBindingExtension4);
				StaticResourceExtension staticResourceExtension6 = new StaticResourceExtension("DoNothingForNullConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
				object? obj9 = staticResourceExtension6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				compiledBindingExtension4.Converter = (IValueConverter)obj9;
				compiledBindingExtension4.Mode = BindingMode.TwoWay;
				context.PopParent();
				context.ProvideTargetProperty = SelectingItemsControl.SelectedItemProperty;
				CompiledBindingExtension compiledBindingExtension6 = compiledBindingExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_46.DynamicSetter_3(listBox3, compiledBindingExtension6);
				listBox3.SetValue(Layoutable.UseLayoutRoundingProperty, value: false, BindingPriority.Template);
				listBox3.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 12.0, 12.0, 12.0), BindingPriority.Template);
				StyledProperty<IDataTemplate?> itemTemplateProperty = ItemsControl.ItemTemplateProperty;
				DataTemplate dataTemplate;
				DataTemplate value = (dataTemplate = new DataTemplate());
				context.PushParent(dataTemplate);
				dataTemplate.DataType = typeof(Color);
				dataTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_47.Build, context);
				context.PopParent();
				listBox3.SetValue(itemTemplateProperty, value, BindingPriority.Template);
				StyledProperty<ITemplate<Panel?>> itemsPanelProperty = ItemsControl.ItemsPanelProperty;
				ItemsPanelTemplate itemsPanelTemplate;
				ItemsPanelTemplate value2 = (itemsPanelTemplate = new ItemsPanelTemplate());
				context.PushParent(itemsPanelTemplate);
				itemsPanelTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_49.Build, context);
				context.PopParent();
				listBox3.SetValue(itemsPanelProperty, value2, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)listBox4).EndInit();
				context.PopParent();
				((ISupportInitialize)tabItem8).EndInit();
				ItemCollection items3 = tabControl3.Items;
				TabItem tabItem10;
				TabItem tabItem11 = (tabItem10 = new TabItem());
				((ISupportInitialize)tabItem11).BeginInit();
				items3.Add(tabItem11);
				TabItem tabItem12 = (tabItem3 = tabItem10);
				context.PushParent(tabItem3);
				TabItem tabItem13 = tabItem3;
				StaticResourceExtension staticResourceExtension7 = new StaticResourceExtension("ColorViewTabItemTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj10 = staticResourceExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_46.DynamicSetter_1(tabItem13, BindingPriority.Template, obj10);
				tabItem13.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorComponentsVisibleProperty).ProvideValue());
				StyledProperty<object?> headerProperty3 = HeaderedContentControl.HeaderProperty;
				Border border18;
				Border border19 = (border18 = new Border());
				((ISupportInitialize)border19).BeginInit();
				tabItem13.SetValue(headerProperty3, border19, BindingPriority.Template);
				Border border20 = (border3 = border18);
				context.PushParent(border3);
				Border border21 = border3;
				StyledProperty<double> heightProperty3 = Layoutable.HeightProperty;
				DynamicResourceExtension dynamicResourceExtension10 = new DynamicResourceExtension("ColorViewTabStripHeight");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				IBinding binding16 = dynamicResourceExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border21.Bind(heightProperty3, binding16);
				PathIcon pathIcon10;
				PathIcon pathIcon11 = (pathIcon10 = new PathIcon());
				((ISupportInitialize)pathIcon11).BeginInit();
				border21.Child = pathIcon11;
				PathIcon pathIcon12 = (pathIcon3 = pathIcon10);
				context.PushParent(pathIcon3);
				PathIcon pathIcon13 = pathIcon3;
				pathIcon13.SetValue(Layoutable.WidthProperty, 20.0, BindingPriority.Template);
				pathIcon13.SetValue(Layoutable.HeightProperty, 20.0, BindingPriority.Template);
				StyledProperty<Geometry> dataProperty3 = PathIcon.DataProperty;
				DynamicResourceExtension dynamicResourceExtension11 = new DynamicResourceExtension("ColorViewComponentsIconGeometry");
				context.ProvideTargetProperty = PathIcon.DataProperty;
				IBinding binding17 = dynamicResourceExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				pathIcon13.Bind(dataProperty3, binding17);
				context.PopParent();
				((ISupportInitialize)pathIcon12).EndInit();
				context.PopParent();
				((ISupportInitialize)border20).EndInit();
				Grid grid7;
				Grid grid8 = (grid7 = new Grid());
				((ISupportInitialize)grid8).BeginInit();
				tabItem13.Content = grid8;
				Grid grid9 = (grid = grid7);
				context.PushParent(grid);
				Grid grid10 = grid;
				ColumnDefinitions columnDefinitions4 = new ColumnDefinitions();
				columnDefinitions4.Capacity = 3;
				columnDefinitions4.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions4.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions4.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid10.ColumnDefinitions = columnDefinitions4;
				RowDefinitions rowDefinitions3 = new RowDefinitions();
				rowDefinitions3.Capacity = 7;
				rowDefinitions3.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(24.0, GridUnitType.Pixel)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(12.0, GridUnitType.Pixel)));
				grid10.RowDefinitions = rowDefinitions3;
				grid10.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 12.0, 12.0, 12.0), BindingPriority.Template);
				Controls children7 = grid10.Children;
				Grid grid11;
				Grid grid12 = (grid11 = new Grid());
				((ISupportInitialize)grid12).BeginInit();
				children7.Add(grid12);
				Grid grid13 = (grid = grid11);
				context.PushParent(grid);
				Grid grid14 = grid;
				grid14.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				grid14.SetValue(Grid.ColumnSpanProperty, 3, BindingPriority.Template);
				grid14.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				ColumnDefinitions columnDefinitions5 = new ColumnDefinitions();
				columnDefinitions5.Capacity = 3;
				columnDefinitions5.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions5.Add(new ColumnDefinition(new GridLength(12.0, GridUnitType.Pixel)));
				columnDefinitions5.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid14.ColumnDefinitions = columnDefinitions5;
				Controls children8 = grid14.Children;
				Grid grid15;
				Grid grid16 = (grid15 = new Grid());
				((ISupportInitialize)grid16).BeginInit();
				children8.Add(grid16);
				Grid grid17 = (grid = grid15);
				context.PushParent(grid);
				Grid grid18 = grid;
				ColumnDefinitions columnDefinitions6 = new ColumnDefinitions();
				columnDefinitions6.Capacity = 2;
				columnDefinitions6.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions6.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid18.ColumnDefinitions = columnDefinitions6;
				grid18.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorModelVisibleProperty).ProvideValue());
				Controls children9 = grid18.Children;
				RadioButton radioButton;
				RadioButton radioButton2 = (radioButton = new RadioButton());
				((ISupportInitialize)radioButton2).BeginInit();
				children9.Add(radioButton2);
				RadioButton radioButton3;
				RadioButton radioButton4 = (radioButton3 = radioButton);
				context.PushParent(radioButton3);
				RadioButton radioButton5 = radioButton3;
				radioButton5.Name = "RgbRadioButton";
				service = radioButton5;
				context.AvaloniaNameScope.Register("RgbRadioButton", service);
				StaticResourceExtension staticResourceExtension8 = new StaticResourceExtension("ColorViewColorModelRadioButtonTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj11 = staticResourceExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_46.DynamicSetter_1(radioButton5, BindingPriority.Template, obj11);
				radioButton5.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				radioButton5.SetValue(ContentControl.ContentProperty, "RGB", BindingPriority.Template);
				radioButton5.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(4.0, 0.0, 0.0, 4.0), BindingPriority.Template);
				radioButton5.SetValue(TemplatedControl.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				StyledProperty<double> heightProperty4 = Layoutable.HeightProperty;
				CompiledBindingExtension obj12 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "PART_HexTextBox").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding18 = obj12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				radioButton5.Bind(heightProperty4, binding18);
				StyledProperty<bool?> isCheckedProperty = ToggleButton.IsCheckedProperty;
				TemplateBinding templateBinding4 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding5 = templateBinding;
				StaticResourceExtension staticResourceExtension9 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj13 = staticResourceExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding5.Converter = (IValueConverter)obj13;
				templateBinding5.ConverterParameter = ColorModel.Rgba;
				templateBinding5.Mode = BindingMode.TwoWay;
				context.PopParent();
				radioButton5.Bind(isCheckedProperty, templateBinding4.ProvideValue());
				context.PopParent();
				((ISupportInitialize)radioButton4).EndInit();
				Controls children10 = grid18.Children;
				RadioButton radioButton6;
				RadioButton radioButton7 = (radioButton6 = new RadioButton());
				((ISupportInitialize)radioButton7).BeginInit();
				children10.Add(radioButton7);
				RadioButton radioButton8 = (radioButton3 = radioButton6);
				context.PushParent(radioButton3);
				RadioButton radioButton9 = radioButton3;
				radioButton9.Name = "HsvRadioButton";
				service = radioButton9;
				context.AvaloniaNameScope.Register("HsvRadioButton", service);
				StaticResourceExtension staticResourceExtension10 = new StaticResourceExtension("ColorViewColorModelRadioButtonTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj14 = staticResourceExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_46.DynamicSetter_1(radioButton9, BindingPriority.Template, obj14);
				radioButton9.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				radioButton9.SetValue(ContentControl.ContentProperty, "HSV", BindingPriority.Template);
				radioButton9.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 4.0, 4.0, 0.0), BindingPriority.Template);
				radioButton9.SetValue(TemplatedControl.BorderThicknessProperty, new Thickness(0.0, 1.0, 1.0, 1.0), BindingPriority.Template);
				StyledProperty<double> heightProperty5 = Layoutable.HeightProperty;
				CompiledBindingExtension obj15 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "PART_HexTextBox").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding19 = obj15.ProvideValue(context);
				context.ProvideTargetProperty = null;
				radioButton9.Bind(heightProperty5, binding19);
				StyledProperty<bool?> isCheckedProperty2 = ToggleButton.IsCheckedProperty;
				TemplateBinding templateBinding6 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding7 = templateBinding;
				StaticResourceExtension staticResourceExtension11 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj16 = staticResourceExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding7.Converter = (IValueConverter)obj16;
				templateBinding7.ConverterParameter = ColorModel.Hsva;
				templateBinding7.Mode = BindingMode.TwoWay;
				context.PopParent();
				radioButton9.Bind(isCheckedProperty2, templateBinding6.ProvideValue());
				context.PopParent();
				((ISupportInitialize)radioButton8).EndInit();
				context.PopParent();
				((ISupportInitialize)grid17).EndInit();
				Controls children11 = grid14.Children;
				Grid grid19;
				Grid grid20 = (grid19 = new Grid());
				((ISupportInitialize)grid20).BeginInit();
				children11.Add(grid20);
				Grid grid21 = (grid = grid19);
				context.PushParent(grid);
				Grid grid22 = grid;
				grid22.Name = "HexInputGrid";
				service = grid22;
				context.AvaloniaNameScope.Register("HexInputGrid", service);
				grid22.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				grid22.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsHexInputVisibleProperty).ProvideValue());
				ColumnDefinitions columnDefinitions7 = grid22.ColumnDefinitions;
				ColumnDefinition columnDefinition4 = new ColumnDefinition();
				columnDefinition4.SetValue(ColumnDefinition.WidthProperty, new GridLength(30.0, GridUnitType.Pixel), BindingPriority.Template);
				columnDefinitions7.Add(columnDefinition4);
				ColumnDefinitions columnDefinitions8 = grid22.ColumnDefinitions;
				ColumnDefinition columnDefinition5 = new ColumnDefinition();
				columnDefinition5.SetValue(ColumnDefinition.WidthProperty, new GridLength(1.0, GridUnitType.Star), BindingPriority.Template);
				columnDefinitions8.Add(columnDefinition5);
				Controls children12 = grid22.Children;
				Border border22;
				Border border23 = (border22 = new Border());
				((ISupportInitialize)border23).BeginInit();
				children12.Add(border23);
				Border border24 = (border3 = border22);
				context.PushParent(border3);
				Border border25 = border3;
				border25.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border25.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty3 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension12 = new DynamicResourceExtension("TextControlBackgroundDisabled");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding20 = dynamicResourceExtension12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border25.Bind(backgroundProperty3, binding20);
				StyledProperty<IBrush?> borderBrushProperty3 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension13 = new DynamicResourceExtension("TextControlBorderBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding21 = dynamicResourceExtension13.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border25.Bind(borderBrushProperty3, binding21);
				border25.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border25.SetValue(Border.CornerRadiusProperty, new CornerRadius(4.0, 0.0, 0.0, 4.0), BindingPriority.Template);
				TextBlock textBlock;
				TextBlock textBlock2 = (textBlock = new TextBlock());
				((ISupportInitialize)textBlock2).BeginInit();
				border25.Child = textBlock2;
				TextBlock textBlock3;
				TextBlock textBlock4 = (textBlock3 = textBlock);
				context.PushParent(textBlock3);
				TextBlock textBlock5 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension14 = new DynamicResourceExtension("TextControlForegroundDisabled");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding22 = dynamicResourceExtension14.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock5.Bind(foregroundProperty, binding22);
				textBlock5.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock5.SetValue(TextBlock.TextProperty, "#", BindingPriority.Template);
				textBlock5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				textBlock5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)textBlock4).EndInit();
				context.PopParent();
				((ISupportInitialize)border24).EndInit();
				Controls children13 = grid22.Children;
				TextBox textBox;
				TextBox textBox2 = (textBox = new TextBox());
				((ISupportInitialize)textBox2).BeginInit();
				children13.Add(textBox2);
				textBox.Name = "PART_HexTextBox";
				service = textBox;
				context.AvaloniaNameScope.Register("PART_HexTextBox", service);
				textBox.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				textBox.SetValue(AutomationProperties.NameProperty, "Hexadecimal Color", BindingPriority.Template);
				textBox.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				textBox.SetValue(TextBox.MaxLengthProperty, 9, BindingPriority.Template);
				textBox.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				textBox.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 4.0, 4.0, 0.0), BindingPriority.Template);
				((ISupportInitialize)textBox).EndInit();
				context.PopParent();
				((ISupportInitialize)grid21).EndInit();
				context.PopParent();
				((ISupportInitialize)grid13).EndInit();
				Controls children14 = grid10.Children;
				Border border26;
				Border border27 = (border26 = new Border());
				((ISupportInitialize)border27).BeginInit();
				children14.Add(border27);
				Border border28 = (border3 = border26);
				context.PushParent(border3);
				Border border29 = border3;
				border29.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border29.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				StyledProperty<double> heightProperty6 = Layoutable.HeightProperty;
				CompiledBindingExtension obj17 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component1NumericUpDown").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding23 = obj17.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border29.Bind(heightProperty6, binding23);
				StyledProperty<double> widthProperty = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension15 = new DynamicResourceExtension("ColorViewComponentLabelWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding24 = dynamicResourceExtension15.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border29.Bind(widthProperty, binding24);
				StyledProperty<IBrush?> backgroundProperty4 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension16 = new DynamicResourceExtension("TextControlBackgroundDisabled");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding25 = dynamicResourceExtension16.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border29.Bind(backgroundProperty4, binding25);
				StyledProperty<IBrush?> borderBrushProperty4 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension17 = new DynamicResourceExtension("TextControlBorderBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding26 = dynamicResourceExtension17.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border29.Bind(borderBrushProperty4, binding26);
				border29.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border29.SetValue(Border.CornerRadiusProperty, new CornerRadius(4.0, 0.0, 0.0, 4.0), BindingPriority.Template);
				border29.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				border29.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				Panel panel;
				Panel panel2 = (panel = new Panel());
				((ISupportInitialize)panel2).BeginInit();
				border29.Child = panel2;
				Panel panel3;
				Panel panel4 = (panel3 = panel);
				context.PushParent(panel3);
				Panel panel5 = panel3;
				panel5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				panel5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Controls children15 = panel5.Children;
				TextBlock textBlock6;
				TextBlock textBlock7 = (textBlock6 = new TextBlock());
				((ISupportInitialize)textBlock7).BeginInit();
				children15.Add(textBlock7);
				TextBlock textBlock8 = (textBlock3 = textBlock6);
				context.PushParent(textBlock3);
				TextBlock textBlock9 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty2 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension18 = new DynamicResourceExtension("TextControlForegroundDisabled");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding27 = dynamicResourceExtension18.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock9.Bind(foregroundProperty2, binding27);
				textBlock9.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock9.SetValue(TextBlock.TextProperty, "R", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty2 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding8 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding9 = templateBinding;
				StaticResourceExtension staticResourceExtension12 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj18 = staticResourceExtension12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding9.Converter = (IValueConverter)obj18;
				templateBinding9.ConverterParameter = ColorModel.Rgba;
				templateBinding9.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock9.Bind(isVisibleProperty2, templateBinding8.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock8).EndInit();
				Controls children16 = panel5.Children;
				TextBlock textBlock10;
				TextBlock textBlock11 = (textBlock10 = new TextBlock());
				((ISupportInitialize)textBlock11).BeginInit();
				children16.Add(textBlock11);
				TextBlock textBlock12 = (textBlock3 = textBlock10);
				context.PushParent(textBlock3);
				TextBlock textBlock13 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty3 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension19 = new DynamicResourceExtension("TextControlForegroundDisabled");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding28 = dynamicResourceExtension19.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock13.Bind(foregroundProperty3, binding28);
				textBlock13.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock13.SetValue(TextBlock.TextProperty, "H", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty3 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding10 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding11 = templateBinding;
				StaticResourceExtension staticResourceExtension13 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj19 = staticResourceExtension13.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding11.Converter = (IValueConverter)obj19;
				templateBinding11.ConverterParameter = ColorModel.Hsva;
				templateBinding11.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock13.Bind(isVisibleProperty3, templateBinding10.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock12).EndInit();
				context.PopParent();
				((ISupportInitialize)panel4).EndInit();
				context.PopParent();
				((ISupportInitialize)border28).EndInit();
				Controls children17 = grid10.Children;
				NumericUpDown numericUpDown;
				NumericUpDown numericUpDown2 = (numericUpDown = new NumericUpDown());
				((ISupportInitialize)numericUpDown2).BeginInit();
				children17.Add(numericUpDown2);
				NumericUpDown numericUpDown3;
				NumericUpDown numericUpDown4 = (numericUpDown3 = numericUpDown);
				context.PushParent(numericUpDown3);
				NumericUpDown numericUpDown5 = numericUpDown3;
				numericUpDown5.Name = "Component1NumericUpDown";
				service = numericUpDown5;
				context.AvaloniaNameScope.Register("Component1NumericUpDown", service);
				numericUpDown5.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				numericUpDown5.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				numericUpDown5.SetValue(NumericUpDown.AllowSpinProperty, value: true, BindingPriority.Template);
				numericUpDown5.SetValue(NumericUpDown.ShowButtonSpinnerProperty, value: false, BindingPriority.Template);
				numericUpDown5.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<double> widthProperty2 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension20 = new DynamicResourceExtension("ColorViewComponentTextInputWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding29 = dynamicResourceExtension20.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown5.Bind(widthProperty2, binding29);
				numericUpDown5.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 4.0, 4.0, 0.0), BindingPriority.Template);
				numericUpDown5.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				numericUpDown5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension14 = new StaticResourceExtension("ColorViewComponentNumberFormat");
				context.ProvideTargetProperty = NumericUpDown.NumberFormatProperty;
				object? obj20 = staticResourceExtension14.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_46.DynamicSetter_4(numericUpDown5, BindingPriority.Template, obj20);
				StyledProperty<decimal> minimumProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component1Slider").Property(RangeBase.MinimumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension binding30 = compiledBindingExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown5.Bind(minimumProperty, binding30);
				StyledProperty<decimal> maximumProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension compiledBindingExtension8 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component1Slider").Property(RangeBase.MaximumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension binding31 = compiledBindingExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown5.Bind(maximumProperty, binding31);
				StyledProperty<decimal?> valueProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension compiledBindingExtension9 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component1Slider").Property(RangeBase.ValueProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension binding32 = compiledBindingExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown5.Bind(valueProperty, binding32);
				numericUpDown5.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)numericUpDown4).EndInit();
				Controls children18 = grid10.Children;
				ColorSlider colorSlider10;
				ColorSlider colorSlider11 = (colorSlider10 = new ColorSlider());
				((ISupportInitialize)colorSlider11).BeginInit();
				children18.Add(colorSlider11);
				ColorSlider colorSlider12 = (colorSlider3 = colorSlider10);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider13 = colorSlider3;
				colorSlider13.Name = "Component1Slider";
				service = colorSlider13;
				context.AvaloniaNameScope.Register("Component1Slider", service);
				colorSlider13.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider13.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				colorSlider13.SetValue(Slider.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				colorSlider13.SetValue(ColorSlider.IsRoundingEnabledProperty, value: true, BindingPriority.Template);
				colorSlider13.SetValue(Slider.IsSnapToTickEnabledProperty, value: true, BindingPriority.Template);
				colorSlider13.SetValue(Slider.TickFrequencyProperty, 1.0, BindingPriority.Template);
				colorSlider13.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Component1, BindingPriority.Template);
				colorSlider13.Bind(ColorSlider.ColorModelProperty, new TemplateBinding(ColorView.ColorModelProperty)
				{
					Mode = BindingMode.OneWay
				}.ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty4 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension obj21 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding33 = obj21.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider13.Bind(hsvColorProperty4, binding33);
				colorSlider13.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSlider13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				colorSlider13.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentSliderVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorSlider12).EndInit();
				Controls children19 = grid10.Children;
				Border border30;
				Border border31 = (border30 = new Border());
				((ISupportInitialize)border31).BeginInit();
				children19.Add(border31);
				Border border32 = (border3 = border30);
				context.PushParent(border3);
				Border border33 = border3;
				border33.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border33.SetValue(Grid.RowProperty, 3, BindingPriority.Template);
				StyledProperty<double> widthProperty3 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension21 = new DynamicResourceExtension("ColorViewComponentLabelWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding34 = dynamicResourceExtension21.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border33.Bind(widthProperty3, binding34);
				StyledProperty<double> heightProperty7 = Layoutable.HeightProperty;
				CompiledBindingExtension obj22 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component2NumericUpDown").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding35 = obj22.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border33.Bind(heightProperty7, binding35);
				StyledProperty<IBrush?> backgroundProperty5 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension22 = new DynamicResourceExtension("TextControlBackgroundDisabled");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding36 = dynamicResourceExtension22.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border33.Bind(backgroundProperty5, binding36);
				StyledProperty<IBrush?> borderBrushProperty5 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension23 = new DynamicResourceExtension("TextControlBorderBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding37 = dynamicResourceExtension23.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border33.Bind(borderBrushProperty5, binding37);
				border33.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border33.SetValue(Border.CornerRadiusProperty, new CornerRadius(4.0, 0.0, 0.0, 4.0), BindingPriority.Template);
				border33.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				border33.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				Panel panel6;
				Panel panel7 = (panel6 = new Panel());
				((ISupportInitialize)panel7).BeginInit();
				border33.Child = panel7;
				Panel panel8 = (panel3 = panel6);
				context.PushParent(panel3);
				Panel panel9 = panel3;
				panel9.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				panel9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Controls children20 = panel9.Children;
				TextBlock textBlock14;
				TextBlock textBlock15 = (textBlock14 = new TextBlock());
				((ISupportInitialize)textBlock15).BeginInit();
				children20.Add(textBlock15);
				TextBlock textBlock16 = (textBlock3 = textBlock14);
				context.PushParent(textBlock3);
				TextBlock textBlock17 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty4 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension24 = new DynamicResourceExtension("TextControlForegroundDisabled");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding38 = dynamicResourceExtension24.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock17.Bind(foregroundProperty4, binding38);
				textBlock17.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock17.SetValue(TextBlock.TextProperty, "G", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty4 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding12 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding13 = templateBinding;
				StaticResourceExtension staticResourceExtension15 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj23 = staticResourceExtension15.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding13.Converter = (IValueConverter)obj23;
				templateBinding13.ConverterParameter = ColorModel.Rgba;
				templateBinding13.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock17.Bind(isVisibleProperty4, templateBinding12.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock16).EndInit();
				Controls children21 = panel9.Children;
				TextBlock textBlock18;
				TextBlock textBlock19 = (textBlock18 = new TextBlock());
				((ISupportInitialize)textBlock19).BeginInit();
				children21.Add(textBlock19);
				TextBlock textBlock20 = (textBlock3 = textBlock18);
				context.PushParent(textBlock3);
				TextBlock textBlock21 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty5 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension25 = new DynamicResourceExtension("TextControlForegroundDisabled");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding39 = dynamicResourceExtension25.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock21.Bind(foregroundProperty5, binding39);
				textBlock21.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock21.SetValue(TextBlock.TextProperty, "S", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty5 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding14 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding15 = templateBinding;
				StaticResourceExtension staticResourceExtension16 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj24 = staticResourceExtension16.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding15.Converter = (IValueConverter)obj24;
				templateBinding15.ConverterParameter = ColorModel.Hsva;
				templateBinding15.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock21.Bind(isVisibleProperty5, templateBinding14.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock20).EndInit();
				context.PopParent();
				((ISupportInitialize)panel8).EndInit();
				context.PopParent();
				((ISupportInitialize)border32).EndInit();
				Controls children22 = grid10.Children;
				NumericUpDown numericUpDown6;
				NumericUpDown numericUpDown7 = (numericUpDown6 = new NumericUpDown());
				((ISupportInitialize)numericUpDown7).BeginInit();
				children22.Add(numericUpDown7);
				NumericUpDown numericUpDown8 = (numericUpDown3 = numericUpDown6);
				context.PushParent(numericUpDown3);
				NumericUpDown numericUpDown9 = numericUpDown3;
				numericUpDown9.Name = "Component2NumericUpDown";
				service = numericUpDown9;
				context.AvaloniaNameScope.Register("Component2NumericUpDown", service);
				numericUpDown9.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				numericUpDown9.SetValue(Grid.RowProperty, 3, BindingPriority.Template);
				numericUpDown9.SetValue(NumericUpDown.AllowSpinProperty, value: true, BindingPriority.Template);
				numericUpDown9.SetValue(NumericUpDown.ShowButtonSpinnerProperty, value: false, BindingPriority.Template);
				numericUpDown9.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<double> widthProperty4 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension26 = new DynamicResourceExtension("ColorViewComponentTextInputWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding40 = dynamicResourceExtension26.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown9.Bind(widthProperty4, binding40);
				numericUpDown9.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 4.0, 4.0, 0.0), BindingPriority.Template);
				numericUpDown9.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				numericUpDown9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension17 = new StaticResourceExtension("ColorViewComponentNumberFormat");
				context.ProvideTargetProperty = NumericUpDown.NumberFormatProperty;
				object? obj25 = staticResourceExtension17.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_46.DynamicSetter_4(numericUpDown9, BindingPriority.Template, obj25);
				StyledProperty<decimal> minimumProperty2 = NumericUpDown.MinimumProperty;
				CompiledBindingExtension compiledBindingExtension10 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component2Slider").Property(RangeBase.MinimumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension binding41 = compiledBindingExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown9.Bind(minimumProperty2, binding41);
				StyledProperty<decimal> maximumProperty2 = NumericUpDown.MaximumProperty;
				CompiledBindingExtension compiledBindingExtension11 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component2Slider").Property(RangeBase.MaximumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension binding42 = compiledBindingExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown9.Bind(maximumProperty2, binding42);
				StyledProperty<decimal?> valueProperty2 = NumericUpDown.ValueProperty;
				CompiledBindingExtension compiledBindingExtension12 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component2Slider").Property(RangeBase.ValueProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension binding43 = compiledBindingExtension12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown9.Bind(valueProperty2, binding43);
				numericUpDown9.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)numericUpDown8).EndInit();
				Controls children23 = grid10.Children;
				ColorSlider colorSlider14;
				ColorSlider colorSlider15 = (colorSlider14 = new ColorSlider());
				((ISupportInitialize)colorSlider15).BeginInit();
				children23.Add(colorSlider15);
				ColorSlider colorSlider16 = (colorSlider3 = colorSlider14);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider17 = colorSlider3;
				colorSlider17.Name = "Component2Slider";
				service = colorSlider17;
				context.AvaloniaNameScope.Register("Component2Slider", service);
				colorSlider17.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider17.SetValue(Grid.RowProperty, 3, BindingPriority.Template);
				colorSlider17.SetValue(Slider.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				colorSlider17.SetValue(ColorSlider.IsRoundingEnabledProperty, value: true, BindingPriority.Template);
				colorSlider17.SetValue(Slider.IsSnapToTickEnabledProperty, value: true, BindingPriority.Template);
				colorSlider17.SetValue(Slider.TickFrequencyProperty, 1.0, BindingPriority.Template);
				colorSlider17.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Component2, BindingPriority.Template);
				colorSlider17.Bind(ColorSlider.ColorModelProperty, new TemplateBinding(ColorView.ColorModelProperty)
				{
					Mode = BindingMode.OneWay
				}.ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty5 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension obj26 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding44 = obj26.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider17.Bind(hsvColorProperty5, binding44);
				colorSlider17.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSlider17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				colorSlider17.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentSliderVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorSlider16).EndInit();
				Controls children24 = grid10.Children;
				Border border34;
				Border border35 = (border34 = new Border());
				((ISupportInitialize)border35).BeginInit();
				children24.Add(border35);
				Border border36 = (border3 = border34);
				context.PushParent(border3);
				Border border37 = border3;
				border37.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border37.SetValue(Grid.RowProperty, 4, BindingPriority.Template);
				StyledProperty<double> widthProperty5 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension27 = new DynamicResourceExtension("ColorViewComponentLabelWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding45 = dynamicResourceExtension27.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border37.Bind(widthProperty5, binding45);
				StyledProperty<double> heightProperty8 = Layoutable.HeightProperty;
				CompiledBindingExtension obj27 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component3NumericUpDown").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding46 = obj27.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border37.Bind(heightProperty8, binding46);
				StyledProperty<IBrush?> backgroundProperty6 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension28 = new DynamicResourceExtension("TextControlBackgroundDisabled");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding47 = dynamicResourceExtension28.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border37.Bind(backgroundProperty6, binding47);
				StyledProperty<IBrush?> borderBrushProperty6 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension29 = new DynamicResourceExtension("TextControlBorderBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding48 = dynamicResourceExtension29.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border37.Bind(borderBrushProperty6, binding48);
				border37.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border37.SetValue(Border.CornerRadiusProperty, new CornerRadius(4.0, 0.0, 0.0, 4.0), BindingPriority.Template);
				border37.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				border37.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				Panel panel10;
				Panel panel11 = (panel10 = new Panel());
				((ISupportInitialize)panel11).BeginInit();
				border37.Child = panel11;
				Panel panel12 = (panel3 = panel10);
				context.PushParent(panel3);
				Panel panel13 = panel3;
				panel13.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				panel13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Controls children25 = panel13.Children;
				TextBlock textBlock22;
				TextBlock textBlock23 = (textBlock22 = new TextBlock());
				((ISupportInitialize)textBlock23).BeginInit();
				children25.Add(textBlock23);
				TextBlock textBlock24 = (textBlock3 = textBlock22);
				context.PushParent(textBlock3);
				TextBlock textBlock25 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty6 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension30 = new DynamicResourceExtension("TextControlForegroundDisabled");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding49 = dynamicResourceExtension30.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock25.Bind(foregroundProperty6, binding49);
				textBlock25.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock25.SetValue(TextBlock.TextProperty, "B", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty6 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding16 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding17 = templateBinding;
				StaticResourceExtension staticResourceExtension18 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj28 = staticResourceExtension18.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding17.Converter = (IValueConverter)obj28;
				templateBinding17.ConverterParameter = ColorModel.Rgba;
				templateBinding17.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock25.Bind(isVisibleProperty6, templateBinding16.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock24).EndInit();
				Controls children26 = panel13.Children;
				TextBlock textBlock26;
				TextBlock textBlock27 = (textBlock26 = new TextBlock());
				((ISupportInitialize)textBlock27).BeginInit();
				children26.Add(textBlock27);
				TextBlock textBlock28 = (textBlock3 = textBlock26);
				context.PushParent(textBlock3);
				TextBlock textBlock29 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty7 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension31 = new DynamicResourceExtension("TextControlForegroundDisabled");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding50 = dynamicResourceExtension31.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock29.Bind(foregroundProperty7, binding50);
				textBlock29.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock29.SetValue(TextBlock.TextProperty, "V", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty7 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding18 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding19 = templateBinding;
				StaticResourceExtension staticResourceExtension19 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj29 = staticResourceExtension19.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding19.Converter = (IValueConverter)obj29;
				templateBinding19.ConverterParameter = ColorModel.Hsva;
				templateBinding19.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock29.Bind(isVisibleProperty7, templateBinding18.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock28).EndInit();
				context.PopParent();
				((ISupportInitialize)panel12).EndInit();
				context.PopParent();
				((ISupportInitialize)border36).EndInit();
				Controls children27 = grid10.Children;
				NumericUpDown numericUpDown10;
				NumericUpDown numericUpDown11 = (numericUpDown10 = new NumericUpDown());
				((ISupportInitialize)numericUpDown11).BeginInit();
				children27.Add(numericUpDown11);
				NumericUpDown numericUpDown12 = (numericUpDown3 = numericUpDown10);
				context.PushParent(numericUpDown3);
				NumericUpDown numericUpDown13 = numericUpDown3;
				numericUpDown13.Name = "Component3NumericUpDown";
				service = numericUpDown13;
				context.AvaloniaNameScope.Register("Component3NumericUpDown", service);
				numericUpDown13.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				numericUpDown13.SetValue(Grid.RowProperty, 4, BindingPriority.Template);
				numericUpDown13.SetValue(NumericUpDown.AllowSpinProperty, value: true, BindingPriority.Template);
				numericUpDown13.SetValue(NumericUpDown.ShowButtonSpinnerProperty, value: false, BindingPriority.Template);
				numericUpDown13.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<double> widthProperty6 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension32 = new DynamicResourceExtension("ColorViewComponentTextInputWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding51 = dynamicResourceExtension32.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown13.Bind(widthProperty6, binding51);
				numericUpDown13.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 4.0, 4.0, 0.0), BindingPriority.Template);
				numericUpDown13.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				numericUpDown13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension20 = new StaticResourceExtension("ColorViewComponentNumberFormat");
				context.ProvideTargetProperty = NumericUpDown.NumberFormatProperty;
				object? obj30 = staticResourceExtension20.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_46.DynamicSetter_4(numericUpDown13, BindingPriority.Template, obj30);
				StyledProperty<decimal> minimumProperty3 = NumericUpDown.MinimumProperty;
				CompiledBindingExtension compiledBindingExtension13 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component3Slider").Property(RangeBase.MinimumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension binding52 = compiledBindingExtension13.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown13.Bind(minimumProperty3, binding52);
				StyledProperty<decimal> maximumProperty3 = NumericUpDown.MaximumProperty;
				CompiledBindingExtension compiledBindingExtension14 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component3Slider").Property(RangeBase.MaximumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension binding53 = compiledBindingExtension14.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown13.Bind(maximumProperty3, binding53);
				StyledProperty<decimal?> valueProperty3 = NumericUpDown.ValueProperty;
				CompiledBindingExtension compiledBindingExtension15 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component3Slider").Property(RangeBase.ValueProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension binding54 = compiledBindingExtension15.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown13.Bind(valueProperty3, binding54);
				numericUpDown13.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)numericUpDown12).EndInit();
				Controls children28 = grid10.Children;
				ColorSlider colorSlider18;
				ColorSlider colorSlider19 = (colorSlider18 = new ColorSlider());
				((ISupportInitialize)colorSlider19).BeginInit();
				children28.Add(colorSlider19);
				ColorSlider colorSlider20 = (colorSlider3 = colorSlider18);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider21 = colorSlider3;
				colorSlider21.Name = "Component3Slider";
				service = colorSlider21;
				context.AvaloniaNameScope.Register("Component3Slider", service);
				colorSlider21.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider21.SetValue(Grid.RowProperty, 4, BindingPriority.Template);
				colorSlider21.SetValue(Slider.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				colorSlider21.SetValue(ColorSlider.IsRoundingEnabledProperty, value: true, BindingPriority.Template);
				colorSlider21.SetValue(Slider.IsSnapToTickEnabledProperty, value: true, BindingPriority.Template);
				colorSlider21.SetValue(Slider.TickFrequencyProperty, 1.0, BindingPriority.Template);
				colorSlider21.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Component3, BindingPriority.Template);
				colorSlider21.Bind(ColorSlider.ColorModelProperty, new TemplateBinding(ColorView.ColorModelProperty)
				{
					Mode = BindingMode.OneWay
				}.ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty6 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension obj31 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding55 = obj31.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider21.Bind(hsvColorProperty6, binding55);
				colorSlider21.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSlider21.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				colorSlider21.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentSliderVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorSlider20).EndInit();
				Controls children29 = grid10.Children;
				Border border38;
				Border border39 = (border38 = new Border());
				((ISupportInitialize)border39).BeginInit();
				children29.Add(border39);
				Border border40 = (border3 = border38);
				context.PushParent(border3);
				Border border41 = border3;
				border41.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border41.SetValue(Grid.RowProperty, 5, BindingPriority.Template);
				StyledProperty<double> widthProperty7 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension33 = new DynamicResourceExtension("ColorViewComponentLabelWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding56 = dynamicResourceExtension33.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border41.Bind(widthProperty7, binding56);
				StyledProperty<double> heightProperty9 = Layoutable.HeightProperty;
				CompiledBindingExtension obj32 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "AlphaComponentNumericUpDown").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding57 = obj32.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border41.Bind(heightProperty9, binding57);
				StyledProperty<IBrush?> backgroundProperty7 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension34 = new DynamicResourceExtension("TextControlBackgroundDisabled");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding58 = dynamicResourceExtension34.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border41.Bind(backgroundProperty7, binding58);
				StyledProperty<IBrush?> borderBrushProperty7 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension35 = new DynamicResourceExtension("TextControlBorderBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding59 = dynamicResourceExtension35.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border41.Bind(borderBrushProperty7, binding59);
				border41.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border41.SetValue(Border.CornerRadiusProperty, new CornerRadius(4.0, 0.0, 0.0, 4.0), BindingPriority.Template);
				border41.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				border41.Bind(InputElement.IsEnabledProperty, new TemplateBinding(ColorView.IsAlphaEnabledProperty).ProvideValue());
				StyledProperty<bool> isVisibleProperty8 = Visual.IsVisibleProperty;
				MultiBinding binding60 = (multiBinding = new MultiBinding());
				context.PushParent(multiBinding);
				MultiBinding multiBinding3 = multiBinding;
				multiBinding3.Converter = BoolConverters.And;
				IList<IBinding> bindings2 = multiBinding3.Bindings;
				CompiledBindingExtension obj33 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsAlphaVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item2 = obj33.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings2.Add(item2);
				IList<IBinding> bindings3 = multiBinding3.Bindings;
				CompiledBindingExtension obj34 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsComponentTextInputVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item3 = obj34.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings3.Add(item3);
				context.PopParent();
				border41.Bind(isVisibleProperty8, binding60);
				TextBlock textBlock30;
				TextBlock textBlock31 = (textBlock30 = new TextBlock());
				((ISupportInitialize)textBlock31).BeginInit();
				border41.Child = textBlock31;
				TextBlock textBlock32 = (textBlock3 = textBlock30);
				context.PushParent(textBlock3);
				TextBlock textBlock33 = textBlock3;
				textBlock33.Name = "AlphaComponentTextBlock";
				service = textBlock33;
				context.AvaloniaNameScope.Register("AlphaComponentTextBlock", service);
				StyledProperty<IBrush?> foregroundProperty8 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension36 = new DynamicResourceExtension("TextControlForegroundDisabled");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding61 = dynamicResourceExtension36.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock33.Bind(foregroundProperty8, binding61);
				textBlock33.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock33.SetValue(TextBlock.TextProperty, "A", BindingPriority.Template);
				textBlock33.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				textBlock33.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)textBlock32).EndInit();
				context.PopParent();
				((ISupportInitialize)border40).EndInit();
				Controls children30 = grid10.Children;
				NumericUpDown numericUpDown14;
				NumericUpDown numericUpDown15 = (numericUpDown14 = new NumericUpDown());
				((ISupportInitialize)numericUpDown15).BeginInit();
				children30.Add(numericUpDown15);
				NumericUpDown numericUpDown16 = (numericUpDown3 = numericUpDown14);
				context.PushParent(numericUpDown3);
				NumericUpDown numericUpDown17 = numericUpDown3;
				numericUpDown17.Name = "AlphaComponentNumericUpDown";
				service = numericUpDown17;
				context.AvaloniaNameScope.Register("AlphaComponentNumericUpDown", service);
				numericUpDown17.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				numericUpDown17.SetValue(Grid.RowProperty, 5, BindingPriority.Template);
				numericUpDown17.SetValue(NumericUpDown.AllowSpinProperty, value: true, BindingPriority.Template);
				numericUpDown17.SetValue(NumericUpDown.ShowButtonSpinnerProperty, value: false, BindingPriority.Template);
				numericUpDown17.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<double> widthProperty8 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension37 = new DynamicResourceExtension("ColorViewComponentTextInputWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding62 = dynamicResourceExtension37.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown17.Bind(widthProperty8, binding62);
				numericUpDown17.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 4.0, 4.0, 0.0), BindingPriority.Template);
				numericUpDown17.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				numericUpDown17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension21 = new StaticResourceExtension("ColorViewComponentNumberFormat");
				context.ProvideTargetProperty = NumericUpDown.NumberFormatProperty;
				object? obj35 = staticResourceExtension21.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_46.DynamicSetter_4(numericUpDown17, BindingPriority.Template, obj35);
				StyledProperty<decimal> minimumProperty4 = NumericUpDown.MinimumProperty;
				CompiledBindingExtension compiledBindingExtension16 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "AlphaComponentSlider").Property(RangeBase.MinimumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension binding63 = compiledBindingExtension16.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown17.Bind(minimumProperty4, binding63);
				StyledProperty<decimal> maximumProperty4 = NumericUpDown.MaximumProperty;
				CompiledBindingExtension compiledBindingExtension17 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "AlphaComponentSlider").Property(RangeBase.MaximumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension binding64 = compiledBindingExtension17.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown17.Bind(maximumProperty4, binding64);
				StyledProperty<decimal?> valueProperty4 = NumericUpDown.ValueProperty;
				CompiledBindingExtension compiledBindingExtension18 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "AlphaComponentSlider").Property(RangeBase.ValueProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension binding65 = compiledBindingExtension18.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown17.Bind(valueProperty4, binding65);
				numericUpDown17.Bind(InputElement.IsEnabledProperty, new TemplateBinding(ColorView.IsAlphaEnabledProperty).ProvideValue());
				StyledProperty<bool> isVisibleProperty9 = Visual.IsVisibleProperty;
				MultiBinding binding66 = (multiBinding = new MultiBinding());
				context.PushParent(multiBinding);
				MultiBinding multiBinding4 = multiBinding;
				multiBinding4.Converter = BoolConverters.And;
				IList<IBinding> bindings4 = multiBinding4.Bindings;
				CompiledBindingExtension obj36 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsAlphaVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item4 = obj36.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings4.Add(item4);
				IList<IBinding> bindings5 = multiBinding4.Bindings;
				CompiledBindingExtension obj37 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsComponentTextInputVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item5 = obj37.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings5.Add(item5);
				context.PopParent();
				numericUpDown17.Bind(isVisibleProperty9, binding66);
				context.PopParent();
				((ISupportInitialize)numericUpDown16).EndInit();
				Controls children31 = grid10.Children;
				ColorSlider colorSlider22;
				ColorSlider colorSlider23 = (colorSlider22 = new ColorSlider());
				((ISupportInitialize)colorSlider23).BeginInit();
				children31.Add(colorSlider23);
				ColorSlider colorSlider24 = (colorSlider3 = colorSlider22);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider25 = colorSlider3;
				colorSlider25.Name = "AlphaComponentSlider";
				service = colorSlider25;
				context.AvaloniaNameScope.Register("AlphaComponentSlider", service);
				colorSlider25.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider25.SetValue(Grid.RowProperty, 5, BindingPriority.Template);
				colorSlider25.SetValue(Slider.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				colorSlider25.SetValue(ColorSlider.IsRoundingEnabledProperty, value: true, BindingPriority.Template);
				colorSlider25.SetValue(Slider.IsSnapToTickEnabledProperty, value: true, BindingPriority.Template);
				colorSlider25.SetValue(Slider.TickFrequencyProperty, 1.0, BindingPriority.Template);
				colorSlider25.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Alpha, BindingPriority.Template);
				colorSlider25.Bind(ColorSlider.ColorModelProperty, new TemplateBinding(ColorView.ColorModelProperty)
				{
					Mode = BindingMode.OneWay
				}.ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty7 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension obj38 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding67 = obj38.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider25.Bind(hsvColorProperty7, binding67);
				colorSlider25.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSlider25.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				colorSlider25.Bind(InputElement.IsEnabledProperty, new TemplateBinding(ColorView.IsAlphaEnabledProperty).ProvideValue());
				StyledProperty<bool> isVisibleProperty10 = Visual.IsVisibleProperty;
				MultiBinding binding68 = (multiBinding = new MultiBinding());
				context.PushParent(multiBinding);
				MultiBinding multiBinding5 = multiBinding;
				multiBinding5.Converter = BoolConverters.And;
				IList<IBinding> bindings6 = multiBinding5.Bindings;
				CompiledBindingExtension obj39 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsAlphaVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item6 = obj39.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings6.Add(item6);
				IList<IBinding> bindings7 = multiBinding5.Bindings;
				CompiledBindingExtension obj40 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsComponentSliderVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item7 = obj40.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings7.Add(item7);
				context.PopParent();
				colorSlider25.Bind(isVisibleProperty10, binding68);
				context.PopParent();
				((ISupportInitialize)colorSlider24).EndInit();
				context.PopParent();
				((ISupportInitialize)grid9).EndInit();
				context.PopParent();
				((ISupportInitialize)tabItem12).EndInit();
				context.PopParent();
				((ISupportInitialize)tabControl4).EndInit();
				Controls children32 = grid2.Children;
				ColorPreviewer colorPreviewer;
				ColorPreviewer colorPreviewer2 = (colorPreviewer = new ColorPreviewer());
				((ISupportInitialize)colorPreviewer2).BeginInit();
				children32.Add(colorPreviewer2);
				ColorPreviewer colorPreviewer3;
				ColorPreviewer colorPreviewer4 = (colorPreviewer3 = colorPreviewer);
				context.PushParent(colorPreviewer3);
				colorPreviewer3.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
				StyledProperty<HsvColor> hsvColorProperty8 = ColorPreviewer.HsvColorProperty;
				CompiledBindingExtension obj41 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorPreviewer.HsvColorProperty;
				CompiledBindingExtension binding69 = obj41.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorPreviewer3.Bind(hsvColorProperty8, binding69);
				colorPreviewer3.Bind(ColorPreviewer.IsAccentColorsVisibleProperty, new TemplateBinding(ColorView.IsAccentColorsVisibleProperty).ProvideValue());
				colorPreviewer3.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 0.0, 12.0, 12.0), BindingPriority.Template);
				colorPreviewer3.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorPreviewVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorPreviewer4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
			controlTheme.TargetType = typeof(ColorView);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = TemplatedControl.CornerRadiusProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ControlCornerRadius");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Setter setter4 = new Setter();
			setter4.Property = ColorView.HexInputAlphaPositionProperty;
			setter4.Value = AlphaComponentPosition.Trailing;
			controlTheme.Add(setter4);
			Setter setter5 = new Setter();
			setter5.Property = ColorView.PaletteProperty;
			setter5.Value = new FluentColorPalette();
			controlTheme.Add(setter5);
			Setter setter6 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter7 = setter;
			setter7.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value2 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.TargetType = typeof(ColorView);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_44.Build, context);
			context.PopParent();
			setter7.Value = value2;
			context.PopParent();
			controlTheme.Add(setter6);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_50
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			VisualBrush visualBrush = new VisualBrush();
			visualBrush.TileMode = TileMode.Tile;
			visualBrush.Stretch = Stretch.Uniform;
			visualBrush.DestinationRect = RelativeRect.Parse("0,0,8,8");
			Image image;
			Image image2 = (image = new Image());
			((ISupportInitialize)image2).BeginInit();
			visualBrush.Visual = image2;
			image.Width = 8.0;
			image.Height = 8.0;
			DrawingImage drawingImage = new DrawingImage();
			DrawingGroup drawingGroup = new DrawingGroup();
			DrawingCollection children = drawingGroup.Children;
			GeometryDrawing geometryDrawing = new GeometryDrawing();
			geometryDrawing.Geometry = Geometry.Parse("M0,0 L2,0 2,2, 0,2Z");
			geometryDrawing.Brush = new ImmutableSolidColorBrush(16777215u);
			children.Add(geometryDrawing);
			DrawingCollection children2 = drawingGroup.Children;
			GeometryDrawing geometryDrawing2 = new GeometryDrawing();
			geometryDrawing2.Geometry = Geometry.Parse("M0,1 L2,1 2,2, 1,2 1,0 0,0Z");
			geometryDrawing2.Brush = new ImmutableSolidColorBrush(427851904u);
			children2.Add(geometryDrawing2);
			drawingImage.Drawing = drawingGroup;
			image.Source = drawingImage;
			((ISupportInitialize)image).EndInit();
			return visualBrush;
		}
	}

	private class XamlClosure_51
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
				Color = Color.FromUInt32(uint.MaxValue)
			};
		}
	}

	private class XamlClosure_52
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
				Color = Color.FromUInt32(3825205248u)
			};
		}
	}

	private class XamlClosure_53
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
				Color = Color.FromUInt32(16777215u)
			};
		}
	}

	private class XamlClosure_54
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
				Color = Color.FromUInt32(16777215u)
			};
		}
	}

	private class XamlClosure_55
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
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
				Color = Color.FromUInt32(16777215u)
			};
		}
	}

	private class XamlClosure_56
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new EnumToBoolConverter();
		}
	}

	private class XamlClosure_57
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new ToBrushConverter();
		}
	}

	private class XamlClosure_58
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new CornerRadiusFilterConverter
			{
				Filter = (Corners.TopLeft | Corners.BottomLeft)
			};
		}
	}

	private class XamlClosure_59
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new CornerRadiusFilterConverter
			{
				Filter = (Corners.TopRight | Corners.BottomRight)
			};
		}
	}

	private class XamlClosure_60
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new CornerRadiusFilterConverter
			{
				Filter = (Corners.TopLeft | Corners.TopRight)
			};
		}
	}

	private class XamlClosure_61
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new CornerRadiusFilterConverter
			{
				Filter = (Corners.BottomLeft | Corners.BottomRight)
			};
		}
	}

	private class XamlClosure_62
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new CornerRadiusToDoubleConverter
			{
				Corner = Corners.TopLeft
			};
		}
	}

	private class XamlClosure_63
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new CornerRadiusToDoubleConverter
			{
				Corner = Corners.BottomRight
			};
		}
	}

	public class NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
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
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.ColorPicker"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.ColorPicker")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("converters", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Controls.Converters", (string)null) });
			return dictionary;
		}

		static NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl();
		}
	}

	private class XamlClosure_64
	{
		private class XamlClosure_65
		{
			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
				Panel panel2 = panel;
				panel2.Name = "PART_LayoutRoot";
				service = panel2;
				context.AvaloniaNameScope.Register("PART_LayoutRoot", service);
				panel2.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				panel2.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				Controls children = panel2.Children;
				Panel panel3;
				Panel panel4 = (panel3 = new Panel());
				((ISupportInitialize)panel4).BeginInit();
				children.Add(panel4);
				Panel panel5 = (panel = panel3);
				context.PushParent(panel);
				Panel panel6 = panel;
				panel6.Name = "PART_SizingPanel";
				service = panel6;
				context.AvaloniaNameScope.Register("PART_SizingPanel", service);
				panel6.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				panel6.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				panel6.SetValue(Visual.ClipToBoundsProperty, value: true, BindingPriority.Template);
				Controls children2 = panel6.Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children2.Add(rectangle2);
				Rectangle rectangle3;
				Rectangle rectangle4 = (rectangle3 = rectangle);
				context.PushParent(rectangle3);
				Rectangle rectangle5 = rectangle3;
				rectangle5.Name = "PART_SpectrumRectangle";
				service = rectangle5;
				context.AvaloniaNameScope.Register("PART_SpectrumRectangle", service);
				rectangle5.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				rectangle5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty = Visual.IsVisibleProperty;
				TemplateBinding templateBinding;
				TemplateBinding templateBinding2 = (templateBinding = new TemplateBinding(ColorSpectrum.ShapeProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding3 = templateBinding;
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding3.Converter = (IValueConverter)obj;
				templateBinding3.ConverterParameter = ColorSpectrumShape.Box;
				context.PopParent();
				rectangle5.Bind(isVisibleProperty, templateBinding2.ProvideValue());
				StyledProperty<double> radiusXProperty = Rectangle.RadiusXProperty;
				TemplateBinding templateBinding4 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding5 = templateBinding;
				StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("TopLeftCornerRadiusConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj2 = staticResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding5.Converter = (IValueConverter)obj2;
				context.PopParent();
				rectangle5.Bind(radiusXProperty, templateBinding4.ProvideValue());
				StyledProperty<double> radiusYProperty = Rectangle.RadiusYProperty;
				TemplateBinding templateBinding6 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding7 = templateBinding;
				StaticResourceExtension staticResourceExtension3 = new StaticResourceExtension("BottomRightCornerRadiusConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj3 = staticResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding7.Converter = (IValueConverter)obj3;
				context.PopParent();
				rectangle5.Bind(radiusYProperty, templateBinding6.ProvideValue());
				context.PopParent();
				((ISupportInitialize)rectangle4).EndInit();
				Controls children3 = panel6.Children;
				Rectangle rectangle6;
				Rectangle rectangle7 = (rectangle6 = new Rectangle());
				((ISupportInitialize)rectangle7).BeginInit();
				children3.Add(rectangle7);
				Rectangle rectangle8 = (rectangle3 = rectangle6);
				context.PushParent(rectangle3);
				Rectangle rectangle9 = rectangle3;
				rectangle9.Name = "PART_SpectrumOverlayRectangle";
				service = rectangle9;
				context.AvaloniaNameScope.Register("PART_SpectrumOverlayRectangle", service);
				rectangle9.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				rectangle9.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty2 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding8 = (templateBinding = new TemplateBinding(ColorSpectrum.ShapeProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding9 = templateBinding;
				StaticResourceExtension staticResourceExtension4 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj4 = staticResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding9.Converter = (IValueConverter)obj4;
				templateBinding9.ConverterParameter = ColorSpectrumShape.Box;
				context.PopParent();
				rectangle9.Bind(isVisibleProperty2, templateBinding8.ProvideValue());
				StyledProperty<double> radiusXProperty2 = Rectangle.RadiusXProperty;
				TemplateBinding templateBinding10 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding11 = templateBinding;
				StaticResourceExtension staticResourceExtension5 = new StaticResourceExtension("TopLeftCornerRadiusConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj5 = staticResourceExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding11.Converter = (IValueConverter)obj5;
				context.PopParent();
				rectangle9.Bind(radiusXProperty2, templateBinding10.ProvideValue());
				StyledProperty<double> radiusYProperty2 = Rectangle.RadiusYProperty;
				TemplateBinding templateBinding12 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding13 = templateBinding;
				StaticResourceExtension staticResourceExtension6 = new StaticResourceExtension("BottomRightCornerRadiusConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj6 = staticResourceExtension6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding13.Converter = (IValueConverter)obj6;
				context.PopParent();
				rectangle9.Bind(radiusYProperty2, templateBinding12.ProvideValue());
				context.PopParent();
				((ISupportInitialize)rectangle8).EndInit();
				Controls children4 = panel6.Children;
				Ellipse ellipse;
				Ellipse ellipse2 = (ellipse = new Ellipse());
				((ISupportInitialize)ellipse2).BeginInit();
				children4.Add(ellipse2);
				Ellipse ellipse3;
				Ellipse ellipse4 = (ellipse3 = ellipse);
				context.PushParent(ellipse3);
				Ellipse ellipse5 = ellipse3;
				ellipse5.Name = "PART_SpectrumEllipse";
				service = ellipse5;
				context.AvaloniaNameScope.Register("PART_SpectrumEllipse", service);
				ellipse5.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				ellipse5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				ellipse5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty3 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding14 = (templateBinding = new TemplateBinding(ColorSpectrum.ShapeProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding15 = templateBinding;
				StaticResourceExtension staticResourceExtension7 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj7 = staticResourceExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding15.Converter = (IValueConverter)obj7;
				templateBinding15.ConverterParameter = ColorSpectrumShape.Ring;
				context.PopParent();
				ellipse5.Bind(isVisibleProperty3, templateBinding14.ProvideValue());
				context.PopParent();
				((ISupportInitialize)ellipse4).EndInit();
				Controls children5 = panel6.Children;
				Ellipse ellipse6;
				Ellipse ellipse7 = (ellipse6 = new Ellipse());
				((ISupportInitialize)ellipse7).BeginInit();
				children5.Add(ellipse7);
				Ellipse ellipse8 = (ellipse3 = ellipse6);
				context.PushParent(ellipse3);
				Ellipse ellipse9 = ellipse3;
				ellipse9.Name = "PART_SpectrumOverlayEllipse";
				service = ellipse9;
				context.AvaloniaNameScope.Register("PART_SpectrumOverlayEllipse", service);
				ellipse9.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				ellipse9.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				ellipse9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty4 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding16 = (templateBinding = new TemplateBinding(ColorSpectrum.ShapeProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding17 = templateBinding;
				StaticResourceExtension staticResourceExtension8 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj8 = staticResourceExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding17.Converter = (IValueConverter)obj8;
				templateBinding17.ConverterParameter = ColorSpectrumShape.Ring;
				context.PopParent();
				ellipse9.Bind(isVisibleProperty4, templateBinding16.ProvideValue());
				context.PopParent();
				((ISupportInitialize)ellipse8).EndInit();
				Controls children6 = panel6.Children;
				Canvas canvas;
				Canvas canvas2 = (canvas = new Canvas());
				((ISupportInitialize)canvas2).BeginInit();
				children6.Add(canvas2);
				canvas.Name = "PART_InputTarget";
				service = canvas;
				context.AvaloniaNameScope.Register("PART_InputTarget", service);
				canvas.SetValue(Panel.BackgroundProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				canvas.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				canvas.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				Controls children7 = canvas.Children;
				Panel panel7;
				Panel panel8 = (panel7 = new Panel());
				((ISupportInitialize)panel8).BeginInit();
				children7.Add(panel8);
				panel7.Name = "PART_SelectionEllipsePanel";
				service = panel7;
				context.AvaloniaNameScope.Register("PART_SelectionEllipsePanel", service);
				panel7.SetValue(ToolTip.VerticalOffsetProperty, -10.0, BindingPriority.Template);
				panel7.SetValue(ToolTip.PlacementProperty, PlacementMode.Top, BindingPriority.Template);
				Controls children8 = panel7.Children;
				Ellipse ellipse10;
				Ellipse ellipse11 = (ellipse10 = new Ellipse());
				((ISupportInitialize)ellipse11).BeginInit();
				children8.Add(ellipse11);
				ellipse10.Name = "FocusEllipse";
				service = ellipse10;
				context.AvaloniaNameScope.Register("FocusEllipse", service);
				ellipse10.SetValue(Layoutable.MarginProperty, new Thickness(-2.0, -2.0, -2.0, -2.0), BindingPriority.Template);
				ellipse10.SetValue(Shape.StrokeThicknessProperty, 2.0, BindingPriority.Template);
				ellipse10.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				ellipse10.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				ellipse10.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				((ISupportInitialize)ellipse10).EndInit();
				Controls children9 = panel7.Children;
				Ellipse ellipse12;
				Ellipse ellipse13 = (ellipse12 = new Ellipse());
				((ISupportInitialize)ellipse13).BeginInit();
				children9.Add(ellipse13);
				ellipse12.Name = "SelectionEllipse";
				service = ellipse12;
				context.AvaloniaNameScope.Register("SelectionEllipse", service);
				ellipse12.SetValue(Shape.StrokeThicknessProperty, 2.0, BindingPriority.Template);
				ellipse12.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				ellipse12.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				ellipse12.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				((ISupportInitialize)ellipse12).EndInit();
				((ISupportInitialize)panel7).EndInit();
				((ISupportInitialize)canvas).EndInit();
				Controls children10 = panel6.Children;
				Rectangle rectangle10;
				Rectangle rectangle11 = (rectangle10 = new Rectangle());
				((ISupportInitialize)rectangle11).BeginInit();
				children10.Add(rectangle11);
				Rectangle rectangle12 = (rectangle3 = rectangle10);
				context.PushParent(rectangle3);
				Rectangle rectangle13 = rectangle3;
				rectangle13.Name = "BorderRectangle";
				service = rectangle13;
				context.AvaloniaNameScope.Register("BorderRectangle", service);
				rectangle13.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				rectangle13.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty5 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding18 = (templateBinding = new TemplateBinding(ColorSpectrum.ShapeProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding19 = templateBinding;
				StaticResourceExtension staticResourceExtension9 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj9 = staticResourceExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding19.Converter = (IValueConverter)obj9;
				templateBinding19.ConverterParameter = ColorSpectrumShape.Box;
				context.PopParent();
				rectangle13.Bind(isVisibleProperty5, templateBinding18.ProvideValue());
				StyledProperty<double> radiusXProperty3 = Rectangle.RadiusXProperty;
				TemplateBinding templateBinding20 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding21 = templateBinding;
				StaticResourceExtension staticResourceExtension10 = new StaticResourceExtension("TopLeftCornerRadiusConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj10 = staticResourceExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding21.Converter = (IValueConverter)obj10;
				context.PopParent();
				rectangle13.Bind(radiusXProperty3, templateBinding20.ProvideValue());
				StyledProperty<double> radiusYProperty3 = Rectangle.RadiusYProperty;
				TemplateBinding templateBinding22 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding23 = templateBinding;
				StaticResourceExtension staticResourceExtension11 = new StaticResourceExtension("BottomRightCornerRadiusConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj11 = staticResourceExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding23.Converter = (IValueConverter)obj11;
				context.PopParent();
				rectangle13.Bind(radiusYProperty3, templateBinding22.ProvideValue());
				context.PopParent();
				((ISupportInitialize)rectangle12).EndInit();
				Controls children11 = panel6.Children;
				Ellipse ellipse14;
				Ellipse ellipse15 = (ellipse14 = new Ellipse());
				((ISupportInitialize)ellipse15).BeginInit();
				children11.Add(ellipse15);
				Ellipse ellipse16 = (ellipse3 = ellipse14);
				context.PushParent(ellipse3);
				Ellipse ellipse17 = ellipse3;
				ellipse17.Name = "BorderEllipse";
				service = ellipse17;
				context.AvaloniaNameScope.Register("BorderEllipse", service);
				ellipse17.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				ellipse17.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				ellipse17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty6 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding24 = (templateBinding = new TemplateBinding(ColorSpectrum.ShapeProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding25 = templateBinding;
				StaticResourceExtension staticResourceExtension12 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj12 = staticResourceExtension12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding25.Converter = (IValueConverter)obj12;
				templateBinding25.ConverterParameter = ColorSpectrumShape.Ring;
				context.PopParent();
				ellipse17.Bind(isVisibleProperty6, templateBinding24.ProvideValue());
				context.PopParent();
				((ISupportInitialize)ellipse16).EndInit();
				context.PopParent();
				((ISupportInitialize)panel5).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
			controlTheme.TargetType = typeof(ColorSpectrum);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.TargetType = typeof(ColorSpectrum);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_65.Build, context);
			context.PopParent();
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Style style;
			Style style2 = (style = new Style());
			context.PushParent(style);
			Style style3 = style;
			style3.Selector = Selectors.Or(new List<Selector>
			{
				((Selector?)null).Nesting().Template().OfType(typeof(Ellipse))
					.Name("BorderEllipse"),
				((Selector?)null).Nesting().Template().OfType(typeof(Rectangle))
					.Name("BorderRectangle")
			});
			Setter setter4 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter5 = setter;
			setter5.Property = Shape.StrokeProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeBorderLowBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value2 = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter5.Value = value2;
			context.PopParent();
			style3.Add(setter4);
			Setter setter6 = new Setter();
			setter6.Property = Shape.StrokeThicknessProperty;
			setter6.Value = 1.0;
			style3.Add(setter6);
			context.PopParent();
			controlTheme.Add(style2);
			Style style4 = new Style();
			style4.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(Ellipse))
				.Name("FocusEllipse");
			Setter setter7 = new Setter();
			setter7.Property = Visual.IsVisibleProperty;
			setter7.Value = false;
			style4.Add(setter7);
			controlTheme.Add(style4);
			Style style5 = new Style();
			style5.Selector = ((Selector?)null).Nesting().Class(":focus-visible").Template()
				.OfType(typeof(Ellipse))
				.Name("FocusEllipse");
			Setter setter8 = new Setter();
			setter8.Property = Visual.IsVisibleProperty;
			setter8.Value = true;
			style5.Add(setter8);
			controlTheme.Add(style5);
			Style style6 = (style = new Style());
			context.PushParent(style);
			Style style7 = style;
			style7.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(Ellipse))
				.Name("FocusEllipse");
			Setter setter9 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter10 = setter;
			setter10.Property = Shape.StrokeProperty;
			DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ColorControlLightSelectorBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value3 = dynamicResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter10.Value = value3;
			context.PopParent();
			style7.Add(setter9);
			context.PopParent();
			controlTheme.Add(style6);
			Style style8 = (style = new Style());
			context.PushParent(style);
			Style style9 = style;
			style9.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(Ellipse))
				.Name("SelectionEllipse");
			Setter setter11 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter12 = setter;
			setter12.Property = Shape.StrokeProperty;
			DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ColorControlDarkSelectorBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value4 = dynamicResourceExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter12.Value = value4;
			context.PopParent();
			style9.Add(setter11);
			context.PopParent();
			controlTheme.Add(style8);
			Style style10 = (style = new Style());
			context.PushParent(style);
			Style style11 = style;
			style11.Selector = ((Selector?)null).Nesting().Class(":light-selector").Template()
				.OfType(typeof(Ellipse))
				.Name("FocusEllipse");
			Setter setter13 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter14 = setter;
			setter14.Property = Shape.StrokeProperty;
			DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("ColorControlDarkSelectorBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value5 = dynamicResourceExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter14.Value = value5;
			context.PopParent();
			style11.Add(setter13);
			context.PopParent();
			controlTheme.Add(style10);
			Style style12 = (style = new Style());
			context.PushParent(style);
			Style style13 = style;
			style13.Selector = ((Selector?)null).Nesting().Class(":light-selector").Template()
				.OfType(typeof(Ellipse))
				.Name("SelectionEllipse");
			Setter setter15 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter16 = setter;
			setter16.Property = Shape.StrokeProperty;
			DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("ColorControlLightSelectorBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value6 = dynamicResourceExtension5.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter16.Value = value6;
			context.PopParent();
			style13.Add(setter15);
			context.PopParent();
			controlTheme.Add(style12);
			Style style14 = new Style();
			style14.Selector = ((Selector?)null).Nesting().Class(":pointerover").Template()
				.OfType(typeof(Ellipse))
				.Name("SelectionEllipse");
			Setter setter17 = new Setter();
			setter17.Property = Visual.OpacityProperty;
			setter17.Value = 0.7;
			style14.Add(setter17);
			controlTheme.Add(style14);
			Style style15 = new Style();
			style15.Selector = ((Selector?)null).Nesting().Class(":pointerover").Class(":light-selector")
				.Template()
				.OfType(typeof(Ellipse))
				.Name("SelectionEllipse");
			Setter setter18 = new Setter();
			setter18.Property = Visual.OpacityProperty;
			setter18.Value = 0.8;
			style15.Add(setter18);
			controlTheme.Add(style15);
			Style style16 = new Style();
			style16.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(Panel))
				.Name("PART_SelectionEllipsePanel");
			Setter setter19 = new Setter();
			setter19.Property = Layoutable.WidthProperty;
			setter19.Value = 16.0;
			style16.Add(setter19);
			Setter setter20 = new Setter();
			setter20.Property = Layoutable.HeightProperty;
			setter20.Value = 16.0;
			style16.Add(setter20);
			controlTheme.Add(style16);
			Style style17 = new Style();
			style17.Selector = ((Selector?)null).Nesting().Class(":large-selector").Template()
				.OfType(typeof(Panel))
				.Name("PART_SelectionEllipsePanel");
			Setter setter21 = new Setter();
			setter21.Property = Layoutable.WidthProperty;
			setter21.Value = 48.0;
			style17.Add(setter21);
			Setter setter22 = new Setter();
			setter22.Property = Layoutable.HeightProperty;
			setter22.Value = 48.0;
			style17.Add(setter22);
			controlTheme.Add(style17);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_66
	{
		private class XamlClosure_67
		{
			private class DynamicSetters_68
			{
				public static void DynamicSetter_1(Border P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(Border.BackgroundProperty, binding);
						return;
					}
					if (P_2 is IBrush)
					{
						IBrush value = (IBrush)P_2;
						int priority = (int)P_1;
						P_0.SetValue(Border.BackgroundProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						IBrush value = (IBrush)P_2;
						int priority = (int)P_1;
						P_0.SetValue(Border.BackgroundProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
				}

				public static void DynamicSetter_2(StyledElement P_0, BindingPriority P_1, object P_2)
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

				public static void DynamicSetter_3(ItemsControl P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(ItemsControl.ItemContainerThemeProperty, binding);
						return;
					}
					if (P_2 is ControlTheme)
					{
						ControlTheme value = (ControlTheme)P_2;
						int priority = (int)P_1;
						P_0.SetValue(ItemsControl.ItemContainerThemeProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						ControlTheme value = (ControlTheme)P_2;
						int priority = (int)P_1;
						P_0.SetValue(ItemsControl.ItemContainerThemeProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
				}

				public static void DynamicSetter_4(SelectingItemsControl P_0, CompiledBindingExtension P_1)
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

				public static void DynamicSetter_5(NumericUpDown P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(NumericUpDown.NumberFormatProperty, binding);
						return;
					}
					if (P_2 is NumberFormatInfo)
					{
						NumberFormatInfo value = (NumberFormatInfo)P_2;
						int priority = (int)P_1;
						P_0.SetValue(NumericUpDown.NumberFormatProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						NumberFormatInfo value = (NumberFormatInfo)P_2;
						int priority = (int)P_1;
						P_0.SetValue(NumericUpDown.NumberFormatProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
				}
			}

			private class XamlClosure_69
			{
				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
					if (P_0 != null)
					{
						object service = P_0.GetService(typeof(IRootObjectProvider));
						if (service != null)
						{
							service = ((IRootObjectProvider)service).RootObject;
							context.RootObject = (Styles)service;
						}
					}
					context.IntermediateRoot = new UniformGrid();
					object intermediateRoot = context.IntermediateRoot;
					((ISupportInitialize)intermediateRoot).BeginInit();
					((AvaloniaObject)intermediateRoot).SetValue(UniformGrid.ColumnsProperty, 0, BindingPriority.Template);
					((AvaloniaObject)intermediateRoot).SetValue(UniformGrid.RowsProperty, 1, BindingPriority.Template);
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			private class XamlClosure_70
			{
				private class DynamicSetters_71
				{
					public static void DynamicSetter_1(ToolTip P_0, BindingPriority P_1, CompiledBindingExtension P_2)
					{
						if (P_2 != null)
						{
							IBinding binding = P_2;
							P_0.Bind(ToolTip.TipProperty, binding);
						}
						else
						{
							object value = P_2;
							int priority = (int)P_1;
							P_0.SetValue(ToolTip.TipProperty, value, (BindingPriority)priority);
						}
					}
				}

				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
					AttachedProperty<string?> nameProperty = AutomationProperties.NameProperty;
					CompiledBindingExtension compiledBindingExtension;
					CompiledBindingExtension compiledBindingExtension2 = (compiledBindingExtension = new CompiledBindingExtension());
					context.PushParent(compiledBindingExtension);
					CompiledBindingExtension compiledBindingExtension3 = compiledBindingExtension;
					StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ColorToDisplayNameConverter");
					context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
					object? obj = staticResourceExtension.ProvideValue(context);
					context.ProvideTargetProperty = null;
					compiledBindingExtension3.Converter = (IValueConverter)obj;
					context.PopParent();
					context.ProvideTargetProperty = AutomationProperties.NameProperty;
					CompiledBindingExtension binding = compiledBindingExtension2.ProvideValue(context);
					context.ProvideTargetProperty = null;
					border.Bind(nameProperty, binding);
					CompiledBindingExtension compiledBindingExtension4 = (compiledBindingExtension = new CompiledBindingExtension());
					context.PushParent(compiledBindingExtension);
					CompiledBindingExtension compiledBindingExtension5 = compiledBindingExtension;
					StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("ColorToDisplayNameConverter");
					context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
					object? obj2 = staticResourceExtension2.ProvideValue(context);
					context.ProvideTargetProperty = null;
					compiledBindingExtension5.Converter = (IValueConverter)obj2;
					context.PopParent();
					context.ProvideTargetProperty = ToolTip.TipProperty;
					CompiledBindingExtension compiledBindingExtension6 = compiledBindingExtension4.ProvideValue(context);
					context.ProvideTargetProperty = null;
					DynamicSetters_71.DynamicSetter_1((ToolTip)(object)border, BindingPriority.Template, compiledBindingExtension6);
					border.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
					border.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
					StyledProperty<IBrush?> backgroundProperty = Border.BackgroundProperty;
					SolidColorBrush solidColorBrush;
					SolidColorBrush value = (solidColorBrush = new SolidColorBrush());
					context.PushParent(solidColorBrush);
					StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
					CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension();
					context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
					CompiledBindingExtension binding2 = compiledBindingExtension7.ProvideValue(context);
					context.ProvideTargetProperty = null;
					solidColorBrush.Bind(colorProperty, binding2);
					context.PopParent();
					border.SetValue(backgroundProperty, value, BindingPriority.Template);
					context.PopParent();
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			private class XamlClosure_72
			{
				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
					if (P_0 != null)
					{
						object service = P_0.GetService(typeof(IRootObjectProvider));
						if (service != null)
						{
							service = ((IRootObjectProvider)service).RootObject;
							context.RootObject = (Styles)service;
						}
					}
					context.IntermediateRoot = new UniformGrid();
					object intermediateRoot = context.IntermediateRoot;
					((ISupportInitialize)intermediateRoot).BeginInit();
					UniformGrid uniformGrid = (UniformGrid)intermediateRoot;
					context.PushParent(uniformGrid);
					StyledProperty<int> columnsProperty = UniformGrid.ColumnsProperty;
					CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(ColorView), 0).Property(ColorView.PaletteColumnCountProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
					context.ProvideTargetProperty = UniformGrid.ColumnsProperty;
					CompiledBindingExtension binding = compiledBindingExtension.ProvideValue(context);
					context.ProvideTargetProperty = null;
					uniformGrid.Bind(columnsProperty, binding);
					context.PopParent();
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
				context.IntermediateRoot = new DropDownButton();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				DropDownButton dropDownButton = (DropDownButton)intermediateRoot;
				context.PushParent(dropDownButton);
				dropDownButton.Bind(TemplatedControl.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				dropDownButton.Bind(Layoutable.HeightProperty, new TemplateBinding(Layoutable.HeightProperty).ProvideValue());
				dropDownButton.Bind(Layoutable.WidthProperty, new TemplateBinding(Layoutable.WidthProperty).ProvideValue());
				dropDownButton.SetValue(ContentControl.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				dropDownButton.SetValue(ContentControl.VerticalContentAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				dropDownButton.SetValue(TemplatedControl.PaddingProperty, new Thickness(0.0, 0.0, 10.0, 0.0), BindingPriority.Template);
				dropDownButton.SetValue(Layoutable.UseLayoutRoundingProperty, value: false, BindingPriority.Template);
				Styles styles = dropDownButton.Styles;
				Style style = new Style();
				style.Selector = ((Selector?)null).OfType(typeof(FlyoutPresenter)).Class("nopadding");
				Setter setter = new Setter();
				setter.Property = TemplatedControl.PaddingProperty;
				setter.Value = new Thickness(0.0, 0.0, 0.0, 0.0);
				style.Add(setter);
				styles.Add(style);
				StyledProperty<object?> contentProperty = ContentControl.ContentProperty;
				Panel panel;
				Panel panel2 = (panel = new Panel());
				((ISupportInitialize)panel2).BeginInit();
				dropDownButton.SetValue(contentProperty, panel2, BindingPriority.Template);
				Panel panel3;
				Panel panel4 = (panel3 = panel);
				context.PushParent(panel3);
				Panel panel5 = panel3;
				Controls children = panel5.Children;
				Border border;
				Border border2 = (border = new Border());
				((ISupportInitialize)border2).BeginInit();
				children.Add(border2);
				Border border3;
				Border border4 = (border3 = border);
				context.PushParent(border3);
				Border border5 = border3;
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ColorControlCheckeredBackgroundBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				object? obj = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_68.DynamicSetter_1(border5, BindingPriority.Template, obj);
				StyledProperty<CornerRadius> cornerRadiusProperty = Border.CornerRadiusProperty;
				TemplateBinding templateBinding;
				TemplateBinding templateBinding2 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding3 = templateBinding;
				StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("LeftCornerRadiusFilterConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj2 = staticResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding3.Converter = (IValueConverter)obj2;
				context.PopParent();
				border5.Bind(cornerRadiusProperty, templateBinding2.ProvideValue());
				border5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				border5.SetValue(Layoutable.MarginProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)border4).EndInit();
				Controls children2 = panel5.Children;
				Border border6;
				Border border7 = (border6 = new Border());
				((ISupportInitialize)border7).BeginInit();
				children2.Add(border7);
				Border border8 = (border3 = border6);
				context.PushParent(border3);
				Border border9 = border3;
				StyledProperty<IBrush?> backgroundProperty = Border.BackgroundProperty;
				TemplateBinding templateBinding4 = (templateBinding = new TemplateBinding(ColorView.HsvColorProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding5 = templateBinding;
				StaticResourceExtension staticResourceExtension3 = new StaticResourceExtension("ToBrushConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj3 = staticResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding5.Converter = (IValueConverter)obj3;
				context.PopParent();
				border9.Bind(backgroundProperty, templateBinding4.ProvideValue());
				StyledProperty<CornerRadius> cornerRadiusProperty2 = Border.CornerRadiusProperty;
				TemplateBinding templateBinding6 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding7 = templateBinding;
				StaticResourceExtension staticResourceExtension4 = new StaticResourceExtension("LeftCornerRadiusFilterConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj4 = staticResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding7.Converter = (IValueConverter)obj4;
				context.PopParent();
				border9.Bind(cornerRadiusProperty2, templateBinding6.ProvideValue());
				border9.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				border9.SetValue(Layoutable.MarginProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)border8).EndInit();
				context.PopParent();
				((ISupportInitialize)panel4).EndInit();
				StyledProperty<FlyoutBase?> flyoutProperty = Button.FlyoutProperty;
				Flyout flyout;
				Flyout value = (flyout = new Flyout());
				context.PushParent(flyout);
				flyout.SetValue(PopupFlyoutBase.PlacementProperty, PlacementMode.Top, BindingPriority.Template);
				Grid grid;
				Grid grid2 = (grid = new Grid());
				((ISupportInitialize)grid2).BeginInit();
				flyout.Content = grid2;
				Grid grid3;
				Grid grid4 = (grid3 = grid);
				context.PushParent(grid3);
				Grid grid5 = grid3;
				RowDefinitions rowDefinitions = new RowDefinitions();
				rowDefinitions.Capacity = 2;
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				grid5.RowDefinitions = rowDefinitions;
				grid5.Resources.Add("ColorViewTabBackgroundCornerRadius", new CornerRadius(0.0, 0.0, 0.0, 0.0));
				Controls children3 = grid5.Children;
				Border border10;
				Border border11 = (border10 = new Border());
				((ISupportInitialize)border11).BeginInit();
				children3.Add(border11);
				Border border12 = (border3 = border10);
				context.PushParent(border3);
				Border border13 = border3;
				border13.Name = "TabBackgroundBorder";
				service = border13;
				context.AvaloniaNameScope.Register("TabBackgroundBorder", service);
				border13.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				border13.SetValue(Grid.RowSpanProperty, 2, BindingPriority.Template);
				border13.SetValue(Layoutable.HeightProperty, 48.0, BindingPriority.Template);
				border13.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top, BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty2 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemControlBackgroundBaseLowBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border13.Bind(backgroundProperty2, binding);
				StyledProperty<IBrush?> borderBrushProperty = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ColorViewTabBorderBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding2 = dynamicResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border13.Bind(borderBrushProperty, binding2);
				StyledProperty<CornerRadius> cornerRadiusProperty3 = Border.CornerRadiusProperty;
				DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ColorViewTabBackgroundCornerRadius");
				context.ProvideTargetProperty = Border.CornerRadiusProperty;
				IBinding binding3 = dynamicResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border13.Bind(cornerRadiusProperty3, binding3);
				context.PopParent();
				((ISupportInitialize)border12).EndInit();
				Controls children4 = grid5.Children;
				Border border14;
				Border border15 = (border14 = new Border());
				((ISupportInitialize)border15).BeginInit();
				children4.Add(border15);
				Border border16 = (border3 = border14);
				context.PushParent(border3);
				Border border17 = border3;
				border17.Name = "ContentBackgroundBorder";
				service = border17;
				context.AvaloniaNameScope.Register("ContentBackgroundBorder", service);
				border17.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				border17.SetValue(Grid.RowSpanProperty, 2, BindingPriority.Template);
				border17.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 48.0, 0.0, 0.0), BindingPriority.Template);
				border17.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<CornerRadius> cornerRadiusProperty4 = Border.CornerRadiusProperty;
				TemplateBinding templateBinding8 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding9 = templateBinding;
				StaticResourceExtension staticResourceExtension5 = new StaticResourceExtension("BottomCornerRadiusFilterConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj5 = staticResourceExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding9.Converter = (IValueConverter)obj5;
				context.PopParent();
				border17.Bind(cornerRadiusProperty4, templateBinding8.ProvideValue());
				StyledProperty<IBrush?> backgroundProperty3 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("ColorViewContentBackgroundBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding4 = dynamicResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border17.Bind(backgroundProperty3, binding4);
				StyledProperty<IBrush?> borderBrushProperty2 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("ColorViewContentBorderBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding5 = dynamicResourceExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border17.Bind(borderBrushProperty2, binding5);
				border17.SetValue(Border.BorderThicknessProperty, new Thickness(0.0, 1.0, 0.0, 0.0), BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)border16).EndInit();
				Controls children5 = grid5.Children;
				TabControl tabControl;
				TabControl tabControl2 = (tabControl = new TabControl());
				((ISupportInitialize)tabControl2).BeginInit();
				children5.Add(tabControl2);
				TabControl tabControl3;
				TabControl tabControl4 = (tabControl3 = tabControl);
				context.PushParent(tabControl3);
				tabControl3.Name = "PART_TabControl";
				service = tabControl3;
				context.AvaloniaNameScope.Register("PART_TabControl", service);
				tabControl3.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				tabControl3.SetValue(Layoutable.HeightProperty, 338.0, BindingPriority.Template);
				tabControl3.SetValue(Layoutable.WidthProperty, 350.0, BindingPriority.Template);
				tabControl3.SetValue(TemplatedControl.PaddingProperty, new Thickness(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				DirectProperty<SelectingItemsControl, int> selectedIndexProperty = SelectingItemsControl.SelectedIndexProperty;
				CompiledBindingExtension obj6 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.SelectedIndexProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = SelectingItemsControl.SelectedIndexProperty;
				CompiledBindingExtension binding6 = obj6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				tabControl3.Bind(selectedIndexProperty, binding6);
				tabControl3.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate
				{
					Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_69.Build, context)
				}, BindingPriority.Template);
				ItemCollection items = tabControl3.Items;
				TabItem tabItem;
				TabItem tabItem2 = (tabItem = new TabItem());
				((ISupportInitialize)tabItem2).BeginInit();
				items.Add(tabItem2);
				TabItem tabItem3;
				TabItem tabItem4 = (tabItem3 = tabItem);
				context.PushParent(tabItem3);
				TabItem tabItem5 = tabItem3;
				StaticResourceExtension staticResourceExtension6 = new StaticResourceExtension("ColorViewTabItemTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj7 = staticResourceExtension6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_68.DynamicSetter_2(tabItem5, BindingPriority.Template, obj7);
				tabItem5.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorSpectrumVisibleProperty).ProvideValue());
				StyledProperty<object?> headerProperty = HeaderedContentControl.HeaderProperty;
				Border border18;
				Border border19 = (border18 = new Border());
				((ISupportInitialize)border19).BeginInit();
				tabItem5.SetValue(headerProperty, border19, BindingPriority.Template);
				Border border20 = (border3 = border18);
				context.PushParent(border3);
				Border border21 = border3;
				StyledProperty<double> heightProperty = Layoutable.HeightProperty;
				DynamicResourceExtension dynamicResourceExtension6 = new DynamicResourceExtension("ColorViewTabStripHeight");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				IBinding binding7 = dynamicResourceExtension6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border21.Bind(heightProperty, binding7);
				PathIcon pathIcon;
				PathIcon pathIcon2 = (pathIcon = new PathIcon());
				((ISupportInitialize)pathIcon2).BeginInit();
				border21.Child = pathIcon2;
				PathIcon pathIcon3;
				PathIcon pathIcon4 = (pathIcon3 = pathIcon);
				context.PushParent(pathIcon3);
				PathIcon pathIcon5 = pathIcon3;
				pathIcon5.SetValue(Layoutable.WidthProperty, 20.0, BindingPriority.Template);
				pathIcon5.SetValue(Layoutable.HeightProperty, 20.0, BindingPriority.Template);
				StyledProperty<Geometry> dataProperty = PathIcon.DataProperty;
				DynamicResourceExtension dynamicResourceExtension7 = new DynamicResourceExtension("ColorViewSpectrumIconGeometry");
				context.ProvideTargetProperty = PathIcon.DataProperty;
				IBinding binding8 = dynamicResourceExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				pathIcon5.Bind(dataProperty, binding8);
				context.PopParent();
				((ISupportInitialize)pathIcon4).EndInit();
				context.PopParent();
				((ISupportInitialize)border20).EndInit();
				Grid grid6;
				Grid grid7 = (grid6 = new Grid());
				((ISupportInitialize)grid7).BeginInit();
				tabItem5.Content = grid7;
				Grid grid8 = (grid3 = grid6);
				context.PushParent(grid3);
				Grid grid9 = grid3;
				RowDefinitions rowDefinitions2 = new RowDefinitions();
				rowDefinitions2.Capacity = 1;
				rowDefinitions2.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid9.RowDefinitions = rowDefinitions2;
				grid9.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 12.0, 12.0, 12.0), BindingPriority.Template);
				ColumnDefinitions columnDefinitions = grid9.ColumnDefinitions;
				ColumnDefinition columnDefinition = new ColumnDefinition();
				columnDefinition.SetValue(ColumnDefinition.WidthProperty, new GridLength(0.0, GridUnitType.Auto), BindingPriority.Template);
				columnDefinition.SetValue(ColumnDefinition.MinWidthProperty, 32.0, BindingPriority.Template);
				columnDefinitions.Add(columnDefinition);
				ColumnDefinitions columnDefinitions2 = grid9.ColumnDefinitions;
				ColumnDefinition columnDefinition2 = new ColumnDefinition();
				columnDefinition2.SetValue(ColumnDefinition.WidthProperty, new GridLength(1.0, GridUnitType.Star), BindingPriority.Template);
				columnDefinitions2.Add(columnDefinition2);
				ColumnDefinitions columnDefinitions3 = grid9.ColumnDefinitions;
				ColumnDefinition columnDefinition3 = new ColumnDefinition();
				columnDefinition3.SetValue(ColumnDefinition.WidthProperty, new GridLength(0.0, GridUnitType.Auto), BindingPriority.Template);
				columnDefinition3.SetValue(ColumnDefinition.MinWidthProperty, 32.0, BindingPriority.Template);
				columnDefinitions3.Add(columnDefinition3);
				Controls children6 = grid9.Children;
				ColorSlider colorSlider;
				ColorSlider colorSlider2 = (colorSlider = new ColorSlider());
				((ISupportInitialize)colorSlider2).BeginInit();
				children6.Add(colorSlider2);
				ColorSlider colorSlider3;
				ColorSlider colorSlider4 = (colorSlider3 = colorSlider);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider5 = colorSlider3;
				colorSlider5.Name = "ColorSpectrumThirdComponentSlider";
				service = colorSlider5;
				context.AvaloniaNameScope.Register("ColorSpectrumThirdComponentSlider", service);
				colorSlider5.SetValue(AutomationProperties.NameProperty, "Third Component", BindingPriority.Template);
				colorSlider5.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				colorSlider5.SetValue(ColorSlider.IsAlphaVisibleProperty, value: false, BindingPriority.Template);
				colorSlider5.SetValue(ColorSlider.IsPerceptiveProperty, value: true, BindingPriority.Template);
				colorSlider5.SetValue(Slider.OrientationProperty, Orientation.Vertical, BindingPriority.Template);
				colorSlider5.SetValue(ColorSlider.ColorModelProperty, ColorModel.Hsva, BindingPriority.Template);
				StyledProperty<ColorComponent> colorComponentProperty = ColorSlider.ColorComponentProperty;
				CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "ColorSpectrum").Property(ColorSpectrum.ThirdComponentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = ColorSlider.ColorComponentProperty;
				CompiledBindingExtension binding9 = compiledBindingExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider5.Bind(colorComponentProperty, binding9);
				StyledProperty<HsvColor> hsvColorProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension compiledBindingExtension2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "ColorSpectrum").Property(ColorSpectrum.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding10 = compiledBindingExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider5.Bind(hsvColorProperty, binding10);
				colorSlider5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				colorSlider5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				colorSlider5.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				colorSlider5.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorSpectrumSliderVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorSlider4).EndInit();
				Controls children7 = grid9.Children;
				ColorSpectrum colorSpectrum;
				ColorSpectrum colorSpectrum2 = (colorSpectrum = new ColorSpectrum());
				((ISupportInitialize)colorSpectrum2).BeginInit();
				children7.Add(colorSpectrum2);
				ColorSpectrum colorSpectrum3;
				ColorSpectrum colorSpectrum4 = (colorSpectrum3 = colorSpectrum);
				context.PushParent(colorSpectrum3);
				colorSpectrum3.Name = "ColorSpectrum";
				service = colorSpectrum3;
				context.AvaloniaNameScope.Register("ColorSpectrum", service);
				colorSpectrum3.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				colorSpectrum3.Bind(ColorSpectrum.ComponentsProperty, new TemplateBinding(ColorView.ColorSpectrumComponentsProperty).ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty2 = ColorSpectrum.HsvColorProperty;
				CompiledBindingExtension obj8 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSpectrum.HsvColorProperty;
				CompiledBindingExtension binding11 = obj8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSpectrum3.Bind(hsvColorProperty2, binding11);
				colorSpectrum3.Bind(ColorSpectrum.MinHueProperty, new TemplateBinding(ColorView.MinHueProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MaxHueProperty, new TemplateBinding(ColorView.MaxHueProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MinSaturationProperty, new TemplateBinding(ColorView.MinSaturationProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MaxSaturationProperty, new TemplateBinding(ColorView.MaxSaturationProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MinValueProperty, new TemplateBinding(ColorView.MinValueProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MaxValueProperty, new TemplateBinding(ColorView.MaxValueProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.ShapeProperty, new TemplateBinding(ColorView.ColorSpectrumShapeProperty).ProvideValue());
				colorSpectrum3.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSpectrum3.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)colorSpectrum4).EndInit();
				Controls children8 = grid9.Children;
				ColorSlider colorSlider6;
				ColorSlider colorSlider7 = (colorSlider6 = new ColorSlider());
				((ISupportInitialize)colorSlider7).BeginInit();
				children8.Add(colorSlider7);
				ColorSlider colorSlider8 = (colorSlider3 = colorSlider6);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider9 = colorSlider3;
				colorSlider9.Name = "ColorSpectrumAlphaSlider";
				service = colorSlider9;
				context.AvaloniaNameScope.Register("ColorSpectrumAlphaSlider", service);
				colorSlider9.SetValue(AutomationProperties.NameProperty, "Alpha Component", BindingPriority.Template);
				colorSlider9.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider9.SetValue(Slider.OrientationProperty, Orientation.Vertical, BindingPriority.Template);
				colorSlider9.SetValue(ColorSlider.ColorModelProperty, ColorModel.Hsva, BindingPriority.Template);
				colorSlider9.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Alpha, BindingPriority.Template);
				StyledProperty<HsvColor> hsvColorProperty3 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "ColorSpectrum").Property(ColorSpectrum.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding12 = compiledBindingExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider9.Bind(hsvColorProperty3, binding12);
				colorSlider9.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				colorSlider9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				colorSlider9.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				colorSlider9.Bind(InputElement.IsEnabledProperty, new TemplateBinding(ColorView.IsAlphaEnabledProperty).ProvideValue());
				StyledProperty<bool> isVisibleProperty = Visual.IsVisibleProperty;
				MultiBinding multiBinding;
				MultiBinding binding13 = (multiBinding = new MultiBinding());
				context.PushParent(multiBinding);
				MultiBinding multiBinding2 = multiBinding;
				multiBinding2.Converter = BoolConverters.And;
				IList<IBinding> bindings = multiBinding2.Bindings;
				CompiledBindingExtension obj9 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsAlphaVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item = obj9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings.Add(item);
				context.PopParent();
				colorSlider9.Bind(isVisibleProperty, binding13);
				context.PopParent();
				((ISupportInitialize)colorSlider8).EndInit();
				context.PopParent();
				((ISupportInitialize)grid8).EndInit();
				context.PopParent();
				((ISupportInitialize)tabItem4).EndInit();
				ItemCollection items2 = tabControl3.Items;
				TabItem tabItem6;
				TabItem tabItem7 = (tabItem6 = new TabItem());
				((ISupportInitialize)tabItem7).BeginInit();
				items2.Add(tabItem7);
				TabItem tabItem8 = (tabItem3 = tabItem6);
				context.PushParent(tabItem3);
				TabItem tabItem9 = tabItem3;
				StaticResourceExtension staticResourceExtension7 = new StaticResourceExtension("ColorViewTabItemTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj10 = staticResourceExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_68.DynamicSetter_2(tabItem9, BindingPriority.Template, obj10);
				tabItem9.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorPaletteVisibleProperty).ProvideValue());
				StyledProperty<object?> headerProperty2 = HeaderedContentControl.HeaderProperty;
				Border border22;
				Border border23 = (border22 = new Border());
				((ISupportInitialize)border23).BeginInit();
				tabItem9.SetValue(headerProperty2, border23, BindingPriority.Template);
				Border border24 = (border3 = border22);
				context.PushParent(border3);
				Border border25 = border3;
				StyledProperty<double> heightProperty2 = Layoutable.HeightProperty;
				DynamicResourceExtension dynamicResourceExtension8 = new DynamicResourceExtension("ColorViewTabStripHeight");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				IBinding binding14 = dynamicResourceExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border25.Bind(heightProperty2, binding14);
				PathIcon pathIcon6;
				PathIcon pathIcon7 = (pathIcon6 = new PathIcon());
				((ISupportInitialize)pathIcon7).BeginInit();
				border25.Child = pathIcon7;
				PathIcon pathIcon8 = (pathIcon3 = pathIcon6);
				context.PushParent(pathIcon3);
				PathIcon pathIcon9 = pathIcon3;
				pathIcon9.SetValue(Layoutable.WidthProperty, 20.0, BindingPriority.Template);
				pathIcon9.SetValue(Layoutable.HeightProperty, 20.0, BindingPriority.Template);
				StyledProperty<Geometry> dataProperty2 = PathIcon.DataProperty;
				DynamicResourceExtension dynamicResourceExtension9 = new DynamicResourceExtension("ColorViewPaletteIconGeometry");
				context.ProvideTargetProperty = PathIcon.DataProperty;
				IBinding binding15 = dynamicResourceExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				pathIcon9.Bind(dataProperty2, binding15);
				context.PopParent();
				((ISupportInitialize)pathIcon8).EndInit();
				context.PopParent();
				((ISupportInitialize)border24).EndInit();
				ListBox listBox;
				ListBox listBox2 = (listBox = new ListBox());
				((ISupportInitialize)listBox2).BeginInit();
				tabItem9.Content = listBox2;
				ListBox listBox3;
				ListBox listBox4 = (listBox3 = listBox);
				context.PushParent(listBox3);
				StaticResourceExtension staticResourceExtension8 = new StaticResourceExtension("ColorViewPaletteListBoxTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj11 = staticResourceExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_68.DynamicSetter_2(listBox3, BindingPriority.Template, obj11);
				StaticResourceExtension staticResourceExtension9 = new StaticResourceExtension("ColorViewPaletteListBoxItemTheme");
				context.ProvideTargetProperty = ItemsControl.ItemContainerThemeProperty;
				object? obj12 = staticResourceExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_68.DynamicSetter_3(listBox3, BindingPriority.Template, obj12);
				listBox3.Bind(ItemsControl.ItemsSourceProperty, new TemplateBinding(ColorView.PaletteColorsProperty).ProvideValue());
				CompiledBindingExtension compiledBindingExtension4;
				CompiledBindingExtension compiledBindingExtension5 = (compiledBindingExtension4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.ColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build()));
				context.PushParent(compiledBindingExtension4);
				StaticResourceExtension staticResourceExtension10 = new StaticResourceExtension("DoNothingForNullConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
				object? obj13 = staticResourceExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				compiledBindingExtension4.Converter = (IValueConverter)obj13;
				compiledBindingExtension4.Mode = BindingMode.TwoWay;
				context.PopParent();
				context.ProvideTargetProperty = SelectingItemsControl.SelectedItemProperty;
				CompiledBindingExtension compiledBindingExtension6 = compiledBindingExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_68.DynamicSetter_4(listBox3, compiledBindingExtension6);
				listBox3.SetValue(Layoutable.UseLayoutRoundingProperty, value: false, BindingPriority.Template);
				listBox3.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 12.0, 12.0, 12.0), BindingPriority.Template);
				StyledProperty<IDataTemplate?> itemTemplateProperty = ItemsControl.ItemTemplateProperty;
				DataTemplate dataTemplate;
				DataTemplate value2 = (dataTemplate = new DataTemplate());
				context.PushParent(dataTemplate);
				dataTemplate.DataType = typeof(Color);
				dataTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_70.Build, context);
				context.PopParent();
				listBox3.SetValue(itemTemplateProperty, value2, BindingPriority.Template);
				StyledProperty<ITemplate<Panel?>> itemsPanelProperty = ItemsControl.ItemsPanelProperty;
				ItemsPanelTemplate itemsPanelTemplate;
				ItemsPanelTemplate value3 = (itemsPanelTemplate = new ItemsPanelTemplate());
				context.PushParent(itemsPanelTemplate);
				itemsPanelTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_72.Build, context);
				context.PopParent();
				listBox3.SetValue(itemsPanelProperty, value3, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)listBox4).EndInit();
				context.PopParent();
				((ISupportInitialize)tabItem8).EndInit();
				ItemCollection items3 = tabControl3.Items;
				TabItem tabItem10;
				TabItem tabItem11 = (tabItem10 = new TabItem());
				((ISupportInitialize)tabItem11).BeginInit();
				items3.Add(tabItem11);
				TabItem tabItem12 = (tabItem3 = tabItem10);
				context.PushParent(tabItem3);
				TabItem tabItem13 = tabItem3;
				StaticResourceExtension staticResourceExtension11 = new StaticResourceExtension("ColorViewTabItemTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj14 = staticResourceExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_68.DynamicSetter_2(tabItem13, BindingPriority.Template, obj14);
				tabItem13.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorComponentsVisibleProperty).ProvideValue());
				StyledProperty<object?> headerProperty3 = HeaderedContentControl.HeaderProperty;
				Border border26;
				Border border27 = (border26 = new Border());
				((ISupportInitialize)border27).BeginInit();
				tabItem13.SetValue(headerProperty3, border27, BindingPriority.Template);
				Border border28 = (border3 = border26);
				context.PushParent(border3);
				Border border29 = border3;
				StyledProperty<double> heightProperty3 = Layoutable.HeightProperty;
				DynamicResourceExtension dynamicResourceExtension10 = new DynamicResourceExtension("ColorViewTabStripHeight");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				IBinding binding16 = dynamicResourceExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border29.Bind(heightProperty3, binding16);
				PathIcon pathIcon10;
				PathIcon pathIcon11 = (pathIcon10 = new PathIcon());
				((ISupportInitialize)pathIcon11).BeginInit();
				border29.Child = pathIcon11;
				PathIcon pathIcon12 = (pathIcon3 = pathIcon10);
				context.PushParent(pathIcon3);
				PathIcon pathIcon13 = pathIcon3;
				pathIcon13.SetValue(Layoutable.WidthProperty, 20.0, BindingPriority.Template);
				pathIcon13.SetValue(Layoutable.HeightProperty, 20.0, BindingPriority.Template);
				StyledProperty<Geometry> dataProperty3 = PathIcon.DataProperty;
				DynamicResourceExtension dynamicResourceExtension11 = new DynamicResourceExtension("ColorViewComponentsIconGeometry");
				context.ProvideTargetProperty = PathIcon.DataProperty;
				IBinding binding17 = dynamicResourceExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				pathIcon13.Bind(dataProperty3, binding17);
				context.PopParent();
				((ISupportInitialize)pathIcon12).EndInit();
				context.PopParent();
				((ISupportInitialize)border28).EndInit();
				Grid grid10;
				Grid grid11 = (grid10 = new Grid());
				((ISupportInitialize)grid11).BeginInit();
				tabItem13.Content = grid11;
				Grid grid12 = (grid3 = grid10);
				context.PushParent(grid3);
				Grid grid13 = grid3;
				ColumnDefinitions columnDefinitions4 = new ColumnDefinitions();
				columnDefinitions4.Capacity = 3;
				columnDefinitions4.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions4.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions4.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid13.ColumnDefinitions = columnDefinitions4;
				RowDefinitions rowDefinitions3 = new RowDefinitions();
				rowDefinitions3.Capacity = 7;
				rowDefinitions3.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(24.0, GridUnitType.Pixel)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(12.0, GridUnitType.Pixel)));
				grid13.RowDefinitions = rowDefinitions3;
				grid13.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 12.0, 12.0, 12.0), BindingPriority.Template);
				Controls children9 = grid13.Children;
				Grid grid14;
				Grid grid15 = (grid14 = new Grid());
				((ISupportInitialize)grid15).BeginInit();
				children9.Add(grid15);
				Grid grid16 = (grid3 = grid14);
				context.PushParent(grid3);
				Grid grid17 = grid3;
				grid17.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				grid17.SetValue(Grid.ColumnSpanProperty, 3, BindingPriority.Template);
				grid17.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				ColumnDefinitions columnDefinitions5 = new ColumnDefinitions();
				columnDefinitions5.Capacity = 3;
				columnDefinitions5.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions5.Add(new ColumnDefinition(new GridLength(12.0, GridUnitType.Pixel)));
				columnDefinitions5.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid17.ColumnDefinitions = columnDefinitions5;
				Controls children10 = grid17.Children;
				Grid grid18;
				Grid grid19 = (grid18 = new Grid());
				((ISupportInitialize)grid19).BeginInit();
				children10.Add(grid19);
				Grid grid20 = (grid3 = grid18);
				context.PushParent(grid3);
				Grid grid21 = grid3;
				ColumnDefinitions columnDefinitions6 = new ColumnDefinitions();
				columnDefinitions6.Capacity = 2;
				columnDefinitions6.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions6.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid21.ColumnDefinitions = columnDefinitions6;
				grid21.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorModelVisibleProperty).ProvideValue());
				Controls children11 = grid21.Children;
				RadioButton radioButton;
				RadioButton radioButton2 = (radioButton = new RadioButton());
				((ISupportInitialize)radioButton2).BeginInit();
				children11.Add(radioButton2);
				RadioButton radioButton3;
				RadioButton radioButton4 = (radioButton3 = radioButton);
				context.PushParent(radioButton3);
				RadioButton radioButton5 = radioButton3;
				radioButton5.Name = "RgbRadioButton";
				service = radioButton5;
				context.AvaloniaNameScope.Register("RgbRadioButton", service);
				StaticResourceExtension staticResourceExtension12 = new StaticResourceExtension("ColorViewColorModelRadioButtonTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj15 = staticResourceExtension12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_68.DynamicSetter_2(radioButton5, BindingPriority.Template, obj15);
				radioButton5.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				radioButton5.SetValue(ContentControl.ContentProperty, "RGB", BindingPriority.Template);
				radioButton5.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				radioButton5.SetValue(TemplatedControl.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				StyledProperty<double> heightProperty4 = Layoutable.HeightProperty;
				CompiledBindingExtension obj16 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "PART_HexTextBox").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding18 = obj16.ProvideValue(context);
				context.ProvideTargetProperty = null;
				radioButton5.Bind(heightProperty4, binding18);
				StyledProperty<bool?> isCheckedProperty = ToggleButton.IsCheckedProperty;
				TemplateBinding templateBinding10 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding11 = templateBinding;
				StaticResourceExtension staticResourceExtension13 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj17 = staticResourceExtension13.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding11.Converter = (IValueConverter)obj17;
				templateBinding11.ConverterParameter = ColorModel.Rgba;
				templateBinding11.Mode = BindingMode.TwoWay;
				context.PopParent();
				radioButton5.Bind(isCheckedProperty, templateBinding10.ProvideValue());
				context.PopParent();
				((ISupportInitialize)radioButton4).EndInit();
				Controls children12 = grid21.Children;
				RadioButton radioButton6;
				RadioButton radioButton7 = (radioButton6 = new RadioButton());
				((ISupportInitialize)radioButton7).BeginInit();
				children12.Add(radioButton7);
				RadioButton radioButton8 = (radioButton3 = radioButton6);
				context.PushParent(radioButton3);
				RadioButton radioButton9 = radioButton3;
				radioButton9.Name = "HsvRadioButton";
				service = radioButton9;
				context.AvaloniaNameScope.Register("HsvRadioButton", service);
				StaticResourceExtension staticResourceExtension14 = new StaticResourceExtension("ColorViewColorModelRadioButtonTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj18 = staticResourceExtension14.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_68.DynamicSetter_2(radioButton9, BindingPriority.Template, obj18);
				radioButton9.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				radioButton9.SetValue(ContentControl.ContentProperty, "HSV", BindingPriority.Template);
				radioButton9.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				radioButton9.SetValue(TemplatedControl.BorderThicknessProperty, new Thickness(0.0, 1.0, 1.0, 1.0), BindingPriority.Template);
				StyledProperty<double> heightProperty5 = Layoutable.HeightProperty;
				CompiledBindingExtension obj19 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "PART_HexTextBox").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding19 = obj19.ProvideValue(context);
				context.ProvideTargetProperty = null;
				radioButton9.Bind(heightProperty5, binding19);
				StyledProperty<bool?> isCheckedProperty2 = ToggleButton.IsCheckedProperty;
				TemplateBinding templateBinding12 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding13 = templateBinding;
				StaticResourceExtension staticResourceExtension15 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj20 = staticResourceExtension15.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding13.Converter = (IValueConverter)obj20;
				templateBinding13.ConverterParameter = ColorModel.Hsva;
				templateBinding13.Mode = BindingMode.TwoWay;
				context.PopParent();
				radioButton9.Bind(isCheckedProperty2, templateBinding12.ProvideValue());
				context.PopParent();
				((ISupportInitialize)radioButton8).EndInit();
				context.PopParent();
				((ISupportInitialize)grid20).EndInit();
				Controls children13 = grid17.Children;
				Grid grid22;
				Grid grid23 = (grid22 = new Grid());
				((ISupportInitialize)grid23).BeginInit();
				children13.Add(grid23);
				Grid grid24 = (grid3 = grid22);
				context.PushParent(grid3);
				Grid grid25 = grid3;
				grid25.Name = "HexInputGrid";
				service = grid25;
				context.AvaloniaNameScope.Register("HexInputGrid", service);
				grid25.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				grid25.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsHexInputVisibleProperty).ProvideValue());
				ColumnDefinitions columnDefinitions7 = grid25.ColumnDefinitions;
				ColumnDefinition columnDefinition4 = new ColumnDefinition();
				columnDefinition4.SetValue(ColumnDefinition.WidthProperty, new GridLength(30.0, GridUnitType.Pixel), BindingPriority.Template);
				columnDefinitions7.Add(columnDefinition4);
				ColumnDefinitions columnDefinitions8 = grid25.ColumnDefinitions;
				ColumnDefinition columnDefinition5 = new ColumnDefinition();
				columnDefinition5.SetValue(ColumnDefinition.WidthProperty, new GridLength(1.0, GridUnitType.Star), BindingPriority.Template);
				columnDefinitions8.Add(columnDefinition5);
				Controls children14 = grid25.Children;
				Border border30;
				Border border31 = (border30 = new Border());
				((ISupportInitialize)border31).BeginInit();
				children14.Add(border31);
				Border border32 = (border3 = border30);
				context.PushParent(border3);
				Border border33 = border3;
				border33.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border33.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty4 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension12 = new DynamicResourceExtension("ThemeControlMidBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding20 = dynamicResourceExtension12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border33.Bind(backgroundProperty4, binding20);
				StyledProperty<IBrush?> borderBrushProperty3 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension13 = new DynamicResourceExtension("ThemeBorderMidBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding21 = dynamicResourceExtension13.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border33.Bind(borderBrushProperty3, binding21);
				border33.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border33.SetValue(Border.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				TextBlock textBlock;
				TextBlock textBlock2 = (textBlock = new TextBlock());
				((ISupportInitialize)textBlock2).BeginInit();
				border33.Child = textBlock2;
				TextBlock textBlock3;
				TextBlock textBlock4 = (textBlock3 = textBlock);
				context.PushParent(textBlock3);
				TextBlock textBlock5 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension14 = new DynamicResourceExtension("ThemeForegroundBrush");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding22 = dynamicResourceExtension14.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock5.Bind(foregroundProperty, binding22);
				textBlock5.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock5.SetValue(TextBlock.TextProperty, "#", BindingPriority.Template);
				textBlock5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				textBlock5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)textBlock4).EndInit();
				context.PopParent();
				((ISupportInitialize)border32).EndInit();
				Controls children15 = grid25.Children;
				TextBox textBox;
				TextBox textBox2 = (textBox = new TextBox());
				((ISupportInitialize)textBox2).BeginInit();
				children15.Add(textBox2);
				textBox.Name = "PART_HexTextBox";
				service = textBox;
				context.AvaloniaNameScope.Register("PART_HexTextBox", service);
				textBox.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				textBox.SetValue(AutomationProperties.NameProperty, "Hexadecimal Color", BindingPriority.Template);
				textBox.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				textBox.SetValue(TextBox.MaxLengthProperty, 9, BindingPriority.Template);
				textBox.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				textBox.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				((ISupportInitialize)textBox).EndInit();
				context.PopParent();
				((ISupportInitialize)grid24).EndInit();
				context.PopParent();
				((ISupportInitialize)grid16).EndInit();
				Controls children16 = grid13.Children;
				Border border34;
				Border border35 = (border34 = new Border());
				((ISupportInitialize)border35).BeginInit();
				children16.Add(border35);
				Border border36 = (border3 = border34);
				context.PushParent(border3);
				Border border37 = border3;
				border37.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border37.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				StyledProperty<double> heightProperty6 = Layoutable.HeightProperty;
				CompiledBindingExtension obj21 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component1NumericUpDown").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding23 = obj21.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border37.Bind(heightProperty6, binding23);
				StyledProperty<double> widthProperty = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension15 = new DynamicResourceExtension("ColorViewComponentLabelWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding24 = dynamicResourceExtension15.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border37.Bind(widthProperty, binding24);
				StyledProperty<IBrush?> backgroundProperty5 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension16 = new DynamicResourceExtension("ThemeControlMidBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding25 = dynamicResourceExtension16.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border37.Bind(backgroundProperty5, binding25);
				StyledProperty<IBrush?> borderBrushProperty4 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension17 = new DynamicResourceExtension("ThemeBorderMidBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding26 = dynamicResourceExtension17.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border37.Bind(borderBrushProperty4, binding26);
				border37.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border37.SetValue(Border.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				border37.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				border37.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				Panel panel6;
				Panel panel7 = (panel6 = new Panel());
				((ISupportInitialize)panel7).BeginInit();
				border37.Child = panel7;
				Panel panel8 = (panel3 = panel6);
				context.PushParent(panel3);
				Panel panel9 = panel3;
				panel9.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				panel9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Controls children17 = panel9.Children;
				TextBlock textBlock6;
				TextBlock textBlock7 = (textBlock6 = new TextBlock());
				((ISupportInitialize)textBlock7).BeginInit();
				children17.Add(textBlock7);
				TextBlock textBlock8 = (textBlock3 = textBlock6);
				context.PushParent(textBlock3);
				TextBlock textBlock9 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty2 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension18 = new DynamicResourceExtension("ThemeForegroundBrush");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding27 = dynamicResourceExtension18.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock9.Bind(foregroundProperty2, binding27);
				textBlock9.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock9.SetValue(TextBlock.TextProperty, "R", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty2 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding14 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding15 = templateBinding;
				StaticResourceExtension staticResourceExtension16 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj22 = staticResourceExtension16.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding15.Converter = (IValueConverter)obj22;
				templateBinding15.ConverterParameter = ColorModel.Rgba;
				templateBinding15.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock9.Bind(isVisibleProperty2, templateBinding14.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock8).EndInit();
				Controls children18 = panel9.Children;
				TextBlock textBlock10;
				TextBlock textBlock11 = (textBlock10 = new TextBlock());
				((ISupportInitialize)textBlock11).BeginInit();
				children18.Add(textBlock11);
				TextBlock textBlock12 = (textBlock3 = textBlock10);
				context.PushParent(textBlock3);
				TextBlock textBlock13 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty3 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension19 = new DynamicResourceExtension("ThemeForegroundBrush");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding28 = dynamicResourceExtension19.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock13.Bind(foregroundProperty3, binding28);
				textBlock13.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock13.SetValue(TextBlock.TextProperty, "H", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty3 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding16 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding17 = templateBinding;
				StaticResourceExtension staticResourceExtension17 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj23 = staticResourceExtension17.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding17.Converter = (IValueConverter)obj23;
				templateBinding17.ConverterParameter = ColorModel.Hsva;
				templateBinding17.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock13.Bind(isVisibleProperty3, templateBinding16.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock12).EndInit();
				context.PopParent();
				((ISupportInitialize)panel8).EndInit();
				context.PopParent();
				((ISupportInitialize)border36).EndInit();
				Controls children19 = grid13.Children;
				NumericUpDown numericUpDown;
				NumericUpDown numericUpDown2 = (numericUpDown = new NumericUpDown());
				((ISupportInitialize)numericUpDown2).BeginInit();
				children19.Add(numericUpDown2);
				NumericUpDown numericUpDown3;
				NumericUpDown numericUpDown4 = (numericUpDown3 = numericUpDown);
				context.PushParent(numericUpDown3);
				NumericUpDown numericUpDown5 = numericUpDown3;
				numericUpDown5.Name = "Component1NumericUpDown";
				service = numericUpDown5;
				context.AvaloniaNameScope.Register("Component1NumericUpDown", service);
				numericUpDown5.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				numericUpDown5.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				numericUpDown5.SetValue(NumericUpDown.AllowSpinProperty, value: true, BindingPriority.Template);
				numericUpDown5.SetValue(NumericUpDown.ShowButtonSpinnerProperty, value: false, BindingPriority.Template);
				numericUpDown5.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<double> widthProperty2 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension20 = new DynamicResourceExtension("ColorViewComponentTextInputWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding29 = dynamicResourceExtension20.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown5.Bind(widthProperty2, binding29);
				numericUpDown5.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				numericUpDown5.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				numericUpDown5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension18 = new StaticResourceExtension("ColorViewComponentNumberFormat");
				context.ProvideTargetProperty = NumericUpDown.NumberFormatProperty;
				object? obj24 = staticResourceExtension18.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_68.DynamicSetter_5(numericUpDown5, BindingPriority.Template, obj24);
				StyledProperty<decimal> minimumProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component1Slider").Property(RangeBase.MinimumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension binding30 = compiledBindingExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown5.Bind(minimumProperty, binding30);
				StyledProperty<decimal> maximumProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension compiledBindingExtension8 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component1Slider").Property(RangeBase.MaximumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension binding31 = compiledBindingExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown5.Bind(maximumProperty, binding31);
				StyledProperty<decimal?> valueProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension compiledBindingExtension9 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component1Slider").Property(RangeBase.ValueProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension binding32 = compiledBindingExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown5.Bind(valueProperty, binding32);
				numericUpDown5.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)numericUpDown4).EndInit();
				Controls children20 = grid13.Children;
				ColorSlider colorSlider10;
				ColorSlider colorSlider11 = (colorSlider10 = new ColorSlider());
				((ISupportInitialize)colorSlider11).BeginInit();
				children20.Add(colorSlider11);
				ColorSlider colorSlider12 = (colorSlider3 = colorSlider10);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider13 = colorSlider3;
				colorSlider13.Name = "Component1Slider";
				service = colorSlider13;
				context.AvaloniaNameScope.Register("Component1Slider", service);
				colorSlider13.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider13.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				colorSlider13.SetValue(Slider.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				colorSlider13.SetValue(ColorSlider.IsRoundingEnabledProperty, value: true, BindingPriority.Template);
				colorSlider13.SetValue(Slider.IsSnapToTickEnabledProperty, value: true, BindingPriority.Template);
				colorSlider13.SetValue(Slider.TickFrequencyProperty, 1.0, BindingPriority.Template);
				colorSlider13.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Component1, BindingPriority.Template);
				colorSlider13.Bind(ColorSlider.ColorModelProperty, new TemplateBinding(ColorView.ColorModelProperty)
				{
					Mode = BindingMode.OneWay
				}.ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty4 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension obj25 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding33 = obj25.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider13.Bind(hsvColorProperty4, binding33);
				colorSlider13.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSlider13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				colorSlider13.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentSliderVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorSlider12).EndInit();
				Controls children21 = grid13.Children;
				Border border38;
				Border border39 = (border38 = new Border());
				((ISupportInitialize)border39).BeginInit();
				children21.Add(border39);
				Border border40 = (border3 = border38);
				context.PushParent(border3);
				Border border41 = border3;
				border41.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border41.SetValue(Grid.RowProperty, 3, BindingPriority.Template);
				StyledProperty<double> widthProperty3 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension21 = new DynamicResourceExtension("ColorViewComponentLabelWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding34 = dynamicResourceExtension21.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border41.Bind(widthProperty3, binding34);
				StyledProperty<double> heightProperty7 = Layoutable.HeightProperty;
				CompiledBindingExtension obj26 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component2NumericUpDown").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding35 = obj26.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border41.Bind(heightProperty7, binding35);
				StyledProperty<IBrush?> backgroundProperty6 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension22 = new DynamicResourceExtension("ThemeControlMidBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding36 = dynamicResourceExtension22.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border41.Bind(backgroundProperty6, binding36);
				StyledProperty<IBrush?> borderBrushProperty5 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension23 = new DynamicResourceExtension("ThemeBorderMidBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding37 = dynamicResourceExtension23.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border41.Bind(borderBrushProperty5, binding37);
				border41.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border41.SetValue(Border.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				border41.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				border41.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				Panel panel10;
				Panel panel11 = (panel10 = new Panel());
				((ISupportInitialize)panel11).BeginInit();
				border41.Child = panel11;
				Panel panel12 = (panel3 = panel10);
				context.PushParent(panel3);
				Panel panel13 = panel3;
				panel13.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				panel13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Controls children22 = panel13.Children;
				TextBlock textBlock14;
				TextBlock textBlock15 = (textBlock14 = new TextBlock());
				((ISupportInitialize)textBlock15).BeginInit();
				children22.Add(textBlock15);
				TextBlock textBlock16 = (textBlock3 = textBlock14);
				context.PushParent(textBlock3);
				TextBlock textBlock17 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty4 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension24 = new DynamicResourceExtension("ThemeForegroundBrush");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding38 = dynamicResourceExtension24.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock17.Bind(foregroundProperty4, binding38);
				textBlock17.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock17.SetValue(TextBlock.TextProperty, "G", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty4 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding18 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding19 = templateBinding;
				StaticResourceExtension staticResourceExtension19 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj27 = staticResourceExtension19.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding19.Converter = (IValueConverter)obj27;
				templateBinding19.ConverterParameter = ColorModel.Rgba;
				templateBinding19.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock17.Bind(isVisibleProperty4, templateBinding18.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock16).EndInit();
				Controls children23 = panel13.Children;
				TextBlock textBlock18;
				TextBlock textBlock19 = (textBlock18 = new TextBlock());
				((ISupportInitialize)textBlock19).BeginInit();
				children23.Add(textBlock19);
				TextBlock textBlock20 = (textBlock3 = textBlock18);
				context.PushParent(textBlock3);
				TextBlock textBlock21 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty5 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension25 = new DynamicResourceExtension("ThemeForegroundBrush");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding39 = dynamicResourceExtension25.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock21.Bind(foregroundProperty5, binding39);
				textBlock21.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock21.SetValue(TextBlock.TextProperty, "S", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty5 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding20 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding21 = templateBinding;
				StaticResourceExtension staticResourceExtension20 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj28 = staticResourceExtension20.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding21.Converter = (IValueConverter)obj28;
				templateBinding21.ConverterParameter = ColorModel.Hsva;
				templateBinding21.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock21.Bind(isVisibleProperty5, templateBinding20.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock20).EndInit();
				context.PopParent();
				((ISupportInitialize)panel12).EndInit();
				context.PopParent();
				((ISupportInitialize)border40).EndInit();
				Controls children24 = grid13.Children;
				NumericUpDown numericUpDown6;
				NumericUpDown numericUpDown7 = (numericUpDown6 = new NumericUpDown());
				((ISupportInitialize)numericUpDown7).BeginInit();
				children24.Add(numericUpDown7);
				NumericUpDown numericUpDown8 = (numericUpDown3 = numericUpDown6);
				context.PushParent(numericUpDown3);
				NumericUpDown numericUpDown9 = numericUpDown3;
				numericUpDown9.Name = "Component2NumericUpDown";
				service = numericUpDown9;
				context.AvaloniaNameScope.Register("Component2NumericUpDown", service);
				numericUpDown9.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				numericUpDown9.SetValue(Grid.RowProperty, 3, BindingPriority.Template);
				numericUpDown9.SetValue(NumericUpDown.AllowSpinProperty, value: true, BindingPriority.Template);
				numericUpDown9.SetValue(NumericUpDown.ShowButtonSpinnerProperty, value: false, BindingPriority.Template);
				numericUpDown9.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<double> widthProperty4 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension26 = new DynamicResourceExtension("ColorViewComponentTextInputWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding40 = dynamicResourceExtension26.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown9.Bind(widthProperty4, binding40);
				numericUpDown9.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				numericUpDown9.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				numericUpDown9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension21 = new StaticResourceExtension("ColorViewComponentNumberFormat");
				context.ProvideTargetProperty = NumericUpDown.NumberFormatProperty;
				object? obj29 = staticResourceExtension21.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_68.DynamicSetter_5(numericUpDown9, BindingPriority.Template, obj29);
				StyledProperty<decimal> minimumProperty2 = NumericUpDown.MinimumProperty;
				CompiledBindingExtension compiledBindingExtension10 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component2Slider").Property(RangeBase.MinimumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension binding41 = compiledBindingExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown9.Bind(minimumProperty2, binding41);
				StyledProperty<decimal> maximumProperty2 = NumericUpDown.MaximumProperty;
				CompiledBindingExtension compiledBindingExtension11 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component2Slider").Property(RangeBase.MaximumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension binding42 = compiledBindingExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown9.Bind(maximumProperty2, binding42);
				StyledProperty<decimal?> valueProperty2 = NumericUpDown.ValueProperty;
				CompiledBindingExtension compiledBindingExtension12 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component2Slider").Property(RangeBase.ValueProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension binding43 = compiledBindingExtension12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown9.Bind(valueProperty2, binding43);
				numericUpDown9.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)numericUpDown8).EndInit();
				Controls children25 = grid13.Children;
				ColorSlider colorSlider14;
				ColorSlider colorSlider15 = (colorSlider14 = new ColorSlider());
				((ISupportInitialize)colorSlider15).BeginInit();
				children25.Add(colorSlider15);
				ColorSlider colorSlider16 = (colorSlider3 = colorSlider14);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider17 = colorSlider3;
				colorSlider17.Name = "Component2Slider";
				service = colorSlider17;
				context.AvaloniaNameScope.Register("Component2Slider", service);
				colorSlider17.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider17.SetValue(Grid.RowProperty, 3, BindingPriority.Template);
				colorSlider17.SetValue(Slider.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				colorSlider17.SetValue(ColorSlider.IsRoundingEnabledProperty, value: true, BindingPriority.Template);
				colorSlider17.SetValue(Slider.IsSnapToTickEnabledProperty, value: true, BindingPriority.Template);
				colorSlider17.SetValue(Slider.TickFrequencyProperty, 1.0, BindingPriority.Template);
				colorSlider17.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Component2, BindingPriority.Template);
				colorSlider17.Bind(ColorSlider.ColorModelProperty, new TemplateBinding(ColorView.ColorModelProperty)
				{
					Mode = BindingMode.OneWay
				}.ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty5 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension obj30 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding44 = obj30.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider17.Bind(hsvColorProperty5, binding44);
				colorSlider17.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSlider17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				colorSlider17.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentSliderVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorSlider16).EndInit();
				Controls children26 = grid13.Children;
				Border border42;
				Border border43 = (border42 = new Border());
				((ISupportInitialize)border43).BeginInit();
				children26.Add(border43);
				Border border44 = (border3 = border42);
				context.PushParent(border3);
				Border border45 = border3;
				border45.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border45.SetValue(Grid.RowProperty, 4, BindingPriority.Template);
				StyledProperty<double> widthProperty5 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension27 = new DynamicResourceExtension("ColorViewComponentLabelWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding45 = dynamicResourceExtension27.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border45.Bind(widthProperty5, binding45);
				StyledProperty<double> heightProperty8 = Layoutable.HeightProperty;
				CompiledBindingExtension obj31 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component3NumericUpDown").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding46 = obj31.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border45.Bind(heightProperty8, binding46);
				StyledProperty<IBrush?> backgroundProperty7 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension28 = new DynamicResourceExtension("ThemeControlMidBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding47 = dynamicResourceExtension28.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border45.Bind(backgroundProperty7, binding47);
				StyledProperty<IBrush?> borderBrushProperty6 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension29 = new DynamicResourceExtension("ThemeBorderMidBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding48 = dynamicResourceExtension29.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border45.Bind(borderBrushProperty6, binding48);
				border45.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border45.SetValue(Border.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				border45.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				border45.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				Panel panel14;
				Panel panel15 = (panel14 = new Panel());
				((ISupportInitialize)panel15).BeginInit();
				border45.Child = panel15;
				Panel panel16 = (panel3 = panel14);
				context.PushParent(panel3);
				Panel panel17 = panel3;
				panel17.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				panel17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Controls children27 = panel17.Children;
				TextBlock textBlock22;
				TextBlock textBlock23 = (textBlock22 = new TextBlock());
				((ISupportInitialize)textBlock23).BeginInit();
				children27.Add(textBlock23);
				TextBlock textBlock24 = (textBlock3 = textBlock22);
				context.PushParent(textBlock3);
				TextBlock textBlock25 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty6 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension30 = new DynamicResourceExtension("ThemeForegroundBrush");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding49 = dynamicResourceExtension30.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock25.Bind(foregroundProperty6, binding49);
				textBlock25.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock25.SetValue(TextBlock.TextProperty, "B", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty6 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding22 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding23 = templateBinding;
				StaticResourceExtension staticResourceExtension22 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj32 = staticResourceExtension22.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding23.Converter = (IValueConverter)obj32;
				templateBinding23.ConverterParameter = ColorModel.Rgba;
				templateBinding23.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock25.Bind(isVisibleProperty6, templateBinding22.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock24).EndInit();
				Controls children28 = panel17.Children;
				TextBlock textBlock26;
				TextBlock textBlock27 = (textBlock26 = new TextBlock());
				((ISupportInitialize)textBlock27).BeginInit();
				children28.Add(textBlock27);
				TextBlock textBlock28 = (textBlock3 = textBlock26);
				context.PushParent(textBlock3);
				TextBlock textBlock29 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty7 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension31 = new DynamicResourceExtension("ThemeForegroundBrush");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding50 = dynamicResourceExtension31.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock29.Bind(foregroundProperty7, binding50);
				textBlock29.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock29.SetValue(TextBlock.TextProperty, "V", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty7 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding24 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding25 = templateBinding;
				StaticResourceExtension staticResourceExtension23 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj33 = staticResourceExtension23.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding25.Converter = (IValueConverter)obj33;
				templateBinding25.ConverterParameter = ColorModel.Hsva;
				templateBinding25.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock29.Bind(isVisibleProperty7, templateBinding24.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock28).EndInit();
				context.PopParent();
				((ISupportInitialize)panel16).EndInit();
				context.PopParent();
				((ISupportInitialize)border44).EndInit();
				Controls children29 = grid13.Children;
				NumericUpDown numericUpDown10;
				NumericUpDown numericUpDown11 = (numericUpDown10 = new NumericUpDown());
				((ISupportInitialize)numericUpDown11).BeginInit();
				children29.Add(numericUpDown11);
				NumericUpDown numericUpDown12 = (numericUpDown3 = numericUpDown10);
				context.PushParent(numericUpDown3);
				NumericUpDown numericUpDown13 = numericUpDown3;
				numericUpDown13.Name = "Component3NumericUpDown";
				service = numericUpDown13;
				context.AvaloniaNameScope.Register("Component3NumericUpDown", service);
				numericUpDown13.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				numericUpDown13.SetValue(Grid.RowProperty, 4, BindingPriority.Template);
				numericUpDown13.SetValue(NumericUpDown.AllowSpinProperty, value: true, BindingPriority.Template);
				numericUpDown13.SetValue(NumericUpDown.ShowButtonSpinnerProperty, value: false, BindingPriority.Template);
				numericUpDown13.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<double> widthProperty6 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension32 = new DynamicResourceExtension("ColorViewComponentTextInputWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding51 = dynamicResourceExtension32.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown13.Bind(widthProperty6, binding51);
				numericUpDown13.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				numericUpDown13.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				numericUpDown13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension24 = new StaticResourceExtension("ColorViewComponentNumberFormat");
				context.ProvideTargetProperty = NumericUpDown.NumberFormatProperty;
				object? obj34 = staticResourceExtension24.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_68.DynamicSetter_5(numericUpDown13, BindingPriority.Template, obj34);
				StyledProperty<decimal> minimumProperty3 = NumericUpDown.MinimumProperty;
				CompiledBindingExtension compiledBindingExtension13 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component3Slider").Property(RangeBase.MinimumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension binding52 = compiledBindingExtension13.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown13.Bind(minimumProperty3, binding52);
				StyledProperty<decimal> maximumProperty3 = NumericUpDown.MaximumProperty;
				CompiledBindingExtension compiledBindingExtension14 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component3Slider").Property(RangeBase.MaximumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension binding53 = compiledBindingExtension14.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown13.Bind(maximumProperty3, binding53);
				StyledProperty<decimal?> valueProperty3 = NumericUpDown.ValueProperty;
				CompiledBindingExtension compiledBindingExtension15 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component3Slider").Property(RangeBase.ValueProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension binding54 = compiledBindingExtension15.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown13.Bind(valueProperty3, binding54);
				numericUpDown13.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)numericUpDown12).EndInit();
				Controls children30 = grid13.Children;
				ColorSlider colorSlider18;
				ColorSlider colorSlider19 = (colorSlider18 = new ColorSlider());
				((ISupportInitialize)colorSlider19).BeginInit();
				children30.Add(colorSlider19);
				ColorSlider colorSlider20 = (colorSlider3 = colorSlider18);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider21 = colorSlider3;
				colorSlider21.Name = "Component3Slider";
				service = colorSlider21;
				context.AvaloniaNameScope.Register("Component3Slider", service);
				colorSlider21.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider21.SetValue(Grid.RowProperty, 4, BindingPriority.Template);
				colorSlider21.SetValue(Slider.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				colorSlider21.SetValue(ColorSlider.IsRoundingEnabledProperty, value: true, BindingPriority.Template);
				colorSlider21.SetValue(Slider.IsSnapToTickEnabledProperty, value: true, BindingPriority.Template);
				colorSlider21.SetValue(Slider.TickFrequencyProperty, 1.0, BindingPriority.Template);
				colorSlider21.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Component3, BindingPriority.Template);
				colorSlider21.Bind(ColorSlider.ColorModelProperty, new TemplateBinding(ColorView.ColorModelProperty)
				{
					Mode = BindingMode.OneWay
				}.ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty6 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension obj35 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding55 = obj35.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider21.Bind(hsvColorProperty6, binding55);
				colorSlider21.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSlider21.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				colorSlider21.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentSliderVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorSlider20).EndInit();
				Controls children31 = grid13.Children;
				Border border46;
				Border border47 = (border46 = new Border());
				((ISupportInitialize)border47).BeginInit();
				children31.Add(border47);
				Border border48 = (border3 = border46);
				context.PushParent(border3);
				Border border49 = border3;
				border49.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border49.SetValue(Grid.RowProperty, 5, BindingPriority.Template);
				StyledProperty<double> widthProperty7 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension33 = new DynamicResourceExtension("ColorViewComponentLabelWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding56 = dynamicResourceExtension33.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border49.Bind(widthProperty7, binding56);
				StyledProperty<double> heightProperty9 = Layoutable.HeightProperty;
				CompiledBindingExtension obj36 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "AlphaComponentNumericUpDown").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding57 = obj36.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border49.Bind(heightProperty9, binding57);
				StyledProperty<IBrush?> backgroundProperty8 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension34 = new DynamicResourceExtension("ThemeControlMidBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding58 = dynamicResourceExtension34.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border49.Bind(backgroundProperty8, binding58);
				StyledProperty<IBrush?> borderBrushProperty7 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension35 = new DynamicResourceExtension("ThemeBorderMidBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding59 = dynamicResourceExtension35.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border49.Bind(borderBrushProperty7, binding59);
				border49.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border49.SetValue(Border.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				border49.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				border49.Bind(InputElement.IsEnabledProperty, new TemplateBinding(ColorView.IsAlphaEnabledProperty).ProvideValue());
				StyledProperty<bool> isVisibleProperty8 = Visual.IsVisibleProperty;
				MultiBinding binding60 = (multiBinding = new MultiBinding());
				context.PushParent(multiBinding);
				MultiBinding multiBinding3 = multiBinding;
				multiBinding3.Converter = BoolConverters.And;
				IList<IBinding> bindings2 = multiBinding3.Bindings;
				CompiledBindingExtension obj37 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsAlphaVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item2 = obj37.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings2.Add(item2);
				IList<IBinding> bindings3 = multiBinding3.Bindings;
				CompiledBindingExtension obj38 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsComponentTextInputVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item3 = obj38.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings3.Add(item3);
				context.PopParent();
				border49.Bind(isVisibleProperty8, binding60);
				TextBlock textBlock30;
				TextBlock textBlock31 = (textBlock30 = new TextBlock());
				((ISupportInitialize)textBlock31).BeginInit();
				border49.Child = textBlock31;
				TextBlock textBlock32 = (textBlock3 = textBlock30);
				context.PushParent(textBlock3);
				TextBlock textBlock33 = textBlock3;
				textBlock33.Name = "AlphaComponentTextBlock";
				service = textBlock33;
				context.AvaloniaNameScope.Register("AlphaComponentTextBlock", service);
				StyledProperty<IBrush?> foregroundProperty8 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension36 = new DynamicResourceExtension("ThemeForegroundBrush");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding61 = dynamicResourceExtension36.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock33.Bind(foregroundProperty8, binding61);
				textBlock33.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock33.SetValue(TextBlock.TextProperty, "A", BindingPriority.Template);
				textBlock33.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				textBlock33.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)textBlock32).EndInit();
				context.PopParent();
				((ISupportInitialize)border48).EndInit();
				Controls children32 = grid13.Children;
				NumericUpDown numericUpDown14;
				NumericUpDown numericUpDown15 = (numericUpDown14 = new NumericUpDown());
				((ISupportInitialize)numericUpDown15).BeginInit();
				children32.Add(numericUpDown15);
				NumericUpDown numericUpDown16 = (numericUpDown3 = numericUpDown14);
				context.PushParent(numericUpDown3);
				NumericUpDown numericUpDown17 = numericUpDown3;
				numericUpDown17.Name = "AlphaComponentNumericUpDown";
				service = numericUpDown17;
				context.AvaloniaNameScope.Register("AlphaComponentNumericUpDown", service);
				numericUpDown17.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				numericUpDown17.SetValue(Grid.RowProperty, 5, BindingPriority.Template);
				numericUpDown17.SetValue(NumericUpDown.AllowSpinProperty, value: true, BindingPriority.Template);
				numericUpDown17.SetValue(NumericUpDown.ShowButtonSpinnerProperty, value: false, BindingPriority.Template);
				numericUpDown17.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<double> widthProperty8 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension37 = new DynamicResourceExtension("ColorViewComponentTextInputWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding62 = dynamicResourceExtension37.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown17.Bind(widthProperty8, binding62);
				numericUpDown17.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				numericUpDown17.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				numericUpDown17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension25 = new StaticResourceExtension("ColorViewComponentNumberFormat");
				context.ProvideTargetProperty = NumericUpDown.NumberFormatProperty;
				object? obj39 = staticResourceExtension25.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_68.DynamicSetter_5(numericUpDown17, BindingPriority.Template, obj39);
				StyledProperty<decimal> minimumProperty4 = NumericUpDown.MinimumProperty;
				CompiledBindingExtension compiledBindingExtension16 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "AlphaComponentSlider").Property(RangeBase.MinimumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension binding63 = compiledBindingExtension16.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown17.Bind(minimumProperty4, binding63);
				StyledProperty<decimal> maximumProperty4 = NumericUpDown.MaximumProperty;
				CompiledBindingExtension compiledBindingExtension17 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "AlphaComponentSlider").Property(RangeBase.MaximumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension binding64 = compiledBindingExtension17.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown17.Bind(maximumProperty4, binding64);
				StyledProperty<decimal?> valueProperty4 = NumericUpDown.ValueProperty;
				CompiledBindingExtension compiledBindingExtension18 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "AlphaComponentSlider").Property(RangeBase.ValueProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension binding65 = compiledBindingExtension18.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown17.Bind(valueProperty4, binding65);
				numericUpDown17.Bind(InputElement.IsEnabledProperty, new TemplateBinding(ColorView.IsAlphaEnabledProperty).ProvideValue());
				StyledProperty<bool> isVisibleProperty9 = Visual.IsVisibleProperty;
				MultiBinding binding66 = (multiBinding = new MultiBinding());
				context.PushParent(multiBinding);
				MultiBinding multiBinding4 = multiBinding;
				multiBinding4.Converter = BoolConverters.And;
				IList<IBinding> bindings4 = multiBinding4.Bindings;
				CompiledBindingExtension obj40 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsAlphaVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item4 = obj40.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings4.Add(item4);
				IList<IBinding> bindings5 = multiBinding4.Bindings;
				CompiledBindingExtension obj41 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsComponentTextInputVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item5 = obj41.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings5.Add(item5);
				context.PopParent();
				numericUpDown17.Bind(isVisibleProperty9, binding66);
				context.PopParent();
				((ISupportInitialize)numericUpDown16).EndInit();
				Controls children33 = grid13.Children;
				ColorSlider colorSlider22;
				ColorSlider colorSlider23 = (colorSlider22 = new ColorSlider());
				((ISupportInitialize)colorSlider23).BeginInit();
				children33.Add(colorSlider23);
				ColorSlider colorSlider24 = (colorSlider3 = colorSlider22);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider25 = colorSlider3;
				colorSlider25.Name = "AlphaComponentSlider";
				service = colorSlider25;
				context.AvaloniaNameScope.Register("AlphaComponentSlider", service);
				colorSlider25.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider25.SetValue(Grid.RowProperty, 5, BindingPriority.Template);
				colorSlider25.SetValue(Slider.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				colorSlider25.SetValue(ColorSlider.IsRoundingEnabledProperty, value: true, BindingPriority.Template);
				colorSlider25.SetValue(Slider.IsSnapToTickEnabledProperty, value: true, BindingPriority.Template);
				colorSlider25.SetValue(Slider.TickFrequencyProperty, 1.0, BindingPriority.Template);
				colorSlider25.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Alpha, BindingPriority.Template);
				colorSlider25.Bind(ColorSlider.ColorModelProperty, new TemplateBinding(ColorView.ColorModelProperty)
				{
					Mode = BindingMode.OneWay
				}.ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty7 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension obj42 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding67 = obj42.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider25.Bind(hsvColorProperty7, binding67);
				colorSlider25.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSlider25.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				colorSlider25.Bind(InputElement.IsEnabledProperty, new TemplateBinding(ColorView.IsAlphaEnabledProperty).ProvideValue());
				StyledProperty<bool> isVisibleProperty10 = Visual.IsVisibleProperty;
				MultiBinding binding68 = (multiBinding = new MultiBinding());
				context.PushParent(multiBinding);
				MultiBinding multiBinding5 = multiBinding;
				multiBinding5.Converter = BoolConverters.And;
				IList<IBinding> bindings6 = multiBinding5.Bindings;
				CompiledBindingExtension obj43 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsAlphaVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item6 = obj43.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings6.Add(item6);
				IList<IBinding> bindings7 = multiBinding5.Bindings;
				CompiledBindingExtension obj44 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsComponentSliderVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item7 = obj44.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings7.Add(item7);
				context.PopParent();
				colorSlider25.Bind(isVisibleProperty10, binding68);
				context.PopParent();
				((ISupportInitialize)colorSlider24).EndInit();
				context.PopParent();
				((ISupportInitialize)grid12).EndInit();
				context.PopParent();
				((ISupportInitialize)tabItem12).EndInit();
				context.PopParent();
				((ISupportInitialize)tabControl4).EndInit();
				Controls children34 = grid5.Children;
				ColorPreviewer colorPreviewer;
				ColorPreviewer colorPreviewer2 = (colorPreviewer = new ColorPreviewer());
				((ISupportInitialize)colorPreviewer2).BeginInit();
				children34.Add(colorPreviewer2);
				ColorPreviewer colorPreviewer3;
				ColorPreviewer colorPreviewer4 = (colorPreviewer3 = colorPreviewer);
				context.PushParent(colorPreviewer3);
				colorPreviewer3.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
				StyledProperty<HsvColor> hsvColorProperty8 = ColorPreviewer.HsvColorProperty;
				CompiledBindingExtension obj45 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorPreviewer.HsvColorProperty;
				CompiledBindingExtension binding69 = obj45.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorPreviewer3.Bind(hsvColorProperty8, binding69);
				colorPreviewer3.Bind(ColorPreviewer.IsAccentColorsVisibleProperty, new TemplateBinding(ColorView.IsAccentColorsVisibleProperty).ProvideValue());
				colorPreviewer3.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 0.0, 12.0, 12.0), BindingPriority.Template);
				colorPreviewer3.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorPreviewVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorPreviewer4).EndInit();
				context.PopParent();
				((ISupportInitialize)grid4).EndInit();
				context.PopParent();
				dropDownButton.SetValue(flyoutProperty, value, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
			controlTheme.TargetType = typeof(ColorPicker);
			Setter setter = new Setter();
			setter.Property = TemplatedControl.CornerRadiusProperty;
			setter.Value = new CornerRadius(0.0, 0.0, 0.0, 0.0);
			controlTheme.Add(setter);
			Setter setter2 = new Setter();
			setter2.Property = ColorView.HexInputAlphaPositionProperty;
			setter2.Value = AlphaComponentPosition.Trailing;
			controlTheme.Add(setter2);
			Setter setter3 = new Setter();
			setter3.Property = Layoutable.HeightProperty;
			setter3.Value = 32.0;
			controlTheme.Add(setter3);
			Setter setter4 = new Setter();
			setter4.Property = Layoutable.WidthProperty;
			setter4.Value = 64.0;
			controlTheme.Add(setter4);
			Setter setter5 = new Setter();
			setter5.Property = Layoutable.MinWidthProperty;
			setter5.Value = 64.0;
			controlTheme.Add(setter5);
			Setter setter6 = new Setter();
			setter6.Property = ColorView.PaletteProperty;
			setter6.Value = new FluentColorPalette();
			controlTheme.Add(setter6);
			Setter setter7;
			Setter setter8 = (setter7 = new Setter());
			context.PushParent(setter7);
			setter7.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.TargetType = typeof(ColorPicker);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_67.Build, context);
			context.PopParent();
			setter7.Value = value;
			context.PopParent();
			controlTheme.Add(setter8);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_73
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new AccentColorConverter();
		}
	}

	private class XamlClosure_74
	{
		private class XamlClosure_75
		{
			private class DynamicSetters_76
			{
				public static void DynamicSetter_1(Layoutable P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(Layoutable.HeightProperty, binding);
						return;
					}
					if (P_2 is double value)
					{
						int priority = (int)P_1;
						P_0.SetValue(Layoutable.HeightProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						throw new NullReferenceException();
					}
					throw new InvalidCastException();
				}

				public static void DynamicSetter_2(Layoutable P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(Layoutable.WidthProperty, binding);
						return;
					}
					if (P_2 is double value)
					{
						int priority = (int)P_1;
						P_0.SetValue(Layoutable.WidthProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						throw new NullReferenceException();
					}
					throw new InvalidCastException();
				}

				public static void DynamicSetter_3(Border P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(Border.BackgroundProperty, binding);
						return;
					}
					if (P_2 is IBrush)
					{
						IBrush value = (IBrush)P_2;
						int priority = (int)P_1;
						P_0.SetValue(Border.BackgroundProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						IBrush value = (IBrush)P_2;
						int priority = (int)P_1;
						P_0.SetValue(Border.BackgroundProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
				Panel panel2 = panel;
				Controls children = panel2.Children;
				Grid grid;
				Grid grid2 = (grid = new Grid());
				((ISupportInitialize)grid2).BeginInit();
				children.Add(grid2);
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
				grid5.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorPreviewer.IsAccentColorsVisibleProperty).ProvideValue());
				Controls children2 = grid5.Children;
				Grid grid6;
				Grid grid7 = (grid6 = new Grid());
				((ISupportInitialize)grid7).BeginInit();
				children2.Add(grid7);
				Grid grid8 = (grid3 = grid6);
				context.PushParent(grid3);
				Grid grid9 = grid3;
				grid9.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ColorPreviewerAccentSectionHeight");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				object? obj = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_76.DynamicSetter_1(grid9, BindingPriority.Template, obj);
				StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("ColorPreviewerAccentSectionWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				object? obj2 = staticResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_76.DynamicSetter_2(grid9, BindingPriority.Template, obj2);
				ColumnDefinitions columnDefinitions2 = new ColumnDefinitions();
				columnDefinitions2.Capacity = 2;
				columnDefinitions2.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions2.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid9.ColumnDefinitions = columnDefinitions2;
				grid9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Controls children3 = grid9.Children;
				Border border;
				Border border2 = (border = new Border());
				((ISupportInitialize)border2).BeginInit();
				children3.Add(border2);
				Border border3;
				Border border4 = (border3 = border);
				context.PushParent(border3);
				Border border5 = border3;
				border5.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border5.SetValue(Grid.ColumnSpanProperty, 2, BindingPriority.Template);
				border5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension3 = new StaticResourceExtension("ColorControlCheckeredBackgroundBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				object? obj3 = staticResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_76.DynamicSetter_3(border5, BindingPriority.Template, obj3);
				context.PopParent();
				((ISupportInitialize)border4).EndInit();
				Controls children4 = grid9.Children;
				Border border6;
				Border border7 = (border6 = new Border());
				((ISupportInitialize)border7).BeginInit();
				children4.Add(border7);
				Border border8 = (border3 = border6);
				context.PushParent(border3);
				Border border9 = border3;
				border9.Name = "PART_AccentDecrement2Border";
				service = border9;
				context.AvaloniaNameScope.Register("PART_AccentDecrement2Border", service);
				border9.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				StyledProperty<CornerRadius> cornerRadiusProperty = Border.CornerRadiusProperty;
				TemplateBinding templateBinding;
				TemplateBinding templateBinding2 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding3 = templateBinding;
				StaticResourceExtension staticResourceExtension4 = new StaticResourceExtension("LeftCornerRadiusFilterConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj4 = staticResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding3.Converter = (IValueConverter)obj4;
				context.PopParent();
				border9.Bind(cornerRadiusProperty, templateBinding2.ProvideValue());
				border9.SetValue(Control.TagProperty, "-2", BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty = Border.BackgroundProperty;
				TemplateBinding templateBinding4 = (templateBinding = new TemplateBinding(ColorPreviewer.HsvColorProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding5 = templateBinding;
				StaticResourceExtension staticResourceExtension5 = new StaticResourceExtension("AccentColorConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj5 = staticResourceExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding5.Converter = (IValueConverter)obj5;
				templateBinding5.ConverterParameter = "-2";
				context.PopParent();
				border9.Bind(backgroundProperty, templateBinding4.ProvideValue());
				context.PopParent();
				((ISupportInitialize)border8).EndInit();
				Controls children5 = grid9.Children;
				Border border10;
				Border border11 = (border10 = new Border());
				((ISupportInitialize)border11).BeginInit();
				children5.Add(border11);
				Border border12 = (border3 = border10);
				context.PushParent(border3);
				Border border13 = border3;
				border13.Name = "PART_AccentDecrement1Border";
				service = border13;
				context.AvaloniaNameScope.Register("PART_AccentDecrement1Border", service);
				border13.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				border13.SetValue(Control.TagProperty, "-1", BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty2 = Border.BackgroundProperty;
				TemplateBinding templateBinding6 = (templateBinding = new TemplateBinding(ColorPreviewer.HsvColorProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding7 = templateBinding;
				StaticResourceExtension staticResourceExtension6 = new StaticResourceExtension("AccentColorConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj6 = staticResourceExtension6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding7.Converter = (IValueConverter)obj6;
				templateBinding7.ConverterParameter = "-1";
				context.PopParent();
				border13.Bind(backgroundProperty2, templateBinding6.ProvideValue());
				context.PopParent();
				((ISupportInitialize)border12).EndInit();
				context.PopParent();
				((ISupportInitialize)grid8).EndInit();
				Controls children6 = grid5.Children;
				Grid grid10;
				Grid grid11 = (grid10 = new Grid());
				((ISupportInitialize)grid11).BeginInit();
				children6.Add(grid11);
				Grid grid12 = (grid3 = grid10);
				context.PushParent(grid3);
				Grid grid13 = grid3;
				grid13.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension7 = new StaticResourceExtension("ColorPreviewerAccentSectionHeight");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				object? obj7 = staticResourceExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_76.DynamicSetter_1(grid13, BindingPriority.Template, obj7);
				StaticResourceExtension staticResourceExtension8 = new StaticResourceExtension("ColorPreviewerAccentSectionWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				object? obj8 = staticResourceExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_76.DynamicSetter_2(grid13, BindingPriority.Template, obj8);
				ColumnDefinitions columnDefinitions3 = new ColumnDefinitions();
				columnDefinitions3.Capacity = 2;
				columnDefinitions3.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions3.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid13.ColumnDefinitions = columnDefinitions3;
				grid13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Controls children7 = grid13.Children;
				Border border14;
				Border border15 = (border14 = new Border());
				((ISupportInitialize)border15).BeginInit();
				children7.Add(border15);
				Border border16 = (border3 = border14);
				context.PushParent(border3);
				Border border17 = border3;
				border17.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border17.SetValue(Grid.ColumnSpanProperty, 2, BindingPriority.Template);
				border17.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension9 = new StaticResourceExtension("ColorControlCheckeredBackgroundBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				object? obj9 = staticResourceExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_76.DynamicSetter_3(border17, BindingPriority.Template, obj9);
				context.PopParent();
				((ISupportInitialize)border16).EndInit();
				Controls children8 = grid13.Children;
				Border border18;
				Border border19 = (border18 = new Border());
				((ISupportInitialize)border19).BeginInit();
				children8.Add(border19);
				Border border20 = (border3 = border18);
				context.PushParent(border3);
				Border border21 = border3;
				border21.Name = "PART_AccentIncrement1Border";
				service = border21;
				context.AvaloniaNameScope.Register("PART_AccentIncrement1Border", service);
				border21.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border21.SetValue(Control.TagProperty, "1", BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty3 = Border.BackgroundProperty;
				TemplateBinding templateBinding8 = (templateBinding = new TemplateBinding(ColorPreviewer.HsvColorProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding9 = templateBinding;
				StaticResourceExtension staticResourceExtension10 = new StaticResourceExtension("AccentColorConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj10 = staticResourceExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding9.Converter = (IValueConverter)obj10;
				templateBinding9.ConverterParameter = "1";
				context.PopParent();
				border21.Bind(backgroundProperty3, templateBinding8.ProvideValue());
				context.PopParent();
				((ISupportInitialize)border20).EndInit();
				Controls children9 = grid13.Children;
				Border border22;
				Border border23 = (border22 = new Border());
				((ISupportInitialize)border23).BeginInit();
				children9.Add(border23);
				Border border24 = (border3 = border22);
				context.PushParent(border3);
				Border border25 = border3;
				border25.Name = "PART_AccentIncrement2Border";
				service = border25;
				context.AvaloniaNameScope.Register("PART_AccentIncrement2Border", service);
				border25.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				StyledProperty<CornerRadius> cornerRadiusProperty2 = Border.CornerRadiusProperty;
				TemplateBinding templateBinding10 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding11 = templateBinding;
				StaticResourceExtension staticResourceExtension11 = new StaticResourceExtension("RightCornerRadiusFilterConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj11 = staticResourceExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding11.Converter = (IValueConverter)obj11;
				context.PopParent();
				border25.Bind(cornerRadiusProperty2, templateBinding10.ProvideValue());
				border25.SetValue(Control.TagProperty, "2", BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty4 = Border.BackgroundProperty;
				TemplateBinding templateBinding12 = (templateBinding = new TemplateBinding(ColorPreviewer.HsvColorProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding13 = templateBinding;
				StaticResourceExtension staticResourceExtension12 = new StaticResourceExtension("AccentColorConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj12 = staticResourceExtension12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding13.Converter = (IValueConverter)obj12;
				templateBinding13.ConverterParameter = "2";
				context.PopParent();
				border25.Bind(backgroundProperty4, templateBinding12.ProvideValue());
				context.PopParent();
				((ISupportInitialize)border24).EndInit();
				context.PopParent();
				((ISupportInitialize)grid12).EndInit();
				Controls children10 = grid5.Children;
				Border border26;
				Border border27 = (border26 = new Border());
				((ISupportInitialize)border27).BeginInit();
				children10.Add(border27);
				Border border28 = (border3 = border26);
				context.PushParent(border3);
				Border border29 = border3;
				border29.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				border29.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border29.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				border29.SetValue(Border.BoxShadowProperty, BoxShadows.Parse("0 0 10 2 #BF000000"), BindingPriority.Template);
				border29.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				Panel panel3;
				Panel panel4 = (panel3 = new Panel());
				((ISupportInitialize)panel4).BeginInit();
				border29.Child = panel4;
				Panel panel5 = (panel = panel3);
				context.PushParent(panel);
				Panel panel6 = panel;
				Controls children11 = panel6.Children;
				Border border30;
				Border border31 = (border30 = new Border());
				((ISupportInitialize)border31).BeginInit();
				children11.Add(border31);
				Border border32 = (border3 = border30);
				context.PushParent(border3);
				Border border33 = border3;
				StaticResourceExtension staticResourceExtension13 = new StaticResourceExtension("ColorControlCheckeredBackgroundBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				object? obj13 = staticResourceExtension13.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_76.DynamicSetter_3(border33, BindingPriority.Template, obj13);
				border33.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)border32).EndInit();
				Controls children12 = panel6.Children;
				Border border34;
				Border border35 = (border34 = new Border());
				((ISupportInitialize)border35).BeginInit();
				children12.Add(border35);
				Border border36 = (border3 = border34);
				context.PushParent(border3);
				Border border37 = border3;
				border37.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				StyledProperty<IBrush?> backgroundProperty5 = Border.BackgroundProperty;
				TemplateBinding templateBinding14 = (templateBinding = new TemplateBinding(ColorPreviewer.HsvColorProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding15 = templateBinding;
				StaticResourceExtension staticResourceExtension14 = new StaticResourceExtension("ToBrushConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj14 = staticResourceExtension14.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding15.Converter = (IValueConverter)obj14;
				context.PopParent();
				border37.Bind(backgroundProperty5, templateBinding14.ProvideValue());
				border37.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border37.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)border36).EndInit();
				context.PopParent();
				((ISupportInitialize)panel5).EndInit();
				context.PopParent();
				((ISupportInitialize)border28).EndInit();
				context.PopParent();
				((ISupportInitialize)grid4).EndInit();
				Controls children13 = panel2.Children;
				Border border38;
				Border border39 = (border38 = new Border());
				((ISupportInitialize)border39).BeginInit();
				children13.Add(border39);
				Border border40 = (border3 = border38);
				context.PushParent(border3);
				Border border41 = border3;
				border41.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				border41.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorPreviewer.IsAccentColorsVisibleProperty)
				{
					Converter = BoolConverters.Not
				}.ProvideValue());
				border41.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border41.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				Panel panel7;
				Panel panel8 = (panel7 = new Panel());
				((ISupportInitialize)panel8).BeginInit();
				border41.Child = panel8;
				Panel panel9 = (panel = panel7);
				context.PushParent(panel);
				Panel panel10 = panel;
				Controls children14 = panel10.Children;
				Border border42;
				Border border43 = (border42 = new Border());
				((ISupportInitialize)border43).BeginInit();
				children14.Add(border43);
				Border border44 = (border3 = border42);
				context.PushParent(border3);
				Border border45 = border3;
				StaticResourceExtension staticResourceExtension15 = new StaticResourceExtension("ColorControlCheckeredBackgroundBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				object? obj15 = staticResourceExtension15.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_76.DynamicSetter_3(border45, BindingPriority.Template, obj15);
				border45.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)border44).EndInit();
				Controls children15 = panel10.Children;
				Border border46;
				Border border47 = (border46 = new Border());
				((ISupportInitialize)border47).BeginInit();
				children15.Add(border47);
				Border border48 = (border3 = border46);
				context.PushParent(border3);
				Border border49 = border3;
				border49.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				StyledProperty<IBrush?> backgroundProperty6 = Border.BackgroundProperty;
				TemplateBinding templateBinding16 = (templateBinding = new TemplateBinding(ColorPreviewer.HsvColorProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding17 = templateBinding;
				StaticResourceExtension staticResourceExtension16 = new StaticResourceExtension("ToBrushConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj16 = staticResourceExtension16.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding17.Converter = (IValueConverter)obj16;
				context.PopParent();
				border49.Bind(backgroundProperty6, templateBinding16.ProvideValue());
				border49.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border49.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)border48).EndInit();
				context.PopParent();
				((ISupportInitialize)panel9).EndInit();
				context.PopParent();
				((ISupportInitialize)border40).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
			controlTheme.TargetType = typeof(ColorPreviewer);
			Setter setter = new Setter();
			setter.Property = Layoutable.HeightProperty;
			setter.Value = 50.0;
			controlTheme.Add(setter);
			Setter setter2 = new Setter();
			setter2.Property = Visual.ClipToBoundsProperty;
			setter2.Value = false;
			controlTheme.Add(setter2);
			Setter setter3 = new Setter();
			setter3.Property = TemplatedControl.CornerRadiusProperty;
			setter3.Value = new CornerRadius(0.0, 0.0, 0.0, 0.0);
			controlTheme.Add(setter3);
			Setter setter4;
			Setter setter5 = (setter4 = new Setter());
			context.PushParent(setter4);
			setter4.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.TargetType = typeof(ColorPreviewer);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_75.Build, context);
			context.PopParent();
			setter4.Value = value;
			context.PopParent();
			controlTheme.Add(setter5);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_77
	{
		private class XamlClosure_78
		{
			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
				((AvaloniaObject)intermediateRoot).Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
			controlTheme.TargetType = typeof(Thumb);
			Setter setter = new Setter();
			setter.Property = TemplatedControl.BackgroundProperty;
			setter.Value = new ImmutableSolidColorBrush(16777215u);
			controlTheme.Add(setter);
			Setter setter2;
			Setter setter3 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter4 = setter2;
			setter4.Property = TemplatedControl.BorderBrushProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeForegroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter4.Value = value;
			context.PopParent();
			controlTheme.Add(setter3);
			Setter setter5 = new Setter();
			setter5.Property = TemplatedControl.BorderThicknessProperty;
			setter5.Value = new Thickness(3.0, 3.0, 3.0, 3.0);
			controlTheme.Add(setter5);
			Setter setter6 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter7 = setter2;
			setter7.Property = TemplatedControl.CornerRadiusProperty;
			DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ColorSliderCornerRadius");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value2 = dynamicResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter7.Value = value2;
			context.PopParent();
			controlTheme.Add(setter6);
			Setter setter8 = new Setter();
			setter8.Property = TemplatedControl.TemplateProperty;
			setter8.Value = new ControlTemplate
			{
				Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_78.Build, context)
			};
			controlTheme.Add(setter8);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_79
	{
		private class XamlClosure_80
		{
			private class DynamicSetters_81
			{
				public static void DynamicSetter_1(Border P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(Border.BackgroundProperty, binding);
						return;
					}
					if (P_2 is IBrush)
					{
						IBrush value = (IBrush)P_2;
						int priority = (int)P_1;
						P_0.SetValue(Border.BackgroundProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						IBrush value = (IBrush)P_2;
						int priority = (int)P_1;
						P_0.SetValue(Border.BackgroundProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
				}

				public static void DynamicSetter_2(StyledElement P_0, BindingPriority P_1, IBinding P_2)
				{
					if (P_2 != null)
					{
						IBinding binding = P_2;
						P_0.Bind(StyledElement.DataContextProperty, binding);
					}
					else
					{
						object value = P_2;
						int priority = (int)P_1;
						P_0.SetValue(StyledElement.DataContextProperty, value, (BindingPriority)priority);
					}
				}
			}

			private class XamlClosure_82
			{
				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
					((StyledElement)intermediateRoot).Name = "FocusTarget";
					service = intermediateRoot;
					context.AvaloniaNameScope.Register("FocusTarget", service);
					((AvaloniaObject)intermediateRoot).SetValue(Border.BackgroundProperty, (IBrush)new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
					((AvaloniaObject)intermediateRoot).SetValue(Layoutable.MarginProperty, new Thickness(0.0, -10.0, 0.0, -10.0), BindingPriority.Template);
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			private class XamlClosure_83
			{
				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
					((StyledElement)intermediateRoot).Name = "FocusTarget";
					service = intermediateRoot;
					context.AvaloniaNameScope.Register("FocusTarget", service);
					((AvaloniaObject)intermediateRoot).SetValue(Border.BackgroundProperty, (IBrush)new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
					((AvaloniaObject)intermediateRoot).SetValue(Layoutable.MarginProperty, new Thickness(0.0, -10.0, 0.0, -10.0), BindingPriority.Template);
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
				border2.Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				border2.Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				border2.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				Grid grid;
				Grid grid2 = (grid = new Grid());
				((ISupportInitialize)grid2).BeginInit();
				border2.Child = grid2;
				Grid grid3;
				Grid grid4 = (grid3 = grid);
				context.PushParent(grid3);
				grid3.Bind(Layoutable.MarginProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				Controls children = grid3.Children;
				Border border3;
				Border border4 = (border3 = new Border());
				((ISupportInitialize)border4).BeginInit();
				children.Add(border4);
				Border border5 = (border = border3);
				context.PushParent(border);
				Border border6 = border;
				border6.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border6.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StyledProperty<double> heightProperty = Layoutable.HeightProperty;
				CompiledBindingExtension obj = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "PART_Track").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding = obj.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border6.Bind(heightProperty, binding);
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ColorControlCheckeredBackgroundBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				object? obj2 = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_81.DynamicSetter_1(border6, BindingPriority.Template, obj2);
				StyledProperty<CornerRadius> cornerRadiusProperty = Border.CornerRadiusProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ColorSliderTrackCornerRadius");
				context.ProvideTargetProperty = Border.CornerRadiusProperty;
				IBinding binding2 = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border6.Bind(cornerRadiusProperty, binding2);
				context.PopParent();
				((ISupportInitialize)border5).EndInit();
				Controls children2 = grid3.Children;
				Border border7;
				Border border8 = (border7 = new Border());
				((ISupportInitialize)border8).BeginInit();
				children2.Add(border8);
				Border border9 = (border = border7);
				context.PushParent(border);
				Border border10 = border;
				border10.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border10.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StyledProperty<double> heightProperty2 = Layoutable.HeightProperty;
				CompiledBindingExtension obj3 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "PART_Track").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding3 = obj3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border10.Bind(heightProperty2, binding3);
				border10.Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				StyledProperty<CornerRadius> cornerRadiusProperty2 = Border.CornerRadiusProperty;
				DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ColorSliderTrackCornerRadius");
				context.ProvideTargetProperty = Border.CornerRadiusProperty;
				IBinding binding4 = dynamicResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border10.Bind(cornerRadiusProperty2, binding4);
				context.PopParent();
				((ISupportInitialize)border9).EndInit();
				Controls children3 = grid3.Children;
				Track track;
				Track track2 = (track = new Track());
				((ISupportInitialize)track2).BeginInit();
				children3.Add(track2);
				Track track3;
				Track track4 = (track3 = track);
				context.PushParent(track3);
				track3.Name = "PART_Track";
				service = track3;
				context.AvaloniaNameScope.Register("PART_Track", service);
				StyledProperty<double> heightProperty3 = Layoutable.HeightProperty;
				DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ColorSliderTrackSize");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				IBinding binding5 = dynamicResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				track3.Bind(heightProperty3, binding5);
				track3.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				track3.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				track3.Bind(Track.MinimumProperty, new TemplateBinding(RangeBase.MinimumProperty).ProvideValue());
				track3.Bind(Track.MaximumProperty, new TemplateBinding(RangeBase.MaximumProperty).ProvideValue());
				track3.Bind(Track.ValueProperty, new TemplateBinding(RangeBase.ValueProperty)
				{
					Mode = BindingMode.TwoWay
				}.ProvideValue());
				track3.Bind(Track.IsDirectionReversedProperty, new TemplateBinding(Slider.IsDirectionReversedProperty).ProvideValue());
				track3.SetValue(Track.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				StyledProperty<Button?> decreaseButtonProperty = Track.DecreaseButtonProperty;
				RepeatButton repeatButton;
				RepeatButton repeatButton2 = (repeatButton = new RepeatButton());
				((ISupportInitialize)repeatButton2).BeginInit();
				track3.SetValue(decreaseButtonProperty, repeatButton2, BindingPriority.Template);
				repeatButton.Name = "PART_DecreaseButton";
				service = repeatButton;
				context.AvaloniaNameScope.Register("PART_DecreaseButton", service);
				repeatButton.SetValue(TemplatedControl.BackgroundProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				repeatButton.SetValue(InputElement.FocusableProperty, value: false, BindingPriority.Template);
				repeatButton.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				repeatButton.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				repeatButton.SetValue(TemplatedControl.TemplateProperty, new ControlTemplate
				{
					Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_82.Build, context)
				}, BindingPriority.Template);
				((ISupportInitialize)repeatButton).EndInit();
				StyledProperty<Button?> increaseButtonProperty = Track.IncreaseButtonProperty;
				RepeatButton repeatButton3;
				RepeatButton repeatButton4 = (repeatButton3 = new RepeatButton());
				((ISupportInitialize)repeatButton4).BeginInit();
				track3.SetValue(increaseButtonProperty, repeatButton4, BindingPriority.Template);
				repeatButton3.Name = "PART_IncreaseButton";
				service = repeatButton3;
				context.AvaloniaNameScope.Register("PART_IncreaseButton", service);
				repeatButton3.SetValue(TemplatedControl.BackgroundProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				repeatButton3.SetValue(InputElement.FocusableProperty, value: false, BindingPriority.Template);
				repeatButton3.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				repeatButton3.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				repeatButton3.SetValue(TemplatedControl.TemplateProperty, new ControlTemplate
				{
					Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_83.Build, context)
				}, BindingPriority.Template);
				((ISupportInitialize)repeatButton3).EndInit();
				Thumb thumb;
				Thumb thumb2 = (thumb = new Thumb());
				((ISupportInitialize)thumb2).BeginInit();
				track3.Thumb = thumb2;
				Thumb thumb3;
				Thumb thumb4 = (thumb3 = thumb);
				context.PushParent(thumb3);
				thumb3.Name = "ColorSliderThumb";
				service = thumb3;
				context.AvaloniaNameScope.Register("ColorSliderThumb", service);
				StyledProperty<ControlTheme?> themeProperty = StyledElement.ThemeProperty;
				DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("ColorSliderThumbTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				IBinding binding6 = dynamicResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				thumb3.Bind(themeProperty, binding6);
				thumb3.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				thumb3.SetValue(TemplatedControl.PaddingProperty, new Thickness(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				DynamicSetters_81.DynamicSetter_2(thumb3, BindingPriority.Template, new TemplateBinding(RangeBase.ValueProperty).ProvideValue());
				thumb3.Bind(Layoutable.HeightProperty, new TemplateBinding(Layoutable.HeightProperty).ProvideValue());
				thumb3.Bind(Layoutable.WidthProperty, new TemplateBinding(Layoutable.HeightProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)thumb4).EndInit();
				context.PopParent();
				((ISupportInitialize)track4).EndInit();
				context.PopParent();
				((ISupportInitialize)grid4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		private class XamlClosure_84
		{
			private class DynamicSetters_85
			{
				public static void DynamicSetter_1(Border P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(Border.BackgroundProperty, binding);
						return;
					}
					if (P_2 is IBrush)
					{
						IBrush value = (IBrush)P_2;
						int priority = (int)P_1;
						P_0.SetValue(Border.BackgroundProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						IBrush value = (IBrush)P_2;
						int priority = (int)P_1;
						P_0.SetValue(Border.BackgroundProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
				}

				public static void DynamicSetter_2(StyledElement P_0, BindingPriority P_1, IBinding P_2)
				{
					if (P_2 != null)
					{
						IBinding binding = P_2;
						P_0.Bind(StyledElement.DataContextProperty, binding);
					}
					else
					{
						object value = P_2;
						int priority = (int)P_1;
						P_0.SetValue(StyledElement.DataContextProperty, value, (BindingPriority)priority);
					}
				}
			}

			private class XamlClosure_86
			{
				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
					((StyledElement)intermediateRoot).Name = "FocusTarget";
					service = intermediateRoot;
					context.AvaloniaNameScope.Register("FocusTarget", service);
					((AvaloniaObject)intermediateRoot).SetValue(Border.BackgroundProperty, (IBrush)new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
					((AvaloniaObject)intermediateRoot).SetValue(Layoutable.MarginProperty, new Thickness(0.0, -10.0, 0.0, -10.0), BindingPriority.Template);
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			private class XamlClosure_87
			{
				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
					((StyledElement)intermediateRoot).Name = "FocusTarget";
					service = intermediateRoot;
					context.AvaloniaNameScope.Register("FocusTarget", service);
					((AvaloniaObject)intermediateRoot).SetValue(Border.BackgroundProperty, (IBrush)new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
					((AvaloniaObject)intermediateRoot).SetValue(Layoutable.MarginProperty, new Thickness(0.0, -10.0, 0.0, -10.0), BindingPriority.Template);
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
				border2.Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				border2.Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				border2.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				Grid grid;
				Grid grid2 = (grid = new Grid());
				((ISupportInitialize)grid2).BeginInit();
				border2.Child = grid2;
				Grid grid3;
				Grid grid4 = (grid3 = grid);
				context.PushParent(grid3);
				grid3.Bind(Layoutable.MarginProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				Controls children = grid3.Children;
				Border border3;
				Border border4 = (border3 = new Border());
				((ISupportInitialize)border4).BeginInit();
				children.Add(border4);
				Border border5 = (border = border3);
				context.PushParent(border);
				Border border6 = border;
				border6.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				border6.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<double> widthProperty = Layoutable.WidthProperty;
				CompiledBindingExtension obj = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "PART_Track").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EWidth_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				CompiledBindingExtension binding = obj.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border6.Bind(widthProperty, binding);
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ColorControlCheckeredBackgroundBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				object? obj2 = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_85.DynamicSetter_1(border6, BindingPriority.Template, obj2);
				StyledProperty<CornerRadius> cornerRadiusProperty = Border.CornerRadiusProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ColorSliderTrackCornerRadius");
				context.ProvideTargetProperty = Border.CornerRadiusProperty;
				IBinding binding2 = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border6.Bind(cornerRadiusProperty, binding2);
				context.PopParent();
				((ISupportInitialize)border5).EndInit();
				Controls children2 = grid3.Children;
				Border border7;
				Border border8 = (border7 = new Border());
				((ISupportInitialize)border8).BeginInit();
				children2.Add(border8);
				Border border9 = (border = border7);
				context.PushParent(border);
				Border border10 = border;
				border10.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				border10.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<double> widthProperty2 = Layoutable.WidthProperty;
				CompiledBindingExtension obj3 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "PART_Track").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EWidth_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				CompiledBindingExtension binding3 = obj3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border10.Bind(widthProperty2, binding3);
				border10.Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				StyledProperty<CornerRadius> cornerRadiusProperty2 = Border.CornerRadiusProperty;
				DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ColorSliderTrackCornerRadius");
				context.ProvideTargetProperty = Border.CornerRadiusProperty;
				IBinding binding4 = dynamicResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border10.Bind(cornerRadiusProperty2, binding4);
				context.PopParent();
				((ISupportInitialize)border9).EndInit();
				Controls children3 = grid3.Children;
				Track track;
				Track track2 = (track = new Track());
				((ISupportInitialize)track2).BeginInit();
				children3.Add(track2);
				Track track3;
				Track track4 = (track3 = track);
				context.PushParent(track3);
				track3.Name = "PART_Track";
				service = track3;
				context.AvaloniaNameScope.Register("PART_Track", service);
				StyledProperty<double> widthProperty3 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ColorSliderTrackSize");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding5 = dynamicResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				track3.Bind(widthProperty3, binding5);
				track3.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				track3.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				track3.Bind(Track.MinimumProperty, new TemplateBinding(RangeBase.MinimumProperty).ProvideValue());
				track3.Bind(Track.MaximumProperty, new TemplateBinding(RangeBase.MaximumProperty).ProvideValue());
				track3.Bind(Track.ValueProperty, new TemplateBinding(RangeBase.ValueProperty)
				{
					Mode = BindingMode.TwoWay
				}.ProvideValue());
				track3.Bind(Track.IsDirectionReversedProperty, new TemplateBinding(Slider.IsDirectionReversedProperty).ProvideValue());
				track3.SetValue(Track.OrientationProperty, Orientation.Vertical, BindingPriority.Template);
				StyledProperty<Button?> decreaseButtonProperty = Track.DecreaseButtonProperty;
				RepeatButton repeatButton;
				RepeatButton repeatButton2 = (repeatButton = new RepeatButton());
				((ISupportInitialize)repeatButton2).BeginInit();
				track3.SetValue(decreaseButtonProperty, repeatButton2, BindingPriority.Template);
				repeatButton.Name = "PART_DecreaseButton";
				service = repeatButton;
				context.AvaloniaNameScope.Register("PART_DecreaseButton", service);
				repeatButton.SetValue(TemplatedControl.BackgroundProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				repeatButton.SetValue(InputElement.FocusableProperty, value: false, BindingPriority.Template);
				repeatButton.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				repeatButton.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				repeatButton.SetValue(TemplatedControl.TemplateProperty, new ControlTemplate
				{
					Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_86.Build, context)
				}, BindingPriority.Template);
				((ISupportInitialize)repeatButton).EndInit();
				StyledProperty<Button?> increaseButtonProperty = Track.IncreaseButtonProperty;
				RepeatButton repeatButton3;
				RepeatButton repeatButton4 = (repeatButton3 = new RepeatButton());
				((ISupportInitialize)repeatButton4).BeginInit();
				track3.SetValue(increaseButtonProperty, repeatButton4, BindingPriority.Template);
				repeatButton3.Name = "PART_IncreaseButton";
				service = repeatButton3;
				context.AvaloniaNameScope.Register("PART_IncreaseButton", service);
				repeatButton3.SetValue(TemplatedControl.BackgroundProperty, new ImmutableSolidColorBrush(16777215u), BindingPriority.Template);
				repeatButton3.SetValue(InputElement.FocusableProperty, value: false, BindingPriority.Template);
				repeatButton3.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				repeatButton3.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				repeatButton3.SetValue(TemplatedControl.TemplateProperty, new ControlTemplate
				{
					Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_87.Build, context)
				}, BindingPriority.Template);
				((ISupportInitialize)repeatButton3).EndInit();
				Thumb thumb;
				Thumb thumb2 = (thumb = new Thumb());
				((ISupportInitialize)thumb2).BeginInit();
				track3.Thumb = thumb2;
				Thumb thumb3;
				Thumb thumb4 = (thumb3 = thumb);
				context.PushParent(thumb3);
				thumb3.Name = "ColorSliderThumb";
				service = thumb3;
				context.AvaloniaNameScope.Register("ColorSliderThumb", service);
				StyledProperty<ControlTheme?> themeProperty = StyledElement.ThemeProperty;
				DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("ColorSliderThumbTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				IBinding binding6 = dynamicResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				thumb3.Bind(themeProperty, binding6);
				thumb3.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				thumb3.SetValue(TemplatedControl.PaddingProperty, new Thickness(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				DynamicSetters_85.DynamicSetter_2(thumb3, BindingPriority.Template, new TemplateBinding(RangeBase.ValueProperty).ProvideValue());
				thumb3.Bind(Layoutable.HeightProperty, new TemplateBinding(Layoutable.WidthProperty).ProvideValue());
				thumb3.Bind(Layoutable.WidthProperty, new TemplateBinding(Layoutable.WidthProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)thumb4).EndInit();
				context.PopParent();
				((ISupportInitialize)track4).EndInit();
				context.PopParent();
				((ISupportInitialize)grid4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
			controlTheme.TargetType = typeof(ColorSlider);
			Style style;
			Style style2 = (style = new Style());
			context.PushParent(style);
			Style style3 = style;
			style3.Selector = ((Selector?)null).Nesting().Class(":horizontal");
			Setter setter = new Setter();
			setter.Property = TemplatedControl.BorderThicknessProperty;
			setter.Value = new Thickness(0.0, 0.0, 0.0, 0.0);
			style3.Add(setter);
			Setter setter2;
			Setter setter3 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter4 = setter2;
			setter4.Property = TemplatedControl.CornerRadiusProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ColorSliderCornerRadius");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter4.Value = value;
			context.PopParent();
			style3.Add(setter3);
			Setter setter5 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter6 = setter2;
			setter6.Property = Layoutable.HeightProperty;
			DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ColorSliderSize");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value2 = dynamicResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter6.Value = value2;
			context.PopParent();
			style3.Add(setter5);
			Setter setter7 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter8 = setter2;
			setter8.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value3 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			ControlTemplate controlTemplate2 = controlTemplate;
			controlTemplate2.TargetType = typeof(ColorSlider);
			controlTemplate2.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_80.Build, context);
			context.PopParent();
			setter8.Value = value3;
			context.PopParent();
			style3.Add(setter7);
			context.PopParent();
			controlTheme.Add(style2);
			Style style4 = (style = new Style());
			context.PushParent(style);
			Style style5 = style;
			style5.Selector = ((Selector?)null).Nesting().Class(":vertical");
			Setter setter9 = new Setter();
			setter9.Property = TemplatedControl.BorderThicknessProperty;
			setter9.Value = new Thickness(0.0, 0.0, 0.0, 0.0);
			style5.Add(setter9);
			Setter setter10 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter11 = setter2;
			setter11.Property = TemplatedControl.CornerRadiusProperty;
			DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ColorSliderCornerRadius");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value4 = dynamicResourceExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter11.Value = value4;
			context.PopParent();
			style5.Add(setter10);
			Setter setter12 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter13 = setter2;
			setter13.Property = Layoutable.WidthProperty;
			DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("ColorSliderSize");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value5 = dynamicResourceExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter13.Value = value5;
			context.PopParent();
			style5.Add(setter12);
			Setter setter14 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter15 = setter2;
			setter15.Property = TemplatedControl.TemplateProperty;
			ControlTemplate value6 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			ControlTemplate controlTemplate3 = controlTemplate;
			controlTemplate3.TargetType = typeof(ColorSlider);
			controlTemplate3.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_84.Build, context);
			context.PopParent();
			setter15.Value = value6;
			context.PopParent();
			style5.Add(setter14);
			context.PopParent();
			controlTheme.Add(style4);
			Style style6 = new Style();
			style6.Selector = ((Selector?)null).Nesting().Class(":pointerover").Template()
				.OfType(typeof(Thumb))
				.Name("ColorSliderThumb");
			Setter setter16 = new Setter();
			setter16.Property = Visual.OpacityProperty;
			setter16.Value = 0.75;
			style6.Add(setter16);
			controlTheme.Add(style6);
			Style style7 = new Style();
			style7.Selector = ((Selector?)null).Nesting().Class(":pointerover").Class(":dark-selector")
				.Template()
				.OfType(typeof(Thumb))
				.Name("ColorSliderThumb");
			Setter setter17 = new Setter();
			setter17.Property = Visual.OpacityProperty;
			setter17.Value = 0.7;
			style7.Add(setter17);
			controlTheme.Add(style7);
			Style style8 = new Style();
			style8.Selector = ((Selector?)null).Nesting().Class(":pointerover").Class(":light-selector")
				.Template()
				.OfType(typeof(Thumb))
				.Name("ColorSliderThumb");
			Setter setter18 = new Setter();
			setter18.Property = Visual.OpacityProperty;
			setter18.Value = 0.8;
			style8.Add(setter18);
			controlTheme.Add(style8);
			Style style9 = (style = new Style());
			context.PushParent(style);
			Style style10 = style;
			style10.Selector = ((Selector?)null).Nesting().Class(":dark-selector").Template()
				.OfType(typeof(Thumb))
				.Name("ColorSliderThumb");
			Setter setter19 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter20 = setter2;
			setter20.Property = TemplatedControl.BorderBrushProperty;
			DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("ColorControlDarkSelectorBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value7 = dynamicResourceExtension5.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter20.Value = value7;
			context.PopParent();
			style10.Add(setter19);
			context.PopParent();
			controlTheme.Add(style9);
			Style style11 = (style = new Style());
			context.PushParent(style);
			Style style12 = style;
			style12.Selector = ((Selector?)null).Nesting().Class(":light-selector").Template()
				.OfType(typeof(Thumb))
				.Name("ColorSliderThumb");
			Setter setter21 = (setter2 = new Setter());
			context.PushParent(setter2);
			Setter setter22 = setter2;
			setter22.Property = TemplatedControl.BorderBrushProperty;
			DynamicResourceExtension dynamicResourceExtension6 = new DynamicResourceExtension("ColorControlLightSelectorBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value8 = dynamicResourceExtension6.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter22.Value = value8;
			context.PopParent();
			style12.Add(setter21);
			context.PopParent();
			controlTheme.Add(style11);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_88
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new ContrastBrushConverter();
		}
	}

	private class XamlClosure_89
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new ColorToDisplayNameConverter();
		}
	}

	private class XamlClosure_90
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new DoNothingForNullConverter();
		}
	}

	private class XamlClosure_91
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new NumberFormatInfo
			{
				NumberDecimalDigits = 0
			};
		}
	}

	private class XamlClosure_92
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return PathGeometry.Parse("\n    M3 2C3.27614 2 3.5 2.22386 3.5 2.5V5.5C3.5 5.77614 3.72386 6 4 6H16C16.2761 6 16.5 5.77614\n    16.5 5.5V2.5C16.5 2.22386 16.7239 2 17 2C17.2761 2 17.5 2.22386 17.5 2.5V5.5C17.5 6.32843\n    16.8284 7 16 7H15.809L12.2236 14.1708C12.0615 14.4951 11.7914 14.7431 11.4695\n    14.8802C11.4905 15.0808 11.5 15.2891 11.5 15.5C11.5 16.0818 11.4278 16.6623 11.2268\n    17.1165C11.019 17.5862 10.6266 18 10 18C9.37343 18 8.98105 17.5862 8.77323 17.1165C8.57222\n    16.6623 8.5 16.0818 8.5 15.5C8.5 15.2891 8.50952 15.0808 8.53051 14.8802C8.20863 14.7431\n    7.93851 14.4951 7.77639 14.1708L4.19098 7H4C3.17157 7 2.5 6.32843 2.5 5.5V2.5C2.5 2.22386\n    2.72386 2 3 2ZM9.11803 14H10.882C11.0714 14 11.2445 13.893 11.3292 13.7236L14.691\n    7H5.30902L8.67082 13.7236C8.75552 13.893 8.92865 14 9.11803 14ZM9.52346 15C9.50787 15.1549\n    9.5 15.3225 9.5 15.5C9.5 16.0228 9.56841 16.4423 9.6877 16.7119C9.8002 16.9661 9.90782 17\n    10 17C10.0922 17 10.1998 16.9661 10.3123 16.7119C10.4316 16.4423 10.5 16.0228 10.5\n    15.5C10.5 15.3225 10.4921 15.1549 10.4765 15H9.52346Z\n  ");
		}
	}

	private class XamlClosure_93
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return PathGeometry.Parse("\n    M9.75003 6.5C10.1642 6.5 10.5 6.16421 10.5 5.75C10.5 5.33579 10.1642 5 9.75003 5C9.33582\n    5 9.00003 5.33579 9.00003 5.75C9.00003 6.16421 9.33582 6.5 9.75003 6.5ZM12.75 7.5C13.1642\n    7.5 13.5 7.16421 13.5 6.75C13.5 6.33579 13.1642 6 12.75 6C12.3358 6 12 6.33579 12 6.75C12\n    7.16421 12.3358 7.5 12.75 7.5ZM15.25 9C15.25 9.41421 14.9142 9.75 14.5 9.75C14.0858 9.75\n    13.75 9.41421 13.75 9C13.75 8.58579 14.0858 8.25 14.5 8.25C14.9142 8.25 15.25 8.58579\n    15.25 9ZM14.5 12.75C14.9142 12.75 15.25 12.4142 15.25 12C15.25 11.5858 14.9142 11.25 14.5\n    11.25C14.0858 11.25 13.75 11.5858 13.75 12C13.75 12.4142 14.0858 12.75 14.5 12.75ZM13.25\n    14C13.25 14.4142 12.9142 14.75 12.5 14.75C12.0858 14.75 11.75 14.4142 11.75 14C11.75\n    13.5858 12.0858 13.25 12.5 13.25C12.9142 13.25 13.25 13.5858 13.25 14ZM13.6972\n    2.99169C10.9426 1.57663 8.1432 1.7124 5.77007 3.16636C4.55909 3.9083 3.25331 5.46925\n    2.51605 7.05899C2.14542 7.85816 1.89915 8.70492 1.90238 9.49318C1.90566 10.2941 2.16983\n    11.0587 2.84039 11.6053C3.45058 12.1026 3.98165 12.353 4.49574 12.3784C5.01375 12.404\n    5.41804 12.1942 5.73429 12.0076C5.80382 11.9666 5.86891 11.927 5.93113 11.8892C6.17332\n    11.7421 6.37205 11.6214 6.62049 11.5426C6.90191 11.4534 7.2582 11.4205 7.77579\n    11.5787C7.96661 11.637 8.08161 11.7235 8.16212 11.8229C8.24792 11.9289 8.31662 12.0774\n    8.36788 12.2886C8.41955 12.5016 8.44767 12.7527 8.46868 13.0491C8.47651 13.1594 8.48379\n    13.2855 8.49142 13.4176C8.50252 13.6098 8.51437 13.8149 8.52974 14.0037C8.58435 14.6744\n    8.69971 15.4401 9.10362 16.1357C9.51764 16.8488 10.2047 17.439 11.307 17.8158C12.9093\n    18.3636 14.3731 17.9191 15.5126 17.0169C16.6391 16.125 17.4691 14.7761 17.8842\n    13.4272C19.1991 9.15377 17.6728 5.03394 13.6972 2.99169ZM6.29249 4.01905C8.35686 2.75426\n    10.7844 2.61959 13.2403 3.88119C16.7473 5.68275 18.1135 9.28161 16.9284 13.1332C16.5624\n    14.3227 15.8338 15.4871 14.8919 16.2329C13.963 16.9684 12.8486 17.286 11.6305\n    16.8696C10.7269 16.5607 10.2467 16.1129 9.96842 15.6336C9.68001 15.1369 9.57799 14.5556\n    9.52644 13.9225C9.51101 13.733 9.50132 13.5621 9.49147 13.3884C9.48399 13.2564 9.47642\n    13.1229 9.46618 12.9783C9.44424 12.669 9.41175 12.3499 9.33968 12.0529C9.26719 11.7541\n    9.14902 11.4527 8.93935 11.1937C8.72439 10.9282 8.43532 10.7346 8.06801 10.6223C7.36648\n    10.408 6.80266 10.4359 6.31839 10.5893C5.94331 10.7082 5.62016 10.9061 5.37179\n    11.0582C5.31992 11.0899 5.2713 11.1197 5.22616 11.1463C4.94094 11.3146 4.75357 11.39\n    4.54514 11.3796C4.33279 11.3691 4.00262 11.2625 3.47218 10.8301C3.0866 10.5158 2.90473\n    10.0668 2.90237 9.48908C2.89995 8.89865 3.08843 8.20165 3.42324 7.47971C4.09686 6.0272\n    5.28471 4.63649 6.29249 4.01905Z\n  ");
		}
	}

	private class XamlClosure_94
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return PathGeometry.Parse("\n    M14.95 5C14.7184 3.85888 13.7095 3 12.5 3C11.2905 3 10.2816 3.85888 10.05 5H2.5C2.22386\n    5 2 5.22386 2 5.5C2 5.77614 2.22386 6 2.5 6H10.05C10.2816 7.14112 11.2905 8 12.5 8C13.7297\n    8 14.752 7.11217 14.961 5.94254C14.9575 5.96177 14.9539 5.98093 14.95 6H17.5C17.7761 6 18\n    5.77614 18 5.5C18 5.22386 17.7761 5 17.5 5H14.95ZM12.5 7C11.6716 7 11 6.32843 11 5.5C11\n    4.67157 11.6716 4 12.5 4C13.3284 4 14 4.67157 14 5.5C14 6.32843 13.3284 7 12.5 7ZM9.94999\n    14C9.71836 12.8589 8.70948 12 7.5 12C6.29052 12 5.28164 12.8589 5.05001 14H2.5C2.22386\n    14 2 14.2239 2 14.5C2 14.7761 2.22386 15 2.5 15H5.05001C5.28164 16.1411 6.29052 17 7.5\n    17C8.70948 17 9.71836 16.1411 9.94999 15H17.5C17.7761 15 18 14.7761 18 14.5C18 14.2239\n    17.7761 14 17.5 14H9.94999ZM7.5 16C6.67157 16 6 15.3284 6 14.5C6 13.6716 6.67157 13 7.5\n    13C8.32843 13 9 13.6716 9 14.5C9 15.3284 8.32843 16 7.5 16Z\n  ");
		}
	}

	private class XamlClosure_95
	{
		private class XamlClosure_96
		{
			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
				((StyledElement)intermediateRoot).Name = "border";
				service = intermediateRoot;
				context.AvaloniaNameScope.Register("border", service);
				((AvaloniaObject)intermediateRoot).Bind(Visual.ClipToBoundsProperty, new TemplateBinding(Visual.ClipToBoundsProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				ScrollViewer scrollViewer;
				ScrollViewer scrollViewer2 = (scrollViewer = new ScrollViewer());
				((ISupportInitialize)scrollViewer2).BeginInit();
				((Decorator)intermediateRoot).Child = scrollViewer2;
				scrollViewer.Name = "PART_ScrollViewer";
				service = scrollViewer;
				context.AvaloniaNameScope.Register("PART_ScrollViewer", service);
				scrollViewer.Bind(ScrollViewer.HorizontalScrollBarVisibilityProperty, new TemplateBinding(ScrollViewer.HorizontalScrollBarVisibilityProperty).ProvideValue());
				scrollViewer.Bind(ScrollViewer.VerticalScrollBarVisibilityProperty, new TemplateBinding(ScrollViewer.VerticalScrollBarVisibilityProperty).ProvideValue());
				scrollViewer.Bind(ScrollViewer.IsScrollChainingEnabledProperty, new TemplateBinding(ScrollViewer.IsScrollChainingEnabledProperty).ProvideValue());
				scrollViewer.Bind(ScrollViewer.AllowAutoHideProperty, new TemplateBinding(ScrollViewer.AllowAutoHideProperty).ProvideValue());
				ItemsPresenter itemsPresenter;
				ItemsPresenter itemsPresenter2 = (itemsPresenter = new ItemsPresenter());
				((ISupportInitialize)itemsPresenter2).BeginInit();
				scrollViewer.Content = itemsPresenter2;
				itemsPresenter.Name = "PART_ItemsPresenter";
				service = itemsPresenter;
				context.AvaloniaNameScope.Register("PART_ItemsPresenter", service);
				itemsPresenter.Bind(ItemsPresenter.ItemsPanelProperty, new TemplateBinding(ItemsControl.ItemsPanelProperty).ProvideValue());
				itemsPresenter.Bind(Layoutable.MarginProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				((ISupportInitialize)itemsPresenter).EndInit();
				((ISupportInitialize)scrollViewer).EndInit();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
			controlTheme.TargetType = typeof(ListBox);
			Setter setter = new Setter();
			setter.Property = ScrollViewer.HorizontalScrollBarVisibilityProperty;
			setter.Value = ScrollBarVisibility.Disabled;
			controlTheme.Add(setter);
			Setter setter2 = new Setter();
			setter2.Property = ScrollViewer.VerticalScrollBarVisibilityProperty;
			setter2.Value = ScrollBarVisibility.Auto;
			controlTheme.Add(setter2);
			Setter setter3 = new Setter();
			setter3.Property = ScrollViewer.IsScrollChainingEnabledProperty;
			setter3.Value = true;
			controlTheme.Add(setter3);
			Setter setter4 = new Setter();
			setter4.Property = TemplatedControl.TemplateProperty;
			setter4.Value = new ControlTemplate
			{
				Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_96.Build, context)
			};
			controlTheme.Add(setter4);
			return controlTheme;
		}
	}

	private class XamlClosure_97
	{
		private class XamlClosure_98
		{
			private class DynamicSetters_99
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
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
				((AvaloniaObject)intermediateRoot).SetValue(Layoutable.UseLayoutRoundingProperty, value: false, BindingPriority.Template);
				Controls children = ((Panel)intermediateRoot).Children;
				ContentPresenter contentPresenter;
				ContentPresenter contentPresenter2 = (contentPresenter = new ContentPresenter());
				((ISupportInitialize)contentPresenter2).BeginInit();
				children.Add(contentPresenter2);
				contentPresenter.Name = "PART_ContentPresenter";
				service = contentPresenter;
				context.AvaloniaNameScope.Register("PART_ContentPresenter", service);
				contentPresenter.Bind(ContentPresenter.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.ContentTemplateProperty, new TemplateBinding(ContentControl.ContentTemplateProperty).ProvideValue());
				DynamicSetters_99.DynamicSetter_1(contentPresenter, BindingPriority.Template, new TemplateBinding(ContentControl.ContentProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.PaddingProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.VerticalContentAlignmentProperty, new TemplateBinding(ContentControl.VerticalContentAlignmentProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.HorizontalContentAlignmentProperty, new TemplateBinding(ContentControl.HorizontalContentAlignmentProperty).ProvideValue());
				((ISupportInitialize)contentPresenter).EndInit();
				Controls children2 = ((Panel)intermediateRoot).Children;
				Rectangle rectangle;
				Rectangle rectangle2 = (rectangle = new Rectangle());
				((ISupportInitialize)rectangle2).BeginInit();
				children2.Add(rectangle2);
				rectangle.Name = "BorderRectangle";
				service = rectangle;
				context.AvaloniaNameScope.Register("BorderRectangle", service);
				rectangle.SetValue(InputElement.IsHitTestVisibleProperty, value: false, BindingPriority.Template);
				rectangle.SetValue(Shape.StrokeThicknessProperty, 3.0, BindingPriority.Template);
				rectangle.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				rectangle.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				((ISupportInitialize)rectangle).EndInit();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
			controlTheme.TargetType = typeof(ListBoxItem);
			Setter setter = new Setter();
			setter.Property = TemplatedControl.BackgroundProperty;
			setter.Value = new ImmutableSolidColorBrush(16777215u);
			controlTheme.Add(setter);
			Setter setter2 = new Setter();
			setter2.Property = TemplatedControl.PaddingProperty;
			setter2.Value = new Thickness(0.0, 0.0, 0.0, 0.0);
			controlTheme.Add(setter2);
			Setter setter3 = new Setter();
			setter3.Property = ContentControl.HorizontalContentAlignmentProperty;
			setter3.Value = HorizontalAlignment.Stretch;
			controlTheme.Add(setter3);
			Setter setter4 = new Setter();
			setter4.Property = ContentControl.VerticalContentAlignmentProperty;
			setter4.Value = VerticalAlignment.Stretch;
			controlTheme.Add(setter4);
			Setter setter5 = new Setter();
			setter5.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate = new ControlTemplate();
			controlTemplate.TargetType = typeof(ListBoxItem);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_98.Build, context);
			setter5.Value = controlTemplate;
			controlTheme.Add(setter5);
			Style style;
			Style style2 = (style = new Style());
			context.PushParent(style);
			Style style3 = style;
			style3.Selector = ((Selector?)null).Nesting().Template().OfType(typeof(Rectangle))
				.Name("BorderRectangle");
			Setter setter6;
			Setter setter7 = (setter6 = new Setter());
			context.PushParent(setter6);
			Setter setter8 = setter6;
			setter8.Property = Shape.StrokeProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemControlHighlightListAccentLowBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter8.Value = value;
			context.PopParent();
			style3.Add(setter7);
			Setter setter9 = new Setter();
			setter9.Property = Visual.OpacityProperty;
			setter9.Value = 0.0;
			style3.Add(setter9);
			context.PopParent();
			controlTheme.Add(style2);
			Style style4 = (style = new Style());
			context.PushParent(style);
			Style style5 = style;
			style5.Selector = ((Selector?)null).Nesting().Class(":pointerover").Template()
				.OfType(typeof(Rectangle))
				.Name("BorderRectangle");
			Setter setter10 = (setter6 = new Setter());
			context.PushParent(setter6);
			Setter setter11 = setter6;
			setter11.Property = Shape.StrokeProperty;
			CompiledBindingExtension compiledBindingExtension;
			CompiledBindingExtension compiledBindingExtension2 = (compiledBindingExtension = new CompiledBindingExtension());
			context.PushParent(compiledBindingExtension);
			CompiledBindingExtension compiledBindingExtension3 = compiledBindingExtension;
			StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ContrastBrushConverter");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
			object? obj = staticResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			compiledBindingExtension3.Converter = (IValueConverter)obj;
			context.PopParent();
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			CompiledBindingExtension value2 = compiledBindingExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter11.Value = value2;
			context.PopParent();
			style5.Add(setter10);
			Setter setter12 = new Setter();
			setter12.Property = Visual.OpacityProperty;
			setter12.Value = 0.5;
			style5.Add(setter12);
			context.PopParent();
			controlTheme.Add(style4);
			Style style6 = (style = new Style());
			context.PushParent(style);
			Style style7 = style;
			style7.Selector = ((Selector?)null).Nesting().Class(":selected").Template()
				.OfType(typeof(Rectangle))
				.Name("BorderRectangle");
			Setter setter13 = (setter6 = new Setter());
			context.PushParent(setter6);
			Setter setter14 = setter6;
			setter14.Property = Shape.StrokeProperty;
			CompiledBindingExtension compiledBindingExtension4 = (compiledBindingExtension = new CompiledBindingExtension());
			context.PushParent(compiledBindingExtension);
			CompiledBindingExtension compiledBindingExtension5 = compiledBindingExtension;
			StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("ContrastBrushConverter");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
			object? obj2 = staticResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			compiledBindingExtension5.Converter = (IValueConverter)obj2;
			context.PopParent();
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			CompiledBindingExtension value3 = compiledBindingExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter14.Value = value3;
			context.PopParent();
			style7.Add(setter13);
			Setter setter15 = new Setter();
			setter15.Property = Visual.OpacityProperty;
			setter15.Value = 1.0;
			style7.Add(setter15);
			context.PopParent();
			controlTheme.Add(style6);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_100
	{
		private class XamlClosure_101
		{
			private class DynamicSetters_102
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
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
				context.IntermediateRoot = new ContentPresenter();
				object intermediateRoot = context.IntermediateRoot;
				((ISupportInitialize)intermediateRoot).BeginInit();
				((StyledElement)intermediateRoot).Name = "PART_ContentPresenter";
				service = intermediateRoot;
				context.AvaloniaNameScope.Register("PART_ContentPresenter", service);
				((AvaloniaObject)intermediateRoot).Bind(ContentPresenter.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(ContentPresenter.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(ContentPresenter.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(ContentPresenter.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				DynamicSetters_102.DynamicSetter_1((ContentPresenter)intermediateRoot, BindingPriority.Template, new TemplateBinding(ContentControl.ContentProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(ContentPresenter.ContentTemplateProperty, new TemplateBinding(ContentControl.ContentTemplateProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(ContentPresenter.PaddingProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).SetValue(ContentPresenter.RecognizesAccessKeyProperty, value: true, BindingPriority.Template);
				((AvaloniaObject)intermediateRoot).Bind(ContentPresenter.HorizontalContentAlignmentProperty, new TemplateBinding(ContentControl.HorizontalContentAlignmentProperty).ProvideValue());
				((AvaloniaObject)intermediateRoot).Bind(ContentPresenter.VerticalContentAlignmentProperty, new TemplateBinding(ContentControl.VerticalContentAlignmentProperty).ProvideValue());
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
			controlTheme.TargetType = typeof(RadioButton);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = TemplatedControl.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeControlMidBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Setter setter4 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter5 = setter;
			setter5.Property = TemplatedControl.ForegroundProperty;
			DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ThemeForegroundBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value2 = dynamicResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter5.Value = value2;
			context.PopParent();
			controlTheme.Add(setter4);
			Setter setter6 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter7 = setter;
			setter7.Property = TemplatedControl.BorderBrushProperty;
			DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ThemeBorderLowBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value3 = dynamicResourceExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter7.Value = value3;
			context.PopParent();
			controlTheme.Add(setter6);
			Setter setter8 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter9 = setter;
			setter9.Property = TemplatedControl.BorderThicknessProperty;
			DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("ThemeBorderThickness");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value4 = dynamicResourceExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter9.Value = value4;
			context.PopParent();
			controlTheme.Add(setter8);
			Setter setter10 = new Setter();
			setter10.Property = TemplatedControl.CornerRadiusProperty;
			setter10.Value = new CornerRadius(0.0, 0.0, 0.0, 0.0);
			controlTheme.Add(setter10);
			Setter setter11 = new Setter();
			setter11.Property = TemplatedControl.PaddingProperty;
			setter11.Value = new Thickness(4.0, 4.0, 4.0, 4.0);
			controlTheme.Add(setter11);
			Setter setter12 = new Setter();
			setter12.Property = Layoutable.HorizontalAlignmentProperty;
			setter12.Value = HorizontalAlignment.Stretch;
			controlTheme.Add(setter12);
			Setter setter13 = new Setter();
			setter13.Property = Layoutable.VerticalAlignmentProperty;
			setter13.Value = VerticalAlignment.Center;
			controlTheme.Add(setter13);
			Setter setter14 = new Setter();
			setter14.Property = ContentControl.HorizontalContentAlignmentProperty;
			setter14.Value = HorizontalAlignment.Center;
			controlTheme.Add(setter14);
			Setter setter15 = new Setter();
			setter15.Property = ContentControl.VerticalContentAlignmentProperty;
			setter15.Value = VerticalAlignment.Center;
			controlTheme.Add(setter15);
			Setter setter16 = new Setter();
			setter16.Property = TemplatedControl.TemplateProperty;
			setter16.Value = new ControlTemplate
			{
				Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_101.Build, context)
			};
			controlTheme.Add(setter16);
			Style style;
			Style style2 = (style = new Style());
			context.PushParent(style);
			Style style3 = style;
			style3.Selector = ((Selector?)null).Nesting().Class(":checked").Template()
				.OfType(typeof(ContentPresenter))
				.Name("PART_ContentPresenter");
			Setter setter17 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter18 = setter;
			setter18.Property = ContentPresenter.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("ThemeControlHighBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value5 = dynamicResourceExtension5.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter18.Value = value5;
			context.PopParent();
			style3.Add(setter17);
			Setter setter19 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter20 = setter;
			setter20.Property = ContentPresenter.BorderBrushProperty;
			DynamicResourceExtension dynamicResourceExtension6 = new DynamicResourceExtension("ThemeBorderMidBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value6 = dynamicResourceExtension6.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter20.Value = value6;
			context.PopParent();
			style3.Add(setter19);
			context.PopParent();
			controlTheme.Add(style2);
			Style style4 = (style = new Style());
			context.PushParent(style);
			Style style5 = style;
			style5.Selector = ((Selector?)null).Nesting().Class(":pointerover").Template()
				.OfType(typeof(ContentPresenter))
				.Name("PART_ContentPresenter");
			Setter setter21 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter22 = setter;
			setter22.Property = ContentPresenter.BorderBrushProperty;
			DynamicResourceExtension dynamicResourceExtension7 = new DynamicResourceExtension("ThemeBorderMidBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value7 = dynamicResourceExtension7.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter22.Value = value7;
			context.PopParent();
			style5.Add(setter21);
			context.PopParent();
			controlTheme.Add(style4);
			Style style6 = (style = new Style());
			context.PushParent(style);
			Style style7 = style;
			style7.Selector = ((Selector?)null).Nesting().Class(":pressed").Template()
				.OfType(typeof(ContentPresenter))
				.Name("PART_ContentPresenter");
			Setter setter23 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter24 = setter;
			setter24.Property = ContentPresenter.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension8 = new DynamicResourceExtension("ThemeControlHighBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value8 = dynamicResourceExtension8.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter24.Value = value8;
			context.PopParent();
			style7.Add(setter23);
			context.PopParent();
			controlTheme.Add(style6);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_103
	{
		private class XamlClosure_104
		{
			private class DynamicSetters_105
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
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
				border2.Name = "PART_LayoutRoot";
				service = border2;
				context.AvaloniaNameScope.Register("PART_LayoutRoot", service);
				border2.Bind(Border.BackgroundProperty, new TemplateBinding(TemplatedControl.BackgroundProperty).ProvideValue());
				border2.Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty).ProvideValue());
				border2.Bind(Border.BorderThicknessProperty, new TemplateBinding(TemplatedControl.BorderThicknessProperty).ProvideValue());
				border2.Bind(Border.CornerRadiusProperty, new TemplateBinding(TemplatedControl.CornerRadiusProperty).ProvideValue());
				border2.Bind(Decorator.PaddingProperty, new TemplateBinding(TemplatedControl.PaddingProperty).ProvideValue());
				Panel panel;
				Panel panel2 = (panel = new Panel());
				((ISupportInitialize)panel2).BeginInit();
				border2.Child = panel2;
				Panel panel3;
				Panel panel4 = (panel3 = panel);
				context.PushParent(panel3);
				Controls children = panel3.Children;
				ContentPresenter contentPresenter;
				ContentPresenter contentPresenter2 = (contentPresenter = new ContentPresenter());
				((ISupportInitialize)contentPresenter2).BeginInit();
				children.Add(contentPresenter2);
				contentPresenter.Name = "PART_ContentPresenter";
				service = contentPresenter;
				context.AvaloniaNameScope.Register("PART_ContentPresenter", service);
				contentPresenter.Bind(Layoutable.HorizontalAlignmentProperty, new TemplateBinding(ContentControl.HorizontalContentAlignmentProperty).ProvideValue());
				contentPresenter.Bind(Layoutable.VerticalAlignmentProperty, new TemplateBinding(ContentControl.VerticalContentAlignmentProperty).ProvideValue());
				DynamicSetters_105.DynamicSetter_1(contentPresenter, BindingPriority.Template, new TemplateBinding(HeaderedContentControl.HeaderProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.ContentTemplateProperty, new TemplateBinding(HeaderedContentControl.HeaderTemplateProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.FontFamilyProperty, new TemplateBinding(TemplatedControl.FontFamilyProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.FontSizeProperty, new TemplateBinding(TemplatedControl.FontSizeProperty).ProvideValue());
				contentPresenter.Bind(ContentPresenter.FontWeightProperty, new TemplateBinding(TemplatedControl.FontWeightProperty).ProvideValue());
				((ISupportInitialize)contentPresenter).EndInit();
				Controls children2 = panel3.Children;
				Border border3;
				Border border4 = (border3 = new Border());
				((ISupportInitialize)border4).BeginInit();
				children2.Add(border4);
				Border border5 = (border = border3);
				context.PushParent(border);
				Border border6 = border;
				border6.Name = "PART_SelectedPipe";
				service = border6;
				context.AvaloniaNameScope.Register("PART_SelectedPipe", service);
				border6.SetValue(Layoutable.HeightProperty, 2.0, BindingPriority.Template);
				border6.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 0.0, 2.0), BindingPriority.Template);
				border6.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border6.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Bottom, BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeAccentColor");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border6.Bind(backgroundProperty, binding);
				StyledProperty<CornerRadius> cornerRadiusProperty = Border.CornerRadiusProperty;
				DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ControlCornerRadius");
				context.ProvideTargetProperty = Border.CornerRadiusProperty;
				IBinding binding2 = dynamicResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border6.Bind(cornerRadiusProperty, binding2);
				border6.SetValue(Visual.IsVisibleProperty, value: false, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)border5).EndInit();
				context.PopParent();
				((ISupportInitialize)panel4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
			controlTheme.TargetType = typeof(TabItem);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = TemplatedControl.ForegroundProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ThemeForegroundLowBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Setter setter4 = new Setter();
			setter4.Property = TemplatedControl.PaddingProperty;
			setter4.Value = new Thickness(6.0, 0.0, 6.0, 0.0);
			controlTheme.Add(setter4);
			Setter setter5 = new Setter();
			setter5.Property = Layoutable.MarginProperty;
			setter5.Value = new Thickness(0.0, 0.0, 0.0, 0.0);
			controlTheme.Add(setter5);
			Setter setter6 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter7 = setter;
			setter7.Property = Layoutable.MinHeightProperty;
			DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ColorViewTabStripHeight");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value2 = dynamicResourceExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter7.Value = value2;
			context.PopParent();
			controlTheme.Add(setter6);
			Setter setter8 = new Setter();
			setter8.Property = ContentControl.VerticalContentAlignmentProperty;
			setter8.Value = VerticalAlignment.Center;
			controlTheme.Add(setter8);
			Setter setter9 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter10 = setter;
			setter10.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value3 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_104.Build, context);
			context.PopParent();
			setter10.Value = value3;
			context.PopParent();
			controlTheme.Add(setter9);
			Style style;
			Style style2 = (style = new Style());
			context.PushParent(style);
			Style style3 = style;
			style3.Selector = ((Selector?)null).Nesting().Class(":selected");
			Setter setter11 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter12 = setter;
			setter12.Property = TemplatedControl.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ThemeAccentBrush4");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value4 = dynamicResourceExtension3.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter12.Value = value4;
			context.PopParent();
			style3.Add(setter11);
			context.PopParent();
			controlTheme.Add(style2);
			Style style4 = new Style();
			style4.Selector = ((Selector?)null).Nesting().Class(":selected").Template()
				.OfType(typeof(Border))
				.Name("PART_SelectedPipe");
			Setter setter13 = new Setter();
			setter13.Property = Visual.IsVisibleProperty;
			setter13.Value = true;
			style4.Add(setter13);
			controlTheme.Add(style4);
			Style style5 = (style = new Style());
			context.PushParent(style);
			Style style6 = style;
			style6.Selector = ((Selector?)null).Nesting().Class(":pointerover").Template()
				.OfType(typeof(Border))
				.Name("PART_LayoutRoot");
			Setter setter14 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter15 = setter;
			setter15.Property = Border.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("ThemeControlHighlightMidBrush");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value5 = dynamicResourceExtension4.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter15.Value = value5;
			context.PopParent();
			style6.Add(setter14);
			context.PopParent();
			controlTheme.Add(style5);
			Style style7 = (style = new Style());
			context.PushParent(style);
			Style style8 = style;
			style8.Selector = ((Selector?)null).Nesting().Class(":selected").Class(":pointerover")
				.Template()
				.OfType(typeof(Border))
				.Name("PART_LayoutRoot");
			Setter setter16 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter17 = setter;
			setter17.Property = Border.BackgroundProperty;
			DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("ThemeAccentBrush3");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value6 = dynamicResourceExtension5.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter17.Value = value6;
			context.PopParent();
			style8.Add(setter16);
			context.PopParent();
			controlTheme.Add(style7);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_106
	{
		private class XamlClosure_107
		{
			private class XamlClosure_108
			{
				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
					if (P_0 != null)
					{
						object service = P_0.GetService(typeof(IRootObjectProvider));
						if (service != null)
						{
							service = ((IRootObjectProvider)service).RootObject;
							context.RootObject = (Styles)service;
						}
					}
					context.IntermediateRoot = new UniformGrid();
					object intermediateRoot = context.IntermediateRoot;
					((ISupportInitialize)intermediateRoot).BeginInit();
					((AvaloniaObject)intermediateRoot).SetValue(UniformGrid.ColumnsProperty, 0, BindingPriority.Template);
					((AvaloniaObject)intermediateRoot).SetValue(UniformGrid.RowsProperty, 1, BindingPriority.Template);
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			private class DynamicSetters_109
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

				public static void DynamicSetter_2(ItemsControl P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(ItemsControl.ItemContainerThemeProperty, binding);
						return;
					}
					if (P_2 is ControlTheme)
					{
						ControlTheme value = (ControlTheme)P_2;
						int priority = (int)P_1;
						P_0.SetValue(ItemsControl.ItemContainerThemeProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						ControlTheme value = (ControlTheme)P_2;
						int priority = (int)P_1;
						P_0.SetValue(ItemsControl.ItemContainerThemeProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
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

				public static void DynamicSetter_4(NumericUpDown P_0, BindingPriority P_1, object P_2)
				{
					if (P_2 is IBinding)
					{
						IBinding binding = (IBinding)P_2;
						P_0.Bind(NumericUpDown.NumberFormatProperty, binding);
						return;
					}
					if (P_2 is NumberFormatInfo)
					{
						NumberFormatInfo value = (NumberFormatInfo)P_2;
						int priority = (int)P_1;
						P_0.SetValue(NumericUpDown.NumberFormatProperty, value, (BindingPriority)priority);
						return;
					}
					if (P_2 == null)
					{
						NumberFormatInfo value = (NumberFormatInfo)P_2;
						int priority = (int)P_1;
						P_0.SetValue(NumericUpDown.NumberFormatProperty, value, (BindingPriority)priority);
						return;
					}
					throw new InvalidCastException();
				}
			}

			private class XamlClosure_110
			{
				private class DynamicSetters_111
				{
					public static void DynamicSetter_1(ToolTip P_0, BindingPriority P_1, CompiledBindingExtension P_2)
					{
						if (P_2 != null)
						{
							IBinding binding = P_2;
							P_0.Bind(ToolTip.TipProperty, binding);
						}
						else
						{
							object value = P_2;
							int priority = (int)P_1;
							P_0.SetValue(ToolTip.TipProperty, value, (BindingPriority)priority);
						}
					}
				}

				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
					AttachedProperty<string?> nameProperty = AutomationProperties.NameProperty;
					CompiledBindingExtension compiledBindingExtension;
					CompiledBindingExtension compiledBindingExtension2 = (compiledBindingExtension = new CompiledBindingExtension());
					context.PushParent(compiledBindingExtension);
					CompiledBindingExtension compiledBindingExtension3 = compiledBindingExtension;
					StaticResourceExtension staticResourceExtension = new StaticResourceExtension("ColorToDisplayNameConverter");
					context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
					object? obj = staticResourceExtension.ProvideValue(context);
					context.ProvideTargetProperty = null;
					compiledBindingExtension3.Converter = (IValueConverter)obj;
					context.PopParent();
					context.ProvideTargetProperty = AutomationProperties.NameProperty;
					CompiledBindingExtension binding = compiledBindingExtension2.ProvideValue(context);
					context.ProvideTargetProperty = null;
					border.Bind(nameProperty, binding);
					CompiledBindingExtension compiledBindingExtension4 = (compiledBindingExtension = new CompiledBindingExtension());
					context.PushParent(compiledBindingExtension);
					CompiledBindingExtension compiledBindingExtension5 = compiledBindingExtension;
					StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("ColorToDisplayNameConverter");
					context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
					object? obj2 = staticResourceExtension2.ProvideValue(context);
					context.ProvideTargetProperty = null;
					compiledBindingExtension5.Converter = (IValueConverter)obj2;
					context.PopParent();
					context.ProvideTargetProperty = ToolTip.TipProperty;
					CompiledBindingExtension compiledBindingExtension6 = compiledBindingExtension4.ProvideValue(context);
					context.ProvideTargetProperty = null;
					DynamicSetters_111.DynamicSetter_1((ToolTip)(object)border, BindingPriority.Template, compiledBindingExtension6);
					border.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
					border.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
					StyledProperty<IBrush?> backgroundProperty = Border.BackgroundProperty;
					SolidColorBrush solidColorBrush;
					SolidColorBrush value = (solidColorBrush = new SolidColorBrush());
					context.PushParent(solidColorBrush);
					StyledProperty<Color> colorProperty = SolidColorBrush.ColorProperty;
					CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension();
					context.ProvideTargetProperty = SolidColorBrush.ColorProperty;
					CompiledBindingExtension binding2 = compiledBindingExtension7.ProvideValue(context);
					context.ProvideTargetProperty = null;
					solidColorBrush.Bind(colorProperty, binding2);
					context.PopParent();
					border.SetValue(backgroundProperty, value, BindingPriority.Template);
					context.PopParent();
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			private class XamlClosure_112
			{
				public static object Build(IServiceProvider P_0)
				{
					XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
					if (P_0 != null)
					{
						object service = P_0.GetService(typeof(IRootObjectProvider));
						if (service != null)
						{
							service = ((IRootObjectProvider)service).RootObject;
							context.RootObject = (Styles)service;
						}
					}
					context.IntermediateRoot = new UniformGrid();
					object intermediateRoot = context.IntermediateRoot;
					((ISupportInitialize)intermediateRoot).BeginInit();
					UniformGrid uniformGrid = (UniformGrid)intermediateRoot;
					context.PushParent(uniformGrid);
					StyledProperty<int> columnsProperty = UniformGrid.ColumnsProperty;
					CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Ancestor(typeof(ColorView), 0).Property(ColorView.PaletteColumnCountProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
					context.ProvideTargetProperty = UniformGrid.ColumnsProperty;
					CompiledBindingExtension binding = compiledBindingExtension.ProvideValue(context);
					context.ProvideTargetProperty = null;
					uniformGrid.Bind(columnsProperty, binding);
					context.PopParent();
					((ISupportInitialize)intermediateRoot).EndInit();
					return intermediateRoot;
				}
			}

			public static object Build(IServiceProvider P_0)
			{
				XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
				RowDefinitions rowDefinitions = new RowDefinitions();
				rowDefinitions.Capacity = 2;
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				rowDefinitions.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				grid2.RowDefinitions = rowDefinitions;
				Controls children = grid2.Children;
				Border border;
				Border border2 = (border = new Border());
				((ISupportInitialize)border2).BeginInit();
				children.Add(border2);
				Border border3;
				Border border4 = (border3 = border);
				context.PushParent(border3);
				Border border5 = border3;
				border5.Name = "TabBackgroundBorder";
				service = border5;
				context.AvaloniaNameScope.Register("TabBackgroundBorder", service);
				border5.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				border5.SetValue(Grid.RowSpanProperty, 2, BindingPriority.Template);
				border5.SetValue(Layoutable.HeightProperty, 48.0, BindingPriority.Template);
				border5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top, BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("SystemControlBackgroundBaseLowBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding = dynamicResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border5.Bind(backgroundProperty, binding);
				StyledProperty<IBrush?> borderBrushProperty = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension2 = new DynamicResourceExtension("ColorViewTabBorderBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding2 = dynamicResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border5.Bind(borderBrushProperty, binding2);
				StyledProperty<CornerRadius> cornerRadiusProperty = Border.CornerRadiusProperty;
				DynamicResourceExtension dynamicResourceExtension3 = new DynamicResourceExtension("ColorViewTabBackgroundCornerRadius");
				context.ProvideTargetProperty = Border.CornerRadiusProperty;
				IBinding binding3 = dynamicResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border5.Bind(cornerRadiusProperty, binding3);
				context.PopParent();
				((ISupportInitialize)border4).EndInit();
				Controls children2 = grid2.Children;
				Border border6;
				Border border7 = (border6 = new Border());
				((ISupportInitialize)border7).BeginInit();
				children2.Add(border7);
				Border border8 = (border3 = border6);
				context.PushParent(border3);
				Border border9 = border3;
				border9.Name = "ContentBackgroundBorder";
				service = border9;
				context.AvaloniaNameScope.Register("ContentBackgroundBorder", service);
				border9.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				border9.SetValue(Grid.RowSpanProperty, 2, BindingPriority.Template);
				border9.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 48.0, 0.0, 0.0), BindingPriority.Template);
				border9.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				border9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				StyledProperty<CornerRadius> cornerRadiusProperty2 = Border.CornerRadiusProperty;
				TemplateBinding templateBinding;
				TemplateBinding templateBinding2 = (templateBinding = new TemplateBinding(TemplatedControl.CornerRadiusProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding3 = templateBinding;
				StaticResourceExtension staticResourceExtension = new StaticResourceExtension("BottomCornerRadiusFilterConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj = staticResourceExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding3.Converter = (IValueConverter)obj;
				context.PopParent();
				border9.Bind(cornerRadiusProperty2, templateBinding2.ProvideValue());
				StyledProperty<IBrush?> backgroundProperty2 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension4 = new DynamicResourceExtension("ColorViewContentBackgroundBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding4 = dynamicResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border9.Bind(backgroundProperty2, binding4);
				StyledProperty<IBrush?> borderBrushProperty2 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension5 = new DynamicResourceExtension("ColorViewContentBorderBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding5 = dynamicResourceExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border9.Bind(borderBrushProperty2, binding5);
				border9.SetValue(Border.BorderThicknessProperty, new Thickness(0.0, 1.0, 0.0, 0.0), BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)border8).EndInit();
				Controls children3 = grid2.Children;
				TabControl tabControl;
				TabControl tabControl2 = (tabControl = new TabControl());
				((ISupportInitialize)tabControl2).BeginInit();
				children3.Add(tabControl2);
				TabControl tabControl3;
				TabControl tabControl4 = (tabControl3 = tabControl);
				context.PushParent(tabControl3);
				tabControl3.Name = "PART_TabControl";
				service = tabControl3;
				context.AvaloniaNameScope.Register("PART_TabControl", service);
				tabControl3.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				tabControl3.SetValue(Layoutable.HeightProperty, 338.0, BindingPriority.Template);
				tabControl3.SetValue(Layoutable.WidthProperty, 350.0, BindingPriority.Template);
				tabControl3.SetValue(TemplatedControl.PaddingProperty, new Thickness(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				DirectProperty<SelectingItemsControl, int> selectedIndexProperty = SelectingItemsControl.SelectedIndexProperty;
				CompiledBindingExtension obj2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.SelectedIndexProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = SelectingItemsControl.SelectedIndexProperty;
				CompiledBindingExtension binding6 = obj2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				tabControl3.Bind(selectedIndexProperty, binding6);
				tabControl3.SetValue(ItemsControl.ItemsPanelProperty, new ItemsPanelTemplate
				{
					Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_108.Build, context)
				}, BindingPriority.Template);
				ItemCollection items = tabControl3.Items;
				TabItem tabItem;
				TabItem tabItem2 = (tabItem = new TabItem());
				((ISupportInitialize)tabItem2).BeginInit();
				items.Add(tabItem2);
				TabItem tabItem3;
				TabItem tabItem4 = (tabItem3 = tabItem);
				context.PushParent(tabItem3);
				TabItem tabItem5 = tabItem3;
				StaticResourceExtension staticResourceExtension2 = new StaticResourceExtension("ColorViewTabItemTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj3 = staticResourceExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_109.DynamicSetter_1(tabItem5, BindingPriority.Template, obj3);
				tabItem5.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorSpectrumVisibleProperty).ProvideValue());
				StyledProperty<object?> headerProperty = HeaderedContentControl.HeaderProperty;
				Border border10;
				Border border11 = (border10 = new Border());
				((ISupportInitialize)border11).BeginInit();
				tabItem5.SetValue(headerProperty, border11, BindingPriority.Template);
				Border border12 = (border3 = border10);
				context.PushParent(border3);
				Border border13 = border3;
				StyledProperty<double> heightProperty = Layoutable.HeightProperty;
				DynamicResourceExtension dynamicResourceExtension6 = new DynamicResourceExtension("ColorViewTabStripHeight");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				IBinding binding7 = dynamicResourceExtension6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border13.Bind(heightProperty, binding7);
				PathIcon pathIcon;
				PathIcon pathIcon2 = (pathIcon = new PathIcon());
				((ISupportInitialize)pathIcon2).BeginInit();
				border13.Child = pathIcon2;
				PathIcon pathIcon3;
				PathIcon pathIcon4 = (pathIcon3 = pathIcon);
				context.PushParent(pathIcon3);
				PathIcon pathIcon5 = pathIcon3;
				pathIcon5.SetValue(Layoutable.WidthProperty, 20.0, BindingPriority.Template);
				pathIcon5.SetValue(Layoutable.HeightProperty, 20.0, BindingPriority.Template);
				StyledProperty<Geometry> dataProperty = PathIcon.DataProperty;
				DynamicResourceExtension dynamicResourceExtension7 = new DynamicResourceExtension("ColorViewSpectrumIconGeometry");
				context.ProvideTargetProperty = PathIcon.DataProperty;
				IBinding binding8 = dynamicResourceExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				pathIcon5.Bind(dataProperty, binding8);
				context.PopParent();
				((ISupportInitialize)pathIcon4).EndInit();
				context.PopParent();
				((ISupportInitialize)border12).EndInit();
				Grid grid3;
				Grid grid4 = (grid3 = new Grid());
				((ISupportInitialize)grid4).BeginInit();
				tabItem5.Content = grid4;
				Grid grid5 = (grid = grid3);
				context.PushParent(grid);
				Grid grid6 = grid;
				RowDefinitions rowDefinitions2 = new RowDefinitions();
				rowDefinitions2.Capacity = 1;
				rowDefinitions2.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid6.RowDefinitions = rowDefinitions2;
				grid6.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 12.0, 12.0, 12.0), BindingPriority.Template);
				ColumnDefinitions columnDefinitions = grid6.ColumnDefinitions;
				ColumnDefinition columnDefinition = new ColumnDefinition();
				columnDefinition.SetValue(ColumnDefinition.WidthProperty, new GridLength(0.0, GridUnitType.Auto), BindingPriority.Template);
				columnDefinition.SetValue(ColumnDefinition.MinWidthProperty, 32.0, BindingPriority.Template);
				columnDefinitions.Add(columnDefinition);
				ColumnDefinitions columnDefinitions2 = grid6.ColumnDefinitions;
				ColumnDefinition columnDefinition2 = new ColumnDefinition();
				columnDefinition2.SetValue(ColumnDefinition.WidthProperty, new GridLength(1.0, GridUnitType.Star), BindingPriority.Template);
				columnDefinitions2.Add(columnDefinition2);
				ColumnDefinitions columnDefinitions3 = grid6.ColumnDefinitions;
				ColumnDefinition columnDefinition3 = new ColumnDefinition();
				columnDefinition3.SetValue(ColumnDefinition.WidthProperty, new GridLength(0.0, GridUnitType.Auto), BindingPriority.Template);
				columnDefinition3.SetValue(ColumnDefinition.MinWidthProperty, 32.0, BindingPriority.Template);
				columnDefinitions3.Add(columnDefinition3);
				Controls children4 = grid6.Children;
				ColorSlider colorSlider;
				ColorSlider colorSlider2 = (colorSlider = new ColorSlider());
				((ISupportInitialize)colorSlider2).BeginInit();
				children4.Add(colorSlider2);
				ColorSlider colorSlider3;
				ColorSlider colorSlider4 = (colorSlider3 = colorSlider);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider5 = colorSlider3;
				colorSlider5.Name = "ColorSpectrumThirdComponentSlider";
				service = colorSlider5;
				context.AvaloniaNameScope.Register("ColorSpectrumThirdComponentSlider", service);
				colorSlider5.SetValue(AutomationProperties.NameProperty, "Third Component", BindingPriority.Template);
				colorSlider5.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				colorSlider5.SetValue(ColorSlider.IsAlphaVisibleProperty, value: false, BindingPriority.Template);
				colorSlider5.SetValue(ColorSlider.IsPerceptiveProperty, value: true, BindingPriority.Template);
				colorSlider5.SetValue(Slider.OrientationProperty, Orientation.Vertical, BindingPriority.Template);
				colorSlider5.SetValue(ColorSlider.ColorModelProperty, ColorModel.Hsva, BindingPriority.Template);
				StyledProperty<ColorComponent> colorComponentProperty = ColorSlider.ColorComponentProperty;
				CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "ColorSpectrum").Property(ColorSpectrum.ThirdComponentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = ColorSlider.ColorComponentProperty;
				CompiledBindingExtension binding9 = compiledBindingExtension.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider5.Bind(colorComponentProperty, binding9);
				StyledProperty<HsvColor> hsvColorProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension compiledBindingExtension2 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "ColorSpectrum").Property(ColorSpectrum.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding10 = compiledBindingExtension2.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider5.Bind(hsvColorProperty, binding10);
				colorSlider5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				colorSlider5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				colorSlider5.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				colorSlider5.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorSpectrumSliderVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorSlider4).EndInit();
				Controls children5 = grid6.Children;
				ColorSpectrum colorSpectrum;
				ColorSpectrum colorSpectrum2 = (colorSpectrum = new ColorSpectrum());
				((ISupportInitialize)colorSpectrum2).BeginInit();
				children5.Add(colorSpectrum2);
				ColorSpectrum colorSpectrum3;
				ColorSpectrum colorSpectrum4 = (colorSpectrum3 = colorSpectrum);
				context.PushParent(colorSpectrum3);
				colorSpectrum3.Name = "ColorSpectrum";
				service = colorSpectrum3;
				context.AvaloniaNameScope.Register("ColorSpectrum", service);
				colorSpectrum3.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				colorSpectrum3.Bind(ColorSpectrum.ComponentsProperty, new TemplateBinding(ColorView.ColorSpectrumComponentsProperty).ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty2 = ColorSpectrum.HsvColorProperty;
				CompiledBindingExtension obj4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSpectrum.HsvColorProperty;
				CompiledBindingExtension binding11 = obj4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSpectrum3.Bind(hsvColorProperty2, binding11);
				colorSpectrum3.Bind(ColorSpectrum.MinHueProperty, new TemplateBinding(ColorView.MinHueProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MaxHueProperty, new TemplateBinding(ColorView.MaxHueProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MinSaturationProperty, new TemplateBinding(ColorView.MinSaturationProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MaxSaturationProperty, new TemplateBinding(ColorView.MaxSaturationProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MinValueProperty, new TemplateBinding(ColorView.MinValueProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.MaxValueProperty, new TemplateBinding(ColorView.MaxValueProperty).ProvideValue());
				colorSpectrum3.Bind(ColorSpectrum.ShapeProperty, new TemplateBinding(ColorView.ColorSpectrumShapeProperty).ProvideValue());
				colorSpectrum3.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSpectrum3.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)colorSpectrum4).EndInit();
				Controls children6 = grid6.Children;
				ColorSlider colorSlider6;
				ColorSlider colorSlider7 = (colorSlider6 = new ColorSlider());
				((ISupportInitialize)colorSlider7).BeginInit();
				children6.Add(colorSlider7);
				ColorSlider colorSlider8 = (colorSlider3 = colorSlider6);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider9 = colorSlider3;
				colorSlider9.Name = "ColorSpectrumAlphaSlider";
				service = colorSlider9;
				context.AvaloniaNameScope.Register("ColorSpectrumAlphaSlider", service);
				colorSlider9.SetValue(AutomationProperties.NameProperty, "Alpha Component", BindingPriority.Template);
				colorSlider9.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider9.SetValue(Slider.OrientationProperty, Orientation.Vertical, BindingPriority.Template);
				colorSlider9.SetValue(ColorSlider.ColorModelProperty, ColorModel.Hsva, BindingPriority.Template);
				colorSlider9.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Alpha, BindingPriority.Template);
				StyledProperty<HsvColor> hsvColorProperty3 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension compiledBindingExtension3 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "ColorSpectrum").Property(ColorSpectrum.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding12 = compiledBindingExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider9.Bind(hsvColorProperty3, binding12);
				colorSlider9.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				colorSlider9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch, BindingPriority.Template);
				colorSlider9.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				colorSlider9.Bind(InputElement.IsEnabledProperty, new TemplateBinding(ColorView.IsAlphaEnabledProperty).ProvideValue());
				StyledProperty<bool> isVisibleProperty = Visual.IsVisibleProperty;
				MultiBinding multiBinding;
				MultiBinding binding13 = (multiBinding = new MultiBinding());
				context.PushParent(multiBinding);
				MultiBinding multiBinding2 = multiBinding;
				multiBinding2.Converter = BoolConverters.And;
				IList<IBinding> bindings = multiBinding2.Bindings;
				CompiledBindingExtension obj5 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsAlphaVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item = obj5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings.Add(item);
				context.PopParent();
				colorSlider9.Bind(isVisibleProperty, binding13);
				context.PopParent();
				((ISupportInitialize)colorSlider8).EndInit();
				context.PopParent();
				((ISupportInitialize)grid5).EndInit();
				context.PopParent();
				((ISupportInitialize)tabItem4).EndInit();
				ItemCollection items2 = tabControl3.Items;
				TabItem tabItem6;
				TabItem tabItem7 = (tabItem6 = new TabItem());
				((ISupportInitialize)tabItem7).BeginInit();
				items2.Add(tabItem7);
				TabItem tabItem8 = (tabItem3 = tabItem6);
				context.PushParent(tabItem3);
				TabItem tabItem9 = tabItem3;
				StaticResourceExtension staticResourceExtension3 = new StaticResourceExtension("ColorViewTabItemTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj6 = staticResourceExtension3.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_109.DynamicSetter_1(tabItem9, BindingPriority.Template, obj6);
				tabItem9.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorPaletteVisibleProperty).ProvideValue());
				StyledProperty<object?> headerProperty2 = HeaderedContentControl.HeaderProperty;
				Border border14;
				Border border15 = (border14 = new Border());
				((ISupportInitialize)border15).BeginInit();
				tabItem9.SetValue(headerProperty2, border15, BindingPriority.Template);
				Border border16 = (border3 = border14);
				context.PushParent(border3);
				Border border17 = border3;
				StyledProperty<double> heightProperty2 = Layoutable.HeightProperty;
				DynamicResourceExtension dynamicResourceExtension8 = new DynamicResourceExtension("ColorViewTabStripHeight");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				IBinding binding14 = dynamicResourceExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border17.Bind(heightProperty2, binding14);
				PathIcon pathIcon6;
				PathIcon pathIcon7 = (pathIcon6 = new PathIcon());
				((ISupportInitialize)pathIcon7).BeginInit();
				border17.Child = pathIcon7;
				PathIcon pathIcon8 = (pathIcon3 = pathIcon6);
				context.PushParent(pathIcon3);
				PathIcon pathIcon9 = pathIcon3;
				pathIcon9.SetValue(Layoutable.WidthProperty, 20.0, BindingPriority.Template);
				pathIcon9.SetValue(Layoutable.HeightProperty, 20.0, BindingPriority.Template);
				StyledProperty<Geometry> dataProperty2 = PathIcon.DataProperty;
				DynamicResourceExtension dynamicResourceExtension9 = new DynamicResourceExtension("ColorViewPaletteIconGeometry");
				context.ProvideTargetProperty = PathIcon.DataProperty;
				IBinding binding15 = dynamicResourceExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				pathIcon9.Bind(dataProperty2, binding15);
				context.PopParent();
				((ISupportInitialize)pathIcon8).EndInit();
				context.PopParent();
				((ISupportInitialize)border16).EndInit();
				ListBox listBox;
				ListBox listBox2 = (listBox = new ListBox());
				((ISupportInitialize)listBox2).BeginInit();
				tabItem9.Content = listBox2;
				ListBox listBox3;
				ListBox listBox4 = (listBox3 = listBox);
				context.PushParent(listBox3);
				StaticResourceExtension staticResourceExtension4 = new StaticResourceExtension("ColorViewPaletteListBoxTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj7 = staticResourceExtension4.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_109.DynamicSetter_1(listBox3, BindingPriority.Template, obj7);
				StaticResourceExtension staticResourceExtension5 = new StaticResourceExtension("ColorViewPaletteListBoxItemTheme");
				context.ProvideTargetProperty = ItemsControl.ItemContainerThemeProperty;
				object? obj8 = staticResourceExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_109.DynamicSetter_2(listBox3, BindingPriority.Template, obj8);
				listBox3.Bind(ItemsControl.ItemsSourceProperty, new TemplateBinding(ColorView.PaletteColorsProperty).ProvideValue());
				CompiledBindingExtension compiledBindingExtension4;
				CompiledBindingExtension compiledBindingExtension5 = (compiledBindingExtension4 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.ColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build()));
				context.PushParent(compiledBindingExtension4);
				StaticResourceExtension staticResourceExtension6 = new StaticResourceExtension("DoNothingForNullConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EBindingBase_002CAvalonia_002EMarkup_002EConverter_0021Property();
				object? obj9 = staticResourceExtension6.ProvideValue(context);
				context.ProvideTargetProperty = null;
				compiledBindingExtension4.Converter = (IValueConverter)obj9;
				compiledBindingExtension4.Mode = BindingMode.TwoWay;
				context.PopParent();
				context.ProvideTargetProperty = SelectingItemsControl.SelectedItemProperty;
				CompiledBindingExtension compiledBindingExtension6 = compiledBindingExtension5.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_109.DynamicSetter_3(listBox3, compiledBindingExtension6);
				listBox3.SetValue(Layoutable.UseLayoutRoundingProperty, value: false, BindingPriority.Template);
				listBox3.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 12.0, 12.0, 12.0), BindingPriority.Template);
				StyledProperty<IDataTemplate?> itemTemplateProperty = ItemsControl.ItemTemplateProperty;
				DataTemplate dataTemplate;
				DataTemplate value = (dataTemplate = new DataTemplate());
				context.PushParent(dataTemplate);
				dataTemplate.DataType = typeof(Color);
				dataTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_110.Build, context);
				context.PopParent();
				listBox3.SetValue(itemTemplateProperty, value, BindingPriority.Template);
				StyledProperty<ITemplate<Panel?>> itemsPanelProperty = ItemsControl.ItemsPanelProperty;
				ItemsPanelTemplate itemsPanelTemplate;
				ItemsPanelTemplate value2 = (itemsPanelTemplate = new ItemsPanelTemplate());
				context.PushParent(itemsPanelTemplate);
				itemsPanelTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_112.Build, context);
				context.PopParent();
				listBox3.SetValue(itemsPanelProperty, value2, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)listBox4).EndInit();
				context.PopParent();
				((ISupportInitialize)tabItem8).EndInit();
				ItemCollection items3 = tabControl3.Items;
				TabItem tabItem10;
				TabItem tabItem11 = (tabItem10 = new TabItem());
				((ISupportInitialize)tabItem11).BeginInit();
				items3.Add(tabItem11);
				TabItem tabItem12 = (tabItem3 = tabItem10);
				context.PushParent(tabItem3);
				TabItem tabItem13 = tabItem3;
				StaticResourceExtension staticResourceExtension7 = new StaticResourceExtension("ColorViewTabItemTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj10 = staticResourceExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_109.DynamicSetter_1(tabItem13, BindingPriority.Template, obj10);
				tabItem13.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorComponentsVisibleProperty).ProvideValue());
				StyledProperty<object?> headerProperty3 = HeaderedContentControl.HeaderProperty;
				Border border18;
				Border border19 = (border18 = new Border());
				((ISupportInitialize)border19).BeginInit();
				tabItem13.SetValue(headerProperty3, border19, BindingPriority.Template);
				Border border20 = (border3 = border18);
				context.PushParent(border3);
				Border border21 = border3;
				StyledProperty<double> heightProperty3 = Layoutable.HeightProperty;
				DynamicResourceExtension dynamicResourceExtension10 = new DynamicResourceExtension("ColorViewTabStripHeight");
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				IBinding binding16 = dynamicResourceExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border21.Bind(heightProperty3, binding16);
				PathIcon pathIcon10;
				PathIcon pathIcon11 = (pathIcon10 = new PathIcon());
				((ISupportInitialize)pathIcon11).BeginInit();
				border21.Child = pathIcon11;
				PathIcon pathIcon12 = (pathIcon3 = pathIcon10);
				context.PushParent(pathIcon3);
				PathIcon pathIcon13 = pathIcon3;
				pathIcon13.SetValue(Layoutable.WidthProperty, 20.0, BindingPriority.Template);
				pathIcon13.SetValue(Layoutable.HeightProperty, 20.0, BindingPriority.Template);
				StyledProperty<Geometry> dataProperty3 = PathIcon.DataProperty;
				DynamicResourceExtension dynamicResourceExtension11 = new DynamicResourceExtension("ColorViewComponentsIconGeometry");
				context.ProvideTargetProperty = PathIcon.DataProperty;
				IBinding binding17 = dynamicResourceExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				pathIcon13.Bind(dataProperty3, binding17);
				context.PopParent();
				((ISupportInitialize)pathIcon12).EndInit();
				context.PopParent();
				((ISupportInitialize)border20).EndInit();
				Grid grid7;
				Grid grid8 = (grid7 = new Grid());
				((ISupportInitialize)grid8).BeginInit();
				tabItem13.Content = grid8;
				Grid grid9 = (grid = grid7);
				context.PushParent(grid);
				Grid grid10 = grid;
				ColumnDefinitions columnDefinitions4 = new ColumnDefinitions();
				columnDefinitions4.Capacity = 3;
				columnDefinitions4.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions4.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
				columnDefinitions4.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid10.ColumnDefinitions = columnDefinitions4;
				RowDefinitions rowDefinitions3 = new RowDefinitions();
				rowDefinitions3.Capacity = 7;
				rowDefinitions3.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(24.0, GridUnitType.Pixel)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(1.0, GridUnitType.Star)));
				rowDefinitions3.Add(new RowDefinition(new GridLength(12.0, GridUnitType.Pixel)));
				grid10.RowDefinitions = rowDefinitions3;
				grid10.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 12.0, 12.0, 12.0), BindingPriority.Template);
				Controls children7 = grid10.Children;
				Grid grid11;
				Grid grid12 = (grid11 = new Grid());
				((ISupportInitialize)grid12).BeginInit();
				children7.Add(grid12);
				Grid grid13 = (grid = grid11);
				context.PushParent(grid);
				Grid grid14 = grid;
				grid14.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				grid14.SetValue(Grid.ColumnSpanProperty, 3, BindingPriority.Template);
				grid14.SetValue(Grid.RowProperty, 0, BindingPriority.Template);
				ColumnDefinitions columnDefinitions5 = new ColumnDefinitions();
				columnDefinitions5.Capacity = 3;
				columnDefinitions5.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions5.Add(new ColumnDefinition(new GridLength(12.0, GridUnitType.Pixel)));
				columnDefinitions5.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid14.ColumnDefinitions = columnDefinitions5;
				Controls children8 = grid14.Children;
				Grid grid15;
				Grid grid16 = (grid15 = new Grid());
				((ISupportInitialize)grid16).BeginInit();
				children8.Add(grid16);
				Grid grid17 = (grid = grid15);
				context.PushParent(grid);
				Grid grid18 = grid;
				ColumnDefinitions columnDefinitions6 = new ColumnDefinitions();
				columnDefinitions6.Capacity = 2;
				columnDefinitions6.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				columnDefinitions6.Add(new ColumnDefinition(new GridLength(1.0, GridUnitType.Star)));
				grid18.ColumnDefinitions = columnDefinitions6;
				grid18.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorModelVisibleProperty).ProvideValue());
				Controls children9 = grid18.Children;
				RadioButton radioButton;
				RadioButton radioButton2 = (radioButton = new RadioButton());
				((ISupportInitialize)radioButton2).BeginInit();
				children9.Add(radioButton2);
				RadioButton radioButton3;
				RadioButton radioButton4 = (radioButton3 = radioButton);
				context.PushParent(radioButton3);
				RadioButton radioButton5 = radioButton3;
				radioButton5.Name = "RgbRadioButton";
				service = radioButton5;
				context.AvaloniaNameScope.Register("RgbRadioButton", service);
				StaticResourceExtension staticResourceExtension8 = new StaticResourceExtension("ColorViewColorModelRadioButtonTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj11 = staticResourceExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_109.DynamicSetter_1(radioButton5, BindingPriority.Template, obj11);
				radioButton5.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				radioButton5.SetValue(ContentControl.ContentProperty, "RGB", BindingPriority.Template);
				radioButton5.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				radioButton5.SetValue(TemplatedControl.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				StyledProperty<double> heightProperty4 = Layoutable.HeightProperty;
				CompiledBindingExtension obj12 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "PART_HexTextBox").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding18 = obj12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				radioButton5.Bind(heightProperty4, binding18);
				StyledProperty<bool?> isCheckedProperty = ToggleButton.IsCheckedProperty;
				TemplateBinding templateBinding4 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding5 = templateBinding;
				StaticResourceExtension staticResourceExtension9 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj13 = staticResourceExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding5.Converter = (IValueConverter)obj13;
				templateBinding5.ConverterParameter = ColorModel.Rgba;
				templateBinding5.Mode = BindingMode.TwoWay;
				context.PopParent();
				radioButton5.Bind(isCheckedProperty, templateBinding4.ProvideValue());
				context.PopParent();
				((ISupportInitialize)radioButton4).EndInit();
				Controls children10 = grid18.Children;
				RadioButton radioButton6;
				RadioButton radioButton7 = (radioButton6 = new RadioButton());
				((ISupportInitialize)radioButton7).BeginInit();
				children10.Add(radioButton7);
				RadioButton radioButton8 = (radioButton3 = radioButton6);
				context.PushParent(radioButton3);
				RadioButton radioButton9 = radioButton3;
				radioButton9.Name = "HsvRadioButton";
				service = radioButton9;
				context.AvaloniaNameScope.Register("HsvRadioButton", service);
				StaticResourceExtension staticResourceExtension10 = new StaticResourceExtension("ColorViewColorModelRadioButtonTheme");
				context.ProvideTargetProperty = StyledElement.ThemeProperty;
				object? obj14 = staticResourceExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_109.DynamicSetter_1(radioButton9, BindingPriority.Template, obj14);
				radioButton9.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				radioButton9.SetValue(ContentControl.ContentProperty, "HSV", BindingPriority.Template);
				radioButton9.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				radioButton9.SetValue(TemplatedControl.BorderThicknessProperty, new Thickness(0.0, 1.0, 1.0, 1.0), BindingPriority.Template);
				StyledProperty<double> heightProperty5 = Layoutable.HeightProperty;
				CompiledBindingExtension obj15 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "PART_HexTextBox").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding19 = obj15.ProvideValue(context);
				context.ProvideTargetProperty = null;
				radioButton9.Bind(heightProperty5, binding19);
				StyledProperty<bool?> isCheckedProperty2 = ToggleButton.IsCheckedProperty;
				TemplateBinding templateBinding6 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding7 = templateBinding;
				StaticResourceExtension staticResourceExtension11 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj16 = staticResourceExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding7.Converter = (IValueConverter)obj16;
				templateBinding7.ConverterParameter = ColorModel.Hsva;
				templateBinding7.Mode = BindingMode.TwoWay;
				context.PopParent();
				radioButton9.Bind(isCheckedProperty2, templateBinding6.ProvideValue());
				context.PopParent();
				((ISupportInitialize)radioButton8).EndInit();
				context.PopParent();
				((ISupportInitialize)grid17).EndInit();
				Controls children11 = grid14.Children;
				Grid grid19;
				Grid grid20 = (grid19 = new Grid());
				((ISupportInitialize)grid20).BeginInit();
				children11.Add(grid20);
				Grid grid21 = (grid = grid19);
				context.PushParent(grid);
				Grid grid22 = grid;
				grid22.Name = "HexInputGrid";
				service = grid22;
				context.AvaloniaNameScope.Register("HexInputGrid", service);
				grid22.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				grid22.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsHexInputVisibleProperty).ProvideValue());
				ColumnDefinitions columnDefinitions7 = grid22.ColumnDefinitions;
				ColumnDefinition columnDefinition4 = new ColumnDefinition();
				columnDefinition4.SetValue(ColumnDefinition.WidthProperty, new GridLength(30.0, GridUnitType.Pixel), BindingPriority.Template);
				columnDefinitions7.Add(columnDefinition4);
				ColumnDefinitions columnDefinitions8 = grid22.ColumnDefinitions;
				ColumnDefinition columnDefinition5 = new ColumnDefinition();
				columnDefinition5.SetValue(ColumnDefinition.WidthProperty, new GridLength(1.0, GridUnitType.Star), BindingPriority.Template);
				columnDefinitions8.Add(columnDefinition5);
				Controls children12 = grid22.Children;
				Border border22;
				Border border23 = (border22 = new Border());
				((ISupportInitialize)border23).BeginInit();
				children12.Add(border23);
				Border border24 = (border3 = border22);
				context.PushParent(border3);
				Border border25 = border3;
				border25.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border25.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<IBrush?> backgroundProperty3 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension12 = new DynamicResourceExtension("ThemeControlMidBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding20 = dynamicResourceExtension12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border25.Bind(backgroundProperty3, binding20);
				StyledProperty<IBrush?> borderBrushProperty3 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension13 = new DynamicResourceExtension("ThemeBorderMidBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding21 = dynamicResourceExtension13.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border25.Bind(borderBrushProperty3, binding21);
				border25.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border25.SetValue(Border.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				TextBlock textBlock;
				TextBlock textBlock2 = (textBlock = new TextBlock());
				((ISupportInitialize)textBlock2).BeginInit();
				border25.Child = textBlock2;
				TextBlock textBlock3;
				TextBlock textBlock4 = (textBlock3 = textBlock);
				context.PushParent(textBlock3);
				TextBlock textBlock5 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension14 = new DynamicResourceExtension("ThemeForegroundBrush");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding22 = dynamicResourceExtension14.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock5.Bind(foregroundProperty, binding22);
				textBlock5.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock5.SetValue(TextBlock.TextProperty, "#", BindingPriority.Template);
				textBlock5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				textBlock5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)textBlock4).EndInit();
				context.PopParent();
				((ISupportInitialize)border24).EndInit();
				Controls children13 = grid22.Children;
				TextBox textBox;
				TextBox textBox2 = (textBox = new TextBox());
				((ISupportInitialize)textBox2).BeginInit();
				children13.Add(textBox2);
				textBox.Name = "PART_HexTextBox";
				service = textBox;
				context.AvaloniaNameScope.Register("PART_HexTextBox", service);
				textBox.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				textBox.SetValue(AutomationProperties.NameProperty, "Hexadecimal Color", BindingPriority.Template);
				textBox.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				textBox.SetValue(TextBox.MaxLengthProperty, 9, BindingPriority.Template);
				textBox.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				textBox.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				((ISupportInitialize)textBox).EndInit();
				context.PopParent();
				((ISupportInitialize)grid21).EndInit();
				context.PopParent();
				((ISupportInitialize)grid13).EndInit();
				Controls children14 = grid10.Children;
				Border border26;
				Border border27 = (border26 = new Border());
				((ISupportInitialize)border27).BeginInit();
				children14.Add(border27);
				Border border28 = (border3 = border26);
				context.PushParent(border3);
				Border border29 = border3;
				border29.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border29.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				StyledProperty<double> heightProperty6 = Layoutable.HeightProperty;
				CompiledBindingExtension obj17 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component1NumericUpDown").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding23 = obj17.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border29.Bind(heightProperty6, binding23);
				StyledProperty<double> widthProperty = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension15 = new DynamicResourceExtension("ColorViewComponentLabelWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding24 = dynamicResourceExtension15.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border29.Bind(widthProperty, binding24);
				StyledProperty<IBrush?> backgroundProperty4 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension16 = new DynamicResourceExtension("ThemeControlMidBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding25 = dynamicResourceExtension16.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border29.Bind(backgroundProperty4, binding25);
				StyledProperty<IBrush?> borderBrushProperty4 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension17 = new DynamicResourceExtension("ThemeBorderMidBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding26 = dynamicResourceExtension17.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border29.Bind(borderBrushProperty4, binding26);
				border29.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border29.SetValue(Border.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				border29.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				border29.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				Panel panel;
				Panel panel2 = (panel = new Panel());
				((ISupportInitialize)panel2).BeginInit();
				border29.Child = panel2;
				Panel panel3;
				Panel panel4 = (panel3 = panel);
				context.PushParent(panel3);
				Panel panel5 = panel3;
				panel5.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				panel5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Controls children15 = panel5.Children;
				TextBlock textBlock6;
				TextBlock textBlock7 = (textBlock6 = new TextBlock());
				((ISupportInitialize)textBlock7).BeginInit();
				children15.Add(textBlock7);
				TextBlock textBlock8 = (textBlock3 = textBlock6);
				context.PushParent(textBlock3);
				TextBlock textBlock9 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty2 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension18 = new DynamicResourceExtension("ThemeForegroundBrush");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding27 = dynamicResourceExtension18.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock9.Bind(foregroundProperty2, binding27);
				textBlock9.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock9.SetValue(TextBlock.TextProperty, "R", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty2 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding8 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding9 = templateBinding;
				StaticResourceExtension staticResourceExtension12 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj18 = staticResourceExtension12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding9.Converter = (IValueConverter)obj18;
				templateBinding9.ConverterParameter = ColorModel.Rgba;
				templateBinding9.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock9.Bind(isVisibleProperty2, templateBinding8.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock8).EndInit();
				Controls children16 = panel5.Children;
				TextBlock textBlock10;
				TextBlock textBlock11 = (textBlock10 = new TextBlock());
				((ISupportInitialize)textBlock11).BeginInit();
				children16.Add(textBlock11);
				TextBlock textBlock12 = (textBlock3 = textBlock10);
				context.PushParent(textBlock3);
				TextBlock textBlock13 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty3 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension19 = new DynamicResourceExtension("ThemeForegroundBrush");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding28 = dynamicResourceExtension19.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock13.Bind(foregroundProperty3, binding28);
				textBlock13.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock13.SetValue(TextBlock.TextProperty, "H", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty3 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding10 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding11 = templateBinding;
				StaticResourceExtension staticResourceExtension13 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj19 = staticResourceExtension13.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding11.Converter = (IValueConverter)obj19;
				templateBinding11.ConverterParameter = ColorModel.Hsva;
				templateBinding11.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock13.Bind(isVisibleProperty3, templateBinding10.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock12).EndInit();
				context.PopParent();
				((ISupportInitialize)panel4).EndInit();
				context.PopParent();
				((ISupportInitialize)border28).EndInit();
				Controls children17 = grid10.Children;
				NumericUpDown numericUpDown;
				NumericUpDown numericUpDown2 = (numericUpDown = new NumericUpDown());
				((ISupportInitialize)numericUpDown2).BeginInit();
				children17.Add(numericUpDown2);
				NumericUpDown numericUpDown3;
				NumericUpDown numericUpDown4 = (numericUpDown3 = numericUpDown);
				context.PushParent(numericUpDown3);
				NumericUpDown numericUpDown5 = numericUpDown3;
				numericUpDown5.Name = "Component1NumericUpDown";
				service = numericUpDown5;
				context.AvaloniaNameScope.Register("Component1NumericUpDown", service);
				numericUpDown5.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				numericUpDown5.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				numericUpDown5.SetValue(NumericUpDown.AllowSpinProperty, value: true, BindingPriority.Template);
				numericUpDown5.SetValue(NumericUpDown.ShowButtonSpinnerProperty, value: false, BindingPriority.Template);
				numericUpDown5.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<double> widthProperty2 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension20 = new DynamicResourceExtension("ColorViewComponentTextInputWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding29 = dynamicResourceExtension20.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown5.Bind(widthProperty2, binding29);
				numericUpDown5.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				numericUpDown5.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				numericUpDown5.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension14 = new StaticResourceExtension("ColorViewComponentNumberFormat");
				context.ProvideTargetProperty = NumericUpDown.NumberFormatProperty;
				object? obj20 = staticResourceExtension14.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_109.DynamicSetter_4(numericUpDown5, BindingPriority.Template, obj20);
				StyledProperty<decimal> minimumProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension compiledBindingExtension7 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component1Slider").Property(RangeBase.MinimumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension binding30 = compiledBindingExtension7.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown5.Bind(minimumProperty, binding30);
				StyledProperty<decimal> maximumProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension compiledBindingExtension8 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component1Slider").Property(RangeBase.MaximumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension binding31 = compiledBindingExtension8.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown5.Bind(maximumProperty, binding31);
				StyledProperty<decimal?> valueProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension compiledBindingExtension9 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component1Slider").Property(RangeBase.ValueProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension binding32 = compiledBindingExtension9.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown5.Bind(valueProperty, binding32);
				numericUpDown5.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)numericUpDown4).EndInit();
				Controls children18 = grid10.Children;
				ColorSlider colorSlider10;
				ColorSlider colorSlider11 = (colorSlider10 = new ColorSlider());
				((ISupportInitialize)colorSlider11).BeginInit();
				children18.Add(colorSlider11);
				ColorSlider colorSlider12 = (colorSlider3 = colorSlider10);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider13 = colorSlider3;
				colorSlider13.Name = "Component1Slider";
				service = colorSlider13;
				context.AvaloniaNameScope.Register("Component1Slider", service);
				colorSlider13.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider13.SetValue(Grid.RowProperty, 2, BindingPriority.Template);
				colorSlider13.SetValue(Slider.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				colorSlider13.SetValue(ColorSlider.IsRoundingEnabledProperty, value: true, BindingPriority.Template);
				colorSlider13.SetValue(Slider.IsSnapToTickEnabledProperty, value: true, BindingPriority.Template);
				colorSlider13.SetValue(Slider.TickFrequencyProperty, 1.0, BindingPriority.Template);
				colorSlider13.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Component1, BindingPriority.Template);
				colorSlider13.Bind(ColorSlider.ColorModelProperty, new TemplateBinding(ColorView.ColorModelProperty)
				{
					Mode = BindingMode.OneWay
				}.ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty4 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension obj21 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding33 = obj21.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider13.Bind(hsvColorProperty4, binding33);
				colorSlider13.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSlider13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				colorSlider13.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentSliderVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorSlider12).EndInit();
				Controls children19 = grid10.Children;
				Border border30;
				Border border31 = (border30 = new Border());
				((ISupportInitialize)border31).BeginInit();
				children19.Add(border31);
				Border border32 = (border3 = border30);
				context.PushParent(border3);
				Border border33 = border3;
				border33.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border33.SetValue(Grid.RowProperty, 3, BindingPriority.Template);
				StyledProperty<double> widthProperty3 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension21 = new DynamicResourceExtension("ColorViewComponentLabelWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding34 = dynamicResourceExtension21.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border33.Bind(widthProperty3, binding34);
				StyledProperty<double> heightProperty7 = Layoutable.HeightProperty;
				CompiledBindingExtension obj22 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component2NumericUpDown").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding35 = obj22.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border33.Bind(heightProperty7, binding35);
				StyledProperty<IBrush?> backgroundProperty5 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension22 = new DynamicResourceExtension("ThemeControlMidBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding36 = dynamicResourceExtension22.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border33.Bind(backgroundProperty5, binding36);
				StyledProperty<IBrush?> borderBrushProperty5 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension23 = new DynamicResourceExtension("ThemeBorderMidBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding37 = dynamicResourceExtension23.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border33.Bind(borderBrushProperty5, binding37);
				border33.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border33.SetValue(Border.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				border33.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				border33.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				Panel panel6;
				Panel panel7 = (panel6 = new Panel());
				((ISupportInitialize)panel7).BeginInit();
				border33.Child = panel7;
				Panel panel8 = (panel3 = panel6);
				context.PushParent(panel3);
				Panel panel9 = panel3;
				panel9.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				panel9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Controls children20 = panel9.Children;
				TextBlock textBlock14;
				TextBlock textBlock15 = (textBlock14 = new TextBlock());
				((ISupportInitialize)textBlock15).BeginInit();
				children20.Add(textBlock15);
				TextBlock textBlock16 = (textBlock3 = textBlock14);
				context.PushParent(textBlock3);
				TextBlock textBlock17 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty4 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension24 = new DynamicResourceExtension("ThemeForegroundBrush");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding38 = dynamicResourceExtension24.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock17.Bind(foregroundProperty4, binding38);
				textBlock17.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock17.SetValue(TextBlock.TextProperty, "G", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty4 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding12 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding13 = templateBinding;
				StaticResourceExtension staticResourceExtension15 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj23 = staticResourceExtension15.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding13.Converter = (IValueConverter)obj23;
				templateBinding13.ConverterParameter = ColorModel.Rgba;
				templateBinding13.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock17.Bind(isVisibleProperty4, templateBinding12.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock16).EndInit();
				Controls children21 = panel9.Children;
				TextBlock textBlock18;
				TextBlock textBlock19 = (textBlock18 = new TextBlock());
				((ISupportInitialize)textBlock19).BeginInit();
				children21.Add(textBlock19);
				TextBlock textBlock20 = (textBlock3 = textBlock18);
				context.PushParent(textBlock3);
				TextBlock textBlock21 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty5 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension25 = new DynamicResourceExtension("ThemeForegroundBrush");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding39 = dynamicResourceExtension25.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock21.Bind(foregroundProperty5, binding39);
				textBlock21.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock21.SetValue(TextBlock.TextProperty, "S", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty5 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding14 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding15 = templateBinding;
				StaticResourceExtension staticResourceExtension16 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj24 = staticResourceExtension16.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding15.Converter = (IValueConverter)obj24;
				templateBinding15.ConverterParameter = ColorModel.Hsva;
				templateBinding15.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock21.Bind(isVisibleProperty5, templateBinding14.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock20).EndInit();
				context.PopParent();
				((ISupportInitialize)panel8).EndInit();
				context.PopParent();
				((ISupportInitialize)border32).EndInit();
				Controls children22 = grid10.Children;
				NumericUpDown numericUpDown6;
				NumericUpDown numericUpDown7 = (numericUpDown6 = new NumericUpDown());
				((ISupportInitialize)numericUpDown7).BeginInit();
				children22.Add(numericUpDown7);
				NumericUpDown numericUpDown8 = (numericUpDown3 = numericUpDown6);
				context.PushParent(numericUpDown3);
				NumericUpDown numericUpDown9 = numericUpDown3;
				numericUpDown9.Name = "Component2NumericUpDown";
				service = numericUpDown9;
				context.AvaloniaNameScope.Register("Component2NumericUpDown", service);
				numericUpDown9.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				numericUpDown9.SetValue(Grid.RowProperty, 3, BindingPriority.Template);
				numericUpDown9.SetValue(NumericUpDown.AllowSpinProperty, value: true, BindingPriority.Template);
				numericUpDown9.SetValue(NumericUpDown.ShowButtonSpinnerProperty, value: false, BindingPriority.Template);
				numericUpDown9.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<double> widthProperty4 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension26 = new DynamicResourceExtension("ColorViewComponentTextInputWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding40 = dynamicResourceExtension26.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown9.Bind(widthProperty4, binding40);
				numericUpDown9.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				numericUpDown9.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				numericUpDown9.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension17 = new StaticResourceExtension("ColorViewComponentNumberFormat");
				context.ProvideTargetProperty = NumericUpDown.NumberFormatProperty;
				object? obj25 = staticResourceExtension17.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_109.DynamicSetter_4(numericUpDown9, BindingPriority.Template, obj25);
				StyledProperty<decimal> minimumProperty2 = NumericUpDown.MinimumProperty;
				CompiledBindingExtension compiledBindingExtension10 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component2Slider").Property(RangeBase.MinimumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension binding41 = compiledBindingExtension10.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown9.Bind(minimumProperty2, binding41);
				StyledProperty<decimal> maximumProperty2 = NumericUpDown.MaximumProperty;
				CompiledBindingExtension compiledBindingExtension11 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component2Slider").Property(RangeBase.MaximumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension binding42 = compiledBindingExtension11.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown9.Bind(maximumProperty2, binding42);
				StyledProperty<decimal?> valueProperty2 = NumericUpDown.ValueProperty;
				CompiledBindingExtension compiledBindingExtension12 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component2Slider").Property(RangeBase.ValueProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension binding43 = compiledBindingExtension12.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown9.Bind(valueProperty2, binding43);
				numericUpDown9.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)numericUpDown8).EndInit();
				Controls children23 = grid10.Children;
				ColorSlider colorSlider14;
				ColorSlider colorSlider15 = (colorSlider14 = new ColorSlider());
				((ISupportInitialize)colorSlider15).BeginInit();
				children23.Add(colorSlider15);
				ColorSlider colorSlider16 = (colorSlider3 = colorSlider14);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider17 = colorSlider3;
				colorSlider17.Name = "Component2Slider";
				service = colorSlider17;
				context.AvaloniaNameScope.Register("Component2Slider", service);
				colorSlider17.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider17.SetValue(Grid.RowProperty, 3, BindingPriority.Template);
				colorSlider17.SetValue(Slider.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				colorSlider17.SetValue(ColorSlider.IsRoundingEnabledProperty, value: true, BindingPriority.Template);
				colorSlider17.SetValue(Slider.IsSnapToTickEnabledProperty, value: true, BindingPriority.Template);
				colorSlider17.SetValue(Slider.TickFrequencyProperty, 1.0, BindingPriority.Template);
				colorSlider17.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Component2, BindingPriority.Template);
				colorSlider17.Bind(ColorSlider.ColorModelProperty, new TemplateBinding(ColorView.ColorModelProperty)
				{
					Mode = BindingMode.OneWay
				}.ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty5 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension obj26 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding44 = obj26.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider17.Bind(hsvColorProperty5, binding44);
				colorSlider17.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSlider17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				colorSlider17.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentSliderVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorSlider16).EndInit();
				Controls children24 = grid10.Children;
				Border border34;
				Border border35 = (border34 = new Border());
				((ISupportInitialize)border35).BeginInit();
				children24.Add(border35);
				Border border36 = (border3 = border34);
				context.PushParent(border3);
				Border border37 = border3;
				border37.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border37.SetValue(Grid.RowProperty, 4, BindingPriority.Template);
				StyledProperty<double> widthProperty5 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension27 = new DynamicResourceExtension("ColorViewComponentLabelWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding45 = dynamicResourceExtension27.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border37.Bind(widthProperty5, binding45);
				StyledProperty<double> heightProperty8 = Layoutable.HeightProperty;
				CompiledBindingExtension obj27 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component3NumericUpDown").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding46 = obj27.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border37.Bind(heightProperty8, binding46);
				StyledProperty<IBrush?> backgroundProperty6 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension28 = new DynamicResourceExtension("ThemeControlMidBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding47 = dynamicResourceExtension28.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border37.Bind(backgroundProperty6, binding47);
				StyledProperty<IBrush?> borderBrushProperty6 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension29 = new DynamicResourceExtension("ThemeBorderMidBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding48 = dynamicResourceExtension29.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border37.Bind(borderBrushProperty6, binding48);
				border37.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border37.SetValue(Border.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				border37.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				border37.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				Panel panel10;
				Panel panel11 = (panel10 = new Panel());
				((ISupportInitialize)panel11).BeginInit();
				border37.Child = panel11;
				Panel panel12 = (panel3 = panel10);
				context.PushParent(panel3);
				Panel panel13 = panel3;
				panel13.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				panel13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				Controls children25 = panel13.Children;
				TextBlock textBlock22;
				TextBlock textBlock23 = (textBlock22 = new TextBlock());
				((ISupportInitialize)textBlock23).BeginInit();
				children25.Add(textBlock23);
				TextBlock textBlock24 = (textBlock3 = textBlock22);
				context.PushParent(textBlock3);
				TextBlock textBlock25 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty6 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension30 = new DynamicResourceExtension("ThemeForegroundBrush");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding49 = dynamicResourceExtension30.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock25.Bind(foregroundProperty6, binding49);
				textBlock25.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock25.SetValue(TextBlock.TextProperty, "B", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty6 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding16 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding17 = templateBinding;
				StaticResourceExtension staticResourceExtension18 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj28 = staticResourceExtension18.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding17.Converter = (IValueConverter)obj28;
				templateBinding17.ConverterParameter = ColorModel.Rgba;
				templateBinding17.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock25.Bind(isVisibleProperty6, templateBinding16.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock24).EndInit();
				Controls children26 = panel13.Children;
				TextBlock textBlock26;
				TextBlock textBlock27 = (textBlock26 = new TextBlock());
				((ISupportInitialize)textBlock27).BeginInit();
				children26.Add(textBlock27);
				TextBlock textBlock28 = (textBlock3 = textBlock26);
				context.PushParent(textBlock3);
				TextBlock textBlock29 = textBlock3;
				StyledProperty<IBrush?> foregroundProperty7 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension31 = new DynamicResourceExtension("ThemeForegroundBrush");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding50 = dynamicResourceExtension31.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock29.Bind(foregroundProperty7, binding50);
				textBlock29.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock29.SetValue(TextBlock.TextProperty, "V", BindingPriority.Template);
				StyledProperty<bool> isVisibleProperty7 = Visual.IsVisibleProperty;
				TemplateBinding templateBinding18 = (templateBinding = new TemplateBinding(ColorView.ColorModelProperty));
				context.PushParent(templateBinding);
				TemplateBinding templateBinding19 = templateBinding;
				StaticResourceExtension staticResourceExtension19 = new StaticResourceExtension("EnumToBoolConverter");
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002ETemplateBinding_002CAvalonia_002EBase_002EConverter_0021Property();
				object? obj29 = staticResourceExtension19.ProvideValue(context);
				context.ProvideTargetProperty = null;
				templateBinding19.Converter = (IValueConverter)obj29;
				templateBinding19.ConverterParameter = ColorModel.Hsva;
				templateBinding19.Mode = BindingMode.OneWay;
				context.PopParent();
				textBlock29.Bind(isVisibleProperty7, templateBinding18.ProvideValue());
				context.PopParent();
				((ISupportInitialize)textBlock28).EndInit();
				context.PopParent();
				((ISupportInitialize)panel12).EndInit();
				context.PopParent();
				((ISupportInitialize)border36).EndInit();
				Controls children27 = grid10.Children;
				NumericUpDown numericUpDown10;
				NumericUpDown numericUpDown11 = (numericUpDown10 = new NumericUpDown());
				((ISupportInitialize)numericUpDown11).BeginInit();
				children27.Add(numericUpDown11);
				NumericUpDown numericUpDown12 = (numericUpDown3 = numericUpDown10);
				context.PushParent(numericUpDown3);
				NumericUpDown numericUpDown13 = numericUpDown3;
				numericUpDown13.Name = "Component3NumericUpDown";
				service = numericUpDown13;
				context.AvaloniaNameScope.Register("Component3NumericUpDown", service);
				numericUpDown13.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				numericUpDown13.SetValue(Grid.RowProperty, 4, BindingPriority.Template);
				numericUpDown13.SetValue(NumericUpDown.AllowSpinProperty, value: true, BindingPriority.Template);
				numericUpDown13.SetValue(NumericUpDown.ShowButtonSpinnerProperty, value: false, BindingPriority.Template);
				numericUpDown13.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<double> widthProperty6 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension32 = new DynamicResourceExtension("ColorViewComponentTextInputWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding51 = dynamicResourceExtension32.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown13.Bind(widthProperty6, binding51);
				numericUpDown13.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				numericUpDown13.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				numericUpDown13.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension20 = new StaticResourceExtension("ColorViewComponentNumberFormat");
				context.ProvideTargetProperty = NumericUpDown.NumberFormatProperty;
				object? obj30 = staticResourceExtension20.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_109.DynamicSetter_4(numericUpDown13, BindingPriority.Template, obj30);
				StyledProperty<decimal> minimumProperty3 = NumericUpDown.MinimumProperty;
				CompiledBindingExtension compiledBindingExtension13 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component3Slider").Property(RangeBase.MinimumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension binding52 = compiledBindingExtension13.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown13.Bind(minimumProperty3, binding52);
				StyledProperty<decimal> maximumProperty3 = NumericUpDown.MaximumProperty;
				CompiledBindingExtension compiledBindingExtension14 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component3Slider").Property(RangeBase.MaximumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension binding53 = compiledBindingExtension14.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown13.Bind(maximumProperty3, binding53);
				StyledProperty<decimal?> valueProperty3 = NumericUpDown.ValueProperty;
				CompiledBindingExtension compiledBindingExtension15 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "Component3Slider").Property(RangeBase.ValueProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension binding54 = compiledBindingExtension15.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown13.Bind(valueProperty3, binding54);
				numericUpDown13.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentTextInputVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)numericUpDown12).EndInit();
				Controls children28 = grid10.Children;
				ColorSlider colorSlider18;
				ColorSlider colorSlider19 = (colorSlider18 = new ColorSlider());
				((ISupportInitialize)colorSlider19).BeginInit();
				children28.Add(colorSlider19);
				ColorSlider colorSlider20 = (colorSlider3 = colorSlider18);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider21 = colorSlider3;
				colorSlider21.Name = "Component3Slider";
				service = colorSlider21;
				context.AvaloniaNameScope.Register("Component3Slider", service);
				colorSlider21.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider21.SetValue(Grid.RowProperty, 4, BindingPriority.Template);
				colorSlider21.SetValue(Slider.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				colorSlider21.SetValue(ColorSlider.IsRoundingEnabledProperty, value: true, BindingPriority.Template);
				colorSlider21.SetValue(Slider.IsSnapToTickEnabledProperty, value: true, BindingPriority.Template);
				colorSlider21.SetValue(Slider.TickFrequencyProperty, 1.0, BindingPriority.Template);
				colorSlider21.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Component3, BindingPriority.Template);
				colorSlider21.Bind(ColorSlider.ColorModelProperty, new TemplateBinding(ColorView.ColorModelProperty)
				{
					Mode = BindingMode.OneWay
				}.ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty6 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension obj31 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding55 = obj31.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider21.Bind(hsvColorProperty6, binding55);
				colorSlider21.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSlider21.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				colorSlider21.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsComponentSliderVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorSlider20).EndInit();
				Controls children29 = grid10.Children;
				Border border38;
				Border border39 = (border38 = new Border());
				((ISupportInitialize)border39).BeginInit();
				children29.Add(border39);
				Border border40 = (border3 = border38);
				context.PushParent(border3);
				Border border41 = border3;
				border41.SetValue(Grid.ColumnProperty, 0, BindingPriority.Template);
				border41.SetValue(Grid.RowProperty, 5, BindingPriority.Template);
				StyledProperty<double> widthProperty7 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension33 = new DynamicResourceExtension("ColorViewComponentLabelWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding56 = dynamicResourceExtension33.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border41.Bind(widthProperty7, binding56);
				StyledProperty<double> heightProperty9 = Layoutable.HeightProperty;
				CompiledBindingExtension obj32 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "AlphaComponentNumericUpDown").Property(Visual.BoundsProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(XamlIlHelpers.Avalonia_002ERect_002CAvalonia_002EBase_002EHeight_0021Property(), PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = Layoutable.HeightProperty;
				CompiledBindingExtension binding57 = obj32.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border41.Bind(heightProperty9, binding57);
				StyledProperty<IBrush?> backgroundProperty7 = Border.BackgroundProperty;
				DynamicResourceExtension dynamicResourceExtension34 = new DynamicResourceExtension("ThemeControlMidBrush");
				context.ProvideTargetProperty = Border.BackgroundProperty;
				IBinding binding58 = dynamicResourceExtension34.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border41.Bind(backgroundProperty7, binding58);
				StyledProperty<IBrush?> borderBrushProperty7 = Border.BorderBrushProperty;
				DynamicResourceExtension dynamicResourceExtension35 = new DynamicResourceExtension("ThemeBorderMidBrush");
				context.ProvideTargetProperty = Border.BorderBrushProperty;
				IBinding binding59 = dynamicResourceExtension35.ProvideValue(context);
				context.ProvideTargetProperty = null;
				border41.Bind(borderBrushProperty7, binding59);
				border41.SetValue(Border.BorderThicknessProperty, new Thickness(1.0, 1.0, 0.0, 1.0), BindingPriority.Template);
				border41.SetValue(Border.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				border41.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				border41.Bind(InputElement.IsEnabledProperty, new TemplateBinding(ColorView.IsAlphaEnabledProperty).ProvideValue());
				StyledProperty<bool> isVisibleProperty8 = Visual.IsVisibleProperty;
				MultiBinding binding60 = (multiBinding = new MultiBinding());
				context.PushParent(multiBinding);
				MultiBinding multiBinding3 = multiBinding;
				multiBinding3.Converter = BoolConverters.And;
				IList<IBinding> bindings2 = multiBinding3.Bindings;
				CompiledBindingExtension obj33 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsAlphaVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item2 = obj33.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings2.Add(item2);
				IList<IBinding> bindings3 = multiBinding3.Bindings;
				CompiledBindingExtension obj34 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsComponentTextInputVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item3 = obj34.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings3.Add(item3);
				context.PopParent();
				border41.Bind(isVisibleProperty8, binding60);
				TextBlock textBlock30;
				TextBlock textBlock31 = (textBlock30 = new TextBlock());
				((ISupportInitialize)textBlock31).BeginInit();
				border41.Child = textBlock31;
				TextBlock textBlock32 = (textBlock3 = textBlock30);
				context.PushParent(textBlock3);
				TextBlock textBlock33 = textBlock3;
				textBlock33.Name = "AlphaComponentTextBlock";
				service = textBlock33;
				context.AvaloniaNameScope.Register("AlphaComponentTextBlock", service);
				StyledProperty<IBrush?> foregroundProperty8 = TextBlock.ForegroundProperty;
				DynamicResourceExtension dynamicResourceExtension36 = new DynamicResourceExtension("ThemeForegroundBrush");
				context.ProvideTargetProperty = TextBlock.ForegroundProperty;
				IBinding binding61 = dynamicResourceExtension36.ProvideValue(context);
				context.ProvideTargetProperty = null;
				textBlock33.Bind(foregroundProperty8, binding61);
				textBlock33.SetValue(TextBlock.FontWeightProperty, FontWeight.DemiBold, BindingPriority.Template);
				textBlock33.SetValue(TextBlock.TextProperty, "A", BindingPriority.Template);
				textBlock33.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
				textBlock33.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				context.PopParent();
				((ISupportInitialize)textBlock32).EndInit();
				context.PopParent();
				((ISupportInitialize)border40).EndInit();
				Controls children30 = grid10.Children;
				NumericUpDown numericUpDown14;
				NumericUpDown numericUpDown15 = (numericUpDown14 = new NumericUpDown());
				((ISupportInitialize)numericUpDown15).BeginInit();
				children30.Add(numericUpDown15);
				NumericUpDown numericUpDown16 = (numericUpDown3 = numericUpDown14);
				context.PushParent(numericUpDown3);
				NumericUpDown numericUpDown17 = numericUpDown3;
				numericUpDown17.Name = "AlphaComponentNumericUpDown";
				service = numericUpDown17;
				context.AvaloniaNameScope.Register("AlphaComponentNumericUpDown", service);
				numericUpDown17.SetValue(Grid.ColumnProperty, 1, BindingPriority.Template);
				numericUpDown17.SetValue(Grid.RowProperty, 5, BindingPriority.Template);
				numericUpDown17.SetValue(NumericUpDown.AllowSpinProperty, value: true, BindingPriority.Template);
				numericUpDown17.SetValue(NumericUpDown.ShowButtonSpinnerProperty, value: false, BindingPriority.Template);
				numericUpDown17.SetValue(Layoutable.HeightProperty, 32.0, BindingPriority.Template);
				StyledProperty<double> widthProperty8 = Layoutable.WidthProperty;
				DynamicResourceExtension dynamicResourceExtension37 = new DynamicResourceExtension("ColorViewComponentTextInputWidth");
				context.ProvideTargetProperty = Layoutable.WidthProperty;
				IBinding binding62 = dynamicResourceExtension37.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown17.Bind(widthProperty8, binding62);
				numericUpDown17.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(0.0, 0.0, 0.0, 0.0), BindingPriority.Template);
				numericUpDown17.SetValue(Layoutable.MarginProperty, new Thickness(0.0, 0.0, 12.0, 0.0), BindingPriority.Template);
				numericUpDown17.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				StaticResourceExtension staticResourceExtension21 = new StaticResourceExtension("ColorViewComponentNumberFormat");
				context.ProvideTargetProperty = NumericUpDown.NumberFormatProperty;
				object? obj35 = staticResourceExtension21.ProvideValue(context);
				context.ProvideTargetProperty = null;
				DynamicSetters_109.DynamicSetter_4(numericUpDown17, BindingPriority.Template, obj35);
				StyledProperty<decimal> minimumProperty4 = NumericUpDown.MinimumProperty;
				CompiledBindingExtension compiledBindingExtension16 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "AlphaComponentSlider").Property(RangeBase.MinimumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MinimumProperty;
				CompiledBindingExtension binding63 = compiledBindingExtension16.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown17.Bind(minimumProperty4, binding63);
				StyledProperty<decimal> maximumProperty4 = NumericUpDown.MaximumProperty;
				CompiledBindingExtension compiledBindingExtension17 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "AlphaComponentSlider").Property(RangeBase.MaximumProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.MaximumProperty;
				CompiledBindingExtension binding64 = compiledBindingExtension17.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown17.Bind(maximumProperty4, binding64);
				StyledProperty<decimal?> valueProperty4 = NumericUpDown.ValueProperty;
				CompiledBindingExtension compiledBindingExtension18 = new CompiledBindingExtension(new CompiledBindingPathBuilder().ElementName(context.AvaloniaNameScope, "AlphaComponentSlider").Property(RangeBase.ValueProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build());
				context.ProvideTargetProperty = NumericUpDown.ValueProperty;
				CompiledBindingExtension binding65 = compiledBindingExtension18.ProvideValue(context);
				context.ProvideTargetProperty = null;
				numericUpDown17.Bind(valueProperty4, binding65);
				numericUpDown17.Bind(InputElement.IsEnabledProperty, new TemplateBinding(ColorView.IsAlphaEnabledProperty).ProvideValue());
				StyledProperty<bool> isVisibleProperty9 = Visual.IsVisibleProperty;
				MultiBinding binding66 = (multiBinding = new MultiBinding());
				context.PushParent(multiBinding);
				MultiBinding multiBinding4 = multiBinding;
				multiBinding4.Converter = BoolConverters.And;
				IList<IBinding> bindings4 = multiBinding4.Bindings;
				CompiledBindingExtension obj36 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsAlphaVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item4 = obj36.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings4.Add(item4);
				IList<IBinding> bindings5 = multiBinding4.Bindings;
				CompiledBindingExtension obj37 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsComponentTextInputVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item5 = obj37.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings5.Add(item5);
				context.PopParent();
				numericUpDown17.Bind(isVisibleProperty9, binding66);
				context.PopParent();
				((ISupportInitialize)numericUpDown16).EndInit();
				Controls children31 = grid10.Children;
				ColorSlider colorSlider22;
				ColorSlider colorSlider23 = (colorSlider22 = new ColorSlider());
				((ISupportInitialize)colorSlider23).BeginInit();
				children31.Add(colorSlider23);
				ColorSlider colorSlider24 = (colorSlider3 = colorSlider22);
				context.PushParent(colorSlider3);
				ColorSlider colorSlider25 = colorSlider3;
				colorSlider25.Name = "AlphaComponentSlider";
				service = colorSlider25;
				context.AvaloniaNameScope.Register("AlphaComponentSlider", service);
				colorSlider25.SetValue(Grid.ColumnProperty, 2, BindingPriority.Template);
				colorSlider25.SetValue(Grid.RowProperty, 5, BindingPriority.Template);
				colorSlider25.SetValue(Slider.OrientationProperty, Orientation.Horizontal, BindingPriority.Template);
				colorSlider25.SetValue(ColorSlider.IsRoundingEnabledProperty, value: true, BindingPriority.Template);
				colorSlider25.SetValue(Slider.IsSnapToTickEnabledProperty, value: true, BindingPriority.Template);
				colorSlider25.SetValue(Slider.TickFrequencyProperty, 1.0, BindingPriority.Template);
				colorSlider25.SetValue(ColorSlider.ColorComponentProperty, ColorComponent.Alpha, BindingPriority.Template);
				colorSlider25.Bind(ColorSlider.ColorModelProperty, new TemplateBinding(ColorView.ColorModelProperty)
				{
					Mode = BindingMode.OneWay
				}.ProvideValue());
				StyledProperty<HsvColor> hsvColorProperty7 = ColorSlider.HsvColorProperty;
				CompiledBindingExtension obj38 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorSlider.HsvColorProperty;
				CompiledBindingExtension binding67 = obj38.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorSlider25.Bind(hsvColorProperty7, binding67);
				colorSlider25.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, BindingPriority.Template);
				colorSlider25.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
				colorSlider25.Bind(InputElement.IsEnabledProperty, new TemplateBinding(ColorView.IsAlphaEnabledProperty).ProvideValue());
				StyledProperty<bool> isVisibleProperty10 = Visual.IsVisibleProperty;
				MultiBinding binding68 = (multiBinding = new MultiBinding());
				context.PushParent(multiBinding);
				MultiBinding multiBinding5 = multiBinding;
				multiBinding5.Converter = BoolConverters.And;
				IList<IBinding> bindings6 = multiBinding5.Bindings;
				CompiledBindingExtension obj39 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsAlphaVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item6 = obj39.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings6.Add(item6);
				IList<IBinding> bindings7 = multiBinding5.Bindings;
				CompiledBindingExtension obj40 = new CompiledBindingExtension
				{
					Path = new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.IsComponentSliderVisibleProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
						.Build()
				};
				context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EData_002EMultiBinding_002CAvalonia_002EMarkup_002EBindings_0021Property();
				CompiledBindingExtension item7 = obj40.ProvideValue(context);
				context.ProvideTargetProperty = null;
				bindings7.Add(item7);
				context.PopParent();
				colorSlider25.Bind(isVisibleProperty10, binding68);
				context.PopParent();
				((ISupportInitialize)colorSlider24).EndInit();
				context.PopParent();
				((ISupportInitialize)grid9).EndInit();
				context.PopParent();
				((ISupportInitialize)tabItem12).EndInit();
				context.PopParent();
				((ISupportInitialize)tabControl4).EndInit();
				Controls children32 = grid2.Children;
				ColorPreviewer colorPreviewer;
				ColorPreviewer colorPreviewer2 = (colorPreviewer = new ColorPreviewer());
				((ISupportInitialize)colorPreviewer2).BeginInit();
				children32.Add(colorPreviewer2);
				ColorPreviewer colorPreviewer3;
				ColorPreviewer colorPreviewer4 = (colorPreviewer3 = colorPreviewer);
				context.PushParent(colorPreviewer3);
				colorPreviewer3.SetValue(Grid.RowProperty, 1, BindingPriority.Template);
				StyledProperty<HsvColor> hsvColorProperty8 = ColorPreviewer.HsvColorProperty;
				CompiledBindingExtension obj41 = new CompiledBindingExtension(new CompiledBindingPathBuilder().Self().Property(StyledElement.TemplatedParentProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Property(ColorView.HsvColorProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor)
					.Build())
				{
					Mode = BindingMode.TwoWay
				};
				context.ProvideTargetProperty = ColorPreviewer.HsvColorProperty;
				CompiledBindingExtension binding69 = obj41.ProvideValue(context);
				context.ProvideTargetProperty = null;
				colorPreviewer3.Bind(hsvColorProperty8, binding69);
				colorPreviewer3.Bind(ColorPreviewer.IsAccentColorsVisibleProperty, new TemplateBinding(ColorView.IsAccentColorsVisibleProperty).ProvideValue());
				colorPreviewer3.SetValue(Layoutable.MarginProperty, new Thickness(12.0, 0.0, 12.0, 12.0), BindingPriority.Template);
				colorPreviewer3.Bind(Visual.IsVisibleProperty, new TemplateBinding(ColorView.IsColorPreviewVisibleProperty).ProvideValue());
				context.PopParent();
				((ISupportInitialize)colorPreviewer4).EndInit();
				context.PopParent();
				((ISupportInitialize)intermediateRoot).EndInit();
				return intermediateRoot;
			}
		}

		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
			controlTheme.TargetType = typeof(ColorView);
			Setter setter;
			Setter setter2 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter3 = setter;
			setter3.Property = TemplatedControl.CornerRadiusProperty;
			DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension("ControlCornerRadius");
			context.ProvideTargetProperty = XamlIlHelpers.Avalonia_002EStyling_002ESetter_002CAvalonia_002EBase_002EValue_0021Property();
			IBinding value = dynamicResourceExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			setter3.Value = value;
			context.PopParent();
			controlTheme.Add(setter2);
			Setter setter4 = new Setter();
			setter4.Property = ColorView.HexInputAlphaPositionProperty;
			setter4.Value = AlphaComponentPosition.Trailing;
			controlTheme.Add(setter4);
			Setter setter5 = new Setter();
			setter5.Property = ColorView.PaletteProperty;
			setter5.Value = new FluentColorPalette();
			controlTheme.Add(setter5);
			Setter setter6 = (setter = new Setter());
			context.PushParent(setter);
			Setter setter7 = setter;
			setter7.Property = TemplatedControl.TemplateProperty;
			ControlTemplate controlTemplate;
			ControlTemplate value2 = (controlTemplate = new ControlTemplate());
			context.PushParent(controlTemplate);
			controlTemplate.TargetType = typeof(ColorView);
			controlTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<Control>(XamlClosure_107.Build, context);
			context.PopParent();
			setter7.Value = value2;
			context.PopParent();
			controlTheme.Add(setter6);
			context.PopParent();
			return result;
		}
	}

	private class XamlClosure_113
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			VisualBrush visualBrush = new VisualBrush();
			visualBrush.TileMode = TileMode.Tile;
			visualBrush.Stretch = Stretch.Uniform;
			visualBrush.DestinationRect = RelativeRect.Parse("0,0,8,8");
			Image image;
			Image image2 = (image = new Image());
			((ISupportInitialize)image2).BeginInit();
			visualBrush.Visual = image2;
			image.Width = 8.0;
			image.Height = 8.0;
			DrawingImage drawingImage = new DrawingImage();
			DrawingGroup drawingGroup = new DrawingGroup();
			DrawingCollection children = drawingGroup.Children;
			GeometryDrawing geometryDrawing = new GeometryDrawing();
			geometryDrawing.Geometry = Geometry.Parse("M0,0 L2,0 2,2, 0,2Z");
			geometryDrawing.Brush = new ImmutableSolidColorBrush(16777215u);
			children.Add(geometryDrawing);
			DrawingCollection children2 = drawingGroup.Children;
			GeometryDrawing geometryDrawing2 = new GeometryDrawing();
			geometryDrawing2.Geometry = Geometry.Parse("M0,1 L2,1 2,2, 1,2 1,0 0,0Z");
			geometryDrawing2.Brush = new ImmutableSolidColorBrush(427851904u);
			children2.Add(geometryDrawing2);
			drawingImage.Drawing = drawingGroup;
			image.Source = drawingImage;
			((ISupportInitialize)image).EndInit();
			return visualBrush;
		}
	}

	private class XamlClosure_114
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
				Color = Color.FromUInt32(uint.MaxValue)
			};
		}
	}

	private class XamlClosure_115
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
				Color = Color.FromUInt32(3825205248u)
			};
		}
	}

	private class XamlClosure_116
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
				Color = Color.FromUInt32(16777215u)
			};
		}
	}

	private class XamlClosure_117
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
				Color = Color.FromUInt32(16777215u)
			};
		}
	}

	private class XamlClosure_118
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
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
				Color = Color.FromUInt32(16777215u)
			};
		}
	}

	private class XamlClosure_119
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new EnumToBoolConverter();
		}
	}

	private class XamlClosure_120
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new ToBrushConverter();
		}
	}

	private class XamlClosure_121
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new CornerRadiusFilterConverter
			{
				Filter = (Corners.TopLeft | Corners.BottomLeft)
			};
		}
	}

	private class XamlClosure_122
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new CornerRadiusFilterConverter
			{
				Filter = (Corners.TopRight | Corners.BottomRight)
			};
		}
	}

	private class XamlClosure_123
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new CornerRadiusFilterConverter
			{
				Filter = (Corners.TopLeft | Corners.TopRight)
			};
		}
	}

	private class XamlClosure_124
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new CornerRadiusFilterConverter
			{
				Filter = (Corners.BottomLeft | Corners.BottomRight)
			};
		}
	}

	private class XamlClosure_125
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new CornerRadiusToDoubleConverter
			{
				Corner = Corners.TopLeft
			};
		}
	}

	private class XamlClosure_126
	{
		public static object Build(IServiceProvider P_0)
		{
			XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (Styles)service;
				}
			}
			return new CornerRadiusToDoubleConverter
			{
				Corner = Corners.BottomRight
			};
		}
	}

	public static void Populate_003A_002FThemes_002FFluent_002FFluent_002Examl(IServiceProvider P_0, Styles P_1)
	{
		XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FFluent_002FFluent_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		Styles styles;
		Styles styles2 = (styles = P_1);
		context.PushParent(styles);
		ResourceDictionary resourceDictionary;
		ResourceDictionary resources = (resourceDictionary = new ResourceDictionary());
		context.PushParent(resourceDictionary);
		resourceDictionary.AddDeferred(typeof(ColorSpectrum), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_1.Build, context));
		resourceDictionary.AddDeferred(typeof(ColorPicker), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_3.Build, context));
		resourceDictionary.AddDeferred("AccentColorConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_10.Build, context));
		resourceDictionary.Add("ColorPreviewerAccentSectionWidth", 80.0);
		resourceDictionary.Add("ColorPreviewerAccentSectionHeight", 40.0);
		resourceDictionary.AddDeferred(typeof(ColorPreviewer), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_11.Build, context));
		resourceDictionary.Add("ColorSliderSize", 20.0);
		resourceDictionary.Add("ColorSliderTrackSize", 20.0);
		resourceDictionary.Add("ColorSliderCornerRadius", new CornerRadius(10.0, 10.0, 10.0, 10.0));
		resourceDictionary.Add("ColorSliderTrackCornerRadius", new CornerRadius(10.0, 10.0, 10.0, 10.0));
		resourceDictionary.AddDeferred("ColorSliderThumbTheme", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_14.Build, context));
		resourceDictionary.AddDeferred(typeof(ColorSlider), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_16.Build, context));
		resourceDictionary.AddDeferred("ContrastBrushConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_25.Build, context));
		resourceDictionary.AddDeferred("ColorToDisplayNameConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_26.Build, context));
		resourceDictionary.AddDeferred("DoNothingForNullConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_27.Build, context));
		resourceDictionary.AddDeferred("ColorViewComponentNumberFormat", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_28.Build, context));
		resourceDictionary.Add("ColorViewTabStripHeight", 48.0);
		resourceDictionary.Add("ColorViewComponentLabelWidth", 30.0);
		resourceDictionary.Add("ColorViewComponentTextInputWidth", 80.0);
		resourceDictionary.AddDeferred("ColorViewSpectrumIconGeometry", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_29.Build, context));
		resourceDictionary.AddDeferred("ColorViewPaletteIconGeometry", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_30.Build, context));
		resourceDictionary.AddDeferred("ColorViewComponentsIconGeometry", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_31.Build, context));
		resourceDictionary.Add("ColorViewTabBackgroundCornerRadius", new CornerRadius(3.0, 3.0, 3.0, 3.0));
		resourceDictionary.AddDeferred("ColorViewPaletteListBoxTheme", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_32.Build, context));
		resourceDictionary.AddDeferred("ColorViewPaletteListBoxItemTheme", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_34.Build, context));
		resourceDictionary.AddDeferred("ColorViewColorModelRadioButtonTheme", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_37.Build, context));
		resourceDictionary.AddDeferred("ColorViewTabItemTheme", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_40.Build, context));
		resourceDictionary.AddDeferred(typeof(ColorView), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_43.Build, context));
		resourceDictionary.AddDeferred("ColorControlCheckeredBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_50.Build, context));
		resourceDictionary.AddDeferred("ColorControlLightSelectorBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_51.Build, context));
		resourceDictionary.AddDeferred("ColorControlDarkSelectorBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_52.Build, context));
		resourceDictionary.AddDeferred("ColorViewContentBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_53.Build, context));
		resourceDictionary.AddDeferred("ColorViewContentBorderBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_54.Build, context));
		resourceDictionary.AddDeferred("ColorViewTabBorderBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_55.Build, context));
		resourceDictionary.AddDeferred("EnumToBoolConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_56.Build, context));
		resourceDictionary.AddDeferred("ToBrushConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_57.Build, context));
		resourceDictionary.AddDeferred("LeftCornerRadiusFilterConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_58.Build, context));
		resourceDictionary.AddDeferred("RightCornerRadiusFilterConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_59.Build, context));
		resourceDictionary.AddDeferred("TopCornerRadiusFilterConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_60.Build, context));
		resourceDictionary.AddDeferred("BottomCornerRadiusFilterConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_61.Build, context));
		resourceDictionary.AddDeferred("TopLeftCornerRadiusConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_62.Build, context));
		resourceDictionary.AddDeferred("BottomRightCornerRadiusConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_63.Build, context));
		context.PopParent();
		styles.Resources = resources;
		context.PopParent();
		if (styles2 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	public static Styles Build_003A_002FThemes_002FFluent_002FFluent_002Examl(IServiceProvider P_0)
	{
		Styles styles = new Styles();
		Populate_003A_002FThemes_002FFluent_002FFluent_002Examl(P_0, styles);
		return styles;
	}

	public static void Populate_003A_002FThemes_002FSimple_002FSimple_002Examl(IServiceProvider P_0, Styles P_1)
	{
		XamlIlContext.Context<Styles> context = new XamlIlContext.Context<Styles>(P_0, new object[1] { NamespaceInfo_003A_002FThemes_002FSimple_002FSimple_002Examl.Singleton }, "avares://Avalonia.Controls.ColorPicker/Themes/Simple/Simple.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		Styles styles;
		Styles styles2 = (styles = P_1);
		context.PushParent(styles);
		ResourceDictionary resourceDictionary;
		ResourceDictionary resources = (resourceDictionary = new ResourceDictionary());
		context.PushParent(resourceDictionary);
		resourceDictionary.AddDeferred(typeof(ColorSpectrum), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_64.Build, context));
		resourceDictionary.AddDeferred(typeof(ColorPicker), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_66.Build, context));
		resourceDictionary.AddDeferred("AccentColorConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_73.Build, context));
		resourceDictionary.Add("ColorPreviewerAccentSectionWidth", 80.0);
		resourceDictionary.Add("ColorPreviewerAccentSectionHeight", 40.0);
		resourceDictionary.AddDeferred(typeof(ColorPreviewer), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_74.Build, context));
		resourceDictionary.Add("ColorSliderSize", 20.0);
		resourceDictionary.Add("ColorSliderTrackSize", 20.0);
		resourceDictionary.Add("ColorSliderCornerRadius", new CornerRadius(10.0, 10.0, 10.0, 10.0));
		resourceDictionary.Add("ColorSliderTrackCornerRadius", new CornerRadius(10.0, 10.0, 10.0, 10.0));
		resourceDictionary.AddDeferred("ColorSliderThumbTheme", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_77.Build, context));
		resourceDictionary.AddDeferred(typeof(ColorSlider), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_79.Build, context));
		resourceDictionary.AddDeferred("ContrastBrushConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_88.Build, context));
		resourceDictionary.AddDeferred("ColorToDisplayNameConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_89.Build, context));
		resourceDictionary.AddDeferred("DoNothingForNullConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_90.Build, context));
		resourceDictionary.AddDeferred("ColorViewComponentNumberFormat", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_91.Build, context));
		resourceDictionary.Add("ColorViewTabStripHeight", 48.0);
		resourceDictionary.Add("ColorViewComponentLabelWidth", 30.0);
		resourceDictionary.Add("ColorViewComponentTextInputWidth", 80.0);
		resourceDictionary.AddDeferred("ColorViewSpectrumIconGeometry", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_92.Build, context));
		resourceDictionary.AddDeferred("ColorViewPaletteIconGeometry", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_93.Build, context));
		resourceDictionary.AddDeferred("ColorViewComponentsIconGeometry", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_94.Build, context));
		resourceDictionary.Add("ColorViewTabBackgroundCornerRadius", new CornerRadius(3.0, 3.0, 3.0, 3.0));
		resourceDictionary.AddDeferred("ColorViewPaletteListBoxTheme", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_95.Build, context));
		resourceDictionary.AddDeferred("ColorViewPaletteListBoxItemTheme", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_97.Build, context));
		resourceDictionary.AddDeferred("ColorViewColorModelRadioButtonTheme", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_100.Build, context));
		resourceDictionary.AddDeferred("ColorViewTabItemTheme", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_103.Build, context));
		resourceDictionary.AddDeferred(typeof(ColorView), XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_106.Build, context));
		resourceDictionary.AddDeferred("ColorControlCheckeredBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_113.Build, context));
		resourceDictionary.AddDeferred("ColorControlLightSelectorBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_114.Build, context));
		resourceDictionary.AddDeferred("ColorControlDarkSelectorBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_115.Build, context));
		resourceDictionary.AddDeferred("ColorViewContentBackgroundBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_116.Build, context));
		resourceDictionary.AddDeferred("ColorViewContentBorderBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_117.Build, context));
		resourceDictionary.AddDeferred("ColorViewTabBorderBrush", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_118.Build, context));
		resourceDictionary.AddDeferred("EnumToBoolConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_119.Build, context));
		resourceDictionary.AddDeferred("ToBrushConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_120.Build, context));
		resourceDictionary.AddDeferred("LeftCornerRadiusFilterConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_121.Build, context));
		resourceDictionary.AddDeferred("RightCornerRadiusFilterConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_122.Build, context));
		resourceDictionary.AddDeferred("TopCornerRadiusFilterConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_123.Build, context));
		resourceDictionary.AddDeferred("BottomCornerRadiusFilterConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_124.Build, context));
		resourceDictionary.AddDeferred("TopLeftCornerRadiusConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_125.Build, context));
		resourceDictionary.AddDeferred("BottomRightCornerRadiusConverter", XamlIlRuntimeHelpers.DeferredTransformationFactoryV2<object>(XamlClosure_126.Build, context));
		context.PopParent();
		styles.Resources = resources;
		context.PopParent();
		if (styles2 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	public static Styles Build_003A_002FThemes_002FSimple_002FSimple_002Examl(IServiceProvider P_0)
	{
		Styles styles = new Styles();
		Populate_003A_002FThemes_002FSimple_002FSimple_002Examl(P_0, styles);
		return styles;
	}
}
