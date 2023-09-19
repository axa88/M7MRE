namespace MauiAppNet7.Permissions;

internal interface IPermissionPrompts
{
	Task DisplayRationalAlert(Page page);
	Task<bool> DisplaySettingsAlert(Page page);
}