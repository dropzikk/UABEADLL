using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Input;
using Avalonia.Platform;
using Avalonia.Platform.Internal;
using Avalonia.SourceGenerator;

namespace Avalonia.X11;

internal class X11CursorFactory : ICursorFactory
{
	private class XImageCursor : CursorImpl, IFramebufferPlatformSurface, IPlatformHandle
	{
		private readonly PixelSize _pixelSize;

		private readonly UnmanagedBlob _blob;

		public string HandleDescriptor => "XCURSOR";

		public unsafe XImageCursor(IntPtr display, IBitmapImpl bitmap, PixelPoint hotSpot)
		{
			int size = Marshal.SizeOf<XcursorImage>() + bitmap.PixelSize.Width * bitmap.PixelSize.Height * 4;
			IPlatformRenderInterface requiredService = AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>();
			_pixelSize = bitmap.PixelSize;
			_blob = new UnmanagedBlob(size);
			XcursorImage* ptr = (XcursorImage*)(void*)_blob.Address;
			ptr->version = 1;
			ptr->size = Marshal.SizeOf<XcursorImage>();
			ptr->width = bitmap.PixelSize.Width;
			ptr->height = bitmap.PixelSize.Height;
			ptr->xhot = hotSpot.X;
			ptr->yhot = hotSpot.Y;
			ptr->pixels = (IntPtr)(ptr + 1);
			using (IPlatformRenderInterfaceContext platformRenderInterfaceContext = requiredService.CreateBackendContext(null))
			{
				using IRenderTarget renderTarget = platformRenderInterfaceContext.CreateRenderTarget(new XImageCursor[1] { this });
				using IDrawingContextImpl drawingContextImpl = renderTarget.CreateDrawingContext();
				Rect rect = new Rect(_pixelSize.ToSize(1.0));
				drawingContextImpl.DrawBitmap(bitmap, 1.0, rect, rect);
			}
			base.Handle = XLib.XcursorImageLoadCursor(display, _blob.Address);
		}

		public override void Dispose()
		{
			XLib.XcursorImageDestroy(base.Handle);
			_blob.Dispose();
		}

		public ILockedFramebuffer Lock()
		{
			return new LockedFramebuffer(_blob.Address + Marshal.SizeOf<XcursorImage>(), _pixelSize, _pixelSize.Width * 4, new Vector(96.0, 96.0), PixelFormat.Bgra8888, null);
		}

		public IFramebufferRenderTarget CreateFramebufferRenderTarget()
		{
			return new FuncFramebufferRenderTarget(Lock);
		}
	}

	private static readonly byte[] NullCursorData = new byte[1];

	private static IntPtr _nullCursor;

	private readonly IntPtr _display;

	private Dictionary<CursorFontShape, IntPtr> _cursors;

	private static readonly Dictionary<StandardCursorType, CursorFontShape> s_mapping = new Dictionary<StandardCursorType, CursorFontShape>
	{
		{
			StandardCursorType.Arrow,
			CursorFontShape.XC_left_ptr
		},
		{
			StandardCursorType.Cross,
			CursorFontShape.XC_cross
		},
		{
			StandardCursorType.Hand,
			CursorFontShape.XC_hand2
		},
		{
			StandardCursorType.Help,
			CursorFontShape.XC_question_arrow
		},
		{
			StandardCursorType.Ibeam,
			CursorFontShape.XC_xterm
		},
		{
			StandardCursorType.No,
			CursorFontShape.XC_X_cursor
		},
		{
			StandardCursorType.Wait,
			CursorFontShape.XC_watch
		},
		{
			StandardCursorType.AppStarting,
			CursorFontShape.XC_watch
		},
		{
			StandardCursorType.BottomSide,
			CursorFontShape.XC_bottom_side
		},
		{
			StandardCursorType.DragCopy,
			CursorFontShape.XC_center_ptr
		},
		{
			StandardCursorType.DragLink,
			CursorFontShape.XC_fleur
		},
		{
			StandardCursorType.DragMove,
			CursorFontShape.XC_diamond_cross
		},
		{
			StandardCursorType.LeftSide,
			CursorFontShape.XC_left_side
		},
		{
			StandardCursorType.RightSide,
			CursorFontShape.XC_right_side
		},
		{
			StandardCursorType.SizeAll,
			CursorFontShape.XC_sizing
		},
		{
			StandardCursorType.TopSide,
			CursorFontShape.XC_top_side
		},
		{
			StandardCursorType.UpArrow,
			CursorFontShape.XC_sb_up_arrow
		},
		{
			StandardCursorType.BottomLeftCorner,
			CursorFontShape.XC_bottom_left_corner
		},
		{
			StandardCursorType.BottomRightCorner,
			CursorFontShape.XC_bottom_right_corner
		},
		{
			StandardCursorType.SizeNorthSouth,
			CursorFontShape.XC_sb_v_double_arrow
		},
		{
			StandardCursorType.SizeWestEast,
			CursorFontShape.XC_sb_h_double_arrow
		},
		{
			StandardCursorType.TopLeftCorner,
			CursorFontShape.XC_top_left_corner
		},
		{
			StandardCursorType.TopRightCorner,
			CursorFontShape.XC_top_right_corner
		}
	};

	[GenerateEnumValueList]
	private static CursorFontShape[] GetAllCursorShapes()
	{
		return new CursorFontShape[78]
		{
			CursorFontShape.XC_X_cursor,
			CursorFontShape.XC_arrow,
			CursorFontShape.XC_based_arrow_down,
			CursorFontShape.XC_based_arrow_up,
			CursorFontShape.XC_boat,
			CursorFontShape.XC_bogosity,
			CursorFontShape.XC_bottom_left_corner,
			CursorFontShape.XC_bottom_right_corner,
			CursorFontShape.XC_bottom_side,
			CursorFontShape.XC_bottom_tee,
			CursorFontShape.XC_box_spiral,
			CursorFontShape.XC_center_ptr,
			CursorFontShape.XC_circle,
			CursorFontShape.XC_clock,
			CursorFontShape.XC_coffee_mug,
			CursorFontShape.XC_cross,
			CursorFontShape.XC_cross_reverse,
			CursorFontShape.XC_crosshair,
			CursorFontShape.XC_diamond_cross,
			CursorFontShape.XC_dot,
			CursorFontShape.XC_dotbox,
			CursorFontShape.XC_double_arrow,
			CursorFontShape.XC_draft_large,
			CursorFontShape.XC_draft_small,
			CursorFontShape.XC_draped_box,
			CursorFontShape.XC_exchange,
			CursorFontShape.XC_fleur,
			CursorFontShape.XC_gobbler,
			CursorFontShape.XC_gumby,
			CursorFontShape.XC_hand1,
			CursorFontShape.XC_hand2,
			CursorFontShape.XC_heart,
			CursorFontShape.XC_icon,
			CursorFontShape.XC_iron_cross,
			CursorFontShape.XC_left_ptr,
			CursorFontShape.XC_left_side,
			CursorFontShape.XC_left_tee,
			CursorFontShape.XC_left_button,
			CursorFontShape.XC_ll_angle,
			CursorFontShape.XC_lr_angle,
			CursorFontShape.XC_man,
			CursorFontShape.XC_middlebutton,
			CursorFontShape.XC_mouse,
			CursorFontShape.XC_pencil,
			CursorFontShape.XC_pirate,
			CursorFontShape.XC_plus,
			CursorFontShape.XC_question_arrow,
			CursorFontShape.XC_right_ptr,
			CursorFontShape.XC_right_side,
			CursorFontShape.XC_right_tee,
			CursorFontShape.XC_rightbutton,
			CursorFontShape.XC_rtl_logo,
			CursorFontShape.XC_sailboat,
			CursorFontShape.XC_sb_down_arrow,
			CursorFontShape.XC_sb_h_double_arrow,
			CursorFontShape.XC_sb_left_arrow,
			CursorFontShape.XC_sb_right_arrow,
			CursorFontShape.XC_sb_up_arrow,
			CursorFontShape.XC_sb_v_double_arrow,
			CursorFontShape.XC_sb_shuttle,
			CursorFontShape.XC_sizing,
			CursorFontShape.XC_spider,
			CursorFontShape.XC_spraycan,
			CursorFontShape.XC_star,
			CursorFontShape.XC_target,
			CursorFontShape.XC_tcross,
			CursorFontShape.XC_top_left_arrow,
			CursorFontShape.XC_top_left_corner,
			CursorFontShape.XC_top_right_corner,
			CursorFontShape.XC_top_side,
			CursorFontShape.XC_top_tee,
			CursorFontShape.XC_trek,
			CursorFontShape.XC_ul_angle,
			CursorFontShape.XC_umbrella,
			CursorFontShape.XC_ur_angle,
			CursorFontShape.XC_watch,
			CursorFontShape.XC_xterm,
			CursorFontShape.XC_num_glyphs
		};
	}

	public X11CursorFactory(IntPtr display)
	{
		_display = display;
		_nullCursor = GetNullCursor(display);
		_cursors = GetAllCursorShapes().ToDictionary((CursorFontShape id) => id, (CursorFontShape id) => XLib.XCreateFontCursor(_display, id));
	}

	public ICursorImpl GetCursor(StandardCursorType cursorType)
	{
		CursorFontShape value;
		IntPtr handle = ((cursorType != StandardCursorType.None) ? (s_mapping.TryGetValue(cursorType, out value) ? _cursors[value] : _cursors[CursorFontShape.XC_left_ptr]) : _nullCursor);
		return new CursorImpl(handle);
	}

	public ICursorImpl CreateCursor(IBitmapImpl cursor, PixelPoint hotSpot)
	{
		return new XImageCursor(_display, cursor, hotSpot);
	}

	private static IntPtr GetNullCursor(IntPtr display)
	{
		XColor foreground_color = default(XColor);
		IntPtr drawable = XLib.XRootWindow(display, 0);
		IntPtr intPtr = XLib.XCreateBitmapFromData(display, drawable, NullCursorData, 1, 1);
		return XLib.XCreatePixmapCursor(display, intPtr, intPtr, ref foreground_color, ref foreground_color, 0, 0);
	}
}
