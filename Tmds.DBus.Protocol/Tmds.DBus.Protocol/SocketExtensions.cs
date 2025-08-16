using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Tmds.DBus.Protocol;

internal static class SocketExtensions
{
	private struct msghdr
	{
		public IntPtr msg_name;

		public uint msg_namelen;

		public unsafe IOVector* msg_iov;

		public UIntPtr msg_iovlen;

		public unsafe void* msg_control;

		public UIntPtr msg_controllen;

		public int msg_flags;
	}

	private struct IOVector
	{
		public unsafe void* Base;

		public UIntPtr Length;
	}

	private struct cmsghdr
	{
		public UIntPtr cmsg_len;

		public int cmsg_level;

		public int cmsg_type;
	}

	private struct cmsg_fd
	{
		public cmsghdr hdr;

		public unsafe fixed int fds[64];
	}

	private const int SOL_SOCKET = 1;

	private const int EINTR = 4;

	private const int EBADF = 9;

	private static readonly int EAGAIN = (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? 35 : 11);

	private const int SCM_RIGHTS = 1;

	public static ValueTask<int> ReceiveAsync(this Socket socket, Memory<byte> memory, UnixFdCollection? fdCollection)
	{
		if (fdCollection == null)
		{
			return socket.ReceiveAsync(memory, SocketFlags.None, default(CancellationToken));
		}
		return socket.ReceiveWithHandlesAsync(memory, fdCollection);
	}

	private static async ValueTask<int> ReceiveWithHandlesAsync(this Socket socket, Memory<byte> memory, UnixFdCollection fdCollection)
	{
		int lastWin32Error;
		do
		{
			await socket.ReceiveAsync(default(Memory<byte>), SocketFlags.None, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
			int num = recvmsg(socket, memory, fdCollection);
			if (num >= 0)
			{
				return num;
			}
			lastWin32Error = Marshal.GetLastWin32Error();
		}
		while (lastWin32Error == EAGAIN || lastWin32Error == 4);
		throw new SocketException(lastWin32Error);
	}

	public static ValueTask SendAsync(this Socket socket, ReadOnlyMemory<byte> buffer, IReadOnlyList<SafeHandle>? handles)
	{
		if (handles == null || handles.Count == 0)
		{
			return socket.SendAsync(buffer);
		}
		return socket.SendAsyncWithHandlesAsync(buffer, handles);
	}

	private static async ValueTask SendAsync(this Socket socket, ReadOnlyMemory<byte> buffer)
	{
		while (buffer.Length > 0)
		{
			buffer = buffer.Slice(await socket.SendAsync(buffer, SocketFlags.None, default(CancellationToken)));
		}
	}

	private static ValueTask SendAsyncWithHandlesAsync(this Socket socket, ReadOnlyMemory<byte> buffer, IReadOnlyList<SafeHandle> handles)
	{
		socket.Blocking = false;
		int lastWin32Error;
		do
		{
			int num = sendmsg(socket, buffer, handles);
			if (num > 0)
			{
				if (buffer.Length == num)
				{
					return default(ValueTask);
				}
				return socket.SendAsync(buffer.Slice(num));
			}
			lastWin32Error = Marshal.GetLastWin32Error();
		}
		while (lastWin32Error == 4);
		return new ValueTask(Task.FromException((Exception)(object)new SocketException(lastWin32Error)));
	}

	private unsafe static int sendmsg(Socket socket, ReadOnlyMemory<byte> buffer, IReadOnlyList<SafeHandle> handles)
	{
		fixed (byte* @base = buffer.Span)
		{
			IOVector* ptr = stackalloc IOVector[1];
			ptr->Base = @base;
			ptr->Length = (UIntPtr)(ulong)buffer.Length;
			msghdr msghdr = default(msghdr);
			msghdr.msg_iov = ptr;
			msghdr.msg_iovlen = (UIntPtr)1uL;
			cmsg_fd cmsg_fd = default(cmsg_fd);
			int num = sizeof(cmsghdr) + 4 * handles.Count;
			msghdr.msg_control = &cmsg_fd;
			msghdr.msg_controllen = (UIntPtr)(ulong)num;
			cmsg_fd.hdr.cmsg_len = (UIntPtr)(ulong)num;
			cmsg_fd.hdr.cmsg_level = 1;
			cmsg_fd.hdr.cmsg_type = 1;
			SafeHandle safeHandle = socket.GetSafeHandle();
			int num2 = 0;
			bool success = false;
			try
			{
				safeHandle.DangerousAddRef(ref success);
				int i = 0;
				int num3 = 0;
				for (; i < handles.Count; i++)
				{
					bool success2 = false;
					SafeHandle safeHandle2 = handles[i];
					safeHandle2.DangerousAddRef(ref success2);
					num2++;
					cmsg_fd.fds[num3++] = safeHandle2.DangerousGetHandle().ToInt32();
				}
				return (int)sendmsg(safeHandle.DangerousGetHandle().ToInt32(), new IntPtr(&msghdr), 0);
			}
			finally
			{
				for (int j = 0; j < num2; j++)
				{
					handles[j].DangerousRelease();
				}
				if (success)
				{
					safeHandle.DangerousRelease();
				}
			}
		}
	}

	private unsafe static int recvmsg(Socket socket, Memory<byte> buffer, UnixFdCollection handles)
	{
		fixed (byte* @base = buffer.Span)
		{
			IOVector iOVector = default(IOVector);
			iOVector.Base = @base;
			iOVector.Length = (UIntPtr)(ulong)buffer.Length;
			msghdr msghdr = default(msghdr);
			msghdr.msg_iov = &iOVector;
			msghdr.msg_iovlen = (UIntPtr)1uL;
			cmsg_fd cmsg_fd = default(cmsg_fd);
			msghdr.msg_control = &cmsg_fd;
			msghdr.msg_controllen = (UIntPtr)(ulong)sizeof(cmsg_fd);
			SafeHandle safeHandle = socket.GetSafeHandle();
			bool success = false;
			try
			{
				safeHandle.DangerousAddRef(ref success);
				int num = (int)recvmsg(safeHandle.DangerousGetHandle().ToInt32(), new IntPtr(&msghdr), 0);
				if (num >= 0 && cmsg_fd.hdr.cmsg_level == 1 && cmsg_fd.hdr.cmsg_type == 1)
				{
					int num2 = ((int)(uint)cmsg_fd.hdr.cmsg_len - sizeof(cmsghdr)) / 4;
					for (int i = 0; i < num2; i++)
					{
						handles.AddHandle(new IntPtr(cmsg_fd.fds[i]));
					}
				}
				return num;
			}
			finally
			{
				if (success)
				{
					safeHandle.DangerousRelease();
				}
			}
		}
	}

	[DllImport("libc", SetLastError = true)]
	public static extern IntPtr sendmsg(int sockfd, IntPtr msg, int flags);

	[DllImport("libc", SetLastError = true)]
	public static extern IntPtr recvmsg(int sockfd, IntPtr msg, int flags);
}
