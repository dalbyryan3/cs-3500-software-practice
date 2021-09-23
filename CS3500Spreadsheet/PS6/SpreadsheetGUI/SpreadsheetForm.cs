// Implementation of a spreadsheet application by Ryan Dalby

using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SS
{
    /// <summary>
    /// Part of SpreadsheetForm GUI for Spreadsheet application
    /// </summary>
    public partial class SpreadsheetForm : Form
    {
        private Spreadsheet spreadsheet; // The spreadsheet associated with the Form
        private Func<string, bool> isValid = s => Regex.IsMatch(s, "^[A-Z][1-9][0-9]?$"); // Restricts valid cell names to A1-Z99
        private Func<string, string> normalize = s => s.ToUpper(); // Sends all variables to uppercase

        public SpreadsheetForm()
        {
            InitializeComponent();

            // Create a spreadsheet associated with the Form with: 
            // isValid restricting valid cell names to A1-Z99
            // normalize sending all variable names to uppercase, 
            // and a version of ps6
            spreadsheet = new Spreadsheet(isValid, normalize, "ps6");

            // Register handlers
            FormSpreadsheetPanel.SelectionChanged += DisplaySelection;

            // Map accept button (ENTER) to the enter content button
            AcceptButton = EnterContentButton;

            // Map cancel button (ESC) to Close 

        }



        /// Event handler methods below:

        /// <summary>
        /// Every time the selection changes, this method is called with the
        /// Spreadsheet as its parameter. 
        /// </summary>
        private void DisplaySelection(SpreadsheetPanel ss)
        {
            UpdateTextBoxesAndFocus(ss);
        }
        /// <summary>
        /// Handles pressing New in file menu
        /// </summary>
        private void NewToolStripMenuItem_click(object sender, EventArgs e)
        {
            // Tell the application context to run the form on the same
            // thread as the other forms.
            RunNewForm();
        }

        /// <summary>
        /// Handles pressing Save in file menu
        /// </summary>
        private void SaveToolStripMenuItem_click(object sender, EventArgs e)
        {
            SaveDialog();
        }
        /// <summary>
        /// Handles pressing Open in file menu
        /// </summary>
        private void OpenToolStripMenuItem_click(object sender, EventArgs e)
        {
            OpenDialog();
        }
        /// <summary>
        /// Handles pressing Close in file menu
        /// </summary>
        private void CloseToolStripMenuItem_click(object sender, EventArgs e)
        {
            Close(); // Will trigger form closing event which may be cancelled by user, handled by HandleClose
        }
        /// <summary>
        /// Handles pressing enter button for entering content into a cell
        /// </summary>
        private void EnterContentButton_click(object sender, EventArgs e)
        {
            AddCellContentToCurrentCell();
        }
        /// <summary>
        /// Handles pressing help 
        /// </summary>
        private void HelpToolStripMenuItem_click(object sender, EventArgs e)
        {
            HelpDialog();
        }
        /// <summary>
        /// Handles pressing save after selecting file save location
        /// </summary>
        private void SaveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            SaveSpreadsheet();
        }
        /// <summary>
        /// Handles pressing open after selecting file
        /// </summary>
        private void OpenFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            OpenSpreadsheet();
        }
        /// <summary>
        /// Handles a SpreadsheetForm closing not from Close in file menu
        /// </summary>
        private void SpreadsheetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            HandleClose(e);
        }
        /// <summary>
        /// Handles when the form is first shown
        /// </summary>
        private void SpreadsheetForm_Shown(object sender, EventArgs e)
        {
            HandleFirstShown();
        }
        /// <summary>
        /// Override ProcessCmdKey to handle up(enter content, move up), down(enter content, move down), 
        /// left(enter content, move left), right(enter content, move right), TAB(enter content, move right),
        /// Ctrl-N: Opens a new spreadsheet, Ctrl-S: Saves file, Ctrl-O: Opens a file,  ESC: Closes a spreadsheet,
        /// Ctrl-H: Opens help menu
        /// presses to navigate spreadsheet panel
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            if (keyData == Keys.Up) // Up arrow key
            {
                return MoveUp();
            }
            else if (keyData == Keys.Down) // Down arrow key
            {
                return MoveDown();
            }
            else if (keyData == Keys.Left) // Left arrow key
            {
                return MoveLeft();
            }
            else if (keyData == Keys.Right) // Right arrow key
            {
                return MoveRight();
            }
            else if (keyData == Keys.Tab) // TAB key
            {
                return MoveRight();
            }
            else if (keyData == Keys.Escape) // ESC
            {
                Close();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.S)) // CTRL-S
            {
                SaveDialog();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.O)) // CTRL-O
            {
                OpenDialog();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.N)) // CTRL-N
            {
                RunNewForm();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.H)) // CTRL-H
            {
                HelpDialog();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }








        /// Helper methods and program logic below:

        /// <summary>
        /// Helper method that converts a zero based column between 0 and 25 corresponding to A-Z
        /// and a zero based row between 0 and 98 corresponding to 1-99
        /// to a cell name between A1 and Z99.  
        /// </summary>
        /// <param name="col">Column of spreadsheet</param>
        /// <param name="row">Row of spreadsheet</param>
        /// <returns>name of cell</returns>
        private string ColRowToCellName(int col, int row)
        {
            string colName = ((char)(col + 65)).ToString(); // Maps zero based col to ASCII A-Z, thus col 0 corresponds to ASCII 65 which is 'A'
            int rowName = row + 1; // Maps zero based row to 1 based row
            return (colName + rowName);
        }
        /// <summary>
        /// Helper method that converts a valid A1-Z99 cell name to 
        /// a zero based column between 0 and 25 corresponding to A-Z
        /// and a zero based row between 0 and 98 corresponding to 1-99
        /// </summary>
        /// <param name="cellName">name of cell</param>
        /// <returns>tuple containing column and then row</returns>
        private Tuple<int, int> CellNameToColRow(string cellName)
        {
            char colChar = cellName[0]; // Extract first part of cellName which is letter
            string rowString = cellName.Substring(1); // Extract last part of cellName 
            int col = (colChar - 65); // Maps ASCII A-Z to zero based col, thus ASCII 65 which is 'A' corresponds to 0
            int row = int.Parse(rowString) - 1; // Maps 1 based row to 0 based row

            return new Tuple<int, int>(col, row);
        }

        /// <summary>
        /// Helper method that will give a string representing of the content of a cell
        /// given the corresponding cell name
        /// </summary>
        /// <param name="cellName">name of cell</param>
        /// <returns>string representation of cell content</returns>
        private string ContentToDisplay(string cellName)
        {
            object content = spreadsheet.GetCellContents(cellName);
            if (content is double)
            {
                return ((double)content).ToString();
            }
            else if (content is Formula)
            {
                return "=" + ((Formula)content).ToString();
            }
            else // We must have a string
            {
                return (string)content;
            }
        }

        /// <summary>
        /// Helper method that given an enumerable collection of cell names to update will update the corresponding values on a spreadsheet panel
        /// </summary>
        /// <param name="cellsToUpdate">enumerable collection of cells by name to update</param>
        private void UpdateSpreadsheetPanelValues(IEnumerable<string> cellsToUpdate)
        {
            // Update dependent cells value to display
            foreach (string cellNameToUpdate in cellsToUpdate)
            {
                object cellValueToUpdate = spreadsheet.GetCellValue(cellNameToUpdate);
                Tuple<int, int> colRowToUpdate = CellNameToColRow(cellNameToUpdate); // Convert the given cell name to a column and row location
                string strCellValueToUpdate;
                if (cellValueToUpdate is FormulaError) // With a FormulaError we will get the Reason and that will be the cell value
                {
                    strCellValueToUpdate = ((FormulaError)cellValueToUpdate).Reason;
                }
                else if (cellValueToUpdate is double) // With a double we convert to a string
                {
                    strCellValueToUpdate = cellValueToUpdate.ToString();
                }
                else // Otherwise we have a string so we cast to a string
                {
                    strCellValueToUpdate = (string)cellValueToUpdate;
                }


                FormSpreadsheetPanel.SetValue(colRowToUpdate.Item1, colRowToUpdate.Item2, strCellValueToUpdate); // Update displayed value
            }
        }
        /// <summary>
        /// Will determine if application should override/close a changed spreadsheet,
        /// prompts user to confirm override/close if spreadsheet has been changed
        /// Returns true if override/close should occur, false otherwise
        /// </summary>
        /// <returns>true if override/close should occur</returns>
        private bool ShouldOverride()
        {
            if (spreadsheet.Changed)
            {
                if (MessageBox.Show("Are you sure you would like to leave this spreadsheet without saving?", "Possible Loss of Data", MessageBoxButtons.OKCancel) == DialogResult.OK) // Close if User OKs it, otherwise nothing happens
                {
                    return true;
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Will attempt to open a spreadsheet
        /// </summary>
        private void OpenSpreadsheet()
        {
            try
            {
                // Make sure we are not overriding data and if so prompt user
                if(!ShouldOverride()) // If we should not override exit method
                {
                    return;
                }

                // Save non empty cells from possible previous spreadsheet to be updated after creating new spreadsheet
                IEnumerable<string> previousNonEmptyCells = spreadsheet.GetNamesOfAllNonemptyCells();
                
                // Try to create spreadsheet from file
                spreadsheet = new Spreadsheet(openFileDialog.FileName, isValid, normalize, "ps6");
                
                // Update spreadsheet pannel values
                UpdateSpreadsheetPanelValues(previousNonEmptyCells);
                UpdateSpreadsheetPanelValues(spreadsheet.GetNamesOfAllNonemptyCells());
            }
            catch (SpreadsheetReadWriteException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary>
        /// Will attempt to save the spreadsheet
        /// </summary>
        private void SaveSpreadsheet()
        {
            try
            {
                spreadsheet.Save(saveFileDialog.FileName);
            }
            catch (SpreadsheetReadWriteException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary>
        /// Will take the content in the CellContentTextBox and attempt to
        /// add it to the currently selected cell.
        /// </summary>
        private void AddCellContentToCurrentCell()
        {
            // Get the currently selected cell location and contents
            FormSpreadsheetPanel.GetSelection(out int col, out int row);
            string cellName = ColRowToCellName(col, row);
            string cellContent = CellContentTextBox.Text;

            // Set the corresponding cell of our spreadsheet to the content
            try
            {
                // Try to add content to cell
                IList<string> cellsToUpdate = spreadsheet.SetContentsOfCell(cellName, cellContent);
                // Update spreadsheet panel values
                UpdateSpreadsheetPanelValues(cellsToUpdate);
            }
            catch (FormulaFormatException)
            {
                MessageBox.Show("The formula entered was formatted incorrectly.", "Error");
            }
            catch (CircularException)
            {
                MessageBox.Show("The formula entered created a circular dependency.", "Error");
            }
        }
        /// <summary>
        /// This will update the displayed cell name text box, cell value text box, and cell content text box.
        /// 
        /// </summary>
        private void UpdateTextBoxes(SpreadsheetPanel ss)
        {
            ss.GetSelection(out int col, out int row);
            ss.GetValue(col, row, out string value);
            string cellName = ColRowToCellName(col, row);
            CellNameUneditableTextBox.Text = cellName; // Sets text box that displays cell name to the selected cell name
            CellValueUneditableTextBox.Text = value; // Sets text box that displays cell value to cell value of the selected cell
            CellContentTextBox.Text = ContentToDisplay(cellName); // Sets text box that displays cell content to cell content string representatin of the selected cell
        }

        /// <summary>
        /// Updates the text of given SpreadsheetPanel and focuses on enter content text box
        /// </summary>
        /// <param name="ss">A SpreadsheetPanel</param>
        private void UpdateTextBoxesAndFocus(SpreadsheetPanel ss)
        {
            UpdateTextBoxes(ss);
            CellContentTextBox.Focus(); // Focus on cell content text box
        }
        /// <summary>
        /// Will display spreadsheet help dialog
        /// </summary>
        private static void HelpDialog()
        {
            MessageBox.Show("Spreadsheet Application Help\n\n" +
                "This is a spreadsheet application created by Ryan Dalby for CS 3500\n\n" +
                "Controls:\n" +
                "Arrow keys, TAB, and mouse clicks allow for selection of highlighted cell. \n" +
                "Arrow keys, TAB, Enter, and the Enter button allow for entering the text in the cell content text box into the cell\n\n" +
                "Shortcuts:\n" +
                "Ctrl-N: Opens a new spreadsheet\n" +
                "Ctrl-S: Saves file\n" +
                "Ctrl-O: Opens a file\n" +
                "ESC: Closes a spreadsheet\n" +
                "Ctrl-H: Opens help menu\n");
        }
        /// <summary>
        /// Will handle the form attempting to close and prompt user if unsaved data exists
        /// </summary>
        /// <param name="e"></param>
        private void HandleClose(FormClosingEventArgs e)
        {
            if (!ShouldOverride())
            {
                e.Cancel = true; // Cancel closing event
            }
        }
        /// <summary>
        /// Will add the content in cell content text box to cell then
        /// move selection on the form spreadsheet panel up preventing it from going beyond the edge.
        /// If movement would cause selection to go beyond edge nothing will happen.
        /// Returns true if key press was handled.
        /// </summary>
        /// <returns>true if key press was handled</returns>
        private bool MoveUp()
        {
            FormSpreadsheetPanel.GetSelection(out int col, out int row);
            if (row > 0)
            {
                AddCellContentToCurrentCell();
                FormSpreadsheetPanel.SetSelection(col, (row - 1));
                UpdateTextBoxesAndFocus(FormSpreadsheetPanel);
            }
            return true;
        }

        /// <summary>
        /// Will add the content in cell content text box to cell then
        /// move selection on the form spreadsheet panel down preventing it from going beyond the edge.
        /// If movement would cause selection to go beyond edge nothing will happen.
        /// Returns true if key press was handled.
        /// </summary>
        /// <returns>true if key press was handled</returns>
        private bool MoveDown()
        {
            FormSpreadsheetPanel.GetSelection(out int col, out int row);
            if (row < 98)
            {
                AddCellContentToCurrentCell();
                FormSpreadsheetPanel.SetSelection(col, (row + 1));
                UpdateTextBoxesAndFocus(FormSpreadsheetPanel);
            }
            return true;
        }

        /// <summary>
        /// Will add the content in cell content text box to cell then
        /// move selection on the form spreadsheet panel to the left preventing it from going beyond the edge.
        /// If movement would cause selection to go beyond edge nothing will happen.
        /// Returns true if key press was handled.
        /// </summary>
        /// <returns>true if key press was handled</returns>
        private bool MoveLeft()
        {
            FormSpreadsheetPanel.GetSelection(out int col, out int row);
            if (col > 0)
            {
                AddCellContentToCurrentCell();
                FormSpreadsheetPanel.SetSelection((col - 1), row);
                UpdateTextBoxesAndFocus(FormSpreadsheetPanel);

            }
            return true;
        }

        /// <summary>
        /// Will add the content in cell content text box to cell then
        /// move selection on the form spreadsheet panel to the right preventing it from going beyond the edge.
        /// If movement would cause selection to go beyond edge nothing will happen.
        /// Returns true if key press was handled.
        /// </summary>
        /// <returns>true if key press was handled</returns>
        private bool MoveRight()
        {
            FormSpreadsheetPanel.GetSelection(out int col, out int row);
            if (col < 25)
            {
                AddCellContentToCurrentCell();
                FormSpreadsheetPanel.SetSelection((col + 1), row);
                UpdateTextBoxesAndFocus(FormSpreadsheetPanel);
            }
            return true;
        }
        /// <summary>
        /// Will set selection box and focus to cell content text box 
        /// when form is first shown
        /// </summary>
        private void HandleFirstShown()
        {
            // Set selection box on start up to A1 or 0,0
            FormSpreadsheetPanel.SetSelection(0, 0);
            // Set focus
            CellContentTextBox.Focus();
        }

        /// <summary>
        /// Tells the application context to run the form on the same
        /// thread as the other forms.
        /// </summary>
        private static void RunNewForm()
        {
            DemoApplicationContext.getAppContext().RunForm(new SpreadsheetForm());
        }
        /// <summary>
        /// Calls save file dialog
        /// </summary>
        private void SaveDialog()
        {
            saveFileDialog.ShowDialog();
        }
        /// <summary>
        /// Calls open file dialog
        /// </summary>
        private void OpenDialog()
        {
            openFileDialog.ShowDialog();
        }


    }
}
