using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLScreensaver
{
    class Uniform
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public Uniform(string name)
        {
            Name = name;
        }

        public virtual object GetValue() => Value;
    }
}
