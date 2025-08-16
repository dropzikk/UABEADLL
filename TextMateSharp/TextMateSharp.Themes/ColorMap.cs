using System.Collections.Generic;

namespace TextMateSharp.Themes;

public class ColorMap
{
	private int _lastColorId;

	private Dictionary<string, int?> _color2id;

	public ColorMap()
	{
		_lastColorId = 0;
		_color2id = new Dictionary<string, int?>();
	}

	public int GetId(string color)
	{
		if (color == null)
		{
			return 0;
		}
		color = color.ToUpper();
		_color2id.TryGetValue(color, out var value);
		if (value.HasValue)
		{
			return value.Value;
		}
		value = (_lastColorId += 1);
		_color2id[color] = value;
		return value.Value;
	}

	public string GetColor(int id)
	{
		foreach (string color in _color2id.Keys)
		{
			if (_color2id[color].Value == id)
			{
				return color;
			}
		}
		return null;
	}

	public ICollection<string> GetColorMap()
	{
		return _color2id.Keys;
	}

	public override int GetHashCode()
	{
		return _color2id.GetHashCode() + _lastColorId.GetHashCode();
	}

	public bool equals(object obj)
	{
		if (this == obj)
		{
			return true;
		}
		if (obj == null)
		{
			return false;
		}
		if (GetType() != obj.GetType())
		{
			return false;
		}
		ColorMap other = (ColorMap)obj;
		if (object.Equals(_color2id, other._color2id))
		{
			return _lastColorId == other._lastColorId;
		}
		return false;
	}
}
