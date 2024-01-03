using BLEPoC.Ui.Models.Collection.Items;


namespace BLEPoC.Ui.Models.Collection;

internal abstract class LifeCycleTriggeredItemCollection<T> : ItemCollection<T> where T : ICollectionItem
{
	// To enforce implementation
	// ReSharper disable once UnusedMemberInSuper.Global
	protected internal abstract void PopulateCollection(string caller, EventArgs eventArgs);
}


internal static class LifeCycleTriggeredItemCollectionExtensions
{
	internal static Window EnableUpdateOnWindowActivation(this Window window, Action<string, EventArgs> populateCollection)
	{
		window.Activated += (_, eventArgs) => populateCollection(nameof(window.Activated), eventArgs);
		return window;
	}

	internal static Page EnableUpdateOnPageAppearing(this Page page, Action<string, EventArgs> populateCollection)
	{
		page.Appearing += (_, eventArgs) => populateCollection(nameof(page.Appearing), eventArgs);
		return page;
	}
}