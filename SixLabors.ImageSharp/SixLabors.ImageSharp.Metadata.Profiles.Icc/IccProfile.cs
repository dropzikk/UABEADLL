using System;
using System.Security.Cryptography;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

public sealed class IccProfile : IDeepCloneable<IccProfile>
{
	private readonly byte[] data;

	private IccTagDataEntry[] entries;

	private IccProfileHeader header;

	public IccProfileHeader Header
	{
		get
		{
			InitializeHeader();
			return header;
		}
		set
		{
			header = value;
		}
	}

	public IccTagDataEntry[] Entries
	{
		get
		{
			InitializeEntries();
			return entries;
		}
	}

	public IccProfile()
		: this((byte[])null)
	{
	}

	public IccProfile(byte[] data)
	{
		this.data = data;
	}

	internal IccProfile(IccProfileHeader header, IccTagDataEntry[] entries)
	{
		this.header = header ?? throw new ArgumentNullException("header");
		this.entries = entries ?? throw new ArgumentNullException("entries");
	}

	private IccProfile(IccProfile other)
	{
		Guard.NotNull(other, "other");
		data = other.ToByteArray();
	}

	public IccProfile DeepClone()
	{
		return new IccProfile(this);
	}

	public static IccProfileId CalculateHash(byte[] data)
	{
		Guard.NotNull(data, "data");
		Guard.IsTrue(data.Length >= 128, "data", "Data length must be at least 128 to be a valid profile header");
		Span<byte> destination = stackalloc byte[24];
		data.AsSpan(44, 4).CopyTo(destination);
		data.AsSpan(64, 4).CopyTo(destination.Slice(4));
		data.AsSpan(84, 16).CopyTo(destination.Slice(8));
		try
		{
			Array.Clear(data, 44, 4);
			Array.Clear(data, 64, 4);
			Array.Clear(data, 84, 16);
			return new IccDataReader(MD5.HashData(data)).ReadProfileId();
		}
		finally
		{
			destination.Slice(0, 4).CopyTo(data.AsSpan(44));
			destination.Slice(4, 4).CopyTo(data.AsSpan(64));
			destination.Slice(8, 16).CopyTo(data.AsSpan(84));
		}
	}

	public bool CheckIsValid()
	{
		bool flag = true;
		if (data != null)
		{
			flag = data.Length >= 128 && data.Length >= Header.Size;
		}
		if (flag && Enum.IsDefined(typeof(IccColorSpaceType), Header.DataColorSpace) && Enum.IsDefined(typeof(IccColorSpaceType), Header.ProfileConnectionSpace) && Enum.IsDefined(typeof(IccRenderingIntent), Header.RenderingIntent))
		{
			uint size = Header.Size;
			if (size >= 128)
			{
				return size < 50000000;
			}
			return false;
		}
		return false;
	}

	public byte[] ToByteArray()
	{
		if (data != null)
		{
			byte[] array = new byte[data.Length];
			Buffer.BlockCopy(data, 0, array, 0, array.Length);
			return array;
		}
		return IccWriter.Write(this);
	}

	private void InitializeHeader()
	{
		if (header == null)
		{
			if (data == null)
			{
				header = new IccProfileHeader();
				return;
			}
			new IccReader();
			header = IccReader.ReadHeader(data);
		}
	}

	private void InitializeEntries()
	{
		if (entries == null)
		{
			if (data == null)
			{
				entries = Array.Empty<IccTagDataEntry>();
				return;
			}
			new IccReader();
			entries = IccReader.ReadTagData(data);
		}
	}
}
