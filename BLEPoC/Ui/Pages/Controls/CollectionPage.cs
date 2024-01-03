#if ANDROID
using Android.Widget;
#endif

using BLEPoC.Ui.Models.Collection;
using BLEPoC.Ui.Models.Collection.Items;

using static Microsoft.Maui.Controls.VisualStateManager;

using Button = Microsoft.Maui.Controls.Button;


namespace BLEPoC.Ui.Pages.Controls;

internal class CollectionPage<T> : ContentPage where T : ICollectionItem
{
	internal CollectionPage(ItemCollection<T> itemCollection)
	{
		//Button testButton = new() { Text = "test-button" };
		//Grid grid = new() { Margin = 20, Children = { new StackLayout { Children = { testButton, new CollectionViewCustom(itemCollection) } } } };

		Grid grid = new() { Margin = 20, Children = { new CollectionViewCustom<T>(itemCollection) } };
		Content = grid;
		//BindingContext = itemCollection;

		// ReSharper disable once UnusedParameter.Local
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
