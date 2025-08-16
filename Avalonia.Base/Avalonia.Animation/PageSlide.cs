using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;

namespace Avalonia.Animation;

public class PageSlide : IPageTransition
{
	public enum SlideAxis
	{
		Horizontal,
		Vertical
	}

	public TimeSpan Duration { get; set; }

	public SlideAxis Orientation { get; set; }

	public Easing SlideInEasing { get; set; } = new LinearEasing();

	public Easing SlideOutEasing { get; set; } = new LinearEasing();

	public PageSlide()
	{
	}

	public PageSlide(TimeSpan duration, SlideAxis orientation = SlideAxis.Horizontal)
	{
		Duration = duration;
		Orientation = orientation;
	}

	public virtual async Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
	{
		if (!cancellationToken.IsCancellationRequested)
		{
			List<Task> list = new List<Task>();
			Visual visualParent = GetVisualParent(from, to);
			double num = ((Orientation == SlideAxis.Horizontal) ? visualParent.Bounds.Width : visualParent.Bounds.Height);
			StyledProperty<double> property = ((Orientation == SlideAxis.Horizontal) ? TranslateTransform.XProperty : TranslateTransform.YProperty);
			if (from != null)
			{
				Animation animation = new Animation
				{
					Easing = SlideOutEasing,
					Children = 
					{
						new KeyFrame
						{
							Setters = { (IAnimationSetter)new Setter
							{
								Property = property,
								Value = 0.0
							} },
							Cue = new Cue(0.0)
						},
						new KeyFrame
						{
							Setters = { (IAnimationSetter)new Setter
							{
								Property = property,
								Value = (forward ? (0.0 - num) : num)
							} },
							Cue = new Cue(1.0)
						}
					},
					Duration = Duration
				};
				list.Add(animation.RunAsync(from, null, cancellationToken));
			}
			if (to != null)
			{
				to.IsVisible = true;
				Animation animation2 = new Animation
				{
					Easing = SlideInEasing,
					Children = 
					{
						new KeyFrame
						{
							Setters = { (IAnimationSetter)new Setter
							{
								Property = property,
								Value = (forward ? num : (0.0 - num))
							} },
							Cue = new Cue(0.0)
						},
						new KeyFrame
						{
							Setters = { (IAnimationSetter)new Setter
							{
								Property = property,
								Value = 0.0
							} },
							Cue = new Cue(1.0)
						}
					},
					Duration = Duration
				};
				list.Add(animation2.RunAsync(to, null, cancellationToken));
			}
			await Task.WhenAll(list);
			if (from != null && !cancellationToken.IsCancellationRequested)
			{
				from.IsVisible = false;
			}
		}
	}

	protected static Visual GetVisualParent(Visual? from, Visual? to)
	{
		Visual visualParent = (from ?? to).VisualParent;
		Visual visualParent2 = (to ?? from).VisualParent;
		if (visualParent != null && visualParent2 != null && visualParent != visualParent2)
		{
			throw new ArgumentException("Controls for PageSlide must have same parent.");
		}
		return visualParent ?? throw new InvalidOperationException("Cannot determine visual parent.");
	}
}
