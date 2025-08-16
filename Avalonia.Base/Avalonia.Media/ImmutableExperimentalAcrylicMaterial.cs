using System;

namespace Avalonia.Media;

public readonly struct ImmutableExperimentalAcrylicMaterial : IExperimentalAcrylicMaterial, IEquatable<ImmutableExperimentalAcrylicMaterial>
{
	public AcrylicBackgroundSource BackgroundSource { get; }

	public Color TintColor { get; }

	public Color MaterialColor { get; }

	public double TintOpacity { get; }

	public Color FallbackColor { get; }

	public ImmutableExperimentalAcrylicMaterial(IExperimentalAcrylicMaterial brush)
	{
		BackgroundSource = brush.BackgroundSource;
		TintColor = brush.TintColor;
		TintOpacity = brush.TintOpacity;
		FallbackColor = brush.FallbackColor;
		MaterialColor = brush.MaterialColor;
	}

	public bool Equals(ImmutableExperimentalAcrylicMaterial other)
	{
		if (TintColor == other.TintColor && TintOpacity == other.TintOpacity && BackgroundSource == other.BackgroundSource && FallbackColor == other.FallbackColor)
		{
			return MaterialColor == other.MaterialColor;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is ImmutableExperimentalAcrylicMaterial other)
		{
			return Equals(other);
		}
		return false;
	}

	public Color GetEffectiveTintColor()
	{
		return TintColor;
	}

	public override int GetHashCode()
	{
		return ((((17 * 23 + TintColor.GetHashCode()) * 23 + TintOpacity.GetHashCode()) * 23 + BackgroundSource.GetHashCode()) * 23 + FallbackColor.GetHashCode()) * 23 + MaterialColor.GetHashCode();
	}

	public static bool operator ==(ImmutableExperimentalAcrylicMaterial left, ImmutableExperimentalAcrylicMaterial right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ImmutableExperimentalAcrylicMaterial left, ImmutableExperimentalAcrylicMaterial right)
	{
		return !left.Equals(right);
	}
}
