using System;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Reactive;

namespace Avalonia.Controls.Chrome;

[TemplatePart("PART_CloseButton", typeof(Button))]
[TemplatePart("PART_RestoreButton", typeof(Button))]
[TemplatePart("PART_MinimizeButton", typeof(Button))]
[TemplatePart("PART_FullScreenButton", typeof(Button))]
[PseudoClasses(new string[] { ":minimized", ":normal", ":maximized", ":fullscreen" })]
public class CaptionButtons : TemplatedControl
{
	private const string PART_CloseButton = "PART_CloseButton";

	private const string PART_RestoreButton = "PART_RestoreButton";

	private const string PART_MinimizeButton = "PART_MinimizeButton";

	private const string PART_FullScreenButton = "PART_FullScreenButton";

	private Button? _restoreButton;

	private IDisposable? _disposables;

	protected Window? HostWindow { get; private set; }

	public virtual void Attach(Window hostWindow)
	{
		if (_disposables != null)
		{
			return;
		}
		HostWindow = hostWindow;
		_disposables = new CompositeDisposable
		{
			HostWindow.GetObservable(Window.CanResizeProperty).Subscribe(delegate(bool x)
			{
				if (_restoreButton != null)
				{
					_restoreButton.IsEnabled = x;
				}
			}),
			HostWindow.GetObservable(Window.WindowStateProperty).Subscribe(delegate(WindowState x)
			{
				base.PseudoClasses.Set(":minimized", x == WindowState.Minimized);
				base.PseudoClasses.Set(":normal", x == WindowState.Normal);
				base.PseudoClasses.Set(":maximized", x == WindowState.Maximized);
				base.PseudoClasses.Set(":fullscreen", x == WindowState.FullScreen);
			})
		};
	}

	public virtual void Detach()
	{
		if (_disposables != null)
		{
			_disposables.Dispose();
			_disposables = null;
			HostWindow = null;
		}
	}

	protected virtual void OnClose()
	{
		HostWindow?.Close();
	}

	protected virtual void OnRestore()
	{
		if (HostWindow != null)
		{
			HostWindow.WindowState = ((HostWindow.WindowState != WindowState.Maximized) ? WindowState.Maximized : WindowState.Normal);
		}
	}

	protected virtual void OnMinimize()
	{
		if (HostWindow != null)
		{
			HostWindow.WindowState = WindowState.Minimized;
		}
	}

	protected virtual void OnToggleFullScreen()
	{
		if (HostWindow != null)
		{
			HostWindow.WindowState = ((HostWindow.WindowState != WindowState.FullScreen) ? WindowState.FullScreen : WindowState.Normal);
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		Button button = e.NameScope.Get<Button>("PART_CloseButton");
		Button button2 = e.NameScope.Get<Button>("PART_RestoreButton");
		Button button3 = e.NameScope.Get<Button>("PART_MinimizeButton");
		Button button4 = e.NameScope.Get<Button>("PART_FullScreenButton");
		button.Click += delegate
		{
			OnClose();
		};
		button2.Click += delegate
		{
			OnRestore();
		};
		button3.Click += delegate
		{
			OnMinimize();
		};
		button4.Click += delegate
		{
			OnToggleFullScreen();
		};
		button2.IsEnabled = HostWindow?.CanResize ?? true;
		_restoreButton = button2;
	}
}
