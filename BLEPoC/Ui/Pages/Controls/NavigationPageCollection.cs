using BLEPoC.Ui.Models.Collection;
using BLEPoC.Ui.Models.Collection.Items;
using BLEPoC.Ui.ViewModels.Ble;
using BLEPoC.Utility;

using static System.Reflection.MethodBase;


namespace BLEPoC.Ui.Pages.Controls;

internal class NavigationPageCollection<T> : NavigationPage where T : ICollectionItem, new()
{
	private readonly CollectionPageWButton<T> _secondPageWButton;

	internal NavigationPageCollection(ItemCollection<T> itemCollection, string title = null)
	{
		BondedBtDeviceCollectionViewModel<T> bondedBtDeviceCollectionViewModel = new();
		_secondPageWButton = new(bondedBtDeviceCollectionViewModel);
		PushAsync(new CollectionPageWButton<T>(itemCollection, PushPage));

		_ = new LifeCycleTracing(this, title);
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

	private void OnBackButtonPressing(string originEvent) => BackButtonPressing?.Invoke(this, new TraceEventArgs(originEvent));
}
