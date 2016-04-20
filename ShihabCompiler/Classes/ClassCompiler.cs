using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ShihabCompiler.Classes
{
    class ClassCompiler
    {
        string Code;
        ClassComplexityAttr Complextiy;

        public ClassComplexityAttr GetReport()
        {
            return this.Complextiy;
        }

        public ClassCompiler(string code)
        {
            Code = code;
            Complextiy = new ClassComplexityAttr();
            oper_var cls = new oper_var(code);
            Complextiy.Varibles.Globel = cls.G;
            Complextiy.Varibles.Local = cls.L;
            Complextiy.FunctionCall = GetFunctionCall();
           Complextiy.NumOfExternalLibANDFun = CountExternalLibAndFuns();
        }

        public void Compile()
        {
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
                        string s = myString.Substring(0,  CStart);
                       // MessageBox.Show(s);
                        DetectedType(s);
                    }
                }
                else if (myString.Contains("/*"))
                {
                    int indexOfMultiComment = myString.IndexOf('/');
                    string s = myString.Substring(0, indexOfMultiComment);
                    DetectedType(s.TrimEnd('\n'));

                    if (!myString.Contains("*/"))                                           //Be sure this multi-comment is not ended in same line
                    {
                        MultiComments = true;
                    }
                }
                else
                {
                    myString = myString.TrimEnd('\n');
                    DetectedType(myString);
                }
            }
         //   MessageBox.Show("Complextiy.NumOfOpreations = " + Complextiy.NumOfOpreations, "Class Funtions");
           if(Complextiy.NumOfOpreations!=0)
            Complextiy.NumOfOpreations =(int) ((Math.Log10(Complextiy.NumOfOpreations)) / (Math.Log10(2)));
           Complextiy.FlowChart.NumRecursion= CountOfRecersionFuntions(Code);
          
        }

        void DetectedType(string Line)
        {
            Regex M = new Regex(@"\)$");
            Regex F = new Regex(@"(for|while|do|foreach|if|else|case)");
            Regex Op = new Regex(@"(Console|cout|cin|print|println|\*|\+|\-|\/|<|<=|!|>|>=|==|!=|%|\|\||&&)");
           
            if (Line.Contains("try"))
                Complextiy.FlowChart.NumIF = (Complextiy.FlowChart.NumIF + 1);
            
            if (M.IsMatch(Line) && !(Line.Contains("for") || Line.Contains("while") || Line.Contains("if") || Line.Contains("switch")))
            {
                ClassFunctions Fun = new ClassFunctions(Line);
                    Complextiy.NumOfFunctions += Fun.GetCountOfFunArg()+1;
            }
            if (F.IsMatch(Line))
            {
                if (Line.Contains("if"))
                    Complextiy.FlowChart.NumIF = (Complextiy.FlowChart.NumIF + 2);
                else if (Line.Contains("case"))
                    Complextiy.FlowChart.NumCases = (Complextiy.FlowChart.NumCases + 1);
                else if (Line.Contains("for") || Line.Contains("while") || Line.EndsWith("do") || Line.EndsWith("foreach"))
                {
                    Complextiy.FlowChart.NumLoops += 3;
                 //   MessageBox.Show(Line);
                }
            }

            if (Op.IsMatch(Line))
            {
                Complextiy.NumOfOpreations+=Op.Matches(Line).Count;
            }
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

        public int CountOfRecersionFuntions(string Code)
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
                              if (myString.Contains(fun) && !myString.Contains("using") && !myString.Contains("catch")&& !myString.Contains("~") && fun.Length >0)
                            {//MessageBox.Show(myString,"Rec 1");
                                Count += 3;
                            }
                        }

                        if (ShihabFlage > 0)
                        {
                              if (myString.Contains(fun) && !myString.Contains("using") && !myString.Contains("catch")&& !myString.Contains("~") && fun.Length >0)
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
                          if (myString.Contains(fun) && !myString.Contains("using") && !myString.Contains("catch")&& !myString.Contains("~") && fun.Length >0)
                        {
                            //MessageBox.Show(myString,"Rec 3");
                            Count += 3;
                        }
                    }

                    if (ShihabFlage > 0)
                    {
                          if (myString.Contains(fun) && !myString.Contains("using") && !myString.Contains("catch")&& !myString.Contains("~") && fun.Length >0)
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
                        if (myString.Contains(fun) && !myString.Contains("using") && !myString.Contains("catch")&& !myString.Contains("~") && fun.Length >1)
                        {
                          //  MessageBox.Show(myString+"\nfun = "+ fun.Length ,"Rec 5");
                            Count += 3;
                        }
                    }
                    if (ShihabFlage > 0)
                    {
                          if (myString.Contains(fun) && !myString.Contains("using") && !myString.Contains("catch")&& !myString.Contains("~") && fun.Length >0)
                            {
                              //  MessageBox.Show(myString+"\nfun = "+ fun,"Rec 6");
                                Count += 3;
                            }
                    }
                }
               // MessageBox.Show("mystring "+ myString+"\nfun "+fun);
            }

            return Count;
        }
        /// <summary>
        /// this function is to extract
        /// function name after detected
        /// function pattern
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
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

        int GetFunctionCall()
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
            return count;
        }

        int GetValue(string Line)
        {
            Line = Line.Trim();
            Regex fun = new Regex(@"\w+\(.*?\);");
            if (fun.IsMatch(Line)&& !Line.Contains("print")&&!Line.Contains("println"))
            {
                return 1;
            }
            return 0;
        }

        public void PrintData()
        {
            MessageBox.Show("Functions = " + Complextiy.NumOfFunctions + "\nOpreations = " + Complextiy.NumOfOpreations + "\n*flow:\ncases = " + Complextiy.FlowChart.NumCases + "\nif/else = " + Complextiy.FlowChart.NumIF + "\nloops = " + Complextiy.FlowChart.NumLoops + "\nRecursion = " + Complextiy.FlowChart.NumRecursion + "\n************\nGlobel Varibels = " + Complextiy.Varibles.Globel + "\nLocal Varibels = " + Complextiy.Varibles.Local+"\nExternal = "+Complextiy.NumOfExternalLibANDFun+"\nCall Functions = "+Complextiy.FunctionCall,"Report!!");
        }

        public string PrintData1()
        {
           return("Functions = " + Complextiy.NumOfFunctions + "\nOpreations = " + Complextiy.NumOfOpreations + "\n*flow:\ncases = " + Complextiy.FlowChart.NumCases + "\nif/else = " + Complextiy.FlowChart.NumIF + "\nloops = " + Complextiy.FlowChart.NumLoops + "\nRecursion = " + Complextiy.FlowChart.NumRecursion + "\n************\nGlobel Varibels = " + Complextiy.Varibles.Globel + "\nLocal Varibels = " + Complextiy.Varibles.Local + "\nExternal = " + Complextiy.NumOfExternalLibANDFun);
        }

        MatchCollection HelpFunctionForExternalLib()
        {
            Regex pattern = new Regex(@"[a-zA-Z]\s{1}\w+\s*(\(\s*(const\s)?)+((\w)+(\[,?\]|\*|&|\*=)?\s{1,}(\s?&\s?)?\w+(\[\])?(\w*\s=\s\d)?(,)?(\s|\n\s*)?){0,}\s?\){1}(\s)?((\{)|(\n\{)|(\n\s*\{|\s*:)){1}");
            MatchCollection ms = pattern.Matches(Code);
            return ms;
        }

        public int CountExternalLibAndFuns()
        {
           
            int count = 0;
            MatchCollection ms = HelpFunctionForExternalLib();                              //Call help function and save all patterns
            List<string> functionNames = new List<string>();
            foreach (Match n in ms)                                                         //fetch all pattern functions
            {
                //this loop to do preprocess to extreact functions name
                string newN = n.Value.Trim();
                int index = newN.IndexOf("(");
                newN = newN.Substring(1, index).Trim();

                functionNames.Add(newN);                                                    //Add the function name to the list to be internal function
              
                //  MessageBox.Show(newN);
            }
          

            //   MessageBox.Show(functionNames.Count.ToString(), "num arg");

            bool MultiComments = false;                                                     //flage for multi comments
            string myString;                                                                //to save line code
            StringReader rd = new StringReader(Code);
            string temp = "";
            while ((myString = rd.ReadLine()) != null)                                  //start fetch code
            {
                if (myString.Contains("//") || MultiComments)
                {
                    if (myString.Contains("*/"))
                    {
                        MultiComments = false;

                    }
                    // MessageBox.Show("Comment",myString);
                }
                else if (myString.Contains("/*"))
                {
                    int indexOfMultiComment = myString.IndexOf('/');
                    string s = myString.Substring(0, indexOfMultiComment);

                    if (!myString.Contains("*/"))                                           //Be sure this multi-comment is not ended in same line
                    {
                        MultiComments = true;
                    }
                }
                else         //if it is not comment
                {
                    if (myString.Contains("new"))
                        continue;
                    // Regex r = new Regex(@"\w+\s*(\(\s*(const\s)?)+((\w)+(\[,?\]|\*|&|\*=)?\s{1,}(\s?&\s?)?\w+(\[\])?(\w*\s=\s\d)?(,)?(\s|\n\s*)?){0,}\s?\){1}(\s)?((\;)|(\n\;)|(\n\s*\;|\s*:)){1}");
                    Regex r = new Regex(@"\)(((\;)|(\n\;)|(\n\s*\;|\s*:))){1}");                                 //regular expresion for call function
                    if (myString.Contains("System") || myString.Contains("import") || myString.Contains("include"))//for external lib
                    {
                        if (myString.Contains("System.out.println"))
                            continue;
                      
                        count++;
                    }
                    else if (r.IsMatch(myString) && !myString.Contains("return") && !myString.Contains("Console"))//check if it is calling function
                    {
                        Regex IOFiles = new Regex(@"(\.read|\.write|\.close|\.open|\.is_open|\.getline|\.seekg|\.tellg|\.tellp|\.seekp|\.put|\.get|Files.move|\.delete|\.createNewFile|\.renameTo|\.exists|\.flush|\.lastModified|\.length|\.getAbsolutePath|\.isHidden|\.reset|\.ReadAllText|\.ReadAllLines|\.WriteAllText|\.WriteAllLines|File.ReadAllBytes|\.Seek|File.Copy|File.Delete|File.Exists|File.GetLastWriteTimeUtc|File.Move|File.Open|File.Replace|File.Open|File.OpenRead|WriteAllBytes|File.WriteAllLines|Directory|File.Create)\(");
                        Regex IOs = new Regex(@"(cout|cin|cerr|clog|WriteLine|ReadLine|println|print|read|readLine|ReadKey)\(?");
                        bool found = false;
                        foreach (string fun in functionNames)
                        {
                            //search if this function is internal or the above line if the above line did not end with ;
                            if (myString.Contains(fun) || temp.Contains(fun))
                            {
                                found = true;
                                break;
                            }
                        }
                      
                        if (!found && !IOFiles.IsMatch(myString) && !IOs.IsMatch(myString))
                        {
                            //if not found and it is not files operations or I/O operations then it is external function
                            count++;
                          
                            // MessageBox.Show(myString, "External function");
                        }
                    }
                    else
                    {
                        if (!myString.Contains(";"))
                        {
                            //here if the line did not end with ; so save it as temp to check
                            temp = myString;
                            // MessageBox.Show(temp);
                        }
                    }
                }
            }
           
            return count;
        }
    }
}
