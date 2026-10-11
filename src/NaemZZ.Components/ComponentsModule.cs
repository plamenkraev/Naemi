using BlazorDatasheet.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NaemZZ.Components.Menus;
using NaemZZ.Components.Themes;
using NaemZZ.Core.Dto;
using NaemZZ.Core.Validators;
using Tjt.AspNetCore.Components.Web.Authentication;
using Tjt.AspNetCore.WebAssembly.Animation;
using Tjt.AspNetCore.WebAssembly.DependencyInjection.Extensions;
using Tjt.AspNetCore.WebAssembly.Theming;
using Tjt.AspNetCore.WebAssembly.Theming.Components;
using Tjt.AspNetCore.WebAssembly.Theming.Navigation;
using Tjt.Modularity;
using Tjt.Validators;

namespace NaemZZ.Components;

public class ComponentsModule : Module
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ConfigureThemes(context.Services);
        context.Services.AddAutoMapper(x => x.AddProfile<CoreAutomaperProfile>());
        context.Services.AddBlazorDatasheet();
        context.Services.AddClientAuthentication();

        context.Services.AddValidators(typeof(LandlordValidator).Assembly);

        Configure<AppNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new MenuContributor());
        });
    }

    public static void ConfigureThemes(IServiceCollection services)
    {
        var configuration = services.GetConfiguration();

        var lightTheme = new LightTheme();
        var darkTheme = new DarkTheme();

        services.AddSingleton(lightTheme);
        services.AddSingleton(darkTheme);

        services.Configure<ThemingOptions>(themes =>
        {
            themes.Themes.Add<LightTheme>();
            themes.Themes.Add<DarkTheme>();

            themes.DefaultThemeName = LightTheme.Name;
        });

        services.Configure<DialogOptions>(options =>
        {
            options.OverlayClass = "bg-black/50 backdrop-blur-xs";
            options.TransitionEnter = AnimateBook.ZoomEntrances.ZoomIn.Build();
            options.TransitionExit = AnimateBook.FadingExits.FadeOut.Build();
            options.TransitionEnterDuration = 250;
            options.TransitionExitDuration = 200;
            options.MaxWidth = "xl";
            options.FullWidth = true;
        });
    }
}