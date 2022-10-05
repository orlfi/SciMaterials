using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using SciMaterials.Domain.Core;
using SciMaterials.UI.MVC.API.Mappings;
using SciMaterials.UI.MVC.API.Services.Interfaces;

namespace SciMaterials.UI.MVC.API.Controllers;

[ApiController]
[Route("api/files")]
public class FilesApiController : ApiBaseController<FilesApiController>
{
    private readonly IFileService<Guid> _fileService;

    private void LogError(Exception ex, [CallerMemberName] string methodName = null!)
        => _logger.LogError(ex, "ошибка выполнения {error}", methodName);

    public FilesApiController(IFileService<Guid> fileService)
    {
        _fileService = fileService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        _logger.LogDebug("Get all files");
        var result = _fileService.GetAll();
        return Ok(result);
    }

    [HttpGet("DownloadByHash/{hash}")]
    public IActionResult DownloadByHash([FromRoute] string hash)
    {
        _logger.LogDebug("Get file by hash");

        var fileInfo = _fileService.GetByHash(hash);
        var fileStream = _fileService.GetFileStream(fileInfo.Data.Id);
        return File(fileStream, fileInfo.Data.ContentType, fileInfo.Data.FileName);
    }

    [HttpGet("DownloadById/{id}")]
    public IActionResult DownloadById([FromRoute] Guid id)
    {
        var fileInfo = _fileService.GetById(id);
        var fileStream = _fileService.GetFileStream(id);
        return File(fileStream, fileInfo.Data.ContentType, fileInfo.Data.FileName);
    }

    [HttpPost("Upload")]
    public async Task<IActionResult> UploadAsync()
    {
        var request = HttpContext.Request;

        if (!request.HasFormContentType ||
           !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
           string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
        {
            return new UnsupportedMediaTypeResult();
        }

        var reader = new MultipartReader(mediaTypeHeader.Boundary.Value, request.Body);
        var section = await reader.ReadNextSectionAsync();

        while (section != null)
        {
            var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                out var contentDisposition);

            if (hasContentDispositionHeader && contentDisposition is not null && contentDisposition.DispositionType.Equals("form-data") &&
                !string.IsNullOrEmpty(contentDisposition.FileName.Value))
            {
                _logger.LogInformation("Section contains file {file}", contentDisposition.FileName.Value);
                var result = await _fileService.UploadAsync(section.Body, contentDisposition.FileName.Value, section.ContentType ?? "application/octet-stream").ConfigureAwait(false);
                return Ok(result);
            }

            section = await reader.ReadNextSectionAsync();
        }

        return Ok(Result.Error("No files data in the request."));
    }

}
