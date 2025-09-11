using Avalonia.Controls;
using Avalonia.Input;
using LilCloudDesktop.ViewModels;

namespace LilCloudDesktop;

public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount == 1)
        {
            (DataContext as MainViewModel)?.SideMenuExpandCommand.Execute(null);
        }
    }
}