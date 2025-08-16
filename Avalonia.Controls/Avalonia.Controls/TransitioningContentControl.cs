using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Animation;
using Avalonia.Controls.Presenters;

namespace Avalonia.Controls;

public class TransitioningContentControl : ContentControl
{
	private class ImmutableCrossFade : IPageTransition
	{
		private readonly CrossFade _inner;

		public ImmutableCrossFade(TimeSpan duration)
		{
			_inner = new CrossFade(duration);
		}

		public Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
		{
			return _inner.Start(from, to, cancellationToken);
		}
	}

	private CancellationTokenSource? _currentTransition;

	private ContentPresenter? _presenter2;

	private bool _isFirstFull;

	private bool _shouldAnimate;

	public static readonly StyledProperty<IPageTransition?> PageTransitionProperty = AvaloniaProperty.Register<TransitioningContentControl, IPageTransition>("PageTransition", new ImmutableCrossFade(TimeSpan.FromMilliseconds(125.0)));

	public IPageTransition? PageTransition
	{
		get
		{
			return GetValue(PageTransitionProperty);
		}
		set
		{
			SetValue(PageTransitionProperty, value);
		}
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		Size result = base.ArrangeOverride(finalSize);
		if (_shouldAnimate)
		{
			_currentTransition?.Cancel();
			if (_presenter2 != null)
			{
				Visual presenter = base.Presenter;
				if (presenter != null)
				{
					IPageTransition pageTransition = PageTransition;
					if (pageTransition != null)
					{
						_shouldAnimate = false;
						CancellationTokenSource cancel = new CancellationTokenSource();
						_currentTransition = cancel;
						Visual from = (_isFirstFull ? _presenter2 : presenter);
						Visual to = (_isFirstFull ? presenter : _presenter2);
						pageTransition.Start(from, to, forward: true, cancel.Token).ContinueWith(delegate
						{
							if (!cancel.IsCancellationRequested)
							{
								HideOldPresenter();
							}
						}, TaskScheduler.FromCurrentSynchronizationContext());
					}
				}
			}
			_shouldAnimate = false;
		}
		return result;
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		UpdateContent(withTransition: false);
	}

	protected override bool RegisterContentPresenter(ContentPresenter presenter)
	{
		if (!base.RegisterContentPresenter(presenter) && presenter != null)
		{
			if (presenter.Name == "PART_ContentPresenter2")
			{
				_presenter2 = presenter;
				_presenter2.IsVisible = false;
				UpdateContent(withTransition: false);
				return true;
			}
		}
		return false;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == ContentControl.ContentProperty)
		{
			UpdateContent(withTransition: true);
		}
	}

	private void UpdateContent(bool withTransition)
	{
		if (base.VisualRoot != null && _presenter2 != null && base.Presenter != null)
		{
			ContentPresenter? obj = (_isFirstFull ? _presenter2 : base.Presenter);
			obj.Content = base.Content;
			obj.IsVisible = true;
			_isFirstFull = !_isFirstFull;
			if (PageTransition != null && withTransition)
			{
				_shouldAnimate = true;
				InvalidateArrange();
			}
			else
			{
				HideOldPresenter();
			}
		}
	}

	private void HideOldPresenter()
	{
		ContentPresenter contentPresenter = (_isFirstFull ? _presenter2 : base.Presenter);
		if (contentPresenter != null)
		{
			contentPresenter.Content = null;
			contentPresenter.IsVisible = false;
		}
	}
}
