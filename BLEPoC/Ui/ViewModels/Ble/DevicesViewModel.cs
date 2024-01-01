using System.Diagnostics;

using BLEPoC.Ui.Models.Collection;
using BLEPoC.Ui.Models.DisplayItems;

using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;


namespace BLEPoC.Ui.ViewModels.Ble;

internal class BondedDevicesViewModel : DevicesViewModel
{
	internal BondedDevicesViewModel()
	{
		var adapter = CrossBluetoothLE.Current.Adapter;
		Devices = adapter.BondedDevices;
		Populate();
		adapter.DeviceBondStateChanged += (_, bondStateChangedEventArgs) =>
		{
			Trace.WriteLine($"{nameof(adapter.DeviceBondStateChanged)} - {nameof(bondStateChangedEventArgs.Address)}:{bondStateChangedEventArgs.Address} {nameof(bondStateChangedEventArgs.State)}:{bondStateChangedEventArgs.State}");
			Populate();
		};
		Items.CollectionChanged += (_, notifyCollectionChangedEventArgs) =>
		{
			Trace.WriteLine($"{nameof(Items.CollectionChanged)}");
			Trace.WriteLine($"{nameof(notifyCollectionChangedEventArgs.Action)}:{notifyCollectionChangedEventArgs.Action}");
		};
	}
}

internal class ConnectedDevicesViewModel : DevicesViewModel
{
	internal ConnectedDevicesViewModel()
	{
		var adapter = CrossBluetoothLE.Current.Adapter;
		Devices = adapter.ConnectedDevices;
		Populate();
		adapter.DeviceConnected += (_, args) =>
		{
			Trace.WriteLine($"{nameof(adapter.DeviceConnected)} : {args.Device.Id}");
			Populate();
		};
		adapter.DeviceDisconnected += (_, args) =>
		{
			Trace.WriteLine($"{nameof(adapter.DeviceDisconnected)} : {args.Device.Id}");
			Populate();
		};
		adapter.DeviceConnectionLost += (_, args) =>
		{
			Trace.WriteLine($"{nameof(adapter.DeviceConnectionLost)} : {args.ErrorMessage}");
			Populate();
		};
		adapter.DeviceConnectionError += (_, args) =>
		{
			Trace.WriteLine($"{nameof(adapter.DeviceConnectionError)} : {args.ErrorMessage}");
			Populate();
		};
	}
}

internal abstract class DevicesViewModel : CollectionViewModel
{
	protected IReadOnlyList<IDevice> Devices { private protected get; init; }

	protected async void Populate()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			// ToDo implement a check for changes and +/- only changes
			Items.Clear();

			if (Devices != null)
			{
				foreach (var device in Devices)
					Items.Add(new Summary { Primary = device.Name, Secondary = $"{device.State}" });

				if (!Items.Any())
					Items.Add(new Summary { Primary = "No Devices" });
			}
			else
				Items.Add(new Summary { Primary = "Devices Not Supported" });
		});
	}
}