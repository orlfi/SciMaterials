using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Result;
using System.Globalization;
using System.Text.Json;
using System.Net.Http.Json;
using SciMaterials.Contracts.API.Constants;

namespace SciMaterials.WebApi.Clients.Files;

public class FilesClient : IFilesClient
{
    private readonly ILogger<FilesClient> _logger;
    private readonly HttpClient _httpClient;

    public FilesClient(HttpClient httpClient, ILogger<FilesClient> logger)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<Result<Guid>> UploadAsync(Stream fileStream, UploadFileRequest uploadFileRequest, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Upload file {file}", uploadFileRequest.Name);

        var matadata = JsonSerializer.Serialize(uploadFileRequest);

        using var multipartFormDataContent = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
        using var streamContent = new StreamContent(fileStream);
        streamContent.Headers.Add("Metadata", matadata);
        multipartFormDataContent.Add(streamContent, "file", uploadFileRequest.Name);

        var response = await _httpClient.PostAsync(WebApiRoute.Files + "/upload", multipartFormDataContent, cancellationToken);
        var result = await response
            .EnsureSuccessStatusCode()
            .Content
            .ReadFromJsonAsync<Result<Guid>>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("No response received from the server.");

        return result;
    }
}
