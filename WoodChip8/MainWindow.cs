using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Timers;
using WoodChip8.Assembly;

namespace WoodChip8
{
    public sealed class MainWindow : GameWindow
    {
        public static String BaseTitle = "WoodChip8 Emulator";

        public MainWindow()
            : base(1280, 720,
                  GraphicsMode.Default,
                  BaseTitle,
                  GameWindowFlags.FixedWindow,
                  DisplayDevice.Default,
                  4, 0,
                  GraphicsContextFlags.ForwardCompatible)
        {
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
        }

        protected override void OnLoad(EventArgs e)
        {
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            this.Title = BaseTitle + " FPS: " + this.RenderFrequency.ToString("G6");

            Color4 ClearColor = new Color4(0, 0, 0, 255);
            GL.ClearColor(ClearColor);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            SwapBuffers();
        }
    }
}
