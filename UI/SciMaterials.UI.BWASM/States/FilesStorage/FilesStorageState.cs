using System.Collections.Immutable;

using Fluxor;

namespace SciMaterials.UI.BWASM.States.FilesStorage;

[FeatureState]
public record FilesStorageState(ImmutableArray<FileStorageState> Files)
{
    public FilesStorageState() : this(ImmutableArray<FileStorageState>.Empty) { }
}

public record FileStorageState(Guid Id, string FileName, string Category, long Size);

public record struct LoadFiles();
public record struct LoadFilesResult(ImmutableArray<FileStorageState> Files);
public record struct DeleteFile(Guid Id);

public class FilesStorageEffects
{
    [EffectMethod(typeof(LoadFiles))]
    public Task LoadFiles(IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new LoadFilesResult(Files.ToImmutableArray()));
        return Task.CompletedTask;
    }

    private IEnumerable<FileStorageState> Files = new []
    {
        new FileStorageState(Guid.NewGuid(), "Outuf.css", "Style", 52),
        new FileStorageState(Guid.NewGuid(), "Go.go", "Script", 12432),
        new FileStorageState(Guid.NewGuid(), "Site.html", "Markup", 1252),
        new FileStorageState(Guid.NewGuid(), "Site.css", "Style", 252),
    };
}

public static class FilesStorageReducers
{
    [ReducerMethod]
    public static FilesStorageState LoadFiles(FilesStorageState state, LoadFilesResult action)
    {
        return state with { Files = action.Files };
    }

    [ReducerMethod]
    public static FilesStorageState DeleteFile(FilesStorageState state, DeleteFile action)
    {
        if (state.Files.FirstOrDefault(x => x.Id == action.Id) is not { } toDelete) return state;

        var afterDelete = state.Files.Remove(toDelete);
        return state with { Files = afterDelete };
    }
}