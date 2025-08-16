using System;
using System.Text;

namespace SixLabors.ImageSharp.Metadata.Profiles.Iptc;

public sealed class IptcValue : IDeepCloneable<IptcValue>
{
	private byte[] data = Array.Empty<byte>();

	private Encoding encoding;

	public Encoding Encoding
	{
		get
		{
			return encoding;
		}
		set
		{
			if (value != null)
			{
				encoding = value;
			}
		}
	}

	public IptcTag Tag { get; }

	public bool Strict { get; set; }

	public string Value
	{
		get
		{
			return encoding.GetString(data);
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				data = Array.Empty<byte>();
				return;
			}
			int num = Tag.MaxLength();
			byte[] bytes;
			if (Strict && value.Length > num)
			{
				string s = value.Substring(0, num);
				bytes = encoding.GetBytes(s);
				if (bytes.Length > num)
				{
					throw new ArgumentException($"The iptc value exceeds the limit of {num} bytes for the tag {Tag}");
				}
			}
			else
			{
				bytes = encoding.GetBytes(value);
			}
			data = bytes;
		}
	}

	public int Length => data.Length;

	internal IptcValue(IptcValue other)
	{
		if (other.data != null)
		{
			data = new byte[other.data.Length];
			other.data.AsSpan().CopyTo(data);
		}
		encoding = (Encoding)other.Encoding.Clone();
		Tag = other.Tag;
		Strict = other.Strict;
	}

	internal IptcValue(IptcTag tag, byte[] value, bool strict)
	{
		Guard.NotNull(value, "value");
		Strict = strict;
		Tag = tag;
		data = value;
		encoding = System.Text.Encoding.UTF8;
	}

	internal IptcValue(IptcTag tag, Encoding encoding, string value, bool strict)
	{
		Strict = strict;
		Tag = tag;
		this.encoding = encoding;
		Value = value;
	}

	internal IptcValue(IptcTag tag, string value, bool strict)
	{
		Strict = strict;
		Tag = tag;
		encoding = System.Text.Encoding.UTF8;
		Value = value;
	}

	public IptcValue DeepClone()
	{
		return new IptcValue(this);
	}

	public override bool Equals(object? obj)
	{
		if (this == obj)
		{
			return true;
		}
		return Equals(obj as IptcValue);
	}

	public bool Equals(IptcValue? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (Tag != other.Tag)
		{
			return false;
		}
		if (data.Length != other.data.Length)
		{
			return false;
		}
		for (int i = 0; i < data.Length; i++)
		{
			if (data[i] != other.data[i])
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(data, Tag);
	}

	public byte[] ToByteArray()
	{
		byte[] array = new byte[data.Length];
		data.CopyTo(array, 0);
		return array;
	}

	public override string ToString()
	{
		return Value;
	}

	public string ToString(Encoding encoding)
	{
		Guard.NotNull(encoding, "encoding");
		return encoding.GetString(data);
	}
}
