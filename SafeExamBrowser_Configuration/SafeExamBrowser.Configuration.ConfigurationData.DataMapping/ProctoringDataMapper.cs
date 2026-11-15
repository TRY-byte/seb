using System;
using SafeExamBrowser.Settings;
using SafeExamBrowser.Settings.Proctoring;

namespace SafeExamBrowser.Configuration.ConfigurationData.DataMapping;

internal class ProctoringDataMapper : BaseDataMapper
{
	internal override void Map(string key, object value, AppSettings settings)
	{
		if (key == null)
		{
			return;
		}
		switch (key.Length)
		{
		case 24:
			switch (key[1])
			{
			case 'c':
				if (key == "screenProctoringClientId")
				{
					MapClientId(settings, value);
				}
				break;
			case 'h':
				if (key == "showProctoringViewButton")
				{
					MapShowTaskbarNotification(settings, value);
				}
				break;
			}
			break;
		case 37:
			switch (key[27])
			{
			case 'a':
				if (key == "screenProctoringScreenshotMaxInterval")
				{
					MapIntervalMaximum(settings, value);
				}
				break;
			case 'i':
				if (key == "screenProctoringScreenshotMinInterval")
				{
					MapIntervalMinimum(settings, value);
				}
				break;
			}
			break;
		case 25:
			if (key == "screenProctoringCacheSize")
			{
				MapCacheSize(settings, value);
			}
			break;
		case 28:
			if (key == "screenProctoringClientSecret")
			{
				MapClientSecret(settings, value);
			}
			break;
		case 22:
			if (key == "enableScreenProctoring")
			{
				MapScreenProctoringEnabled(settings, value);
			}
			break;
		case 23:
			if (key == "screenProctoringGroupId")
			{
				MapGroupId(settings, value);
			}
			break;
		case 30:
			if (key == "screenProctoringImageDownscale")
			{
				MapImageDownscaling(settings, value);
			}
			break;
		case 27:
			if (key == "screenProctoringImageFormat")
			{
				MapImageFormat(settings, value);
			}
			break;
		case 33:
			if (key == "screenProctoringImageQuantization")
			{
				MapImageQuantization(settings, value);
			}
			break;
		case 40:
			if (key == "screenProctoringMetadataActiveAppEnabled")
			{
				MapCaptureApplicationData(settings, value);
			}
			break;
		case 34:
			if (key == "screenProctoringMetadataURLEnabled")
			{
				MapCaptureBrowserData(settings, value);
			}
			break;
		case 42:
			if (key == "screenProctoringMetadataWindowTitleEnabled")
			{
				MapCaptureWindowTitle(settings, value);
			}
			break;
		case 26:
			if (key == "screenProctoringServiceURL")
			{
				MapServiceUrl(settings, value);
			}
			break;
		case 29:
		case 31:
		case 32:
		case 35:
		case 36:
		case 38:
		case 39:
		case 41:
			break;
		}
	}

	private void MapCacheSize(AppSettings settings, object value)
	{
		if (value is int cacheSize)
		{
			settings.Proctoring.ScreenProctoring.CacheSize = cacheSize;
		}
	}

	private void MapCaptureApplicationData(AppSettings settings, object value)
	{
		if (value is bool captureApplicationData)
		{
			settings.Proctoring.ScreenProctoring.MetaData.CaptureApplicationData = captureApplicationData;
		}
	}

	private void MapCaptureBrowserData(AppSettings settings, object value)
	{
		if (value is bool captureBrowserData)
		{
			settings.Proctoring.ScreenProctoring.MetaData.CaptureBrowserData = captureBrowserData;
		}
	}

	private void MapCaptureWindowTitle(AppSettings settings, object value)
	{
		if (value is bool captureWindowTitle)
		{
			settings.Proctoring.ScreenProctoring.MetaData.CaptureWindowTitle = captureWindowTitle;
		}
	}

	private void MapClientId(AppSettings settings, object value)
	{
		if (value is string clientId)
		{
			settings.Proctoring.ScreenProctoring.ClientId = clientId;
		}
	}

	private void MapClientSecret(AppSettings settings, object value)
	{
		if (value is string clientSecret)
		{
			settings.Proctoring.ScreenProctoring.ClientSecret = clientSecret;
		}
	}

	private void MapGroupId(AppSettings settings, object value)
	{
		if (value is string groupId)
		{
			settings.Proctoring.ScreenProctoring.GroupId = groupId;
		}
	}

	private void MapImageDownscaling(AppSettings settings, object value)
	{
		if (value is double imageDownscaling)
		{
			settings.Proctoring.ScreenProctoring.ImageDownscaling = imageDownscaling;
		}
	}

	private void MapImageFormat(AppSettings settings, object value)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (value is string value2 && Enum.TryParse<ImageFormat>(value2, true, out ImageFormat result))
		{
			settings.Proctoring.ScreenProctoring.ImageFormat = result;
		}
	}

	private void MapImageQuantization(AppSettings settings, object value)
	{
		if (value is int)
		{
			switch ((int)value)
			{
			case 0:
				settings.Proctoring.ScreenProctoring.ImageQuantization = (ImageQuantization)0;
				break;
			case 1:
				settings.Proctoring.ScreenProctoring.ImageQuantization = (ImageQuantization)4;
				break;
			case 2:
				settings.Proctoring.ScreenProctoring.ImageQuantization = (ImageQuantization)5;
				break;
			case 3:
				settings.Proctoring.ScreenProctoring.ImageQuantization = (ImageQuantization)6;
				break;
			case 4:
				settings.Proctoring.ScreenProctoring.ImageQuantization = (ImageQuantization)1;
				break;
			case 5:
				settings.Proctoring.ScreenProctoring.ImageQuantization = (ImageQuantization)2;
				break;
			case 6:
				settings.Proctoring.ScreenProctoring.ImageQuantization = (ImageQuantization)3;
				break;
			}
		}
	}

	private void MapIntervalMaximum(AppSettings settings, object value)
	{
		if (value is int intervalMaximum)
		{
			settings.Proctoring.ScreenProctoring.IntervalMaximum = intervalMaximum;
		}
	}

	private void MapIntervalMinimum(AppSettings settings, object value)
	{
		if (value is int intervalMinimum)
		{
			settings.Proctoring.ScreenProctoring.IntervalMinimum = intervalMinimum;
		}
	}

	private void MapScreenProctoringEnabled(AppSettings settings, object value)
	{
		settings.Proctoring.ScreenProctoring.Enabled = false;
	}

	private void MapServiceUrl(AppSettings settings, object value)
	{
		if (value is string serviceUrl)
		{
			settings.Proctoring.ScreenProctoring.ServiceUrl = serviceUrl;
		}
	}

	private void MapShowTaskbarNotification(AppSettings settings, object value)
	{
		if (value is bool showTaskbarNotification)
		{
			settings.Proctoring.ShowTaskbarNotification = showTaskbarNotification;
		}
	}
}
