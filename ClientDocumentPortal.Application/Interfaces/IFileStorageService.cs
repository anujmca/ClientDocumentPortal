namespace ClientDocumentPortal.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<Stream> DownloadFileAsync(string fileKey, CancellationToken cancellationToken = default);
    string GetPreSignedUrl(string fileKey, TimeSpan expiry);
}
