using Avalonia.Metadata;

namespace Avalonia.Controls.Notifications;

[NotClientImplementable]
public interface IManagedNotificationManager : INotificationManager
{
	void Show(object content);
}
