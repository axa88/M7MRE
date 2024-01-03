using System.Diagnostics;

using BLEPoC.Ui.Models.Collection;
using BLEPoC.Ui.Models.DisplayItems;

using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;


namespace BLEPoC.Ui.ViewModels.Ble;

internal abstract class DevicesViewModel : LifeCycleTriggeredCollectionDisplayItems
{
	protected DevicesViewModel()
	{
		Adapter = CrossBluetoothLE.Current.Adapter;

		// not likely useful, perhaps only useful when being changed by view, this will alert model
		Items.CollectionChanged += (_, notifyCollectionChangedEventArgs) =>
		{
			Trace.WriteLine($"{GetType().Name}:{nameof(Items.CollectionChanged)}");
			Trace.WriteLine($"{nameof(notifyCollectionChangedEventArgs.Action)}:{notifyCollectionChangedEventArgs.Action}");
		};
	}

	protected IAdapter Adapter { get; }

	protected IReadOnlyList<IDevice> Devices { get; set; }

	#region Overrides of LifeCycleTriggeredCollectionDisplayItems

	protected internal override async void PopulateCollection(string caller, EventArgs eventArgs)
	{
		Trace.WriteLine(caller);

		switch (eventArgs)
		{
			case DeviceBondStateChangedEventArgs deviceBondStateChangedEventArgs:
				Trace.WriteLine($"{deviceBondStateChangedEventArgs.State} {deviceBondStateChangedEventArgs.Address} {deviceBondStateChangedEventArgs.Device.Id}");
				break;
			case DeviceErrorEventArgs deviceErrorEventArgs:
				Trace.WriteLine($"{deviceErrorEventArgs.ErrorMessage} : {deviceErrorEventArgs.Device.Id}");
				break;
			case DeviceEventArgs deviceEventArgs:
				Trace.WriteLine($"{deviceEventArgs.Device.Id}");
				break;
		}

		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			UpdateFromSource();

			// ToDo implement a check for changes and +/- only changes
			// ToDo remove DeviceType after adding some kind of page title
			Items.Clear();

			if (Devices != null)
			{
				foreach (var device in Devices)
					Items.Add(new Summary { Primary = device.Name, Secondary = $"{device.State}" });

				if (!Items.Any())
					Items.Add(new Summary { Primary = $"No {DeviceType()}" });
			}
			else
				Items.Add(new Summary { Primary = $"{DeviceType()} not Supported on this platform" });
		});

		#pragma warning disable IDE0061 // Use block body for local function
		#pragma warning disable CA2208 // Instantiate argument exceptions correctly
		string DeviceType() => this switch
		{
			BondedDevicesViewModel => nameof(IAdapter.BondedDevices),
			ConnectedDevicesViewModel => nameof(IAdapter.ConnectedDevices),
			_ => throw new ArgumentOutOfRangeException()
		};
		#pragma warning restore CA2208 // Instantiate argument exceptions correctly
		#pragma warning restore IDE0061 // Use block body for local function
	}

	#endregion

	/// <summary>
	/// Apparently IReadOnlyCollection of BT devices needs to be reread for whatever reason
	/// </summary>
	private protected abstract void UpdateFromSource();
}


internal abstract class LifeCycleTriggeredCollectionDisplayItems : DisplayItemCollection
{
	protected internal abstract void PopulateCollection(string caller, EventArgs eventArgs);
}


internal static class LifeCycleTriggeredCollectionExtensions
{
	/// <summary>
	/// Allows fluent instantiation of a new window/page/viewModel
	/// </summary>
	/// <param name="window">Activated window</param>
	/// <param name="viewModel">Collection View Model in need of the window trigger re-population</param>
	/// <returns></returns>
	internal static Window EnableUpdateOnWindowActivation(this Window window, LifeCycleTriggeredCollectionDisplayItems viewModel)
	{
		window.Activated += (_, eventArgs) => viewModel.PopulateCollection(nameof(window.Activated), eventArgs);
		return window;
	}
}