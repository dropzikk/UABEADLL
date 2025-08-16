using System;
using System.Globalization;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccTextDescriptionTagDataEntry : IccTagDataEntry, IEquatable<IccTextDescriptionTagDataEntry>
{
	public string Ascii { get; }

	public string Unicode { get; }

	public string ScriptCode { get; }

	public uint UnicodeLanguageCode { get; }

	public ushort ScriptCodeCode { get; }

	public IccTextDescriptionTagDataEntry(string ascii, string unicode, string scriptCode, uint unicodeLanguageCode, ushort scriptCodeCode)
		: this(ascii, unicode, scriptCode, unicodeLanguageCode, scriptCodeCode, IccProfileTag.Unknown)
	{
	}

	public IccTextDescriptionTagDataEntry(string ascii, string unicode, string scriptCode, uint unicodeLanguageCode, ushort scriptCodeCode, IccProfileTag tagSignature)
		: base(IccTypeSignature.TextDescription, tagSignature)
	{
		Ascii = ascii;
		Unicode = unicode;
		ScriptCode = scriptCode;
		UnicodeLanguageCode = unicodeLanguageCode;
		ScriptCodeCode = scriptCodeCode;
	}

	public static explicit operator IccMultiLocalizedUnicodeTagDataEntry(IccTextDescriptionTagDataEntry textEntry)
	{
		if (textEntry == null)
		{
			return null;
		}
		IccLocalizedString iccLocalizedString;
		if (string.IsNullOrEmpty(textEntry.Unicode))
		{
			iccLocalizedString = ((!string.IsNullOrEmpty(textEntry.Ascii)) ? new IccLocalizedString(textEntry.Ascii) : (string.IsNullOrEmpty(textEntry.ScriptCode) ? new IccLocalizedString(string.Empty) : new IccLocalizedString(textEntry.ScriptCode)));
		}
		else
		{
			CultureInfo cultureInfo = GetCulture(textEntry.UnicodeLanguageCode);
			iccLocalizedString = ((cultureInfo != null) ? new IccLocalizedString(cultureInfo, textEntry.Unicode) : new IccLocalizedString(textEntry.Unicode));
		}
		return new IccMultiLocalizedUnicodeTagDataEntry(new IccLocalizedString[1] { iccLocalizedString }, textEntry.TagSignature);
		static CultureInfo GetCulture(uint value)
		{
			if (value == 0)
			{
				return null;
			}
			byte b = (byte)(value >> 24);
			byte b2 = (byte)(value >> 16);
			byte b3 = (byte)(value >> 8);
			byte b4 = (byte)value;
			if (b >= 97 && b <= 122 && b2 >= 97 && b2 <= 122 && b3 >= 65 && b3 <= 90 && b4 >= 65 && b4 <= 90)
			{
				return new CultureInfo(new string(new char[5]
				{
					(char)b,
					(char)b2,
					'-',
					(char)b3,
					(char)b4
				}));
			}
			return null;
		}
	}

	public override bool Equals(IccTagDataEntry other)
	{
		if (other is IccTextDescriptionTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccTextDescriptionTagDataEntry other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (base.Equals(other) && string.Equals(Ascii, other.Ascii, StringComparison.OrdinalIgnoreCase) && string.Equals(Unicode, other.Unicode, StringComparison.OrdinalIgnoreCase) && string.Equals(ScriptCode, other.ScriptCode, StringComparison.OrdinalIgnoreCase) && UnicodeLanguageCode == other.UnicodeLanguageCode)
		{
			return ScriptCodeCode == other.ScriptCodeCode;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is IccTextDescriptionTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, Ascii, Unicode, ScriptCode, UnicodeLanguageCode, ScriptCodeCode);
	}
}
