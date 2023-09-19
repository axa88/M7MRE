// ReSharper disable once CheckNamespace
namespace MauiAppNet7.Permissions;

public partial class BluetoothPermissions
{
	public string RationalTitle => "Bluetooth permission required";
	public string RationalMessage => "Permission to use Bluetooth is required to communicate with your Ride";
	public string RationalButtonText => "Continue";
	public string GoToSettingsTitle => "Bluetooth Access Denied?";
	public string GoToSettingsMessage => "Go to Settings and enable \"Nearby devices\" to use this App";
	public string GoToSettingsAcceptText => "Go";
	public string GoToSettingsCancelText => "Exit";

	public static partial void EnableBluetooth() { }
}