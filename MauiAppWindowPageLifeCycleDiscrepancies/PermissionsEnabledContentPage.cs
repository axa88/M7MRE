using MauiAppNet7.Permissions;


namespace MauiAppWindowPageLifeCycleDiscrepancies;

public class PermissionsEnabledContentPage : ContentPage, IDisposable, IAsyncDisposable
{
	private readonly bool _checkPermissionsOnStart;
	private readonly bool _checkPermissionsOnResumed;

	protected PermissionsEnabledContentPage(bool checkPermissionsOnStart, bool checkPermissionsOnResumed)
	{
		_checkPermissionsOnStart = checkPermissionsOnStart;
		_checkPermissionsOnResumed = checkPermissionsOnResumed;

		Loaded += OnLoaded;
	}

	private void OnLoaded(object _, EventArgs __)
	{
		Loaded -= OnLoaded;

		if (_checkPermissionsOnStart)
			PermissionsProcessor.Request(Window);

		if (_checkPermissionsOnResumed)
			PermissionsProcessor.Subscribe(this);
	}

	#region IDisposables

	public void Dispose()
	{
		PermissionsProcessor.Unsubscribe(this);
		GC.SuppressFinalize(this);
	}

	public async ValueTask DisposeAsync()
	{
		await PermissionsProcessor.UnsubscribeAsync(this);
		GC.SuppressFinalize(this);
	}

	#endregion
}
