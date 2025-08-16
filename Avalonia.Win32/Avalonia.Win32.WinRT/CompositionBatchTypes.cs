using System;

namespace Avalonia.Win32.WinRT;

[Flags]
internal enum CompositionBatchTypes
{
	None = 0,
	Animation = 1,
	Effect = 2,
	InfiniteAnimation = 4,
	AllAnimations = 5
}
