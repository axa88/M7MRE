using BLEPoC.Ui.Models.Collection;
using BLEPoC.Ui.Models.Collection.Items;
using BLEPoC.Ui.ViewModels.Ble;
using BLEPoC.Utility;

using static System.Reflection.MethodBase;


namespace BLEPoC.Ui.Pages.Controls;

internal class NavigationPageCollection<T> : NavigationPage where T : ICollectionItem, new()
{
	private readonly CollectionPageWButton<T> _secondPageWButton = new(new BondedBtDeviceCollectionViewModel<T>());

	internal NavigationPageCollection(ItemCollection<T> itemCollection, string title = null)
	{
		_ = new LifeCycleTracing(this, title);

		PushAsync(new CollectionPageWButton<T>(itemCollection, PushPage));
	}


	private async void PushPage() => await PushAsync(_secondPageWButton);

	internal event EventHandler<TraceEventArgs> BackButtonPressing;

	#region Overrides of NavigationPage

	protected override bool OnBackButtonPressed()
	{
		OnBackButtonPressing(GetCurrentMethod()?.Name);
		return base.OnBackButtonPressed();
	}

	#endregion

	private void OnBackButtonPressing(string originEvent) => BackButtonPressing?.Invoke(this, new(originEvent));
}
