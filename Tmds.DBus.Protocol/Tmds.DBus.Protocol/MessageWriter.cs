using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Nerdbank.Streams;

namespace Tmds.DBus.Protocol;

public ref struct MessageWriter
{
	private sealed class ArrayTypeWriter<T> : ITypeWriter<IEnumerable<T>>, ITypeWriter where T : notnull
	{
		public void Write(ref MessageWriter writer, IEnumerable<T> value)
		{
			writer.WriteArray(value);
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteArraySignature<T>(ref writer);
			writer.WriteArray((IEnumerable<T>)value);
		}
	}

	private sealed class DictionaryTypeWriter<TKey, TValue> : ITypeWriter<IEnumerable<KeyValuePair<TKey, TValue>>>, ITypeWriter where TKey : notnull where TValue : notnull
	{
		public void Write(ref MessageWriter writer, IEnumerable<KeyValuePair<TKey, TValue>> value)
		{
			writer.WriteDictionary(value);
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteDictionarySignature<TKey, TValue>(ref writer);
			writer.WriteDictionary((IEnumerable<KeyValuePair<TKey, TValue>>)value);
		}
	}

	private sealed class ValueTupleTypeWriter<T1> : ITypeWriter<ValueTuple<T1>>, ITypeWriter where T1 : notnull
	{
		public void Write(ref MessageWriter writer, ValueTuple<T1> value)
		{
			writer.WriteStruct(new ValueTuple<T1>(value.Item1));
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1>(ref writer);
			Write(ref writer, (ValueTuple<T1>)value);
		}
	}

	private sealed class TupleTypeWriter<T1> : ITypeWriter<Tuple<T1>>, ITypeWriter where T1 : notnull
	{
		public void Write(ref MessageWriter writer, Tuple<T1> value)
		{
			writer.WriteStruct(new ValueTuple<T1>(value.Item1));
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1>(ref writer);
			Write(ref writer, (Tuple<T1>)value);
		}
	}

	private sealed class ValueTupleTypeWriter<T1, T2> : ITypeWriter<(T1, T2)>, ITypeWriter where T1 : notnull where T2 : notnull
	{
		public void Write(ref MessageWriter writer, (T1, T2) value)
		{
			writer.WriteStruct(value);
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1, T2>(ref writer);
			Write(ref writer, ((T1, T2))value);
		}
	}

	private sealed class TupleTypeWriter<T1, T2> : ITypeWriter<Tuple<T1, T2>>, ITypeWriter where T1 : notnull where T2 : notnull
	{
		public void Write(ref MessageWriter writer, Tuple<T1, T2> value)
		{
			writer.WriteStruct((value.Item1, value.Item2));
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1, T2>(ref writer);
			Write(ref writer, (Tuple<T1, T2>)value);
		}
	}

	private sealed class ValueTupleTypeWriter<T1, T2, T3> : ITypeWriter<(T1, T2, T3)>, ITypeWriter where T1 : notnull where T2 : notnull where T3 : notnull
	{
		public void Write(ref MessageWriter writer, (T1, T2, T3) value)
		{
			writer.WriteStruct(value);
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1, T2, T3>(ref writer);
			Write(ref writer, ((T1, T2, T3))value);
		}
	}

	private sealed class TupleTypeWriter<T1, T2, T3> : ITypeWriter<Tuple<T1, T2, T3>>, ITypeWriter where T1 : notnull where T2 : notnull where T3 : notnull
	{
		public void Write(ref MessageWriter writer, Tuple<T1, T2, T3> value)
		{
			writer.WriteStruct((value.Item1, value.Item2, value.Item3));
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1, T2, T3>(ref writer);
			Write(ref writer, (Tuple<T1, T2, T3>)value);
		}
	}

	private sealed class ValueTupleTypeWriter<T1, T2, T3, T4> : ITypeWriter<(T1, T2, T3, T4)>, ITypeWriter where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull
	{
		public void Write(ref MessageWriter writer, (T1, T2, T3, T4) value)
		{
			writer.WriteStruct(value);
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1, T2, T3, T4>(ref writer);
			Write(ref writer, ((T1, T2, T3, T4))value);
		}
	}

	private sealed class TupleTypeWriter<T1, T2, T3, T4> : ITypeWriter<Tuple<T1, T2, T3, T4>>, ITypeWriter where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull
	{
		public void Write(ref MessageWriter writer, Tuple<T1, T2, T3, T4> value)
		{
			writer.WriteStruct((value.Item1, value.Item2, value.Item3, value.Item4));
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1, T2, T3, T4>(ref writer);
			Write(ref writer, (Tuple<T1, T2, T3, T4>)value);
		}
	}

	private sealed class ValueTupleTypeWriter<T1, T2, T3, T4, T5> : ITypeWriter<(T1, T2, T3, T4, T5)>, ITypeWriter where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull
	{
		public void Write(ref MessageWriter writer, (T1, T2, T3, T4, T5) value)
		{
			writer.WriteStruct(value);
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1, T2, T3, T4, T5>(ref writer);
			Write(ref writer, ((T1, T2, T3, T4, T5))value);
		}
	}

	private sealed class TupleTypeWriter<T1, T2, T3, T4, T5> : ITypeWriter<Tuple<T1, T2, T3, T4, T5>>, ITypeWriter where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull
	{
		public void Write(ref MessageWriter writer, Tuple<T1, T2, T3, T4, T5> value)
		{
			writer.WriteStruct((value.Item1, value.Item2, value.Item3, value.Item4, value.Item5));
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1, T2, T3, T4, T5>(ref writer);
			Write(ref writer, (Tuple<T1, T2, T3, T4, T5>)value);
		}
	}

	private sealed class ValueTupleTypeWriter<T1, T2, T3, T4, T5, T6> : ITypeWriter<(T1, T2, T3, T4, T5, T6)>, ITypeWriter where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull
	{
		public void Write(ref MessageWriter writer, (T1, T2, T3, T4, T5, T6) value)
		{
			writer.WriteStruct(value);
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1, T2, T3, T4, T5, T6>(ref writer);
			Write(ref writer, ((T1, T2, T3, T4, T5, T6))value);
		}
	}

	private sealed class TupleTypeWriter<T1, T2, T3, T4, T5, T6> : ITypeWriter<Tuple<T1, T2, T3, T4, T5, T6>>, ITypeWriter where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull
	{
		public void Write(ref MessageWriter writer, Tuple<T1, T2, T3, T4, T5, T6> value)
		{
			writer.WriteStruct((value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6));
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1, T2, T3, T4, T5, T6>(ref writer);
			Write(ref writer, (Tuple<T1, T2, T3, T4, T5, T6>)value);
		}
	}

	private sealed class ValueTupleTypeWriter<T1, T2, T3, T4, T5, T6, T7> : ITypeWriter<(T1, T2, T3, T4, T5, T6, T7)>, ITypeWriter where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull
	{
		public void Write(ref MessageWriter writer, (T1, T2, T3, T4, T5, T6, T7) value)
		{
			writer.WriteStruct(value);
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1, T2, T3, T4, T5, T6, T7>(ref writer);
			Write(ref writer, ((T1, T2, T3, T4, T5, T6, T7))value);
		}
	}

	private sealed class TupleTypeWriter<T1, T2, T3, T4, T5, T6, T7> : ITypeWriter<Tuple<T1, T2, T3, T4, T5, T6, T7>>, ITypeWriter where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull
	{
		public void Write(ref MessageWriter writer, Tuple<T1, T2, T3, T4, T5, T6, T7> value)
		{
			writer.WriteStruct((value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6, value.Item7));
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1, T2, T3, T4, T5, T6, T7>(ref writer);
			Write(ref writer, (Tuple<T1, T2, T3, T4, T5, T6, T7>)value);
		}
	}

	private sealed class ValueTupleTypeWriter<T1, T2, T3, T4, T5, T6, T7, T8> : ITypeWriter<(T1, T2, T3, T4, T5, T6, T7, T8)>, ITypeWriter where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull
	{
		public void Write(ref MessageWriter writer, (T1, T2, T3, T4, T5, T6, T7, T8) value)
		{
			writer.WriteStruct(value);
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1, T2, T3, T4, T5, T6, T7, T8>(ref writer);
			Write(ref writer, ((T1, T2, T3, T4, T5, T6, T7, T8))value);
		}
	}

	private sealed class TupleTypeWriter<T1, T2, T3, T4, T5, T6, T7, T8> : ITypeWriter<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>>, ITypeWriter where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull
	{
		public void Write(ref MessageWriter writer, Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> value)
		{
			writer.WriteStruct((value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6, value.Item7, value.Rest.Item1));
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1, T2, T3, T4, T5, T6, T7, T8>(ref writer);
			Write(ref writer, (Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>)value);
		}
	}

	private sealed class ValueTupleTypeWriter<T1, T2, T3, T4, T5, T6, T7, T8, T9> : ITypeWriter<(T1, T2, T3, T4, T5, T6, T7, T8, T9)>, ITypeWriter where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull where T9 : notnull
	{
		public void Write(ref MessageWriter writer, (T1, T2, T3, T4, T5, T6, T7, T8, T9) value)
		{
			writer.WriteStruct(value);
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref writer);
			Write(ref writer, ((T1, T2, T3, T4, T5, T6, T7, T8, T9))value);
		}
	}

	private sealed class TupleTypeWriter<T1, T2, T3, T4, T5, T6, T7, T8, T9> : ITypeWriter<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>>, ITypeWriter where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull where T9 : notnull
	{
		public void Write(ref MessageWriter writer, Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>> value)
		{
			writer.WriteStruct((value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6, value.Item7, value.Rest.Item1, value.Rest.Item2));
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref writer);
			Write(ref writer, (Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>)value);
		}
	}

	private sealed class ValueTupleTypeWriter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : ITypeWriter<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)>, ITypeWriter where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull where T9 : notnull where T10 : notnull
	{
		public void Write(ref MessageWriter writer, (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) value)
		{
			writer.WriteStruct(value);
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref writer);
			Write(ref writer, ((T1, T2, T3, T4, T5, T6, T7, T8, T9, T10))value);
		}
	}

	private sealed class TupleTypeWriter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : ITypeWriter<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>>, ITypeWriter where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull where T9 : notnull where T10 : notnull
	{
		public void Write(ref MessageWriter writer, Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>> value)
		{
			writer.WriteStruct((value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6, value.Item7, value.Rest.Item1, value.Rest.Item2, value.Rest.Item3));
		}

		public void WriteVariant(ref MessageWriter writer, object value)
		{
			WriteStructSignature<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref writer);
			Write(ref writer, (Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>)value);
		}
	}

	private interface ITypeWriter
	{
		void WriteVariant(ref MessageWriter writer, object value);
	}

	private interface ITypeWriter<in T> : ITypeWriter
	{
		void Write(ref MessageWriter writer, T value);
	}

	private const int LengthOffset = 4;

	private const int SerialOffset = 8;

	private const int HeaderFieldsLengthOffset = 12;

	private const int UnixFdLengthOffset = 20;

	private MessageBuffer _message;

	private Sequence<byte> _data;

	private UnixFdCollection? _handles;

	private readonly uint _serial;

	private MessageFlags _flags;

	private Span<byte> _firstSpan;

	private Span<byte> _span;

	private int _offset;

	private int _buffered;

	private static readonly Dictionary<Type, ITypeWriter> _typeWriters = new Dictionary<Type, ITypeWriter>();

	private IBufferWriter<byte> Writer
	{
		get
		{
			Flush();
			return _data;
		}
	}

	internal UnixFdCollection? Handles => _handles;

	private int HandleCount => _handles?.Count ?? 0;

	public void WriteArray<T>(IEnumerable<T> value) where T : notnull
	{
		ArrayStart start = WriteArrayStart(TypeModel.GetTypeAlignment<T>());
		foreach (T item in value)
		{
			Write(item);
		}
		WriteArrayEnd(start);
	}

	public void WriteArray<T>(T[] value) where T : notnull
	{
		ArrayStart start = WriteArrayStart(TypeModel.GetTypeAlignment<T>());
		foreach (T value2 in value)
		{
			Write(value2);
		}
		WriteArrayEnd(start);
	}

	public static void AddArrayTypeWriter<T>() where T : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof(IEnumerable<T>);
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new ArrayTypeWriter<T>());
			}
		}
	}

	private ITypeWriter CreateArrayTypeWriter(Type elementType)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(ArrayTypeWriter<>).MakeGenericType(elementType));
	}

	private static void WriteArraySignature<T>(ref MessageWriter writer)
	{
		writer.WriteSignature(TypeModel.GetSignature<T[]>());
	}

	public void WriteBool(bool value)
	{
		WriteUInt32(value ? 1u : 0u);
	}

	public void WriteByte(byte value)
	{
		WritePrimitiveCore((short)value, DBusType.Byte);
	}

	public void WriteInt16(short value)
	{
		WritePrimitiveCore(value, DBusType.Int16);
	}

	public void WriteUInt16(ushort value)
	{
		WritePrimitiveCore(value, DBusType.UInt16);
	}

	public void WriteInt32(int value)
	{
		WritePrimitiveCore(value, DBusType.Int32);
	}

	public void WriteUInt32(uint value)
	{
		WritePrimitiveCore(value, DBusType.UInt32);
	}

	public void WriteInt64(long value)
	{
		WritePrimitiveCore(value, DBusType.Int64);
	}

	public void WriteUInt64(ulong value)
	{
		WritePrimitiveCore(value, DBusType.UInt64);
	}

	public void WriteDouble(double value)
	{
		WritePrimitiveCore(value, DBusType.Double);
	}

	public void WriteString(Utf8Span value)
	{
		WriteStringCore(value);
	}

	public void WriteString(string value)
	{
		WriteStringCore(value);
	}

	public void WriteSignature(Utf8Span value)
	{
		ReadOnlySpan<byte> readOnlySpan = value;
		int length = readOnlySpan.Length;
		WriteByte((byte)length);
		Span<byte> span = GetSpan(length);
		readOnlySpan.CopyTo(span);
		Advance(length);
		WriteByte(0);
	}

	public void WriteSignature(string s)
	{
		Span<byte> span = GetSpan(1);
		Advance(1);
		int num = (int)Encoding.UTF8.GetBytes(s.AsSpan(), Writer);
		span[0] = (byte)num;
		_offset += num;
		WriteByte(0);
	}

	public void WriteObjectPath(Utf8Span value)
	{
		WriteStringCore(value);
	}

	public void WriteObjectPath(string value)
	{
		WriteStringCore(value);
	}

	public void WriteVariantBool(bool value)
	{
		WriteSignature(ProtocolConstants.BooleanSignature);
		WriteBool(value);
	}

	public void WriteVariantByte(byte value)
	{
		WriteSignature(ProtocolConstants.ByteSignature);
		WriteByte(value);
	}

	public void WriteVariantInt16(short value)
	{
		WriteSignature(ProtocolConstants.Int16Signature);
		WriteInt16(value);
	}

	public void WriteVariantUInt16(ushort value)
	{
		WriteSignature(ProtocolConstants.UInt16Signature);
		WriteUInt16(value);
	}

	public void WriteVariantInt32(int value)
	{
		WriteSignature(ProtocolConstants.Int32Signature);
		WriteInt32(value);
	}

	public void WriteVariantUInt32(uint value)
	{
		WriteSignature(ProtocolConstants.UInt32Signature);
		WriteUInt32(value);
	}

	public void WriteVariantInt64(long value)
	{
		WriteSignature(ProtocolConstants.Int64Signature);
		WriteInt64(value);
	}

	public void WriteVariantUInt64(ulong value)
	{
		WriteSignature(ProtocolConstants.UInt64Signature);
		WriteUInt64(value);
	}

	public void WriteVariantDouble(double value)
	{
		WriteSignature(ProtocolConstants.DoubleSignature);
		WriteDouble(value);
	}

	public void WriteVariantString(Utf8Span value)
	{
		WriteSignature(ProtocolConstants.StringSignature);
		WriteString(value);
	}

	public void WriteVariantSignature(Utf8Span value)
	{
		WriteSignature(ProtocolConstants.SignatureSignature);
		WriteSignature(value);
	}

	public void WriteVariantObjectPath(Utf8Span value)
	{
		WriteSignature(ProtocolConstants.ObjectPathSignature);
		WriteObjectPath(value);
	}

	public void WriteVariantString(string value)
	{
		WriteSignature(ProtocolConstants.StringSignature);
		WriteString(value);
	}

	public void WriteVariantSignature(string value)
	{
		WriteSignature(ProtocolConstants.SignatureSignature);
		WriteSignature(value);
	}

	public void WriteVariantObjectPath(string value)
	{
		WriteSignature(ProtocolConstants.ObjectPathSignature);
		WriteObjectPath(value);
	}

	private void WriteStringCore(ReadOnlySpan<byte> span)
	{
		int length = span.Length;
		WriteUInt32((uint)length);
		Span<byte> span2 = GetSpan(length);
		span.CopyTo(span2);
		Advance(length);
		WriteByte(0);
	}

	private void WriteStringCore(string s)
	{
		WritePadding(DBusType.UInt32);
		Span<byte> span = GetSpan(4);
		Advance(4);
		int num = (int)Encoding.UTF8.GetBytes(s.AsSpan(), Writer);
		Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(span), (uint)num);
		_offset += num;
		WriteByte(0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void WritePrimitiveCore<T>(T value, DBusType type)
	{
		WritePadding(type);
		int fixedTypeLength = ProtocolConstants.GetFixedTypeLength(type);
		Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(GetSpan(fixedTypeLength)), value);
		Advance(fixedTypeLength);
	}

	public MessageBuffer CreateMessage()
	{
		Flush();
		Span<byte> firstSpan = _firstSpan;
		uint num = Unsafe.ReadUnaligned<uint>(ref MemoryMarshal.GetReference(firstSpan.Slice(12)));
		uint num2 = num % 8;
		if (num2 != 0)
		{
			num += 8 - num2;
		}
		uint value = (uint)((int)_data.Length - (int)num - 4 - 12);
		Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(firstSpan.Slice(4)), value);
		Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(firstSpan.Slice(20)), (uint)HandleCount);
		uint serial = _serial;
		MessageFlags flags = _flags;
		_ = (ReadOnlySequence<byte>)_data;
		UnixFdCollection handles = _handles;
		MessageBuffer message = _message;
		_message = null;
		_handles = null;
		_data = null;
		message.Init(serial, flags, handles);
		return message;
	}

	internal MessageWriter(MessageBufferPool messagePool, uint serial)
	{
		_message = messagePool.Rent();
		_data = _message.Sequence;
		_handles = null;
		_flags = MessageFlags.None;
		_offset = 0;
		_buffered = 0;
		_serial = serial;
		_firstSpan = (_span = _data.GetSpan(0));
	}

	public ArrayStart WriteArrayStart(DBusType elementType)
	{
		WritePadding(DBusType.UInt32);
		Span<byte> span = GetSpan(4);
		Advance(4);
		WritePadding(elementType);
		return new ArrayStart(span, _offset);
	}

	public void WriteArrayEnd(ArrayStart start)
	{
		start.WriteLength(_offset);
	}

	public void WriteStructureStart()
	{
		WritePadding(DBusType.Struct);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Advance(int count)
	{
		_buffered += count;
		_offset += count;
		_span = _span.Slice(count);
	}

	private void WritePadding(DBusType type)
	{
		int padding = ProtocolConstants.GetPadding(_offset, type);
		if (padding != 0)
		{
			Span<byte> span = GetSpan(padding);
			span = span.Slice(0, padding);
			span.Fill(0);
			Advance(padding);
		}
	}

	private Span<byte> GetSpan(int sizeHint)
	{
		Ensure(sizeHint);
		return _span;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Ensure(int count = 1)
	{
		if (_span.Length < count)
		{
			EnsureMore(count);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void EnsureMore(int count = 0)
	{
		if (_buffered > 0)
		{
			Flush();
		}
		_span = _data.GetSpan(count);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Flush()
	{
		int buffered = _buffered;
		if (buffered > 0)
		{
			_buffered = 0;
			_data.Advance(buffered);
			_span = default(Span<byte>);
		}
	}

	public void Dispose()
	{
		_message?.ReturnToPool();
		_handles?.Dispose();
		_message = null;
		_data = null;
		_handles = null;
	}

	internal ReadOnlySequence<byte> AsReadOnlySequence()
	{
		Flush();
		return _data.AsReadOnlySequence;
	}

	public void WriteDictionary<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> value) where TKey : notnull where TValue : notnull
	{
		ArrayStart start = WriteArrayStart(DBusType.Struct);
		foreach (KeyValuePair<TKey, TValue> item in value)
		{
			WriteStructureStart();
			Write(item.Key);
			Write(item.Value);
		}
		WriteArrayEnd(start);
	}

	public void WriteDictionary<TKey, TValue>(KeyValuePair<TKey, TValue>[] value) where TKey : notnull where TValue : notnull
	{
		ArrayStart start = WriteArrayStart(DBusType.Struct);
		for (int i = 0; i < value.Length; i++)
		{
			KeyValuePair<TKey, TValue> keyValuePair = value[i];
			WriteStructureStart();
			Write(keyValuePair.Key);
			Write(keyValuePair.Value);
		}
		WriteArrayEnd(start);
	}

	public void WriteDictionary<TKey, TValue>(Dictionary<TKey, TValue> value) where TKey : notnull where TValue : notnull
	{
		ArrayStart start = WriteArrayStart(DBusType.Struct);
		foreach (KeyValuePair<TKey, TValue> item in value)
		{
			WriteStructureStart();
			Write(item.Key);
			Write(item.Value);
		}
		WriteArrayEnd(start);
	}

	public void WriteVariantDictionary<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> value) where TKey : notnull where TValue : notnull
	{
		WriteDictionarySignature<TKey, TValue>(ref this);
		WriteDictionary(value);
	}

	public void WriteVariantDictionary<TKey, TValue>(KeyValuePair<TKey, TValue>[] value) where TKey : notnull where TValue : notnull
	{
		WriteDictionarySignature<TKey, TValue>(ref this);
		WriteDictionary(value);
	}

	public void WriteVariantDictionary<TKey, TValue>(Dictionary<TKey, TValue> value) where TKey : notnull where TValue : notnull
	{
		WriteDictionarySignature<TKey, TValue>(ref this);
		WriteDictionary(value);
	}

	public static void AddDictionaryTypeWriter<TKey, TValue>() where TKey : notnull where TValue : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof(IEnumerable<KeyValuePair<TKey, TValue>>);
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new DictionaryTypeWriter<TKey, TValue>());
			}
		}
	}

	private ITypeWriter CreateDictionaryTypeWriter(Type keyType, Type valueType)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(DictionaryTypeWriter<, >).MakeGenericType(keyType, valueType));
	}

	private static void WriteDictionarySignature<TKey, TValue>(ref MessageWriter writer)
	{
		writer.WriteSignature(TypeModel.GetSignature<IDictionary<TKey, TValue>>());
	}

	public void WriteHandle(SafeHandle value)
	{
		int handleCount = HandleCount;
		AddHandle(value);
		WriteInt32(handleCount);
	}

	private void AddHandle(SafeHandle handle)
	{
		if (_handles == null)
		{
			_handles = new UnixFdCollection(isRawHandleCollection: false);
		}
		_handles.AddHandle(handle);
	}

	public void WriteMethodCallHeader(string? destination = null, string? path = null, string? @interface = null, string? member = null, string? signature = null, MessageFlags flags = MessageFlags.None)
	{
		ArrayStart start = WriteHeaderStart(MessageType.MethodCall, flags);
		if (path != null)
		{
			WriteStructureStart();
			WriteByte(1);
			WriteVariantObjectPath(path);
		}
		if (@interface != null)
		{
			WriteStructureStart();
			WriteByte(2);
			WriteVariantString(@interface);
		}
		if (member != null)
		{
			WriteStructureStart();
			WriteByte(3);
			WriteVariantString(member);
		}
		if (destination != null)
		{
			WriteStructureStart();
			WriteByte(6);
			WriteVariantString(destination);
		}
		if (signature != null)
		{
			WriteStructureStart();
			WriteByte(8);
			WriteVariantSignature(signature);
		}
		WriteHeaderEnd(start);
	}

	public void WriteMethodReturnHeader(uint replySerial, Utf8Span destination = default(Utf8Span), string? signature = null)
	{
		ArrayStart start = WriteHeaderStart(MessageType.MethodReturn, MessageFlags.None);
		WriteStructureStart();
		WriteByte(5);
		WriteVariantUInt32(replySerial);
		if (!destination.IsEmpty)
		{
			WriteStructureStart();
			WriteByte(6);
			WriteVariantString(destination);
		}
		if (signature != null)
		{
			WriteStructureStart();
			WriteByte(8);
			WriteVariantSignature(signature);
		}
		WriteHeaderEnd(start);
	}

	public void WriteError(uint replySerial, ReadOnlySpan<byte> destination = default(ReadOnlySpan<byte>), string? errorName = null, string? errorMsg = null)
	{
		ArrayStart start = WriteHeaderStart(MessageType.Error, MessageFlags.None);
		WriteStructureStart();
		WriteByte(5);
		WriteVariantUInt32(replySerial);
		if (!destination.IsEmpty)
		{
			WriteStructureStart();
			WriteByte(6);
			WriteVariantString(destination);
		}
		if (errorName != null)
		{
			WriteStructureStart();
			WriteByte(4);
			WriteVariantString(errorName);
		}
		if (errorMsg != null)
		{
			WriteStructureStart();
			WriteByte(8);
			WriteVariantSignature(ProtocolConstants.StringSignature);
		}
		WriteHeaderEnd(start);
		if (errorMsg != null)
		{
			WriteString(errorMsg);
		}
	}

	public void WriteSignalHeader(string? destination = null, string? path = null, string? @interface = null, string? member = null, string? signature = null)
	{
		ArrayStart start = WriteHeaderStart(MessageType.Signal, MessageFlags.None);
		if (path != null)
		{
			WriteStructureStart();
			WriteByte(1);
			WriteVariantObjectPath(path);
		}
		if (@interface != null)
		{
			WriteStructureStart();
			WriteByte(2);
			WriteVariantString(@interface);
		}
		if (member != null)
		{
			WriteStructureStart();
			WriteByte(3);
			WriteVariantString(member);
		}
		if (destination != null)
		{
			WriteStructureStart();
			WriteByte(6);
			WriteVariantString(destination);
		}
		if (signature != null)
		{
			WriteStructureStart();
			WriteByte(8);
			WriteVariantSignature(signature);
		}
		WriteHeaderEnd(start);
	}

	private void WriteHeaderEnd(ArrayStart start)
	{
		WriteArrayEnd(start);
		WritePadding(DBusType.Struct);
	}

	private ArrayStart WriteHeaderStart(MessageType type, MessageFlags flags)
	{
		_flags = flags;
		WriteByte((byte)(BitConverter.IsLittleEndian ? 108 : 66));
		WriteByte((byte)type);
		WriteByte((byte)flags);
		WriteByte(1);
		WriteUInt32(0u);
		WriteUInt32(_serial);
		ArrayStart result = WriteArrayStart(DBusType.Struct);
		WriteStructureStart();
		WriteByte(9);
		WriteVariantUInt32(0u);
		return result;
	}

	public void WriteStruct<T1>(ValueTuple<T1> value) where T1 : notnull
	{
		WriteStructureStart();
		Write(value.Item1);
	}

	public static void AddValueTupleTypeWriter<T1>() where T1 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof(ValueTuple<T1>);
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new ValueTupleTypeWriter<T1>());
			}
		}
	}

	public static void AddTupleTypeWriter<T1>() where T1 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof(Tuple<T1>);
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new TupleTypeWriter<T1>());
			}
		}
	}

	private ITypeWriter CreateValueTupleTypeWriter(Type type1)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(ValueTupleTypeWriter<>).MakeGenericType(type1));
	}

	private ITypeWriter CreateTupleTypeWriter(Type type1)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(TupleTypeWriter<>).MakeGenericType(type1));
	}

	private static void WriteStructSignature<T1>(ref MessageWriter writer)
	{
		writer.WriteSignature(TypeModel.GetSignature<ValueTuple<T1>>());
	}

	public void WriteStruct<T1, T2>((T1, T2) value) where T1 : notnull where T2 : notnull
	{
		WriteStructureStart();
		Write(value.Item1);
		Write(value.Item2);
	}

	public static void AddValueTupleTypeWriter<T1, T2>() where T1 : notnull where T2 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof((T1, T2));
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new ValueTupleTypeWriter<T1, T2>());
			}
		}
	}

	public static void AddTupleTypeWriter<T1, T2>() where T1 : notnull where T2 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof(Tuple<T1, T2>);
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new TupleTypeWriter<T1, T2>());
			}
		}
	}

	private ITypeWriter CreateValueTupleTypeWriter(Type type1, Type type2)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(ValueTupleTypeWriter<, >).MakeGenericType(type1, type2));
	}

	private ITypeWriter CreateTupleTypeWriter(Type type1, Type type2)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(TupleTypeWriter<, >).MakeGenericType(type1, type2));
	}

	private static void WriteStructSignature<T1, T2>(ref MessageWriter writer)
	{
		writer.WriteSignature(TypeModel.GetSignature<(T1, T2)>());
	}

	public void WriteStruct<T1, T2, T3>((T1, T2, T3) value) where T1 : notnull where T2 : notnull where T3 : notnull
	{
		WriteStructureStart();
		Write(value.Item1);
		Write(value.Item2);
		Write(value.Item3);
	}

	public static void AddValueTupleTypeWriter<T1, T2, T3>() where T1 : notnull where T2 : notnull where T3 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof((T1, T2, T3));
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new ValueTupleTypeWriter<T1, T2, T3>());
			}
		}
	}

	public static void AddTupleTypeWriter<T1, T2, T3>() where T1 : notnull where T2 : notnull where T3 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof(Tuple<T1, T2, T3>);
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new TupleTypeWriter<T1, T2, T3>());
			}
		}
	}

	private ITypeWriter CreateValueTupleTypeWriter(Type type1, Type type2, Type type3)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(ValueTupleTypeWriter<, , >).MakeGenericType(type1, type2, type3));
	}

	private ITypeWriter CreateTupleTypeWriter(Type type1, Type type2, Type type3)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(TupleTypeWriter<, , >).MakeGenericType(type1, type2, type3));
	}

	private static void WriteStructSignature<T1, T2, T3>(ref MessageWriter writer)
	{
		writer.WriteSignature(TypeModel.GetSignature<(T1, T2, T3)>());
	}

	public void WriteStruct<T1, T2, T3, T4>((T1, T2, T3, T4) value) where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull
	{
		WriteStructureStart();
		Write(value.Item1);
		Write(value.Item2);
		Write(value.Item3);
		Write(value.Item4);
	}

	public static void AddValueTupleTypeWriter<T1, T2, T3, T4>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof((T1, T2, T3, T4));
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new ValueTupleTypeWriter<T1, T2, T3, T4>());
			}
		}
	}

	public static void AddTupleTypeWriter<T1, T2, T3, T4>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof(Tuple<T1, T2, T3, T4>);
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new TupleTypeWriter<T1, T2, T3, T4>());
			}
		}
	}

	private ITypeWriter CreateValueTupleTypeWriter(Type type1, Type type2, Type type3, Type type4)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(ValueTupleTypeWriter<, , , >).MakeGenericType(type1, type2, type3, type4));
	}

	private ITypeWriter CreateTupleTypeWriter(Type type1, Type type2, Type type3, Type type4)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(TupleTypeWriter<, , , >).MakeGenericType(type1, type2, type3, type4));
	}

	private static void WriteStructSignature<T1, T2, T3, T4>(ref MessageWriter writer)
	{
		writer.WriteSignature(TypeModel.GetSignature<(T1, T2, T3, T4)>());
	}

	public void WriteStruct<T1, T2, T3, T4, T5>((T1, T2, T3, T4, T5) value) where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull
	{
		WriteStructureStart();
		Write(value.Item1);
		Write(value.Item2);
		Write(value.Item3);
		Write(value.Item4);
		Write(value.Item5);
	}

	public static void AddValueTupleTypeWriter<T1, T2, T3, T4, T5>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof((T1, T2, T3, T4, T5));
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new ValueTupleTypeWriter<T1, T2, T3, T4, T5>());
			}
		}
	}

	public static void AddTupleTypeWriter<T1, T2, T3, T4, T5>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof(Tuple<T1, T2, T3, T4, T5>);
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new TupleTypeWriter<T1, T2, T3, T4, T5>());
			}
		}
	}

	private ITypeWriter CreateValueTupleTypeWriter(Type type1, Type type2, Type type3, Type type4, Type type5)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(ValueTupleTypeWriter<, , , , >).MakeGenericType(type1, type2, type3, type4, type5));
	}

	private ITypeWriter CreateTupleTypeWriter(Type type1, Type type2, Type type3, Type type4, Type type5)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(TupleTypeWriter<, , , , >).MakeGenericType(type1, type2, type3, type4, type5));
	}

	private static void WriteStructSignature<T1, T2, T3, T4, T5>(ref MessageWriter writer)
	{
		writer.WriteSignature(TypeModel.GetSignature<(T1, T2, T3, T4, T5)>());
	}

	public void WriteStruct<T1, T2, T3, T4, T5, T6>((T1, T2, T3, T4, T5, T6) value) where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull
	{
		WriteStructureStart();
		Write(value.Item1);
		Write(value.Item2);
		Write(value.Item3);
		Write(value.Item4);
		Write(value.Item5);
		Write(value.Item6);
	}

	public static void AddValueTupleTypeWriter<T1, T2, T3, T4, T5, T6>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof((T1, T2, T3, T4, T5, T6));
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new ValueTupleTypeWriter<T1, T2, T3, T4, T5, T6>());
			}
		}
	}

	public static void AddTupleTypeWriter<T1, T2, T3, T4, T5, T6>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof(Tuple<T1, T2, T3, T4, T5, T6>);
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new TupleTypeWriter<T1, T2, T3, T4, T5, T6>());
			}
		}
	}

	private ITypeWriter CreateValueTupleTypeWriter(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(ValueTupleTypeWriter<, , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6));
	}

	private ITypeWriter CreateTupleTypeWriter(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(TupleTypeWriter<, , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6));
	}

	private static void WriteStructSignature<T1, T2, T3, T4, T5, T6>(ref MessageWriter writer)
	{
		writer.WriteSignature(TypeModel.GetSignature<(T1, T2, T3, T4, T5, T6)>());
	}

	public void WriteStruct<T1, T2, T3, T4, T5, T6, T7>((T1, T2, T3, T4, T5, T6, T7) value) where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull
	{
		WriteStructureStart();
		Write(value.Item1);
		Write(value.Item2);
		Write(value.Item3);
		Write(value.Item4);
		Write(value.Item5);
		Write(value.Item6);
		Write(value.Item7);
	}

	public static void AddValueTupleTypeWriter<T1, T2, T3, T4, T5, T6, T7>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof((T1, T2, T3, T4, T5, T6, T7));
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new ValueTupleTypeWriter<T1, T2, T3, T4, T5, T6, T7>());
			}
		}
	}

	public static void AddTupleTypeWriter<T1, T2, T3, T4, T5, T6, T7>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof(Tuple<T1, T2, T3, T4, T5, T6, T7>);
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new TupleTypeWriter<T1, T2, T3, T4, T5, T6, T7>());
			}
		}
	}

	private ITypeWriter CreateValueTupleTypeWriter(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(ValueTupleTypeWriter<, , , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6, type7));
	}

	private ITypeWriter CreateTupleTypeWriter(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(TupleTypeWriter<, , , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6, type7));
	}

	private static void WriteStructSignature<T1, T2, T3, T4, T5, T6, T7>(ref MessageWriter writer)
	{
		writer.WriteSignature(TypeModel.GetSignature<(T1, T2, T3, T4, T5, T6, T7)>());
	}

	public void WriteStruct<T1, T2, T3, T4, T5, T6, T7, T8>((T1, T2, T3, T4, T5, T6, T7, T8) value) where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull
	{
		WriteStructureStart();
		Write(value.Item1);
		Write(value.Item2);
		Write(value.Item3);
		Write(value.Item4);
		Write(value.Item5);
		Write(value.Item6);
		Write(value.Item7);
		Write(value.Rest.Item1);
	}

	public static void AddValueTupleTypeWriter<T1, T2, T3, T4, T5, T6, T7, T8>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof((T1, T2, T3, T4, T5, T6, T7, T8));
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new ValueTupleTypeWriter<T1, T2, T3, T4, T5, T6, T7, T8>());
			}
		}
	}

	public static void AddTupleTypeWriter<T1, T2, T3, T4, T5, T6, T7, T8>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof(Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>);
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new TupleTypeWriter<T1, T2, T3, T4, T5, T6, T7, T8>());
			}
		}
	}

	private ITypeWriter CreateValueTupleTypeWriter(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7, Type type8)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(ValueTupleTypeWriter<, , , , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6, type7, type8));
	}

	private ITypeWriter CreateTupleTypeWriter(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7, Type type8)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(TupleTypeWriter<, , , , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6, type7, type8));
	}

	private static void WriteStructSignature<T1, T2, T3, T4, T5, T6, T7, T8>(ref MessageWriter writer)
	{
		writer.WriteSignature(TypeModel.GetSignature<(T1, T2, T3, T4, T5, T6, T7, T8)>());
	}

	public void WriteStruct<T1, T2, T3, T4, T5, T6, T7, T8, T9>((T1, T2, T3, T4, T5, T6, T7, T8, T9) value) where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull where T9 : notnull
	{
		WriteStructureStart();
		Write(value.Item1);
		Write(value.Item2);
		Write(value.Item3);
		Write(value.Item4);
		Write(value.Item5);
		Write(value.Item6);
		Write(value.Item7);
		Write(value.Rest.Item1);
		Write(value.Rest.Item2);
	}

	public static void AddValueTupleTypeWriter<T1, T2, T3, T4, T5, T6, T7, T8, T9>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull where T9 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof((T1, T2, T3, T4, T5, T6, T7, T8, T9));
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new ValueTupleTypeWriter<T1, T2, T3, T4, T5, T6, T7, T8, T9>());
			}
		}
	}

	public static void AddTupleTypeWriter<T1, T2, T3, T4, T5, T6, T7, T8, T9>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull where T9 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof(Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>);
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new TupleTypeWriter<T1, T2, T3, T4, T5, T6, T7, T8, T9>());
			}
		}
	}

	private ITypeWriter CreateValueTupleTypeWriter(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7, Type type8, Type type9)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(ValueTupleTypeWriter<, , , , , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6, type7, type8));
	}

	private ITypeWriter CreateTupleTypeWriter(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7, Type type8, Type type9)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(TupleTypeWriter<, , , , , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6, type7, type8, type9));
	}

	private static void WriteStructSignature<T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref MessageWriter writer)
	{
		writer.WriteSignature(TypeModel.GetSignature<(T1, T2, T3, T4, T5, T6, T7, T8, T9)>());
	}

	public void WriteStruct<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>((T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) value) where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull where T9 : notnull where T10 : notnull
	{
		WriteStructureStart();
		Write(value.Item1);
		Write(value.Item2);
		Write(value.Item3);
		Write(value.Item4);
		Write(value.Item5);
		Write(value.Item6);
		Write(value.Item7);
		Write(value.Rest.Item1);
		Write(value.Rest.Item2);
		Write(value.Rest.Item3);
	}

	public static void AddValueTupleTypeWriter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull where T9 : notnull where T10 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof((T1, T2, T3, T4, T5, T6, T7, T8, T9, T10));
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new ValueTupleTypeWriter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>());
			}
		}
	}

	public static void AddTupleTypeWriter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>() where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull where T6 : notnull where T7 : notnull where T8 : notnull where T9 : notnull where T10 : notnull
	{
		lock (_typeWriters)
		{
			Type typeFromHandle = typeof(Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>);
			if (!_typeWriters.ContainsKey(typeFromHandle))
			{
				_typeWriters.Add(typeFromHandle, new TupleTypeWriter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>());
			}
		}
	}

	private ITypeWriter CreateValueTupleTypeWriter(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7, Type type8, Type type9, Type type10)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(ValueTupleTypeWriter<, , , , , , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6, type7, type8, type10));
	}

	private ITypeWriter CreateTupleTypeWriter(Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7, Type type8, Type type9, Type type10)
	{
		return (ITypeWriter)Activator.CreateInstance(typeof(TupleTypeWriter<, , , , , , , , , >).MakeGenericType(type1, type2, type3, type4, type5, type6, type7, type8, type9, type10));
	}

	private static void WriteStructSignature<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref MessageWriter writer)
	{
		writer.WriteSignature(TypeModel.GetSignature<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)>());
	}

	public void WriteVariant(object value)
	{
		Type type = value.GetType();
		if (type == typeof(byte))
		{
			WriteVariantByte((byte)value);
		}
		else if (type == typeof(bool))
		{
			WriteVariantBool((bool)value);
		}
		else if (type == typeof(short))
		{
			WriteVariantInt16((short)value);
		}
		else if (type == typeof(ushort))
		{
			WriteVariantUInt16((ushort)value);
		}
		else if (type == typeof(int))
		{
			WriteVariantInt32((int)value);
		}
		else if (type == typeof(uint))
		{
			WriteVariantUInt32((uint)value);
		}
		else if (type == typeof(long))
		{
			WriteVariantInt64((long)value);
		}
		else if (type == typeof(ulong))
		{
			WriteVariantUInt64((ulong)value);
		}
		else if (type == typeof(double))
		{
			WriteVariantDouble((double)value);
		}
		else if (type == typeof(string))
		{
			WriteVariantString((string)value);
		}
		else if (type == typeof(ObjectPath))
		{
			WriteVariantObjectPath(value.ToString());
		}
		else if (type == typeof(Signature))
		{
			WriteVariantSignature(value.ToString());
		}
		else
		{
			GetTypeWriter(type).WriteVariant(ref this, value);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Write<T>(T value) where T : notnull
	{
		Type typeFromHandle = typeof(T);
		if (typeFromHandle == typeof(object))
		{
			WriteVariant(value);
		}
		else if (typeFromHandle == typeof(byte))
		{
			WriteByte((byte)(object)value);
		}
		else if (typeFromHandle == typeof(bool))
		{
			WriteBool((bool)(object)value);
		}
		else if (typeFromHandle == typeof(short))
		{
			WriteInt16((short)(object)value);
		}
		else if (typeFromHandle == typeof(ushort))
		{
			WriteUInt16((ushort)(object)value);
		}
		else if (typeFromHandle == typeof(int))
		{
			WriteInt32((int)(object)value);
		}
		else if (typeFromHandle == typeof(uint))
		{
			WriteUInt32((uint)(object)value);
		}
		else if (typeFromHandle == typeof(long))
		{
			WriteInt64((long)(object)value);
		}
		else if (typeFromHandle == typeof(ulong))
		{
			WriteUInt64((ulong)(object)value);
		}
		else if (typeFromHandle == typeof(double))
		{
			WriteDouble((double)(object)value);
		}
		else if (typeFromHandle == typeof(string))
		{
			WriteString((string)(object)value);
		}
		else if (typeFromHandle == typeof(ObjectPath))
		{
			WriteString(((ObjectPath)(object)value/*cast due to .constrained prefix*/).ToString());
		}
		else if (typeFromHandle == typeof(Signature))
		{
			WriteSignature(((Signature)(object)value/*cast due to .constrained prefix*/).ToString());
		}
		else
		{
			((ITypeWriter<T>)GetTypeWriter(typeFromHandle)).Write(ref this, value);
		}
	}

	private ITypeWriter GetTypeWriter(Type type)
	{
		lock (_typeWriters)
		{
			if (_typeWriters.TryGetValue(type, out ITypeWriter value))
			{
				return value;
			}
			value = CreateWriterForType(type);
			_typeWriters.Add(type, value);
			return value;
		}
	}

	private ITypeWriter CreateWriterForType(Type type)
	{
		if (type.IsGenericType && type.FullName.StartsWith("System.ValueTuple"))
		{
			switch (type.GenericTypeArguments.Length)
			{
			case 1:
				return CreateValueTupleTypeWriter(type.GenericTypeArguments[0]);
			case 2:
				return CreateValueTupleTypeWriter(type.GenericTypeArguments[0], type.GenericTypeArguments[1]);
			case 3:
				return CreateValueTupleTypeWriter(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2]);
			case 4:
				return CreateValueTupleTypeWriter(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3]);
			case 5:
				return CreateValueTupleTypeWriter(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4]);
			case 6:
				return CreateValueTupleTypeWriter(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5]);
			case 7:
				return CreateValueTupleTypeWriter(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5], type.GenericTypeArguments[6]);
			case 8:
				switch (type.GenericTypeArguments[7].GenericTypeArguments.Length)
				{
				case 1:
					return CreateValueTupleTypeWriter(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5], type.GenericTypeArguments[6], type.GenericTypeArguments[7].GenericTypeArguments[0]);
				case 2:
					return CreateValueTupleTypeWriter(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5], type.GenericTypeArguments[6], type.GenericTypeArguments[7].GenericTypeArguments[0], type.GenericTypeArguments[7].GenericTypeArguments[1]);
				case 3:
					return CreateValueTupleTypeWriter(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5], type.GenericTypeArguments[6], type.GenericTypeArguments[7].GenericTypeArguments[0], type.GenericTypeArguments[7].GenericTypeArguments[1], type.GenericTypeArguments[7].GenericTypeArguments[2]);
				}
				break;
			}
		}
		if (type.IsGenericType && type.FullName.StartsWith("System.Tuple"))
		{
			switch (type.GenericTypeArguments.Length)
			{
			case 1:
				return CreateTupleTypeWriter(type.GenericTypeArguments[0]);
			case 2:
				return CreateTupleTypeWriter(type.GenericTypeArguments[0], type.GenericTypeArguments[1]);
			case 3:
				return CreateTupleTypeWriter(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2]);
			case 4:
				return CreateTupleTypeWriter(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3]);
			case 5:
				return CreateTupleTypeWriter(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4]);
			case 6:
				return CreateTupleTypeWriter(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5]);
			case 7:
				return CreateTupleTypeWriter(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5], type.GenericTypeArguments[6]);
			case 8:
				switch (type.GenericTypeArguments[7].GenericTypeArguments.Length)
				{
				case 1:
					return CreateTupleTypeWriter(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5], type.GenericTypeArguments[6], type.GenericTypeArguments[7].GenericTypeArguments[0]);
				case 2:
					return CreateTupleTypeWriter(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5], type.GenericTypeArguments[6], type.GenericTypeArguments[7].GenericTypeArguments[0], type.GenericTypeArguments[7].GenericTypeArguments[1]);
				case 3:
					return CreateTupleTypeWriter(type.GenericTypeArguments[0], type.GenericTypeArguments[1], type.GenericTypeArguments[2], type.GenericTypeArguments[3], type.GenericTypeArguments[4], type.GenericTypeArguments[5], type.GenericTypeArguments[6], type.GenericTypeArguments[7].GenericTypeArguments[0], type.GenericTypeArguments[7].GenericTypeArguments[1], type.GenericTypeArguments[7].GenericTypeArguments[2]);
				}
				break;
			}
		}
		Type type2 = TypeModel.ExtractGenericInterface(type, typeof(IEnumerable<>));
		if (type2 != null)
		{
			if (_typeWriters.TryGetValue(type2, out ITypeWriter value))
			{
				return value;
			}
			Type type3 = type2.GenericTypeArguments[0];
			if (type3.IsGenericType && type3.GetGenericTypeDefinition() == typeof(KeyValuePair<, >))
			{
				Type keyType = type3.GenericTypeArguments[0];
				Type valueType = type3.GenericTypeArguments[1];
				value = CreateDictionaryTypeWriter(keyType, valueType);
			}
			else
			{
				value = CreateArrayTypeWriter(type3);
			}
			if (type != type2)
			{
				_typeWriters.Add(type2, value);
			}
			return value;
		}
		ThrowNotSupportedType(type);
		return null;
	}

	private static void ThrowNotSupportedType(Type type)
	{
		throw new NotSupportedException("Cannot write type " + type.FullName);
	}
}
