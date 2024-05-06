using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BafflerStandalone
{
	public class Win32
	{
		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
		public static extern int SetWindowsHookEx(int idHook, Win32.WindowsHookProc lpfn, IntPtr hInstance, int threadId);

		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
		public static extern bool UnhookWindowsHookEx(int idHook);

		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
		public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll")]
		public static extern uint GetDlgItemText(IntPtr hDlg, int nIDDlgItem, [Out] StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr GetParent(IntPtr hWnd);

		[DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool GetWindowRect(IntPtr handle, ref Win32.RECT r);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(string dllToLoad);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		[DllImport("kernel32.dll")]
		public static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		public static string GetClassName(IntPtr hWnd)
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			Win32.GetClassName(hWnd, stringBuilder, stringBuilder.Capacity);
			return stringBuilder.ToString();
		}

		public static string GetWindowText(IntPtr hWnd)
		{
			int windowTextLength = Win32.GetWindowTextLength(hWnd);
			StringBuilder stringBuilder = new StringBuilder(windowTextLength + 1);
			Win32.GetWindowText(hWnd, stringBuilder, stringBuilder.Capacity);
			return stringBuilder.ToString();
		}

		public static string GetDlgItemText(IntPtr hDlg, int nIDDlgItem)
		{
			IntPtr dlgItem = Win32.GetDlgItem(hDlg, nIDDlgItem);
			if (dlgItem == IntPtr.Zero)
			{
				return null;
			}
			int windowTextLength = Win32.GetWindowTextLength(dlgItem);
			StringBuilder stringBuilder = new StringBuilder(windowTextLength + 1);
			Win32.GetWindowText(dlgItem, stringBuilder, stringBuilder.Capacity);
			return stringBuilder.ToString();
		}

		public const int WH_CBT = 5;
		public const int HCBT_ACTIVATE = 5;

		public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}

		public delegate int WindowsHookProc(int nCode, IntPtr wParam, IntPtr lParam);
		
		public class Bit
		{
			public static int HiWord(int iValue)
			{
				return iValue >> 16 & 0xFFFF;
			}

			public static int LoWord(int iValue)
			{
				return iValue & 0xFFFF;
			}
		}
	}
}
