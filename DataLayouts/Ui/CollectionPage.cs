using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

using Microsoft.Maui.Controls.Shapes;

using static Microsoft.Maui.Controls.VisualStateManager;

using Button = Microsoft.Maui.Controls.Button;
using Switch = Microsoft.Maui.Controls.Switch;
using TimePicker = Microsoft.Maui.Controls.TimePicker;

#if ANDROID
using Android.Widget;
#endif


namespace DataLayouts.Ui;

internal class CollectionPage : ContentPage
{
	public CollectionPage()
	{
		Content = new Grid { Margin = 20, Children = { new CollectionViewCustom() } };
		var testItemsModel = new TestItemsViewModel();
		BindingContext = testItemsModel;

		Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("MyCustomization", (handler, view) =>
		{
		#if ANDROID
			if (handler.PlatformView is EditText edit)
			{
				edit.Focusable = false;
				edit.FocusableInTouchMode = false;
			}
		#endif
		});

		//Setter textColorSetter = new() { TargetName = ???, Property = Label.TextColorProperty, Value = Colors.Red };
		Setter backgroundColorSetter = new() { Property = BackgroundColorProperty, Value = Colors.LightSkyBlue };
		VisualState stateSelected = new() { Name = CommonStates.Selected, Setters = { backgroundColorSetter /*, textColorSetter*/ } };
		VisualState stateNormal = new() { Name = CommonStates.Normal }; // VisualStateGroup always requires a "Normal" VisualState entry.
		VisualStateGroup visualStateGroup = new() { Name = nameof(CommonStates), States = { stateSelected, stateNormal } };
		Style style = new(typeof(Grid)) { Setters = { new() { Property = VisualStateGroupsProperty, Value = new VisualStateGroupList { visualStateGroup } } } }; // applies to anything within the type (Grid)
		Resources.Add(style);
	}
}


internal class CollectionViewCustom : CollectionView
{
	public CollectionViewCustom()
	{
		ItemTemplate = new DisplayItemDataTemplateSelector();
		SetBinding(ItemsSourceProperty, new Binding(nameof(TestItemsViewModel.Items)));

		ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 2 };
		VerticalScrollBarVisibility = ScrollBarVisibility.Always;
		SelectionMode = SelectionMode.Single;
		SelectionChanged += (sender, selectionChangedEventArgs) => Trace.WriteLine($"{nameof(SelectionChanged)}");
		SelectionChangedCommand = new Command<object>(o => Trace.WriteLine($"{nameof(SelectionChangedCommand)} : {o}"));
	}
}


public class DisplayItemDataTemplateSelector : DataTemplateSelector
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
			DisplayItemFolder _ => DisplayItemTemplateFolder,
			DisplayItemBinary _ => DisplayItemTemplateBinary,
			DisplayItemMulti _ => DisplayItemTemplateMulti,
			DisplayItemTime _ => DisplayItemTemplateTime,
			DisplayItemDigits _ => DisplayItemTemplateDigits,
			DisplayItemEntry _ => DisplayItemTemplateEntry,
			_ => throw new NotImplementedException(nameof(DisplayItemDataTemplateSelector))
		};
	}
}


public class DisplayItemTemplateFolder : DataTemplate
{
	public DisplayItemTemplateFolder()
	{
		LoadTemplate = () =>
		{
			var outerBorder = new OuterBorder(new BaseLayout());
			var tapGestureRecognizer = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
			tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, nameof(IDisplayItem.ItemCommand));
			outerBorder.GestureRecognizers.Add(tapGestureRecognizer);
			return outerBorder;
		};
	}
}


public class DisplayItemTemplateBinary : DataTemplate
{
	public DisplayItemTemplateBinary()
	{
		LoadTemplate = () =>
		{
			Switch @switch = new() { Margin = new Thickness(10), HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
			@switch.SetBinding(Switch.IsToggledProperty, nameof(DisplayItemBinary.IsOn), BindingMode.TwoWay);

			HorizontalStackLayout horizontalStackLayout = new() { Children = { new BaseLayout(), @switch } };

			return new OuterBorder(horizontalStackLayout);
		};
	}
}


public class DisplayItemTemplateMulti : DataTemplate
{
	public DisplayItemTemplateMulti()
	{
		LoadTemplate = () =>
		{
			Picker picker = new() { Margin = new Thickness(10), Title = "Picker Title" };
			picker.SetBinding(Picker.ItemsSourceProperty, nameof(DisplayItemMulti.AvailableItems)); // items available for selection
			picker.ItemDisplayBinding = new Binding(nameof(IDisplayItem.Primary)); // display selected
			picker.SetBinding(Picker.SelectedItemProperty, nameof(DisplayItemMulti.SelectedItem)); // act upon and store when selected changes

			VerticalStackLayout horizontalStackLayoutOuter = new() { Children = { new BaseLayout(), picker } };

			return new OuterBorder(horizontalStackLayoutOuter);
		};
	}
}


public class DisplayItemTemplateTime : DataTemplate
{
	public DisplayItemTemplateTime()
	{
		LoadTemplate = () =>
		{
#if ANDROID
			TimePicker picker = new();
#else
			TimePicker picker = new() { Format = "H" };
#endif

			picker.SetBinding(TimePicker.TimeProperty, nameof(DisplayItemTime.Time), BindingMode.TwoWay);

			VerticalStackLayout horizontalStackLayoutOuter = new() { Children = { new BaseLayout(), picker } };
			return new OuterBorder(horizontalStackLayoutOuter);
		};
	}
}


public class DisplayItemTemplateDigits : DataTemplate
{
	public DisplayItemTemplateDigits()
	{
		LoadTemplate = () =>
		{
			Picker picker0 = new() { HorizontalTextAlignment = TextAlignment.Center };
			Picker picker1 = new() { HorizontalTextAlignment = TextAlignment.Center };
			Picker picker2 = new() { HorizontalTextAlignment = TextAlignment.Center };
			Picker picker3 = new() { HorizontalTextAlignment = TextAlignment.Center };
			Picker picker4 = new() { HorizontalTextAlignment = TextAlignment.Center };
			Picker picker5 = new() { HorizontalTextAlignment = TextAlignment.Center };

			picker0.SetBinding(Picker.ItemsSourceProperty, $"{nameof(DisplayItemDigits.AvailableDigits)}[0]");
			picker1.SetBinding(Picker.ItemsSourceProperty, $"{nameof(DisplayItemDigits.AvailableDigits)}[1]");
			picker2.SetBinding(Picker.ItemsSourceProperty, $"{nameof(DisplayItemDigits.AvailableDigits)}[2]");
			picker3.SetBinding(Picker.ItemsSourceProperty, $"{nameof(DisplayItemDigits.AvailableDigits)}[3]");
			picker4.SetBinding(Picker.ItemsSourceProperty, $"{nameof(DisplayItemDigits.AvailableDigits)}[4]");
			picker5.SetBinding(Picker.ItemsSourceProperty, $"{nameof(DisplayItemDigits.AvailableDigits)}[5]");

			picker0.ItemDisplayBinding = new Binding(nameof(IDigit.DisplayValue));
			picker1.ItemDisplayBinding = new Binding(nameof(IDigit.DisplayValue));
			picker2.ItemDisplayBinding = new Binding(nameof(IDigit.DisplayValue));
			picker3.ItemDisplayBinding = new Binding(nameof(IDigit.DisplayValue));
			picker4.ItemDisplayBinding = new Binding(nameof(IDigit.DisplayValue));
			picker5.ItemDisplayBinding = new Binding(nameof(IDigit.DisplayValue));

			DigitMultiValueConverter<IDigit> digitMuVaCo = new();
			picker0.SetBinding(Picker.SelectedItemProperty, new MultiBinding { Converter = digitMuVaCo, Mode = BindingMode.TwoWay, Bindings = new Collection<BindingBase> { new Binding($"{nameof(DisplayItemDigits.SelectedDigits)}[0]"), new Binding(nameof(DisplayItemDigits.SelectedDigitChange), BindingMode.OneWayToSource) } });
			picker1.SetBinding(Picker.SelectedItemProperty, new MultiBinding { Converter = digitMuVaCo, Mode = BindingMode.TwoWay, Bindings = new Collection<BindingBase> { new Binding($"{nameof(DisplayItemDigits.SelectedDigits)}[1]"), new Binding(nameof(DisplayItemDigits.SelectedDigitChange), BindingMode.OneWayToSource) } });
			picker2.SetBinding(Picker.SelectedItemProperty, new MultiBinding { Converter = digitMuVaCo, Mode = BindingMode.TwoWay, Bindings = new Collection<BindingBase> { new Binding($"{nameof(DisplayItemDigits.SelectedDigits)}[2]"), new Binding(nameof(DisplayItemDigits.SelectedDigitChange), BindingMode.OneWayToSource) } });
			picker3.SetBinding(Picker.SelectedItemProperty, new MultiBinding { Converter = digitMuVaCo, Mode = BindingMode.TwoWay, Bindings = new Collection<BindingBase> { new Binding($"{nameof(DisplayItemDigits.SelectedDigits)}[3]"), new Binding(nameof(DisplayItemDigits.SelectedDigitChange), BindingMode.OneWayToSource) } });
			picker4.SetBinding(Picker.SelectedItemProperty, new MultiBinding { Converter = digitMuVaCo, Mode = BindingMode.TwoWay, Bindings = new Collection<BindingBase> { new Binding($"{nameof(DisplayItemDigits.SelectedDigits)}[4]"), new Binding(nameof(DisplayItemDigits.SelectedDigitChange), BindingMode.OneWayToSource) } });
			picker5.SetBinding(Picker.SelectedItemProperty, new MultiBinding { Converter = digitMuVaCo, Mode = BindingMode.TwoWay, Bindings = new Collection<BindingBase> { new Binding($"{nameof(DisplayItemDigits.SelectedDigits)}[5]"), new Binding(nameof(DisplayItemDigits.SelectedDigitChange), BindingMode.OneWayToSource) } });

			picker0.SetBinding(Picker.IsVisibleProperty, $"{nameof(DisplayItemDigits.IsVisible)}[0]");
			picker1.SetBinding(Picker.IsVisibleProperty, $"{nameof(DisplayItemDigits.IsVisible)}[1]");
			picker2.SetBinding(Picker.IsVisibleProperty, $"{nameof(DisplayItemDigits.IsVisible)}[2]");
			picker3.SetBinding(Picker.IsVisibleProperty, $"{nameof(DisplayItemDigits.IsVisible)}[3]");
			picker4.SetBinding(Picker.IsVisibleProperty, $"{nameof(DisplayItemDigits.IsVisible)}[4]");
			picker5.SetBinding(Picker.IsVisibleProperty, $"{nameof(DisplayItemDigits.IsVisible)}[5]");

			HorizontalStackLayout pickerLayout = new() {Children = { picker5, picker4, picker3, picker2, picker1, picker0 } };

			Button submitButton = new() { Text = "Submit" };
			submitButton.SetBinding(Button.CommandProperty, nameof(DisplayItemDigits.SubmitCommand));

			VerticalStackLayout verticalStackLayout = new() { Children = { new BaseLayout(), pickerLayout, /*submitButton*/ } };
			return new OuterBorder(verticalStackLayout);
		};
	}
}


public class DisplayItemTemplateEntry : DataTemplate
{
	public DisplayItemTemplateEntry()
	{
		LoadTemplate = () =>
		{
			Entry entry = new() { ClearButtonVisibility = ClearButtonVisibility.WhileEditing, ReturnType = ReturnType.Send };
			entry.SetBinding(Entry.TextProperty, nameof(DisplayItemEntry.Text));
			entry.SetBinding(Entry.TextColorProperty, nameof(DisplayItemEntry.TextColor));
			entry.SetBinding(Entry.PlaceholderProperty, nameof(DisplayItemEntry.TextPlaceHolder));
			entry.SetBinding(Entry.MaxLengthProperty, nameof(DisplayItemEntry.MaxLength));
			entry.SetBinding(Entry.KeyboardProperty, nameof(DisplayItemEntry.Keyboard));
			entry.SetBinding(Entry.ReturnCommandProperty, nameof(DisplayItemEntry.SubmitCommand));
			entry.SetBinding(Entry.ReturnCommandParameterProperty, nameof(DisplayItemEntry.Text));

			VerticalStackLayout verticalStackLayout = new() { Children = { new BaseLayout(), entry } };
			return new OuterBorder(verticalStackLayout);
		};
	}
}


public class BaseLayout : VerticalStackLayout
{
	public BaseLayout()
	{
		Label primaryLabel = new() { Margin = new Thickness(10), FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
		primaryLabel.SetBinding(Label.TextProperty, nameof(IDisplayItem.Primary));

		Label secondaryLabel = new() { Margin = new Thickness(10), VerticalOptions = LayoutOptions.Center };
		secondaryLabel.SetBinding(Label.TextProperty, nameof(IDisplayItem.Secondary));

		Children.Add(primaryLabel);
		Children.Add(secondaryLabel);
	}
}


public class OuterBorder : Border
{
	public OuterBorder(View content)
	{
		BackgroundColor = Colors.Transparent;
		Padding = new Thickness(10);
		StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(15, 0, 0, 15) };
		Stroke = new LinearGradientBrush { EndPoint = new Point(0, 1), GradientStops = new GradientStopCollection
			{ new() { Color = Colors.Orange, Offset = 0.1f }, new() { Color = Colors.Brown, Offset = 1.0f } } };
		Content = content;
	}
}


public class DigitMultiValueConverter<T> : IMultiValueConverter
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