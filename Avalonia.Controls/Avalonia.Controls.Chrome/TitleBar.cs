using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Reactive;
using Avalonia.Rendering;

namespace Avalonia.Controls.Chrome;

[TemplatePart("PART_CaptionButtons", typeof(CaptionButtons))]
[PseudoClasses(new string[] { ":minimized", ":normal", ":maximized", ":fullscreen" })]
public class TitleBar : TemplatedControl
{
	private CompositeDisposable? _disposables;

	private CaptionButtons? _captionButtons;

	private void UpdateSize(Window window)
	{
		base.Margin = new Thickness(window.OffScreenMargin.Left, window.OffScreenMargin.Top, window.OffScreenMargin.Right, window.OffScreenMargin.Bottom);
		if (window.WindowState != WindowState.FullScreen)
		{
			base.Height = window.WindowDecorationMargin.Top;
			if (_captionButtons != null)
			{
				_captionButtons.Height = base.Height;
			}
		}
		base.IsVisible = window.PlatformImpl?.NeedsManagedDecorations ?? false;
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_captionButtons?.Detach();
		_captionButtons = e.NameScope.Get<CaptionButtons>("PART_CaptionButtons");
		if (base.VisualRoot is Window window)
		{
			_captionButtons?.Attach(window);
			UpdateSize(window);
		}
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		IRenderRoot visualRoot = base.VisualRoot;
		Window window = visualRoot as Window;
		if (window != null)
		{
			_disposables = new CompositeDisposable(6)
			{
				window.GetObservable(Window.WindowDecorationMarginProperty).Subscribe(delegate
				{
					UpdateSize(window);
				}),
				window.GetObservable(Window.ExtendClientAreaTitleBarHeightHintProperty).Subscribe(delegate
				{
					UpdateSize(window);
				}),
				window.GetObservable(Window.OffScreenMarginProperty).Subscribe(delegate
				{
					UpdateSize(window);
				}),
				window.GetObservable(Window.ExtendClientAreaChromeHintsProperty).Subscribe(delegate
				{
					UpdateSize(window);
				}),
				window.GetObservable(Window.WindowStateProperty).Subscribe(delegate(WindowState x)
				{
					base.PseudoClasses.Set(":minimized", x == WindowState.Minimized);
					base.PseudoClasses.Set(":normal", x == WindowState.Normal);
					base.PseudoClasses.Set(":maximized", x == WindowState.Maximized);
					base.PseudoClasses.Set(":fullscreen", x == WindowState.FullScreen);
				}),
				window.GetObservable(Window.IsExtendedIntoWindowDecorationsProperty).Subscribe(delegate
				{
					UpdateSize(window);
				})
			};
		}
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		_disposables?.Dispose();
		_captionButtons?.Detach();
		_captionButtons = null;
	}
}
