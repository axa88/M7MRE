using System.Runtime.CompilerServices;

using BLEPoC.Ui.Models.Collection.Items;


namespace BLEPoC.Ui.Models.Collection;

internal abstract class LifeCycleTriggeredItemCollection<T> : ItemCollection<T> where T : ICollectionItem
{
	private readonly Timer _triggerUpdate;
	private readonly List<(EventArgs EventArgs, string Name)> _callers = [];
	private readonly object _lock = new();

	// To enforce implementation
	// ReSharper disable once UnusedMemberInSuper.Global
	protected LifeCycleTriggeredItemCollection()
	{
		// zero ensure trigger upon creation // Todo verify this
		_triggerUpdate = new Timer(state => TimerExpired(), null, TimeSpan.Zero, Timeout.InfiniteTimeSpan);
	}

	//protected internal abstract void Populate(string caller, EventArgs eventArgs);
	protected abstract void UpdateCollection(List<(EventArgs EventArgs, string CallerName)> callers);

	protected internal void RequestCollectionUpdate(EventArgs eventArgs = null, [CallerMemberName] string callerName = null)
	{
		lock (_lock)
		{
			_callers.Add((eventArgs, callerName));
			_triggerUpdate.Change(TimeSpan.FromSeconds(.25), Timeout.InfiniteTimeSpan);
		}
	}

	private void TimerExpired()
	{
		lock (_lock)
		{
			UpdateCollection(_callers);
			_callers.Clear();
		}
	}
}


internal static class LifeCycleTriggeredItemCollectionExtensions
{
	internal static Window EnableUpdateOnWindowActivation(this Window window, Action<EventArgs, string> populateCollection)
	{
		window.Activated += (_, eventArgs) => populateCollection(eventArgs, nameof(window.Activated));
		return window;
	}

	internal static Page EnableUpdateOnPageAppearing(this Page page, Action<EventArgs, string> populateCollection)
	{
		page.Appearing += (_, eventArgs) => populateCollection(eventArgs, nameof(page.Appearing));
		return page;
	}
}