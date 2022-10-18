using System.Xml.Linq;

namespace SciMaterials.UI.BWASM.Models;

public class Authority
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = null!;

    public static Authority Create(string name) =>
        new(){Name = name};

    public static Authority Create(Authority origin) =>
        new() { Id = origin.Id, Name = origin.Name };

    public static readonly Authority CanAccessUsers = Create(nameof(CanAccessUsers));
    public static readonly Authority CanDeleteUsers = Create(nameof(CanDeleteUsers));
    public static readonly Authority CanChangeUsersAuthority = Create(nameof(CanChangeUsersAuthority));
}