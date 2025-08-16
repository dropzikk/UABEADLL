using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp;

public delegate void PixelAccessorAction<TPixel>(PixelAccessor<TPixel> pixelAccessor) where TPixel : unmanaged, IPixel<TPixel>;
public delegate void PixelAccessorAction<TPixel1, TPixel2>(PixelAccessor<TPixel1> pixelAccessor1, PixelAccessor<TPixel2> pixelAccessor2) where TPixel1 : unmanaged, IPixel<TPixel1> where TPixel2 : unmanaged, IPixel<TPixel2>;
public delegate void PixelAccessorAction<TPixel1, TPixel2, TPixel3>(PixelAccessor<TPixel1> pixelAccessor1, PixelAccessor<TPixel2> pixelAccessor2, PixelAccessor<TPixel3> pixelAccessor3) where TPixel1 : unmanaged, IPixel<TPixel1> where TPixel2 : unmanaged, IPixel<TPixel2> where TPixel3 : unmanaged, IPixel<TPixel3>;
