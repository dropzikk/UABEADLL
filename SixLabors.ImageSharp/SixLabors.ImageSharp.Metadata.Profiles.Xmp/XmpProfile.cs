using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace SixLabors.ImageSharp.Metadata.Profiles.Xmp;

public sealed class XmpProfile : IDeepCloneable<XmpProfile>
{
	internal byte[]? Data { get; private set; }

	public XmpProfile()
		: this((byte[]?)null)
	{
	}

	public XmpProfile(byte[]? data)
	{
		Data = data;
	}

	private XmpProfile(XmpProfile other)
	{
		Guard.NotNull(other, "other");
		Data = other.Data;
	}

	public XDocument? GetDocument()
	{
		byte[] data = Data;
		if (data == null)
		{
			return null;
		}
		int num = data.Length;
		for (int num2 = num - 1; num2 > 0; num2--)
		{
			byte b = data[num2];
			if ((b == 0 || b == 15) ? true : false)
			{
				num--;
			}
		}
		using MemoryStream stream = new MemoryStream(data, 0, num);
		using StreamReader textReader = new StreamReader(stream, Encoding.UTF8);
		return XDocument.Load((TextReader)textReader);
	}

	public byte[] ToByteArray()
	{
		Guard.NotNull(Data, "this.Data");
		byte[] array = new byte[Data.Length];
		Data.AsSpan().CopyTo(array);
		return array;
	}

	public XmpProfile DeepClone()
	{
		return new XmpProfile(this);
	}
}
