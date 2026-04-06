using ClientDocumentPortal.Domain.Entities;
using ClientDocumentPortal.Domain.Enums;

namespace ClientDocumentPortal.Application.Interfaces;

public interface IDocumentRequestService
{
    Task<List<DocumentRequest>> GetRequestsByClientAsync(Guid clientId, CancellationToken cancellationToken = default);
    Task<DocumentRequest?> GetRequestByIdAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task<DocumentRequest?> GetRequestBySecureTokenAsync(string token, CancellationToken cancellationToken = default);
    
    Task<DocumentRequest> CreateRequestAsync(Guid clientId, string title, string? description, List<string> documentNames, CancellationToken cancellationToken = default);
    
    Task AddDocumentItemAsync(Guid requestId, string name, string? description, CancellationToken cancellationToken = default);
    Task UpdateDocumentStatusAsync(Guid documentItemId, DocumentStatus status, string? remarks = null, CancellationToken cancellationToken = default);
    Task AttachFileToDocumentItemAsync(Guid documentItemId, string fileKey, string originalFileName, string contentType, long fileSizeBytes, CancellationToken cancellationToken = default);
}
