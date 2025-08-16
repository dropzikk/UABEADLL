using System;
using Avalonia.Media;
using Avalonia.Metadata;

namespace Avalonia.Controls.Platform;

[Unstable]
[NotClientImplementable]
public interface IInsetsManager
{
	bool? IsSystemBarVisible { get; set; }

	bool DisplayEdgeToEdge { get; set; }

	Thickness SafeAreaPadding { get; }

	Color? SystemBarColor { get; set; }

	event EventHandler<SafeAreaChangedArgs>? SafeAreaChanged;
}
