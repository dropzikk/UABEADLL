using System.IO;

namespace SixLabors.ImageSharp.IO;

internal sealed class LocalFileSystem : IFileSystem
{
	public Stream OpenRead(string path)
	{
		return File.OpenRead(path);
	}

	public Stream OpenReadAsynchronous(string path)
	{
		return File.Open(path, new FileStreamOptions
		{
			Mode = FileMode.Open,
			Access = FileAccess.Read,
			Share = FileShare.Read,
			Options = FileOptions.Asynchronous
		});
	}

	public Stream Create(string path)
	{
		return File.Create(path);
	}

	public Stream CreateAsynchronous(string path)
	{
		return File.Open(path, new FileStreamOptions
		{
			Mode = FileMode.Create,
			Access = FileAccess.ReadWrite,
			Share = FileShare.None,
			Options = FileOptions.Asynchronous
		});
	}
}
