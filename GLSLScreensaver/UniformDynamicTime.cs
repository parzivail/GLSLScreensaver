using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLScreensaver
{
    class UniformDynamicTime : Uniform
    {
        public UniformDynamicTime() : base("time")
        {
        }

        public override object GetValue()
        {
            return DateTime.Now.ToUnixTime();
        }
    }
}
