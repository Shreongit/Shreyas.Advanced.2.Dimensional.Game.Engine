using System.Runtime.InteropServices;


namespace SA2DGE.Engine.Platform.Window;


public sealed class WindowsWindowBackend : IWindowBackend
{
    private const int CW_USEDEFAULT = unchecked((int)0x80000000);

    private const uint WS_OVERLAPPED = 0x00000000;
    private const uint WS_CAPTION = 0x00C00000;
    private const uint WS_SYSMENU = 0x00080000;
    private const uint WS_THICKFRAME = 0x00040000;
    private const uint WS_MINIMIZEBOX = 0x00020000;
    private const uint WS_MAXIMIZEBOX = 0x00010000;
    private const uint WM_MOUSEWHEEL = 0x020A;

    private const uint WS_EX_APPWINDOW = 0x00040000;

    private const int SW_SHOW = 5;

    private const uint WM_CLOSE = 0x0010;
    private const uint WM_DESTROY = 0x0002;
    private const uint WM_SIZE = 0x0005;
    private const uint WM_QUIT = 0x0012;
    
    private const nuint SIZE_RESTORED = 0;
    private const nuint SIZE_MINIMIZED = 1;
    private const nuint SIZE_MAXIMIZED = 2;

    private const uint PM_REMOVE = 0x0001;

    

    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVATE = 0x0010;

    private const int GWLP_USERDATA = -21;

    private const uint CS_HREDRAW = 0x0002;
    private const uint CS_VREDRAW = 0x0001;

    private const string WindowClassName =
        "SA2DGE.NativeWindow";

    private static readonly object SyncRoot = new();

    private static readonly WndProc WindowProcedureDelegate =
        WindowProcedure;

    private static ushort _windowClassAtom;
    private static bool _classRegistered;

    private readonly WindowConfig _config;
    private readonly Queue<WindowEvent> _events = new();

    private nint _windowHandle;
    private nint _moduleHandle;

    private bool _open;
    private bool _disposed;

    public bool IsOpen =>
        _open;

    public int Width { get; private set; }

    public int Height { get; private set; }

    public string Title { get; private set; }

    nint IWindowBackend.NativeHandle =>
        _windowHandle;

    public WindowsWindowBackend(
        WindowConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException(
                "WindowsWindowBackend requires Windows.");
        }

        _config = config;

        Width = config.Width;
        Height = config.Height;
        Title = config.Title;
    }

    public void Open()
    {
        ThrowIfDisposed();

        if (_open)
        {
            return;
        }

        RegisterWindowClass();

        _windowHandle =
            CreateNativeWindow();

        if (_windowHandle == nint.Zero)
        {
            int error =
                Marshal.GetLastWin32Error();

            throw new InvalidOperationException(
                $"Failed to create the native Windows window. Win32 error: {error}.");
        }

        GCHandle handle =
            GCHandle.Alloc(this);

        SetWindowLongPtr(
            _windowHandle,
            GWLP_USERDATA,
            GCHandle.ToIntPtr(handle));

        ShowWindow(
            _windowHandle,
            SW_SHOW);

        UpdateWindow(
            _windowHandle);

        _open = true;
    }

    public void Close()
    {
        if (_disposed)
        {
            return;
        }

        if (_windowHandle == nint.Zero)
        {
            _open = false;
            return;
        }

        nint windowHandle =
            _windowHandle;

        _open = false;
        _windowHandle = nint.Zero;

        nint userData =
            GetWindowLongPtr(
                windowHandle,
                GWLP_USERDATA);

        if (userData != nint.Zero)
        {
            GCHandle handle =
                GCHandle.FromIntPtr(userData);

            SetWindowLongPtr(
                windowHandle,
                GWLP_USERDATA,
                nint.Zero);

            if (handle.IsAllocated)
            {
                handle.Free();
            }
        }

        DestroyWindow(
            windowHandle);
    }

    public void ProcessEvents()
    {
        ThrowIfDisposed();

        if (!_open)
        {
            return;
        }

        while (PeekMessage(
                   out MSG message,
                   nint.Zero,
                   0,
                   0,
                   PM_REMOVE))
        {
            if (message.message == WM_QUIT)
            {
                _open = false;
                _windowHandle = nint.Zero;
                break;
            }

            TranslateMessage(
                ref message);

            DispatchMessage(
                ref message);
        }
    }
    
    public bool TryGetEvent(out WindowEvent windowEvent)
    {
        ThrowIfDisposed();

        if (_events.Count == 0)
        {
            windowEvent = default;
            return false;
        }

        windowEvent = _events.Dequeue();
        return true;
    }

    public void Resize(
        int width,
        int height)
    {
        ThrowIfDisposed();

        ValidateSize(
            width,
            height);

        Width = width;
        Height = height;

        if (_windowHandle == nint.Zero)
        {
            return;
        }

        SetWindowSize(
            width,
            height);
    }

    public void SetTitle(
        string title)
    {
        ThrowIfDisposed();

        ArgumentException.ThrowIfNullOrWhiteSpace(
            title);

        Title = title;

        if (_windowHandle != nint.Zero)
        {
            SetWindowText(
                _windowHandle,
                title);
        }
    }

    public void SetVSync(
        bool enabled)
    {
        ThrowIfDisposed();

        // VSync is controlled by the graphics backend.
    }

    public void SetResizable(
        bool enabled)
    {
        ThrowIfDisposed();

        // Runtime style changes will be implemented
        // in the expanded window configuration layer.
    }

    public void SetFullscreen(
        bool enabled)
    {
        ThrowIfDisposed();

        // Fullscreen transitions will be implemented
        // in the expanded window configuration layer.
    }

    public void SetBorderless(
        bool enabled)
    {
        ThrowIfDisposed();

        // Borderless transitions will be implemented
        // in the expanded window configuration layer.
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Close();

        _disposed = true;
    }

    private nint CreateNativeWindow()
    {
        nint classAtom =
            (nint)_windowClassAtom;

        return CreateWindowEx(
            WS_EX_APPWINDOW,
            classAtom,
            Title,
            GetWindowStyle(),
            CW_USEDEFAULT,
            CW_USEDEFAULT,
            Width,
            Height,
            nint.Zero,
            nint.Zero,
            _moduleHandle,
            nint.Zero);
    }

    private uint GetWindowStyle()
    {
        uint style =
            WS_OVERLAPPED |
            WS_CAPTION |
            WS_SYSMENU |
            WS_MINIMIZEBOX;

        if (_config.Resizable)
        {
            style |=
                WS_THICKFRAME |
                WS_MAXIMIZEBOX;
        }

        return style;
    }

    private void SetWindowSize(
        int width,
        int height)
    {
        if (_windowHandle == nint.Zero)
        {
            return;
        }

        SetWindowPos(
            _windowHandle,
            nint.Zero,
            0,
            0,
            width,
            height,
            SWP_NOMOVE |
            SWP_NOZORDER |
            SWP_NOACTIVATE);
    }

    private void RegisterWindowClass()
    {
        lock (SyncRoot)
        {
            if (_classRegistered)
            {
                _moduleHandle =
                    GetModuleHandle(null);

                return;
            }

            _moduleHandle =
                GetModuleHandle(null);

            WNDCLASS windowClass = new()
            {
                style =
                    CS_HREDRAW |
                    CS_VREDRAW,

                lpfnWndProc =
                    WindowProcedureDelegate,

                hInstance =
                    _moduleHandle,

                hIcon =
                    nint.Zero,

                hCursor =
                    nint.Zero,

                hbrBackground =
                    nint.Zero,

                lpszMenuName =
                    null,

                lpszClassName =
                    WindowClassName
            };

            _windowClassAtom =
                RegisterClass(
                    ref windowClass);

            if (_windowClassAtom == 0)
            {
                int error =
                    Marshal.GetLastWin32Error();

                throw new InvalidOperationException(
                    $"Failed to register the Windows window class. Win32 error: {error}.");
            }

            _classRegistered = true;
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }

    private static void ValidateSize(
        int width,
        int height)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(width));
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(height));
        }
    }

    private static nint WindowProcedure(
    nint windowHandle,
    uint message,
    nuint wParam,
    nint lParam)
{
    WindowsWindowBackend? backend = null;

    nint userData = GetWindowLongPtr(
        windowHandle,
        GWLP_USERDATA);

    if (userData != nint.Zero)
    {
        GCHandle handle = GCHandle.FromIntPtr(userData);
        backend = handle.Target as WindowsWindowBackend;
    }

    switch (message)
    {
        case WM_CLOSE:
        {
            backend?._events.Enqueue(
                WindowEvent.CloseRequested());

            DestroyWindow(windowHandle);

            return nint.Zero;
        }
        
        case WM_MOUSEWHEEL:
        {
            if (backend is null)
                break;

            short wheelDelta =
                unchecked((short)((wParam >> 16) & 0xFFFF));

            backend._events.Enqueue(
                WindowEvent.MouseWheel(
                    wheelDelta));

            break;
        }

        case WM_SIZE:
        {
            if (backend is null)
                break;

            long size = lParam.ToInt64();

            int width = (int)(size & 0xFFFF);
            int height = (int)((size >> 16) & 0xFFFF);

            switch (wParam)
            {
                case SIZE_MINIMIZED:
                    backend._events.Enqueue(
                        WindowEvent.Minimized());

                    break;

                case SIZE_MAXIMIZED:
                    if (width > 0 && height > 0)
                    {
                        backend.Width = width;
                        backend.Height = height;

                        backend._events.Enqueue(
                            WindowEvent.Resized(
                                width,
                                height));
                    }

                    backend._events.Enqueue(
                        WindowEvent.Restored(
                            width,
                            height));

                    break;

                case SIZE_RESTORED:
                    if (width > 0 && height > 0)
                    {
                        backend.Width = width;
                        backend.Height = height;

                        backend._events.Enqueue(
                            WindowEvent.Resized(
                                width,
                                height));
                    }

                    break;
            }

            break;
        }

        case WM_DESTROY:
        {
            if (backend is not null)
            {
                backend._open = false;

                backend._events.Enqueue(
                    WindowEvent.Closed());
            }

            PostQuitMessage(0);

            return nint.Zero;
        }
    }

    return DefWindowProc(
        windowHandle,
        message,
        wParam,
        lParam);
}

    [UnmanagedFunctionPointer(
        CallingConvention.Winapi)]
    private delegate nint WndProc(
        nint hWnd,
        uint msg,
        nuint wParam,
        nint lParam);

    [StructLayout(
        LayoutKind.Sequential,
        CharSet = CharSet.Unicode)]
    private struct WNDCLASS
    {
        public uint style;

        [MarshalAs(
            UnmanagedType.FunctionPtr)]
        public WndProc lpfnWndProc;

        public int cbClsExtra;
        public int cbWndExtra;
        public nint hInstance;
        public nint hIcon;
        public nint hCursor;
        public nint hbrBackground;

        [MarshalAs(
            UnmanagedType.LPWStr)]
        public string? lpszMenuName;

        [MarshalAs(
            UnmanagedType.LPWStr)]
        public string lpszClassName;
    }

    [StructLayout(
        LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(
        LayoutKind.Sequential)]
    private struct MSG
    {
        public nint hwnd;
        public uint message;
        public nuint wParam;
        public nint lParam;
        public uint time;
        public POINT point;
    }

    [DllImport(
        "user32.dll",
        CharSet = CharSet.Unicode,
        SetLastError = true)]
    private static extern ushort RegisterClass(
        [In] ref WNDCLASS windowClass);

    [DllImport(
        "kernel32.dll",
        CharSet = CharSet.Unicode)]
    private static extern nint GetModuleHandle(
        string? moduleName);

    [DllImport(
        "user32.dll",
        CharSet = CharSet.Unicode,
        SetLastError = true)]
    private static extern nint CreateWindowEx(
        uint extendedStyle,
        nint className,
        string windowName,
        uint style,
        int x,
        int y,
        int width,
        int height,
        nint parent,
        nint menu,
        nint instance,
        nint parameter);

    [DllImport(
        "user32.dll",
        SetLastError = true)]
    [return: MarshalAs(
        UnmanagedType.Bool)]
    private static extern bool DestroyWindow(
        nint windowHandle);

    [DllImport(
        "user32.dll")]
    private static extern nint DefWindowProc(
        nint windowHandle,
        uint message,
        nuint wParam,
        nint lParam);

    [DllImport(
        "user32.dll")]
    private static extern nint GetWindowLongPtr(
        nint windowHandle,
        int index);

    [DllImport(
        "user32.dll")]
    private static extern nint SetWindowLongPtr(
        nint windowHandle,
        int index,
        nint newLong);

    [DllImport(
        "user32.dll")]
    [return: MarshalAs(
        UnmanagedType.Bool)]
    private static extern bool ShowWindow(
        nint windowHandle,
        int command);

    [DllImport(
        "user32.dll")]
    [return: MarshalAs(
        UnmanagedType.Bool)]
    private static extern bool UpdateWindow(
        nint windowHandle);

    [DllImport(
        "user32.dll",
        CharSet = CharSet.Unicode)]
    [return: MarshalAs(
        UnmanagedType.Bool)]
    private static extern bool SetWindowText(
        nint windowHandle,
        string text);

    [DllImport(
        "user32.dll")]
    [return: MarshalAs(
        UnmanagedType.Bool)]
    private static extern bool SetWindowPos(
        nint windowHandle,
        nint insertAfter,
        int x,
        int y,
        int width,
        int height,
        uint flags);

    [DllImport(
        "user32.dll")]
    [return: MarshalAs(
        UnmanagedType.Bool)]
    private static extern bool PeekMessage(
        out MSG message,
        nint windowHandle,
        uint minimumMessage,
        uint maximumMessage,
        uint removeMessage);

    [DllImport(
        "user32.dll")]
    [return: MarshalAs(
        UnmanagedType.Bool)]
    private static extern bool TranslateMessage(
        ref MSG message);

    [DllImport(
        "user32.dll")]
    private static extern nint DispatchMessage(
        ref MSG message);

    [DllImport(
        "user32.dll")]
    private static extern void PostQuitMessage(
        int exitCode);
}