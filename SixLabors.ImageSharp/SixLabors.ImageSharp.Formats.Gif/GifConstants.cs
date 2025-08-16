using System;
using System.Collections.Generic;
using System.Text;

namespace SixLabors.ImageSharp.Formats.Gif;

internal static class GifConstants
{
	public const string FileType = "GIF";

	public const string FileVersion = "89a";

	public const byte ExtensionIntroducer = 33;

	public const byte GraphicControlLabel = 249;

	public const byte ApplicationExtensionLabel = byte.MaxValue;

	public const byte ApplicationBlockSize = 11;

	public const string NetscapeApplicationIdentification = "NETSCAPE2.0";

	public const byte NetscapeLoopingSubBlockSize = 3;

	public const byte CommentLabel = 254;

	public const int MaxCommentSubBlockLength = 255;

	public const byte ImageDescriptorLabel = 44;

	public const byte PlainTextLabel = 1;

	public const byte ImageLabel = 44;

	public const byte Terminator = 0;

	public const byte EndIntroducer = 59;

	public static readonly Encoding Encoding = System.Text.Encoding.ASCII;

	public static readonly IEnumerable<string> MimeTypes = new string[1] { "image/gif" };

	public static readonly IEnumerable<string> FileExtensions = new string[1] { "gif" };

	internal static ReadOnlySpan<byte> MagicNumber => "GIF89a"u8;

	internal static ReadOnlySpan<byte> NetscapeApplicationIdentificationBytes => "NETSCAPE2.0"u8;

	internal static ReadOnlySpan<byte> XmpApplicationIdentificationBytes => "XMP DataXMP"u8;
}
