namespace SixLabors.ImageSharp.Memory.Internals;

internal interface IRefCounted
{
	void AddRef();

	void ReleaseRef();
}
