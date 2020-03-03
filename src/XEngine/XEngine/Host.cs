using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace XEngine
{
	public static class Host
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

		internal static bool CurrentApplicationIsActive
		{
			get
			{
				var activatedHandle = GetForegroundWindow();
				if (activatedHandle == IntPtr.Zero) return false;
				GetWindowThreadProcessId(activatedHandle, out var activeProcId);
				return activeProcId == Process.GetCurrentProcess().Id;
			}
		}
	}
}
