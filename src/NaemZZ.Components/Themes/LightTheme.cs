using System.Text;
using Tjt.AspNetCore.Components.Enums;
using Tjt.AspNetCore.WebAssembly.Theming;
using Tjt.AspNetCore.WebAssembly.Theming.Components;

namespace NaemZZ.Components.Themes;

[ThemeName(Name)]
public class LightTheme : Theme
{
    public const string Name = "Light";

    private static readonly ThemeColor Primary = "#2E7D32";
    private static readonly ThemeColor Secondary = "#455A48";
    private static readonly ThemeColor Accent = "#8BC34A";

    public LightTheme()
    {
        ConfigureTypography();
        ConfigurePrimitives();

        ThemePaletteBuilder.BuildLight(Palette);

        Palette.Semantic.SurfaceScale.SunkenStep = 0.015f;
        Palette.Semantic.SurfaceScale.MutedStep = 0.05f;
        Palette.Semantic.SurfaceScale.SubtleStep = 0.12f;
        Palette.Semantic.SurfaceScale.ElevatedStep = 0.17f;
        Palette.Semantic.SurfaceScale.RaisedStep = 0.24f;
        Palette.Semantic.SurfaceScale.BorderStep = 0.32f;
        Palette.Semantic.SurfaceScale.BorderMutedOpacity = 0.65f;

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

        Palette.Primitive.Success.Base = "#2E7D32";
        Palette.Primitive.Info.Base = "#0288D1";
        Palette.Primitive.Warning.Base = "#B7791F";
        Palette.Primitive.Error.Base = "#D32F2F";

        Palette.Primitive.Black.Base = "#000000";
        Palette.Primitive.White.Base = "#ffffff";
        Palette.Primitive.Neutral.Base = "#E8ECE6";

        Palette.Primitive.Primary[400] = Primary.Lighten(.12f);
        Palette.Primitive.Primary[700] = Primary.Darken(.12f);
        Palette.Primitive.Error[50] = "#FDE8E7";
    }

    private void ConfigureSemanticOverrides()
    {
        Palette.Semantic.Background = "#F3F5F0";
        Palette.Semantic.Surface = "#ffffff";

        Palette.Semantic.TextPrimary = "#1F2D20";
        Palette.Semantic.TextSecondary = "#526355";
        Palette.Semantic.TextTertiary = "#758477";

        Palette.Semantic.Primary = Primary;
        Palette.Semantic.PrimaryForeground = "#ffffff";
        Palette.Semantic.PrimaryHover = "#388E3C";
        Palette.Semantic.PrimaryActive = "#1B5E20";
        Palette.Semantic.PrimarySunken = "#E8F5E9";

        Palette.Semantic.Secondary = Secondary;
        Palette.Semantic.SecondaryForeground = "#ffffff";

        Palette.Semantic.Overlay = Palette.Primitive.Black.Base.SetOpacity(0.4f);
    }

    private void ConfigureComponentOverrides()
    {
        Palette.Components.Appbar.Background = Primary;
        Palette.Components.Appbar.Foreground = "#ffffff";

        Palette.Components.Drawer.Background = "#ffffff";
        Palette.Components.Drawer.Foreground = "#1F2D20";

        Palette.Components.Table.Lines = Palette.Primitive.Black.Base.SetOpacity(.12f);

        Palette.Components.Table.Striped = "#F8FAF7";
        Palette.Components.Table.Hover = "#EDF5EB";

        Palette.Components.Input.Background = Palette.Semantic.SurfaceScale.Muted;
        Palette.Components.Input.Placeholder = Palette.Semantic.SurfaceScale.Raised;

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

    public override void SetStyle(StringBuilder builder)
    {
        builder.AppendLine("--scroll-bar-thumb: color-mix(in srgb, currentColor 25%, transparent);");
        builder.AppendLine("--scroll-bar-track: color-mix(in srgb, currentColor 10%, transparent);");
        builder.AppendLine("--scroll-bar-thumb-hover: color-mix(in srgb, currentColor 30%, transparent);");
        base.SetStyle(builder);
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