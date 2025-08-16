using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace Avalonia.Diagnostics.Screenshots;

public abstract class BaseRenderToStreamHandler : IScreenshotHandler
{
	protected abstract Task<Stream?> GetStream(Control control);

	public async Task Take(Control control)
	{
		await using Stream output = await GetStream(control);
		if (output != null)
		{
			control.RenderTo(output);
			await output.FlushAsync();
		}
	}
}
