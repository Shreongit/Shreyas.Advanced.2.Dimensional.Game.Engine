using EngineRuntime = SA2DGE.Engine.Core.Engine;
using SA2DGE.Engine.Core;
using StbImageSharp;
using SA2DGE.Engine.Platform.Window;
using SA2DGE.Engine.Platform.Input;
using SA2DGE.Engine.Graphics.Buffers;
using SA2DGE.Engine.Graphics.Textures;
using SA2DGE.Engine.Graphics.Shaders;
using SA2DGE.Engine.Graphics;
using SA2DGE.Engine.Math;
using System;
using System.Collections.Generic;
using System.Linq;

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
    private Texture2D? _soldierTexture;
    private BackgroundLayer? _backgroundLayer;
    private SpriteSheet? _soldierSpriteSheet;
    private SpriteAnimation? _testAnimation;
    private SpriteAnimationController? _animationController;
    private PlayerController? _playerController;
    private const float SpeedAdjustment = 30.0f;
    private const float MinimumMoveSpeed = 0.0f;
    private const float MaximumMoveSpeed = 600.0f;
    private readonly EnvironmentManager _environmentManager = new();
    


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


private Texture2D LoadPngTexture(string filePath)
{
    using var stream = File.OpenRead(filePath);

    var image = StbImageSharp.ImageResult.FromStream(
        stream,
        StbImageSharp.ColorComponents.RedGreenBlueAlpha);

    Console.WriteLine(
        $"Loaded PNG: {image.Width}x{image.Height}");

    return Graphics!.CreateTexture2D(
        image.Width,
        image.Height,
        TextureFormat.RGBA8,
        image.Data);
}


public override void Initialize()
{
    Console.WriteLine("SA2DGE initialized.");
    Console.WriteLine("  3 = Crawl animation");

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
    string backgroundPath = Path.GetFullPath(
        Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "..",
            "Assets",
            "Backgrounds",
            "robot_warfare_background.png"));

    _backgroundLayer = new BackgroundLayer(
        LoadPngTexture(backgroundPath),
        layer: 0);

    Console.WriteLine(
        "Stage 4.14: Background layer initialized.");

    Console.WriteLine("Stage 4.13: Background texture loaded.");
    string soldierPath = Path.GetFullPath(
        Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "..",
            "Assets",
            "Sprites",
            "Assault-Class.png"));

    _soldierTexture = LoadPngTexture(soldierPath);
    
    _soldierSpriteSheet = new SpriteSheet(
        columns: 5,
        rows: 7);
    
    _testSpriteSheet = new SpriteSheet(
        columns: 2,
        rows: 2);
    
    _animationController = new SpriteAnimationController();

    _animationController.AddAnimation(
        "Idle",
        new SpriteAnimation(
            _soldierSpriteSheet!,
            frameDuration: 0.8f,
            loop: true,
            startFrame: 0,
            frameCount: 2));

    _animationController.AddAnimation(
        "Walk",
        new SpriteAnimation(
            _soldierSpriteSheet!,
            frameDuration: 0.25f,
            loop: true,
            startFrame: 5,
            frameCount: 2));

    _animationController.AddAnimation(
        "Crawl",
        new SpriteAnimation(
            _soldierSpriteSheet!,
            frameDuration: 0.25f,
            loop: true,
            startFrame: 10,
            frameCount: 2));

    _animationController.Play("Idle");

    _testAnimation = _animationController.CurrentAnimation
                     ?? throw new InvalidOperationException(
                         "The animation controller failed to select the Idle animation.");
    
    _playerController = new PlayerController(
        animations: _animationController,
        movementInput: new KeyboardPlayerMovementInput(Input.Keyboard),
        initialPosition: new Vector2(
            Graphics!.Width / 2.0f,
            Graphics.Height / 2.0f),
        moveSpeed: 180.0f,
        windowWidth: Graphics.Width,
        windowHeight: Graphics.Height,
        spriteWidth: 80.0f,
        spriteHeight: 112.0f);
    
    _environmentManager.GenerateRandomized(
        playerPosition: _playerController.Position,
        windowWidth: Graphics!.Width,
        windowHeight: Graphics.Height,
        seed: 42);

    _environmentManager.SynchronizeCollisionBounds(_playerController);

    Console.WriteLine(
        "Stage 4.7.5: Idle, Walk, and Run animations registered.");
    
    Console.WriteLine("Controls:");
    Console.WriteLine("  1 = Idle animation");
    Console.WriteLine("  2 = Walk animation");
    Console.WriteLine("  3 = Run animation");
    Console.WriteLine("  P = Play");
    Console.WriteLine("  O = Pause");
    Console.WriteLine("  K = Stop");
    Console.WriteLine("  R = Restart");
    Console.WriteLine("  L = Toggle looping");
    Console.WriteLine("  4 / 5 = Select animation frame");

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
    
    
    // Test 9: Animation stays within its assigned frame range.
    var rangeTest = new SpriteAnimation(
        _testSpriteSheet!,
        frameDuration: 0.5f,
        loop: true,
        startFrame: 1,
        frameCount: 2);

    if (rangeTest.CurrentFrame != 1)
        throw new Exception("FAIL: Frame range start.");

    rangeTest.Play();
    rangeTest.Update(0.5f);

    if (rangeTest.CurrentFrame != 2)
        throw new Exception("FAIL: Frame range advancement.");

    rangeTest.Update(0.5f);

    if (rangeTest.CurrentFrame != 1)
        throw new Exception("FAIL: Frame range loop boundary.");

    Console.WriteLine("PASS: Animation stays within its assigned frame range.");

    // Test 10: Manual selection cannot escape the assigned range.
    bool rejectedOutOfRangeFrame = false;

    try
    {
        rangeTest.SetFrame(3);
    }
    catch (ArgumentOutOfRangeException)
    {
        rejectedOutOfRangeFrame = true;
    }

    if (!rejectedOutOfRangeFrame)
        throw new Exception("FAIL: Frame range validation.");

    Console.WriteLine("PASS: Manual frame selection respects range boundaries.");

}



public override void Update(float deltaTime)
{
    
    
    if (_playerController is not null && Graphics is not null)
    {
        _playerController.ResizeWindow(
            Graphics.Width,
            Graphics.Height);

        _playerController.Update(deltaTime);
    }

    _testAnimation = _playerController?.CurrentAnimation;

    
    
    if (_playerController is not null)
    {
        if (Input.Keyboard.IsPressed(InputKey.F2))
        {
            _playerController.MoveSpeed = Math.Max(
                MinimumMoveSpeed,
                _playerController.MoveSpeed - SpeedAdjustment);

            Console.WriteLine(
                $"Player speed: {_playerController.MoveSpeed:F0}");
        }

        if (Input.Keyboard.IsPressed(InputKey.F1))
        {
            _playerController.MoveSpeed = Math.Min(
                MaximumMoveSpeed,
                _playerController.MoveSpeed + SpeedAdjustment);

            Console.WriteLine(
                $"Player speed: {_playerController.MoveSpeed:F0}");
        }
    }
    
    

    
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
        
        
        if (Input.Keyboard.IsPressed(InputKey.Number4))
        {
            _testAnimation.Pause();

            int previousFrame = _testAnimation.CurrentFrame - 1;

            if (previousFrame < _testAnimation.StartFrame)
            {
                previousFrame = _testAnimation.Loop
                    ? _testAnimation.EndFrame
                    : _testAnimation.StartFrame;
            }

            _testAnimation.SetFrame(previousFrame);

            Console.WriteLine(
                $"Animation: Frame {_testAnimation.CurrentFrame}");
        }


        
        if (Input.Keyboard.IsPressed(InputKey.Number5))
        {
            _testAnimation.Pause();

            int nextFrame = _testAnimation.CurrentFrame + 1;

            if (nextFrame > _testAnimation.EndFrame)
            {
                nextFrame = _testAnimation.Loop
                    ? _testAnimation.StartFrame
                    : _testAnimation.EndFrame;
            }

            _testAnimation.SetFrame(nextFrame);

            Console.WriteLine(
                $"Animation: Frame {_testAnimation.CurrentFrame}");
        }

        
        if (_animationController is not null)
        {
            if (Input.Keyboard.IsPressed(InputKey.Number1))
            {
                _animationController.Play("Idle");
                _testAnimation = _animationController.CurrentAnimation;
                Console.WriteLine("Active animation: Idle");
            }

            if (Input.Keyboard.IsPressed(InputKey.Number2))
            {
                _animationController.Play("Walk");
                _testAnimation = _animationController.CurrentAnimation;
                Console.WriteLine("Active animation: Walk");
            }

            if (Input.Keyboard.IsPressed(InputKey.Number3))
            {
                _animationController.Play("Crawl");
                _testAnimation = _animationController.CurrentAnimation;
                Console.WriteLine("Active animation: Crawl");
            }
        }
        
        
    }
    
    
    if (Input.Mouse.DeltaX != 0.0f ||
        Input.Mouse.DeltaY != 0.0f)
    {
        //Console.WriteLine(
            //$"Mouse Move: X={Input.Mouse.X}, Y={Input.Mouse.Y}, " +
            //$"DeltaX={Input.Mouse.DeltaX}, DeltaY={Input.Mouse.DeltaY}");
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

public override void OnWindowEvent(WindowEvent windowEvent)
{
    base.OnWindowEvent(windowEvent);

    Console.WriteLine(
        $"Window Event: {windowEvent.Type} " +
        $"({windowEvent.Width}x{windowEvent.Height})");

    if (_playerController is not null &&
        windowEvent.Width > 0 &&
        windowEvent.Height > 0)
    {
        _playerController.ResizeWindow(
            windowEvent.Width,
            windowEvent.Height);
    }
}



public override void Render()
{
    Graphics!.Clear(
        new Color(20, 20, 30, 255));

    _renderer!.BeginFrame();
    
    _backgroundLayer?.Draw(
        _renderer!,
        Graphics!.Width,
        Graphics.Height);
    
    _environmentManager.Draw(_renderer!);

    // Layer 3: Magenta top-left UV region of the test texture.
    
    if (_playerController is not null &&
        _soldierTexture is not null &&
        _soldierSpriteSheet is not null)
    {
        _renderer.DrawSpriteFrame(
            position: _playerController.Position,
            size: new Vector2(80.0f, 112.0f),
            color: new Color(1.0f, 1.0f, 1.0f, 1.0f),
            texture: _soldierTexture,
            spriteSheet: _soldierSpriteSheet,
            frameIndex: _playerController.CurrentFrame,
            layer: 3,
            flipHorizontal: _playerController.IsFacingLeft);
    }
    _renderer.EndFrame();
    _renderer.Present();
}



public override void Shutdown()
{
    _testTexture?.Dispose();
    _testTexture = null;
    
    _backgroundLayer?.Dispose();
    _backgroundLayer = null;
    
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
