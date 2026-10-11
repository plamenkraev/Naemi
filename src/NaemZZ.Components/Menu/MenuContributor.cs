using Blazicons;
using Tjt.AspNetCore.WebAssembly.Helpers;
using Tjt.AspNetCore.WebAssembly.Theming.Navigation;
using Tjt.Localization;

namespace NaemZZ.Components.Menus;

public class MenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name is StandardMenus.Main or StandardMenus.Manager)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<DefaultResource>();

        var landRoad = new ApplicationMenuItem("LandRoad", l["Наемодатели"], url: "/", icon: BootstrapIcon.People.ToIcon());
        var settings = new ApplicationMenuItem("Settings", l["Настройки"], url: "/settings", icon: BootstrapIcon.People.ToIcon());

        settings
            .AddItem(new ApplicationMenuItem("Settings.companies", l["Фирми"], icon: FluentUiIcon.Organization.ToIcon(), url: "settings/companies"))
            .AddItem(new ApplicationMenuItem("Settings.templates", l["Шаблони на договори"], icon: FontAwesomeSolidIcon.FileContract.ToIcon(), url: "settings/templates"));

        context.Menu.AddItem(landRoad);
        context.Menu.AddItem(settings);
    }
}