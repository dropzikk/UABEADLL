using System;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace Avalonia.LogicalTree;

[NotClientImplementable]
public interface ILogical
{
	bool IsAttachedToLogicalTree { get; }

	ILogical? LogicalParent { get; }

	IAvaloniaReadOnlyList<ILogical> LogicalChildren { get; }

	event EventHandler<LogicalTreeAttachmentEventArgs>? AttachedToLogicalTree;

	event EventHandler<LogicalTreeAttachmentEventArgs>? DetachedFromLogicalTree;

	void NotifyAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e);

	void NotifyDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e);

	void NotifyResourcesChanged(ResourcesChangedEventArgs e);
}
