// Spreadsheet tests written by Ryan Dalby

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Xml;
using System.IO;

namespace SpreadsheetTests
{
    [TestClass]
    public class UnitTest1
    {

        /// <summary>
        /// Helper method that builds a Spreadsheet with specific contents in order to test functionalities 
        /// only testable on a Spreadsheet that already has values.
        /// </summary>
        /// <returns>A Spreadsheet with values</returns>
        static AbstractSpreadsheet buildSheet()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=B1 + 3");
            s.SetContentsOfCell("B1", "5.0");
            s.SetContentsOfCell("C1", "=A2");
            s.SetContentsOfCell("B2", "=A1 * 2");
            s.SetContentsOfCell("C2", "6.0");
            s.SetContentsOfCell("A3", "Hello World");
            s.SetContentsOfCell("B3", "=B1 * 3");

            return s;
        }
        /// <summary>
        /// Helper method will check if two lists have the exact same contents
        /// in a specific order.
        /// </summary>
        /// <param name="l1">list 1</param>
        /// <param name="l2">list 2</param>
        /// <returns>if equal</returns>
        static bool areListsEqual(IList<string> l1, IList<string> l2)
        {
            return l1.SequenceEqual<string>(l2);
        }

        // General Tests:
        [TestMethod]
        public void TestGetNamesOfAllNonemptyCells()
        {
            AbstractSpreadsheet s = buildSheet();
            // Remove some content
            s.SetContentsOfCell("A1", "");
            s.SetContentsOfCell("B1", "");
            s.SetContentsOfCell("B2", "");
            s.SetContentsOfCell("C1", "");

            IEnumerator<string> e = s.GetNamesOfAllNonemptyCells().GetEnumerator();
            e.MoveNext();
            string val1 = e.Current;
            e.MoveNext();
            string val2 = e.Current;
            e.MoveNext();
            string val3 = e.Current;
            Assert.IsTrue(val1 == "A3" || val1 == "B3" || val1 == "C2");
            Assert.IsTrue(val2 == "A3" || val2 == "B3" || val2 == "C2");
            Assert.IsTrue(val3 == "A3" || val3 == "B3" || val3 == "C2");

        }

        [TestMethod]
        public void TestGeneralSet()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1","=A1*2");
            s.SetContentsOfCell("C1", "=B1+A1");
            Assert.IsTrue(areListsEqual(s.SetContentsOfCell("A1", "5.0"), new List<string>() { "A1", "B1", "C1" }));
        }
        [TestMethod]
        public void TestGeneralSetAndGetCellContents()
        {
            AbstractSpreadsheet s = buildSheet();

            Assert.AreEqual(s.GetCellContents("A1").GetType(), typeof(Formula)); 
            Assert.AreEqual("", s.GetCellContents("A2"));
            Assert.AreEqual("Hello World", s.GetCellContents("A3"));
            Assert.AreEqual(5.0, s.GetCellContents("B1"));
            Assert.AreEqual(s.GetCellContents("B2").GetType(), typeof(Formula)); 
            Assert.AreEqual(s.GetCellContents("B3").GetType(), typeof(Formula));
            Assert.AreEqual(s.GetCellContents("C1").GetType(), typeof(Formula));
            Assert.AreEqual(6.0, s.GetCellContents("C2"));
            Assert.AreEqual("", s.GetCellContents("C3"));
        }


        // Test SetCellContents(String name, double number) indirectly:
        [TestMethod]
        public void TestSetContentsOfCellDbl()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.IsTrue(areListsEqual(s.SetContentsOfCell("A1", "5.0"), new List<string>() { "A1" }));
            Assert.IsTrue(areListsEqual(s.SetContentsOfCell("B1", "=A1"), new List<string>() { "B1" }));
            Assert.IsTrue(areListsEqual(s.SetContentsOfCell("A1", "6.0"), new List<string>() { "A1", "B1" }));
        }
        [TestMethod]
        public void TestSetContentsOfCellOverrideDbl()
        {
            AbstractSpreadsheet s = buildSheet();
            s.SetContentsOfCell("A1", "5.5"); // Set formula to double
            s.SetContentsOfCell("B1", "8.3"); // Set double to double
            s.SetContentsOfCell("A3", "5.3"); // Set string to double

            Assert.AreEqual(5.5, s.GetCellContents("A1"));
            Assert.AreEqual(8.3, s.GetCellContents("B1"));
            Assert.AreEqual(5.3, s.GetCellContents("A3"));

        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetContentsOfCellNullNameDbl()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "5.0");
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetContentsOfCellInvalidNameDbl()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("2x", "5.0");
        }


        // Test SetCellContents(String name, String text) indirectly:
        [TestMethod]
        public void TestSetContentsOfCellStr()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.IsTrue(areListsEqual(s.SetContentsOfCell("A1", "hi"), new List<string>() { "A1" }));
            Assert.IsTrue(areListsEqual(s.SetContentsOfCell("B1", "=A1"), new List<string>() { "B1" }));
            Assert.IsTrue(areListsEqual(s.SetContentsOfCell("A1", "hello"), new List<string>() { "A1", "B1" }));
        }
        [TestMethod]
        public void TestSetContentsOfCellOverrideStr()
        {
            AbstractSpreadsheet s = buildSheet();
            s.SetContentsOfCell("A1", "hi"); // Set formula to string
            s.SetContentsOfCell("B1", "hello"); // Set double to string
            s.SetContentsOfCell("A3", "test"); // Set string to string

            Assert.AreEqual("hi", s.GetCellContents("A1"));
            Assert.AreEqual("hello", s.GetCellContents("B1"));
            Assert.AreEqual("test", s.GetCellContents("A3"));

        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetContentsOfCellNullTextStr()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            string text = null;
            s.SetContentsOfCell("A1", text);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetContentsOfCellNullNameStr()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "hi");
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetContentsOfCellInvalidNameStr()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("2x", "hi");
        }


        // Test SetCellContents(String name, Formula formula) indirectly:
        [TestMethod]
        public void TestSetContentsOfCellFormula()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.IsTrue(areListsEqual(s.SetContentsOfCell("A1", "=A2"), new List<string>() { "A1" }));
            Assert.IsTrue(areListsEqual(s.SetContentsOfCell("B1", "=A1"), new List<string>() { "B1" }));
            Assert.IsTrue(areListsEqual(s.SetContentsOfCell("A1", "=C1"), new List<string>() { "A1", "B1" }));
        }
        [TestMethod]
        public void TestSetContentsOfCellOverrideFormula()
        {
            AbstractSpreadsheet s = buildSheet();
            s.SetContentsOfCell("A1", "=B1"); // Set formula to formula
            s.SetContentsOfCell("B1", "=C3"); // Set double to formula
            s.SetContentsOfCell("A3", "=C2"); // Set string to formula

            Assert.AreEqual(s.GetCellContents("A1").GetType(), typeof(Formula));
            Assert.AreEqual(s.GetCellContents("B1").GetType(), typeof(Formula));
            Assert.AreEqual(s.GetCellContents("A3").GetType(), typeof(Formula));

        }
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetContentsOfCellCircularException()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "5.0");
            s.SetContentsOfCell("B1", "=A1");
            s.SetContentsOfCell("A1", "=B1");
        }
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetContentsOfCellCircularException2()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=B1*2");
            s.SetContentsOfCell("B1", "=C1*2");
            s.SetContentsOfCell("C1", "=A1*2");
        }

        // Test SetContentsOfCell:
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetContentsOfCellNullContent()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", null);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSetContentsOfCellInvalidFormula()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=+B1*/2");
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSetContentsOfCellEmptyInvalidFormula()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=");
        }

        // Test GetCellContents:

        [TestMethod]
        public void TestGetCellContentsValidNames()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            // Test names that should still be valid and give us an empty value
            Assert.AreEqual("", s.GetCellContents("A15"));
            Assert.AreEqual("", s.GetCellContents("a15"));
            Assert.AreEqual("", s.GetCellContents("XY032"));
            Assert.AreEqual("", s.GetCellContents("BC7"));
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsNull()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsInvalidName1()
        {

            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents("25");
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsInvalidName2()
        {

            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents("2x");
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsInvalidName3()
        {

            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents("&");
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsInvalidName4()
        {

            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents("Z");
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsInvalidName5()
        {

            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents("X_");
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsInvalidName6()
        {

            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents("hello");
        }

        // Test GetCellValue:
        [TestMethod]
        public void TestGetCellValueStr()
        {
            AbstractSpreadsheet s = buildSheet();
            Assert.AreEqual("Hello World", s.GetCellValue("A3"));

        }
        [TestMethod]
        public void TestGetCellValueDbl()
        {
            AbstractSpreadsheet s = buildSheet();
            Assert.AreEqual(5.0, s.GetCellValue("B1"));

        }
        [TestMethod]
        public void TestGetCellValueFormula()
        {
            AbstractSpreadsheet s = buildSheet();
            Assert.AreEqual(8.0, s.GetCellValue("A1"));

        }
        [TestMethod]
        public void TestGetCellValueFormulaError()
        {
            AbstractSpreadsheet s = buildSheet();
            s.SetContentsOfCell("B1", "apple");
            FormulaError val = (FormulaError) s.GetCellValue("A1");
            Assert.AreEqual("Lookup of a variable failed", val.Reason); // Lookup of B1 should fail since it does not map to number
        }
        [TestMethod]
        public void TestGetCellValueEmptyCell()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellValue("A1")); 
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellValueNullName()
        {

            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellValue(null);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellValueInvalidName()
        {

            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellValue("&");
        }

        // Test Save and GetSavedVersion:
        [TestMethod]
        public void TestSaveAndGetSavedVersion()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "6.0");
            s.SetContentsOfCell("A1", "=B1*2");
            s.Save("File1.xml");
            Assert.AreEqual("default", s.GetSavedVersion("File1.xml"));
        }
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestGetSaveNull()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.Save(null);
        }
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestGetSavedVersionNonExistentFile()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetSavedVersion("FileDoesNotExist");
        }
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestGetSavedVersionNullFile()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetSavedVersion(null);
        }
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestSaveNonExistentFilepath()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.Save("/some/random/stuff.xml"); // Invalid in both Windows and Linux
        }
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestGetSavedVersionNoVersion()
        {
            // Construct a new Spreadsheet file manually
            using (XmlWriter writer = XmlWriter.Create("save1.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            AbstractSpreadsheet ss = new Spreadsheet();
            ss.GetSavedVersion("save1.txt");
        }
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestGetSavedVersionNoVersion2()
        {
            // Construct a new Spreadsheet file manually
            using (XmlWriter writer = XmlWriter.Create("save2.txt"))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteEndDocument();
            }

            AbstractSpreadsheet ss = new Spreadsheet();
            ss.GetSavedVersion("save2.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestGetSavedVersionXmlException()
        {
            // Construct a new Spreadsheet file manually
            using (StreamWriter writer = new StreamWriter("save3.txt"))
            {
                writer.Write("<tag");
            }

            AbstractSpreadsheet ss = new Spreadsheet();
            ss.GetSavedVersion("save3.txt");
        }

        // Test Multi-Argument Constructors (Reading a file into a Spreadsheet, using isValid and Normalize)
        [TestMethod]
        public void TestReadingFile()
        {
            // Construct a new Spreadsheet file manually
            using (XmlWriter writer = XmlWriter.Create("save4.txt")) 
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            AbstractSpreadsheet ss = new Spreadsheet("save4.txt", s => true, s => s, "");
            Assert.AreEqual("hello", ss.GetCellContents("A1"));
            Assert.AreEqual("", ss.GetSavedVersion("save4.txt"));
            ss.SetContentsOfCell("B1", "6.0");
            Assert.AreEqual(6.0, ss.GetCellValue("B1"));
            ss.Save("save5.xml");
            Assert.AreEqual("", ss.GetSavedVersion("save5.xml"));
        }
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestReadingFileVersionMismatch()
        {
            // Construct a new Spreadsheet file manually
            using (XmlWriter writer = XmlWriter.Create("save6.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "defaultMismatch");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            AbstractSpreadsheet ss = new Spreadsheet("save6.txt", s => true, s => s, "default");
        }
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestReadingFileInvalidName()
        {
            // Construct a new Spreadsheet file manually
            using (XmlWriter writer = XmlWriter.Create("save7.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "1A");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            AbstractSpreadsheet ss = new Spreadsheet("save7.txt", s => true, s => s, "");
        }
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestReadingFileCircularException()
        {
            // Construct a new Spreadsheet file manually
            using (XmlWriter writer = XmlWriter.Create("save8.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "=B1");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "B1");
                writer.WriteElementString("contents", "=A1");
                writer.WriteEndElement();


                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            AbstractSpreadsheet ss = new Spreadsheet("save8.txt", s => true, s => s, "");
        }
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestReadingFileFormulaFormatExceptioh()
        {
            // Construct a new Spreadsheet file manually
            using (XmlWriter writer = XmlWriter.Create("save9.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "=.$%&#");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            AbstractSpreadsheet ss = new Spreadsheet("save9.txt", s => true, s => s, "");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestReadingFileXmlException()
        {
            // Construct a new Spreadsheet file manually
            using (XmlWriter writer = XmlWriter.Create("save10.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "=hello");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            using (StreamWriter writer = new StreamWriter("save10.txt",true))
            {
                writer.Write("<tag");
            }

            AbstractSpreadsheet ss = new Spreadsheet("save10.txt", s => true, s => s, "");
        }
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestReadingFileError()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetSavedVersion("FileDoesNotExist");
        }
        [TestMethod]
        public void TestNormalize()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s=>true, s=>s.ToUpper(), "");
            ss.SetContentsOfCell("a1", "hello");
            Assert.AreEqual("hello", ss.GetCellContents("A1"));
            Assert.AreEqual("hello", ss.GetCellValue("A1"));
            Assert.AreEqual("hello", ss.GetCellContents("a1"));
            Assert.AreEqual("hello", ss.GetCellValue("a1"));
            ss.SetContentsOfCell("A2", "=5.0");
            ss.SetContentsOfCell("b1", "=a2");
            Assert.AreEqual(5.0, ss.GetCellValue("B1"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestIsValid()
        {
            
            Func<string, bool> isValid = isValidMethod;
            AbstractSpreadsheet ss = new Spreadsheet(isValid, s => s, "");
            ss.SetContentsOfCell("B1", "5.0");
            ss.SetContentsOfCell("A1", "hello");
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestIsValid2()
        {
            Func<string, bool> isValid = isValidMethod;
            AbstractSpreadsheet ss = new Spreadsheet(isValid, s => s, "");
            ss.SetContentsOfCell("B1", "5.0");
            ss.SetContentsOfCell("A2", "=A1");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool isValidMethod(string arg)
        {
            if (arg == "A1")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // Test Changed Property
        [TestMethod]
        public void TestChangedFalse()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.IsFalse(s.Changed);
            s.SetContentsOfCell("A1", "=B1*2");
            s.Save("File1");
            Assert.IsFalse(s.Changed);
        }
        [TestMethod]
        public void TestChangedTrue()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=B1*2");
            Assert.IsTrue(s.Changed);
        }

        // Stress Test:
        [TestMethod]
        public void StressTest()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            for (int i = 0; i < 1000; i++)
            {
                string name = "A" + i.ToString();
                string content = i.ToString();
                s.SetContentsOfCell(name, content);
                s.GetCellValue(name);
            }
        }

        [TestMethod]
        public void StressTest2()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 0; i < 500; i++)
            {
                s.SetContentsOfCell("A1" + i, "=A1" + (i + 1));
            }
            LinkedList<string> firstCells = new LinkedList<string>();
            LinkedList<string> lastCells = new LinkedList<string>();
            for (int i = 0; i < 250; i++)
            {
                firstCells.AddFirst("A1" + i);
                lastCells.AddFirst("A1" + (i + 250));
            }
            Assert.IsTrue(s.SetContentsOfCell("A1249", "25.0").SequenceEqual(firstCells));
            Assert.IsTrue(s.SetContentsOfCell("A1499", "0").SequenceEqual(lastCells));
        }
    }


    /// <summary>
    ///This is a test class for SpreadsheetTest and is intended
    ///to contain all SpreadsheetTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GradingTests
    {

        // Verifies cells and their values, which must alternate.
        public void VV(AbstractSpreadsheet sheet, params object[] constraints)
        {
            for (int i = 0; i < constraints.Length; i += 2)
            {
                if (constraints[i + 1] is double)
                {
                    Assert.AreEqual((double)constraints[i + 1], (double)sheet.GetCellValue((string)constraints[i]), 1e-9);
                }
                else
                {
                    Assert.AreEqual(constraints[i + 1], sheet.GetCellValue((string)constraints[i]));
                }
            }
        }


        // For setting a spreadsheet cell.
        public IEnumerable<string> Set(AbstractSpreadsheet sheet, string name, string contents)
        {
            List<string> result = new List<string>(sheet.SetContentsOfCell(name, contents));
            return result;
        }

        // Tests IsValid
        [TestMethod, Timeout(5000)]
        public void IsValidTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "x");
        }

        [TestMethod, Timeout(5000)]
        [ExpectedException(typeof(InvalidNameException))]
        public void IsValidTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("A1", "x");
        }

        [TestMethod, Timeout(5000)]
        public void IsValidTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "= A1 + C1");
        }

        [TestMethod, Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void IsValidTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("B1", "= A1 + C1");
        }

        // Tests Normalize
        [TestMethod, Timeout(5000)]
        public void NormalizeTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("", s.GetCellContents("b1"));
        }

        [TestMethod, Timeout(5000)]
        public void NormalizeTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("hello", ss.GetCellContents("b1"));
        }

        [TestMethod, Timeout(5000)]
        public void NormalizeTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            s.SetContentsOfCell("A1", "6");
            s.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(5.0, (double)s.GetCellValue("B1"), 1e-9);
        }

        [TestMethod, Timeout(5000)]
        public void NormalizeTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("a1", "5");
            ss.SetContentsOfCell("A1", "6");
            ss.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(6.0, (double)ss.GetCellValue("B1"), 1e-9);
        }

        // Simple tests
        [TestMethod, Timeout(5000)]
        public void EmptySheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            VV(ss, "A1", "");
        }


        [TestMethod, Timeout(5000)]
        public void OneString()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneString(ss);
        }

        public void OneString(AbstractSpreadsheet ss)
        {
            Set(ss, "B1", "hello");
            VV(ss, "B1", "hello");
        }


        [TestMethod, Timeout(5000)]
        public void OneNumber()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneNumber(ss);
        }

        public void OneNumber(AbstractSpreadsheet ss)
        {
            Set(ss, "C1", "17.5");
            VV(ss, "C1", 17.5);
        }


        [TestMethod, Timeout(5000)]
        public void OneFormula()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneFormula(ss);
        }

        public void OneFormula(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "5.2");
            Set(ss, "C1", "= A1+B1");
            VV(ss, "A1", 4.1, "B1", 5.2, "C1", 9.3);
        }


        [TestMethod, Timeout(5000)]
        public void Changed()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Assert.IsFalse(ss.Changed);
            Set(ss, "C1", "17.5");
            Assert.IsTrue(ss.Changed);
        }


        [TestMethod, Timeout(5000)]
        public void DivisionByZero1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero1(ss);
        }

        public void DivisionByZero1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "0.0");
            Set(ss, "C1", "= A1 / B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }

        [TestMethod, Timeout(5000)]
        public void DivisionByZero2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero2(ss);
        }

        public void DivisionByZero2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "5.0");
            Set(ss, "A3", "= A1 / 0.0");
            Assert.IsInstanceOfType(ss.GetCellValue("A3"), typeof(FormulaError));
        }



        [TestMethod, Timeout(5000)]
        public void EmptyArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            EmptyArgument(ss);
        }

        public void EmptyArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod, Timeout(5000)]
        public void StringArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            StringArgument(ss);
        }

        public void StringArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "hello");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod, Timeout(5000)]
        public void ErrorArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ErrorArgument(ss);
        }

        public void ErrorArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= C1");
            Assert.IsInstanceOfType(ss.GetCellValue("D1"), typeof(FormulaError));
        }


        [TestMethod, Timeout(5000)]
        public void NumberFormula1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula1(ss);
        }

        public void NumberFormula1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + 4.2");
            VV(ss, "C1", 8.3);
        }


        [TestMethod, Timeout(5000)]
        public void NumberFormula2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula2(ss);
        }

        public void NumberFormula2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "= 4.6");
            VV(ss, "A1", 4.6);
        }


        // Repeats the simple tests all together
        [TestMethod, Timeout(5000)]
        public void RepeatSimpleTests()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "17.32");
            Set(ss, "B1", "This is a test");
            Set(ss, "C1", "= A1+B1");
            OneString(ss);
            OneNumber(ss);
            OneFormula(ss);
            DivisionByZero1(ss);
            DivisionByZero2(ss);
            StringArgument(ss);
            ErrorArgument(ss);
            NumberFormula1(ss);
            NumberFormula2(ss);
        }

        // Four kinds of formulas
        [TestMethod, Timeout(5000)]
        public void Formulas()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formulas(ss);
        }

        public void Formulas(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.4");
            Set(ss, "B1", "2.2");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= A1 - B1");
            Set(ss, "E1", "= A1 * B1");
            Set(ss, "F1", "= A1 / B1");
            VV(ss, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0);
        }

        [TestMethod, Timeout(5000)]
        public void Formulasa()
        {
            Formulas();
        }

        [TestMethod, Timeout(5000)]
        public void Formulasb()
        {
            Formulas();
        }


        // Are multiple spreadsheets supported?
        [TestMethod, Timeout(5000)]
        public void Multiple()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            AbstractSpreadsheet s2 = new Spreadsheet();
            Set(s1, "X1", "hello");
            Set(s2, "X1", "goodbye");
            VV(s1, "X1", "hello");
            VV(s2, "X1", "goodbye");
        }

        [TestMethod, Timeout(5000)]
        public void Multiplea()
        {
            Multiple();
        }

        [TestMethod, Timeout(5000)]
        public void Multipleb()
        {
            Multiple();
        }

        [TestMethod, Timeout(5000)]
        public void Multiplec()
        {
            Multiple();
        }

        // Reading/writing spreadsheets
        [TestMethod, Timeout(5000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save(Path.GetFullPath("/missing/save.txt"));
        }

        [TestMethod, Timeout(5000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest2() 
        {
            AbstractSpreadsheet ss = new Spreadsheet(Path.GetFullPath("/missing/save.txt"), s => true, s => s, "");
        }

        [TestMethod, Timeout(5000)]
        public void SaveTest3()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            Set(s1, "A1", "hello");
            s1.Save("save1.txt");
            s1 = new Spreadsheet("save1.txt", s => true, s => s, "default");
            Assert.AreEqual("hello", s1.GetCellContents("A1"));
        }

        [TestMethod, Timeout(5000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest4()
        {
            using (StreamWriter writer = new StreamWriter("save2.txt"))
            {
                writer.WriteLine("This");
                writer.WriteLine("is");
                writer.WriteLine("a");
                writer.WriteLine("test!");
            }
            AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s => s, "");
        }

        [TestMethod, Timeout(5000)]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest5()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("save3.txt");
            ss = new Spreadsheet("save3.txt", s => true, s => s, "version");
        }

        [TestMethod, Timeout(5000)]
        public void SaveTest6()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "hello");
            ss.Save("save4.txt");
            Assert.AreEqual("hello", new Spreadsheet().GetSavedVersion("save4.txt"));
        }

        [TestMethod, Timeout(5000)]
        public void SaveTest7()
        {
            using (XmlWriter writer = XmlWriter.Create("save5.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A2");
                writer.WriteElementString("contents", "5.0");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A3");
                writer.WriteElementString("contents", "4.0");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A4");
                writer.WriteElementString("contents", "= A2 + A3");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            AbstractSpreadsheet ss = new Spreadsheet("save5.txt", s => true, s => s, "");
            VV(ss, "A1", "hello", "A2", 5.0, "A3", 4.0, "A4", 9.0);
        }

        [TestMethod, Timeout(5000)]
        public void SaveTest8()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "hello");
            Set(ss, "A2", "5.0");
            Set(ss, "A3", "4.0");
            Set(ss, "A4", "= A2 + A3");
            ss.Save("save6.txt");
            using (XmlReader reader = XmlReader.Create("save6.txt"))
            {
                int spreadsheetCount = 0;
                int cellCount = 0;
                bool A1 = false;
                bool A2 = false;
                bool A3 = false;
                bool A4 = false;
                string name = null;
                string contents = null;

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                Assert.AreEqual("default", reader["version"]);
                                spreadsheetCount++;
                                break;

                            case "cell":
                                cellCount++;
                                break;

                            case "name":
                                reader.Read();
                                name = reader.Value;
                                break;

                            case "contents":
                                reader.Read();
                                contents = reader.Value;
                                break;
                        }
                    }
                    else
                    {
                        switch (reader.Name)
                        {
                            case "cell":
                                if (name.Equals("A1")) { Assert.AreEqual("hello", contents); A1 = true; }
                                else if (name.Equals("A2")) { Assert.AreEqual(5.0, Double.Parse(contents), 1e-9); A2 = true; }
                                else if (name.Equals("A3")) { Assert.AreEqual(4.0, Double.Parse(contents), 1e-9); A3 = true; }
                                else if (name.Equals("A4")) { contents = contents.Replace(" ", ""); Assert.AreEqual("=A2+A3", contents); A4 = true; }
                                else Assert.Fail();
                                break;
                        }
                    }
                }
                Assert.AreEqual(1, spreadsheetCount);
                Assert.AreEqual(4, cellCount); 
                Assert.IsTrue(A1);
                Assert.IsTrue(A2);
                Assert.IsTrue(A3);
                Assert.IsTrue(A4);
            }
        }


        // Fun with formulas
        [TestMethod, Timeout(5000)]
        public void Formula1()
        {
            Formula1(new Spreadsheet());
        }
        public void Formula1(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= b1 + b2");
            Assert.IsInstanceOfType(ss.GetCellValue("a1"), typeof(FormulaError));
            Assert.IsInstanceOfType(ss.GetCellValue("a2"), typeof(FormulaError));
            Set(ss, "a3", "5.0");
            Set(ss, "b1", "2.0");
            Set(ss, "b2", "3.0");
            VV(ss, "a1", 10.0, "a2", 5.0);
            Set(ss, "b2", "4.0");
            VV(ss, "a1", 11.0, "a2", 6.0);
        }

        [TestMethod, Timeout(5000)]
        public void Formula2()
        {
            Formula2(new Spreadsheet());
        }
        public void Formula2(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= a3");
            Set(ss, "a3", "6.0");
            VV(ss, "a1", 12.0, "a2", 6.0, "a3", 6.0);
            Set(ss, "a3", "5.0");
            VV(ss, "a1", 10.0, "a2", 5.0, "a3", 5.0);
        }

        [TestMethod, Timeout(5000)]
        public void Formula3()
        {
            Formula3(new Spreadsheet());
        }
        public void Formula3(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a3 + a5");
            Set(ss, "a2", "= a5 + a4");
            Set(ss, "a3", "= a5");
            Set(ss, "a4", "= a5");
            Set(ss, "a5", "9.0");
            VV(ss, "a1", 18.0);
            VV(ss, "a2", 18.0);
            Set(ss, "a5", "8.0");
            VV(ss, "a1", 16.0);
            VV(ss, "a2", 16.0);
        }

        [TestMethod, Timeout(5000)]
        public void Formula4()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formula1(ss);
            Formula2(ss);
            Formula3(ss);
        }

        [TestMethod, Timeout(5000)]
        public void Formula4a()
        {
            Formula4();
        }


        [TestMethod, Timeout(5000)]
        public void MediumSheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
        }

        public void MediumSheet(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "1.0");
            Set(ss, "A2", "2.0");
            Set(ss, "A3", "3.0");
            Set(ss, "A4", "4.0");
            Set(ss, "B1", "= A1 + A2");
            Set(ss, "B2", "= A3 * A4");
            Set(ss, "C1", "= B1 + B2");
            VV(ss, "A1", 1.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 3.0, "B2", 12.0, "C1", 15.0);
            Set(ss, "A1", "2.0");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 4.0, "B2", 12.0, "C1", 16.0);
            Set(ss, "B1", "= A1 / A2");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod, Timeout(5000)]
        public void MediumSheeta()
        {
            MediumSheet();
        }


        [TestMethod, Timeout(5000)]
        public void MediumSave()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
            ss.Save("save7.txt");
            ss = new Spreadsheet("save7.txt", s => true, s => s, "default");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod, Timeout(5000)]
        public void MediumSavea()
        {
            MediumSave();
        }


        // A long chained formula. Solutions that re-evaluate 
        // cells on every request, rather than after a cell changes,
        // will timeout on this test.
        // This test is repeated to increase its scoring weight
        [TestMethod, Timeout(7000)]
        public void LongFormulaTest()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        [TestMethod, Timeout(7000)]
        public void LongFormulaTest2()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        [TestMethod, Timeout(7000)]
        public void LongFormulaTest3()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        [TestMethod, Timeout(7000)]
        public void LongFormulaTest4()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        [TestMethod, Timeout(7000)]
        public void LongFormulaTest5()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        public void LongFormulaHelper(out object result)
        {
            try
            {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("sum1", "= a1 + a2");
                int i;
                int depth = 100;
                for (i = 1; i <= depth * 2; i += 2)
                {
                    s.SetContentsOfCell("a" + i, "= a" + (i + 2) + " + a" + (i + 3));
                    s.SetContentsOfCell("a" + (i + 1), "= a" + (i + 2) + "+ a" + (i + 3));
                }
                s.SetContentsOfCell("a" + i, "1");
                s.SetContentsOfCell("a" + (i + 1), "1");
                Assert.AreEqual(Math.Pow(2, depth + 1), (double)s.GetCellValue("sum1"), 1.0);
                s.SetContentsOfCell("a" + i, "0");
                Assert.AreEqual(Math.Pow(2, depth), (double)s.GetCellValue("sum1"), 1.0);
                s.SetContentsOfCell("a" + (i + 1), "0");
                Assert.AreEqual(0.0, (double)s.GetCellValue("sum1"), 0.1);
                result = "ok";
            }
            catch (Exception e)
            {
                result = e;
            }
        }

    }
}
