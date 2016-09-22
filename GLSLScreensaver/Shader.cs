using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GLSLScreensaver
{
    abstract class Shader
    {
        protected int FsId;
        protected int PgmId;
        protected int VsId;

        public void InitProgram()
        {
            PgmId = GL.CreateProgram();
            Init();
        }

        public void Use(params object[] uniforms)
        {
            SetupUniforms(uniforms);
            GL.UseProgram(PgmId);
        }

        protected abstract void Init();

        protected abstract void SetupUniforms(params object[] uniforms);

        protected void LoadShader(string filename, ShaderType type, int program, out int address)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"File not found: {filename}");
                address = -1;
                return;
            }
            address = GL.CreateShader(type);
            using (var sr = new StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }
    }
}
