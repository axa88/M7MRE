// ReSharper disable once CheckNamespace
namespace MauiAppNet7.Permissions;

internal interface IPermissionPrompts
{
	internal Task DisplayRationalAlert(Page page);
	internal Task<bool> DisplaySettingsAlert(Page page);
}