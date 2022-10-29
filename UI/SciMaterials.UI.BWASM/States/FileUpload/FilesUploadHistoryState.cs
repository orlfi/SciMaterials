using System.Collections.Immutable;

using Fluxor;

using Microsoft.AspNetCore.Components.Forms;

using SciMaterials.UI.BWASM.Extensions;
using SciMaterials.UI.BWASM.Models;
using SciMaterials.UI.BWASM.Services;

namespace SciMaterials.UI.BWASM.States.FileUpload;

[FeatureState]
public record FilesUploadHistoryState(ImmutableArray<FileUploadState> Files)
{
    public static readonly FilesUploadHistoryState Empty = new();

    private FilesUploadHistoryState() : this(ImmutableArray<FileUploadState>.Empty) { }
}

public record FileUploadState(IBrowserFile BrowserFile, string CategoryName = "", Guid CategoryId = default)
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string FileName { get; init; } = BrowserFile.Name;
    public long Size { get; init; } = BrowserFile.Size;
    public IBrowserFile BrowserFile { get; init; } = BrowserFile;
    public string CategoryName { get; init; } = CategoryName;
    public Guid CategoryId { get; init; } = CategoryId;
    public string AuthorName { get; init; } = string.Empty;
    public Guid AuthorId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ContentType { get; init; } = BrowserFile.ContentType;
    public UploadState State { get; init; } = UploadState.NotScheduled;
}

public enum UploadState
{
    NotScheduled,
    Scheduled,
    Uploading,
    Uploaded,
    Failure,
    Canceled
}

public record struct RegisterMultipleFilesUpload(IReadOnlyCollection<IBrowserFile> Files, string? Category = null);
public record struct RegisterMultipleFilesUploadResult(ImmutableArray<FileUploadState> Files);

public record struct ScheduleFilesUpload(IReadOnlyCollection<FileUploadState> Files);

public record struct FileUploadScheduled(Guid Id, CancellationTokenSource CancellationTokenSource);
public record struct FileUploading(Guid Id);
public record struct FileUploaded(Guid Id);
public record struct FileUploadFailed(Guid Id, int ErrorCode);
public record struct FileUploadCanceled(Guid Id);
public record struct DeleteFileUpload(Guid Id);
public record struct ChangeCategoryOfFileUpload(Guid Id, string CategoryName, Guid CategoryId);
public record struct UpdateFileStateFromEditForm(Guid Id, UploadFilesFormState Form);

public class FileUploadEffects
{
    private readonly FileUploadScheduleService _scheduleService;

    public FileUploadEffects(FileUploadScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [EffectMethod]
    public Task RegisterFilesToUploadList(RegisterMultipleFilesUpload action, IDispatcher dispatcher)
    {
        var data = action.Files.Select(file => new FileUploadState(file)).ToImmutableArray();
        dispatcher.Dispatch(new RegisterMultipleFilesUploadResult(data));
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task ScheduleFilesUpload(ScheduleFilesUpload action, IDispatcher dispatcher)
    {
        CancellationTokenSource cancellationTokenSource = new();
        foreach (var data in action.Files)
        {
            await Task.Delay(200);
            var fileUploadCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token);
            _scheduleService.ScheduleUpload(new FileUploadData()
            {
                Id = data.Id,
                File = data.BrowserFile,
                FileName = data.FileName,
                Category = data.CategoryId,
                Title = data.Title,
                AuthorId = data.AuthorId,
                CancellationToken = fileUploadCancellationSource.Token
            });
            dispatcher.Dispatch(new FileUploadScheduled(data.Id, fileUploadCancellationSource));
        }
    }
}

public static class FileUploadReducers
{
    [ReducerMethod]
    public static FilesUploadHistoryState RegisterFilesUpload(FilesUploadHistoryState state, RegisterMultipleFilesUploadResult action)
    {
        return state with
        {
            Files = state.Files.AddRange(action.Files)
        };
    }

    [ReducerMethod]
    public static FilesUploadHistoryState FileUploadScheduled(FilesUploadHistoryState state, FileUploadScheduled action)
    {
        return !state.Files.ReplaceOne(
                selector: x => x.Id == action.Id,
                replacement: x => x with { State = UploadState.Scheduled},
                result: out ImmutableArray<FileUploadState> files)
            ? state 
            : state with {Files = files};
    }

    [ReducerMethod]
    public static FilesUploadHistoryState FileUploading(FilesUploadHistoryState state, FileUploading action)
    {
        return !state.Files.ReplaceOne(
                selector: x => x.Id == action.Id,
                replacement: x => x with { State = UploadState.Uploading},
                result: out ImmutableArray<FileUploadState> files)
            ? state 
            : state with {Files = files};
    }

    [ReducerMethod]
    public static FilesUploadHistoryState FileUploaded(FilesUploadHistoryState state, FileUploaded action)
    {
        return !state.Files.ReplaceOne(
                selector: x => x.Id == action.Id,
                replacement: x => x with { State = UploadState.Uploaded},
                result: out ImmutableArray<FileUploadState> files)
            ? state 
            : state with {Files = files};
    }

    [ReducerMethod]
    public static FilesUploadHistoryState FileUploadFailed(FilesUploadHistoryState state, FileUploadFailed action)
    {
        return !state.Files.ReplaceOne(
                selector: x => x.Id == action.Id,
                replacement: x => x with { State = UploadState.Failure},
                result: out ImmutableArray<FileUploadState> files)
            ? state 
            : state with {Files = files};
    }

    [ReducerMethod]
    public static FilesUploadHistoryState FileUploadCanceled(FilesUploadHistoryState state, FileUploadCanceled action)
    {
        return !state.Files.ReplaceOne(
                selector: x => x.Id == action.Id,
                replacement: x => x with { State = UploadState.Canceled},
                result: out ImmutableArray<FileUploadState> files)
            ? state 
            : state with {Files = files};
    }

    [ReducerMethod]
    public static FilesUploadHistoryState DeleteFileUpload(FilesUploadHistoryState state, DeleteFileUpload action)
    {
        if (state.Files.FirstOrDefault(x => x.Id == action.Id) is not {} toDelete) return state;

        var afterDelete = state.Files.Remove(toDelete);
        return state with {Files = afterDelete };
    }

    [ReducerMethod]
    public static FilesUploadHistoryState ChangeCategoryOfFileUpload(FilesUploadHistoryState state, ChangeCategoryOfFileUpload action)
    {
        return !state.Files.ReplaceOne(
                selector: x => x.Id == action.Id,
                replacement: x => x with
                {
                    CategoryName = action.CategoryName,
                    CategoryId = action.CategoryId
                },
                result: out ImmutableArray<FileUploadState> files)
            ? state 
            : state with {Files = files};
    }

    [ReducerMethod]
    public static FilesUploadHistoryState UpdateFileStateFromEditForm(FilesUploadHistoryState state, UpdateFileStateFromEditForm action)
    {
        return !state.Files.ReplaceOne(
                selector: x => x.Id == action.Id,
                replacement: x => x with
                {
                    //FileName = action.Form.FileName,
                    //Title = action.Form.ShortInfo,
                    //CategoryName = action.Form.CategoryName,
                    //CategoryId = action.Form.CategoryId,
                    //AuthorName = action.Form.AuthorName,
                    //AuthorId = action.Form.AuthorId
                },
                result: out ImmutableArray<FileUploadState> files)
            ? state 
            : state with {Files = files};
    }
}