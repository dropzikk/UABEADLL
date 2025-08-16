using System.Threading.Tasks;
using Avalonia.Controls;

namespace Avalonia.Diagnostics;

public interface IScreenshotHandler
{
	Task Take(Control control);
}
