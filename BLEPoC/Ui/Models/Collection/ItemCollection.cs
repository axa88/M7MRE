using System.Collections.ObjectModel;

using BLEPoC.Ui.Models.Collection.Items;


namespace BLEPoC.Ui.Models.Collection;

internal abstract class ItemCollection<T> where T : ICollectionItem
{
	// ReSharper disable once MemberCanBeProtected.Global
	// ReSharper disable once MemberCanBePrivate.Global
	public ObservableCollection<T> Items { get; protected set; } = [];
}