using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShihabCompiler.Classes
{
    class ClassComplexityAttr
    {
        public int NumOfFunctions;
        public int NumOfExternalLibANDFun;
        public int NumOfOpreations;
        public int FunctionCall;
        public ClassFlowChart FlowChart;
        public ClassVaribales Varibles;

        public ClassComplexityAttr()
        {
            FunctionCall= NumOfExternalLibANDFun = NumOfFunctions = NumOfOpreations = 0;
            FlowChart = new ClassFlowChart();
            Varibles = new ClassVaribales();
        }
    }
}
