
using CoreWCF;

[ServiceContract(Namespace = "http://example.com/library")]
[XmlSerializerFormat(
    Style = OperationFormatStyle.Document,
    Use = OperationFormatUse.Literal)]
public interface ILibraryService
{
    // BOOK

    [OperationContract(Action = "http://example.com/library/ILibraryService/CreateBook")]
    [FaultContract(typeof(ValidationFault))]
    IdResponse CreateBook(CreateBookRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/GetBookById")]
    [FaultContract(typeof(NotFoundFault))]
    Book GetBookById(GetBookByIdRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/UpdateBook")]
    [FaultContract(typeof(ValidationFault))]
    [FaultContract(typeof(NotFoundFault))]
    Acknowledgement UpdateBook(UpdateBookRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/DeleteBook")]
    [FaultContract(typeof(NotFoundFault))]
    Acknowledgement DeleteBook(DeleteBookRequest request);

    // AUTHOR

    [OperationContract(Action = "http://example.com/library/ILibraryService/CreateAuthor")]
    [FaultContract(typeof(ValidationFault))]
    IdResponse CreateAuthor(CreateAuthorRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/GetAuthorById")]
    [FaultContract(typeof(NotFoundFault))]
    Author GetAuthorById(GetAuthorByIdRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/ListAuthors")]
    AuthorList ListAuthors(ListAuthorsRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/UpdateAuthor")]
    [FaultContract(typeof(ValidationFault))]
    [FaultContract(typeof(NotFoundFault))]
    Acknowledgement UpdateAuthor(UpdateAuthorRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/DeleteAuthor")]
    [FaultContract(typeof(NotFoundFault))]
    [FaultContract(typeof(ConflictFault))]
    Acknowledgement DeleteAuthor(DeleteAuthorRequest request);

    // PUBLISHING COMPANY

    [OperationContract(Action = "http://example.com/library/ILibraryService/CreatePublishingCompany")]
    [FaultContract(typeof(ValidationFault))]
    IdResponse CreatePublishingCompany(CreatePublishingCompanyRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/GetPublishingCompanyById")]
    [FaultContract(typeof(NotFoundFault))]
    PublishingCompany GetPublishingCompanyById(GetPublishingCompanyByIdRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/ListPublishingCompanies")]
    PublishingCompanyList ListPublishingCompanies(ListPublishingCompaniesRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/UpdatePublishingCompany")]
    [FaultContract(typeof(ValidationFault))]
    [FaultContract(typeof(NotFoundFault))]
    Acknowledgement UpdatePublishingCompany(UpdatePublishingCompanyRequest request);

    [OperationContract(Action = "http://example.com/library/ILibraryService/DeletePublishingCompany")]
    [FaultContract(typeof(NotFoundFault))]
    [FaultContract(typeof(ConflictFault))]
    Acknowledgement DeletePublishingCompany(DeletePublishingCompanyRequest request);
}
