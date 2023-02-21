using CommunityToolkit.Mvvm.ComponentModel;

namespace AdskLicensingModifier.ViewModels;

public partial class ModifyLicensingViewModel : ObservableRecipient
{
    [ObservableProperty] private string? searchText;
    [ObservableProperty] private ListViewItem? selectedLicense;
    public ModifyLicensingViewModel()
    {
        // Initialize / check for exe
        CheckPath();
        // load product names
    }

    private void CheckPath()
    {
        var exeCheck =
            Directory.Exists(
                @"C:\Program Files (x86)\Common Files\Autodesk Shared\AdskLicensing\Current\helper\AdskLicensingInstHelper.exe");

        // Show Messagebox if 
    }
}
