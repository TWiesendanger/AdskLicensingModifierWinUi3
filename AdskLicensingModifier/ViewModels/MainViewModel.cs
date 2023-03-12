using AdskLicensingModifier.Contracts.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdskLicensingModifier.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    public MainViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    [RelayCommand]
    public async Task OpenDocumentation()
    {
        var uri = new Uri("https://github.com/TWiesendanger/AdskLicensingModifierWinUi3");
        await Launcher.LaunchUriAsync(uri);
    }

    [RelayCommand]
    public void MoveToSettings()
    {
        _navigationService.NavigateTo(typeof(SettingsViewModel).FullName!);
    }

}