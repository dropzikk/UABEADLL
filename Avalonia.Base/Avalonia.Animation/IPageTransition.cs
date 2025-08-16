using System.Threading;
using System.Threading.Tasks;

namespace Avalonia.Animation;

public interface IPageTransition
{
	Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken);
}
