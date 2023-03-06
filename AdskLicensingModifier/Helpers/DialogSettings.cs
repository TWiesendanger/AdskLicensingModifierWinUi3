using Windows.UI;
using CommunityToolkit.Mvvm.Input;

namespace AdskLicensingModifier.Helpers;

public class DialogSettings
{
    public string Message { get; set; } = "";
    public string Title { get; set; } = "";
    public Color Color { get; set; } = Color.FromArgb(255, 160, 209, 77);
    public string Symbol { get; set; } = ((char)0xE73E).ToString();
    public string PrimaryButtonText { get; set; } = "Ok";
    public bool PrimaryButtonIsEnabled { get; set; } = true;
    public string SecondaryButtonText { get; set; } = "";
    public bool SecondaryButtonIsEnabled
    {
        get; set;
    }
    public IRelayCommand? SecondaryButtonCommand
    {
        get; set;
    }
}