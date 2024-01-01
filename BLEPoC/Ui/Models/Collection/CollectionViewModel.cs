using System.Collections.ObjectModel;

using BLEPoC.Ui.Models.DisplayItems;


namespace BLEPoC.Ui.Models.Collection;

internal abstract class CollectionViewModel
{
	// ReSharper disable once MemberCanBeProtected.Global
	// ReSharper disable once MemberCanBePrivate.Global
	public ObservableCollection<IDisplayItem> Items { get; protected set; } = [];
}