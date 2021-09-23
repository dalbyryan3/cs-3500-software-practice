// Tester for Formula made by Ryan Dalby u0848407

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;

namespace DevelopmentTests
{

    [TestClass]
    public class FormulaTester
    {
        [TestMethod]
        public void TestParsingError()
        {
            try
            {
                Formula f = new Formula("5 + %");
            }
            catch(FormulaFormatException e)
            {
                Assert.AreEqual("(Parsing) An invalid token was provided, the only valid tokens are (, ), +, -, *, /, variables, and decimal real numbers (including scientific notation)", e.Message);
            }
        }

        [TestMethod]
        public void TestInvalidVariableName()
        {
            try
            {
                Formula f = new Formula("#2");

            }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("(Parsing) An invalid token was provided, the only valid tokens are (, ), +, -, *, /, variables, and decimal real numbers (including scientific notation)", e.Message);
            }
        }

        [TestMethod]
        public void TestOneTokenRule()
        {
            try
            {
                Formula f = new Formula(" ");

            }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("(One Token Rule) The formula given has no tokens, there must be at least one token", e.Message);
            }
        }

        [TestMethod]
        public void TestRightParenthesesRule()
        {
            try
            {
                Formula f = new Formula("(8*3)) + 2");

            }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("(Right Parentheses Rule) The number of closing parentheses seen was greater than the number of opening parentheses seen going right to left.  Check balance of the parentheses.", e.Message);
            }
        }


        [TestMethod]
        public void TestBalancedParenthesesRule()
        {
            try
            {
                Formula f = new Formula("((8-3)");

            }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("(Balanced Parentheses Rule) The number of opening and closing parentheses for the formula given do not match.  Check the balance of the parentheses.", e.Message);
            }
        }

        [TestMethod]
        public void TestStartingTokenRule()
        {
            try
            {
                Formula f = new Formula("* 3");

            }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("(Starting Token Rule) An invalid starting token was provided, the only valid starting tokens are (, variables, and decimal real numbers (including scientific notation)", e.Message);
            }
        }

        [TestMethod]
        public void TestEndingTokenRule()
        {
            try
            {
                Formula f = new Formula("8 + 3 /");

            }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("(Ending Token Rule) An invalid ending token was provided, the only valid starting tokens are ), variables, and decimal real numbers (including scientific notation)", e.Message);
            }
        }

        [TestMethod]
        public void TestParenthesisOperatorFollowingRule()
        {
            try
            {
                Formula f = new Formula("7 * ( + 8 )");

            } 
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("(Parenthesis/Operator Following Rule) An invalid token following an open parenthesis or an operator was provided, the only valid tokens here are (, variables, and decimal real numbers (including scientific notation)", e.Message);
            }
            
        }
        [TestMethod]
        public void TestExtraFollowingRule()
        {
            try
            {
                Formula f = new Formula("8 + 3 5 ");

            }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("(Extra Following Rule) An invalid token following an number or a variable or an closing parenthesis was provided, the only valid tokens here are +, -, *, /, )", e.Message);
            }
        }

        [TestMethod]
        public void TestSimpleCorrectFormula()
        {
            Formula f = new Formula("5.3 + 3");
            Assert.AreEqual("5.3+3", f.ToString());
        }

        [TestMethod]
        public void TestComplexCorrectFormula()
        {
            Formula f = new Formula("(a1 + (5e-1 + 3)) * 8E1");
            Assert.AreEqual("(a1+(0.5+3))*80", f.ToString());
        }

        [TestMethod]
        public void TestFormulaWithIsValidConstraint()
        {
            try
            {
                Formula f = new Formula("5 * a1", s => s, s => false);

            }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("The formula given contains a token that is not valid based on the rules given by the implementer.  See implementaion of isValid details.", e.Message);
            }
        }

        [TestMethod]
        public void TestFormulaWithNormalizer()
        {
            Formula f = new Formula("5 * a1", s => s.ToUpper(), s => true);
            Assert.AreEqual("5*A1", f.ToString());
        }

        [TestMethod]
        public void TestFormulaWithNormalizerFail()
        {
            try
            {
                Formula f = new Formula("5 * a1", s => "1a", s => true);

            }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("(Parsing) An invalid token was provided, the only valid tokens are (, ), +, -, *, /, variables, and decimal real numbers (including scientific notation)", e.Message);
            }
        }

        [TestMethod]
        public void TestFormulaEquals()
        {
            // Test with formula, null, and non-formula
            Formula f = new Formula("5 * 3");
            Formula f2 = new Formula("5* 3");
            Formula f3 = new Formula("5 + 3");

            Assert.IsTrue(f.Equals(f2));
            Assert.IsFalse(f.Equals(f3));
            Assert.IsFalse(f.Equals(null));
            Assert.IsFalse(f.Equals(new List<string>()));

        }


        [TestMethod]
        public void TestFormulaEqualsOperator()
        {
            //Test null null, formula null, null formula, and formula formula
            Formula f = new Formula("5 * 3");
            Formula f2 = new Formula("5 +3");
            Formula f3 = new Formula("5*3");
            Formula f4 = null;
            Formula f5 = null;
            Assert.IsTrue(f4 == f5);
            Assert.IsFalse(f == f4);
            Assert.IsFalse(f4 == f);
            Assert.IsTrue(f == f3);
            Assert.IsFalse(f == f2);
            
        }


        [TestMethod]
        public void TestFormulaNotEqualsOperator()
        {
            //Test null null, formula null, null formula, and formula formula
            Formula f = new Formula("5 * 3");
            Formula f2 = new Formula("5 +3");
            Formula f3 = new Formula ("5*3");
            Formula f4 = null;
            Formula f5 = null;
            Assert.IsFalse(f4 != f5);
            Assert.IsTrue(f != f4);
            Assert.IsTrue(f4 != f);
            Assert.IsFalse(f != f3);
            Assert.IsTrue(f != f2);

        }

        [TestMethod]
        public void TestFormulaGetVariables()
        {
            Formula f = new Formula("5 * a1", s => s, s => true);
            IEnumerator<string> ans = f.GetVariables().GetEnumerator();
            ans.MoveNext();
            Assert.AreEqual("a1", ans.Current);
        }

        [TestMethod]
        public void TestFormulaToString()
        {
            Formula f = new Formula("5 * a1", s => s, s => true);
            Assert.AreEqual("5*a1", f.ToString());
        }

        [TestMethod]
        public void TestFormulaGetHashCode()
        {
            Formula f = new Formula("5 * a1", s => s, s => true);
            Formula f2 = new Formula("5 * a1", s => s, s => true);
            Assert.IsTrue(f.GetHashCode() == f2.GetHashCode());
        }




        /// <summary>
        /// This is a simple method that defines the values for variables that will be used in the infix expressions.
        /// It is the method that will be passed in as a delegate to the infix expression f.
        /// This contains variables that strictly have at least one letter followed by at least one number.
        /// </summary>
        /// <param name="s">A string that represents a variable</param>
        /// <returns>An integer that the variable evaluates to</returns>
        public static double Eval(string s)
        {
            if (s == "a1")
            {
                return 5.0;
            }
            else if (s == "A6")
            {
                return 6.5;
            }
            else if (s == "ASDFJjfkdjsakfjkdsjkfj03848456454848496")
            {
                return 8.2;
            }
            else if (s == "a123456789")
            {
                return 0.0;
            }
            else
            {
                throw new ArgumentException("Not a valid variable name");
            }
        }
    }


    /// <summary>
    /// This is the PS1 grading test class modified to perform the same
    /// evaluate tests using Formula objects for PS3
    ///</summary>
    [TestClass()]
    public class EvaluateTest
    {

        [TestMethod(), Timeout(5000)]
        public void TestSingleNumber()
        {
            Formula f = new Formula("5");
            Assert.AreEqual(5.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestSingleVariable()
        {
            Formula f = new Formula("X5");
            Assert.AreEqual(13.0, f.Evaluate(s => 13));
        }

        [TestMethod(), Timeout(5000)]
        public void TestAddition()
        {
            Formula f = new Formula("5+3");
            Assert.AreEqual(8.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestSubtraction()
        {
            Formula f = new Formula("18-10");
            Assert.AreEqual(8.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestMultiplication()
        {
            Formula f = new Formula("2*4");
            Assert.AreEqual(8.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestDivision()
        {
            Formula f = new Formula("16/2");
            Assert.AreEqual(8.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestArithmeticWithVariable()
        {
            Formula f = new Formula("2+X1");
            Assert.AreEqual(6.0, f.Evaluate(s => 4));
        }

        [TestMethod(), Timeout(5000)]
        public void TestLeftToRight()
        {
            Formula f = new Formula("2*6+3");
            Assert.AreEqual(15.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestOrderOperations()
        {
            Formula f = new Formula("2+6*3");
            Assert.AreEqual(20.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestParenthesesTimes()
        {
            Formula f = new Formula("(2+6)*3");
            Assert.AreEqual(24.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestTimesParentheses()
        {
            Formula f = new Formula("2*(3+5)");
            Assert.AreEqual(16.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestPlusParentheses()
        {
            Formula f = new Formula("2+(3+5)");
            Assert.AreEqual(10.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestPlusComplex()
        {
            Formula f = new Formula("2+(3+5*9)");
            Assert.AreEqual(50.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestOperatorAfterParens()
        {
            Formula f = new Formula("(1*1)-2/2");
            Assert.AreEqual(0.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexTimesParentheses()
        {
            Formula f = new Formula("2+3*(3+5)");
            Assert.AreEqual(26.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexAndParentheses()
        {
            Formula f = new Formula("2+3*5+(3+4*8)*5+2");
            Assert.AreEqual(194.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexMultiVar()
        {
            Formula f = new Formula("y1*3-8/2+4*(8-9*2)/14*x7");
            double ans = (4.0 * 3.0 - 8.0 / 2.0 + 4.0 * (8.0 - 9.0 * 2.0) / 14.0 * 1.0);
            Assert.AreEqual(ans, f.Evaluate(s => (s == "x7") ? 1 : 4));
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexNestedParensRight()
        {
            Formula f = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
            Assert.AreEqual(6.0, f.Evaluate(s => 1));
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexNestedParensLeft()
        {
            Formula f = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12.0, f.Evaluate(s => 2));
        }

        [TestMethod(), Timeout(5000)]
        public void TestRepeatedVar()
        {
            Formula f = new Formula("a4-a4*a4/a4");
            Assert.AreEqual(0.0, f.Evaluate(s => 3));
        }

        [TestMethod(), Timeout(5000)]
        public void TestDivideByZero()
        {
            Formula f = new Formula("5.0/0");
            object ans = f.Evaluate(s => 0);
            Assert.AreEqual(typeof(FormulaError), ans.GetType());
            Assert.AreEqual("Divide by zero", ((FormulaError)ans).Reason);
        }

        [TestMethod(), Timeout(5000)]
        public void TestLookUpFail()
        {
            Formula f = new Formula("z9 * 3");
            object ans = f.Evaluate(FormulaTester.Eval);
            Assert.AreEqual(typeof(FormulaError), ans.GetType());
            Assert.AreEqual("Lookup of a variable failed", ((FormulaError)ans).Reason);
        }



    }
}
