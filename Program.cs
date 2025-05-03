using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;

namespace App
{
    enum GameColor
    {
        White, Gray, Yellow, Blue, Red, Green,
        Black
    }

    static class ColorHelper
    {
        public static Vector3 GetColorVector(GameColor color)
        {
            switch (color)
            {
                case GameColor.White: return new Vector3(1.0f, 1.0f, 1.0f);
                case GameColor.Gray: return new Vector3(0.5f, 0.5f, 0.5f);
                case GameColor.Yellow: return new Vector3(1.0f, 1.0f, 0.0f);
                case GameColor.Blue: return new Vector3(0.0f, 0.0f, 1.0f);
                case GameColor.Red: return new Vector3(1.0f, 0.0f, 0.0f);
                case GameColor.Green: return new Vector3(0.0f, 1.0f, 0.0f);
                case GameColor.Black: return Vector3.Zero;
                default: return Vector3.Zero;
            }
        }

        public static readonly Vector3 Black = Vector3.Zero;
    }

    struct RectangleCell
    {
        public GameColor CurrentColor;

        public RectangleCell(GameColor color)
        {
            CurrentColor = color;
        }
    }

    struct ColorButton
    {
        public GameColor Color;
        public Vector2 Position;
        public float Width;
        public float Height;
        public bool IsEnabled;

        public ColorButton(GameColor color, Vector2 position, float width, float height)
        {
            Color = color;
            Position = position;
            Width = width;
            Height = height;
            IsEnabled = true;
        }

        public bool IsPointInside(Vector2 point)
        {
            float halfW = Width / 2.0f;
            float halfH = Height / 2.0f;
            return point.X >= Position.X - halfW && point.X <= Position.X + halfW &&
                   point.Y >= Position.Y - halfH && point.Y <= Position.Y + halfH;
        }
    }

    static class TextureLoader
    {
        public static int LoadTexture(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Texture file not found", path);
            }

            int handle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, handle);

            ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            return handle;
        }
    }

    static class Program
    {
        private static int _rectangleVao;
        private static int _rectangleVbo;
        private static int _shaderProgram;
        private static Random _random = new Random();

        private const int GridRows = 4;
        private const int GridCols = 7;
        private static RectangleCell[,] _grid = new RectangleCell[GridRows, GridCols];

        private const float RectWidth = 0.15f;
        private const float RectHeight = 0.15f;
        private const float GridSpacingX = 0.05f;
        private const float GridSpacingY = 0.05f;
        private const float GridOffsetX = -((GridCols - 1) * (RectWidth + GridSpacingX)) / 2.0f;
        private const float GridOffsetY = 0.5f;

        private static List<ColorButton> _colorButtons = new List<ColorButton>();
        private const float ButtonWidth = 0.18f;
        private const float ButtonHeight = 1f;
        private const float ButtonSpacingX = 0.05f;
        private const float ButtonOffsetY = -0.9f;

        private static Dictionary<GameColor, int> _animalTextureIds = new Dictionary<GameColor, int>();
        private static Dictionary<GameColor, int> _buttonTextureIds = new Dictionary<GameColor, int>();
        private static Dictionary<GameColor, string> _colorToAnimal = new Dictionary<GameColor, string>
        {
            { GameColor.White, "horse" },
            { GameColor.Gray, "snake" },
            { GameColor.Yellow, "chicken" },
            { GameColor.Blue, "cat" },
            { GameColor.Red, "dog" },
            { GameColor.Green, "cow" }
        };

        private static int _score = 0;
        private static int _turnsTaken = 0;
        private static bool _gameOver = false;
        private static GameWindow _gameWindowRef;

        private const int PenaltyPerTurn = -5;

        static void Main()
        {
            GameWindowSettings gameWindowSettings = new GameWindowSettings();
            gameWindowSettings.UpdateFrequency = 60;

            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings();
            nativeWindowSettings.Size = new Vector2i(1280, 720);
            nativeWindowSettings.Title = "Vendendo a fazenda game";

            GameWindow gameWindow = new GameWindow(gameWindowSettings, nativeWindowSettings);
            _gameWindowRef = gameWindow;

            gameWindow.Load += OnLoad;
            gameWindow.Unload += OnUnload;

            gameWindow.MouseDown += (MouseButtonEventArgs args) =>
            {
                if (!_gameOver && args.Button == MouseButton.Left && args.Action == InputAction.Press)
                {
                    Vector2 mousePos = gameWindow.MousePosition;
                    float x = (mousePos.X / gameWindow.Size.X) * 2 - 1;
                    float y = -((mousePos.Y / gameWindow.Size.Y) * 2 - 1);
                    Vector2 clickNdc = new Vector2(x, y);

                    for (int i = 0; i < _colorButtons.Count; i++)
                    {
                        ColorButton button = _colorButtons[i];
                        if (button.IsEnabled)
                        {
                            float halfW = button.Width / 2.0f;
                            float halfH = button.Height / 2.0f;
                            float topY = button.Position.Y + halfH;
                            float bottomY = button.Position.Y - halfH;
                            float leftX = button.Position.X - halfW;
                            float rightX = button.Position.X + halfW;

                            bool hit = button.IsPointInside(clickNdc);

                            if (hit)
                            {
                                ProcessTurn(button.Color, i);
                                break;
                            }
                            else
                            {
                            }
                        }
                    }
                }
            };

            gameWindow.KeyUp += (KeyboardKeyEventArgs args) =>
            {
                if (_gameOver && args.Key == Keys.R)
                {
                    ResetGame();
                }
            };

            gameWindow.RenderFrame += OnRenderFrame;

            gameWindow.Run();
        }

        private static void ProcessTurn(GameColor chosenColor, int buttonIndex)
        {
            _turnsTaken++;

            if (_turnsTaken > 1)
            {
                _score += PenaltyPerTurn;
            }

            int pointsPerRectThisTurn = Math.Max(1, 7 - _turnsTaken);

            int rectanglesRemoved = 0;
            int pointsGainedThisTurn = 0;
            for (int row = 0; row < GridRows; row++)
            {
                for (int col = 0; col < GridCols; col++)
                {
                    if (_grid[row, col].CurrentColor == chosenColor)
                    {
                        _grid[row, col].CurrentColor = GameColor.Black;
                        _score += pointsPerRectThisTurn;
                        pointsGainedThisTurn += pointsPerRectThisTurn;
                        rectanglesRemoved++;
                    }
                }
            }

            ColorButton clickedButton = _colorButtons[buttonIndex];
            clickedButton.IsEnabled = false;
            _colorButtons[buttonIndex] = clickedButton;

            UpdateWindowTitle();

            if (_colorButtons.All(b => !b.IsEnabled))
            {
                _gameOver = true;
                UpdateWindowTitle();
            }
        }

        private static void UpdateWindowTitle()
        {
            if (_gameWindowRef != null)
            {
                if (_gameOver)
                {
                     _gameWindowRef.Title = $"Game Over! Voce Fez: ${_score} Reais - Aperte R Para Jogar Denovo";
                }
                else
                {
                    _gameWindowRef.Title = $"Vendendo a Fazenda Game - Score: {_score}";
                }
            }
        }

        private static int CreateRectangle()
        {
            float halfW = 0.5f;
            float halfH = 0.5f;
            float[] vertices = {
                -halfW,  halfH, 0.0f,  0.0f, 0.0f,
                 halfW,  halfH, 0.0f,  1.0f, 0.0f,
                 halfW, -halfH, 0.0f,  1.0f, 1.0f,

                 halfW, -halfH, 0.0f,  1.0f, 1.0f,
                -halfW, -halfH, 0.0f,  0.0f, 1.0f,
                -halfW,  halfH, 0.0f,  0.0f, 0.0f
            };

            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            _rectangleVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _rectangleVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            int stride = 5 * sizeof(float);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            return vao;
        }

        private static void InitializeGrid()
        {
            GameColor[] availableColors = _colorToAnimal.Keys.ToArray();

            for (int row = 0; row < GridRows; row++)
            {
                for (int col = 0; col < GridCols; col++)
                {
                    GameColor randomColor = availableColors[_random.Next(availableColors.Length)];
                    _grid[row, col] = new RectangleCell(randomColor);
                }
            }
        }

        private static void InitializeButtons()
        {
            _colorButtons.Clear();
            GameColor[] buttonColors = _colorToAnimal.Keys.ToArray();

            int numButtons = buttonColors.Length;
            float totalButtonWidth = numButtons * ButtonWidth + (numButtons - 1) * ButtonSpacingX;
            float startX = -totalButtonWidth / 2.0f;

            for (int i = 0; i < numButtons; i++)
            {
                GameColor color = buttonColors[i];
                float buttonCenterX = startX + i * (ButtonWidth + ButtonSpacingX) + ButtonWidth / 2.0f;
                Vector2 position = new Vector2(buttonCenterX, ButtonOffsetY);
                _colorButtons.Add(new ColorButton(color, position, ButtonWidth, ButtonHeight) { IsEnabled = true });
            }
        }

        private static void ResetGame()
        {
            _score = 0;
            _turnsTaken = 0;
            _gameOver = false;
            InitializeGrid();
            InitializeButtons();
            UpdateWindowTitle();
        }

        static void OnLoad()
        {
            _rectangleVao = CreateRectangle();
            SetupShaders();
            LoadAllTextures();
            ResetGame();
        }

        static void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(_rectangleVbo);

            GL.UseProgram(0);
            GL.DeleteProgram(_shaderProgram);

            foreach (var textureId in _animalTextureIds.Values)
            {
                GL.DeleteTexture(textureId);
            }
            foreach (var textureId in _buttonTextureIds.Values)
            {
                GL.DeleteTexture(textureId);
            }
            _animalTextureIds.Clear();
            _buttonTextureIds.Clear();

            GL.DeleteVertexArray(_rectangleVao);
        }

        static void SetupShaders()
        {
            string vertexShaderSource = @"
            #version 330 core
            layout(location = 0) in vec3 aPosition;
            layout(location = 1) in vec2 aTexCoord;

            uniform vec2 uPosition;
            uniform vec2 uScale;

            out vec2 TexCoord;

            void main()
            {
                vec3 scaledPosition = aPosition;
                scaledPosition.xy *= uScale;
                scaledPosition.xy += uPosition;
                gl_Position = vec4(scaledPosition, 1.0);
                TexCoord = aTexCoord;
            }";

            string fragmentShaderSource = @"
            #version 330 core
            out vec4 FragColor;

            in vec2 TexCoord;

            uniform vec3 uColor;
            uniform sampler2D textureSampler;
            uniform bool uRenderSolidColor;

            void main()
            {
                if (uRenderSolidColor)
                {
                    FragColor = vec4(uColor, 1.0);
                }
                else
                {
                    FragColor = texture(textureSampler, TexCoord);
                }
            }";

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource); GL.CompileShader(vertexShader); CheckShaderCompilation(vertexShader);
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource); GL.CompileShader(fragmentShader); CheckShaderCompilation(fragmentShader);
            _shaderProgram = GL.CreateProgram();
            GL.AttachShader(_shaderProgram, vertexShader); GL.AttachShader(_shaderProgram, fragmentShader); GL.LinkProgram(_shaderProgram); CheckProgramLinking(_shaderProgram);
            GL.DeleteShader(vertexShader); GL.DeleteShader(fragmentShader);

            GL.UseProgram(_shaderProgram);
            int samplerLoc = GL.GetUniformLocation(_shaderProgram, "textureSampler");
            GL.Uniform1(samplerLoc, 0);
            GL.UseProgram(0);
        }

        static void LoadAllTextures()
        {
            string textureDir = "Textures";
            if (!Directory.Exists(textureDir))
            {
                Console.WriteLine($"Warning: Texture directory not found at '{Path.GetFullPath(textureDir)}'. Textures will not load.");
                return;
            }

            foreach (var kvp in _colorToAnimal)
            {
                GameColor color = kvp.Key;
                string animal = kvp.Value;

                try {
                    string rectTexturePath = Path.Combine(textureDir, $"{animal}.jpg");
                    _animalTextureIds[color] = TextureLoader.LoadTexture(rectTexturePath);

                    string buttonTexturePath = Path.Combine(textureDir, $"button-{animal}.jpg");
                    _buttonTextureIds[color] = TextureLoader.LoadTexture(buttonTexturePath);
                }
                catch (FileNotFoundException ex) {
                     Console.WriteLine($"Error loading texture: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred loading texture for {animal}: {ex.Message}");
                }
            }
        }

        static void OnRenderFrame(FrameEventArgs args)
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(_shaderProgram);
            GL.BindVertexArray(_rectangleVao);
            GL.ActiveTexture(TextureUnit.Texture0);

            int colorLocation = GL.GetUniformLocation(_shaderProgram, "uColor");
            int positionLocation = GL.GetUniformLocation(_shaderProgram, "uPosition");
            int scaleLocation = GL.GetUniformLocation(_shaderProgram, "uScale");
            int renderSolidColorLocation = GL.GetUniformLocation(_shaderProgram, "uRenderSolidColor");

            Vector2 gridCellScale = new Vector2(RectWidth, RectHeight);
            for (int row = 0; row < GridRows; row++)
            {
                for (int col = 0; col < GridCols; col++)
                {
                    GameColor cellColor = _grid[row, col].CurrentColor;
                    float posX = GridOffsetX + col * (RectWidth + GridSpacingX);
                    float posY = GridOffsetY - row * (RectHeight + GridSpacingY);
                    Vector2 positionOffset = new Vector2(posX, posY);

                    GL.Uniform2(positionLocation, positionOffset);
                    GL.Uniform2(scaleLocation, gridCellScale);

                    if (cellColor == GameColor.Black || !_animalTextureIds.TryGetValue(cellColor, out int textureId))
                    {
                        GL.Uniform1(renderSolidColorLocation, 1);
                        GL.Uniform3(colorLocation, ColorHelper.Black);
                        GL.BindTexture(TextureTarget.Texture2D, 0);
                    }
                    else
                    {
                        GL.Uniform1(renderSolidColorLocation, 0);
                        GL.BindTexture(TextureTarget.Texture2D, textureId);
                    }

                    GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                }
            }

            foreach (var button in _colorButtons)
            {
                Vector2 buttonScale = new Vector2(button.Width, button.Height);
                GL.Uniform2(positionLocation, button.Position);
                GL.Uniform2(scaleLocation, buttonScale);

                if (!button.IsEnabled || !_buttonTextureIds.TryGetValue(button.Color, out int textureId))
                {
                    GL.Uniform1(renderSolidColorLocation, 1);
                    GL.Uniform3(colorLocation, ColorHelper.Black);
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                }
                else
                {
                    GL.Uniform1(renderSolidColorLocation, 0);
                    GL.BindTexture(TextureTarget.Texture2D, textureId);
                }

                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            _gameWindowRef.SwapBuffers();
        }

        static void CheckShaderCompilation(int shader)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                Console.WriteLine($"Shader compilation error: {infoLog}");
            }
        }

        static void CheckProgramLinking(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                Console.WriteLine($"Program linking error: {infoLog}");
            }
        }
    }
}
