
using AdskLicensingModifier.Contracts.Services;
using AdskLicensingModifier.Helpers;
using AdskLicensingModifier.Views;

namespace AdskLicensingModifier.Services;

public class GenericMessageDialogService : IGenericMessageDialogService
{
    public async Task ShowDialog(DialogSettings dialogSettings)
    {
        var mainXamlRoot = App.MainWindow.Content.XamlRoot;

        var messageDialog = new ContentDialog()
        {
            XamlRoot = mainXamlRoot,
            Title = dialogSettings.Title,
            PrimaryButtonText = dialogSettings.PrimaryButtonText,
            IsPrimaryButtonEnabled = dialogSettings.PrimaryButtonIsEnabled,
            SecondaryButtonText = dialogSettings.SecondaryButtonText,
            IsSecondaryButtonEnabled = true,
            SecondaryButtonCommand = dialogSettings.SecondaryButtonCommand,
            Content = new MessageDialog(dialogSettings.Message, dialogSettings.Color, dialogSettings.Symbol),
        };
        await messageDialog.ShowAsync();
    }
}