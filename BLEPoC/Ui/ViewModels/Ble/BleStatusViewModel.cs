using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;


namespace BLEPoC.Ui.ViewModels.Ble;

internal class BleStatusViewModel : INotifyPropertyChanged
{
	private readonly IBluetoothLE _ble;

	internal BleStatusViewModel()
	{
		_ble = CrossBluetoothLE.Current;

		Trace.WriteLine($"{nameof(_ble.IsAvailable)}:{_ble.IsAvailable}");
		Trace.WriteLine($"{nameof(_ble.IsOn)}:{_ble.IsOn}");
		Trace.WriteLine($"{nameof(_ble.State)}:{_ble.State}");
		Trace.WriteLine($"{nameof(_ble.Adapter.SupportsCodedPHY)}:{_ble.Adapter.SupportsCodedPHY()}");
		Trace.WriteLine($"{nameof(_ble.Adapter.SupportsExtendedAdvertising)}:{_ble.Adapter.SupportsExtendedAdvertising()}");

		if (_ble.Adapter.BondedDevices != null)
		{
			foreach (var bondedDevice in _ble.Adapter.BondedDevices)
			{
				Trace.WriteLine($"{nameof(bondedDevice.Name)}:{bondedDevice.Name}");
				Trace.WriteLine($"{nameof(bondedDevice.Id)}:{bondedDevice.Id}");
				Trace.WriteLine($"{nameof(bondedDevice.Rssi)}:{bondedDevice.Rssi}");
				Trace.WriteLine($"{nameof(bondedDevice.BondState)}:{bondedDevice.BondState}");
				Trace.WriteLine($"{nameof(bondedDevice.State)}:{bondedDevice.State}");
				Trace.WriteLine($"{nameof(bondedDevice.SupportsIsConnectable)}:{bondedDevice.SupportsIsConnectable}");
				Trace.WriteLine($"{nameof(bondedDevice.IsConnectable)}:{bondedDevice.IsConnectable}");
			}

			_ble.Adapter.DeviceBondStateChanged += (_, deviceBondStateChangedEventArgs) =>
			{
				Trace.WriteLine($"event: {nameof(_ble.Adapter.DeviceBondStateChanged)}");
				Trace.WriteLine($"{nameof(deviceBondStateChangedEventArgs.Address)}:{deviceBondStateChangedEventArgs.Address}");
				Trace.WriteLine($"{nameof(deviceBondStateChangedEventArgs.State)}:{deviceBondStateChangedEventArgs.State}");

				Trace.WriteLine($"{nameof(deviceBondStateChangedEventArgs.Device.Name)}:{deviceBondStateChangedEventArgs.Device.Name}");
				Trace.WriteLine($"{nameof(deviceBondStateChangedEventArgs.Device.Id)}:{deviceBondStateChangedEventArgs.Device.Id}");
				Trace.WriteLine($"{nameof(deviceBondStateChangedEventArgs.Device.Rssi)}:{deviceBondStateChangedEventArgs.Device.Rssi}");
				Trace.WriteLine($"{nameof(deviceBondStateChangedEventArgs.Device.BondState)}:{deviceBondStateChangedEventArgs.Device.BondState}");
				Trace.WriteLine($"{nameof(deviceBondStateChangedEventArgs.Device.State)}:{deviceBondStateChangedEventArgs.Device.State}");
				Trace.WriteLine($"{nameof(deviceBondStateChangedEventArgs.Device.SupportsIsConnectable)}:{deviceBondStateChangedEventArgs.Device.SupportsIsConnectable}");
				Trace.WriteLine($"{nameof(deviceBondStateChangedEventArgs.Device.IsConnectable)}:{deviceBondStateChangedEventArgs.Device.IsConnectable}");
			};
		}

		if (_ble.Adapter.ConnectedDevices != null)
		{
			foreach (var connectedDevice in _ble.Adapter.ConnectedDevices)
			{
				Trace.WriteLine($"{nameof(connectedDevice.Name)}:{connectedDevice.Name}");
				Trace.WriteLine($"{nameof(connectedDevice.Id)}:{connectedDevice.Id}");
				Trace.WriteLine($"{nameof(connectedDevice.Rssi)}:{connectedDevice.Rssi}");
				Trace.WriteLine($"{nameof(connectedDevice.BondState)}:{connectedDevice.BondState}");
				Trace.WriteLine($"{nameof(connectedDevice.State)}:{connectedDevice.State}");
				Trace.WriteLine($"{nameof(connectedDevice.SupportsIsConnectable)}:{connectedDevice.SupportsIsConnectable}");
				Trace.WriteLine($"{nameof(connectedDevice.IsConnectable)}:{connectedDevice.IsConnectable}");
			}
		}

		_ble.StateChanged += (_, args) =>
		{
			Trace.WriteLine($"event: {nameof(_ble.StateChanged)}");

			Trace.WriteLine($"The bluetooth state changed to {args.NewState}");
			OnPropertyChanged(nameof(BleIsAvailable));
			OnPropertyChanged(nameof(BleIsOn));
			OnPropertyChanged(nameof(BleState));
		};
	}

	public string BleIsAvailable => $"{nameof(BleIsAvailable)} : {CrossBluetoothLE.Current.IsAvailable}";

	public string BleIsOn => $"{nameof(BleIsOn)} : {CrossBluetoothLE.Current.IsOn}";
	public ICommand BleOnCommand => new Command(() =>
	{

	});

	public string BleState => $"{nameof(BleState)} : {CrossBluetoothLE.Current.State}";

	public string BleSupportsCodedPhy => $"{nameof(BleSupportsCodedPhy)} : {CrossBluetoothLE.Current.Adapter.SupportsCodedPHY()}";
	public string BleSupportsExtendedAdvertising => $"{nameof(BleSupportsExtendedAdvertising)} : {CrossBluetoothLE.Current.Adapter.SupportsExtendedAdvertising()}";

	public async void StartScan()
	{
		List<IDevice> deviceList = [];
		_ble.Adapter.DeviceDiscovered += (s, deviceEventArgs) => deviceList.Add(deviceEventArgs.Device);
		await _ble.Adapter.StartScanningForDevicesAsync();
	}

	#region INotifyPropertyChanged

	public event PropertyChangedEventHandler PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

	private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(field, value))
			return false;

		field = value;
		OnPropertyChanged(propertyName);
		return true;
	}

	#endregion
}

// you have to use the Adapter "DeviceAdvertised" event after discovering a device to get further advertising data