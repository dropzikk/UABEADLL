using System;
using Avalonia.Metadata;

namespace Avalonia.Controls.Notifications;

[NotClientImplementable]
public interface INotification
{
	string? Title { get; }

	string? Message { get; }

	NotificationType Type { get; }

	TimeSpan Expiration { get; }

	Action? OnClick { get; }

	Action? OnClose { get; }
}
