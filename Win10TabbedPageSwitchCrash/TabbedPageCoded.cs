namespace Win10TabbedPageSwitchCrash
{
	public class TabbedPageCoded : TabbedPage
	{
		public TabbedPageCoded()
		{
			Title = "TabbedPage";

			ItemsSource = new[]
			{
				new NamedColor("Red"),
				new NamedColor("Yellow"),
				new NamedColor("Green"),
				new NamedColor("Aqua"),
				new NamedColor("Blue"),
				new NamedColor("Purple")
			};

			ItemTemplate = new DataTemplate(() => new ColorsDataTemplate());
		}
	}

	// Data type:
	class NamedColor
	{
		public NamedColor(string name)
		{
			if (Color.TryParse(name, out var color))
			{
				Name = name;
				Color = color;
			}
			else
			{
				Name = "Invalid";
				Color = new Color(0, 0, 0, 0);
			}
		}

		public string Name { private set; get; }

		public Color Color { private set; get; }

		public override string ToString() => Name;
	}


	// Format page
	class ColorsDataTemplate : ContentPage
	{
		public ColorsDataTemplate()
		{
			// This binding is necessary to label the tabs in the TabbedPage.
			this.SetBinding(TitleProperty, "Name");
			// BoxView to show the color.
			var boxView = new BoxView { WidthRequest = 100, HeightRequest = 100, HorizontalOptions = LayoutOptions.Center };
			boxView.SetBinding(BoxView.ColorProperty, "Color");

			// Build the page
			Content = boxView;
		}
	}
}