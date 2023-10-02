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


public class DisplayItemNumbers : DisplayItemBase
{
	private const int _maxDigits = 6;
	private readonly int _displayedDigits;
	private readonly IList<INumberItem>[] _availableDigits = new IList<INumberItem>[_maxDigits];

	public DisplayItemNumbers(IList<int> digits0, IList<int> digits1 = null, IList<int> digits2 = null, IList<int> digits3 = null, IList<int> digits4 = null, IList<int> digits5 = null)
	{
		if (digits0 != null && digits0.Any())
		{
			AvailableDigits0 = new List<INumberItem>(digits0.Select(i => new DisplayDigit(i)));
			IsVisible[0] = true;
			_displayedDigits++;
		}

		if (digits1 != null && digits1.Any())
		{
			AvailableDigits1 = new List<INumberItem>(digits1.Select(i => new DisplayDigit(i)));
			IsVisible[1] = true;
			_displayedDigits++;
		}

		if (digits2 != null && digits2.Any())
		{
			AvailableDigits2 = new List<INumberItem>(digits2.Select(i => new DisplayDigit(i)));
			IsVisible[2] = true;
			_displayedDigits++;
		}

		if (digits3 != null && digits3.Any())
		{
			AvailableDigits3 = new List<INumberItem>(digits3.Select(i => new DisplayDigit(i)));
			IsVisible[3] = true;
			_displayedDigits++;
		}

		if (digits4 != null && digits4.Any())
		{
			AvailableDigits4 = new List<INumberItem>(digits4.Select(i => new DisplayDigit(i)));
			IsVisible[4] = true;
			_displayedDigits++;
		}

		if (digits5 != null && digits5.Any())
		{
			AvailableDigits5 = new List<INumberItem>(digits5.Select(i => new DisplayDigit(i)));
			IsVisible[5] = true;
			_displayedDigits++;
		}

		SubmitCommand = new Command(() => ItemCommand.Execute(Value));
	}

	#region Available
	public IList<INumberItem> AvailableDigits0 { get => _availableDigits[0]; set => _availableDigits[0] = value; }
	public IList<INumberItem> AvailableDigits1 { get => _availableDigits[1]; set => _availableDigits[1] = value; }
	public IList<INumberItem> AvailableDigits2 { get => _availableDigits[2]; set => _availableDigits[2] = value; }
	public IList<INumberItem> AvailableDigits3 { get => _availableDigits[3]; set => _availableDigits[3] = value; }
	public IList<INumberItem> AvailableDigits4 { get => _availableDigits[4]; set => _availableDigits[4] = value; }
	public IList<INumberItem> AvailableDigits5 { get => _availableDigits[5]; set => _availableDigits[5] = value; }
	#endregion

	public INumberItem[] SelectedDigits { get; init; } = new INumberItem[_maxDigits];

	public bool[] IsVisible { get; set; } = new bool[_maxDigits];

	public int Value
	{
		get
		{
			var value = 0;
			for (var i = 0; i < _displayedDigits; i++)
				value += (int)SelectedDigits[i].RawValue * (int)Math.Pow(10, i);
			return value;
		}
		init => UpdateDisplay(value);
	}

	public ICommand SubmitCommand { get; init; }

	private void UpdateDisplay(int value)
	{
		var s = value.ToString();
		if (s.Length > _displayedDigits)
			throw new ArgumentOutOfRangeException(nameof(UpdateDisplay), "value too large to display");

		var reversed = s.Reverse().ToArray();
		for (var i = 0; i < reversed.Length; i++)
		{
			try { SelectedDigits[i]= _availableDigits[i].First(item => item.DisplayValue == $"{reversed[i]}"); }
			catch { throw new ArgumentOutOfRangeException(nameof(UpdateDisplay), $"Digit {i} does not contain {reversed[i]}"); }
		}
	}
}


public interface INumberItem
{
	public string DisplayValue { get; init; }
	public object RawValue { get; }
}


public class DisplayDigit : INumberItem
{
	private readonly string _displayValue;

	public DisplayDigit(int value)
	{
		DisplayValue = value.ToString();
		if (DisplayValue.Length > 1)
			throw new ArgumentOutOfRangeException(nameof(DisplayDigit), $"Digit value {value} is too large to represent as a single character");
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
