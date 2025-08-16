using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Tmds.DBus.Protocol;

public ref struct Reader
{
	private sealed class KeyValueArrayTypeReader<TKey, TValue> : ITypeReader<KeyValuePair<TKey, TValue>[]>, ITypeReader, ITypeReader<object> where TKey : notnull where TValue : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public KeyValuePair<TKey, TValue>[] Read(ref Reader reader)
		{
			return reader.ReadKeyValueArray<TKey, TValue>();
		}
	}

	private sealed class ArrayTypeReader<T> : ITypeReader<T[]>, ITypeReader, ITypeReader<object> where T : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public T[] Read(ref Reader reader)
		{
			return reader.ReadArray<T>();
		}
	}

	private delegate object ValueReader(ref Reader reader);

	private sealed class DictionaryTypeReader<TKey, TValue> : ITypeReader<Dictionary<TKey, TValue>>, ITypeReader, ITypeReader<object> where TKey : notnull where TValue : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public Dictionary<TKey, TValue> Read(ref Reader reader)
		{
			return reader.ReadDictionary<TKey, TValue>();
		}
	}

	private interface ITypeReader
	{
	}

	private interface ITypeReader<T> : ITypeReader
	{
		T Read(ref Reader reader);
	}

	private sealed class ValueTupleTypeReader<T1> : ITypeReader<ValueTuple<T1>>, ITypeReader, ITypeReader<object> where T1 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public ValueTuple<T1> Read(ref Reader reader)
		{
			return reader.ReadStruct<T1>();
		}
	}

	private sealed class TupleTypeReader<T1> : ITypeReader<Tuple<T1>>, ITypeReader, ITypeReader<object> where T1 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public Tuple<T1> Read(ref Reader reader)
		{
			return reader.ReadStructAsTuple<T1>();
		}
	}

	private sealed class ValueTupleTypeReader<T1, T2> : ITypeReader<(T1, T2)>, ITypeReader, ITypeReader<object> where T1 : notnull where T2 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public (T1, T2) Read(ref Reader reader)
		{
			return reader.ReadStruct<T1, T2>();
		}
	}

	private sealed class TupleTypeReader<T1, T2> : ITypeReader<Tuple<T1, T2>>, ITypeReader, ITypeReader<object> where T1 : notnull where T2 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public Tuple<T1, T2> Read(ref Reader reader)
		{
			return reader.ReadStructAsTuple<T1, T2>();
		}
	}

	private sealed class ValueTupleTypeReader<T1, T2, T3> : ITypeReader<(T1, T2, T3)>, ITypeReader, ITypeReader<object> where T1 : notnull where T2 : notnull where T3 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public (T1, T2, T3) Read(ref Reader reader)
		{
			return reader.ReadStruct<T1, T2, T3>();
		}
	}

	private sealed class TupleTypeReader<T1, T2, T3> : ITypeReader<Tuple<T1, T2, T3>>, ITypeReader, ITypeReader<object> where T1 : notnull where T2 : notnull where T3 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public Tuple<T1, T2, T3> Read(ref Reader reader)
		{
			return reader.ReadStructAsTuple<T1, T2, T3>();
		}
	}

	private sealed class ValueTupleTypeReader<T1, T2, T3, T4> : ITypeReader<(T1, T2, T3, T4)>, ITypeReader, ITypeReader<object> where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public (T1, T2, T3, T4) Read(ref Reader reader)
		{
			return reader.ReadStruct<T1, T2, T3, T4>();
		}
	}

	private sealed class TupleTypeReader<T1, T2, T3, T4> : ITypeReader<Tuple<T1, T2, T3, T4>>, ITypeReader, ITypeReader<object> where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public Tuple<T1, T2, T3, T4> Read(ref Reader reader)
		{
			return reader.ReadStructAsTuple<T1, T2, T3, T4>();
		}
	}

	private sealed class ValueTupleTypeReader<T1, T2, T3, T4, T5> : ITypeReader<(T1, T2, T3, T4, T5)>, ITypeReader, ITypeReader<object> where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public (T1, T2, T3, T4, T5) Read(ref Reader reader)
		{
			return reader.ReadStruct<T1, T2, T3, T4, T5>();
		}
	}

	private sealed class TupleTypeReader<T1, T2, T3, T4, T5> : ITypeReader<Tuple<T1, T2, T3, T4, T5>>, ITypeReader, ITypeReader<object> where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public Tuple<T1, T2, T3, T4, T5> Read(ref Reader reader)
		{
			return reader.ReadStructAsTuple<T1, T2, T3, T4, T5>();
		}
	}

	private sealed class ValueTupleTypeReader<T1, T2, T3, T4, T5, T6> : ITypeReader<(T1, T2, T3, T4, T5, T6)>, ITypeReader, ITypeReader<object> where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public (T1, T2, T3, T4, T5, T6) Read(ref Reader reader)
		{
			return reader.ReadStruct<T1, T2, T3, T4, T5, T6>();
		}
	}

	private sealed class TupleTypeReader<T1, T2, T3, T4, T5, T6> : ITypeReader<Tuple<T1, T2, T3, T4, T5, T6>>, ITypeReader, ITypeReader<object> where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public Tuple<T1, T2, T3, T4, T5, T6> Read(ref Reader reader)
		{
			return reader.ReadStructAsTuple<T1, T2, T3, T4, T5, T6>();
		}
	}

	private sealed class ValueTupleTypeReader<T1, T2, T3, T4, T5, T6, T7> : ITypeReader<(T1, T2, T3, T4, T5, T6, T7)>, ITypeReader, ITypeReader<object> where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public (T1, T2, T3, T4, T5, T6, T7) Read(ref Reader reader)
		{
			return reader.ReadStruct<T1, T2, T3, T4, T5, T6, T7>();
		}
	}

	private sealed class TupleTypeReader<T1, T2, T3, T4, T5, T6, T7> : ITypeReader<Tuple<T1, T2, T3, T4, T5, T6, T7>>, ITypeReader, ITypeReader<object> where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public Tuple<T1, T2, T3, T4, T5, T6, T7> Read(ref Reader reader)
		{
			return reader.ReadStructAsTuple<T1, T2, T3, T4, T5, T6, T7>();
		}
	}

	private sealed class ValueTupleTypeReader<T1, T2, T3, T4, T5, T6, T7, T8> : ITypeReader<(T1, T2, T3, T4, T5, T6, T7, T8)>, ITypeReader, ITypeReader<object> where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public (T1, T2, T3, T4, T5, T6, T7, T8) Read(ref Reader reader)
		{
			return reader.ReadStruct<T1, T2, T3, T4, T5, T6, T7, T8>();
		}
	}

	private sealed class TupleTypeReader<T1, T2, T3, T4, T5, T6, T7, T8> : ITypeReader<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>>, ITypeReader, ITypeReader<object> where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> Read(ref Reader reader)
		{
			return reader.ReadStructAsTuple<T1, T2, T3, T4, T5, T6, T7, T8>();
		}
	}

	private sealed class ValueTupleTypeReader<T1, T2, T3, T4, T5, T6, T7, T8, T9> : ITypeReader<(T1, T2, T3, T4, T5, T6, T7, T8, T9)>, ITypeReader, ITypeReader<object> where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull where T9 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public (T1, T2, T3, T4, T5, T6, T7, T8, T9) Read(ref Reader reader)
		{
			return reader.ReadStruct<T1, T2, T3, T4, T5, T6, T7, T8, T9>();
		}
	}

	private sealed class TupleTypeReader<T1, T2, T3, T4, T5, T6, T7, T8, T9> : ITypeReader<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>>, ITypeReader, ITypeReader<object> where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull where T9 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>> Read(ref Reader reader)
		{
			return reader.ReadStructAsTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>();
		}
	}

	private sealed class ValueTupleTypeReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : ITypeReader<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)>, ITypeReader, ITypeReader<object> where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull where T9 : notnull where T10 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) Read(ref Reader reader)
		{
			return reader.ReadStruct<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
		}
	}

	private sealed class TupleTypeReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : ITypeReader<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>>, ITypeReader, ITypeReader<object> where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull where T9 : notnull where T10 : notnull
	{
		object ITypeReader<object>.Read(ref Reader reader)
		{
			return Read(ref reader);
		}

		public Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>> Read(ref Reader reader)
		{
			return reader.ReadStructAsTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
		}
	}

	private readonly bool _isBigEndian;

	private readonly UnixFdCollection? _handles;

	private readonly int _handleCount;

	private SequenceReader<byte> _reader;

	private static readonly Dictionary<Type, ITypeReader> _typeReaders = new Dictionary<Type, ITypeReader>();

	internal ReadOnlySequence<byte> UnreadSequence => _reader.Sequence.Slice(_reader.Position);

	public T[] ReadArray<T>()
	{
		List<T> list = new List<T>();
		ArrayEnd iterator = ReadArrayStart(TypeModel.GetTypeAlignment<T>());
		while (HasNext(iterator))
		{
			list.Add(Read<T>());
		}
		return list.ToArray();
	}

	private KeyValuePair<TKey, TValue>[] ReadKeyValueArray<TKey, TValue>()
	{
		List<KeyValuePair<TKey, TValue>> list = new List<KeyValuePair<TKey, TValue>>();
		ArrayEnd iterator = ReadArrayStart(DBusType.Struct);
		while (HasNext(iterator))
		{
			TKey key = Read<TKey>();
			TValue value = Read<TValue>();
			list.Add(new KeyValuePair<TKey, TValue>(key, value));
		}
		return list.ToArray();
	}

	public static void AddArrayTypeReader<T>() where T : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof(T[]);
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new ArrayTypeReader<T>());
			}
		}
	}

	public static void AddKeyValueArrayTypeReader<TKey, TValue>() where TKey : notnull where TValue : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof(KeyValuePair<TKey, TValue>[]);
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new KeyValueArrayTypeReader<TKey, TValue>());
			}
		}
	}

	private ITypeReader CreateArrayTypeReader(Type elementType)
	{
		Type type3;
		if (elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof(KeyValuePair<, >))
		{
			Type type = elementType.GenericTypeArguments[0];
			Type type2 = elementType.GenericTypeArguments[1];
			type3 = typeof(KeyValueArrayTypeReader<, >).MakeGenericType(type, type2);
		}
		else
		{
			type3 = typeof(ArrayTypeReader<>).MakeGenericType(elementType);
		}
		return (ITypeReader)Activator.CreateInstance(type3);
	}

	public byte ReadByte()
	{
		if (!_reader.TryRead(out var value))
		{
			ThrowHelper.ThrowIndexOutOfRange();
		}
		return value;
	}

	public bool ReadBool()
	{
		return ReadInt32() != 0;
	}

	public ushort ReadUInt16()
	{
		return (ushort)ReadInt16();
	}

	public short ReadInt16()
	{
		AlignReader(DBusType.Int16);
		if (!(_isBigEndian ? _reader.TryReadBigEndian(out short value) : _reader.TryReadLittleEndian(out value)))
		{
			ThrowHelper.ThrowIndexOutOfRange();
		}
		return value;
	}

	public uint ReadUInt32()
	{
		return (uint)ReadInt32();
	}

	public int ReadInt32()
	{
		AlignReader(DBusType.Int32);
		if (!(_isBigEndian ? _reader.TryReadBigEndian(out int value) : _reader.TryReadLittleEndian(out value)))
		{
			ThrowHelper.ThrowIndexOutOfRange();
		}
		return value;
	}

	public ulong ReadUInt64()
	{
		return (ulong)ReadInt64();
	}

	public long ReadInt64()
	{
		AlignReader(DBusType.Int64);
		if (!(_isBigEndian ? _reader.TryReadBigEndian(out long value) : _reader.TryReadLittleEndian(out value)))
		{
			ThrowHelper.ThrowIndexOutOfRange();
		}
		return value;
	}

	public unsafe double ReadDouble()
	{
		double result = default(double);
		*(long*)(&result) = ReadInt64();
		return result;
	}

	public Utf8Span ReadSignature()
	{
		int length = ReadByte();
		return ReadSpan(length);
	}

	public void ReadSignature(string expected)
	{
		ReadOnlySpan<byte> span = ReadSignature().Span;
		if (span.Length != expected.Length)
		{
			ThrowHelper.ThrowUnexpectedSignature(span, expected);
		}
		for (int i = 0; i < span.Length; i++)
		{
			if (span[i] != expected[i])
			{
				ThrowHelper.ThrowUnexpectedSignature(span, expected);
			}
		}
	}

	public Utf8Span ReadObjectPathAsSpan()
	{
		return ReadSpan();
	}

	public ObjectPath ReadObjectPath()
	{
		return new ObjectPath(ReadString());
	}

	public ObjectPath ReadObjectPathAsString()
	{
		return ReadString();
	}

	public Utf8Span ReadStringAsSpan()
	{
		return ReadSpan();
	}

	public string ReadString()
	{
		return Encoding.UTF8.GetString(ReadSpan());
	}

	public Signature ReadSignatureAsSignature()
	{
		return new Signature(ReadSignature().ToString());
	}

	public string ReadSignatureAsString()
	{
		return ReadSignature().ToString();
	}

	private ReadOnlySpan<byte> ReadSpan()
	{
		int length = (int)ReadUInt32();
		return ReadSpan(length);
	}

	private ReadOnlySpan<byte> ReadSpan(int length)
	{
		ReadOnlySpan<byte> unreadSpan = _reader.UnreadSpan;
		if (unreadSpan.Length >= length)
		{
			_reader.Advance(length + 1);
			return unreadSpan.Slice(0, length);
		}
		byte[] array = new byte[length];
		if (!_reader.TryCopyTo(array))
		{
			ThrowHelper.ThrowIndexOutOfRange();
		}
		_reader.Advance(length + 1);
		return new ReadOnlySpan<byte>(array);
	}

	internal void Advance(long count)
	{
		_reader.Advance(count);
	}

	internal Reader(bool isBigEndian, ReadOnlySequence<byte> sequence)
		: this(isBigEndian, sequence, null, 0)
	{
	}

	internal Reader(bool isBigEndian, ReadOnlySequence<byte> sequence, UnixFdCollection? handles, int handleCount)
	{
		_reader = new SequenceReader<byte>(sequence);
		_isBigEndian = isBigEndian;
		_handles = handles;
		_handleCount = handleCount;
	}

	public void AlignStruct()
	{
		AlignReader(DBusType.Struct);
	}

	private void AlignReader(DBusType type)
	{
		long num = ProtocolConstants.GetPadding((int)_reader.Consumed, type);
		if (num != 0L)
		{
			_reader.Advance(num);
		}
	}

	public ArrayEnd ReadArrayStart(DBusType elementType)
	{
		uint num = ReadUInt32();
		AlignReader(elementType);
		int endOfArray = (int)(_reader.Consumed + num);
		return new ArrayEnd(elementType, endOfArray);
	}

	public bool HasNext(ArrayEnd iterator)
	{
		int num = (int)_reader.Consumed;
		int num2 = ProtocolConstants.Align(num, iterator.Type);
		if (num2 >= iterator.EndOfArray)
		{
			return false;
		}
		int num3 = num2 - num;
		if (num3 != 0)
		{
			_reader.Advance(num3);
		}
		return true;
	}

	public Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>() where TKey : notnull where TValue : notnull
	{
		Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
		ArrayEnd iterator = ReadArrayStart(DBusType.Struct);
		while (HasNext(iterator))
		{
			TKey key = Read<TKey>();
			TValue value = Read<TValue>();
			dictionary.Add(key, value);
		}
		return dictionary;
	}

	public static void AddDictionaryTypeReader<TKey, TValue>() where TKey : notnull where TValue : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof(Dictionary<TKey, TValue>);
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new DictionaryTypeReader<TKey, TValue>());
			}
		}
	}

	private ITypeReader CreateDictionaryTypeReader(Type keyType, Type valueType)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(DictionaryTypeReader<, >).MakeGenericType(keyType, valueType));
	}

	public T? ReadHandle<T>() where T : SafeHandle
	{
		int num = (int)ReadUInt32();
		if (num >= _handleCount)
		{
			throw new IndexOutOfRangeException();
		}
		if (_handles != null)
		{
			return _handles.RemoveHandle<T>(num);
		}
		return null;
	}

	public IntPtr ReadHandleRaw()
	{
		int num = (int)ReadUInt32();
		if (num >= _handleCount)
		{
			throw new IndexOutOfRangeException();
		}
		if (_handles != null)
		{
			return _handles.DangerousGetHandle(num);
		}
		return new IntPtr(-1);
	}

	public object ReadVariant()
	{
		return Read<object>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private T Read<T>()
	{
		Type type = typeof(T);
		if (type == typeof(object))
		{
			type = TypeModel.DetermineVariantType(ReadSignature());
		}
		if (type == typeof(byte))
		{
			return (T)(object)ReadByte();
		}
		if (type == typeof(bool))
		{
			return (T)(object)ReadBool();
		}
		if (type == typeof(short))
		{
			return (T)(object)ReadInt16();
		}
		if (type == typeof(ushort))
		{
			return (T)(object)ReadUInt16();
		}
		if (type == typeof(int))
		{
			return (T)(object)ReadInt32();
		}
		if (type == typeof(uint))
		{
			return (T)(object)ReadUInt32();
		}
		if (type == typeof(long))
		{
			return (T)(object)ReadInt64();
		}
		if (type == typeof(ulong))
		{
			return (T)(object)ReadUInt64();
		}
		if (type == typeof(double))
		{
			return (T)(object)ReadDouble();
		}
		if (type == typeof(string))
		{
			return (T)(object)ReadString();
		}
		if (type == typeof(ObjectPath))
		{
			return (T)(object)ReadObjectPath();
		}
		if (type == typeof(Signature))
		{
			return (T)(object)ReadSignatureAsSignature();
		}
		return ((ITypeReader<T>)GetTypeReader(type)).Read(ref this);
	}

	private ITypeReader GetTypeReader(Type type)
	{
		lock (_typeReaders)
		{
			if (_typeReaders.TryGetValue(type, out ITypeReader value))
			{
				return value;
			}
			value = CreateReaderForType(type);
			_typeReaders.Add(type, value);
			return value;
		}
	}

	private ITypeReader CreateReaderForType(Type type)
	{
		if (type.IsArray)
		{
			return CreateArrayTypeReader(type.GetElementType());
		}
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<, >))
		{
			Type keyType = type.GenericTypeArguments[0];
			Type valueType = type.GenericTypeArguments[1];
			return CreateDictionaryTypeReader(keyType, valueType);
		}
		if (type.IsGenericType && type.FullName.StartsWith("System.ValueTuple"))
		{
			switch (type.GenericTypeArguments.Length)
			{
			case 1:
				return CreateValueTupleTypeReader(type.GenericTypeArguments[0]);
			case 2:
				return CreateValueTupleTypeReader(type.GenericTypeArguments[0], type.GenericTypeArguments[1]);
			case 3:
				return CreateValueTupleTypeReader(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2]);
			case 4:
				return CreateValueTupleTypeReader(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3]);
			case 5:
				return CreateValueTupleTypeReader(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4]);
			case 6:
				return CreateValueTupleTypeReader(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5]);
			case 7:
				return CreateValueTupleTypeReader(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5], type.GenericTypeArguments[6]);
			case 8:
				switch (type.GenericTypeArguments[7].GenericTypeArguments.Length)
				{
				case 1:
					return CreateValueTupleTypeReader(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5], type.GenericTypeArguments[6], type.GenericTypeArguments[7].GenericTypeArguments[0]);
				case 2:
					return CreateValueTupleTypeReader(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5], type.GenericTypeArguments[6], type.GenericTypeArguments[7].GenericTypeArguments[0], type.GenericTypeArguments[7].GenericTypeArguments[1]);
				case 3:
					return CreateValueTupleTypeReader(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5], type.GenericTypeArguments[6], type.GenericTypeArguments[7].GenericTypeArguments[0], type.GenericTypeArguments[7].GenericTypeArguments[1], type.GenericTypeArguments[7].GenericTypeArguments[2]);
				}
				break;
			}
		}
		if (type.IsGenericType && type.FullName.StartsWith("System.Tuple"))
		{
			switch (type.GenericTypeArguments.Length)
			{
			case 1:
				return CreateTupleTypeReader(type.GenericTypeArguments[0]);
			case 2:
				return CreateTupleTypeReader(type.GenericTypeArguments[0], type.GenericTypeArguments[1]);
			case 3:
				return CreateTupleTypeReader(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2]);
			case 4:
				return CreateTupleTypeReader(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3]);
			case 5:
				return CreateTupleTypeReader(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4]);
			case 6:
				return CreateTupleTypeReader(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5]);
			case 7:
				return CreateTupleTypeReader(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5], type.GenericTypeArguments[6]);
			case 8:
				switch (type.GenericTypeArguments[7].GenericTypeArguments.Length)
				{
				case 1:
					return CreateTupleTypeReader(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5], type.GenericTypeArguments[6], type.GenericTypeArguments[7].GenericTypeArguments[0]);
				case 2:
					return CreateTupleTypeReader(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5], type.GenericTypeArguments[6], type.GenericTypeArguments[7].GenericTypeArguments[0], type.GenericTypeArguments[7].GenericTypeArguments[1]);
				case 3:
					return CreateTupleTypeReader(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5], type.GenericTypeArguments[6], type.GenericTypeArguments[7].GenericTypeArguments[0], type.GenericTypeArguments[7].GenericTypeArguments[1], type.GenericTypeArguments[7].GenericTypeArguments[2]);
				}
				break;
			}
		}
		ThrowNotSupportedType(type);
		return null;
	}

	private static void ThrowNotSupportedType(Type type)
	{
		throw new NotSupportedException("Cannot read type " + type.FullName);
	}

	public ValueTuple<T1> ReadStruct<T1>()
	{
		AlignStruct();
		return ValueTuple.Create(Read<T1>());
	}

	private Tuple<T1> ReadStructAsTuple<T1>()
	{
		AlignStruct();
		return Tuple.Create(Read<T1>());
	}

	public static void AddValueTupleTypeReader<T1>() where T1 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof(ValueTuple<T1>);
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new ValueTupleTypeReader<T1>());
			}
		}
	}

	public static void AddTupleTypeReader<T1>() where T1 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof(Tuple<T1>);
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new TupleTypeReader<T1>());
			}
		}
	}

	private ITypeReader CreateValueTupleTypeReader(Type type1)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(ValueTupleTypeReader<>).MakeGenericType(type1));
	}

	private ITypeReader CreateTupleTypeReader(Type type1)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(TupleTypeReader<>).MakeGenericType(type1));
	}

	public (T1, T2) ReadStruct<T1, T2>() where T1 : notnull where T2 : notnull
	{
		AlignStruct();
		return ValueTuple.Create(Read<T1>(), Read<T2>());
	}

	private Tuple<T1, T2> ReadStructAsTuple<T1, T2>() where T1 : notnull where T2 : notnull
	{
		AlignStruct();
		return Tuple.Create(Read<T1>(), Read<T2>());
	}

	public static void AddValueTupleTypeReader<T1, T2>() where T1 : notnull where T2 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof((T1, T2));
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new ValueTupleTypeReader<T1, T2>());
			}
		}
	}

	public static void AddTupleTypeReader<T1, T2>() where T1 : notnull where T2 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof(Tuple<T1, T2>);
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new TupleTypeReader<T1, T2>());
			}
		}
	}

	private ITypeReader CreateValueTupleTypeReader(Type type1, Type type2)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(ValueTupleTypeReader<, >).MakeGenericType(type1, type2));
	}

	private ITypeReader CreateTupleTypeReader(Type type1, Type type2)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(TupleTypeReader<, >).MakeGenericType(type1, type2));
	}

	public (T1, T2, T3) ReadStruct<T1, T2, T3>() where T1 : notnull where T2 : notnull where T3 : notnull
	{
		AlignStruct();
		return ValueTuple.Create(Read<T1>(), Read<T2>(), Read<T3>());
	}

	private Tuple<T1, T2, T3> ReadStructAsTuple<T1, T2, T3>()
	{
		AlignStruct();
		return Tuple.Create(Read<T1>(), Read<T2>(), Read<T3>());
	}

	public static void AddValueTupleTypeReader<T1, T2, T3>() where T1 : notnull where T2 : notnull where T3 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof((T1, T2, T3));
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new ValueTupleTypeReader<T1, T2, T3>());
			}
		}
	}

	public static void AddTupleTypeReader<T1, T2, T3>() where T1 : notnull where T2 : notnull where T3 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof(Tuple<T1, T2, T3>);
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new TupleTypeReader<T1, T2, T3>());
			}
		}
	}

	private ITypeReader CreateValueTupleTypeReader(Type type1, Type type2, Type type3)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(ValueTupleTypeReader<, , >).MakeGenericType(type1, type2, type3));
	}

	private ITypeReader CreateTupleTypeReader(Type type1, Type type2, Type type3)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(TupleTypeReader<, , >).MakeGenericType(type1, type2, type3));
	}

	public (T1, T2, T3, T4) ReadStruct<T1, T2, T3, T4>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull
	{
		AlignStruct();
		return ValueTuple.Create(Read<T1>(), Read<T2>(), Read<T3>(), Read<T4>());
	}

	private Tuple<T1, T2, T3, T4> ReadStructAsTuple<T1, T2, T3, T4>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull
	{
		AlignStruct();
		return Tuple.Create(Read<T1>(), Read<T2>(), Read<T3>(), Read<T4>());
	}

	public static void AddValueTupleTypeReader<T1, T2, T3, T4>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof((T1, T2, T3, T4));
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new ValueTupleTypeReader<T1, T2, T3, T4>());
			}
		}
	}

	public static void AddTupleTypeReader<T1, T2, T3, T4>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof(Tuple<T1, T2, T3, T4>);
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new TupleTypeReader<T1, T2, T3, T4>());
			}
		}
	}

	private ITypeReader CreateValueTupleTypeReader(Type type1, Type type2, Type type3, Type type4)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(ValueTupleTypeReader<, , , >).MakeGenericType(type1, type2, type3, type4));
	}

	private ITypeReader CreateTupleTypeReader(Type type1, Type type2, Type type3, Type type4)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(TupleTypeReader<, , , >).MakeGenericType(type1, type2, type3, type4));
	}

	public (T1, T2, T3, T4, T5) ReadStruct<T1, T2, T3, T4, T5>()
	{
		AlignStruct();
		return ValueTuple.Create(Read<T1>(), Read<T2>(), Read<T3>(), Read<T4>(), Read<T5>());
	}

	private Tuple<T1, T2, T3, T4, T5> ReadStructAsTuple<T1, T2, T3, T4, T5>()
	{
		AlignStruct();
		return Tuple.Create(Read<T1>(), Read<T2>(), Read<T3>(), Read<T4>(), Read<T5>());
	}

	public static void AddValueTupleTypeReader<T1, T2, T3, T4, T5>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof((T1, T2, T3, T4, T5));
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new ValueTupleTypeReader<T1, T2, T3, T4, T5>());
			}
		}
	}

	public static void AddTupleTypeReader<T1, T2, T3, T4, T5>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof(Tuple<T1, T2, T3, T4, T5>);
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new TupleTypeReader<T1, T2, T3, T4, T5>());
			}
		}
	}

	private ITypeReader CreateValueTupleTypeReader(Type type1, Type type2, Type type3, Type type4, Type type5)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(ValueTupleTypeReader<, , , , >).MakeGenericType(type1, type2, type3, type4, type5));
	}

	private ITypeReader CreateTupleTypeReader(Type type1, Type type2, Type type3, Type type4, Type type5)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(TupleTypeReader<, , , , >).MakeGenericType(type1, type2, type3, type4, type5, type5));
	}

	public (T1, T2, T3, T4, T5, T6) ReadStruct<T1, T2, T3, T4, T5, T6>()
	{
		AlignStruct();
		return ValueTuple.Create(Read<T1>(), Read<T2>(), Read<T3>(), Read<T4>(), Read<T5>(), Read<T6>());
	}

	private Tuple<T1, T2, T3, T4, T5, T6> ReadStructAsTuple<T1, T2, T3, T4, T5, T6>()
	{
		AlignStruct();
		return Tuple.Create(Read<T1>(), Read<T2>(), Read<T3>(), Read<T4>(), Read<T5>(), Read<T6>());
	}

	public static void AddValueTupleTypeReader<T1, T2, T3, T4, T5, T6>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof((T1, T2, T3, T4, T5, T6));
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new ValueTupleTypeReader<T1, T2, T3, T4, T5, T6>());
			}
		}
	}

	public static void AddTupleTypeReader<T1, T2, T3, T4, T5, T6>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof(Tuple<T1, T2, T3, T4, T5, T6>);
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new TupleTypeReader<T1, T2, T3, T4, T5, T6>());
			}
		}
	}

	private ITypeReader CreateValueTupleTypeReader(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(ValueTupleTypeReader<, , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6));
	}

	private ITypeReader CreateTupleTypeReader(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(TupleTypeReader<, , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6));
	}

	public (T1, T2, T3, T4, T5, T6, T7) ReadStruct<T1, T2, T3, T4, T5, T6, T7>()
	{
		AlignStruct();
		return ValueTuple.Create(Read<T1>(), Read<T2>(), Read<T3>(), Read<T4>(), Read<T5>(), Read<T6>(), Read<T7>());
	}

	private Tuple<T1, T2, T3, T4, T5, T6, T7> ReadStructAsTuple<T1, T2, T3, T4, T5, T6, T7>()
	{
		AlignStruct();
		return Tuple.Create(Read<T1>(), Read<T2>(), Read<T3>(), Read<T4>(), Read<T5>(), Read<T6>(), Read<T7>());
	}

	public static void AddValueTupleTypeReader<T1, T2, T3, T4, T5, T6, T7>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof((T1, T2, T3, T4, T5, T6, T7));
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new ValueTupleTypeReader<T1, T2, T3, T4, T5, T6, T7>());
			}
		}
	}

	public static void AddTupleTypeReader<T1, T2, T3, T4, T5, T6, T7>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof(Tuple<T1, T2, T3, T4, T5, T6, T7>);
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new TupleTypeReader<T1, T2, T3, T4, T5, T6, T7>());
			}
		}
	}

	private ITypeReader CreateValueTupleTypeReader(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(ValueTupleTypeReader<, , , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6, type7));
	}

	private ITypeReader CreateTupleTypeReader(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(TupleTypeReader<, , , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6, type7));
	}

	public (T1, T2, T3, T4, T5, T6, T7, T8) ReadStruct<T1, T2, T3, T4, T5, T6, T7, T8>()
	{
		AlignStruct();
		return ValueTuple.Create(Read<T1>(), Read<T2>(), Read<T3>(), Read<T4>(), Read<T5>(), Read<T6>(), Read<T7>(), Read<T8>());
	}

	private Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> ReadStructAsTuple<T1, T2, T3, T4, T5, T6, T7, T8>()
	{
		AlignStruct();
		return Tuple.Create(Read<T1>(), Read<T2>(), Read<T3>(), Read<T4>(), Read<T5>(), Read<T6>(), Read<T7>(), Read<T8>());
	}

	public static void AddValueTupleTypeReader<T1, T2, T3, T4, T5, T6, T7, T8>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof((T1, T2, T3, T4, T5, T6, T7, T8));
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new ValueTupleTypeReader<T1, T2, T3, T4, T5, T6, T7, T8>());
			}
		}
	}

	public static void AddTupleTypeReader<T1, T2, T3, T4, T5, T6, T7, T8>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof(Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>);
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new TupleTypeReader<T1, T2, T3, T4, T5, T6, T7, T8>());
			}
		}
	}

	private ITypeReader CreateValueTupleTypeReader(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7, Type type8)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(ValueTupleTypeReader<, , , , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6, type7, type8));
	}

	private ITypeReader CreateTupleTypeReader(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7, Type type8)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(TupleTypeReader<, , , , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6, type7, type8));
	}

	public (T1, T2, T3, T4, T5, T6, T7, T8, T9) ReadStruct<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
	{
		AlignStruct();
		return (Read<T1>(), Read<T2>(), Read<T3>(), Read<T4>(), Read<T5>(), Read<T6>(), Read<T7>(), Read<T8>(), Read<T9>());
	}

	private Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>> ReadStructAsTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
	{
		AlignStruct();
		return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>(Read<T1>(), Read<T2>(), Read<T3>(), Read<T4>(), Read<T5>(), Read<T6>(), Read<T7>(), Tuple.Create(Read<T8>(), Read<T9>()));
	}

	public static void AddValueTupleTypeReader<T1, T2, T3, T4, T5, T6, T7, T8, T9>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull where T9 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof((T1, T2, T3, T4, T5, T6, T7, T8, T9));
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new ValueTupleTypeReader<T1, T2, T3, T4, T5, T6, T7, T8, T9>());
			}
		}
	}

	public static void AddTupleTypeReader<T1, T2, T3, T4, T5, T6, T7, T8, T9>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull where T9 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof(Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>);
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new TupleTypeReader<T1, T2, T3, T4, T5, T6, T7, T8, T9>());
			}
		}
	}

	private ITypeReader CreateValueTupleTypeReader(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7, Type type8, Type type9)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(ValueTupleTypeReader<, , , , , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6, type7, type8, type9));
	}

	private ITypeReader CreateTupleTypeReader(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7, Type type8, Type type9)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(TupleTypeReader<, , , , , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6, type7, type8, type9));
	}

	public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) ReadStruct<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
	{
		AlignStruct();
		return (Read<T1>(), Read<T2>(), Read<T3>(), Read<T4>(), Read<T5>(), Read<T6>(), Read<T7>(), Read<T8>(), Read<T9>(), Read<T10>());
	}

	private Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>> ReadStructAsTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
	{
		AlignStruct();
		return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>(Read<T1>(), Read<T2>(), Read<T3>(), Read<T4>(), Read<T5>(), Read<T6>(), Read<T7>(), Tuple.Create(Read<T8>(), Read<T9>(), Read<T10>()));
	}

	public static void AddValueTupleTypeReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull where T9 : notnull where T10 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof((T1, T2, T3, T4, T5, T6, T7, T8, T9, T10));
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new ValueTupleTypeReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>());
			}
		}
	}

	public static void AddTupleTypeReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull where T9 : notnull where T10 : notnull
	{
		lock (_typeReaders)
		{
			Type typeFromHandle = typeof(Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>);
			if (!_typeReaders.ContainsKey(typeFromHandle))
			{
				_typeReaders.Add(typeFromHandle, new TupleTypeReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>());
			}
		}
	}

	private ITypeReader CreateValueTupleTypeReader(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7, Type type8, Type type9, Type type10)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(ValueTupleTypeReader<, , , , , , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6, type7, type8, type9, type10));
	}

	private ITypeReader CreateTupleTypeReader(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7, Type type8, Type type9, Type type10)
	{
		return (ITypeReader)Activator.CreateInstance(typeof(TupleTypeReader<, , , , , , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6, type7, type8, type9, type10));
	}
}
