using BLEPoC.Ui.Models;
using BLEPoC.Ui.Pages.Controls;
using BLEPoC.Ui.Pages.Permissions;
using BLEPoC.Ui.ViewModels.Ble;
using BLEPoC.Utility;


namespace BLEPoC.Ui.Pages.Ble;

internal class BleStatusPage : PermissionsEnabledContentPage
{
	internal BleStatusPage() : base(true, true)
	{
		var bleVm = new BleStatusViewModel();
		BindingContext = bleVm;

		Label isAvailableLabel = new() { Text = nameof(bleVm.BleIsAvailable) };
		isAvailableLabel.SetBinding(Label.TextProperty, nameof(bleVm.BleIsAvailable));
		var isAvailableBorder = new OuterBorder(isAvailableLabel);

		Label isOnLabel = new() { Text = nameof(bleVm.BleIsOn) };
		isOnLabel.SetBinding(Label.TextProperty, nameof(bleVm.BleIsOn));
		var isOnBorder = new OuterBorder(isOnLabel);
		var isOnRecognizer = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
		isOnRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, nameof(bleVm.BleOnCommand));
		isOnBorder.GestureRecognizers.Add(isOnRecognizer);

		Label stateLabel = new() { Text = nameof(bleVm.BleState) };
		stateLabel.SetBinding(Label.TextProperty, nameof(bleVm.BleState));
		var stateBorder = new OuterBorder(stateLabel);

		Label supportsCodedPhyLabel = new() { Text = nameof(bleVm.BleSupportsCodedPhy) };
		supportsCodedPhyLabel.SetBinding(Label.TextProperty, nameof(bleVm.BleSupportsCodedPhy));
		var supportsCodedPhyBorder = new OuterBorder(supportsCodedPhyLabel);

		Label supportsExtendedAdvertisingLabel = new() { Text = nameof(bleVm.BleSupportsExtendedAdvertising) };
		supportsExtendedAdvertisingLabel.SetBinding(Label.TextProperty, nameof(bleVm.BleSupportsExtendedAdvertising));
		var supportsExtendedAdvertisingBorder = new OuterBorder(supportsExtendedAdvertisingLabel);

		//Button testCollectionButton = new() { Text = "test Devices" };
		//testCollectionButton.Clicked += (_, _) => Application.Current?.OpenWindow(new WindowCustom(new CollectionPage(new TestDevicesViewModel())));

		Button bondedDeviceButton = new() { Text = "Bonded Devices" };
		bondedDeviceButton.Clicked += (_, _) =>
		{
			BondedDevicesViewModel bondedDevicesViewModel = new();
			Application.Current?.OpenWindow(new WindowCustom(new CollectionPage(bondedDevicesViewModel)).EnableUpdateOnWindowActivation(bondedDevicesViewModel));
		};

		Button connectedDeviceButton = new() { Text = "Connected Devices" };
		connectedDeviceButton.Clicked += (_, _) =>
		{
			ConnectedDevicesViewModel connectedDevicesViewModel = new();
			Application.Current?.OpenWindow(new WindowCustom(new CollectionPage(connectedDevicesViewModel)).EnableUpdateOnWindowActivation(connectedDevicesViewModel));
		};

		var verticalStack = new VerticalStackLayout { Children = { isAvailableBorder , isOnBorder, stateBorder, supportsCodedPhyBorder, supportsExtendedAdvertisingBorder, bondedDeviceButton, connectedDeviceButton/*, testCollectionButton*/ } };
		//var outerBorder = new OuterBorder(verticalStack);

		Content = verticalStack;
	}
}
