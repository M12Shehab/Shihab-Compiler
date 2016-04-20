using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShihabCompiler.Classes
{
    class ClassFlowChart
    {
        public int NumIF;
        public int NumCases;
        public int NumLoops;
        public int NumRecursion;

        public ClassFlowChart()
        {
            NumIF = 0;
            NumCases = 0;
            NumLoops = 0;
            NumRecursion = 0;
        }

        public int GetFlowchart()
        {
            return (NumIF + NumCases + NumLoops + NumRecursion);
        }
    }
}
