using static Microsoft.Maui.ApplicationModel.Permissions;


// ReSharper disable once CheckNamespace
namespace MauiAppNet7.Permissions;

internal partial class LocationPermissions : LocationWhenInUse, IPermissionPrompts
{
	public Task DisplayRationalAlert(Page page) => page.DisplayAlert(RationalTitle, RationalMessage, RationalButtonText);

	public Task<bool> DisplaySettingsAlert(Page page) => page.DisplayAlert(GoToSettingsTitle, GoToSettingsMessage, GoToSettingsAcceptText, GoToSettingsCancelText);
}


internal class CoarseLocationPermissions : LocationPermissions
{
	public CoarseLocationPermissions() : base("Coarse") { }
}


internal class FineLocationPermissions : LocationPermissions
{
	public FineLocationPermissions() : base("Fine") { }
}
