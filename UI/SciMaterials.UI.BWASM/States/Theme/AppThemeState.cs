using System.Collections.Immutable;
using Fluxor;
using MudBlazor;

namespace SciMaterials.UI.BWASM.States.Theme;

[FeatureState]
public record AppThemeState
{
    public MudTheme? CurrentTheme { get; init; }
    public bool IsDarkTheme { get; init; }

    public ImmutableDictionary<string, MudTheme> Themes { get; init; } = new[]
    {
        new KeyValuePair<string, MudTheme>("default", new MudTheme()
        {
            Palette = new Palette
            {
                White = "#fff",
                Black = "#000",

                Primary = "#fff",
                PrimaryDarken = "#fff",
                PrimaryLighten = "#fff",
                PrimaryContrastText = "#fff",

                Secondary = "#fff",
                SecondaryDarken = "#fff",
                SecondaryLighten = "#fff",
                SecondaryContrastText = "#fff",

                Tertiary = "#fff",
                TertiaryDarken = "#fff",
                TertiaryLighten = "#fff",
                TertiaryContrastText = "#fff",

                Info = "#fff",
                InfoDarken = "#fff",
                InfoLighten = "#fff",
                InfoContrastText = "#fff",

                Success = "#fff",
                SuccessDarken = "#fff",
                SuccessLighten = "#fff",
                SuccessContrastText = "#fff",

                Warning = "#fff",
                WarningDarken = "#fff",
                WarningLighten = "#fff",
                WarningContrastText = "#fff",

                Error = "#fff",
                ErrorDarken = "#fff",
                ErrorLighten = "#fff",
                ErrorContrastText = "#fff",

                Dark = "#fff",
                DarkDarken = "#fff",
                DarkLighten = "#fff",
                DarkContrastText = "#fff",

                HoverOpacity = 0,

                GrayDefault = "#fff",
                GrayLight = "#fff",
                GrayLighter = "#fff",
                GrayDark = "#fff",
                GrayDarker = "#fff",
                OverlayDark = "#fff",
                OverlayLight = "#fff",

                TextPrimary = "#fff",
                TextSecondary = "#fff",
                TextDisabled = "#fff",
                ActionDefault = "#fff",
                ActionDisabled = "#fff",
                ActionDisabledBackground = "#fff",
                Background = "#fff",
                BackgroundGrey = "#fff",
                Surface = "#fff",
                DrawerBackground = "#fff",
                DrawerText = "#fff",
                DrawerIcon = "#fff",
                AppbarBackground = "#fff",
                AppbarText = "#fff",
                LinesDefault = "#fff",
                LinesInputs = "#fff",
                TableLines = "#fff",
                TableStriped = "#fff",
                TableHover = "#fff",
                Divider = "#fff",
                DividerLight = "#fff",
            }
        })
    }.ToImmutableDictionary(p => p.Key, p => p.Value);
}