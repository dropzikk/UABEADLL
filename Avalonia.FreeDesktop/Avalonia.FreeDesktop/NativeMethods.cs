using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;

namespace Avalonia.FreeDesktop;

internal static class NativeMethods
{
	[DllImport("libc", SetLastError = true)]
	private static extern long readlink([MarshalAs(UnmanagedType.LPArray)] byte[] filename, [MarshalAs(UnmanagedType.LPArray)] byte[] buffer, long len);

	public static string ReadLink(string path)
	{
		int byteCount = Encoding.UTF8.GetByteCount(path);
		byte[] array = ArrayPool<byte>.Shared.Rent(byteCount + 1);
		byte[] array2 = ArrayPool<byte>.Shared.Rent(4097);
		try
		{
			Encoding.UTF8.GetBytes(path, 0, path.Length, array, 0);
			array[byteCount] = 0;
			long num = readlink(array, array2, 4097L);
			return Encoding.UTF8.GetString(array2, 0, (int)num);
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(array);
			ArrayPool<byte>.Shared.Return(array2);
		}
	}
}
