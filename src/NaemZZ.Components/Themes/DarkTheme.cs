using Tjt.AspNetCore.Components.Enums;
using Tjt.AspNetCore.WebAssembly.Theming;
using Tjt.AspNetCore.WebAssembly.Theming.Components;

namespace NaemZZ.Components.Themes;

[ThemeName(Name)]
public class DarkTheme : Theme
{
    public const string Name = "Dark";

    private static readonly ThemeColor Primary = "#1369ad";
    private static readonly ThemeColor Secondary = "#343a40";
    private static readonly ThemeColor Accent = "#FF515E";

    public DarkTheme()
    {
        ConfigureTypography();
        ConfigurePrimitives();

        ThemePaletteBuilder.BuildDark(Palette);

        ConfigureSemanticOverrides();
        ConfigureComponentOverrides();

        Slots.InputFieldClasses = new InputFieldSlots
        {
            Color = PaletteColor.Surface
        };

        Slots.AlertClasses = new AlertSlots
        {
            Variant = AlertVariant.Outlined
        };

        Slots.TabsSlots = new TabsSlots
        {
            TabListContainer = "w-full"
        };

        //Slots.CardClasses = new CardSlots
        //{
        //    Base = "bg-surface text-surface-foreground"
        //};

        LayoutProperties.Radiuses[ThemeRadius.Small] = "0.125rem";
        LayoutProperties.Radiuses[ThemeRadius.Medium] = "0.375rem";
        LayoutProperties.Radiuses[ThemeRadius.Large] = "0.5rem";
        LayoutProperties.DefaultBorderRadius = ThemeRadius.Small;
        LayoutProperties.ButtonBorderRadius = ThemeRadius.Medium;
        LayoutProperties.DrawerWidthLeft = "200px";
        LayoutProperties.DrawerMiniWidthLeft = "64px";
        LayoutProperties.AppbarHeight = "3.5rem";
        ScrollbarWidth = "12px";
    }

    private void ConfigureTypography()
    {
        Typography.Default.FontFamily = "Inter";
        Typography.Default.FontSize = ".875rem";

        Typography.Caption.FontSize = ".7rem";
        Typography.Caption.LetterSpacing = ".01em";

        Typography.Button.TextTransform = "inherit";

        Typography.H1.FontSize = "1.75rem";
        Typography.H5.FontSize = ".85rem";

        Typography.H1.FontWeight = 600;
        Typography.H2.FontWeight = 600;
        Typography.H3.FontWeight = 600;
        Typography.H4.FontWeight = 600;
    }

    private void ConfigurePrimitives()
    {
        Palette.Primitive.Primary.Base = Primary;
        Palette.Primitive.Primary.Foreground = "#ffffff";

        Palette.Primitive.Secondary.Base = Secondary;
        Palette.Primitive.Secondary.Foreground = "#ffffff";

        Palette.Primitive.Success.Base = "#56c26a";
        Palette.Primitive.Info.Base = "#0084d1";
        Palette.Primitive.Warning.Base = "#fee685";
        Palette.Primitive.Error.Base = "#FF515E";

        Palette.Primitive.Black.Base = "#000000";
        Palette.Primitive.White.Base = "#ffffff";
        Palette.Primitive.Neutral.Base = "#343a40";

        // Optional tuning
        Palette.Primitive.Primary[300] = Primary.Lighten(.20f);
        Palette.Primitive.Primary[400] = Primary.Lighten(.12f);
        Palette.Primitive.Primary[700] = Primary.Darken(.12f);

        Palette.Primitive.Error[400] = Accent.Lighten(.08f);
        Palette.Primitive.Error[900] = Accent.Darken(.45f);
    }

    private void ConfigureSemanticOverrides()
    {
        Palette.Semantic.Background = "#121417";
        Palette.Semantic.Surface = "#262a2f";

        Palette.Semantic.TextPrimary = "#f5f7fa";
        Palette.Semantic.TextSecondary = "#b4bcc6";
        Palette.Semantic.TextTertiary = "#8b95a1";
        Palette.Semantic.TextDisabled = "#65707c";

        Palette.Semantic.Primary = Palette.Primitive.Primary[400];
        Palette.Semantic.PrimaryForeground = "#ffffff";
        Palette.Semantic.PrimaryHover = Palette.Primitive.Primary[300];
        Palette.Semantic.PrimaryActive = Palette.Primitive.Primary[700];
        Palette.Semantic.PrimarySunken = Palette.Primitive.Primary[900];

        Palette.Semantic.Secondary = "#4b5563";
        Palette.Semantic.SecondaryForeground = "#ffffff";

        Palette.Semantic.Error = Accent;
        Palette.Semantic.ErrorForeground = "#ffffff";

        Palette.Semantic.Info = Palette.Semantic.Primary;
        Palette.Semantic.InfoForeground = "#ffffff";

        Palette.Semantic.Overlay = Palette.Primitive.Black.Base.SetOpacity(0.65f);
    }

    private void ConfigureComponentOverrides()
    {
        Palette.Components.Appbar.Background = "#15191e";
        Palette.Components.Appbar.Foreground = "#ffffff";

        Palette.Components.Drawer.Background = "#171b20";
        Palette.Components.Drawer.Foreground = Palette.Semantic.TextPrimary;

        Palette.Components.Table.Lines = Palette.Primitive.White.Base.SetOpacity(.10f);
        Palette.Components.Table.Striped = "#20252b";
        Palette.Components.Table.Hover = "#262c33";

        Palette.Components.Input.Background = "#34363a";
        Palette.Components.Input.Foreground = Palette.Semantic.TextPrimary;
        Palette.Components.Input.Placeholder = Palette.Semantic.TextTertiary;
        Palette.Components.Input.Border = Palette.Primitive.White.Base.SetOpacity(.14f);
        Palette.Components.Input.HoverBorder = Palette.Primitive.White.Base.SetOpacity(.24f);
        Palette.Components.Input.FocusBorder = Palette.Semantic.Primary;
        Palette.Components.Input.FocusRing = Palette.Semantic.Primary.SetOpacity(.35f);
        Palette.Components.Input.DisabledBackground = "#0d1014";

        // Re-apply defaults for any missing component values
        ThemePaletteBuilder.BuildComponents(Palette);

        // Keep primary button aligned to brand
        Palette.Components.Button.Primary.Background = Palette.Semantic.Primary;
        Palette.Components.Button.Primary.Foreground = Palette.Semantic.PrimaryForeground;
        Palette.Components.Button.Primary.HoverBackground = Palette.Semantic.PrimaryHover;
        Palette.Components.Button.Primary.ActiveBackground = Palette.Semantic.PrimaryActive;

        Palette.Components.Button.Surface.Background = Palette.Semantic.SurfaceSubtle;
        Palette.Components.Button.Surface.DisabledBackground = Palette.Semantic.TextDisabled.SetOpacity(0.12f);
    }

    protected override void BuildStyleVars(IDictionary<string, string> vars)
    {
        base.BuildStyleVars(vars);

        SetVar(vars, "scroll-bar-thumb", "color-mix(in srgb, currentColor 25%, transparent)");
        SetVar(vars, "scroll-bar-track", "color-mix(in srgb, currentColor 10%, transparent)");
        SetVar(vars, "scroll-bar-thumb-hover", "color-mix(in srgb, currentColor 30%, transparent)");

        SetVar(vars, "opacity-divider", "40%");
    }
}