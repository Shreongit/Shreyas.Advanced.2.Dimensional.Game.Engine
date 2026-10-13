using EngineRuntime = SA2DGE.Engine.Core.Engine;
using SA2DGE.Engine.Core;
using SA2DGE.Engine.Platform.Window;
using SA2DGE.Engine.Platform.Input;
using SA2DGE.Engine.Graphics.Buffers;
using SA2DGE.Engine.Graphics.Shaders;
using SA2DGE.Engine.Graphics;
using SA2DGE.Engine.Math;

internal struct Vertex
{
public float PositionX;
public float PositionY;
public float PositionZ;


public float ColorR;
public float ColorG;
public float ColorB;
public float ColorA;

public Vertex(
    float positionX,
    float positionY,
    float positionZ,
    float colorR,
    float colorG,
    float colorB,
    float colorA)
{
    PositionX = positionX;
    PositionY = positionY;
    PositionZ = positionZ;

    ColorR = colorR;
    ColorG = colorG;
    ColorB = colorB;
    ColorA = colorA;
}


}

internal sealed class RuntimeGame : Game
{
    private VertexBuffer? _vertexBuffer;
    private IndexBuffer? _indexBuffer;
    private VertexArray? _vertexArray;

    private Shader? _vertexShader;
    private Shader? _fragmentShader;
    private ShaderProgram? _shaderProgram;
    private Direct3D11Renderer2D? _renderer;


public RuntimeGame()
    : base(
        new SA2DGE.Engine.Platform.Window.WindowConfig
        {
            Title = "SA2DGE Stage 2.5",
            Width = 1280,
            Height = 720,
            VSync = true,
            Resizable = true,
            Fullscreen = false,
            Borderless = false
        })
{
}

public override void Initialize()
{
    Console.WriteLine("SA2DGE initialized.");

    /*
     * Vertex layout:
     *
     * Position = Float3 = 12 bytes
     * Color    = Float4 = 16 bytes
     *
     * Total vertex size = 28 bytes
     */

    Vertex[] vertices =
    {
        new Vertex(
            -0.5f, -0.5f, 0.0f,
            1.0f, 0.0f, 0.0f, 1.0f),

        new Vertex(
             0.5f, -0.5f, 0.0f,
            0.0f, 1.0f, 0.0f, 1.0f),

        new Vertex(
            -0.5f,  0.5f, 0.0f,
            0.0f, 0.0f, 1.0f, 1.0f),

        new Vertex(
             0.5f,  0.5f, 0.0f,
            1.0f, 1.0f, 1.0f, 1.0f)
    };

    uint[] indices =
    {
        0, 2, 1,
        2, 3, 1
    };
    
    _renderer = new Direct3D11Renderer2D(Graphics!);
    _renderer.Initialize();
    
   

    Console.WriteLine("Stage 3 Renderer2D initialized.");

    _vertexBuffer = Graphics!.CreateVertexBuffer(
        vertexCount: 4,
        vertexSize: 28);

    _vertexBuffer.SetData(vertices);

    _indexBuffer = Graphics.CreateIndexBuffer(
        indexCount: 6);

    _indexBuffer.SetData(indices);

    _vertexArray = Graphics.CreateVertexArray();

    _vertexArray.AddVertexBuffer(
        _vertexBuffer);

    _vertexArray.SetIndexBuffer(
        _indexBuffer);

    /*
     * Vertex layout:
     *
     * POSITION:
     *   Float3
     *   Offset = 0
     *
     * COLOR:
     *   Float4
     *   Offset = 12
     */

    VertexLayout layout = new();

    layout.Add(
        new VertexAttribute(
            "POSITION",
            0,
            VertexAttributeType.Float3,
            0));

    layout.Add(
        new VertexAttribute(
            "COLOR",
            0,
            VertexAttributeType.Float4,
            12));

    Console.WriteLine(
        $"Vertex layout created. Stride = {layout.Stride} bytes.");

    const string vertexShaderSource = """
                                      struct VSInput
                                      {
                                          float3 Position : POSITION;
                                          float4 Color : COLOR;
                                      };

                                      struct VSOutput
                                      {
                                          float4 Position : SV_POSITION;
                                          float4 Color : COLOR;
                                      };

                                      VSOutput main(VSInput input)
                                      {
                                          VSOutput output;

                                          output.Position = float4(
                                              input.Position,
                                              1.0f);

                                          output.Color = input.Color;

                                          return output;
                                      }
                                      """;

    const string fragmentShaderSource = """
                                        struct PSInput
                                        {
                                            float4 Position : SV_POSITION;
                                            float4 Color : COLOR;
                                        };

                                        float4 main(PSInput input) : SV_TARGET
                                        {
                                            return input.Color;
                                        }
                                        """;

    _vertexShader =
        Graphics.CreateVertexShader(
            vertexShaderSource);

    _fragmentShader =
        Graphics.CreateFragmentShader(
            fragmentShaderSource);

    _vertexShader.Compile();
    _fragmentShader.Compile();

    _vertexArray!.SetLayout(
        layout,
        _vertexShader.Bytecode.Span);

    _shaderProgram =
        Graphics.CreateShaderProgram();
    
    

    _shaderProgram.Attach(_vertexShader);
    _shaderProgram.Attach(_fragmentShader);

    _shaderProgram.Link();

    Console.WriteLine(
        "D3D11 shaders compiled and linked successfully.");

    Console.WriteLine(
        "Stage 2.5 graphics resources initialized.");
}

public override void Update(float deltaTime)
{
    if (Input.Mouse.DeltaX != 0.0f ||
        Input.Mouse.DeltaY != 0.0f)
    {
        Console.WriteLine(
            $"Mouse Move: X={Input.Mouse.X}, Y={Input.Mouse.Y}, " +
            $"DeltaX={Input.Mouse.DeltaX}, DeltaY={Input.Mouse.DeltaY}");
    }

    if (Input.Mouse.IsPressed(MouseButton.Left))
    {
        Console.WriteLine("LEFT MOUSE PRESSED");
    }

    if (Input.Mouse.IsDown(MouseButton.Left))
    {
        Console.WriteLine("LEFT MOUSE HELD");
    }

    if (Input.Mouse.IsReleased(MouseButton.Left))
    {
        Console.WriteLine("LEFT MOUSE RELEASED");
    }

    if (Input.Keyboard.IsPressed(InputKey.Escape))
    {
        Engine.Stop();
    }

    if (Input.Mouse.WheelDelta != 0.0f)
    {
        Console.WriteLine(
            $"Mouse Wheel: {Input.Mouse.WheelDelta}");
    }

    if (Gamepad.IsConnected)
    {
        Console.WriteLine(
            $"Gamepad: " +
            $"LX={Gamepad.LeftStickX:F2} " +
            $"LY={Gamepad.LeftStickY:F2} " +
            $"RX={Gamepad.RightStickX:F2} " +
            $"RY={Gamepad.RightStickY:F2} " +
            $"LT={Gamepad.LeftTrigger:F2} " +
            $"RT={Gamepad.RightTrigger:F2}");

        if (Gamepad.IsDown(GamepadButton.A))
        {
            Console.WriteLine("GAMEPAD A");
        }

        if (Gamepad.IsDown(GamepadButton.B))
        {
            Console.WriteLine("GAMEPAD B");
        }
    }
}

public override void OnWindowEvent(
    WindowEvent windowEvent)
{
    base.OnWindowEvent(windowEvent);

    Console.WriteLine(
        $"Window Event: {windowEvent.Type} " +
        $"({windowEvent.Width}x{windowEvent.Height})");
}

public override void Render()
{
    Graphics!.Clear(
        new Color(
            20,
            20,
            30,
            255));

    _renderer!.BeginFrame();

    _renderer.DrawRectangle(
        new Vector2(640.0f, 360.0f),
        new Vector2(400.0f, 250.0f),
        new Color(
            255,
            0,
            0,
            255));
    // Layer 0: drawn first.
    _renderer.DrawRectangle(
        new Vector2(500.0f, 300.0f),
        new Vector2(300.0f, 180.0f),
        new Color(1.0f, 0.0f, 0.0f, 1.0f),
        layer: 0);

// Layer 1: drawn over the red rectangle.
    _renderer.DrawRectangle(
        new Vector2(500.0f, 300.0f),
        new Vector2(220.0f, 130.0f),
        new Color(0.0f, 1.0f, 0.0f, 1.0f),
        layer: 1);

// Layer 2: rotated and drawn on top.
    _renderer.DrawRectangle(
        new Vector2(500.0f, 300.0f),
        new Vector2(150.0f, 70.0f),
        new Color(0.0f, 0.0f, 1.0f, 1.0f),
        layer: 2,
        rotation: 0.785398f);

    _renderer.EndFrame();
    _renderer.Present();
}

public override void Shutdown()
{
    
    _renderer?.Shutdown();
    _renderer?.Dispose();
    _renderer = null;
    _shaderProgram?.Dispose();
    _shaderProgram = null;

    _vertexShader?.Dispose();
    _vertexShader = null;

    _fragmentShader?.Dispose();
    _fragmentShader = null;

    _vertexArray?.Dispose();
    _vertexArray = null;

    _indexBuffer?.Dispose();
    _indexBuffer = null;

    _vertexBuffer?.Dispose();
    _vertexBuffer = null;

    Console.WriteLine(
        "SA2DGE shutdown.");
}


}

internal static class Program
{
private static void Main()
{
using RuntimeGame game = new();


    EngineRuntime.Run(game);
}


}
