using System;

namespace Metsys.Bson;

internal class ObjectId
{
	private string _string;

	public static ObjectId Empty => new ObjectId("000000000000000000000000");

	public byte[] Value { get; private set; }

	public ObjectId()
	{
	}

	public ObjectId(string value)
		: this(DecodeHex(value))
	{
	}

	internal ObjectId(byte[] value)
	{
		Value = value;
	}

	public static ObjectId NewObjectId()
	{
		return new ObjectId
		{
			Value = ObjectIdGenerator.Generate()
		};
	}

	public static bool TryParse(string value, out ObjectId id)
	{
		id = Empty;
		if (value == null || value.Length != 24)
		{
			return false;
		}
		try
		{
			id = new ObjectId(value);
			return true;
		}
		catch (FormatException)
		{
			return false;
		}
	}

	public static bool operator ==(ObjectId a, ObjectId b)
	{
		if ((object)a == b)
		{
			return true;
		}
		if ((object)a == null || (object)b == null)
		{
			return false;
		}
		return a.Equals(b);
	}

	public static bool operator !=(ObjectId a, ObjectId b)
	{
		return !(a == b);
	}

	public override int GetHashCode()
	{
		if (Value == null)
		{
			return 0;
		}
		return ToString().GetHashCode();
	}

	public override string ToString()
	{
		if (_string == null && Value != null)
		{
			_string = BitConverter.ToString(Value).Replace("-", string.Empty).ToLowerInvariant();
		}
		return _string;
	}

	public override bool Equals(object o)
	{
		ObjectId other = o as ObjectId;
		return Equals(other);
	}

	public bool Equals(ObjectId other)
	{
		if (other != null)
		{
			return ToString() == other.ToString();
		}
		return false;
	}

	protected static byte[] DecodeHex(string val)
	{
		char[] array = val.ToCharArray();
		int num = array.Length;
		byte[] array2 = new byte[num / 2];
		for (int i = 0; i < num; i += 2)
		{
			array2[i / 2] = Convert.ToByte(new string(array, i, 2), 16);
		}
		return array2;
	}

	public static implicit operator string(ObjectId oid)
	{
		if (!(oid == null))
		{
			return oid.ToString();
		}
		return null;
	}

	public static implicit operator ObjectId(string oidString)
	{
		return new ObjectId(oidString);
	}
}
