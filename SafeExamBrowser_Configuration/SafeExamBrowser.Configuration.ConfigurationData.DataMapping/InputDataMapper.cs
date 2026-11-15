using SafeExamBrowser.Settings;

namespace SafeExamBrowser.Configuration.ConfigurationData.DataMapping;

internal class InputDataMapper : BaseDataMapper
{
	public static bool enabled = true;

	internal override void Map(string key, object value, AppSettings settings)
	{
		if (key == null)
		{
			return;
		}
		switch (key.Length)
		{
		case 12:
			switch (key[9])
			{
			case 'E':
				if (key == "enableAltEsc")
				{
					MapEnableAltEsc(settings, value);
				}
				break;
			case 'T':
				if (key == "enableAltTab")
				{
					MapEnableAltTab(settings, value);
				}
				break;
			}
			break;
		case 9:
			switch (key[8])
			{
			case 'c':
				if (key == "enableEsc")
				{
					MapEnableEsc(settings, value);
				}
				break;
			case '0':
				if (key == "enableF10")
				{
					MapEnableF10(settings, value);
				}
				break;
			case '1':
				if (key == "enableF11")
				{
					MapEnableF11(settings, value);
				}
				break;
			case '2':
				if (key == "enableF12")
				{
					MapEnableF12(settings, value);
				}
				break;
			}
			break;
		case 8:
			switch (key[7])
			{
			case '1':
				if (key == "enableF1")
				{
					MapEnableF1(settings, value);
				}
				break;
			case '2':
				if (key == "enableF2")
				{
					MapEnableF2(settings, value);
				}
				break;
			case '3':
				if (key == "enableF3")
				{
					MapEnableF3(settings, value);
				}
				break;
			case '4':
				if (key == "enableF4")
				{
					MapEnableF4(settings, value);
				}
				break;
			case '5':
				if (key == "enableF5")
				{
					MapEnableF5(settings, value);
				}
				break;
			case '6':
				if (key == "enableF6")
				{
					MapEnableF6(settings, value);
				}
				break;
			case '7':
				if (key == "enableF7")
				{
					MapEnableF7(settings, value);
				}
				break;
			case '8':
				if (key == "enableF8")
				{
					MapEnableF8(settings, value);
				}
				break;
			case '9':
				if (key == "enableF9")
				{
					MapEnableF9(settings, value);
				}
				break;
			}
			break;
		case 17:
			switch (key[6])
			{
			case 'P':
				if (key == "enablePrintScreen")
				{
					MapEnablePrintScreen(settings, value);
				}
				break;
			case 'M':
				if (key == "enableMiddleMouse")
				{
					MapEnableMiddleMouseButton(settings, value);
				}
				break;
			}
			break;
		case 11:
			if (key == "enableAltF4")
			{
				MapEnableAltF4(settings, value);
			}
			break;
		case 13:
			if (key == "enableCtrlEsc")
			{
				MapEnableCtrlEsc(settings, value);
			}
			break;
		case 15:
			if (key == "enableStartMenu")
			{
				MapEnableSystemKey(settings, value);
			}
			break;
		case 16:
			if (key == "enableRightMouse")
			{
				MapEnableRightMouseButton(settings, value);
			}
			break;
		case 10:
		case 14:
			break;
		}
	}

	private void MapEnableAltEsc(AppSettings settings, object value)
	{
		settings.Keyboard.AllowAltEsc = enabled;
	}

	private void MapEnableAltF4(AppSettings settings, object value)
	{
		settings.Keyboard.AllowAltF4 = enabled;
	}

	private void MapEnableAltTab(AppSettings settings, object value)
	{
		settings.Keyboard.AllowAltTab = enabled;
	}

	private void MapEnableCtrlEsc(AppSettings settings, object value)
	{
		settings.Keyboard.AllowCtrlEsc = enabled;
	}

	private void MapEnableEsc(AppSettings settings, object value)
	{
		settings.Keyboard.AllowEsc = enabled;
	}

	private void MapEnableF1(AppSettings settings, object value)
	{
		settings.Keyboard.AllowF1 = enabled;
	}

	private void MapEnableF2(AppSettings settings, object value)
	{
		settings.Keyboard.AllowF2 = enabled;
	}

	private void MapEnableF3(AppSettings settings, object value)
	{
		settings.Keyboard.AllowF3 = enabled;
	}

	private void MapEnableF4(AppSettings settings, object value)
	{
		settings.Keyboard.AllowF4 = enabled;
	}

	private void MapEnableF5(AppSettings settings, object value)
	{
		settings.Keyboard.AllowF5 = enabled;
	}

	private void MapEnableF6(AppSettings settings, object value)
	{
		settings.Keyboard.AllowF6 = enabled;
	}

	private void MapEnableF7(AppSettings settings, object value)
	{
		settings.Keyboard.AllowF7 = enabled;
	}

	private void MapEnableF8(AppSettings settings, object value)
	{
		settings.Keyboard.AllowF8 = enabled;
	}

	private void MapEnableF9(AppSettings settings, object value)
	{
		settings.Keyboard.AllowF9 = enabled;
	}

	private void MapEnableF10(AppSettings settings, object value)
	{
		settings.Keyboard.AllowF10 = enabled;
	}

	private void MapEnableF11(AppSettings settings, object value)
	{
		settings.Keyboard.AllowF11 = enabled;
	}

	private void MapEnableF12(AppSettings settings, object value)
	{
		settings.Keyboard.AllowF12 = enabled;
	}

	private void MapEnablePrintScreen(AppSettings settings, object value)
	{
		settings.Keyboard.AllowPrintScreen = enabled;
	}

	private void MapEnableSystemKey(AppSettings settings, object value)
	{
		settings.Keyboard.AllowSystemKey = enabled;
	}

	private void MapEnableMiddleMouseButton(AppSettings settings, object value)
	{
		settings.Mouse.AllowMiddleButton = enabled;
	}

	private void MapEnableRightMouseButton(AppSettings settings, object value)
	{
		settings.Mouse.AllowRightButton = enabled;
	}
}
