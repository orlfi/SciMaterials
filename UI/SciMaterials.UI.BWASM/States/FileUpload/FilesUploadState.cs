using System.Collections.Immutable;

using Fluxor;

using Microsoft.AspNetCore.Components.Forms;

using SciMaterials.UI.BWASM.Extensions;
using SciMaterials.UI.BWASM.Models;
using SciMaterials.UI.BWASM.Services;

namespace SciMaterials.UI.BWASM.States.FileUpload;

[FeatureState]
public record FilesUploadState(ImmutableArray<FileUploadState> Files)
{
    public static readonly FilesUploadState Empty = new();

    private FilesUploadState() : this(ImmutableArray<FileUploadState>.Empty) { }
}

public record FileUploadState(IBrowserFile BrowserFile, string? Category = null)
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string FileName { get; init; } = BrowserFile.Name;
    public long Size { get; init; } = BrowserFile.Size;
    public IBrowserFile BrowserFile { get; init; } = BrowserFile;
    public string? Category { get; init; } = Category;
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
public record struct FileUploadFailed(Guid Id);
public record struct FileUploadCanceled(Guid Id);
public record struct DeleteFileUpload(Guid Id);
public record struct ChangeCategoryOfFileUpload(Guid Id, string? Category);

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
                CancellationToken = fileUploadCancellationSource.Token
            });
            dispatcher.Dispatch(new FileUploadScheduled(data.Id, fileUploadCancellationSource));
        }
    }
}

public static class FileUploadReducers
{
    [ReducerMethod]
    public static FilesUploadState RegisterFilesUpload(FilesUploadState state, RegisterMultipleFilesUploadResult action)
    {
        return state with
        {
            Files = state.Files.AddRange(action.Files)
        };
    }

    [ReducerMethod]
    public static FilesUploadState FileUploadScheduled(FilesUploadState state, FileUploadScheduled action)
    {
        return !state.Files.ReplaceOne(
                selector: x => x.Id == action.Id,
                replacement: x => x with { State = UploadState.Scheduled},
                result: out ImmutableArray<FileUploadState> files)
            ? state 
            : state with {Files = files};
    }

    [ReducerMethod]
    public static FilesUploadState FileUploading(FilesUploadState state, FileUploading action)
    {
        return !state.Files.ReplaceOne(
                selector: x => x.Id == action.Id,
                replacement: x => x with { State = UploadState.Uploading},
                result: out ImmutableArray<FileUploadState> files)
            ? state 
            : state with {Files = files};
    }

    [ReducerMethod]
    public static FilesUploadState FileUploaded(FilesUploadState state, FileUploaded action)
    {
        return !state.Files.ReplaceOne(
                selector: x => x.Id == action.Id,
                replacement: x => x with { State = UploadState.Uploaded},
                result: out ImmutableArray<FileUploadState> files)
            ? state 
            : state with {Files = files};
    }

    [ReducerMethod]
    public static FilesUploadState FileUploadFailed(FilesUploadState state, FileUploadFailed action)
    {
        return !state.Files.ReplaceOne(
                selector: x => x.Id == action.Id,
                replacement: x => x with { State = UploadState.Failure},
                result: out ImmutableArray<FileUploadState> files)
            ? state 
            : state with {Files = files};
    }

    [ReducerMethod]
    public static FilesUploadState FileUploadCanceled(FilesUploadState state, FileUploadCanceled action)
    {
        return !state.Files.ReplaceOne(
                selector: x => x.Id == action.Id,
                replacement: x => x with { State = UploadState.Canceled},
                result: out ImmutableArray<FileUploadState> files)
            ? state 
            : state with {Files = files};
    }

    [ReducerMethod]
    public static FilesUploadState DeleteFileUpload(FilesUploadState state, DeleteFileUpload action)
    {
        if (state.Files.FirstOrDefault(x => x.Id == action.Id) is not {} toDelete) return state;

        var afterDelete = state.Files.Remove(toDelete);
        return state with {Files = afterDelete };
    }

    [ReducerMethod]
    public static FilesUploadState ChangeCategoryOfFileUpload(FilesUploadState state, ChangeCategoryOfFileUpload action)
    {
        return !state.Files.ReplaceOne(
                selector: x => x.Id == action.Id,
                replacement: x => x with { Category = action.Category },
                result: out ImmutableArray<FileUploadState> files)
            ? state 
            : state with {Files = files};
    }
}