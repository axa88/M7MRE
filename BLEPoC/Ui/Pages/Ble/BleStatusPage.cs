using BLEPoC.Ui.Pages.Controls;
using BLEPoC.Ui.Pages.Permissions;
using BLEPoC.Ui.ViewModels.Ble;
using BLEPoC.Ui.ViewModels.Controls;
using BLEPoC.Utility;

using Microsoft.Maui.Controls.Shapes;


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

		Button testCollectionButton = new() { Text = "test Devices" };
		testCollectionButton.Clicked += (_, _) => Application.Current?.OpenWindow(new CustomWindow(new CollectionPage(new TestDevicesViewModel())));

		Button bondedDeviceButton = new() { Text = "Bonded Devices" };
		bondedDeviceButton.Clicked += (_, _) => Application.Current?.OpenWindow(new CustomWindow(new CollectionPage(new BondedDevicesViewModel())));

		Button connectedDeviceButton = new() { Text = "Connected Devices" };
		connectedDeviceButton.Clicked += (_, _) => Application.Current?.OpenWindow(new CustomWindow(new CollectionPage(new ConnectedDevicesViewModel())));

		var verticalStack = new VerticalStackLayout { Children = { isAvailableBorder , isOnBorder, stateBorder, supportsCodedPhyBorder, supportsExtendedAdvertisingBorder, bondedDeviceButton, connectedDeviceButton, testCollectionButton } };
		//var outerBorder = new OuterBorder(verticalStack);

		Content = verticalStack;
	}
}


internal class OuterBorder : Border
{
	internal OuterBorder(View content)
	{
		BackgroundColor = Colors.Transparent;
		Padding = new Thickness(20);
		StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(15, 0, 0, 10) };
		Stroke = new LinearGradientBrush { EndPoint = new Point(0, 1), GradientStops = [new() { Color = Colors.Orange, Offset = 0.1f }, new() { Color = Colors.Brown, Offset = 1.0f }] };
		Content = content;
	}
}