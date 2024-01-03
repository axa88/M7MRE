using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
//using System.Windows.Input;

using BLEPoC.Ui.Models.Collection;
using BLEPoC.Ui.Models.Collection.Items;


namespace BLEPoC.Ui.ViewModels.Tests;

internal class TestControlsCollectionViewModel<T> : ItemCollection<T>, INotifyPropertyChanged where T : ICollectionItem
{
	private readonly IList<T> _displayItems = new List<T>();
	//private int _selectionCount = 1;

	internal TestControlsCollectionViewModel()
	{
		CreateFakeItems(); // mock data
	}

	//public ICollectionItem SelectedCollectionItem { get; set; }

	// ReSharper disable MemberCanBePrivate.Global
	//public ObservableCollection<object> SelectedMonkeys { get; set; }
	// ReSharper restore MemberCanBePrivate.Global

	//public string SelectedMonkeyMessage { get; private set; }

	/*public ICommand FilterCommand => new Command<string>(FilterItems);
	public ICommand MonkeySelectionChangedCommand => new Command(MonkeySelectionChanged);
	public ICommand DeleteCommand => new Command<ICollectionItem>(RemoveMonkey);*/

	private void CreateFakeItems()
	{
		_displayItems.Add((T)(ICollectionItem)new Summary { Primary = "folder", Secondary = "Africa & Asia", ItemCommand = new Command<object>(SumFuc) });

		_displayItems.Add((T)(ICollectionItem)new Summary { Primary = "binary", Secondary = "Central & South America", ItemCommand = new Command<object>(SumFuc) });


		var defaultMultiItem = (T)(ICollectionItem)new CollectionItem { Primary = "teo" };

		var listItems = new List<T> { (T)(ICollectionItem)new Summary { Primary = "one" }, defaultMultiItem, (T)(ICollectionItem)new Summary { Primary = "tree" } };

		_displayItems.Add((T)(ICollectionItem)new MultiIo<T>(listItems) { Primary = "Blue Monkey", Secondary = "Central and East Africa", SelectedItem = defaultMultiItem as CollectionItem, ItemCommand = new Command<object>(SumFuc) });

		//_displayItems.Add((T)(ICollectionItem)new MultiIo(listItems) { Primary = "Blue Monkey", Secondary = "Central and East Africa", SelectedItem = defaultMultiItem, ItemCommand = new Command<object>(SumFuc) });


		_displayItems.Add((T)(ICollectionItem)new TimeIo { Primary = "Squirrel Monkey", Secondary = "Central & South America", ItemCommand = new Command<object>(SumFuc), Time = new TimeSpan(0, 8, 3) });

		var digits = new List<int> { 0, 1, 2 };
		_displayItems.Add((T)(ICollectionItem)new DigitIo(new[] { digits, digits, digits }) { Primary = "custom whole", Secondary = "these should be whole", ItemCommand = new Command<object>(SumFuc) });

		var hex = new List<int> { 0x0, 0x1, 0x2, 0xa, 0xb, 0xc };
		_displayItems.Add((T)(ICollectionItem)new DigitIo(new[] { hex, hex }, "X") { Primary = "custom hex", Secondary = "these number should be base 16", ItemCommand = new Command<object>(SumFuc) });

		_displayItems.Add((T)(ICollectionItem)new TextIo { Primary = "An Entry", Secondary = "2nd", TextPlaceHolder = "input goes here", MaxLength = 3, Keyboard = Keyboard.Numeric, ItemCommand = new Command<object>(SumFuc) });

		_displayItems.Add((T)(ICollectionItem)new DigitIo(4, "x") { Primary = "built in hex", Value = 0xFF, ItemCommand = new Command<object>(SumFuc) });
		_displayItems.Add((T)(ICollectionItem)new DigitIo(4, "X") { Primary = "built in hex", Value = 0xFF, ItemCommand = new Command<object>(SumFuc) });
		_displayItems.Add((T)(ICollectionItem)new DigitIo(4, "B") { Primary = "built in binary", Value = 0b_1100, ItemCommand = new Command<object>(SumFuc) });
		_displayItems.Add((T)(ICollectionItem)new DigitIo(4, "D") { Primary = "built in whole", Value = 1234, ItemCommand = new Command<object>(SumFuc) });

		Items = new ObservableCollection<T>(_displayItems);
		//Items = new ObservableCollection<T>((IList<T>)_displayItems);
	}

	private static void SumFuc(object b) { Trace.WriteLine($"sum fuc is {b}"); }

	/*private void FilterItems(string filter)
	{
		var filteredItems = _displayItems.Where(item => item.Primary.Contains(filter, StringComparison.CurrentCultureIgnoreCase)).ToList();
		foreach (var item in _displayItems)
		{
			if (!filteredItems.Contains(item))
				Items.Remove(item);
			else if (!Items.Contains(item))
				Items.Add(item);
		}
	}

	private void MonkeySelectionChanged()
	{
		SelectedMonkeyMessage = $"Selection {_selectionCount}: {SelectedCollectionItem.Primary}";
		OnPropertyChanged(nameof(SelectedMonkeyMessage));
		_selectionCount++;
	}

	private void RemoveMonkey(ICollectionItem collectionItem) => Items.Remove(collectionItem);*/

	#region INotifyPropertyChanged

	public event PropertyChangedEventHandler PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	#endregion
}
