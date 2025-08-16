using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Utilities;

namespace Avalonia.Controls.Converters;

public class PlatformKeyGestureConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		if (value is KeyGesture gesture && targetType == typeof(string))
		{
			return ToPlatformString(gesture);
		}
		throw new NotSupportedException();
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public static string ToPlatformString(KeyGesture gesture)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return ToString(gesture, "Win");
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return ToString(gesture, "Super");
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return ToOSXString(gesture);
		}
		return gesture.ToString();
	}

	private static string ToString(KeyGesture gesture, string meta)
	{
		StringBuilder stringBuilder = StringBuilderCache.Acquire();
		if (gesture.KeyModifiers.HasAllFlags(KeyModifiers.Control))
		{
			stringBuilder.Append("Ctrl");
		}
		if (gesture.KeyModifiers.HasAllFlags(KeyModifiers.Shift))
		{
			Plus(stringBuilder);
			stringBuilder.Append("Shift");
		}
		if (gesture.KeyModifiers.HasAllFlags(KeyModifiers.Alt))
		{
			Plus(stringBuilder);
			stringBuilder.Append("Alt");
		}
		if (gesture.KeyModifiers.HasAllFlags(KeyModifiers.Meta))
		{
			Plus(stringBuilder);
			stringBuilder.Append(meta);
		}
		Plus(stringBuilder);
		stringBuilder.Append(ToString(gesture.Key));
		return StringBuilderCache.GetStringAndRelease(stringBuilder);
		static void Plus(StringBuilder s)
		{
			if (s.Length > 0)
			{
				s.Append("+");
			}
		}
	}

	private static string ToOSXString(KeyGesture gesture)
	{
		StringBuilder stringBuilder = StringBuilderCache.Acquire();
		if (gesture.KeyModifiers.HasAllFlags(KeyModifiers.Control))
		{
			stringBuilder.Append('⌃');
		}
		if (gesture.KeyModifiers.HasAllFlags(KeyModifiers.Alt))
		{
			stringBuilder.Append('⌥');
		}
		if (gesture.KeyModifiers.HasAllFlags(KeyModifiers.Shift))
		{
			stringBuilder.Append('⇧');
		}
		if (gesture.KeyModifiers.HasAllFlags(KeyModifiers.Meta))
		{
			stringBuilder.Append('⌘');
		}
		stringBuilder.Append(ToOSXString(gesture.Key));
		return StringBuilderCache.GetStringAndRelease(stringBuilder);
	}

	private static string ToString(Key key)
	{
		return key switch
		{
			Key.Add => "+", 
			Key.Back => "Backspace", 
			Key.D0 => "0", 
			Key.D1 => "1", 
			Key.D2 => "2", 
			Key.D3 => "3", 
			Key.D4 => "4", 
			Key.D5 => "5", 
			Key.D6 => "6", 
			Key.D7 => "7", 
			Key.D8 => "8", 
			Key.D9 => "9", 
			Key.Decimal => ".", 
			Key.Divide => "/", 
			Key.Down => "Down Arrow", 
			Key.Left => "Left Arrow", 
			Key.Multiply => "*", 
			Key.OemBackslash => "\\", 
			Key.OemCloseBrackets => "]", 
			Key.OemComma => ",", 
			Key.OemMinus => "-", 
			Key.OemOpenBrackets => "[", 
			Key.OemPeriod => ".", 
			Key.OemPipe => "|", 
			Key.OemPlus => "+", 
			Key.OemQuestion => "/", 
			Key.OemQuotes => "\"", 
			Key.OemSemicolon => ";", 
			Key.OemTilde => "`", 
			Key.Right => "Right Arrow", 
			Key.Separator => "/", 
			Key.Subtract => "-", 
			Key.Up => "Up Arrow", 
			_ => key.ToString(), 
		};
	}

	private static string ToOSXString(Key key)
	{
		return key switch
		{
			Key.Back => "⌫", 
			Key.Down => "↓", 
			Key.End => "↘", 
			Key.Escape => "⎋", 
			Key.Home => "↖", 
			Key.Left => "←", 
			Key.Return => "↩", 
			Key.PageDown => "⇞", 
			Key.PageUp => "⇟", 
			Key.Right => "→", 
			Key.Space => "␣", 
			Key.Tab => "⇥", 
			Key.Up => "↑", 
			_ => ToString(key), 
		};
	}
}
