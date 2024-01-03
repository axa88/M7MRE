namespace BLEPoC.Ui.Models.Collection.Items;

internal class BinaryIo : CollectionItem
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
