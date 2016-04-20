using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ShihabCompiler.Classes
{
    class ClassShaoWang
    {
        string Code;
        public  int fff;
        public ClassShaoWang(string C)
        {
            fff = 0;
            Code = C;
        }
       
        string ExtractFunctionName(string Line)
        {
            Line = Line.Trim();
            string FunctionName = "";
            Regex pattern = new Regex(@"[a-zA-Z]\s{1}\w+\s*(\(\s*(const\s)?)+((\w)+(\[,?\]|\*|&|\*=)?\s{1,}(\s?&\s?)?\w+(\[\])?(\w*\s=\s\d)?(,)?(\s|\n\s*)?){0,}\s?\){1}");
            MatchCollection ms = pattern.Matches(Line);

            foreach (Match n in ms)                                                         //fetch all pattern functions
            {
                //this loop to do preprocess to extreact functions name
                string newN = n.Value.Trim();
                int index = newN.IndexOf("(");
                newN = newN.Substring(1, index).Trim();
                FunctionName = newN;
            }
            // MessageBox.Show(FunctionName+"\n"+Line, "FunctionName");
            return FunctionName;
        }
        bool IsFunction(string Line)
        {
            Line = Line.Trim();
            Regex M = new Regex(@"\)$");
            if (M.IsMatch(Line) && !(Line.Contains("for") || !Line.Contains("while") || !Line.Contains("if") || !Line.Contains("switch") || !Line.Contains("foreach")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int CountOfRecersionFuntions()
        {
            int Count = 0;
            string myString;
            StringReader rd = new StringReader(Code);
            bool MultiComments = false;
            List<string> FunctionsName = new List<string>();
            bool StartCount = false;
            int ShihabFlage = 0;
            string fun = "";

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
                        if (IsFunction(s))
                        {
                            string dd = ExtractFunctionName(s);
                            dd = dd.Trim();
                            if (!FunctionsName.Contains(dd))
                            {
                                FunctionsName.Add(dd);
                                StartCount = true;
                                fun = dd;
                            }
                        }

                        if (s.Contains('{') && StartCount && !s.Contains('}'))
                        {
                            ShihabFlage++;
                        }
                        else if (s.Contains('}') && StartCount)
                        {
                            ShihabFlage--;
                        }

                        if (myString.Contains('{') && StartCount && myString.Contains('}'))
                        {
                            if (myString.Contains(fun) && !myString.Contains("using") && !myString.Contains("catch") && !myString.Contains("~") && fun.Length > 0)
                            {//MessageBox.Show(myString,"Rec 1");
                                Count += 3;
                            }
                        }

                        if (ShihabFlage > 0)
                        {
                            if (myString.Contains(fun) && !myString.Contains("using") && !myString.Contains("catch") && !myString.Contains("~") && fun.Length > 0)
                            {//MessageBox.Show(myString,"Rec 2");
                                Count += 3;
                            }
                        }
                    }
                }
                else if (myString.Contains("/*"))
                {
                    int indexOfMultiComment = myString.IndexOf('/');
                    string s = myString.Substring(0, indexOfMultiComment);
                    if (IsFunction(s.TrimEnd('\n')))
                    {
                        string dd = ExtractFunctionName(s);
                        dd = dd.Trim();
                        if (!FunctionsName.Contains(dd))
                        {
                            FunctionsName.Add(dd);
                            StartCount = true;
                            fun = dd;
                        }
                    }

                    if (s.Contains('{') && StartCount && !s.Contains('}'))
                    {
                        ShihabFlage++;
                    }
                    else if (s.Contains('}') && StartCount)
                    {
                        ShihabFlage--;
                    }

                    if (myString.Contains('{') && StartCount && myString.Contains('}'))
                    {
                        if (myString.Contains(fun) && !myString.Contains("using") && !myString.Contains("catch") && !myString.Contains("~") && fun.Length > 0)
                        {
                            //MessageBox.Show(myString,"Rec 3");
                            Count += 3;
                        }
                    }

                    if (ShihabFlage > 0)
                    {
                        if (myString.Contains(fun) && !myString.Contains("using") && !myString.Contains("catch") && !myString.Contains("~") && fun.Length > 0)
                        {//MessageBox.Show(myString,"Rec 4");
                            Count += 3;
                        }
                    }

                    if (!myString.Contains("*/"))                                           //Be sure this multi-comment is not ended in same line
                    {
                        MultiComments = true;
                    }
                }
                else
                {
                    //Code Section
                    myString = myString.TrimEnd('\n');
                    if (IsFunction(myString))
                    {
                        string dd = ExtractFunctionName(myString);
                        dd = dd.Trim();
                        if (!FunctionsName.Contains(dd))
                        {
                            FunctionsName.Add(dd);
                            StartCount = true;
                            fun = dd;
                            continue;
                        }
                    }

                    if (myString.Contains('{') && StartCount && !myString.Contains('}'))
                    {
                        ShihabFlage++;
                    }
                    else if (myString.Contains('}') && StartCount)
                    {
                        ShihabFlage--;
                    }
                    if (myString.Contains('{') && StartCount && myString.Contains('}'))
                    {
                        if (myString.Contains(fun) && !myString.Contains("using") && !myString.Contains("catch") && !myString.Contains("~") && fun.Length > 1)
                        {
                            //  MessageBox.Show(myString+"\nfun = "+ fun.Length ,"Rec 5");
                            Count += 3;
                        }
                    }
                    if (ShihabFlage > 0)
                    {
                        if (myString.Contains(fun) && !myString.Contains("using") && !myString.Contains("catch") && !myString.Contains("~") && fun.Length > 0)
                        {
                            //  
                            Count += 3;
                        }
                    }
                }
                // MessageBox.Show("mystring "+ myString+"\nfun "+fun);
            }
            return Count;
        }

        int GetNumIfAndSwitch()
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
                       if (s.Contains("thread") || s.Contains("Thread"))
                       {
                           count += 4;

                       }
                    }
                }
                else if (myString.Contains("/*"))
                {
                    int indexOfMultiComment = myString.IndexOf('/');
                    string s = myString.Substring(0, indexOfMultiComment);
                    count += GetValue(s.Trim());

                    if (s.Contains("thread") || s.Contains("Thread"))
                    {
                        count += 4;
                        
                    }

                    if (!myString.Contains("*/"))                                           //Be sure this multi-comment is not ended in same line
                    {
                        MultiComments = true;
                    }
                }
                else
                {
                    //Code Section
                    count += GetValue(myString.Trim());

                    if (myString.Contains("thread") || myString.Contains("Thread"))
                    {
                        count += 4;

                    }
                }
                
            }

            count+=((int)(Math.Log10(fff)/Math.Log10(2)));
            return count;
        }

        int GetValue(string Line)
        {
            Line = Line.Trim();
            Regex fun = new Regex(@"\w+\(.*?\);");
            Regex Loop = new Regex(@"(for|while|do|foreach)");
            Regex If = new Regex(@"(if|case)");

            if (fun.IsMatch(Line))
            {
                fff+=2;
               // MessageBox.Show("Line "+Line , "Rec 6");
                return 0;
            }

            if (If.IsMatch(Line))
            {
                if (Line.Contains("if"))
                    return 2;
                else
                    return 1;
            }
            if (Loop.IsMatch(Line))
            {
                return 3;
            }

            return 0;
        }

        public int GetShaoWangComplexity()
        {
            int total = 0;
            total = CountOfRecersionFuntions();

            total += GetNumIfAndSwitch();
            return total;
        }
    }
}
