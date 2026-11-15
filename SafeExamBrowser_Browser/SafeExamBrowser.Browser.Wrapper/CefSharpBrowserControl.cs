using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using CefSharp;
using CefSharp.Enums;
using CefSharp.WinForms;
using SafeExamBrowser.Browser.Wrapper.Events;
using SafeExamBrowser.Browser.Wrapper.Handlers;

namespace SafeExamBrowser.Browser.Wrapper;

internal class CefSharpBrowserControl : ChromiumWebBrowser, ICefSharpControl, IChromiumWebBrowserBase, IDisposable, IWinFormsChromiumWebBrowser, IWin32Window, IComponent, ISynchronizeInvoke
{
	public event AuthCredentialsEventHandler AuthCredentialsRequired;

	public event BeforeBrowseEventHandler BeforeBrowse;

	public event BeforeContextMenuEventHandler BeforeContextMenu;

	public event BeforeDownloadEventHandler BeforeDownload;

	public event BeforeUnloadDialogEventHandler BeforeUnloadDialog;

	public event CanDownloadEventHandler CanDownload;

	public event ContextCreatedEventHandler ContextCreated;

	public event ContextMenuCommandEventHandler ContextMenuCommand;

	public event ContextMenuDismissedEventHandler ContextMenuDismissed;

	public event ContextReleasedEventHandler ContextReleased;

	public event DialogClosedEventHandler DialogClosed;

	public event DownloadUpdatedEventHandler DownloadUpdated;

	public event DragEnterEventHandler DragEnterCefSharp;

	public event DraggableRegionsChangedEventHandler DraggableRegionsChanged;

	public event FaviconUrlChangedEventHandler FaviconUrlChanged;

	public event FileDialogRequestedEventHandler FileDialogRequested;

	public event FocusedNodeChangedEventHandler FocusedNodeChanged;

	public event GotFocusEventHandler GotFocusCefSharp;

	public event JavaScriptDialogEventHandler JavaScriptDialog;

	public event SafeExamBrowser.Browser.Wrapper.Events.KeyEventHandler KeyEvent;

	public event LoadingProgressChangedEventHandler LoadingProgressChanged;

	public event OpenUrlFromTabEventHandler OpenUrlFromTab;

	public event PreKeyEventHandler PreKeyEvent;

	public event ResetDialogStateEventHandler ResetDialogState;

	public event ResourceRequestEventHandler ResourceRequestHandlerRequired;

	public event RunContextMenuEventHandler RunContextMenu;

	public event SetFocusEventHandler SetFocus;

	public event TakeFocusEventHandler TakeFocus;

	public event UncaughtExceptionEventHandler UncaughtExceptionEvent;

	public CefSharpBrowserControl(ILifeSpanHandler lifeSpanHandler, string url)
		: base(url, (IRequestContext)null)
	{
		((ChromiumWebBrowser)this).DialogHandler = (IDialogHandler)(object)new DialogHandlerSwitch();
		((ChromiumWebBrowser)this).DisplayHandler = (IDisplayHandler)(object)new DisplayHandlerSwitch();
		((ChromiumWebBrowser)this).DownloadHandler = (IDownloadHandler)(object)new DownloadHandlerSwitch();
		((ChromiumWebBrowser)this).DragHandler = (IDragHandler)(object)new DragHandlerSwitch();
		((ChromiumWebBrowser)this).FocusHandler = (IFocusHandler)(object)new FocusHandlerSwitch();
		((ChromiumWebBrowser)this).JsDialogHandler = (IJsDialogHandler)(object)new JavaScriptDialogHandlerSwitch();
		((ChromiumWebBrowser)this).KeyboardHandler = (IKeyboardHandler)(object)new KeyboardHandlerSwitch();
		((ChromiumWebBrowser)this).LifeSpanHandler = lifeSpanHandler;
		((ChromiumWebBrowser)this).MenuHandler = (IContextMenuHandler)(object)new ContextMenuHandlerSwitch();
		((ChromiumWebBrowser)this).RenderProcessMessageHandler = (IRenderProcessMessageHandler)(object)new RenderProcessMessageHandlerSwitch();
		((ChromiumWebBrowser)this).RequestHandler = (IRequestHandler)(object)new RequestHandlerSwitch();
	}

	void ICefSharpControl.Dispose(bool disposing)
	{
		((ChromiumWebBrowser)this).Dispose(disposing);
	}

	public void GetAuthCredentials(IWebBrowser webBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback, GenericEventArgs args)
	{
		AuthCredentialsRequired?.Invoke(webBrowser, browser, originUrl, isProxy, host, port, realm, scheme, callback, args);
	}

	public void GetResourceRequestHandler(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling, ResourceRequestEventArgs args)
	{
		ResourceRequestHandlerRequired?.Invoke(webBrowser, browser, frame, request, isNavigation, isDownload, requestInitiator, ref disableDefaultHandling, args);
	}

	public void OnBeforeBrowse(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect, GenericEventArgs args)
	{
		BeforeBrowse?.Invoke(webBrowser, browser, frame, request, userGesture, isRedirect, args);
	}

	public void OnBeforeContextMenu(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
	{
		BeforeContextMenu?.Invoke(webBrowser, browser, frame, parameters, model);
	}

	public void OnBeforeDownload(IWebBrowser webBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback, GenericEventArgs args)
	{
		BeforeDownload?.Invoke(webBrowser, browser, downloadItem, callback, args);
	}

	public void OnBeforeUnloadDialog(IWebBrowser webBrowser, IBrowser browser, string message, bool isReload, IJsDialogCallback callback, GenericEventArgs args)
	{
		BeforeUnloadDialog?.Invoke(webBrowser, browser, message, isReload, callback, args);
	}

	public void OnCanDownload(IWebBrowser webBrowser, IBrowser browser, string url, string requestMethod, GenericEventArgs args)
	{
		CanDownload?.Invoke(webBrowser, browser, url, requestMethod, args);
	}

	public void OnContextCreated(IWebBrowser webBrowser, IBrowser browser, IFrame frame)
	{
		ContextCreated?.Invoke(webBrowser, browser, frame);
	}

	public void OnContextMenuCommand(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags, GenericEventArgs args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		ContextMenuCommand?.Invoke(webBrowser, browser, frame, parameters, commandId, eventFlags, args);
	}

	public void OnContextMenuDismissed(IWebBrowser webBrowser, IBrowser browser, IFrame frame)
	{
		ContextMenuDismissed?.Invoke(webBrowser, browser, frame);
	}

	public void OnContextReleased(IWebBrowser webBrowser, IBrowser browser, IFrame frame)
	{
		ContextReleased?.Invoke(webBrowser, browser, frame);
	}

	public void OnDialogClosed(IWebBrowser webBrowser, IBrowser browser)
	{
		DialogClosed?.Invoke(webBrowser, browser);
	}

	public void OnDownloadUpdated(IWebBrowser webBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
	{
		DownloadUpdated?.Invoke(webBrowser, browser, downloadItem, callback);
	}

	public void OnDragEnter(IWebBrowser webBrowser, IBrowser browser, IDragData dragData, DragOperationsMask mask, GenericEventArgs args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		DragEnterCefSharp?.Invoke(webBrowser, browser, dragData, mask, args);
	}

	public void OnDraggableRegionsChanged(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IList<DraggableRegion> regions)
	{
		DraggableRegionsChanged?.Invoke(webBrowser, browser, frame, regions);
	}

	public void OnFaviconUrlChange(IWebBrowser webBrowser, IBrowser browser, IList<string> urls)
	{
		FaviconUrlChanged?.Invoke(webBrowser, browser, urls);
	}

	public void OnFileDialog(IWebBrowser webBrowser, IBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, IReadOnlyCollection<string> acceptFilters, IReadOnlyCollection<string> acceptExtensions, IReadOnlyCollection<string> acceptDescriptions, IFileDialogCallback callback)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		FileDialogRequested?.Invoke(webBrowser, browser, mode, title, defaultFilePath, acceptFilters, acceptExtensions, acceptDescriptions, callback);
	}

	public void OnFocusedNodeChanged(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IDomNode node)
	{
		FocusedNodeChanged?.Invoke(webBrowser, browser, frame, node);
	}

	public void OnGotFocus(IWebBrowser webBrowser, IBrowser browser)
	{
		GotFocusCefSharp?.Invoke(webBrowser, browser);
	}

	public void OnJavaScriptDialog(IWebBrowser webBrowser, IBrowser browser, string originUrl, CefJsDialogType type, string message, string promptText, IJsDialogCallback callback, ref bool suppress, GenericEventArgs args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		JavaScriptDialog?.Invoke(webBrowser, browser, originUrl, type, message, promptText, callback, ref suppress, args);
	}

	public void OnKeyEvent(IWebBrowser webBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		KeyEvent?.Invoke(webBrowser, browser, type, windowsKeyCode, nativeKeyCode, modifiers, isSystemKey);
	}

	public void OnLoadingProgressChange(IWebBrowser webBrowser, IBrowser browser, double progress)
	{
		LoadingProgressChanged?.Invoke(webBrowser, browser, progress);
	}

	public void OnOpenUrlFromTab(IWebBrowser webBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture, GenericEventArgs args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		OpenUrlFromTab?.Invoke(webBrowser, browser, frame, targetUrl, targetDisposition, userGesture, args);
	}

	public void OnPreKeyEvent(IWebBrowser webBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut, GenericEventArgs args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		PreKeyEvent?.Invoke(webBrowser, browser, type, windowsKeyCode, nativeKeyCode, modifiers, isSystemKey, ref isKeyboardShortcut, args);
	}

	public void OnResetDialogState(IWebBrowser webBrowser, IBrowser browser)
	{
		ResetDialogState?.Invoke(webBrowser, browser);
	}

	public void OnRunContextMenu(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback, GenericEventArgs args)
	{
		RunContextMenu?.Invoke(webBrowser, browser, frame, parameters, model, callback, args);
	}

	public void OnSetFocus(IWebBrowser webBrowser, IBrowser browser, CefFocusSource source, GenericEventArgs args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SetFocus?.Invoke(webBrowser, browser, source, args);
	}

	public void OnTakeFocus(IWebBrowser webBrowser, IBrowser browser, bool next)
	{
		TakeFocus?.Invoke(webBrowser, browser, next);
	}

	public void OnUncaughtException(IWebBrowser webBrowser, IBrowser browser, IFrame frame, JavascriptException exception)
	{
		UncaughtExceptionEvent?.Invoke(webBrowser, browser, frame, exception);
	}
}
