
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace MauiAppNet7.Permissions;

public partial class BluetoothPermissions : BasePlatformPermission , IPermissionPrompts
{
	public static partial void EnableBluetooth();

	public Task DisplayRationalAlert(Page page) => page.DisplayAlert(RationalTitle, RationalMessage, RationalButtonText);

	public Task<bool> DisplaySettingsAlert(Page page) => page.DisplayAlert(GoToSettingsTitle, GoToSettingsMessage, GoToSettingsAcceptText, GoToSettingsCancelText);
}