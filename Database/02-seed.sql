
USE library_api;

-- AUTHORS
INSERT INTO tauthor (nAuthorID, cName, cSurname)
VALUES
(1, 'J.K.', 'Rowling'),
(2, 'George R.R.', 'Martin'),
(3, 'J.R.R.', 'Tolkien'),
(4, 'Stephen', 'King'),
(5, 'Agatha', 'Christie');

-- PUBLISHERS
INSERT INTO tpublishingcompany
(nPublishingCompanyID, cName)
VALUES
(1, 'Bloomsbury'),
(2, 'Bantam Books'),
(3, 'Allen & Unwin'),
(4, 'Scribner'),
(5, 'Collins Crime Club');

-- BOOKS
INSERT INTO tbook
(nBookID, cTitle, nAuthorID,
 nPublishingYear, nPublishingCompanyID)
VALUES
(1, 'Harry Potter and the Philosopher''s Stone',
 1, 1997, 1),

(2, 'A Game of Thrones',
 2, 1996, 2),

(3, 'The Hobbit',
 3, 1937, 3),

(4, 'The Shining',
 4, 1977, 4),

(5, 'Murder on the Orient Express',
 5, 1934, 5),

(6, 'Harry Potter and the Chamber of Secrets',
 1, 1998, 1),

(7, 'A Clash of Kings',
 2, 1998, 2),

(8, 'The Lord of the Rings',
 3, 1954, 3);
