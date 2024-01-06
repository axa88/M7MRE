#if ANDROID
using Android.Widget;
#endif

using BLEPoC.Ui.Models.Collection;
using BLEPoC.Ui.Models.Collection.Items;

using static Microsoft.Maui.Controls.VisualStateManager;

using Button = Microsoft.Maui.Controls.Button;


namespace BLEPoC.Ui.Pages.Controls;

internal class TestCollectionPage<T> : ContentPage where T : ICollectionItem, new()
{
	internal TestCollectionPage(ItemCollection<T> itemCollection, Action buttonAction = null)
	{
		var stackLayout = new StackLayout();

		if (buttonAction != null)
		{
			Button pageButton = new() { Text = "Push Page" };
			pageButton.Clicked += (_, _) => buttonAction();
			stackLayout.Add(pageButton);
		}

		stackLayout.Children.Add(new CollectionViewCustom<T>(itemCollection));
		Content = new Grid { Margin = 20, Children = { stackLayout } };
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
