using BLEPoC.Ui.Models.Collection.Items;

namespace BLEPoC.Ui.ViewModels.Ble;

internal class ConnectedBtDeviceCollectionViewModel<T> : BtDeviceCollectionViewModel<T> where T : ICollectionItem, new()
{
	internal ConnectedBtDeviceCollectionViewModel()
	{
		Adapter.DeviceConnected += (_, deviceEventArgs) => RequestCollectionUpdate(deviceEventArgs);
		Adapter.DeviceDisconnected += (_, deviceEventArgs) => RequestCollectionUpdate(deviceEventArgs);
		Adapter.DeviceConnectionLost += (_, deviceErrorEventArgs) => RequestCollectionUpdate(deviceErrorEventArgs);
		Adapter.DeviceConnectionError += (_, deviceErrorEventArgs) => RequestCollectionUpdate(deviceErrorEventArgs);
	}

	#region Overrides of BtDeviceCollectionViewModel

	private protected override void UpdateFromSource() => Devices = Adapter.ConnectedDevices;

	#endregion
}
