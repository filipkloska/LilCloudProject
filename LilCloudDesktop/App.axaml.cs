using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LilCloudDesktop.Data;
using LilCloudDesktop.Factories;
using LilCloudDesktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LilCloudDesktop;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        
        collection.AddSingleton<MainViewModel>();
        collection.AddTransient<AccountPageViewModel>();
        collection.AddTransient<CloudPageViewModel>();
        collection.AddTransient<HomePageViewModel>();
        collection.AddTransient<SettingsPageViewModel>();

        collection.AddSingleton<PageFactory>();
        
        collection.AddSingleton<Func<ApplicationPageNames, PageViewModel>>(x => name 
            => name switch
        {
            ApplicationPageNames.Home => x.GetRequiredService<HomePageViewModel>(),
            ApplicationPageNames.Account => x.GetRequiredService<AccountPageViewModel>(),
            ApplicationPageNames.Settings => x.GetRequiredService<SettingsPageViewModel>(),
            ApplicationPageNames.Cloud => x.GetRequiredService<CloudPageViewModel>(),
            _ => throw new InvalidOperationException()
        });
        
        var services = collection.BuildServiceProvider();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainView
            {
                DataContext = services.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}