using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data;

namespace RemoteServer
{
	public class Program
	{
		internal static Properties.Settings Settings { get { return Properties.Settings.Default; } }
		public const string logFile = "log.txt";
		public const string AppName = "FTP Sync";
		protected NotifyIcon icon;

		[STAThread]
		static void Main(string[] args)
		{
			var p = new Program();
			Application.ThreadException += (o, e) => p.Log(e.Exception);			
			Application.Run();
		}

		public Program()
		{
			InitTrayIcon();
			//Log(String.Format("Sync started. Local path: {0}, Remote path: {1}", Settings.LocalPath, Settings.RemotePath));
		}

		private void InitTrayIcon()
		{
			ContextMenuStrip context = new ContextMenuStrip();
			context.Items.AddRange(new ToolStripItem[] 
			{
				new ToolStripMenuItem("Show log", null, (q, w) => Process.Start(logFile)),
				new ToolStripMenuItem("Clear log", null, (q, w) => File.Delete(logFile)),
				new ToolStripMenuItem("Open app location", null, (q, w) => Process.Start(Path.GetDirectoryName(Application.ExecutablePath))),
				new ToolStripMenuItem("Exit", null, (q, w) => Application.Exit())
			});

			icon = new NotifyIcon()
			{
				Icon = Properties.Resources.synchronize,
				Text = AppName,
				ContextMenuStrip = context,
				Visible = true
			};
			icon.MouseClick += (obj, args) =>
			{
				if (args.Button == MouseButtons.Left)
					Process.Start(logFile);
			};

			Application.ApplicationExit += (obj, args) =>
			{
				icon.Dispose();
				context.Dispose();
			};
		}

		public void Log(string message)
		{
			File.AppendAllText(logFile, String.Format("[{0}] {1}\r\n", DateTime.Now, message));
		}
		public void Log(Exception e)
		{
			Log(String.Format("Exception: {0}\r\n{1}", e.Message, e.StackTrace));
			if (icon != null)
				icon.ShowBalloonTip(1000, "Error", e.Message, ToolTipIcon.Error);
		}
	}
}
