using System;

namespace Avalonia.X11;

internal delegate int XErrorHandler(IntPtr DisplayHandle, ref XErrorEvent error_event);
