using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ShihabCompiler.Classes
{
    class ClassCycloma2
    {
        string Code;
        public  int fff;
        public ClassCycloma2(string C)
        {
            fff = 0;
            Code = C;
        }

       public int GetNumIfAndSwitch()
        {
            int count = 0;
            string myString;

            StringReader rd = new StringReader(Code);
            bool MultiComments = false;

            while ((myString = rd.ReadLine()) != null)                                  //start fetch code
            {
                if (myString.Contains("//") || MultiComments)
                {
                    if (myString.Contains("*/"))
                    {
                        MultiComments = false;
                    }

                    int CStart = myString.IndexOf("//");

                    if (CStart > 0)
                    {
                        string s = myString.Substring(0, CStart);
                        count += GetValue(s.Trim());
                      
                    }
                }
                else if (myString.Contains("/*"))
                {
                    int indexOfMultiComment = myString.IndexOf('/');
                    string s = myString.Substring(0, indexOfMultiComment);
                    count += GetValue(s.Trim());

                    if (!myString.Contains("*/"))                                           //Be sure this multi-comment is not ended in same line
                    {
                        MultiComments = true;
                    }
                }
                else
                {
                    //Code Section
                  //  MessageBox.Show(myString,"0000000000");
                    count += GetValue(myString);
                    
                }

            }

            //count += ((int)(Math.Log10(fff) / Math.Log10(2)));
            return count;
        }

        int GetValue(string Line)
        {
         //   MessageBox.Show(Line.Contains("if").ToString(),"111");
          //  Line = Line.Trim();
            Regex fun = new Regex(@"\w+\(.*?\)");
            Regex Loop = new Regex(@"(for|while|do|foreach)");
            Regex If = new Regex(@"(if|case)");
  
            if (Line.Contains("if") )
            {  
                    return 1;
            }
            if (Line.Contains("for") || Line.Contains("while") || Line.Contains("do") || Line.Contains("foreach"))
            {// MessageBox.Show(Line , "Rec 6");
                return 1;
            }
            if (fun.IsMatch(Line) && !(Line.Contains("println") || Line.Contains("print") || Line.Contains(";")))
            {
                fff += 2;
                
                return 0;
            }

           

            return 0;
        }
    }
}
