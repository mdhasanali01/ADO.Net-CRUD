CREATE TABLE Books
(
  bookId int PRIMARY KEY,
  title nvarchar(100) NOT NULL,
  author nvarchar(100) NOT NULL,
  published_date DATE NOT NULL,
  coverphoto nvarchar(200)
  
)
Go
CREATE TABLE Reviews 
(
  reviewId int PRIMARY KEY,
  reviewer_name nvarchar(255),
  content nvarchar(255),
  rating int,
  BookId INT NOT NULL REFERENCES Books(BookId)
)
Go