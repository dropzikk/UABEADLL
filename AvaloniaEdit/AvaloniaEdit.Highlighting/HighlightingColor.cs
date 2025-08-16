using System;
using System.Globalization;
using System.Text;
using Avalonia.Media;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Highlighting;

public class HighlightingColor : IFreezable, ICloneable, IEquatable<HighlightingColor>
{
	internal static readonly HighlightingColor Empty = FreezableHelper.FreezeAndReturn(new HighlightingColor());

	private string _name;

	private FontFamily _fontFamily;

	private int? _fontSize;

	private FontWeight? _fontWeight;

	private FontStyle? _fontStyle;

	private bool? _underline;

	private bool? _strikethrough;

	private HighlightingBrush _foreground;

	private HighlightingBrush _background;

	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			if (IsFrozen)
			{
				throw new InvalidOperationException();
			}
			_name = value;
		}
	}

	public FontFamily FontFamily
	{
		get
		{
			return _fontFamily;
		}
		set
		{
			if (IsFrozen)
			{
				throw new InvalidOperationException();
			}
			_fontFamily = value;
		}
	}

	public int? FontSize
	{
		get
		{
			return _fontSize;
		}
		set
		{
			if (IsFrozen)
			{
				throw new InvalidOperationException();
			}
			_fontSize = value;
		}
	}

	public FontWeight? FontWeight
	{
		get
		{
			return _fontWeight;
		}
		set
		{
			if (IsFrozen)
			{
				throw new InvalidOperationException();
			}
			_fontWeight = value;
		}
	}

	public FontStyle? FontStyle
	{
		get
		{
			return _fontStyle;
		}
		set
		{
			if (IsFrozen)
			{
				throw new InvalidOperationException();
			}
			_fontStyle = value;
		}
	}

	public bool? Underline
	{
		get
		{
			return _underline;
		}
		set
		{
			if (IsFrozen)
			{
				throw new InvalidOperationException();
			}
			_underline = value;
		}
	}

	public bool? Strikethrough
	{
		get
		{
			return _strikethrough;
		}
		set
		{
			if (IsFrozen)
			{
				throw new InvalidOperationException();
			}
			_strikethrough = value;
		}
	}

	public HighlightingBrush Foreground
	{
		get
		{
			return _foreground;
		}
		set
		{
			if (IsFrozen)
			{
				throw new InvalidOperationException();
			}
			_foreground = value;
		}
	}

	public HighlightingBrush Background
	{
		get
		{
			return _background;
		}
		set
		{
			if (IsFrozen)
			{
				throw new InvalidOperationException();
			}
			_background = value;
		}
	}

	public bool IsFrozen { get; private set; }

	internal bool IsEmptyForMerge
	{
		get
		{
			if (!_fontWeight.HasValue && !_fontStyle.HasValue && !_underline.HasValue && !_strikethrough.HasValue && _foreground == null && _background == null && _fontFamily == null)
			{
				return !_fontSize.HasValue;
			}
			return false;
		}
	}

	public virtual string ToCss()
	{
		StringBuilder stringBuilder = new StringBuilder();
		Color? color = Foreground?.GetColor(null);
		if (color.HasValue)
		{
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "color: #{0:x2}{1:x2}{2:x2}; ", color.Value.R, color.Value.G, color.Value.B);
		}
		if (FontFamily != null)
		{
			stringBuilder.Append("font-family: ");
			stringBuilder.Append(FontFamily.Name.ToLowerInvariant());
			stringBuilder.Append("; ");
		}
		if (FontSize.HasValue)
		{
			stringBuilder.Append("font-size: ");
			stringBuilder.Append(FontSize.Value.ToString());
			stringBuilder.Append("; ");
		}
		if (FontWeight.HasValue)
		{
			stringBuilder.Append("font-weight: ");
			stringBuilder.Append(FontWeight.Value.ToString().ToLowerInvariant());
			stringBuilder.Append("; ");
		}
		if (FontStyle.HasValue)
		{
			stringBuilder.Append("font-style: ");
			stringBuilder.Append(FontStyle.Value.ToString().ToLowerInvariant());
			stringBuilder.Append("; ");
		}
		if (Underline.HasValue)
		{
			stringBuilder.Append("text-decoration: ");
			stringBuilder.Append(Underline.Value ? "underline" : "none");
			stringBuilder.Append("; ");
		}
		if (Strikethrough.HasValue)
		{
			if (!Underline.HasValue)
			{
				stringBuilder.Append("text-decoration:  ");
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			stringBuilder.Append(Strikethrough.Value ? " line-through" : " none");
			stringBuilder.Append("; ");
		}
		return stringBuilder.ToString();
	}

	public override string ToString()
	{
		return "[" + GetType().Name + " " + (string.IsNullOrEmpty(Name) ? ToCss() : Name) + "]";
	}

	public virtual void Freeze()
	{
		IsFrozen = true;
	}

	public virtual HighlightingColor Clone()
	{
		HighlightingColor obj = (HighlightingColor)MemberwiseClone();
		obj.IsFrozen = false;
		return obj;
	}

	object ICloneable.Clone()
	{
		return Clone();
	}

	public sealed override bool Equals(object obj)
	{
		return Equals(obj as HighlightingColor);
	}

	public virtual bool Equals(HighlightingColor other)
	{
		if (other == null)
		{
			return false;
		}
		if (_name == other._name && _fontWeight == other._fontWeight && _fontStyle == other._fontStyle && _underline == other._underline && _strikethrough == other._strikethrough && object.Equals(_foreground, other._foreground) && object.Equals(_background, other._background) && object.Equals(_fontFamily, other._fontFamily))
		{
			return object.Equals(_fontSize, other._fontSize);
		}
		return false;
	}

	public override int GetHashCode()
	{
		int num = 0;
		if (_name != null)
		{
			num += 1000000007 * _name.GetHashCode();
		}
		num += 1000000009 * _fontWeight.GetHashCode();
		num += 1000000021 * _fontStyle.GetHashCode();
		if (_foreground != null)
		{
			num += 1000000033 * _foreground.GetHashCode();
		}
		if (_background != null)
		{
			num += 1000000087 * _background.GetHashCode();
		}
		if (_fontFamily != null)
		{
			num += 1000000123 * _fontFamily.GetHashCode();
		}
		if (_fontSize.HasValue)
		{
			num += 1000000167 * _fontSize.GetHashCode();
		}
		return num;
	}

	public void MergeWith(HighlightingColor color)
	{
		FreezableHelper.ThrowIfFrozen(this);
		if (color._fontFamily != null)
		{
			_fontFamily = color._fontFamily;
		}
		if (color._fontSize.HasValue)
		{
			_fontSize = color._fontSize;
		}
		if (color._fontWeight.HasValue)
		{
			_fontWeight = color._fontWeight;
		}
		if (color._fontStyle.HasValue)
		{
			_fontStyle = color._fontStyle;
		}
		if (color._foreground != null)
		{
			_foreground = color._foreground;
		}
		if (color._background != null)
		{
			_background = color._background;
		}
		if (color._underline.HasValue)
		{
			_underline = color._underline;
		}
		if (color._strikethrough.HasValue)
		{
			_strikethrough = color._strikethrough;
		}
	}
}
