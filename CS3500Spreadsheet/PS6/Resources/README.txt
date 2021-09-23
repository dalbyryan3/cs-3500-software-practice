Ryan Dalby 

This PS6 Solution utilizes the as submitted versions of DependencyGraph(PS2) and Formula(PS3)
and uses an updated version compared to the submitted PS5 version of Spreadsheet.

There is code from PS6Skeleton for managing multiple windows, displaying selection, 
and for having a file menu that can make new spreadsheets or close the current spreadsheet.

The extra features implemented are as follows:
	Multiple ways to select and enter contents into cells fluidly and quickly:
		Navigation with arrow keys allows for selection of cells and entering of contents once the cell is left.
		Navigation with tab allows for right traversal and entering of contents once the cell is left.
		Enter has the same functionality as the Enter Contents button.
	Shortcuts:
		 Ctrl-N: Opens a new spreadsheet 
		 Ctrl-S: Saves file
		 Ctrl-O: Opens a file
		 ESC: Closes a spreadsheet,
         Ctrl-H: Opens help menu
	Prompting for possible data loss on general close event not just Close from within menu
