namespace BLEPoC.Ui.Models.DisplayItems;

internal class MultiIo : CollectionItem
{
	private ICollectionItem _selectedItem;

	internal MultiIo(IList<ICollectionItem> listItems)
	{
		if (listItems == null || !listItems.Any())
			throw new ArgumentNullException(nameof(listItems));

		AvailableItems = listItems;
	}
	public IList<ICollectionItem> AvailableItems { get; set; }

	public ICollectionItem SelectedItem
	{
		get => _selectedItem;
		set
		{
			_selectedItem = value;
			ItemCommand?.Execute(_selectedItem.Primary);
		}
	}
}
