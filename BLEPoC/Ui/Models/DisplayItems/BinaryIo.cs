namespace BLEPoC.Ui.Models.DisplayItems;

internal class BinaryIo : DisplayItem
{
	private bool _isOn;

	public bool IsOn
	{
		get => _isOn;
		set
		{
			_isOn = value;
			ItemCommand?.Execute(_isOn);
		}
	}
}
