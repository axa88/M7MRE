using MauiAppNet7.Permissions;


namespace LifecycleTriggeredPermissionsUi;

public partial class SecondPage : PermissionsEnabledContentPage
{
	public SecondPage(bool checkPermissionsOnStart, bool checkPermissionsOnResumed, string title = "") : base(checkPermissionsOnStart, checkPermissionsOnResumed)
	{
		new PageTrace(this, title);
		InitializeComponent();
	}

	internal void UpdateLabel2(string text) => Label2!.Text = text;

	private void OnButtonClicked(object sender, EventArgs e) { PermissionsProcessor.Unsubscribe(this); }
}
