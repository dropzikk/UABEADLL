using Avalonia.Input.Raw;

namespace Avalonia.Input;

public record struct PointerPointProperties
{
	public bool IsLeftButtonPressed { get; }

	public bool IsMiddleButtonPressed { get; }

	public bool IsRightButtonPressed { get; }

	public bool IsXButton1Pressed { get; }

	public bool IsXButton2Pressed { get; }

	public bool IsBarrelButtonPressed { get; }

	public bool IsEraser { get; }

	public bool IsInverted { get; }

	public float Twist { get; }

	public float Pressure { get; }

	public float XTilt { get; }

	public float YTilt { get; }

	public PointerUpdateKind PointerUpdateKind { get; }

	public static PointerPointProperties None { get; } = new PointerPointProperties();

	public PointerPointProperties()
	{
		IsLeftButtonPressed = false;
		IsMiddleButtonPressed = false;
		IsRightButtonPressed = false;
		IsXButton1Pressed = false;
		IsXButton2Pressed = false;
		IsBarrelButtonPressed = false;
		IsEraser = false;
		IsInverted = false;
		Twist = 0f;
		Pressure = 0.5f;
		XTilt = 0f;
		YTilt = 0f;
		PointerUpdateKind = PointerUpdateKind.LeftButtonPressed;
	}

	public PointerPointProperties(RawInputModifiers modifiers, PointerUpdateKind kind)
	{
		IsLeftButtonPressed = false;
		IsMiddleButtonPressed = false;
		IsRightButtonPressed = false;
		IsXButton1Pressed = false;
		IsXButton2Pressed = false;
		IsBarrelButtonPressed = false;
		IsEraser = false;
		IsInverted = false;
		Twist = 0f;
		Pressure = 0.5f;
		XTilt = 0f;
		YTilt = 0f;
		PointerUpdateKind = PointerUpdateKind.LeftButtonPressed;
		PointerUpdateKind = kind;
		IsLeftButtonPressed = modifiers.HasAllFlags(RawInputModifiers.LeftMouseButton);
		IsMiddleButtonPressed = modifiers.HasAllFlags(RawInputModifiers.MiddleMouseButton);
		IsRightButtonPressed = modifiers.HasAllFlags(RawInputModifiers.RightMouseButton);
		IsXButton1Pressed = modifiers.HasAllFlags(RawInputModifiers.XButton1MouseButton);
		IsXButton2Pressed = modifiers.HasAllFlags(RawInputModifiers.XButton2MouseButton);
		IsInverted = modifiers.HasAllFlags(RawInputModifiers.PenInverted);
		IsEraser = modifiers.HasAllFlags(RawInputModifiers.PenEraser);
		IsBarrelButtonPressed = modifiers.HasAllFlags(RawInputModifiers.PenBarrelButton);
		if (kind == PointerUpdateKind.LeftButtonPressed)
		{
			IsLeftButtonPressed = true;
		}
		if (kind == PointerUpdateKind.LeftButtonReleased)
		{
			IsLeftButtonPressed = false;
		}
		if (kind == PointerUpdateKind.MiddleButtonPressed)
		{
			IsMiddleButtonPressed = true;
		}
		if (kind == PointerUpdateKind.MiddleButtonReleased)
		{
			IsMiddleButtonPressed = false;
		}
		if (kind == PointerUpdateKind.RightButtonPressed)
		{
			IsRightButtonPressed = true;
		}
		if (kind == PointerUpdateKind.RightButtonReleased)
		{
			IsRightButtonPressed = false;
		}
		if (kind == PointerUpdateKind.XButton1Pressed)
		{
			IsXButton1Pressed = true;
		}
		if (kind == PointerUpdateKind.XButton1Released)
		{
			IsXButton1Pressed = false;
		}
		if (kind == PointerUpdateKind.XButton2Pressed)
		{
			IsXButton2Pressed = true;
		}
		if (kind == PointerUpdateKind.XButton2Released)
		{
			IsXButton2Pressed = false;
		}
	}

	public PointerPointProperties(RawInputModifiers modifiers, PointerUpdateKind kind, float twist, float pressure, float xTilt, float yTilt)
		: this(modifiers, kind)
	{
		Twist = twist;
		Pressure = pressure;
		XTilt = xTilt;
		YTilt = yTilt;
	}

	internal PointerPointProperties(PointerPointProperties basedOn, RawPointerPoint rawPoint)
	{
		IsLeftButtonPressed = false;
		IsMiddleButtonPressed = false;
		IsRightButtonPressed = false;
		IsXButton1Pressed = false;
		IsXButton2Pressed = false;
		IsBarrelButtonPressed = false;
		IsEraser = false;
		IsInverted = false;
		Twist = 0f;
		Pressure = 0.5f;
		XTilt = 0f;
		YTilt = 0f;
		PointerUpdateKind = PointerUpdateKind.LeftButtonPressed;
		IsLeftButtonPressed = basedOn.IsLeftButtonPressed;
		IsMiddleButtonPressed = basedOn.IsMiddleButtonPressed;
		IsRightButtonPressed = basedOn.IsRightButtonPressed;
		IsXButton1Pressed = basedOn.IsXButton1Pressed;
		IsXButton2Pressed = basedOn.IsXButton2Pressed;
		IsInverted = basedOn.IsInverted;
		IsEraser = basedOn.IsEraser;
		IsBarrelButtonPressed = basedOn.IsBarrelButtonPressed;
		Twist = rawPoint.Twist;
		Pressure = rawPoint.Pressure;
		XTilt = rawPoint.XTilt;
		YTilt = rawPoint.YTilt;
	}
}
