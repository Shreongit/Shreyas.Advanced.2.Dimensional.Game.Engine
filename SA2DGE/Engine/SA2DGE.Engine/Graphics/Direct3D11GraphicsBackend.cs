using SA2DGE.Engine.Math;
using SA2DGE.Engine.Platform.Window;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace SA2DGE.Engine.Graphics;

internal sealed class Direct3D11GraphicsBackend : GraphicsBackend
{
    private ID3D11Device? _device;
    private ID3D11DeviceContext? _context;

    private IDXGIFactory2? _factory;
    private IDXGISwapChain1? _swapChain;

    private ID3D11RenderTargetView? _renderTargetView;

    

    protected override void InitializeBackend(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        CreateDevice();
        CreateFactory();
        CreateSwapChain(window);
        CreateRenderTarget();

        if (_device is null || _context is null)
        {
            throw new InvalidOperationException(
                "Direct3D 11 device initialization failed.");
        }

        Resources = new Direct3D11GraphicsResources(
            _device,
            _context);

        Commands = new Direct3D11GraphicsCommands(
            _context);
        
        Shaders = new Direct3D11GraphicsShaders(
            _device,
            _context);
    }

    protected override void ResizeBackend(
        int width,
        int height)
    {
        if (_swapChain is null || _context is null)
        {
            throw new InvalidOperationException(
                "The Direct3D 11 backend is not initialized.");
        }

        ReleaseRenderTarget();

        _context.Flush();

        var result = _swapChain.ResizeBuffers(
            2,
            checked((uint)width),
            checked((uint)height),
            Format.R8G8B8A8_UNorm,
            SwapChainFlags.None);

        result.CheckError();

        CreateRenderTargetForSize(width, height);
    }

    protected override void ClearBackend(Color color)
    {
        if (_context is null)
            throw new InvalidOperationException(
                "The Direct3D 11 device context is not initialized.");

        if (_renderTargetView is null)
            throw new InvalidOperationException(
                "The Direct3D 11 render target view is not initialized.");

        Console.WriteLine(
            $"CLEAR: R={color.R} G={color.G} B={color.B} A={color.A}");

        _context.OMSetRenderTargets(
            new ID3D11RenderTargetView[]
            {
                _renderTargetView
            },
            null);

        _context.RSSetViewport(
            0.0f,
            0.0f,
            Width,
            Height,
            0.0f,
            1.0f);

        _context.ClearRenderTargetView(
            _renderTargetView,
            new Vortice.Mathematics.Color4(
                color.R,
                color.G,
                color.B,
                color.A));

        _context.Flush();
    }

        

    protected override void PresentBackend()
    {
        if (_swapChain is null)
            throw new InvalidOperationException(
                "The Direct3D 11 swap chain is not initialized.");

        var result = _swapChain.Present(
            0u,
            PresentFlags.None);

        result.CheckError();
    }

    protected override void ShutdownBackend()
    {
        Commands?.Dispose();
        Commands = null;
        
        Shaders = null;

        Resources = null;

        ReleaseRenderTarget();

        _swapChain?.Dispose();
        _swapChain = null;

        _factory?.Dispose();
        _factory = null;

        _context?.ClearState();
        _context?.Flush();

        _context?.Dispose();
        _context = null;

        _device?.Dispose();
        _device = null;
    }

    private void CreateDevice()
    {
        FeatureLevel[] featureLevels =
        {
            FeatureLevel.Level_11_1,
            FeatureLevel.Level_11_0,
            FeatureLevel.Level_10_1,
            FeatureLevel.Level_10_0
        };

        var result = D3D11.D3D11CreateDevice(
            null,
            DriverType.Hardware,
            DeviceCreationFlags.BgraSupport,
            featureLevels,
            out ID3D11Device? device,
            out FeatureLevel _,
            out ID3D11DeviceContext? context);

        if (result.Failure)
        {
            device?.Dispose();
            context?.Dispose();

            result = D3D11.D3D11CreateDevice(
                null,
                DriverType.Warp,
                DeviceCreationFlags.BgraSupport,
                featureLevels,
                out device,
                out _,
                out context);
        }

        result.CheckError();

        _device = device
            ?? throw new InvalidOperationException(
                "Failed to create the Direct3D 11 device.");

        _context = context
            ?? throw new InvalidOperationException(
                "Failed to create the Direct3D 11 device context.");

        Console.WriteLine("D3D11 device initialized.");
    }

    private void CreateFactory()
    {
        _factory = DXGI.CreateDXGIFactory2<IDXGIFactory2>(false);
    }

    private void CreateSwapChain(Window window)
    {
        if (_factory is null || _device is null)
        {
            throw new InvalidOperationException(
                "The Direct3D 11 graphics infrastructure is not initialized.");
        }

        SwapChainDescription1 description = new()
        {
            Width = checked((uint)window.Width),
            Height = checked((uint)window.Height),
            Format = Format.R8G8B8A8_UNorm,
            Stereo = false,
            SampleDescription = new SampleDescription(1, 0),
            BufferUsage = Usage.RenderTargetOutput,
            BufferCount = 2,
            Scaling = Scaling.Stretch,
            SwapEffect = SwapEffect.FlipDiscard,
            AlphaMode = AlphaMode.Ignore,
            Flags = SwapChainFlags.None
        };

        _swapChain = _factory.CreateSwapChainForHwnd(
            _device,
            window.NativeHandle,
            description);

        Console.WriteLine(
            $"D3D11 flip swap chain created: {window.Width}x{window.Height}");
    }

    private void CreateRenderTarget()
    {
        if (_swapChain is null ||
            _device is null ||
            _context is null)
        {
            throw new InvalidOperationException(
                "The Direct3D 11 graphics infrastructure is not initialized.");
        }

        using ID3D11Texture2D backBuffer =
            _swapChain.GetBuffer<ID3D11Texture2D>(0);

        _renderTargetView =
            _device.CreateRenderTargetView(backBuffer);

        _context.RSSetViewport(
            0.0f,
            0.0f,
            Width,
            Height,
            0.0f,
            1.0f);
    }
    
    private void CreateRenderTargetForSize(
        int width,
        int height)
    {
        if (_swapChain is null ||
            _device is null ||
            _context is null)
        {
            throw new InvalidOperationException(
                "The Direct3D 11 graphics infrastructure is not initialized.");
        }

        using ID3D11Texture2D backBuffer =
            _swapChain.GetBuffer<ID3D11Texture2D>(0);

        _renderTargetView =
            _device.CreateRenderTargetView(backBuffer);

        _context.RSSetViewport(
            0.0f,
            0.0f,
            width,
            height,
            0.0f,
            1.0f);
    }

    private void ReleaseRenderTarget()
    {
        if (_context is not null)
        {
            _context.OMSetRenderTargets(
                Array.Empty<ID3D11RenderTargetView>(),
                null);
        }

        _renderTargetView?.Dispose();
        _renderTargetView = null;
    }
}