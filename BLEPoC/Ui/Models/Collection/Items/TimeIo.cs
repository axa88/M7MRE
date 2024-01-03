namespace BLEPoC.Ui.Models.Collection.Items;

internal class TimeIo : CollectionItem
{
	private TimeSpan _time;

	public TimeSpan Time
	{
		get => _time;
		set
		{
			_time = value;
			ItemCommand.Execute(_time);
		}
	}
}
