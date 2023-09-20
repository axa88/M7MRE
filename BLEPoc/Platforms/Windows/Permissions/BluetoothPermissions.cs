// ReSharper disable once CheckNamespace
namespace BLEPoc.Permissions;

internal partial class BluetoothPermissions
{
	private static string RationalTitle => "Bluetooth permission required";
	private static string RationalMessage => "Permission to use Bluetooth is required to communicate with your Ride";
	private static string RationalButtonText => "Continue";
	private static string GoToSettingsTitle => "Bluetooth Access Denied?";
	private static string GoToSettingsMessage => "Go to Settings and enable \"Nearby devices\" to use this App";
	private static string GoToSettingsAcceptText => "Go";
	private static string GoToSettingsCancelText => "Exit";

	public static partial void EnableBluetooth() { }
}