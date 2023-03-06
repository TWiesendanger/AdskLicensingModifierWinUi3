using Windows.UI;
using AdskLicensingModifier.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using AdskLicensingModifier.Contracts.Services;
using Newtonsoft.Json.Linq;

namespace AdskLicensingModifier.ViewModels;

public partial class ModifyLicensingViewModel : ObservableRecipient
{
    private readonly IGenericMessageDialogService _messageDialogService;
    public Task Initialization
    {
        get;
    }

    [ObservableProperty] private string? searchText;
    [ObservableProperty] private ListViewItem? selectedLicense;
    [ObservableProperty] private bool uiIsenabled;
    [ObservableProperty] private Dictionary<string, string> adskProducts;
    [ObservableProperty] private Dictionary<string, string> filteredAdskProducts;
    [ObservableProperty] private Dictionary<string, string> filteredYearAdskProducts;
    [ObservableProperty] private string selectedProduct;
    [ObservableProperty] private string selectedYear = "2023";
    public ModifyLicensingViewModel(IGenericMessageDialogService messageDialogService)
    {
        _messageDialogService = messageDialogService;
        Initialization = InitializeAsync();

        // load product names
    }

    public async Task InitializeAsync()
    {
        await CheckPath();
        AdskProducts = await ReadAutodeskProductsAsync();
        FilteredAdskProducts = new Dictionary<string, string>();
        FilteredYearAdskProducts = new Dictionary<string, string>();
        FilteredYearAdskProducts = string.IsNullOrEmpty(SelectedYear) ? AdskProducts : AdskProducts.Where(x => x.Key.Contains(SelectedYear, StringComparison.OrdinalIgnoreCase))
        .ToDictionary(pair => pair.Key, pair => pair.Value);
        FilteredAdskProducts = FilteredYearAdskProducts;
    }

    partial void OnSelectedYearChanged(string value)
    {
        FilteredYearAdskProducts = string.IsNullOrEmpty(value) ? AdskProducts : AdskProducts.Where(x => x.Key.Contains(value, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(pair => pair.Key, pair => pair.Value); ;
        FilteredAdskProducts = FilteredYearAdskProducts;
        SearchText = "";
    }

    private async Task<Dictionary<string, string>> ReadAutodeskProductsAsync()
    {
        var uri = new Uri("ms-appx:///Assets/resources/AutodeskProducts.txt");
        var productKeys = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
        var productKeyList = await Windows.Storage.FileIO.ReadLinesAsync(productKeys);

        return productKeyList
            .Select(line => line.Split(';'))
            .ToDictionary(parts => parts[0], parts => parts[1]); ;
    }

    private async Task CheckPath()
    {
        var exeCheck =
            File.Exists(
                @"C:\Program Files (x86)\Common Files\Autodesk Shared\AdskLicensing\Current\helper\AdskLicensingInstHelper.exe");

        if (exeCheck == false)
        {
            var dialogSettings = new DialogSettings()
            {
                Title = "AdskLicensingInstHelper not found",
                Message = $"AdskLicensingInstHelper was not found. This exe is needed to be able to change any license settings for autodesk products. Make sure you have the autodesk licensing service installed. ",
                Color = Color.FromArgb(255, 234, 93, 97),
                Symbol = ((char)0xEA39).ToString(),
            };
            UiIsenabled = false;
            await _messageDialogService.ShowDialog(dialogSettings);
            return;
        }
        UiIsenabled = true;
    }

    partial void OnSearchTextChanged(string? value)
    {
        //TODO maybe add a delay here
        FilteredAdskProducts = string.IsNullOrEmpty(value) ? FilteredYearAdskProducts : FilteredYearAdskProducts.Where(x => x.Key.Contains(value, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }
}
