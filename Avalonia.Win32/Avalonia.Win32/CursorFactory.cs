using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Avalonia.Input;
using Avalonia.Platform;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32;

internal class CursorFactory : ICursorFactory
{
	private static readonly Dictionary<StandardCursorType, int> CursorTypeMapping;

	private static readonly Dictionary<StandardCursorType, CursorImpl> Cache;

	public static CursorFactory Instance { get; }

	private CursorFactory()
	{
	}

	static CursorFactory()
	{
		Instance = new CursorFactory();
		CursorTypeMapping = new Dictionary<StandardCursorType, int>
		{
			{
				StandardCursorType.None,
				0
			},
			{
				StandardCursorType.AppStarting,
				32650
			},
			{
				StandardCursorType.Arrow,
				32512
			},
			{
				StandardCursorType.Cross,
				32515
			},
			{
				StandardCursorType.Hand,
				32649
			},
			{
				StandardCursorType.Help,
				32651
			},
			{
				StandardCursorType.Ibeam,
				32513
			},
			{
				StandardCursorType.No,
				32648
			},
			{
				StandardCursorType.SizeAll,
				32646
			},
			{
				StandardCursorType.UpArrow,
				32516
			},
			{
				StandardCursorType.SizeNorthSouth,
				32645
			},
			{
				StandardCursorType.SizeWestEast,
				32644
			},
			{
				StandardCursorType.Wait,
				32514
			},
			{
				StandardCursorType.TopSide,
				32645
			},
			{
				StandardCursorType.BottomSide,
				32645
			},
			{
				StandardCursorType.LeftSide,
				32644
			},
			{
				StandardCursorType.RightSide,
				32644
			},
			{
				StandardCursorType.TopLeftCorner,
				32642
			},
			{
				StandardCursorType.BottomRightCorner,
				32642
			},
			{
				StandardCursorType.TopRightCorner,
				32643
			},
			{
				StandardCursorType.BottomLeftCorner,
				32643
			},
			{
				StandardCursorType.DragMove,
				32516
			},
			{
				StandardCursorType.DragCopy,
				32516
			},
			{
				StandardCursorType.DragLink,
				32516
			}
		};
		Cache = new Dictionary<StandardCursorType, CursorImpl>();
		LoadModuleCursor(StandardCursorType.DragMove, "ole32.dll", 2);
		LoadModuleCursor(StandardCursorType.DragCopy, "ole32.dll", 3);
		LoadModuleCursor(StandardCursorType.DragLink, "ole32.dll", 4);
	}

	private static void LoadModuleCursor(StandardCursorType cursorType, string module, int id)
	{
		IntPtr moduleHandle = UnmanagedMethods.GetModuleHandle(module);
		if (moduleHandle != IntPtr.Zero)
		{
			IntPtr intPtr = UnmanagedMethods.LoadCursor(moduleHandle, new IntPtr(id));
			if (intPtr != IntPtr.Zero)
			{
				Cache.Add(cursorType, new CursorImpl(intPtr, isCustom: false));
			}
		}
	}

	public ICursorImpl GetCursor(StandardCursorType cursorType)
	{
		if (!Cache.TryGetValue(cursorType, out CursorImpl value))
		{
			value = new CursorImpl(UnmanagedMethods.LoadCursor(IntPtr.Zero, new IntPtr(CursorTypeMapping[cursorType])), isCustom: false);
			Cache.Add(cursorType, value);
		}
		return value;
	}

	public ICursorImpl CreateCursor(IBitmapImpl cursor, PixelPoint hotSpot)
	{
		using Bitmap bitmap = LoadSystemDrawingBitmap(cursor);
		using Bitmap bitmap2 = AlphaToMask(bitmap);
		UnmanagedMethods.ICONINFO iCONINFO = default(UnmanagedMethods.ICONINFO);
		iCONINFO.IsIcon = false;
		iCONINFO.xHotspot = hotSpot.X;
		iCONINFO.yHotspot = hotSpot.Y;
		iCONINFO.MaskBitmap = bitmap2.GetHbitmap();
		iCONINFO.ColorBitmap = bitmap.GetHbitmap();
		UnmanagedMethods.ICONINFO iconInfo = iCONINFO;
		return new CursorImpl(UnmanagedMethods.CreateIconIndirect(ref iconInfo), isCustom: true);
	}

	private static Bitmap LoadSystemDrawingBitmap(IBitmapImpl bitmap)
	{
		using MemoryStream stream = new MemoryStream();
		bitmap.Save(stream, null);
		return new Bitmap(stream);
	}

	private unsafe Bitmap AlphaToMask(Bitmap source)
	{
		Bitmap bitmap = new Bitmap(source.Width, source.Height, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
		if (source.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppPArgb)
		{
			throw new NotSupportedException("Images with premultiplied alpha not yet supported as cursor images.");
		}
		if (source.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
		{
			return bitmap;
		}
		BitmapData bitmapData = source.LockBits(new Rectangle(default(System.Drawing.Point), source.Size), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		BitmapData bitmapData2 = bitmap.LockBits(new Rectangle(default(System.Drawing.Point), source.Size), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
		try
		{
			byte* ptr = (byte*)bitmapData.Scan0.ToPointer();
			byte* ptr2 = (byte*)bitmapData2.Scan0.ToPointer();
			for (int i = 0; i < bitmap.Height; i++)
			{
				for (int j = 0; j < bitmap.Width; j++)
				{
					if (ptr[j * 4] == 0)
					{
						byte* num = ptr2 + j / 8;
						*num |= (byte)(1 << j % 8);
					}
				}
				ptr += bitmapData.Stride;
				ptr2 += bitmapData2.Stride;
			}
			return bitmap;
		}
		finally
		{
			source.UnlockBits(bitmapData);
			bitmap.UnlockBits(bitmapData2);
		}
	}
}
