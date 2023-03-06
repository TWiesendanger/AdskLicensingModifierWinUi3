using AdskLicensingModifier.Helpers;

namespace AdskLicensingModifier.Contracts.Services;

public interface IGenericMessageDialogService
{
    Task ShowDialog(DialogSettings dialogSettings);
}