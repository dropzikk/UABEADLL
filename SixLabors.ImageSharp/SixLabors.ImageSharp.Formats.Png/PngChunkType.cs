namespace SixLabors.ImageSharp.Formats.Png;

internal enum PngChunkType : uint
{
	Data = 1229209940u,
	End = 1229278788u,
	Header = 1229472850u,
	Palette = 1347179589u,
	Exif = 1700284774u,
	Gamma = 1732332865u,
	Physical = 1883789683u,
	Text = 1950701684u,
	CompressedText = 2052348020u,
	InternationalText = 1767135348u,
	Transparency = 1951551059u,
	Time = 1950960965u,
	Background = 1649100612u,
	EmbeddedColorProfile = 1766015824u,
	SignificantBits = 1933723988u,
	StandardRgbColourSpace = 1934772034u,
	Histogram = 1749635924u,
	SuggestedPalette = 1934642260u,
	Chroma = 1665684045u,
	Cicp = 1665745744u,
	AnimationControl = 1633899596u,
	FrameControl = 1717785676u,
	FrameData = 1717846356u,
	ProprietaryApple = 1130840649u
}
