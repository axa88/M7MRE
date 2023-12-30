using BLEPoC.Ui.Pages.Basic;
using BLEPoC.Utility;


namespace BLEPoC;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		_ = new LifeCycleTracing(this, nameof(App));
	}

	internal event EventHandler<TraceEventArgs> Starting;
	internal event EventHandler<TraceEventArgs> Resuming;
	internal event EventHandler<TraceEventArgs> Sleeping;
	internal event EventHandler<TraceEventArgs> CreatingWindow;

	#region Overrides of Application

	protected override Window CreateWindow(IActivationState activationState)
	{
		OnCreatingWindow(nameof(CreateWindow));
		return new CustomWindow(new SelectorPage(), "S.Window"); // ToDo eliminate hardcode
	}

	protected override void OnStart() => OnStarting(nameof(OnStart)); //base.OnStart(); is empty
	protected override void OnResume() => OnResuming(nameof(OnResume)); //base.OnResume(); is empty
	protected override void OnSleep() => OnSleeping(nameof(OnSleep)); //base.OnSleep(); is empty

	#endregion

	private void OnStarting(string originEvent) => Starting?.Invoke(this, new TraceEventArgs(originEvent));

	private void OnResuming(string originEvent) => Resuming?.Invoke(this, new TraceEventArgs(originEvent));

	private void OnSleeping(string originEvent) => Sleeping?.Invoke(this, new TraceEventArgs(originEvent));

	private void OnCreatingWindow(string originEvent) => CreatingWindow?.Invoke(this, new TraceEventArgs(originEvent));
}
