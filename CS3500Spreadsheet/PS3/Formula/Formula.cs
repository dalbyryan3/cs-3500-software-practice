// Implementation of PS3 skeleton code done by Ryan Dalby u0848407

// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private List<string> normalizedTokens; // This field will hold our normalized parsed tokens for the immutable formula
        private HashSet<string> variables; // This field will hold our variables that are in the formula
        private string normalizedFormula; // This field will hold our normalzied fomula, this will just be directly made up of our normalizedTokens concatenated together, will be useful to have hashable string available that uniquely identifies this specific formula

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            

            IEnumerable<string> tokens = GetTokens(formula); // Get formula's associated tokens
            normalizedTokens = new List<string>(); // Create new list to hold normalized tokens, will be used in Evaluate
            variables = new HashSet<string>(); // Create new set to hold our variables
            normalizedFormula = ""; // Create empty string to hold our complete normalized formula

            // Patterns for individual tokens (Have been slightly modified from the ones in GetTokens to remove whitespace and use start and end delimiters) 
            string lpPattern = @"^\(";
            string rpPattern = @"^\)";
            string opPattern = @"^[\+\-*/]";
            string varPattern = @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            string doublePattern = @"^((?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?)$";

            // The following regex string will indicate if something is a valid token
            string tokenPattern = string.Format("({0})|({1})|({2})|({3})|({4})", lpPattern, rpPattern, opPattern, varPattern, doublePattern);

            // Variables to assist with formula syntax checking
            int tokenCount = 0;  // Count of the number of tokens parsed
            int lpCount = 0; // Count of number of left parentheses in formula
            int rpCount = 0; // Count of number of right parentheses in formula
            bool lastTokenWasLpOp = false; // If last operator was a left parenthesis or an operator
            bool lastTokenWasDoubleVarRp = false; // If last operator was a number or variable or right parenthesis
            
            // Now we check if the formula is syntactically correct by parsing through the tokens and making syntax checks on the tokens and the formula's structure as a whole
            foreach (string t in tokens)
            {
                // First we must attempt to normalize if the token is a variable then check that the token is a valid token
                string tNorm = t; // This string holds our normalized token if a variable or just the current token if not a variable
                if (Regex.IsMatch(t, varPattern)) // Token is a variable so we will normalize the token
                {
                    tNorm = normalize(t); 
                }
                if (!Regex.IsMatch(tNorm, tokenPattern)) // Now that we normalized, check token is invalid 
                {
                    throw new FormulaFormatException("(Parsing) An invalid token was provided, the only valid tokens are (, ), +, -, *, /, variables, and decimal real numbers (including scientific notation)");

                }

                // Now we check for first and last token conditions along with following rules
                if (tokenCount == 0) // If this is first token
                {
                    string startingTokenConditions = string.Format("({0})|({1})|({2})", lpPattern, varPattern, doublePattern);
                    if (!Regex.IsMatch(tNorm, startingTokenConditions)) // If starting token conditions are violated
                    {
                        throw new FormulaFormatException("(Starting Token Rule) An invalid starting token was provided, the only valid starting tokens are (, variables, and decimal real numbers (including scientific notation)");
                    }
                }
                if (tokenCount == (tokens.Count() - 1)) // If this is last token
                {
                    string endingTokenConditions = string.Format("({0})|({1})|({2})", rpPattern, varPattern, doublePattern);
                    if (!Regex.IsMatch(tNorm, endingTokenConditions)) // If starting ending conditions are violated
                    {
                        throw new FormulaFormatException("(Ending Token Rule) An invalid ending token was provided, the only valid starting tokens are ), variables, and decimal real numbers (including scientific notation)");
                    }
                }
                if (lastTokenWasLpOp) // If last operator was a left parenthesis or an operator
                {
                    string parenthesisOperatorFollowingCondition = string.Format("({0})|({1})|({2})", lpPattern, varPattern, doublePattern);
                    if (!Regex.IsMatch(tNorm, parenthesisOperatorFollowingCondition)) // If Parenthesis/Operator Following Rule conditions are violated
                    {
                        throw new FormulaFormatException("(Parenthesis/Operator Following Rule) An invalid token following an open parenthesis or an operator was provided, the only valid tokens here are (, variables, and decimal real numbers (including scientific notation)");
                    }
                    lastTokenWasLpOp = false; // Reset flag

                }
                if (lastTokenWasDoubleVarRp) // If last operator was a number or variable or right parenthesis
                {
                    string extraFollowingCondition = string.Format("({0})|({1})", opPattern, rpPattern);
                    if (!Regex.IsMatch(tNorm, extraFollowingCondition)) // If Extra Following Rule conditions are violated
                    {
                        throw new FormulaFormatException("(Extra Following Rule) An invalid token following an number or a variable or an closing parenthesis was provided, the only valid tokens here are +, -, *, /, )");
                    }
                    lastTokenWasDoubleVarRp = false; // Reset flag
                }

                // Now we check specfifcally what type of token we have and check for overall structure of formula, and set flags for following rules if necessary.
                // We will also check for validity of variable name with user set restrictions.
                // We also know that our token is valid so it must match one of the following token checks.
                if (Regex.IsMatch(tNorm, opPattern)) // If we have an operator
                {
                    lastTokenWasLpOp = true; // Set flag for next token
                }
                else if (Regex.IsMatch(tNorm, rpPattern)) // If we have a right parentheses
                {
                    rpCount++;
                    if (rpCount > lpCount) // Check right parentheses rule
                    {
                        throw new FormulaFormatException("(Right Parentheses Rule) The number of closing parentheses seen was greater than the number of opening parentheses seen going right to left.  Check balance of the parentheses.");
                    }

                    lastTokenWasDoubleVarRp = true; // Set flag for next token
                }
                else if (Regex.IsMatch(tNorm, lpPattern)) // If we have a left parentheses
                {
                    lpCount++;
                    lastTokenWasLpOp = true; // Set flag for next token
                }
                else if (Regex.IsMatch(tNorm, varPattern)) // If we have a variable
                {
                    // Check if valid as dictated by user
                    if (!isValid(tNorm)) 
                    {
                        throw new FormulaFormatException("The formula given contains a token that is not valid based on the rules given by the implementer.  See implementaion of isValid details.");
                    }
                    
                    lastTokenWasDoubleVarRp = true; // Set flag for next token
                    variables.Add(tNorm); // We have a valid variable, add to set containing all of our variables in the formula, duplcates will not add

                }
                else // We have number
                {
                    tNorm = Double.Parse(tNorm).ToString(); // Will convert our number to a common format 
                                                            
                    lastTokenWasDoubleVarRp = true; // Set flag for next token
                }
                    
                // Now we have a valid token in our formula so we add the token to our fields for the formula
                tokenCount++; // Have parsed a token
                normalizedTokens.Add(tNorm); // Our normalized token is valid in our formula and will be added to our list of valid normalized tokens
                normalizedFormula += tNorm; // Builds our normalized formula represenation
            }

            if (tokenCount == 0) // Must have at least 1 token
            {
                throw new FormulaFormatException("(One Token Rule) The formula given has no tokens, there must be at least one token");
            }
            if (rpCount != lpCount) // Must have balanced number of parentheses
            {
                throw new FormulaFormatException("(Balanced Parentheses Rule) The number of opening and closing parentheses for the formula given do not match.  Check the balance of the parentheses.");
            }
            

        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            //Here we declare our two stacks to evaluate infix expressions. No need to be generic becasue of strict operator guidlines given.
            Stack<double> values = new Stack<double>();
            Stack<string> operators = new Stack<string>();

            try
            {
                foreach (string t in normalizedTokens)
                {
                    // Patterns for individual tokens (Have been slightly modified from the ones in GetTokens to remove whitespace and use start and end delimiters) 
                    string doublePattern = @"^((?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?)$";
                    string varPattern = @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*";
                    bool tIsDouble = Regex.IsMatch(t, doublePattern); // t is an number
                    bool tIsVariable = Regex.IsMatch(t, varPattern); // t is a variable

                    if (tIsDouble || tIsVariable)
                    {
                        double tDouble;
                        if (tIsDouble)
                        {
                            tDouble = Double.Parse(t); //It is known that t is an double
                        }
                        else // Means tIsVariable
                        {
                            tDouble = lookup(t); // May throw an ArgumentException will be caught
                        }
                        // Will perform * or / operation with tInt if the operator stack has a * or /, since our formula is correct we must have a value to perform the operation with
                        if (operators.IsOnTop("*") || operators.IsOnTop("/"))
                        {
                            double val1 = values.Pop();
                            applyMultOrDivide(values, operators, val1, tDouble);
                        }
                        else // Just push t if we have no values on the stack or do not have to * or /
                        {
                            values.Push(tDouble);
                        }
                    }
                    else if (t == "+" || t == "-") //t is a + or -
                    {
                        // If operator on stack is + or - will evaluate last part of expression since the formula is correct there should be at least two values on the stack 
                        if (operators.IsOnTop("+") || operators.IsOnTop("-"))
                        {
                            applyPlusOrMinus(values, operators);
                        }
                        operators.Push(t);  //Regardless of if we performed an operation or not we will push our new operator onto the stack
                    }
                    else if (t == "*" || t == "/") //t is a * or /
                    {
                        operators.Push(t);
                    }
                    else if (t == "(") // t is a left parenthesis
                    {
                        operators.Push(t);
                    }
                    else if (t == ")") // t is a right parenthesis
                    {
                        // If operator on stack is + or - will attempt to evaluate last part of expression
                        if (operators.IsOnTop("+") || operators.IsOnTop("-"))
                        {
                            applyPlusOrMinus(values, operators);
                        }

                        //Now top of operator stack should be ( and will remove
                        operators.Pop();
                        
                        // Now will check if * / are on top of operator stack and evaluate if so, we know we have a valid formula so we should have at least two values on stack if so
                        if (operators.IsOnTop("*") || operators.IsOnTop("/"))
                        {
                            double val2 = values.Pop(); //Keep in mind order of stack
                            double val1 = values.Pop();
                            applyMultOrDivide(values, operators, val1, val2);
                        }

                    }

                }
            }
            catch (ArgumentException)
            {
                return new FormulaError("Lookup of a variable failed");
            }
            catch (DivideByZeroException)
            {

                return new FormulaError("Divide by zero");
            }

            // Here we will look at our stict stacks ending conditions based on the algorithm, must be one of these two as we have a valid formula
            if (operators.Count == 0 && values.Count == 1) //operator stack is empty thus value stack should contain a single number
            {
                return values.Pop();
            }

            else // operator stack is not empty so should have two values and either a + or -
            {
                applyPlusOrMinus(values, operators);
                double result = values.Pop();
                return result;
            }

        }

        /// <summary>
        /// This is a helper method.
        /// Will pop the value stack twice and the operator stack once to apply a + or -
        /// then will push the result of the operation to the value stack.
        /// </summary>
        /// <param name="values">A stack of integers</param>
        /// <param name="operators">A stack of operators</param>
        private static void applyPlusOrMinus(Stack<double> values, Stack<string> operators)
        {
               
            double val2 = values.Pop(); //Keep in mind order of stack
            double val1 = values.Pop();
            if (operators.Pop() == "-") //If subtraction we will make val 2 negative to have same effect
            {
                val2 *= -1;
            }
            double result = val1 + val2;
            values.Push(result);

        }

        /// <summary>
        /// This is a helper method.
        /// Will apply a * or / then push the result to the value stack.
        /// The operation is applied in the order of val1 * val2 or val1 / val2
        /// Must take in val1 and val2 so this helper can be used for the tIsDouble || tIsVariable case where val2 is current token
        /// </summary>
        /// <param name="values">A stack of integers</param>
        /// <param name="operators">A stack of operators</param>
        /// <param name="val1">The first value to apply multiply or divide (dividend)</param>
        /// <param name="val2">The second value to apply multiply or divide to (divisor)</param>
        private static void applyMultOrDivide(Stack<double> values, Stack<string> operators, double val1, double val2)
        {
            if (operators.Peek() == "*") //If multiplication 
            {
                operators.Pop();
                double result = val1 * val2;
                values.Push(result);
            }
            else //Then division
            {
                operators.Pop();
                if (val2 == 0.0) // Will get infinity if we divide by 0 with a double 
                {
                    throw new DivideByZeroException();
                }
                double result = val1 / val2;
                values.Push(result);
            }
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return variables.AsEnumerable<String>(); // IEnumerable<String> is immutable
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return normalizedFormula; // string is immutable
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Formula)) // If we our obj is null or not a formula we return false
            {
                return false;
            }
            return this.ToString() == obj.ToString(); // Comparing the Formula's unique ToString (for a given formula) we can determine if two formulas are equal
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (ReferenceEquals(f1, null)) // If f1 is null then we see if the other is null, otherwise we return false
            {
                if (ReferenceEquals(f2, null))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return f1.Equals(f2); // Since f1 is not null we can compare using Equals
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1 == f2); // We will use our overloaded == operator as define above and invert it
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode(); // Will use our normalized string which uniquely represents the formula.
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
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
            if (s.Count > 0 && val.Equals(s.Peek()))
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




