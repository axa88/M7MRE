﻿using BLEPoC.Ui.Pages.Permissions;
using BLEPoC.Utility;


namespace BLEPoC.Ui.Pages.Tests;

public partial class FirstPage : PermissionsEnabledContentPage
{
	private int _count;
	private readonly Page _secondPage;

	public FirstPage(Page secondPage, bool checkPermissionsOnStart, bool checkPermissionsOnResumed, string title = "") : base(checkPermissionsOnStart, checkPermissionsOnResumed)
	{
		_secondPage = secondPage;
		_ = new LifeCycleTracing(this, title);
		InitializeComponent();
	}

	private async void OnCounterClicked(object sender, EventArgs e)
	{
		CounterBtn.Text = ++_count == 1 ? $"Clicked {_count} time" : $"Clicked {_count} times";

		switch (Window.Page)
		{
			/*case ContentPage cp:
				//Application.Current?.OpenWindow(new WindowCustom(new SecondPage(true, true, "CPage #2"), "Window #2"));
				break;*/
			case NavigationPage np:
				await np.PushAsync(_secondPage);
				break;
			case TabbedPage tp:
				if (!tp.Children.Remove(_secondPage))
					tp.Children.Add(_secondPage);
				break;
			case FlyoutPage fp:
				fp.FlyoutLayoutBehavior = fp.FlyoutLayoutBehavior switch
				{
					FlyoutLayoutBehavior.Default => FlyoutLayoutBehavior.SplitOnLandscape,
					FlyoutLayoutBehavior.SplitOnLandscape => FlyoutLayoutBehavior.Split,
					FlyoutLayoutBehavior.Split => FlyoutLayoutBehavior.Popover,
					FlyoutLayoutBehavior.Popover => FlyoutLayoutBehavior.SplitOnPortrait,
					FlyoutLayoutBehavior.SplitOnPortrait => FlyoutLayoutBehavior.Default,
					_ => FlyoutLayoutBehavior.Default
				};

				CounterBtn.Text = $"{fp.FlyoutLayoutBehavior}";
				break;
		}
	}
}
