using System;

namespace Avalonia.Controls.Notifications;

public class Notification : INotification
{
	public string? Title { get; private set; }

	public string? Message { get; private set; }

	public NotificationType Type { get; private set; }

	public TimeSpan Expiration { get; private set; }

	public Action? OnClick { get; private set; }

	public Action? OnClose { get; private set; }

	public Notification(string? title, string? message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Action? onClick = null, Action? onClose = null)
	{
		Title = title;
		Message = message;
		Type = type;
		Expiration = (expiration.HasValue ? expiration.Value : TimeSpan.FromSeconds(5.0));
		OnClick = onClick;
		OnClose = onClose;
	}
}
