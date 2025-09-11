using CommunityToolkit.Mvvm.ComponentModel;
using LilCloudDesktop.Data;

namespace LilCloudDesktop.ViewModels;

public partial class PageViewModel : ViewModelBase
{
    [ObservableProperty]
    private ApplicationPageNames _pageName;
}