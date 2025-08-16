using System;
using System.Globalization;
using Avalonia.Utilities;

namespace Avalonia.Media.Transformation;

internal static class TransformParser
{
	private enum Unit
	{
		None,
		Pixel,
		Radian,
		Gradian,
		Degree,
		Turn
	}

	private readonly struct UnitValue
	{
		public readonly Unit Unit;

		public readonly double Value;

		public static UnitValue Zero => new UnitValue(Unit.None, 0.0);

		public static UnitValue One => new UnitValue(Unit.None, 1.0);

		public UnitValue(Unit unit, double value)
		{
			Unit = unit;
			Value = value;
		}
	}

	private enum TransformFunction
	{
		Invalid,
		Translate,
		TranslateX,
		TranslateY,
		Scale,
		ScaleX,
		ScaleY,
		Skew,
		SkewX,
		SkewY,
		Rotate,
		Matrix
	}

	private static readonly (string, TransformFunction)[] s_functionMapping = new(string, TransformFunction)[11]
	{
		("translate", TransformFunction.Translate),
		("translateX", TransformFunction.TranslateX),
		("translateY", TransformFunction.TranslateY),
		("scale", TransformFunction.Scale),
		("scaleX", TransformFunction.ScaleX),
		("scaleY", TransformFunction.ScaleY),
		("skew", TransformFunction.Skew),
		("skewX", TransformFunction.SkewX),
		("skewY", TransformFunction.SkewY),
		("rotate", TransformFunction.Rotate),
		("matrix", TransformFunction.Matrix)
	};

	private static readonly (string, Unit)[] s_unitMapping = new(string, Unit)[5]
	{
		("deg", Unit.Degree),
		("grad", Unit.Gradian),
		("rad", Unit.Radian),
		("turn", Unit.Turn),
		("px", Unit.Pixel)
	};

	public static TransformOperations Parse(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			throw new ArgumentException("s");
		}
		ReadOnlySpan<char> span = s.AsSpan().Trim();
		if (MemoryExtensions.Equals(span, "none".AsSpan(), StringComparison.OrdinalIgnoreCase))
		{
			return TransformOperations.Identity;
		}
		TransformOperations.Builder builder = TransformOperations.CreateBuilder(0);
		do
		{
			int num = span.IndexOf('(');
			int num2 = span.IndexOf(')');
			if (num == -1 || num2 == -1)
			{
				ThrowInvalidFormat();
			}
			TransformFunction transformFunction = ParseTransformFunction(span.Slice(0, num).Trim());
			if (transformFunction == TransformFunction.Invalid)
			{
				ThrowInvalidFormat();
			}
			ParseFunction(span.Slice(num + 1, num2 - num - 1).Trim(), transformFunction, in builder);
			span = span.Slice(num2 + 1);
		}
		while (!span.IsWhiteSpace());
		return builder.Build();
		void ThrowInvalidFormat()
		{
			throw new FormatException("Invalid transform string: '" + s + "'.");
		}
	}

	private static void ParseFunction(in ReadOnlySpan<char> functionPart, TransformFunction function, in TransformOperations.Builder builder)
	{
		switch (function)
		{
		case TransformFunction.Scale:
		case TransformFunction.ScaleX:
		case TransformFunction.ScaleY:
		{
			UnitValue leftValue5 = UnitValue.One;
			UnitValue rightValue5 = UnitValue.One;
			int num = ParseValuePair(in functionPart, ref leftValue5, ref rightValue5);
			if (num != 1 && (function == TransformFunction.ScaleX || function == TransformFunction.ScaleY))
			{
				ThrowFormatInvalidValueCount(function, 1);
			}
			VerifyZeroOrUnit(function, in leftValue5, Unit.None);
			VerifyZeroOrUnit(function, in rightValue5, Unit.None);
			switch (function)
			{
			case TransformFunction.ScaleY:
				rightValue5 = leftValue5;
				leftValue5 = UnitValue.One;
				break;
			case TransformFunction.Scale:
				if (num == 1)
				{
					rightValue5 = leftValue5;
				}
				break;
			}
			builder.AppendScale(leftValue5.Value, rightValue5.Value);
			break;
		}
		case TransformFunction.Skew:
		case TransformFunction.SkewX:
		case TransformFunction.SkewY:
		{
			UnitValue leftValue2 = UnitValue.Zero;
			UnitValue rightValue2 = UnitValue.Zero;
			if (ParseValuePair(in functionPart, ref leftValue2, ref rightValue2) != 1 && (function == TransformFunction.SkewX || function == TransformFunction.SkewY))
			{
				ThrowFormatInvalidValueCount(function, 1);
			}
			VerifyZeroOrAngle(function, in leftValue2);
			VerifyZeroOrAngle(function, in rightValue2);
			if (function == TransformFunction.SkewY)
			{
				rightValue2 = leftValue2;
				leftValue2 = UnitValue.Zero;
			}
			builder.AppendSkew(ToRadians(in leftValue2), ToRadians(in rightValue2));
			break;
		}
		case TransformFunction.Rotate:
		{
			UnitValue leftValue4 = UnitValue.Zero;
			UnitValue rightValue4 = default(UnitValue);
			if (ParseValuePair(in functionPart, ref leftValue4, ref rightValue4) != 1)
			{
				ThrowFormatInvalidValueCount(function, 1);
			}
			VerifyZeroOrAngle(function, in leftValue4);
			builder.AppendRotate(ToRadians(in leftValue4));
			break;
		}
		case TransformFunction.Translate:
		case TransformFunction.TranslateX:
		case TransformFunction.TranslateY:
		{
			UnitValue leftValue3 = UnitValue.Zero;
			UnitValue rightValue3 = UnitValue.Zero;
			if (ParseValuePair(in functionPart, ref leftValue3, ref rightValue3) != 1 && (function == TransformFunction.TranslateX || function == TransformFunction.TranslateY))
			{
				ThrowFormatInvalidValueCount(function, 1);
			}
			VerifyZeroOrUnit(function, in leftValue3, Unit.Pixel);
			VerifyZeroOrUnit(function, in rightValue3, Unit.Pixel);
			if (function == TransformFunction.TranslateY)
			{
				rightValue3 = leftValue3;
				leftValue3 = UnitValue.Zero;
			}
			builder.AppendTranslate(leftValue3.Value, rightValue3.Value);
			break;
		}
		case TransformFunction.Matrix:
		{
			Span<UnitValue> outValues2 = stackalloc UnitValue[6];
			if (ParseCommaDelimitedValues(functionPart, in outValues2) != 6)
			{
				ThrowFormatInvalidValueCount(function, 6);
			}
			Span<UnitValue> span = outValues2;
			for (int i = 0; i < span.Length; i++)
			{
				UnitValue value = span[i];
				VerifyZeroOrUnit(function, in value, Unit.None);
			}
			Matrix matrix = new Matrix(outValues2[0].Value, outValues2[1].Value, outValues2[2].Value, outValues2[3].Value, outValues2[4].Value, outValues2[5].Value);
			builder.AppendMatrix(matrix);
			break;
		}
		}
		static int ParseCommaDelimitedValues(ReadOnlySpan<char> part, in Span<UnitValue> outValues)
		{
			int num2 = 0;
			while (true)
			{
				if (num2 >= outValues.Length)
				{
					throw new FormatException("Too many provided values.");
				}
				int num3 = part.IndexOf(',');
				if (num3 == -1)
				{
					break;
				}
				ReadOnlySpan<char> part2 = part.Slice(0, num3).Trim();
				outValues[num2++] = ParseValue(part2);
				part = part.Slice(num3 + 1, part.Length - num3 - 1);
			}
			if (!part.IsWhiteSpace())
			{
				outValues[num2++] = ParseValue(part);
			}
			return num2;
		}
		static UnitValue ParseValue(ReadOnlySpan<char> part)
		{
			int num4 = -1;
			for (int j = 0; j < part.Length; j++)
			{
				char c = part[j];
				if (!char.IsDigit(c) && c != '-' && c != '.')
				{
					num4 = j;
					break;
				}
			}
			Unit unit = Unit.None;
			if (num4 != -1)
			{
				unit = ParseUnit(part.Slice(num4, part.Length - num4));
				part = part.Slice(0, num4);
			}
			double value2 = double.Parse(part.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture);
			return new UnitValue(unit, value2);
		}
		static int ParseValuePair(in ReadOnlySpan<char> part, ref UnitValue leftValue, ref UnitValue rightValue)
		{
			int num5 = part.IndexOf(',');
			if (num5 != -1)
			{
				ReadOnlySpan<char> part3 = part.Slice(0, num5).Trim();
				ReadOnlySpan<char> part4 = part.Slice(num5 + 1, part.Length - num5 - 1).Trim();
				leftValue = ParseValue(part3);
				rightValue = ParseValue(part4);
				return 2;
			}
			leftValue = ParseValue(part);
			return 1;
		}
	}

	private static void VerifyZeroOrUnit(TransformFunction function, in UnitValue value, Unit unit)
	{
		if ((value.Unit != 0 || value.Value != 0.0) && value.Unit != unit)
		{
			ThrowFormatInvalidValue(function, in value);
		}
	}

	private static void VerifyZeroOrAngle(TransformFunction function, in UnitValue value)
	{
		if (value.Value != 0.0 && !IsAngleUnit(value.Unit))
		{
			ThrowFormatInvalidValue(function, in value);
		}
	}

	private static bool IsAngleUnit(Unit unit)
	{
		if ((uint)(unit - 2) <= 3u)
		{
			return true;
		}
		return false;
	}

	private static void ThrowFormatInvalidValue(TransformFunction function, in UnitValue value)
	{
		string value2 = ((value.Unit == Unit.None) ? string.Empty : value.Unit.ToString());
		throw new FormatException($"Invalid value {value.Value} {value2} for {function}");
	}

	private static void ThrowFormatInvalidValueCount(TransformFunction function, int count)
	{
		throw new FormatException($"Invalid format. {function} expects {count} value(s).");
	}

	private static Unit ParseUnit(in ReadOnlySpan<char> part)
	{
		(string, Unit)[] array = s_unitMapping;
		for (int i = 0; i < array.Length; i++)
		{
			var (text, result) = array[i];
			if (MemoryExtensions.Equals(part, text.AsSpan(), StringComparison.OrdinalIgnoreCase))
			{
				return result;
			}
		}
		throw new FormatException("Invalid unit: " + part);
	}

	private static TransformFunction ParseTransformFunction(in ReadOnlySpan<char> part)
	{
		(string, TransformFunction)[] array = s_functionMapping;
		for (int i = 0; i < array.Length; i++)
		{
			var (text, result) = array[i];
			if (MemoryExtensions.Equals(part, text.AsSpan(), StringComparison.OrdinalIgnoreCase))
			{
				return result;
			}
		}
		return TransformFunction.Invalid;
	}

	private static double ToRadians(in UnitValue value)
	{
		return value.Unit switch
		{
			Unit.Radian => value.Value, 
			Unit.Gradian => MathUtilities.Grad2Rad(value.Value), 
			Unit.Degree => MathUtilities.Deg2Rad(value.Value), 
			Unit.Turn => MathUtilities.Turn2Rad(value.Value), 
			_ => value.Value, 
		};
	}
}
