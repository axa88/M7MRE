using System.Collections.ObjectModel;

using BLEPoC.Ui.Models.DisplayItems;


namespace BLEPoC.Ui.Models;

internal class CollectionBaseModel
{
	// ReSharper disable once MemberCanBeProtected.Global
	// ReSharper disable once MemberCanBePrivate.Global
	public ObservableCollection<IDisplayItem> Items { get; protected set; } = [];
}
