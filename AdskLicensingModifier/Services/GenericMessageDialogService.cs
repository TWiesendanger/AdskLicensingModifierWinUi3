
using AdskLicensingModifier.Contracts.Services;
using AdskLicensingModifier.Helpers;
using AdskLicensingModifier.Views;

namespace AdskLicensingModifier.Services;

public class GenericMessageDialogService : IGenericMessageDialogService
{
    private readonly ILocalSettingsService _localSettingsService;
    private ElementTheme appTheme;
    public Task Initialization
    {
        get;
    }
    public GenericMessageDialogService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;

        Initialization = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        var appBackground = await _localSettingsService.ReadSettingAsync<string>("AppBackgroundRequestedTheme");
        appTheme = appBackground == "Dark" ? ElementTheme.Dark : ElementTheme.Light;
    }

    public async Task ShowDialog(DialogSettings dialogSettings)
    {
        var appBackground = await _localSettingsService.ReadSettingAsync<string>("AppBackgroundRequestedTheme");
        appTheme = appBackground == "Dark" ? ElementTheme.Dark : ElementTheme.Light;

        var mainXamlRoot = App.MainWindow.Content.XamlRoot;

        var messageDialog = new ContentDialog()
        {
            XamlRoot = mainXamlRoot,
            Title = dialogSettings.Title,
            PrimaryButtonText = dialogSettings.PrimaryButtonText,
            IsPrimaryButtonEnabled = dialogSettings.PrimaryButtonIsEnabled,
            PrimaryButtonCommand = dialogSettings.PrimaryButtonCommand,
            SecondaryButtonText = dialogSettings.SecondaryButtonText,
            IsSecondaryButtonEnabled = true,
            SecondaryButtonCommand = dialogSettings.SecondaryButtonCommand,
            Content = new MessageDialog(dialogSettings.Message, dialogSettings.Color, dialogSettings.Symbol),
            RequestedTheme = appTheme
        };
        await messageDialog.ShowAsync();
    }
}