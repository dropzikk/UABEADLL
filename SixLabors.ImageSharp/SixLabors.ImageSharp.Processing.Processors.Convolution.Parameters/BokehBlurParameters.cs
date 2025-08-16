using System;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution.Parameters;

internal readonly struct BokehBlurParameters : IEquatable<BokehBlurParameters>
{
	public readonly int Radius;

	public readonly int Components;

	public BokehBlurParameters(int radius, int components)
	{
		Radius = radius;
		Components = components;
	}

	public bool Equals(BokehBlurParameters other)
	{
		if (Radius.Equals(other.Radius))
		{
			return Components.Equals(other.Components);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is BokehBlurParameters other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (Radius.GetHashCode() * 397) ^ Components.GetHashCode();
	}
}
