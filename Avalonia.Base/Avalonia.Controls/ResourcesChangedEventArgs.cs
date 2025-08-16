using System;

namespace Avalonia.Controls;

public class ResourcesChangedEventArgs : EventArgs
{
	public new static readonly ResourcesChangedEventArgs Empty = new ResourcesChangedEventArgs();
}
