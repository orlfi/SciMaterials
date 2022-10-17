using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.API.Services.Files;
using SciMaterials.Contracts.Enums;
using SciMaterials.Contracts.Result;
using SciMaterials.UI.MVC.API.Filters;

namespace SciMaterials.UI.MVC.API.Controllers;

[ApiController]
[Route(WebApiRoute.Files)]
public class FilesApiController : ApiBaseController<FilesApiController>
{
    private readonly IFileService _fileService;

    public FilesApiController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogDebug("Get all files");
        var result = await _fileService.GetAllAsync();
        return Ok(result);
    }

    /// <summary> Get file metadata by Id. </summary>
    /// <param name="id"> File Id. </param>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        _logger.LogDebug("Get by id {id}", id);
        var result = await _fileService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPut("Edit")]
    public async Task<IActionResult> EditAsync([FromBody] EditFileRequest request)
    {
        var result = await _fileService.EditAsync(request);
        return Ok(result);
    }

    [HttpGet("DownloadByHash/{hash}")]
    public async Task<IActionResult> DownloadByHash([FromRoute] string hash)
    {
        _logger.LogDebug("Get file by hash");
        var fileStreamInfoResult = await _fileService.DownloadByHash(hash);
        return File(fileStreamInfoResult.Data.FileStream, fileStreamInfoResult.Data.ContentTypeName, fileStreamInfoResult.Data.FileName);
    }

    [HttpGet("DownloadById/{id}")]
    public async Task<IActionResult> DownloadByIdAsync([FromRoute] Guid id)
    {
        _logger.LogDebug("Download by Id");
        var fileStreamInfoResult = await _fileService.DownloadById(id);
        return File(fileStreamInfoResult.Data.FileStream, fileStreamInfoResult.Data.ContentTypeName, fileStreamInfoResult.Data.FileName);
    }

    [DisableFormValueModelBinding]
    [HttpPost("Upload")]
    public async Task<IActionResult> UploadAsync()
    {
        _logger.LogDebug("Upload file");
        var request = HttpContext.Request;

        if (!request.HasFormContentType ||
           !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
           string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
        {
            return new UnsupportedMediaTypeResult();
        }

        var boundary = mediaTypeHeader.Boundary is { Length: > 2 } b && b[0] == '"' && b[^1] == '"'
            ? b.Subsegment(1, b.Length - 2)
            : mediaTypeHeader.Boundary;

        var boundaryValue = boundary.Value;
        _logger.LogInformation("Область загрузки {0}", boundaryValue);

        var reader = new MultipartReader(boundaryValue, request.Body);
        var section = await reader.ReadNextSectionAsync();

        while (section != null)
        {
            var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                out var contentDisposition);

            if (hasContentDispositionHeader && contentDisposition is not null && contentDisposition.DispositionType.Equals("form-data") &&
                !string.IsNullOrEmpty(contentDisposition.FileName.Value))
            {
                _logger.LogInformation("Section contains file {file}", contentDisposition.FileName.Value);
                if (section.Headers is null
                    || !section.Headers.ContainsKey("Metadata")
                    || System.Text.Json.JsonSerializer.Deserialize<UploadFileRequest>(section.Headers["Metadata"]) is not { } uploadFileRequest)
                    return Ok(await Result.ErrorAsync((int)ResultCodes.NotFound, "Metadata not found"));

                var result = await _fileService.UploadAsync(section.Body, uploadFileRequest).ConfigureAwait(false);
                return Ok(result);
            }

            section = await reader.ReadNextSectionAsync();
        }

        return Ok(Result.Error((int)ResultCodes.FormDataFileMissing, "Form-data sections does not contains files"));
    }

    /// <summary> Delete a file. </summary>
    /// <param name="id"> File Id. </param>
    /// <returns> Status 200 OK response. </returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _fileService.DeleteAsync(id);
        return Ok(result);
    }
}
