using EngineRuntime = SA2DGE.Engine.Core.Engine;
using SA2DGE.Engine.Core;
using SA2DGE.Engine.Platform.Window;
using SA2DGE.Engine.Platform.Input;
using SA2DGE.Engine.Graphics.Buffers;
using SA2DGE.Engine.Graphics.Textures;
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
    private Texture2D? _testTexture;
    private SpriteSheet? _testSpriteSheet;
    private SpriteAnimation? _testAnimation;


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
    
    
    const int textureSize = 64;
    byte[] pixels = new byte[textureSize * textureSize * 4];

    for (int y = 0; y < textureSize; y++)
    {
        for (int x = 0; x < textureSize; x++)
        {
            bool left = x < textureSize / 2;
            bool top = y < textureSize / 2;

            int index = (y * textureSize + x) * 4;

            if (top && left) // Top-left: magenta
            {
                pixels[index] = 255;
                pixels[index + 1] = 0;
                pixels[index + 2] = 255;
            }
            else if (top && !left) // Top-right: green
            {
                pixels[index] = 0;
                pixels[index + 1] = 255;
                pixels[index + 2] = 0;
            }
            else if (!top && left) // Bottom-left: blue
            {
                pixels[index] = 0;
                pixels[index + 1] = 0;
                pixels[index + 2] = 255;
            }
            else // Bottom-right: yellow
            {
                pixels[index] = 255;
                pixels[index + 1] = 255;
                pixels[index + 2] = 0;
            }

            pixels[index + 3] = 255;
        }
    }

    if (Graphics!.Resources is null)
    {
        throw new InvalidOperationException(
            "Graphics resources are unavailable.");
    }

    _testTexture = Graphics.Resources.CreateTexture2D(
        textureSize,
        textureSize,
        TextureFormat.RGBA8,
        pixels);
    
    _testSpriteSheet = new SpriteSheet(
        columns: 2,
        rows: 2);
    
    _testAnimation = new SpriteAnimation(
        spriteSheet: _testSpriteSheet,
        frameDuration: 0.5f,
        loop: true);

    _testAnimation.Play();

    Console.WriteLine("Stage 3 checkerboard texture created.");

    
   

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
    
    TestSpriteAnimation();
}


private void TestSpriteAnimation()
{
    if (_testSpriteSheet is null)
    {
        throw new InvalidOperationException(
            "Sprite sheet must be initialized before testing.");
    }

    // Test 1: Pause preserves the current frame.
    var pauseTest = new SpriteAnimation(
        _testSpriteSheet, 0.5f);

    pauseTest.Play();
    pauseTest.Update(0.5f);

    if (pauseTest.CurrentFrame != 1)
        throw new Exception("FAIL: Frame advancement.");

    pauseTest.Pause();
    pauseTest.Update(1.0f);

    if (pauseTest.CurrentFrame != 1 || pauseTest.IsPlaying)
        throw new Exception("FAIL: Pause.");

    Console.WriteLine("PASS: Pause preserves the current frame.");

    // Test 2: Stop resets the animation.
    pauseTest.Play();
    pauseTest.Stop();

    if (pauseTest.CurrentFrame != 0 || pauseTest.IsPlaying)
        throw new Exception("FAIL: Stop.");

    Console.WriteLine("PASS: Stop resets to frame 0.");

    // Test 3: Non-looping playback stops on the last frame.
    var nonLoopTest = new SpriteAnimation(
        _testSpriteSheet,
        frameDuration: 0.5f,
        loop: false);

    nonLoopTest.Play();
    nonLoopTest.Update(2.0f);

    if (nonLoopTest.CurrentFrame != 3 || nonLoopTest.IsPlaying)
        throw new Exception("FAIL: Non-looping playback.");

    Console.WriteLine("PASS: Non-looping playback stops on the last frame.");

    // Test 4: A large delta advances multiple frames.
    var timingTest = new SpriteAnimation(
        _testSpriteSheet, 0.5f);

    timingTest.Play();
    timingTest.Update(1.1f);

    if (timingTest.CurrentFrame != 2)
        throw new Exception("FAIL: Multi-frame timing.");

    Console.WriteLine("PASS: 1.1 seconds advances two frames.");

    Console.WriteLine("All SpriteAnimation tests passed.");
    
    // Test 5: Manual frame selection resets elapsed time.
    var manualTest = new SpriteAnimation(_testSpriteSheet, 0.5f);
    manualTest.Play();
    manualTest.Update(0.3f);
    manualTest.SetFrame(2);
    manualTest.Update(0.2f);

    if (manualTest.CurrentFrame != 2)
        throw new Exception("FAIL: Manual frame selection timer reset.");

    Console.WriteLine("PASS: Manual frame selection resets elapsed time.");

// Test 6: Looping advances from the last frame to frame 0.
    var loopTest = new SpriteAnimation(
        _testSpriteSheet,
        frameDuration: 0.5f,
        loop: true);

    loopTest.SetFrame(3);
    loopTest.Play();
    loopTest.Update(0.5f);

    if (loopTest.CurrentFrame != 0 || !loopTest.IsPlaying)
        throw new Exception("FAIL: Looping frame boundary.");

    Console.WriteLine("PASS: Looping wraps from the last frame to frame 0.");

// Test 7: Non-looping playback stays on the last frame.
    var boundaryTest = new SpriteAnimation(
        _testSpriteSheet,
        frameDuration: 0.5f,
        loop: false);

    boundaryTest.SetFrame(3);
    boundaryTest.Play();
    boundaryTest.Update(0.5f);

    if (boundaryTest.CurrentFrame != 3 || boundaryTest.IsPlaying)
        throw new Exception("FAIL: Non-looping frame boundary.");

    Console.WriteLine("PASS: Non-looping playback stops at the last frame.");

// Test 8: Restart resets the frame and starts playback.
    var restartTest = new SpriteAnimation(_testSpriteSheet, 0.5f);
    restartTest.SetFrame(2);
    restartTest.Play();
    restartTest.Update(0.3f);
    restartTest.Restart();

    if (restartTest.CurrentFrame != 0 || !restartTest.IsPlaying)
        throw new Exception("FAIL: Restart state.");

    restartTest.Update(0.2f);

    if (restartTest.CurrentFrame != 0)
        throw new Exception("FAIL: Restart did not reset elapsed time.");

    Console.WriteLine("PASS: Restart resets frame, timer, and playback state.");

    Console.WriteLine("All SpriteAnimation behaviour tests passed.");
}


public override void Update(float deltaTime)
{
    
    _testAnimation?.Update(deltaTime);
    
    if (_testAnimation is not null)
    {
        if (Input.Keyboard.IsPressed(InputKey.P))
        {
            _testAnimation.Play();
            Console.WriteLine("Animation: Playing");
        }

        if (Input.Keyboard.IsPressed(InputKey.O))
        {
            _testAnimation.Pause();
            Console.WriteLine("Animation: Paused");
        }

        if (Input.Keyboard.IsPressed(InputKey.K))
        {
            _testAnimation.Stop();
            Console.WriteLine("Animation: Stopped at frame 0");
        }

        if (Input.Keyboard.IsPressed(InputKey.R))
        {
            _testAnimation.Restart();
            Console.WriteLine("Animation: Restarted");
        }

        if (Input.Keyboard.IsPressed(InputKey.L))
        {
            _testAnimation.Loop = !_testAnimation.Loop;
            Console.WriteLine(
                $"Animation: Looping = {_testAnimation.Loop}");
        }
        
        if (Input.Keyboard.IsPressed(InputKey.Left))
        {
            _testAnimation.Pause();

            int previousFrame = _testAnimation.CurrentFrame - 1;

            if (previousFrame < 0)
            {
                previousFrame = _testAnimation.Loop
                    ? _testAnimation.SpriteSheet.FrameCount - 1
                    : 0;
            }

            _testAnimation.SetFrame(previousFrame);

            Console.WriteLine(
                $"Animation: Frame {_testAnimation.CurrentFrame}");
        }

        if (Input.Keyboard.IsPressed(InputKey.Right))
        {
            _testAnimation.Pause();

            int nextFrame = _testAnimation.CurrentFrame + 1;

            if (nextFrame >= _testAnimation.SpriteSheet.FrameCount)
            {
                nextFrame = _testAnimation.Loop
                    ? 0
                    : _testAnimation.SpriteSheet.FrameCount - 1;
            }

            _testAnimation.SetFrame(nextFrame);

            Console.WriteLine(
                $"Animation: Frame {_testAnimation.CurrentFrame}");
        }
    }
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
        new Color(20, 20, 30, 255));

    _renderer!.BeginFrame();

    // Layer 0: Large red background rectangle.
    _renderer.DrawRectangle(
        new Vector2(640.0f, 360.0f),
        new Vector2(400.0f, 250.0f),
        new Color(255, 0, 0, 255));

    // Layer 0: Red rectangle.
    _renderer.DrawRectangle(
        new Vector2(500.0f, 300.0f),
        new Vector2(300.0f, 180.0f),
        new Color(1.0f, 0.0f, 0.0f, 1.0f),
        layer: 0);

    // Layer 1: Green rectangle over the red rectangle.
    _renderer.DrawRectangle(
        new Vector2(500.0f, 300.0f),
        new Vector2(220.0f, 130.0f),
        new Color(0.0f, 1.0f, 0.0f, 1.0f),
        layer: 1);

    // Layer 2: Rotated blue rectangle.
    _renderer.DrawRectangle(
        new Vector2(500.0f, 300.0f),
        new Vector2(150.0f, 70.0f),
        new Color(0.0f, 0.0f, 1.0f, 1.0f),
        layer: 2,
        rotation: 0.785398f);

    // Layer 3: Magenta top-left UV region of the test texture.
    
    if (_testAnimation is not null)
    {
        _renderer.DrawSpriteFrame(
            position: new Vector2(850.0f, 350.0f),
            size: new Vector2(180.0f, 180.0f),
            color: new Color(1.0f, 1.0f, 1.0f, 1.0f),
            texture: _testTexture,
            spriteSheet: _testAnimation.SpriteSheet,
            frameIndex: _testAnimation.CurrentFrame,
            layer: 3);
    }

    _renderer.EndFrame();
    _renderer.Present();
}



public override void Shutdown()
{
    _testTexture?.Dispose();
    _testTexture = null;
    
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
