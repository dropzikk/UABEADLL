using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Animation;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Diagnostics;
using Avalonia.LogicalTree;
using Avalonia.PropertyStore;
using Avalonia.Styling;

namespace Avalonia;

public class StyledElement : Animatable, IDataContextProvider, ILogical, IThemeVariantHost, IResourceHost, IResourceNode, IStyleHost, ISetLogicalParent, ISetInheritanceParent, ISupportInitialize, IStyleable, INamed
{
	public static readonly StyledProperty<object?> DataContextProperty;

	public static readonly DirectProperty<StyledElement, string?> NameProperty;

	public static readonly DirectProperty<StyledElement, StyledElement?> ParentProperty;

	public static readonly DirectProperty<StyledElement, AvaloniaObject?> TemplatedParentProperty;

	public static readonly StyledProperty<ControlTheme?> ThemeProperty;

	private static readonly ControlTheme s_invalidTheme;

	private int _initCount;

	private string? _name;

	private Classes? _classes;

	private ILogicalRoot? _logicalRoot;

	private IAvaloniaList<ILogical>? _logicalChildren;

	private IResourceDictionary? _resources;

	private Styles? _styles;

	private bool _stylesApplied;

	private bool _themeApplied;

	private bool _templatedParentThemeApplied;

	private AvaloniaObject? _templatedParent;

	private bool _dataContextUpdating;

	private ControlTheme? _implicitTheme;

	public string? Name
	{
		get
		{
			return _name;
		}
		set
		{
			if (_stylesApplied)
			{
				throw new InvalidOperationException("Cannot set Name : styled element already styled.");
			}
			_name = value;
		}
	}

	public Classes Classes => _classes ?? (_classes = new Classes());

	public object? DataContext
	{
		get
		{
			return GetValue(DataContextProperty);
		}
		set
		{
			SetValue(DataContextProperty, value);
		}
	}

	public bool IsInitialized { get; private set; }

	public Styles Styles => _styles ?? (_styles = new Styles(this));

	public Type StyleKey => StyleKeyOverride;

	public IResourceDictionary Resources
	{
		get
		{
			return _resources ?? (_resources = new ResourceDictionary(this));
		}
		set
		{
			value = value ?? throw new ArgumentNullException("value");
			_resources?.RemoveOwner(this);
			_resources = value;
			_resources.AddOwner(this);
		}
	}

	public AvaloniaObject? TemplatedParent
	{
		get
		{
			return _templatedParent;
		}
		internal set
		{
			SetAndRaise(TemplatedParentProperty, ref _templatedParent, value);
		}
	}

	public ControlTheme? Theme
	{
		get
		{
			return GetValue(ThemeProperty);
		}
		set
		{
			SetValue(ThemeProperty, value);
		}
	}

	protected internal IAvaloniaList<ILogical> LogicalChildren
	{
		get
		{
			if (_logicalChildren == null)
			{
				AvaloniaList<ILogical> avaloniaList = new AvaloniaList<ILogical>
				{
					ResetBehavior = ResetBehavior.Remove,
					Validate = delegate(ILogical logical)
					{
						ValidateLogicalChild(logical);
					}
				};
				avaloniaList.CollectionChanged += LogicalChildrenCollectionChanged;
				_logicalChildren = avaloniaList;
			}
			return _logicalChildren;
		}
	}

	protected IPseudoClasses PseudoClasses => Classes;

	protected virtual Type StyleKeyOverride => GetType();

	bool ILogical.IsAttachedToLogicalTree => _logicalRoot != null;

	public StyledElement? Parent { get; private set; }

	public ThemeVariant ActualThemeVariant => GetValue(ThemeVariant.ActualThemeVariantProperty);

	ILogical? ILogical.LogicalParent => Parent;

	IAvaloniaReadOnlyList<ILogical> ILogical.LogicalChildren => LogicalChildren;

	bool IResourceNode.HasResources
	{
		get
		{
			IResourceDictionary? resources = _resources;
			if (resources == null || !resources.HasResources)
			{
				return ((IResourceNode)_styles)?.HasResources ?? false;
			}
			return true;
		}
	}

	IAvaloniaReadOnlyList<string> IStyleable.Classes => Classes;

	bool IStyleHost.IsStylesInitialized => _styles != null;

	IStyleHost? IStyleHost.StylingParent => (IStyleHost)base.InheritanceParent;

	public event EventHandler<LogicalTreeAttachmentEventArgs>? AttachedToLogicalTree;

	public event EventHandler<LogicalTreeAttachmentEventArgs>? DetachedFromLogicalTree;

	public event EventHandler? DataContextChanged;

	public event EventHandler? Initialized;

	public event EventHandler<ResourcesChangedEventArgs>? ResourcesChanged;

	public event EventHandler? ActualThemeVariantChanged;

	static StyledElement()
	{
		DataContextProperty = AvaloniaProperty.Register<StyledElement, object>("DataContext", null, inherits: true, BindingMode.OneWay, null, null, enableDataValidation: false, DataContextNotifying);
		NameProperty = AvaloniaProperty.RegisterDirect("Name", (StyledElement o) => o.Name, delegate(StyledElement o, string? v)
		{
			o.Name = v;
		});
		ParentProperty = AvaloniaProperty.RegisterDirect("Parent", (StyledElement o) => o.Parent);
		TemplatedParentProperty = AvaloniaProperty.RegisterDirect("TemplatedParent", (StyledElement o) => o.TemplatedParent);
		ThemeProperty = AvaloniaProperty.Register<StyledElement, ControlTheme>("Theme");
		s_invalidTheme = new ControlTheme();
		DataContextProperty.Changed.AddClassHandler(delegate(StyledElement x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnDataContextChangedCore(e);
		});
	}

	public StyledElement()
	{
		_logicalRoot = this as ILogicalRoot;
	}

	public virtual void BeginInit()
	{
		_initCount++;
	}

	public virtual void EndInit()
	{
		if (_initCount == 0)
		{
			throw new InvalidOperationException("BeginInit was not called.");
		}
		if (--_initCount == 0 && _logicalRoot != null)
		{
			ApplyStyling();
			InitializeIfNeeded();
		}
	}

	public bool ApplyStyling()
	{
		if (_initCount == 0 && (!_stylesApplied || !_themeApplied || !_templatedParentThemeApplied))
		{
			GetValueStore().BeginStyling();
			try
			{
				if (!_themeApplied)
				{
					ApplyControlTheme();
					_themeApplied = true;
				}
				if (!_templatedParentThemeApplied)
				{
					ApplyTemplatedParentControlTheme();
					_templatedParentThemeApplied = true;
				}
				if (!_stylesApplied)
				{
					ApplyStyles(this);
					_stylesApplied = true;
				}
			}
			finally
			{
				GetValueStore().EndStyling();
			}
		}
		return _stylesApplied;
	}

	protected void InitializeIfNeeded()
	{
		if (_initCount == 0 && !IsInitialized)
		{
			IsInitialized = true;
			OnInitialized();
			this.Initialized?.Invoke(this, EventArgs.Empty);
		}
	}

	internal StyleDiagnostics GetStyleDiagnosticsInternal()
	{
		List<AppliedStyle> list = new List<AppliedStyle>();
		foreach (ValueFrame frame in GetValueStore().Frames)
		{
			if (frame is IStyleInstance instance)
			{
				list.Add(new AppliedStyle(instance));
			}
		}
		return new StyleDiagnostics(list);
	}

	void ILogical.NotifyAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		OnAttachedToLogicalTreeCore(e);
	}

	void ILogical.NotifyDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		OnDetachedFromLogicalTreeCore(e);
	}

	void ILogical.NotifyResourcesChanged(ResourcesChangedEventArgs e)
	{
		NotifyResourcesChanged(e);
	}

	void IResourceHost.NotifyHostedResourcesChanged(ResourcesChangedEventArgs e)
	{
		NotifyResourcesChanged(e);
	}

	public bool TryGetResource(object key, ThemeVariant? theme, out object? value)
	{
		value = null;
		IResourceDictionary? resources = _resources;
		if (resources == null || !resources.TryGetResource(key, theme, out value))
		{
			return _styles?.TryGetResource(key, theme, out value) ?? false;
		}
		return true;
	}

	void ISetLogicalParent.SetParent(ILogical? parent)
	{
		StyledElement parent2 = Parent;
		if (parent != parent2)
		{
			if (parent2 != null && parent != null)
			{
				throw new InvalidOperationException("The Control already has a parent.");
			}
			if (base.InheritanceParent == null || parent == null)
			{
				base.InheritanceParent = parent as AvaloniaObject;
			}
			Parent = (StyledElement)parent;
			if (_logicalRoot != null)
			{
				LogicalTreeAttachmentEventArgs e = new LogicalTreeAttachmentEventArgs(_logicalRoot, this, parent2);
				OnDetachedFromLogicalTreeCore(e);
			}
			ILogicalRoot logicalRoot = FindLogicalRoot(this);
			if (logicalRoot != null)
			{
				LogicalTreeAttachmentEventArgs e2 = new LogicalTreeAttachmentEventArgs(logicalRoot, this, parent);
				OnAttachedToLogicalTreeCore(e2);
			}
			else if (parent == null)
			{
				NotifyResourcesChanged();
			}
			RaisePropertyChanged(ParentProperty, parent2, Parent);
		}
	}

	void ISetInheritanceParent.SetParent(AvaloniaObject? parent)
	{
		base.InheritanceParent = parent;
	}

	void IStyleHost.StylesAdded(IReadOnlyList<IStyle> styles)
	{
		if (HasSettersOrAnimations(styles))
		{
			InvalidateStyles(recurse: true);
		}
	}

	void IStyleHost.StylesRemoved(IReadOnlyList<IStyle> styles)
	{
		IReadOnlyList<Style> readOnlyList = FlattenStyles(styles);
		if (readOnlyList != null)
		{
			DetachStyles(readOnlyList);
		}
	}

	protected virtual void LogicalChildrenCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		switch (e.Action)
		{
		case NotifyCollectionChangedAction.Add:
			SetLogicalParent(e.NewItems);
			break;
		case NotifyCollectionChangedAction.Remove:
			ClearLogicalParent(e.OldItems);
			break;
		case NotifyCollectionChangedAction.Replace:
			ClearLogicalParent(e.OldItems);
			SetLogicalParent(e.NewItems);
			break;
		case NotifyCollectionChangedAction.Reset:
			throw new NotSupportedException("Reset should not be signaled on LogicalChildren collection");
		case NotifyCollectionChangedAction.Move:
			break;
		}
	}

	internal virtual void NotifyChildResourcesChanged(ResourcesChangedEventArgs e)
	{
		if (_logicalChildren == null)
		{
			return;
		}
		int count = _logicalChildren.Count;
		if (count > 0)
		{
			if (e == null)
			{
				e = ResourcesChangedEventArgs.Empty;
			}
			for (int i = 0; i < count; i++)
			{
				_logicalChildren[i].NotifyResourcesChanged(e);
			}
		}
	}

	protected virtual void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
	}

	protected virtual void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
	}

	protected virtual void OnDataContextChanged(EventArgs e)
	{
		this.DataContextChanged?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void OnDataContextBeginUpdate()
	{
	}

	protected virtual void OnDataContextEndUpdate()
	{
	}

	protected virtual void OnInitialized()
	{
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == ThemeProperty)
		{
			OnControlThemeChanged();
		}
		else if (change.Property == ThemeVariant.RequestedThemeVariantProperty)
		{
			ThemeVariant newValue = change.GetNewValue<ThemeVariant>();
			if ((object)newValue != null && newValue != ThemeVariant.Default)
			{
				SetValue(ThemeVariant.ActualThemeVariantProperty, newValue);
			}
			else
			{
				ClearValue(ThemeVariant.ActualThemeVariantProperty);
			}
		}
		else if (change.Property == ThemeVariant.ActualThemeVariantProperty)
		{
			this.ActualThemeVariantChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private protected virtual void OnControlThemeChanged()
	{
		ValueStore valueStore = GetValueStore();
		valueStore.BeginStyling();
		try
		{
			valueStore.RemoveFrames(FrameType.Theme);
		}
		finally
		{
			valueStore.EndStyling();
			_themeApplied = false;
		}
	}

	internal virtual void OnTemplatedParentControlThemeChanged()
	{
		ValueStore valueStore = GetValueStore();
		valueStore.BeginStyling();
		try
		{
			valueStore.RemoveFrames(FrameType.TemplatedParentTheme);
		}
		finally
		{
			valueStore.EndStyling();
			_templatedParentThemeApplied = false;
		}
	}

	internal ControlTheme? GetEffectiveTheme()
	{
		ControlTheme theme = Theme;
		if (theme != null)
		{
			return theme;
		}
		if (_implicitTheme == null)
		{
			Type styleKey = GetStyleKey(this);
			if (this.TryFindResource(styleKey, out object value) && value is ControlTheme implicitTheme)
			{
				_implicitTheme = implicitTheme;
			}
			else
			{
				_implicitTheme = s_invalidTheme;
			}
		}
		if (_implicitTheme != s_invalidTheme)
		{
			return _implicitTheme;
		}
		return null;
	}

	internal virtual void InvalidateStyles(bool recurse)
	{
		ValueStore valueStore = GetValueStore();
		valueStore.BeginStyling();
		try
		{
			valueStore.RemoveFrames(FrameType.Style);
		}
		finally
		{
			valueStore.EndStyling();
		}
		_stylesApplied = false;
		if (!recurse)
		{
			return;
		}
		IReadOnlyList<AvaloniaObject> inheritanceChildren = GetInheritanceChildren();
		if (inheritanceChildren != null)
		{
			int count = inheritanceChildren.Count;
			for (int i = 0; i < count; i++)
			{
				(inheritanceChildren[i] as StyledElement)?.InvalidateStyles(recurse);
			}
		}
	}

	internal static Type GetStyleKey(StyledElement e)
	{
		return ((IStyleable)e).StyleKey;
	}

	private static void DataContextNotifying(AvaloniaObject o, bool updateStarted)
	{
		if (o is StyledElement element)
		{
			DataContextNotifying(element, updateStarted);
		}
	}

	private static void DataContextNotifying(StyledElement element, bool updateStarted)
	{
		if (updateStarted)
		{
			if (element._dataContextUpdating)
			{
				return;
			}
			element._dataContextUpdating = true;
			element.OnDataContextBeginUpdate();
			int count = element.LogicalChildren.Count;
			for (int i = 0; i < count; i++)
			{
				if (element.LogicalChildren[i] is StyledElement styledElement && styledElement.InheritanceParent == element && !styledElement.IsSet(DataContextProperty))
				{
					DataContextNotifying(styledElement, updateStarted);
				}
			}
		}
		else if (element._dataContextUpdating)
		{
			element.OnDataContextEndUpdate();
			element._dataContextUpdating = false;
		}
	}

	private static ILogicalRoot? FindLogicalRoot(IStyleHost? e)
	{
		while (e != null)
		{
			if (e is ILogicalRoot result)
			{
				return result;
			}
			e = e.StylingParent;
		}
		return null;
	}

	private static void ValidateLogicalChild(ILogical c)
	{
		if (c == null)
		{
			throw new ArgumentException("Cannot add null to LogicalChildren.");
		}
	}

	private void ApplyControlTheme()
	{
		ControlTheme effectiveTheme = GetEffectiveTheme();
		if (effectiveTheme != null)
		{
			ApplyControlTheme(effectiveTheme, FrameType.Theme);
		}
	}

	private void ApplyTemplatedParentControlTheme()
	{
		ControlTheme controlTheme = (TemplatedParent as StyledElement)?.GetEffectiveTheme();
		if (controlTheme != null)
		{
			ApplyControlTheme(controlTheme, FrameType.TemplatedParentTheme);
		}
	}

	private void ApplyControlTheme(ControlTheme theme, FrameType type)
	{
		ControlTheme basedOn = theme.BasedOn;
		if (basedOn != null)
		{
			ApplyControlTheme(basedOn, type);
		}
		theme.TryAttach(this, type);
		if (theme.HasChildren)
		{
			IList<IStyle> children = theme.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ApplyStyle(children[i], null, type);
			}
		}
	}

	private void ApplyStyles(IStyleHost host)
	{
		IStyleHost stylingParent = host.StylingParent;
		if (stylingParent != null)
		{
			ApplyStyles(stylingParent);
		}
		if (host.IsStylesInitialized)
		{
			Styles styles = host.Styles;
			for (int i = 0; i < styles.Count; i++)
			{
				ApplyStyle(styles[i], host, FrameType.Style);
			}
		}
	}

	private void ApplyStyle(IStyle style, IStyleHost? host, FrameType type)
	{
		if (style is Style style2)
		{
			style2.TryAttach(this, host, type);
		}
		IReadOnlyList<IStyle> children = style.Children;
		for (int i = 0; i < children.Count; i++)
		{
			ApplyStyle(children[i], host, type);
		}
	}

	private void ReevaluateImplicitTheme()
	{
		if (Theme == null)
		{
			ControlTheme controlTheme = ((_implicitTheme == s_invalidTheme) ? null : _implicitTheme);
			_implicitTheme = null;
			GetEffectiveTheme();
			if (((_implicitTheme == s_invalidTheme) ? null : _implicitTheme) != controlTheme)
			{
				OnControlThemeChanged();
				_themeApplied = false;
			}
		}
	}

	private void OnAttachedToLogicalTreeCore(LogicalTreeAttachmentEventArgs e)
	{
		if (this.GetLogicalParent() == null && !(this is ILogicalRoot))
		{
			throw new InvalidOperationException("AttachedToLogicalTreeCore called for '" + GetType().Name + "' but control has no logical parent.");
		}
		if (_logicalRoot == null)
		{
			_logicalRoot = e.Root;
			ReevaluateImplicitTheme();
			ApplyStyling();
			NotifyResourcesChanged(null, propagate: false);
			OnAttachedToLogicalTree(e);
			this.AttachedToLogicalTree?.Invoke(this, e);
		}
		IAvaloniaList<ILogical> logicalChildren = LogicalChildren;
		int count = logicalChildren.Count;
		for (int i = 0; i < count; i++)
		{
			if (logicalChildren[i] is StyledElement styledElement && styledElement._logicalRoot != e.Root)
			{
				styledElement.OnAttachedToLogicalTreeCore(e);
			}
		}
	}

	private void OnDetachedFromLogicalTreeCore(LogicalTreeAttachmentEventArgs e)
	{
		if (_logicalRoot == null)
		{
			return;
		}
		_logicalRoot = null;
		InvalidateStyles(recurse: false);
		OnDetachedFromLogicalTree(e);
		this.DetachedFromLogicalTree?.Invoke(this, e);
		IAvaloniaList<ILogical> logicalChildren = LogicalChildren;
		int count = logicalChildren.Count;
		for (int i = 0; i < count; i++)
		{
			if (logicalChildren[i] is StyledElement styledElement)
			{
				styledElement.OnDetachedFromLogicalTreeCore(e);
			}
		}
	}

	private void OnDataContextChangedCore(AvaloniaPropertyChangedEventArgs e)
	{
		OnDataContextChanged(EventArgs.Empty);
	}

	private void SetLogicalParent(IList children)
	{
		int count = children.Count;
		for (int i = 0; i < count; i++)
		{
			ILogical logical = (ILogical)children[i];
			if (logical.LogicalParent == null)
			{
				((ISetLogicalParent)logical).SetParent(this);
			}
		}
	}

	private void ClearLogicalParent(IList children)
	{
		int count = children.Count;
		for (int i = 0; i < count; i++)
		{
			ILogical logical = (ILogical)children[i];
			if (logical.LogicalParent == this)
			{
				((ISetLogicalParent)logical).SetParent(null);
			}
		}
	}

	private void DetachStyles(IReadOnlyList<Style> styles)
	{
		ValueStore valueStore = GetValueStore();
		valueStore.BeginStyling();
		try
		{
			valueStore.RemoveFrames(styles);
		}
		finally
		{
			valueStore.EndStyling();
		}
		if (_logicalChildren != null)
		{
			int count = _logicalChildren.Count;
			for (int i = 0; i < count; i++)
			{
				(_logicalChildren[i] as StyledElement)?.DetachStyles(styles);
			}
		}
	}

	private void NotifyResourcesChanged(ResourcesChangedEventArgs? e = null, bool propagate = true)
	{
		if (this.ResourcesChanged != null)
		{
			if (e == null)
			{
				e = ResourcesChangedEventArgs.Empty;
			}
			this.ResourcesChanged(this, e);
		}
		if (propagate)
		{
			if (e == null)
			{
				e = ResourcesChangedEventArgs.Empty;
			}
			NotifyChildResourcesChanged(e);
		}
	}

	private static IReadOnlyList<Style>? FlattenStyles(IReadOnlyList<IStyle> styles)
	{
		List<Style> result2 = null;
		FlattenStyles(styles, ref result2);
		return result2;
		static void FlattenStyle(IStyle style, ref List<Style>? result)
		{
			if (style is Style item)
			{
				(result ?? (result = new List<Style>())).Add(item);
			}
			FlattenStyles(style.Children, ref result);
		}
		static void FlattenStyles(IReadOnlyList<IStyle> styles, ref List<Style>? result)
		{
			int count = styles.Count;
			for (int i = 0; i < count; i++)
			{
				FlattenStyle(styles[i], ref result);
			}
		}
	}

	private static bool HasSettersOrAnimations(IReadOnlyList<IStyle> styles)
	{
		int count = styles.Count;
		for (int i = 0; i < count; i++)
		{
			if (StyleHasSettersOrAnimations(styles[i]))
			{
				return true;
			}
		}
		return false;
		static bool StyleHasSettersOrAnimations(IStyle style)
		{
			if (style is StyleBase { HasSettersOrAnimations: not false })
			{
				return true;
			}
			return HasSettersOrAnimations(style.Children);
		}
	}

	private static IReadOnlyList<StyleBase> RecurseStyles(IReadOnlyList<IStyle> styles)
	{
		List<StyleBase> result = new List<StyleBase>();
		RecurseStyles(styles, result);
		return result;
	}

	private static void RecurseStyles(IReadOnlyList<IStyle> styles, List<StyleBase> result)
	{
		int count = styles.Count;
		for (int i = 0; i < count; i++)
		{
			IStyle style = styles[i];
			if (style is StyleBase item)
			{
				result.Add(item);
			}
			else if (style is IReadOnlyList<IStyle> styles2)
			{
				RecurseStyles(styles2, result);
			}
		}
	}
}
