using System.Collections.Generic;
using Avalonia.Controls;

namespace Avalonia.Diagnostics;

internal interface IDevToolsTopLevelGroup
{
	IReadOnlyList<TopLevel> Items { get; }
}
