using System;
using System.Text;

namespace SixLabors.ImageSharp.Formats.Qoi;

internal static class QoiConstants
{
	private static readonly byte[] SMagic = Encoding.UTF8.GetBytes("qoif");

	public static ReadOnlySpan<byte> Magic => SMagic;

	public static string[] MimeTypes { get; } = new string[3] { "image/qoi", "image/x-qoi", "image/vnd.qoi" };

	public static string[] FileExtensions { get; } = new string[1] { "qoi" };
}
