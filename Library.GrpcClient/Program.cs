using Grpc.Core;
using Grpc.Net.Client;
using Library.gRPCApi;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

using var channel = GrpcChannel.ForAddress("http://localhost:5030");
var client = new LibraryService.LibraryServiceClient(channel);

var mode = args.FirstOrDefault()?.ToLowerInvariant();

switch (mode)
{
    case "watch":
        await WatchBooksAsync();
        break;
    case "create":
        await CreateBookAsync();
        break;
    case "get":
        await GetBookAsync();
        break;
    default:
        Console.WriteLine("Choose a mode: watch, create, or get.");
        break;
}

async Task GetBookAsync()
{
    Console.Write("Enter a book ID to retrieve: ");
    if (!int.TryParse(Console.ReadLine(), out var bookId) || bookId <= 0)
    {
        Console.WriteLine("Book ID must be a positive number.");
        return;
    }

    try
    {
        var book = await client.GetBookByIdAsync(new GetBookByIdRequest
        {
            BookId = bookId
        });

        Console.WriteLine($"Book: {book.Id} - {book.Name}");
    }
    catch (RpcException exception)
    {
        Console.WriteLine($"Request failed: {exception.Status.Detail}");
    }
}

async Task CreateBookAsync()
{
    Console.Write("Enter the new book name: ");
    var bookName = Console.ReadLine();

    Console.Write("Enter the author ID: ");
    var authorIdInput = Console.ReadLine();

    Console.Write("Enter the publisher ID: ");
    var publisherIdInput = Console.ReadLine();

    Console.Write("Enter the publication year: ");
    var publicationYearInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(bookName)
        || !int.TryParse(authorIdInput, out var authorId)
        || authorId <= 0
        || !int.TryParse(publisherIdInput, out var publisherId)
        || publisherId <= 0
        || !int.TryParse(publicationYearInput, out var publicationYear)
        || publicationYear < 1900)
    {
        Console.WriteLine("Invalid book details.");
        return;
    }

    try
    {
        var response = await client.CreateBookAsync(new CreateBookRequest
        {
            Name = bookName.Trim(),
            AuthorId = authorId,
            PublisherId = publisherId,
            PublicationYear = publicationYear
        });

        Console.WriteLine(
            $"Created book: {response.Book.Id} - {response.Book.Name}");
    }
    catch (RpcException exception)
    {
        Console.WriteLine($"Request failed: {exception.Status.Detail}");
    }
}

async Task WatchBooksAsync()
{
    Console.WriteLine("Watching for new books. Press Ctrl+C to stop.");

    using var watchCall = client.WatchBooks(new WatchBooksRequest());

    await foreach (var book in watchCall.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine($"Streamed book: {book.Id} - {book.Name}");
    }
}
