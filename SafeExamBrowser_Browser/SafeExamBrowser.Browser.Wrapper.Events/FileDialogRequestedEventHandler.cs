using System.Collections.Generic;
using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void FileDialogRequestedEventHandler(IWebBrowser webBrowser, IBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, IReadOnlyCollection<string> acceptFilters, IReadOnlyCollection<string> acceptExtensions, IReadOnlyCollection<string> acceptDescriptions, IFileDialogCallback callback);
