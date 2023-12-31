﻿using BLEPoC.Ui.Pages.Tests;
using BLEPoC.Utility;

using static System.Reflection.MethodBase;


namespace BLEPoC.Ui.Pages.Basic;

internal class NavigationPageCustom : NavigationPage
{
	internal NavigationPageCustom(string title = null) : base(new FirstPage(new SecondPage(true, true, "NPage #2"), true, true, "NPage #0")) => _ = new LifeCycleTracing(this, title);

	internal event EventHandler<TraceEventArgs> BackButtonPressing;

	#region Overrides of NavigationPage

	protected override bool OnBackButtonPressed()
	{
		OnBackButtonPressing(GetCurrentMethod()?.Name);
		return base.OnBackButtonPressed();
	}

	#endregion

	private void OnBackButtonPressing(string originEvent) => BackButtonPressing?.Invoke(this, new TraceEventArgs(originEvent));
}