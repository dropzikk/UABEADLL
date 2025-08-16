using System.Threading.Tasks;

namespace Avalonia.Platform;

public interface IPlatformBehaviorInhibition
{
	Task SetInhibitAppSleep(bool inhibitAppSleep, string reason);
}
