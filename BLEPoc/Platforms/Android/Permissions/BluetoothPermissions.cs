using Android;
using Android.OS;

// ReSharper disable once CheckNamespace
namespace BLEPoC.Permissions;

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

	#region Overrides of BasePlatformPermission

	public override (string androidPermission, bool isRuntime)[] RequiredPermissions
	{
		get
		{
			#pragma warning disable CA1416

			var permissions = Build.VERSION.SdkInt switch
			{
				>= BuildVersionCodes.S => [Manifest.Permission.BluetoothConnect, Manifest.Permission.BluetoothScan],
				> BuildVersionCodes.P => [Manifest.Permission.AccessFineLocation],
				_ => new[] { Manifest.Permission.AccessCoarseLocation }
			};

			/*#if ANDROID31_0_OR_GREATER
				permissions = [Manifest.Permission.BluetoothConnect, Manifest.Permission.BluetoothScan];
			#elif ANDROID29_0_OR_GREATER
				permissions = new[] { Manifest.Permission.AccessFineLocation };
			#else
				permissions = new[] { Manifest.Permission.AccessCoarseLocation };
			#endif*/

			#pragma warning restore CA1416

			return permissions.Select(permission => (permission, true)).ToArray();
		}
	}

#endregion
}