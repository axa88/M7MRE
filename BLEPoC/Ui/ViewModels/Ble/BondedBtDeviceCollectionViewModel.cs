using BLEPoC.Ui.Models.Collection.Items;


namespace BLEPoC.Ui.ViewModels.Ble;

internal class BondedBtDeviceCollectionViewModel<T> : BtDeviceCollectionViewModel<T> where T : ICollectionItem, new()
{
	internal BondedBtDeviceCollectionViewModel() => Adapter.DeviceBondStateChanged += (_, bondStateChangedEventArgs) => RequestCollectionUpdate(bondStateChangedEventArgs);

	#region Overrides of BtDeviceCollectionViewModel

	private protected override void UpdateFromSource() => Devices = Adapter.BondedDevices;

	#endregion
}
