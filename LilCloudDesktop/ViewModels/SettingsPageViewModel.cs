using LilCloudDesktop.Data;

namespace LilCloudDesktop.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
public partial class SettingsPageViewModel : PageViewModel
{
    [ObservableProperty]
    private string _title = "Settings";

    public SettingsPageViewModel()
    {
        PageName = ApplicationPageNames.Settings;
    }
}