using SA2DGE.Engine.Graphics.Buffers;
using SA2DGE.Engine.Graphics.Shaders;
using SA2DGE.Engine.Graphics.Textures;
using SA2DGE.Engine.Math;
using System.Runtime.InteropServices;

namespace SA2DGE.Engine.Graphics;

public sealed class Direct3D11Renderer2D : Renderer2D
{
    private readonly GraphicsBackend _graphics;

    private VertexBuffer? _rectangleVertexBuffer;
    private IndexBuffer? _rectangleIndexBuffer;
    private VertexArray? _rectangleVertexArray;

    private Shader? _rectangleVertexShader;
    private Shader? _rectangleFragmentShader;
    private ShaderProgram? _rectangleShaderProgram;

    private ConstantBuffer? _rectangleTransformBuffer;
    private ConstantBuffer? _rectangleColorBuffer;

    private bool _rectanglePipelineInitialized;
    
    
    // Sprite pipeline resources
    private VertexBuffer? _spriteVertexBuffer;
    private IndexBuffer? _spriteIndexBuffer;
    private VertexArray? _spriteVertexArray;

    private Shader? _spriteVertexShader;
    private Shader? _spriteFragmentShader;
    private ShaderProgram? _spriteShaderProgram;

    private ConstantBuffer? _spriteTransformBuffer;
    private ConstantBuffer? _spriteColorBuffer;

    private bool _spritePipelineInitialized;


    private Matrix4 _projectionMatrix;

    public Direct3D11Renderer2D(GraphicsBackend graphics)
    {
        ArgumentNullException.ThrowIfNull(graphics);

        _graphics = graphics;
    }

    public override string Name =>
        "SA2DGE Direct3D11 Renderer2D";

    protected override void OnInitialize()
    {
        if (!_graphics.IsInitialized)
        {
            throw new InvalidOperationException(
                "The graphics backend must be initialized before the 2D renderer.");
        }

        if (_graphics.Resources is not GraphicsResources resources)
        {
            throw new InvalidOperationException(
                "Graphics resources are not available.");
        }

        _projectionMatrix = Matrix4.CreateOrthographicOffCenter(
            0.0f,
            _graphics.Width,
            _graphics.Height,
            0.0f,
            -1.0f,
            1.0f);

        _rectangleTransformBuffer =
            resources.CreateConstantBuffer(64);

        _rectangleColorBuffer =
            resources.CreateConstantBuffer(16);

        CreateRectanglePipeline();
        
        CreateSpritePipeline();

        Console.WriteLine(
            "Stage 3 Renderer2D initialized.");
    }

    protected override void BeginFrame2D()
    {
        if (_graphics.Commands is null)
        {
            throw new InvalidOperationException(
                "Graphics commands are not available.");
        }

        _graphics.Commands.Reset();
    }

    protected override void EndFrame2D()
    {
    }

    protected override void ExecuteCommand(RenderCommand command)
    {
        switch (command.Type)
        {
            case RenderCommandType.Rectangle:
                ExecuteRectangle(command);
                break;

            case RenderCommandType.Sprite:
                ExecuteSprite(command);
                break;

            case RenderCommandType.Circle:
                ExecuteCircle(command);
                break;

            case RenderCommandType.Line:
                ExecuteLine(command);
                break;

            case RenderCommandType.Text:
                ExecuteText(command);
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(command),
                    command.Type,
                    "Unsupported 2D render command.");
        }
    }

    protected override void OnPresent()
    {
        _graphics.Present();
    }

    protected override void OnShutdown()
    {
        _graphics.Commands?.Reset();

        _rectangleColorBuffer?.Dispose();
        _rectangleColorBuffer = null;

        _rectangleTransformBuffer?.Dispose();
        _rectangleTransformBuffer = null;

        _rectangleShaderProgram?.Dispose();
        _rectangleShaderProgram = null;

        _rectangleVertexShader?.Dispose();
        _rectangleVertexShader = null;

        _rectangleFragmentShader?.Dispose();
        _rectangleFragmentShader = null;

        _rectangleVertexArray?.Dispose();
        _rectangleVertexArray = null;

        _rectangleIndexBuffer?.Dispose();
        _rectangleIndexBuffer = null;

        _rectangleVertexBuffer?.Dispose();
        _rectangleVertexBuffer = null;

        _rectanglePipelineInitialized = false;
        
        
        _spriteColorBuffer?.Dispose();
        _spriteColorBuffer = null;

        _spriteTransformBuffer?.Dispose();
        _spriteTransformBuffer = null;

        _spriteShaderProgram?.Dispose();
        _spriteShaderProgram = null;

        _spriteVertexShader?.Dispose();
        _spriteVertexShader = null;

        _spriteFragmentShader?.Dispose();
        _spriteFragmentShader = null;

        _spriteVertexArray?.Dispose();
        _spriteVertexArray = null;

        _spriteIndexBuffer?.Dispose();
        _spriteIndexBuffer = null;

        _spriteVertexBuffer?.Dispose();
        _spriteVertexBuffer = null;

        _spritePipelineInitialized = false;

    }

    private void CreateRectanglePipeline()
    {
        if (_rectanglePipelineInitialized)
        {
            return;
        }

        if (_graphics.Commands is null)
        {
            throw new InvalidOperationException(
                "Graphics commands are not available.");
        }

        if (_graphics.Resources is null)
        {
            throw new InvalidOperationException(
                "Graphics resources are not available.");
        }

        if (_graphics.Shaders is null)
        {
            throw new InvalidOperationException(
                "Graphics shaders are not available.");
        }

        if (_rectangleTransformBuffer is null)
        {
            throw new InvalidOperationException(
                "The rectangle transform buffer is not initialized.");
        }

        Vertex[] vertices =
        {
            new Vertex(
                -0.5f,
                -0.5f,
                0.0f,
                1.0f,
                1.0f,
                1.0f,
                1.0f),

            new Vertex(
                0.5f,
                -0.5f,
                0.0f,
                1.0f,
                1.0f,
                1.0f,
                1.0f),

            new Vertex(
                -0.5f,
                0.5f,
                0.0f,
                1.0f,
                1.0f,
                1.0f,
                1.0f),

            new Vertex(
                0.5f,
                0.5f,
                0.0f,
                1.0f,
                1.0f,
                1.0f,
                1.0f)
        };

        // The orthographic projection flips Y because the
        // 2D coordinate system starts at the top-left.
        //
        // Therefore the triangle winding must be reversed
        // for Direct3D11's default clockwise front-face rule.
        uint[] indices =
        {
            0, 1, 2,
            2, 1, 3
        };

        _rectangleVertexBuffer =
            _graphics.CreateVertexBuffer(
                vertices.Length,
                28);

        _rectangleVertexBuffer.SetData(vertices);

        _rectangleIndexBuffer =
            _graphics.CreateIndexBuffer(
                indices.Length);

        _rectangleIndexBuffer.SetData(indices);

        _rectangleVertexArray =
            _graphics.CreateVertexArray();

        _rectangleVertexArray.AddVertexBuffer(
            _rectangleVertexBuffer);

        _rectangleVertexArray.SetIndexBuffer(
            _rectangleIndexBuffer);

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

        const string vertexShaderSource = """
            cbuffer TransformBuffer : register(b0)
            {
                float4x4 Transform;
            };

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

                output.Position =
                    mul(
                        float4(input.Position, 1.0f),
                        Transform);

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

                                            cbuffer ColorBuffer : register(b0)
                                            {
                                                float4 Color;
                                            };

                                            float4 main(PSInput input) : SV_TARGET
                                            {
                                                return input.Color * Color;
                                            }
                                            """;

        _rectangleVertexShader =
            _graphics.CreateVertexShader(
                vertexShaderSource);

        _rectangleFragmentShader =
            _graphics.CreateFragmentShader(
                fragmentShaderSource);

        _rectangleVertexShader.Compile();
        _rectangleFragmentShader.Compile();

        _rectangleVertexArray.SetLayout(
            layout,
            _rectangleVertexShader.Bytecode.Span);

        _rectangleShaderProgram =
            _graphics.CreateShaderProgram();

        _rectangleShaderProgram.Attach(
            _rectangleVertexShader);

        _rectangleShaderProgram.Attach(
            _rectangleFragmentShader);

        _rectangleShaderProgram.Link();

        _rectanglePipelineInitialized = true;
    }
    
    
    private void CreateSpritePipeline()
    {
        if (_spritePipelineInitialized)
            return;

        if (_graphics.Resources is not GraphicsResources resources)
            throw new InvalidOperationException(
                "Graphics resources are not available.");

        if (_graphics.Shaders is null)
            throw new InvalidOperationException(
                "Graphics shaders are not available.");

        _spriteTransformBuffer =
            resources.CreateConstantBuffer(64);

        _spriteColorBuffer =
            resources.CreateConstantBuffer(16);

        // Position (3 floats) + Color (4 floats) + UV (2 floats).
        // Total stride: 36 bytes.
        
        SpriteVertex[] vertices =
        {
            // Top-left
            new SpriteVertex(
                -0.5f, -0.5f, 0.0f,
                1.0f, 1.0f, 1.0f, 1.0f,
                0.0f, 0.0f),

            // Top-right
            new SpriteVertex(
                0.5f, -0.5f, 0.0f,
                1.0f, 1.0f, 1.0f, 1.0f,
                1.0f, 0.0f),

            // Bottom-left
            new SpriteVertex(
                -0.5f, 0.5f, 0.0f,
                1.0f, 1.0f, 1.0f, 1.0f,
                0.0f, 1.0f),

            // Bottom-right
            new SpriteVertex(
                0.5f, 0.5f, 0.0f,
                1.0f, 1.0f, 1.0f, 1.0f,
                1.0f, 1.0f)
        };

        uint[] indices =
        {
            0, 1, 2,
            2, 1, 3
        };

        _spriteVertexBuffer =
            _graphics.CreateVertexBuffer(vertices.Length, 36);
        _spriteVertexBuffer.SetData(vertices);

        _spriteIndexBuffer =
            _graphics.CreateIndexBuffer(indices.Length);
        _spriteIndexBuffer.SetData(indices);

        _spriteVertexArray = _graphics.CreateVertexArray();
        _spriteVertexArray.AddVertexBuffer(_spriteVertexBuffer);
        _spriteVertexArray.SetIndexBuffer(_spriteIndexBuffer);

        VertexLayout layout = new();

        layout.Add(new VertexAttribute(
            "POSITION", 0, VertexAttributeType.Float3, 0));

        layout.Add(new VertexAttribute(
            "COLOR", 0, VertexAttributeType.Float4, 12));

        layout.Add(new VertexAttribute(
            "TEXCOORD", 0, VertexAttributeType.Float2, 28));


        

        const string vertexShaderSource = """
            cbuffer TransformBuffer : register(b0)
            {
                float4x4 Transform;
            };

            struct VSInput
            {
                float3 Position : POSITION;
                float4 Color : COLOR;
                float2 TexCoord : TEXCOORD;
            };

            struct VSOutput
            {
                float4 Position : SV_POSITION;
                float4 Color : COLOR;
                float2 TexCoord : TEXCOORD;
            };

            VSOutput main(VSInput input)
            {
                VSOutput output;
                output.Position = mul(float4(input.Position, 1.0f), Transform);
                output.Color = input.Color;
                output.TexCoord = input.TexCoord;
                return output;
            }
            """;

        const string fragmentShaderSource = """
            Texture2D SpriteTexture : register(t0);
            SamplerState SpriteSampler : register(s0);

            cbuffer ColorBuffer : register(b0)
            {
                float4 Color;
            };

            struct PSInput
            {
                float4 Position : SV_POSITION;
                float4 Color : COLOR;
                float2 TexCoord : TEXCOORD;
            };

            float4 main(PSInput input) : SV_TARGET
            {
                return SpriteTexture.Sample(
                    SpriteSampler, input.TexCoord) * input.Color * Color;
            }
            """;

        _spriteVertexShader =
            _graphics.CreateVertexShader(vertexShaderSource);

        _spriteFragmentShader =
            _graphics.CreateFragmentShader(fragmentShaderSource);

        _spriteVertexShader.Compile();
        _spriteFragmentShader.Compile();

        _spriteVertexArray.SetLayout(
            layout,
            _spriteVertexShader.Bytecode.Span);

        _spriteShaderProgram = _graphics.CreateShaderProgram();
        _spriteShaderProgram.Attach(_spriteVertexShader);
        _spriteShaderProgram.Attach(_spriteFragmentShader);
        _spriteShaderProgram.Link();

        _spritePipelineInitialized = true;

        Console.WriteLine("Sprite pipeline created.");
        
    }


    
    private void UpdateRectangleTransform(RenderCommand command)
    {
        if (_rectangleTransformBuffer is null)
        {
            throw new InvalidOperationException(
                "The rectangle transform buffer is not initialized.");
        }

        if (_graphics.Width <= 0 || _graphics.Height <= 0)
        {
            throw new InvalidOperationException(
                "Renderer dimensions must be positive.");
        }

        float cos = MathF.Cos(command.Rotation);
        float sin = MathF.Sin(command.Rotation);

        float sx = command.Size.X;
        float sy = command.Size.Y;

        float screenWidth = _graphics.Width;
        float screenHeight = _graphics.Height;

        // Row-major CPU matrix. The shader below uses mul(vector, matrix).
        float[] values =
        {
            2.0f * cos * sx / screenWidth,
            -2.0f * sin * sy / screenWidth,
            0.0f,
            2.0f * command.Position.X / screenWidth - 1.0f,

            -2.0f * sin * sx / screenHeight,
            -2.0f * cos * sy / screenHeight,
            0.0f,
            1.0f - 2.0f * command.Position.Y / screenHeight,

            0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f
        };

        ReadOnlySpan<byte> bytes =
            MemoryMarshal.AsBytes<float>(values);

        _rectangleTransformBuffer.SetData(bytes);
    }
    
    
    private void UpdateSpriteTransform(
        RenderCommand command)
    {
        if (_spriteTransformBuffer is null)
        {
            throw new InvalidOperationException(
                "The sprite transform buffer is not initialized.");
        }

        Matrix4 scale =
            Matrix4.CreateScale(
                new Vector3(
                    command.Size.X,
                    command.Size.Y,
                    1.0f));

        Matrix4 rotation =
            Matrix4.CreateRotationZ(
                command.Rotation);

        Matrix4 translation =
            Matrix4.CreateTranslation(
                new Vector3(
                    command.Position.X,
                    command.Position.Y,
                    0.0f));

        Matrix4 transform =
            _projectionMatrix
            * translation
            * rotation
            * scale;

        float[] values =
        {
            transform.M11,
            transform.M12,
            transform.M13,
            transform.M14,

            transform.M21,
            transform.M22,
            transform.M23,
            transform.M24,

            transform.M31,
            transform.M32,
            transform.M33,
            transform.M34,

            transform.M41,
            transform.M42,
            transform.M43,
            transform.M44
        };

        ReadOnlySpan<byte> bytes =
            MemoryMarshal.AsBytes<float>(values);

        _spriteTransformBuffer.SetData(bytes);
    }



    private void UpdateRectangleColor(
        RenderCommand command)
    {
        if (_rectangleColorBuffer is null)
        {
            throw new InvalidOperationException(
                "The rectangle color buffer is not initialized.");
        }

        
        float[] values =
        {
            command.Color.R,
            command.Color.G,
            command.Color.B,
            command.Color.A
        };


        ReadOnlySpan<byte> bytes =
            MemoryMarshal.AsBytes<float>(
                values);

        _rectangleColorBuffer.SetData(
            bytes);
    }

    private void ExecuteRectangle(
        RenderCommand command)
    {
        if (!_rectanglePipelineInitialized ||
            _rectangleVertexArray is null ||
            _rectangleShaderProgram is null ||
            _rectangleTransformBuffer is null ||
            _rectangleColorBuffer is null ||
            _graphics.Commands is null)
        {
            throw new InvalidOperationException(
                "The rectangle rendering pipeline is not initialized.");
        }

        Console.WriteLine(
            $"RECTANGLE DRAW: X={command.Position.X}, " +
            $"Y={command.Position.Y}, " +
            $"W={command.Size.X}, " +
            $"H={command.Size.Y}");

        UpdateRectangleTransform(command);
        UpdateRectangleColor(command);

        _graphics.Commands.SetVertexArray(
            _rectangleVertexArray);

        _graphics.Commands.SetShaderProgram(
            _rectangleShaderProgram);

        _graphics.Commands.SetVertexConstantBuffer(
            0,
            _rectangleTransformBuffer);

        _graphics.Commands.SetPixelConstantBuffer(
            0,
            _rectangleColorBuffer);

        _graphics.Commands.DrawIndexed(6);
    }

    private void ExecuteSprite(RenderCommand command)
    {
        if (!_spritePipelineInitialized ||
            _spriteVertexArray is null ||
            _spriteShaderProgram is null ||
            _spriteTransformBuffer is null ||
            _spriteColorBuffer is null ||
            _graphics.Commands is null)
        {
            throw new InvalidOperationException(
                "The sprite rendering pipeline is not initialized.");
        }

        if (command.Resource is not Texture2D texture)
        {
            throw new ArgumentException(
                "A sprite render command must contain a Texture2D resource.",
                nameof(command));
        }

        UpdateSpriteTransform(command);

        float[] color =
        {
            command.Color.R,
            command.Color.G,
            command.Color.B,
            command.Color.A
        };

        ReadOnlySpan<byte> colorBytes =
            MemoryMarshal.AsBytes<float>(color);

        _spriteColorBuffer.SetData(colorBytes);

        texture.Bind(0);

        _graphics.Commands.SetVertexArray(
            _spriteVertexArray);

        _graphics.Commands.SetShaderProgram(
            _spriteShaderProgram);

        _graphics.Commands.SetVertexConstantBuffer(
            0,
            _spriteTransformBuffer);

        _graphics.Commands.SetPixelConstantBuffer(
            0,
            _spriteColorBuffer);

        _graphics.Commands.SetAlphaBlending(true);

        _graphics.Commands.SetAlphaBlending(true);

        try
        {
            texture.Bind(0);

            _graphics.Commands.SetVertexArray(_spriteVertexArray);
            _graphics.Commands.SetShaderProgram(_spriteShaderProgram);

            _graphics.Commands.SetVertexConstantBuffer(
                0,
                _spriteTransformBuffer);

            _graphics.Commands.SetPixelConstantBuffer(
                0,
                _spriteColorBuffer);

            _graphics.Commands.DrawIndexed(6);
        }
        finally
        {
            texture.Unbind();
            _graphics.Commands.SetAlphaBlending(false);
        }
    }

    private static void ExecuteCircle(
        RenderCommand command)
    {
        throw new NotImplementedException(
            "Circle rendering is not implemented yet.");
    }

    private static void ExecuteLine(
        RenderCommand command)
    {
        throw new NotImplementedException(
            "Line rendering is not implemented yet.");
    }

    private static void ExecuteText(
        RenderCommand command)
    {
        throw new NotImplementedException(
            "Text rendering is not implemented yet.");
    }

    private readonly struct Vertex
    {
        public readonly float PositionX;
        public readonly float PositionY;
        public readonly float PositionZ;

        public readonly float ColorR;
        public readonly float ColorG;
        public readonly float ColorB;
        public readonly float ColorA;

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
    
    
    private readonly struct SpriteVertex
    {
        public readonly float PositionX;
        public readonly float PositionY;
        public readonly float PositionZ;

        public readonly float ColorR;
        public readonly float ColorG;
        public readonly float ColorB;
        public readonly float ColorA;

        public readonly float U;
        public readonly float V;

        public SpriteVertex(
            float positionX,
            float positionY,
            float positionZ,
            float colorR,
            float colorG,
            float colorB,
            float colorA,
            float u,
            float v)
        {
            PositionX = positionX;
            PositionY = positionY;
            PositionZ = positionZ;

            ColorR = colorR;
            ColorG = colorG;
            ColorB = colorB;
            ColorA = colorA;

            U = u;
            V = v;
        }
    }

}