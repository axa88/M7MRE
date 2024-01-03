using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace BLEPoC.Ui.Models.Collection.Items;

internal class TextIo : CollectionItem, INotifyPropertyChanged
{
	private string _text;

	internal TextIo() => SubmitCommand = new Command(() => ItemCommand.Execute(Text), () => !string.IsNullOrWhiteSpace(Text));

	public string Text
	{
		get => _text;
		set
		{
			if (SetField(ref _text, value))
				((Command)SubmitCommand).ChangeCanExecute();
		}
	}

	public Color TextColor { get; set; }
	public string TextPlaceHolder { get; set; }
	public Keyboard Keyboard { get; set; }
	public int MaxLength { get; set; }

	public ICommand SubmitCommand { get; init; }

	#region INotifyPropertyChanged

	public event PropertyChangedEventHandler PropertyChanged;

	private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(field, value))
			return false;

		field = value;
		OnPropertyChanged(propertyName);
		return true;
	}

	private void OnPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

	#endregion
}
