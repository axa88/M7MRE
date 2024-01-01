namespace BLEPoC.Utility;

internal class WindowCustom : Window
{
	internal WindowCustom(Page page, string title = "") : base(page) => _ = new LifeCycleTracing(this, title);
}