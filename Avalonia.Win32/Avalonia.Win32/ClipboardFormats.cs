using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Avalonia.Input;
using Avalonia.Utilities;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32;

internal static class ClipboardFormats
{
	private class ClipboardFormat
	{
		public ushort Format { get; }

		public string Name { get; }

		public ushort[] Synthesized { get; }

		public ClipboardFormat(string name, ushort format, params ushort[] synthesized)
		{
			Format = format;
			Name = name;
			Synthesized = synthesized;
		}
	}

	private const int MAX_FORMAT_NAME_LENGTH = 260;

	private static readonly List<ClipboardFormat> s_formatList = new List<ClipboardFormat>
	{
		new ClipboardFormat(DataFormats.Text, 13, 1),
		new ClipboardFormat(DataFormats.Files, 15),
		new ClipboardFormat(DataFormats.FileNames, 15)
	};

	private static string? QueryFormatName(ushort format)
	{
		StringBuilder stringBuilder = StringBuilderCache.Acquire(260);
		if (UnmanagedMethods.GetClipboardFormatName(format, stringBuilder, stringBuilder.Capacity) > 0)
		{
			return StringBuilderCache.GetStringAndRelease(stringBuilder);
		}
		return null;
	}

	public static string GetFormat(ushort format)
	{
		lock (s_formatList)
		{
			ClipboardFormat clipboardFormat = s_formatList.FirstOrDefault((ClipboardFormat f) => f.Format == format || Array.IndexOf(f.Synthesized, format) >= 0);
			if (clipboardFormat == null)
			{
				string text = QueryFormatName(format);
				if (string.IsNullOrEmpty(text))
				{
					text = $"Unknown_Format_{format}";
				}
				clipboardFormat = new ClipboardFormat(text, format);
				s_formatList.Add(clipboardFormat);
			}
			return clipboardFormat.Name;
		}
	}

	public static ushort GetFormat(string format)
	{
		lock (s_formatList)
		{
			ClipboardFormat clipboardFormat = s_formatList.FirstOrDefault((ClipboardFormat f) => StringComparer.OrdinalIgnoreCase.Equals(f.Name, format));
			if (clipboardFormat == null)
			{
				int num = UnmanagedMethods.RegisterClipboardFormat(format);
				if (num == 0)
				{
					throw new Win32Exception();
				}
				clipboardFormat = new ClipboardFormat(format, (ushort)num);
				s_formatList.Add(clipboardFormat);
			}
			return clipboardFormat.Format;
		}
	}
}
