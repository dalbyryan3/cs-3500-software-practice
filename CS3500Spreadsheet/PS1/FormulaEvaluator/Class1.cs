using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    public static class Evaluator
    {
        public delegate int Lookup(String v);

        /// <summary>
        /// Will evaluate a given integer arithmetic expression using standard
        /// infix notation and a delegate to determine the value of any variable
        /// given in the arithmetic expression.  
        /// </summary>
        /// <param name="exp">An arithmetic expression</param>
        /// <param name="variableEvaluator">Delegate which takes a string that represents a variable in the expression and will return an int or an ArugmentException based on its value.</param>
        /// <returns>Value of arithmetic expression</returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            //Here we split our string into tokens
            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            
            //Here we declare our two stacks to evaluate infix expressions. No need to be generic becasue of strict operator guidlines given.
            Stack<int> values = new Stack<int>();
            Stack<string> operators = new Stack<string>();
  
            //Here we will parse our expression
            foreach(string str in substrings)
            {
                string t = str.Trim();
                if (t == "" || Regex.IsMatch(t, "^[ ]+$")) //t is an empty string
                { 
                    continue; //Ignore all empty strings when parsing
                }
                bool tIsInteger = Regex.IsMatch(t, "^[0-9]+$"); //t is an integer, including zero but no negatives
                bool tIsVariable = Regex.IsMatch(t, "^[a-zA-Z]+[0-9]+$"); //t is a variable as defined by having at least one letter then at least one number
                if (tIsInteger || tIsVariable) //t is an integer or variable
                {
                    int tInt;
                    if (tIsInteger)
                    {
                        tInt = int.Parse(t); //It is known that t is an integer
                    }
                    else //Means tIsVariable
                    {
                        try
                        {
                            tInt = variableEvaluator(t);
                        }
                        catch (ArgumentException)
                        {
                            throw; //Will rethrow ArgumentException
                        }
                    }
                    //Will perform * or / operation with tInt if there is a value on stack and operator stack has a * or /
                    if (values.Count != 0 && (operators.IsOnTop("*") || operators.IsOnTop("/")))
                    {
                        if (operators.Peek() == "*") //If multiplication 
                        {
                            operators.Pop();
                            int result = values.Pop() * tInt;
                            values.Push(result);
                        }
                        else //Then division
                        {
                            operators.Pop();
                            int dividend = values.Pop();
                            try
                            {
                                int result = dividend / tInt;
                                values.Push(result);
                            }
                            catch (DivideByZeroException)
                            {
                                throw new ArgumentException("Divide by zero");
                            }
                        }
                    }
                    else //Just push t if we have no values on the stack or do not have to * or /
                    {
                        values.Push(tInt);
                    }
                }
                else if (t == "+" || t == "-") //t is a + or -
                {
                    //If operator on stack is + or - will attempt to evaluate last part of expression
                    applyPlusOrMinus(values, operators);
                    operators.Push(t);  //Regardless of if we performed an operation or not we will push our new operator onto the stack
                }
                else if (t == "*" || t == "/") //t is a * or /
                {
                    operators.Push(t);
                }
                else if (t == "(") //t is a left parenthesis
                {
                    operators.Push(t);
                }
                else if (t == ")") //t is a right parenthesis
                {
                    //If operator on stack is + or - will attempt to evaluate last part of expression
                    applyPlusOrMinus(values, operators);

                    //Now top of operator stack should be ( and will remove
                    if (operators.Count != 0 && operators.Peek() == "(")
                    {
                        operators.Pop();
                    }
                    else
                    {
                        throw new ArgumentException("( is not on top of operator stack when it should be");
                    }

                    //Now will check if * / are on top of operator stack and evaluate if so
                    if (operators.IsOnTop("*") || operators.IsOnTop("/"))
                    {
                        if (values.Count >= 2)
                        {
                            int val2 = values.Pop(); //Keep in mind order of stack
                            int val1 = values.Pop();
                            if (operators.Peek() == "*") //If multiplication 
                            {
                                operators.Pop();
                                int result = val1 * val2;
                                values.Push(result);
                            }
                            else if (operators.Peek() == "/") //If division
                            {
                                operators.Pop();
                                try
                                {
                                    int result = val1 / val2;
                                    values.Push(result);
                                }
                                catch (DivideByZeroException)
                                {
                                    throw new ArgumentException("Divide by 0");
                                }
                            }
                        }
                        else //Means stack contains less than 2 values 
                        {
                            throw new ArgumentException("Stack contains less than 2 values");
                        }

                    }



                }
                else //If parsed token was none of the possible token values
                {
                    throw new ArgumentException("Token given was not one of the possible token values");
                }

            }



            //Here we will look at our stict stacks ending conditions based on the algorithm 
            if(operators.Count == 0 && values.Count == 1) //operator stack is empty thus value stack should contain a single number
            {
                return values.Pop();
            }

            else if(operators.Count == 1 && values.Count == 2 && (operators.Peek() == "+" || operators.Peek() == "-"))
            {
                
                int val2 = values.Pop(); //Keep in mind order of stack
                int val1 = values.Pop();
                if (operators.Pop() == "-") //If subtraction we will make val 2 negative to have same effect
                {
                    val2 *= -1;
                }

                int result = val1 + val2;
                return result;
                
            } //operator stack is not empty so should have two values and either a + or -

            else //If code gets here there was a problem as we did not end up with any of the ending senarios the algorithm dictates
            {
                throw new ArgumentException("Invalid expression was passed in");
            }
        }

        /// <summary>
        /// This is a helper method.
        /// If a + or - is on the top of the operator stack will pop the value stack twice and the operator stack once
        /// to apply a + or -.  If the value stack has less than two values will throw an error.  
        /// Will do nothing if there is no + or - on the top of the stack.
        /// </summary>
        /// <param name="values">A stack of integers</param>
        /// <param name="operators">A stack of operators</param>
        private static void applyPlusOrMinus(Stack<int> values, Stack<string> operators)
        {
            //If operator on stack is + or - will attempt to evaluate last part of expression
            if (operators.IsOnTop("+") || operators.IsOnTop("-"))
            {
                if (values.Count >= 2)
                {
                    int val2 = values.Pop(); //Keep in mind order of stack
                    int val1 = values.Pop();
                    if (operators.Pop() == "-") //If subtraction we will make val 2 negative to have same effect
                    {
                        val2 *= -1;
                    }
                    int result = val1 + val2;
                    values.Push(result);
                }
                else //This implies that our values has fewer than 2 values when trying to pop it
                {
                    throw new ArgumentException("Values stack has less than 2 values");
                }

            }
        }
    }

    static class PS1StackExtensions
    {
        /// <summary>
        /// This method is an extension of the stack class to help 
        /// simplify checking if a specific value is on top of the 
        /// stack without throwing an exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="val">value to check if on top of stack</param>
        /// <returns></returns>
        public static bool IsOnTop<T>(this Stack<T> s, T val)
        {
            if(s.Count > 0 && val.Equals(s.Peek()))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
