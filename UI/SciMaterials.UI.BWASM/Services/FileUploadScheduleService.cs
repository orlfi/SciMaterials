using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.Result;
using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services;

public class FileUploadScheduleService : IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<FileUploadScheduleService> _logger;
    private const string FilesApiRoot = WebApiRoute.Files;

    private readonly ConcurrentQueue<FileUploadData> _uploadRequests = new();
    private readonly PeriodicTimer _sender; 


    public FileUploadScheduleService(IServiceScopeFactory scopeFactory, ILogger<FileUploadScheduleService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _sender = new(TimeSpan.FromSeconds(30));
        Upload();
    }

    public void ScheduleUpload(FileUploadData data)
    {
        _logger.LogInformation("Schedule upload of file {name}", data.FileName);
        _uploadRequests.Enqueue(data);
    }

    public void ScheduleUpload(IEnumerable<FileUploadData> data)
    {
        foreach (var uploadData in data)
            ScheduleUpload(uploadData);
    }

    private async Task Upload()
    {
        while (true)
        {
            if(!await _sender.WaitForNextTickAsync()) return;
            if(_uploadRequests.IsEmpty) continue;

            if(!_uploadRequests.TryDequeue(out var data)) continue;

            _logger.LogInformation("Building upload data for file {name}", data.FileName);

            using MultipartFormDataContent content = new();
            bool canUpload = false;

            try
            {
                // add max file size rule and cancellation token
                var fileContent = new StreamContent(data.File.OpenReadStream());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
                    data.File.ContentType is {Length:>0} contentType 
                    ? contentType
                    : "application/octet-stream");

                content.Add(fileContent, "file", data.File.Name);
                canUpload = true;
                _logger.LogInformation("Builded upload data for file {name}", data.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail to build upload file data stream");
                // log error
                // send message with file upload status
            }

            if (!canUpload) continue;

            await using (var scope = _scopeFactory.CreateAsyncScope())
            {
                try
                {
                    using var client = scope.ServiceProvider.GetRequiredService<HttpClient>();
                    var response = await client.PostAsync(FilesApiRoot + "\\Upload", content);
                    var result = await response.Content.ReadFromJsonAsync<Result<Guid>>();
                    // TODO: parse result
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fail to upload file data");
                }
            }
        }
    }

    public void Dispose()
    {
        _sender.Dispose();
    }
}