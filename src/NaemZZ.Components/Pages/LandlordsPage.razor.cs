using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NaemZZ.Components.Dialogs;
using NaemZZ.Components.Events;
using NaemZZ.Core.Dto;
using NaemZZ.Core.Entities;
using NaemZZ.Core.Interfaces;
using NaemZZ.Core.Validation;
using R3;
using Tjt.AspNetCore.WebAssembly.Animation;
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
    private readonly Dictionary<int, IReadOnlyList<Municipality>> _municipalities = [];
    private readonly Dictionary<int, IReadOnlyList<Settlement>> _settlements = [];
    private IReadOnlyList<PropertyCategory> _categories = [];
    private LandlordDto? _current;
    private readonly LandlordEditorState _editor = new();
    private IReadOnlyList<LandlordListDto> _landlords = [];
    private bool _showHistory;
    private bool _suppressSelection;
    private bool HasUnsavedChanges => _editor.HasUnsavedChanges;

    private void RecordEdit(string label = "Редакция") => _editor.Record(label);

    private void Undo()
    { _current = _editor.Undo(); _ = InvokeAsync(PrepareCurrentLocationsAsync); }

    private void Redo()
    { _current = _editor.Redo(); _ = InvokeAsync(PrepareCurrentLocationsAsync); }

    private void JumpToHistory(int index)
    { _current = _editor.JumpTo(index); _ = InvokeAsync(PrepareCurrentLocationsAsync); }

    private async Task<bool> ConfirmLeaveAsync()
    {
        if (!HasUnsavedChanges)
        {
            return true;
        }
        // Three choices: Yes = save, No = discard, Cancel = stay.
        var answer = await DialogService.ShowMessageBox(
            "Незапазени промени",
            "Има незапазени промени. Да ги запазя ли преди преминаване към друг наемодател?",
            yesText: "Запази", noText: "Без запис", cancelText: "Отказ");
        if (answer is null)
        {
            return false;
        }

        if (answer == false)
        {
            return true;
        }

        return await SaveCurrentAsync();
    }

    private int? _selectedLandlordId;
    private bool _isLoadingLandlord;
    private int _selectionVersion;
    private IReadOnlyList<Company> _firms = [];
    private IReadOnlyList<ContractTemplate> _contractTemplates = [];
    private bool _isPropertyEditMode;
    private List<PropertyDto>? _propertyBackup;
    private PropertyDto? _propertyClipboard;
    private IReadOnlyList<Region> _regions = [];

    private AsyncServiceScope? _scope;
    private bool _showValidation;

    private ILandlordService Service { get; set; } = null!;
    private ReactiveProperty<string> SearchFilter { get; } = new();

    private readonly AsyncReactiveCommand SaveCommand;
    private readonly AsyncReactiveCommand DeleteCommand;
    private int? defaultRegionId = null;
    private int? defaultMunicipalityId = null;
    private int? defaultCityId = null;

    public LandlordsPage()
    {
        SaveCommand = new AsyncReactiveCommand().WithSubscribe(SaveAsync).DisposeWith(this);
        DeleteCommand = new AsyncReactiveCommand().WithSubscribe(DeleteLandlord).DisposeWith(this);
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
        _contractTemplates = await FirmManagement.ListTemplatesAsync();
        _categories = await Lookups.GetCategoriesAsync();
        _regions = await Lookups.GetRegionsAsync();

        defaultRegionId = _regions.FirstOrDefault(w => w.NAME == "Силистра")?.ID;
        defaultMunicipalityId = null;
        defaultCityId = null;

        if (defaultRegionId is not null)
        {
            await EnsureMunicipalitiesAsync(defaultRegionId.Value);

            if (_municipalities.TryGetValue(defaultRegionId.Value, out var municipalities))
            {
                defaultMunicipalityId = municipalities.FirstOrDefault(w => w.NAME == "Дулово")?.ID;
                if (defaultMunicipalityId is not null)
                {
                    await EnsureSettlementsAsync(defaultMunicipalityId.Value);

                    if (_settlements.TryGetValue(defaultMunicipalityId.Value, out var settlements))
                    {
                        defaultCityId = settlements.FirstOrDefault(w => w.NAME == "Дулово")?.ID;
                    }
                }
            }
        }

        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        using var loading = SetLoading();
        _landlords = await Service.GetListAsync(SearchFilter.Value);
        // Filtering the list must never discard the currently edited record.
        if (_current is null && _landlords.Count > 0 && !_suppressSelection)
        {
            await Select(_landlords[0]);
        }
    }
    private void CompanyChanged()
    {
        if (_current is not null)
        {
            var firm = _firms.FirstOrDefault(x => x.Id == _current.FIRM_ID);

            _current.DefaultPrice = firm?.Price;
        }
    }
    private async Task PrepareCurrentLocationsAsync()
    {
        if (_current is null)
        {
            return;
        }

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

    private IReadOnlyList<Municipality> MunicipalitiesFor(PropertyDto property)
    {
        return property.EKATTE_AREA is { } id && _municipalities.TryGetValue(id, out var items) ? items : [];
    }

    private IReadOnlyList<Settlement> SettlementsFor(PropertyDto property)
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
        if (property.EKATTE_AREA == id)
        {
            return;
        }

        property.EKATTE_AREA = id;
        property.EKATTE_MUNICIPALITY = null;
        property.EKATTE_CITY = null;

        if (id is { } region)
        {
            await EnsureMunicipalitiesAsync(region);
        }
    }

    private async Task OnPropertyMunicipalityChanged(PropertyDto property, int? id)
    {
        if (property.EKATTE_MUNICIPALITY == id)
        {
            return;
        }

        property.EKATTE_MUNICIPALITY = id;
        property.EKATTE_CITY = null;

        if (id is { } municipality)
        {
            await EnsureSettlementsAsync(municipality);
        }
    }

    private static void OnPropertySettlementChanged(PropertyDto property, int? id)
    {
        property.EKATTE_CITY = id;
    }

    private async Task Select(LandlordListDto landlord)
    {
        if (_selectedLandlordId == landlord.ID && _current is not null)
        {
            return;
        }

        if (!await ConfirmLeaveAsync())
        {
            return;
        }

        var version = ++_selectionVersion;
        _isPropertyEditMode = false;
        _propertyBackup = null;
        _selectedLandlordId = landlord.ID;
        _current = null;
        _showValidation = false;
        _isLoadingLandlord = true;
        try
        {
            var detail = await Service.GetAsync(landlord.ID);
            if (version != _selectionVersion)
            {
                return;
            }

            _current = detail;
            if (detail is not null)
            {
                _editor.Open(detail);
            }

            await PrepareCurrentLocationsAsync();
        }
        finally
        {
            if (version == _selectionVersion)
            {
                _isLoadingLandlord = false;
            }
        }
    }

    private async Task AddLandlord()
    {
        if (!await ConfirmLeaveAsync())
        {
            return;
        }

        ++_selectionVersion;
        _isLoadingLandlord = false;
        _isPropertyEditMode = false;
        _propertyBackup = null;
        _selectedLandlordId = null;

        _current = new LandlordDto()
        {
            OBLAST = defaultRegionId,
            OBSTINA = defaultMunicipalityId,
            CITY = defaultCityId,
        };
        _editor.Open(_current);
        _showValidation = false;
    }

    private async Task DeleteLandlord()
    {
        if (_current is null)
            return;

        var confirm = await DialogService.ShowMessageBox(
            L["Потвърждение"].ToString(),
            L["Наистина ли искате да изтриете този запис?"].ToString(),
            yesText: L["Изтрий"].ToString(),
            cancelText: L["Откажи"].ToString(),
            options: new DialogOptions
            {
                MaxWidth = "sm",
                CloseButton = true,
                TransitionEnter = AnimateBook.ZoomEntrances.ZoomIn.Build()
            });

        if (confirm != true)
            return;

        try
        {
            var deletedId = _current.ID;

            // Remember the position before deletion.
            var index = _landlords
                .Select((item, i) => (item, i))
                .FirstOrDefault(x => x.item.ID == deletedId).i;

            await Service.DeleteAsync(deletedId);

            ++_selectionVersion;
            _current = null;
            _editor.Clear();
            _selectedLandlordId = null;
            _isPropertyEditMode = false;
            _propertyBackup = null;

            _landlords = _landlords
                .Where(x => x.ID != deletedId)
                .ToArray();

            // Select next, or previous if the last item was deleted.
            if (_landlords.Count > 0)
            {
                index = Math.Min(index, _landlords.Count - 1);
                await Select(_landlords[index]);
            }

            await NotificationService.Success("Наемодателят е изтрит.");
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void TogglePropertyEditMode()
    {
        if (_current is null)
        {
            return;
        }

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
            RecordEdit("Отказ на редакция на имоти");
        }
        _propertyBackup = null;
        _isPropertyEditMode = false;
    }

    private void AddProperty()
    {
        if (_current is not null)
        {
            _current.IMOTIs.Add(new PropertyDto()
            {
                EKATTE_AREA = defaultRegionId,
                EKATTE_MUNICIPALITY = defaultMunicipalityId,
                EKATTE_CITY = defaultCityId,
            });
            RecordEdit("Добавен имот");
        }
    }

    private void DeleteProperty(PropertyDto property)
    {
        if (_current is not null) { _current.IMOTIs.Remove(property); RecordEdit("Изтрит имот"); }
    }

    private void CopyProperty(PropertyDto property)
    {
        _propertyClipboard = property;
    }

    private void PasteProperty(PropertyDto target)
    {
        if (_propertyClipboard is null)
        {
            return;
        }

        var originalId = target.Id;
        var ownerId = target.LandlordId;
        Mapper.Map(_propertyClipboard, target);
        target.Id = originalId;
        target.LandlordId = ownerId;
        RecordEdit("Поставени данни за имот");
        // CopyFields may change the FK values; refresh dependent lookups.
        _ = InvokeAsync(async () =>
        {
            await PrepareCurrentLocationsAsync();
            StateHasChanged();
        });
    }

    private IReadOnlyDictionary<string, string> LandlordErrors(LandlordDto landlord)
    {
        return _showValidation ? BulgarianValidation.ValidateLandlord(Mapper.Map<Landlord>(landlord)) : new Dictionary<string, string>();
    }

    private IReadOnlyDictionary<string, string> PropertyErrors(PropertyDto property)
    {
        return _showValidation ? BulgarianValidation.ValidateProperty(Mapper.Map<Property>(property)) : new Dictionary<string, string>();
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

    private async Task SaveAsync() => await SaveCurrentAsync();

    private async Task<bool> SaveCurrentAsync()
    {
        if (IsProcessing.Value || _current is null)
            return false;

        using var processing = SetProcessing();

        _showValidation = true;
        await InvokeAsync(StateHasChanged);
        await Task.Yield();

        try
        {
            var current = _current;
            var isNew = current.ID == 0;

            if (isNew)
            {
                await Service.CreateAsync(current);
            }
            else
            {
                await Service.UpdateAsync(current);
            }

            _selectedLandlordId = current.ID;
            _showValidation = false;
            _propertyBackup = null;
            _isPropertyEditMode = false;
            _editor.MarkSaved();

            _landlords = await Service.GetListAsync(SearchFilter.Value);

            if (isNew)
            {
                // Ensure the new record is visible even if the search filter excludes it.
                if (_landlords.All(x => x.ID != current.ID))
                {
                    SearchFilter.Value = string.Empty;
                    _landlords = await Service.GetListAsync();
                }

                // Wait until the list has rendered.
                await InvokeAsync(StateHasChanged);
                await Task.Delay(1000);

                await ScrollManager.ScrollIntoViewAsync($"#landlord-{current.ID}");
            }

            MessageBroker.Publish(new RefreshEvent());

            await NotificationService.Success("Промените са записани.");
            return true;
        }
        catch (ArgumentException)
        {
            _isPropertyEditMode = true;
            await NotificationService.Error("Моля, коригирайте невалидните полета преди запис.");
            return false;
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(exception);
            return false;
        }
    }

    private async Task PrintAsync()
    {
        if (_current is null)
        {
            return;
        }

        try
        {
            if (_current.ID == 0)
            {
                await NotificationService.Error("Първо запазете наемодателя.");
                return;
            }

            var contractLandlord = await Service.GetForContractAsync(_current.ID);
            if (contractLandlord is null)
            {
                return;
            }

            if (contractLandlord.FIRM_ID is null)
            {
                await NotificationService.Error("Моля, изберете фирма за наемодателя.");
                return;
            }

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