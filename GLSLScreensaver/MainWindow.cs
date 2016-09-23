// Created by Colby on 11/09/2016
// Location: LifePanel/LifePanel/MainWindow.cs

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GLSLScreensaver
{
    internal class MainWindow : GameWindow
    {
        private bool _shouldDie;

        readonly ScreensaverShader _shader = new ScreensaverShader("sines");

        readonly UniformDynamicTime _uniformDynamicTime = new UniformDynamicTime();
        private readonly Uniform _uniformResolution = new Uniform("resolution") {Value = new Vector2(1920, 1080)};

        public MainWindow() : base(1920, 1080, new GraphicsMode(32, 24, 0, 8))
        {
            Load += LoadHandler;
            Resize += ResizeHandler;
            UpdateFrame += UpdateHandler;
            RenderFrame += RenderHandler;
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
            GL.Ortho(0, 1920, 1080, 0, 0, 1);
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
                _uniformResolution
            };
            _shader.Use(uniforms.ToArray());

            GL.Begin(BeginMode.Quads);
            GL.Color3(Color.White);
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex2(0f, 1080f);
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex2(1920f, 1080f);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2(1920f, 0f);
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex2(0f, 0f);
            GL.End();

            SwapBuffers();
        }

        public void Kill()
        {
            _shouldDie = true;
        }

        public static void DoTheThingZhuLi()
        {
            var mainWindow = new MainWindow();
            mainWindow.Run(20);
        }
    }
}