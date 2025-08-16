namespace AssetRipper.TextureDecoder.Rgb;

public interface IColor<T> where T : unmanaged
{
	T R { get; set; }

	T G { get; set; }

	T B { get; set; }

	T A { get; set; }

	void GetChannels(out T r, out T g, out T b, out T a);

	void SetChannels(T r, T g, T b, T a);
}
