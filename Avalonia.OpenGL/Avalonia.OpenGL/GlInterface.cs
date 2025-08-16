using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia.Platform.Interop;
using Avalonia.SourceGenerator;

namespace Avalonia.OpenGL;

public class GlInterface : GlBasicInfoInterface
{
	public class GlContextInfo
	{
		public GlVersion Version { get; }

		public HashSet<string> Extensions { get; }

		public GlContextInfo(GlVersion version, HashSet<string> extensions)
		{
			Version = version;
			Extensions = extensions;
		}

		public static GlContextInfo Create(GlVersion version, Func<string, IntPtr> getProcAddress)
		{
			List<string> extensions = new GlBasicInfoInterface(getProcAddress).GetExtensions();
			return new GlContextInfo(version, new HashSet<string>(extensions));
		}
	}

	private readonly Func<string, IntPtr> _getProcAddress;

	private unsafe delegate* unmanaged[Stdcall]<int> _addr_GetError;

	private unsafe delegate* unmanaged[Stdcall]<int, void> _addr_ClearStencil;

	private unsafe delegate* unmanaged[Stdcall]<float, float, float, float, void> _addr_ClearColor;

	private unsafe delegate* unmanaged[Stdcall]<int, void> _addr_Clear;

	private unsafe delegate* unmanaged[Stdcall]<int, int, int, int, void> _addr_Viewport;

	private unsafe delegate* unmanaged[Stdcall]<void> _addr_Flush;

	private unsafe delegate* unmanaged[Stdcall]<void> _addr_Finish;

	private unsafe delegate* unmanaged[Stdcall]<int, int*, void> _addr_GenFramebuffers;

	private unsafe delegate* unmanaged[Stdcall]<int, int*, void> _addr_DeleteFramebuffers;

	private unsafe delegate* unmanaged[Stdcall]<int, int, void> _addr_BindFramebuffer;

	private unsafe delegate* unmanaged[Stdcall]<int, int> _addr_CheckFramebufferStatus;

	private unsafe delegate* unmanaged[Stdcall]<int, int, int, int, int, int, int, int, int, int, void> _addr_BlitFramebuffer;

	private unsafe delegate* unmanaged[Stdcall]<int, int*, void> _addr_GenRenderbuffers;

	private unsafe delegate* unmanaged[Stdcall]<int, int*, void> _addr_DeleteRenderbuffers;

	private unsafe delegate* unmanaged[Stdcall]<int, int, void> _addr_BindRenderbuffer;

	private unsafe delegate* unmanaged[Stdcall]<int, int, int, int, void> _addr_RenderbufferStorage;

	private unsafe delegate* unmanaged[Stdcall]<int, int, int, int, void> _addr_FramebufferRenderbuffer;

	private unsafe delegate* unmanaged[Stdcall]<int, int*, void> _addr_GenTextures;

	private unsafe delegate* unmanaged[Stdcall]<int, int, void> _addr_BindTexture;

	private unsafe delegate* unmanaged[Stdcall]<int, void> _addr_ActiveTexture;

	private unsafe delegate* unmanaged[Stdcall]<int, int*, void> _addr_DeleteTextures;

	private unsafe delegate* unmanaged[Stdcall]<int, int, int, int, int, int, int, int, IntPtr, void> _addr_TexImage2D;

	private unsafe delegate* unmanaged[Stdcall]<int, int, int, int, int, int, int, int, void> _addr_CopyTexSubImage2D;

	private unsafe delegate* unmanaged[Stdcall]<int, int, int, void> _addr_TexParameteri;

	private unsafe delegate* unmanaged[Stdcall]<int, int, int, int, int, void> _addr_FramebufferTexture2D;

	private unsafe delegate* unmanaged[Stdcall]<int, int> _addr_CreateShader;

	private unsafe delegate* unmanaged[Stdcall]<int, int, IntPtr, IntPtr, void> _addr_ShaderSource;

	private unsafe delegate* unmanaged[Stdcall]<int, void> _addr_CompileShader;

	private unsafe delegate* unmanaged[Stdcall]<int, int, int*, void> _addr_GetShaderiv;

	private unsafe delegate* unmanaged[Stdcall]<int, int, out int, void*, void> _addr_GetShaderInfoLog;

	private unsafe delegate* unmanaged[Stdcall]<int> _addr_CreateProgram;

	private unsafe delegate* unmanaged[Stdcall]<int, int, void> _addr_AttachShader;

	private unsafe delegate* unmanaged[Stdcall]<int, void> _addr_LinkProgram;

	private unsafe delegate* unmanaged[Stdcall]<int, int, int*, void> _addr_GetProgramiv;

	private unsafe delegate* unmanaged[Stdcall]<int, int, out int, void*, void> _addr_GetProgramInfoLog;

	private unsafe delegate* unmanaged[Stdcall]<int, int, IntPtr, void> _addr_BindAttribLocation;

	private unsafe delegate* unmanaged[Stdcall]<int, int*, void> _addr_GenBuffers;

	private unsafe delegate* unmanaged[Stdcall]<int, int, void> _addr_BindBuffer;

	private unsafe delegate* unmanaged[Stdcall]<int, IntPtr, IntPtr, int, void> _addr_BufferData;

	private unsafe delegate* unmanaged[Stdcall]<int, IntPtr, int> _addr_GetAttribLocation;

	private unsafe delegate* unmanaged[Stdcall]<int, int, int, int, int, IntPtr, void> _addr_VertexAttribPointer;

	private unsafe delegate* unmanaged[Stdcall]<int, void> _addr_EnableVertexAttribArray;

	private unsafe delegate* unmanaged[Stdcall]<int, void> _addr_UseProgram;

	private unsafe delegate* unmanaged[Stdcall]<int, int, IntPtr, void> _addr_DrawArrays;

	private unsafe delegate* unmanaged[Stdcall]<int, int, int, IntPtr, void> _addr_DrawElements;

	private unsafe delegate* unmanaged[Stdcall]<int, IntPtr, int> _addr_GetUniformLocation;

	private unsafe delegate* unmanaged[Stdcall]<int, float, void> _addr_Uniform1f;

	private unsafe delegate* unmanaged[Stdcall]<int, int, int, void*, void> _addr_UniformMatrix4fv;

	private unsafe delegate* unmanaged[Stdcall]<int, void> _addr_Enable;

	private unsafe delegate* unmanaged[Stdcall]<int, int*, void> _addr_DeleteBuffers;

	private unsafe delegate* unmanaged[Stdcall]<int, void> _addr_DeleteProgram;

	private unsafe delegate* unmanaged[Stdcall]<int, void> _addr_DeleteShader;

	private unsafe delegate* unmanaged[Stdcall]<int, int, out int, void> _addr_GetRenderbufferParameteriv;

	private unsafe delegate* unmanaged[Stdcall]<int, int*, void> _addr_DeleteVertexArrays;

	private unsafe delegate* unmanaged[Stdcall]<int, void> _addr_BindVertexArray;

	private unsafe delegate* unmanaged[Stdcall]<int, int*, void> _addr_GenVertexArrays;

	public string? Version { get; }

	public string? Vendor { get; }

	public string? Renderer { get; }

	public GlContextInfo ContextInfo { get; }

	public unsafe bool IsBlitFramebufferAvailable => _addr_BlitFramebuffer != (delegate* unmanaged[Stdcall]<int, int, int, int, int, int, int, int, int, int, void>)null;

	public unsafe bool IsDeleteVertexArraysAvailable => _addr_DeleteVertexArrays != (delegate* unmanaged[Stdcall]<int, int*, void>)null;

	public unsafe bool IsBindVertexArrayAvailable => _addr_BindVertexArray != (delegate* unmanaged[Stdcall]<int, void>)null;

	public unsafe bool IsGenVertexArraysAvailable => _addr_GenVertexArrays != (delegate* unmanaged[Stdcall]<int, int*, void>)null;

	private GlInterface(GlContextInfo info, Func<string, IntPtr> getProcAddress)
		: base(getProcAddress)
	{
		_getProcAddress = getProcAddress;
		ContextInfo = info;
		Version = GetString(7938);
		Renderer = GetString(7937);
		Vendor = GetString(7936);
		Initialize(getProcAddress, ContextInfo);
	}

	public GlInterface(GlVersion version, Func<string, IntPtr> getProcAddress)
		: this(GlContextInfo.Create(version, getProcAddress), getProcAddress)
	{
	}

	public IntPtr GetProcAddress(string proc)
	{
		return _getProcAddress(proc);
	}

	[GetProcAddress("glGetError")]
	public unsafe int GetError()
	{
		return _addr_GetError();
	}

	[GetProcAddress("glClearStencil")]
	public unsafe void ClearStencil(int s)
	{
		_addr_ClearStencil(s);
	}

	[GetProcAddress("glClearColor")]
	public unsafe void ClearColor(float r, float g, float b, float a)
	{
		_addr_ClearColor(r, g, b, a);
	}

	[GetProcAddress("glClear")]
	public unsafe void Clear(int bits)
	{
		_addr_Clear(bits);
	}

	[GetProcAddress("glViewport")]
	public unsafe void Viewport(int x, int y, int width, int height)
	{
		_addr_Viewport(x, y, width, height);
	}

	[GetProcAddress("glFlush")]
	public unsafe void Flush()
	{
		_addr_Flush();
	}

	[GetProcAddress("glFinish")]
	public unsafe void Finish()
	{
		_addr_Finish();
	}

	[GetProcAddress("glGenFramebuffers")]
	public unsafe void GenFramebuffers(int count, int* res)
	{
		_addr_GenFramebuffers(count, res);
	}

	public unsafe int GenFramebuffer()
	{
		int result = 0;
		GenFramebuffers(1, &result);
		return result;
	}

	[GetProcAddress("glDeleteFramebuffers")]
	public unsafe void DeleteFramebuffers(int count, int* framebuffers)
	{
		_addr_DeleteFramebuffers(count, framebuffers);
	}

	public unsafe void DeleteFramebuffer(int fb)
	{
		DeleteFramebuffers(1, &fb);
	}

	[GetProcAddress("glBindFramebuffer")]
	public unsafe void BindFramebuffer(int target, int fb)
	{
		_addr_BindFramebuffer(target, fb);
	}

	[GetProcAddress("glCheckFramebufferStatus")]
	public unsafe int CheckFramebufferStatus(int target)
	{
		return _addr_CheckFramebufferStatus(target);
	}

	[GlMinVersionEntryPoint("glBlitFramebuffer", 3, 0)]
	[GetProcAddress(true)]
	public unsafe void BlitFramebuffer(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, int mask, int filter)
	{
		if (_addr_BlitFramebuffer == (delegate* unmanaged[Stdcall]<int, int, int, int, int, int, int, int, int, int, void>)null)
		{
			throw new EntryPointNotFoundException("BlitFramebuffer");
		}
		_addr_BlitFramebuffer(srcX0, srcY0, srcX1, srcY1, dstX0, dstY0, dstX1, dstY1, mask, filter);
	}

	[GetProcAddress("glGenRenderbuffers")]
	public unsafe void GenRenderbuffers(int count, int* res)
	{
		_addr_GenRenderbuffers(count, res);
	}

	public unsafe int GenRenderbuffer()
	{
		int result = 0;
		GenRenderbuffers(1, &result);
		return result;
	}

	[GetProcAddress("glDeleteRenderbuffers")]
	public unsafe void DeleteRenderbuffers(int count, int* renderbuffers)
	{
		_addr_DeleteRenderbuffers(count, renderbuffers);
	}

	public unsafe void DeleteRenderbuffer(int renderbuffer)
	{
		DeleteRenderbuffers(1, &renderbuffer);
	}

	[GetProcAddress("glBindRenderbuffer")]
	public unsafe void BindRenderbuffer(int target, int fb)
	{
		_addr_BindRenderbuffer(target, fb);
	}

	[GetProcAddress("glRenderbufferStorage")]
	public unsafe void RenderbufferStorage(int target, int internalFormat, int width, int height)
	{
		_addr_RenderbufferStorage(target, internalFormat, width, height);
	}

	[GetProcAddress("glFramebufferRenderbuffer")]
	public unsafe void FramebufferRenderbuffer(int target, int attachment, int renderbufferTarget, int renderbuffer)
	{
		_addr_FramebufferRenderbuffer(target, attachment, renderbufferTarget, renderbuffer);
	}

	[GetProcAddress("glGenTextures")]
	public unsafe void GenTextures(int count, int* res)
	{
		_addr_GenTextures(count, res);
	}

	public unsafe int GenTexture()
	{
		int result = 0;
		GenTextures(1, &result);
		return result;
	}

	[GetProcAddress("glBindTexture")]
	public unsafe void BindTexture(int target, int fb)
	{
		_addr_BindTexture(target, fb);
	}

	[GetProcAddress("glActiveTexture")]
	public unsafe void ActiveTexture(int texture)
	{
		_addr_ActiveTexture(texture);
	}

	[GetProcAddress("glDeleteTextures")]
	public unsafe void DeleteTextures(int count, int* textures)
	{
		_addr_DeleteTextures(count, textures);
	}

	public unsafe void DeleteTexture(int texture)
	{
		DeleteTextures(1, &texture);
	}

	[GetProcAddress("glTexImage2D")]
	public unsafe void TexImage2D(int target, int level, int internalFormat, int width, int height, int border, int format, int type, IntPtr data)
	{
		_addr_TexImage2D(target, level, internalFormat, width, height, border, format, type, data);
	}

	[GetProcAddress("glCopyTexSubImage2D")]
	public unsafe void CopyTexSubImage2D(int target, int level, int xoffset, int yoffset, int x, int y, int width, int height)
	{
		_addr_CopyTexSubImage2D(target, level, xoffset, yoffset, x, y, width, height);
	}

	[GetProcAddress("glTexParameteri")]
	public unsafe void TexParameteri(int target, int name, int value)
	{
		_addr_TexParameteri(target, name, value);
	}

	[GetProcAddress("glFramebufferTexture2D")]
	public unsafe void FramebufferTexture2D(int target, int attachment, int texTarget, int texture, int level)
	{
		_addr_FramebufferTexture2D(target, attachment, texTarget, texture, level);
	}

	[GetProcAddress("glCreateShader")]
	public unsafe int CreateShader(int shaderType)
	{
		return _addr_CreateShader(shaderType);
	}

	[GetProcAddress("glShaderSource")]
	public unsafe void ShaderSource(int shader, int count, IntPtr strings, IntPtr lengths)
	{
		_addr_ShaderSource(shader, count, strings, lengths);
	}

	public unsafe void ShaderSourceString(int shader, string source)
	{
		using Utf8Buffer utf8Buffer = new Utf8Buffer(source);
		IntPtr intPtr = utf8Buffer.DangerousGetHandle();
		IntPtr intPtr2 = new IntPtr(utf8Buffer.ByteLen);
		ShaderSource(shader, 1, new IntPtr(&intPtr), new IntPtr(&intPtr2));
	}

	[GetProcAddress("glCompileShader")]
	public unsafe void CompileShader(int shader)
	{
		_addr_CompileShader(shader);
	}

	[GetProcAddress("glGetShaderiv")]
	public unsafe void GetShaderiv(int shader, int name, int* parameters)
	{
		_addr_GetShaderiv(shader, name, parameters);
	}

	[GetProcAddress("glGetShaderInfoLog")]
	public unsafe void GetShaderInfoLog(int shader, int maxLength, out int length, void* infoLog)
	{
		_addr_GetShaderInfoLog(shader, maxLength, out length, infoLog);
	}

	public unsafe string? CompileShaderAndGetError(int shader, string source)
	{
		ShaderSourceString(shader, source);
		CompileShader(shader);
		int num = default(int);
		GetShaderiv(shader, 35713, &num);
		if (num != 0)
		{
			return null;
		}
		int num2 = default(int);
		GetShaderiv(shader, 35716, &num2);
		if (num2 == 0)
		{
			num2 = 4096;
		}
		byte[] array = new byte[num2];
		int length;
		fixed (byte* ptr = array)
		{
			void* infoLog = ptr;
			GetShaderInfoLog(shader, num2, out length, infoLog);
		}
		return Encoding.UTF8.GetString(array, 0, length);
	}

	[GetProcAddress("glCreateProgram")]
	public unsafe int CreateProgram()
	{
		return _addr_CreateProgram();
	}

	[GetProcAddress("glAttachShader")]
	public unsafe void AttachShader(int program, int shader)
	{
		_addr_AttachShader(program, shader);
	}

	[GetProcAddress("glLinkProgram")]
	public unsafe void LinkProgram(int program)
	{
		_addr_LinkProgram(program);
	}

	[GetProcAddress("glGetProgramiv")]
	public unsafe void GetProgramiv(int program, int name, int* parameters)
	{
		_addr_GetProgramiv(program, name, parameters);
	}

	[GetProcAddress("glGetProgramInfoLog")]
	public unsafe void GetProgramInfoLog(int program, int maxLength, out int len, void* infoLog)
	{
		_addr_GetProgramInfoLog(program, maxLength, out len, infoLog);
	}

	public unsafe string? LinkProgramAndGetError(int program)
	{
		LinkProgram(program);
		int num = default(int);
		GetProgramiv(program, 35714, &num);
		if (num != 0)
		{
			return null;
		}
		int num2 = default(int);
		GetProgramiv(program, 35716, &num2);
		byte[] array = new byte[num2];
		int len;
		fixed (byte* ptr = array)
		{
			void* infoLog = ptr;
			GetProgramInfoLog(program, num2, out len, infoLog);
		}
		return Encoding.UTF8.GetString(array, 0, len);
	}

	[GetProcAddress("glBindAttribLocation")]
	public unsafe void BindAttribLocation(int program, int index, IntPtr name)
	{
		_addr_BindAttribLocation(program, index, name);
	}

	public void BindAttribLocationString(int program, int index, string name)
	{
		using Utf8Buffer utf8Buffer = new Utf8Buffer(name);
		BindAttribLocation(program, index, utf8Buffer.DangerousGetHandle());
	}

	[GetProcAddress("glGenBuffers")]
	public unsafe void GenBuffers(int len, int* rv)
	{
		_addr_GenBuffers(len, rv);
	}

	public unsafe int GenBuffer()
	{
		int result = default(int);
		GenBuffers(1, &result);
		return result;
	}

	[GetProcAddress("glBindBuffer")]
	public unsafe void BindBuffer(int target, int buffer)
	{
		_addr_BindBuffer(target, buffer);
	}

	[GetProcAddress("glBufferData")]
	public unsafe void BufferData(int target, IntPtr size, IntPtr data, int usage)
	{
		_addr_BufferData(target, size, data, usage);
	}

	[GetProcAddress("glGetAttribLocation")]
	public unsafe int GetAttribLocation(int program, IntPtr name)
	{
		return _addr_GetAttribLocation(program, name);
	}

	public int GetAttribLocationString(int program, string name)
	{
		using Utf8Buffer utf8Buffer = new Utf8Buffer(name);
		return GetAttribLocation(program, utf8Buffer.DangerousGetHandle());
	}

	[GetProcAddress("glVertexAttribPointer")]
	public unsafe void VertexAttribPointer(int index, int size, int type, int normalized, int stride, IntPtr pointer)
	{
		_addr_VertexAttribPointer(index, size, type, normalized, stride, pointer);
	}

	[GetProcAddress("glEnableVertexAttribArray")]
	public unsafe void EnableVertexAttribArray(int index)
	{
		_addr_EnableVertexAttribArray(index);
	}

	[GetProcAddress("glUseProgram")]
	public unsafe void UseProgram(int program)
	{
		_addr_UseProgram(program);
	}

	[GetProcAddress("glDrawArrays")]
	public unsafe void DrawArrays(int mode, int first, IntPtr count)
	{
		_addr_DrawArrays(mode, first, count);
	}

	[GetProcAddress("glDrawElements")]
	public unsafe void DrawElements(int mode, int count, int type, IntPtr indices)
	{
		_addr_DrawElements(mode, count, type, indices);
	}

	[GetProcAddress("glGetUniformLocation")]
	public unsafe int GetUniformLocation(int program, IntPtr name)
	{
		return _addr_GetUniformLocation(program, name);
	}

	public int GetUniformLocationString(int program, string name)
	{
		using Utf8Buffer utf8Buffer = new Utf8Buffer(name);
		return GetUniformLocation(program, utf8Buffer.DangerousGetHandle());
	}

	[GetProcAddress("glUniform1f")]
	public unsafe void Uniform1f(int location, float falue)
	{
		_addr_Uniform1f(location, falue);
	}

	[GetProcAddress("glUniformMatrix4fv")]
	public unsafe void UniformMatrix4fv(int location, int count, bool transpose, void* value)
	{
		_addr_UniformMatrix4fv(location, count, transpose ? 1 : 0, value);
	}

	[GetProcAddress("glEnable")]
	public unsafe void Enable(int what)
	{
		_addr_Enable(what);
	}

	[GetProcAddress("glDeleteBuffers")]
	public unsafe void DeleteBuffers(int count, int* buffers)
	{
		_addr_DeleteBuffers(count, buffers);
	}

	public unsafe void DeleteBuffer(int buffer)
	{
		DeleteBuffers(1, &buffer);
	}

	[GetProcAddress("glDeleteProgram")]
	public unsafe void DeleteProgram(int program)
	{
		_addr_DeleteProgram(program);
	}

	[GetProcAddress("glDeleteShader")]
	public unsafe void DeleteShader(int shader)
	{
		_addr_DeleteShader(shader);
	}

	[GetProcAddress("glGetRenderbufferParameteriv")]
	public unsafe void GetRenderbufferParameteriv(int target, int name, out int value)
	{
		_addr_GetRenderbufferParameteriv(target, name, out value);
	}

	[GetProcAddress(true)]
	[GlMinVersionEntryPoint("glDeleteVertexArrays", 3, 0)]
	[GlExtensionEntryPoint("glDeleteVertexArraysOES", "GL_OES_vertex_array_object")]
	public unsafe void DeleteVertexArrays(int count, int* arrays)
	{
		if (_addr_DeleteVertexArrays == (delegate* unmanaged[Stdcall]<int, int*, void>)null)
		{
			throw new EntryPointNotFoundException("DeleteVertexArrays");
		}
		_addr_DeleteVertexArrays(count, arrays);
	}

	public unsafe void DeleteVertexArray(int array)
	{
		DeleteVertexArrays(1, &array);
	}

	[GetProcAddress(true)]
	[GlMinVersionEntryPoint("glBindVertexArray", 3, 0)]
	[GlExtensionEntryPoint("glBindVertexArrayOES", "GL_OES_vertex_array_object")]
	public unsafe void BindVertexArray(int array)
	{
		if (_addr_BindVertexArray == (delegate* unmanaged[Stdcall]<int, void>)null)
		{
			throw new EntryPointNotFoundException("BindVertexArray");
		}
		_addr_BindVertexArray(array);
	}

	[GetProcAddress(true)]
	[GlMinVersionEntryPoint("glGenVertexArrays", 3, 0)]
	[GlExtensionEntryPoint("glGenVertexArraysOES", "GL_OES_vertex_array_object")]
	public unsafe void GenVertexArrays(int n, int* rv)
	{
		if (_addr_GenVertexArrays == (delegate* unmanaged[Stdcall]<int, int*, void>)null)
		{
			throw new EntryPointNotFoundException("GenVertexArrays");
		}
		_addr_GenVertexArrays(n, rv);
	}

	public unsafe int GenVertexArray()
	{
		int result = 0;
		GenVertexArrays(1, &result);
		return result;
	}

	public static GlInterface FromNativeUtf8GetProcAddress(GlVersion version, Func<IntPtr, IntPtr> getProcAddress)
	{
		return new GlInterface(version, delegate(string s)
		{
			IntPtr intPtr = Marshal.StringToHGlobalAnsi(s);
			IntPtr result = getProcAddress(intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		});
	}

	private unsafe void Initialize(Func<string, IntPtr> getProcAddress, GlContextInfo context)
	{
		IntPtr zero = IntPtr.Zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGetError");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetError");
		}
		_addr_GetError = (delegate* unmanaged[Stdcall]<int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glClearStencil");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_ClearStencil");
		}
		_addr_ClearStencil = (delegate* unmanaged[Stdcall]<int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glClearColor");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_ClearColor");
		}
		_addr_ClearColor = (delegate* unmanaged[Stdcall]<float, float, float, float, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glClear");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_Clear");
		}
		_addr_Clear = (delegate* unmanaged[Stdcall]<int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glViewport");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_Viewport");
		}
		_addr_Viewport = (delegate* unmanaged[Stdcall]<int, int, int, int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glFlush");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_Flush");
		}
		_addr_Flush = (delegate* unmanaged[Stdcall]<void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glFinish");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_Finish");
		}
		_addr_Finish = (delegate* unmanaged[Stdcall]<void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGenFramebuffers");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GenFramebuffers");
		}
		_addr_GenFramebuffers = (delegate* unmanaged[Stdcall]<int, int*, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glDeleteFramebuffers");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_DeleteFramebuffers");
		}
		_addr_DeleteFramebuffers = (delegate* unmanaged[Stdcall]<int, int*, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glBindFramebuffer");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_BindFramebuffer");
		}
		_addr_BindFramebuffer = (delegate* unmanaged[Stdcall]<int, int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glCheckFramebufferStatus");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_CheckFramebufferStatus");
		}
		_addr_CheckFramebufferStatus = (delegate* unmanaged[Stdcall]<int, int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = GlMinVersionEntryPoint.GetProcAddress(getProcAddress, context, "glBlitFramebuffer", 3, 0, null);
		_addr_BlitFramebuffer = (delegate* unmanaged[Stdcall]<int, int, int, int, int, int, int, int, int, int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGenRenderbuffers");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GenRenderbuffers");
		}
		_addr_GenRenderbuffers = (delegate* unmanaged[Stdcall]<int, int*, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glDeleteRenderbuffers");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_DeleteRenderbuffers");
		}
		_addr_DeleteRenderbuffers = (delegate* unmanaged[Stdcall]<int, int*, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glBindRenderbuffer");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_BindRenderbuffer");
		}
		_addr_BindRenderbuffer = (delegate* unmanaged[Stdcall]<int, int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glRenderbufferStorage");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_RenderbufferStorage");
		}
		_addr_RenderbufferStorage = (delegate* unmanaged[Stdcall]<int, int, int, int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glFramebufferRenderbuffer");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_FramebufferRenderbuffer");
		}
		_addr_FramebufferRenderbuffer = (delegate* unmanaged[Stdcall]<int, int, int, int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGenTextures");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GenTextures");
		}
		_addr_GenTextures = (delegate* unmanaged[Stdcall]<int, int*, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glBindTexture");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_BindTexture");
		}
		_addr_BindTexture = (delegate* unmanaged[Stdcall]<int, int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glActiveTexture");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_ActiveTexture");
		}
		_addr_ActiveTexture = (delegate* unmanaged[Stdcall]<int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glDeleteTextures");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_DeleteTextures");
		}
		_addr_DeleteTextures = (delegate* unmanaged[Stdcall]<int, int*, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glTexImage2D");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_TexImage2D");
		}
		_addr_TexImage2D = (delegate* unmanaged[Stdcall]<int, int, int, int, int, int, int, int, IntPtr, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glCopyTexSubImage2D");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_CopyTexSubImage2D");
		}
		_addr_CopyTexSubImage2D = (delegate* unmanaged[Stdcall]<int, int, int, int, int, int, int, int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glTexParameteri");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_TexParameteri");
		}
		_addr_TexParameteri = (delegate* unmanaged[Stdcall]<int, int, int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glFramebufferTexture2D");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_FramebufferTexture2D");
		}
		_addr_FramebufferTexture2D = (delegate* unmanaged[Stdcall]<int, int, int, int, int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glCreateShader");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_CreateShader");
		}
		_addr_CreateShader = (delegate* unmanaged[Stdcall]<int, int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glShaderSource");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_ShaderSource");
		}
		_addr_ShaderSource = (delegate* unmanaged[Stdcall]<int, int, IntPtr, IntPtr, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glCompileShader");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_CompileShader");
		}
		_addr_CompileShader = (delegate* unmanaged[Stdcall]<int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGetShaderiv");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetShaderiv");
		}
		_addr_GetShaderiv = (delegate* unmanaged[Stdcall]<int, int, int*, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGetShaderInfoLog");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetShaderInfoLog");
		}
		_addr_GetShaderInfoLog = (delegate* unmanaged[Stdcall]<int, int, out int, void*, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glCreateProgram");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_CreateProgram");
		}
		_addr_CreateProgram = (delegate* unmanaged[Stdcall]<int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glAttachShader");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_AttachShader");
		}
		_addr_AttachShader = (delegate* unmanaged[Stdcall]<int, int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glLinkProgram");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_LinkProgram");
		}
		_addr_LinkProgram = (delegate* unmanaged[Stdcall]<int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGetProgramiv");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetProgramiv");
		}
		_addr_GetProgramiv = (delegate* unmanaged[Stdcall]<int, int, int*, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGetProgramInfoLog");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetProgramInfoLog");
		}
		_addr_GetProgramInfoLog = (delegate* unmanaged[Stdcall]<int, int, out int, void*, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glBindAttribLocation");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_BindAttribLocation");
		}
		_addr_BindAttribLocation = (delegate* unmanaged[Stdcall]<int, int, IntPtr, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGenBuffers");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GenBuffers");
		}
		_addr_GenBuffers = (delegate* unmanaged[Stdcall]<int, int*, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glBindBuffer");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_BindBuffer");
		}
		_addr_BindBuffer = (delegate* unmanaged[Stdcall]<int, int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glBufferData");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_BufferData");
		}
		_addr_BufferData = (delegate* unmanaged[Stdcall]<int, IntPtr, IntPtr, int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGetAttribLocation");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetAttribLocation");
		}
		_addr_GetAttribLocation = (delegate* unmanaged[Stdcall]<int, IntPtr, int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glVertexAttribPointer");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_VertexAttribPointer");
		}
		_addr_VertexAttribPointer = (delegate* unmanaged[Stdcall]<int, int, int, int, int, IntPtr, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glEnableVertexAttribArray");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_EnableVertexAttribArray");
		}
		_addr_EnableVertexAttribArray = (delegate* unmanaged[Stdcall]<int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glUseProgram");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_UseProgram");
		}
		_addr_UseProgram = (delegate* unmanaged[Stdcall]<int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glDrawArrays");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_DrawArrays");
		}
		_addr_DrawArrays = (delegate* unmanaged[Stdcall]<int, int, IntPtr, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glDrawElements");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_DrawElements");
		}
		_addr_DrawElements = (delegate* unmanaged[Stdcall]<int, int, int, IntPtr, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGetUniformLocation");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetUniformLocation");
		}
		_addr_GetUniformLocation = (delegate* unmanaged[Stdcall]<int, IntPtr, int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glUniform1f");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_Uniform1f");
		}
		_addr_Uniform1f = (delegate* unmanaged[Stdcall]<int, float, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glUniformMatrix4fv");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_UniformMatrix4fv");
		}
		_addr_UniformMatrix4fv = (delegate* unmanaged[Stdcall]<int, int, int, void*, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glEnable");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_Enable");
		}
		_addr_Enable = (delegate* unmanaged[Stdcall]<int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glDeleteBuffers");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_DeleteBuffers");
		}
		_addr_DeleteBuffers = (delegate* unmanaged[Stdcall]<int, int*, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glDeleteProgram");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_DeleteProgram");
		}
		_addr_DeleteProgram = (delegate* unmanaged[Stdcall]<int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glDeleteShader");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_DeleteShader");
		}
		_addr_DeleteShader = (delegate* unmanaged[Stdcall]<int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGetRenderbufferParameteriv");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetRenderbufferParameteriv");
		}
		_addr_GetRenderbufferParameteriv = (delegate* unmanaged[Stdcall]<int, int, out int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = GlMinVersionEntryPoint.GetProcAddress(getProcAddress, context, "glDeleteVertexArrays", 3, 0, null);
		if (zero == IntPtr.Zero)
		{
			zero = GlExtensionEntryPoint.GetProcAddress(getProcAddress, context, "glDeleteVertexArraysOES", "GL_OES_vertex_array_object", null);
		}
		_addr_DeleteVertexArrays = (delegate* unmanaged[Stdcall]<int, int*, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = GlMinVersionEntryPoint.GetProcAddress(getProcAddress, context, "glBindVertexArray", 3, 0, null);
		if (zero == IntPtr.Zero)
		{
			zero = GlExtensionEntryPoint.GetProcAddress(getProcAddress, context, "glBindVertexArrayOES", "GL_OES_vertex_array_object", null);
		}
		_addr_BindVertexArray = (delegate* unmanaged[Stdcall]<int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = GlMinVersionEntryPoint.GetProcAddress(getProcAddress, context, "glGenVertexArrays", 3, 0, null);
		if (zero == IntPtr.Zero)
		{
			zero = GlExtensionEntryPoint.GetProcAddress(getProcAddress, context, "glGenVertexArraysOES", "GL_OES_vertex_array_object", null);
		}
		_addr_GenVertexArrays = (delegate* unmanaged[Stdcall]<int, int*, void>)(void*)zero;
	}
}
