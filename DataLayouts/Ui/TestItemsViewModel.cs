using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;


namespace DataLayouts.Ui;

public class TestItemsViewModel : INotifyPropertyChanged
{
	private readonly IList<IDisplayItem> _displayItems;

	public TestItemsViewModel()
	{
		_displayItems = new List<IDisplayItem>();
		CreateListItems(); // mock data
	}

	// ReSharper disable MemberCanBePrivate.Global
	public ObservableCollection<IDisplayItem> Items { get; private set; }
	// ReSharper restore MemberCanBePrivate.Global

	private void CreateListItems()
	{
		_displayItems.Add(new DisplayItemTime { Primary = "Squirrel Monkey", Secondary = "Central & South America", ItemCommand = new Command<object>(SumFuc), Time = new TimeSpan(0, 8, 3) });

		var digits = new List<int> { 0, 1, 2 };
		_displayItems.Add(new DisplayItemDigits(new[] { digits, digits, digits }) { Primary = "custom whole", Secondary = "these should be whole", ItemCommand = new Command<object>(SumFuc) });

		var hex = new List<int> { 0x0, 0x1, 0x2, 0xa, 0xb, 0xc };
		_displayItems.Add(new DisplayItemDigits(new[] { hex, hex }, "X") { Primary = "custom hex", Secondary = "these number should be base 16", ItemCommand = new Command<object>(SumFuc) });

		_displayItems.Add(new DisplayItemEntry { Primary = "An Entry", Secondary = "2nd", TextPlaceHolder = "input goes here", MaxLength = 3, Keyboard = Keyboard.Numeric, ItemCommand = new Command<object>(SumFuc)});

		_displayItems.Add(new DisplayItemDigits(4, "D") { Primary = "built in whole", Value = 1234, ItemCommand = new Command<object>(SumFuc)});

		Items = new ObservableCollection<IDisplayItem>(_displayItems);
	}

	private static void SumFuc(object b) { Trace.WriteLine($"sum fuc is {b}"); }

	#region INotifyPropertyChanged

	public event PropertyChangedEventHandler PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	#endregion
}
