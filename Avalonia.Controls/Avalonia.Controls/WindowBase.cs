using System;
using System.ComponentModel;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Platform;

namespace Avalonia.Controls;

public class WindowBase : TopLevel
{
	private readonly struct IgnoreVisibilityChangesDisposable : IDisposable
	{
		private readonly WindowBase _windowBase;

		public IgnoreVisibilityChangesDisposable(WindowBase windowBase)
		{
			_windowBase = windowBase;
			_windowBase._ignoreVisibilityChanges++;
		}

		public void Dispose()
		{
			_windowBase._ignoreVisibilityChanges--;
		}
	}

	public static readonly DirectProperty<WindowBase, bool> IsActiveProperty;

	public static readonly DirectProperty<WindowBase, WindowBase?> OwnerProperty;

	public static readonly StyledProperty<bool> TopmostProperty;

	private bool _hasExecutedInitialLayoutPass;

	private bool _isActive;

	private int _ignoreVisibilityChanges;

	private WindowBase? _owner;

	protected bool IgnoreVisibilityChanges => _ignoreVisibilityChanges > 0;

	public new IWindowBaseImpl? PlatformImpl => (IWindowBaseImpl)base.PlatformImpl;

	public bool IsActive
	{
		get
		{
			return _isActive;
		}
		private set
		{
			SetAndRaise(IsActiveProperty, ref _isActive, value);
		}
	}

	public Screens Screens { get; }

	public WindowBase? Owner
	{
		get
		{
			return _owner;
		}
		protected set
		{
			SetAndRaise(OwnerProperty, ref _owner, value);
		}
	}

	public bool Topmost
	{
		get
		{
			return GetValue(TopmostProperty);
		}
		set
		{
			SetValue(TopmostProperty, value);
		}
	}

	public double DesktopScaling => PlatformImpl?.DesktopScaling ?? 1.0;

	public event EventHandler? Activated;

	public event EventHandler? Deactivated;

	public event EventHandler<PixelPointEventArgs>? PositionChanged;

	public event EventHandler<WindowResizedEventArgs>? Resized;

	static WindowBase()
	{
		IsActiveProperty = AvaloniaProperty.RegisterDirect("IsActive", (WindowBase o) => o.IsActive, null, unsetValue: false);
		OwnerProperty = AvaloniaProperty.RegisterDirect("Owner", (WindowBase o) => o.Owner);
		TopmostProperty = AvaloniaProperty.Register<WindowBase, bool>("Topmost", defaultValue: false);
		Visual.IsVisibleProperty.OverrideDefaultValue<WindowBase>(defaultValue: false);
		Visual.IsVisibleProperty.Changed.AddClassHandler(delegate(WindowBase x, AvaloniaPropertyChangedEventArgs e)
		{
			x.IsVisibleChanged(e);
		});
		TopmostProperty.Changed.AddClassHandler(delegate(WindowBase w, AvaloniaPropertyChangedEventArgs e)
		{
			w.PlatformImpl?.SetTopmost((bool)e.NewValue);
		});
	}

	public WindowBase(IWindowBaseImpl impl)
		: this(impl, AvaloniaLocator.Current)
	{
	}

	public WindowBase(IWindowBaseImpl impl, IAvaloniaDependencyResolver? dependencyResolver)
		: base(impl, dependencyResolver)
	{
		Screens = new Screens(impl.Screen);
		impl.Activated = HandleActivated;
		impl.Deactivated = HandleDeactivated;
		impl.PositionChanged = HandlePositionChanged;
	}

	private protected IDisposable FreezeVisibilityChangeHandling()
	{
		return new IgnoreVisibilityChangesDisposable(this);
	}

	public void Activate()
	{
		PlatformImpl?.Activate();
	}

	public virtual void Hide()
	{
		using (FreezeVisibilityChangeHandling())
		{
			StopRendering();
			PlatformImpl?.Hide();
			base.IsVisible = false;
		}
	}

	public virtual void Show()
	{
		using (FreezeVisibilityChangeHandling())
		{
			EnsureInitialized();
			ApplyStyling();
			base.IsVisible = true;
			if (!_hasExecutedInitialLayoutPass)
			{
				base.LayoutManager.ExecuteInitialLayoutPass();
				_hasExecutedInitialLayoutPass = true;
			}
			PlatformImpl?.Show(activate: true, isDialog: false);
			StartRendering();
			OnOpened(EventArgs.Empty);
		}
	}

	protected void EnsureInitialized()
	{
		if (!base.IsInitialized)
		{
			((ISupportInitialize)this).BeginInit();
			((ISupportInitialize)this).EndInit();
		}
	}

	protected override void OnClosed(EventArgs e)
	{
		OnUnloadedCore();
		base.OnClosed(e);
	}

	protected override void OnOpened(EventArgs e)
	{
		ScheduleOnLoadedCore();
		base.OnOpened(e);
	}

	protected virtual void OnResized(WindowResizedEventArgs e)
	{
		this.Resized?.Invoke(this, e);
	}

	private protected override void HandleClosed()
	{
		using (FreezeVisibilityChangeHandling())
		{
			base.IsVisible = false;
			if (this is IFocusScope scope)
			{
				((FocusManager)base.FocusManager)?.RemoveFocusScope(scope);
			}
			base.HandleClosed();
		}
	}

	internal override void HandleResized(Size clientSize, WindowResizeReason reason)
	{
		base.FrameSize = PlatformImpl?.FrameSize;
		bool num = base.ClientSize != clientSize;
		base.ClientSize = clientSize;
		OnResized(new WindowResizedEventArgs(clientSize, reason));
		if (num)
		{
			base.LayoutManager.ExecuteLayoutPass();
			base.Renderer.Resized(clientSize);
		}
	}

	protected override Size MeasureCore(Size availableSize)
	{
		ApplyStyling();
		ApplyTemplate();
		Size availableSize2 = LayoutHelper.ApplyLayoutConstraints(this, availableSize);
		return MeasureOverride(availableSize2);
	}

	protected override void ArrangeCore(Rect finalRect)
	{
		Size finalSize = ArrangeSetBounds(finalRect.Size);
		Size size = ArrangeOverride(finalSize);
		base.Bounds = new Rect(size);
	}

	protected virtual Size ArrangeSetBounds(Size size)
	{
		return size;
	}

	private void HandlePositionChanged(PixelPoint pos)
	{
		this.PositionChanged?.Invoke(this, new PixelPointEventArgs(pos));
	}

	private void HandleActivated()
	{
		this.Activated?.Invoke(this, EventArgs.Empty);
		if (this is IFocusScope focusScope)
		{
			((FocusManager)base.FocusManager)?.SetFocusScope(focusScope);
		}
		IsActive = true;
	}

	private void HandleDeactivated()
	{
		IsActive = false;
		this.Deactivated?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void IsVisibleChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (_ignoreVisibilityChanges == 0)
		{
			if ((bool)e.NewValue)
			{
				Show();
			}
			else
			{
				Hide();
			}
		}
	}
}
