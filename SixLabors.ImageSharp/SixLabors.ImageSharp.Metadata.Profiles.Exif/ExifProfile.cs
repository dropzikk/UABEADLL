using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

public sealed class ExifProfile : IDeepCloneable<ExifProfile>
{
	private readonly byte[]? data;

	private List<IExifValue>? values;

	private int thumbnailOffset;

	private int thumbnailLength;

	public ExifParts Parts { get; set; }

	public IReadOnlyList<ExifTag> InvalidTags { get; private set; }

	[MemberNotNull("values")]
	public IReadOnlyList<IExifValue> Values
	{
		[MemberNotNull("values")]
		get
		{
			InitializeValues();
			return values;
		}
	}

	public ExifProfile()
		: this((byte[]?)null)
	{
	}

	public ExifProfile(byte[]? data)
	{
		Parts = ExifParts.All;
		this.data = data;
		InvalidTags = Array.Empty<ExifTag>();
	}

	internal ExifProfile(List<IExifValue> values, IReadOnlyList<ExifTag> invalidTags)
	{
		Parts = ExifParts.All;
		this.values = values;
		InvalidTags = invalidTags;
	}

	private ExifProfile(ExifProfile other)
	{
		Guard.NotNull(other, "other");
		Parts = other.Parts;
		thumbnailLength = other.thumbnailLength;
		thumbnailOffset = other.thumbnailOffset;
		IReadOnlyList<ExifTag> invalidTags;
		if (other.InvalidTags.Count <= 0)
		{
			IReadOnlyList<ExifTag> readOnlyList = Array.Empty<ExifTag>();
			invalidTags = readOnlyList;
		}
		else
		{
			IReadOnlyList<ExifTag> readOnlyList = new List<ExifTag>(other.InvalidTags);
			invalidTags = readOnlyList;
		}
		InvalidTags = invalidTags;
		if (other.values != null)
		{
			values = new List<IExifValue>(other.Values.Count);
			foreach (IExifValue value in other.Values)
			{
				values.Add(value.DeepClone());
			}
		}
		if (other.data != null)
		{
			data = new byte[other.data.Length];
			other.data.AsSpan().CopyTo(data);
		}
	}

	public bool TryCreateThumbnail([NotNullWhen(true)] out Image? image)
	{
		if (TryCreateThumbnail(out Image<Rgba32> image2))
		{
			image = image2;
			return true;
		}
		image = null;
		return false;
	}

	public bool TryCreateThumbnail<TPixel>([NotNullWhen(true)] out Image<TPixel>? image) where TPixel : unmanaged, IPixel<TPixel>
	{
		InitializeValues();
		image = null;
		if (thumbnailOffset == 0 || thumbnailLength == 0)
		{
			return false;
		}
		if (data == null || data.Length < thumbnailOffset + thumbnailLength)
		{
			return false;
		}
		using MemoryStream stream = new MemoryStream(data, thumbnailOffset, thumbnailLength);
		image = Image.Load<TPixel>(stream);
		return true;
	}

	public bool TryGetValue<TValueType>(ExifTag<TValueType> tag, [NotNullWhen(true)] out IExifValue<TValueType>? exifValue)
	{
		IExifValue valueInternal = GetValueInternal(tag);
		if (valueInternal == null)
		{
			exifValue = null;
			return false;
		}
		exifValue = (IExifValue<TValueType>)valueInternal;
		return true;
	}

	public bool RemoveValue(ExifTag tag)
	{
		InitializeValues();
		for (int i = 0; i < values.Count; i++)
		{
			if (values[i].Tag == tag)
			{
				values.RemoveAt(i);
				return true;
			}
		}
		return false;
	}

	public void SetValue<TValueType>(ExifTag<TValueType> tag, TValueType value)
	{
		SetValueInternal(tag, value);
	}

	public byte[]? ToByteArray()
	{
		if (values == null)
		{
			return data;
		}
		if (values.Count == 0)
		{
			return Array.Empty<byte>();
		}
		return new ExifWriter(values, Parts).GetData();
	}

	public ExifProfile DeepClone()
	{
		return new ExifProfile(this);
	}

	internal IExifValue? GetValueInternal(ExifTag tag)
	{
		foreach (IExifValue value in Values)
		{
			if (value.Tag == tag)
			{
				return value;
			}
		}
		return null;
	}

	internal void SetValueInternal(ExifTag tag, object? value)
	{
		foreach (IExifValue value2 in Values)
		{
			if (value2.Tag == tag)
			{
				value2.TrySetValue(value);
				return;
			}
		}
		ExifValue exifValue = ExifValues.Create(tag);
		if ((object)exifValue == null)
		{
			throw new NotSupportedException($"Newly created value for tag {tag} is null.");
		}
		exifValue.TrySetValue(value);
		values.Add(exifValue);
	}

	internal void Sync(ImageMetadata metadata)
	{
		SyncResolution(ExifTag.XResolution, metadata.HorizontalResolution);
		SyncResolution(ExifTag.YResolution, metadata.VerticalResolution);
	}

	private void SyncResolution(ExifTag<Rational> tag, double resolution)
	{
		if (TryGetValue(tag, out IExifValue<Rational> exifValue))
		{
			if (exifValue.IsArray || exifValue.DataType != ExifDataType.Rational)
			{
				RemoveValue(exifValue.Tag);
			}
			Rational value = new Rational(resolution, bestPrecision: false);
			SetValue(tag, value);
		}
	}

	[MemberNotNull("values")]
	private void InitializeValues()
	{
		if (values != null)
		{
			return;
		}
		if (data == null)
		{
			values = new List<IExifValue>();
			return;
		}
		ExifReader exifReader = new ExifReader(data);
		values = exifReader.ReadValues();
		IReadOnlyList<ExifTag> invalidTags;
		if (exifReader.InvalidTags.Count <= 0)
		{
			IReadOnlyList<ExifTag> readOnlyList = Array.Empty<ExifTag>();
			invalidTags = readOnlyList;
		}
		else
		{
			IReadOnlyList<ExifTag> readOnlyList = new List<ExifTag>(exifReader.InvalidTags);
			invalidTags = readOnlyList;
		}
		InvalidTags = invalidTags;
		thumbnailOffset = (int)exifReader.ThumbnailOffset;
		thumbnailLength = (int)exifReader.ThumbnailLength;
	}
}
