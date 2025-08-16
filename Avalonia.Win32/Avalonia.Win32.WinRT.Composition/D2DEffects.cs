using System;

namespace Avalonia.Win32.WinRT.Composition;

internal class D2DEffects
{
	public static readonly Guid CLSID_D2D12DAffineTransform = new Guid(1789490309, 25428, 19708, 144, 140, 228, 167, 79, 98, 201, 108);

	public static readonly Guid CLSID_D2D13DPerspectiveTransform = new Guid(3263450379u, 15750, 18151, 133, 186, 82, 108, 146, 64, 243, 251);

	public static readonly Guid CLSID_D2D13DTransform = new Guid(3896933124u, 60513, 19338, 181, 222, 212, 215, 61, 235, 234, 90);

	public static readonly Guid CLSID_D2D1ArithmeticComposite = new Guid(4229239863u, 1178, 18308, 162, 74, 241, 196, 218, 242, 9, 135);

	public static readonly Guid CLSID_D2D1Atlas = new Guid(2436770788u, 64975, 20450, 165, 240, 36, 84, 241, 79, 244, 8);

	public static readonly Guid CLSID_D2D1BitmapSource = new Guid(1605812813u, 50909, 16945, 148, 4, 80, 244, 213, 195, 37, 45);

	public static readonly Guid CLSID_D2D1Blend = new Guid(2177218427u, 5112, 19677, 173, 32, 200, 144, 84, 122, 198, 93);

	public static readonly Guid CLSID_D2D1Border = new Guid(707611072, 19151, 17351, 140, 106, 124, 74, 39, 135, 77, 39);

	public static readonly Guid CLSID_D2D1Opacity = new Guid("811d79a4-de28-4454-8094-c64685f8bd4c");

	public static readonly Guid CLSID_D2D1Brightness = new Guid(2364181790u, 30640, 18822, 179, 185, 47, 12, 14, 174, 120, 135);

	public static readonly Guid CLSID_D2D1ColorManagement = new Guid(438850124u, 64982, 19108, 174, 143, 131, 126, 184, 38, 123, 55);

	public static readonly Guid CLSID_D2D1ColorMatrix = new Guid(2451506134u, 25628, 18399, 133, 45, 180, 187, 97, 83, 174, 17);

	public static readonly Guid CLSID_D2D1Composite = new Guid(1224515409u, 63148, 18673, 139, 88, 59, 40, 172, 70, 247, 109);

	public static readonly Guid CLSID_D2D1ConvolveMatrix = new Guid(1082100744, 21811, 17201, 163, 65, 35, 204, 56, 119, 132, 62);

	public static readonly Guid CLSID_D2D1Crop = new Guid(3795808528u, 3738, 17188, 175, 71, 106, 44, 12, 70, 243, 91);

	public static readonly Guid CLSID_D2D1DirectionalBlur = new Guid(390273446, 22761, 18866, 187, 99, 202, 242, 200, 17, 163, 219);

	public static readonly Guid CLSID_D2D1DiscreteTransfer = new Guid(2424729549u, 18574, 17739, 175, 6, 229, 4, 27, 102, 195, 108);

	public static readonly Guid CLSID_D2D1DisplacementMap = new Guid(3989078884u, 1047, 16657, 148, 80, 67, 132, 95, 169, 248, 144);

	public static readonly Guid CLSID_D2D1DistantDiffuse = new Guid(1048509794u, 41773, 18132, 168, 60, 82, 120, 136, 154, 201, 84);

	public static readonly Guid CLSID_D2D1DistantSpecular = new Guid(1116479205, 30648, 17488, 138, 181, 114, 33, 156, 33, 171, 218);

	public static readonly Guid CLSID_D2D1DpiCompensation = new Guid(1814480327, 13536, 18172, 156, 253, 229, 130, 55, 6, 226, 40);

	public static readonly Guid CLSID_D2D1Flood = new Guid(1640119328u, 44649, 19854, 148, 207, 80, 7, 141, 246, 56, 242);

	public static readonly Guid CLSID_D2D1GammaTransfer = new Guid(1083458756u, 50201, 16800, 176, 193, 140, 208, 192, 161, 142, 66);

	public static readonly Guid CLSID_D2D1GaussianBlur = new Guid(535522665, 12262, 19145, 140, 88, 29, 127, 147, 231, 166, 165);

	public static readonly Guid CLSID_D2D1Scale = new Guid(2645529449u, 14406, 19726, 164, 78, 12, 96, 121, 52, 165, 215);

	public static readonly Guid CLSID_D2D1Histogram = new Guid(2283648976u, 63470, 19789, 166, 210, 70, 151, 172, 198, 110, 232);

	public static readonly Guid CLSID_D2D1HueRotation = new Guid(256137452, 19250, 18715, 158, 133, 189, 115, 244, 77, 62, 182);

	public static readonly Guid CLSID_D2D1LinearTransfer = new Guid(2907162877u, 25583, 19148, 155, 81, 103, 151, 156, 3, 108, 6);

	public static readonly Guid CLSID_D2D1LuminanceToAlpha = new Guid(1092950711, 3051, 18168, 157, 167, 89, 233, 63, 204, 229, 222);

	public static readonly Guid CLSID_D2D1Morphology = new Guid(3940992013u, 25194, 19501, 191, 203, 57, 16, 1, 171, 226, 2);

	public static readonly Guid CLSID_D2D1OpacityMetadata = new Guid(1817378922, 17488, 16793, 170, 91, 173, 22, 86, 254, 206, 94);

	public static readonly Guid CLSID_D2D1PointDiffuse = new Guid(3118662595u, 49292, 20369, 139, 123, 56, 101, 107, 196, 140, 32);

	public static readonly Guid CLSID_D2D1PointSpecular = new Guid(163826214, 15074, 20233, 158, 188, 237, 56, 101, 213, 63, 34);

	public static readonly Guid CLSID_D2D1Premultiply = new Guid(116044825u, 57069, 16408, 128, 210, 62, 29, 71, 26, 222, 178);

	public static readonly Guid CLSID_D2D1Saturation = new Guid(1555225039, 12925, 17823, 160, 206, 64, 192, 178, 8, 107, 247);

	public static readonly Guid CLSID_D2D1Shadow = new Guid(3330188129u, 6243, 20073, 137, 219, 105, 93, 62, 154, 91, 107);

	public static readonly Guid CLSID_D2D1SpotDiffuse = new Guid(2173309189u, 31026, 17652, 170, 134, 8, 174, 123, 47, 44, 147);

	public static readonly Guid CLSID_D2D1SpotSpecular = new Guid(3987620382u, 30292, 18999, 157, 184, 113, 172, 193, 190, 179, 193);

	public static readonly Guid CLSID_D2D1TableTransfer = new Guid(1542985923, 24131, 18635, 182, 49, 134, 131, 150, 214, 161, 212);

	public static readonly Guid CLSID_D2D1Tile = new Guid(2960671032u, 15222, 19397, 177, 59, 15, 162, 173, 2, 101, 159);

	public static readonly Guid CLSID_D2D1Turbulence = new Guid(3475748526u, 34970, 19159, 186, 41, 162, 253, 115, 44, 159, 201);

	public static readonly Guid CLSID_D2D1UnPremultiply = new Guid(4221224073u, 44429, 16877, 153, 153, 187, 99, 71, 209, 16, 247);
}
