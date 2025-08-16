using System;
using Avalonia.Platform;

namespace Avalonia.Media.Imaging;

internal static class PixelFormatReader
{
	public interface IPixelFormatReader
	{
		Rgba8888Pixel ReadNext();

		void Reset(IntPtr address);
	}

	public struct BlackWhitePixelReader : IPixelFormatReader
	{
		private int _bit;

		private unsafe byte* _address;

		public unsafe void Reset(IntPtr address)
		{
			_address = (byte*)(void*)address;
			_bit = 0;
		}

		public unsafe Rgba8888Pixel ReadNext()
		{
			int num = 7 - _bit;
			int num2 = (*_address >> num) & 1;
			_bit++;
			if (_bit == 8)
			{
				_address++;
				_bit = 0;
			}
			if (num2 != 1)
			{
				return s_black;
			}
			return s_white;
		}
	}

	public struct Gray2PixelReader : IPixelFormatReader
	{
		private int _bit;

		private unsafe byte* _address;

		private static Rgba8888Pixel[] Palette = new Rgba8888Pixel[4]
		{
			s_black,
			new Rgba8888Pixel
			{
				A = byte.MaxValue,
				B = 85,
				G = 85,
				R = 85
			},
			new Rgba8888Pixel
			{
				A = byte.MaxValue,
				B = 170,
				G = 170,
				R = 170
			},
			s_white
		};

		public unsafe void Reset(IntPtr address)
		{
			_address = (byte*)(void*)address;
			_bit = 0;
		}

		public unsafe Rgba8888Pixel ReadNext()
		{
			int num = 6 - _bit;
			byte b = (byte)(*_address >> num);
			b &= 3;
			_bit += 2;
			if (_bit == 8)
			{
				_address++;
				_bit = 0;
			}
			return Palette[b];
		}
	}

	public struct Gray4PixelReader : IPixelFormatReader
	{
		private int _bit;

		private unsafe byte* _address;

		public unsafe void Reset(IntPtr address)
		{
			_address = (byte*)(void*)address;
			_bit = 0;
		}

		public unsafe Rgba8888Pixel ReadNext()
		{
			int num = 4 - _bit;
			byte b = (byte)(*_address >> num);
			b &= 0xF;
			b = (byte)(b | (b << 4));
			_bit += 4;
			if (_bit == 8)
			{
				_address++;
				_bit = 0;
			}
			Rgba8888Pixel result = default(Rgba8888Pixel);
			result.A = byte.MaxValue;
			result.B = b;
			result.G = b;
			result.R = b;
			return result;
		}
	}

	public struct Gray8PixelReader : IPixelFormatReader
	{
		private unsafe byte* _address;

		public unsafe void Reset(IntPtr address)
		{
			_address = (byte*)(void*)address;
		}

		public unsafe Rgba8888Pixel ReadNext()
		{
			byte address = *_address;
			_address++;
			Rgba8888Pixel result = default(Rgba8888Pixel);
			result.A = byte.MaxValue;
			result.B = address;
			result.G = address;
			result.R = address;
			return result;
		}
	}

	public struct Gray16PixelReader : IPixelFormatReader
	{
		private unsafe ushort* _address;

		public unsafe Rgba8888Pixel ReadNext()
		{
			ushort address = *_address;
			_address++;
			byte b = (byte)(address >> 8);
			Rgba8888Pixel result = default(Rgba8888Pixel);
			result.A = byte.MaxValue;
			result.B = b;
			result.G = b;
			result.R = b;
			return result;
		}

		public unsafe void Reset(IntPtr address)
		{
			_address = (ushort*)(void*)address;
		}
	}

	public struct Gray32FloatPixelReader : IPixelFormatReader
	{
		private unsafe byte* _address;

		public unsafe Rgba8888Pixel ReadNext()
		{
			byte b = (byte)(Math.Pow(*(float*)_address, 0.45454545454545453) * 255.0);
			_address += 4;
			Rgba8888Pixel result = default(Rgba8888Pixel);
			result.A = byte.MaxValue;
			result.B = b;
			result.G = b;
			result.R = b;
			return result;
		}

		public unsafe void Reset(IntPtr address)
		{
			_address = (byte*)(void*)address;
		}
	}

	private struct Rgba64
	{
		public ushort R;

		public ushort G;

		public ushort B;

		public ushort A;
	}

	public struct Rgba64PixelFormatReader : IPixelFormatReader
	{
		private unsafe Rgba64* _address;

		public unsafe Rgba8888Pixel ReadNext()
		{
			Rgba64 address = *_address;
			_address++;
			Rgba8888Pixel result = default(Rgba8888Pixel);
			result.A = (byte)(address.A >> 8);
			result.B = (byte)(address.B >> 8);
			result.G = (byte)(address.G >> 8);
			result.R = (byte)(address.R >> 8);
			return result;
		}

		public unsafe void Reset(IntPtr address)
		{
			_address = (Rgba64*)(void*)address;
		}
	}

	public struct Rgb24PixelFormatReader : IPixelFormatReader
	{
		private unsafe byte* _address;

		public unsafe Rgba8888Pixel ReadNext()
		{
			byte* address = _address;
			_address += 3;
			Rgba8888Pixel result = default(Rgba8888Pixel);
			result.R = *address;
			result.G = address[1];
			result.B = address[2];
			result.A = byte.MaxValue;
			return result;
		}

		public unsafe void Reset(IntPtr address)
		{
			_address = (byte*)(void*)address;
		}
	}

	public struct Bgr24PixelFormatReader : IPixelFormatReader
	{
		private unsafe byte* _address;

		public unsafe Rgba8888Pixel ReadNext()
		{
			byte* address = _address;
			_address += 3;
			Rgba8888Pixel result = default(Rgba8888Pixel);
			result.R = address[2];
			result.G = address[1];
			result.B = *address;
			result.A = byte.MaxValue;
			return result;
		}

		public unsafe void Reset(IntPtr address)
		{
			_address = (byte*)(void*)address;
		}
	}

	private static readonly Rgba8888Pixel s_white = new Rgba8888Pixel
	{
		A = byte.MaxValue,
		B = byte.MaxValue,
		G = byte.MaxValue,
		R = byte.MaxValue
	};

	private static readonly Rgba8888Pixel s_black = new Rgba8888Pixel
	{
		A = byte.MaxValue,
		B = 0,
		G = 0,
		R = 0
	};

	public static void Transcode(IntPtr dst, IntPtr src, PixelSize size, int strideSrc, int strideDst, PixelFormat format)
	{
		if (format == PixelFormats.BlackWhite)
		{
			Transcode<BlackWhitePixelReader>(dst, src, size, strideSrc, strideDst);
			return;
		}
		if (format == PixelFormats.Gray2)
		{
			Transcode<Gray2PixelReader>(dst, src, size, strideSrc, strideDst);
			return;
		}
		if (format == PixelFormats.Gray4)
		{
			Transcode<Gray4PixelReader>(dst, src, size, strideSrc, strideDst);
			return;
		}
		if (format == PixelFormats.Gray8)
		{
			Transcode<Gray8PixelReader>(dst, src, size, strideSrc, strideDst);
			return;
		}
		if (format == PixelFormats.Gray16)
		{
			Transcode<Gray16PixelReader>(dst, src, size, strideSrc, strideDst);
			return;
		}
		if (format == PixelFormats.Rgb24)
		{
			Transcode<Rgb24PixelFormatReader>(dst, src, size, strideSrc, strideDst);
			return;
		}
		if (format == PixelFormats.Bgr24)
		{
			Transcode<Bgr24PixelFormatReader>(dst, src, size, strideSrc, strideDst);
			return;
		}
		if (format == PixelFormats.Gray32Float)
		{
			Transcode<Gray32FloatPixelReader>(dst, src, size, strideSrc, strideDst);
			return;
		}
		if (format == PixelFormats.Rgba64)
		{
			Transcode<Rgba64PixelFormatReader>(dst, src, size, strideSrc, strideDst);
			return;
		}
		throw new NotSupportedException($"Pixel format {format} is not supported");
	}

	public static bool SupportsFormat(PixelFormat format)
	{
		if (!(format == PixelFormats.BlackWhite) && !(format == PixelFormats.Gray2) && !(format == PixelFormats.Gray4) && !(format == PixelFormats.Gray8) && !(format == PixelFormats.Gray16) && !(format == PixelFormats.Gray32Float) && !(format == PixelFormats.Rgba64) && !(format == PixelFormats.Bgr24))
		{
			return format == PixelFormats.Rgb24;
		}
		return true;
	}

	public unsafe static void Transcode<TReader>(IntPtr dst, IntPtr src, PixelSize size, int strideSrc, int strideDst) where TReader : struct, IPixelFormatReader
	{
		int width = size.Width;
		int height = size.Height;
		TReader val = default(TReader);
		for (int i = 0; i < height; i++)
		{
			val.Reset(src + strideSrc * i);
			Rgba8888Pixel* ptr = (Rgba8888Pixel*)(void*)(dst + strideDst * i);
			for (int j = 0; j < width; j++)
			{
				*ptr = val.ReadNext();
				ptr++;
			}
		}
	}
}
