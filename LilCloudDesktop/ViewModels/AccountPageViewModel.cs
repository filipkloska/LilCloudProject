using LilCloudDesktop.Data;

namespace LilCloudDesktop.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class AccountPageViewModel : PageViewModel
{
    [ObservableProperty]
    private string _title = "Account";

    public AccountPageViewModel()
    {
        PageName = ApplicationPageNames.Account;
    }
}