using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ShihabCompiler.Classes
{
    class ClassFunctions
    {
        string Line;
        public ClassFunctions(string C)
        {
            Line = C;
        }

        public int GetCountOfFunArg()
        {
            int Args =Line.Split(',').Length - 1;
           
            if (Args > 0)
            {
                Args++;
            }
            else
            {
                try
                {
                    int index1 = Line.IndexOf('(');                                  //get the index of '('
                    int index2 = Line.IndexOf(')');                                  //get the index of ')'
                    for (int i = 0; i < (index2 - index1); i++)                         //search for char inside '()' if there is at least 1 char so then there is 1 arg
                    {
                        if (Char.IsLetter(Convert.ToChar(Line.Substring(i + index1, 1))))    //if there is 1 char
                        {
                            Args++;                                                   //add it to total
                            break;                                                     //exit from loop to reduce time consuming
                        }
                    }
                }
                catch (Exception ex)
                { }
            }
            return Args;           
        }
    }
}
