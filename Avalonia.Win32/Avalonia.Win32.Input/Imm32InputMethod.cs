using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Input.TextInput;
using Avalonia.Threading;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32.Input;

internal class Imm32InputMethod : ITextInputMethodImpl
{
	private IntPtr _currentHimc;

	private WindowImpl? _parent;

	private Imm32CaretManager _caretManager;

	private ushort _langId;

	private const int CaretMargin = 1;

	private bool _ignoreComposition;

	public IntPtr Hwnd { get; private set; }

	public TextInputMethodClient? Client { get; private set; }

	[MemberNotNullWhen(true, "Client")]
	public bool IsActive
	{
		[MemberNotNullWhen(true, "Client")]
		get
		{
			return Client != null;
		}
	}

	public bool IsComposing { get; set; }

	public bool ShowCompositionWindow => false;

	public string? Composition { get; internal set; }

	public static Imm32InputMethod Current { get; } = new Imm32InputMethod();

	public void CreateCaret()
	{
		_caretManager.TryCreate(Hwnd);
	}

	public void EnableImm()
	{
		IntPtr intPtr = UnmanagedMethods.ImmGetContext(Hwnd);
		if (intPtr == IntPtr.Zero)
		{
			intPtr = UnmanagedMethods.ImmCreateContext();
		}
		if (intPtr != _currentHimc)
		{
			if (_currentHimc != IntPtr.Zero)
			{
				DisableImm();
			}
			UnmanagedMethods.ImmAssociateContext(Hwnd, intPtr);
			UnmanagedMethods.ImmReleaseContext(Hwnd, intPtr);
			_currentHimc = intPtr;
			_caretManager.TryCreate(Hwnd);
		}
	}

	public void DisableImm()
	{
		_caretManager.TryDestroy();
		Reset();
		UnmanagedMethods.ImmAssociateContext(Hwnd, IntPtr.Zero);
		_caretManager.TryDestroy();
		_currentHimc = IntPtr.Zero;
	}

	public void SetLanguageAndWindow(WindowImpl parent, IntPtr hwnd, IntPtr HKL)
	{
		Hwnd = hwnd;
		_parent = parent;
		_langId = UnmanagedMethods.PRIMARYLANGID(UnmanagedMethods.LGID(HKL));
		_parent = parent;
		ushort num = UnmanagedMethods.PRIMARYLANGID(UnmanagedMethods.LGID(HKL));
		if (IsActive && num != _langId)
		{
			DisableImm();
			EnableImm();
		}
		_langId = num;
	}

	public void ClearLanguageAndWindow()
	{
		DisableImm();
		Hwnd = IntPtr.Zero;
		_parent = null;
		Client = null;
		_langId = 0;
		IsComposing = false;
	}

	public void Reset()
	{
		Dispatcher.UIThread.Post(delegate
		{
			IntPtr intPtr = UnmanagedMethods.ImmGetContext(Hwnd);
			if (intPtr != IntPtr.Zero)
			{
				_ignoreComposition = true;
				if (_parent != null)
				{
					_parent._ignoreWmChar = true;
				}
				UnmanagedMethods.ImmNotifyIME(intPtr, 21, 1, 0);
				UnmanagedMethods.ImmReleaseContext(Hwnd, intPtr);
				IsComposing = false;
				Composition = null;
			}
		});
	}

	public void SetClient(TextInputMethodClient? client)
	{
		if (Client != null)
		{
			Composition = null;
			Client.SetPreeditText(null);
		}
		Client = client;
		Dispatcher.UIThread.Post(delegate
		{
			if (IsActive)
			{
				EnableImm();
			}
			else
			{
				DisableImm();
			}
		});
	}

	public void SetCursorRect(Rect rect)
	{
		if (!(UnmanagedMethods.GetActiveWindow() == Hwnd))
		{
			return;
		}
		Dispatcher.UIThread.Post(delegate
		{
			IntPtr intPtr = UnmanagedMethods.ImmGetContext(Hwnd);
			if (!(intPtr == IntPtr.Zero))
			{
				MoveImeWindow(rect, intPtr);
				UnmanagedMethods.ImmReleaseContext(Hwnd, intPtr);
			}
		});
	}

	private void MoveImeWindow(Rect rect, IntPtr himc)
	{
		Point topLeft = rect.TopLeft;
		Point bottomRight = rect.BottomRight;
		double num = _parent?.DesktopScaling ?? 1.0;
		int num2 = (int)(topLeft.X * num);
		int num3 = (int)(topLeft.Y * num);
		int num4 = (int)(bottomRight.X * num);
		int num5 = (int)(bottomRight.Y * num);
		int num6 = num2;
		int num7 = num3;
		int num8 = num4;
		int num9 = num5;
		if (!ShowCompositionWindow && _langId == 4)
		{
			UnmanagedMethods.CANDIDATEFORM cANDIDATEFORM = default(UnmanagedMethods.CANDIDATEFORM);
			cANDIDATEFORM.dwIndex = 0;
			cANDIDATEFORM.dwStyle = 64;
			cANDIDATEFORM.ptCurrentPos = new UnmanagedMethods.POINT
			{
				X = num8,
				Y = num9
			};
			UnmanagedMethods.CANDIDATEFORM lpCandidate = cANDIDATEFORM;
			UnmanagedMethods.ImmSetCandidateWindow(himc, ref lpCandidate);
		}
		_caretManager.TryMove(num8, num9);
		if (ShowCompositionWindow)
		{
			ConfigureCompositionWindow(num6, num7, himc, num9 - num7);
			return;
		}
		if (_langId == 18)
		{
			num9++;
		}
		if (_langId != 4)
		{
			UnmanagedMethods.CANDIDATEFORM cANDIDATEFORM = default(UnmanagedMethods.CANDIDATEFORM);
			cANDIDATEFORM.dwIndex = 0;
			cANDIDATEFORM.dwStyle = 128;
			cANDIDATEFORM.ptCurrentPos = new UnmanagedMethods.POINT
			{
				X = num6,
				Y = num7
			};
			cANDIDATEFORM.rcArea = new UnmanagedMethods.RECT
			{
				left = num6,
				top = num7,
				right = num8,
				bottom = num9 + 1
			};
			UnmanagedMethods.CANDIDATEFORM lpCandidate2 = cANDIDATEFORM;
			UnmanagedMethods.ImmSetCandidateWindow(himc, ref lpCandidate2);
		}
	}

	private static void ConfigureCompositionWindow(int x1, int y1, IntPtr himc, int height)
	{
		UnmanagedMethods.COMPOSITIONFORM cOMPOSITIONFORM = default(UnmanagedMethods.COMPOSITIONFORM);
		cOMPOSITIONFORM.dwStyle = 2;
		cOMPOSITIONFORM.ptCurrentPos = new UnmanagedMethods.POINT
		{
			X = x1,
			Y = y1
		};
		UnmanagedMethods.COMPOSITIONFORM lpComp = cOMPOSITIONFORM;
		UnmanagedMethods.ImmSetCompositionWindow(himc, ref lpComp);
		UnmanagedMethods.LOGFONT lOGFONT = default(UnmanagedMethods.LOGFONT);
		lOGFONT.lfHeight = height;
		lOGFONT.lfQuality = 5;
		UnmanagedMethods.LOGFONT lf = lOGFONT;
		UnmanagedMethods.ImmSetCompositionFont(himc, ref lf);
	}

	public void SetOptions(TextInputOptions options)
	{
	}

	public void CompositionChanged(string? composition)
	{
		Composition = composition;
		if (IsActive && Client.SupportsPreedit)
		{
			Client.SetPreeditText(composition);
		}
	}

	public string? GetCompositionString(UnmanagedMethods.GCS flag)
	{
		if (!IsComposing)
		{
			return null;
		}
		return UnmanagedMethods.ImmGetCompositionString(UnmanagedMethods.ImmGetContext(Hwnd), flag);
	}

	public void HandleCompositionStart()
	{
		Composition = null;
		if (IsActive)
		{
			Client.SetPreeditText(null);
			if (Client.SupportsSurroundingText && Client.Selection.Start != Client.Selection.End)
			{
				KeyPress(Key.Delete);
			}
		}
		IsComposing = true;
	}

	public void HandleCompositionEnd()
	{
		IsComposing = false;
		Composition = null;
		if (IsActive)
		{
			Client.SetPreeditText(null);
		}
	}

	public void HandleComposition(IntPtr wParam, IntPtr lParam, uint timestamp)
	{
		if (_ignoreComposition)
		{
			_ignoreComposition = false;
			return;
		}
		int num = ToInt32(lParam);
		if ((num & 0x800) != 0)
		{
			string compositionString = GetCompositionString(UnmanagedMethods.GCS.GCS_RESULTSTR);
			if (_parent != null && !string.IsNullOrEmpty(compositionString))
			{
				Composition = null;
				if (IsActive)
				{
					Client.SetPreeditText(null);
				}
				RawTextInputEventArgs obj = new RawTextInputEventArgs(WindowsKeyboardDevice.Instance, timestamp, _parent.Owner, compositionString);
				if (_parent.Input != null)
				{
					_parent.Input(obj);
					_parent._ignoreWmChar = true;
				}
			}
		}
		if ((num & 8) != 0)
		{
			string compositionString2 = GetCompositionString(UnmanagedMethods.GCS.GCS_COMPSTR);
			CompositionChanged(compositionString2);
		}
	}

	private static int ToInt32(IntPtr ptr)
	{
		if (IntPtr.Size == 4)
		{
			return ptr.ToInt32();
		}
		return (int)(ptr.ToInt64() & 0xFFFFFFFFu);
	}

	private void KeyPress(Key key)
	{
		if (_parent?.Input != null)
		{
			_parent.Input(new RawKeyEventArgs(KeyboardDevice.Instance, (ulong)DateTime.Now.Ticks, _parent.Owner, RawKeyEventType.KeyDown, key, RawInputModifiers.None));
			_parent.Input(new RawKeyEventArgs(KeyboardDevice.Instance, (ulong)DateTime.Now.Ticks, _parent.Owner, RawKeyEventType.KeyUp, key, RawInputModifiers.None));
		}
	}

	~Imm32InputMethod()
	{
		_caretManager.TryDestroy();
	}
}
