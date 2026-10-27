
using CoreWCF;

[ServiceContract(Namespace = "http://example.com/library")]
[XmlSerializerFormat(
    Style = OperationFormatStyle.Document,
    Use = OperationFormatUse.Literal)]
public interface ILibraryService
{
    [OperationContract(Action = "http://example.com/library/ILibraryService/CreateBook")]
    IdResponse CreateBook(CreateBookRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/GetBookById")]
    Book GetBookById(GetBookByIdRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/UpdateBook")]
    Acknowledgement UpdateBook(UpdateBookRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/DeleteBook")]
    Acknowledgement DeleteBook(DeleteBookRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/CreateAuthor")]
    IdResponse CreateAuthor(CreateAuthorRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/GetAuthorById")]
    Author GetAuthorById(GetAuthorByIdRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/ListAuthors")]
    AuthorList ListAuthors(ListAuthorsRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/UpdateAuthor")]
    Acknowledgement UpdateAuthor(UpdateAuthorRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/DeleteAuthor")]
    Acknowledgement DeleteAuthor(DeleteAuthorRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/CreatePublishingCompany")]
    IdResponse CreatePublishingCompany(CreatePublishingCompanyRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/GetPublishingCompanyById")]
    PublishingCompany GetPublishingCompanyById(GetPublishingCompanyByIdRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/ListPublishingCompanies")]
    PublishingCompanyList ListPublishingCompanies(ListPublishingCompaniesRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/UpdatePublishingCompany")]
    Acknowledgement UpdatePublishingCompany(UpdatePublishingCompanyRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/DeletePublishingCompany")]
    Acknowledgement DeletePublishingCompany(DeletePublishingCompanyRequest request);
}
