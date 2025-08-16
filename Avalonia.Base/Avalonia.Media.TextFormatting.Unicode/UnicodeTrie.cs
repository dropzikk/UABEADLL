using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Avalonia.Media.TextFormatting.Unicode;

internal class UnicodeTrie
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	private struct UnicodeTrieHeader
	{
		public int HighStart
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
		}

		public uint ErrorValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
		}

		public int DataLength
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UnicodeTrieHeader Parse(ReadOnlySpan<byte> data)
		{
			return MemoryMarshal.Cast<byte, UnicodeTrieHeader>(data)[0];
		}
	}

	private readonly uint[] _data;

	private readonly int _highStart;

	private readonly uint _errorValue;

	public UnicodeTrie(ReadOnlySpan<byte> rawData)
	{
		UnicodeTrieHeader unicodeTrieHeader = UnicodeTrieHeader.Parse(rawData);
		int dataLength = unicodeTrieHeader.DataLength;
		uint[] array = new uint[dataLength / 4];
		MemoryMarshal.Cast<byte, uint>(rawData.Slice(rawData.Length - dataLength)).CopyTo(array);
		_highStart = unicodeTrieHeader.HighStart;
		_errorValue = unicodeTrieHeader.ErrorValue;
		_data = array;
	}

	public UnicodeTrie(Stream stream)
	{
		using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true))
		{
			_highStart = binaryReader.ReadInt32();
			_errorValue = binaryReader.ReadUInt32();
			_data = new uint[binaryReader.ReadInt32() / 4];
		}
		using BinaryReader binaryReader2 = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
		for (int i = 0; i < _data.Length; i++)
		{
			_data[i] = binaryReader2.ReadUInt32();
		}
	}

	public UnicodeTrie(uint[] data, int highStart, uint errorValue)
	{
		_data = data;
		_highStart = highStart;
		_errorValue = errorValue;
	}

	internal void Save(Stream stream)
	{
		using (BinaryWriter binaryWriter = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
		{
			binaryWriter.Write(_highStart);
			binaryWriter.Write(_errorValue);
			binaryWriter.Write(_data.Length * 4);
		}
		using BinaryWriter binaryWriter2 = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
		for (int i = 0; i < _data.Length; i++)
		{
			binaryWriter2.Write(_data[i]);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public uint Get(uint codePoint)
	{
		ref uint reference = ref MemoryMarshal.GetReference(_data.AsSpan());
		if (codePoint > 56319)
		{
			if (codePoint <= 65535)
			{
				goto IL_002b;
			}
		}
		else if (codePoint < 55296)
		{
			goto IL_002b;
		}
		bool flag = false;
		goto IL_0031;
		IL_0031:
		if (flag)
		{
			uint num = _data[codePoint >> 5];
			num = (num << 2) + (codePoint & 0x1F);
			return Unsafe.Add(ref reference, (nint)num);
		}
		if (codePoint <= 65535)
		{
			uint num = _data[2048 + (codePoint - 55296 >> 5)];
			num = (num << 2) + (codePoint & 0x1F);
			return Unsafe.Add(ref reference, (nint)num);
		}
		if (codePoint < _highStart)
		{
			uint num = 2080 + (codePoint >> 11);
			num = _data[num];
			num += (codePoint >> 5) & 0x3F;
			num = _data[num];
			num = (num << 2) + (codePoint & 0x1F);
			return Unsafe.Add(ref reference, (nint)num);
		}
		if (codePoint <= 1114111)
		{
			return Unsafe.Add(ref reference, (IntPtr)(_data.Length - 4));
		}
		return _errorValue;
		IL_002b:
		flag = true;
		goto IL_0031;
	}
}
