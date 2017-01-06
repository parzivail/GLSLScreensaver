// Created by Colby on 11/09/2016
// Location: LifePanel/LifePanel/MainWindow.cs

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Newtonsoft.Json;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GLSLScreensaver
{
    internal class MainWindow : GameWindow
    {
        private bool _shouldDie;
        public static readonly Vector2 Resolution = new Vector2(DisplayDevice.Default.Width, DisplayDevice.Default.Height);

        readonly ScreensaverShader _shader;
        readonly UniformDynamicTime _uniformDynamicTime = new UniformDynamicTime();
        private readonly Uniform _uniformResolution;
        private readonly Uniform _uniformMouse = new Uniform("mouse");

        public static Config Config { get; set; }

        public MainWindow() : base((int)Resolution.X, (int)Resolution.Y, new GraphicsMode(32, 24, 0, 8))
        {
            Load += LoadHandler;
            Resize += ResizeHandler;
            UpdateFrame += UpdateHandler;
            RenderFrame += RenderHandler;

            Config = new Config
            {
                Shader = ":random"
            };

            _shader = new ScreensaverShader(Config.Shader);
            _uniformMouse.Value = new Vector2(Config.MouseX, Config.MouseY);
            _uniformResolution = new Uniform("resolution") {Value = Resolution/Config.Speed};
            ClientRectangle = new Rectangle(ClientRectangle.X, ClientRectangle.Y, (int)(ClientRectangle.Width / Config.Speed), (int)(ClientRectangle.Height / Config.Speed));
        }

        public void LoadHandler(object sender, EventArgs e)
        {
            _shader.InitProgram();

            WindowState = WindowState.Fullscreen;
        }

        private void ResizeHandler(object sender, EventArgs e)
        {
            GL.Viewport(ClientRectangle);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, 1, 0, 1, -0.1f, 0.1f);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Texture2D);
        }

        private void UpdateHandler(object sender, FrameEventArgs e)
        {
            if (_shouldDie || Keyboard[Key.Escape])
                Exit();
        }

        private void RenderHandler(object sender, FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit |
                     ClearBufferMask.DepthBufferBit |
                     ClearBufferMask.StencilBufferBit);
            
            var uniforms = new List<Uniform>
            {
                _uniformDynamicTime,
                _uniformResolution,
                _uniformMouse
            };
            _shader.Use(uniforms.ToArray());

            GL.PushMatrix();
            GL.Begin(BeginMode.Quads);
            GL.Color3(Color.White);
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex2(0f, 1 / Config.Speed);
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex2(1 / Config.Speed, 1 / Config.Speed);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2(1 / Config.Speed, 0f);
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex2(0f, 0f);
            GL.End();
            GL.PopMatrix();

            SwapBuffers();
        }

        public void Kill()
        {
            _shouldDie = true;
        }

        public static void DoTheThingZhuLi()
        {
            var mainWindow = new MainWindow();
            mainWindow.Run(10, 30);
        }
    }

    internal class Config
    {
        [JsonProperty("shaderFileName")]
        public string Shader { get; set; }

        [JsonProperty("qualitySpeedTradeoff")]
        public float Speed { get; set; } = 1;

        [JsonProperty("timeScale")]
        public float TimeScale { get; set; } = 1;

        [JsonProperty("mouseX")]
        public float MouseX { get; set; } = 0.5f;

        [JsonProperty("mouseY")]
        public float MouseY { get; set; } = 0.5f;
    }
}