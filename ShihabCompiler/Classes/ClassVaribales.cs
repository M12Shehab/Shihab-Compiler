using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShihabCompiler.Classes
{
    class ClassVaribales
    {
        public int Local;
        public int Globel;

        public ClassVaribales()
        {
            Local = Globel = 0;
        }

        public int GetVaribles()
        {
            return (Local+Globel);
        }
    }
}
