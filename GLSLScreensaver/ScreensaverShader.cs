using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GLSLScreensaver
{
    class ScreensaverShader : Shader
    {
        protected override void Init()
        {
            LoadShader("Shaders\\main.fs", ShaderType.FragmentShader, PgmId, out FsId);

            GL.LinkProgram(PgmId);
            Console.WriteLine(GL.GetProgramInfoLog(PgmId));
        }

        protected override void SetupUniforms(params object[] uniforms)
        {
            var timeLoc = GL.GetUniformLocation(PgmId, "time");
            GL.Uniform1(timeLoc, (float) uniforms[0]);
        }
    }
}
