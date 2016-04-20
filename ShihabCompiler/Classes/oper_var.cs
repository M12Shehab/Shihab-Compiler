
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Windows.Forms;

namespace ShihabCompiler.Classes
{
    
    /// <summary>
    /// this class calculate halested metrics including :
    /// 1) the number of distinct operators 
    /// 2) = the number of distinct operands 
    /// 3) = the total number of operators 
    /// 4) = the total number of operators 
    /// </summary>
  class oper_var
    {
          public  int operatores,variables;

          public int G, L;
          public oper_var(string code)
        { 
           try
            {
             operatores = count_operators(code);
             variables = count_obj(code) + count_variables(code);
        }
           catch (Exception e)
           {
               operatores =0;
               variables = 0;
           }

                
            }

      int count_operators(string source_code)
      {
          Regex reg_operatores = new Regex(@"([<>!=+-/^*%&^:|]?[=]{2}|[+]?[+]{2}|[-]?[-]{2}|[&]?[&]{2}|[|]?[|]{2}|[+-/^%!><*|?~]|(new|delete|throw|type))");

          Regex reg_key_word = new Regex("(#include|import|System.out.print|cout|cin)");
          Regex reg_if_while = new Regex("(if|while|switch)");

          string code_only = retrive_code_only(source_code);

          //calculated number of ditinc operatores....
          string[] code_line = code_only.ToString().Split(Environment.NewLine.ToCharArray());

          int count_oper = 0;            // counter of  operatores
          for (int i = 0; i < code_line.Length; i++)
          {
              if (!reg_key_word.IsMatch(code_line[i]))
              {
                  string line_code = code_line[i];
                  line_code = line_code.Replace(" ", "");
                  line_code = line_code.Replace(",", "");
                  line_code = line_code.Replace(".", "");

                  MatchCollection oper_found = reg_operatores.Matches(line_code);////// count operatores per each line
                  foreach (Match op in oper_found)
                  {
                      count_oper++;
                  }
              }
          }


          return count_oper;
      }

      /// <summary>
      /// /calculate total of operands in programes source code
      /// </summary>
      /// <param name="calc_operands"></param>
      /// <returns></returns>


      int count_variables(string source_code)
      {
          bool globel = true; //to distinguesh globel variables from local
          bool globel_in_class = false;
          int count_brace = 0;

          
          G = L = 0;

          Regex reg_function = new Regex(@"^(void|int|float|bool|double|string|char|)\s(\w)+\s?([(])\S?.+([)]$)");// to ignore function decleration
          string[] decleration = { "" };////// to store number of decleration in line code";" 
          string[] variable;////// to store number of variables in each decleration","
          string code_only = "";
          int count_var = 0;
          Regex globel_class = new Regex(@"(class)");
          Regex reg_keyword = new Regex("(int |float |double |char |string |long |int?[[]|float?[[]|double?[[]|char?[[]|string?[[]|long?[[]|bool |Boolean )");
          string[] code_line = source_code.ToString().Split(Environment.NewLine.ToCharArray());
          for (int i = 0; i < code_line.Length; i++)
          {
              
              code_only = retrive_code_only(code_line[i].ToString());
              if (globel_class.IsMatch(code_only))
              globel_in_class = true; //globel in class


              if (code_only.Contains('{'))  //////to check globel in class or out class...
                  if (!globel_in_class)
                      globel = false;
                  else
                  {
                      count_brace++;
                      if (count_brace > 1)
                      {
                          globel = false;
                          globel_in_class = false;
                          count_brace = 0;
                      }
                  }

              code_only = code_only.Replace("public", ""); ////remove public and protected to facility discover function decleration
              code_only = code_only.Replace("protected", "");
              code_only = code_only.Trim();////remove spaces boundaries to facility discover function decleration
              if (!reg_function.IsMatch(code_only)) ///if not decleration function
              {
                  decleration = code_only.Split(';');/////to store each  decleration in line code";"
                  for (int j = 0; j < decleration.Length; j++)
                      if (reg_keyword.IsMatch(decleration[j])) //////// count variables with normal declearation "int,float,,,etc..
                      {
                          variable = decleration[j].Split(',');    /// to store each  variable in each decleration","
                          if (globel && !code_only.Contains("main") && !code_only.Contains("("))
                          {
                             // MessageBox.Show(code_only,"G");
                              G += (variable.Length) * 2;
                              count_var += (variable.Length) * 2;////globel variables
                          }
                          else if (!code_only.Contains("main"))
                          {
                             //  MessageBox.Show(code_only,"L");
                              L+=variable.Length;
                              count_var += variable.Length;///local variables
                          }

                      }
              }
          }

          return count_var;
      }


      /// <summary>
      /// //////////
      /// </summary>
      /// <param name="code"></param>
      int count_obj(string source_code)
      {
          Regex reg_class = new Regex(@"[^\s]class\s+([^\s]+)[\s\r\n{]");/// to find name of classe
          string class_name;
          int count_obj = 0;
          ArrayList class_keyword = new ArrayList();// list to add name of classes
          string code_only = "";
          string[] split_code;
          string[] code_line = source_code.ToString().Split(Environment.NewLine.ToCharArray());
          for (int i = 0; i < code_line.Length; i++)
          {
              code_only = retrive_code_only(code_line[i].ToString());
              code_only = code_only.Replace("{", "");
              split_code = code_only.Split(' ');
              for (int j = 0; j < code_only.Length; j++)
              {
                  if (split_code.Contains("class"))

                      if (code_only.Substring(j, 5) == "class")
                      {
                          class_name = code_only.Substring(j + 5, code_only.Length - (j + 5));
                          class_name = class_name.Trim(); /// remove spaces
                          class_keyword.Add(class_name);////////add name of class ..
                          j = code_only.Length;
                          //MessageBox.Show(class_name);
                      }


              }

              foreach (string name_clas in class_keyword)
                  if (split_code.Contains(name_clas) && !(split_code.Contains("class")))
                  {
                      string[] obj = code_only.Split(',');
                      count_obj += obj.Length; //// to count more objects declerations in the same class.


                  }

          }
          return count_obj;

      }

      /// <summary>
      /// take a source code of program and reove all comments from it
      /// </summary>
      /// <param name="allcode"></param>
      /// <returns></returns>
      public string retrive_code_only(string allcode)
      {
          string[] lines_code = allcode.ToString().Split(Environment.NewLine.ToCharArray());

          bool str_flag, char_flag, comm_line, comm_multi_line;// flages of comments and string
          comm_multi_line = false;
          string all_line, code_line;
          code_line = "";
          for (int i = 0; i < lines_code.Length; i++)
          {
              str_flag = false; char_flag = false; comm_line = false;
              all_line = lines_code[i].ToString();
              for (int j = 0; j < all_line.Length; j++)
              {
                  if (j < all_line.Length - 1)
                  {
                      if (all_line.Substring(j, 1) == "/" && all_line.Substring(j + 1, 1) == "/") // check for comments in one line
                          comm_line = true;
                      if (all_line.Substring(j, 1) == "/" && all_line.Substring(j + 1, 1) == "*") // check for comments in multi line
                          comm_multi_line = true;
                      if (all_line.Substring(j, 1) == "*" && all_line.Substring(j + 1, 1) == "*") // check for comments in multi line
                          comm_multi_line = true;
                      if (all_line.Substring(j, 1) == "*" && all_line.Substring(j + 1, 1) == "/") // check for comments in multi line
                      {
                          comm_multi_line = false;
                          j += 2; //to remove two sympoles */
                      }
                  }
                  if (j < all_line.Length) //to set rang of code
                  {
                      if (comm_line == false && comm_multi_line == false)
                      {
                          if (all_line.Substring(j, 1) == "\"") ////////check for string
                          {
                              if (str_flag == true)
                                  str_flag = false;
                              else
                                  str_flag = true;
                          }

                          else if (all_line.Substring(j, 1) == "\'") ////////check for characters
                          {
                              if (char_flag == true)
                                  char_flag = false;
                              else
                                  char_flag = true;
                          }
                          else if (str_flag == false && char_flag == false)
                              code_line += all_line.Substring(j, 1);  // add characters if not comment or string or characters
                      }
                  }
              }
              code_line += "\n";
          }



          return code_line.ToString();
      }

    }
}

