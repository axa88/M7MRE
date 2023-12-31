﻿using BLEPoC.Permissions;
using BLEPoC.Ui.Models.Collection;
using BLEPoC.Ui.Models.Collection.Items;
using BLEPoC.Ui.Pages.Basic;
using BLEPoC.Ui.Pages.Ble;
using BLEPoC.Ui.Pages.Controls;
using BLEPoC.Ui.Pages.Permissions;
using BLEPoC.Ui.ViewModels.Ble;
using BLEPoC.Ui.ViewModels.Tests;
using BLEPoC.Utility;


namespace BLEPoC.Ui.Pages.Tests;

internal class SelectorPage : PermissionsEnabledContentPage
{
	internal SelectorPage() : base(true, true)
	{
		Button contentWindowButton = new() { Text = "Content Page Windows" };
		contentWindowButton.Clicked += (_, _) =>
		{
			Application.Current?.OpenWindow(new WindowCustom(new FirstPage(null, true, true, "CPage #0"), "MultiWin #0"));
			Application.Current?.OpenWindow(new WindowCustom(new SecondPage(true, true, "CPage #2"), "MultiWin #2"));

		}; // test multi window using 2 windows with ContentPages

		Button exitButton = new() { Text = "Exit" };
		exitButton.Clicked += (_, _) => Application.Current?.Quit();

		Button navigationPageButton = new() { Text = "Navigation Page Window" };
		navigationPageButton.Clicked += (_, _) => Application.Current?.OpenWindow(new WindowCustom(new NavigationPageCustom(), "NavWin #0")); // test NavigationPage using built in stacked ContentPages

		Button tabbedPageButton = new() { Text = "Tabbed Page Window" };
		tabbedPageButton.Clicked += (_, _) => Application.Current?.OpenWindow(new WindowCustom(new TabbedPageCustom(), "TabWin #0")); // test TabbedPage using multiple tabs of ContentPages)

		Button flyoutPageButton = new() { Text = "Flyout Page Window" };
		flyoutPageButton.Clicked += (_, _) => Application.Current?.OpenWindow(new WindowCustom(new FlyoutPageCustom(), "FlyWin #0")); // test FlyoutPage using ContentPages for the Flyout and Detail pages

		Button controlsCollectionButton = new() { Text = "Collection of Custom Controls" };
		controlsCollectionButton.Clicked += (_, _) => Application.Current?.OpenWindow(new WindowCustom(new CollectionPage<CollectionItem>(new TestControlsCollectionViewModel<CollectionItem>()), "ctrls Coll Win"));

		Button navigationPageCollectionButton = new() { Text = "Nav Page with Content Page collections" };
		BondedBtDeviceCollectionViewModel<CollectionItem> bondedBtDeviceCollectionViewModel = new();
		navigationPageCollectionButton.Clicked += (_, _) => Application.Current?.OpenWindow(new WindowCustom(new NavigationPageCollection<CollectionItem>(bondedBtDeviceCollectionViewModel), "NavPage Coll Win").EnableUpdateOnWindowActivation(bondedBtDeviceCollectionViewModel.RequestCollectionUpdate));

		Button bleButton = new() { Text = "Ble" };
		bleButton.Clicked += (_, _) => Application.Current?.OpenWindow(new WindowCustom(new BleStatusPage(), "BleStatusWin #0"));
		bleButton.IsEnabled = PermissionsProcessor.Instance.PermissionGranted;
		PermissionsProcessor.Instance.PermissionsChanged += (_, permissionEventArgs) => bleButton.IsEnabled = permissionEventArgs.PermissionGranted;

		Content = new VerticalStackLayout
		{
			Children =
			{
				exitButton,
				contentWindowButton,
				navigationPageButton,
				tabbedPageButton,
				flyoutPageButton,
				controlsCollectionButton,
				navigationPageCollectionButton,
				bleButton
			}
		};
	}
}