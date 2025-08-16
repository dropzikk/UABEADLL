namespace Avalonia.X11;

internal enum Status
{
	Success = 0,
	BadRequest = 1,
	BadValue = 2,
	BadWindow = 3,
	BadPixmap = 4,
	BadAtom = 5,
	BadCursor = 6,
	BadFont = 7,
	BadMatch = 8,
	BadDrawable = 9,
	BadAccess = 10,
	BadAlloc = 11,
	BadColor = 12,
	BadGC = 13,
	BadIDChoice = 14,
	BadName = 15,
	BadLength = 16,
	BadImplementation = 17,
	FirstExtensionError = 128,
	LastExtensionError = 255
}
