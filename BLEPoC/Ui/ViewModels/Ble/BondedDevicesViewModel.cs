using BLEPoC.Ui.Models.Collection.Items;


namespace BLEPoC.Ui.ViewModels.Ble;

internal class BondedDevicesViewModel<T> : DevicesViewModel<T> where T : ICollectionItem, new()
{
	internal BondedDevicesViewModel() => Adapter.DeviceBondStateChanged += (_, bondStateChangedEventArgs) => PopulateCollection(nameof(Adapter.DeviceBondStateChanged), bondStateChangedEventArgs);

	#region Overrides of DevicesViewModel

	private protected override void UpdateFromSource() => Devices = Adapter.BondedDevices;

	#endregion
}
