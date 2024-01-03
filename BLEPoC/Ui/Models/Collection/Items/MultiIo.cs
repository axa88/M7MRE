namespace BLEPoC.Ui.Models.Collection.Items;

internal class MultiIo<T> : CollectionItem where T: ICollectionItem
{
	private CollectionItem _selectedItem;

	internal MultiIo(List<T> listItems)
	{
		#pragma warning disable CA1860
		if (listItems == null || !listItems.Any())
		#pragma warning restore CA1860
			throw new ArgumentNullException(nameof(listItems));

		AvailableItems = listItems;
	}
	public List<T> AvailableItems { get; set; }

	public CollectionItem SelectedItem
	{
		get => _selectedItem;
		set
		{
			_selectedItem = value;
			ItemCommand?.Execute(_selectedItem.Primary);
		}
	}
}
