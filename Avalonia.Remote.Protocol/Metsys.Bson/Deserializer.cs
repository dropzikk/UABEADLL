using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Metsys.Bson;

[RequiresUnreferencedCode("Bson uses reflection")]
internal class Deserializer
{
	internal class Options
	{
		public bool LongIntegers { get; set; }

		public bool StringDates { get; set; }
	}

	private static readonly IDictionary<Types, Type> _typeMap = new Dictionary<Types, Type>
	{
		{
			Types.Int32,
			typeof(int)
		},
		{
			Types.Int64,
			typeof(long)
		},
		{
			Types.Boolean,
			typeof(bool)
		},
		{
			Types.String,
			typeof(string)
		},
		{
			Types.Double,
			typeof(double)
		},
		{
			Types.Binary,
			typeof(byte[])
		},
		{
			Types.Regex,
			typeof(Regex)
		},
		{
			Types.DateTime,
			typeof(DateTime)
		},
		{
			Types.ObjectId,
			typeof(ObjectId)
		},
		{
			Types.Array,
			typeof(List<object>)
		},
		{
			Types.Object,
			typeof(Dictionary<string, object>)
		},
		{
			Types.Null,
			null
		}
	};

	private readonly BinaryReader _reader;

	private Document _current;

	private Deserializer(BinaryReader reader)
	{
		_reader = reader;
	}

	public static T Deserialize<T>(byte[] objectData, Options options = null) where T : class
	{
		using MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(objectData, 0, objectData.Length);
		memoryStream.Position = 0L;
		return Deserialize<T>(new BinaryReader(memoryStream), options ?? new Options());
	}

	private static T Deserialize<T>(BinaryReader stream, Options options)
	{
		return new Deserializer(stream).Read<T>(options);
	}

	private T Read<T>(Options options)
	{
		NewDocument(_reader.ReadInt32());
		return (T)DeserializeValue(typeof(T), Types.Object, options);
	}

	public static object Deserialize(BinaryReader stream, Type t, Options options = null)
	{
		return new Deserializer(stream).Read(t, options ?? new Options());
	}

	private object Read(Type t, Options options)
	{
		NewDocument(_reader.ReadInt32());
		return DeserializeValue(t, Types.Object, options);
	}

	private void Read(int read)
	{
		_current.Digested += read;
	}

	private bool IsDone()
	{
		bool num = _current.Digested + 1 == _current.Length;
		if (num)
		{
			_reader.ReadByte();
			Document current = _current;
			_current = current.Parent;
			if (_current != null)
			{
				Read(current.Length);
			}
		}
		return num;
	}

	private void NewDocument(int length)
	{
		Document current = _current;
		_current = new Document
		{
			Length = length,
			Parent = current,
			Digested = 4
		};
	}

	private object DeserializeValue(Type type, Types storedType, Options options)
	{
		return DeserializeValue(type, storedType, null, options);
	}

	private object DeserializeValue(Type type, Types storedType, object container, Options options)
	{
		if (storedType == Types.Null)
		{
			return null;
		}
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
		{
			type = Nullable.GetUnderlyingType(type);
		}
		if (type == typeof(string))
		{
			return ReadString();
		}
		if (type == typeof(int))
		{
			int num = ReadInt(storedType);
			if (!options.LongIntegers)
			{
				return num;
			}
			return (long)num;
		}
		if (type.IsEnum)
		{
			return ReadEnum(type, storedType);
		}
		if (type == typeof(float))
		{
			Read(8);
			return (float)_reader.ReadDouble();
		}
		if (storedType == Types.Binary)
		{
			return ReadBinary();
		}
		if (typeof(IEnumerable).IsAssignableFrom(type))
		{
			return ReadList(type, container, options);
		}
		if (type == typeof(bool))
		{
			Read(1);
			return _reader.ReadBoolean();
		}
		if (type == typeof(DateTime))
		{
			DateTime dateTime = Helper.Epoch.AddMilliseconds(ReadLong(Types.Int64));
			if (!options.StringDates)
			{
				return dateTime;
			}
			return dateTime.ToString("s", CultureInfo.InvariantCulture);
		}
		if (type == typeof(ObjectId))
		{
			Read(12);
			return new ObjectId(_reader.ReadBytes(12));
		}
		if (type == typeof(long))
		{
			return ReadLong(storedType);
		}
		if (type == typeof(double))
		{
			Read(8);
			return _reader.ReadDouble();
		}
		if (type == typeof(Regex))
		{
			return ReadRegularExpression();
		}
		if (type == typeof(ScopedCode))
		{
			return ReadScopedCode(options);
		}
		return ReadObject(type, options);
	}

	private object ReadObject(Type type, Options options)
	{
		object obj = Activator.CreateInstance(type, nonPublic: true);
		TypeHelper helperForType = TypeHelper.GetHelperForType(type);
		do
		{
			Types types = ReadType();
			string text = ReadName();
			bool flag = false;
			if (types == Types.Object)
			{
				int num = _reader.ReadInt32();
				if (num == 5)
				{
					_reader.ReadByte();
					Read(5);
					flag = true;
				}
				else
				{
					NewDocument(num);
				}
			}
			object obj2 = null;
			MagicProperty magicProperty = helperForType.FindProperty(text);
			Type value;
			Type type2 = magicProperty?.Type ?? (_typeMap.TryGetValue(types, out value) ? value : null) ?? typeof(object);
			if (magicProperty != null && magicProperty.Setter == null)
			{
				obj2 = magicProperty.Getter(obj);
			}
			object obj3 = (flag ? null : DeserializeValue(type2, types, obj2, options));
			if (magicProperty == null)
			{
				if (helperForType.Expando != null)
				{
					((IDictionary<string, object>)helperForType.Expando.Getter(obj))[text] = obj3;
				}
			}
			else if (obj2 == null && obj3 != null && !magicProperty.Ignored)
			{
				magicProperty.Setter(obj, obj3);
			}
		}
		while (!IsDone());
		return obj;
	}

	private object ReadList(Type listType, object existingContainer, Options options)
	{
		if (IsDictionary(listType))
		{
			return ReadDictionary(listType, existingContainer, options);
		}
		NewDocument(_reader.ReadInt32());
		Type listItemType = ListHelper.GetListItemType(listType);
		bool flag = typeof(object) == listItemType;
		BaseWrapper baseWrapper = BaseWrapper.Create(listType, listItemType, existingContainer);
		while (!IsDone())
		{
			Types types = ReadType();
			ReadName();
			if (types == Types.Object)
			{
				NewDocument(_reader.ReadInt32());
			}
			Type type = (flag ? _typeMap[types] : listItemType);
			object value = DeserializeValue(type, types, options);
			baseWrapper.Add(value);
		}
		return baseWrapper.Collection;
	}

	private static bool IsDictionary(Type type)
	{
		List<Type> list = new List<Type>(type.GetInterfaces());
		list.Insert(0, type);
		foreach (Type item in list)
		{
			if (item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IDictionary<, >))
			{
				return true;
			}
		}
		return false;
	}

	private object ReadDictionary(Type listType, object existingContainer, Options options)
	{
		Type dictionaryValueType = ListHelper.GetDictionaryValueType(listType);
		bool flag = typeof(object) == dictionaryValueType;
		IDictionary dictionary = ((existingContainer == null) ? ListHelper.CreateDictionary(listType, ListHelper.GetDictionaryKeyType(listType), dictionaryValueType) : ((IDictionary)existingContainer));
		while (!IsDone())
		{
			Types types = ReadType();
			string key = ReadName();
			if (types == Types.Object)
			{
				NewDocument(_reader.ReadInt32());
			}
			Type type = (flag ? _typeMap[types] : dictionaryValueType);
			object value = DeserializeValue(type, types, options);
			dictionary.Add(key, value);
		}
		return dictionary;
	}

	private object ReadBinary()
	{
		int num = _reader.ReadInt32();
		byte b = _reader.ReadByte();
		Read(5 + num);
		return b switch
		{
			2 => _reader.ReadBytes(_reader.ReadInt32()), 
			3 => new Guid(_reader.ReadBytes(num)), 
			_ => throw new BsonException("No support for binary type: " + b), 
		};
	}

	private string ReadName()
	{
		List<byte> list = new List<byte>(128);
		byte item;
		while ((item = _reader.ReadByte()) > 0)
		{
			list.Add(item);
		}
		Read(list.Count + 1);
		return Encoding.UTF8.GetString(list.ToArray());
	}

	private string ReadString()
	{
		int num = _reader.ReadInt32();
		byte[] bytes = _reader.ReadBytes(num - 1);
		_reader.ReadByte();
		Read(4 + num);
		return Encoding.UTF8.GetString(bytes);
	}

	private int ReadInt(Types storedType)
	{
		switch (storedType)
		{
		case Types.Int32:
			Read(4);
			return _reader.ReadInt32();
		case Types.Int64:
			Read(8);
			return (int)_reader.ReadInt64();
		case Types.Double:
			Read(8);
			return (int)_reader.ReadDouble();
		default:
			throw new BsonException("Could not create an int from " + storedType);
		}
	}

	private long ReadLong(Types storedType)
	{
		switch (storedType)
		{
		case Types.Int32:
			Read(4);
			return _reader.ReadInt32();
		case Types.Int64:
			Read(8);
			return _reader.ReadInt64();
		case Types.Double:
			Read(8);
			return (long)_reader.ReadDouble();
		default:
			throw new BsonException("Could not create an int64 from " + storedType);
		}
	}

	private object ReadEnum(Type type, Types storedType)
	{
		if (storedType == Types.Int64)
		{
			return Enum.Parse(type, ReadLong(storedType).ToString(), ignoreCase: false);
		}
		return Enum.Parse(type, ReadInt(storedType).ToString(), ignoreCase: false);
	}

	private object ReadRegularExpression()
	{
		string pattern = ReadName();
		string text = ReadName();
		RegexOptions regexOptions = RegexOptions.Compiled;
		if (text.Contains('e'))
		{
			regexOptions |= RegexOptions.ECMAScript;
		}
		if (text.Contains('i'))
		{
			regexOptions |= RegexOptions.IgnoreCase;
		}
		if (text.Contains('l'))
		{
			regexOptions |= RegexOptions.CultureInvariant;
		}
		if (text.Contains('m'))
		{
			regexOptions |= RegexOptions.Multiline;
		}
		if (text.Contains('s'))
		{
			regexOptions |= RegexOptions.Singleline;
		}
		if (text.Contains('w'))
		{
			regexOptions |= RegexOptions.IgnorePatternWhitespace;
		}
		if (text.Contains('x'))
		{
			regexOptions |= RegexOptions.ExplicitCapture;
		}
		return new Regex(pattern, regexOptions);
	}

	private Types ReadType()
	{
		Read(1);
		return (Types)_reader.ReadByte();
	}

	private ScopedCode ReadScopedCode(Options options)
	{
		_reader.ReadInt32();
		Read(4);
		string codeString = ReadString();
		NewDocument(_reader.ReadInt32());
		return new ScopedCode
		{
			CodeString = codeString,
			Scope = DeserializeValue(typeof(object), Types.Object, options)
		};
	}
}
