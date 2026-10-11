using Microsoft.AspNetCore.Components;
using NaemZZ.Core.Entities;
using NaemZZ.Core.Interfaces;

namespace NaemZZ.Components.Shared;

public partial class LocationCascadeSelect : ComponentBase
{
    [Inject] private ILookupService Lookups { get; set; } = default!;

    [Parameter] public int? RegionId { get; set; }
    [Parameter] public EventCallback<int?> RegionIdChanged { get; set; }

    [Parameter] public int? MunicipalityId { get; set; }
    [Parameter] public EventCallback<int?> MunicipalityIdChanged { get; set; }

    [Parameter] public int? SettlementId { get; set; }
    [Parameter] public EventCallback<int?> SettlementIdChanged { get; set; }

    [Parameter] public bool ShowLabels { get; set; } = true;

    [Parameter] public string? Class { get; set; }

    private IReadOnlyList<EK_OBL> _regions = [];
    private IReadOnlyList<EK_OBST> _municipalities = [];
    private IReadOnlyList<EKATTE> _settlements = [];

    private int? _loadedMunicipalitiesForRegion;
    private int? _loadedSettlementsForMunicipality;

    protected override async Task OnInitializedAsync()
    {
        _regions = await Lookups.GetRegionsAsync();
        await RefreshMunicipalitiesAsync();
        await RefreshSettlementsAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        await RefreshMunicipalitiesAsync();
        await RefreshSettlementsAsync();
    }

    private async Task RefreshMunicipalitiesAsync()
    {
        if (RegionId == _loadedMunicipalitiesForRegion)
        {
            return;
        }

        _municipalities = RegionId is int regionId
            ? await Lookups.GetMunicipalitiesAsync(regionId)
            : [];
        _loadedMunicipalitiesForRegion = RegionId;
    }

    private async Task RefreshSettlementsAsync()
    {
        if (MunicipalityId == _loadedSettlementsForMunicipality)
        {
            return;
        }

        _settlements = MunicipalityId is int municipalityId
            ? await Lookups.GetSettlementsAsync(municipalityId)
            : [];
        _loadedSettlementsForMunicipality = MunicipalityId;
    }

    private async Task OnRegionChangedAsync()
    {
        await RegionIdChanged.InvokeAsync(RegionId);

        // Cascade-clear children when the parent selection changes (mirrors legacy cascading combos).
        MunicipalityId = null;
        await MunicipalityIdChanged.InvokeAsync(null);
        SettlementId = null;
        await SettlementIdChanged.InvokeAsync(null);

        await RefreshMunicipalitiesAsync();
        await RefreshSettlementsAsync();
    }

    private async Task OnMunicipalityChangedAsync()
    {
        await MunicipalityIdChanged.InvokeAsync(MunicipalityId);

        SettlementId = null;
        await SettlementIdChanged.InvokeAsync(null);

        await RefreshSettlementsAsync();
    }

    private async Task OnSettlementChangedAsync()
    {
        await SettlementIdChanged.InvokeAsync(SettlementId);
    }
}
