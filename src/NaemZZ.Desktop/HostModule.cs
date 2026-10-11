using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NaemZZ.Components;
using Tjt;
using Tjt.AspNetCore.Components.Manager;
using Tjt.AspNetCore.Components.Web.Bundling;
using Tjt.AspNetCore.Components.Web.DependencyInjection;
using Tjt.AspNetCore.Components.WebAssembly;
using Tjt.AspNetCore.Components.WebAssembly.Security;
using Tjt.AspNetCore.WebAssembly.ExceptionHandling;
using Tjt.AspNetCore.WebAssembly.Http.Client;
using Tjt.AspNetCore.WebAssembly.Security.Claims;
using Tjt.AspNetCore.WebAssembly.Theming.Routing;
using Tjt.Grid.Blazor;
using Tjt.Modularity;

namespace NaemZZ.Desktop;

internal class HybridHostContext : HostContextBase
{
    public override bool IsInteractive => true;

    public override bool IsServerSide => false;
}

public sealed class WebAssemblyHostEnvironment : IWebAssemblyHostEnvironment
{
    public WebAssemblyHostEnvironment(string environment, string baseAddress)
    {
        Environment = environment;
        BaseAddress = baseAddress;
    }

    public string Environment { get; }

    public string BaseAddress { get; }
}

[DependsOn(
    typeof(AspNetCoreComponentsWebAssemblyModule),
    typeof(AspNetCoreComponentsManagerModule),
    typeof(AspNetCoreComponentsManagerModule),
    typeof(GridBlazorModule),
    typeof(ComponentsModule)
)]
internal class HostModule : Module
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AppRenderMode.Runtime = Runtimes.Hybrid;
        context.Services.AddSingleton<IHostContext, HybridHostContext>();

        var environment = Environments.Production;

#if DEBUG
        environment = Environments.Development;
#endif

        var hostEnvironment = new WebAssemblyHostEnvironment(environment, "https://0.0.0.0");
        context.Services.AddSingleton<IWebAssemblyHostEnvironment>(hostEnvironment);
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<RouterOptions>(options => { options.AppAssembly = typeof(ComponentsModule).Assembly; });

        Configure<BundleOptions>(options =>
        {
            options.AddScript("js/tinymce.js");
        });
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        Configure<HttpPipelineOptions>(options =>
        {
            options.EventHandlers.Clear();
            options.DelegatingHandlers.Clear();
        });

        Configure<ExceptionHandlingOptions>(options =>
        {
            options.SendExceptionsDetailsToClients =
                context.Services.GetSingleton<IWebAssemblyHostEnvironment>().Environment == Environments.Development;
            options.SendStackTraceToClients = false;
        });

        Configure<RemoteServiceOptions>(o => { o.RemoteServices.Clear(); });

        context.Services.ReplaceTransient<ICurrentPrincipalAccessor, WebAssemblyCurrentPrincipalAccessor>();
        context.Services
            .AddSingleton<IClientScopeServiceProviderAccessor, ComponentsClientScopeServiceProviderAccessor>();
    }
}