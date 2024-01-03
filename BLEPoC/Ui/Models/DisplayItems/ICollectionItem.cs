namespace BLEPoC.Ui.Models.DisplayItems
{
	internal interface ICollectionItem
	{
		public string Primary { get; set; }
		public string Secondary { get; set; }
		public Command<object> ItemCommand { get; set; }
	}


	internal class CollectionItem : ICollectionItem
	{
		public string Primary { get; set; }
		public string Secondary { get; set; }
		public Command<object> ItemCommand { get; set; }
	}
}
