using System;
using Avalonia.Metadata;

namespace Avalonia.Controls;

[NotClientImplementable]
public interface IResourceHost : IResourceNode
{
	event EventHandler<ResourcesChangedEventArgs>? ResourcesChanged;

	void NotifyHostedResourcesChanged(ResourcesChangedEventArgs e);
}
