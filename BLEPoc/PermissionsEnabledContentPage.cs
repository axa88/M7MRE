using BLEPoC.Permissions;


namespace BLEPoC;

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
			PermissionsProcessor.Instance.Request(Window);

		if (_checkPermissionsOnResumed)
			PermissionsProcessor.Instance.Subscribe(this);
	}

	#region IDisposables

	public void Dispose()
	{
		PermissionsProcessor.Instance.Unsubscribe(this);
		GC.SuppressFinalize(this);
	}

	public async ValueTask DisposeAsync()
	{
		await PermissionsProcessor.Instance.UnsubscribeAsync(this);
		GC.SuppressFinalize(this);
	}

	#endregion
}

public class PermissionsEnabledContentPageComposition : IDisposable, IAsyncDisposable
{
	private readonly bool _checkPermissionsOnStart;
	private readonly bool _checkPermissionsOnResumed;
	private readonly Page _page;

	internal PermissionsEnabledContentPageComposition(Page page, bool checkPermissionsOnStart, bool checkPermissionsOnResumed)
	{
		_page = page;
		_checkPermissionsOnStart = checkPermissionsOnStart;
		_checkPermissionsOnResumed = checkPermissionsOnResumed;

		_page.Loaded += OnLoaded;
	}

	private void OnLoaded(object _, EventArgs __)
	{
		_page.Loaded -= OnLoaded;

		if (_checkPermissionsOnStart)
			PermissionsProcessor.Instance.Request(_page.Window);

		if (_checkPermissionsOnResumed)
			PermissionsProcessor.Instance.Subscribe(_page);
	}

	#region IDisposables

	public void Dispose()
	{
		PermissionsProcessor.Instance.Unsubscribe(_page);
		GC.SuppressFinalize(this);
	}

	public async ValueTask DisposeAsync()
	{
		await PermissionsProcessor.Instance.UnsubscribeAsync(_page);
		GC.SuppressFinalize(this);
	}

	#endregion
}
