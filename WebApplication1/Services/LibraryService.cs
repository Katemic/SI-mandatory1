
using CoreWCF;
using System.Linq;
using System.ServiceModel;

public class LibraryService : ILibraryService
{
    private static readonly Dictionary<int, Book> Books = new();
    private static readonly Dictionary<int, Author> Authors = new();
    private static readonly Dictionary<int, PublishingCompany> Companies = new();

    private static int nextBookId = 1;
    private static int nextAuthorId = 1;
    private static int nextCompanyId = 1;

    private static readonly object Gate = new();

    private static void ValidateText(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new FaultException($"{field} is required.");
    }

    private static void ValidateBook(CreateBookRequest request)
    {
        ValidateText(request.title, "title");

        if (request.publishingYear < 1900)
            throw new FaultException(
                "Publishing year must be at least 1900.");

        if (!Authors.ContainsKey(request.authorId))
            throw new FaultException("Author does not exist.");

        if (!Companies.ContainsKey(request.publishingCompanyId))
            throw new FaultException(
                "Publishing company does not exist.");
    }

    private static Acknowledgement Ack(string message) =>
        new() { success = true, message = message };

    public IdResponse CreateAuthor(CreateAuthorRequest request)
    {
        lock (Gate)
        {
            if (request == null)
                throw new FaultException("Request cannot be null.");

            ValidateText(request.name, "name");
            ValidateText(request.surname, "surname");

            int id = nextAuthorId++;

            Authors[id] = new Author
            {
                id = id,
                name = request.name.Trim(),
                surname = request.surname.Trim()
            };

            return new IdResponse { id = id };
        }
    }

    public IdResponse CreatePublishingCompany(
        CreatePublishingCompanyRequest request)
    {
        lock (Gate)
        {
            ValidateText(request.name, "name");

            int id = nextCompanyId++;
            Companies[id] = new PublishingCompany
            {
                id = id,
                name = request.name.Trim()
            };

            return new IdResponse { id = id };
        }
    }

    public IdResponse CreateBook(CreateBookRequest request)
    {
        lock (Gate)
        {
            ValidateBook(request);

            int id = nextBookId++;
            Books[id] = new Book
            {
                id = id,
                title = request.title.Trim(),
                authorId = request.authorId,
                publishingCompanyId = request.publishingCompanyId,
                publishingYear = request.publishingYear
            };

            return new IdResponse { id = id };
        }
    }

    public Book GetBookById(GetBookByIdRequest request)
    {
        lock (Gate)
        {
            if (!Books.TryGetValue(request.id, out var book))
                throw new FaultException("Book not found.");

            return book;
        }
    }

    public Author GetAuthorById(GetAuthorByIdRequest request)
    {
        lock (Gate)
        {
            if (!Authors.TryGetValue(request.id, out var author))
                throw new FaultException("Author not found.");

            return author;
        }
    }

    public AuthorList ListAuthors(ListAuthorsRequest request)
    {
        lock (Gate)
        {
            return new AuthorList
            {
                author = Authors.Values.ToArray()
            };
        }
    }

    public PublishingCompany GetPublishingCompanyById(
        GetPublishingCompanyByIdRequest request)
    {
        lock (Gate)
        {
            if (!Companies.TryGetValue(request.id, out var company))
                throw new FaultException(
                    "Publishing company not found.");

            return company;
        }
    }

    public PublishingCompanyList ListPublishingCompanies(
        ListPublishingCompaniesRequest request)
    {
        lock (Gate)
        {
            return new PublishingCompanyList
            {
                publishingCompany = Companies.Values.ToArray()
            };
        }
    }

    public Acknowledgement UpdateBook(UpdateBookRequest request)
    {
        lock (Gate)
        {
            if (!Books.ContainsKey(request.id))
                throw new FaultException("Book not found.");

            ValidateBook(new CreateBookRequest
            {
                title = request.title,
                authorId = request.authorId,
                publishingCompanyId = request.publishingCompanyId,
                publishingYear = request.publishingYear
            });

            Books[request.id] = new Book
            {
                id = request.id,
                title = request.title.Trim(),
                authorId = request.authorId,
                publishingCompanyId = request.publishingCompanyId,
                publishingYear = request.publishingYear
            };

            return Ack("Book updated.");
        }
    }

    public Acknowledgement UpdateAuthor(UpdateAuthorRequest request)
    {
        lock (Gate)
        {
            if (!Authors.ContainsKey(request.id))
                throw new FaultException("Author not found.");

            ValidateText(request.name, "name");
            ValidateText(request.surname, "surname");

            Authors[request.id] = new Author
            {
                id = request.id,
                name = request.name.Trim(),
                surname = request.surname.Trim()
            };

            return Ack("Author updated.");
        }
    }

    public Acknowledgement UpdatePublishingCompany(
        UpdatePublishingCompanyRequest request)
    {
        lock (Gate)
        {
            if (!Companies.ContainsKey(request.id))
                throw new FaultException(
                    "Publishing company not found.");

            ValidateText(request.name, "name");

            Companies[request.id] = new PublishingCompany
            {
                id = request.id,
                name = request.name.Trim()
            };

            return Ack("Publishing company updated.");
        }
    }

    public Acknowledgement DeleteBook(DeleteBookRequest request)
    {
        lock (Gate)
        {
            if (!Books.Remove(request.id))
                throw new FaultException("Book not found.");

            return Ack("Book deleted.");
        }
    }

    public Acknowledgement DeleteAuthor(DeleteAuthorRequest request)
    {
        lock (Gate)
        {
            if (!Authors.ContainsKey(request.id))
                throw new FaultException("Author not found.");

            if (Books.Values.Any(b => b.authorId == request.id))
                throw new FaultException(
                    "Cannot delete an author referenced by a book.");

            Authors.Remove(request.id);
            return Ack("Author deleted.");
        }
    }

    public Acknowledgement DeletePublishingCompany(
        DeletePublishingCompanyRequest request)
    {
        lock (Gate)
        {
            if (!Companies.ContainsKey(request.id))
                throw new FaultException(
                    "Publishing company not found.");

            if (Books.Values.Any(
                b => b.publishingCompanyId == request.id))
                throw new FaultException(
                    "Cannot delete a publishing company referenced by a book.");

            Companies.Remove(request.id);
            return Ack("Publishing company deleted.");
        }
    }
}
