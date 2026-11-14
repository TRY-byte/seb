using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace patch_seb.Properties
{
	// Token: 0x02000007 RID: 7
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "18.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x06000018 RID: 24 RVA: 0x00003D70 File Offset: 0x00001F70
		internal Resources()
		{
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00003D78 File Offset: 0x00001F78
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("patch_seb.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00003DA4 File Offset: 0x00001FA4
		// (set) Token: 0x0600001B RID: 27 RVA: 0x00003DAB File Offset: 0x00001FAB
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600001C RID: 28 RVA: 0x00003DB3 File Offset: 0x00001FB3
		internal static string cert
		{
			get
			{
				return Resources.ResourceManager.GetString("cert", Resources.resourceCulture);
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00003DC9 File Offset: 0x00001FC9
		internal static string cert_offline
		{
			get
			{
				return Resources.ResourceManager.GetString("cert_offline", Resources.resourceCulture);
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600001E RID: 30 RVA: 0x00003DDF File Offset: 0x00001FDF
		internal static byte[] SafeExamBrowser
		{
			get
			{
				return (byte[])Resources.ResourceManager.GetObject("SafeExamBrowser", Resources.resourceCulture);
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00003DFA File Offset: 0x00001FFA
		internal static byte[] SafeExamBrowser_Browser
		{
			get
			{
				return (byte[])Resources.ResourceManager.GetObject("SafeExamBrowser_Browser", Resources.resourceCulture);
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000020 RID: 32 RVA: 0x00003E15 File Offset: 0x00002015
		internal static byte[] SafeExamBrowser_Browser1
		{
			get
			{
				return (byte[])Resources.ResourceManager.GetObject("SafeExamBrowser_Browser1", Resources.resourceCulture);
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00003E30 File Offset: 0x00002030
		internal static byte[] SafeExamBrowser_Client
		{
			get
			{
				return (byte[])Resources.ResourceManager.GetObject("SafeExamBrowser_Client", Resources.resourceCulture);
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000022 RID: 34 RVA: 0x00003E4B File Offset: 0x0000204B
		internal static byte[] SafeExamBrowser_Client1
		{
			get
			{
				return (byte[])Resources.ResourceManager.GetObject("SafeExamBrowser_Client1", Resources.resourceCulture);
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00003E66 File Offset: 0x00002066
		internal static byte[] SafeExamBrowser_Configuration
		{
			get
			{
				return (byte[])Resources.ResourceManager.GetObject("SafeExamBrowser_Configuration", Resources.resourceCulture);
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000024 RID: 36 RVA: 0x00003E81 File Offset: 0x00002081
		internal static byte[] SafeExamBrowser_Configuration1
		{
			get
			{
				return (byte[])Resources.ResourceManager.GetObject("SafeExamBrowser_Configuration1", Resources.resourceCulture);
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000025 RID: 37 RVA: 0x00003E9C File Offset: 0x0000209C
		internal static byte[] SafeExamBrowser_Monitoring
		{
			get
			{
				return (byte[])Resources.ResourceManager.GetObject("SafeExamBrowser_Monitoring", Resources.resourceCulture);
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00003EB7 File Offset: 0x000020B7
		internal static byte[] SafeExamBrowser_Monitoring1
		{
			get
			{
				return (byte[])Resources.ResourceManager.GetObject("SafeExamBrowser_Monitoring1", Resources.resourceCulture);
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00003ED2 File Offset: 0x000020D2
		internal static byte[] SafeExamBrowser1
		{
			get
			{
				return (byte[])Resources.ResourceManager.GetObject("SafeExamBrowser1", Resources.resourceCulture);
			}
		}

		// Token: 0x04000024 RID: 36
		private static ResourceManager resourceMan;

		// Token: 0x04000025 RID: 37
		private static CultureInfo resourceCulture;
	}
}
