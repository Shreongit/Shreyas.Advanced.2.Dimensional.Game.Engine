using System.Runtime.InteropServices;
using SA2DGEWindow = SA2DGE.Engine.Platform.Window.Window;

namespace SA2DGE.Engine.Platform.Input;

public sealed class WindowsInputBackend : IInputBackend
{
    private bool _initialized;
    private bool _disposed;
    private SA2DGEWindow? _window;

    public void Initialize(SA2DGEWindow window)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(window);

        if (_initialized)
            return;

        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException(
                "WindowsInputBackend requires Windows.");
        }

        _window = window;
        _initialized = true;
    }

    public void Update(Input input)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(input);

        if (!_initialized)
        {
            throw new InvalidOperationException(
                "The Windows input backend has not been initialized.");
        }

        UpdateKeyboard(input);
        UpdateMouse(input);
        UpdateGamepad();
    }

    public void Shutdown()
    {
        if (_disposed || !_initialized)
            return;

        Gamepad.Clear();

        _initialized = false;
        _window = null;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        Shutdown();
        _disposed = true;
    }

    private static void UpdateKeyboard(Input input)
    {
        UpdateKey(input, InputKey.A, VK_A);
        UpdateKey(input, InputKey.B, VK_B);
        UpdateKey(input, InputKey.C, VK_C);
        UpdateKey(input, InputKey.D, VK_D);
        UpdateKey(input, InputKey.E, VK_E);
        UpdateKey(input, InputKey.F, VK_F);
        UpdateKey(input, InputKey.G, VK_G);
        UpdateKey(input, InputKey.H, VK_H);
        UpdateKey(input, InputKey.I, VK_I);
        UpdateKey(input, InputKey.J, VK_J);
        UpdateKey(input, InputKey.K, VK_K);
        UpdateKey(input, InputKey.L, VK_L);
        UpdateKey(input, InputKey.M, VK_M);
        UpdateKey(input, InputKey.N, VK_N);
        UpdateKey(input, InputKey.O, VK_O);
        UpdateKey(input, InputKey.P, VK_P);
        UpdateKey(input, InputKey.Q, VK_Q);
        UpdateKey(input, InputKey.R, VK_R);
        UpdateKey(input, InputKey.S, VK_S);
        UpdateKey(input, InputKey.T, VK_T);
        UpdateKey(input, InputKey.U, VK_U);
        UpdateKey(input, InputKey.V, VK_V);
        UpdateKey(input, InputKey.W, VK_W);
        UpdateKey(input, InputKey.X, VK_X);
        UpdateKey(input, InputKey.Y, VK_Y);
        UpdateKey(input, InputKey.Z, VK_Z);

        UpdateKey(input, InputKey.Number0, VK_0);
        UpdateKey(input, InputKey.Number1, VK_1);
        UpdateKey(input, InputKey.Number2, VK_2);
        UpdateKey(input, InputKey.Number3, VK_3);
        UpdateKey(input, InputKey.Number4, VK_4);
        UpdateKey(input, InputKey.Number5, VK_5);
        UpdateKey(input, InputKey.Number6, VK_6);
        UpdateKey(input, InputKey.Number7, VK_7);
        UpdateKey(input, InputKey.Number8, VK_8);
        UpdateKey(input, InputKey.Number9, VK_9);

        UpdateKey(input, InputKey.Escape, VK_ESCAPE);
        UpdateKey(input, InputKey.Enter, VK_RETURN);
        UpdateKey(input, InputKey.Tab, VK_TAB);
        UpdateKey(input, InputKey.Backspace, VK_BACK);
        UpdateKey(input, InputKey.Space, VK_SPACE);

        UpdateKey(input, InputKey.Left, VK_LEFT);
        UpdateKey(input, InputKey.Up, VK_UP);
        UpdateKey(input, InputKey.Right, VK_RIGHT);
        UpdateKey(input, InputKey.Down, VK_DOWN);

        UpdateKey(input, InputKey.LeftShift, VK_LSHIFT);
        UpdateKey(input, InputKey.RightShift, VK_RSHIFT);
        UpdateKey(input, InputKey.LeftControl, VK_LCONTROL);
        UpdateKey(input, InputKey.RightControl, VK_RCONTROL);
        UpdateKey(input, InputKey.LeftAlt, VK_LMENU);
        UpdateKey(input, InputKey.RightAlt, VK_RMENU);

        UpdateKey(input, InputKey.Insert, VK_INSERT);
        UpdateKey(input, InputKey.Delete, VK_DELETE);
        UpdateKey(input, InputKey.Home, VK_HOME);
        UpdateKey(input, InputKey.End, VK_END);
        UpdateKey(input, InputKey.PageUp, VK_PRIOR);
        UpdateKey(input, InputKey.PageDown, VK_NEXT);

        UpdateKey(input, InputKey.CapsLock, VK_CAPITAL);
        UpdateKey(input, InputKey.NumLock, VK_NUMLOCK);
        UpdateKey(input, InputKey.ScrollLock, VK_SCROLL);

        UpdateKey(input, InputKey.F1, VK_F1);
        UpdateKey(input, InputKey.F2, VK_F2);
        UpdateKey(input, InputKey.F3, VK_F3);
        UpdateKey(input, InputKey.F4, VK_F4);
        UpdateKey(input, InputKey.F5, VK_F5);
        UpdateKey(input, InputKey.F6, VK_F6);
        UpdateKey(input, InputKey.F7, VK_F7);
        UpdateKey(input, InputKey.F8, VK_F8);
        UpdateKey(input, InputKey.F9, VK_F9);
        UpdateKey(input, InputKey.F10, VK_F10);
        UpdateKey(input, InputKey.F11, VK_F11);
        UpdateKey(input, InputKey.F12, VK_F12);
    }

    private void UpdateMouse(Input input)
    {
        if (_window is not null &&
            GetCursorPos(out POINT point))
        {
            if (ScreenToClient(
                    _window.NativeHandle,
                    ref point))
            {
                input.SetMousePosition(
                    point.X,
                    point.Y);
            }
        }

        UpdateMouseButton(
            input,
            MouseButton.Left,
            VK_LBUTTON);

        UpdateMouseButton(
            input,
            MouseButton.Right,
            VK_RBUTTON);

        UpdateMouseButton(
            input,
            MouseButton.Middle,
            VK_MBUTTON);

        UpdateMouseButton(
            input,
            MouseButton.XButton1,
            VK_XBUTTON1);

        UpdateMouseButton(
            input,
            MouseButton.XButton2,
            VK_XBUTTON2);
    }

    private static void UpdateGamepad()
    {
        const uint userIndex = 0;

        uint result = XInputGetState(
            userIndex,
            out XINPUT_STATE state);

        if (result != ERROR_SUCCESS)
        {
            Gamepad.Clear();
            return;
        }

        Gamepad.IsConnected = true;

        XINPUT_GAMEPAD gamepad = state.Gamepad;

        Gamepad.LeftStickX =
            NormalizeStick(gamepad.ThumbLX);

        Gamepad.LeftStickY =
            NormalizeStick(gamepad.ThumbLY);

        Gamepad.RightStickX =
            NormalizeStick(gamepad.ThumbRX);

        Gamepad.RightStickY =
            NormalizeStick(gamepad.ThumbRY);

        Gamepad.LeftTrigger =
            gamepad.LeftTrigger / XINPUT_MAX_TRIGGER;

        Gamepad.RightTrigger =
            gamepad.RightTrigger / XINPUT_MAX_TRIGGER;

        Gamepad.SetButtonState(
            GamepadButton.A,
            HasButton(
                gamepad.Buttons,
                XINPUT_GAMEPAD_A));

        Gamepad.SetButtonState(
            GamepadButton.B,
            HasButton(
                gamepad.Buttons,
                XINPUT_GAMEPAD_B));

        Gamepad.SetButtonState(
            GamepadButton.X,
            HasButton(
                gamepad.Buttons,
                XINPUT_GAMEPAD_X));

        Gamepad.SetButtonState(
            GamepadButton.Y,
            HasButton(
                gamepad.Buttons,
                XINPUT_GAMEPAD_Y));

        Gamepad.SetButtonState(
            GamepadButton.LeftShoulder,
            HasButton(
                gamepad.Buttons,
                XINPUT_GAMEPAD_LEFT_SHOULDER));

        Gamepad.SetButtonState(
            GamepadButton.RightShoulder,
            HasButton(
                gamepad.Buttons,
                XINPUT_GAMEPAD_RIGHT_SHOULDER));

        Gamepad.SetButtonState(
            GamepadButton.LeftStick,
            HasButton(
                gamepad.Buttons,
                XINPUT_GAMEPAD_LEFT_THUMB));

        Gamepad.SetButtonState(
            GamepadButton.RightStick,
            HasButton(
                gamepad.Buttons,
                XINPUT_GAMEPAD_RIGHT_THUMB));

        Gamepad.SetButtonState(
            GamepadButton.Start,
            HasButton(
                gamepad.Buttons,
                XINPUT_GAMEPAD_START));

        Gamepad.SetButtonState(
            GamepadButton.Back,
            HasButton(
                gamepad.Buttons,
                XINPUT_GAMEPAD_BACK));

        Gamepad.SetButtonState(
            GamepadButton.DPadUp,
            HasButton(
                gamepad.Buttons,
                XINPUT_GAMEPAD_DPAD_UP));

        Gamepad.SetButtonState(
            GamepadButton.DPadDown,
            HasButton(
                gamepad.Buttons,
                XINPUT_GAMEPAD_DPAD_DOWN));

        Gamepad.SetButtonState(
            GamepadButton.DPadLeft,
            HasButton(
                gamepad.Buttons,
                XINPUT_GAMEPAD_DPAD_LEFT));

        Gamepad.SetButtonState(
            GamepadButton.DPadRight,
            HasButton(
                gamepad.Buttons,
                XINPUT_GAMEPAD_DPAD_RIGHT));
    }

    private static bool HasButton(
        ushort buttons,
        uint button)
    {
        return (buttons & button) != 0;
    }

    private static float NormalizeStick(short value)
    {
        return global::System.Math.Clamp(
            value / XINPUT_MAX_THUMB,
            -1.0f,
            1.0f);
    }

    private static void UpdateKey(
        Input input,
        InputKey key,
        int virtualKey)
    {
        short state = GetAsyncKeyState(
            virtualKey);

        bool isDown =
            (state & 0x8000) != 0;

        input.SetKeyState(
            key,
            isDown);
    }

    private static void UpdateMouseButton(
        Input input,
        MouseButton button,
        int virtualKey)
    {
        short state = GetAsyncKeyState(
            virtualKey);

        bool isDown =
            (state & 0x8000) != 0;

        input.SetMouseButtonState(
            button,
            isDown);
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }

    [DllImport(
        "user32.dll",
        SetLastError = false)]
    private static extern short GetAsyncKeyState(
        int virtualKey);

    [DllImport(
        "user32.dll",
        SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(
        out POINT point);

    [DllImport(
        "user32.dll",
        SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ScreenToClient(
        nint hWnd,
        ref POINT point);

    [DllImport(
        "xinput1_4.dll",
        CallingConvention = CallingConvention.StdCall)]
    private static extern uint XInputGetState(
        uint userIndex,
        out XINPUT_STATE state);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct XINPUT_STATE
    {
        public uint PacketNumber;
        public XINPUT_GAMEPAD Gamepad;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct XINPUT_GAMEPAD
    {
        public ushort Buttons;
        public byte LeftTrigger;
        public byte RightTrigger;
        public short ThumbLX;
        public short ThumbLY;
        public short ThumbRX;
        public short ThumbRY;
    }

    private const uint ERROR_SUCCESS = 0;

    private const int VK_BACK = 0x08;
    private const int VK_TAB = 0x09;
    private const int VK_RETURN = 0x0D;
    private const int VK_CAPITAL = 0x14;

    private const int VK_ESCAPE = 0x1B;
    private const int VK_SPACE = 0x20;
    private const int VK_PRIOR = 0x21;
    private const int VK_NEXT = 0x22;
    private const int VK_END = 0x23;
    private const int VK_HOME = 0x24;
    private const int VK_LEFT = 0x25;
    private const int VK_UP = 0x26;
    private const int VK_RIGHT = 0x27;
    private const int VK_DOWN = 0x28;
    private const int VK_INSERT = 0x2D;
    private const int VK_DELETE = 0x2E;

    private const int VK_0 = 0x30;
    private const int VK_1 = 0x31;
    private const int VK_2 = 0x32;
    private const int VK_3 = 0x33;
    private const int VK_4 = 0x34;
    private const int VK_5 = 0x35;
    private const int VK_6 = 0x36;
    private const int VK_7 = 0x37;
    private const int VK_8 = 0x38;
    private const int VK_9 = 0x39;

    private const int VK_A = 0x41;
    private const int VK_B = 0x42;
    private const int VK_C = 0x43;
    private const int VK_D = 0x44;
    private const int VK_E = 0x45;
    private const int VK_F = 0x46;
    private const int VK_G = 0x47;
    private const int VK_H = 0x48;
    private const int VK_I = 0x49;
    private const int VK_J = 0x4A;
    private const int VK_K = 0x4B;
    private const int VK_L = 0x4C;
    private const int VK_M = 0x4D;
    private const int VK_N = 0x4E;
    private const int VK_O = 0x4F;
    private const int VK_P = 0x50;
    private const int VK_Q = 0x51;
    private const int VK_R = 0x52;
    private const int VK_S = 0x53;
    private const int VK_T = 0x54;
    private const int VK_U = 0x55;
    private const int VK_V = 0x56;
    private const int VK_W = 0x57;
    private const int VK_X = 0x58;
    private const int VK_Y = 0x59;
    private const int VK_Z = 0x5A;

    private const int VK_LSHIFT = 0xA0;
    private const int VK_RSHIFT = 0xA1;
    private const int VK_LCONTROL = 0xA2;
    private const int VK_RCONTROL = 0xA3;
    private const int VK_LMENU = 0xA4;
    private const int VK_RMENU = 0xA5;

    private const int VK_NUMLOCK = 0x90;
    private const int VK_SCROLL = 0x91;

    private const int VK_F1 = 0x70;
    private const int VK_F2 = 0x71;
    private const int VK_F3 = 0x72;
    private const int VK_F4 = 0x73;
    private const int VK_F5 = 0x74;
    private const int VK_F6 = 0x75;
    private const int VK_F7 = 0x76;
    private const int VK_F8 = 0x77;
    private const int VK_F9 = 0x78;
    private const int VK_F10 = 0x79;
    private const int VK_F11 = 0x7A;
    private const int VK_F12 = 0x7B;

    private const int VK_LBUTTON = 0x01;
    private const int VK_RBUTTON = 0x02;
    private const int VK_MBUTTON = 0x04;
    private const int VK_XBUTTON1 = 0x05;
    private const int VK_XBUTTON2 = 0x06;

    private const uint XINPUT_GAMEPAD_DPAD_UP = 0x0001;
    private const uint XINPUT_GAMEPAD_DPAD_DOWN = 0x0002;
    private const uint XINPUT_GAMEPAD_DPAD_LEFT = 0x0004;
    private const uint XINPUT_GAMEPAD_DPAD_RIGHT = 0x0008;

    private const uint XINPUT_GAMEPAD_START = 0x0010;
    private const uint XINPUT_GAMEPAD_BACK = 0x0020;

    private const uint XINPUT_GAMEPAD_LEFT_THUMB = 0x0040;
    private const uint XINPUT_GAMEPAD_RIGHT_THUMB = 0x0080;

    private const uint XINPUT_GAMEPAD_LEFT_SHOULDER = 0x0100;
    private const uint XINPUT_GAMEPAD_RIGHT_SHOULDER = 0x0200;

    private const uint XINPUT_GAMEPAD_A = 0x1000;
    private const uint XINPUT_GAMEPAD_B = 0x2000;
    private const uint XINPUT_GAMEPAD_X = 0x4000;
    private const uint XINPUT_GAMEPAD_Y = 0x8000;

    private const float XINPUT_MAX_THUMB = 32767.0f;
    private const float XINPUT_MAX_TRIGGER = 255.0f;
}