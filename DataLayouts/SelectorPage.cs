﻿using DataLayouts.Ui;

namespace DataLayouts;

public class SelectorPage : ContentPage
{
	public SelectorPage()
	{
		Button contentWindowButton = new() { Text = "Content Page Windows" };
		contentWindowButton.Clicked += (_, __) =>
		{
			Application.Current?.OpenWindow(new CustomWindow(new MainPage(null, true, true, "CPage #0"), "MultiWin #0"));
			Application.Current?.OpenWindow(new CustomWindow(new SecondPage(true, true, "CPage #2"), "MultiWin #2"));

		}; // test multi window using 2 windows with ContentPages

		Button exitButton = new() { Text = "Exit" };
		exitButton.Clicked += (_, __) => Application.Current?.Quit();

		Button navigationPageButton = new() { Text = "Navigation Page Window" };
		navigationPageButton.Clicked += (_, __) => Application.Current?.OpenWindow(new CustomWindow(new NavigationCustom(), "NavWin #0")); // test NavigationPage using built in stacked ContentPages

		Button tabbedPageButton = new() { Text = "Tabbed Page Window" };
		tabbedPageButton.Clicked += (_, __) => Application.Current?.OpenWindow(new CustomWindow(new TabbedCustom(), "TabWin #0")); // test TabbedPage using multiple tabs of ContentPages)

		Button flyoutPageButton = new() { Text = "Flyout Page Window" };
		flyoutPageButton.Clicked += (_, __) => Application.Current?.OpenWindow(new CustomWindow(new FlyoutCustom(), "FlyWin #0")); // test FlyoutPage using ContentPages for the Flyout and Detail pages

		Button collectionButton = new() { Text = "Collection" };
		collectionButton.Clicked += (_, __) => Application.Current?.OpenWindow(new CustomWindow(new CollectionPage()));

		Content = new StackLayout
		{
			Children =
			{
				exitButton,
				contentWindowButton,
				navigationPageButton,
				tabbedPageButton,
				flyoutPageButton,
				collectionButton
			}
		};
	}
}