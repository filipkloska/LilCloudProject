using CommunityToolkit.Mvvm.ComponentModel;
using LilCloudDesktop.Data;

namespace LilCloudDesktop.ViewModels;

public partial class CloudPageViewModel : PageViewModel
{
    [ObservableProperty]
    private string _title = "Cloud";

    public CloudPageViewModel()
    {
        PageName = ApplicationPageNames.Cloud;
    }
}