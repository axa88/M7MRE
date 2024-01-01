#if ANDROID
using Android.Widget;
#endif

using BLEPoC.Ui.Models.Collection;

using static Microsoft.Maui.Controls.VisualStateManager;

using Button = Microsoft.Maui.Controls.Button;


namespace BLEPoC.Ui.Pages.Controls;

internal class CollectionPage : ContentPage
{
	internal CollectionPage(CollectionViewModel collectionViewModel)
	{
		Grid grid = new() { Margin = 20, Children = { new CollectionViewCustom(collectionViewModel) } };
		Content = grid;
		//BindingContext = collectionViewModel;

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
