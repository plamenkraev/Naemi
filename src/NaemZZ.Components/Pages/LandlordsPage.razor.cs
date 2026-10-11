using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NaemZZ.Components.Dialogs;
using NaemZZ.Core.Entities;
using NaemZZ.Core.Dto;
using AutoMapper;
using NaemZZ.Core.Interfaces;
using NaemZZ.Core.Validation;
using R3;
using Tjt.AspNetCore.WebAssembly.Theming.Components;

namespace NaemZZ.Components.Pages;

[AllowAnonymous]
public partial class LandlordsPage : BaseDomComponent
{
    [Inject] private IMapper Mapper { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private IFirmManagementService FirmManagement { get; set; } = null!;
    [Inject] private IServiceScopeFactory ScopeFactory { get; set; } = null!;
    [Inject] private ILookupService Lookups { get; set; } = null!;

    
    private readonly Dictionary<int, IReadOnlyList<EK_OBST>> _municipalities = [];
    private readonly Dictionary<int, IReadOnlyList<EKATTE>> _settlements = [];
    private IReadOnlyList<CATEGORI> _categories = [];
    private LandlordDto? _current;
    private IReadOnlyList<FIRM> _firms = [];
    private bool _isPropertyEditMode;
    private List<PropertyDto>? _propertyBackup;
    private PropertyDto? _propertyClipboard;
    private IReadOnlyList<EK_OBL> _regions = [];

    private AsyncServiceScope? _scope;
    private bool _showValidation;
   
    private ILandlordService Service { get; set; } = null!;
    private ReactiveProperty<string> SearchFilter { get; } = new();

    private readonly AsyncReactiveCommand SaveCommand;

    public LandlordsPage()
    {
        SaveCommand = new AsyncReactiveCommand().WithSubscribe(SaveAsync).DisposeWith(this);
    }
    protected override async Task OnInitializedAsync()
    {
        _scope = ScopeFactory.CreateAsyncScope();

        Service = _scope.Value.ServiceProvider.GetRequiredService<ILandlordService>();

        SearchFilter.Skip(1).Debounce(TimeSpan.FromMilliseconds(300))
            .SubscribeAwait(async (_, _) =>
            {
                await InvokeAsync(ReloadAsync);
                await InvokeStateHasChanged();
            })
            .DisposeWith(this);
        
        _firms = await Lookups.GetFirmsAsync();
        _categories = await Lookups.GetCategoriesAsync();
        _regions = await Lookups.GetRegionsAsync();
        
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        using var loading = SetLoading();
        
        await Service.LoadAsync(SearchFilter.Value);
        
        if (_current is null || !Service.Landlords.Contains(_current))
        {
            _current = Service.Landlords.FirstOrDefault();
        }

        await PrepareCurrentLocationsAsync();
    }

    private async Task PrepareCurrentLocationsAsync()
    {
        if (_current is null) return;
        
        var properties = _current.IMOTIs.ToArray();
        
        foreach (var region in properties.Select(x => x.EKATTE_AREA).OfType<int>().Distinct())
        {
            await EnsureMunicipalitiesAsync(region);
        }

        foreach (var municipality in properties.Select(x => x.EKATTE_MUNICIPALITY).OfType<int>().Distinct())
        {
            await EnsureSettlementsAsync(municipality);
        }
    }

    private async Task EnsureMunicipalitiesAsync(int region)
    {
        if (!_municipalities.ContainsKey(region))
        {
            _municipalities[region] = await Lookups.GetMunicipalitiesAsync(region);
        }
    }

    private async Task EnsureSettlementsAsync(int municipality)
    {
        if (!_settlements.ContainsKey(municipality))
        {
            _settlements[municipality] = await Lookups.GetSettlementsAsync(municipality);
        }
    }

    private IReadOnlyList<EK_OBST> MunicipalitiesFor(PropertyDto property)
    {
        return property.EKATTE_AREA is { } id && _municipalities.TryGetValue(id, out var items) ? items : [];
    }

    private IReadOnlyList<EKATTE> SettlementsFor(PropertyDto property)
    {
        return property.EKATTE_MUNICIPALITY is { } id && _settlements.TryGetValue(id, out var items) ? items : [];
    }

    private string RegionName(int? id)
    {
        return _regions.FirstOrDefault(x => x.ID == id)?.NAME ?? "";
    }

    private string MunicipalityName(PropertyDto property)
    {
        return MunicipalitiesFor(property).FirstOrDefault(x => x.ID == property.EKATTE_MUNICIPALITY)?.NAME ?? "";
    }

    private string SettlementName(PropertyDto property)
    {
        return SettlementsFor(property).FirstOrDefault(x => x.ID == property.EKATTE_CITY)?.NAME ?? "";
    }

    private string CategoryName(int? id)
    {
        return _categories.FirstOrDefault(x => x.ID == id)?.NAME ?? "";
    }

    private async Task OnPropertyRegionChanged(PropertyDto property, int? id)
    {
        if (property.EKATTE_AREA == id) return;
        property.EKATTE_AREA = id;
        property.EKATTE_MUNICIPALITY = null;
        property.EKATTE_CITY = null;
        
        
        
        if (id is { } region) await EnsureMunicipalitiesAsync(region);
    }

    private async Task OnPropertyMunicipalityChanged(PropertyDto property, int? id)
    {
        if (property.EKATTE_MUNICIPALITY == id) return;
        property.EKATTE_MUNICIPALITY = id;
        property.EKATTE_CITY = null;
        
        
        if (id is { } municipality) await EnsureSettlementsAsync(municipality);
    }

    private static void OnPropertySettlementChanged(PropertyDto property, int? id)
    {
        property.EKATTE_CITY = id;
        
    }

    private async Task Select(LandlordDto landlord)
    {
        if (_isPropertyEditMode) DiscardPropertyChanges();
        _current = landlord;
        _showValidation = false;
        await PrepareCurrentLocationsAsync();
    }

    private void AddLandlord()
    {
        _current = Service.AddNew();
        _showValidation = false;
    }

    private async Task DeleteLandlord()
    {
        if (_current is null) return;
        Service.Delete(_current);
        _current = Service.Landlords.FirstOrDefault();
        await PrepareCurrentLocationsAsync();
    }

    private void TogglePropertyEditMode()
    {
        if (_current is null) return;
        if (!_isPropertyEditMode)
        {
            _propertyBackup = Mapper.Map<List<PropertyDto>>(_current.IMOTIs);
            _isPropertyEditMode = true;
        }
        else
        {
            DiscardPropertyChanges();
        }
    }

    private void DiscardPropertyChanges()
    {
        if (_current is not null && _propertyBackup is not null)
        {
            _current.IMOTIs = _propertyBackup;
            Service.RestorePropertyDeletions(_current.IMOTIs.Select(x => x.imotiID));
        }
        _propertyBackup = null;
        _isPropertyEditMode = false;
    }

    private void AddProperty()
    {
        if (_current is not null) Service.AddProperty(_current);
    }

    private void DeleteProperty(PropertyDto property)
    {
        if (_current is not null) Service.DeleteProperty(_current, property);
    }

    private void CopyProperty(PropertyDto property)
    {
        _propertyClipboard = property;
    }

    private void PasteProperty(PropertyDto target)
    {
        if (_propertyClipboard is null) return;
        var originalId = target.imotiID;
        var ownerId = target.nID;
        Mapper.Map(_propertyClipboard, target);
        target.imotiID = originalId;
        target.nID = ownerId;
        // CopyFields may change the FK values; refresh dependent lookups.
        _ = InvokeAsync(async () =>
        {
            await PrepareCurrentLocationsAsync();
            StateHasChanged();
        });
    }

    private IReadOnlyDictionary<string, string> LandlordErrors(LandlordDto landlord)
    {
        return _showValidation ? BulgarianValidation.ValidateLandlord(Mapper.Map<NAEMODATELI>(landlord)) : new Dictionary<string, string>();
    }

    private IReadOnlyDictionary<string, string> PropertyErrors(PropertyDto property)
    {
        return _showValidation ? BulgarianValidation.ValidateProperty(Mapper.Map<IMOTI>(property)) : new Dictionary<string, string>();
    }

    private bool HasLandlordError(string name)
    {
        return _current is not null && LandlordErrors(_current).ContainsKey(name);
    }

    private string? LandlordError(string name)
    {
        return _current is null ? null : LandlordErrors(_current).GetValueOrDefault(name);
    }
    
    private string? PropertyError(PropertyDto property, string name)
    {
        return PropertyErrors(property).GetValueOrDefault(name);
    }

    private async Task SaveAsync()
    {
        if (IsProcessing.Value) return;

        using var _ = SetProcessing();
        
        _showValidation = true;
        try
        {
            // The service remains the authoritative validator and persistence boundary.
            var result = await Service.SaveChangesAsync();
            if (result.Success)
            {
                _current = Service.Landlords.FirstOrDefault(x => x.ID == _current?.ID) ?? Service.Landlords.FirstOrDefault();
                _showValidation = false;
                _propertyBackup = null;
                _isPropertyEditMode = false;
                await NotificationService.Success("Промените са записани.");
            }
            else if (result.FirstInvalidLandlord is { } invalid)
            {
                _current = invalid;
                await PrepareCurrentLocationsAsync();
                _isPropertyEditMode = true;
                await NotificationService.Error("Моля, коригирайте невалидните полета преди запис.");
            }
            else
            {
                await NotificationService.Error(result.ErrorMessage ?? "Грешка при запис.");
            }
        }
        catch(Exception exception)
        {
           await HandleErrorAsync(exception);
        }
    }

    private async Task OpenTemplatesDialogAsync()
    {
        var dialog = await DialogService.ShowAsync<ContractTemplatesDialog>(
            "Шаблони на договори", new DialogParameters(),
            new DialogOptions { MaxWidth = "6xl", FullWidth = true, FullScreen = true });
        await dialog.Result;
    }

    private async Task OpenFirmDialogAsync()
    {
        var source = _firms.FirstOrDefault(x => x.firmID == _current?.FIRM_ID);
        var company = source is null ? new FirmDto() : Mapper.Map<FirmDto>(source);
        
        var dialog = await DialogService.ShowAsync<CompanyDialog>(
            source is null ? "Нова фирма" : "Редакция на фирма",
            new DialogParameters
            {
                [nameof(CompanyDialog.Company)] = company
            }, new DialogOptions());

        var result = await dialog.Result;
        if (result is { Cancelled: false, Data: FirmDto firm })
        {
           await SaveCompanyAsync(firm);
        }
    }

    private async Task SaveCompanyAsync(FirmDto company)
    {
        try
        {
            var saved = await FirmManagement.SaveAsync(company);
            _firms = await Lookups.GetFirmsAsync();
            if (_current is not null)
            {
                _current.FIRM_ID = saved.firmID;
            }

            await NotificationService.Success("Фирмата е записана.");
        }
        catch (Exception ex)
        {
            await NotificationService.Error(ex.Message);
        }
    }

    private async Task PrintAsync()
    {
        if (_current is null) return;
        try
        {
            if (_current.ID == 0) { await NotificationService.Error("Първо запазете наемодателя."); return; }
            var contractLandlord = await Service.GetForContractAsync(_current.ID);
            if (contractLandlord is null) return;
            var dialog = await DialogService.ShowAsync<ContractEditorDialog>(
                "Договор за наем", new DialogParameters
                {
                    [nameof(ContractEditorDialog.Landlord)] = contractLandlord
                }, new DialogOptions { MaxWidth = "6xl", FullWidth = true, FullScreen = true });
            await dialog.Result;
        }
        catch (Exception ex)
        {
            await NotificationService.Error(ex.Message);
        }
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing && _scope is { } scope)
        {
            await scope.DisposeAsync();
            _scope = null;
        }

        await base.DisposeAsync(disposing);
    }
}