using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ShihabCompiler.Classes
{
    class CyclomaticClass
    {
        string[] CCKeywords = { "do ", "while", "for", "if", "||", "&&", "case ", "foreach", "try", "elif" };//this keywords to calculate CC

        /// <summary>
        /// this  function is the main function to calculate
        /// the Cyclomatic Complexity of any program by take
        /// input as code param here and return the cc value
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public int Calculate_Cyclomatic_Complexity(string Code)
        {
            TokensClass token = new TokensClass();                                          //create object to get tokens from text
            bool flage=false;                                                               //this flage for switch
            int CC = 0;                                                                     //save the Cyclomatic Complexity
            Regex functionImplement = new Regex(@"(\((const\s)?)+(([a-zA-Z])+(\[,?\]|\*|&|\*=)?\s{1}(&)?\w+(\[\])?(\w*\s=\s\d)?(,)?\s?){0,}\s?\){1}(\s)?((\{)|(\n\{)|(\n\s*\{)){1}");//this Regular Expression for function declaration
          //  Regex classfunction = new Regex(@"(([a-zA-Z])*\s{1})?\w+(\:\:){1}\w+\s?\(((const\s)?)+(([a-zA-Z])+(\[\])?( &|& )?(\* |\*= )?\w+(\[\])?(\w*\s=\s\d)?(,)?\s?){0,}\s?\){1}");
            Regex pythonFunction = new Regex(@"^(\s*?def)");
            int CountOfFun = 0;
            StringReader reader = new StringReader(Code);                                   //make code as text line by line
            string nn = "";
            bool MulComments = false;                                                       //this flage for multi-comment line
            string myString;                                                                //to save line code
            while ((myString = reader.ReadLine()) != null)                                  //start fetch code
            {
               
                bool IsComment = false;                                                     //comment flage init OFF

                if (pythonFunction.IsMatch(myString))
                {//here count number of function method in python
                    CountOfFun++;
                }

                string[] m = token.Tokenize(myString);                                      //Get tokens in singel line
                
                foreach (string n in m)                                                     //fetch tokens
                {
                    if (n.Contains("//"))                                                   //it is a comment
                    {
                        IsComment = true;                                                   //comment flage ON
                    }
                    else if (n.Contains("/*"))
                    {
                        MulComments = true;
                    }
                    else if (n.Contains("*/"))                                              //make multi-comment line OFF
                    {
                        MulComments = false;
                        continue;
                    }

                    if (!IsComment && !MulComments)                                           //if the token is not comment
                    {
                        if (CCKeywords.Contains(n))                                         //if the token is CC token "if", "else", "for", "do", "while"
                        {
                            if (flage && !n.Equals("case"))
                            {
                                continue;
                            }
                           // MessageBox.Show("Key = "+n+"\nCC = "+CC.ToString());
                            CC++;
                            if (n.Equals("switch"))
                            {
                                flage = true;
                            }
                        }
                    }
                }
                nn = myString;
            }
          
            if (CountOfFun == 0)
            {
                MatchCollection functions = functionImplement.Matches(Code);               //this to check number of functions in code
                foreach (Match n in functions)
                {
                    if (n.Value.Contains("in") && !(n.Value.Contains("string") || n.Value.Contains("int")))
                    {
                        continue;
                    }
                    else
                    {
                        
                        CountOfFun++;
                    }
                }
               // CountOfFun = functions.Count;
            }
            
           
            return (CC +CountOfFun );
        }
    }
}
