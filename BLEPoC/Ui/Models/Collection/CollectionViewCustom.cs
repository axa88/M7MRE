using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

using BLEPoC.Ui.Models.DisplayItems;

using Microsoft.Maui.Controls.Shapes;

using Button = Microsoft.Maui.Controls.Button;
using Switch = Microsoft.Maui.Controls.Switch;
using TimePicker = Microsoft.Maui.Controls.TimePicker;


namespace BLEPoC.Ui.Models.Collection;

internal class CollectionViewCustom : CollectionView
{
	internal CollectionViewCustom(CollectionViewModel bindingContextViewModel)
	{
		BindingContext = bindingContextViewModel;
		SetBinding(ItemsSourceProperty, new Binding(nameof(CollectionViewModel.Items)));

		ItemTemplate = new DisplayItemDataTemplateSelector();

		ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 2 };
		VerticalScrollBarVisibility = ScrollBarVisibility.Always;
		SelectionMode = SelectionMode.Single;
		SelectionChanged += (_, selectionChangedEventArgs) =>
		{
			Trace.WriteLine($"{nameof(SelectionChanged)}");
			Trace.WriteLine($"{nameof(selectionChangedEventArgs.PreviousSelection)} : {selectionChangedEventArgs.PreviousSelection}");
			Trace.WriteLine($"{nameof(selectionChangedEventArgs.CurrentSelection)} : {selectionChangedEventArgs.CurrentSelection}");
		};
		SelectionChangedCommand = new Command<object>(o => Trace.WriteLine($"{nameof(SelectionChangedCommand)} : {o}"));
	}
}


internal class DisplayItemDataTemplateSelector : DataTemplateSelector
{
	private DisplayItemTemplateFolder DisplayItemTemplateFolder { get; } = new();
	private DisplayItemTemplateBinary DisplayItemTemplateBinary { get; } = new();
	private DisplayItemTemplateMulti DisplayItemTemplateMulti { get; } = new();
	private DisplayItemTemplateTime DisplayItemTemplateTime { get; } = new();
	private DisplayItemTemplateDigits DisplayItemTemplateDigits { get; } = new();
	private DisplayItemTemplateEntry DisplayItemTemplateEntry { get; } = new();

	protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
	{
		return item switch
		{
			Summary _ => DisplayItemTemplateFolder,
			BinaryIo _ => DisplayItemTemplateBinary,
			MultiIo _ => DisplayItemTemplateMulti,
			TimeIo _ => DisplayItemTemplateTime,
			DigitIo _ => DisplayItemTemplateDigits,
			TextIo _ => DisplayItemTemplateEntry,
			_ => throw new NotImplementedException(nameof(DisplayItemDataTemplateSelector))
		};
	}
}


internal class DisplayItemTemplateFolder : DataTemplate
{
	internal DisplayItemTemplateFolder()
	{
		LoadTemplate = () =>
		{
			var outerBorder = new OuterBorder(new TextLayout());
			var tapGestureRecognizer = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
			tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, nameof(IDisplayItem.ItemCommand));
			outerBorder.GestureRecognizers.Add(tapGestureRecognizer);
			return outerBorder;
		};
	}
}


internal class DisplayItemTemplateBinary : DataTemplate
{
	internal DisplayItemTemplateBinary()
	{
		LoadTemplate = () =>
		{
			Switch @switch = new() { Margin = new Thickness(10), HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
			@switch.SetBinding(Switch.IsToggledProperty, nameof(BinaryIo.IsOn), BindingMode.TwoWay);

			HorizontalStackLayout horizontalStackLayout = new() { Children = { new TextLayout(), @switch } };

			return new OuterBorder(horizontalStackLayout);
		};
	}
}


internal class DisplayItemTemplateMulti : DataTemplate
{
	internal DisplayItemTemplateMulti()
	{
		LoadTemplate = () =>
		{
			Picker picker = new() { Margin = new Thickness(10), Title = "Picker Title" };
			picker.SetBinding(Picker.ItemsSourceProperty, nameof(MultiIo.AvailableItems)); // items available for selection
			picker.ItemDisplayBinding = new Binding(nameof(IDisplayItem.Primary)); // display selected
			picker.SetBinding(Picker.SelectedItemProperty, nameof(MultiIo.SelectedItem)); // act upon and store when selected changes

			VerticalStackLayout horizontalStackLayoutOuter = new() { Children = { new TextLayout(), picker } };

			return new OuterBorder(horizontalStackLayoutOuter);
		};
	}
}


internal class DisplayItemTemplateTime : DataTemplate
{
	internal DisplayItemTemplateTime()
	{
		LoadTemplate = () =>
		{
#if ANDROID
			TimePicker picker = new();
#else
			TimePicker picker = new() { Format = "H" };
#endif

			picker.SetBinding(TimePicker.TimeProperty, nameof(TimeIo.Time), BindingMode.TwoWay);

			VerticalStackLayout horizontalStackLayoutOuter = new() { Children = { new TextLayout(), picker } };
			return new OuterBorder(horizontalStackLayoutOuter);
		};
	}
}


internal class DisplayItemTemplateDigits : DataTemplate
{
	internal DisplayItemTemplateDigits()
	{
		LoadTemplate = () =>
		{
			Picker picker0 = new() { HorizontalTextAlignment = TextAlignment.Center };
			Picker picker1 = new() { HorizontalTextAlignment = TextAlignment.Center };
			Picker picker2 = new() { HorizontalTextAlignment = TextAlignment.Center };
			Picker picker3 = new() { HorizontalTextAlignment = TextAlignment.Center };
			Picker picker4 = new() { HorizontalTextAlignment = TextAlignment.Center };
			Picker picker5 = new() { HorizontalTextAlignment = TextAlignment.Center };

			picker0.SetBinding(Picker.ItemsSourceProperty, $"{nameof(DigitIo.AvailableDigits)}[0]");
			picker1.SetBinding(Picker.ItemsSourceProperty, $"{nameof(DigitIo.AvailableDigits)}[1]");
			picker2.SetBinding(Picker.ItemsSourceProperty, $"{nameof(DigitIo.AvailableDigits)}[2]");
			picker3.SetBinding(Picker.ItemsSourceProperty, $"{nameof(DigitIo.AvailableDigits)}[3]");
			picker4.SetBinding(Picker.ItemsSourceProperty, $"{nameof(DigitIo.AvailableDigits)}[4]");
			picker5.SetBinding(Picker.ItemsSourceProperty, $"{nameof(DigitIo.AvailableDigits)}[5]");

			picker0.ItemDisplayBinding = new Binding(nameof(IDigit.DisplayValue));
			picker1.ItemDisplayBinding = new Binding(nameof(IDigit.DisplayValue));
			picker2.ItemDisplayBinding = new Binding(nameof(IDigit.DisplayValue));
			picker3.ItemDisplayBinding = new Binding(nameof(IDigit.DisplayValue));
			picker4.ItemDisplayBinding = new Binding(nameof(IDigit.DisplayValue));
			picker5.ItemDisplayBinding = new Binding(nameof(IDigit.DisplayValue));

			DigitMultiValueConverter<IDigit> digitMuVaCo = new();
			picker0.SetBinding(Picker.SelectedItemProperty, new MultiBinding { Converter = digitMuVaCo, Mode = BindingMode.TwoWay, Bindings = new Collection<BindingBase> { new Binding($"{nameof(DigitIo.SelectedDigits)}[0]"), new Binding(nameof(DigitIo.SelectedDigitChange), BindingMode.OneWayToSource) } });
			picker1.SetBinding(Picker.SelectedItemProperty, new MultiBinding { Converter = digitMuVaCo, Mode = BindingMode.TwoWay, Bindings = new Collection<BindingBase> { new Binding($"{nameof(DigitIo.SelectedDigits)}[1]"), new Binding(nameof(DigitIo.SelectedDigitChange), BindingMode.OneWayToSource) } });
			picker2.SetBinding(Picker.SelectedItemProperty, new MultiBinding { Converter = digitMuVaCo, Mode = BindingMode.TwoWay, Bindings = new Collection<BindingBase> { new Binding($"{nameof(DigitIo.SelectedDigits)}[2]"), new Binding(nameof(DigitIo.SelectedDigitChange), BindingMode.OneWayToSource) } });
			picker3.SetBinding(Picker.SelectedItemProperty, new MultiBinding { Converter = digitMuVaCo, Mode = BindingMode.TwoWay, Bindings = new Collection<BindingBase> { new Binding($"{nameof(DigitIo.SelectedDigits)}[3]"), new Binding(nameof(DigitIo.SelectedDigitChange), BindingMode.OneWayToSource) } });
			picker4.SetBinding(Picker.SelectedItemProperty, new MultiBinding { Converter = digitMuVaCo, Mode = BindingMode.TwoWay, Bindings = new Collection<BindingBase> { new Binding($"{nameof(DigitIo.SelectedDigits)}[4]"), new Binding(nameof(DigitIo.SelectedDigitChange), BindingMode.OneWayToSource) } });
			picker5.SetBinding(Picker.SelectedItemProperty, new MultiBinding { Converter = digitMuVaCo, Mode = BindingMode.TwoWay, Bindings = new Collection<BindingBase> { new Binding($"{nameof(DigitIo.SelectedDigits)}[5]"), new Binding(nameof(DigitIo.SelectedDigitChange), BindingMode.OneWayToSource) } });

			// ReSharper disable AccessToStaticMemberViaDerivedType
			picker0.SetBinding(Picker.IsVisibleProperty, $"{nameof(DigitIo.IsVisible)}[0]");
			picker1.SetBinding(Picker.IsVisibleProperty, $"{nameof(DigitIo.IsVisible)}[1]");
			picker2.SetBinding(Picker.IsVisibleProperty, $"{nameof(DigitIo.IsVisible)}[2]");
			picker3.SetBinding(Picker.IsVisibleProperty, $"{nameof(DigitIo.IsVisible)}[3]");
			picker4.SetBinding(Picker.IsVisibleProperty, $"{nameof(DigitIo.IsVisible)}[4]");
			picker5.SetBinding(Picker.IsVisibleProperty, $"{nameof(DigitIo.IsVisible)}[5]");
			// ReSharper restore AccessToStaticMemberViaDerivedType

			HorizontalStackLayout pickerLayout = new() { Children = { picker5, picker4, picker3, picker2, picker1, picker0 } };

			Button submitButton = new() { Text = "Submit" };
			submitButton.SetBinding(Button.CommandProperty, nameof(DigitIo.SubmitCommand));

			VerticalStackLayout verticalStackLayout = new() { Children = { new TextLayout(), pickerLayout, /*submitButton*/ } };
			return new OuterBorder(verticalStackLayout);
		};
	}
}


internal class DisplayItemTemplateEntry : DataTemplate
{
	internal DisplayItemTemplateEntry()
	{
		LoadTemplate = () =>
		{
			Entry entry = new() { ClearButtonVisibility = ClearButtonVisibility.WhileEditing, ReturnType = ReturnType.Send };
			entry.SetBinding(Entry.TextProperty, nameof(TextIo.Text));
			entry.SetBinding(Entry.TextColorProperty, nameof(TextIo.TextColor));
			entry.SetBinding(Entry.PlaceholderProperty, nameof(TextIo.TextPlaceHolder));
			// ReSharper disable once AccessToStaticMemberViaDerivedType
			entry.SetBinding(Entry.MaxLengthProperty, nameof(TextIo.MaxLength));
			entry.SetBinding(Entry.KeyboardProperty, nameof(TextIo.Keyboard));
			entry.SetBinding(Entry.ReturnCommandProperty, nameof(TextIo.SubmitCommand));
			entry.SetBinding(Entry.ReturnCommandParameterProperty, nameof(TextIo.Text));

			VerticalStackLayout verticalStackLayout = new() { Children = { new TextLayout(), entry } };
			return new OuterBorder(verticalStackLayout);
		};
	}
}


internal class TextLayout : VerticalStackLayout
{
	internal TextLayout()
	{
		Label primaryLabel = new() { Margin = new Thickness(10), FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
		primaryLabel.SetBinding(Label.TextProperty, nameof(IDisplayItem.Primary));

		Label secondaryLabel = new() { Margin = new Thickness(10), VerticalOptions = LayoutOptions.Center };
		secondaryLabel.SetBinding(Label.TextProperty, nameof(IDisplayItem.Secondary));

		Children.Add(primaryLabel);
		Children.Add(secondaryLabel);
	}
}


internal class OuterBorder : Border
{
	internal OuterBorder(View content)
	{
		BackgroundColor = Colors.Transparent;
		Padding = new Thickness(10);
		StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(15, 0, 0, 15) };
		Stroke = new LinearGradientBrush { EndPoint = new Point(0, 1), GradientStops = [new() { Color = Colors.Orange, Offset = 0.1f }, new() { Color = Colors.Brown, Offset = 1.0f }] };
		Content = content;
	}
}


internal class DigitMultiValueConverter<T> : IMultiValueConverter
{
	// source: IDigit SelectedDigit, object SelectedDigitChange <> target: object SelectedItem

	#region Implementation of IMultiValueConverter

	/// <summary>
	/// Convert date from source to target
	/// </summary>
	/// <param name="values"></param>
	/// <param name="targetType"></param>
	/// <param name="parameter"></param>
	/// <param name="culture"></param>
	/// <returns></returns>
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) { return values?.FirstOrDefault(o => o is T); }

	/// <summary>
	/// convert from target to source
	/// </summary>
	/// <param name="value"></param>
	/// <param name="targetTypes"></param>
	/// <param name="parameter"></param>
	/// <param name="culture"></param>
	/// <returns></returns>
	/// <exception cref="NotImplementedException"></exception>
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => targetTypes.Select(_ => value).ToArray();

	#endregion
}