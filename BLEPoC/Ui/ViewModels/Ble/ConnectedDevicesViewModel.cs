﻿using BLEPoC.Ui.Models.Collection.Items;

namespace BLEPoC.Ui.ViewModels.Ble;

internal class ConnectedDevicesViewModel<T> : DevicesViewModel<T> where T : ICollectionItem, new()
{
	internal ConnectedDevicesViewModel()
	{
		Adapter.DeviceConnected += (_, deviceEventArgs) => PopulateCollection(nameof(Adapter.DeviceConnected), deviceEventArgs);
		Adapter.DeviceDisconnected += (_, deviceEventArgs) => PopulateCollection(nameof(Adapter.DeviceDisconnected), deviceEventArgs);
		Adapter.DeviceConnectionLost += (_, deviceErrorEventArgs) => PopulateCollection(nameof(Adapter.DeviceConnectionLost),deviceErrorEventArgs);
		Adapter.DeviceConnectionError += (_, deviceErrorEventArgs) => PopulateCollection(nameof(Adapter.DeviceConnectionLost), deviceErrorEventArgs);
	}

	#region Overrides of DevicesViewModel

	private protected override void UpdateFromSource() => Devices = Adapter.ConnectedDevices;

	#endregion
}