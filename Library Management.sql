CREATE TABLE `LIBRARIAN` (
  `ID` int PRIMARY KEY,
  `FullName` varchar(255),
  `DateOfBirth` datetime,
  `Address` varchar(255),
  `Email` varchar(255),
  `DateCreated` datetime,
  `Username` varchar(255),
  `Password` varchar(255)
);

CREATE TABLE `READER` (
  `ID` int PRIMARY KEY,
  `FullName` varchar(255),
  `DateOfBirth` datetime,
  `Address` varchar(255),
  `Email` varchar(255),
  `DateCreated` datetime,
  `Code` int
);

CREATE TABLE `BOOK` (
  `ID` int PRIMARY KEY,
  `Title` varchar(255),
  `Author` varchar(255),
  `Amount` int,
  `CategoryID` int
);

CREATE TABLE `BOOK_ITEM` (
  `BookID` int,
  `Barcode` int,
  PRIMARY KEY (`BookID`, `Barcode`)
);

CREATE TABLE `CATEGORY` (
  `ID` int PRIMARY KEY,
  `Name` varchar(255),
  `Describe` varchar(255)
);

CREATE TABLE `BOOK_CHECKOUT` (
  `ID` int,
  `BookItemCode` int,
  `UserID` int,
  `type` enum,
  `CheckoutDate` datetime,
  `ReturnDate` datetime,
  PRIMARY KEY (`UserID`, `ID`)
);

CREATE TABLE `CATEGORY_EDIT` (
  `CategoryID` int,
  `LibrarianID` int,
  `DateEdit` datetime
);

ALTER TABLE `CATEGORY` ADD FOREIGN KEY (`ID`) REFERENCES `BOOK` (`CategoryID`);

ALTER TABLE `BOOK` ADD FOREIGN KEY (`ID`) REFERENCES `BOOK_ITEM` (`BookID`);

ALTER TABLE `BOOK_ITEM` ADD FOREIGN KEY (`Barcode`) REFERENCES `BOOK_CHECKOUT` (`BookItemCode`);

ALTER TABLE `READER` ADD FOREIGN KEY (`ID`) REFERENCES `BOOK_CHECKOUT` (`UserID`);

ALTER TABLE `CATEGORY` ADD FOREIGN KEY (`ID`) REFERENCES `CATEGORY_EDIT` (`CategoryID`);

ALTER TABLE `LIBRARIAN` ADD FOREIGN KEY (`ID`) REFERENCES `CATEGORY_EDIT` (`LibrarianID`);

