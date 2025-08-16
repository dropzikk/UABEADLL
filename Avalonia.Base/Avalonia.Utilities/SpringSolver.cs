using System;

namespace Avalonia.Utilities;

internal struct SpringSolver
{
	private double m_w0;

	private double m_zeta;

	private double m_wd;

	private double m_A;

	private double m_B;

	public SpringSolver(TimeSpan period, double zeta, double initialVelocity)
		: this(Math.PI * 2.0 / period.TotalSeconds, zeta, initialVelocity)
	{
	}

	public SpringSolver(double m, double k, double c, double initialVelocity)
		: this(Math.Sqrt(k / m), c / (2.0 * Math.Sqrt(k * m)), initialVelocity)
	{
	}

	public SpringSolver(double ωn, double zeta, double initialVelocity)
	{
		m_w0 = ωn;
		m_zeta = zeta;
		if (m_zeta < 1.0)
		{
			m_wd = m_w0 * Math.Sqrt(1.0 - m_zeta * m_zeta);
			m_A = 1.0;
			m_B = (m_zeta * m_w0 + (0.0 - initialVelocity)) / m_wd;
		}
		else
		{
			m_A = 1.0;
			m_B = 0.0 - initialVelocity + m_w0;
			m_wd = 0.0;
		}
	}

	public readonly double Solve(double t)
	{
		t = ((!(m_zeta < 1.0)) ? ((m_A + m_B * t) * Math.Exp((0.0 - t) * m_w0)) : (Math.Exp((0.0 - t) * m_zeta * m_w0) * (m_A * Math.Cos(m_wd * t) + m_B * Math.Sin(m_wd * t))));
		return 1.0 - t;
	}
}
