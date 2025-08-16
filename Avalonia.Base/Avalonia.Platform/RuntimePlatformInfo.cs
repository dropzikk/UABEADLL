using Avalonia.Metadata;

namespace Avalonia.Platform;

[PrivateApi]
public record struct RuntimePlatformInfo
{
	public FormFactorType FormFactor
	{
		get
		{
			if (!IsDesktop)
			{
				if (!IsMobile)
				{
					return FormFactorType.Unknown;
				}
				return FormFactorType.Mobile;
			}
			return FormFactorType.Desktop;
		}
	}

	public bool IsDesktop { get; set; }

	public bool IsMobile { get; set; }
}
