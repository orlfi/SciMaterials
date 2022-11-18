using Microsoft.AspNetCore.Components;

using MudBlazor;

using SciMaterials.Contracts.API.DTO.Urls;
using SciMaterials.Contracts.WebApi.Clients.Urls;

namespace SciMaterials.UI.BWASM.Pages.urls;

public partial class UrlsStorage
{
    [Inject] private IUrlsClient UrlsClient { get; set; }
    private IEnumerable<GetUrlResponse> pagedData;
    private MudTable<GetUrlResponse> table;

    private string? searchString = null;

    private async Task<TableData<GetUrlResponse>> ServerReload(TableState state)
    {
        var result = await UrlsClient.GetAllAsync();
        if (result.Succeeded)
        {
            var data = result.Data;
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }).ToArray();

            switch (state.SortLabel)
            {
                case "id_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Id);
                    break;
                case "name_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Name);
                    break;
                case "title_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Title);
                    break;
                case "description_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Description);
                    break;
                case "link_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Link);
                    break;
            }

            pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
            return new TableData<GetUrlResponse>() { TotalItems = result.Data.Count(), Items = pagedData };
        }

        return new TableData<GetUrlResponse>() { TotalItems = 0, Items = null };
    }

    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }
}
