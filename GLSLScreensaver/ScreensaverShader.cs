using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.ES10;
using OpenTK.Graphics.ES20;
using GL = OpenTK.Graphics.OpenGL.GL;
using ShaderType = OpenTK.Graphics.OpenGL.ShaderType;

namespace GLSLScreensaver
{
    class ScreensaverShader : Shader
    {
        public string ShaderName { get; set; }

        public ScreensaverShader(string name)
        {
            ShaderName = name;
        }

        protected override void Init()
        {
            LoadShader($"Shaders\\{ShaderName}.frag", ShaderType.FragmentShader, PgmId, out FsId);

            GL.LinkProgram(PgmId);
            Console.WriteLine(GL.GetProgramInfoLog(PgmId));
        }
    }
}
