-- Library APIs database schema for MySQL 8.0.16+
-- Based on the assignment's entity-relationship diagram.
-- This creates the schema; it does not migrate data from the supplied SQLite file.

CREATE DATABASE IF NOT EXISTS library_api
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_0900_ai_ci;

USE library_api;

CREATE TABLE IF NOT EXISTS tauthor (
    nAuthorID INT NOT NULL AUTO_INCREMENT,
    cName VARCHAR(40) NOT NULL,
    cSurname VARCHAR(60) NOT NULL,
    PRIMARY KEY (nAuthorID)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tpublishingcompany (
    nPublishingCompanyID INT NOT NULL AUTO_INCREMENT,
    cName VARCHAR(40) NOT NULL,
    PRIMARY KEY (nPublishingCompanyID)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tbook (
    nBookID INT NOT NULL AUTO_INCREMENT,
    cTitle VARCHAR(255) NOT NULL,
    nAuthorID INT NOT NULL,
    nPublishingYear INT NOT NULL,
    nPublishingCompanyID INT NOT NULL,
    PRIMARY KEY (nBookID),
    CONSTRAINT chk_book_publishing_year CHECK (nPublishingYear >= 1900),
    CONSTRAINT fk_book_author
        FOREIGN KEY (nAuthorID)
        REFERENCES tauthor (nAuthorID)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,
    CONSTRAINT fk_book_publishing_company
        FOREIGN KEY (nPublishingCompanyID)
        REFERENCES tpublishingcompany (nPublishingCompanyID)
        ON UPDATE CASCADE
        ON DELETE RESTRICT
) ENGINE=InnoDB;
