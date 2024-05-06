using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;
using AxShockwaveFlashObjects;
using Microsoft.Win32;
using BafflerStandalone.Database;
using BafflerStandalone.Forms.Options;
using BafflerStandalone.Forms.ServerList;
using BafflerStandalone.Forms.QuickAlign;
using BafflerStandalone.Database.RSS;
using System.Text;
using System.Net.Sockets;

namespace BafflerStandalone
{
	public partial class MainForm : Form, IMessageFilter
	{
		private const string GAME_URL = "http://www.transformice.com/TransformiceChargeur.swf";
		private readonly List<TabInfo> tabList = new List<TabInfo>();
		private int selectedTabIndex = -1;
		private readonly RssManager reader = new RssManager();
		private Collection<Rss.Items> rsslist;
		private string windowTitle = "";
		private Size oldSize = new Size(800, 600);
		private Point oldPos = new Point(0, 0);
		private FormWindowState oldWinState;
		private Keys LastKeyCode;
		private bool canProcessHotKeys = true;
		private bool mRepeating;
		private bool inFullscreen;
		private Options fOptions;
		private QuickAlign frmQuickAlign;
		private readonly StickyWindow stickyWindow;

		[DllImport("user32.dll")]
		private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

		public MainForm()
		{
			this.InitializeComponent();
			Application.AddMessageFilter(this);
			this.stickyWindow = new StickyWindow(this)
			{
				StickOnMove = true,
				StickOnResize = false,
				StickToOther = false,
				StickToScreen = true
			};
		}

		private void NewServerTab(string label, string url)
		{
			this.panel1.Hide();
			foreach (TabInfo tabInfo in this.tabList)
			{
				tabInfo.swf.Hide();
			}
			FlashPlayer flashPlayer = new FlashPlayer();
			flashPlayer.BeginInit();
			flashPlayer.FlashCall += this.TmpSWF_FlashCall;
			flashPlayer.OnReadyStateChange += this.TmpSWF_OnReadyStateChange;
			flashPlayer.Location = new Point(0, 0);
			flashPlayer.Margin = new Padding(0);
			flashPlayer.Name = "Flash";
			flashPlayer.Size = base.ClientSize;
			flashPlayer.EndInit();
			base.Controls.Add(flashPlayer);
			flashPlayer.ScaleMode = 3;
			flashPlayer.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			flashPlayer.BackgroundColor = 6911633;
			flashPlayer.AlignMode = Database.Database.Instance.Alignment;
			ToolStripButton toolStripButton = new ToolStripButton(label);
			toolStripButton.Click += this.NewBtn_Click;
			this.toolStrip1.Items.Add(toolStripButton);
			TabInfo tabInfo2 = new TabInfo(toolStripButton, flashPlayer);
			this.tabList.Add(tabInfo2);
			int tabIndex = this.tabList.IndexOf(tabInfo2);
			tabInfo2.LoadMovie(url);
			if (this.tabList.Count > 1)
			{
				this.toolStrip1.Visible = true;
			}
			this.closeTabToolStripMenuItem.Visible = true;
			this.viewToolStripMenuItem.Enabled = true;
			this.refreshToolStripMenuItem2.Enabled = true;
			this.renameToolStripMenuItem2.Enabled = true;

			this.SelectTab(tabIndex);
			flashPlayer.Focus();
		}

		private int GetFlashPlayerVersion()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Macromedia\\FlashPlayer"))
			{
				if (registryKey != null)
				{
					string text = registryKey.GetValue("CurrentVersion") as string;
					if (!string.IsNullOrEmpty(text))
					{
						int num = text.IndexOf(",");
						if (num > 0 && int.TryParse(text.Substring(0, num), out int result))
						{
							return result;
						}
					}
				}
			}
			return 0;
		}
		public static DialogResult InputBox(string title, string promptText, ref string value)
		{
			Form form = new Form();
			Label label = new Label();
			TextBox textBox = new TextBox();
			Button button = new Button();
			Button button2 = new Button();
			form.Text = title;
			label.Text = promptText;
			textBox.Text = value;
			button.Text = "OK";
			button2.Text = "Cancel";
			button.DialogResult = DialogResult.OK;
			button2.DialogResult = DialogResult.Cancel;
			label.SetBounds(9, 20, 372, 13);
			textBox.SetBounds(12, 36, 372, 20);
			button.SetBounds(228, 72, 75, 23);
			button2.SetBounds(309, 72, 75, 23);
			label.AutoSize = true;
			textBox.Anchor |= AnchorStyles.Right;
			button.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			button2.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			form.ClientSize = new Size(396, 107);
			form.Controls.AddRange(new Control[] { label, textBox, button, button2 });
			form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
			form.FormBorderStyle = FormBorderStyle.FixedDialog;
			form.StartPosition = FormStartPosition.CenterScreen;
			form.MinimizeBox = false;
			form.MaximizeBox = false;
			form.AcceptButton = button;
			form.CancelButton = button2;
			DialogResult result = form.ShowDialog();
			value = textBox.Text;
			return result;
		}
		private int GetIndexFromBtn(ToolStripButton btn)
		{
			for (int i = 0; i < this.tabList.Count; i++)
			{
				if (this.tabList[i].button == btn)
				{
					return i;
				}
			}
			return 0;
		}

		private void SelectTab(ToolStripButton btn)
		{
			int num = this.selectedTabIndex;
			this.selectedTabIndex = this.GetIndexFromBtn(btn);
			this.windowTitle = btn.Text + " - Baffler's Standalone";
			this.Text = this.windowTitle;
			if (num != this.selectedTabIndex)
			{
				foreach (TabInfo tabInfo in this.tabList)
				{
					tabInfo.button.BackColor = Color.FromKnownColor(KnownColor.Control);
					tabInfo.swf.Hide();
				}
				btn.BackColor = Color.CornflowerBlue;
				this.tabList[this.selectedTabIndex].swf.Show();
				this.tabList[this.selectedTabIndex].swf.Focus();
			}
		}

		private void SelectTab(int tabIndex)
		{
			int num = this.selectedTabIndex;
			this.selectedTabIndex = tabIndex;
			if (num != this.selectedTabIndex)
			{
				foreach (TabInfo tabInfo in this.tabList)
				{
					tabInfo.button.BackColor = Color.FromKnownColor(KnownColor.Control);
					tabInfo.swf.Hide();
				}
				this.windowTitle = this.tabList[tabIndex].button.Text + " - Baffler's Standalone";
				this.Text = this.windowTitle;
				this.tabList[tabIndex].button.BackColor = Color.CornflowerBlue;
				this.tabList[tabIndex].swf.Show();
				this.tabList[tabIndex].swf.Focus();
			}
		}

		private void NewBtn_Click(object sender, EventArgs e)
		{
			if (sender as ToolStripButton != this.tabList[this.selectedTabIndex].button)
			{
				this.SelectTab(sender as ToolStripButton);
				this.windowTitle = (sender as ToolStripButton).Text + " - Baffler's Standalone";
				this.Text = this.windowTitle;
			}
			this.tabList[this.selectedTabIndex].swf.Focus();
		}

		private void SelectTabServer(string label, string url)
		{
			if (this.selectedTabIndex < 0)
			{
				return;
			}
			this.panel1.Hide();
			foreach (TabInfo tabInfo in this.tabList)
			{
				tabInfo.swf.Hide();
			}
			base.Controls.Remove(this.tabList[this.selectedTabIndex].swf);
			this.tabList[this.selectedTabIndex].swf.Dispose();
			this.tabList[this.selectedTabIndex].swf = null;
			FlashPlayer flashPlayer = new FlashPlayer();
			flashPlayer.BeginInit();
			flashPlayer.FlashCall += this.TmpSWF_FlashCall;
			flashPlayer.OnReadyStateChange += this.TmpSWF_OnReadyStateChange;
			flashPlayer.Location = new Point(0, 0);
			flashPlayer.Margin = new Padding(0);
			flashPlayer.Name = "Flash";
			flashPlayer.Size = base.ClientSize;
			flashPlayer.EndInit();
			base.Controls.Add(flashPlayer);
			this.tabList[this.selectedTabIndex].swf = flashPlayer;
			flashPlayer.ScaleMode = 3;
			flashPlayer.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			flashPlayer.BackgroundColor = 6911633;
			flashPlayer.AlignMode = Database.Database.Instance.Alignment;
			this.windowTitle = label + " - Baffler's Standalone";
			this.Text = this.windowTitle;
			this.tabList[this.selectedTabIndex].button.Text = label;
			this.tabList[this.selectedTabIndex].LoadMovie(url);
			flashPlayer.Focus();
		}

		private void TmpSWF_OnReadyStateChange(object sender, _IShockwaveFlashEvents_OnReadyStateChangeEvent e)
		{
			if (e.newState == 4)
			{
				(sender as FlashPlayer).AlignMode = Database.Database.Instance.Alignment;
			}
		}

		private void TmpSWF_FlashCall(object sender, _IShockwaveFlashEvents_FlashCallEvent e)
		{
			if (e.request.ToString() == "<invoke name=\"recupLangue\" returntype=\"xml\"><arguments></arguments></invoke>")
			{
				(sender as FlashPlayer).SetReturnValue("<string>EN</string>");
				return;
			}
			if (e.request.ToString() == "<invoke name=\"function(){return navigator.appVersion+'-'+navigator.appName;}\" returntype=\"xml\"><arguments></arguments></invoke>")
			{
				(sender as FlashPlayer).SetReturnValue("<string>5.0 (Windows)-Netscape</string>");
				return;
			}
			if (e.request.ToString() == "<invoke name=\"window.location.href.toString\" returntype=\"xml\"><arguments></arguments></invoke>")
			{
				string autoJoinRoom = Database.Database.Instance.AutoJoinRoom;
				if (autoJoinRoom.Length > 0)
				{
					(sender as FlashPlayer).SetReturnValue("<string>http://www.transformice.com/?salon=" + autoJoinRoom + "</string>");
					return;
				}
				(sender as FlashPlayer).SetReturnValue("<string>http://www.transformice.com/</string>");
			}
		}

		private bool RefreshRSS()
		{
			try
			{
				this.reader.Url = "https://scammer.info/latest.rss";
				this.reader.GetFeed();
				this.rsslist = this.reader.RssItems;
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			string text = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Transforhook.dll");
			if (File.Exists(text))
			{
				Win32.LoadLibrary(text);
			}
			try
			{
				Database.Database.Instance.LoadSettings();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			this.tbAutoJoin.Text = Database.Database.Instance.AutoJoinRoom;
			if (Database.Database.Instance.SnapToEdges)
			{
				this.stickyWindow.StickOnMove = true;
				this.stickyWindow.StickOnResize = false;
				this.stickyWindow.StickToOther = false;
				this.stickyWindow.StickToScreen = true;
			}
			else
			{
				this.stickyWindow.StickOnMove = false;
				this.stickyWindow.StickOnResize = false;
				this.stickyWindow.StickToOther = false;
				this.stickyWindow.StickToScreen = false;
			}
			this.toolStrip1.Left = this.menuStrip1.Right;
			this.fOptions = new Options();
			this.frmQuickAlign = new QuickAlign();
			string text2 = "<html>\n<body>\n";
			text2 += "Please wait for news feed to load...";
			text2 += "</body>\n</html>";
			this.WriteDynamicHTMLToWebBrowser(text2);
			this.webBrowser1.Update();
			System.Timers.Timer timer = new System.Timers.Timer(100.0);
			timer.Elapsed += this.LoadRSS_Elapsed;
			timer.Start();
		}

		private void WriteDynamicHTMLToWebBrowser(string html)
		{
			this.webBrowser1.AllowNavigation = false;
			if (this.webBrowser1.Document != null)
			{
				this.webBrowser1.Document.OpenNew(true);
			}
			else
			{
				this.webBrowser1.Navigate("about:blank");
			}
			this.webBrowser1.DocumentText = html;
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			if (Database.Database.Instance.AutoJoinRoom != this.tbAutoJoin.Text)
			{
				Database.Database.Instance.AutoJoinRoom = this.tbAutoJoin.Text;
			}
			this.NewServerTab("English (1)", "http://en.transformice.com/ChargeurTransformice.swf");
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Application.RemoveMessageFilter(this);
			Database.Database.Instance.SaveSettings();
		}

		private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			HtmlElementCollection links = this.webBrowser1.Document.Links;
			foreach (object obj in links)
			{
				HtmlElement htmlElement = (HtmlElement)obj;
				htmlElement.Click += this.Element_Click;
			}
		}

		private void Element_Click(object sender, HtmlElementEventArgs e)
		{
			Process.Start((sender as HtmlElement).GetAttribute("href"));
		}

		private void ShowOptions()
		{
			this.fOptions.loadSettings();
			bool flag = false;
			if (this.fOptions.ShowDialog() == DialogResult.OK)
			{
				flag = true;
			}
			if (flag)
			{
				foreach (TabInfo tabInfo in this.tabList)
				{
					tabInfo.swf.AlignMode = Database.Database.Instance.Alignment;
				}
				if (Database.Database.Instance.SnapToEdges)
				{
					this.stickyWindow.StickOnMove = true;
					this.stickyWindow.StickOnResize = false;
					this.stickyWindow.StickToOther = false;
					this.stickyWindow.StickToScreen = true;
				}
				else
				{
					this.stickyWindow.StickOnMove = false;
					this.stickyWindow.StickOnResize = false;
					this.stickyWindow.StickToOther = false;
					this.stickyWindow.StickToScreen = false;
				}
				base.Focus();
				if (this.selectedTabIndex > -1)
				{
					this.tabList[this.selectedTabIndex].swf.Focus();
				}
			}
		}

		private void Button1_Click_1(object sender, EventArgs e)
		{
			this.ShowOptions();
		}

		private void GoFullScreen()
		{
			if (!this.inFullscreen)
			{
				this.oldWinState = base.WindowState;
				if (Database.Database.Instance.FsBorderless)
				{
					this.oldSize.Width = base.ClientSize.Width;
					this.oldSize.Height = base.ClientSize.Height;
				}
				else
				{
					this.oldSize.Width = base.Width;
					this.oldSize.Height = base.Height;
				}
				this.oldPos = base.Location;
				this.menuStrip1.Visible = false;
				this.inFullscreen = true;
				base.WindowState = FormWindowState.Normal;
				if (Database.Database.Instance.FsBorderless)
				{
					base.FormBorderStyle = FormBorderStyle.None;
				}
				if (Database.Database.Instance.FsExpand)
				{
					if (Database.Database.Instance.FsBorderless)
					{
						Rectangle bounds = Screen.GetBounds(this);
						base.Location = bounds.Location;
						base.Size = bounds.Size;
						return;
					}
					base.WindowState = FormWindowState.Maximized;
				}
			}
		}

		private void ExitFullScreen()
		{
			if (this.inFullscreen)
			{
				base.Size = this.oldSize;
				base.Location = this.oldPos;
				base.WindowState = this.oldWinState;
				this.inFullscreen = false;
				base.FormBorderStyle = FormBorderStyle.Sizable;
			}
		}

		private void ToggleFullScreen()
		{
			if (this.inFullscreen)
			{
				this.ExitFullScreen();
				return;
			}
			this.GoFullScreen();
		}

		private void ScreenShot()
		{
			bool flag = false;
			if (Database.Database.Instance.ScreenshotDir == null || Database.Database.Instance.ScreenshotDir.Length < 1)
			{
				this.canProcessHotKeys = false;
				if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
				{
					Database.Database.Instance.ScreenshotDir = this.folderBrowserDialog1.SelectedPath;
					flag = true;
				}
				this.canProcessHotKeys = true;
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				FlashPlayer swf = this.tabList[this.selectedTabIndex].swf;
				Rectangle bounds = swf.Bounds;
				string str = DateTime.Now.ToString(Database.Database.Instance.FileTimeFormat);
				string text = Database.Database.Instance.FileNamePrefix + str + ".png";
				try
				{
					using (Bitmap bitmap = new Bitmap(bounds.Width - 1, bounds.Height - 1, PixelFormat.Format32bppArgb))
					{
						using (Graphics graphics = Graphics.FromImage(bitmap))
						{
							IntPtr hdc = graphics.GetHdc();
							MainForm.PrintWindow(swf.Handle, hdc, 0U);
							graphics.ReleaseHdc(hdc);
							graphics.Flush();
							bitmap.Save(Database.Database.Instance.ScreenshotDir + "\\" + text, ImageFormat.Png);
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Could not save the screenshot!\n\n" + ex.Message, "Error Saving Screenshot", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				this.lbScreenShot.Text = text;
				this.lbScreenShot.Visible = true;
				this.timer_ScreenshotLabel.Enabled = false;
				this.timer_ScreenshotLabel.Enabled = true;
				return;
			}
			MessageBox.Show("You must choose a screenshot directory before you can take screenshots.", "Choose Directory", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}

		private void ProcessHotKey(object sender, HotKeysItem hotkey)
		{
			foreach (HotKeyActionItem hotKeyActionItem in hotkey.HotKeyActionList)
			{
				if (sender != null)
				{
					(sender as FlashPlayer).Focus();
				}
				switch (hotKeyActionItem.Action)
				{
					case HotKeyAction.ShowOptions:
						this.ShowOptions();
						break;
					case HotKeyAction.ToggleFullscreen:
						this.ToggleFullScreen();
						break;
					case HotKeyAction.GoFullScreen:
						this.GoFullScreen();
						break;
					case HotKeyAction.ExitFullScreen:
						this.ExitFullScreen();
						break;
					case HotKeyAction.SetSizeAbsolute:
						if (hotKeyActionItem.Params.Count > 1)
						{
							base.Width = int.Parse(hotKeyActionItem.Params[0]);
							base.Height = int.Parse(hotKeyActionItem.Params[1]);
						}
						break;
					case HotKeyAction.SetSizeRelative:
						if (hotKeyActionItem.Params.Count > 1)
						{
							double num = double.Parse(hotKeyActionItem.Params[0]);
							double num2 = double.Parse(hotKeyActionItem.Params[1]);
							if (num > 0.0 && num < 101.0)
							{
								num *= 0.01;
								base.Width = (int)((double)Screen.PrimaryScreen.Bounds.Width * num);
							}
							if (num2 > 0.0 && num2 < 101.0)
							{
								num2 *= 0.01;
								base.Height = (int)((double)Screen.PrimaryScreen.Bounds.Height * num2);
							}
						}
						break;
					case HotKeyAction.ShowQuickAlign:
						this.canProcessHotKeys = false;
						if (this.frmQuickAlign.ShowDialog() == DialogResult.OK && sender != null)
						{
							(sender as FlashPlayer).AlignMode = Database.Database.Instance.Alignment;
						}
						base.Focus();
						if (sender != null)
						{
							(sender as FlashPlayer).Focus();
						}
						this.canProcessHotKeys = true;
						break;
					case HotKeyAction.Screenshot:
						this.ScreenShot();
						break;
					case HotKeyAction.ChatCommand:
						if (hotKeyActionItem.Params.Count > 0)
						{
							SendKeys.Send("{ENTER}" + hotKeyActionItem.Params[0] + "{ENTER}");
						}
						break;
					case HotKeyAction.GoToNextTab:
						if (this.tabList.Count > 1)
						{
							int num3 = this.selectedTabIndex + 1;
							if (num3 >= this.tabList.Count)
							{
								num3 = 0;
							}
							this.SelectTab(num3);
						}
						break;
					case HotKeyAction.GoToPreviousTab:
						if (this.tabList.Count > 1)
						{
							int num4 = this.selectedTabIndex - 1;
							if (num4 < 0)
							{
								num4 = this.tabList.Count - 1;
							}
							this.SelectTab(num4);
						}
						break;
					case HotKeyAction.ChangeAlign:
						if (hotKeyActionItem.Params.Count > 0 && sender != null)
						{
							(sender as FlashPlayer).AlignMode = Utilities.AlignToInt(hotKeyActionItem.Params[0]);
						}
						break;
					case HotKeyAction.ChangeScale:
						if (hotKeyActionItem.Params.Count > 0)
						{
							if (hotKeyActionItem.Params[0] == "ShowAll")
							{
								if (sender != null)
								{
									(sender as FlashPlayer).ScaleMode = 0;
								}
							}
							else if (sender != null)
							{
								(sender as FlashPlayer).ScaleMode = 3;
							}
						}
						break;
				}
			}
			if (sender != null)
			{
				(sender as FlashPlayer).Focus();
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys KeyCode)
		{
			this.LastKeyCode = KeyCode;
			this.mRepeating = true;
			return base.ProcessCmdKey(ref msg, KeyCode);
		}

		public bool PreFilterMessage(ref Message m)
		{
			if ((m.Msg == 257 || m.Msg == 261) && this.canProcessHotKeys && this.mRepeating)
			{
				this.mRepeating = false;
				if (this.LastKeyCode == Keys.Escape)
				{
					this.menuStrip1.Visible = !this.menuStrip1.Visible;
					return true;
				}
				if (this.LastKeyCode == (Keys.LButton | Keys.Back | Keys.Control) && this.tabList.Count > 1)
				{
					int num = this.selectedTabIndex + 1;
					if (num >= this.tabList.Count)
					{
						num = 0;
					}
					this.SelectTab(num);
				}
				if (this.LastKeyCode == (Keys.LButton | Keys.Back | Keys.Shift | Keys.Control) && this.tabList.Count > 1)
				{
					int num2 = this.selectedTabIndex - 1;
					if (num2 < 0)
					{
						num2 = this.tabList.Count - 1;
					}
					this.SelectTab(num2);
				}
				if (this.LastKeyCode == (Keys)131157 && this.selectedTabIndex > -1)
				{
					Win32.SendMessage(this.tabList[this.selectedTabIndex].swf.Handle, 256, 17, 0);
					Win32.SendMessage(this.tabList[this.selectedTabIndex].swf.Handle, 256, 90, 0);
					Win32.SendMessage(this.tabList[this.selectedTabIndex].swf.Handle, 257, 90, 0);
					Win32.SendMessage(this.tabList[this.selectedTabIndex].swf.Handle, 257, 17, 0);
				}
				if (this.LastKeyCode == (Keys)131145 && this.selectedTabIndex > -1)
				{
					Win32.SendMessage(this.tabList[this.selectedTabIndex].swf.Handle, 256, 17, 0);
					Win32.SendMessage(this.tabList[this.selectedTabIndex].swf.Handle, 256, 89, 0);
					Win32.SendMessage(this.tabList[this.selectedTabIndex].swf.Handle, 257, 89, 0);
					Win32.SendMessage(this.tabList[this.selectedTabIndex].swf.Handle, 257, 17, 0);
				}
				foreach (HotKeysItem hotKeysItem in HotKeys.Instance.HotKeyList.Values)
				{
					if (this.LastKeyCode == (hotKeysItem.Modifier | hotKeysItem.HotKey))
					{
						if (this.selectedTabIndex >= 0)
						{
							this.ProcessHotKey(this.tabList[this.selectedTabIndex].swf, hotKeysItem);
						}
						else
						{
							this.ProcessHotKey(null, hotKeysItem);
						}
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private void Timer_ScreenshotLabel_Tick(object sender, EventArgs e)
		{
			this.timer_ScreenshotLabel.Enabled = false;
			this.lbScreenShot.Visible = false;
		}

		private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.ShowOptions();
		}

		private void FullscreenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.ToggleFullScreen();
		}

		private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void ToolStripMenuItem4_Click(object sender, EventArgs e)
		{
			this.tabList[this.selectedTabIndex].swf.ScaleMode = 3;
		}

		private void ToolStripMenuItem11_Click(object sender, EventArgs e)
		{
			this.tabList[this.selectedTabIndex].swf.ScaleMode = 4;
		}

		private void ToolStripMenuItem12_Click(object sender, EventArgs e)
		{
			this.tabList[this.selectedTabIndex].swf.ScaleMode = 1;
		}

		private void ShowAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.tabList[this.selectedTabIndex].swf.ScaleMode = 0;
		}

		private void LowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.tabList[this.selectedTabIndex].swf.Quality2 = "Low";
		}

		private void MediumToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.tabList[this.selectedTabIndex].swf.Quality2 = "Medium";
		}

		private void HighToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.tabList[this.selectedTabIndex].swf.Quality2 = "High";
		}

		private void English1ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.NewServerTab("Transformice", GAME_URL);
		}

		public bool WebsiteIsAvailable(string url)
		{
			WebRequest webRequest = WebRequest.Create(url);
			try
			{
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)webRequest.GetResponse())
				{
					if (httpWebResponse != null || httpWebResponse.StatusCode == HttpStatusCode.OK)
					{
						httpWebResponse.Close();
						return true;
					}
				}
			}
			catch
			{
				return false;
			}
			return false;
		}

		private void Button3_Click(object sender, EventArgs e) { }

		private void CheckAllComplete() { }

		private void Button2_Click(object sender, EventArgs e)
		{
			ServerList frmServerList = new ServerList();
			frmServerList.LoadList();
			if (frmServerList.ShowDialog() == DialogResult.OK)
			{
				if (DbServers.Instance.currentServerLabel != string.Empty && DbServers.Instance.currentServerURL != string.Empty)
				{
					if (Database.Database.Instance.AutoJoinRoom != this.tbAutoJoin.Text)
					{
						Database.Database.Instance.AutoJoinRoom = this.tbAutoJoin.Text;
					}
					this.NewServerTab(DbServers.Instance.currentServerLabel, DbServers.Instance.currentServerURL);
					return;
				}
				MessageBox.Show("Couldn't load that server!", "Error Loading Server", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		private void OtherToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ServerList frmServerList = new ServerList();
			frmServerList.LoadList();
			if (frmServerList.ShowDialog() == DialogResult.OK)
			{
				if (DbServers.Instance.currentServerLabel != string.Empty && DbServers.Instance.currentServerURL != string.Empty)
				{
					this.NewServerTab(DbServers.Instance.currentServerLabel, DbServers.Instance.currentServerURL);
					return;
				}
				MessageBox.Show("Couldn't load that server!", "Error Loading Server", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		private void RefreshToolStripMenuItem2_Click(object sender, EventArgs e)
		{
			if (this.selectedTabIndex < 0)
			{
				return;
			}
			string msg = "Are you sure you want to refresh the connection?";
			string title = "Refresh";
			this.canProcessHotKeys = false;
			bool flag = false;
			if (MessageBox.Show(msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				flag = true;
			}
			if (flag)
			{
				this.panel1.Hide();
				foreach (TabInfo tabInfo in this.tabList)
				{
					tabInfo.swf.Hide();
				}
				string swfUrl = this.tabList[this.selectedTabIndex].swfUrl;
				base.Controls.Remove(this.tabList[this.selectedTabIndex].swf);
				this.tabList[this.selectedTabIndex].swf.Dispose();
				this.tabList[this.selectedTabIndex].swf = null;
				FlashPlayer flashPlayer = new FlashPlayer();
				flashPlayer.BeginInit();
				flashPlayer.FlashCall += this.TmpSWF_FlashCall;
				flashPlayer.OnReadyStateChange += this.TmpSWF_OnReadyStateChange;
				flashPlayer.Location = new Point(0, 0);
				flashPlayer.Margin = new Padding(0);
				flashPlayer.Name = "Flash";
				flashPlayer.Size = base.ClientSize;
				flashPlayer.EndInit();
				base.Controls.Add(flashPlayer);
				this.tabList[this.selectedTabIndex].swf = flashPlayer;
				flashPlayer.ScaleMode = 3;
				flashPlayer.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
				flashPlayer.BackgroundColor = 6911633;
				flashPlayer.AlignMode = Database.Database.Instance.Alignment;
				this.tabList[this.selectedTabIndex].LoadMovie(swfUrl);
				flashPlayer.Focus();
			}
			this.canProcessHotKeys = true;
		}

		private void RenameToolStripMenuItem2_Click(object sender, EventArgs e)
		{
			string text = this.tabList[this.selectedTabIndex].button.Text;
			this.canProcessHotKeys = false;
			if (InputBox("Rename Tab", "Tab Name:", ref text) == DialogResult.OK)
			{
				this.tabList[this.selectedTabIndex].button.Text = text;
				this.windowTitle = text + " - Baffler's Standalone";
				this.Text = this.windowTitle;
			}
			this.canProcessHotKeys = true;
		}

		private void CloseTabToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string msg = "Are you sure you want to close this tab?";
			string title = "Close Tab";
			this.canProcessHotKeys = false;
			bool flag = false;
			if (MessageBox.Show(msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
			{
				flag = true;
			}
			if (flag)
			{
				FlashPlayer swf = this.tabList[this.selectedTabIndex].swf;
				TabInfo tabInfo = this.tabList[this.selectedTabIndex];
				this.toolStrip1.Items.Remove(tabInfo.button);
				swf.Dispose();
				tabInfo.button.Dispose();
				this.tabList.RemoveAt(this.selectedTabIndex);
				this.selectedTabIndex = -1;
				if (this.tabList.Count > 0)
				{
					this.SelectTab(0);
				}
				else
				{
					this.closeTabToolStripMenuItem.Visible = false;
					this.viewToolStripMenuItem.Enabled = false;
					this.refreshToolStripMenuItem2.Enabled = false;
					this.renameToolStripMenuItem2.Enabled = false;

					this.panel1.Show();
				}
				if (this.tabList.Count < 2)
				{
					this.toolStrip1.Visible = false;
				}
			}
			this.canProcessHotKeys = true;
		}

		private void Button4_Click_1(object sender, EventArgs e)
		{
			this.NewServerTab("Transformice", GAME_URL);
		}

		private void HideMenuToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.menuStrip1.Visible = false;
		}

		private void AlignmentToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FlashPlayer flashPlayer = null;
			if (this.selectedTabIndex > -1 && this.tabList[this.selectedTabIndex] != null)
			{
				flashPlayer = this.tabList[this.selectedTabIndex].swf;
			}
			if (flashPlayer == null)
			{
				return;
			}
			if (this.frmQuickAlign.ShowDialog() == DialogResult.OK)
			{
				flashPlayer.AlignMode = Database.Database.Instance.Alignment;
			}
			base.Focus();
			flashPlayer.Focus();
		}

		private void MainForm_Shown(object sender, EventArgs e) {}

		private void LoadRSS_Elapsed(object sender, ElapsedEventArgs e)
		{
			(sender as System.Timers.Timer).Stop();
			BackgroundWorker backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += this.RssBG_DoWork;
			backgroundWorker.RunWorkerCompleted += this.RssBG_RunWorkerCompleted;
			backgroundWorker.RunWorkerAsync();
		}

		private void RssBG_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (this.webBrowser1.InvokeRequired)
			{
				this.webBrowser1.Invoke(new MethodInvoker(delegate ()
				{
					this.webBrowser1.Update();
				}));
				return;
			}
			this.webBrowser1.Update();
		}

		private void RssBG_DoWork(object sender, DoWorkEventArgs e)
		{
			bool flag = this.RefreshRSS();
			string text = "<html>\n<body style=\"background-color: #F0F0F0\">\n";
			text += "<div style=\"border: 1px solid black; background-color: #FFFFFF; padding-left: 40px; padding-right: 40px; margin: 40px;\">\n";
			text += "<h1 style=\"border-bottom: 1px solid gray\">Transformice News</h1>";
			if (flag)
			{
				for (int i = 0; i < this.rsslist.Count; i++)
				{
					if(i >= 10) break;
					string text2 = text;
					text = string.Concat(new string[]
					{
						text2,
						"<a href=\"",
						this.rsslist[i].Link,
						"\"><b>",
						this.rsslist[i].Title,
						"</b></a> (",
						this.rsslist[i].Description,
						""
					});
					string text3 = text;
					string[] array = new string[6];
					array[0] = text3;
					array[1] = "<small>";
					string[] array2 = array;
					int num = 2;
					DateTime date = this.rsslist[i].Date;
					array2[num] = date.ToLongDateString();
					array[3] = " ";
					string[] array3 = array;
					int num2 = 4;
					DateTime date2 = this.rsslist[i].Date;
					array3[num2] = date2.ToLongTimeString();
					array[5] = "</small><br />\n";
					text = string.Concat(array);
					text += "<br />";
				}
			}
			else
			{
				text += "There was an error retrieving the news feed!";
			}
			text += "</div>\n</body>\n</html>";
			this.webBrowser1.DocumentText = text;
		}

		private void SendSecurityProtocol_Click(object sender, EventArgs e)
		{
			string web = this.tabList[this.selectedTabIndex].swfUrl;
			Uri uri = new Uri(web);

			string content = "<cross-domain-policy><allow-access-from domain=\"*\" to-ports=\"*\"/></cross-domain-policy>\x00";
            try
            {
				IPAddress[] addresses = Dns.GetHostAddresses(uri.Host);
				IPAddress targetAddress = addresses[0];

				using (TcpClient client = new TcpClient())
				{
					client.Connect(targetAddress, 80);

					using (NetworkStream stream = client.GetStream())
					{
						byte[] dataBytes = Encoding.UTF8.GetBytes(content);
						stream.Write(dataBytes, 0, dataBytes.Length);
						MessageBox.Show(this, "Done", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
				}
            }
			catch (Exception ex)
            {
				MessageBox.Show(this, ex.Message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
	}
}