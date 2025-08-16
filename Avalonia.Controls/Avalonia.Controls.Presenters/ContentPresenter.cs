using System;
using Avalonia.Collections;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Templates;
using Avalonia.Controls.Utils;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Utilities;

namespace Avalonia.Controls.Presenters;

[PseudoClasses(new string[] { ":empty" })]
public class ContentPresenter : Control
{
	public static readonly StyledProperty<IBrush?> BackgroundProperty;

	public static readonly StyledProperty<IBrush?> BorderBrushProperty;

	public static readonly StyledProperty<Thickness> BorderThicknessProperty;

	public static readonly StyledProperty<CornerRadius> CornerRadiusProperty;

	public static readonly StyledProperty<BoxShadows> BoxShadowProperty;

	public static readonly StyledProperty<IBrush?> ForegroundProperty;

	public static readonly StyledProperty<FontFamily> FontFamilyProperty;

	public static readonly StyledProperty<double> FontSizeProperty;

	public static readonly StyledProperty<FontStyle> FontStyleProperty;

	public static readonly StyledProperty<FontWeight> FontWeightProperty;

	public static readonly StyledProperty<FontStretch> FontStretchProperty;

	public static readonly StyledProperty<TextAlignment> TextAlignmentProperty;

	public static readonly StyledProperty<TextWrapping> TextWrappingProperty;

	public static readonly StyledProperty<TextTrimming> TextTrimmingProperty;

	public static readonly StyledProperty<double> LineHeightProperty;

	public static readonly StyledProperty<int> MaxLinesProperty;

	public static readonly DirectProperty<ContentPresenter, Control?> ChildProperty;

	public static readonly StyledProperty<object?> ContentProperty;

	public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty;

	public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty;

	public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty;

	public static readonly StyledProperty<Thickness> PaddingProperty;

	public static readonly StyledProperty<bool> RecognizesAccessKeyProperty;

	private Control? _child;

	private bool _createdChild;

	private IRecyclingDataTemplate? _recyclingDataTemplate;

	private readonly BorderRenderHelper _borderRenderer = new BorderRenderHelper();

	private Thickness? _layoutThickness;

	private double _scale;

	public IBrush? Background
	{
		get
		{
			return GetValue(BackgroundProperty);
		}
		set
		{
			SetValue(BackgroundProperty, value);
		}
	}

	public IBrush? BorderBrush
	{
		get
		{
			return GetValue(BorderBrushProperty);
		}
		set
		{
			SetValue(BorderBrushProperty, value);
		}
	}

	public Thickness BorderThickness
	{
		get
		{
			return GetValue(BorderThicknessProperty);
		}
		set
		{
			SetValue(BorderThicknessProperty, value);
		}
	}

	public CornerRadius CornerRadius
	{
		get
		{
			return GetValue(CornerRadiusProperty);
		}
		set
		{
			SetValue(CornerRadiusProperty, value);
		}
	}

	public BoxShadows BoxShadow
	{
		get
		{
			return GetValue(BoxShadowProperty);
		}
		set
		{
			SetValue(BoxShadowProperty, value);
		}
	}

	public IBrush? Foreground
	{
		get
		{
			return GetValue(ForegroundProperty);
		}
		set
		{
			SetValue(ForegroundProperty, value);
		}
	}

	public FontFamily FontFamily
	{
		get
		{
			return GetValue(FontFamilyProperty);
		}
		set
		{
			SetValue(FontFamilyProperty, value);
		}
	}

	public double FontSize
	{
		get
		{
			return GetValue(FontSizeProperty);
		}
		set
		{
			SetValue(FontSizeProperty, value);
		}
	}

	public FontStyle FontStyle
	{
		get
		{
			return GetValue(FontStyleProperty);
		}
		set
		{
			SetValue(FontStyleProperty, value);
		}
	}

	public FontWeight FontWeight
	{
		get
		{
			return GetValue(FontWeightProperty);
		}
		set
		{
			SetValue(FontWeightProperty, value);
		}
	}

	public FontStretch FontStretch
	{
		get
		{
			return GetValue(FontStretchProperty);
		}
		set
		{
			SetValue(FontStretchProperty, value);
		}
	}

	public TextAlignment TextAlignment
	{
		get
		{
			return GetValue(TextAlignmentProperty);
		}
		set
		{
			SetValue(TextAlignmentProperty, value);
		}
	}

	public TextWrapping TextWrapping
	{
		get
		{
			return GetValue(TextWrappingProperty);
		}
		set
		{
			SetValue(TextWrappingProperty, value);
		}
	}

	public TextTrimming TextTrimming
	{
		get
		{
			return GetValue(TextTrimmingProperty);
		}
		set
		{
			SetValue(TextTrimmingProperty, value);
		}
	}

	public double LineHeight
	{
		get
		{
			return GetValue(LineHeightProperty);
		}
		set
		{
			SetValue(LineHeightProperty, value);
		}
	}

	public int MaxLines
	{
		get
		{
			return GetValue(MaxLinesProperty);
		}
		set
		{
			SetValue(MaxLinesProperty, value);
		}
	}

	public Control? Child
	{
		get
		{
			return _child;
		}
		private set
		{
			SetAndRaise(ChildProperty, ref _child, value);
		}
	}

	[DependsOn("ContentTemplate")]
	public object? Content
	{
		get
		{
			return GetValue(ContentProperty);
		}
		set
		{
			SetValue(ContentProperty, value);
		}
	}

	public IDataTemplate? ContentTemplate
	{
		get
		{
			return GetValue(ContentTemplateProperty);
		}
		set
		{
			SetValue(ContentTemplateProperty, value);
		}
	}

	public HorizontalAlignment HorizontalContentAlignment
	{
		get
		{
			return GetValue(HorizontalContentAlignmentProperty);
		}
		set
		{
			SetValue(HorizontalContentAlignmentProperty, value);
		}
	}

	public VerticalAlignment VerticalContentAlignment
	{
		get
		{
			return GetValue(VerticalContentAlignmentProperty);
		}
		set
		{
			SetValue(VerticalContentAlignmentProperty, value);
		}
	}

	public Thickness Padding
	{
		get
		{
			return GetValue(PaddingProperty);
		}
		set
		{
			SetValue(PaddingProperty, value);
		}
	}

	public bool RecognizesAccessKey
	{
		get
		{
			return GetValue(RecognizesAccessKeyProperty);
		}
		set
		{
			SetValue(RecognizesAccessKeyProperty, value);
		}
	}

	internal IContentPresenterHost? Host { get; private set; }

	private Thickness LayoutThickness
	{
		get
		{
			VerifyScale();
			if (!_layoutThickness.HasValue)
			{
				Thickness thickness = BorderThickness;
				if (base.UseLayoutRounding)
				{
					thickness = LayoutHelper.RoundLayoutThickness(thickness, _scale, _scale);
				}
				_layoutThickness = thickness;
			}
			return _layoutThickness.Value;
		}
	}

	static ContentPresenter()
	{
		BackgroundProperty = Border.BackgroundProperty.AddOwner<ContentPresenter>();
		BorderBrushProperty = Border.BorderBrushProperty.AddOwner<ContentPresenter>();
		BorderThicknessProperty = Border.BorderThicknessProperty.AddOwner<ContentPresenter>();
		CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner<ContentPresenter>();
		BoxShadowProperty = Border.BoxShadowProperty.AddOwner<ContentPresenter>();
		ForegroundProperty = TextElement.ForegroundProperty.AddOwner<ContentPresenter>();
		FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner<ContentPresenter>();
		FontSizeProperty = TextElement.FontSizeProperty.AddOwner<ContentPresenter>();
		FontStyleProperty = TextElement.FontStyleProperty.AddOwner<ContentPresenter>();
		FontWeightProperty = TextElement.FontWeightProperty.AddOwner<ContentPresenter>();
		FontStretchProperty = TextElement.FontStretchProperty.AddOwner<ContentPresenter>();
		TextAlignmentProperty = TextBlock.TextAlignmentProperty.AddOwner<ContentPresenter>();
		TextWrappingProperty = TextBlock.TextWrappingProperty.AddOwner<ContentPresenter>();
		TextTrimmingProperty = TextBlock.TextTrimmingProperty.AddOwner<ContentPresenter>();
		LineHeightProperty = TextBlock.LineHeightProperty.AddOwner<ContentPresenter>();
		MaxLinesProperty = TextBlock.MaxLinesProperty.AddOwner<ContentPresenter>();
		ChildProperty = AvaloniaProperty.RegisterDirect("Child", (ContentPresenter o) => o.Child);
		ContentProperty = ContentControl.ContentProperty.AddOwner<ContentPresenter>();
		ContentTemplateProperty = ContentControl.ContentTemplateProperty.AddOwner<ContentPresenter>();
		HorizontalContentAlignmentProperty = ContentControl.HorizontalContentAlignmentProperty.AddOwner<ContentPresenter>();
		VerticalContentAlignmentProperty = ContentControl.VerticalContentAlignmentProperty.AddOwner<ContentPresenter>();
		PaddingProperty = Decorator.PaddingProperty.AddOwner<ContentPresenter>();
		RecognizesAccessKeyProperty = AvaloniaProperty.Register<ContentPresenter, bool>("RecognizesAccessKey", defaultValue: false);
		Visual.AffectsRender<ContentPresenter>(new AvaloniaProperty[4] { BackgroundProperty, BorderBrushProperty, BorderThicknessProperty, CornerRadiusProperty });
		Layoutable.AffectsArrange<ContentPresenter>(new AvaloniaProperty[2] { HorizontalContentAlignmentProperty, VerticalContentAlignmentProperty });
		Layoutable.AffectsMeasure<ContentPresenter>(new AvaloniaProperty[2] { BorderThicknessProperty, PaddingProperty });
	}

	public ContentPresenter()
	{
		UpdatePseudoClasses();
	}

	public sealed override void ApplyTemplate()
	{
		if (!_createdChild && ((ILogical)this).IsAttachedToLogicalTree)
		{
			UpdateChild();
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		switch (change.Property.Name)
		{
		case "Content":
		case "ContentTemplate":
			ContentChanged(change);
			break;
		case "TemplatedParent":
			TemplatedParentChanged(change);
			break;
		case "UseLayoutRounding":
		case "BorderThickness":
			_layoutThickness = null;
			break;
		}
	}

	public void UpdateChild()
	{
		object content = Content;
		UpdateChild(content);
	}

	private void UpdateChild(object? content)
	{
		IDataTemplate contentTemplate = ContentTemplate;
		Control child = Child;
		Control control = CreateChild(content, child, contentTemplate);
		IAvaloniaList<ILogical> effectiveLogicalChildren = GetEffectiveLogicalChildren();
		if (control != child && child != null)
		{
			base.VisualChildren.Remove(child);
			effectiveLogicalChildren.Remove(child);
			((ISetInheritanceParent)child).SetParent((AvaloniaObject?)child.Parent);
		}
		if (contentTemplate != null || !(content is Control))
		{
			base.DataContext = content;
		}
		else
		{
			ClearValue(StyledElement.DataContextProperty);
		}
		if (control == null)
		{
			Child = null;
		}
		else if (control != child)
		{
			((ISetInheritanceParent)control).SetParent((AvaloniaObject?)this);
			Child = control;
			if (!effectiveLogicalChildren.Contains(control))
			{
				effectiveLogicalChildren.Add(control);
			}
			base.VisualChildren.Add(control);
		}
		_createdChild = true;
	}

	private IAvaloniaList<ILogical> GetEffectiveLogicalChildren()
	{
		return Host?.LogicalChildren ?? base.LogicalChildren;
	}

	protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		base.OnAttachedToLogicalTree(e);
		_recyclingDataTemplate = null;
		_createdChild = false;
		InvalidateMeasure();
	}

	private void VerifyScale()
	{
		double layoutScale = LayoutHelper.GetLayoutScale(this);
		if (!MathUtilities.AreClose(layoutScale, _scale))
		{
			_scale = layoutScale;
			_layoutThickness = null;
		}
	}

	public sealed override void Render(DrawingContext context)
	{
		_borderRenderer.Render(context, base.Bounds.Size, LayoutThickness, CornerRadius, Background, BorderBrush, BoxShadow);
	}

	private Control? CreateChild(object? content, Control? oldChild, IDataTemplate? template)
	{
		Control control = content as Control;
		if ((control == null && (content != null || template != null)) || (control != null && template != null))
		{
			IDataTemplate dataTemplate = this.FindDataTemplate(content, template) ?? (RecognizesAccessKey ? FuncDataTemplate.Access : FuncDataTemplate.Default);
			if (dataTemplate is IRecyclingDataTemplate recyclingDataTemplate)
			{
				Control existing = ((recyclingDataTemplate == _recyclingDataTemplate) ? oldChild : null);
				control = recyclingDataTemplate.Build(content, existing);
				_recyclingDataTemplate = recyclingDataTemplate;
			}
			else
			{
				control = dataTemplate.Build(content);
				_recyclingDataTemplate = null;
			}
		}
		else
		{
			_recyclingDataTemplate = null;
		}
		return control;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		return LayoutHelper.MeasureChild(Child, availableSize, Padding, BorderThickness);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		return ArrangeOverrideImpl(finalSize, default(Vector));
	}

	internal Size ArrangeOverrideImpl(Size finalSize, Vector offset)
	{
		if (Child == null)
		{
			return finalSize;
		}
		bool useLayoutRounding = base.UseLayoutRounding;
		double layoutScale = LayoutHelper.GetLayoutScale(this);
		Thickness thickness = Padding;
		Thickness thickness2 = BorderThickness;
		if (useLayoutRounding)
		{
			thickness = LayoutHelper.RoundLayoutThickness(thickness, layoutScale, layoutScale);
			thickness2 = LayoutHelper.RoundLayoutThickness(thickness2, layoutScale, layoutScale);
		}
		thickness += thickness2;
		HorizontalAlignment horizontalContentAlignment = HorizontalContentAlignment;
		VerticalAlignment verticalContentAlignment = VerticalContentAlignment;
		Size size = finalSize;
		Size size2 = size;
		double num = offset.X;
		double num2 = offset.Y;
		if (horizontalContentAlignment != 0)
		{
			size2 = size2.WithWidth(Math.Min(size2.Width, base.DesiredSize.Width));
		}
		if (verticalContentAlignment != 0)
		{
			size2 = size2.WithHeight(Math.Min(size2.Height, base.DesiredSize.Height));
		}
		if (useLayoutRounding)
		{
			size2 = LayoutHelper.RoundLayoutSizeUp(size2, layoutScale, layoutScale);
			size = LayoutHelper.RoundLayoutSizeUp(size, layoutScale, layoutScale);
		}
		switch (horizontalContentAlignment)
		{
		case HorizontalAlignment.Center:
			num += (size.Width - size2.Width) / 2.0;
			break;
		case HorizontalAlignment.Right:
			num += size.Width - size2.Width;
			break;
		}
		switch (verticalContentAlignment)
		{
		case VerticalAlignment.Center:
			num2 += (size.Height - size2.Height) / 2.0;
			break;
		case VerticalAlignment.Bottom:
			num2 += size.Height - size2.Height;
			break;
		}
		if (useLayoutRounding)
		{
			num = LayoutHelper.RoundLayoutValue(num, layoutScale);
			num2 = LayoutHelper.RoundLayoutValue(num2, layoutScale);
		}
		Rect rect = new Rect(num, num2, size2.Width, size2.Height).Deflate(thickness);
		Child.Arrange(rect);
		return finalSize;
	}

	private void ContentChanged(AvaloniaPropertyChangedEventArgs e)
	{
		_createdChild = false;
		if (((ILogical)this).IsAttachedToLogicalTree)
		{
			if (e.Property.Name == "Content")
			{
				UpdateChild(e.NewValue);
			}
			else
			{
				UpdateChild();
			}
		}
		else if (Child != null)
		{
			base.VisualChildren.Remove(Child);
			GetEffectiveLogicalChildren().Remove(Child);
			((ISetInheritanceParent)Child).SetParent((AvaloniaObject?)Child.Parent);
			Child = null;
			_recyclingDataTemplate = null;
		}
		UpdatePseudoClasses();
		InvalidateMeasure();
	}

	private void UpdatePseudoClasses()
	{
		base.PseudoClasses.Set(":empty", Content == null);
	}

	private void TemplatedParentChanged(AvaloniaPropertyChangedEventArgs e)
	{
		IContentPresenterHost contentPresenterHost = e.NewValue as IContentPresenterHost;
		Host = ((contentPresenterHost != null && contentPresenterHost.RegisterContentPresenter(this)) ? contentPresenterHost : null);
	}
}
