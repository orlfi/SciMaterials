using System.Collections.Immutable;

using Fluxor;

using Microsoft.AspNetCore.Components.Forms;

namespace SciMaterials.UI.BWASM.States.FileUpload;

[FeatureState]
public record UploadFilesFormState
{
    public static readonly UploadFilesFormState Empty = new();

    public Guid Id { get; init; } = Guid.NewGuid();
    public string ShortInfo { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;

    public CategoryInfo Category { get; set; }
    public AuthorInfo Author { get; set; }

    public ImmutableArray<FileData> Files { get; init; } = ImmutableArray<FileData>.Empty;
}

public record struct CategoryInfo(Guid Id, string Name);
public record struct AuthorInfo(Guid Id, string FirstName, string Surname);

public record FileData(IBrowserFile BrowserFile)
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string FileName { get; set; } = BrowserFile.Name;
    public string ContentType { get; init; } = BrowserFile.ContentType;
    public long Size { get; init; } = BrowserFile.Size;
}

public record struct UpdateShortInfo(string ShortInfo);
public record struct UpdateDescription(string Description);
public record struct ChangeCategory(CategoryInfo Category);
public record struct ChangeAuthor(AuthorInfo Author);

public record struct AddFiles(IEnumerable<IBrowserFile> Files);
public record struct RemoveFile(Guid Id);

public class UploadFilesMetadataFormStateReducers
{
    [ReducerMethod]
    public static UploadFilesFormState UpdateShortInfo(UploadFilesFormState state, UpdateShortInfo action)
    {
        return state with { ShortInfo = action.ShortInfo };
    }

    [ReducerMethod]
    public static UploadFilesFormState UpdateDescription(UploadFilesFormState state, UpdateDescription action)
    {
        return state with { Description = action.Description };
    }

    [ReducerMethod]
    public static UploadFilesFormState ChangeCategory(UploadFilesFormState state, ChangeCategory action)
    {
        return state with { Category = action.Category };
    }

    [ReducerMethod]
    public static UploadFilesFormState ChangeAuthor(UploadFilesFormState state, ChangeAuthor action)
    {
        return state with { Author = action.Author };
    }

    [ReducerMethod]
    public static UploadFilesFormState AddFiles(UploadFilesFormState state, AddFiles action)
    {
        return state with { Files = state.Files.AddRange(action.Files.Select(x=>new FileData(x))) };
    }

    [ReducerMethod]
    public static UploadFilesFormState RemoveFile(UploadFilesFormState state, RemoveFile action)
    {
        if (state.Files.FirstOrDefault(x => x.Id == action.Id) is not { } toDelete) return state;

        var afterDelete = state.Files.Remove(toDelete);
        return state with { Files = afterDelete };
    }
}