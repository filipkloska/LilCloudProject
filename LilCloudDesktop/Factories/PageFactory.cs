using System;
using LilCloudDesktop.Data;
using LilCloudDesktop.ViewModels;

namespace LilCloudDesktop.Factories;

public class PageFactory(Func<ApplicationPageNames, PageViewModel> pageFactory)
{
    public PageViewModel GetPageViewModel(ApplicationPageNames pageName) => pageFactory.Invoke(pageName);
}