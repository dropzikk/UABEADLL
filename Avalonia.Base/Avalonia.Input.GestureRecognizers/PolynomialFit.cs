namespace Avalonia.Input.GestureRecognizers;

internal class PolynomialFit
{
	public double[] Coefficients { get; }

	public double Confidence { get; set; }

	internal PolynomialFit(int degree)
	{
		Coefficients = new double[degree + 1];
	}
}
