using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AssetRipper.TextureDecoder.Astc;
using AssetRipper.TextureDecoder.Atc;
using AssetRipper.TextureDecoder.Bc;
using AssetRipper.TextureDecoder.Dxt;
using AssetRipper.TextureDecoder.Etc;
using AssetRipper.TextureDecoder.Pvrtc;
using AssetRipper.TextureDecoder.Rgb;
using AssetRipper.TextureDecoder.Rgb.Formats;
using AssetRipper.TextureDecoder.Yuy2;
using AssetsTools.NET.Extra;

namespace AssetsTools.NET.Texture;

public class TextureFile
{
	public struct GLTextureSettings
	{
		public int m_FilterMode;

		public int m_Aniso;

		public float m_MipBias;

		public int m_WrapMode;

		public int m_WrapU;

		public int m_WrapV;

		public int m_WrapW;
	}

	public struct StreamingInfo
	{
		public ulong offset;

		public uint size;

		public string path;
	}

	public string m_Name;

	public int m_ForcedFallbackFormat;

	public bool m_DownscaleFallback;

	public int m_Width;

	public int m_Height;

	public int m_CompleteImageSize;

	public int m_TextureFormat;

	public int m_MipCount;

	public bool m_MipMap;

	public bool m_IsReadable;

	public bool m_ReadAllowed;

	public bool m_StreamingMipmaps;

	public int m_StreamingMipmapsPriority;

	public int m_ImageCount;

	public int m_TextureDimension;

	public GLTextureSettings m_TextureSettings;

	public int m_LightmapFormat;

	public int m_ColorSpace;

	public byte[] pictureData;

	public StreamingInfo m_StreamData;

	public static TextureFile ReadTextureFile(AssetTypeValueField baseField)
	{
		TextureFile textureFile = new TextureFile();
		textureFile.m_Name = baseField["m_Name"].AsString;
		AssetTypeValueField assetTypeValueField;
		if (!(assetTypeValueField = baseField["m_ForcedFallbackFormat"]).IsDummy)
		{
			textureFile.m_ForcedFallbackFormat = assetTypeValueField.AsInt;
		}
		if (!(assetTypeValueField = baseField["m_DownscaleFallback"]).IsDummy)
		{
			textureFile.m_DownscaleFallback = assetTypeValueField.AsBool;
		}
		textureFile.m_Width = baseField["m_Width"].AsInt;
		textureFile.m_Height = baseField["m_Height"].AsInt;
		if (!(assetTypeValueField = baseField["m_CompleteImageSize"]).IsDummy)
		{
			textureFile.m_CompleteImageSize = assetTypeValueField.AsInt;
		}
		textureFile.m_TextureFormat = baseField["m_TextureFormat"].AsInt;
		if (!(assetTypeValueField = baseField["m_MipCount"]).IsDummy)
		{
			textureFile.m_MipCount = assetTypeValueField.AsInt;
		}
		if (!(assetTypeValueField = baseField["m_MipMap"]).IsDummy)
		{
			textureFile.m_MipMap = assetTypeValueField.AsBool;
		}
		textureFile.m_IsReadable = baseField["m_IsReadable"].AsBool;
		if (!(assetTypeValueField = baseField["m_ReadAllowed"]).IsDummy)
		{
			textureFile.m_ReadAllowed = assetTypeValueField.AsBool;
		}
		if (!(assetTypeValueField = baseField["m_StreamingMipmaps"]).IsDummy)
		{
			textureFile.m_StreamingMipmaps = assetTypeValueField.AsBool;
		}
		if (!(assetTypeValueField = baseField["m_StreamingMipmapsPriority"]).IsDummy)
		{
			textureFile.m_StreamingMipmapsPriority = assetTypeValueField.AsInt;
		}
		textureFile.m_ImageCount = baseField["m_ImageCount"].AsInt;
		textureFile.m_TextureDimension = baseField["m_TextureDimension"].AsInt;
		AssetTypeValueField assetTypeValueField2 = baseField["m_TextureSettings"];
		textureFile.m_TextureSettings.m_FilterMode = assetTypeValueField2["m_FilterMode"].AsInt;
		textureFile.m_TextureSettings.m_Aniso = assetTypeValueField2["m_Aniso"].AsInt;
		textureFile.m_TextureSettings.m_MipBias = assetTypeValueField2["m_MipBias"].AsFloat;
		if (!(assetTypeValueField = assetTypeValueField2["m_WrapMode"]).IsDummy)
		{
			textureFile.m_TextureSettings.m_WrapMode = assetTypeValueField.AsInt;
		}
		if (!(assetTypeValueField = assetTypeValueField2["m_WrapU"]).IsDummy)
		{
			textureFile.m_TextureSettings.m_WrapU = assetTypeValueField.AsInt;
		}
		if (!(assetTypeValueField = assetTypeValueField2["m_WrapV"]).IsDummy)
		{
			textureFile.m_TextureSettings.m_WrapV = assetTypeValueField.AsInt;
		}
		if (!(assetTypeValueField = assetTypeValueField2["m_WrapW"]).IsDummy)
		{
			textureFile.m_TextureSettings.m_WrapW = assetTypeValueField.AsInt;
		}
		if (!(assetTypeValueField = baseField["m_LightmapFormat"]).IsDummy)
		{
			textureFile.m_LightmapFormat = assetTypeValueField.AsInt;
		}
		if (!(assetTypeValueField = baseField["m_ColorSpace"]).IsDummy)
		{
			textureFile.m_ColorSpace = assetTypeValueField.AsInt;
		}
		AssetTypeValueField assetTypeValueField3 = baseField["image data"];
		if (assetTypeValueField3.TemplateField.ValueType == AssetValueType.ByteArray)
		{
			textureFile.pictureData = assetTypeValueField3.AsByteArray;
		}
		else
		{
			int count = assetTypeValueField3.Children.Count;
			textureFile.pictureData = new byte[count];
			for (int i = 0; i < count; i++)
			{
				textureFile.pictureData[i] = (byte)assetTypeValueField3[i].AsInt;
			}
		}
		AssetTypeValueField assetTypeValueField4;
		if (!(assetTypeValueField4 = baseField["m_StreamData"]).IsDummy)
		{
			textureFile.m_StreamData.offset = assetTypeValueField4["offset"].AsULong;
			textureFile.m_StreamData.size = assetTypeValueField4["size"].AsUInt;
			textureFile.m_StreamData.path = assetTypeValueField4["path"].AsString;
		}
		return textureFile;
	}

	public void WriteTo(AssetTypeValueField baseField)
	{
		baseField["m_Name"].AsString = m_Name;
		AssetTypeValueField assetTypeValueField;
		if (!(assetTypeValueField = baseField["m_ForcedFallbackFormat"]).IsDummy)
		{
			assetTypeValueField.AsInt = m_ForcedFallbackFormat;
		}
		if (!(assetTypeValueField = baseField["m_DownscaleFallback"]).IsDummy)
		{
			assetTypeValueField.AsBool = m_DownscaleFallback;
		}
		baseField["m_Width"].AsInt = m_Width;
		baseField["m_Height"].AsInt = m_Height;
		if (!(assetTypeValueField = baseField["m_CompleteImageSize"]).IsDummy)
		{
			assetTypeValueField.AsInt = m_CompleteImageSize;
		}
		baseField["m_TextureFormat"].AsInt = m_TextureFormat;
		if (!(assetTypeValueField = baseField["m_MipCount"]).IsDummy)
		{
			assetTypeValueField.AsInt = m_MipCount;
		}
		if (!(assetTypeValueField = baseField["m_MipMap"]).IsDummy)
		{
			assetTypeValueField.AsBool = m_MipMap;
		}
		baseField["m_IsReadable"].AsBool = m_IsReadable;
		if (!(assetTypeValueField = baseField["m_ReadAllowed"]).IsDummy)
		{
			assetTypeValueField.AsBool = m_ReadAllowed;
		}
		if (!(assetTypeValueField = baseField["m_StreamingMipmaps"]).IsDummy)
		{
			assetTypeValueField.AsBool = m_StreamingMipmaps;
		}
		if (!(assetTypeValueField = baseField["m_StreamingMipmapsPriority"]).IsDummy)
		{
			assetTypeValueField.AsInt = m_StreamingMipmapsPriority;
		}
		baseField["m_ImageCount"].AsInt = m_ImageCount;
		baseField["m_TextureDimension"].AsInt = m_TextureDimension;
		AssetTypeValueField assetTypeValueField2 = baseField["m_TextureSettings"];
		assetTypeValueField2["m_FilterMode"].AsInt = m_TextureSettings.m_FilterMode;
		assetTypeValueField2["m_Aniso"].AsInt = m_TextureSettings.m_Aniso;
		assetTypeValueField2["m_MipBias"].AsFloat = m_TextureSettings.m_MipBias;
		if (!(assetTypeValueField = assetTypeValueField2["m_WrapMode"]).IsDummy)
		{
			assetTypeValueField.AsInt = m_TextureSettings.m_WrapMode;
		}
		if (!(assetTypeValueField = assetTypeValueField2["m_WrapU"]).IsDummy)
		{
			assetTypeValueField.AsInt = m_TextureSettings.m_WrapU;
		}
		if (!(assetTypeValueField = assetTypeValueField2["m_WrapV"]).IsDummy)
		{
			assetTypeValueField.AsInt = m_TextureSettings.m_WrapV;
		}
		if (!(assetTypeValueField = assetTypeValueField2["m_WrapW"]).IsDummy)
		{
			assetTypeValueField.AsInt = m_TextureSettings.m_WrapW;
		}
		if (!(assetTypeValueField = baseField["m_LightmapFormat"]).IsDummy)
		{
			assetTypeValueField.AsInt = m_LightmapFormat;
		}
		if (!(assetTypeValueField = baseField["m_ColorSpace"]).IsDummy)
		{
			assetTypeValueField.AsInt = m_ColorSpace;
		}
		AssetTypeValueField assetTypeValueField3 = baseField["image data"];
		if (assetTypeValueField3.TemplateField.ValueType == AssetValueType.ByteArray)
		{
			assetTypeValueField3.AsByteArray = pictureData;
		}
		else
		{
			assetTypeValueField3.AsArray = new AssetTypeArrayInfo(pictureData.Length);
			List<AssetTypeValueField> list = new List<AssetTypeValueField>(pictureData.Length);
			for (int i = 0; i < pictureData.Length; i++)
			{
				AssetTypeValueField assetTypeValueField4 = ValueBuilder.DefaultValueFieldFromArrayTemplate(assetTypeValueField3);
				assetTypeValueField4.AsByte = pictureData[i];
				list[i] = assetTypeValueField4;
			}
			assetTypeValueField3.Children = list;
		}
		AssetTypeValueField assetTypeValueField5;
		if (!(assetTypeValueField5 = baseField["m_StreamData"]).IsDummy)
		{
			assetTypeValueField5["offset"].AsULong = m_StreamData.offset;
			assetTypeValueField5["size"].AsUInt = m_StreamData.size;
			assetTypeValueField5["path"].AsString = m_StreamData.path;
		}
	}

	public bool SetPictureDataFromBundle(BundleFileInstance inst)
	{
		StreamingInfo streamData = m_StreamData;
		string text = streamData.path;
		if (streamData.path.StartsWith("archive:/"))
		{
			text = text.Substring(9);
		}
		text = Path.GetFileName(text);
		AssetBundleFile file = inst.file;
		AssetsFileReader dataReader = file.DataReader;
		AssetBundleDirectoryInfo[] directoryInfos = file.BlockAndDirInfo.DirectoryInfos;
		bool result = false;
		foreach (AssetBundleDirectoryInfo assetBundleDirectoryInfo in directoryInfos)
		{
			if (assetBundleDirectoryInfo.Name == text)
			{
				dataReader.Position = assetBundleDirectoryInfo.Offset + (long)streamData.offset;
				pictureData = dataReader.ReadBytes((int)streamData.size);
				m_StreamData.offset = 0uL;
				m_StreamData.size = 0u;
				m_StreamData.path = "";
				result = true;
				break;
			}
		}
		return result;
	}

	public byte[] GetTextureData(AssetsFileInstance inst)
	{
		if (inst.parentBundle != null && m_StreamData.path != string.Empty)
		{
			SetPictureDataFromBundle(inst.parentBundle);
		}
		return GetTextureData(Path.GetDirectoryName(inst.path));
	}

	public byte[] GetTextureData(AssetsFile file)
	{
		string rootPath = null;
		if (file.Reader.BaseStream is FileStream fileStream)
		{
			rootPath = Path.GetDirectoryName(fileStream.Name);
		}
		return GetTextureData(rootPath);
	}

	public byte[] GetTextureData(string rootPath, bool useBgra = true)
	{
		if ((pictureData == null || pictureData.Length == 0) && !string.IsNullOrEmpty(m_StreamData.path) && m_StreamData.size != 0)
		{
			string text = m_StreamData.path;
			if (!Path.IsPathRooted(text) && rootPath != null)
			{
				text = Path.Combine(rootPath, text);
			}
			if (!File.Exists(text))
			{
				return null;
			}
			using Stream stream = File.OpenRead(text);
			stream.Position = (long)m_StreamData.offset;
			pictureData = new byte[m_StreamData.size];
			stream.Read(pictureData, 0, (int)m_StreamData.size);
		}
		return DecodeManaged(pictureData, (TextureFormat)m_TextureFormat, m_Width, m_Height, useBgra);
	}

	public byte[] GetTextureData(string rootPath, AssetBundleFile bundle)
	{
		if ((pictureData == null || pictureData.Length == 0) && m_StreamData.path != null && m_StreamData.path.StartsWith("archive:/") && bundle != null)
		{
			string name = m_StreamData.path.Split('/').Last();
			int fileIndex = bundle.GetFileIndex(name);
			if (fileIndex >= 0)
			{
				bundle.GetFileRange(fileIndex, out var offset, out var _);
				pictureData = new byte[m_StreamData.size];
				bundle.Reader.Position = offset + (long)m_StreamData.offset;
				bundle.Reader.Read(pictureData, 0, pictureData.Length);
			}
		}
		return GetTextureData(rootPath);
	}

	public void SetTextureData(byte[] bgra, int width, int height)
	{
		byte[] array = Encode(bgra, (TextureFormat)m_TextureFormat, width, height);
		if (array == null)
		{
			throw new NotSupportedException("The current texture format is not supported for encoding.");
		}
		SetTextureDataRaw(array, width, height);
	}

	public void SetTextureDataRaw(byte[] encodedData, int width, int height)
	{
		m_Width = width;
		m_Height = height;
		m_StreamData.path = "";
		m_StreamData.offset = 0uL;
		m_StreamData.size = 0u;
		pictureData = encodedData;
		m_CompleteImageSize = encodedData.Length;
	}

	public static byte[] DecodeManaged(byte[] data, TextureFormat format, int width, int height, bool useBgra = true)
	{
		byte[] output = Array.Empty<byte>();
		if (1 == 0)
		{
		}
		int num;
		switch (format)
		{
		case TextureFormat.Alpha8:
			num = RgbConverter.Convert<ColorA8, byte, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.ARGB4444:
			num = RgbConverter.Convert<ColorARGB16, byte, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.RGB24:
			num = RgbConverter.Convert<ColorRGB24, byte, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.RGBA32:
			num = RgbConverter.Convert<ColorRGBA32, byte, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.ARGB32:
			num = RgbConverter.Convert<ColorARGB32, byte, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.R16:
			num = RgbConverter.Convert<ColorR16, ushort, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.RGBA4444:
			num = RgbConverter.Convert<ColorRGBA16, byte, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.BGRA32:
			num = data.Length;
			break;
		case TextureFormat.RG16:
			num = RgbConverter.Convert<ColorRG16, byte, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.R8:
			num = RgbConverter.Convert<ColorR8, byte, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.RHalf:
			num = RgbConverter.Convert<ColorRHalf, Half, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.RGHalf:
			num = RgbConverter.Convert<ColorRGHalf, Half, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.RGBAHalf:
			num = RgbConverter.Convert<ColorRGBAHalf, Half, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.RFloat:
			num = RgbConverter.Convert<ColorRSingle, float, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.RGFloat:
			num = RgbConverter.Convert<ColorRGSingle, float, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.RGBAFloat:
			num = RgbConverter.Convert<ColorRGBASingle, float, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.RGB9e5Float:
			num = RgbConverter.Convert<ColorRGB9e5, double, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.RG32:
			num = RgbConverter.Convert<ColorRG32, ushort, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.RGB48:
			num = RgbConverter.Convert<ColorRGB48, ushort, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.RGBA64:
			num = RgbConverter.Convert<ColorRGBA64, ushort, ColorBGRA32, byte>(data, width, height, out output);
			break;
		case TextureFormat.DXT1:
			num = DxtDecoder.DecompressDXT1(data, width, height, out output);
			break;
		case TextureFormat.DXT3:
			num = DxtDecoder.DecompressDXT3(data, width, height, out output);
			break;
		case TextureFormat.DXT5:
			num = DxtDecoder.DecompressDXT5(data, width, height, out output);
			break;
		case TextureFormat.BC4:
			num = BcDecoder.DecompressBC4(data, width, height, out output);
			break;
		case TextureFormat.BC5:
			num = BcDecoder.DecompressBC5(data, width, height, out output);
			break;
		case TextureFormat.BC6H:
			num = BcDecoder.DecompressBC6H(data, width, height, isSigned: false, out output);
			break;
		case TextureFormat.BC7:
			num = BcDecoder.DecompressBC7(data, width, height, out output);
			break;
		case TextureFormat.ETC_RGB4:
			num = EtcDecoder.DecompressETC(data, width, height, out output);
			break;
		case TextureFormat.ETC2_RGB4:
			num = EtcDecoder.DecompressETC2(data, width, height, out output);
			break;
		case TextureFormat.ETC2_RGBA1:
			num = EtcDecoder.DecompressETC2A1(data, width, height, out output);
			break;
		case TextureFormat.ETC2_RGBA8:
			num = EtcDecoder.DecompressETC2A8(data, width, height, out output);
			break;
		case TextureFormat.EAC_R:
			num = EtcDecoder.DecompressEACRUnsigned(data, width, height, out output);
			break;
		case TextureFormat.EAC_R_SIGNED:
			num = EtcDecoder.DecompressEACRSigned(data, width, height, out output);
			break;
		case TextureFormat.EAC_RG:
			num = EtcDecoder.DecompressEACRGUnsigned(data, width, height, out output);
			break;
		case TextureFormat.EAC_RG_SIGNED:
			num = EtcDecoder.DecompressEACRGSigned(data, width, height, out output);
			break;
		case TextureFormat.ASTC_RGB_4x4:
		case TextureFormat.ASTC_RGBA_4x4:
			num = AstcDecoder.DecodeASTC(data, width, height, 4, 4, out output);
			break;
		case TextureFormat.ASTC_RGB_5x5:
		case TextureFormat.ASTC_RGBA_5x5:
			num = AstcDecoder.DecodeASTC(data, width, height, 5, 5, out output);
			break;
		case TextureFormat.ASTC_RGB_6x6:
		case TextureFormat.ASTC_RGBA_6x6:
			num = AstcDecoder.DecodeASTC(data, width, height, 6, 6, out output);
			break;
		case TextureFormat.ASTC_RGB_8x8:
		case TextureFormat.ASTC_RGBA_8x8:
			num = AstcDecoder.DecodeASTC(data, width, height, 8, 8, out output);
			break;
		case TextureFormat.ASTC_RGB_10x10:
		case TextureFormat.ASTC_RGBA_10x10:
			num = AstcDecoder.DecodeASTC(data, width, height, 10, 10, out output);
			break;
		case TextureFormat.ASTC_RGB_12x12:
		case TextureFormat.ASTC_RGBA_12x12:
			num = AstcDecoder.DecodeASTC(data, width, height, 12, 12, out output);
			break;
		case TextureFormat.ATC_RGB4:
			num = AtcDecoder.DecompressAtcRgb4(data, width, height, out output);
			break;
		case TextureFormat.ATC_RGBA8:
			num = AtcDecoder.DecompressAtcRgba8(data, width, height, out output);
			break;
		case TextureFormat.PVRTC_RGB2:
		case TextureFormat.PVRTC_RGBA2:
			num = PvrtcDecoder.DecompressPVRTC(data, width, height, do2bitMode: true, out output);
			break;
		case TextureFormat.PVRTC_RGB4:
		case TextureFormat.PVRTC_RGBA4:
			num = PvrtcDecoder.DecompressPVRTC(data, width, height, do2bitMode: false, out output);
			break;
		case TextureFormat.YUY2:
			num = Yuy2Decoder.DecompressYUY2(data, width, height, out output);
			break;
		default:
			num = 0;
			break;
		}
		if (1 == 0)
		{
		}
		if (num == 0)
		{
			return null;
		}
		return output;
	}

	public static byte[] Encode(byte[] data, TextureFormat format, int width, int height)
	{
		if (1 == 0)
		{
		}
		byte[] result = format switch
		{
			TextureFormat.R8 => RGBAEncoders.EncodeR8(data, width, height), 
			TextureFormat.R16 => RGBAEncoders.EncodeR16(data, width, height), 
			TextureFormat.RG16 => RGBAEncoders.EncodeRG16(data, width, height), 
			TextureFormat.RGB24 => RGBAEncoders.EncodeRGB24(data, width, height), 
			TextureFormat.RGB565 => RGBAEncoders.EncodeRGB565(data, width, height), 
			TextureFormat.RGBA32 => RGBAEncoders.EncodeRGBA32(data, width, height), 
			TextureFormat.ARGB32 => RGBAEncoders.EncodeARGB32(data, width, height), 
			TextureFormat.RGBA4444 => RGBAEncoders.EncodeRGBA4444(data, width, height), 
			TextureFormat.ARGB4444 => RGBAEncoders.EncodeARGB4444(data, width, height), 
			TextureFormat.Alpha8 => RGBAEncoders.EncodeAlpha8(data, width, height), 
			TextureFormat.RHalf => RGBAEncoders.EncodeRHalf(data, width, height), 
			TextureFormat.RGHalf => RGBAEncoders.EncodeRGHalf(data, width, height), 
			TextureFormat.RGBAHalf => RGBAEncoders.EncodeRGBAHalf(data, width, height), 
			_ => null, 
		};
		if (1 == 0)
		{
		}
		return result;
	}
}
