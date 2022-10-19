using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Result;
using System.Globalization;
using System.Text.Json;
using System.Net.Http.Json;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.Models;

namespace SciMaterials.WebApi.Clients.Files;

public class FilesClient : ApiClientBase<FilesClient, EditFileRequest, Guid, GetFileResponse>, IFilesClient
{
    private readonly ILogger<FilesClient> _logger;
    private readonly HttpClient _httpClient;

    public FilesClient(HttpClient httpClient, ILogger<FilesClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.Files;

    public async Task<Result<GetFileResponse>> GetByHashIdAsync(string hash, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Get file by hash {hash}", hash);

        var result = await _httpClient.GetFromJsonAsync<Result<GetFileResponse>>($"{WebApiRoute.Files}/{hash}", cancellationToken)
            ?? throw new InvalidOperationException("No response received from the server.");
        return result;
    }

    public async Task<Result<Guid>> UploadAsync(Stream fileStream, UploadFileRequest uploadFileRequest, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Upload file {file}", uploadFileRequest.Name);

        var matadata = JsonSerializer.Serialize(uploadFileRequest);

        using var multipartFormDataContent = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.Add("Metadata", matadata);
        multipartFormDataContent.Add(streamContent, "file", uploadFileRequest.Name);

        var response = await _httpClient.PostAsync($"{WebApiRoute.Files}/upload", multipartFormDataContent, cancellationToken);
        var result = await response
            .EnsureSuccessStatusCode()
            .Content
            .ReadFromJsonAsync<Result<Guid>>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("No response received from the server.");

        return result;
    }

    public async Task<Result<FileStreamInfo>> DownloadByHashAsync(string hash, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Download by hash {hash}", hash);

        return await Download($"{WebApiRoute.Files}/DownloadByHash/{hash}", cancellationToken);
    }

    public async Task<Result<FileStreamInfo>> DownloadByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Download by Id {id}", id);

        return await Download($"{WebApiRoute.Files}/DownloadById/{id}", cancellationToken);
    }

    private async Task<Result<FileStreamInfo>> Download(string path, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(path, cancellationToken);
        response.EnsureSuccessStatusCode();

        if (response.Content.Headers.ContentType is not null
            && response.Content.Headers.ContentDisposition is not null
            && response.Content.Headers.ContentDisposition.DispositionType.Equals("attachment"))
        {
            var stream = await response.Content.ReadAsStreamAsync()
                ?? throw new InvalidOperationException("No response received from the server.");

            return new FileStreamInfo(
                response.Content.Headers.ContentDisposition.FileName,
                response.Content.Headers.ContentType.MediaType,
                stream);
        }

        var result = await response.Content.ReadFromJsonAsync<Result<FileStreamInfo>>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("No response received from the server.");
        return result;
    }
}
