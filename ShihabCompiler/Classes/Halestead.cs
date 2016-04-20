using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
  
    public class Halestead
    {
         Regex operatores ; // names of all operatores
        Regex if_while; //to check operatores inside if or while
        

        public Halestead()
        {
          operatores = new Regex(@"([<>!=+-/^*%&^:|]?[=]|[+]?[+]|[-]?[-]|[&]?[&]|[|]?[|]|[=+-/^%!><&*|?~]|\b(if|while|switch|new|delete|throw|type)\b)");
           if_while = new Regex(@"[(].*?[)]");
        }

        public double calc_halested(string code)
        {
            /////// calculate Hlestead effort.....
            
             int n1, n2, N1, N2,N,n;
             double D, V,E;
             try
             { int []operands=new int[2];
            n1 = calc_distinc_operators(code);
            N1 = calc_operators(code);
            operands=distinct_operand_andAll(code);
            n2 = operands[0];
            N2 = operands[1];
            N = N1 + N2;
            n = n1 + n2;
            D = (n1 / 2) * (N2 / n2);
            V = N*((Math.Log10(n))/(Math.Log10(2)));
       
            E = D * V;
            return E;
             }
             catch (Exception e)
             {
                 MessageBox.Show("Invalied source code.!","Warning");
                 return 0;
             }
        }

         int calc_distinc_operators(string code)
        {
            //calculated number of ditinc operatores....

             int count_oper = 0;            // counter of distinct operatores

            ArrayList dis_oper_found = new ArrayList();   //arraylist to keep distinct operatores

            string code_only = retrive_code_only(code);
            MatchCollection oper_found = operatores.Matches(code_only.ToString());
                foreach (Match op in oper_found)            /// count operatores in each line code
                    if (!dis_oper_found.Contains(op.ToString())) ///////  check if not found in arralist
                    {
                        dis_oper_found.Add(op.ToString());/////add distinc operatores into arraylist to avoid frequency of it
                        
                        count_oper++;   }
             
            return count_oper;
        }
 
         int calc_operators(string code)
        {

                             //calculated number of ditinc operatores....

             int count_oper = 0;            // counter of  operatores

            string code_only = retrive_code_only(code);
            MatchCollection oper_found = operatores.Matches(code_only.ToString());
                foreach (Match op in oper_found)            
                        count_oper++;
 
            return count_oper;  
      }

        /// <summary>
        /// /calculate total of operands in programes source code
        /// </summary>
        /// <param name="calc_operands"></param>
        /// <returns></returns>
       


            int[] distinct_operand_andAll(string source_code)
        {
            int flag_oper;
            bool start_flag, flag_Stop;
            int count_oper = 0;
            bool flag_if_while_switch;
            ArrayList distinct_operand = new ArrayList();

            string[] double_oper = { "+=", "-=", "*=", "/=", "^=", "&=", "%=", ">=", "<=", "==", "||", "&&" };
            string[] single_oper = { "+", "-", "*", "/", "^", "%", "&", "=", ";", "{", "(", ")", ">", "<" };
            Regex reg_remove1 = new Regex(@"(\b(if|while|true|false|throw|int|float|bool|Boolean|char|string|boolean|Exception|new|delete)\b)");
            Regex reg_remove2 = new Regex(@"([}]|[ ])");
            Regex reg_if_while = new Regex("@(\b(if|while|switch)\b)");
 
             string[] code_line = source_code.ToString().Split(Environment.NewLine.ToCharArray());
            string operand = "";
            string code_only;
            for (int i = 0; i < code_line.Length; i++)
            {
                flag_oper = 0;
                start_flag = false;
                flag_Stop = false;
                flag_if_while_switch = false;
                int j;

                code_only = retrive_code_only(code_line[i]); //retrive code only without comments

                if (reg_if_while.IsMatch(code_only)) /// to check if found "if,while,switch"
                    flag_if_while_switch = true;

                code_only = reg_remove1.Replace(code_only, "");///remove special words to facility process of counting operands
                code_only = reg_remove2.Replace(code_only, "");//remove spaces  to facility process of counting operands
                code_only += ";";                                      

                for (j = 0; j < code_only.Length - 1; j++)
                {
                    if (single_oper.Contains(code_only.Substring(j, 1)))

                        if (code_only.Substring(j, 1) == ";")
                        {
                            if (start_flag)
                            {
                                operand = code_only.Substring(flag_oper, j - flag_oper);/////////////to store name of last operand in line code
                                if (!distinct_operand.Contains(operand))
                                { distinct_operand.Add(operand); }

                                count_oper++;
                            }
                            j = code_only.Length - 1;
                        }
                        else if (code_only.Substring(j, 1) == "{" || code_only.Substring(j, 1) == "(")/////////////to move flag wh branches {,(.
                            flag_oper = j + 1;
                        else if (code_only.Substring(j, 1) == ")")/////////////t store name of operand before )
                        {
                            if (start_flag || flag_if_while_switch)
                            {
                                operand = code_only.Substring(flag_oper, j - flag_oper);
                                if (!distinct_operand.Contains(operand))
                                { distinct_operand.Add(operand); }

                                count_oper++;
                            }
                            flag_oper = j + 1;
                        }

                        else
                        {
                            operand = code_only.Substring(flag_oper, j - flag_oper);/////////////to store name of operatore before single operation
                            if (!distinct_operand.Contains(operand))
                            { distinct_operand.Add(operand); }

                            count_oper++;
                            flag_oper = j + 1;
                            start_flag = true;
                        }

                    else if (double_oper.Contains(code_only.Substring(j, 2)))
                    {
                        operand = code_only.Substring(flag_oper, j - flag_oper);/////////////to store name of operatore before double operation
                        if (!distinct_operand.Contains(operand))
                        { distinct_operand.Add(operand); }

                        count_oper++;
                        flag_oper = j + 2;
                        start_flag = true;
                    }
                }
              }

            int[] result=new int[2];       /////////this a result of distinct operands and total operands;
            result[0] = distinct_operand.Count;
            result[1] = count_oper;

            return result;

        }

       /// <summary>
       /// take a source code of program and reove all comments from it
       /// </summary>
       /// <param name="allcode"></param>
       /// <returns></returns>
        string retrive_code_only(string allcode)
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
                        if (all_line.Substring(j, 1) == "*" && all_line.Substring(j + 1, 1) == "/") // check for comments in multi line
                        { comm_multi_line = false;
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
              }

            return code_line.ToString() ;
        }

    }
}
