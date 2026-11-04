
using CoreWCF;
using System.Xml.Serialization;

// Error when an entity does not exist
[XmlType(TypeName = "NotFoundFault",
    Namespace = "http://example.com/library")]
public class NotFoundFault
{
    public string message { get; set; } = string.Empty;
}

// Error when input values are invalid
[XmlType(TypeName = "ValidationFault",
    Namespace = "http://example.com/library")]
public class ValidationFault
{
    public string message { get; set; } = string.Empty;
    public string? field { get; set; }
}

// Error when deleting an entity that is still referenced
[XmlType(TypeName = "ConflictFault",
    Namespace = "http://example.com/library")]
public class ConflictFault
{
    public string message { get; set; } = string.Empty;
}

// Helper methods for creating SOAP Faults
public static class SoapFaults
{
    public static FaultException<NotFoundFault> NotFound(
        string message)
    {
        return new FaultException<NotFoundFault>(
            new NotFoundFault { message = message },
            new FaultReason(message));
    }

    public static FaultException<ValidationFault> Validation(
        string message, string? field = null)
    {
        return new FaultException<ValidationFault>(
            new ValidationFault
            {
                message = message,
                field = field
            },
            new FaultReason(message));
    }

    public static FaultException<ConflictFault> Conflict(
        string message)
    {
        return new FaultException<ConflictFault>(
            new ConflictFault { message = message },
            new FaultReason(message));
    }
}
