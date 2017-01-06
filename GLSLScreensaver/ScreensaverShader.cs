using System;
using System.Collections.Generic;
using System.IO;
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
            if (name == ":random")
            {
                var files = Directory.GetFiles("Shaders");
                ShaderName = Path.GetFileNameWithoutExtension(files[new Random().Next(files.Length)]);
            }
            else
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
