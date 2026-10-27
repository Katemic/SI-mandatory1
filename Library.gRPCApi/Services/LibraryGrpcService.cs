using System.Threading.Channels;
using Grpc.Core;
using Library.Data.Models;
using Library.Data.Repositories;

namespace Library.gRPCApi.Services;

public sealed class LibraryGrpcService : LibraryService.LibraryServiceBase
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookNotificationService _notifications;

    public LibraryGrpcService(
        IBookRepository bookRepository,
        IBookNotificationService notifications)
    {
        _bookRepository = bookRepository;
        _notifications = notifications;
    }

    public override async Task<Book> GetBookById(
        GetBookByIdRequest request,
        ServerCallContext context)
    {
        if (request.BookId <= 0)
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Book ID must be positive."));
        }

        var book = await _bookRepository.GetByIdAsync(
            request.BookId,
            context.CancellationToken);

        if (book is null)
        {
            throw new RpcException(
                new Status(StatusCode.NotFound, "Book was not found."));
        }

        return ToGrpcBook(book);
    }

    public override async Task<CreateBookResponse> CreateBook(
        CreateBookRequest request,
        ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Book name is required."));
        }

        if (request.AuthorId <= 0 || request.PublisherId <= 0)
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    "Author ID and publisher ID must be positive."));
        }

        if (request.PublicationYear < 1900)
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    "Publication year must be at least 1900."));
        }

        var cancellationToken = context.CancellationToken;

        if (!await _bookRepository.AuthorExistsAsync(
                request.AuthorId,
                cancellationToken))
        {
            throw new RpcException(
                new Status(StatusCode.NotFound, "Author was not found."));
        }

        if (!await _bookRepository.PublishingCompanyExistsAsync(
                request.PublisherId,
                cancellationToken))
        {
            throw new RpcException(
                new Status(StatusCode.NotFound, "Publisher was not found."));
        }

        var book = await _bookRepository.CreateAsync(
            new Tbook
            {
                CTitle = request.Name.Trim(),
                NAuthorId = request.AuthorId,
                NPublishingCompanyId = request.PublisherId,
                NPublishingYear = request.PublicationYear
            },
            cancellationToken);

        await _notifications.PublishAsync(book, cancellationToken);

        return new CreateBookResponse
        {
            Book = ToGrpcBook(book)
        };
    }

    public override async Task WatchBooks(
        WatchBooksRequest request,
        IServerStreamWriter<Book> responseStream,
        ServerCallContext context)
    {
        var channel = Channel.CreateUnbounded<Tbook>();
        using var subscription = _notifications.Subscribe(channel.Writer);

        await foreach (var book in channel.Reader.ReadAllAsync(
            context.CancellationToken))
        {
            await responseStream.WriteAsync(ToGrpcBook(book));
        }
    }

    private static Book ToGrpcBook(Tbook book)
    {
        return new Book
        {
            Id = book.NBookId,
            Name = book.CTitle,
            AuthorId = book.NAuthorId,
            PublisherId = book.NPublishingCompanyId,
            PublicationYear = book.NPublishingYear
        };
    }
}
