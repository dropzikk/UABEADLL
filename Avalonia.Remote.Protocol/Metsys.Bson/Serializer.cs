using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Metsys.Bson;

[RequiresUnreferencedCode("Bson uses reflection")]
internal class Serializer
{
	private static readonly IDictionary<Type, Types> _typeMap = new Dictionary<Type, Types>
	{
		{
			typeof(int),
			Types.Int32
		},
		{
			typeof(long),
			Types.Int64
		},
		{
			typeof(bool),
			Types.Boolean
		},
		{
			typeof(string),
			Types.String
		},
		{
			typeof(double),
			Types.Double
		},
		{
			typeof(Guid),
			Types.Binary
		},
		{
			typeof(Regex),
			Types.Regex
		},
		{
			typeof(DateTime),
			Types.DateTime
		},
		{
			typeof(float),
			Types.Double
		},
		{
			typeof(byte[]),
			Types.Binary
		},
		{
			typeof(ObjectId),
			Types.ObjectId
		},
		{
			typeof(ScopedCode),
			Types.ScopedCode
		}
	};

	private readonly BinaryWriter _writer;

	private Document _current;

	public static byte[] Serialize<T>(T document)
	{
		Type type = document.GetType();
		if (type.IsValueType || (typeof(IEnumerable).IsAssignableFrom(type) && !typeof(IDictionary).IsAssignableFrom(type)))
		{
			throw new BsonException("Root type must be an object");
		}
		using MemoryStream memoryStream = new MemoryStream(250);
		using BinaryWriter writer = new BinaryWriter(memoryStream);
		new Serializer(writer).WriteDocument(document);
		return memoryStream.ToArray();
	}

	public static byte[] Serialize(object document)
	{
		Type type = document.GetType();
		if (type.IsValueType || (typeof(IEnumerable).IsAssignableFrom(type) && !typeof(IDictionary).IsAssignableFrom(type)))
		{
			throw new BsonException("Root type must be an object");
		}
		using MemoryStream memoryStream = new MemoryStream(250);
		using BinaryWriter writer = new BinaryWriter(memoryStream);
		new Serializer(writer).WriteDocument(document);
		return memoryStream.ToArray();
	}

	private Serializer(BinaryWriter writer)
	{
		_writer = writer;
	}

	private void NewDocument()
	{
		Document current = _current;
		_current = new Document
		{
			Parent = current,
			Length = (int)_writer.BaseStream.Position,
			Digested = 4
		};
		_writer.Write(0);
	}

	private void EndDocument(bool includeEeo)
	{
		Document current = _current;
		if (includeEeo)
		{
			Written(1);
			_writer.Write((byte)0);
		}
		_writer.Seek(_current.Length, SeekOrigin.Begin);
		_writer.Write(_current.Digested);
		_writer.Seek(0, SeekOrigin.End);
		_current = _current.Parent;
		if (_current != null)
		{
			Written(current.Digested);
		}
	}

	private void Written(int length)
	{
		_current.Digested += length;
	}

	private void WriteDocument(object document)
	{
		NewDocument();
		WriteObject(document);
		EndDocument(includeEeo: true);
	}

	private void WriteObject(object document)
	{
		if (document is IDictionary dictionary)
		{
			Write(dictionary);
			return;
		}
		foreach (MagicProperty property in TypeHelper.GetHelperForType(document.GetType()).GetProperties())
		{
			if (!property.Ignored)
			{
				string name = property.Name;
				object obj = property.Getter(document);
				if (obj != null || !property.IgnoredIfNull)
				{
					SerializeMember(name, obj);
				}
			}
		}
	}

	private void SerializeMember(string name, object value)
	{
		if (value == null)
		{
			Write(Types.Null);
			WriteName(name);
			return;
		}
		Type type = value.GetType();
		if (type.IsEnum)
		{
			type = Enum.GetUnderlyingType(type);
		}
		if (!_typeMap.TryGetValue(type, out var value2))
		{
			Write(name, value);
			return;
		}
		Write(value2);
		WriteName(name);
		switch (value2)
		{
		case Types.Int32:
			Written(4);
			_writer.Write((int)value);
			break;
		case Types.Int64:
			Written(8);
			_writer.Write((long)value);
			break;
		case Types.String:
			Write((string)value);
			break;
		case Types.Double:
			Written(8);
			if (value is float)
			{
				_writer.Write(Convert.ToDouble((float)value));
			}
			else
			{
				_writer.Write((double)value);
			}
			break;
		case Types.Boolean:
			Written(1);
			_writer.Write((byte)(((bool)value) ? 1 : 0));
			break;
		case Types.DateTime:
			Written(8);
			_writer.Write((long)((DateTime)value).ToUniversalTime().Subtract(Helper.Epoch).TotalMilliseconds);
			break;
		case Types.Binary:
			WriteBinary(value);
			break;
		case Types.ScopedCode:
			Write((ScopedCode)value);
			break;
		case Types.ObjectId:
			Written(((ObjectId)value).Value.Length);
			_writer.Write(((ObjectId)value).Value);
			break;
		case Types.Regex:
			Write((Regex)value);
			break;
		case Types.Object:
		case Types.Array:
		case Types.Undefined:
		case Types.Null:
		case Types.Reference:
		case Types.Code:
		case Types.Symbol:
		case Types.Timestamp:
			break;
		}
	}

	private void Write(string name, object value)
	{
		if (value is IDictionary)
		{
			Write(Types.Object);
			WriteName(name);
			NewDocument();
			Write((IDictionary)value);
			EndDocument(includeEeo: true);
		}
		else if (value is IEnumerable)
		{
			Write(Types.Array);
			WriteName(name);
			NewDocument();
			Write((IEnumerable)value);
			EndDocument(includeEeo: true);
		}
		else
		{
			Write(Types.Object);
			WriteName(name);
			WriteDocument(value);
		}
	}

	private void Write(IEnumerable enumerable)
	{
		int num = 0;
		foreach (object item in enumerable)
		{
			SerializeMember(num++.ToString(), item);
		}
	}

	private void Write(IDictionary dictionary)
	{
		foreach (object key in dictionary.Keys)
		{
			SerializeMember((string)key, dictionary[key]);
		}
	}

	private void WriteBinary(object value)
	{
		if (value is byte[])
		{
			byte[] array = (byte[])value;
			int num = array.Length;
			_writer.Write(num + 4);
			_writer.Write((byte)2);
			_writer.Write(num);
			_writer.Write(array);
			Written(9 + num);
		}
		else if (value is Guid guid)
		{
			byte[] array2 = guid.ToByteArray();
			_writer.Write(array2.Length);
			_writer.Write((byte)3);
			_writer.Write(array2);
			Written(5 + array2.Length);
		}
	}

	private void Write(Types type)
	{
		_writer.Write((byte)type);
		Written(1);
	}

	private void WriteName(string name)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(name);
		_writer.Write(bytes);
		_writer.Write((byte)0);
		Written(bytes.Length + 1);
	}

	private void Write(string name)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(name);
		_writer.Write(bytes.Length + 1);
		_writer.Write(bytes);
		_writer.Write((byte)0);
		Written(bytes.Length + 5);
	}

	private void Write(Regex regex)
	{
		WriteName(regex.ToString());
		string text = string.Empty;
		if ((regex.Options & RegexOptions.ECMAScript) == RegexOptions.ECMAScript)
		{
			text += 'e';
		}
		if ((regex.Options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase)
		{
			text += 'i';
		}
		if ((regex.Options & RegexOptions.CultureInvariant) == RegexOptions.CultureInvariant)
		{
			text += 'l';
		}
		if ((regex.Options & RegexOptions.Multiline) == RegexOptions.Multiline)
		{
			text += 'm';
		}
		if ((regex.Options & RegexOptions.Singleline) == RegexOptions.Singleline)
		{
			text += 's';
		}
		text += 'u';
		if ((regex.Options & RegexOptions.IgnorePatternWhitespace) == RegexOptions.IgnorePatternWhitespace)
		{
			text += 'w';
		}
		if ((regex.Options & RegexOptions.ExplicitCapture) == RegexOptions.ExplicitCapture)
		{
			text += 'x';
		}
		WriteName(text);
	}

	private void Write(ScopedCode value)
	{
		NewDocument();
		Write(value.CodeString);
		WriteDocument(value.Scope);
		EndDocument(includeEeo: false);
	}
}
