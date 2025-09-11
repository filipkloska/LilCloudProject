using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LilCloudDesktop.Data;
using LilCloudDesktop.Factories;
using LilCloudDesktop.Views;

namespace LilCloudDesktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private PageFactory _pageFactory;
    
    [ObservableProperty]
    private bool _sideMenuExpanded = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HomePageIsActive))]
    [NotifyPropertyChangedFor(nameof(AccountPageIsActive))]
    [NotifyPropertyChangedFor(nameof(CloudPageIsActive))]
    [NotifyPropertyChangedFor(nameof(SettingsPageIsActive))]
    private PageViewModel _currentPage;
    
    public bool HomePageIsActive => CurrentPage.PageName == ApplicationPageNames.Home;
    public bool AccountPageIsActive => CurrentPage.PageName == ApplicationPageNames.Account;
    public bool CloudPageIsActive => CurrentPage.PageName == ApplicationPageNames.Cloud;
    public bool SettingsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Settings;
    public MainViewModel(PageFactory pageFactory)
    {
        _pageFactory = pageFactory;
        GoToHomePage();
    }

    [RelayCommand]
    private void SideMenuExpand() => SideMenuExpanded = !SideMenuExpanded;
    [RelayCommand]
    private void GoToHomePage() =>  CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Home);
    [RelayCommand]
    private void GoToAccountPage() => CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Account);
    [RelayCommand]
    private void GoToCloudPage() => CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Cloud);
    [RelayCommand]
    private void GoToSettingsPage() => CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Settings);
}