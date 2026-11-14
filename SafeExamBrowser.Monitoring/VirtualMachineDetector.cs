using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using SafeExamBrowser.Configuration.Contracts.Integrity;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts;
using SafeExamBrowser.SystemComponents.Contracts;
using SafeExamBrowser.SystemComponents.Contracts.Registry;

namespace SafeExamBrowser.Monitoring;

public class VirtualMachineDetector : IVirtualMachineDetector
{
	private const string MANIPULATED = "000000000000";

	private const string QEMU_MAC_PREFIX = "525400";

	private const string VIRTUALBOX_MAC_PREFIX = "080027";

	private static readonly string[] DeviceBlacklist = new string[11]
	{
		"PROD_VIRTUAL", "HYPER_V", "qemu", "ven_1af4", "ven_1b36", "subsys_11001af4", "VEN_VBOX", "vid_80ee", "PROD_VMWARE", "VEN_VMWARE",
		"VMWARE_IDE"
	};

	private static readonly string[] DeviceWhitelist = new string[2] { "PROD_VIRTUAL_DISK", "PROD_VIRTUAL_DVD" };

	private static readonly string[] SystemHardware = new string[8] { "CIM_Memory", "CIM_NumericSensor", "CIM_Sensor", "CIM_TemperatureSensor", "CIM_VoltageSensor", "Win32_CacheMemory", "Win32_Fan", "Win32_VoltageProbe" };

	private readonly IIntegrityModule integrityModule;

	private readonly ILogger logger;

	private readonly IRegistry registry;

	private readonly ISystemInfo systemInfo;

	public VirtualMachineDetector(IIntegrityModule integrityModule, ILogger logger, IRegistry registry, ISystemInfo systemInfo)
	{
		this.integrityModule = integrityModule;
		this.logger = logger;
		this.registry = registry;
		this.systemInfo = systemInfo;
	}

	public bool IsVirtualMachine()
	{
		bool flag = false;
		string arg = default;
		int num = default;
		flag |= integrityModule.IsVirtualMachine(ref arg, ref num);
		logger.Debug("Computer '" + systemInfo.Name + "' appears " + (flag ? "" : "not ") + "to be a virtual machine" + (flag ? $" ({arg}, {num}%)" : "") + ".");
		return flag;
	}

	private bool HasNoSystemHardware()
	{
		bool flag = false;
		try
		{
			string[] systemHardware = SystemHardware;
			foreach (string text in systemHardware)
			{
				using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM " + text);
				using ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
				flag |= managementObjectCollection.Count > 0;
			}
		}
		catch (Exception ex)
		{
			flag = false;
			logger.Error("Failed to query system hardware!", ex);
		}
		return !flag;
	}

	private bool HasVirtualDevice()
	{
		bool flag = false;
		string[] plugAndPlayDeviceIds = systemInfo.PlugAndPlayDeviceIds;
		foreach (string device in plugAndPlayDeviceIds)
		{
			flag |= DeviceBlacklist.Any((string d) => device.ToLower().Contains(d.ToLower())) && DeviceWhitelist.All((string d) => !device.ToLower().Contains(d.ToLower()));
		}
		return flag;
	}

	private bool HasVirtualMacAddress()
	{
		bool flag = false;
		string macAddress = systemInfo.MacAddress;
		if (macAddress != null && macAddress.Length > 2)
		{
			flag |= macAddress.StartsWith("000000000000");
			flag |= macAddress.StartsWith("525400");
			flag |= macAddress.StartsWith("080027");
		}
		return flag;
	}

	private bool IsVirtualCpu()
	{
		return (byte)(0u | (systemInfo.CpuName.ToLower().Contains(" kvm ") ? 1u : 0u)) != 0;
	}

	private bool IsVirtualRegistry()
	{
		return (byte)(0u | (HasLocalVirtualMachineDeviceCache() ? 1u : 0u)) != 0;
	}

	private bool IsVirtualSystem(string biosInfo, string manufacturer, string model)
	{
		biosInfo = biosInfo.ToLower();
		manufacturer = manufacturer.ToLower();
		model = model.ToLower();
		return (byte)(0u | (biosInfo.Contains("hyper-v") ? 1u : 0u) | (biosInfo.Contains("virtualbox") ? 1u : 0u) | (biosInfo.Contains("vmware") ? 1u : 0u) | (biosInfo.Contains("ovmf") ? 1u : 0u) | (biosInfo.Contains("edk ii unknown") ? 1u : 0u) | ((manufacturer.Contains("microsoft corporation") && !model.Contains("surface")) ? 1u : 0u) | (manufacturer.Contains("parallels software") ? 1u : 0u) | (manufacturer.Contains("qemu") ? 1u : 0u) | (manufacturer.Contains("vmware") ? 1u : 0u) | (model.Contains("virtualbox") ? 1u : 0u) | (model.Contains("Q35 +") ? 1u : 0u)) != 0;
	}

	private bool HasLocalVirtualMachineDeviceCache()
	{
		string environmentVariable = Environment.GetEnvironmentVariable("COMPUTERNAME");
		bool flag = false;
		IEnumerable<string> enumerable = default;
		bool flag2 = registry.TryGetSubKeys("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\TaskFlow\\DeviceCache", ref enumerable);
		if ((environmentVariable != null) & flag2)
		{
			object obj = default;
			object obj2 = default;
			object obj3 = default;
			foreach (string item in enumerable)
			{
				string text = "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\TaskFlow\\DeviceCache\\" + item;
				bool flag3 = true;
				flag3 &= registry.TryRead(text, "DeviceName", ref obj);
				if (flag3 && environmentVariable.ToLower() == ((string)obj).ToLower())
				{
					flag3 &= registry.TryRead(text, "DeviceMake", ref obj2);
					if (flag3 & registry.TryRead(text, "DeviceModel", ref obj3))
					{
						flag |= IsVirtualSystem("", (string)obj2, (string)obj3);
					}
				}
			}
		}
		return flag;
	}
}
