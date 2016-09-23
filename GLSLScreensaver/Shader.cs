using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.ES20;
using GL = OpenTK.Graphics.OpenGL.GL;
using ShaderType = OpenTK.Graphics.OpenGL.ShaderType;

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

        public void Use(IEnumerable<Uniform> uniforms)
        {
            SetupUniforms(uniforms);
            GL.UseProgram(PgmId);
        }

        protected abstract void Init();

        protected virtual void SetupUniforms(IEnumerable<Uniform> uniforms)
        {
            foreach (var uniform in uniforms)
            {
                var loc = GL.GetUniformLocation(PgmId, uniform.Name);
                var val = uniform.GetValue();
                var type = val.GetType();
                if (type == typeof(float))
                    GL.Uniform1(loc, (float)val);
                else if (type == typeof(double))
                    GL.Uniform1(loc, (double)val);
                else if (type == typeof(int))
                    GL.Uniform1(loc, (int)val);
                else if (type == typeof(uint))
                    GL.Uniform1(loc, (uint)val);
                else if (type == typeof(Vector2))
                {
                    var vec2 = (Vector2)val;
                    GL.Uniform2(loc, vec2.X, vec2.Y);
                }
                else
                    throw new ArgumentException($"Unsupported uniform type: {type}");
            }
        }

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
