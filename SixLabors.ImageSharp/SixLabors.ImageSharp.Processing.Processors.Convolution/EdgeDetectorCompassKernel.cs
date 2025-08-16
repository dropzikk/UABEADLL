using System;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

public readonly struct EdgeDetectorCompassKernel : IEquatable<EdgeDetectorCompassKernel>
{
	public static readonly EdgeDetectorCompassKernel Kirsch = new EdgeDetectorCompassKernel(KirschKernels.North, KirschKernels.NorthWest, KirschKernels.West, KirschKernels.SouthWest, KirschKernels.South, KirschKernels.SouthEast, KirschKernels.East, KirschKernels.NorthEast);

	public static readonly EdgeDetectorCompassKernel Robinson = new EdgeDetectorCompassKernel(RobinsonKernels.North, RobinsonKernels.NorthWest, RobinsonKernels.West, RobinsonKernels.SouthWest, RobinsonKernels.South, RobinsonKernels.SouthEast, RobinsonKernels.East, RobinsonKernels.NorthEast);

	public DenseMatrix<float> North { get; }

	public DenseMatrix<float> NorthWest { get; }

	public DenseMatrix<float> West { get; }

	public DenseMatrix<float> SouthWest { get; }

	public DenseMatrix<float> South { get; }

	public DenseMatrix<float> SouthEast { get; }

	public DenseMatrix<float> East { get; }

	public DenseMatrix<float> NorthEast { get; }

	public EdgeDetectorCompassKernel(DenseMatrix<float> north, DenseMatrix<float> northWest, DenseMatrix<float> west, DenseMatrix<float> southWest, DenseMatrix<float> south, DenseMatrix<float> southEast, DenseMatrix<float> east, DenseMatrix<float> northEast)
	{
		North = north;
		NorthWest = northWest;
		West = west;
		SouthWest = southWest;
		South = south;
		SouthEast = southEast;
		East = east;
		NorthEast = northEast;
	}

	public static bool operator ==(EdgeDetectorCompassKernel left, EdgeDetectorCompassKernel right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(EdgeDetectorCompassKernel left, EdgeDetectorCompassKernel right)
	{
		return !(left == right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is EdgeDetectorCompassKernel other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(EdgeDetectorCompassKernel other)
	{
		if (North.Equals(other.North) && NorthWest.Equals(other.NorthWest) && West.Equals(other.West) && SouthWest.Equals(other.SouthWest) && South.Equals(other.South) && SouthEast.Equals(other.SouthEast) && East.Equals(other.East))
		{
			return NorthEast.Equals(other.NorthEast);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(North, NorthWest, West, SouthWest, South, SouthEast, East, NorthEast);
	}

	internal DenseMatrix<float>[] Flatten()
	{
		return new DenseMatrix<float>[8] { North, NorthWest, West, SouthWest, South, SouthEast, East, NorthEast };
	}
}
