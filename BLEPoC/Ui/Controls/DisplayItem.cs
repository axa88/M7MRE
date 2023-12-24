using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace BLEPoC.Ui.Controls;

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

	public DisplayItemMulti(IList<IDisplayItem> listItems)
	{
		if (listItems == null || !listItems.Any())
			throw new ArgumentNullException(nameof(listItems));

		AvailableItems = listItems;
	}
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


public class DisplayItemDigits : DisplayItemBase, INotifyPropertyChanged
{
	private const int _maxDigits = 6;
	private readonly int _base;
	private readonly string _digitFormat;
	private readonly int _displayedDigits;

	public DisplayItemDigits(int count, string @base)
	{
		if (count > _maxDigits)
			throw new ArgumentOutOfRangeException(nameof(DisplayItemDigits), "too many digits to display");

		_digitFormat = @base;
		_base = string.Equals(@base, "B", StringComparison.OrdinalIgnoreCase) ? 2 : string.Equals(@base, "X", StringComparison.OrdinalIgnoreCase) ? 16 : 10;

		var digits = Enumerable.Range(0, _base).ToArray();

		for (var i = 0; i < count; i++)
		{
			AvailableDigits[i] = digits.Select(value => new Digit(value, i, _digitFormat, _base)).ToArray();
			IsVisible[i] = true;
			_displayedDigits++;
		}

		SubmitCommand = new Command(() => ItemCommand.Execute(Value), () => SelectedDigits.Take(_displayedDigits).All(digit => digit != null));

		// force change notification cuz change of array elements do not trigger property change notification. Therefore notification is possibly only sent on initial creation
		OnPropertyChanged(nameof(AvailableDigits));
	}

	/// <summary>
	/// Base digit picker
	/// </summary>
	/// <param name="digits"> Jagged Array of up to 6 lists of digits to display in up to 6 separate pickers.
	/// Little Endian from right to left 0 = LSB, n = MSB </param>
	/// <param name="base">set 2 for binary, 16 for hex, default base 10</param>
	/// <exception cref="ArgumentNullException"></exception>
	public DisplayItemDigits(IReadOnlyList<IReadOnlyCollection<int>> digits, string @base = "D")
	{
		if (digits == null || !digits.Any())
			throw new ArgumentNullException(nameof(DisplayItemDigits), $"custom digit needs a collection of numbers");

		_digitFormat = @base;
		_base = string.Equals(@base, "B", StringComparison.OrdinalIgnoreCase) ? 2 : string.Equals(@base, "X", StringComparison.OrdinalIgnoreCase) ? 16 : 10;

		for (var i = 0; i < digits.Count; i++)
		{
			if (digits[i] != null && digits[i].Any())
			{
				AvailableDigits[i] = digits[i].Select(value => new Digit(value, i, _digitFormat, _base)).ToArray();
				IsVisible[i] = true;
				_displayedDigits++;
			}
		}

		SubmitCommand = new Command(() => ItemCommand.Execute(Value), () => SelectedDigits.Take(_displayedDigits).All(digit => digit != null));

		// force change notification cuz change of array elements do not trigger property change notification. Therefore notification is possibly only sent on initial creation
		OnPropertyChanged(nameof(AvailableDigits));
	}

	public IDigit[][] AvailableDigits { get; init; } = new IDigit[_maxDigits][];

	public IDigit[] SelectedDigits { get; init; } = new IDigit[_maxDigits];

	// ReSharper disable once ValueParameterNotUsed
	public object SelectedDigitChange { set => ((Command)SubmitCommand).ChangeCanExecute(); } // only used to indicate target changed

	public bool[] IsVisible { get; init; } = new bool[_maxDigits];

	public ICommand SubmitCommand { get; init; }

	public int Value
	{
		get => SelectedDigits.Where(digit => digit != null).Sum(digit => digit.RawValue * (int)Math.Pow(_base, digit.Position));
		set
		{
			var s = _base == 2 ? Convert.ToString(value, _base) : value.ToString(_digitFormat); // int.ToString(format) "B" format is supported only for integral types and only on .NET 8+.

			if (s.Length > _displayedDigits)
				throw new ArgumentOutOfRangeException(GetType().Name, $"{nameof(Value)} has too many digits to display");

			s = s.PadLeft(_displayedDigits, '0');
			var c = s.Reverse().ToArray();

			for (var i = 0; i < s.Length; i++)
			{
				try { SelectedDigits[i] = AvailableDigits[i].First(item => item.DisplayValue == $"{c[i]}"); }
				catch { throw new ArgumentOutOfRangeException($"{GetType().Name}, {nameof(AvailableDigits)} does not contain {c[i]}"); }
			}
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


public interface IDigit
{
	public int RawValue { get; }
	public string DisplayValue { get; }

	public int Position { get; }
}


public class Digit : IDigit
{
	private readonly string _format;
	private readonly int _base;

	public Digit(int value, int position, string format, int @base)
	{
		if (@base == 2)
		{
			if (Convert.ToString(value, @base).Length > 1)
				throw new ArgumentOutOfRangeException(nameof(Digit), $"Digit value {value} is too large to be represented as a single character in base {@base}");
		}
		else
		{
			if (value.ToString(format).Length > 1) // int.ToString(format) "B" format is supported only for integral types and only on .NET 8+.
				throw new ArgumentOutOfRangeException(nameof(Digit), $"Digit value {value} is too large to be represented as a single character in base {@base}");
		}

		_format = format;
		_base = @base;
		RawValue = value;
		Position = position;
	}

	public int RawValue { get; }

	public string DisplayValue => _base == 2 ? Convert.ToString(RawValue, _base) : RawValue.ToString(_format); // int.ToString(format) "B" format is supported only for integral types and only on .NET 8+.

	public int Position { get; }
}
