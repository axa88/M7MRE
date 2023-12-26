namespace BLEPoC.Ui.Models.DisplayItems;

internal class MultiIo : DisplayItem
{
	private IDisplayItem _selectedItem;

	internal MultiIo(IList<IDisplayItem> listItems)
	{
		if (listItems == null || !listItems.Any())
			throw new ArgumentNullException(nameof(listItems));

		AvailableItems = listItems;
	}
	public IList<IDisplayItem> AvailableItems { get; set; }

	public IDisplayItem SelectedItem
	{
		get => _selectedItem;
		set
		{
			_selectedItem = value;
			ItemCommand?.Execute(_selectedItem.Primary);
		}
	}
}
