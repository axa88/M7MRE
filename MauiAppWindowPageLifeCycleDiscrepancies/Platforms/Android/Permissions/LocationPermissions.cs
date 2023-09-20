// ReSharper disable once CheckNamespace
namespace MauiAppWindowPageLifeCycleDiscrepancies.Permissions;

internal partial class LocationPermissions
{
	private readonly string _granularity;

	// for direct use where granularity isn't required
	public LocationPermissions() => _granularity = "General";

	protected LocationPermissions(string granularity) => _granularity = granularity;

	private string RationalTitle => $"{_granularity} Location permission required";
	private string RationalMessage => $"Permission to use {_granularity} Location is required by Bluetooth on your version of Android";
	private static string RationalButtonText => "Continue";
	private string GoToSettingsTitle => $"{_granularity} Location Access Denied?";
	private string GoToSettingsMessage => $"Go to Settings and enable \"{_granularity} Location\" to use this App";
	private static string GoToSettingsAcceptText => "Go";
	private static string GoToSettingsCancelText => "Exit";
}