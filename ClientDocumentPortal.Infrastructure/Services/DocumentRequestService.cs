using ClientDocumentPortal.Application.Interfaces;
using ClientDocumentPortal.Domain.Entities;
using ClientDocumentPortal.Domain.Enums;
using ClientDocumentPortal.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClientDocumentPortal.Infrastructure.Services;

public class DocumentRequestService : IDocumentRequestService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantProvider _tenantProvider;

    public DocumentRequestService(ApplicationDbContext context, ITenantProvider tenantProvider)
    {
        _context = context;
        _tenantProvider = tenantProvider;
    }

    public Task<List<DocumentRequest>> GetRequestsByClientAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        return _context.DocumentRequests
            .Include(r => r.Items)
            .Where(r => r.ClientId == clientId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<DocumentRequest?> GetRequestByIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        return _context.DocumentRequests
            .Include(r => r.Client)
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);
    }

    public Task<DocumentRequest?> GetRequestBySecureTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        // Public access so we skip tenant check globally by reading Unfiltered or assuming secure token is universally unique
        return _context.DocumentRequests
            .IgnoreQueryFilters()
            .Include(r => r.Client)
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.SecureToken == token && !r.IsDeleted, cancellationToken);
    }

    public async Task<DocumentRequest> CreateRequestAsync(Guid clientId, string title, string? description, List<string> documentNames, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantProvider.GetTenantId() ?? throw new InvalidOperationException("Tenant context required");

        var request = new DocumentRequest
        {
            TenantId = tenantId,
            ClientId = clientId,
            Title = title,
            Description = description,
        };

        foreach (var docName in documentNames)
        {
            request.Items.Add(new DocumentItem
            {
                TenantId = tenantId,
                Name = docName,
                Status = DocumentStatus.Pending
            });
        }

        _context.DocumentRequests.Add(request);
        await _context.SaveChangesAsync(cancellationToken);

        return request;
    }

    public async Task AddDocumentItemAsync(Guid requestId, string name, string? description, CancellationToken cancellationToken = default)
    {
        var request = await GetRequestByIdAsync(requestId, cancellationToken);
        if (request == null) throw new Exception("Request not found");

        request.Items.Add(new DocumentItem
        {
            TenantId = request.TenantId,
            Name = name,
            Description = description,
            Status = DocumentStatus.Pending
        });

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateDocumentStatusAsync(Guid documentItemId, DocumentStatus status, string? remarks = null, CancellationToken cancellationToken = default)
    {
        var item = await _context.DocumentItems.FirstOrDefaultAsync(i => i.Id == documentItemId, cancellationToken);
        if (item == null) return;

        item.Status = status;
        if (remarks != null) item.Remarks = remarks;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AttachFileToDocumentItemAsync(Guid documentItemId, string fileKey, string originalFileName, string contentType, long fileSizeBytes, CancellationToken cancellationToken = default)
    {
        // This can be called from public portal without tenant context, use ignore filters or retrieve carefully
        var item = await _context.DocumentItems.IgnoreQueryFilters()
            .FirstOrDefaultAsync(i => i.Id == documentItemId && !i.IsDeleted, cancellationToken);
            
        if (item == null) throw new Exception("Item not found");

        item.FileKey = fileKey;
        item.OriginalFileName = originalFileName;
        item.ContentType = contentType;
        item.FileSizeBytes = fileSizeBytes;
        item.Status = DocumentStatus.Uploaded;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
