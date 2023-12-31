﻿using static Microsoft.Maui.ApplicationModel.Permissions;


namespace BLEPoC.Permissions;

internal partial class LocationPermissions : LocationWhenInUse, IPermissionPrompts
{
	public Task DisplayRationalAlert(Page page) => page.DisplayAlert(RationalTitle, RationalMessage, RationalButtonText);

	public Task<bool> DisplaySettingsAlert(Page page) => page.DisplayAlert(GoToSettingsTitle, GoToSettingsMessage, GoToSettingsAcceptText, GoToSettingsCancelText);
}


internal class CoarseLocationPermissions() : LocationPermissions("Coarse");


internal class FineLocationPermissions() : LocationPermissions("Fine");
