namespace BLEPoC.Ui.Models.DisplayItems;

internal class TimeIo : DisplayItem
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
