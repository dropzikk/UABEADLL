using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using LibCpp2IL.Logging;

namespace LibCpp2IL;

public class ClassReadingBinaryReader : EndianAwareBinaryReader
{
	private static readonly Dictionary<Type, FieldInfo[]> CachedFields = new Dictionary<Type, FieldInfo[]>();

	private SpinLock PositionShiftLock;

	public bool is32Bit;

	private MemoryStream _memoryStream;

	private Dictionary<FieldInfo, bool> _cachedNoSerialize = new Dictionary<FieldInfo, bool>();

	public long Position
	{
		get
		{
			return BaseStream.Position;
		}
		protected set
		{
			BaseStream.Position = value;
		}
	}

	public ClassReadingBinaryReader(MemoryStream input)
		: base(input)
	{
		_memoryStream = input;
	}

	internal virtual object? ReadPrimitive(Type type, bool overrideArchCheck = false)
	{
		if (type == typeof(bool))
		{
			return ReadBoolean();
		}
		if (type == typeof(char))
		{
			return ReadChar();
		}
		if (type == typeof(int))
		{
			return ReadInt32();
		}
		if (type == typeof(uint))
		{
			return ReadUInt32();
		}
		if (type == typeof(short))
		{
			return ReadInt16();
		}
		if (type == typeof(ushort))
		{
			return ReadUInt16();
		}
		if (type == typeof(sbyte))
		{
			return ReadSByte();
		}
		if (type == typeof(byte))
		{
			return ReadByte();
		}
		if (type == typeof(long))
		{
			return (is32Bit && !overrideArchCheck) ? ReadInt32() : ReadInt64();
		}
		if (type == typeof(ulong))
		{
			return (is32Bit && !overrideArchCheck) ? ReadUInt32() : ReadUInt64();
		}
		if (type == typeof(float))
		{
			return ReadSingle();
		}
		if (type == typeof(double))
		{
			return ReadDouble();
		}
		return null;
	}

	public T ReadClassAtRawAddr<T>(long offset, bool overrideArchCheck = false) where T : new()
	{
		bool lockTaken = false;
		PositionShiftLock.Enter(ref lockTaken);
		if (!lockTaken)
		{
			throw new Exception("Failed to obtain lock");
		}
		try
		{
			if (offset >= 0)
			{
				Position = offset;
			}
			return InternalReadClass<T>(overrideArchCheck);
		}
		finally
		{
			PositionShiftLock.Exit();
		}
	}

	public uint ReadUnityCompressedUIntAtRawAddr(long offset, out int bytesRead)
	{
		bool lockTaken = false;
		PositionShiftLock.Enter(ref lockTaken);
		if (!lockTaken)
		{
			throw new Exception("Failed to obtain lock");
		}
		try
		{
			if (offset >= 0)
			{
				Position = offset;
			}
			byte b = ReadByte();
			bytesRead = 1;
			if (b < 128)
			{
				return b;
			}
			switch (b)
			{
			case 240:
				bytesRead = 5;
				return ReadUInt32();
			case byte.MaxValue:
				return uint.MaxValue;
			case 254:
				return 4294967294u;
			default:
				if ((b & 0xC0) == 192)
				{
					bytesRead = 4;
					return (uint)(((b & -193) << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte());
				}
				if ((b & 0x80) == 128)
				{
					bytesRead = 2;
					return (uint)(((b & -129) << 8) | ReadByte());
				}
				throw new Exception($"How did we even get here? Invalid compressed int first byte {b}");
			}
		}
		finally
		{
			PositionShiftLock.Exit();
		}
	}

	public int ReadUnityCompressedIntAtRawAddr(long position, out int bytesRead)
	{
		uint num = ReadUnityCompressedUIntAtRawAddr(position, out bytesRead);
		if (num == uint.MaxValue)
		{
			return int.MinValue;
		}
		bool num2 = (num & 1) == 1;
		num >>= 1;
		if (num2)
		{
			return (int)(0 - (num + 1));
		}
		return (int)num;
	}

	private T InternalReadClass<T>(bool overrideArchCheck = false) where T : new()
	{
		return (T)InternalReadClass(typeof(T), overrideArchCheck);
	}

	private object InternalReadClass(Type type, bool overrideArchCheck = false)
	{
		object obj = Activator.CreateInstance(type);
		if (type.IsPrimitive)
		{
			return ReadAndConvertPrimitive(overrideArchCheck, type);
		}
		if (type.IsEnum)
		{
			return ReadPrimitive(type.GetEnumUnderlyingType());
		}
		ReadClassFieldwise(overrideArchCheck, obj);
		return obj;
	}

	private object ReadAndConvertPrimitive(bool overrideArchCheck, Type type)
	{
		object obj = ReadPrimitive(type, overrideArchCheck);
		if (obj is uint && type == typeof(ulong))
		{
			obj = Convert.ToUInt64(obj);
		}
		if (obj is int && type == typeof(long))
		{
			obj = Convert.ToInt64(obj);
		}
		return obj;
	}

	private void ReadClassFieldwise<T>(bool overrideArchCheck, T t) where T : new()
	{
		if (t == null)
		{
			throw new ArgumentNullException("t");
		}
		FieldInfo[] fieldsCached = GetFieldsCached(t.GetType());
		foreach (FieldInfo fieldInfo in fieldsCached)
		{
			if (!_cachedNoSerialize.ContainsKey(fieldInfo))
			{
				_cachedNoSerialize[fieldInfo] = Attribute.GetCustomAttribute(fieldInfo, typeof(NonSerializedAttribute)) != null;
			}
			if (!_cachedNoSerialize[fieldInfo] && LibCpp2ILUtils.ShouldReadFieldOnThisVersion(fieldInfo))
			{
				if (fieldInfo.FieldType.IsPrimitive)
				{
					fieldInfo.SetValue(t, ReadPrimitive(fieldInfo.FieldType));
				}
				else
				{
					fieldInfo.SetValue(t, InternalReadClass(fieldInfo.FieldType, overrideArchCheck));
				}
			}
		}
	}

	public T[] ReadClassArrayAtRawAddr<T>(ulong offset, ulong count) where T : new()
	{
		return ReadClassArrayAtRawAddr<T>((long)offset, (long)count);
	}

	public T[] ReadClassArrayAtRawAddr<T>(long offset, long count) where T : new()
	{
		T[] array = new T[count];
		bool lockTaken = false;
		PositionShiftLock.Enter(ref lockTaken);
		if (!lockTaken)
		{
			throw new Exception("Failed to obtain lock");
		}
		try
		{
			if (offset != -1)
			{
				Position = offset;
			}
			for (int i = 0; i < count; i++)
			{
				array[i] = InternalReadClass<T>();
			}
			return array;
		}
		finally
		{
			PositionShiftLock.Exit();
		}
	}

	public string ReadStringToNull(ulong offset)
	{
		return ReadStringToNull((long)offset);
	}

	public virtual string ReadStringToNull(long offset)
	{
		List<byte> list = new List<byte>();
		bool lockTaken = false;
		PositionShiftLock.Enter(ref lockTaken);
		if (!lockTaken)
		{
			throw new Exception("Failed to obtain lock");
		}
		try
		{
			Position = offset;
			byte item;
			while ((item = (byte)_memoryStream.ReadByte()) != 0)
			{
				list.Add(item);
			}
			return Encoding.UTF8.GetString(list.ToArray());
		}
		finally
		{
			PositionShiftLock.Exit();
		}
	}

	public byte[] ReadByteArrayAtRawAddress(long offset, int count)
	{
		bool lockTaken = false;
		PositionShiftLock.Enter(ref lockTaken);
		if (!lockTaken)
		{
			throw new Exception("Failed to obtain lock");
		}
		try
		{
			Position = offset;
			byte[] array = new byte[count];
			Read(array, 0, count);
			return array;
		}
		finally
		{
			PositionShiftLock.Exit();
		}
	}

	protected void WriteWord(int position, ulong word)
	{
		WriteWord(position, (long)word);
	}

	protected void WriteWord(int position, long word)
	{
		bool lockTaken = false;
		PositionShiftLock.Enter(ref lockTaken);
		if (!lockTaken)
		{
			throw new Exception("Failed to obtain lock");
		}
		try
		{
			byte[] array = ((!is32Bit) ? BitConverter.GetBytes(word) : BitConverter.GetBytes((int)word));
			if (shouldReverseArrays)
			{
				array = array.Reverse();
			}
			if (position > _memoryStream.Length)
			{
				throw new Exception($"WriteWord: Position {position} beyond length {_memoryStream.Length}");
			}
			int num = (is32Bit ? 4 : 8);
			if (position + num > _memoryStream.Length)
			{
				throw new Exception($"WriteWord: Writing {num} bytes at {position} would go beyond length {_memoryStream.Length}");
			}
			if (array.Length != num)
			{
				throw new Exception($"WriteWord: Expected {num} bytes from BitConverter, got {position}");
			}
			try
			{
				_memoryStream.Seek(position, SeekOrigin.Begin);
				_memoryStream.Write(array, 0, num);
			}
			catch
			{
				LibLogger.ErrorNewline("WriteWord: Unexpected exception!");
				throw;
			}
		}
		finally
		{
			PositionShiftLock.Exit();
		}
	}

	private static FieldInfo[] GetFieldsCached(Type t)
	{
		if (CachedFields.TryGetValue(t, out FieldInfo[] value))
		{
			return value;
		}
		return CachedFields[t] = t.GetFields();
	}

	public ulong ReadNUint()
	{
		if (!is32Bit)
		{
			return ReadUInt64();
		}
		return ReadUInt32();
	}
}
