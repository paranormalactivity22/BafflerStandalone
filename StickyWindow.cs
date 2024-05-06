using System;
using System.Collections;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
namespace BafflerStandalone
{
	public class StickyWindow : NativeWindow
	{
		private delegate bool ProcessMessage(ref Message m);
		private static ArrayList GlobalStickyWindows = new ArrayList();
		private StickyWindow.ProcessMessage MessageProcessor;
		private StickyWindow.ProcessMessage DefaultMessageProcessor;
		private StickyWindow.ProcessMessage MoveMessageProcessor;
		private StickyWindow.ProcessMessage ResizeMessageProcessor;
		private bool movingForm;
		private Point formOffsetPoint;
		private Point offsetPoint;
		private bool resizingForm;
		private StickyWindow.ResizeDir resizeDirection;
		private Rectangle formOffsetRect;
		private Point mousePoint;
		private Form originalForm;
		private Rectangle formRect;
		private Rectangle formOriginalRect;
		private static int stickGap = 20;
		private bool stickOnResize;
		private bool stickOnMove;
		private bool stickToScreen;
		private bool stickToOther;
		private enum ResizeDir
		{
			Top = 2,
			Bottom = 4,
			Left = 8,
			Right = 16
		}
		
		public int StickGap { get => StickyWindow.stickGap; set => StickyWindow.stickGap = value; }
		public bool StickOnResize { get => this.stickOnResize; set => this.stickOnResize = value; }
		public bool StickOnMove { get => this.stickOnMove; set => this.stickOnMove = value; }
		public bool StickToScreen { get => this.stickToScreen; set => this.stickToScreen = value; }
		public bool StickToOther { get => this.stickToOther; set => this.stickToOther = value; }


		public static void RegisterExternalReferenceForm(Form frmExternal)
		{
			StickyWindow.GlobalStickyWindows.Add(frmExternal);
		}

		public static void UnregisterExternalReferenceForm(Form frmExternal)
		{
			StickyWindow.GlobalStickyWindows.Remove(frmExternal);
		}

		public StickyWindow(Form form)
		{
			this.resizingForm = false;
			this.movingForm = false;
			this.originalForm = form;
			this.formRect = Rectangle.Empty;
			this.formOffsetRect = Rectangle.Empty;
			this.formOffsetPoint = Point.Empty;
			this.offsetPoint = Point.Empty;
			this.mousePoint = Point.Empty;
			this.stickOnMove = true;
			this.stickOnResize = true;
			this.stickToScreen = true;
			this.stickToOther = true;
			this.DefaultMessageProcessor = new StickyWindow.ProcessMessage(this.DefaultMsgProcessor);
			this.MoveMessageProcessor = new StickyWindow.ProcessMessage(this.MoveMsgProcessor);
			this.ResizeMessageProcessor = new StickyWindow.ProcessMessage(this.ResizeMsgProcessor);
			this.MessageProcessor = this.DefaultMessageProcessor;
			base.AssignHandle(this.originalForm.Handle);
		}

		private bool DefaultMsgProcessor(ref Message m)
		{
			int msg = m.Msg;
			if (msg == 161)
			{
				this.originalForm.Activate();
				this.mousePoint.X = (int)((short)Win32.Bit.LoWord((int)m.LParam));
				this.mousePoint.Y = (int)((short)Win32.Bit.HiWord((int)m.LParam));
				if (this.OnNCLButtonDown((int)m.WParam, this.mousePoint))
				{
					m.Result = (IntPtr)((this.resizingForm || this.movingForm) ? 1 : 0);
					return true;
				}
			}
			return false;
		}

		private bool OnNCLButtonDown(int iHitTest, Point point)
		{
			Rectangle bounds = this.originalForm.Bounds;
			this.offsetPoint = point;
			switch (iHitTest)
			{
			case 2:
				if (this.stickOnMove)
				{
					this.offsetPoint.Offset(-bounds.Left, -bounds.Top);
					this.StartMove();
					return true;
				}
				return false;
			case 10:
			
				return this.StartResize(StickyWindow.ResizeDir.Left);
			case 11:
			
				return this.StartResize(StickyWindow.ResizeDir.Right);
			case 12:
			
				return this.StartResize(StickyWindow.ResizeDir.Top);
			case 13:
			
				return this.StartResize((StickyWindow.ResizeDir)10);
			case 14:
			
				return this.StartResize((StickyWindow.ResizeDir)18);
			case 15:
			
				return this.StartResize(StickyWindow.ResizeDir.Bottom);
			case 16:
			
				return this.StartResize((StickyWindow.ResizeDir)12);
			case 17:
			
				return this.StartResize((StickyWindow.ResizeDir)20);
			}
			return false;
		}

		private bool StartResize(StickyWindow.ResizeDir resDir)
		{
			if (this.stickOnResize)
			{
				this.resizeDirection = resDir;
				this.formRect = this.originalForm.Bounds;
				this.formOriginalRect = this.originalForm.Bounds;
				if (!this.originalForm.Capture)
				{
					this.originalForm.Capture = true;
				}
				this.MessageProcessor = this.ResizeMessageProcessor;
				return true;
			}
			return false;
		}

		private bool ResizeMsgProcessor(ref Message m)
		{
			if (!this.originalForm.Capture)
			{
				this.Cancel();
				return false;
			}
			int msg = m.Msg;
			if (msg != 256)
			{
				switch (msg)
				{
					case 512:
						this.mousePoint.X = (int)((short)Win32.Bit.LoWord((int)m.LParam));
						this.mousePoint.Y = (int)((short)Win32.Bit.HiWord((int)m.LParam));
						this.Resize(this.mousePoint);
						break;
					case 514:
						this.EndResize();
						break;
				}
			}
			else if ((int)m.WParam == 27)
			{
				this.originalForm.Bounds = this.formOriginalRect;
				this.Cancel();
			}
			return false;
		}

		private void EndResize()
		{
			this.Cancel();
		}

		private void Resize(Point p)
		{
			p = this.originalForm.PointToScreen(p);
			Screen screen = Screen.FromPoint(p);
			this.formRect = this.originalForm.Bounds;
			int right = this.formRect.Right;
			int bottom = this.formRect.Bottom;
			if ((this.resizeDirection & StickyWindow.ResizeDir.Left) == StickyWindow.ResizeDir.Left)
			{
				this.formRect.Width = this.formRect.X - p.X + this.formRect.Width;
				this.formRect.X = right - this.formRect.Width;
			}
			if ((this.resizeDirection & StickyWindow.ResizeDir.Right) == StickyWindow.ResizeDir.Right)
			{
				this.formRect.Width = p.X - this.formRect.Left;
			}
			if ((this.resizeDirection & StickyWindow.ResizeDir.Top) == StickyWindow.ResizeDir.Top)
			{
				this.formRect.Height = this.formRect.Height - p.Y + this.formRect.Top;
				this.formRect.Y = bottom - this.formRect.Height;
			}
			if ((this.resizeDirection & StickyWindow.ResizeDir.Bottom) == StickyWindow.ResizeDir.Bottom)
			{
				this.formRect.Height = p.Y - this.formRect.Top;
			}
			this.formOffsetRect.X = StickyWindow.stickGap + 1;
			this.formOffsetRect.Y = StickyWindow.stickGap + 1;
			this.formOffsetRect.Height = 0;
			this.formOffsetRect.Width = 0;
			if (this.stickToScreen)
			{
				this.Resize_Stick(screen.WorkingArea, false);
			}
			if (this.stickToOther)
			{
				foreach (object obj in StickyWindow.GlobalStickyWindows)
				{
					Form form = (Form)obj;
					if (form != this.originalForm)
					{
						this.Resize_Stick(form.Bounds, true);
					}
				}
			}
			if (this.formOffsetRect.X == StickyWindow.stickGap + 1)
			{
				this.formOffsetRect.X = 0;
			}
			if (this.formOffsetRect.Width == StickyWindow.stickGap + 1)
			{
				this.formOffsetRect.Width = 0;
			}
			if (this.formOffsetRect.Y == StickyWindow.stickGap + 1)
			{
				this.formOffsetRect.Y = 0;
			}
			if (this.formOffsetRect.Height == StickyWindow.stickGap + 1)
			{
				this.formOffsetRect.Height = 0;
			}
			if ((this.resizeDirection & StickyWindow.ResizeDir.Left) == StickyWindow.ResizeDir.Left)
			{
				int num = this.formRect.Width + this.formOffsetRect.Width + this.formOffsetRect.X;
				if (this.originalForm.MaximumSize.Width != 0)
				{
					num = Math.Min(num, this.originalForm.MaximumSize.Width);
				}
				num = Math.Min(num, SystemInformation.MaxWindowTrackSize.Width);
				num = Math.Max(num, this.originalForm.MinimumSize.Width);
				num = Math.Max(num, SystemInformation.MinWindowTrackSize.Width);
				this.formRect.X = right - num;
				this.formRect.Width = num;
			}
			else
			{
				this.formRect.Width = this.formRect.Width + (this.formOffsetRect.Width + this.formOffsetRect.X);
			}
			if ((this.resizeDirection & StickyWindow.ResizeDir.Top) == StickyWindow.ResizeDir.Top)
			{
				int num2 = this.formRect.Height + this.formOffsetRect.Height + this.formOffsetRect.Y;
				if (this.originalForm.MaximumSize.Height != 0)
				{
					num2 = Math.Min(num2, this.originalForm.MaximumSize.Height);
				}
				num2 = Math.Min(num2, SystemInformation.MaxWindowTrackSize.Height);
				num2 = Math.Max(num2, this.originalForm.MinimumSize.Height);
				num2 = Math.Max(num2, SystemInformation.MinWindowTrackSize.Height);
				this.formRect.Y = bottom - num2;
				this.formRect.Height = num2;
			}
			else
			{
				this.formRect.Height = this.formRect.Height + (this.formOffsetRect.Height + this.formOffsetRect.Y);
			}
			this.originalForm.Bounds = this.formRect;
		}

		private void Resize_Stick(Rectangle toRect, bool bInsideStick)
		{
			if (this.formRect.Right >= toRect.Left - StickyWindow.stickGap && this.formRect.Left <= toRect.Right + StickyWindow.stickGap)
			{
				if ((this.resizeDirection & StickyWindow.ResizeDir.Top) == StickyWindow.ResizeDir.Top)
				{
					if (Math.Abs(this.formRect.Top - toRect.Bottom) <= Math.Abs(this.formOffsetRect.Top) && bInsideStick)
					{
						this.formOffsetRect.Y = this.formRect.Top - toRect.Bottom;
					}
					else if (Math.Abs(this.formRect.Top - toRect.Top) <= Math.Abs(this.formOffsetRect.Top))
					{
						this.formOffsetRect.Y = this.formRect.Top - toRect.Top;
					}
				}
				if ((this.resizeDirection & StickyWindow.ResizeDir.Bottom) == StickyWindow.ResizeDir.Bottom)
				{
					if (Math.Abs(this.formRect.Bottom - toRect.Top) <= Math.Abs(this.formOffsetRect.Bottom) && bInsideStick)
					{
						this.formOffsetRect.Height = toRect.Top - this.formRect.Bottom;
					}
					else if (Math.Abs(this.formRect.Bottom - toRect.Bottom) <= Math.Abs(this.formOffsetRect.Bottom))
					{
						this.formOffsetRect.Height = toRect.Bottom - this.formRect.Bottom;
					}
				}
			}
			if (this.formRect.Bottom >= toRect.Top - StickyWindow.stickGap && this.formRect.Top <= toRect.Bottom + StickyWindow.stickGap)
			{
				if ((this.resizeDirection & StickyWindow.ResizeDir.Right) == StickyWindow.ResizeDir.Right)
				{
					if (Math.Abs(this.formRect.Right - toRect.Left) <= Math.Abs(this.formOffsetRect.Right) && bInsideStick)
					{
						this.formOffsetRect.Width = toRect.Left - this.formRect.Right;
					}
					else if (Math.Abs(this.formRect.Right - toRect.Right) <= Math.Abs(this.formOffsetRect.Right))
					{
						this.formOffsetRect.Width = toRect.Right - this.formRect.Right;
					}
				}
				if ((this.resizeDirection & StickyWindow.ResizeDir.Left) == StickyWindow.ResizeDir.Left)
				{
					if (Math.Abs(this.formRect.Left - toRect.Right) <= Math.Abs(this.formOffsetRect.Left) && bInsideStick)
					{
						this.formOffsetRect.X = this.formRect.Left - toRect.Right;
						return;
					}
					if (Math.Abs(this.formRect.Left - toRect.Left) <= Math.Abs(this.formOffsetRect.Left))
					{
						this.formOffsetRect.X = this.formRect.Left - toRect.Left;
					}
				}
			}
		}

		private void StartMove()
		{
			this.formRect = this.originalForm.Bounds;
			this.formOriginalRect = this.originalForm.Bounds;
			if (!this.originalForm.Capture)
			{
				this.originalForm.Capture = true;
			}
			this.MessageProcessor = this.MoveMessageProcessor;
		}

		private bool MoveMsgProcessor(ref Message m)
		{
			if (!this.originalForm.Capture)
			{
				this.Cancel();
				return false;
			}
			int msg = m.Msg;
			if (msg != 256)
			{
				switch (msg)
				{
				case 512:
					this.mousePoint.X = (int)((short)Win32.Bit.LoWord((int)m.LParam));
					this.mousePoint.Y = (int)((short)Win32.Bit.HiWord((int)m.LParam));
					this.Move(this.mousePoint);
					break;
				case 514:
					this.EndMove();
					break;
				}
			}
			else if ((int)m.WParam == 27)
			{
				this.originalForm.Bounds = this.formOriginalRect;
				this.Cancel();
			}
			return false;
		}

		private void EndMove()
		{
			this.Cancel();
		}

		private void Move(Point p)
		{
			p = this.originalForm.PointToScreen(p);
			Screen screen = Screen.FromPoint(p);
			if (!screen.WorkingArea.Contains(p))
			{
				p.X = this.NormalizeInside(p.X, screen.WorkingArea.Left, screen.WorkingArea.Right);
				p.Y = this.NormalizeInside(p.Y, screen.WorkingArea.Top, screen.WorkingArea.Bottom);
			}
			p.Offset(-this.offsetPoint.X, -this.offsetPoint.Y);
			this.formRect.Location = p;
			this.formOffsetPoint.X = StickyWindow.stickGap + 1;
			this.formOffsetPoint.Y = StickyWindow.stickGap + 1;
			if (this.stickToScreen)
			{
				this.Move_Stick(screen.WorkingArea, false);
			}
			if (this.stickToOther)
			{
				foreach (object obj in StickyWindow.GlobalStickyWindows)
				{
					Form form = (Form)obj;
					if (form != this.originalForm)
					{
						this.Move_Stick(form.Bounds, true);
					}
				}
			}
			if (this.formOffsetPoint.X == StickyWindow.stickGap + 1)
			{
				this.formOffsetPoint.X = 0;
			}
			if (this.formOffsetPoint.Y == StickyWindow.stickGap + 1)
			{
				this.formOffsetPoint.Y = 0;
			}
			this.formRect.Offset(this.formOffsetPoint);
			this.originalForm.Bounds = this.formRect;
		}

		private void Move_Stick(Rectangle toRect, bool bInsideStick)
		{
			if (this.formRect.Bottom >= toRect.Top - StickyWindow.stickGap && this.formRect.Top <= toRect.Bottom + StickyWindow.stickGap)
			{
				if (bInsideStick)
				{
					if (Math.Abs(this.formRect.Left - toRect.Right) <= Math.Abs(this.formOffsetPoint.X))
					{
						this.formOffsetPoint.X = toRect.Right - this.formRect.Left;
					}
					if (Math.Abs(this.formRect.Left + this.formRect.Width - toRect.Left) <= Math.Abs(this.formOffsetPoint.X))
					{
						this.formOffsetPoint.X = toRect.Left - this.formRect.Width - this.formRect.Left;
					}
				}
				if (Math.Abs(this.formRect.Left - toRect.Left) <= Math.Abs(this.formOffsetPoint.X))
				{
					this.formOffsetPoint.X = toRect.Left - this.formRect.Left;
				}
				if (Math.Abs(this.formRect.Left + this.formRect.Width - toRect.Left - toRect.Width) <= Math.Abs(this.formOffsetPoint.X))
				{
					this.formOffsetPoint.X = toRect.Left + toRect.Width - this.formRect.Width - this.formRect.Left;
				}
			}
			if (this.formRect.Right >= toRect.Left - StickyWindow.stickGap && this.formRect.Left <= toRect.Right + StickyWindow.stickGap)
			{
				if (bInsideStick)
				{
					if (Math.Abs(this.formRect.Top - toRect.Bottom) <= Math.Abs(this.formOffsetPoint.Y) && bInsideStick)
					{
						this.formOffsetPoint.Y = toRect.Bottom - this.formRect.Top;
					}
					if (Math.Abs(this.formRect.Top + this.formRect.Height - toRect.Top) <= Math.Abs(this.formOffsetPoint.Y) && bInsideStick)
					{
						this.formOffsetPoint.Y = toRect.Top - this.formRect.Height - this.formRect.Top;
					}
				}
				if (Math.Abs(this.formRect.Top - toRect.Top) <= Math.Abs(this.formOffsetPoint.Y))
				{
					this.formOffsetPoint.Y = toRect.Top - this.formRect.Top;
				}
				if (Math.Abs(this.formRect.Top + this.formRect.Height - toRect.Top - toRect.Height) <= Math.Abs(this.formOffsetPoint.Y))
				{
					this.formOffsetPoint.Y = toRect.Top + toRect.Height - this.formRect.Height - this.formRect.Top;
				}
			}
		}

		private int NormalizeInside(int iP1, int iM1, int iM2)
		{
			if (iP1 <= iM1)
			{
				return iM1;
			}
			if (iP1 >= iM2)
			{
				return iM2;
			}
			return iP1;
		}

		private void Cancel()
		{
			this.originalForm.Capture = false;
			this.movingForm = false;
			this.resizingForm = false;
			this.MessageProcessor = this.DefaultMessageProcessor;
		}
	}
}
