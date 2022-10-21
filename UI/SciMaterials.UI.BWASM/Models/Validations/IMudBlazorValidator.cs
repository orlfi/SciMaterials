namespace SciMaterials.UI.BWASM.Models.Validations;

public interface IMudBlazorValidator<T>
{
    Func<object, string, Task<IEnumerable<string>>> ValidateValue { get; }
}