namespace DataLayouts;

internal class CustomWindow : Window
{
	internal CustomWindow(Page page, string title = "") : base(page) => _ = new PageTrace(this, title);
}