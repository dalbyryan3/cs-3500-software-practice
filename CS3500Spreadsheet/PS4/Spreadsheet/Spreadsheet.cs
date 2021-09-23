// Spreadsheet implementation written by Ryan Dalby

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using SpreadsheetUtilities;

namespace SS
{
    /// <summary>
    /// Implementation of AbstractSpreadsheet
    /// 
    /// A Spreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a cell name if and only if it consists of one or more letters,
    /// followed by one or more digits AND it satisfies the predicate IsValid.
    /// For example, "A15", "a15", "XY032", and "BC7" are cell names so long as they
    /// satisfy IsValid.  On the other hand, "Z", "X_", and "hello" are not cell names,
    /// regardless of IsValid.
    /// 
    /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
    /// must be normalized with the Normalize method before it is used by or saved in 
    /// this spreadsheet.  For example, if Normalize is s => s.ToUpper(), then
    /// the Formula "x3+a5" should be converted to "X3+A5" before use.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<String, Cell> cellsWithContent = new Dictionary<string, Cell>(); // cellsWithContent will store all cells that have content
        private DependencyGraph cellGraph = new DependencyGraph(); // Will hold our relationships between cells


        // ADDED FOR PS5
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get; protected set; } = false; // The Spreadsheet has not been changed initially; Auto implemented properties for get and set



        /// <summary>
        /// Creates an empty Spreadsheet with an infinite number of cells
        /// (empty means no content in cells) 
        /// This constructor has no extra IsValid constraints and Normalize maps to itself
        /// The version is "default"
        /// </summary>
        public Spreadsheet():base(s=>true, s=>s, "default")
        {

        }

        /// <summary>
        /// Creates an empty Spreadsheet with an infinite number of cells
        /// (empty means no content in cells) 
        /// This constructor takes in an IsValid constraint and a way to Normalize cell names
        /// it also takes in a version for the spreadsheet
        /// </summary>
        /// <param name="isValid">Imposes constrains on what a valid variable name is beyond Spreadsheet's definition</param>
        /// <param name="normalize">Normalizes cell names in a particular way</param>
        /// <param name="version">The version of the spreadsheet</param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {

        }
        /// <summary>
        /// Creates a Spreadsheet from a given Spreadsheet xml file
        /// This constructor takes in an IsValid constraint and a way to Normalize cell names
        /// it also takes in a version for the spreadsheet
        /// </summary>
        /// <param name="filepath">Filepath of the Spreadsheet to be read in</param>
        /// <param name="isValid">Imposes constrains on what a valid variable name is beyond Spreadsheet's definition</param>
        /// <param name="normalize">Normalizes cell names in a particular way</param>
        /// <param name="version">The version of the spreadsheet</param>
        public Spreadsheet(string filepath, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            // Read spreadsheet

            // Check filename matches 
            if (GetSavedVersion(filepath) != version)
            {
                throw new SpreadsheetReadWriteException("The version of the saved spreadsheet does not match the version parameter provided to the constructor");
            }
            CreateSpreadsheetFromFilepath(filepath);

            Changed = false;  // Must reset the Changed flag since we used SetContentsOfCell to build spreadsheet from file
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        /// <param name="name">cell name</param>
        /// <returns>content that is either a string, a double, or a Formula</returns>
        public override object GetCellContents(string name)
        {
            // Attempt to get cell
            object cell = GetCell(name);

            // Determine what GetCell gave us
            if (cell is Cell)
            {
                return ((Cell)cell).CellContent; // Returns value of cell
            }
            else // We have an empty cell
            {
                return cell;  // Returns ""
            }
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cellsWithContent.Keys.ToList();  // Will give an IEnumerable copy of the non empty cells stored in cells
        }

        // MODIFIED PROTECTION FOR PS5
        /// <summary>
        /// The contents of the named cell becomes number.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, double number)
        {
            // We know we have a valid cell name so we will update cellGraph and then update what cellsWithContent:

            // Remove all previous dependees of the cell (remove all previous relationships that this cell depends on) 
            cellGraph.ReplaceDependees(name, new HashSet<string>());

            // Add this cell to cellsWithContent (If cell already has content then replace with new content, if not just add new content)
            AddToCellsWithContent(name, number);

            return GetCellsToRecalculate(name).ToList(); // Will get a copy of a list of name and all cells that depend on it, will not get a circular dependency since we are not adding any dependencies

        }

        // MODIFIED PROTECTION FOR PS5
        /// <summary>
        /// The contents of the named cell becomes text.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, string text)
        {
            // We know text is not null
            // We know we have a valid cell name so we will update cellGraph and then update what cellsWithContent:

            // Remove all previous dependees of the cell (remove all previous relationships that this cell depends on) 
            cellGraph.ReplaceDependees(name, new HashSet<string>());

            if (text == "") // If our text is "" we have an empty string we want to remove the cell from our cellsWithContent
            {
                cellsWithContent.Remove(name);
            }
            else // Our text is not an empty string so add this cell to cellsWithContent (If cell already has content then replace with new content, if not just add new content)

            {
                AddToCellsWithContent(name, text);
            }

            return GetCellsToRecalculate(name).ToList(); // Will get a copy of a list of name and all cells that depend on it, will not get a circular dependency since we are not adding any dependencies
        }
        // MODIFIED PROTECTION FOR PS5
        /// <summary>
        /// If changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
        /// 
        /// Otherwise, the contents of the named cell becomes formula. The method returns a
        /// list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            // We know formula is not null
            // Get variables contained in formula passed in, get current dependees and dependents of the cell
            IEnumerable<string> formulaVariables = formula.GetVariables();
            IEnumerable<string> oldDependees = cellGraph.GetDependees(name);

            // Will modify the cellGraph and then check if we created circular dependencies, may have to undo
            cellGraph.ReplaceDependees(name, formulaVariables);

            IEnumerable<string> cellsToRecalculate; // Will hold results of GetCellsToRecalculate
            try
            {
                cellsToRecalculate = GetCellsToRecalculate(name);
            }
            catch(CircularException)
            {
                // Undo our cellGraph changes because we have a circular dependency
                cellGraph.ReplaceDependees(name, oldDependees);
                throw;
            }

            // Add this cell to cellsWithContent (If cell already has content then replace with new content, if not just add new content)
            AddToCellsWithContent(name, formula);

            return cellsToRecalculate.ToList(); // Will get a copy of a list of name and all cells that depend on it, already know we did not create a circular dependency

        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            // Requirements for throwing an ArgumentNullException if null and throwing an InvalidNameException if cell name isn't valid have been removed as per instructor
            Debug.Assert(name != null && IsValidCellName(name));

            return cellGraph.GetDependents(name).ToList();
        }
        // ADDED FOR PS5
        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            if (reader.Name == "spreadsheet")
                            {
                                string version = reader.GetAttribute("version");
                                if(version != null)
                                {
                                    return version;
                                }
                            }
                        }
                    }
                    throw new SpreadsheetReadWriteException("The version was not found");
                }
            }
            catch (ArgumentNullException)
            {
                throw new SpreadsheetReadWriteException("A null filename was given");
            }
            catch (System.IO.FileNotFoundException)
            {
                throw new SpreadsheetReadWriteException("File given was not found");
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                throw new SpreadsheetReadWriteException("Directory for file given was not found");
            }

            catch (SpreadsheetReadWriteException)
            {
                throw;
            }
            catch (XmlException)
            {
                throw new SpreadsheetReadWriteException("There was an error reading the XML of the given spreadsheet");
            }
        }
        // ADDED FOR PS5
        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>
        /// cell name goes here
        /// </name>
        /// <contents>
        /// cell contents goes here
        /// </contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "    ";
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);


                    foreach (KeyValuePair<string, Cell> pair in cellsWithContent)
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", pair.Key);
                        string content = DetermineCellContentToWrite(pair.Value);
                        writer.WriteElementString("contents", content);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                   
            }
            catch(ArgumentNullException)
            {
                throw new SpreadsheetReadWriteException("A null filename was given");
            }
            catch(Exception)
            {
                throw new SpreadsheetReadWriteException("There was an error writing the XML spreadsheet file to the given filename");
            }

            // If we successfully wrote a file we have a new saved version so we reset Changed
            Changed = false;
        }

        // ADDED FOR PS5
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            // Attempt to get cell
            object cell = GetCell(name);

            // Determine what GetCell gave us
            if (cell is Cell)
            {
                return ((Cell)cell).CellValue; // Returns value of cell
            }
            else // We have an empty cell
            {
                return cell;  // Returns ""
            }
        }
        // ADDED FOR PS5
        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown,
        ///       and no change is made to the spreadsheet.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a list consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException();
            }

            // Check cell name is valid and not null 
            CheckCellNameAndNull(name);
            string normalizedName = Normalize(name);  // Normalize now that we have a valid name

            // Need a list that will hold our cell values that we need to recalculate
            IList<string> cellsToRecalculate;

            // Determine what type of content we have
            double contentDbl;
            if (Double.TryParse(content, out contentDbl)) // We have content that is a double
            {
                cellsToRecalculate = SetCellContents(normalizedName, contentDbl); // Will pass our normalized name and the content that is a double into its respective SetCellContents method
            }
            else if (content.StartsWith("=")) // We have content that is a formula
            {
                cellsToRecalculate = SetCellContents(normalizedName, new Formula(content.Substring(1), Normalize, IsValid)); // Will pass our normalized name and the content that could be a Formula into its respective SetCellContents method 
            }
            else // We have content that is a string
            {
                cellsToRecalculate = SetCellContents(normalizedName, content); // Will pass our normalized name and content that is a string into its respective SetCellContents method
            }

            //  Cell values that need to be updated will be here:
            UpdateCellValues(cellsToRecalculate);

            Changed = true; // Our spreadsheet has been changed if SetContentsOfCell is called
            return cellsToRecalculate;
        }



        // Helper Methods below:

        /// <summary>
        /// This helper method will update the values of all the cells given in the order given
        /// </summary>
        /// <param name="cellsToRecalculate">A list of cells to recalculate in order</param>
        private void UpdateCellValues(IList<string> cellsToRecalculate)
        {
            foreach (string cell in cellsToRecalculate)
            {
                if (!cellsWithContent.ContainsKey(cell)) // If our cell from cellsToRecalculate is not in cellsWithContents it means we have a cell with no content (likely just removed) we still want to update other values but skip this one
                {
                    continue;
                }
                Cell currentCell = cellsWithContent[cell];

                Debug.Assert(currentCell.CellContent is double || currentCell.CellContent is string || currentCell.CellContent is Formula); // The CellContent should only be a double, string, or Formula

                if (currentCell.CellContent is Formula) // If the cell content is a Formula
                {
                    currentCell.CellValue = ((Formula)currentCell.CellContent).Evaluate(CellLookup); // Will give a the value of the formula or a FormulaError if evaluation fails
                }
                else // The cell content is a double or string
                {
                    currentCell.CellValue = currentCell.CellContent; // Just make CellValue the CellContent 
                }
            }
        }

        /// <summary>
        /// This helper method determines if a name is a valid cell name as defined by AbstractSpreadsheet: 
        /// A string is a valid cell name if and only if:
        ///   (1) The string starts with one or more letters and is followed by one or more numbers.
        ///   (2) The (application programmer's) IsValid function returns true for that string, and should be called only for variable strings that are valid first by (1) above.
        /// This method determines if the name of the cell is valid.
        /// </summary>
        /// <param name="name">name of cell</param>
        /// <returns>if the name of the cell is valid</returns>
        private bool IsValidCellName(string name)
        {
            if (!Regex.IsMatch(name, @"^[a-zA-Z]+[0-9]+$")) // If we do not have a string that starts with one or more letters and is followed by one or more numbers
            {
                return false;
            }
            if (!IsValid(name)) // If we do not have a valid variable as defined by user
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// This helper method determines if a name is null or if a name is a valid cell name as defined by AbstractSpreadsheet: 
        /// A string is a valid cell name if and only if:
        ///   (1) The string starts with one or more letters and is followed by one or more numbers.
        ///   (2) The (application programmer's) IsValid function returns true for that string, and should be called only for variable strings that are valid first by (1) above.
        ///   
        /// This method will ether throw an InvalidNameException if cell name is either invalid or null otherwise do nothing.
        /// 
        /// </summary>
        /// <param name="name">name of cell</param>
        private void CheckCellNameAndNull(string name)
        {
            if (name == null || !IsValidCellName(name)) // If we have a null or invalid name we throw an InvalidNameException
            {
                throw new InvalidNameException();
            }
        }

        /// <summary>
        /// This helper method will add a cell to cellsWithContent
        /// If cell already has content will replace with new content, 
        /// If cell does not have content then will add new content
        /// </summary>
        /// <param name="name">name of cell</param>
        /// <param name="content">double, string, or Formula content for the cell</param>
        private void AddToCellsWithContent(string name, object content)
        {
            if (cellsWithContent.ContainsKey(name))
            {
                // Update cell value
                cellsWithContent[name].CellContent = content;
            }
            else
            {
                // Create new Cell with new content
                cellsWithContent.Add(name, new Cell(content));
            }
        }

        /// <summary>
        /// Helper method that will attempt to lookup a cell value and either return a Cell object for cells that have content and value 
        /// or return an empty string "" for cells with no content or value      
        /// If name is null or invalid, throws an InvalidNameException.
        /// </summary>
        /// <param name="name">Cell name</param>
        /// <returns>Cell object or string</returns>
        private object GetCell(string name)
        {
            // Check cell name is valid and not null 
            CheckCellNameAndNull(name);
            string normalizedName = Normalize(name);  // Normalize now that we have a valid name

            // Now that we know we have a valid name we will see if the cell has content
            if (cellsWithContent.TryGetValue(normalizedName, out Cell currentCell))
            {
                return currentCell;
            }
            else // Means we have am empty cell if it has a valid name and no content
            {
                return "";
            }
        }

        /// <summary>
        /// Look up function for determining the value of a cell
        /// Will either reutrn a double if the current cell value is a double
        /// Otherwise it will throw an ArgumentException
        /// </summary>
        /// <param name="name">cell name</param>
        /// <returns>value of cell</returns>
        private double CellLookup(string name)
        {
            try
            {
                Cell currentCell = cellsWithContent[name];
                if (currentCell.CellValue is double)
                {
                    return (double)currentCell.CellValue;
                }
                else
                {
                    throw new ArgumentException("Cell value during cell lookup was not a double");
                }
            }
            catch (Exception)
            {
                throw new ArgumentException("Cell lookup failed");
            }
        }
        /// <summary>
        /// Helper method that will determine the string representation of cell content
        /// If the cell contains a string, will return it.  
        /// If the cell contains a double d, d.ToString() will be returned.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended will be returned.
        /// </summary>
        /// <param name="value">Cell to determine string representation of content</param>
        /// <returns>string representation of content</returns>
        private string DetermineCellContentToWrite(Cell value)
        {
            if (value.CellContent is double)
            {
                return ((double)value.CellContent).ToString();
            }
            else if (value.CellContent is Formula)
            {
                return "=" + ((Formula)value.CellContent).ToString();
            }
            else // We must have a string
            {
                return (string)value.CellContent;
            }
        }
        /// <summary>
        /// Helper method that will attempt to read a given XML spreadsheet file
        /// and construct a spreadsheet from it using SetContentsOfCell
        /// </summary>
        /// <param name="filepath">filepath for XML spreadsheet to read</param>
        private void CreateSpreadsheetFromFilepath(string filepath)
        {
            try
            {

                using (XmlReader reader = XmlReader.Create(filepath))
                {
                    string name = "";
                    bool hasName = false;
                    string contents = "";
                    bool hasContents = false;
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "name":
                                    reader.Read();
                                    name = reader.Value;
                                    hasName = true;
                                    break;
                                case "contents":
                                    reader.Read();
                                    contents = reader.Value;
                                    hasContents = true;
                                    break;
                            }
                        }
                        if (hasName && hasContents)
                        {
                            this.SetContentsOfCell(name, contents);
                            hasName = false;
                            hasContents = false;
                        }
                    }
                }
            }
            catch (InvalidNameException)
            {
                throw new SpreadsheetReadWriteException("A name contained in the saved spreadsheet was invalid");
            }
            catch (CircularException)
            {
                throw new SpreadsheetReadWriteException("A circular dependency in the saved spreadsheet was detected");
            }
            catch (FormulaFormatException)
            {
                throw new SpreadsheetReadWriteException("An invalid formula was found in the saved spreadsheet");
            }
            catch (XmlException)
            {
                throw new SpreadsheetReadWriteException("There was an error reading the XML of the given spreadsheet");
            }
        }

        /// <summary>
        /// A Cell holds CellContent and CellValue with get and set access.  
        /// For this spreadsheet a Cell will always have a double, string, or Formula for its content
        /// and a string, double, or a SpreadsheetUtilities.FormulaError for its value. 
        /// </summary>
        private class Cell
        {
            // Auto implemented properties for get and set
            public object CellValue { get; set; }
            public object CellContent { get; set; }

            /// <summary>
            /// Creates a Cell with content
            /// </summary>
            /// <param name="content">double, string, or formula content</param>
            public Cell(object content)
            {
                this.CellContent = content;
            }
        }
    }
}
