using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLScreensaver
{
    class UniformDynamicTime : Uniform
    {
        private static readonly DateTime _start = DateTime.Now;

        public UniformDynamicTime() : base("time")
        {
        }

        public override object GetValue()
        {
            return (float)(DateTime.Now - _start).TotalSeconds * MainWindow.Config.TimeScale;
        }
    }
}
