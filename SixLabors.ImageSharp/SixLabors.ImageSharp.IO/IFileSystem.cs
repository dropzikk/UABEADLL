using System.IO;

namespace SixLabors.ImageSharp.IO;

internal interface IFileSystem
{
	Stream OpenRead(string path);

	Stream OpenReadAsynchronous(string path);

	Stream Create(string path);

	Stream CreateAsynchronous(string path);
}
