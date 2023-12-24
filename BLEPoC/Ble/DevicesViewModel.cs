using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using BLEPoC.Ui.Controls;

using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;


namespace BLEPoC.Ble;

public class BondedDevicesViewModel : DevicesViewModel
{
	public BondedDevicesViewModel()
	{
		var adapter = CrossBluetoothLE.Current.Adapter;
		Devices = adapter.BondedDevices;
		Populate();
		adapter.DeviceBondStateChanged += (sender, args) => Populate();
	}
}

public class ConnectedDevicesViewModel : DevicesViewModel
{
	public ConnectedDevicesViewModel()
	{
		var adapter = CrossBluetoothLE.Current.Adapter;
		Devices = adapter.ConnectedDevices;
		Populate();
		adapter.DeviceConnected += (sender, args) => Populate();
		adapter.DeviceDisconnected += (sender, args) => Populate();
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

public class DevicesViewModel : INotifyPropertyChanged
{
	// ReSharper disable MemberCanBePrivate.Global
	public ObservableCollection<IDisplayItem> Items { get; } = [];
	// ReSharper restore MemberCanBePrivate.Global

	protected IReadOnlyList<IDevice> Devices { get; init; }

	protected void Populate()
	{
		// ToDo implement a check for changes and +/- only changes
		Items.Clear();

		if (Devices != null)
		{
			foreach (var device in Devices)
				Items.Add(new DisplayItemFolder { Primary = device.Name, Secondary = $"{device.State}" });

			if (!Items.Any())
				Items.Add(new DisplayItemFolder { Primary = "No Devices" });
		}
		else
			Items.Add(new DisplayItemFolder { Primary = "Devices Not Supported" });
	}

	#region INotifyPropertyChanged

	public event PropertyChangedEventHandler PropertyChanged;

	private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(field, value))
			return false;

		field = value;
		OnPropertyChanged(propertyName);
		return true;
	}

	private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	#endregion
}