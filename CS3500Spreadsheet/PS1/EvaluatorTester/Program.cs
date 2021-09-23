using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluatorTester
{
    public class Program
    {
        /// <summary>
        /// This is the main method to test the infix expression evaluator.
        /// Will run unit test-like tests then allow for continual user inputted expressions.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            runEvaluatorTests();  //Will run tests for the evaluator, will output results to the console

            //This allows for continual expression evaluation, testing is done in the corresponding testing project
            Console.WriteLine("\nNow can try continual expression evaluation: ");
            while (true)
            {
                try
                {
                    Console.Write("Type an infix expression: ");
                    string exp = Console.ReadLine();
                    int res = FormulaEvaluator.Evaluator.Evaluate(exp, Eval);
                    Console.Write("Result: ");
                    Console.WriteLine(res);
                }

                catch(ArgumentException)
                {
                    Console.WriteLine("Invalid Expression");
                }   
            }
        }

        /// <summary>
        /// This is a unit testing-like method which executes tests for the infix expression evaluator.
        /// </summary>
        public static void runEvaluatorTests()
        {
            string expression;
            //Test integers
            Console.Write("Test integers: ");
            expression = "15";
            assertPassEvaluate(expression, 15, "Failed with lone integer");

            //Test variables
            Console.Write("Test variables: ");
            expression = "a1";
            assertPassEvaluate(expression, 5, "Failed with lone variable");

            //Test add
            Console.Write("Test add: ");
            expression = "5+3";
            assertPassEvaluate(expression, 8, "Failed adding");

            //Test subtraction
            Console.Write("Test subtraction: ");
            expression = "5-3";
            assertPassEvaluate(expression, 2, "Failed subtracting");

            //Test multiplication
            Console.Write("Test multiplication: ");
            expression = "9*3";
            assertPassEvaluate(expression, 27, "Failed multiplicaiton");

            //Test division
            Console.Write("Test division: ");
            expression = "9/3";
            assertPassEvaluate(expression, 3, "Failed division");

            //Test parenthesis 
            Console.Write("Test parenthesis: ");
            expression = "(8)";
            assertPassEvaluate(expression, 8, "Failed parenthesis");

            //Test multiple parenthesis 
            Console.Write("Test multiple parenthesis: ");
            expression = "(((8)))";
            assertPassEvaluate(expression, 8, "Failed multiple parenthesis");

            //Test whitespace
            Console.Write("Test whitespace: ");
            expression = "  (   8       +      2     -3  ) /7 *8  ";
            assertPassEvaluate(expression, 8, "Failed multiple parenthesis");

            //Test complex expression- this expression contains all valid tokens: whitespace, non negative integers, variables, *, /, +, -, (, )
            Console.Write("Test complex expression: ");
            expression = "(( (   (  (((25)      + (ASDFJjfkdjsakfjkdsjkfj03848456454848496)) - (A6)) /    (3) )* 2)    + a123456789   ) * a1 ) / a1";
            assertPassEvaluate(expression, 18, "Failed complex expression");

            //Test throwing exception for multiple integers with no operators
            Console.Write("Test throws for multiple integers with no operators: ");
            expression = " 5 3 a1 8 ";
            assertThrowsEvaluate(expression, "Failed throws for multiple integers with no operators");

            //Test throwing exception for multiple subsequent operators
            Console.Write("Test throws for multiple subsequent operators: ");
            expression = " * / + - ";
            assertThrowsEvaluate(expression, "Failed throws for multiple subsequent operators");

            //Test throwing exception for division by 0
            Console.Write("Test throws for division by 0: ");
            expression = " 8 / 0 ";
            assertThrowsEvaluate(expression, "Failed throws for division by 0");

            //Test throwing exception for an undefined variable
            Console.Write("Test throws for an undefined variable: ");
            expression = " n5 ";
            assertThrowsEvaluate(expression, "Failed throws for an undefined variable");

            //Test throwing exception for an incorrect variable name
            Console.Write("Test throws for an incorrect variable name: ");
            expression = " 1a ";
            assertThrowsEvaluate(expression, "Failed throws for an incorrect variable name");

            //Test throwing when + or - and value stack contains less than 2 values when popping
            Console.Write("Test throws for when + or - and value stack contains less than 2 values when popping: ");
            expression = " 5 + + 2";
            assertThrowsEvaluate(expression, "Failed throws for when + or - and value stack contains less than 2 values when popping");

            //Test throwing when a parentheses does not properly enclose an expression 
            Console.Write("Test throws for when a parentheses does not properly enclose an expression: ");
            expression = " (  ( ( 8 + 3) * 2 )";
            assertThrowsEvaluate(expression, "Failed throws for when a parentheses does not properly enclose an expression");


        }

        /// <summary>
        /// This is a helper method which acts like an assertPass unit test method specifically for testing the infix expression evaluator.
        /// </summary>
        /// <param name="expression">Infix expression to evaluate</param>
        /// <param name="expected">Expected integer result of expression</param>
        /// <param name="failMessage">Message to display if the test fails</param>
        /// <returns></returns>
        public static bool assertPassEvaluate(string expression, int expected, string failMessage)
        {
            int actual = FormulaEvaluator.Evaluator.Evaluate(expression, Eval);
            if (actual == expected)
            {
                Console.WriteLine("Test Passed");
                return true;
            }
            else
            {
                Console.WriteLine("Test Failed- " + failMessage);
                return false;
            }
        }

        /// <summary>
        /// This is a helper method which acts like an assertThrows unit test method specifically for testing the infix expression evaluator.
        /// </summary>
        /// <param name="expression">Infix expression to evaluate</param>
        /// <param name="failMessage">Message to display if the test fails (Does not throw an ArgumentException)</param>
        /// <returns></returns>
        public static bool assertThrowsEvaluate(string expression, string failMessage)
        {
            try
            {
                int actual = FormulaEvaluator.Evaluator.Evaluate(expression, Eval);
                Console.WriteLine("Test Failed- " + failMessage);
                return false;
            }
            catch(ArgumentException)
            {
                Console.WriteLine("Test Passed");
                return true;
            }
        }

        /// <summary>
        /// This is a simple method that defines the values for variables that will be used in the infix expressions.
        /// It is the method that will be passed in as a delegate to the infix expression evaluator.
        /// This contains variables that strictly have at least one letter followed by at least one number.
        /// </summary>
        /// <param name="s">A string that represents a variable</param>
        /// <returns>An integer that the variable evaluates to</returns>
        public static int Eval(string s)
        {
            if (s == "a1")
            {
                return 5;
            }
            else if (s == "A6")
            {
                return 6;
            }
            else if (s == "ASDFJjfkdjsakfjkdsjkfj03848456454848496")
            {
                return 8;
            }
            else if (s == "a123456789")
            {
                return 0;
            }
            else
            {
                throw new ArgumentException("Not a valid variable name");
            }
        }

    }
}
