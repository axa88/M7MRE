namespace BLEPoC.Ui.Models.DisplayItems
{
	internal interface IDisplayItem
	{
		public string Primary { get; set; }
		public string Secondary { get; set; }
		public Command<object> ItemCommand { get; set; }
	}


	internal class DisplayItem : IDisplayItem
	{
		public string Primary { get; set; }
		public string Secondary { get; set; }
		public Command<object> ItemCommand { get; set; }
	}
}
