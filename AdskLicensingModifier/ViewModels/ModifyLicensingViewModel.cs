using Windows.ApplicationModel.DataTransfer;
using Windows.UI;
using AdskLicensingModifier.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using AdskLicensingModifier.Contracts.Services;
using CommunityToolkit.Mvvm.Input;

namespace AdskLicensingModifier.ViewModels;

public partial class ModifyLicensingViewModel : ObservableRecipient
{
    private readonly IGenericMessageDialogService _messageDialogService;
    public Task Initialization
    {
        get;
    }

    private const string LICENSE_HELPER_EXE =
        @"C:\Program Files (x86)\Common Files\Autodesk Shared\AdskLicensing\Current\helper\AdskLicensingInstHelper.exe";
    [ObservableProperty] private string? searchText;
    [ObservableProperty] private string? result;
    [ObservableProperty] private string? serverNames;
    [ObservableProperty] private LicenseType selectedLicenseType = LicenseType.Reset;
    [ObservableProperty] private ServerType selectedServerType;
    [ObservableProperty] private List<ServerType>? serverTypes;
    [ObservableProperty] private List<LicenseType>? licenseTypes;
    [ObservableProperty] private bool uiIsenabled;
    [ObservableProperty] private bool serverTypeIsEnabled;
    [ObservableProperty] private Dictionary<string, string>? adskProducts;
    [ObservableProperty] private Dictionary<string, string>? filteredAdskProducts;
    private Dictionary<string, string>? _filteredYearAdskProducts;
    [ObservableProperty] private KeyValuePair<string, string> selectedProduct;
    [ObservableProperty] private string selectedYear = "2023";
    private string _productFeatureCode = "2020.0.0.F";

    public ModifyLicensingViewModel(IGenericMessageDialogService messageDialogService)
    {
        _messageDialogService = messageDialogService;
        Initialization = InitializeAsync();
        //Task.Run(async () => await InitializeAsync());
    }

    public async Task InitializeAsync()
    {


        AdskProducts = await ReadAutodeskProductsAsync();
        FilteredAdskProducts = new Dictionary<string, string>();
        _filteredYearAdskProducts = new Dictionary<string, string>();
        if (AdskProducts != null)
        {
            _filteredYearAdskProducts = string.IsNullOrEmpty(SelectedYear)
                ? AdskProducts
                : AdskProducts.Where(x => x.Key.Contains(SelectedYear, StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        FilteredAdskProducts = _filteredYearAdskProducts;


        ServerTypes = new List<ServerType>(Enum.GetValues(typeof(ServerType)).Cast<ServerType>());
        LicenseTypes = new List<LicenseType>(Enum.GetValues(typeof(LicenseType)).Cast<LicenseType>());

        await CheckPathAsync();

    }

    partial void OnSelectedYearChanged(string value)
    {
        if (AdskProducts != null)
        {
            _filteredYearAdskProducts = string.IsNullOrEmpty(value)
                ? AdskProducts
                : AdskProducts.Where(x => x.Key.Contains(value, StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        FilteredAdskProducts = _filteredYearAdskProducts;

        _productFeatureCode = value switch
        {
            "2020" => "2020.0.0.F",
            "2021" => "2021.0.0.F",
            "2022" => "2022.0.0.F",
            "2023" => "2023.0.0.F",
            "2024" => "2024.0.0.F",
            _ => ""
        };
        SearchText = "";
    }

    private async Task<Dictionary<string, string>?> ReadAutodeskProductsAsync()
    {
        var uri = new Uri("ms-appx:///Assets/resources/AutodeskProducts.txt");
        var productKeys = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
        var productKeyList = await Windows.Storage.FileIO.ReadLinesAsync(productKeys);

        return productKeyList
            .Select(line => line.Split(';'))
            .ToDictionary(parts => parts[0], parts => parts[1]);
    }

    private async Task CheckPathAsync()
    {

        if (File.Exists(LICENSE_HELPER_EXE) == false)
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
        if (_filteredYearAdskProducts != null)
        {
            FilteredAdskProducts = string.IsNullOrEmpty(value)
                ? _filteredYearAdskProducts
                : _filteredYearAdskProducts.Where(x => x.Key.Contains(value, StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }

    partial void OnSelectedLicenseTypeChanged(LicenseType value)
    {
        switch (value)
        {
            case LicenseType.Network:
                SelectedServerType = SelectedServerType = ServerType.Single;
                ServerTypeIsEnabled = true;
                break;
            default:
                SelectedServerType = SelectedServerType;
                ServerTypeIsEnabled = false;
                break;
        }
    }

    [RelayCommand]
    public async Task Generate()
    {
        if (string.IsNullOrWhiteSpace(SelectedProduct.Key))
        {
            var dialogSettings = new DialogSettings()
            {
                Title = "No product selected",
                Message = $"No product was selected. Make sure you have selected one product. ",
                Color = Color.FromArgb(255, 234, 93, 97),
                Symbol = ((char)0xEA39).ToString(),
            };
            await _messageDialogService.ShowDialog(dialogSettings);
            return;
        }

        var cmdCommand = SetCmdCommand();
        Result = cmdCommand;

        var dataPackage = new DataPackage();
        dataPackage.SetText(cmdCommand);
        Clipboard.SetContent(dataPackage);
    }

    private string SetCmdCommand()
    {
        var cmdCommand = SelectedLicenseType switch
        {
            LicenseType.Network =>
                $"{LICENSE_HELPER_EXE} change --prod_key {SelectedProduct.Value} --prod_ver {_productFeatureCode} --lic_method {SelectedLicenseType.ToString().ToUpper()} --lic_server_type {SelectedServerType.ToString().ToUpper()} --lic_servers {ServerNames}",
            LicenseType.Standalone or LicenseType.User =>
                $"{LICENSE_HELPER_EXE} change --prod_key {SelectedProduct.Value} --prod_ver {_productFeatureCode} --lic_method {SelectedLicenseType.ToString().ToUpper()}",
            LicenseType.Reset =>
                $"{LICENSE_HELPER_EXE} change --prod_key {SelectedProduct.Value} --prod_ver {_productFeatureCode} --lic_method \"\" --lic_server_type \"\" --lic_servers \"\"",
            _ => ""
        };
        return cmdCommand;
    }

    [RelayCommand]
    public async Task Run()
    {
        //TODO dont overwrite if already text in it
        if (string.IsNullOrWhiteSpace(SelectedProduct.Key))
        {
            var dialogSettings = new DialogSettings()
            {
                Title = "No product selected",
                Message = $"No product was selected. Make sure you have selected one product. ",
                Color = Color.FromArgb(255, 234, 93, 97),
                Symbol = ((char)0xEA39).ToString(),
            };
            await _messageDialogService.ShowDialog(dialogSettings);
        }

        Result = SetCmdCommand();



    }
}
