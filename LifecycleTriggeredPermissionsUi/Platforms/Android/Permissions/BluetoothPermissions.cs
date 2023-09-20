using Android;
using Android.OS;

// ReSharper disable once CheckNamespace
namespace LifecycleTriggeredPermissionsUi.Permissions;

internal partial class BluetoothPermissions
{
	private static string RationalTitle => "Bluetooth permission required";
	private static string RationalMessage => "Permission to use Bluetooth is required to communicate with your Ride";
	private static string RationalButtonText => "Continue";
	private static string GoToSettingsTitle => "Bluetooth Access Denied?";
	private static string GoToSettingsMessage => "Go to Settings and enable \"Nearby devices\" to use this App";
	private static string GoToSettingsAcceptText => "Go";
	private static string GoToSettingsCancelText => "Exit";

	public static partial void EnableBluetooth()
	{
		/*var activity = Platform.CurrentActivity;
		activity?.StartActivity(new Intent(BluetoothAdapter.ActionRequestEnable));*/
	}

	public override (string androidPermission, bool isRuntime)[] RequiredPermissions
	{
		get
		{
			var permissions = Build.VERSION.SdkInt switch
			{
				>= BuildVersionCodes.S => new[] { Manifest.Permission.BluetoothConnect, Manifest.Permission.BluetoothScan },
				> BuildVersionCodes.P => new[] { Manifest.Permission.AccessFineLocation },
				_ => new[] { Manifest.Permission.AccessCoarseLocation }
			};

			return permissions.Select(permission => (permission, true)).ToArray();
		}
	}
}