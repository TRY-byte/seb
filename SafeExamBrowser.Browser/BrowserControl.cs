using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Enums;
using CefSharp.WinForms;
using SafeExamBrowser.Browser.Wrapper;
using SafeExamBrowser.Browser.Wrapper.Events;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Browser;
using SafeExamBrowser.UserInterface.Contracts.Browser.Data;
using SafeExamBrowser.UserInterface.Contracts.Browser.Events;

namespace SafeExamBrowser.Browser;

internal class BrowserControl : IBrowserControl
{
	private readonly Clipboard clipboard;

	private readonly ICefSharpControl control;

	private readonly IContextMenuHandler contextMenuHandler;

	private readonly IDialogHandler dialogHandler;

	private readonly IDisplayHandler displayHandler;

	private readonly IDownloadHandler downloadHandler;

	private readonly IDragHandler dragHandler;

	private readonly IFocusHandler focusHandler;

	private readonly IJsDialogHandler javaScriptDialogHandler;

	private readonly IKeyboardHandler keyboardHandler;

	private readonly ILogger logger;

	private readonly IRenderProcessMessageHandler renderProcessMessageHandler;

	private readonly IRequestHandler requestHandler;

	[CompilerGenerated]
	private AddressChangedEventHandler m_AddressChanged;

	[CompilerGenerated]
	private LoadFailedEventHandler m_LoadFailed;

	[CompilerGenerated]
	private LoadingStateChangedEventHandler m_LoadingStateChanged;

	[CompilerGenerated]
	private TitleChangedEventHandler m_TitleChanged;

	public string Address => ((IChromiumWebBrowserBase)control).Address;

	public bool CanNavigateBackwards
	{
		get
		{
			if (((IChromiumWebBrowserBase)control).IsBrowserInitialized)
			{
				return ((IChromiumWebBrowserBase)control).BrowserCore.CanGoBack;
			}
			return false;
		}
	}

	public bool CanNavigateForwards
	{
		get
		{
			if (((IChromiumWebBrowserBase)control).IsBrowserInitialized)
			{
				return ((IChromiumWebBrowserBase)control).BrowserCore.CanGoForward;
			}
			return false;
		}
	}

	public object EmbeddableControl => control;

	public event AddressChangedEventHandler AddressChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			AddressChangedEventHandler val = this.m_AddressChanged;
			AddressChangedEventHandler val2;
			do
			{
				val2 = val;
				AddressChangedEventHandler value2 = (AddressChangedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_AddressChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			AddressChangedEventHandler val = this.m_AddressChanged;
			AddressChangedEventHandler val2;
			do
			{
				val2 = val;
				AddressChangedEventHandler value2 = (AddressChangedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_AddressChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public event LoadFailedEventHandler LoadFailed
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			LoadFailedEventHandler val = this.m_LoadFailed;
			LoadFailedEventHandler val2;
			do
			{
				val2 = val;
				LoadFailedEventHandler value2 = (LoadFailedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_LoadFailed, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			LoadFailedEventHandler val = this.m_LoadFailed;
			LoadFailedEventHandler val2;
			do
			{
				val2 = val;
				LoadFailedEventHandler value2 = (LoadFailedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_LoadFailed, value2, val2);
			}
			while (val != val2);
		}
	}

	public event LoadingStateChangedEventHandler LoadingStateChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			LoadingStateChangedEventHandler val = this.m_LoadingStateChanged;
			LoadingStateChangedEventHandler val2;
			do
			{
				val2 = val;
				LoadingStateChangedEventHandler value2 = (LoadingStateChangedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_LoadingStateChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			LoadingStateChangedEventHandler val = this.m_LoadingStateChanged;
			LoadingStateChangedEventHandler val2;
			do
			{
				val2 = val;
				LoadingStateChangedEventHandler value2 = (LoadingStateChangedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_LoadingStateChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public event TitleChangedEventHandler TitleChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			TitleChangedEventHandler val = this.m_TitleChanged;
			TitleChangedEventHandler val2;
			do
			{
				val2 = val;
				TitleChangedEventHandler value2 = (TitleChangedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_TitleChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			TitleChangedEventHandler val = this.m_TitleChanged;
			TitleChangedEventHandler val2;
			do
			{
				val2 = val;
				TitleChangedEventHandler value2 = (TitleChangedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_TitleChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public BrowserControl(Clipboard clipboard, ICefSharpControl control, IContextMenuHandler contextMenuHandler, IDialogHandler dialogHandler, IDisplayHandler displayHandler, IDownloadHandler downloadHandler, IDragHandler dragHandler, IFocusHandler focusHandler, IJsDialogHandler javaScriptDialogHandler, IKeyboardHandler keyboardHandler, ILogger logger, IRenderProcessMessageHandler renderProcessMessageHandler, IRequestHandler requestHandler)
	{
		this.clipboard = clipboard;
		this.control = control;
		this.contextMenuHandler = contextMenuHandler;
		this.dialogHandler = dialogHandler;
		this.displayHandler = displayHandler;
		this.downloadHandler = downloadHandler;
		this.dragHandler = dragHandler;
		this.focusHandler = focusHandler;
		this.javaScriptDialogHandler = javaScriptDialogHandler;
		this.keyboardHandler = keyboardHandler;
		this.logger = logger;
		this.renderProcessMessageHandler = renderProcessMessageHandler;
		this.requestHandler = requestHandler;
	}

	public void Destroy()
	{
		if (!((IChromiumWebBrowserBase)control).IsDisposed)
		{
			WebBrowserExtensions.CloseDevTools((IChromiumWebBrowserBase)control);
			control.Dispose(disposing: true);
		}
	}

	public void ExecuteJavaScript(string code, Action<JavaScriptResult> callback = null)
	{
		try
		{
			if (((IChromiumWebBrowserBase)control).BrowserCore != null && ((IChromiumWebBrowserBase)control).BrowserCore.MainFrame != null)
			{
				WebBrowserExtensions.EvaluateScriptAsync(((IChromiumWebBrowserBase)control).BrowserCore, code, (TimeSpan?)null, false).ContinueWith((Task<JavascriptResponse> t) =>
				{
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0010: Unknown result type (might be due to invalid IL or missing references)
					//IL_0021: Unknown result type (might be due to invalid IL or missing references)
					//IL_0032: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Expected Obj, but got Unknown
					callback?.Invoke(new JavaScriptResult
					{
						Message = t.Result.Message,
						Result = t.Result.Result,
						Success = t.Result.Success
					});
				});
			}
			else
			{
				Task.Run(() =>
				{
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0010: Unknown result type (might be due to invalid IL or missing references)
					//IL_001b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0027: Expected Obj, but got Unknown
					callback?.Invoke(new JavaScriptResult
					{
						Message = "Could not execute JavaScript in main frame!",
						Success = false
					});
				});
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			Exception e = ex2;
			string message = "Failed to execute JavaScript in main frame!";
			logger.Error(message, e);
			Task.Run(() =>
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Expected Obj, but got Unknown
				callback?.Invoke(new JavaScriptResult
				{
					Message = message + " Reason: " + e.Message,
					Success = false
				});
			});
		}
	}

	public void Find(string term, bool isInitial, bool caseSensitive, bool forward = true)
	{
		WebBrowserExtensions.Find((IChromiumWebBrowserBase)control, term, forward, caseSensitive, !isInitial);
	}

	public void Initialize()
	{
		clipboard.Changed += Clipboard_Changed;
		((IWinFormsChromiumWebBrowser)control).AddressChanged += (object o, AddressChangedEventArgs e) =>
		{
			AddressChangedEventHandler val2 = this.m_AddressChanged;
			if (val2 != null)
			{
				val2.Invoke(e.Address);
			}
		};
		control.AuthCredentialsRequired += (IWebBrowser w, IBrowser b, string o, bool i, string h, int p, string r, string s, IAuthCallback c, GenericEventArgs a) =>
		{
			a.Value = requestHandler.GetAuthCredentials(w, b, o, i, h, p, r, s, c);
		};
		control.BeforeBrowse += (IWebBrowser w, IBrowser b, IFrame f, IRequest r, bool u, bool i, GenericEventArgs a) =>
		{
			a.Value = requestHandler.OnBeforeBrowse(w, b, f, r, u, i);
		};
		control.BeforeContextMenu += (IWebBrowser w, IBrowser b, IFrame f, IContextMenuParams p, IMenuModel m) =>
		{
			contextMenuHandler.OnBeforeContextMenu(w, b, f, p, m);
		};
		control.BeforeDownload += (IWebBrowser w, IBrowser b, DownloadItem d, IBeforeDownloadCallback c, GenericEventArgs a) =>
		{
			bool flag = (a.Value = downloadHandler.OnBeforeDownload(w, b, d, c));
			return a.Value = flag;
		};
		control.BeforeUnloadDialog += (IWebBrowser w, IBrowser b, string m, bool r, IJsDialogCallback c, GenericEventArgs a) =>
		{
			a.Value = javaScriptDialogHandler.OnBeforeUnloadDialog(w, b, m, r, c);
		};
		control.CanDownload += (IWebBrowser w, IBrowser b, string u, string r, GenericEventArgs a) =>
		{
			a.Value = downloadHandler.CanDownload(w, b, u, r);
		};
		control.ContextCreated += (IWebBrowser w, IBrowser b, IFrame f) =>
		{
			renderProcessMessageHandler.OnContextCreated(w, b, f);
		};
		control.ContextMenuCommand += (IWebBrowser w, IBrowser b, IFrame f, IContextMenuParams p, CefMenuCommand c, CefEventFlags e, GenericEventArgs a) =>
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			a.Value = contextMenuHandler.OnContextMenuCommand(w, b, f, p, c, e);
		};
		control.ContextMenuDismissed += (IWebBrowser w, IBrowser b, IFrame f) =>
		{
			contextMenuHandler.OnContextMenuDismissed(w, b, f);
		};
		control.ContextReleased += (IWebBrowser w, IBrowser b, IFrame f) =>
		{
			renderProcessMessageHandler.OnContextReleased(w, b, f);
		};
		control.DialogClosed += (IWebBrowser w, IBrowser b) =>
		{
			javaScriptDialogHandler.OnDialogClosed(w, b);
		};
		control.DownloadUpdated += (IWebBrowser w, IBrowser b, DownloadItem d, IDownloadItemCallback c) =>
		{
			downloadHandler.OnDownloadUpdated(w, b, d, c);
		};
		control.DragEnterCefSharp += (IWebBrowser w, IBrowser b, IDragData d, DragOperationsMask m, GenericEventArgs a) =>
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			a.Value = dragHandler.OnDragEnter(w, b, d, m);
		};
		control.DraggableRegionsChanged += (IWebBrowser w, IBrowser b, IFrame f, IList<DraggableRegion> r) =>
		{
			dragHandler.OnDraggableRegionsChanged(w, b, f, r);
		};
		control.FaviconUrlChanged += (IWebBrowser w, IBrowser b, IList<string> u) =>
		{
			displayHandler.OnFaviconUrlChange(w, b, u);
		};
		control.FileDialogRequested += (IWebBrowser w, IBrowser b, CefFileDialogMode m, string t, string p, IReadOnlyCollection<string> f, IReadOnlyCollection<string> e, IReadOnlyCollection<string> d, IFileDialogCallback c) =>
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			dialogHandler.OnFileDialog(w, b, m, t, p, f, e, d, c);
		};
		control.FocusedNodeChanged += (IWebBrowser w, IBrowser b, IFrame f, IDomNode n) =>
		{
			renderProcessMessageHandler.OnFocusedNodeChanged(w, b, f, n);
		};
		control.GotFocusCefSharp += (IWebBrowser w, IBrowser b) =>
		{
			focusHandler.OnGotFocus(w, b);
		};
		((IWinFormsChromiumWebBrowser)control).IsBrowserInitializedChanged += Control_IsBrowserInitializedChanged;
		control.JavaScriptDialog += delegate(IWebBrowser w, IBrowser b, string u, CefJsDialogType t, string m, string p, IJsDialogCallback c, ref bool s, GenericEventArgs a)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			a.Value = javaScriptDialogHandler.OnJSDialog(w, b, u, t, m, p, c, ref s);
		};
		control.KeyEvent += (IWebBrowser w, IBrowser b, KeyType t, int k, int n, CefEventFlags m, bool s) =>
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			keyboardHandler.OnKeyEvent(w, b, t, k, n, m, s);
		};
		((IChromiumWebBrowserBase)control).LoadError += (object o, LoadErrorEventArgs e) =>
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected I4, but got Unknown
			LoadFailedEventHandler val2 = this.m_LoadFailed;
			if (val2 != null)
			{
				val2.Invoke((int)e.ErrorCode, e.ErrorText, e.Frame.IsMain, e.FailedUrl);
			}
		};
		control.LoadingProgressChanged += (IWebBrowser w, IBrowser b, double p) =>
		{
			displayHandler.OnLoadingProgressChange(w, b, p);
		};
		((IChromiumWebBrowserBase)control).LoadingStateChanged += (object o, LoadingStateChangedEventArgs e) =>
		{
			LoadingStateChangedEventHandler val2 = this.m_LoadingStateChanged;
			if (val2 != null)
			{
				val2.Invoke(e.IsLoading);
			}
		};
		control.OpenUrlFromTab += (IWebBrowser w, IBrowser b, IFrame f, string u, WindowOpenDisposition t, bool g, GenericEventArgs a) =>
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			a.Value = requestHandler.OnOpenUrlFromTab(w, b, f, u, t, g);
		};
		control.PreKeyEvent += delegate(IWebBrowser w, IBrowser b, KeyType t, int k, int n, CefEventFlags m, bool i, ref bool s, GenericEventArgs a)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			a.Value = keyboardHandler.OnPreKeyEvent(w, b, t, k, n, m, i, ref s);
		};
		control.ResetDialogState += (IWebBrowser w, IBrowser b) =>
		{
			javaScriptDialogHandler.OnResetDialogState(w, b);
		};
		control.ResourceRequestHandlerRequired += delegate(IWebBrowser w, IBrowser b, IFrame f, IRequest r, bool n, bool d, string i, ref bool h, ResourceRequestEventArgs a)
		{
			a.Handler = requestHandler.GetResourceRequestHandler(w, b, f, r, n, d, i, ref h);
		};
		control.RunContextMenu += (IWebBrowser w, IBrowser b, IFrame f, IContextMenuParams p, IMenuModel m, IRunContextMenuCallback c, GenericEventArgs a) =>
		{
			a.Value = contextMenuHandler.RunContextMenu(w, b, f, p, m, c);
		};
		control.SetFocus += (IWebBrowser w, IBrowser b, CefFocusSource s, GenericEventArgs a) =>
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			a.Value = focusHandler.OnSetFocus(w, b, s);
		};
		control.TakeFocus += (IWebBrowser w, IBrowser b, bool n) =>
		{
			focusHandler.OnTakeFocus(w, b, n);
		};
		((IWinFormsChromiumWebBrowser)control).TitleChanged += (object o, TitleChangedEventArgs e) =>
		{
			TitleChangedEventHandler val2 = this.m_TitleChanged;
			if (val2 != null)
			{
				val2.Invoke(e.Title);
			}
		};
		control.UncaughtExceptionEvent += (IWebBrowser w, IBrowser b, IFrame f, JavascriptException e) =>
		{
			renderProcessMessageHandler.OnUncaughtException(w, b, f, e);
		};
		ICefSharpControl cefSharpControl = control;
		IWebBrowser val = (IWebBrowser)((cefSharpControl is IWebBrowser) ? cefSharpControl : null);
		if (val != null)
		{
			val.JavascriptMessageReceived += WebBrowser_JavascriptMessageReceived;
		}
	}

	public void NavigateBackwards()
	{
		((IChromiumWebBrowserBase)control).BrowserCore.GoBack();
	}

	public void NavigateForwards()
	{
		((IChromiumWebBrowserBase)control).BrowserCore.GoForward();
	}

	public void NavigateTo(string address)
	{
		control.Load(address);
	}

	public void ShowDeveloperConsole()
	{
		WebBrowserExtensions.ShowDevTools(((IChromiumWebBrowserBase)control).BrowserCore, (IWindowInfo)null, 0, 0);
	}

	public void Reload()
	{
		((IChromiumWebBrowserBase)control).BrowserCore.Reload(false);
	}

	public void Zoom(double level)
	{
		WebBrowserExtensions.SetZoomLevel(((IChromiumWebBrowserBase)control).BrowserCore, level);
	}

	private void Clipboard_Changed(string id)
	{
		try
		{
			string text = "SafeExamBrowser.clipboard.update('" + id + "', '" + clipboard.Content + "');";
			IBrowser browserCore = ((IChromiumWebBrowserBase)control).BrowserCore;
			IEnumerable<IFrame> enumerable = ((browserCore != null) ? browserCore.GetAllFrames() : null);
			foreach (IFrame item in enumerable ?? Enumerable.Empty<IFrame>())
			{
				item.EvaluateScriptAsync(text, "about:blank", 1, (TimeSpan?)null, false);
			}
		}
		catch (Exception ex)
		{
			logger.Error("Failed to update JavaScript clipboard!", ex);
		}
	}

	private void Control_IsBrowserInitializedChanged(object sender, EventArgs e)
	{
		if (((IChromiumWebBrowserBase)control).IsBrowserInitialized)
		{
			((IChromiumWebBrowserBase)control).BrowserCore.GetHost().SetFocus(true);
		}
	}

	private void WebBrowser_JavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
	{
		clipboard.Update(e);
	}
}
