﻿using BLEPoC.Ui.Models.Collection;
using BLEPoC.Ui.Models.Collection.Items;


namespace BLEPoC.Ui.ViewModels.Tests;

internal class TestUpdatingViewModel<T> : ItemCollection<T> where T : ICollectionItem, new()
{
	private readonly List<(string, string)> _devices;
	private readonly Timer _collectionUpdateTimer;

	internal TestUpdatingViewModel()
	{
		_devices =
		[
			("first", "one"), ("second", "two"), ("third", "three"), ("fourth", "four"), ("fifth", "five"),
			("sixth", "six"), ("seventh", "seven"), ("eighth", "eight"), ("ninth", "nine"), ("tenth", "ten")
		];

		_collectionUpdateTimer = new Timer(_ => Populate(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
		Populate();

		Items.CollectionChanged += ItemsCollectionChanged;
	}

	private async void Populate()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			// ToDo implement a check for changes and +/- only changes
			Items.Clear();

			Random rnd = new();
			var enumerable = _devices.OrderBy(_ => rnd.Next()).Take(3);
			foreach (var valueTuple in enumerable)
				Items.Add((T)(ICollectionItem)new Summary { Primary = valueTuple.Item1, Secondary = valueTuple.Item2 });
		});

		_collectionUpdateTimer.Change(TimeSpan.FromSeconds(5), Timeout.InfiniteTimeSpan);
	}

	private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
	{
	}
}
