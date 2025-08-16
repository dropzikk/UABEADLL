using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Utilities;

namespace Avalonia.Input;

public sealed class KeyGesture : IEquatable<KeyGesture>
{
	private static readonly Dictionary<string, Key> s_keySynonyms = new Dictionary<string, Key>
	{
		{
			"+",
			Key.OemPlus
		},
		{
			"-",
			Key.OemMinus
		},
		{
			".",
			Key.OemPeriod
		},
		{
			",",
			Key.OemComma
		}
	};

	public Key Key { get; }

	public KeyModifiers KeyModifiers { get; }

	public KeyGesture(Key key, KeyModifiers modifiers = KeyModifiers.None)
	{
		Key = key;
		KeyModifiers = modifiers;
	}

	public bool Equals(KeyGesture? other)
	{
		if ((object)other == null)
		{
			return false;
		}
		if ((object)this == other)
		{
			return true;
		}
		if (Key == other.Key)
		{
			return KeyModifiers == other.KeyModifiers;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj is KeyGesture other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((int)Key * 397) ^ (int)KeyModifiers;
	}

	public static bool operator ==(KeyGesture? left, KeyGesture? right)
	{
		return object.Equals(left, right);
	}

	public static bool operator !=(KeyGesture? left, KeyGesture? right)
	{
		return !object.Equals(left, right);
	}

	public static KeyGesture Parse(string gesture)
	{
		Key key = Key.None;
		KeyModifiers keyModifiers = KeyModifiers.None;
		int num = 0;
		for (int i = 0; i <= gesture.Length; i++)
		{
			char c = ((i != gesture.Length) ? gesture[i] : '\0');
			bool flag = i == gesture.Length;
			if (flag || (c == '+' && num != i))
			{
				ReadOnlySpan<char> modifier = gesture.AsSpan(num, i - num).Trim();
				if (flag)
				{
					key = ParseKey(modifier.ToString());
				}
				else
				{
					keyModifiers |= ParseModifier(modifier);
				}
				num = i + 1;
			}
		}
		return new KeyGesture(key, keyModifiers);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = StringBuilderCache.Acquire();
		if (KeyModifiers.HasAllFlags(KeyModifiers.Control))
		{
			stringBuilder.Append("Ctrl");
		}
		if (KeyModifiers.HasAllFlags(KeyModifiers.Shift))
		{
			Plus(stringBuilder);
			stringBuilder.Append("Shift");
		}
		if (KeyModifiers.HasAllFlags(KeyModifiers.Alt))
		{
			Plus(stringBuilder);
			stringBuilder.Append("Alt");
		}
		if (KeyModifiers.HasAllFlags(KeyModifiers.Meta))
		{
			Plus(stringBuilder);
			stringBuilder.Append("Cmd");
		}
		Plus(stringBuilder);
		stringBuilder.Append(Key);
		return StringBuilderCache.GetStringAndRelease(stringBuilder);
		static void Plus(StringBuilder s)
		{
			if (s.Length > 0)
			{
				s.Append("+");
			}
		}
	}

	public bool Matches(KeyEventArgs? keyEvent)
	{
		if (keyEvent != null && keyEvent.KeyModifiers == KeyModifiers)
		{
			return ResolveNumPadOperationKey(keyEvent.Key) == ResolveNumPadOperationKey(Key);
		}
		return false;
	}

	private static Key ParseKey(string key)
	{
		if (s_keySynonyms.TryGetValue(key.ToLower(CultureInfo.InvariantCulture), out var value))
		{
			return value;
		}
		return EnumHelper.Parse<Key>(key, ignoreCase: true);
	}

	private static KeyModifiers ParseModifier(ReadOnlySpan<char> modifier)
	{
		if (MemoryExtensions.Equals(modifier, "ctrl".AsSpan(), StringComparison.OrdinalIgnoreCase))
		{
			return KeyModifiers.Control;
		}
		if (MemoryExtensions.Equals(modifier, "cmd".AsSpan(), StringComparison.OrdinalIgnoreCase) || MemoryExtensions.Equals(modifier, "win".AsSpan(), StringComparison.OrdinalIgnoreCase) || MemoryExtensions.Equals(modifier, "⌘".AsSpan(), StringComparison.OrdinalIgnoreCase))
		{
			return KeyModifiers.Meta;
		}
		return EnumHelper.Parse<KeyModifiers>(modifier.ToString(), ignoreCase: true);
	}

	private static Key ResolveNumPadOperationKey(Key key)
	{
		return key switch
		{
			Key.Add => Key.OemPlus, 
			Key.Subtract => Key.OemMinus, 
			Key.Decimal => Key.OemPeriod, 
			_ => key, 
		};
	}
}
