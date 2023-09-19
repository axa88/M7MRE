// ReSharper disable once CheckNamespace
namespace MauiAppNet7.Permissions;

internal partial class LocationPermissions
{
	private readonly string _granularity;

	// for direct use where granularity isn't required
	public LocationPermissions() => _granularity = "General";

	protected LocationPermissions(string granularity) => _granularity = granularity;

	public string RationalTitle => $"{_granularity} Location permission required";
	public string RationalMessage => $"Permission to use {_granularity} Location is required by Bluetooth on your version of Android";
	public string RationalButtonText => "Continue";
	public string GoToSettingsTitle => $"{_granularity} Location Access Denied?";
	public string GoToSettingsMessage => $"Go to Settings and enable \"{_granularity} Location\" to use this App";
	public string GoToSettingsAcceptText => "Go";
	public string GoToSettingsCancelText => "Exit";
}