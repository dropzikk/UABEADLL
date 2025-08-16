using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using MicroCom.Runtime;

namespace Avalonia.Win32.Interop;

internal static class UnmanagedMethods
{
	public delegate void TimerProc(IntPtr hWnd, uint uMsg, IntPtr nIDEvent, uint dwTime);

	public delegate void TimeCallback(uint uTimerID, uint uMsg, UIntPtr dwUser, UIntPtr dw1, UIntPtr dw2);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void WaitOrTimerCallback(IntPtr lpParameter, bool timerOrWaitFired);

	public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

	public enum Cursor
	{
		IDC_ARROW = 32512,
		IDC_IBEAM = 32513,
		IDC_WAIT = 32514,
		IDC_CROSS = 32515,
		IDC_UPARROW = 32516,
		IDC_SIZE = 32640,
		IDC_ICON = 32641,
		IDC_SIZENWSE = 32642,
		IDC_SIZENESW = 32643,
		IDC_SIZEWE = 32644,
		IDC_SIZENS = 32645,
		IDC_SIZEALL = 32646,
		IDC_NO = 32648,
		IDC_HAND = 32649,
		IDC_APPSTARTING = 32650,
		IDC_HELP = 32651
	}

	public enum MouseActivate
	{
		MA_ACTIVATE = 1,
		MA_ACTIVATEANDEAT,
		MA_NOACTIVATE,
		MA_NOACTIVATEANDEAT
	}

	[Flags]
	public enum SetWindowPosFlags : uint
	{
		SWP_ASYNCWINDOWPOS = 0x4000u,
		SWP_DEFERERASE = 0x2000u,
		SWP_DRAWFRAME = 0x20u,
		SWP_FRAMECHANGED = 0x20u,
		SWP_HIDEWINDOW = 0x80u,
		SWP_NOACTIVATE = 0x10u,
		SWP_NOCOPYBITS = 0x100u,
		SWP_NOMOVE = 2u,
		SWP_NOOWNERZORDER = 0x200u,
		SWP_NOREDRAW = 8u,
		SWP_NOREPOSITION = 0x200u,
		SWP_NOSENDCHANGING = 0x400u,
		SWP_NOSIZE = 1u,
		SWP_NOZORDER = 4u,
		SWP_SHOWWINDOW = 0x40u,
		SWP_RESIZE = 0x16u
	}

	public static class WindowPosZOrder
	{
		public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

		public static readonly IntPtr HWND_TOP = new IntPtr(0);

		public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

		public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
	}

	public enum SizeCommand
	{
		Restored,
		Minimized,
		Maximized,
		MaxShow,
		MaxHide
	}

	public enum ShowWindowCommand
	{
		Hide = 0,
		Normal = 1,
		ShowMinimized = 2,
		Maximize = 3,
		ShowMaximized = 3,
		ShowNoActivate = 4,
		Show = 5,
		Minimize = 6,
		ShowMinNoActive = 7,
		ShowNA = 8,
		Restore = 9,
		ShowDefault = 10,
		ForceMinimize = 11
	}

	public enum SystemMetric
	{
		SM_CXSCREEN = 0,
		SM_CYSCREEN = 1,
		SM_CXVSCROLL = 2,
		SM_CYHSCROLL = 3,
		SM_CYCAPTION = 4,
		SM_CXBORDER = 5,
		SM_CYBORDER = 6,
		SM_CXDLGFRAME = 7,
		SM_CXFIXEDFRAME = 7,
		SM_CYDLGFRAME = 8,
		SM_CYFIXEDFRAME = 8,
		SM_CYVTHUMB = 9,
		SM_CXHTHUMB = 10,
		SM_CXICON = 11,
		SM_CYICON = 12,
		SM_CXCURSOR = 13,
		SM_CYCURSOR = 14,
		SM_CYMENU = 15,
		SM_CXFULLSCREEN = 16,
		SM_CYFULLSCREEN = 17,
		SM_CYKANJIWINDOW = 18,
		SM_MOUSEPRESENT = 19,
		SM_CYVSCROLL = 20,
		SM_CXHSCROLL = 21,
		SM_DEBUG = 22,
		SM_SWAPBUTTON = 23,
		SM_CXMIN = 28,
		SM_CYMIN = 29,
		SM_CXSIZE = 30,
		SM_CYSIZE = 31,
		SM_CXSIZEFRAME = 32,
		SM_CXFRAME = 32,
		SM_CYSIZEFRAME = 33,
		SM_CYFRAME = 33,
		SM_CXMINTRACK = 34,
		SM_CYMINTRACK = 35,
		SM_CXDOUBLECLK = 36,
		SM_CYDOUBLECLK = 37,
		SM_CXICONSPACING = 38,
		SM_CYICONSPACING = 39,
		SM_MENUDROPALIGNMENT = 40,
		SM_PENWINDOWS = 41,
		SM_DBCSENABLED = 42,
		SM_CMOUSEBUTTONS = 43,
		SM_SECURE = 44,
		SM_CXEDGE = 45,
		SM_CYEDGE = 46,
		SM_CXMINSPACING = 47,
		SM_CYMINSPACING = 48,
		SM_CXSMICON = 49,
		SM_CYSMICON = 50,
		SM_CYSMCAPTION = 51,
		SM_CXSMSIZE = 52,
		SM_CYSMSIZE = 53,
		SM_CXMENUSIZE = 54,
		SM_CYMENUSIZE = 55,
		SM_ARRANGE = 56,
		SM_CXMINIMIZED = 57,
		SM_CYMINIMIZED = 58,
		SM_CXMAXTRACK = 59,
		SM_CYMAXTRACK = 60,
		SM_CXMAXIMIZED = 61,
		SM_CYMAXIMIZED = 62,
		SM_NETWORK = 63,
		SM_CLEANBOOT = 67,
		SM_CXDRAG = 68,
		SM_CYDRAG = 69,
		SM_SHOWSOUNDS = 70,
		SM_CXMENUCHECK = 71,
		SM_CYMENUCHECK = 72,
		SM_SLOWMACHINE = 73,
		SM_MIDEASTENABLED = 74,
		SM_MOUSEWHEELPRESENT = 75,
		SM_XVIRTUALSCREEN = 76,
		SM_YVIRTUALSCREEN = 77,
		SM_CXVIRTUALSCREEN = 78,
		SM_CYVIRTUALSCREEN = 79,
		SM_CMONITORS = 80,
		SM_SAMEDISPLAYFORMAT = 81,
		SM_IMMENABLED = 82,
		SM_CXFOCUSBORDER = 83,
		SM_CYFOCUSBORDER = 84,
		SM_TABLETPC = 86,
		SM_MEDIACENTER = 87,
		SM_STARTER = 88,
		SM_SERVERR2 = 89,
		SM_MOUSEHORIZONTALWHEELPRESENT = 91,
		SM_CXPADDEDBORDER = 92,
		SM_DIGITIZER = 94,
		SM_MAXIMUMTOUCHES = 95,
		SM_REMOTESESSION = 4096,
		SM_SHUTTINGDOWN = 8192,
		SM_REMOTECONTROL = 8193,
		SM_CONVERTABLESLATEMODE = 8195,
		SM_SYSTEMDOCKED = 8196
	}

	[Flags]
	public enum ModifierKeys
	{
		MK_NONE = 0,
		MK_LBUTTON = 1,
		MK_RBUTTON = 2,
		MK_SHIFT = 4,
		MK_CONTROL = 8,
		MK_MBUTTON = 0x10,
		MK_ALT = 0x20,
		MK_XBUTTON1 = 0x20,
		MK_XBUTTON2 = 0x40
	}

	public enum VirtualKeyStates
	{
		VK_LBUTTON = 1,
		VK_RBUTTON = 2,
		VK_CANCEL = 3,
		VK_MBUTTON = 4,
		VK_XBUTTON1 = 5,
		VK_XBUTTON2 = 6,
		VK_BACK = 8,
		VK_TAB = 9,
		VK_CLEAR = 12,
		VK_RETURN = 13,
		VK_SHIFT = 16,
		VK_CONTROL = 17,
		VK_MENU = 18,
		VK_PAUSE = 19,
		VK_CAPITAL = 20,
		VK_KANA = 21,
		VK_HANGEUL = 21,
		VK_HANGUL = 21,
		VK_JUNJA = 23,
		VK_FINAL = 24,
		VK_HANJA = 25,
		VK_KANJI = 25,
		VK_ESCAPE = 27,
		VK_CONVERT = 28,
		VK_NONCONVERT = 29,
		VK_ACCEPT = 30,
		VK_MODECHANGE = 31,
		VK_SPACE = 32,
		VK_PRIOR = 33,
		VK_NEXT = 34,
		VK_END = 35,
		VK_HOME = 36,
		VK_LEFT = 37,
		VK_UP = 38,
		VK_RIGHT = 39,
		VK_DOWN = 40,
		VK_SELECT = 41,
		VK_PRINT = 42,
		VK_EXECUTE = 43,
		VK_SNAPSHOT = 44,
		VK_INSERT = 45,
		VK_DELETE = 46,
		VK_HELP = 47,
		VK_LWIN = 91,
		VK_RWIN = 92,
		VK_APPS = 93,
		VK_SLEEP = 95,
		VK_NUMPAD0 = 96,
		VK_NUMPAD1 = 97,
		VK_NUMPAD2 = 98,
		VK_NUMPAD3 = 99,
		VK_NUMPAD4 = 100,
		VK_NUMPAD5 = 101,
		VK_NUMPAD6 = 102,
		VK_NUMPAD7 = 103,
		VK_NUMPAD8 = 104,
		VK_NUMPAD9 = 105,
		VK_MULTIPLY = 106,
		VK_ADD = 107,
		VK_SEPARATOR = 108,
		VK_SUBTRACT = 109,
		VK_DECIMAL = 110,
		VK_DIVIDE = 111,
		VK_F1 = 112,
		VK_F2 = 113,
		VK_F3 = 114,
		VK_F4 = 115,
		VK_F5 = 116,
		VK_F6 = 117,
		VK_F7 = 118,
		VK_F8 = 119,
		VK_F9 = 120,
		VK_F10 = 121,
		VK_F11 = 122,
		VK_F12 = 123,
		VK_F13 = 124,
		VK_F14 = 125,
		VK_F15 = 126,
		VK_F16 = 127,
		VK_F17 = 128,
		VK_F18 = 129,
		VK_F19 = 130,
		VK_F20 = 131,
		VK_F21 = 132,
		VK_F22 = 133,
		VK_F23 = 134,
		VK_F24 = 135,
		VK_NUMLOCK = 144,
		VK_SCROLL = 145,
		VK_OEM_NEC_EQUAL = 146,
		VK_OEM_FJ_JISHO = 146,
		VK_OEM_FJ_MASSHOU = 147,
		VK_OEM_FJ_TOUROKU = 148,
		VK_OEM_FJ_LOYA = 149,
		VK_OEM_FJ_ROYA = 150,
		VK_LSHIFT = 160,
		VK_RSHIFT = 161,
		VK_LCONTROL = 162,
		VK_RCONTROL = 163,
		VK_LMENU = 164,
		VK_RMENU = 165,
		VK_BROWSER_BACK = 166,
		VK_BROWSER_FORWARD = 167,
		VK_BROWSER_REFRESH = 168,
		VK_BROWSER_STOP = 169,
		VK_BROWSER_SEARCH = 170,
		VK_BROWSER_FAVORITES = 171,
		VK_BROWSER_HOME = 172,
		VK_VOLUME_MUTE = 173,
		VK_VOLUME_DOWN = 174,
		VK_VOLUME_UP = 175,
		VK_MEDIA_NEXT_TRACK = 176,
		VK_MEDIA_PREV_TRACK = 177,
		VK_MEDIA_STOP = 178,
		VK_MEDIA_PLAY_PAUSE = 179,
		VK_LAUNCH_MAIL = 180,
		VK_LAUNCH_MEDIA_SELECT = 181,
		VK_LAUNCH_APP1 = 182,
		VK_LAUNCH_APP2 = 183,
		VK_OEM_1 = 186,
		VK_OEM_PLUS = 187,
		VK_OEM_COMMA = 188,
		VK_OEM_MINUS = 189,
		VK_OEM_PERIOD = 190,
		VK_OEM_2 = 191,
		VK_OEM_3 = 192,
		VK_OEM_4 = 219,
		VK_OEM_5 = 220,
		VK_OEM_6 = 221,
		VK_OEM_7 = 222,
		VK_OEM_8 = 223,
		VK_OEM_AX = 225,
		VK_OEM_102 = 226,
		VK_ICO_HELP = 227,
		VK_ICO_00 = 228,
		VK_PROCESSKEY = 229,
		VK_ICO_CLEAR = 230,
		VK_PACKET = 231,
		VK_OEM_RESET = 233,
		VK_OEM_JUMP = 234,
		VK_OEM_PA1 = 235,
		VK_OEM_PA2 = 236,
		VK_OEM_PA3 = 237,
		VK_OEM_WSCTRL = 238,
		VK_OEM_CUSEL = 239,
		VK_OEM_ATTN = 240,
		VK_OEM_FINISH = 241,
		VK_OEM_COPY = 242,
		VK_OEM_AUTO = 243,
		VK_OEM_ENLW = 244,
		VK_OEM_BACKTAB = 245,
		VK_ATTN = 246,
		VK_CRSEL = 247,
		VK_EXSEL = 248,
		VK_EREOF = 249,
		VK_PLAY = 250,
		VK_ZOOM = 251,
		VK_NONAME = 252,
		VK_PA1 = 253,
		VK_OEM_CLEAR = 254
	}

	public enum WindowActivate
	{
		WA_INACTIVE,
		WA_ACTIVE,
		WA_CLICKACTIVE
	}

	public enum HitTestValues
	{
		HTERROR = -2,
		HTTRANSPARENT,
		HTNOWHERE,
		HTCLIENT,
		HTCAPTION,
		HTSYSMENU,
		HTGROWBOX,
		HTMENU,
		HTHSCROLL,
		HTVSCROLL,
		HTMINBUTTON,
		HTMAXBUTTON,
		HTLEFT,
		HTRIGHT,
		HTTOP,
		HTTOPLEFT,
		HTTOPRIGHT,
		HTBOTTOM,
		HTBOTTOMLEFT,
		HTBOTTOMRIGHT,
		HTBORDER,
		HTOBJECT,
		HTCLOSE,
		HTHELP
	}

	[Flags]
	public enum WindowStyles : uint
	{
		WS_BORDER = 0x800000u,
		WS_CAPTION = 0xC00000u,
		WS_CHILD = 0x40000000u,
		WS_CLIPCHILDREN = 0x2000000u,
		WS_CLIPSIBLINGS = 0x4000000u,
		WS_DISABLED = 0x8000000u,
		WS_DLGFRAME = 0x400000u,
		WS_GROUP = 0x20000u,
		WS_HSCROLL = 0x100000u,
		WS_MAXIMIZE = 0x1000000u,
		WS_MAXIMIZEBOX = 0x10000u,
		WS_MINIMIZE = 0x20000000u,
		WS_MINIMIZEBOX = 0x20000u,
		WS_OVERLAPPED = 0u,
		WS_OVERLAPPEDWINDOW = 0xCF0000u,
		WS_POPUP = 0x80000000u,
		WS_POPUPWINDOW = 0x80880000u,
		WS_SIZEFRAME = 0x40000u,
		WS_SYSMENU = 0x80000u,
		WS_TABSTOP = 0x10000u,
		WS_THICKFRAME = 0x40000u,
		WS_VISIBLE = 0x10000000u,
		WS_VSCROLL = 0x200000u,
		WS_EX_DLGMODALFRAME = 1u,
		WS_EX_NOPARENTNOTIFY = 4u,
		WS_EX_NOREDIRECTIONBITMAP = 0x200000u,
		WS_EX_TOPMOST = 8u,
		WS_EX_ACCEPTFILES = 0x10u,
		WS_EX_TRANSPARENT = 0x20u,
		WS_EX_MDICHILD = 0x40u,
		WS_EX_TOOLWINDOW = 0x80u,
		WS_EX_WINDOWEDGE = 0x100u,
		WS_EX_CLIENTEDGE = 0x200u,
		WS_EX_CONTEXTHELP = 0x400u,
		WS_EX_RIGHT = 0x1000u,
		WS_EX_LEFT = 0u,
		WS_EX_RTLREADING = 0x2000u,
		WS_EX_LTRREADING = 0u,
		WS_EX_LEFTSCROLLBAR = 0x4000u,
		WS_EX_RIGHTSCROLLBAR = 0u,
		WS_EX_CONTROLPARENT = 0x10000u,
		WS_EX_STATICEDGE = 0x20000u,
		WS_EX_APPWINDOW = 0x40000u,
		WS_EX_OVERLAPPEDWINDOW = 0x300u,
		WS_EX_PALETTEWINDOW = 0x188u,
		WS_EX_LAYERED = 0x80000u,
		WS_EX_NOINHERITLAYOUT = 0x100000u,
		WS_EX_LAYOUTRTL = 0x400000u,
		WS_EX_COMPOSITED = 0x2000000u,
		WS_EX_NOACTIVATE = 0x8000000u
	}

	[Flags]
	public enum ClassStyles : uint
	{
		CS_VREDRAW = 1u,
		CS_HREDRAW = 2u,
		CS_DBLCLKS = 8u,
		CS_OWNDC = 0x20u,
		CS_CLASSDC = 0x40u,
		CS_PARENTDC = 0x80u,
		CS_NOCLOSE = 0x200u,
		CS_SAVEBITS = 0x800u,
		CS_BYTEALIGNCLIENT = 0x1000u,
		CS_BYTEALIGNWINDOW = 0x2000u,
		CS_GLOBALCLASS = 0x4000u,
		CS_IME = 0x10000u,
		CS_DROPSHADOW = 0x20000u
	}

	[Flags]
	public enum PointerDeviceChangeFlags
	{
		PDC_ARRIVAL = 1,
		PDC_REMOVAL = 2,
		PDC_ORIENTATION_0 = 4,
		PDC_ORIENTATION_90 = 8,
		PDC_ORIENTATION_180 = 0x10,
		PDC_ORIENTATION_270 = 0x20,
		PDC_MODE_DEFAULT = 0x40,
		PDC_MODE_CENTERED = 0x80,
		PDC_MAPPING_CHANGE = 0x100,
		PDC_RESOLUTION = 0x200,
		PDC_ORIGIN = 0x400,
		PDC_MODE_ASPECTRATIOPRESERVED = 0x800
	}

	public enum PointerInputType
	{
		PT_NONE,
		PT_POINTER,
		PT_TOUCH,
		PT_PEN,
		PT_MOUSE,
		PT_TOUCHPAD
	}

	public enum WindowsMessage : uint
	{
		WM_NULL = 0u,
		WM_CREATE = 1u,
		WM_DESTROY = 2u,
		WM_MOVE = 3u,
		WM_SIZE = 5u,
		WM_ACTIVATE = 6u,
		WM_SETFOCUS = 7u,
		WM_KILLFOCUS = 8u,
		WM_ENABLE = 10u,
		WM_SETREDRAW = 11u,
		WM_SETTEXT = 12u,
		WM_GETTEXT = 13u,
		WM_GETTEXTLENGTH = 14u,
		WM_PAINT = 15u,
		WM_CLOSE = 16u,
		WM_QUERYENDSESSION = 17u,
		WM_QUERYOPEN = 19u,
		WM_ENDSESSION = 22u,
		WM_QUIT = 18u,
		WM_ERASEBKGND = 20u,
		WM_SYSCOLORCHANGE = 21u,
		WM_SHOWWINDOW = 24u,
		WM_WININICHANGE = 26u,
		WM_SETTINGCHANGE = 26u,
		WM_DEVMODECHANGE = 27u,
		WM_ACTIVATEAPP = 28u,
		WM_FONTCHANGE = 29u,
		WM_TIMECHANGE = 30u,
		WM_CANCELMODE = 31u,
		WM_SETCURSOR = 32u,
		WM_MOUSEACTIVATE = 33u,
		WM_CHILDACTIVATE = 34u,
		WM_QUEUESYNC = 35u,
		WM_GETMINMAXINFO = 36u,
		WM_PAINTICON = 38u,
		WM_ICONERASEBKGND = 39u,
		WM_NEXTDLGCTL = 40u,
		WM_SPOOLERSTATUS = 42u,
		WM_DRAWITEM = 43u,
		WM_MEASUREITEM = 44u,
		WM_DELETEITEM = 45u,
		WM_VKEYTOITEM = 46u,
		WM_CHARTOITEM = 47u,
		WM_SETFONT = 48u,
		WM_GETFONT = 49u,
		WM_SETHOTKEY = 50u,
		WM_GETHOTKEY = 51u,
		WM_QUERYDRAGICON = 55u,
		WM_COMPAREITEM = 57u,
		WM_GETOBJECT = 61u,
		WM_COMPACTING = 65u,
		WM_WINDOWPOSCHANGING = 70u,
		WM_WINDOWPOSCHANGED = 71u,
		WM_COPYDATA = 74u,
		WM_CANCELJOURNAL = 75u,
		WM_NOTIFY = 78u,
		WM_INPUTLANGCHANGEREQUEST = 80u,
		WM_INPUTLANGCHANGE = 81u,
		WM_TCARD = 82u,
		WM_HELP = 83u,
		WM_USERCHANGED = 84u,
		WM_NOTIFYFORMAT = 85u,
		WM_CONTEXTMENU = 123u,
		WM_STYLECHANGING = 124u,
		WM_STYLECHANGED = 125u,
		WM_DISPLAYCHANGE = 126u,
		WM_GETICON = 127u,
		WM_SETICON = 128u,
		WM_NCCREATE = 129u,
		WM_NCDESTROY = 130u,
		WM_NCCALCSIZE = 131u,
		WM_NCHITTEST = 132u,
		WM_NCPAINT = 133u,
		WM_NCACTIVATE = 134u,
		WM_GETDLGCODE = 135u,
		WM_SYNCPAINT = 136u,
		WM_NCMOUSEMOVE = 160u,
		WM_NCLBUTTONDOWN = 161u,
		WM_NCLBUTTONUP = 162u,
		WM_NCLBUTTONDBLCLK = 163u,
		WM_NCRBUTTONDOWN = 164u,
		WM_NCRBUTTONUP = 165u,
		WM_NCRBUTTONDBLCLK = 166u,
		WM_NCMBUTTONDOWN = 167u,
		WM_NCMBUTTONUP = 168u,
		WM_NCMBUTTONDBLCLK = 169u,
		WM_NCXBUTTONDOWN = 171u,
		WM_NCXBUTTONUP = 172u,
		WM_NCXBUTTONDBLCLK = 173u,
		WM_INPUT_DEVICE_CHANGE = 254u,
		WM_INPUT = 255u,
		WM_KEYFIRST = 256u,
		WM_KEYDOWN = 256u,
		WM_KEYUP = 257u,
		WM_CHAR = 258u,
		WM_DEADCHAR = 259u,
		WM_SYSKEYDOWN = 260u,
		WM_SYSKEYUP = 261u,
		WM_SYSCHAR = 262u,
		WM_SYSDEADCHAR = 263u,
		WM_UNICHAR = 265u,
		WM_KEYLAST = 265u,
		WM_IME_STARTCOMPOSITION = 269u,
		WM_IME_ENDCOMPOSITION = 270u,
		WM_IME_COMPOSITION = 271u,
		WM_IME_KEYLAST = 271u,
		WM_INITDIALOG = 272u,
		WM_COMMAND = 273u,
		WM_SYSCOMMAND = 274u,
		WM_TIMER = 275u,
		WM_HSCROLL = 276u,
		WM_VSCROLL = 277u,
		WM_INITMENU = 278u,
		WM_INITMENUPOPUP = 279u,
		WM_MENUSELECT = 287u,
		WM_MENUCHAR = 288u,
		WM_ENTERIDLE = 289u,
		WM_MENURBUTTONUP = 290u,
		WM_MENUDRAG = 291u,
		WM_MENUGETOBJECT = 292u,
		WM_UNINITMENUPOPUP = 293u,
		WM_MENUCOMMAND = 294u,
		WM_CHANGEUISTATE = 295u,
		WM_UPDATEUISTATE = 296u,
		WM_QUERYUISTATE = 297u,
		WM_CTLCOLORMSGBOX = 306u,
		WM_CTLCOLOREDIT = 307u,
		WM_CTLCOLORLISTBOX = 308u,
		WM_CTLCOLORBTN = 309u,
		WM_CTLCOLORDLG = 310u,
		WM_CTLCOLORSCROLLBAR = 311u,
		WM_CTLCOLORSTATIC = 312u,
		WM_MOUSEFIRST = 512u,
		WM_MOUSEMOVE = 512u,
		WM_LBUTTONDOWN = 513u,
		WM_LBUTTONUP = 514u,
		WM_LBUTTONDBLCLK = 515u,
		WM_RBUTTONDOWN = 516u,
		WM_RBUTTONUP = 517u,
		WM_RBUTTONDBLCLK = 518u,
		WM_MBUTTONDOWN = 519u,
		WM_MBUTTONUP = 520u,
		WM_MBUTTONDBLCLK = 521u,
		WM_MOUSEWHEEL = 522u,
		WM_XBUTTONDOWN = 523u,
		WM_XBUTTONUP = 524u,
		WM_XBUTTONDBLCLK = 525u,
		WM_MOUSEHWHEEL = 526u,
		WM_MOUSELAST = 526u,
		WM_PARENTNOTIFY = 528u,
		WM_ENTERMENULOOP = 529u,
		WM_EXITMENULOOP = 530u,
		WM_NEXTMENU = 531u,
		WM_SIZING = 532u,
		WM_CAPTURECHANGED = 533u,
		WM_MOVING = 534u,
		WM_POWERBROADCAST = 536u,
		WM_DEVICECHANGE = 537u,
		WM_MDICREATE = 544u,
		WM_MDIDESTROY = 545u,
		WM_MDIACTIVATE = 546u,
		WM_MDIRESTORE = 547u,
		WM_MDINEXT = 548u,
		WM_MDIMAXIMIZE = 549u,
		WM_MDITILE = 550u,
		WM_MDICASCADE = 551u,
		WM_MDIICONARRANGE = 552u,
		WM_MDIGETACTIVE = 553u,
		WM_MDISETMENU = 560u,
		WM_ENTERSIZEMOVE = 561u,
		WM_EXITSIZEMOVE = 562u,
		WM_DROPFILES = 563u,
		WM_MDIREFRESHMENU = 564u,
		WM_POINTERDEVICECHANGE = 568u,
		WM_POINTERDEVICEINRANGE = 569u,
		WM_POINTERDEVICEOUTOFRANGE = 570u,
		WM_NCPOINTERUPDATE = 577u,
		WM_NCPOINTERDOWN = 578u,
		WM_NCPOINTERUP = 579u,
		WM_POINTERUPDATE = 581u,
		WM_POINTERDOWN = 582u,
		WM_POINTERUP = 583u,
		WM_POINTERENTER = 585u,
		WM_POINTERLEAVE = 586u,
		WM_POINTERACTIVATE = 587u,
		WM_POINTERCAPTURECHANGED = 588u,
		WM_TOUCHHITTESTING = 589u,
		WM_POINTERWHEEL = 590u,
		WM_POINTERHWHEEL = 591u,
		DM_POINTERHITTEST = 592u,
		WM_IME_SETCONTEXT = 641u,
		WM_IME_NOTIFY = 642u,
		WM_IME_CONTROL = 643u,
		WM_IME_COMPOSITIONFULL = 644u,
		WM_IME_SELECT = 645u,
		WM_IME_CHAR = 646u,
		WM_IME_REQUEST = 648u,
		WM_IME_KEYDOWN = 656u,
		WM_IME_KEYUP = 657u,
		WM_MOUSEHOVER = 673u,
		WM_MOUSELEAVE = 675u,
		WM_NCMOUSEHOVER = 672u,
		WM_NCMOUSELEAVE = 674u,
		WM_WTSSESSION_CHANGE = 689u,
		WM_TABLET_FIRST = 704u,
		WM_TABLET_LAST = 735u,
		WM_DPICHANGED = 736u,
		WM_CUT = 768u,
		WM_COPY = 769u,
		WM_PASTE = 770u,
		WM_CLEAR = 771u,
		WM_UNDO = 772u,
		WM_RENDERFORMAT = 773u,
		WM_RENDERALLFORMATS = 774u,
		WM_DESTROYCLIPBOARD = 775u,
		WM_DRAWCLIPBOARD = 776u,
		WM_PAINTCLIPBOARD = 777u,
		WM_VSCROLLCLIPBOARD = 778u,
		WM_SIZECLIPBOARD = 779u,
		WM_ASKCBFORMATNAME = 780u,
		WM_CHANGECBCHAIN = 781u,
		WM_HSCROLLCLIPBOARD = 782u,
		WM_QUERYNEWPALETTE = 783u,
		WM_PALETTEISCHANGING = 784u,
		WM_PALETTECHANGED = 785u,
		WM_HOTKEY = 786u,
		WM_PRINT = 791u,
		WM_PRINTCLIENT = 792u,
		WM_APPCOMMAND = 793u,
		WM_THEMECHANGED = 794u,
		WM_CLIPBOARDUPDATE = 797u,
		WM_DWMCOMPOSITIONCHANGED = 798u,
		WM_DWMNCRENDERINGCHANGED = 799u,
		WM_DWMCOLORIZATIONCOLORCHANGED = 800u,
		WM_DWMWINDOWMAXIMIZEDCHANGE = 801u,
		WM_GETTITLEBARINFOEX = 831u,
		WM_HANDHELDFIRST = 856u,
		WM_HANDHELDLAST = 863u,
		WM_AFXFIRST = 864u,
		WM_AFXLAST = 895u,
		WM_PENWINFIRST = 896u,
		WM_PENWINLAST = 911u,
		WM_TOUCH = 576u,
		WM_APP = 32768u,
		WM_USER = 1024u,
		WM_DISPATCH_WORK_ITEM = 1024u
	}

	public enum DwmWindowAttribute : uint
	{
		DWMWA_NCRENDERING_ENABLED = 1u,
		DWMWA_NCRENDERING_POLICY = 2u,
		DWMWA_TRANSITIONS_FORCEDISABLED = 3u,
		DWMWA_ALLOW_NCPAINT = 4u,
		DWMWA_CAPTION_BUTTON_BOUNDS = 5u,
		DWMWA_NONCLIENT_RTL_LAYOUT = 6u,
		DWMWA_FORCE_ICONIC_REPRESENTATION = 7u,
		DWMWA_FLIP3D_POLICY = 8u,
		DWMWA_EXTENDED_FRAME_BOUNDS = 9u,
		DWMWA_HAS_ICONIC_BITMAP = 10u,
		DWMWA_DISALLOW_PEEK = 11u,
		DWMWA_EXCLUDED_FROM_PEEK = 12u,
		DWMWA_CLOAK = 13u,
		DWMWA_CLOAKED = 14u,
		DWMWA_FREEZE_REPRESENTATION = 15u,
		DWMWA_PASSIVE_UPDATE_MODE = 16u,
		DWMWA_USE_HOSTBACKDROPBRUSH = 17u,
		DWMWA_USE_IMMERSIVE_DARK_MODE = 20u,
		DWMWA_WINDOW_CORNER_PREFERENCE = 33u,
		DWMWA_BORDER_COLOR = 34u,
		DWMWA_CAPTION_COLOR = 35u,
		DWMWA_TEXT_COLOR = 36u,
		DWMWA_VISIBLE_FRAME_BORDER_THICKNESS = 37u,
		DWMWA_LAST = 38u
	}

	public enum DwmWindowCornerPreference : uint
	{
		DWMWCP_DEFAULT,
		DWMWCP_DONOTROUND,
		DWMWCP_ROUND,
		DWMWCP_ROUNDSMALL
	}

	public enum MapVirtualKeyMapTypes : uint
	{
		MAPVK_VK_TO_VSC,
		MAPVK_VSC_TO_VK,
		MAPVK_VK_TO_CHAR,
		MAPVK_VSC_TO_VK_EX
	}

	public enum BitmapCompressionMode : uint
	{
		BI_RGB,
		BI_RLE8,
		BI_RLE4,
		BI_BITFIELDS,
		BI_JPEG,
		BI_PNG
	}

	public enum DIBColorTable
	{
		DIB_RGB_COLORS,
		DIB_PAL_COLORS
	}

	public enum WindowLongParam
	{
		GWL_WNDPROC = -4,
		GWL_HINSTANCE = -6,
		GWL_HWNDPARENT = -8,
		GWL_ID = -12,
		GWL_STYLE = -16,
		GWL_EXSTYLE = -20,
		GWL_USERDATA = -21
	}

	public enum MenuCharParam
	{
		MNC_IGNORE,
		MNC_CLOSE,
		MNC_EXECUTE,
		MNC_SELECT
	}

	public enum SysCommands
	{
		SC_SIZE = 61440,
		SC_MOVE = 61456,
		SC_MINIMIZE = 61472,
		SC_MAXIMIZE = 61488,
		SC_NEXTWINDOW = 61504,
		SC_PREVWINDOW = 61520,
		SC_CLOSE = 61536,
		SC_VSCROLL = 61552,
		SC_HSCROLL = 61568,
		SC_MOUSEMENU = 61584,
		SC_KEYMENU = 61696,
		SC_ARRANGE = 61712,
		SC_RESTORE = 61728,
		SC_TASKLIST = 61744,
		SC_SCREENSAVE = 61760,
		SC_HOTKEY = 61776,
		SC_DEFAULT = 61792,
		SC_MONITORPOWER = 61808,
		SC_CONTEXTHELP = 61824,
		SC_SEPARATOR = 61455,
		SCF_ISSECURE = 1
	}

	[Flags]
	public enum PointerFlags
	{
		POINTER_FLAG_NONE = 0,
		POINTER_FLAG_NEW = 1,
		POINTER_FLAG_INRANGE = 2,
		POINTER_FLAG_INCONTACT = 4,
		POINTER_FLAG_FIRSTBUTTON = 0x10,
		POINTER_FLAG_SECONDBUTTON = 0x20,
		POINTER_FLAG_THIRDBUTTON = 0x40,
		POINTER_FLAG_FOURTHBUTTON = 0x80,
		POINTER_FLAG_FIFTHBUTTON = 0x100,
		POINTER_FLAG_PRIMARY = 0x2000,
		POINTER_FLAG_CONFIDENCE = 0x400,
		POINTER_FLAG_CANCELED = 0x800,
		POINTER_FLAG_DOWN = 0x10000,
		POINTER_FLAG_UPDATE = 0x20000,
		POINTER_FLAG_UP = 0x40000,
		POINTER_FLAG_WHEEL = 0x80000,
		POINTER_FLAG_HWHEEL = 0x100000,
		POINTER_FLAG_CAPTURECHANGED = 0x200000,
		POINTER_FLAG_HASTRANSFORM = 0x400000
	}

	public enum PointerButtonChangeType : ulong
	{
		POINTER_CHANGE_NONE,
		POINTER_CHANGE_FIRSTBUTTON_DOWN,
		POINTER_CHANGE_FIRSTBUTTON_UP,
		POINTER_CHANGE_SECONDBUTTON_DOWN,
		POINTER_CHANGE_SECONDBUTTON_UP,
		POINTER_CHANGE_THIRDBUTTON_DOWN,
		POINTER_CHANGE_THIRDBUTTON_UP,
		POINTER_CHANGE_FOURTHBUTTON_DOWN,
		POINTER_CHANGE_FOURTHBUTTON_UP,
		POINTER_CHANGE_FIFTHBUTTON_DOWN,
		POINTER_CHANGE_FIFTHBUTTON_UP
	}

	[Flags]
	public enum PenFlags
	{
		PEN_FLAGS_NONE = 0,
		PEN_FLAGS_BARREL = 1,
		PEN_FLAGS_INVERTED = 2,
		PEN_FLAGS_ERASER = 4
	}

	[Flags]
	public enum PenMask
	{
		PEN_MASK_NONE = 0,
		PEN_MASK_PRESSURE = 1,
		PEN_MASK_ROTATION = 2,
		PEN_MASK_TILT_X = 4,
		PEN_MASK_TILT_Y = 8
	}

	[Flags]
	public enum TouchFlags
	{
		TOUCH_FLAG_NONE = 0
	}

	[Flags]
	public enum TouchMask
	{
		TOUCH_MASK_NONE = 0,
		TOUCH_MASK_CONTACTAREA = 1,
		TOUCH_MASK_ORIENTATION = 2,
		TOUCH_MASK_PRESSURE = 4
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct POINTER_TOUCH_INFO
	{
		public POINTER_INFO pointerInfo;

		public TouchFlags touchFlags;

		public TouchMask touchMask;

		public int rcContactLeft;

		public int rcContactTop;

		public int rcContactRight;

		public int rcContactBottom;

		public int rcContactRawLeft;

		public int rcContactRawTop;

		public int rcContactRawRight;

		public int rcContactRawBottom;

		public uint orientation;

		public uint pressure;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct POINTER_PEN_INFO
	{
		public POINTER_INFO pointerInfo;

		public PenFlags penFlags;

		public PenMask penMask;

		public uint pressure;

		public uint rotation;

		public int tiltX;

		public int tiltY;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct POINTER_INFO
	{
		public PointerInputType pointerType;

		public uint pointerId;

		public uint frameId;

		public PointerFlags pointerFlags;

		public IntPtr sourceDevice;

		public IntPtr hwndTarget;

		public int ptPixelLocationX;

		public int ptPixelLocationY;

		public int ptHimetricLocationX;

		public int ptHimetricLocationY;

		public int ptPixelLocationRawX;

		public int ptPixelLocationRawY;

		public int ptHimetricLocationRawX;

		public int ptHimetricLocationRawY;

		public uint dwTime;

		public uint historyCount;

		public int inputData;

		public ModifierKeys dwKeyStates;

		public ulong PerformanceCount;

		public PointerButtonChangeType ButtonChangeType;
	}

	public struct RGBQUAD
	{
		public byte rgbBlue;

		public byte rgbGreen;

		public byte rgbRed;

		public byte rgbReserved;
	}

	public struct BITMAPINFOHEADER
	{
		public uint biSize;

		public int biWidth;

		public int biHeight;

		public ushort biPlanes;

		public ushort biBitCount;

		public uint biCompression;

		public uint biSizeImage;

		public int biXPelsPerMeter;

		public int biYPelsPerMeter;

		public uint biClrUsed;

		public uint biClrImportant;

		public unsafe void Init()
		{
			biSize = (uint)sizeof(BITMAPINFOHEADER);
		}
	}

	public struct BITMAPINFO
	{
		public uint biSize;

		public int biWidth;

		public int biHeight;

		public ushort biPlanes;

		public ushort biBitCount;

		public BitmapCompressionMode biCompression;

		public uint biSizeImage;

		public int biXPelsPerMeter;

		public int biYPelsPerMeter;

		public uint biClrUsed;

		public uint biClrImportant;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public uint[] cols;
	}

	public struct MINMAXINFO
	{
		public POINT ptReserved;

		public POINT ptMaxSize;

		public POINT ptMaxPosition;

		public POINT ptMinTrackSize;

		public POINT ptMaxTrackSize;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct MOUSEMOVEPOINT
	{
		public int x;

		public int y;

		public int time;

		public IntPtr dwExtraInfo;
	}

	public delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

	public enum GetAncestorFlags
	{
		GA_PARENT = 1,
		GA_ROOT,
		GA_ROOTOWNER
	}

	public enum ClassLongIndex
	{
		GCLP_MENUNAME = -8,
		GCLP_HBRBACKGROUND = -10,
		GCLP_HCURSOR = -12,
		GCLP_HICON = -14,
		GCLP_HMODULE = -16,
		GCL_CBWNDEXTRA = -18,
		GCL_CBCLSEXTRA = -20,
		GCLP_WNDPROC = -24,
		GCL_STYLE = -26,
		GCLP_HICONSM = -34,
		GCW_ATOM = -32
	}

	[Flags]
	public enum LayeredWindowFlags
	{
		LWA_ALPHA = 2,
		LWA_COLORKEY = 1
	}

	[Flags]
	public enum DWM_BB
	{
		Enable = 1,
		BlurRegion = 2,
		TransitionMaximized = 4
	}

	public struct DWM_BLURBEHIND
	{
		public DWM_BB dwFlags;

		public bool fEnable;

		public IntPtr hRgnBlur;

		public bool fTransitionOnMaximized;

		public DWM_BLURBEHIND(bool enabled)
		{
			fEnable = enabled;
			hRgnBlur = IntPtr.Zero;
			fTransitionOnMaximized = false;
			dwFlags = DWM_BB.Enable;
		}
	}

	internal struct RTL_OSVERSIONINFOEX
	{
		internal uint dwOSVersionInfoSize;

		internal uint dwMajorVersion;

		internal uint dwMinorVersion;

		internal uint dwBuildNumber;

		internal uint dwPlatformId;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		internal string szCSDVersion;
	}

	[Flags]
	internal enum QueueStatusFlags
	{
		QS_KEY = 1,
		QS_MOUSEMOVE = 2,
		QS_MOUSEBUTTON = 4,
		QS_POSTMESSAGE = 8,
		QS_TIMER = 0x10,
		QS_PAINT = 0x20,
		QS_SENDMESSAGE = 0x40,
		QS_HOTKEY = 0x80,
		QS_ALLPOSTMESSAGE = 0x100,
		QS_EVENT = 0x2000,
		QS_MOUSE = 6,
		QS_INPUT = 7,
		QS_ALLEVENTS = 0xBF,
		QS_ALLINPUT = 0xFF
	}

	[Flags]
	internal enum MsgWaitForMultipleObjectsFlags
	{
		MWMO_WAITALL = 1,
		MWMO_ALERTABLE = 2,
		MWMO_INPUTAVAILABLE = 4
	}

	[Flags]
	public enum GCS : uint
	{
		GCS_COMPATTR = 0x10u,
		GCS_COMPCLAUSE = 0x20u,
		GCS_COMPREADATTR = 2u,
		GCS_COMPREADCLAUSE = 4u,
		GCS_COMPREADSTR = 1u,
		GCS_COMPSTR = 8u,
		GCS_CURSORPOS = 0x80u,
		GCS_DELTASTART = 0x100u,
		GCS_RESULTCLAUSE = 0x1000u,
		GCS_RESULTREADCLAUSE = 0x400u,
		GCS_RESULTREADSTR = 0x200u,
		GCS_RESULTSTR = 0x800u
	}

	internal struct CANDIDATEFORM
	{
		public int dwIndex;

		public int dwStyle;

		public POINT ptCurrentPos;

		public RECT rcArea;
	}

	internal struct COMPOSITIONFORM
	{
		public int dwStyle;

		public POINT ptCurrentPos;

		public RECT rcArea;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct LOGFONT
	{
		public int lfHeight;

		public int lfWidth;

		public int lfEscapement;

		public int lfOrientation;

		public int lfWeight;

		public byte lfItalic;

		public byte lfUnderline;

		public byte lfStrikeOut;

		public byte lfCharSet;

		public byte lfOutPrecision;

		public byte lfClipPrecision;

		public byte lfQuality;

		public byte lfPitchAndFamily;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string lfFaceName;
	}

	internal struct WindowCompositionAttributeData
	{
		public WindowCompositionAttribute Attribute;

		public IntPtr Data;

		public int SizeOfData;
	}

	internal enum WindowCompositionAttribute
	{
		WCA_ACCENT_POLICY = 19
	}

	internal enum AccentState
	{
		ACCENT_DISABLED,
		ACCENT_ENABLE_GRADIENT,
		ACCENT_ENABLE_TRANSPARENTGRADIENT,
		ACCENT_ENABLE_BLURBEHIND,
		ACCENT_ENABLE_ACRYLIC,
		ACCENT_ENABLE_HOSTBACKDROP,
		ACCENT_INVALID_STATE
	}

	internal enum AccentFlags
	{
		DrawLeftBorder = 0x20,
		DrawTopBorder = 0x40,
		DrawRightBorder = 0x80,
		DrawBottomBorder = 0x100
	}

	internal struct AccentPolicy
	{
		public AccentState AccentState;

		public int AccentFlags;

		public int GradientColor;

		public int AnimationId;
	}

	internal struct MARGINS
	{
		public int cxLeftWidth;

		public int cxRightWidth;

		public int cyTopHeight;

		public int cyBottomHeight;
	}

	public enum MONITOR
	{
		MONITOR_DEFAULTTONULL,
		MONITOR_DEFAULTTOPRIMARY,
		MONITOR_DEFAULTTONEAREST
	}

	internal struct MONITORINFO
	{
		public enum MonitorOptions : uint
		{
			MONITOR_DEFAULTTONULL,
			MONITOR_DEFAULTTOPRIMARY,
			MONITOR_DEFAULTTONEAREST
		}

		public int cbSize;

		public RECT rcMonitor;

		public RECT rcWork;

		public int dwFlags;

		public static MONITORINFO Create()
		{
			MONITORINFO result = default(MONITORINFO);
			result.cbSize = Marshal.SizeOf<MONITORINFO>();
			return result;
		}
	}

	public enum DEVICECAP
	{
		HORZRES = 8,
		DESKTOPHORZRES = 118
	}

	public enum PROCESS_DPI_AWARENESS
	{
		PROCESS_DPI_UNAWARE,
		PROCESS_SYSTEM_DPI_AWARE,
		PROCESS_PER_MONITOR_DPI_AWARE
	}

	public enum MONITOR_DPI_TYPE
	{
		MDT_EFFECTIVE_DPI = 0,
		MDT_ANGULAR_DPI = 1,
		MDT_RAW_DPI = 2,
		MDT_DEFAULT = 0
	}

	public enum ClipboardFormat
	{
		CF_TEXT = 1,
		CF_BITMAP = 2,
		CF_DIB = 3,
		CF_UNICODETEXT = 13,
		CF_HDROP = 15
	}

	public struct MSG
	{
		public IntPtr hwnd;

		public uint message;

		public IntPtr wParam;

		public IntPtr lParam;

		public uint time;

		public POINT pt;
	}

	public struct PAINTSTRUCT
	{
		public IntPtr hdc;

		public bool fErase;

		public RECT rcPaint;

		public bool fRestore;

		public bool fIncUpdate;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public byte[] rgbReserved;
	}

	public struct POINT
	{
		public int X;

		public int Y;
	}

	public struct SIZE
	{
		public int X;

		public int Y;
	}

	public struct SIZE_F
	{
		public float X;

		public float Y;
	}

	public struct RECT
	{
		public int left;

		public int top;

		public int right;

		public int bottom;

		public int Width => right - left;

		public int Height => bottom - top;

		public RECT(Rect rect)
		{
			left = (int)rect.X;
			top = (int)rect.Y;
			right = (int)(rect.X + rect.Width);
			bottom = (int)(rect.Y + rect.Height);
		}

		public void Offset(POINT pt)
		{
			left += pt.X;
			right += pt.X;
			top += pt.Y;
			bottom += pt.Y;
		}
	}

	public struct WINDOWPOS
	{
		public IntPtr hwnd;

		public IntPtr hwndInsertAfter;

		public int x;

		public int y;

		public int cx;

		public int cy;

		public uint flags;
	}

	public struct NCCALCSIZE_PARAMS
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public RECT[] rgrc;

		public WINDOWPOS lppos;
	}

	public struct TRACKMOUSEEVENT
	{
		public int cbSize;

		public uint dwFlags;

		public IntPtr hwndTrack;

		public int dwHoverTime;
	}

	public struct WINDOWPLACEMENT
	{
		public int Length;

		public int Flags;

		public ShowWindowCommand ShowCmd;

		public POINT MinPosition;

		public POINT MaxPosition;

		public RECT NormalPosition;

		public static WINDOWPLACEMENT Default
		{
			get
			{
				WINDOWPLACEMENT result = default(WINDOWPLACEMENT);
				result.Length = Marshal.SizeOf<WINDOWPLACEMENT>();
				return result;
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct WNDCLASSEX
	{
		public int cbSize;

		public int style;

		public WndProc lpfnWndProc;

		public int cbClsExtra;

		public int cbWndExtra;

		public IntPtr hInstance;

		public IntPtr hIcon;

		public IntPtr hCursor;

		public IntPtr hbrBackground;

		public string lpszMenuName;

		public string lpszClassName;

		public IntPtr hIconSm;
	}

	public struct TOUCHINPUT
	{
		public int X;

		public int Y;

		public IntPtr Source;

		public uint Id;

		public TouchInputFlags Flags;

		public int Mask;

		public uint Time;

		public IntPtr ExtraInfo;

		public int CxContact;

		public int CyContact;
	}

	public struct ICONINFO
	{
		public bool IsIcon;

		public int xHotspot;

		public int yHotspot;

		public IntPtr MaskBitmap;

		public IntPtr ColorBitmap;
	}

	[Flags]
	public enum TouchInputFlags
	{
		TOUCHEVENTF_MOVE = 1,
		TOUCHEVENTF_DOWN = 2,
		TOUCHEVENTF_UP = 4,
		TOUCHEVENTF_INRANGE = 8,
		TOUCHEVENTF_PRIMARY = 0x10,
		TOUCHEVENTF_NOCOALESCE = 0x20,
		TOUCHEVENTF_PALM = 0x80
	}

	[Flags]
	public enum OpenFileNameFlags
	{
		OFN_ALLOWMULTISELECT = 0x200,
		OFN_EXPLORER = 0x80000,
		OFN_HIDEREADONLY = 4,
		OFN_NOREADONLYRETURN = 0x8000,
		OFN_OVERWRITEPROMPT = 2
	}

	public enum HRESULT : uint
	{
		S_FALSE = 1u,
		S_OK = 0u,
		E_INVALIDARG = 2147942487u,
		E_OUTOFMEMORY = 2147942414u,
		E_NOTIMPL = 2147500033u,
		E_UNEXPECTED = 2147549183u,
		E_CANCELLED = 2147943623u
	}

	public enum Icons
	{
		ICON_SMALL,
		ICON_BIG
	}

	public static class ShellIds
	{
		public static readonly Guid OpenFileDialog = Guid.Parse("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7");

		public static readonly Guid SaveFileDialog = Guid.Parse("C0B4E2F3-BA21-4773-8DBA-335EC946EB8B");

		public static readonly Guid IFileDialog = Guid.Parse("42F85136-DB7E-439C-85F1-E4075D135FC8");

		public static readonly Guid IShellItem = Guid.Parse("43826D1E-E718-42EE-BC55-A1E261C37BFE");

		public static readonly Guid TaskBarList = Guid.Parse("56FDF344-FD6D-11D0-958A-006097C9A090");

		public static readonly Guid ITaskBarList2 = Guid.Parse("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf");
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct COMDLG_FILTERSPEC
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public string pszName;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string pszSpec;
	}

	public delegate void MarkFullscreenWindow(IntPtr This, IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fullscreen);

	public delegate HRESULT HrInit(IntPtr This);

	public struct ITaskBarList2VTable
	{
		public IntPtr IUnknown1;

		public IntPtr IUnknown2;

		public IntPtr IUnknown3;

		public IntPtr HrInit;

		public IntPtr AddTab;

		public IntPtr DeleteTab;

		public IntPtr ActivateTab;

		public IntPtr SetActiveAlt;

		public IntPtr MarkFullscreenWindow;
	}

	public const int CW_USEDEFAULT = int.MinValue;

	public static readonly IntPtr DPI_AWARENESS_CONTEXT_UNAWARE = new IntPtr(-1);

	public static readonly IntPtr DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = new IntPtr(-2);

	public static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = new IntPtr(-3);

	public static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new IntPtr(-4);

	public const int SizeOf_BITMAPINFOHEADER = 40;

	public const int WAIT_FAILED = -1;

	public const int SORT_DEFAULT = 0;

	public const int LANG_ZH = 4;

	public const int LANG_JA = 17;

	public const int LANG_KO = 18;

	public const int CFS_FORCE_POSITION = 32;

	public const int CFS_CANDIDATEPOS = 64;

	public const int CFS_EXCLUDE = 128;

	public const int CFS_POINT = 2;

	public const int CFS_RECT = 1;

	public const long ISC_SHOWUICANDIDATEWINDOW = 1L;

	public const long ISC_SHOWUICOMPOSITIONWINDOW = 2147483648L;

	public const long ISC_SHOWUIGUIDELINE = 1073741824L;

	public const long ISC_SHOWUIALLCANDIDATEWINDOW = 15L;

	public const long ISC_SHOWUIALL = 3221225487L;

	public const int NI_COMPOSITIONSTR = 21;

	public const int CPS_COMPLETE = 1;

	public const int CPS_CONVERT = 2;

	public const int CPS_REVERT = 3;

	public const int CPS_CANCEL = 4;

	public static bool ShCoreAvailable => LoadLibrary("shcore.dll") != IntPtr.Zero;

	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
	internal unsafe static extern int GetMouseMovePointsEx(uint cbSize, MOUSEMOVEPOINT* pointsIn, MOUSEMOVEPOINT* pointsBufferOut, int nBufPoints, uint resolution);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool IsMouseInPointerEnabled();

	[DllImport("user32.dll", SetLastError = true)]
	public static extern int EnableMouseInPointer(bool enable);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool GetPointerCursorId(uint pointerId, out uint cursorId);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool GetPointerType(uint pointerId, out PointerInputType pointerType);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool GetPointerInfo(uint pointerId, out POINTER_INFO pointerInfo);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool GetPointerInfoHistory(uint pointerId, ref int entriesCount, [In][Out][MarshalAs(UnmanagedType.LPArray)] POINTER_INFO[] pointerInfos);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool GetPointerPenInfo(uint pointerId, out POINTER_PEN_INFO penInfo);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool GetPointerPenInfoHistory(uint pointerId, ref int entriesCount, [In][Out][MarshalAs(UnmanagedType.LPArray)] POINTER_PEN_INFO[] penInfos);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool GetPointerTouchInfo(uint pointerId, out POINTER_TOUCH_INFO touchInfo);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool GetPointerTouchInfoHistory(uint pointerId, ref int entriesCount, [In][Out][MarshalAs(UnmanagedType.LPArray)] POINTER_TOUCH_INFO[] touchInfos);

	[DllImport("user32.dll")]
	public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr GetDC(IntPtr hWnd);

	[DllImport("gdi32.dll")]
	public static extern int SetDIBitsToDevice(IntPtr hdc, int XDest, int YDest, uint dwWidth, uint dwHeight, int XSrc, int YSrc, uint uStartScan, uint cScanLines, IntPtr lpvBits, [In] ref BITMAPINFO lpbmi, uint fuColorUse);

	[DllImport("user32.dll")]
	public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool AdjustWindowRectEx(ref RECT lpRect, uint dwStyle, bool bMenu, uint dwExStyle);

	[DllImport("user32.dll")]
	public static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

	[DllImport("user32.dll")]
	public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

	[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "CreateWindowExW", ExactSpelling = true, SetLastError = true)]
	public static extern IntPtr CreateWindowEx(int dwExStyle, uint lpClassName, string? lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

	[DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
	public static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll", EntryPoint = "DispatchMessageW")]
	public static extern IntPtr DispatchMessage(ref MSG lpmsg);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool DestroyWindow(IntPtr hwnd);

	[DllImport("user32.dll")]
	public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

	[DllImport("user32.dll")]
	public static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

	[DllImport("user32.dll")]
	public static extern uint GetCaretBlinkTime();

	[DllImport("user32.dll")]
	public static extern bool GetClientRect(IntPtr hwnd, out RECT lpRect);

	[DllImport("user32.dll")]
	public static extern bool GetCursorPos(out POINT lpPoint);

	[DllImport("user32.dll")]
	public static extern uint GetDoubleClickTime();

	[DllImport("user32.dll")]
	public static extern bool GetKeyboardState(byte[] lpKeyState);

	[DllImport("user32.dll", EntryPoint = "MapVirtualKeyW")]
	public static extern uint MapVirtualKey(uint uCode, uint uMapType);

	[DllImport("user32.dll", EntryPoint = "GetMessageW", SetLastError = true)]
	public static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

	[DllImport("user32.dll")]
	public static extern int GetMessageTime();

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetModuleHandleW", ExactSpelling = true)]
	public static extern IntPtr GetModuleHandle(string? lpModuleName);

	[DllImport("user32.dll")]
	public static extern int GetSystemMetrics(SystemMetric smIndex);

	[DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", ExactSpelling = true, SetLastError = true)]
	public static extern uint GetWindowLongPtr(IntPtr hWnd, int nIndex);

	[DllImport("user32.dll", EntryPoint = "GetWindowLongW", ExactSpelling = true, SetLastError = true)]
	public static extern uint GetWindowLong32b(IntPtr hWnd, int nIndex);

	public static uint GetWindowLong(IntPtr hWnd, int nIndex)
	{
		if (IntPtr.Size == 4)
		{
			return GetWindowLong32b(hWnd, nIndex);
		}
		return GetWindowLongPtr(hWnd, nIndex);
	}

	[DllImport("user32.dll", EntryPoint = "SetWindowLongW", ExactSpelling = true, SetLastError = true)]
	private static extern uint SetWindowLong32b(IntPtr hWnd, int nIndex, uint value);

	[DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", ExactSpelling = true, SetLastError = true)]
	private static extern IntPtr SetWindowLong64b(IntPtr hWnd, int nIndex, IntPtr value);

	public static uint SetWindowLong(IntPtr hWnd, int nIndex, uint value)
	{
		if (IntPtr.Size == 4)
		{
			return SetWindowLong32b(hWnd, nIndex, value);
		}
		return (uint)SetWindowLong64b(hWnd, nIndex, new IntPtr(value)).ToInt32();
	}

	public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr handle)
	{
		if (IntPtr.Size == 4)
		{
			return new IntPtr(SetWindowLong32b(hWnd, nIndex, (uint)handle.ToInt32()));
		}
		return SetWindowLong64b(hWnd, nIndex, handle);
	}

	[DllImport("user32.dll")]
	public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

	[DllImport("user32.dll")]
	public static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

	[DllImport("user32.dll")]
	public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

	[DllImport("user32.dll")]
	public static extern bool GetUpdateRect(IntPtr hwnd, out RECT lpRect, bool bErase);

	[DllImport("user32.dll")]
	public static extern bool InvalidateRect(IntPtr hWnd, ref RECT lpRect, bool bErase);

	[DllImport("user32.dll")]
	public unsafe static extern bool InvalidateRect(IntPtr hWnd, RECT* lpRect, bool bErase);

	[DllImport("user32.dll")]
	public static extern bool ValidateRect(IntPtr hWnd, IntPtr lpRect);

	[DllImport("user32.dll")]
	public static extern bool IsWindow(IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern bool IsWindowEnabled(IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern bool IsWindowUnicode(IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern bool IsWindowVisible(IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern bool KillTimer(IntPtr hWnd, IntPtr uIDEvent);

	[DllImport("user32.dll", EntryPoint = "LoadCursorW", ExactSpelling = true)]
	public static extern IntPtr LoadCursor(IntPtr hInstance, IntPtr lpCursorName);

	[DllImport("user32.dll")]
	public static extern IntPtr CreateIconIndirect([In] ref ICONINFO iconInfo);

	[DllImport("user32.dll")]
	public static extern bool DestroyIcon(IntPtr hIcon);

	[DllImport("user32.dll", EntryPoint = "PeekMessageW", ExactSpelling = true)]
	public static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

	[DllImport("user32")]
	public static extern IntPtr GetMessageExtraInfo();

	[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegisterClassExW", SetLastError = true)]
	public static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

	[DllImport("user32.dll")]
	public static extern void RegisterTouchWindow(IntPtr hWnd, int flags);

	[DllImport("user32.dll")]
	public static extern bool ReleaseCapture();

	[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegisterWindowMessageW", ExactSpelling = true, SetLastError = true)]
	public static extern uint RegisterWindowMessage(string lpString);

	[DllImport("user32.dll")]
	public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr GetActiveWindow();

	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr SetActiveWindow(IntPtr hWnd);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool SetForegroundWindow(IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern IntPtr SetCapture(IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern IntPtr SetTimer(IntPtr hWnd, IntPtr nIDEvent, uint uElapse, TimerProc lpTimerFunc);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

	[DllImport("user32.dll")]
	public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags);

	[DllImport("user32.dll")]
	public static extern bool SetFocus(IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern IntPtr GetFocus();

	[DllImport("user32.dll")]
	public static extern bool SetParent(IntPtr hWnd, IntPtr hWndNewParent);

	[DllImport("user32.dll")]
	public static extern IntPtr GetParent(IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern IntPtr GetAncestor(IntPtr hwnd, GetAncestorFlags gaFlags);

	[DllImport("user32.dll")]
	public static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommand nCmdShow);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern IntPtr CreateTimerQueue();

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool DeleteTimerQueueEx(IntPtr TimerQueue, IntPtr CompletionEvent);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CreateTimerQueueTimer(out IntPtr phNewTimer, IntPtr TimerQueue, WaitOrTimerCallback Callback, IntPtr Parameter, uint DueTime, uint Period, uint Flags);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool DeleteTimerQueueTimer(IntPtr TimerQueue, IntPtr Timer, IntPtr CompletionEvent);

	[DllImport("user32.dll")]
	public static extern int ToUnicode(uint virtualKeyCode, uint scanCode, byte[] keyboardState, [Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder receivingBuffer, int bufferSize, uint flags);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

	[DllImport("user32.dll")]
	public static extern bool TranslateMessage(ref MSG lpMsg);

	[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "UnregisterClassW", ExactSpelling = true)]
	public static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

	[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "SetWindowTextW", SetLastError = true)]
	public static extern bool SetWindowText(IntPtr hwnd, string? lpString);

	[DllImport("shell32", CharSet = CharSet.Auto)]
	public static extern int Shell_NotifyIcon(NIM dwMessage, NOTIFYICONDATA lpData);

	[DllImport("user32.dll", EntryPoint = "SetClassLongPtrW", ExactSpelling = true)]
	private static extern IntPtr SetClassLong64(IntPtr hWnd, ClassLongIndex nIndex, IntPtr dwNewLong);

	[DllImport("user32.dll", EntryPoint = "SetClassLongW", ExactSpelling = true)]
	private static extern IntPtr SetClassLong32(IntPtr hWnd, ClassLongIndex nIndex, IntPtr dwNewLong);

	public static IntPtr SetClassLong(IntPtr hWnd, ClassLongIndex nIndex, IntPtr dwNewLong)
	{
		if (IntPtr.Size == 4)
		{
			return SetClassLong32(hWnd, nIndex, dwNewLong);
		}
		return SetClassLong64(hWnd, nIndex, dwNewLong);
	}

	public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
	{
		if (IntPtr.Size > 4)
		{
			return GetClassLongPtr64(hWnd, nIndex);
		}
		return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
	}

	[DllImport("user32.dll", EntryPoint = "GetClassLongW", ExactSpelling = true)]
	public static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

	[DllImport("user32.dll", EntryPoint = "GetClassLongPtrW", ExactSpelling = true)]
	public static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

	[DllImport("user32.dll")]
	internal static extern IntPtr SetCursor(IntPtr hCursor);

	[DllImport("ole32.dll")]
	internal static extern int CoCreateInstance(ref Guid clsid, IntPtr ignore1, int ignore2, ref Guid iid, [MarshalAs(UnmanagedType.IUnknown)] out object pUnkOuter);

	[DllImport("ole32.dll")]
	internal static extern int CoCreateInstance(ref Guid clsid, IntPtr ignore1, int ignore2, ref Guid iid, out IntPtr pUnkOuter);

	internal static T CreateInstance<T>(ref Guid clsid, ref Guid iid) where T : IUnknown
	{
		IntPtr pUnkOuter;
		int num = CoCreateInstance(ref clsid, IntPtr.Zero, 1, ref iid, out pUnkOuter);
		if (num != 0)
		{
			throw new COMException("CreateInstance", num);
		}
		using IUnknown unknown = MicroComRuntime.CreateProxyFor<IUnknown>(pUnkOuter, ownsHandle: true);
		return unknown.QueryInterface<T>();
	}

	[DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	internal static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IntPtr pbc, ref Guid riid, out IntPtr ppv);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool OpenClipboard(IntPtr hWndOwner);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool CloseClipboard();

	[DllImport("user32.dll")]
	public static extern bool EmptyClipboard();

	[DllImport("user32.dll")]
	public static extern IntPtr GetClipboardData(ClipboardFormat uFormat);

	[DllImport("user32.dll")]
	public static extern IntPtr SetClipboardData(ClipboardFormat uFormat, IntPtr hMem);

	[DllImport("ole32.dll", PreserveSig = false)]
	public static extern int OleGetClipboard(out IntPtr dataObject);

	[DllImport("ole32.dll")]
	public static extern int OleSetClipboard(IntPtr dataObject);

	[DllImport("kernel32.dll", ExactSpelling = true)]
	public static extern IntPtr GlobalLock(IntPtr handle);

	[DllImport("kernel32.dll", ExactSpelling = true)]
	public static extern bool GlobalUnlock(IntPtr handle);

	[DllImport("kernel32.dll", ExactSpelling = true)]
	public static extern IntPtr GlobalAlloc(int uFlags, int dwBytes);

	[DllImport("kernel32.dll", ExactSpelling = true)]
	public static extern IntPtr GlobalFree(IntPtr hMem);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryW", ExactSpelling = true, SetLastError = true)]
	public static extern IntPtr LoadLibrary(string fileName);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryExW", ExactSpelling = true, SetLastError = true)]
	public static extern IntPtr LoadLibraryEx(string fileName, IntPtr hFile, int flags);

	[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
	public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

	[DllImport("comdlg32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetSaveFileNameW")]
	public static extern bool GetSaveFileName(IntPtr lpofn);

	[DllImport("comdlg32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetOpenFileNameW")]
	public static extern bool GetOpenFileName(IntPtr lpofn);

	[DllImport("comdlg32.dll")]
	public static extern int CommDlgExtendedError();

	[DllImport("shcore.dll")]
	public static extern void SetProcessDpiAwareness(PROCESS_DPI_AWARENESS value);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool SetProcessDpiAwarenessContext(IntPtr dpiAWarenessContext);

	[DllImport("shcore.dll")]
	public static extern long GetDpiForMonitor(IntPtr hmonitor, MONITOR_DPI_TYPE dpiType, out uint dpiX, out uint dpiY);

	[DllImport("gdi32.dll")]
	public static extern int GetDeviceCaps(IntPtr hdc, DEVICECAP nIndex);

	[DllImport("shcore.dll")]
	public static extern void GetScaleFactorForMonitor(IntPtr hMon, out uint pScale);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool SetProcessDPIAware();

	[DllImport("user32.dll")]
	public static extern IntPtr MonitorFromPoint(POINT pt, MONITOR dwFlags);

	[DllImport("user32.dll")]
	public static extern IntPtr MonitorFromRect(RECT rect, MONITOR dwFlags);

	[DllImport("user32.dll")]
	public static extern IntPtr MonitorFromWindow(IntPtr hwnd, MONITOR dwFlags);

	[DllImport("user32", CharSet = CharSet.Unicode, EntryPoint = "GetMonitorInfoW", ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetMonitorInfo([In] IntPtr hMonitor, ref MONITORINFO lpmi);

	[DllImport("user32")]
	public unsafe static extern bool GetTouchInputInfo(IntPtr hTouchInput, uint cInputs, TOUCHINPUT* pInputs, int cbSize);

	[DllImport("user32")]
	public static extern bool CloseTouchInputHandle(IntPtr hTouchInput);

	[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "PostMessageW", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

	[DllImport("gdi32.dll")]
	public static extern int SetDIBitsToDevice(IntPtr hdc, int XDest, int YDest, uint dwWidth, uint dwHeight, int XSrc, int YSrc, uint uStartScan, uint cScanLines, IntPtr lpvBits, [In] ref BITMAPINFOHEADER lpbmi, uint fuColorUse);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CloseHandle(IntPtr hObject);

	[DllImport("gdi32.dll", SetLastError = true)]
	public static extern IntPtr CreateDIBSection(IntPtr hDC, ref BITMAPINFOHEADER pBitmapInfo, int un, out IntPtr lplpVoid, IntPtr handle, int dw);

	[DllImport("gdi32.dll")]
	public static extern int DeleteObject(IntPtr hObject);

	[DllImport("gdi32.dll", SetLastError = true)]
	public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

	[DllImport("gdi32.dll")]
	public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

	[DllImport("gdi32.dll")]
	public static extern int ChoosePixelFormat(IntPtr hdc, ref PixelFormatDescriptor pfd);

	[DllImport("gdi32.dll")]
	public static extern int DescribePixelFormat(IntPtr hdc, ref PixelFormatDescriptor pfd);

	[DllImport("gdi32.dll")]
	public static extern int SetPixelFormat(IntPtr hdc, int iPixelFormat, ref PixelFormatDescriptor pfd);

	[DllImport("gdi32.dll")]
	public static extern int DescribePixelFormat(IntPtr hdc, int iPixelFormat, int bytes, ref PixelFormatDescriptor pfd);

	[DllImport("gdi32.dll")]
	public static extern bool SwapBuffers(IntPtr hdc);

	[DllImport("opengl32.dll")]
	public static extern IntPtr wglCreateContext(IntPtr hdc);

	[DllImport("opengl32.dll")]
	public static extern bool wglDeleteContext(IntPtr context);

	[DllImport("opengl32.dll", SetLastError = true)]
	public static extern bool wglMakeCurrent(IntPtr hdc, IntPtr context);

	[DllImport("opengl32.dll")]
	public static extern IntPtr wglGetCurrentContext();

	[DllImport("opengl32.dll")]
	public static extern IntPtr wglGetCurrentDC();

	[DllImport("opengl32.dll", CharSet = CharSet.Ansi)]
	public static extern IntPtr wglGetProcAddress(string name);

	[DllImport("kernel32.dll", EntryPoint = "CreateFileMappingW", ExactSpelling = true, SetLastError = true)]
	public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

	[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "memcpy")]
	public static extern IntPtr CopyMemory(IntPtr dest, IntPtr src, UIntPtr count);

	[DllImport("ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
	public static extern HRESULT RegisterDragDrop(IntPtr hwnd, IntPtr target);

	[DllImport("ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
	public static extern HRESULT RevokeDragDrop(IntPtr hwnd);

	[DllImport("ole32.dll")]
	public static extern HRESULT OleInitialize(IntPtr val);

	[DllImport("ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
	internal static extern void ReleaseStgMedium(ref STGMEDIUM medium);

	[DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetClipboardFormatNameW", ExactSpelling = true, SetLastError = true)]
	public static extern int GetClipboardFormatName(int format, StringBuilder lpString, int cchMax);

	[DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegisterClipboardFormatW", ExactSpelling = true, SetLastError = true)]
	public static extern int RegisterClipboardFormat(string format);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
	public static extern IntPtr GlobalSize(IntPtr hGlobal);

	[DllImport("shell32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "DragQueryFileW", ExactSpelling = true)]
	public static extern int DragQueryFile(IntPtr hDrop, int iFile, StringBuilder? lpszFile, int cch);

	[DllImport("ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true, PreserveSig = false)]
	internal static extern void DoDragDrop(IntPtr dataObject, IntPtr dropSource, int allowedEffects, out int finalEffect);

	[DllImport("dwmapi.dll")]
	public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

	[DllImport("dwmapi.dll")]
	public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

	[DllImport("dwmapi.dll")]
	public unsafe static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, void* pvAttribute, int cbAttribute);

	[DllImport("dwmapi.dll")]
	public static extern int DwmIsCompositionEnabled(out bool enabled);

	[DllImport("dwmapi.dll")]
	public static extern void DwmFlush();

	[DllImport("dwmapi.dll")]
	public static extern bool DwmDefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, ref IntPtr plResult);

	[DllImport("dwmapi.dll")]
	public static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);

	[DllImport("user32.dll")]
	public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, LayeredWindowFlags dwFlags);

	[DllImport("ntdll")]
	private static extern int RtlGetVersion(ref RTL_OSVERSIONINFOEX lpVersionInformation);

	internal static Version RtlGetVersion()
	{
		RTL_OSVERSIONINFOEX lpVersionInformation = default(RTL_OSVERSIONINFOEX);
		lpVersionInformation.dwOSVersionInfoSize = (uint)Marshal.SizeOf<RTL_OSVERSIONINFOEX>();
		if (RtlGetVersion(ref lpVersionInformation) == 0)
		{
			return new Version((int)lpVersionInformation.dwMajorVersion, (int)lpVersionInformation.dwMinorVersion, (int)lpVersionInformation.dwBuildNumber);
		}
		throw new Exception("RtlGetVersion failed!");
	}

	[DllImport("kernel32", CharSet = CharSet.Auto, EntryPoint = "WaitForMultipleObjectsEx", SetLastError = true)]
	private static extern int IntWaitForMultipleObjectsEx(int nCount, IntPtr[] pHandles, bool bWaitAll, int dwMilliseconds, bool bAlertable);

	internal static int WaitForMultipleObjectsEx(int nCount, IntPtr[] pHandles, bool bWaitAll, int dwMilliseconds, bool bAlertable)
	{
		int num = IntWaitForMultipleObjectsEx(nCount, pHandles, bWaitAll, dwMilliseconds, bAlertable);
		if (num == -1)
		{
			throw new Win32Exception();
		}
		return num;
	}

	[DllImport("user32", CharSet = CharSet.Auto, EntryPoint = "MsgWaitForMultipleObjectsEx", ExactSpelling = true, SetLastError = true)]
	private static extern int IntMsgWaitForMultipleObjectsEx(int nCount, IntPtr[]? pHandles, int dwMilliseconds, QueueStatusFlags dwWakeMask, MsgWaitForMultipleObjectsFlags dwFlags);

	internal static int MsgWaitForMultipleObjectsEx(int nCount, IntPtr[]? pHandles, int dwMilliseconds, QueueStatusFlags dwWakeMask, MsgWaitForMultipleObjectsFlags dwFlags)
	{
		int num = IntMsgWaitForMultipleObjectsEx(nCount, pHandles, dwMilliseconds, dwWakeMask, dwFlags);
		if (num == -1)
		{
			throw new Win32Exception();
		}
		return num;
	}

	internal unsafe static int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data)
	{
		delegate* unmanaged[Stdcall]<IntPtr, WindowCompositionAttributeData*, int> delegate_002A = (delegate* unmanaged[Stdcall]<IntPtr, WindowCompositionAttributeData*, int>)(void*)GetProcAddress(LoadLibrary("user32.dll"), "SetWindowCompositionAttribute");
		if (delegate_002A == (delegate* unmanaged[Stdcall]<IntPtr, WindowCompositionAttributeData*, int>)null)
		{
			throw new EntryPointNotFoundException("The unsupported SetWindowCompositionAttribute-function has been removed from the operating system.");
		}
		fixed (WindowCompositionAttributeData* ptr = &data)
		{
			return delegate_002A(hwnd, ptr);
		}
	}

	[DllImport("imm32.dll", SetLastError = true)]
	public static extern IntPtr ImmGetContext(IntPtr hWnd);

	[DllImport("imm32.dll", SetLastError = true)]
	public static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);

	[DllImport("imm32.dll", SetLastError = true)]
	public static extern IntPtr ImmCreateContext();

	[DllImport("imm32.dll")]
	public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

	[DllImport("imm32.dll")]
	public static extern bool ImmSetOpenStatus(IntPtr hIMC, bool flag);

	[DllImport("imm32.dll")]
	public static extern bool ImmSetActiveContext(IntPtr hIMC, bool flag);

	[DllImport("imm32.dll")]
	public static extern bool ImmSetStatusWindowPos(IntPtr hIMC, ref POINT lpptPos);

	[DllImport("imm32.dll")]
	public static extern bool ImmIsIME(IntPtr HKL);

	[DllImport("imm32.dll")]
	public static extern bool ImmSetCandidateWindow(IntPtr hIMC, ref CANDIDATEFORM lpCandidate);

	[DllImport("imm32.dll")]
	public static extern bool ImmSetCompositionWindow(IntPtr hIMC, ref COMPOSITIONFORM lpComp);

	[DllImport("imm32.dll")]
	public static extern bool ImmSetCompositionFont(IntPtr hIMC, ref LOGFONT lf);

	[DllImport("imm32.dll", CharSet = CharSet.Unicode, EntryPoint = "ImmGetCompositionStringW", ExactSpelling = true)]
	public static extern int ImmGetCompositionString(IntPtr hIMC, GCS dwIndex, [Optional][Out] IntPtr lpBuf, uint dwBufLen);

	public unsafe static string? ImmGetCompositionString(IntPtr hIMC, GCS dwIndex)
	{
		int num = ImmGetCompositionString(hIMC, dwIndex, IntPtr.Zero, 0u);
		if (num > 0)
		{
			Span<byte> span = ((num > 64) ? ((Span<byte>)new byte[num]) : stackalloc byte[num]);
			Span<byte> span2 = span;
			fixed (byte* ptr = span2)
			{
				int num2 = ImmGetCompositionString(hIMC, dwIndex, (IntPtr)ptr, (uint)num);
				if (num2 >= 0)
				{
					return Encoding.Unicode.GetString(ptr, num2);
				}
			}
		}
		return null;
	}

	[DllImport("imm32.dll")]
	public static extern bool ImmNotifyIME(IntPtr hIMC, int dwAction, int dwIndex, int dwValue);

	[DllImport("user32.dll")]
	public static extern bool CreateCaret(IntPtr hwnd, IntPtr hBitmap, int nWidth, int nHeight);

	[DllImport("user32.dll")]
	public static extern bool SetCaretPos(int X, int Y);

	[DllImport("user32.dll")]
	public static extern bool DestroyCaret();

	[DllImport("user32.dll")]
	public static extern IntPtr GetKeyboardLayout(int idThread);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern int LCIDToLocaleName(uint Locale, StringBuilder lpName, int cchName, int dwFlags);

	public static uint MAKELCID(uint lgid, uint srtid)
	{
		return (uint)(((ushort)srtid << 16) | (ushort)lgid);
	}

	public static ushort PRIMARYLANGID(uint lgid)
	{
		return (ushort)(lgid & 0x3FF);
	}

	public static uint LGID(IntPtr HKL)
	{
		return (uint)((long)HKL & 0xFFFF);
	}
}
