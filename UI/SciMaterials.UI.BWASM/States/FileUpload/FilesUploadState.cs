using System.Collections.Immutable;

using Fluxor;

using SciMaterials.UI.BWASM.Extensions;

namespace SciMaterials.UI.BWASM.States.FileUpload;

[FeatureState]
public record FilesUploadState(ImmutableArray<FileUploadState> Files)
{
    public static readonly FilesUploadState Empty = new();

    private FilesUploadState() : this(ImmutableArray<FileUploadState>.Empty) { }
}

public record FileUploadState(Guid Id, string Name, string Category, long Size, UploadState State = UploadState.NotScheduled);

public enum UploadState
{
    NotScheduled,
    Scheduled,
    Uploading,
    Uploaded,
    Failure,
    Canceled
}

public record struct RegisterFileUpload(Guid Id, string Name, string Category, long Size);

public record struct ScheduleFileUpload(Guid Id);
public record struct FileUploading(Guid Id);
public record struct FileUploaded(Guid Id);
public record struct FileUploadFailed(Guid Id);
public record struct FileUploadCanceled(Guid Id);

public static class FileUploadReducers
{
    [ReducerMethod]
    public static FilesUploadState ScheduleFileUpload(FilesUploadState state, RegisterFileUpload action)
    {
        return state with
        {
            Files = state.Files.Add(new(action.Id, action.Name, action.Category, action.Size))
        };
    }

    [ReducerMethod]
    public static FilesUploadState ScheduleFileUpload(FilesUploadState state, ScheduleFileUpload action)
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
}