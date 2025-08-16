using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace Avalonia.Styling;

[NotClientImplementable]
public interface IStyle : IResourceNode
{
	IReadOnlyList<IStyle> Children { get; }
}
