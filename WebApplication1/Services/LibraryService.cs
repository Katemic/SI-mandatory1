
using CoreWCF;
using Library.Data.Context;
using Library.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.ServiceModel;

public class LibraryService : ILibraryService
{
    private readonly LibraryDbContext _db;

    public LibraryService(LibraryDbContext db)
    {
        _db = db;
    }

    private static void ValidateText(
        string? value, string field, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new FaultException($"{field} is required.");

        if (value.Trim().Length > maxLength)
            throw new FaultException(
                $"{field} cannot exceed {maxLength} characters.");
    }

    private static Acknowledgement Ack(string message)
    {
        return new Acknowledgement
        {
            success = true,
            message = message
        };
    }

    private void ValidateBook(
        string title, int authorId,
        int companyId, int year)
    {
        ValidateText(title, "title", 255);

        if (year < 1900)
            throw new FaultException(
                "Publishing year must be at least 1900.");

        if (!_db.Tauthors.Any(a => a.NAuthorId == authorId))
            throw new FaultException("Author does not exist.");

        if (!_db.Tpublishingcompanies.Any(
            c => c.NPublishingCompanyId == companyId))
            throw new FaultException(
                "Publishing company does not exist.");
    }

    // AUTHORS

    public IdResponse CreateAuthor(CreateAuthorRequest request)
    {
        if (request == null)
            throw new FaultException("Request cannot be null.");

        ValidateText(request.name, "name", 40);
        ValidateText(request.surname, "surname", 60);

        var author = new Tauthor
        {
            CName = request.name.Trim(),
            CSurname = request.surname.Trim()
        };

        _db.Tauthors.Add(author);
        _db.SaveChanges();

        return new IdResponse { id = author.NAuthorId };
    }

    public Author GetAuthorById(GetAuthorByIdRequest request)
    {
        if (request == null)
            throw new FaultException("Request cannot be null.");

        var author = _db.Tauthors.Find(request.id);

        if (author == null)
            throw new FaultException("Author not found.");

        return new Author
        {
            id = author.NAuthorId,
            name = author.CName,
            surname = author.CSurname
        };
    }

    public AuthorList ListAuthors(ListAuthorsRequest request)
    {
        var authors = _db.Tauthors
            .AsNoTracking()
            .OrderBy(a => a.NAuthorId)
            .Select(a => new Author
            {
                id = a.NAuthorId,
                name = a.CName,
                surname = a.CSurname
            })
            .ToArray();

        return new AuthorList { author = authors };
    }

    public Acknowledgement UpdateAuthor(UpdateAuthorRequest request)
    {
        if (request == null)
            throw new FaultException("Request cannot be null.");

        var author = _db.Tauthors.Find(request.id);

        if (author == null)
            throw new FaultException("Author not found.");

        ValidateText(request.name, "name", 40);
        ValidateText(request.surname, "surname", 60);

        author.CName = request.name.Trim();
        author.CSurname = request.surname.Trim();

        _db.SaveChanges();

        return Ack("Author updated.");
    }

    public Acknowledgement DeleteAuthor(DeleteAuthorRequest request)
    {
        if (request == null)
            throw new FaultException("Request cannot be null.");

        var author = _db.Tauthors.Find(request.id);

        if (author == null)
            throw new FaultException("Author not found.");

        if (_db.Tbooks.Any(b => b.NAuthorId == request.id))
            throw new FaultException(
                "Cannot delete an author referenced by a book.");

        _db.Tauthors.Remove(author);
        _db.SaveChanges();

        return Ack("Author deleted.");
    }

    // PUBLISHING COMPANIES

    public IdResponse CreatePublishingCompany(
        CreatePublishingCompanyRequest request)
    {
        if (request == null)
            throw new FaultException("Request cannot be null.");

        ValidateText(request.name, "name", 40);

        var company = new Tpublishingcompany
        {
            CName = request.name.Trim()
        };

        _db.Tpublishingcompanies.Add(company);
        _db.SaveChanges();

        return new IdResponse
        {
            id = company.NPublishingCompanyId
        };
    }

    public PublishingCompany GetPublishingCompanyById(
        GetPublishingCompanyByIdRequest request)
    {
        if (request == null)
            throw new FaultException("Request cannot be null.");

        var company = _db.Tpublishingcompanies.Find(request.id);

        if (company == null)
            throw new FaultException(
                "Publishing company not found.");

        return new PublishingCompany
        {
            id = company.NPublishingCompanyId,
            name = company.CName
        };
    }

    public PublishingCompanyList ListPublishingCompanies(
        ListPublishingCompaniesRequest request)
    {
        var companies = _db.Tpublishingcompanies
            .AsNoTracking()
            .OrderBy(c => c.NPublishingCompanyId)
            .Select(c => new PublishingCompany
            {
                id = c.NPublishingCompanyId,
                name = c.CName
            })
            .ToArray();

        return new PublishingCompanyList
        {
            publishingCompany = companies
        };
    }

    public Acknowledgement UpdatePublishingCompany(
        UpdatePublishingCompanyRequest request)
    {
        if (request == null)
            throw new FaultException("Request cannot be null.");

        var company = _db.Tpublishingcompanies.Find(request.id);

        if (company == null)
            throw new FaultException(
                "Publishing company not found.");

        ValidateText(request.name, "name", 40);

        company.CName = request.name.Trim();

        _db.SaveChanges();

        return Ack("Publishing company updated.");
    }

    public Acknowledgement DeletePublishingCompany(
        DeletePublishingCompanyRequest request)
    {
        if (request == null)
            throw new FaultException("Request cannot be null.");

        var company = _db.Tpublishingcompanies.Find(request.id);

        if (company == null)
            throw new FaultException(
                "Publishing company not found.");

        if (_db.Tbooks.Any(
            b => b.NPublishingCompanyId == request.id))
            throw new FaultException(
                "Cannot delete a publishing company referenced by a book.");

        _db.Tpublishingcompanies.Remove(company);
        _db.SaveChanges();

        return Ack("Publishing company deleted.");
    }

    // BOOKS

    public IdResponse CreateBook(CreateBookRequest request)
    {
        if (request == null)
            throw new FaultException("Request cannot be null.");

        ValidateBook(
            request.title,
            request.authorId,
            request.publishingCompanyId,
            request.publishingYear);

        var book = new Tbook
        {
            CTitle = request.title.Trim(),
            NAuthorId = request.authorId,
            NPublishingCompanyId = request.publishingCompanyId,
            NPublishingYear = request.publishingYear
        };

        _db.Tbooks.Add(book);
        _db.SaveChanges();

        return new IdResponse { id = book.NBookId };
    }

    public Book GetBookById(GetBookByIdRequest request)
    {
        if (request == null)
            throw new FaultException("Request cannot be null.");

        var book = _db.Tbooks.Find(request.id);

        if (book == null)
            throw new FaultException("Book not found.");

        return new Book
        {
            id = book.NBookId,
            title = book.CTitle,
            authorId = book.NAuthorId,
            publishingCompanyId = book.NPublishingCompanyId,
            publishingYear = book.NPublishingYear
        };
    }

    public Acknowledgement UpdateBook(UpdateBookRequest request)
    {
        if (request == null)
            throw new FaultException("Request cannot be null.");

        var book = _db.Tbooks.Find(request.id);

        if (book == null)
            throw new FaultException("Book not found.");

        ValidateBook(
            request.title,
            request.authorId,
            request.publishingCompanyId,
            request.publishingYear);

        book.CTitle = request.title.Trim();
        book.NAuthorId = request.authorId;
        book.NPublishingCompanyId = request.publishingCompanyId;
        book.NPublishingYear = request.publishingYear;

        _db.SaveChanges();

        return Ack("Book updated.");
    }

    public Acknowledgement DeleteBook(DeleteBookRequest request)
    {
        if (request == null)
            throw new FaultException("Request cannot be null.");

        var book = _db.Tbooks.Find(request.id);

        if (book == null)
            throw new FaultException("Book not found.");

        _db.Tbooks.Remove(book);
        _db.SaveChanges();

        return Ack("Book deleted.");
    }
}
