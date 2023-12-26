using System.Diagnostics;

using BLEPoC.Ui.Models;
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
		adapter.DeviceBondStateChanged += (sender, bondStateChangedEventArgs) => Populate();
		Items.CollectionChanged += (sender, args) => { };
	}
}

internal class ConnectedDevicesViewModel : DevicesViewModel
{
	internal ConnectedDevicesViewModel()
	{
		var adapter = CrossBluetoothLE.Current.Adapter;
		Devices = adapter.ConnectedDevices;
		Populate();
		adapter.DeviceConnected += (sender, args) =>
		{
			Trace.WriteLine($"{nameof(adapter.DeviceConnected)} : {args.Device.Id}");
			Populate();
		};
		adapter.DeviceDisconnected += (sender, args) =>
		{
			Trace.WriteLine($"{nameof(adapter.DeviceDisconnected)} : {args.Device.Id}");
			Populate();
		};
		adapter.DeviceConnectionLost += (sender, args) =>
		{
			Trace.WriteLine($"{nameof(adapter.DeviceConnectionLost)} : {args.ErrorMessage}");
			Populate();
		};
		adapter.DeviceConnectionError += (sender, args) =>
		{
			Trace.WriteLine($"{nameof(adapter.DeviceConnectionError)} : {args.ErrorMessage}");
			Populate();
		};
	}
}

internal abstract class DevicesViewModel : CollectionBaseModel
{
	protected IReadOnlyList<IDevice> Devices { private protected get; init; }

	protected void Populate()
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
	}
}