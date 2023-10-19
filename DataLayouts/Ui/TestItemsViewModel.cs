using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace DataLayouts.Ui;

public class TestItemsViewModel : INotifyPropertyChanged
{
	private readonly IList<IDisplayItem> _displayItems;
	private int _selectionCount = 1;

	public TestItemsViewModel()
	{
		_displayItems = new List<IDisplayItem>();
		CreateFakeItems(); // mock data
	}

	public IDisplayItem SelectedDisplayItem { get; set; }

	// ReSharper disable MemberCanBePrivate.Global
	public ObservableCollection<IDisplayItem> Items { get; private set; }
	public ObservableCollection<object> SelectedMonkeys { get; set; }
	// ReSharper restore MemberCanBePrivate.Global

	public string SelectedMonkeyMessage { get; private set; }

	public ICommand FilterCommand => new Command<string>(FilterItems);
	public ICommand MonkeySelectionChangedCommand => new Command(MonkeySelectionChanged);
	public ICommand DeleteCommand => new Command<IDisplayItem>(RemoveMonkey);

	private void CreateFakeItems()
	{
		_displayItems.Add(new DisplayItemFolder { Primary = "folder", Secondary = "Africa & Asia", ItemCommand = new Command<object>(SumFuc) });

		_displayItems.Add(new DisplayItemBinary { Primary = "binary", Secondary = "Central & South America", ItemCommand = new Command<object>(SumFuc) });

		var defaultMultiItem = new DisplayItemBase {Primary = "teo"};
		var listItems = new List<IDisplayItem> { new DisplayItemBase { Primary = "one" }, defaultMultiItem, new DisplayItemBase { Primary = "tree" }, };
		_displayItems.Add(new DisplayItemMulti(listItems) { Primary = "Blue Monkey", Secondary = "Central and East Africa", SelectedItem = defaultMultiItem, ItemCommand = new Command<object>(SumFuc) });

		_displayItems.Add(new DisplayItemTime { Primary = "Squirrel Monkey", Secondary = "Central & South America", ItemCommand = new Command<object>(SumFuc), Time = new TimeSpan(0, 8, 3) });

		var digits = new List<int> { 0, 1, 2 };
		_displayItems.Add(new DisplayItemDigits(new[] { digits, digits, digits }) { Primary = "custom whole", Secondary = "these should be whole", ItemCommand = new Command<object>(SumFuc) });

		var hex = new List<int> { 0x0, 0x1, 0x2, 0xa, 0xb, 0xc };
		_displayItems.Add(new DisplayItemDigits(new[] { hex, hex }, "X") { Primary = "custom hex", Secondary = "these number should be base 16", ItemCommand = new Command<object>(SumFuc) });

		_displayItems.Add(new DisplayItemEntry { Primary = "An Entry", Secondary = "2nd", TextPlaceHolder = "input goes here", MaxLength = 3, Keyboard = Keyboard.Numeric, ItemCommand = new Command<object>(SumFuc)});

		_displayItems.Add(new DisplayItemDigits(4, "x") { Primary = "built in hex", Value = 0xFF, ItemCommand = new Command<object>(SumFuc)});
		_displayItems.Add(new DisplayItemDigits(4, "X") { Primary = "built in hex", Value = 0xFF, ItemCommand = new Command<object>(SumFuc)});
		_displayItems.Add(new DisplayItemDigits(4, "B") { Primary = "built in binary", Value = 0b_1100, ItemCommand = new Command<object>(SumFuc)});
		_displayItems.Add(new DisplayItemDigits(4, "D") { Primary = "built in whole", Value = 1234, ItemCommand = new Command<object>(SumFuc)});

		Items = new ObservableCollection<IDisplayItem>(_displayItems);
	}

	private static void SumFuc(object b) { Trace.WriteLine($"sum fuc is {b}"); }

	private void FilterItems(string filter)
	{
		var filteredItems = _displayItems.Where(item => item.Primary.ToLower().Contains(filter.ToLower())).ToList();
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
		SelectedMonkeyMessage = $"Selection {_selectionCount}: {SelectedDisplayItem.Primary}";
		OnPropertyChanged(nameof(SelectedMonkeyMessage));
		_selectionCount++;
	}

	private void RemoveMonkey(IDisplayItem displayItem) => Items.Remove(displayItem);

	#region INotifyPropertyChanged

	public event PropertyChangedEventHandler PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	#endregion
}
