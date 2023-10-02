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

	private void SumFuc(object b) { Trace.WriteLine($"sum fuc is {b}"); }

	private void CreateFakeItems()
	{
		_displayItems.Add(new DisplayItemFolder { Primary = "folder", Secondary = "Africa & Asia", ItemCommand = new Command<object>(SumFuc) });

		_displayItems.Add(new DisplayItemBinary { Primary = "binary", Secondary = "Central & South America", ItemCommand = new Command<object>(SumFuc) });

		var defaultMultiItem = new DisplayItemBase {Primary = "teo"};
		_displayItems.Add(new DisplayItemMulti { Primary = "Blue Monkey", Secondary = "Central and East Africa", SelectedItem = defaultMultiItem, ItemCommand = new Command<object>(SumFuc),
			AvailableItems = new List<IDisplayItem>
			{
				new DisplayItemBase {Primary = "one"},
				defaultMultiItem,
				new DisplayItemBase {Primary = "tree"},
			}
		});

		//_displayItems.Add(new DisplayItemTime { Primary = "Squirrel Monkey", Secondary = "Central & South America", ItemCommand = new Command<TimeSpan>(SumFuc), Time = new TimeSpan(0, 8, 3) });

		var digits = new List<int> { 1, 2, 3 };
		_displayItems.Add(new DisplayItemNumbers(digits, digits) { Primary = "Golden Lion Tamarin", Secondary = "Brazil", Value = 22, ItemCommand = new Command<object>(SumFuc) });

		Items = new ObservableCollection<IDisplayItem>(_displayItems);
	}

	private void FilterItems(string filter)
	{
		var filteredItems = _displayItems.Where(item => item.Primary.ToLower().Contains(filter.ToLower())).ToList();
		foreach (var item in _displayItems)
		{
			if (!filteredItems.Contains(item))
			{
				Items.Remove(item);
			}
			else
			{
				if (!Items.Contains(item))
				{
					Items.Add(item);
				}
			}
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
