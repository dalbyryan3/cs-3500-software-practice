Ryan Dalby 

September 26 2019
PS5-
To implement the changes indicated in PS5 I am planning on modifying my old unit tests and then adding 
tests to cover the new specficiations.  I will then implement the changes specifically:
Variables and Validity: 
	Will change the definition of a valid cell name
	Will pass IsValid and Normalize to any formula's and use as needed.
	All valid cell names need to be replaced with their normalized versions.
Spreadsheet Versioning:
	For PS5 will have versions be "default" unless specified in the constructor
New Constructors:
	Will add zero, three, and four argument constructor as specified.
	The four argument constructor will read a file, must handle possible errors.
SetContentsOfCell:
	Will use SetCellContents:
		Check for null content, if not null determine content type the just pass to SetCellContents.
GetCellValue:
	Return value of cell: Lookup; Will use evaluate on formulas.
Changed:
	Considered changed when a cell is set with content until it is saved.
Save:
	Write XML file, reset changed.  Handle possible errors.
GetSavedVersion:
	Essentialy a static method.  Given a file will return version info.


September 17 2019
PS4-
To implement Spreadsheet, I am planning on first writing tests for the methods to be implemented.
I will then make a Cell class to hold necessary data.  
Then I will implement the AbstractSpreadsheet class to specification and test as I do.
I will then write tests to complete code coverage.

Currently the original handed in versions of PS2 and PS3 are being used.

