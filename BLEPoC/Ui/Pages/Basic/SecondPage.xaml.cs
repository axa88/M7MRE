using BLEPoC.Permissions;
using BLEPoC.Ui.Pages.Permissions;
using BLEPoC.Utility;


namespace BLEPoC.Ui.Pages.Basic;

public partial class SecondPage : PermissionsEnabledContentPage
{
	public SecondPage(bool checkPermissionsOnStart, bool checkPermissionsOnResumed, string title = "") : base(checkPermissionsOnStart, checkPermissionsOnResumed)
	{
		new LifeCycleTracing(this, title);
		InitializeComponent();
	}

	internal void UpdateLabel2(string text) => Label2!.Text = text;

	private void OnButtonClicked(object sender, EventArgs e) { PermissionsProcessor.Instance.Unsubscribe(this); }
}
