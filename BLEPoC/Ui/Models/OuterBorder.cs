using Microsoft.Maui.Controls.Shapes;


namespace BLEPoC.Ui.Models;

internal class OuterBorder : Border
{
	internal OuterBorder(View content)
	{
		BackgroundColor = Colors.Transparent;
		Padding = new Thickness(20);
		StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(15, 0, 0, 15) };
		Stroke = new LinearGradientBrush { EndPoint = new Point(0, 1), GradientStops = [new() { Color = Colors.Orange, Offset = 0.1f }, new() { Color = Colors.Brown, Offset = 1.0f }] };
		Content = content;
	}
}
