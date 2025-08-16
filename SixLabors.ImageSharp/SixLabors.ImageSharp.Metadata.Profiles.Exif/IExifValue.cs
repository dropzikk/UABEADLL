namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

public interface IExifValue : IDeepCloneable<IExifValue>
{
	ExifDataType DataType { get; }

	bool IsArray { get; }

	ExifTag Tag { get; }

	object? GetValue();

	bool TrySetValue(object? value);
}
public interface IExifValue<TValueType> : IExifValue, IDeepCloneable<IExifValue>
{
	TValueType? Value { get; set; }
}
