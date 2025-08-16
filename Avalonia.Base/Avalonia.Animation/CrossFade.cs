using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Animation.Easings;
using Avalonia.Data;
using Avalonia.Reactive;
using Avalonia.Styling;

namespace Avalonia.Animation;

public class CrossFade : IPageTransition
{
	private readonly Animation _fadeOutAnimation;

	private readonly Animation _fadeInAnimation;

	public TimeSpan Duration
	{
		get
		{
			return _fadeOutAnimation.Duration;
		}
		set
		{
			Animation fadeOutAnimation = _fadeOutAnimation;
			TimeSpan duration = (_fadeInAnimation.Duration = value);
			fadeOutAnimation.Duration = duration;
		}
	}

	public Easing FadeInEasing
	{
		get
		{
			return _fadeInAnimation.Easing;
		}
		set
		{
			_fadeInAnimation.Easing = value;
		}
	}

	public Easing FadeOutEasing
	{
		get
		{
			return _fadeOutAnimation.Easing;
		}
		set
		{
			_fadeOutAnimation.Easing = value;
		}
	}

	public CrossFade()
		: this(TimeSpan.Zero)
	{
	}

	public CrossFade(TimeSpan duration)
	{
		_fadeOutAnimation = new Animation
		{
			Children = 
			{
				new KeyFrame
				{
					Setters = { (IAnimationSetter)new Setter
					{
						Property = Visual.OpacityProperty,
						Value = 0.0
					} },
					Cue = new Cue(1.0)
				}
			}
		};
		_fadeInAnimation = new Animation
		{
			Children = 
			{
				new KeyFrame
				{
					Setters = { (IAnimationSetter)new Setter
					{
						Property = Visual.OpacityProperty,
						Value = 1.0
					} },
					Cue = new Cue(1.0)
				}
			}
		};
		_fadeOutAnimation.Duration = (_fadeInAnimation.Duration = duration);
	}

	public async Task Start(Visual? from, Visual? to, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return;
		}
		List<Task> list = new List<Task>();
		using CompositeDisposable disposables = new CompositeDisposable(1);
		if (to != null)
		{
			disposables.Add(to.SetValue(Visual.OpacityProperty, 0.0, BindingPriority.Animation));
		}
		if (from != null)
		{
			list.Add(_fadeOutAnimation.RunAsync(from, null, cancellationToken));
		}
		if (to != null)
		{
			to.IsVisible = true;
			list.Add(_fadeInAnimation.RunAsync(to, null, cancellationToken));
		}
		await Task.WhenAll(list);
		if (from != null && !cancellationToken.IsCancellationRequested)
		{
			from.IsVisible = false;
		}
	}

	Task IPageTransition.Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
	{
		return Start(from, to, cancellationToken);
	}
}
