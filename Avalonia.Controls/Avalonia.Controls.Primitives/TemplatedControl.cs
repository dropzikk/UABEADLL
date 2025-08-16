using System;
using Avalonia.Collections;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace Avalonia.Controls.Primitives;

public class TemplatedControl : Control
{
	public static readonly StyledProperty<IBrush?> BackgroundProperty;

	public static readonly StyledProperty<IBrush?> BorderBrushProperty;

	public static readonly StyledProperty<Thickness> BorderThicknessProperty;

	public static readonly StyledProperty<CornerRadius> CornerRadiusProperty;

	public static readonly StyledProperty<FontFamily> FontFamilyProperty;

	public static readonly StyledProperty<double> FontSizeProperty;

	public static readonly StyledProperty<FontStyle> FontStyleProperty;

	public static readonly StyledProperty<FontWeight> FontWeightProperty;

	public static readonly StyledProperty<FontStretch> FontStretchProperty;

	public static readonly StyledProperty<IBrush?> ForegroundProperty;

	public static readonly StyledProperty<Thickness> PaddingProperty;

	public static readonly StyledProperty<IControlTemplate?> TemplateProperty;

	public static readonly AttachedProperty<bool> IsTemplateFocusTargetProperty;

	public static readonly RoutedEvent<TemplateAppliedEventArgs> TemplateAppliedEvent;

	private IControlTemplate? _appliedTemplate;

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

	public IControlTemplate? Template
	{
		get
		{
			return GetValue(TemplateProperty);
		}
		set
		{
			SetValue(TemplateProperty, value);
		}
	}

	public event EventHandler<TemplateAppliedEventArgs>? TemplateApplied
	{
		add
		{
			AddHandler(TemplateAppliedEvent, value);
		}
		remove
		{
			RemoveHandler(TemplateAppliedEvent, value);
		}
	}

	static TemplatedControl()
	{
		BackgroundProperty = Border.BackgroundProperty.AddOwner<TemplatedControl>();
		BorderBrushProperty = Border.BorderBrushProperty.AddOwner<TemplatedControl>();
		BorderThicknessProperty = Border.BorderThicknessProperty.AddOwner<TemplatedControl>();
		CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner<TemplatedControl>();
		FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner<TemplatedControl>();
		FontSizeProperty = TextElement.FontSizeProperty.AddOwner<TemplatedControl>();
		FontStyleProperty = TextElement.FontStyleProperty.AddOwner<TemplatedControl>();
		FontWeightProperty = TextElement.FontWeightProperty.AddOwner<TemplatedControl>();
		FontStretchProperty = TextElement.FontStretchProperty.AddOwner<TemplatedControl>();
		ForegroundProperty = TextElement.ForegroundProperty.AddOwner<TemplatedControl>();
		PaddingProperty = Decorator.PaddingProperty.AddOwner<TemplatedControl>();
		TemplateProperty = AvaloniaProperty.Register<TemplatedControl, IControlTemplate>("Template");
		IsTemplateFocusTargetProperty = AvaloniaProperty.RegisterAttached<TemplatedControl, Control, bool>("IsTemplateFocusTarget", defaultValue: false);
		TemplateAppliedEvent = RoutedEvent.Register<TemplatedControl, TemplateAppliedEventArgs>("TemplateApplied", RoutingStrategies.Direct);
		Visual.ClipToBoundsProperty.OverrideDefaultValue<TemplatedControl>(defaultValue: true);
		TemplateProperty.Changed.AddClassHandler(delegate(TemplatedControl x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnTemplateChanged(e);
		});
	}

	public static bool GetIsTemplateFocusTarget(Control control)
	{
		return control.GetValue(IsTemplateFocusTargetProperty);
	}

	public static void SetIsTemplateFocusTarget(Control control, bool value)
	{
		control.SetValue(IsTemplateFocusTargetProperty, value);
	}

	public sealed override void ApplyTemplate()
	{
		IControlTemplate template = Template;
		if (_appliedTemplate == template || (template == null && !((ILogical)this).IsAttachedToLogicalTree))
		{
			return;
		}
		if (base.VisualChildren.Count > 0)
		{
			foreach (Control templateChild in this.GetTemplateChildren())
			{
				templateChild.TemplatedParent = null;
				((ISetLogicalParent)templateChild).SetParent((ILogical?)null);
			}
			base.VisualChildren.Clear();
		}
		if (template != null)
		{
			Logger.TryGet(LogEventLevel.Verbose, "Control")?.Log(this, "Creating control template");
			TemplateResult<Control> templateResult = template.Build(this);
			if (templateResult != null)
			{
				var (control2, nameScope2) = templateResult;
				ApplyTemplatedParent(control2, this);
				((ISetLogicalParent)control2).SetParent((ILogical?)this);
				base.VisualChildren.Add(control2);
				TemplateAppliedEventArgs e = new TemplateAppliedEventArgs(nameScope2);
				OnApplyTemplate(e);
				RaiseEvent(e);
			}
		}
		_appliedTemplate = template;
	}

	protected override Control GetTemplateFocusTarget()
	{
		foreach (Control templateChild in this.GetTemplateChildren())
		{
			if (GetIsTemplateFocusTarget(templateChild))
			{
				return templateChild;
			}
		}
		return this;
	}

	internal sealed override void NotifyChildResourcesChanged(ResourcesChangedEventArgs e)
	{
		int count = base.VisualChildren.Count;
		for (int i = 0; i < count; i++)
		{
			((ILogical)base.VisualChildren[i])?.NotifyResourcesChanged(e);
		}
		base.NotifyChildResourcesChanged(e);
	}

	protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		if (base.VisualChildren.Count > 0)
		{
			((ILogical)base.VisualChildren[0]).NotifyAttachedToLogicalTree(e);
		}
		base.OnAttachedToLogicalTree(e);
	}

	protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		if (base.VisualChildren.Count > 0)
		{
			((ILogical)base.VisualChildren[0]).NotifyDetachedFromLogicalTree(e);
		}
		base.OnDetachedFromLogicalTree(e);
	}

	protected virtual void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
	}

	protected virtual void OnTemplateChanged(AvaloniaPropertyChangedEventArgs e)
	{
		InvalidateMeasure();
	}

	internal static void ApplyTemplatedParent(StyledElement control, AvaloniaObject? templatedParent)
	{
		control.TemplatedParent = templatedParent;
		IAvaloniaList<ILogical> logicalChildren = control.LogicalChildren;
		int count = logicalChildren.Count;
		for (int i = 0; i < count; i++)
		{
			if (logicalChildren[i] is StyledElement { TemplatedParent: null } styledElement)
			{
				ApplyTemplatedParent(styledElement, templatedParent);
			}
		}
	}

	private protected override void OnControlThemeChanged()
	{
		base.OnControlThemeChanged();
		int count = base.VisualChildren.Count;
		for (int i = 0; i < count; i++)
		{
			StyledElement styledElement = base.VisualChildren[i];
			if (styledElement != null && styledElement.TemplatedParent == this)
			{
				styledElement.OnTemplatedParentControlThemeChanged();
			}
		}
	}
}
