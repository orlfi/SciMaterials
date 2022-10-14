using System.Globalization;
using System.Net.Http.Json;
using SciMaterials.Contracts.Result;

namespace SciMaterials.ConsoleTests;

public class FilesClient
{
    public HttpClient Http { get; }

    public FilesClient(HttpClient Http) => this.Http = Http;

    public async Task<Result<Guid>> UploadAsync(string FilePath, string metadata, CancellationToken Cancel = default)
    {
        var fileInfo = new FileInfo(FilePath);

        using var multipartFormDataContent = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));

        await using var fileStream = File.OpenRead(FilePath);
        var streamContent = new StreamContent(fileStream);

        streamContent.Headers.Add("Metadata", metadata);
        multipartFormDataContent.Add(streamContent, "file", fileInfo.Name);

        var response = await Http.PostAsync("api/files/upload", multipartFormDataContent, Cancel);
        var result = await response
               .EnsureSuccessStatusCode()
               .Content
               .ReadFromJsonAsync<Result<Guid>>(cancellationToken: Cancel)
                ?? throw new InvalidOperationException("Не получен ответ от сервера");

        return result;
    }
}
