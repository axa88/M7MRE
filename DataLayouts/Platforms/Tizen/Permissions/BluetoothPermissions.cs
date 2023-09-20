// ReSharper disable once CheckNamespace
namespace DataLayouts.Permissions;

internal partial class BluetoothPermissions
{
	private string RationalTitle => "Bluetooth permission required";
	private string RationalMessage => "Permission to use Bluetooth is required to communicate with your Ride";
	private string RationalButtonText => "Continue";
	private string GoToSettingsTitle => "Bluetooth Access Denied?";
	private string GoToSettingsMessage => "Go to Settings and enable \"Nearby devices\" to use this App";
	private string GoToSettingsAcceptText => "Go";
	private string GoToSettingsCancelText => "Exit";

	public static partial void EnableBluetooth() { }
}
