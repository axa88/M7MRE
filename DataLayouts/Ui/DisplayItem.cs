using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace DataLayouts.Ui;

public interface IDisplayItem
{
	public string Primary { get; set; }
	public string Secondary { get; set; }
	public Command<object> ItemCommand { get; set; }
}


public class DisplayItemBase : IDisplayItem
{
	public string Primary { get; set; }
	public string Secondary { get; set; }
	public Command<object> ItemCommand { get; set; }
}


public class DisplayItemFolder : DisplayItemBase { }


public class DisplayItemBinary : DisplayItemBase
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


public class DisplayItemMulti : DisplayItemBase
{
	private IDisplayItem _selectedItem;

	public IList<IDisplayItem> AvailableItems { get; set; }

	public IDisplayItem SelectedItem
	{
		get => _selectedItem;
		set
		{
			_selectedItem = value;
			ItemCommand?.Execute(_selectedItem.Primary);
		}
	}
}


public class DisplayItemTime : DisplayItemBase
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


public class DisplayItemNumbers : DisplayItemBase, INotifyPropertyChanged
{
	private const int _maxDigits = 6;
	private readonly int _displayedDigits;

	/// <summary>
	/// Base digit picker
	/// </summary>
	/// <param name="digits"> Jagged Array of up to 6 lists of digits to display in up to 6 separate pickers.
	/// Little Endian from right to left 0 = LSB, n = MSB </param>
	/// <exception cref="ArgumentNullException"></exception>
	public DisplayItemNumbers(IReadOnlyList<IReadOnlyList<int>> digits)
	{
		if (digits == null || !digits.Any())
			throw new ArgumentNullException(nameof(digits));

		for (var i = 0; i < digits.Count; i++)
		{
			if (digits[i] != null && digits[i].Any())
			{
				AvailableDigits[i] = digits[i].Select(j => new Digit(j)).ToArray();
				IsVisible[i] = true;
				_displayedDigits++;
			}
		}

		SubmitCommand = new Command(() => ItemCommand.Execute(Value), () => Value != 111 );

		// force change notification cuz change of array elements do not trigger property change notification. Therefore notification is possibly only sent on initial creation
		OnPropertyChanged(nameof(AvailableDigits));
	}

	public IDigit[][] AvailableDigits { get; init; } = new IDigit[_maxDigits][];

	public IDigit[] SelectedDigits { get; init; } = new IDigit[_maxDigits];

	public bool[] IsVisible { get; init; } = new bool[_maxDigits];

	public ICommand SubmitCommand { get; init; }

	public int Value
	{
		get
		{
			var value = 0;
			for (var i = 0; i < _displayedDigits; i++)
			{
				if (SelectedDigits[i] != null && SelectedDigits[i].RawValue != null)
					value += (int)SelectedDigits[i].RawValue * (int)Math.Pow(10, i);
			}
			return value;
		}
		set => UpdateValue(value);
	}

	private void UpdateValue(int value)
	{
		var s = value.ToString();
		if (s.Length > _displayedDigits)
			throw new ArgumentOutOfRangeException(nameof(UpdateValue), "too many digits");

		var reversed = s.Reverse().ToArray();
		for (var i = 0; i < reversed.Length; i++)
		{
			try { SelectedDigits[i] = AvailableDigits[i].First(item => item.DisplayValue == $"{reversed[i]}"); }
			catch { throw new ArgumentOutOfRangeException($"{nameof(AvailableDigits)} does not contain {reversed[i]}"); }
		}
	}

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

	private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	#endregion
}


public class DisplayItemEntry : DisplayItemBase, INotifyPropertyChanged
{
	private string _text;

	public DisplayItemEntry() => SubmitCommand = new Command(() => ItemCommand.Execute(Text), () => !string.IsNullOrWhiteSpace(Text));

	public string Text
	{
		get => _text;
		set
		{
			SetField(ref _text, value);
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

public interface IDigit
{
	public string DisplayValue { get; init; }
	public object RawValue { get; }
}


public class Digit : IDigit
{
	private readonly string _displayValue;

	public Digit(int value)
	{
		DisplayValue = value.ToString();
		if (DisplayValue.Length > 1)
			throw new ArgumentOutOfRangeException(nameof(Digit), $"Digit value {value} is too large to represent as a single character");
	}

	public string DisplayValue
	{
		get => _displayValue;
		init
		{
			_displayValue = value;
			RawValue = int.Parse(value);
		}
	}

	public object RawValue { get; private init; }
}
