using AdskLicensingModifier.Helpers;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using WinRT.Interop;

namespace AdskLicensingModifier;

public sealed partial class MainWindow : WindowEx
{
    public MainWindow()
    {

        InitializeComponent();
        Content = null;

        Title = "AppDisplayName".GetLocalized();

    }
}
