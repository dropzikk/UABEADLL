using System;
using System.Collections.Generic;

namespace Avalonia.X11;

internal class X11Atoms
{
	private readonly IntPtr _display;

	public IntPtr AnyPropertyType = (IntPtr)0;

	public IntPtr XA_PRIMARY = (IntPtr)1;

	public IntPtr XA_SECONDARY = (IntPtr)2;

	public IntPtr XA_ARC = (IntPtr)3;

	public IntPtr XA_ATOM = (IntPtr)4;

	public IntPtr XA_BITMAP = (IntPtr)5;

	public IntPtr XA_CARDINAL = (IntPtr)6;

	public IntPtr XA_COLORMAP = (IntPtr)7;

	public IntPtr XA_CURSOR = (IntPtr)8;

	public IntPtr XA_CUT_BUFFER0 = (IntPtr)9;

	public IntPtr XA_CUT_BUFFER1 = (IntPtr)10;

	public IntPtr XA_CUT_BUFFER2 = (IntPtr)11;

	public IntPtr XA_CUT_BUFFER3 = (IntPtr)12;

	public IntPtr XA_CUT_BUFFER4 = (IntPtr)13;

	public IntPtr XA_CUT_BUFFER5 = (IntPtr)14;

	public IntPtr XA_CUT_BUFFER6 = (IntPtr)15;

	public IntPtr XA_CUT_BUFFER7 = (IntPtr)16;

	public IntPtr XA_DRAWABLE = (IntPtr)17;

	public IntPtr XA_FONT = (IntPtr)18;

	public IntPtr XA_INTEGER = (IntPtr)19;

	public IntPtr XA_PIXMAP = (IntPtr)20;

	public IntPtr XA_POINT = (IntPtr)21;

	public IntPtr XA_RECTANGLE = (IntPtr)22;

	public IntPtr XA_RESOURCE_MANAGER = (IntPtr)23;

	public IntPtr XA_RGB_COLOR_MAP = (IntPtr)24;

	public IntPtr XA_RGB_BEST_MAP = (IntPtr)25;

	public IntPtr XA_RGB_BLUE_MAP = (IntPtr)26;

	public IntPtr XA_RGB_DEFAULT_MAP = (IntPtr)27;

	public IntPtr XA_RGB_GRAY_MAP = (IntPtr)28;

	public IntPtr XA_RGB_GREEN_MAP = (IntPtr)29;

	public IntPtr XA_RGB_RED_MAP = (IntPtr)30;

	public IntPtr XA_STRING = (IntPtr)31;

	public IntPtr XA_VISUALID = (IntPtr)32;

	public IntPtr XA_WINDOW = (IntPtr)33;

	public IntPtr XA_WM_COMMAND = (IntPtr)34;

	public IntPtr XA_WM_HINTS = (IntPtr)35;

	public IntPtr XA_WM_CLIENT_MACHINE = (IntPtr)36;

	public IntPtr XA_WM_ICON_NAME = (IntPtr)37;

	public IntPtr XA_WM_ICON_SIZE = (IntPtr)38;

	public IntPtr XA_WM_NAME = (IntPtr)39;

	public IntPtr XA_WM_NORMAL_HINTS = (IntPtr)40;

	public IntPtr XA_WM_SIZE_HINTS = (IntPtr)41;

	public IntPtr XA_WM_ZOOM_HINTS = (IntPtr)42;

	public IntPtr XA_MIN_SPACE = (IntPtr)43;

	public IntPtr XA_NORM_SPACE = (IntPtr)44;

	public IntPtr XA_MAX_SPACE = (IntPtr)45;

	public IntPtr XA_END_SPACE = (IntPtr)46;

	public IntPtr XA_SUPERSCRIPT_X = (IntPtr)47;

	public IntPtr XA_SUPERSCRIPT_Y = (IntPtr)48;

	public IntPtr XA_SUBSCRIPT_X = (IntPtr)49;

	public IntPtr XA_SUBSCRIPT_Y = (IntPtr)50;

	public IntPtr XA_UNDERLINE_POSITION = (IntPtr)51;

	public IntPtr XA_UNDERLINE_THICKNESS = (IntPtr)52;

	public IntPtr XA_STRIKEOUT_ASCENT = (IntPtr)53;

	public IntPtr XA_STRIKEOUT_DESCENT = (IntPtr)54;

	public IntPtr XA_ITALIC_ANGLE = (IntPtr)55;

	public IntPtr XA_X_HEIGHT = (IntPtr)56;

	public IntPtr XA_QUAD_WIDTH = (IntPtr)57;

	public IntPtr XA_WEIGHT = (IntPtr)58;

	public IntPtr XA_POINT_SIZE = (IntPtr)59;

	public IntPtr XA_RESOLUTION = (IntPtr)60;

	public IntPtr XA_COPYRIGHT = (IntPtr)61;

	public IntPtr XA_NOTICE = (IntPtr)62;

	public IntPtr XA_FONT_NAME = (IntPtr)63;

	public IntPtr XA_FAMILY_NAME = (IntPtr)64;

	public IntPtr XA_FULL_NAME = (IntPtr)65;

	public IntPtr XA_CAP_HEIGHT = (IntPtr)66;

	public IntPtr XA_WM_CLASS = (IntPtr)67;

	public IntPtr XA_WM_TRANSIENT_FOR = (IntPtr)68;

	public IntPtr EDID;

	public IntPtr WM_PROTOCOLS;

	public IntPtr WM_DELETE_WINDOW;

	public IntPtr WM_TAKE_FOCUS;

	public IntPtr _NET_SUPPORTED;

	public IntPtr _NET_CLIENT_LIST;

	public IntPtr _NET_NUMBER_OF_DESKTOPS;

	public IntPtr _NET_DESKTOP_GEOMETRY;

	public IntPtr _NET_DESKTOP_VIEWPORT;

	public IntPtr _NET_CURRENT_DESKTOP;

	public IntPtr _NET_DESKTOP_NAMES;

	public IntPtr _NET_ACTIVE_WINDOW;

	public IntPtr _NET_WORKAREA;

	public IntPtr _NET_SUPPORTING_WM_CHECK;

	public IntPtr _NET_VIRTUAL_ROOTS;

	public IntPtr _NET_DESKTOP_LAYOUT;

	public IntPtr _NET_SHOWING_DESKTOP;

	public IntPtr _NET_CLOSE_WINDOW;

	public IntPtr _NET_MOVERESIZE_WINDOW;

	public IntPtr _NET_WM_MOVERESIZE;

	public IntPtr _NET_RESTACK_WINDOW;

	public IntPtr _NET_REQUEST_FRAME_EXTENTS;

	public IntPtr _NET_WM_NAME;

	public IntPtr _NET_WM_VISIBLE_NAME;

	public IntPtr _NET_WM_ICON_NAME;

	public IntPtr _NET_WM_VISIBLE_ICON_NAME;

	public IntPtr _NET_WM_DESKTOP;

	public IntPtr _NET_WM_WINDOW_TYPE;

	public IntPtr _NET_WM_STATE;

	public IntPtr _NET_WM_ALLOWED_ACTIONS;

	public IntPtr _NET_WM_STRUT;

	public IntPtr _NET_WM_STRUT_PARTIAL;

	public IntPtr _NET_WM_ICON_GEOMETRY;

	public IntPtr _NET_WM_ICON;

	public IntPtr _NET_WM_PID;

	public IntPtr _NET_WM_HANDLED_ICONS;

	public IntPtr _NET_WM_USER_TIME;

	public IntPtr _NET_FRAME_EXTENTS;

	public IntPtr _NET_WM_PING;

	public IntPtr _NET_WM_SYNC_REQUEST;

	public IntPtr _NET_WM_SYNC_REQUEST_COUNTER;

	public IntPtr _NET_SYSTEM_TRAY_S;

	public IntPtr _NET_SYSTEM_TRAY_ORIENTATION;

	public IntPtr _NET_SYSTEM_TRAY_OPCODE;

	public IntPtr _NET_WM_STATE_MAXIMIZED_HORZ;

	public IntPtr _NET_WM_STATE_MAXIMIZED_VERT;

	public IntPtr _NET_WM_STATE_FULLSCREEN;

	public IntPtr _XEMBED;

	public IntPtr _XEMBED_INFO;

	public IntPtr _MOTIF_WM_HINTS;

	public IntPtr _NET_WM_STATE_SKIP_TASKBAR;

	public IntPtr _NET_WM_STATE_ABOVE;

	public IntPtr _NET_WM_STATE_MODAL;

	public IntPtr _NET_WM_STATE_HIDDEN;

	public IntPtr _NET_WM_CONTEXT_HELP;

	public IntPtr _NET_WM_WINDOW_OPACITY;

	public IntPtr _NET_WM_WINDOW_TYPE_DESKTOP;

	public IntPtr _NET_WM_WINDOW_TYPE_DOCK;

	public IntPtr _NET_WM_WINDOW_TYPE_TOOLBAR;

	public IntPtr _NET_WM_WINDOW_TYPE_MENU;

	public IntPtr _NET_WM_WINDOW_TYPE_UTILITY;

	public IntPtr _NET_WM_WINDOW_TYPE_SPLASH;

	public IntPtr _NET_WM_WINDOW_TYPE_DIALOG;

	public IntPtr _NET_WM_WINDOW_TYPE_NORMAL;

	public IntPtr CLIPBOARD;

	public IntPtr CLIPBOARD_MANAGER;

	public IntPtr SAVE_TARGETS;

	public IntPtr MULTIPLE;

	public IntPtr PRIMARY;

	public IntPtr OEMTEXT;

	public IntPtr UNICODETEXT;

	public IntPtr TARGETS;

	public IntPtr UTF8_STRING;

	public IntPtr UTF16_STRING;

	public IntPtr ATOM_PAIR;

	public IntPtr MANAGER;

	public IntPtr _KDE_NET_WM_BLUR_BEHIND_REGION;

	public IntPtr INCR;

	private readonly Dictionary<string, IntPtr> _namesToAtoms = new Dictionary<string, IntPtr>();

	private readonly Dictionary<IntPtr, string> _atomsToNames = new Dictionary<IntPtr, string>();

	public X11Atoms(IntPtr display)
	{
		_display = display;
		PopulateAtoms(display);
	}

	private void InitAtom(ref IntPtr field, string name, IntPtr value)
	{
		if (value != IntPtr.Zero)
		{
			field = value;
			_namesToAtoms[name] = value;
			_atomsToNames[value] = name;
		}
	}

	public IntPtr GetAtom(string name)
	{
		if (_namesToAtoms.TryGetValue(name, out var value))
		{
			return value;
		}
		IntPtr intPtr = XLib.XInternAtom(_display, name, only_if_exists: false);
		_namesToAtoms[name] = intPtr;
		_atomsToNames[intPtr] = name;
		return intPtr;
	}

	public string GetAtomName(IntPtr atom)
	{
		if (_atomsToNames.TryGetValue(atom, out var value))
		{
			return value;
		}
		string atomName = XLib.GetAtomName(_display, atom);
		if (atomName == null)
		{
			return null;
		}
		_atomsToNames[atom] = atomName;
		_namesToAtoms[atomName] = atom;
		return atomName;
	}

	private void PopulateAtoms(IntPtr display)
	{
		IntPtr[] array = new IntPtr[147];
		string[] array2 = new string[147]
		{
			"AnyPropertyType", "XA_PRIMARY", "XA_SECONDARY", "XA_ARC", "XA_ATOM", "XA_BITMAP", "XA_CARDINAL", "XA_COLORMAP", "XA_CURSOR", "XA_CUT_BUFFER0",
			"XA_CUT_BUFFER1", "XA_CUT_BUFFER2", "XA_CUT_BUFFER3", "XA_CUT_BUFFER4", "XA_CUT_BUFFER5", "XA_CUT_BUFFER6", "XA_CUT_BUFFER7", "XA_DRAWABLE", "XA_FONT", "XA_INTEGER",
			"XA_PIXMAP", "XA_POINT", "XA_RECTANGLE", "XA_RESOURCE_MANAGER", "XA_RGB_COLOR_MAP", "XA_RGB_BEST_MAP", "XA_RGB_BLUE_MAP", "XA_RGB_DEFAULT_MAP", "XA_RGB_GRAY_MAP", "XA_RGB_GREEN_MAP",
			"XA_RGB_RED_MAP", "XA_STRING", "XA_VISUALID", "XA_WINDOW", "XA_WM_COMMAND", "XA_WM_HINTS", "XA_WM_CLIENT_MACHINE", "XA_WM_ICON_NAME", "XA_WM_ICON_SIZE", "XA_WM_NAME",
			"XA_WM_NORMAL_HINTS", "XA_WM_SIZE_HINTS", "XA_WM_ZOOM_HINTS", "XA_MIN_SPACE", "XA_NORM_SPACE", "XA_MAX_SPACE", "XA_END_SPACE", "XA_SUPERSCRIPT_X", "XA_SUPERSCRIPT_Y", "XA_SUBSCRIPT_X",
			"XA_SUBSCRIPT_Y", "XA_UNDERLINE_POSITION", "XA_UNDERLINE_THICKNESS", "XA_STRIKEOUT_ASCENT", "XA_STRIKEOUT_DESCENT", "XA_ITALIC_ANGLE", "XA_X_HEIGHT", "XA_QUAD_WIDTH", "XA_WEIGHT", "XA_POINT_SIZE",
			"XA_RESOLUTION", "XA_COPYRIGHT", "XA_NOTICE", "XA_FONT_NAME", "XA_FAMILY_NAME", "XA_FULL_NAME", "XA_CAP_HEIGHT", "XA_WM_CLASS", "XA_WM_TRANSIENT_FOR", "EDID",
			"WM_PROTOCOLS", "WM_DELETE_WINDOW", "WM_TAKE_FOCUS", "_NET_SUPPORTED", "_NET_CLIENT_LIST", "_NET_NUMBER_OF_DESKTOPS", "_NET_DESKTOP_GEOMETRY", "_NET_DESKTOP_VIEWPORT", "_NET_CURRENT_DESKTOP", "_NET_DESKTOP_NAMES",
			"_NET_ACTIVE_WINDOW", "_NET_WORKAREA", "_NET_SUPPORTING_WM_CHECK", "_NET_VIRTUAL_ROOTS", "_NET_DESKTOP_LAYOUT", "_NET_SHOWING_DESKTOP", "_NET_CLOSE_WINDOW", "_NET_MOVERESIZE_WINDOW", "_NET_WM_MOVERESIZE", "_NET_RESTACK_WINDOW",
			"_NET_REQUEST_FRAME_EXTENTS", "_NET_WM_NAME", "_NET_WM_VISIBLE_NAME", "_NET_WM_ICON_NAME", "_NET_WM_VISIBLE_ICON_NAME", "_NET_WM_DESKTOP", "_NET_WM_WINDOW_TYPE", "_NET_WM_STATE", "_NET_WM_ALLOWED_ACTIONS", "_NET_WM_STRUT",
			"_NET_WM_STRUT_PARTIAL", "_NET_WM_ICON_GEOMETRY", "_NET_WM_ICON", "_NET_WM_PID", "_NET_WM_HANDLED_ICONS", "_NET_WM_USER_TIME", "_NET_FRAME_EXTENTS", "_NET_WM_PING", "_NET_WM_SYNC_REQUEST", "_NET_WM_SYNC_REQUEST_COUNTER",
			"_NET_SYSTEM_TRAY_S", "_NET_SYSTEM_TRAY_ORIENTATION", "_NET_SYSTEM_TRAY_OPCODE", "_NET_WM_STATE_MAXIMIZED_HORZ", "_NET_WM_STATE_MAXIMIZED_VERT", "_NET_WM_STATE_FULLSCREEN", "_XEMBED", "_XEMBED_INFO", "_MOTIF_WM_HINTS", "_NET_WM_STATE_SKIP_TASKBAR",
			"_NET_WM_STATE_ABOVE", "_NET_WM_STATE_MODAL", "_NET_WM_STATE_HIDDEN", "_NET_WM_CONTEXT_HELP", "_NET_WM_WINDOW_OPACITY", "_NET_WM_WINDOW_TYPE_DESKTOP", "_NET_WM_WINDOW_TYPE_DOCK", "_NET_WM_WINDOW_TYPE_TOOLBAR", "_NET_WM_WINDOW_TYPE_MENU", "_NET_WM_WINDOW_TYPE_UTILITY",
			"_NET_WM_WINDOW_TYPE_SPLASH", "_NET_WM_WINDOW_TYPE_DIALOG", "_NET_WM_WINDOW_TYPE_NORMAL", "CLIPBOARD", "CLIPBOARD_MANAGER", "SAVE_TARGETS", "MULTIPLE", "PRIMARY", "OEMTEXT", "UNICODETEXT",
			"TARGETS", "UTF8_STRING", "UTF16_STRING", "ATOM_PAIR", "MANAGER", "_KDE_NET_WM_BLUR_BEHIND_REGION", "INCR"
		};
		XLib.XInternAtoms(display, array2, array2.Length, only_if_exists: true, array);
		InitAtom(ref AnyPropertyType, "AnyPropertyType", array[0]);
		InitAtom(ref XA_PRIMARY, "XA_PRIMARY", array[1]);
		InitAtom(ref XA_SECONDARY, "XA_SECONDARY", array[2]);
		InitAtom(ref XA_ARC, "XA_ARC", array[3]);
		InitAtom(ref XA_ATOM, "XA_ATOM", array[4]);
		InitAtom(ref XA_BITMAP, "XA_BITMAP", array[5]);
		InitAtom(ref XA_CARDINAL, "XA_CARDINAL", array[6]);
		InitAtom(ref XA_COLORMAP, "XA_COLORMAP", array[7]);
		InitAtom(ref XA_CURSOR, "XA_CURSOR", array[8]);
		InitAtom(ref XA_CUT_BUFFER0, "XA_CUT_BUFFER0", array[9]);
		InitAtom(ref XA_CUT_BUFFER1, "XA_CUT_BUFFER1", array[10]);
		InitAtom(ref XA_CUT_BUFFER2, "XA_CUT_BUFFER2", array[11]);
		InitAtom(ref XA_CUT_BUFFER3, "XA_CUT_BUFFER3", array[12]);
		InitAtom(ref XA_CUT_BUFFER4, "XA_CUT_BUFFER4", array[13]);
		InitAtom(ref XA_CUT_BUFFER5, "XA_CUT_BUFFER5", array[14]);
		InitAtom(ref XA_CUT_BUFFER6, "XA_CUT_BUFFER6", array[15]);
		InitAtom(ref XA_CUT_BUFFER7, "XA_CUT_BUFFER7", array[16]);
		InitAtom(ref XA_DRAWABLE, "XA_DRAWABLE", array[17]);
		InitAtom(ref XA_FONT, "XA_FONT", array[18]);
		InitAtom(ref XA_INTEGER, "XA_INTEGER", array[19]);
		InitAtom(ref XA_PIXMAP, "XA_PIXMAP", array[20]);
		InitAtom(ref XA_POINT, "XA_POINT", array[21]);
		InitAtom(ref XA_RECTANGLE, "XA_RECTANGLE", array[22]);
		InitAtom(ref XA_RESOURCE_MANAGER, "XA_RESOURCE_MANAGER", array[23]);
		InitAtom(ref XA_RGB_COLOR_MAP, "XA_RGB_COLOR_MAP", array[24]);
		InitAtom(ref XA_RGB_BEST_MAP, "XA_RGB_BEST_MAP", array[25]);
		InitAtom(ref XA_RGB_BLUE_MAP, "XA_RGB_BLUE_MAP", array[26]);
		InitAtom(ref XA_RGB_DEFAULT_MAP, "XA_RGB_DEFAULT_MAP", array[27]);
		InitAtom(ref XA_RGB_GRAY_MAP, "XA_RGB_GRAY_MAP", array[28]);
		InitAtom(ref XA_RGB_GREEN_MAP, "XA_RGB_GREEN_MAP", array[29]);
		InitAtom(ref XA_RGB_RED_MAP, "XA_RGB_RED_MAP", array[30]);
		InitAtom(ref XA_STRING, "XA_STRING", array[31]);
		InitAtom(ref XA_VISUALID, "XA_VISUALID", array[32]);
		InitAtom(ref XA_WINDOW, "XA_WINDOW", array[33]);
		InitAtom(ref XA_WM_COMMAND, "XA_WM_COMMAND", array[34]);
		InitAtom(ref XA_WM_HINTS, "XA_WM_HINTS", array[35]);
		InitAtom(ref XA_WM_CLIENT_MACHINE, "XA_WM_CLIENT_MACHINE", array[36]);
		InitAtom(ref XA_WM_ICON_NAME, "XA_WM_ICON_NAME", array[37]);
		InitAtom(ref XA_WM_ICON_SIZE, "XA_WM_ICON_SIZE", array[38]);
		InitAtom(ref XA_WM_NAME, "XA_WM_NAME", array[39]);
		InitAtom(ref XA_WM_NORMAL_HINTS, "XA_WM_NORMAL_HINTS", array[40]);
		InitAtom(ref XA_WM_SIZE_HINTS, "XA_WM_SIZE_HINTS", array[41]);
		InitAtom(ref XA_WM_ZOOM_HINTS, "XA_WM_ZOOM_HINTS", array[42]);
		InitAtom(ref XA_MIN_SPACE, "XA_MIN_SPACE", array[43]);
		InitAtom(ref XA_NORM_SPACE, "XA_NORM_SPACE", array[44]);
		InitAtom(ref XA_MAX_SPACE, "XA_MAX_SPACE", array[45]);
		InitAtom(ref XA_END_SPACE, "XA_END_SPACE", array[46]);
		InitAtom(ref XA_SUPERSCRIPT_X, "XA_SUPERSCRIPT_X", array[47]);
		InitAtom(ref XA_SUPERSCRIPT_Y, "XA_SUPERSCRIPT_Y", array[48]);
		InitAtom(ref XA_SUBSCRIPT_X, "XA_SUBSCRIPT_X", array[49]);
		InitAtom(ref XA_SUBSCRIPT_Y, "XA_SUBSCRIPT_Y", array[50]);
		InitAtom(ref XA_UNDERLINE_POSITION, "XA_UNDERLINE_POSITION", array[51]);
		InitAtom(ref XA_UNDERLINE_THICKNESS, "XA_UNDERLINE_THICKNESS", array[52]);
		InitAtom(ref XA_STRIKEOUT_ASCENT, "XA_STRIKEOUT_ASCENT", array[53]);
		InitAtom(ref XA_STRIKEOUT_DESCENT, "XA_STRIKEOUT_DESCENT", array[54]);
		InitAtom(ref XA_ITALIC_ANGLE, "XA_ITALIC_ANGLE", array[55]);
		InitAtom(ref XA_X_HEIGHT, "XA_X_HEIGHT", array[56]);
		InitAtom(ref XA_QUAD_WIDTH, "XA_QUAD_WIDTH", array[57]);
		InitAtom(ref XA_WEIGHT, "XA_WEIGHT", array[58]);
		InitAtom(ref XA_POINT_SIZE, "XA_POINT_SIZE", array[59]);
		InitAtom(ref XA_RESOLUTION, "XA_RESOLUTION", array[60]);
		InitAtom(ref XA_COPYRIGHT, "XA_COPYRIGHT", array[61]);
		InitAtom(ref XA_NOTICE, "XA_NOTICE", array[62]);
		InitAtom(ref XA_FONT_NAME, "XA_FONT_NAME", array[63]);
		InitAtom(ref XA_FAMILY_NAME, "XA_FAMILY_NAME", array[64]);
		InitAtom(ref XA_FULL_NAME, "XA_FULL_NAME", array[65]);
		InitAtom(ref XA_CAP_HEIGHT, "XA_CAP_HEIGHT", array[66]);
		InitAtom(ref XA_WM_CLASS, "XA_WM_CLASS", array[67]);
		InitAtom(ref XA_WM_TRANSIENT_FOR, "XA_WM_TRANSIENT_FOR", array[68]);
		InitAtom(ref EDID, "EDID", array[69]);
		InitAtom(ref WM_PROTOCOLS, "WM_PROTOCOLS", array[70]);
		InitAtom(ref WM_DELETE_WINDOW, "WM_DELETE_WINDOW", array[71]);
		InitAtom(ref WM_TAKE_FOCUS, "WM_TAKE_FOCUS", array[72]);
		InitAtom(ref _NET_SUPPORTED, "_NET_SUPPORTED", array[73]);
		InitAtom(ref _NET_CLIENT_LIST, "_NET_CLIENT_LIST", array[74]);
		InitAtom(ref _NET_NUMBER_OF_DESKTOPS, "_NET_NUMBER_OF_DESKTOPS", array[75]);
		InitAtom(ref _NET_DESKTOP_GEOMETRY, "_NET_DESKTOP_GEOMETRY", array[76]);
		InitAtom(ref _NET_DESKTOP_VIEWPORT, "_NET_DESKTOP_VIEWPORT", array[77]);
		InitAtom(ref _NET_CURRENT_DESKTOP, "_NET_CURRENT_DESKTOP", array[78]);
		InitAtom(ref _NET_DESKTOP_NAMES, "_NET_DESKTOP_NAMES", array[79]);
		InitAtom(ref _NET_ACTIVE_WINDOW, "_NET_ACTIVE_WINDOW", array[80]);
		InitAtom(ref _NET_WORKAREA, "_NET_WORKAREA", array[81]);
		InitAtom(ref _NET_SUPPORTING_WM_CHECK, "_NET_SUPPORTING_WM_CHECK", array[82]);
		InitAtom(ref _NET_VIRTUAL_ROOTS, "_NET_VIRTUAL_ROOTS", array[83]);
		InitAtom(ref _NET_DESKTOP_LAYOUT, "_NET_DESKTOP_LAYOUT", array[84]);
		InitAtom(ref _NET_SHOWING_DESKTOP, "_NET_SHOWING_DESKTOP", array[85]);
		InitAtom(ref _NET_CLOSE_WINDOW, "_NET_CLOSE_WINDOW", array[86]);
		InitAtom(ref _NET_MOVERESIZE_WINDOW, "_NET_MOVERESIZE_WINDOW", array[87]);
		InitAtom(ref _NET_WM_MOVERESIZE, "_NET_WM_MOVERESIZE", array[88]);
		InitAtom(ref _NET_RESTACK_WINDOW, "_NET_RESTACK_WINDOW", array[89]);
		InitAtom(ref _NET_REQUEST_FRAME_EXTENTS, "_NET_REQUEST_FRAME_EXTENTS", array[90]);
		InitAtom(ref _NET_WM_NAME, "_NET_WM_NAME", array[91]);
		InitAtom(ref _NET_WM_VISIBLE_NAME, "_NET_WM_VISIBLE_NAME", array[92]);
		InitAtom(ref _NET_WM_ICON_NAME, "_NET_WM_ICON_NAME", array[93]);
		InitAtom(ref _NET_WM_VISIBLE_ICON_NAME, "_NET_WM_VISIBLE_ICON_NAME", array[94]);
		InitAtom(ref _NET_WM_DESKTOP, "_NET_WM_DESKTOP", array[95]);
		InitAtom(ref _NET_WM_WINDOW_TYPE, "_NET_WM_WINDOW_TYPE", array[96]);
		InitAtom(ref _NET_WM_STATE, "_NET_WM_STATE", array[97]);
		InitAtom(ref _NET_WM_ALLOWED_ACTIONS, "_NET_WM_ALLOWED_ACTIONS", array[98]);
		InitAtom(ref _NET_WM_STRUT, "_NET_WM_STRUT", array[99]);
		InitAtom(ref _NET_WM_STRUT_PARTIAL, "_NET_WM_STRUT_PARTIAL", array[100]);
		InitAtom(ref _NET_WM_ICON_GEOMETRY, "_NET_WM_ICON_GEOMETRY", array[101]);
		InitAtom(ref _NET_WM_ICON, "_NET_WM_ICON", array[102]);
		InitAtom(ref _NET_WM_PID, "_NET_WM_PID", array[103]);
		InitAtom(ref _NET_WM_HANDLED_ICONS, "_NET_WM_HANDLED_ICONS", array[104]);
		InitAtom(ref _NET_WM_USER_TIME, "_NET_WM_USER_TIME", array[105]);
		InitAtom(ref _NET_FRAME_EXTENTS, "_NET_FRAME_EXTENTS", array[106]);
		InitAtom(ref _NET_WM_PING, "_NET_WM_PING", array[107]);
		InitAtom(ref _NET_WM_SYNC_REQUEST, "_NET_WM_SYNC_REQUEST", array[108]);
		InitAtom(ref _NET_WM_SYNC_REQUEST_COUNTER, "_NET_WM_SYNC_REQUEST_COUNTER", array[109]);
		InitAtom(ref _NET_SYSTEM_TRAY_S, "_NET_SYSTEM_TRAY_S", array[110]);
		InitAtom(ref _NET_SYSTEM_TRAY_ORIENTATION, "_NET_SYSTEM_TRAY_ORIENTATION", array[111]);
		InitAtom(ref _NET_SYSTEM_TRAY_OPCODE, "_NET_SYSTEM_TRAY_OPCODE", array[112]);
		InitAtom(ref _NET_WM_STATE_MAXIMIZED_HORZ, "_NET_WM_STATE_MAXIMIZED_HORZ", array[113]);
		InitAtom(ref _NET_WM_STATE_MAXIMIZED_VERT, "_NET_WM_STATE_MAXIMIZED_VERT", array[114]);
		InitAtom(ref _NET_WM_STATE_FULLSCREEN, "_NET_WM_STATE_FULLSCREEN", array[115]);
		InitAtom(ref _XEMBED, "_XEMBED", array[116]);
		InitAtom(ref _XEMBED_INFO, "_XEMBED_INFO", array[117]);
		InitAtom(ref _MOTIF_WM_HINTS, "_MOTIF_WM_HINTS", array[118]);
		InitAtom(ref _NET_WM_STATE_SKIP_TASKBAR, "_NET_WM_STATE_SKIP_TASKBAR", array[119]);
		InitAtom(ref _NET_WM_STATE_ABOVE, "_NET_WM_STATE_ABOVE", array[120]);
		InitAtom(ref _NET_WM_STATE_MODAL, "_NET_WM_STATE_MODAL", array[121]);
		InitAtom(ref _NET_WM_STATE_HIDDEN, "_NET_WM_STATE_HIDDEN", array[122]);
		InitAtom(ref _NET_WM_CONTEXT_HELP, "_NET_WM_CONTEXT_HELP", array[123]);
		InitAtom(ref _NET_WM_WINDOW_OPACITY, "_NET_WM_WINDOW_OPACITY", array[124]);
		InitAtom(ref _NET_WM_WINDOW_TYPE_DESKTOP, "_NET_WM_WINDOW_TYPE_DESKTOP", array[125]);
		InitAtom(ref _NET_WM_WINDOW_TYPE_DOCK, "_NET_WM_WINDOW_TYPE_DOCK", array[126]);
		InitAtom(ref _NET_WM_WINDOW_TYPE_TOOLBAR, "_NET_WM_WINDOW_TYPE_TOOLBAR", array[127]);
		InitAtom(ref _NET_WM_WINDOW_TYPE_MENU, "_NET_WM_WINDOW_TYPE_MENU", array[128]);
		InitAtom(ref _NET_WM_WINDOW_TYPE_UTILITY, "_NET_WM_WINDOW_TYPE_UTILITY", array[129]);
		InitAtom(ref _NET_WM_WINDOW_TYPE_SPLASH, "_NET_WM_WINDOW_TYPE_SPLASH", array[130]);
		InitAtom(ref _NET_WM_WINDOW_TYPE_DIALOG, "_NET_WM_WINDOW_TYPE_DIALOG", array[131]);
		InitAtom(ref _NET_WM_WINDOW_TYPE_NORMAL, "_NET_WM_WINDOW_TYPE_NORMAL", array[132]);
		InitAtom(ref CLIPBOARD, "CLIPBOARD", array[133]);
		InitAtom(ref CLIPBOARD_MANAGER, "CLIPBOARD_MANAGER", array[134]);
		InitAtom(ref SAVE_TARGETS, "SAVE_TARGETS", array[135]);
		InitAtom(ref MULTIPLE, "MULTIPLE", array[136]);
		InitAtom(ref PRIMARY, "PRIMARY", array[137]);
		InitAtom(ref OEMTEXT, "OEMTEXT", array[138]);
		InitAtom(ref UNICODETEXT, "UNICODETEXT", array[139]);
		InitAtom(ref TARGETS, "TARGETS", array[140]);
		InitAtom(ref UTF8_STRING, "UTF8_STRING", array[141]);
		InitAtom(ref UTF16_STRING, "UTF16_STRING", array[142]);
		InitAtom(ref ATOM_PAIR, "ATOM_PAIR", array[143]);
		InitAtom(ref MANAGER, "MANAGER", array[144]);
		InitAtom(ref _KDE_NET_WM_BLUR_BEHIND_REGION, "_KDE_NET_WM_BLUR_BEHIND_REGION", array[145]);
		InitAtom(ref INCR, "INCR", array[146]);
	}
}
