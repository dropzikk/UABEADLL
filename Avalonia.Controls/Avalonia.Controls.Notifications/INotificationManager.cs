using Avalonia.Metadata;

namespace Avalonia.Controls.Notifications;

[NotClientImplementable]
public interface INotificationManager
{
	void Show(INotification notification);
}
