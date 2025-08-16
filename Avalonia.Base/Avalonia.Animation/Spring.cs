using System.ComponentModel;
using System.Globalization;
using Avalonia.Utilities;

namespace Avalonia.Animation;

[TypeConverter(typeof(SpringTypeConverter))]
internal class Spring
{
	private SpringSolver _springSolver;

	private double _mass;

	private double _stiffness;

	private double _damping;

	private double _initialVelocity;

	private bool _isDirty;

	public double Mass
	{
		get
		{
			return _mass;
		}
		set
		{
			_mass = value;
			_isDirty = true;
		}
	}

	public double Stiffness
	{
		get
		{
			return _stiffness;
		}
		set
		{
			_stiffness = value;
			_isDirty = true;
		}
	}

	public double Damping
	{
		get
		{
			return _damping;
		}
		set
		{
			_damping = value;
			_isDirty = true;
		}
	}

	public double InitialVelocity
	{
		get
		{
			return _initialVelocity;
		}
		set
		{
			_initialVelocity = value;
			_isDirty = true;
		}
	}

	public Spring()
	{
		_mass = 0.0;
		_stiffness = 0.0;
		_damping = 0.0;
		_initialVelocity = 0.0;
		_isDirty = true;
	}

	public Spring(double mass, double stiffness, double damping, double initialVelocity)
	{
		_mass = mass;
		_stiffness = stiffness;
		_damping = damping;
		_initialVelocity = initialVelocity;
		_isDirty = true;
	}

	public static Spring Parse(string value, CultureInfo? culture)
	{
		if (culture == null)
		{
			culture = CultureInfo.InvariantCulture;
		}
		using StringTokenizer stringTokenizer = new StringTokenizer(value, culture, "Invalid Spring string: \"" + value + "\".");
		return new Spring(stringTokenizer.ReadDouble(null), stringTokenizer.ReadDouble(null), stringTokenizer.ReadDouble(null), stringTokenizer.ReadDouble(null));
	}

	public double GetSpringProgress(double linearProgress)
	{
		if (_isDirty)
		{
			Build();
		}
		return _springSolver.Solve(linearProgress);
	}

	private void Build()
	{
		_springSolver = new SpringSolver(_mass, _stiffness, _damping, _initialVelocity);
		_isDirty = false;
	}
}
