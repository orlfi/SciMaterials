using Fluxor;

using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.WebApi.Clients.Files;
using SciMaterials.UI.BWASM.Models;
using SciMaterials.UI.BWASM.States.FileUpload;

namespace SciMaterials.UI.BWASM.Services;

public class FileUploadScheduleService : IDisposable
{
    private readonly IFilesClient _filesClient;
    private readonly IDispatcher _dispatcher;
    private readonly ILogger<FileUploadScheduleService> _logger;

    private readonly Queue<FileUploadData> _uploadRequests = new();
    private readonly PeriodicTimer _sender; 


    public FileUploadScheduleService(
        IFilesClient filesClient,
        IDispatcher dispatcher,
        ILogger<FileUploadScheduleService> logger)
    {
        _filesClient = filesClient;
        _dispatcher = dispatcher;
        _logger = logger;

        _sender = new(TimeSpan.FromSeconds(30));

        _ = Upload();
    }

    public void ScheduleUpload(FileUploadData data)
    {
        _logger.LogInformation("Schedule upload of file {name}", data.FileName);
        _uploadRequests.Enqueue(data);
    }

    private async Task Upload()
    {
        while (true)
        {
            if(!await _sender.WaitForNextTickAsync()) return;
            if(_uploadRequests.Count <= 0) continue;

            if(!_uploadRequests.TryDequeue(out FileUploadData? data)) continue;

            _logger.LogInformation("Building upload data for file {name}", data.FileName);
            _dispatcher.Dispatch(new FileUploading(data.Id));
            
            var result = await _filesClient.UploadAsync(
                data.File.OpenReadStream(),
                new UploadFileRequest()
                {
                    Name = data.FileName,
                    Size = data.File.Size,
                    // TODO
                    Categories = string.Empty,
                    AuthorId = Guid.Empty,
                    ContentTypeName = string.Empty,
                    Title = string.Empty
                },
                data.CancellationToken);

            if (data.CancellationToken.IsCancellationRequested)
            {
                _dispatcher.Dispatch(new FileUploadCanceled(data.Id));
                continue;
            }

            if (result.Succeeded)
            {
                _dispatcher.Dispatch(new FileUploaded(data.Id));
                continue;
            }

            _dispatcher.Dispatch(new FileUploadFailed(data.Id, result.Code));
        }
    }

    public void Dispose()
    {
        _sender.Dispose();
    }
}