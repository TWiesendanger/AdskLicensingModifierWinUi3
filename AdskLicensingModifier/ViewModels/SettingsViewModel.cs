using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;
using System.ServiceProcess;

using AdskLicensingModifier.Contracts.Services;
using AdskLicensingModifier.Helpers;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;

namespace AdskLicensingModifier.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly IGenericMessageDialogService _messageDialogService;
    private ElementTheme _elementTheme;
    [ObservableProperty] private bool desktopServiceIsOn;
    private string _versionDescription;
    private const string LICENSE_HELPER_EXE =
        @"C:\Program Files (x86)\Common Files\Autodesk Shared\AdskLicensing\Current\helper\AdskLicensingInstHelper.exe";

    public ElementTheme ElementTheme
    {
        get => _elementTheme;
        set => SetProperty(ref _elementTheme, value);
    }

    public string VersionDescription
    {
        get => _versionDescription;
        set => SetProperty(ref _versionDescription, value);
    }

    public ICommand SwitchThemeCommand
    {
        get;
    }

    public SettingsViewModel(IThemeSelectorService themeSelectorService, IGenericMessageDialogService messageDialogService)
    {
        _themeSelectorService = themeSelectorService;
        _messageDialogService = messageDialogService;
        _elementTheme = _themeSelectorService.Theme;
        _versionDescription = GetVersionDescription();

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await _themeSelectorService.SetThemeAsync(param);
                }
            });

        CheckLicensingService();
    }

    private void CheckLicensingService()
    {
        var sc = new ServiceController("AdskLicensingService");
        DesktopServiceIsOn = sc.Status == ServiceControllerStatus.Running;
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    [RelayCommand]
    private async Task PrintListCopy()
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText($"\"{LICENSE_HELPER_EXE}\" list");
        Clipboard.SetContent(dataPackage);

        var dialogSettings = new DialogSettings()
        {
            Title = "Command copied",
            Message = $"Print list command was copied. Use it in a terminal window. ",
            Color = Color.FromArgb(255, 160, 209, 77),
            Symbol = ((char)0xE73E).ToString(),
        };
        await _messageDialogService.ShowDialog(dialogSettings);
    }

    [RelayCommand]
    private async Task PrintList()
    {
        //TODO Check Path 
        var process = new Process();

        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = $"/c \"{LICENSE_HELPER_EXE}\" list";
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        //process.StartInfo.RedirectStandardError = true;

        process.Start();
        string output;
        using (StreamReader reader = process.StandardOutput)
        {
            output = await reader.ReadToEndAsync();
        }
        await File.WriteAllTextAsync(Path.Combine(Path.GetTempPath(), "AdskLicenseOutput.json"), output);

        var dialogSettings = new DialogSettings()
        {
            Title = "Export successful",
            Message = $"Export was successful. Do you want to open the file from {Path.Combine(Path.GetTempPath(), "AdskLicenseOutput.json")} ?",
            Color = Color.FromArgb(255, 160, 209, 77),
            Symbol = ((char)0xE73E).ToString(),
            PrimaryButtonIsEnabled = true,
            PrimaryButtonCommand = OpenLicenseOutputCommand,
            PrimaryButtonText = "Yes",
            SecondaryButtonIsEnabled = true,
            SecondaryButtonText = "No"
        };
        await _messageDialogService.ShowDialog(dialogSettings);

    }

    [RelayCommand]
    private void OpenLicenseOutput()
    {
        Process.Start("notepad.exe", Path.Combine(Path.GetTempPath(), "AdskLicenseOutput.json"));
    }

    [RelayCommand]
    private async void OpenLoginStatePath()
    {
        const string path = @"C:\Users\Tobias\AppData\Local\Autodesk\Web Services";
        if (Directory.Exists(path))
        {
            Process.Start("explorer.exe", path);
            return;
        }

        var dialogSettings = new DialogSettings()
        {
            Title = "Path not found",
            Message = "Path was not found and could not be opened.",
            Color = Color.FromArgb(255, 234, 93, 97),
            Symbol = ((char)0xEA39).ToString(),
        };
        await _messageDialogService.ShowDialog(dialogSettings);
    }

    [RelayCommand]
    private async void OpenAdskLicensingPath()
    {
        const string path = @"C:\ProgramData\Autodesk\AdskLicensingService";
        if (Directory.Exists(path))
        {
            Process.Start("explorer.exe", path);
            return;
        }

        var dialogSettings = new DialogSettings()
        {
            Title = "Path not found",
            Message = "Path was not found and could not be opened.",
            Color = Color.FromArgb(255, 234, 93, 97),
            Symbol = ((char)0xEA39).ToString(),
        };
        await _messageDialogService.ShowDialog(dialogSettings);
    }

    public void DesktopLicensingServiceToggled(object sender, RoutedEventArgs e)
    {
        var toogleSwitch = (ToggleSwitch)sender;
        switch (toogleSwitch.IsOn)
        {
            case true:
                {
                    var sc = new ServiceController("AdskLicensingService");
                    if (sc.Status == ServiceControllerStatus.Running || sc.Status == ServiceControllerStatus.StartPending)
                    {
                        return;
                    }

                    sc.Start();
                    break;
                }
            case false:
                {
                    var sc = new ServiceController("AdskLicensingService");
                    if (sc.Status == ServiceControllerStatus.Stopped || sc.Status == ServiceControllerStatus.StopPending)
                    {
                        return;
                    }
                    sc.Stop();
                    break;
                }
        }
    }

    [RelayCommand]
    public void RefreshDesktopLicensingState()
    {
        CheckLicensingService();
    }
}