using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SixLabors;

[DebuggerStepThrough]
internal static class DebugGuard
{
	[Conditional("DEBUG")]
	public static void IsTrue(bool target, string message)
	{
		if (!target)
		{
			throw new InvalidOperationException(message);
		}
	}

	[Conditional("DEBUG")]
	public static void NotDisposed(bool isDisposed, string objectName)
	{
		if (isDisposed)
		{
			throw new ObjectDisposedException(objectName);
		}
	}

	[Conditional("DEBUG")]
	public static void MustBeSameSized<T>(ReadOnlySpan<T> target, ReadOnlySpan<T> other, string parameterName) where T : struct
	{
		if (target.Length != other.Length)
		{
			throw new ArgumentException("Span-s must be the same size!", parameterName);
		}
	}

	[Conditional("DEBUG")]
	public static void MustBeSizedAtLeast<T>(ReadOnlySpan<T> target, ReadOnlySpan<T> minSpan, string parameterName) where T : struct
	{
		if (target.Length < minSpan.Length)
		{
			throw new ArgumentException($"Span-s must be at least of length {minSpan.Length}!", parameterName);
		}
	}

	[Conditional("DEBUG")]
	public static void NotNull<TValue>([NotNull] TValue? value, [CallerArgumentExpression("value")] string? parameterName = null) where TValue : class
	{
		ArgumentNullException.ThrowIfNull(value, parameterName);
	}

	[Conditional("DEBUG")]
	public static void NotNullOrWhiteSpace([NotNull] string? value, [CallerArgumentExpression("value")] string? paramName = null)
	{
		ArgumentNullException.ThrowIfNull(value, "value");
		if (string.IsNullOrWhiteSpace(value))
		{
			ThrowArgumentException("Must not be empty or whitespace.", paramName);
		}
	}

	[Conditional("DEBUG")]
	public static void MustBeLessThan<TValue>(TValue value, TValue max, string parameterName) where TValue : IComparable<TValue>
	{
		if (value.CompareTo(max) >= 0)
		{
			ThrowArgumentOutOfRangeException(parameterName, $"Value {value} must be less than {max}.");
		}
	}

	[Conditional("DEBUG")]
	public static void MustBeLessThanOrEqualTo<TValue>(TValue value, TValue max, string parameterName) where TValue : IComparable<TValue>
	{
		if (value.CompareTo(max) > 0)
		{
			ThrowArgumentOutOfRangeException(parameterName, $"Value {value} must be less than or equal to {max}.");
		}
	}

	[Conditional("DEBUG")]
	public static void MustBeGreaterThan<TValue>(TValue value, TValue min, string parameterName) where TValue : IComparable<TValue>
	{
		if (value.CompareTo(min) <= 0)
		{
			ThrowArgumentOutOfRangeException(parameterName, $"Value {value} must be greater than {min}.");
		}
	}

	[Conditional("DEBUG")]
	public static void MustBeGreaterThanOrEqualTo<TValue>(TValue value, TValue min, string parameterName) where TValue : IComparable<TValue>
	{
		if (value.CompareTo(min) < 0)
		{
			ThrowArgumentOutOfRangeException(parameterName, $"Value {value} must be greater than or equal to {min}.");
		}
	}

	[Conditional("DEBUG")]
	public static void MustBeBetweenOrEqualTo<TValue>(TValue value, TValue min, TValue max, string parameterName) where TValue : IComparable<TValue>
	{
		if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
		{
			ThrowArgumentOutOfRangeException(parameterName, $"Value {value} must be greater than or equal to {min} and less than or equal to {max}.");
		}
	}

	[Conditional("DEBUG")]
	public static void IsTrue(bool target, string parameterName, string message)
	{
		if (!target)
		{
			ThrowArgumentException(message, parameterName);
		}
	}

	[Conditional("DEBUG")]
	public static void IsFalse(bool target, string parameterName, string message)
	{
		if (target)
		{
			ThrowArgumentException(message, parameterName);
		}
	}

	[Conditional("DEBUG")]
	public static void MustBeSizedAtLeast<T>(ReadOnlySpan<T> source, int minLength, string parameterName)
	{
		if (source.Length < minLength)
		{
			ThrowArgumentException($"Span-s must be at least of length {minLength}!", parameterName);
		}
	}

	[Conditional("DEBUG")]
	public static void MustBeSizedAtLeast<T>(Span<T> source, int minLength, string parameterName)
	{
		if (source.Length < minLength)
		{
			ThrowArgumentException($"The size must be at least {minLength}.", parameterName);
		}
	}

	[Conditional("DEBUG")]
	public static void DestinationShouldNotBeTooShort<TSource, TDest>(ReadOnlySpan<TSource> source, Span<TDest> destination, string destinationParamName)
	{
		if (destination.Length < source.Length)
		{
			ThrowArgumentException("Destination span is too short!", destinationParamName);
		}
	}

	[Conditional("DEBUG")]
	public static void DestinationShouldNotBeTooShort<TSource, TDest>(Span<TSource> source, Span<TDest> destination, string destinationParamName)
	{
		if (destination.Length < source.Length)
		{
			ThrowArgumentException("Destination span is too short!", destinationParamName);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void ThrowArgumentException(string message, string parameterName)
	{
		throw new ArgumentException(message, parameterName);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void ThrowArgumentOutOfRangeException(string parameterName, string message)
	{
		throw new ArgumentOutOfRangeException(parameterName, message);
	}
}
