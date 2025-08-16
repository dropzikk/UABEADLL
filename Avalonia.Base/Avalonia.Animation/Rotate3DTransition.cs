using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Styling;

namespace Avalonia.Animation;

public class Rotate3DTransition : PageSlide
{
	public double? Depth { get; set; }

	public Rotate3DTransition(TimeSpan duration, SlideAxis orientation = SlideAxis.Horizontal, double? depth = null)
		: base(duration, orientation)
	{
		Depth = depth;
	}

	public Rotate3DTransition()
	{
	}

	public override async Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return;
		}
		Task[] array = new Task[(from == null || to == null) ? 1 : 2];
		Visual visualParent = PageSlide.GetVisualParent(from, to);
		(StyledProperty<double>, double) tuple = base.Orientation switch
		{
			SlideAxis.Vertical => (Rotate3DTransform.AngleXProperty, visualParent.Bounds.Height), 
			SlideAxis.Horizontal => (Rotate3DTransform.AngleYProperty, visualParent.Bounds.Width), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		StyledProperty<double> rotateProperty = tuple.Item1;
		double item = tuple.Item2;
		Setter depthSetter = new Setter
		{
			Property = Rotate3DTransform.DepthProperty,
			Value = (Depth ?? item)
		};
		Setter centerZSetter = new Setter
		{
			Property = Rotate3DTransform.CenterZProperty,
			Value = (0.0 - item) / 2.0
		};
		if (from != null)
		{
			Animation animation = new Animation
			{
				Easing = base.SlideOutEasing,
				Duration = base.Duration,
				Children = 
				{
					CreateKeyFrame(0.0, 0.0, 2),
					CreateKeyFrame(0.5, 45.0 * (double)((!forward) ? 1 : (-1)), 1),
					CreateKeyFrame(1.0, 90.0 * (double)((!forward) ? 1 : (-1)), 1, isVisible: false)
				}
			};
			array[0] = animation.RunAsync(from, null, cancellationToken);
		}
		if (to != null)
		{
			to.IsVisible = true;
			Animation animation2 = new Animation
			{
				Easing = base.SlideInEasing,
				Duration = base.Duration,
				FillMode = FillMode.Forward,
				Children = 
				{
					CreateKeyFrame(0.0, 90.0 * (double)(forward ? 1 : (-1)), 1),
					CreateKeyFrame(0.5, 45.0 * (double)(forward ? 1 : (-1)), 1),
					CreateKeyFrame(1.0, 0.0, 2)
				}
			};
			array[(from != null) ? 1u : 0u] = animation2.RunAsync(to, null, cancellationToken);
		}
		await Task.WhenAll(array);
		if (!cancellationToken.IsCancellationRequested)
		{
			if (to != null)
			{
				to.ZIndex = 2;
			}
			if (from != null)
			{
				from.IsVisible = false;
				from.ZIndex = 1;
			}
		}
		KeyFrame CreateKeyFrame(double cue, double rotation, int zIndex, bool isVisible = true)
		{
			return new KeyFrame
			{
				Setters = 
				{
					(IAnimationSetter)new Setter
					{
						Property = rotateProperty,
						Value = rotation
					},
					(IAnimationSetter)new Setter
					{
						Property = Visual.ZIndexProperty,
						Value = zIndex
					},
					(IAnimationSetter)new Setter
					{
						Property = Visual.IsVisibleProperty,
						Value = isVisible
					},
					(IAnimationSetter)centerZSetter,
					(IAnimationSetter)depthSetter
				},
				Cue = new Cue(cue)
			};
		}
	}
}
