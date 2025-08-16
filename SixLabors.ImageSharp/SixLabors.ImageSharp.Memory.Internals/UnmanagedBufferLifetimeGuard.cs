namespace SixLabors.ImageSharp.Memory.Internals;

internal abstract class UnmanagedBufferLifetimeGuard : RefCountedMemoryLifetimeGuard
{
	public sealed class FreeHandle : UnmanagedBufferLifetimeGuard
	{
		public FreeHandle(UnmanagedMemoryHandle handle)
			: base(handle)
		{
		}

		protected override void Release()
		{
			base.Handle.Free();
		}
	}

	private UnmanagedMemoryHandle handle;

	public ref UnmanagedMemoryHandle Handle => ref handle;

	protected UnmanagedBufferLifetimeGuard(UnmanagedMemoryHandle handle)
	{
		this.handle = handle;
	}
}
