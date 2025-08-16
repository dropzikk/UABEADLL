using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Metadata;

namespace Avalonia.Animation;

public class CompositePageTransition : IPageTransition
{
	[Content]
	public List<IPageTransition> PageTransitions { get; set; } = new List<IPageTransition>();

	public Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
	{
		return Task.WhenAll(PageTransitions.Select((IPageTransition transition) => transition.Start(from, to, forward, cancellationToken)).ToArray());
	}
}
