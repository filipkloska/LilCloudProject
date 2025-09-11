using CommunityToolkit.Mvvm.ComponentModel;
using LilCloudDesktop.Data;

namespace LilCloudDesktop.ViewModels;

public partial class HomePageViewModel : PageViewModel
{
    [ObservableProperty]
    private string _title = "Home";

    public HomePageViewModel()
    {
        PageName = ApplicationPageNames.Home;
    }
}