namespace BLEPoC;

internal class CustomWindow : Window
{
	internal CustomWindow(Page page, string title = "") : base(page) => _ = new LifeCycleTracing(this, title);
}