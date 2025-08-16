namespace Avalonia.Animation.Easings;

public class SpringEasing : Easing
{
	private readonly Spring _internalSpring;

	public double Mass
	{
		get
		{
			return _internalSpring.Mass;
		}
		set
		{
			_internalSpring.Mass = value;
		}
	}

	public double Stiffness
	{
		get
		{
			return _internalSpring.Stiffness;
		}
		set
		{
			_internalSpring.Stiffness = value;
		}
	}

	public double Damping
	{
		get
		{
			return _internalSpring.Damping;
		}
		set
		{
			_internalSpring.Damping = value;
		}
	}

	public double InitialVelocity
	{
		get
		{
			return _internalSpring.InitialVelocity;
		}
		set
		{
			_internalSpring.InitialVelocity = value;
		}
	}

	public SpringEasing(double mass = 0.0, double stiffness = 0.0, double damping = 0.0, double initialVelocity = 0.0)
	{
		_internalSpring = new Spring();
		Mass = mass;
		Stiffness = stiffness;
		Damping = damping;
		InitialVelocity = initialVelocity;
	}

	public SpringEasing()
	{
		_internalSpring = new Spring();
	}

	public override double Ease(double progress)
	{
		return _internalSpring.GetSpringProgress(progress);
	}
}
