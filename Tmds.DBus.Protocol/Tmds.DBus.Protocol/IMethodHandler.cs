using System.Threading.Tasks;

namespace Tmds.DBus.Protocol;

public interface IMethodHandler
{
	string Path { get; }

	ValueTask HandleMethodAsync(MethodContext context);

	bool RunMethodHandlerSynchronously(Message message);
}
